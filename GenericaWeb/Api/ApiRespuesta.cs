using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GenericaWeb.Api
{
    internal static class ApiRespuesta
    {
        private static readonly JsonSerializerSettings Opciones = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        };

        public static void Json(HttpContext contexto, int estado, object cuerpo)
        {
            contexto.Response.StatusCode = estado;
            contexto.Response.TrySkipIisCustomErrors = true;
            contexto.Response.ContentType = "application/json";
            contexto.Response.ContentEncoding = System.Text.Encoding.UTF8;
            contexto.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            contexto.Response.Write(JsonConvert.SerializeObject(cuerpo, Opciones));
        }
    }
}
