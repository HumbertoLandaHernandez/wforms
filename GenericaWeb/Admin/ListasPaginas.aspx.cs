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
            var id = byte.Parse((string)e.CommandArgument);
            if (e.CommandName == "Editar")
            {
                var fila = ListaPaginaRepositorio.Obtener(id);
                if (fila == null)
                    return;

                hfId.Value = id.ToString();
                txtDesc.Text = (string)fila["ListaPaginaDesc"];
                txtComentario.Text = (string)fila["ListaPaginaComentario"];
                var elementos = ListaPaginaRepositorio.Elementos(id);
                foreach (ListItem item in cblPaginas.Items)
                    item.Selected = elementos.Contains(short.Parse(item.Value));
                litTitulo.Text = "Editar grupo";
            }
            else if (e.CommandName == "Eliminar" && ExigirEdicion(lblMensaje))
            {
                try
                {
                    ListaPaginaRepositorio.Eliminar(id);
                    MostrarMensaje(lblMensaje, "Grupo eliminado.", true);
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

            var comentario = txtComentario.Text.Trim();
            if (comentario.Length > 500)
                comentario = comentario.Substring(0, 500);

            var paginas = cblPaginas.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => short.Parse(i.Value));
            try
            {
                ListaPaginaRepositorio.Guardar(
                    string.IsNullOrEmpty(hfId.Value) ? (byte)0 : byte.Parse(hfId.Value),
                    txtDesc.Text.Trim(), comentario, paginas, UsuarioActual.UsuarioDesc);

                MostrarMensaje(lblMensaje, "Grupo guardado.", true);
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
            litTitulo.Text = "Nuevo grupo";
        }

        private void CargarGrid()
        {
            gvListas.DataSource = ListaPaginaRepositorio.Listar();
            gvListas.DataBind();
        }
    }
}
