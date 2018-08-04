using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GratisForGratis
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.IgnoreRoute("elmah.axd");

            /*
            routes.RouteExistingFiles = true;

            routes.MapRoute(
                name: "CrazyPants",
                url: "{page}.html",
                defaults: new { controller = "Home", action = "Html", page = UrlParameter.Optional }
            );
            */
            routes.MapRoute(
                name: "DefaultOggetti",
                url: "Oggetti/{nomeCategoria}/{sottocategoria}/{id}",
                defaults: new { controller = "Cerca", action = "Oggetti", nomeCategoria = "Tutti", sottocategoria = "", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "DefaultServizi",
                url: "Servizi/{nomeCategoria}/{sottocategoria}/{id}",
                defaults: new { controller = "Cerca", action = "Servizi", nomeCategoria = "Tutti", sottocategoria = "", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Oggetto",
                url: "Oggetto/{nomeAnnuncio}/{id}",
                defaults: new { controller = "Annuncio", action = "", nomeAnnuncio = UrlParameter.Optional, id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Servizio",
                url: "Servizio/{nomeAnnuncio}/{id}",
                defaults: new { controller = "Annuncio", action = "", nomeAnnuncio = UrlParameter.Optional, id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
