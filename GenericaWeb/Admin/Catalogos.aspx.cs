using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using GenericaWeb.Datos;
using GenericaWeb.Seguridad;

namespace GenericaWeb.Admin
{
    /// <summary>Mantenimiento de Nombre, NombreTipo y Permiso (?t=Clave).</summary>
    public partial class Catalogos : PaginaSegura
    {
        protected CatalogoDefinicion Definicion { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Definicion = CatalogoRepositorio.Buscar(Request.QueryString["t"]);
            Title = Definicion.Titulo;
            txtDesc.MaxLength = Definicion.LongitudDesc;
            txtId.MaxLength = Definicion.LongitudId;

            rptCatalogos.DataSource = CatalogoRepositorio.Catalogos;
            rptCatalogos.DataBind();

            if (IsPostBack)
                return;

            pnlEdicion.Visible = PuedeEditar;
            Limpiar();
            CargarGrid();
        }

        protected void gvCatalogo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var id = (string)e.CommandArgument;
            if (e.CommandName == "Editar")
            {
                var desc = CatalogoRepositorio.ObtenerDescripcion(Definicion, id);
                if (desc == null)
                    return;

                hfId.Value = id;
                txtId.Text = id;
                txtId.Enabled = false;   // la clave la referencian otras tablas
                txtDesc.Text = desc;
                litTitulo.Text = "Editar registro";
            }
            else if (e.CommandName == "Eliminar" && ExigirEdicion(lblMensaje))
            {
                try
                {
                    CatalogoRepositorio.Eliminar(Definicion, id);
                    MostrarMensaje(lblMensaje, "Registro eliminado.", true);
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

            try
            {
                CatalogoRepositorio.Guardar(Definicion, hfId.Value, txtId.Text.Trim(), txtDesc.Text.Trim(), UsuarioActual.UsuarioDesc);
                MostrarMensaje(lblMensaje, "Registro guardado.", true);
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
            hfId.Value = txtId.Text = txtDesc.Text = string.Empty;
            txtId.Enabled = true;
            phClave.Visible = rfvId.Enabled = Definicion.IdEsTexto;
            litTitulo.Text = "Nuevo registro";
        }

        private void CargarGrid()
        {
            gvCatalogo.DataSource = CatalogoRepositorio.Listar(Definicion);
            gvCatalogo.DataBind();
        }
    }
}
