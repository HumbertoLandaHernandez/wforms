using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GenericaWeb.Datos
{
    /// <summary>
    /// Sistema.PermisoLista (encabezado) y Sistema.PermisoListaElemento (permisos de la lista).
    /// Cada rol tiene una lista de permisos (Rol.RolPermisosId).
    /// </summary>
    public static class PermisoListaRepositorio
    {
        public static DataTable Listar()
        {
            return Db.Consultar(@"
SELECT pl.PermisoListaId, pl.PermisoListaDesc, pl.PermisoListaUsys,
       STUFF((SELECT ', ' + p.PermisoDesc
              FROM Sistema.PermisoListaElemento e JOIN Sistema.Permiso p ON p.PermisoId = e.PermisoId
              WHERE e.PermisoListaId = pl.PermisoListaId
              ORDER BY p.PermisoDesc
              FOR XML PATH(''), TYPE).value('.', 'varchar(max)'), 1, 2, '') AS Permisos,
       (SELECT COUNT(*) FROM Sistema.Rol r WHERE r.RolPermisosId = pl.PermisoListaId) AS Roles
FROM Sistema.PermisoLista pl
ORDER BY pl.PermisoListaDesc");
        }

        public static string ObtenerDescripcion(int listaId)
        {
            return Db.Escalar("SELECT PermisoListaDesc FROM Sistema.PermisoLista WHERE PermisoListaId = @Id", Db.P("@Id", listaId)) as string;
        }

        public static HashSet<short> Elementos(int listaId)
        {
            var tabla = Db.Consultar("SELECT PermisoId FROM Sistema.PermisoListaElemento WHERE PermisoListaId = @Id", Db.P("@Id", listaId));
            return new HashSet<short>(tabla.Rows.Cast<DataRow>().Select(f => (short)f[0]));
        }

        /// <summary>Inserta o actualiza la lista y reemplaza sus permisos. Devuelve el Id.</summary>
        public static int Guardar(int listaId, string desc, IEnumerable<short> permisos, string usys)
        {
            var id = listaId;
            Db.Transaccion((c, t) =>
            {
                if (id == 0)
                {
                    id = Convert.ToInt32(Db.Escalar(c, t, @"
INSERT INTO Sistema.PermisoLista (PermisoListaUsys, PermisoListaDesc) VALUES (@Usys, @Desc);
SELECT CAST(SCOPE_IDENTITY() AS int);",
                        Db.Texto("@Usys", usys, 30), Db.Texto("@Desc", desc, 50)));
                }
                else
                {
                    Db.Ejecutar(c, t, @"UPDATE Sistema.PermisoLista SET PermisoListaUsys = @Usys, PermisoListaRV = SYSDATETIME(),
                                        PermisoListaDesc = @Desc WHERE PermisoListaId = @Id",
                        Db.P("@Id", id), Db.Texto("@Usys", usys, 30), Db.Texto("@Desc", desc, 50));
                }

                Db.Ejecutar(c, t, "DELETE FROM Sistema.PermisoListaElemento WHERE PermisoListaId = @Id", Db.P("@Id", id));
                foreach (var permisoId in permisos.Distinct())
                {
                    Db.Ejecutar(c, t, @"INSERT INTO Sistema.PermisoListaElemento (PermisoListaElementoUsys, PermisoListaId, PermisoId)
                                        VALUES (@Usys, @Id, @PermisoId)",
                        Db.Texto("@Usys", usys, 30), Db.P("@Id", id), Db.P("@PermisoId", permisoId));
                }
            });
            return id;
        }

        /// <summary>Elimina la lista y sus elementos. Falla (FK) si algún rol la usa.</summary>
        public static void Eliminar(int listaId)
        {
            Db.Transaccion((c, t) =>
            {
                Db.Ejecutar(c, t, "DELETE FROM Sistema.PermisoListaElemento WHERE PermisoListaId = @Id", Db.P("@Id", listaId));
                Db.Ejecutar(c, t, "DELETE FROM Sistema.PermisoLista WHERE PermisoListaId = @Id", Db.P("@Id", listaId));
            });
        }
    }
}
