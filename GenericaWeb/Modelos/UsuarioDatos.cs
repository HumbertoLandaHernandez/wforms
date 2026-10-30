namespace GenericaWeb.Modelos
{
    /// <summary>Campos editables de Sistema.Usuario.</summary>
    public class UsuarioDatos
    {
        public short UsuarioId { get; set; }
        public string UsuarioDesc { get; set; }
        public byte RolId { get; set; }
        public short PermisosId { get; set; }
        public int NombreId { get; set; }
        public int PermisosPaginasId { get; set; }
        public short PaginaHomeId { get; set; }
    }
}
