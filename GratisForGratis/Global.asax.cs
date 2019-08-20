using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using GratisForGratis.Filters;
using GratisForGratis.Models;
using System.Linq;
using System;
using System.Web.Security;
using System.Web;
using GratisForGratis.Controllers;
using System.Data.Entity;
using System.Web.Helpers;
using System.Globalization;

namespace GratisForGratis
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            CultureInfo cultureInfoHappyCoin = new CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.Name);
            var numberFormatHappyCoin = cultureInfoHappyCoin.NumberFormat;
            numberFormatHappyCoin.CurrencySymbol = "H";
            Application["numberFormatHappyCoin"] = numberFormatHappyCoin;

            using (DatabaseContext db = new DatabaseContext())
            {
                int idNazioneCorriere = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["nazioneCorriere"]);
                Application["serviziSpedizione"] = db.CORRIERE_SERVIZIO.Include(m => m.CORRIERE)
                    .Where(m => m.STATO == (int)Stato.ATTIVO).ToList();
                Application["tipoSpedizione"] = db.TIPO_SPEDIZIONE.Where(m => m.STATO == (int)Stato.ATTIVO).ToList();
                Application["tipoValuta"] = db.TIPO_VALUTA.Where(m => m.STATO == (int)Stato.ATTIVO).ToList();
                Application["categorie"] = db.FINDSOTTOCATEGORIE("Tutti", new int?(0)).ToList<FINDSOTTOCATEGORIE_Result>();
            }
            //add the authentication filter for all controllers
            //GlobalFilters.Filters.Add(new AuthorizeAttribute());
            //GlobalFilters.Filters.Add(new LocalizationAttribute("it-IT"));

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalFilters.Filters.Add(new ActionFilter());
            GlobalFilters.Filters.Add(new HandleExceptionsAttribute());
        }
        /*
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute(), 2); //by default added
            
            filters.Add(new HandleExceptionsAttribute());
        }
        */
        protected void Session_Start(object sender, EventArgs e)
        {
            HttpRequest richiesta = HttpContext.Current.Request;
            HttpResponse risposta = HttpContext.Current.Response;

            HttpCookie ricerca = richiesta.Cookies.Get("ricerca");
            if (ricerca == null)
            {
                ricerca = new HttpCookie("ricerca");
                ricerca["IDCategoria"] = "1";
                ricerca["Categoria"] = "Tutti";
            }
            HttpCookie filtro = richiesta.Cookies.Get("filtro");
            if (filtro == null)
                filtro = new HttpCookie("filtro");
            risposta.SetCookie(ricerca);
            risposta.SetCookie(filtro);

            if ((!FormsAuthentication.CookiesSupported ? false : richiesta.Cookies[FormsAuthentication.FormsCookieName] != null))
            {
                Guid name = Guid.Parse(FormsAuthentication.Decrypt(richiesta.Cookies[FormsAuthentication.FormsCookieName].Value).Name);
                using (DatabaseContext db = new DatabaseContext())
                {
                    PERSONA utente = db.PERSONA.SingleOrDefault<PERSONA>((PERSONA u) => u.CONTO_CORRENTE.TOKEN == name);
                    if (utente != null)
                    {
                        (new AdvancedController()).setSessioneUtente(new HttpSessionStateWrapper(HttpContext.Current.Session), db, utente.ID, true, new ControllerContext());
                    }
                }
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // gestione totalmente imprevisto
        }
    }
}
