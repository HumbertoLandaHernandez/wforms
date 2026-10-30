using GenericaWeb.Seguridad;

namespace GenericaWeb
{
    public partial class AccesoDenegado : PaginaSegura
    {
        protected override bool ValidarPermisoPagina => false;

        protected override void OnLoad(System.EventArgs e)
        {
            Response.StatusCode = 403;
            Response.TrySkipIisCustomErrors = true;
            base.OnLoad(e);
        }
    }
}
