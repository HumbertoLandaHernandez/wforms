using System;
using System.Threading;
using System.Web;
using GenericaWeb.Datos;
using GenericaWeb.Modelos;

namespace GenericaWeb.Seguridad
{
    /// <summary>
    /// Autentica cada petición a partir del JWT recibido en el encabezado
    /// "Authorization: Bearer ..." (API) o en la cookie HttpOnly (páginas Web Forms).
    /// Renueva la cookie cuando ha transcurrido más de la mitad de su vigencia.
    /// </summary>
    public class JwtAuthModule : IHttpModule
    {
        public void Init(HttpApplication aplicacion)
        {
            aplicacion.AuthenticateRequest += AlAutenticar;
        }

        public void Dispose()
        {
        }

        private static void AlAutenticar(object sender, EventArgs e)
        {
            var contexto = ((HttpApplication)sender).Context;

            string token = null;
            var desdeCookie = false;
            var encabezado = contexto.Request.Headers["Authorization"];
            if (encabezado != null && encabezado.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = encabezado.Substring(7).Trim();
            }
            else
            {
                var cookie = contexto.Request.Cookies[JwtServicio.NombreCookie];
                if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                {
                    token = cookie.Value;
                    desdeCookie = true;
                }
            }

            if (token == null)
                return;

            DateTime expiraUtc;
            var principal = JwtServicio.ValidarToken(token, out expiraUtc);
            if (principal == null)
            {
                if (desdeCookie)
                    JwtServicio.EliminarCookie(contexto);
                return;
            }

            contexto.User = principal;
            Thread.CurrentPrincipal = principal;

            var mitadVigencia = TimeSpan.FromMinutes(Configuracion.JwtMinutosExpiracion / 2.0);
            if (desdeCookie && expiraUtc - DateTime.UtcNow < mitadVigencia)
                Renovar(contexto);
        }

        /// <summary>Emite un token nuevo con los datos actuales del usuario (por si cambiaron rol, permisos, etc.).</summary>
        private static void Renovar(HttpContext contexto)
        {
            var actual = UsuarioSesion.DesdePrincipal(contexto.User);
            var usuario = actual == null ? null : UsuarioRepositorio.ObtenerSesion(actual.UsuarioId);
            if (usuario == null)
            {
                JwtServicio.EliminarCookie(contexto);
                return;
            }

            DateTime expiraUtc;
            var token = JwtServicio.GenerarToken(usuario, out expiraUtc);
            JwtServicio.EscribirCookie(contexto, token, expiraUtc);
        }
    }
}
