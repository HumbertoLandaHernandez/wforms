using System.Data;
using System.Web.UI.WebControls;

namespace GenericaWeb.Datos
{
    /// <summary>Carga de DropDownList / CheckBoxList desde catálogos.</summary>
    public static class Combos
    {
        public static void Roles(ListControl lista) =>
            Llenar(lista, Db.Consultar("SELECT RolId AS Id, RolDesc AS Texto FROM Sistema.Rol ORDER BY RolDesc"), true);

        public static void NombresPersonales(ListControl lista) =>
            Llenar(lista, Db.Consultar("SELECT NombrePersonalId AS Id, NombrePersonalDesc AS Texto FROM Sistema.NombrePersonal ORDER BY NombrePersonalDesc"), true);

        public static void NombresTipo(ListControl lista) =>
            Llenar(lista, Db.Consultar("SELECT NombreTipoId AS Id, NombreTipoDesc AS Texto FROM Sistema.NombreTipo ORDER BY NombreTipoId"), true);

        public static void NombresBase(ListControl lista) =>
            Llenar(lista, Db.Consultar("SELECT NombreBaseId AS Id, NombreBaseDesc AS Texto FROM Sistema.NombreBase ORDER BY NombreBaseDesc"), true);

        public static void Permisos(ListControl lista) =>
            Llenar(lista, Db.Consultar("SELECT PermisoId AS Id, PermisoDesc AS Texto FROM Sistema.Permiso ORDER BY PermisoId"), true);

        /// <summary>Páginas; maxId limita a 255 para Rol.RolPaginaHomeId (tinyint).</summary>
        public static void Paginas(ListControl lista, bool conVacio, int maxId = short.MaxValue) =>
            Llenar(lista, Db.Consultar(@"SELECT PaginaId AS Id, Descripcion + ' (' + PaginaDesc + ')' AS Texto
                                         FROM Sistema.Pagina WHERE PaginaId <= @Max ORDER BY Descripcion", Db.P("@Max", maxId)), conVacio);

        public static void ListasPermisos(ListControl lista, bool incluirNinguna)
        {
            Llenar(lista, Db.Consultar(@"SELECT DISTINCT PermisoListaId AS Id, 'Lista ' + CAST(PermisoListaId AS varchar(11)) AS Texto
                                         FROM Sistema.PermisoLista ORDER BY PermisoListaId"), true);
            if (incluirNinguna)
                lista.Items.Insert(1, new ListItem("Ninguna (solo la del rol)", "0"));
        }

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
