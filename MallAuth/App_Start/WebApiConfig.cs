using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MallAuth
{   
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            // Web API routes// Attribute routing.
            config.MapHttpAttributeRoutes();

            // Convention-based routing.ignore when 'Attribute routing' explicitly specific
            config.Routes.MapHttpRoute(
                name: "ResourceApi",
                routeTemplate: "api/rsc/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
              name: "ActionApi",
              routeTemplate: "api/{controller}/{action}/{id}",
              defaults: new { id = RouteParameter.Optional }
            );
            
            //config.Filters.Add(new Thinktecture.IdentityModel.Authorization.WebApi.ClaimsAuthorizeAttribute());

            var jsonFormatter = config.Formatters.OfType<System.Net.Http.Formatting.JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        }
    }
     
}


 
