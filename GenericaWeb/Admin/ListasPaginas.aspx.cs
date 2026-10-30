using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;
using GenericaWeb.Datos;
using GenericaWeb.Seguridad;

namespace GenericaWeb.Admin
{
    public partial class ListasPaginas : PaginaSegura
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            Combos.Paginas(cblPaginas, false);
            pnlEdicion.Visible = PuedeEditar;
            CargarGrid();
        }

        protected void gvListas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var id = short.Parse((string)e.CommandArgument);
            if (e.CommandName == "Editar")
            {
                var fila = PaginaListaRepositorio.Obtener(id);
                if (fila == null)
                    return;

                hfId.Value = id.ToString();
                txtDesc.Text = (string)fila["PaginaListaDesc"];
                txtComentario.Text = (string)fila["PaginaListaComentario"];
                var elementos = PaginaListaRepositorio.Elementos(id);
                foreach (ListItem item in cblPaginas.Items)
                    item.Selected = elementos.Contains(short.Parse(item.Value));
                litTitulo.Text = "Editar lista";
            }
            else if (e.CommandName == "Eliminar" && ExigirEdicion(lblMensaje))
            {
                try
                {
                    if (PaginaListaRepositorio.Eliminar(id))
                    {
                        MostrarMensaje(lblMensaje, "Lista eliminada.", true);
                        Limpiar();
                        CargarGrid();
                    }
                    else
                    {
                        MostrarMensaje(lblMensaje, "La lista está asignada a uno o más usuarios.", false);
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

            var comentario = txtComentario.Text.Trim();
            if (comentario.Length > 500)
                comentario = comentario.Substring(0, 500);

            var paginas = cblPaginas.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => short.Parse(i.Value));
            try
            {
                PaginaListaRepositorio.Guardar(
                    string.IsNullOrEmpty(hfId.Value) ? (short)0 : short.Parse(hfId.Value),
                    txtDesc.Text.Trim(), comentario, paginas, UsuarioActual.UsuarioDesc);

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
            hfId.Value = txtDesc.Text = txtComentario.Text = string.Empty;
            cblPaginas.ClearSelection();
            litTitulo.Text = "Nueva lista";
        }

        private void CargarGrid()
        {
            gvListas.DataSource = PaginaListaRepositorio.Listar();
            gvListas.DataBind();
        }
    }
}
