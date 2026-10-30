using System.Security.Claims;
using System.Security.Principal;
using GenericaWeb.Seguridad;

namespace GenericaWeb.Modelos
{
    /// <summary>Datos del usuario autenticado que viajan en el JWT.</summary>
    public class UsuarioSesion
    {
        public short UsuarioId { get; set; }
        public string UsuarioDesc { get; set; }
        public byte RolId { get; set; }
        public string RolDesc { get; set; }
        public string NombreCompleto { get; set; }
        public short PermisoId { get; set; }
        public string PaginaHome { get; set; }

        public static UsuarioSesion DesdePrincipal(IPrincipal principal)
        {
            var cp = principal as ClaimsPrincipal;
            if (cp == null || cp.Identity == null || !cp.Identity.IsAuthenticated)
                return null;

            System.Func<string, string> Valor = tipo => cp.FindFirst(tipo)?.Value;

            short usuarioId, permisoId;
            byte rolId;
            if (!short.TryParse(Valor(JwtServicio.Claims.UsuarioId), out usuarioId)
                || !byte.TryParse(Valor(JwtServicio.Claims.RolId), out rolId)
                || !short.TryParse(Valor(JwtServicio.Claims.PermisoId), out permisoId))
                return null;

            return new UsuarioSesion
            {
                UsuarioId = usuarioId,
                UsuarioDesc = Valor(JwtServicio.Claims.Usuario),
                RolId = rolId,
                RolDesc = Valor(JwtServicio.Claims.Rol),
                NombreCompleto = Valor(JwtServicio.Claims.NombreCompleto),
                PermisoId = permisoId,
                PaginaHome = Valor(JwtServicio.Claims.PaginaHome)
            };
        }
    }
}
