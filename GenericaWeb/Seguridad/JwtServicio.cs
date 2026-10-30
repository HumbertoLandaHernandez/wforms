using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using GenericaWeb.Modelos;
using Microsoft.IdentityModel.Tokens;

namespace GenericaWeb.Seguridad
{
    /// <summary>Emisión y validación de JWT (HS256) y manejo de la cookie que lo transporta.</summary>
    public static class JwtServicio
    {
        public const string NombreCookie = "Generica.Token";

        /// <summary>Nombres de los claims incluidos en el token.</summary>
        public static class Claims
        {
            public const string UsuarioId = JwtRegisteredClaimNames.Sub;
            public const string Usuario = "name";
            public const string Rol = "role";
            public const string RolId = "rol_id";
            public const string NombreCompleto = "nombre";
            public const string PermisoId = "permiso_id";
            public const string PaginaHome = "home";
        }

        /// <summary>Lanza una excepción si la configuración JWT no es válida. Se llama desde Application_Start.</summary>
        public static void ValidarConfiguracion()
        {
            ObtenerLlave();
        }

        public static string GenerarToken(UsuarioSesion usuario, out DateTime expiraUtc)
        {
            var ahora = DateTime.UtcNow;
            expiraUtc = ahora.AddMinutes(Configuracion.JwtMinutosExpiracion);

            var claims = new[]
            {
                new Claim(Claims.UsuarioId, usuario.UsuarioId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(ahora).ToString(), ClaimValueTypes.Integer64),
                new Claim(Claims.Usuario, usuario.UsuarioDesc),
                new Claim(Claims.Rol, usuario.RolDesc ?? string.Empty),
                new Claim(Claims.RolId, usuario.RolId.ToString()),
                new Claim(Claims.NombreCompleto, usuario.NombreCompleto ?? string.Empty),
                new Claim(Claims.PermisoId, usuario.PermisoId.ToString()),
                new Claim(Claims.PaginaHome, usuario.PaginaHome ?? string.Empty)
            };

            var credenciales = new SigningCredentials(ObtenerLlave(), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(Configuracion.JwtEmisor, Configuracion.JwtAudiencia, claims, ahora, expiraUtc, credenciales);
            return CrearManejador().WriteToken(token);
        }

        /// <summary>Devuelve el principal si el token es válido; null en caso contrario.</summary>
        public static ClaimsPrincipal ValidarToken(string token, out DateTime expiraUtc)
        {
            expiraUtc = DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var parametros = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Configuracion.JwtEmisor,
                ValidateAudience = true,
                ValidAudience = Configuracion.JwtAudiencia,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = ObtenerLlave(),
                ClockSkew = TimeSpan.FromMinutes(1),
                NameClaimType = Claims.Usuario,
                RoleClaimType = Claims.Rol
            };

            try
            {
                SecurityToken validado;
                var principal = CrearManejador().ValidateToken(token, parametros, out validado);
                var jwt = validado as JwtSecurityToken;
                if (jwt == null || jwt.Header.Alg != SecurityAlgorithms.HmacSha256)
                    return null;

                expiraUtc = jwt.ValidTo;
                return principal;
            }
            catch (Exception)
            {
                // Firma inválida, token expirado, mal formado, etc.
                return null;
            }
        }

        public static void EscribirCookie(HttpContext contexto, string token, DateTime expiraUtc)
        {
            contexto.Response.Cookies.Set(new HttpCookie(NombreCookie, token)
            {
                HttpOnly = true,
                Secure = contexto.Request.IsSecureConnection,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = expiraUtc
            });
        }

        public static void EliminarCookie(HttpContext contexto)
        {
            contexto.Response.Cookies.Set(new HttpCookie(NombreCookie, string.Empty)
            {
                HttpOnly = true,
                Secure = contexto.Request.IsSecureConnection,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(-1)
            });
        }

        private static SymmetricSecurityKey ObtenerLlave()
        {
            var clave = Configuracion.JwtClave;
            if (string.IsNullOrEmpty(clave) || clave.Length < 32)
                throw new ConfigurationErrorsException("La clave JWT (appSettings 'Jwt:Clave' o GENERICA_JWT_CLAVE) debe tener al menos 32 caracteres.");
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave));
        }

        private static JwtSecurityTokenHandler CrearManejador()
        {
            // Sin mapeo: los claims conservan los nombres cortos del token (sub, name, role...).
            var manejador = new JwtSecurityTokenHandler();
            manejador.InboundClaimTypeMap.Clear();
            manejador.OutboundClaimTypeMap.Clear();
            return manejador;
        }
    }
}
