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
                name: "Categoria",
                url: "categoria/{Cerca_Categoria}/{Cerca_IDCategoria}",
                defaults: new { controller = "Cerca", action = "Index", Cerca_Categoria = UrlParameter.Optional, Cerca_IDCategoria = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Servizio",
                url: "servizio/{nomeAnnuncio}/{token}/{azione}",
                defaults: new { controller = "Annuncio", action = "Index", nomeAnnuncio = UrlParameter.Optional, token = UrlParameter.Optional, azione = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Oggetto",
                url: "oggetto/{nomeAnnuncio}/{token}/{azione}",
                defaults: new { controller = "Annuncio", action = "Index", nomeAnnuncio = UrlParameter.Optional, token = UrlParameter.Optional, azione = UrlParameter.Optional }
            );

            //routes.MapRoute(
            //    name: "Annuncio",
            //    url: "annuncio/{nomeAnnuncio}/{token}/{azione}",
            //    defaults: new { controller = "Annuncio", action = "Index", nomeAnnuncio = UrlParameter.Optional, token = UrlParameter.Optional, azione = UrlParameter.Optional }
            //);

            routes.MapRoute(
                name: "AnnuncioBaratto",
                url: "baratto/{nomeAnnuncio}/{token}/{azione}",
                defaults: new { controller = "Annuncio", action = "Index", nomeAnnuncio = UrlParameter.Optional, token = UrlParameter.Optional, azione = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Profilo",
                url: "profilo/{nome}/{id}",
                defaults: new { controller = "Utente", action = "Happy", nome = "", id="" }
            );

            routes.MapRoute(
                name: "Negozio",
                url: "negozio/{nome}/{id}",
                defaults: new { controller = "PortaleWeb", action = "Profilo", nome = "" }
            );

            routes.MapRoute(
                name: "Team",
                url: "team",
                defaults: new { controller = "Home", action = "Team", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Bando",
                url: "bando",
                defaults: new { controller = "Home", action = "Bando", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Contatti",
                url: "contatti",
                defaults: new { controller = "Home", action = "Contatti", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "HappyCoin",
                url: "happy-coin",
                defaults: new { controller = "Home", action = "MonetaVirtuale", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "BarattoAsincrono",
                url: "barattoasincrono",
                defaults: new { controller = "Home", action = "BarattoAsincrono", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "BarattoMultilaterale",
                url: "barattomultilaterale",
                defaults: new { controller = "Home", action = "BarattoMultilaterale", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Regalo",
                url: "regalo",
                defaults: new { controller = "Home", action = "Regalo", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Criptovaluta",
                url: "criptovaluta",
                defaults: new { controller = "Home", action = "Criptovaluta", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Missione",
                url: "missione",
                defaults: new { controller = "Home", action = "Missione", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Bitcoin",
                url: "bitcoin",
                defaults: new { controller = "Home", action = "Bitcoin", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Login",
                url: "login",
                defaults: new { controller = "Utente", action = "Login", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Registrazione",
                url: "registrazione",
                defaults: new { controller = "Utente", action = "Registrazione", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
