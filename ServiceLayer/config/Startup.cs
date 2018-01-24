using Newtonsoft.Json;
using Owin;
using System.Linq;
using System.Web.Http;

namespace Organisation.ServiceLayer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.Routes.MapHttpRoute(name: "UserV10Api", routeTemplate: "api/user", defaults: new { controller = "UserV10" });
            config.Routes.MapHttpRoute(name: "OrgUnitV10Api", routeTemplate: "api/orgunit", defaults: new { controller = "OrgUnitV10" });
            config.Routes.MapHttpRoute(name: "UserV11Api", routeTemplate: "api/v1_1/user", defaults: new { controller = "UserV11" });
            config.Routes.MapHttpRoute(name: "OrgUnitV11Api", routeTemplate: "api/v1_1/orgunit", defaults: new { controller = "OrgUnitV11" });

            // use JSON as default serializer
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings();
            config.Formatters.JsonFormatter.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            app.UseWebApi(config);
        }
    }
}
