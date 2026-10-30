using System;
using System.IO;
using System.Text;
using System.Web;
using GenericaWeb.Seguridad;
using Newtonsoft.Json;

namespace GenericaWeb.Api
{
    /// <summary>
    /// POST /Api/Login.ashx   { "usuario": "admin", "contrasena": "..." }
    /// 200 → { "token": "...", "tipo": "Bearer", "expiraUtc": "..." }
    /// </summary>
    public class Login : IHttpHandler
    {
        public bool IsReusable => true;

        private class Solicitud
        {
            public string Usuario { get; set; }
            public string Contrasena { get; set; }
        }

        public void ProcessRequest(HttpContext contexto)
        {
            if (!string.Equals(contexto.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
            {
                contexto.Response.AddHeader("Allow", "POST");
                ApiRespuesta.Json(contexto, 405, new { error = "Use POST." });
                return;
            }

            Solicitud solicitud;
            try
            {
                using (var lector = new StreamReader(contexto.Request.InputStream, Encoding.UTF8))
                    solicitud = JsonConvert.DeserializeObject<Solicitud>(lector.ReadToEnd());
            }
            catch (JsonException)
            {
                solicitud = null;
            }

            if (solicitud == null || string.IsNullOrWhiteSpace(solicitud.Usuario) || string.IsNullOrEmpty(solicitud.Contrasena))
            {
                ApiRespuesta.Json(contexto, 400, new { error = "Envíe usuario y contrasena en formato JSON." });
                return;
            }

            var usuario = Autenticacion.Autenticar(solicitud.Usuario, solicitud.Contrasena);
            if (usuario == null)
            {
                ApiRespuesta.Json(contexto, 401, new { error = "Usuario o contraseña incorrectos." });
                return;
            }

            DateTime expiraUtc;
            var token = JwtServicio.GenerarToken(usuario, out expiraUtc);
            ApiRespuesta.Json(contexto, 200, new { token, tipo = "Bearer", expiraUtc });
        }
    }
}
