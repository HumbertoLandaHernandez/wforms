using System.Data;
using System.Web.UI.WebControls;

namespace GenericaWeb.Datos
{
    /// <summary>Carga de DropDownList / CheckBoxList desde catálogos.</summary>
    public static class Combos
    {
        public static void Roles(ListControl lista, bool conVacio = true) =>
            Llenar(lista, Db.Consultar("SELECT RolId AS Id, RolDesc AS Texto FROM Sistema.Rol ORDER BY RolDesc"), conVacio);

        public static void NombresCompletos(ListControl lista) =>
            Llenar(lista, Db.Consultar("SELECT NombreListaId AS Id, NombreListaDesc AS Texto FROM Sistema.NombreLista ORDER BY NombreListaDesc"), true);

        public static void NombresTipo(ListControl lista) =>
            Llenar(lista, Db.Consultar("SELECT NombreTipoId AS Id, NombreTipoDesc AS Texto FROM Sistema.NombreTipo ORDER BY NombreTipoDesc"), true);

        public static void Nombres(ListControl lista) =>
            Llenar(lista, Db.Consultar("SELECT NombreId AS Id, NombreDesc AS Texto FROM Sistema.Nombre ORDER BY NombreDesc"), true);

        public static void Permisos(ListControl lista, bool conVacio = true) =>
            Llenar(lista, Db.Consultar("SELECT PermisoId AS Id, PermisoDesc AS Texto FROM Sistema.Permiso ORDER BY PermisoDesc"), conVacio);

        public static void ListasPermisos(ListControl lista) =>
            Llenar(lista, Db.Consultar("SELECT PermisoListaId AS Id, PermisoListaDesc AS Texto FROM Sistema.PermisoLista ORDER BY PermisoListaDesc"), true);

        public static void ListasPaginas(ListControl lista) =>
            Llenar(lista, Db.Consultar("SELECT PaginaListaId AS Id, PaginaListaDesc AS Texto FROM Sistema.PaginaLista ORDER BY PaginaListaDesc"), true);

        /// <summary>Páginas; maxId limita a 255 para Rol.RolPaginaHomeId (tinyint).</summary>
        public static void Paginas(ListControl lista, bool conVacio, int maxId = short.MaxValue) =>
            Llenar(lista, Db.Consultar(@"SELECT PaginaId AS Id, Descripcion + ' (' + PaginaDesc + ')' AS Texto
                                         FROM Sistema.Pagina WHERE PaginaId <= @Max ORDER BY Descripcion", Db.P("@Max", maxId)), conVacio);

        /// <summary>Selecciona un valor; si no existe en la lista lo agrega para no perder el dato.</summary>
        public static void Seleccionar(ListControl lista, object valor)
        {
            var texto = valor?.ToString() ?? string.Empty;
            if (lista.Items.FindByValue(texto) == null)
                lista.Items.Add(new ListItem("Id " + texto + " (no encontrado)", texto));
            lista.SelectedValue = texto;
        }

        private static void Llenar(ListControl lista, DataTable datos, bool conVacio)
        {
            lista.Items.Clear();
            lista.DataSource = datos;
            lista.DataValueField = "Id";
            lista.DataTextField = "Texto";
            lista.DataBind();
            if (conVacio)
                lista.Items.Insert(0, new ListItem("-- Seleccione --", string.Empty));
        }
    }
}
