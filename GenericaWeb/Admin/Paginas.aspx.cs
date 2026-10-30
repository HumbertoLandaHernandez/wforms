using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using GenericaWeb.Datos;
using GenericaWeb.Seguridad;

namespace GenericaWeb.Admin
{
    public partial class Paginas : PaginaSegura
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            pnlEdicion.Visible = PuedeEditar;
            CargarGrid();
        }

        protected void gvPaginas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var id = short.Parse((string)e.CommandArgument);
            if (e.CommandName == "Editar")
            {
                var fila = PaginaRepositorio.Obtener(id);
                if (fila == null)
                    return;

                hfId.Value = id.ToString();
                txtRuta.Text = (string)fila["PaginaDesc"];
                txtDescripcion.Text = (string)fila["Descripcion"];
                litTitulo.Text = "Editar página";
            }
            else if (e.CommandName == "Eliminar" && ExigirEdicion(lblMensaje))
            {
                try
                {
                    if (PaginaRepositorio.Eliminar(id))
                    {
                        MostrarMensaje(lblMensaje, "Página eliminada.", true);
                        Limpiar();
                        CargarGrid();
                    }
                    else
                    {
                        MostrarMensaje(lblMensaje, "La página es la página de inicio de algún rol.", false);
                    }
                }
                catch (SqlException ex)
                {
                    MostrarMensaje(lblMensaje, Db.MensajeError(ex), false);
                }
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ExigirEdicion(lblMensaje) || !Page.IsValid)
                return;

            try
            {
                PaginaRepositorio.Guardar(
                    string.IsNullOrEmpty(hfId.Value) ? (short)0 : short.Parse(hfId.Value),
                    txtRuta.Text.Trim(),
                    txtDescripcion.Text.Trim(),
                    UsuarioActual.UsuarioDesc);

                MostrarMensaje(lblMensaje, "Página guardada.", true);
                Limpiar();
                CargarGrid();
            }
            catch (SqlException ex)
            {
                MostrarMensaje(lblMensaje, Db.MensajeError(ex), false);
            }
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void Limpiar()
        {
            hfId.Value = txtRuta.Text = txtDescripcion.Text = string.Empty;
            litTitulo.Text = "Nueva página";
        }

        private void CargarGrid()
        {
            gvPaginas.DataSource = PaginaRepositorio.Listar();
            gvPaginas.DataBind();
        }
    }
}
