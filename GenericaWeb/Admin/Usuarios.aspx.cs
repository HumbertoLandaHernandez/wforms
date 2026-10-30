using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using GenericaWeb.Datos;
using GenericaWeb.Modelos;
using GenericaWeb.Seguridad;

namespace GenericaWeb.Admin
{
    public partial class Usuarios : PaginaSegura
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            Combos.NombresPersonales(ddlNombre);
            Combos.Roles(ddlRol);
            Combos.Permisos(ddlPermiso);
            Combos.ListasPermisos(ddlPermisosPaginas, true);
            Combos.Paginas(ddlPaginaHome, true);
            pnlEdicion.Visible = PuedeEditar;
            CargarGrid();
        }

        protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var id = short.Parse((string)e.CommandArgument);
            if (e.CommandName == "Editar")
            {
                var fila = UsuarioRepositorio.Obtener(id);
                if (fila == null)
                    return;

                hfId.Value = id.ToString();
                txtUsuario.Text = (string)fila["UsuarioDesc"];
                txtContrasena.Text = string.Empty;
                Combos.Seleccionar(ddlNombre, fila["UsuarioNombreId"]);
                Combos.Seleccionar(ddlRol, fila["UsuarioRolId"]);
                Combos.Seleccionar(ddlPermiso, fila["UsuarioPermisosId"]);
                Combos.Seleccionar(ddlPermisosPaginas, fila["UsuarioPermisosPaginasId"]);
                Combos.Seleccionar(ddlPaginaHome, fila["UsuarioPaginaHomeId"]);
                litTitulo.Text = "Editar usuario";
                litAyudaContrasena.Text = "(en blanco = no cambiar)";
            }
            else if (e.CommandName == "Eliminar" && ExigirEdicion(lblMensaje))
            {
                if (id == UsuarioActual.UsuarioId)
                {
                    MostrarMensaje(lblMensaje, "No puede eliminar su propio usuario.", false);
                    return;
                }
                try
                {
                    UsuarioRepositorio.Eliminar(id);
                    MostrarMensaje(lblMensaje, "Usuario eliminado.", true);
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

            var datos = new UsuarioDatos
            {
                UsuarioId = string.IsNullOrEmpty(hfId.Value) ? (short)0 : short.Parse(hfId.Value),
                UsuarioDesc = txtUsuario.Text.Trim(),
                NombreId = int.Parse(ddlNombre.SelectedValue),
                RolId = byte.Parse(ddlRol.SelectedValue),
                PermisosId = short.Parse(ddlPermiso.SelectedValue),
                PermisosPaginasId = int.Parse(ddlPermisosPaginas.SelectedValue),
                PaginaHomeId = short.Parse(ddlPaginaHome.SelectedValue)
            };

            string hash = null;
            if (txtContrasena.Text.Length > 0)
            {
                if (txtContrasena.Text.Length < HashContrasena.LongitudMinima)
                {
                    MostrarMensaje(lblMensaje, "La contraseña debe tener al menos " + HashContrasena.LongitudMinima + " caracteres.", false);
                    return;
                }
                hash = HashContrasena.Generar(txtContrasena.Text);
            }
            else if (datos.UsuarioId == 0)
            {
                MostrarMensaje(lblMensaje, "La contraseña es obligatoria para un usuario nuevo.", false);
                return;
            }

            try
            {
                if (datos.UsuarioId == 0)
                    UsuarioRepositorio.Insertar(datos, hash, UsuarioActual.UsuarioDesc);
                else
                    UsuarioRepositorio.Actualizar(datos, hash, UsuarioActual.UsuarioDesc);

                MostrarMensaje(lblMensaje, "Usuario guardado.", true);
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
            txtUsuario.Text = string.Empty;
            txtContrasena.Text = string.Empty;
            ddlNombre.SelectedIndex = ddlRol.SelectedIndex = ddlPermiso.SelectedIndex = 0;
            ddlPermisosPaginas.SelectedIndex = ddlPaginaHome.SelectedIndex = 0;
            litTitulo.Text = "Nuevo usuario";
            litAyudaContrasena.Text = "(mínimo 8 caracteres)";
        }

        private void CargarGrid()
        {
            gvUsuarios.DataSource = UsuarioRepositorio.Listar();
            gvUsuarios.DataBind();
        }
    }
}
