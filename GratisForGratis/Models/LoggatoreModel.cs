using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GratisForGratis.Models
{
    public static class LoggatoreModel
    {
        #region METODI PUBBLICI
        public static void Errore(Exception ex)
        {
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    var model = new LOG_ERRORE();

                    model.ID_ATTIVITA = Convert.ToInt32(((PortaleWebViewModel)HttpContext.Current.Session["portaleweb"]).Id);
                    model.ID_PERSONA = ((PersonaModel)HttpContext.Current.Session["utente"]).Persona.ID;
                    model.CONTROLLER = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
                    model.ACTION = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
                    model.IP = HttpContext.Current.Request.UserHostAddress;
                    model.PARAMETRI = String.Join(",", HttpContext.Current.Request.Params);
                    model.RICHIESTA = new System.IO.StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();
                    model.RISPOSTA = new System.IO.StreamReader(HttpContext.Current.Response.OutputStream).ReadToEnd();
                    model.SESSIONE = HttpContext.Current.Session.SessionID;
                    db.LOG_ERRORE.Add(model);
                    db.SaveChanges();
                }
            }
            catch
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
        #endregion
    }
}