using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using GenericaWeb.Datos;
using GenericaWeb.Seguridad;

namespace GenericaWeb.Admin
{
    public partial class NombresLista : PaginaSegura
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            Combos.NombresPersonales(ddlNombrePersonal);
            Combos.NombresTipo(ddlNombreTipo);
            Combos.NombresBase(ddlNombreBase);
            pnlEdicion.Visible = PuedeEditar;
            CargarGrid();
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!ExigirEdicion(lblMensaje) || !Page.IsValid)
                return;

            try
            {
                NombreListaRepositorio.Agregar(
                    int.Parse(ddlNombrePersonal.SelectedValue),
                    byte.Parse(ddlNombreTipo.SelectedValue),
                    int.Parse(ddlNombreBase.SelectedValue),
                    UsuarioActual.UsuarioDesc);
                MostrarMensaje(lblMensaje, "Relación agregada.", true);
                CargarGrid();
            }
            catch (SqlException ex)
            {
                MostrarMensaje(lblMensaje, Db.MensajeError(ex), false);
            }
        }

        protected void gvNombres_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Eliminar" || !ExigirEdicion(lblMensaje))
                return;

            var partes = ((string)e.CommandArgument).Split('|');
            try
            {
                NombreListaRepositorio.Eliminar(int.Parse(partes[0]), byte.Parse(partes[1]), int.Parse(partes[2]));
                MostrarMensaje(lblMensaje, "Relación eliminada.", true);
                CargarGrid();
            }
            catch (SqlException ex)
            {
                MostrarMensaje(lblMensaje, Db.MensajeError(ex), false);
            }
        }

        private void CargarGrid()
        {
            gvNombres.DataSource = NombreListaRepositorio.Listar();
            gvNombres.DataBind();
        }
    }
}
