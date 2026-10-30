using System.Collections.Generic;

namespace GenericaWeb.Modelos
{
    /// <summary>Campos editables de Sistema.Usuario y sus roles (Sistema.UsuarioRol).</summary>
    public class UsuarioDatos
    {
        public short UsuarioId { get; set; }
        public string UsuarioDesc { get; set; }
        public byte RolId { get; set; }
        public short PermisosId { get; set; }
        public int NombreId { get; set; }
        public int PermisosPaginasId { get; set; }
        public short PaginaHomeId { get; set; }
        /// <summary>Roles adicionales; el rol principal se agrega siempre a UsuarioRol.</summary>
        public List<byte> Roles { get; set; } = new List<byte>();
    }
}
