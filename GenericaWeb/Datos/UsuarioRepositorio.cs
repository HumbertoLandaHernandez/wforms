using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using GenericaWeb.Modelos;
using GenericaWeb.Seguridad;

namespace GenericaWeb.Datos
{
    public static class UsuarioRepositorio
    {
        private const string SelectSesion = @"
SELECT u.UsuarioId, u.UsuarioDesc, u.[Contraseña] AS Hash, u.UsuarioRolId, r.RolDesc,
       n.NombreListaDesc, u.UsuarioPermisosId, p.PaginaDesc AS PaginaHome
FROM Sistema.Usuario u
JOIN Sistema.Rol r         ON r.RolId = u.UsuarioRolId
JOIN Sistema.NombreLista n ON n.NombreListaId = u.UsuarioNombreId
JOIN Sistema.Pagina p      ON p.PaginaId = u.UsuarioPaginaHomeId";

        /// <summary>Ids de todos los roles del usuario: principal + Sistema.UsuarioRol.</summary>
        private const string SubconsultaRoles = @"
SELECT ur.RolId FROM Sistema.UsuarioRol ur WHERE ur.UsuarioId = @Id
UNION
SELECT u.UsuarioRolId FROM Sistema.Usuario u WHERE u.UsuarioId = @Id";

        public static UsuarioSesion ObtenerParaLogin(string usuarioDesc, out string hash)
        {
            var fila = Db.ConsultarFila(SelectSesion + " WHERE u.UsuarioDesc = @Desc", Db.Texto("@Desc", usuarioDesc, 30));
            hash = fila == null ? null : (string)fila["Hash"];
            return fila == null ? null : ASesion(fila);
        }

        public static UsuarioSesion ObtenerSesion(short usuarioId)
        {
            var fila = Db.ConsultarFila(SelectSesion + " WHERE u.UsuarioId = @Id", Db.P("@Id", usuarioId));
            return fila == null ? null : ASesion(fila);
        }

        public static bool EsAdministrador(short usuarioId)
        {
            return Convert.ToInt32(Db.Escalar(
                "SELECT CASE WHEN @RolAdmin IN (" + SubconsultaRoles + ") THEN 1 ELSE 0 END",
                Db.P("@Id", usuarioId), Db.P("@RolAdmin", Configuracion.RolAdministradorId))) == 1;
        }

        /// <summary>Permisos de las listas (Rol.RolPermisosId) de todos sus roles + Usuario.UsuarioPermisosId.</summary>
        public static HashSet<short> PermisosEfectivos(short usuarioId)
        {
            var tabla = Db.Consultar(@"
SELECT ple.PermisoId
FROM Sistema.Rol r
JOIN Sistema.PermisoListaElemento ple ON ple.PermisoListaId = r.RolPermisosId
WHERE r.RolId IN (" + SubconsultaRoles + @")
UNION
SELECT u.UsuarioPermisosId FROM Sistema.Usuario u WHERE u.UsuarioId = @Id",
                Db.P("@Id", usuarioId));
            return new HashSet<short>(tabla.Rows.Cast<DataRow>().Select(f => (short)f[0]));
        }

        public static List<string> DescripcionRoles(short usuarioId)
        {
            var tabla = Db.Consultar("SELECT RolDesc FROM Sistema.Rol WHERE RolId IN (" + SubconsultaRoles + ") ORDER BY RolDesc",
                Db.P("@Id", usuarioId));
            return tabla.Rows.Cast<DataRow>().Select(f => (string)f[0]).ToList();
        }

        /// <summary>Roles registrados en Sistema.UsuarioRol (para edición).</summary>
        public static HashSet<byte> RolesAsignados(short usuarioId)
        {
            var tabla = Db.Consultar("SELECT RolId FROM Sistema.UsuarioRol WHERE UsuarioId = @Id", Db.P("@Id", usuarioId));
            return new HashSet<byte>(tabla.Rows.Cast<DataRow>().Select(f => (byte)f[0]));
        }

        public static DataTable Listar()
        {
            return Db.Consultar(@"
SELECT u.UsuarioId, u.UsuarioDesc, n.NombreListaDesc, r.RolDesc,
       STUFF((SELECT ', ' + r2.RolDesc
              FROM Sistema.UsuarioRol ur JOIN Sistema.Rol r2 ON r2.RolId = ur.RolId
              WHERE ur.UsuarioId = u.UsuarioId AND ur.RolId <> u.UsuarioRolId
              ORDER BY r2.RolDesc
              FOR XML PATH(''), TYPE).value('.', 'varchar(max)'), 1, 2, '') AS RolesAdicionales,
       pe.PermisoDesc, pl.PaginaListaDesc, p.Descripcion AS PaginaHome, u.UsuarioUsys, u.UsuarioDT
FROM Sistema.Usuario u
JOIN Sistema.Rol r              ON r.RolId = u.UsuarioRolId
JOIN Sistema.NombreLista n      ON n.NombreListaId = u.UsuarioNombreId
JOIN Sistema.Permiso pe         ON pe.PermisoId = u.UsuarioPermisosId
JOIN Sistema.Pagina p           ON p.PaginaId = u.UsuarioPaginaHomeId
LEFT JOIN Sistema.PaginaLista pl ON pl.PaginaListaId = u.UsuarioPermisosPaginasId
ORDER BY u.UsuarioDesc");
        }

        public static DataRow Obtener(short usuarioId)
        {
            return Db.ConsultarFila(@"
SELECT UsuarioId, UsuarioDesc, UsuarioRolId, UsuarioPermisosId, UsuarioNombreId,
       UsuarioPermisosPaginasId, UsuarioPaginaHomeId
FROM Sistema.Usuario WHERE UsuarioId = @Id", Db.P("@Id", usuarioId));
        }

        public static void Insertar(UsuarioDatos d, string hash, string usys)
        {
            if (hash == null)
                throw new ArgumentNullException(nameof(hash));

            Db.Transaccion((c, t) =>
            {
                d.UsuarioId = Convert.ToInt16(Db.Escalar(c, t, @"
INSERT INTO Sistema.Usuario (UsuarioUsys, UsuarioDT, UsuarioDesc, [Contraseña], UsuarioRolId, UsuarioPermisosId,
                             UsuarioNombreId, UsuarioPermisosPaginasId, UsuarioPaginaHomeId)
VALUES (@Usys, SYSDATETIME(), @Desc, @Hash, @RolId, @PermisosId, @NombreId, @PermisosPaginasId, @PaginaHomeId);
SELECT CAST(SCOPE_IDENTITY() AS int);",
                    Parametros(d, hash, usys)));
                GuardarRoles(c, t, d, usys);
            });
        }

        /// <summary>Actualiza el usuario y sus roles; si hash es null la contraseña no cambia.</summary>
        public static void Actualizar(UsuarioDatos d, string hash, string usys)
        {
            Db.Transaccion((c, t) =>
            {
                Db.Ejecutar(c, t, @"
UPDATE Sistema.Usuario SET
    UsuarioUsys = @Usys, UsuarioDT = SYSDATETIME(), UsuarioDesc = @Desc,
    [Contraseña] = COALESCE(@Hash, [Contraseña]),
    UsuarioRolId = @RolId, UsuarioPermisosId = @PermisosId, UsuarioNombreId = @NombreId,
    UsuarioPermisosPaginasId = @PermisosPaginasId, UsuarioPaginaHomeId = @PaginaHomeId
WHERE UsuarioId = @Id",
                    Parametros(d, hash, usys));
                GuardarRoles(c, t, d, usys);
            });
        }

        public static void Eliminar(short usuarioId)
        {
            Db.Transaccion((c, t) =>
            {
                Db.Ejecutar(c, t, "DELETE FROM Sistema.UsuarioRol WHERE UsuarioId = @Id", Db.P("@Id", usuarioId));
                Db.Ejecutar(c, t, "DELETE FROM Sistema.Usuario WHERE UsuarioId = @Id", Db.P("@Id", usuarioId));
            });
        }

        /// <summary>Reemplaza Sistema.UsuarioRol con los roles adicionales + el rol principal.</summary>
        private static void GuardarRoles(SqlConnection c, SqlTransaction t, UsuarioDatos d, string usys)
        {
            Db.Ejecutar(c, t, "DELETE FROM Sistema.UsuarioRol WHERE UsuarioId = @Id", Db.P("@Id", d.UsuarioId));
            foreach (var rolId in d.Roles.Concat(new[] { d.RolId }).Distinct())
            {
                Db.Ejecutar(c, t, @"INSERT INTO Sistema.UsuarioRol (UsuarioRolUsys, UsuarioRolDT, UsuarioId, RolId)
                                    VALUES (@Usys, SYSDATETIME(), @Id, @RolId)",
                    Db.Texto("@Usys", usys, 30), Db.P("@Id", d.UsuarioId), Db.P("@RolId", rolId));
            }
        }

        private static SqlParameter[] Parametros(UsuarioDatos d, string hash, string usys)
        {
            return new[]
            {
                Db.P("@Id", d.UsuarioId),
                Db.Texto("@Usys", usys, 50),
                Db.Texto("@Desc", d.UsuarioDesc, 30),
                Db.Texto("@Hash", hash, 100),
                Db.P("@RolId", d.RolId),
                Db.P("@PermisosId", d.PermisosId),
                Db.P("@NombreId", d.NombreId),
                Db.P("@PermisosPaginasId", d.PermisosPaginasId),
                Db.P("@PaginaHomeId", d.PaginaHomeId)
            };
        }

        private static UsuarioSesion ASesion(DataRow f)
        {
            var id = (short)f["UsuarioId"];
            return new UsuarioSesion
            {
                UsuarioId = id,
                UsuarioDesc = (string)f["UsuarioDesc"],
                RolId = (byte)f["UsuarioRolId"],
                RolDesc = (string)f["RolDesc"],
                Roles = DescripcionRoles(id),
                NombreCompleto = (string)f["NombreListaDesc"],
                PermisoId = (short)f["UsuarioPermisosId"],
                Permisos = PermisosEfectivos(id).OrderBy(p => p).ToList(),
                PaginaHome = (string)f["PaginaHome"]
            };
        }
    }
}
