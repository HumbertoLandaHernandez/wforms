using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using GenericaWeb.Datos;
using GenericaWeb.Seguridad;

namespace GenericaWeb.Admin
{
    public partial class Roles : PaginaSegura
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            // RolPaginaHomeId es tinyint: solo páginas con Id <= 255.
            Combos.Paginas(ddlPaginaHome, true, byte.MaxValue);
            Combos.ListasPermisos(ddlPermisos);
            pnlEdicion.Visible = PuedeEditar;
            CargarGrid();
        }

        protected void gvRoles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var id = byte.Parse((string)e.CommandArgument);
            if (e.CommandName == "Editar")
            {
                var fila = RolRepositorio.Obtener(id);
                if (fila == null)
                    return;

                hfId.Value = id.ToString();
                txtDesc.Text = (string)fila["RolDesc"];
                Combos.Seleccionar(ddlPaginaHome, fila["RolPaginaHomeId"]);
                Combos.Seleccionar(ddlPermisos, fila["RolPermisosId"]);
                txtClassId.Text = fila["RolClassId"].ToString();
                litTitulo.Text = "Editar rol";
            }
            else if (e.CommandName == "Eliminar" && ExigirEdicion(lblMensaje))
            {
                try
                {
                    RolRepositorio.Eliminar(id);
                    MostrarMensaje(lblMensaje, "Rol eliminado.", true);
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
                RolRepositorio.Guardar(
                    string.IsNullOrEmpty(hfId.Value) ? (byte)0 : byte.Parse(hfId.Value),
                    txtDesc.Text.Trim(),
                    byte.Parse(ddlPaginaHome.SelectedValue),
                    int.Parse(ddlPermisos.SelectedValue),
                    byte.Parse(txtClassId.Text),
                    UsuarioActual.UsuarioDesc);

                MostrarMensaje(lblMensaje, "Rol guardado.", true);
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
            hfId.Value = string.Empty;
            txtDesc.Text = string.Empty;
            txtClassId.Text = "0";
            ddlPaginaHome.SelectedIndex = ddlPermisos.SelectedIndex = 0;
            litTitulo.Text = "Nuevo rol";
        }

        private void CargarGrid()
        {
            gvRoles.DataSource = RolRepositorio.Listar();
            gvRoles.DataBind();
        }
    }
}
