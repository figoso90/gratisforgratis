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

            routes.MapRoute(
                name: "Gratis",
                url: "gratis/{Cerca_Categoria}/{Cerca_IDCategoria}",
                defaults: new { controller = "Cerca", action = "Index", Cerca_Categoria = "", Cerca_IDCategoria = "" }
            );

            routes.MapRoute(
                name: "Categoria",
                url: "categoria/{Cerca_Categoria}/{Cerca_IDCategoria}",
                defaults: new { controller = "Cerca", action = "Index", Cerca_Categoria = "", Cerca_IDCategoria = "" }
            );

            routes.MapRoute(
                name: "Servizio",
                url: "servizio/{nomeAnnuncio}/{token}",
                defaults: new { controller = "Annuncio", action = "Index", nomeAnnuncio = "", token = "" }
            );

            routes.MapRoute(
                name: "Oggetto",
                url: "oggetto/{nomeAnnuncio}/{token}",
                defaults: new { controller = "Annuncio", action = "Index", nomeAnnuncio = "", token = "" }
            );

            routes.MapRoute(
                name: "Profilo",
                url: "profilo/{id}",
                defaults: new { controller = "Utente", action = "Profilo" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Home",
                url: "{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
