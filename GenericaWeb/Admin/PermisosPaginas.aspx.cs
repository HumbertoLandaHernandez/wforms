using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;
using GenericaWeb.Datos;
using GenericaWeb.Seguridad;

namespace GenericaWeb.Admin
{
    public partial class PermisosPaginas : PaginaSegura
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            Combos.Paginas(cblPaginas, false);
            pnlEdicion.Visible = PuedeEditar;
            CargarGrid();
            Limpiar();
        }

        protected void gvListas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var id = int.Parse((string)e.CommandArgument);
            if (e.CommandName == "Editar")
            {
                var paginas = PermisoListaRepositorio.PaginasDeLista(id);
                txtListaId.Text = id.ToString();
                foreach (ListItem item in cblPaginas.Items)
                    item.Selected = paginas.Contains(int.Parse(item.Value));
            }
            else if (e.CommandName == "Eliminar" && ExigirEdicion(lblMensaje))
            {
                try
                {
                    PermisoListaRepositorio.Eliminar(id);
                    MostrarMensaje(lblMensaje, "Lista eliminada.", true);
                    Limpiar();
                    CargarGrid();
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

            var paginas = cblPaginas.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => int.Parse(i.Value)).ToList();
            if (paginas.Count == 0)
            {
                MostrarMensaje(lblMensaje, "Seleccione al menos una página (o use Eliminar para borrar la lista).", false);
                return;
            }

            try
            {
                PermisoListaRepositorio.Guardar(int.Parse(txtListaId.Text), paginas, UsuarioActual.UsuarioDesc);
                MostrarMensaje(lblMensaje, "Lista guardada.", true);
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
            var siguiente = PermisoListaRepositorio.ListarResumen().Rows.Cast<System.Data.DataRow>()
                .Select(f => (int)f["PermisoListaId"]).DefaultIfEmpty(0).Max() + 1;
            txtListaId.Text = siguiente.ToString();
            cblPaginas.ClearSelection();
        }

        private void CargarGrid()
        {
            gvListas.DataSource = PermisoListaRepositorio.ListarResumen();
            gvListas.DataBind();
        }
    }
}
