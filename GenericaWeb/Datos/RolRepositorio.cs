using System.Data;

namespace GenericaWeb.Datos
{
    public static class RolRepositorio
    {
        public static DataTable Listar()
        {
            return Db.Consultar(@"
SELECT r.RolId, r.RolDesc, r.RolPaginaHomeId, p.Descripcion AS PaginaHome, r.RolPermisosId, pl.PermisoListaDesc,
       r.RolClassId, r.RolUsys,
       (SELECT COUNT(*) FROM Sistema.UsuarioRol ur WHERE ur.RolId = r.RolId) AS Usuarios
FROM Sistema.Rol r
LEFT JOIN Sistema.Pagina p        ON p.PaginaId = r.RolPaginaHomeId
JOIN Sistema.PermisoLista pl      ON pl.PermisoListaId = r.RolPermisosId
ORDER BY r.RolDesc");
        }

        public static DataRow Obtener(byte rolId)
        {
            return Db.ConsultarFila("SELECT RolId, RolDesc, RolPaginaHomeId, RolPermisosId, RolClassId FROM Sistema.Rol WHERE RolId = @Id",
                Db.P("@Id", rolId));
        }

        public static void Guardar(byte rolId, string desc, byte paginaHomeId, int permisosId, byte classId, string usys)
        {
            var parametros = new[]
            {
                Db.P("@Id", rolId),
                Db.Texto("@Usys", usys, 30),
                Db.Texto("@Desc", desc, 50),
                Db.P("@Home", paginaHomeId),
                Db.P("@Permisos", permisosId),
                Db.P("@Class", classId)
            };

            if (rolId == 0)
                Db.Ejecutar(@"INSERT INTO Sistema.Rol (RolUsys, RolDesc, RolPaginaHomeId, RolPermisosId, RolClassId)
                              VALUES (@Usys, @Desc, @Home, @Permisos, @Class)", parametros);
            else
                Db.Ejecutar(@"UPDATE Sistema.Rol SET RolUsys = @Usys, RolDesc = @Desc, RolPaginaHomeId = @Home,
                              RolPermisosId = @Permisos, RolClassId = @Class WHERE RolId = @Id", parametros);
        }

        public static void Eliminar(byte rolId)
        {
            Db.Ejecutar("DELETE FROM Sistema.Rol WHERE RolId = @Id", Db.P("@Id", rolId));
        }
    }
}
