using System;
using System.Configuration;

namespace GenericaWeb.Seguridad
{
    /// <summary>Valores de configuración leídos de Web.config (appSettings).</summary>
    public static class Configuracion
    {
        /// <summary>Clave HMAC del JWT. La variable de entorno GENERICA_JWT_CLAVE tiene prioridad sobre Web.config.</summary>
        public static string JwtClave
        {
            get
            {
                var entorno = Environment.GetEnvironmentVariable("GENERICA_JWT_CLAVE");
                return string.IsNullOrEmpty(entorno) ? ConfigurationManager.AppSettings["Jwt:Clave"] : entorno;
            }
        }

        public static string JwtEmisor => ConfigurationManager.AppSettings["Jwt:Emisor"] ?? "GenericaWeb";
        public static string JwtAudiencia => ConfigurationManager.AppSettings["Jwt:Audiencia"] ?? "GenericaWeb";
        public static int JwtMinutosExpiracion => int.Parse(ConfigurationManager.AppSettings["Jwt:MinutosExpiracion"] ?? "60");

        /// <summary>Los usuarios con este rol (principal o en UsuarioRol) acceden a todo.</summary>
        public static byte RolAdministradorId => byte.Parse(ConfigurationManager.AppSettings["Seguridad:RolAdministradorId"] ?? "1");

        /// <summary>PermisoId necesario para crear, modificar y eliminar.</summary>
        public static short PermisoEscrituraId => short.Parse(ConfigurationManager.AppSettings["Seguridad:PermisoEscrituraId"] ?? "2");
    }
}
