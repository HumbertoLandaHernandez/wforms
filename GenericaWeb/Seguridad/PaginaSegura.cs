using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GenericaWeb.Datos;
using GenericaWeb.Modelos;

namespace GenericaWeb.Seguridad
{
    /// <summary>
    /// Página base: exige un JWT válido y que la página (Sistema.Pagina.PaginaDesc) esté en la
    /// lista de páginas del usuario (Usuario.UsuarioPermisosPaginasId → Sistema.PaginaListaElemento).
    /// Los permisos de edición se leen de la BD en cada petición para reflejar cambios de inmediato.
    /// </summary>
    public class PaginaSegura : Page
    {
        protected UsuarioSesion UsuarioActual { get; private set; }

        /// <summary>Las páginas accesibles para cualquier usuario autenticado devuelven false.</summary>
        protected virtual bool ValidarPermisoPagina => true;

        protected bool EsAdministrador { get; private set; }

        /// <summary>Permisos efectivos actuales (listas de permisos de sus roles + permiso individual).</summary>
        protected HashSet<short> Permisos { get; private set; } = new HashSet<short>();

        protected bool PuedeEditar => EsAdministrador || Permisos.Contains(Configuracion.PermisoEscrituraId);

        protected bool TienePermiso(short permisoId) => EsAdministrador || Permisos.Contains(permisoId);

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

            EsAdministrador = UsuarioRepositorio.EsAdministrador(UsuarioActual.UsuarioId);
            Permisos = UsuarioRepositorio.PermisosEfectivos(UsuarioActual.UsuarioId);

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
            MostrarMensaje(etiqueta, "No tiene permiso de escritura.", false);
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
