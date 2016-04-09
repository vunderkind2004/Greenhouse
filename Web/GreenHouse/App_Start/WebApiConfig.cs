using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using GreenHouse.App_Start;
using Microsoft.Practices.Unity;
using Unity.WebApi;

namespace GreenHouse
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var container = UnityConfig.GetConfiguredContainer();
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);

        }
    }
}
