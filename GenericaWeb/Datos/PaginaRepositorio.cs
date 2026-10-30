using System;
using System.Collections.Generic;
using System.Data;
using GenericaWeb.Modelos;
using GenericaWeb.Seguridad;

namespace GenericaWeb.Datos
{
    public static class PaginaRepositorio
    {
        /// <summary>
        /// Condición de acceso: rol administrador, o la página está en la PermisoLista
        /// asignada al usuario (UsuarioPermisosPaginasId) o a su rol (RolPermisosId).
        /// Requiere alias u (Usuario), r (Rol) y p (Pagina).
        /// </summary>
        private const string CondicionAcceso = @"
(u.UsuarioRolId = @RolAdmin OR EXISTS (
    SELECT 1 FROM Sistema.PermisoLista pl
    WHERE pl.PermisoListaPaginaId = p.PaginaId
      AND pl.PermisoListaId IN (u.UsuarioPermisosPaginasId, r.RolPermisosId)))";

        public static bool TieneAcceso(short usuarioId, string ruta)
        {
            var resultado = Db.Escalar(@"
SELECT CASE WHEN EXISTS (
    SELECT 1
    FROM Sistema.Usuario u
    JOIN Sistema.Rol r ON r.RolId = u.UsuarioRolId
    LEFT JOIN Sistema.Pagina p ON p.PaginaDesc = @Ruta
    WHERE u.UsuarioId = @UsuarioId AND " + CondicionAcceso + @"
) THEN 1 ELSE 0 END",
                Db.P("@UsuarioId", usuarioId),
                Db.Texto("@Ruta", ruta, 50),
                Db.P("@RolAdmin", Configuracion.RolAdministradorId));
            return Convert.ToInt32(resultado) == 1;
        }

        /// <summary>Menú del usuario: grupos de Sistema.ListaPagina con las páginas a las que tiene acceso.</summary>
        public static List<MenuGrupo> MenuDeUsuario(short usuarioId)
        {
            var tabla = Db.Consultar(@"
SELECT lp.ListaPaginaId, lp.ListaPaginaDesc, p.PaginaDesc AS Ruta, p.Descripcion AS Titulo
FROM Sistema.ListaPagina lp
JOIN Sistema.ListaPaginaElemento e ON e.ListaPaginaId = lp.ListaPaginaId
JOIN Sistema.Pagina p              ON p.PaginaId = e.PaginaId
JOIN Sistema.Usuario u             ON u.UsuarioId = @UsuarioId
JOIN Sistema.Rol r                 ON r.RolId = u.UsuarioRolId
WHERE " + CondicionAcceso + @"
ORDER BY lp.ListaPaginaDesc, lp.ListaPaginaId, p.Descripcion",
                Db.P("@UsuarioId", usuarioId),
                Db.P("@RolAdmin", Configuracion.RolAdministradorId));

            var grupos = new List<MenuGrupo>();
            MenuGrupo actual = null;
            object idActual = null;
            foreach (DataRow fila in tabla.Rows)
            {
                if (actual == null || !fila["ListaPaginaId"].Equals(idActual))
                {
                    idActual = fila["ListaPaginaId"];
                    actual = new MenuGrupo { Titulo = (string)fila["ListaPaginaDesc"] };
                    grupos.Add(actual);
                }
                actual.Paginas.Add(new MenuElemento { Ruta = (string)fila["Ruta"], Titulo = (string)fila["Titulo"] });
            }
            return grupos;
        }

        public static DataTable Listar()
        {
            return Db.Consultar("SELECT PaginaId, PaginaDesc, Descripcion, PaginaUsys, PaginaDT FROM Sistema.Pagina ORDER BY PaginaDesc");
        }

        public static DataRow Obtener(short paginaId)
        {
            return Db.ConsultarFila("SELECT PaginaId, PaginaDesc, Descripcion FROM Sistema.Pagina WHERE PaginaId = @Id", Db.P("@Id", paginaId));
        }

        public static void Guardar(short paginaId, string ruta, string descripcion, string usys)
        {
            var parametros = new[]
            {
                Db.P("@Id", paginaId),
                Db.Texto("@Usys", usys, 30),
                Db.Texto("@Ruta", ruta, 50),
                Db.Texto("@Descripcion", descripcion, 150)
            };

            if (paginaId == 0)
                Db.Ejecutar("INSERT INTO Sistema.Pagina (PaginaUsys, PaginaDesc, Descripcion) VALUES (@Usys, @Ruta, @Descripcion)", parametros);
            else
                Db.Ejecutar(@"UPDATE Sistema.Pagina SET PaginaUsys = @Usys, PaginaDT = SYSDATETIME(), PaginaDesc = @Ruta,
                              Descripcion = @Descripcion WHERE PaginaId = @Id", parametros);
        }

        /// <summary>Elimina la página y sus asignaciones. Devuelve false si es página de inicio de algún rol.</summary>
        public static bool Eliminar(short paginaId)
        {
            var usadaPorRol = Convert.ToInt32(Db.Escalar("SELECT COUNT(1) FROM Sistema.Rol WHERE RolPaginaHomeId = @Id", Db.P("@Id", paginaId)));
            if (usadaPorRol > 0)
                return false;

            Db.Transaccion((c, t) =>
            {
                Db.Ejecutar(c, t, "DELETE FROM Sistema.PermisoLista WHERE PermisoListaPaginaId = @Id", Db.P("@Id", paginaId));
                Db.Ejecutar(c, t, "DELETE FROM Sistema.ListaPaginaElemento WHERE PaginaId = @Id", Db.P("@Id", paginaId));
                Db.Ejecutar(c, t, "DELETE FROM Sistema.Pagina WHERE PaginaId = @Id", Db.P("@Id", paginaId));
            });
            return true;
        }
    }
}
