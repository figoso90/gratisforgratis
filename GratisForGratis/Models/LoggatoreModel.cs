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

                    if (HttpContext.Current.Session["portaleweb"] != null)
                        model.ID_ATTIVITA = Convert.ToInt32(((PortaleWebViewModel)HttpContext.Current.Session["portaleweb"]).Id);
                    if (HttpContext.Current.Session["utente"] != null)
                        model.ID_PERSONA = ((PersonaModel)HttpContext.Current.Session["utente"]).Persona.ID;
                    model.CONTROLLER = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
                    model.ACTION = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
                    model.IP = HttpContext.Current.Request.UserHostAddress;
                    model.PARAMETRI = String.Join(",", HttpContext.Current.Request.Params);
                    model.RICHIESTA = HttpContext.Current.Request.Headers.ToString();
                    model.RISPOSTA = string.IsNullOrWhiteSpace(ex.StackTrace)? ex.Message : ex.StackTrace;
                    model.SESSIONE = HttpContext.Current.Session.SessionID;
                    model.DATA_INSERIMENTO = DateTime.Now;
                    db.LOG_ERRORE.Add(model);
                    db.SaveChanges();
                }
            }
            catch (Exception eccezionedb)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                Elmah.ErrorSignal.FromCurrentContext().Raise(eccezionedb);
            }
        }
        #endregion
    }
}