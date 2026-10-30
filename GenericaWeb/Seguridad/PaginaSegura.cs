using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GenericaWeb.Datos;
using GenericaWeb.Modelos;

namespace GenericaWeb.Seguridad
{
    /// <summary>
    /// Página base: exige un JWT válido y que la página (Sistema.Pagina.PaginaDesc) esté
    /// en la lista de permisos del usuario o de su rol (Sistema.PermisoLista).
    /// </summary>
    public class PaginaSegura : Page
    {
        protected UsuarioSesion UsuarioActual { get; private set; }

        /// <summary>Las páginas accesibles para cualquier usuario autenticado devuelven false.</summary>
        protected virtual bool ValidarPermisoPagina => true;

        protected bool PuedeEditar => UsuarioActual != null && UsuarioActual.PermisoId != Configuracion.PermisoSoloLecturaId;

        protected override void OnInit(EventArgs e)
        {
            UsuarioActual = UsuarioSesion.DesdePrincipal(Context.User);
            if (UsuarioActual == null)
            {
                Response.Redirect("~/Login.aspx?ReturnUrl=" + HttpUtility.UrlEncode(Request.RawUrl));
                return;
            }

            // Protección CSRF: el ViewState queda ligado al usuario autenticado.
            ViewStateUserKey = UsuarioActual.UsuarioId.ToString();

            if (ValidarPermisoPagina && !PaginaRepositorio.TieneAcceso(UsuarioActual.UsuarioId, Request.AppRelativeCurrentExecutionFilePath))
            {
                Response.Redirect("~/AccesoDenegado.aspx");
                return;
            }

            base.OnInit(e);
        }

        /// <summary>Verifica permiso de escritura antes de modificar datos.</summary>
        protected bool ExigirEdicion(Label etiqueta)
        {
            if (PuedeEditar)
                return true;
            MostrarMensaje(etiqueta, "Su permiso es de solo lectura.", false);
            return false;
        }

        protected static void MostrarMensaje(Label etiqueta, string texto, bool exito)
        {
            etiqueta.Text = HttpUtility.HtmlEncode(texto);
            etiqueta.CssClass = "alert d-block " + (exito ? "alert-success" : "alert-danger");
            etiqueta.Visible = true;
        }
    }
}
