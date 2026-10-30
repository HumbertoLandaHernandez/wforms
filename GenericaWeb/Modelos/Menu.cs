using System.Collections.Generic;

namespace GenericaWeb.Modelos
{
    public class MenuGrupo
    {
        public string Titulo { get; set; }
        public List<MenuElemento> Paginas { get; } = new List<MenuElemento>();
    }

    public class MenuElemento
    {
        public string Ruta { get; set; }
        public string Titulo { get; set; }
    }
}
