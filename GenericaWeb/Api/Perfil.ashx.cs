using System.Web;
using GenericaWeb.Datos;
using GenericaWeb.Modelos;

namespace GenericaWeb.Api
{
    /// <summary>
    /// GET /Api/Perfil.ashx   (Authorization: Bearer &lt;token&gt;)
    /// Devuelve los datos del usuario del token y su menú de páginas.
    /// </summary>
    public class Perfil : IHttpHandler
    {
        public bool IsReusable => true;

        public void ProcessRequest(HttpContext contexto)
        {
            var usuario = UsuarioSesion.DesdePrincipal(contexto.User);
            if (usuario == null)
            {
                contexto.Response.AddHeader("WWW-Authenticate", "Bearer");
                ApiRespuesta.Json(contexto, 401, new { error = "Token ausente, inválido o expirado." });
                return;
            }

            ApiRespuesta.Json(contexto, 200, new
            {
                usuario.UsuarioId,
                usuario.UsuarioDesc,
                usuario.NombreCompleto,
                usuario.RolId,
                usuario.RolDesc,
                usuario.PermisoId,
                usuario.PaginaHome,
                menu = PaginaRepositorio.MenuDeUsuario(usuario.UsuarioId)
            });
        }
    }
}
