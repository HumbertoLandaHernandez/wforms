using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;
using GenericaWeb.Datos;
using GenericaWeb.Seguridad;

namespace GenericaWeb.Admin
{
    public partial class ListasPermisos : PaginaSegura
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            Combos.Permisos(cblPermisos, false);
            pnlEdicion.Visible = PuedeEditar;
            CargarGrid();
        }

        protected void gvListas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var id = int.Parse((string)e.CommandArgument);
            if (e.CommandName == "Editar")
            {
                var desc = PermisoListaRepositorio.ObtenerDescripcion(id);
                if (desc == null)
                    return;

                hfId.Value = id.ToString();
                txtDesc.Text = desc;
                var permisos = PermisoListaRepositorio.Elementos(id);
                foreach (ListItem item in cblPermisos.Items)
                    item.Selected = permisos.Contains(short.Parse(item.Value));
                litTitulo.Text = "Editar lista";
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

            var permisos = cblPermisos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => short.Parse(i.Value));
            try
            {
                PermisoListaRepositorio.Guardar(
                    string.IsNullOrEmpty(hfId.Value) ? 0 : int.Parse(hfId.Value),
                    txtDesc.Text.Trim(), permisos, UsuarioActual.UsuarioDesc);

                MostrarMensaje(lblMensaje, "Lista guardada.", true);
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
            hfId.Value = txtDesc.Text = string.Empty;
            cblPermisos.ClearSelection();
            litTitulo.Text = "Nueva lista";
        }

        private void CargarGrid()
        {
            gvListas.DataSource = PermisoListaRepositorio.Listar();
            gvListas.DataBind();
        }
    }
}
