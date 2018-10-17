using Elmah;
using GratisForGratis.Controllers;
using GratisForGratis.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace GratisForGratis.Filters
{
    public class HandleExceptionsAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            HttpRequestBase richiesta = filterContext.RequestContext.HttpContext.Request;

            if (typeof(System.Data.SqlClient.SqlException).IsAssignableFrom(filterContext.GetType()) || 
                typeof(System.Data.DataException).IsAssignableFrom(filterContext.GetType()))
            {
                // errore di connessione al database, scrivo il log sul file
                Elmah.ErrorSignal.FromCurrentContext().Raise(filterContext.Exception);
            }
            else
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    db.Database.Connection.Open();
                    LOG_ERRORE logErrore = new LOG_ERRORE();
                    logErrore.ACTION = filterContext.RouteData.Values["action"].ToString();
                    logErrore.CONTROLLER = filterContext.RouteData.Values["controller"].ToString();
                    logErrore.PARAMETRI = System.Web.Helpers.Json.Encode(filterContext.HttpContext.Request.QueryString);
                    logErrore.IP = richiesta.UserHostAddress;
                    logErrore.MAC_ADDRESS = "";
                    logErrore.SESSIONE = filterContext.RequestContext.HttpContext.Session.SessionID;
                    logErrore.RICHIESTA = richiesta.ToString();
                    logErrore.RISPOSTA = filterContext.HttpContext.Response.ToString();
                    logErrore.ALIAS = richiesta.UserHostName;
                    if (richiesta.IsAuthenticated)
                    {
                        AdvancedController controller = new AdvancedController();
                        PersonaModel utente = filterContext.RequestContext.HttpContext.Session["utente"] as PersonaModel;
                        int bonus = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["bonusMessaggioErrore"]);
                        Guid portale = Guid.Parse(System.Configuration.ConfigurationManager.AppSettings["portaleweb"]);
                        logErrore.ID_PERSONA = utente.Persona.ID;
                        controller.AddBonus(db, utente.Persona, portale, bonus, TipoTransazione.BonusSegnalazioneErrore, App_GlobalResources.Bonus.MessageError);
                    }
                    db.LOG_ERRORE.Add(logErrore);
                    db.SaveChanges();
                }
            }
            base.OnException(filterContext);
        }
    }
}
 