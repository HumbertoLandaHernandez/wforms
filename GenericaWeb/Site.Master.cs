using System;
using System.Web;
using System.Web.UI;
using GenericaWeb.Datos;
using GenericaWeb.Modelos;
using GenericaWeb.Seguridad;

namespace GenericaWeb
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var usuario = UsuarioSesion.DesdePrincipal(Context.User);
            if (usuario == null)
                return;

            litUsuario.Text = HttpUtility.HtmlEncode(usuario.NombreCompleto + " · " + usuario.RolDesc);

            // Se arma en cada petición para reflejar cambios de permisos de inmediato.
            rptMenu.DataSource = PaginaRepositorio.MenuDeUsuario(usuario.UsuarioId);
            rptMenu.DataBind();
        }

        protected void btnSalir_Click(object sender, EventArgs e)
        {
            JwtServicio.EliminarCookie(Context);
            Response.Redirect("~/Login.aspx");
        }
    }
}
