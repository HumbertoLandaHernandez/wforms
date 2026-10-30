using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GenericaWeb.Datos
{
    /// <summary>
    /// Sistema.PermisoLista: cada PermisoListaId agrupa las páginas permitidas.
    /// Se asigna a usuarios (UsuarioPermisosPaginasId) y a roles (RolPermisosId).
    /// </summary>
    public static class PermisoListaRepositorio
    {
        public static DataTable ListarResumen()
        {
            return Db.Consultar(@"
SELECT pl.PermisoListaId,
       COUNT(*) AS Paginas,
       (SELECT COUNT(*) FROM Sistema.Usuario u WHERE u.UsuarioPermisosPaginasId = pl.PermisoListaId) AS Usuarios,
       (SELECT COUNT(*) FROM Sistema.Rol r WHERE r.RolPermisosId = pl.PermisoListaId) AS Roles
FROM Sistema.PermisoLista pl
GROUP BY pl.PermisoListaId
ORDER BY pl.PermisoListaId");
        }

        public static bool Existe(int listaId)
        {
            return Convert.ToInt32(Db.Escalar("SELECT COUNT(1) FROM Sistema.PermisoLista WHERE PermisoListaId = @Id", Db.P("@Id", listaId))) > 0;
        }

        public static HashSet<int> PaginasDeLista(int listaId)
        {
            var tabla = Db.Consultar("SELECT PermisoListaPaginaId FROM Sistema.PermisoLista WHERE PermisoListaId = @Id", Db.P("@Id", listaId));
            return new HashSet<int>(tabla.Rows.Cast<DataRow>().Select(f => (int)f[0]));
        }

        /// <summary>Reemplaza las páginas de la lista. Con cero páginas la lista deja de existir.</summary>
        public static void Guardar(int listaId, IEnumerable<int> paginas, string usys)
        {
            Db.Transaccion((c, t) =>
            {
                Db.Ejecutar(c, t, "DELETE FROM Sistema.PermisoLista WHERE PermisoListaId = @Id", Db.P("@Id", listaId));
                foreach (var paginaId in paginas.Distinct())
                {
                    Db.Ejecutar(c, t, @"INSERT INTO Sistema.PermisoLista (PermisoListaUsys, PermisoListaId, PermisoListaPaginaId)
                                        VALUES (@Usys, @Id, @PaginaId)",
                        Db.Texto("@Usys", usys, 30), Db.P("@Id", listaId), Db.P("@PaginaId", paginaId));
                }
            });
        }

        public static void Eliminar(int listaId)
        {
            Db.Ejecutar("DELETE FROM Sistema.PermisoLista WHERE PermisoListaId = @Id", Db.P("@Id", listaId));
        }
    }
}
