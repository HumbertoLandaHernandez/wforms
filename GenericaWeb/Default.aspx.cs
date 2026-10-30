using System;
using GenericaWeb.Datos;
using GenericaWeb.Seguridad;

namespace GenericaWeb
{
    public partial class _Default : PaginaSegura
    {
        // Disponible para cualquier usuario autenticado.
        protected override bool ValidarPermisoPagina => false;

        protected void Page_Load(object sender, EventArgs e)
        {
            litNombre.Text = UsuarioActual.NombreCompleto;
            litUsuario.Text = UsuarioActual.UsuarioDesc;
            litRol.Text = string.Join(", ", UsuarioActual.Roles);
            litPermiso.Text = EsAdministrador ? "Administrador (acceso total)" : PuedeEditar ? "Lectura y escritura" : "Solo lectura";

            var cookie = Request.Cookies[JwtServicio.NombreCookie];
            DateTime expiraUtc;
            if (cookie != null && JwtServicio.ValidarToken(cookie.Value, out expiraUtc) != null)
                litExpira.Text = expiraUtc.ToString("yyyy-MM-dd HH:mm:ss");

            var menu = PaginaRepositorio.MenuDeUsuario(UsuarioActual.UsuarioId);
            rptGrupos.DataSource = menu;
            rptGrupos.DataBind();
            litSinPaginas.Visible = menu.Count == 0;
        }
    }
}
