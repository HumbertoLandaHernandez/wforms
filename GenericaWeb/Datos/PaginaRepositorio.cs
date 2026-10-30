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
        /// Condición de acceso: el usuario tiene el rol administrador (principal o en UsuarioRol),
        /// o la página está en su lista de páginas (Usuario.UsuarioPermisosPaginasId → PaginaListaElemento).
        /// Requiere alias u (Usuario) y p (Pagina).
        /// </summary>
        private const string CondicionAcceso = @"
(u.UsuarioRolId = @RolAdmin
 OR EXISTS (SELECT 1 FROM Sistema.UsuarioRol ur WHERE ur.UsuarioId = u.UsuarioId AND ur.RolId = @RolAdmin)
 OR EXISTS (SELECT 1 FROM Sistema.PaginaListaElemento ple
            WHERE ple.PaginaId = p.PaginaId AND ple.PaginaListaId = u.UsuarioPermisosPaginasId))";

        public static bool TieneAcceso(short usuarioId, string ruta)
        {
            var resultado = Db.Escalar(@"
SELECT CASE WHEN EXISTS (
    SELECT 1
    FROM Sistema.Usuario u
    LEFT JOIN Sistema.Pagina p ON p.PaginaDesc = @Ruta
    WHERE u.UsuarioId = @UsuarioId AND " + CondicionAcceso + @"
) THEN 1 ELSE 0 END",
                Db.P("@UsuarioId", usuarioId),
                Db.Texto("@Ruta", ruta, 50),
                Db.P("@RolAdmin", Configuracion.RolAdministradorId));
            return Convert.ToInt32(resultado) == 1;
        }

        /// <summary>Menú del usuario: grupos de Sistema.PaginaLista con las páginas a las que tiene acceso.</summary>
        public static List<MenuGrupo> MenuDeUsuario(short usuarioId)
        {
            var tabla = Db.Consultar(@"
SELECT pl.PaginaListaId, pl.PaginaListaDesc, p.PaginaDesc AS Ruta, p.Descripcion AS Titulo
FROM Sistema.PaginaLista pl
JOIN Sistema.PaginaListaElemento e ON e.PaginaListaId = pl.PaginaListaId
JOIN Sistema.Pagina p              ON p.PaginaId = e.PaginaId
JOIN Sistema.Usuario u             ON u.UsuarioId = @UsuarioId
WHERE " + CondicionAcceso + @"
ORDER BY pl.PaginaListaDesc, pl.PaginaListaId, p.Descripcion",
                Db.P("@UsuarioId", usuarioId),
                Db.P("@RolAdmin", Configuracion.RolAdministradorId));

            var grupos = new List<MenuGrupo>();
            MenuGrupo actual = null;
            object idActual = null;
            foreach (DataRow fila in tabla.Rows)
            {
                if (actual == null || !fila["PaginaListaId"].Equals(idActual))
                {
                    idActual = fila["PaginaListaId"];
                    actual = new MenuGrupo { Titulo = (string)fila["PaginaListaDesc"] };
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
                Db.Ejecutar(c, t, "DELETE FROM Sistema.PaginaListaElemento WHERE PaginaId = @Id", Db.P("@Id", paginaId));
                Db.Ejecutar(c, t, "DELETE FROM Sistema.Pagina WHERE PaginaId = @Id", Db.P("@Id", paginaId));
            });
            return true;
        }
    }
}
