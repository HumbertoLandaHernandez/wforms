using System;
using System.Web;
using GenericaWeb.Seguridad;

namespace GenericaWeb
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // Falla al iniciar si la clave JWT no está configurada correctamente.
            JwtServicio.ValidarConfiguracion();
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
        }
    }
}
