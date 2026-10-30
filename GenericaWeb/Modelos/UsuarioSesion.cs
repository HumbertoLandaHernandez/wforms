using System.Collections.Generic;
using System.Linq;
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
        /// <summary>Rol principal (Usuario.UsuarioRolId).</summary>
        public byte RolId { get; set; }
        public string RolDesc { get; set; }
        /// <summary>Descripción de todos los roles: principal + Sistema.UsuarioRol.</summary>
        public List<string> Roles { get; set; } = new List<string>();
        public string NombreCompleto { get; set; }
        /// <summary>Permiso individual (Usuario.UsuarioPermisosId).</summary>
        public short PermisoId { get; set; }
        /// <summary>Permisos efectivos: listas de permisos de sus roles + permiso individual.</summary>
        public List<short> Permisos { get; set; } = new List<short>();
        public string PaginaHome { get; set; }

        public static UsuarioSesion DesdePrincipal(IPrincipal principal)
        {
            var cp = principal as ClaimsPrincipal;
            if (cp == null || cp.Identity == null || !cp.Identity.IsAuthenticated)
                return null;

            System.Func<string, string> valor = tipo => cp.FindFirst(tipo)?.Value;

            short usuarioId, permisoId;
            byte rolId;
            if (!short.TryParse(valor(JwtServicio.Claims.UsuarioId), out usuarioId)
                || !byte.TryParse(valor(JwtServicio.Claims.RolId), out rolId)
                || !short.TryParse(valor(JwtServicio.Claims.PermisoId), out permisoId))
                return null;

            short p;
            return new UsuarioSesion
            {
                UsuarioId = usuarioId,
                UsuarioDesc = valor(JwtServicio.Claims.Usuario),
                RolId = rolId,
                RolDesc = valor(JwtServicio.Claims.RolPrincipal),
                Roles = cp.FindAll(JwtServicio.Claims.Rol).Select(c => c.Value).ToList(),
                NombreCompleto = valor(JwtServicio.Claims.NombreCompleto),
                PermisoId = permisoId,
                Permisos = cp.FindAll(JwtServicio.Claims.Permisos)
                             .Where(c => short.TryParse(c.Value, out p)).Select(c => short.Parse(c.Value)).ToList(),
                PaginaHome = valor(JwtServicio.Claims.PaginaHome)
            };
        }
    }
}
