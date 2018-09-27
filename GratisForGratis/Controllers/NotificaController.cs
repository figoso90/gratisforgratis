using GratisForGratis.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace GratisForGratis.Controllers
{
    public class NotificaController : Controller
    {

        [HttpGet]
        public ActionResult Elenco(int pagina = 1)
        {
            PersonaModel utente = base.Session["utente"] as PersonaModel;
            List<UtenteNotificaViewModel> listModel = new List<UtenteNotificaViewModel>();
            using (DatabaseContext db = new DatabaseContext())
            {
                db.Database.Connection.Open();
                IQueryable<NOTIFICA> listaNotifiche = db.NOTIFICA.Include(m => m.ANNUNCIO_NOTIFICA)
                    .Where(m => m.ID_PERSONA_DESTINATARIO == utente.Persona.ID);
                int numeroElementi = System.Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["numeroNotifiche"]);
                int numNotificheTotali = listaNotifiche.Count();

                listaNotifiche = listaNotifiche.OrderByDescending(m => m.DATA_INSERIMENTO)
                    .Skip((pagina - 1) * numeroElementi)
                    .Take(numeroElementi);

                listaNotifiche.ToList().ForEach(m => {
                    UtenteNotificaViewModel utenteNotifica = new UtenteNotificaViewModel();
                    utenteNotifica.getTipoNotifica(db, m);
                    listModel.Add(utenteNotifica);
                });

                ViewData["TotalePagine"] = (int)System.Math.Ceiling((decimal)numNotificheTotali / (decimal)numeroElementi);
                if (pagina == 0)
                    pagina = 1;
                ViewData["Pagina"] = pagina;
            }
            return View(listModel);
        }

        [HttpGet]
        public ActionResult Index(int id)
        {
            UtenteNotificaViewModel utenteNotifica = new UtenteNotificaViewModel();
            using (DatabaseContext db = new DatabaseContext())
            {
                int idUtente = (Session["utente"] as PersonaModel).Persona.ID;
                NOTIFICA model = db.NOTIFICA.Include(m => m.ANNUNCIO_NOTIFICA)
                    .SingleOrDefault(m => m.ID == id && m.ID_PERSONA_DESTINATARIO == idUtente);
                if (model == null)
                    RedirectToAction("", "Home");
                
                utenteNotifica.getTipoNotifica(db, model);
            }
            return View(utenteNotifica);
        }

        #region AJAX
        [HttpDelete]
        public JsonResult DeleteNews(int id)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                db.Database.Connection.Open();
                PersonaModel utente = (Session["utente"] as PersonaModel);
                NOTIFICA notifica = db.NOTIFICA.SingleOrDefault(m => m.ID == id 
                    && m.ID_PERSONA == utente.Persona.ID);

                if (notifica != null)
                {
                    if (notifica.ANNUNCIO_NOTIFICA != null && notifica.ANNUNCIO_NOTIFICA.Count > 0)
                    {
                        db.ANNUNCIO_NOTIFICA.RemoveRange(notifica.ANNUNCIO_NOTIFICA);
                    }
                    db.NOTIFICA.Remove(notifica);
                    if (db.SaveChanges() > 0)
                    {
                        return Json(true);
                    }
                }
            }
            return Json(false);
        }
        #endregion
    }
}