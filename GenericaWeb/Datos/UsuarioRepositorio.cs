using System;
using System.Data;
using GenericaWeb.Modelos;

namespace GenericaWeb.Datos
{
    public static class UsuarioRepositorio
    {
        private const string SelectSesion = @"
SELECT u.UsuarioId, u.UsuarioDesc, u.[Contraseña] AS Hash, u.UsuarioRolId, r.RolDesc,
       n.NombrePersonalDesc, u.UsuarioPermisosId, p.PaginaDesc AS PaginaHome
FROM Sistema.Usuario u
JOIN Sistema.Rol r            ON r.RolId = u.UsuarioRolId
JOIN Sistema.NombrePersonal n ON n.NombrePersonalId = u.UsuarioNombreId
JOIN Sistema.Pagina p         ON p.PaginaId = u.UsuarioPaginaHomeId";

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

        public static DataTable Listar()
        {
            return Db.Consultar(@"
SELECT u.UsuarioId, u.UsuarioDesc, n.NombrePersonalDesc, r.RolDesc, pe.PermisoDesc,
       u.UsuarioPermisosPaginasId, p.Descripcion AS PaginaHome, u.UsuarioUsys, u.UsuarioDT
FROM Sistema.Usuario u
JOIN Sistema.Rol r            ON r.RolId = u.UsuarioRolId
JOIN Sistema.NombrePersonal n ON n.NombrePersonalId = u.UsuarioNombreId
JOIN Sistema.Permiso pe       ON pe.PermisoId = u.UsuarioPermisosId
JOIN Sistema.Pagina p         ON p.PaginaId = u.UsuarioPaginaHomeId
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

            Db.Ejecutar(@"
INSERT INTO Sistema.Usuario (UsuarioUsys, UsuarioDesc, [Contraseña], UsuarioRolId, UsuarioPermisosId,
                             UsuarioNombreId, UsuarioPermisosPaginasId, UsuarioPaginaHomeId)
VALUES (@Usys, @Desc, @Hash, @RolId, @PermisosId, @NombreId, @PermisosPaginasId, @PaginaHomeId)",
                Parametros(d, hash, usys));
        }

        /// <summary>Actualiza el usuario; si hash es null la contraseña no cambia.</summary>
        public static void Actualizar(UsuarioDatos d, string hash, string usys)
        {
            Db.Ejecutar(@"
UPDATE Sistema.Usuario SET
    UsuarioUsys = @Usys, UsuarioDT = SYSDATETIME(), UsuarioDesc = @Desc,
    [Contraseña] = COALESCE(@Hash, [Contraseña]),
    UsuarioRolId = @RolId, UsuarioPermisosId = @PermisosId, UsuarioNombreId = @NombreId,
    UsuarioPermisosPaginasId = @PermisosPaginasId, UsuarioPaginaHomeId = @PaginaHomeId
WHERE UsuarioId = @Id",
                Parametros(d, hash, usys));
        }

        public static void Eliminar(short usuarioId)
        {
            Db.Ejecutar("DELETE FROM Sistema.Usuario WHERE UsuarioId = @Id", Db.P("@Id", usuarioId));
        }

        private static System.Data.SqlClient.SqlParameter[] Parametros(UsuarioDatos d, string hash, string usys)
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
            return new UsuarioSesion
            {
                UsuarioId = (short)f["UsuarioId"],
                UsuarioDesc = (string)f["UsuarioDesc"],
                RolId = (byte)f["UsuarioRolId"],
                RolDesc = (string)f["RolDesc"],
                NombreCompleto = (string)f["NombrePersonalDesc"],
                PermisoId = (short)f["UsuarioPermisosId"],
                PaginaHome = (string)f["PaginaHome"]
            };
        }
    }
}
