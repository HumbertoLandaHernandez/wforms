using System;
using System.Web.UI;
using GenericaWeb.Modelos;
using GenericaWeb.Seguridad;

namespace GenericaWeb
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var usuario = UsuarioSesion.DesdePrincipal(Context.User);
            if (!IsPostBack && usuario != null)
                Response.Redirect(UrlDestino(usuario));
        }

        protected void btnEntrar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var usuario = Autenticacion.Autenticar(txtUsuario.Text, txtContrasena.Text);
            if (usuario == null)
            {
                lblError.Text = "Usuario o contraseña incorrectos.";
                lblError.Visible = true;
                return;
            }

            DateTime expiraUtc;
            var token = JwtServicio.GenerarToken(usuario, out expiraUtc);
            JwtServicio.EscribirCookie(Context, token, expiraUtc);
            Response.Redirect(UrlDestino(usuario));
        }

        /// <summary>ReturnUrl solo si es local (evita redirecciones abiertas); si no, la página de inicio del usuario.</summary>
        private string UrlDestino(UsuarioSesion usuario)
        {
            var retorno = Request.QueryString["ReturnUrl"];
            var esLocal = !string.IsNullOrEmpty(retorno)
                          && retorno.StartsWith("/", StringComparison.Ordinal)
                          && !retorno.StartsWith("//", StringComparison.Ordinal)
                          && !retorno.StartsWith("/\\", StringComparison.Ordinal);
            if (esLocal)
                return retorno;

            return string.IsNullOrEmpty(usuario.PaginaHome) ? "~/Default.aspx" : usuario.PaginaHome;
        }
    }
}
