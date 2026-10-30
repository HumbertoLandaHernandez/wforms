using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using GenericaWeb.Datos;
using GenericaWeb.Seguridad;

namespace GenericaWeb.Admin
{
    public partial class NombresLista : PaginaSegura
    {
        private int IdActual => string.IsNullOrEmpty(hfId.Value) ? 0 : int.Parse(hfId.Value);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            Combos.NombresTipo(ddlTipo);
            Combos.Nombres(ddlNombre);
            btnGuardar.Visible = phAgregar.Visible = PuedeEditar;
            CargarGrid();
        }

        protected void gvListas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var id = int.Parse((string)e.CommandArgument);
            if (e.CommandName == "Editar")
            {
                var desc = NombreListaRepositorio.ObtenerDescripcion(id);
                if (desc == null)
                    return;

                hfId.Value = id.ToString();
                txtDesc.Text = desc;
                litTitulo.Text = "Editar nombre completo";
                MostrarElementos();
            }
            else if (e.CommandName == "Eliminar" && ExigirEdicion(lblMensaje))
            {
                try
                {
                    NombreListaRepositorio.Eliminar(id);
                    MostrarMensaje(lblMensaje, "Nombre eliminado.", true);
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
                var esNuevo = IdActual == 0;
                hfId.Value = NombreListaRepositorio.Guardar(IdActual, txtDesc.Text.Trim(), UsuarioActual.UsuarioDesc).ToString();
                MostrarMensaje(lblMensaje, esNuevo ? "Nombre creado. Agregue sus elementos." : "Nombre guardado.", true);
                litTitulo.Text = "Editar nombre completo";
                MostrarElementos();
                CargarGrid();
            }
            catch (SqlException ex)
            {
                MostrarMensaje(lblMensaje, Db.MensajeError(ex), false);
            }
        }

        protected void btnComponer_Click(object sender, EventArgs e)
        {
            if (IdActual == 0)
                return;

            var texto = NombreListaRepositorio.ComponerDescripcion(IdActual);
            if (texto.Length > 0)
                txtDesc.Text = texto;
            MostrarMensaje(lblMensaje, "Revise la descripción y presione Guardar.", true);
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!ExigirEdicion(lblMensaje) || !Page.IsValid || IdActual == 0)
                return;

            try
            {
                NombreListaRepositorio.AgregarElemento(IdActual, ddlTipo.SelectedValue, int.Parse(ddlNombre.SelectedValue), UsuarioActual.UsuarioDesc);
                ddlTipo.SelectedIndex = ddlNombre.SelectedIndex = 0;
                MostrarElementos();
                CargarGrid();
            }
            catch (SqlException ex)
            {
                MostrarMensaje(lblMensaje, Db.MensajeError(ex), false);
            }
        }

        protected void gvElementos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Quitar" || !ExigirEdicion(lblMensaje) || IdActual == 0)
                return;

            // El NombreTipoId puede contener '|': el NombreId (entero) va después del último separador.
            var argumento = (string)e.CommandArgument;
            var separador = argumento.LastIndexOf('|');
            try
            {
                NombreListaRepositorio.EliminarElemento(IdActual, argumento.Substring(0, separador), int.Parse(argumento.Substring(separador + 1)));
                MostrarElementos();
                CargarGrid();
            }
            catch (SqlException ex)
            {
                MostrarMensaje(lblMensaje, Db.MensajeError(ex), false);
            }
        }

        private void MostrarElementos()
        {
            pnlElementos.Visible = true;
            btnComponer.Visible = PuedeEditar;
            gvElementos.DataSource = NombreListaRepositorio.Elementos(IdActual);
            gvElementos.DataBind();
        }

        private void Limpiar()
        {
            hfId.Value = txtDesc.Text = string.Empty;
            litTitulo.Text = "Nuevo nombre completo";
            pnlElementos.Visible = btnComponer.Visible = false;
        }

        private void CargarGrid()
        {
            gvListas.DataSource = NombreListaRepositorio.Listar();
            gvListas.DataBind();
        }
    }
}
