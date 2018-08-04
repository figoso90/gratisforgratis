using GratisForGratis.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace GratisForGratis.Controllers
{
    public class NotificaController : Controller
    {
        // GET: Notifica
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
                ANNUNCIO_NOTIFICA annuncioNotifica = db.ANNUNCIO_NOTIFICA.SingleOrDefault(m => m.ID_NOTIFICA == id && m.NOTIFICA.ID_PERSONA == (Session["utente"] as PersonaModel).Persona.ID);

                if (annuncioNotifica != null)
                {
                    NOTIFICA notifica = annuncioNotifica.NOTIFICA;
                    db.ANNUNCIO_NOTIFICA.Remove(annuncioNotifica);
                    db.NOTIFICA.Remove(notifica);
                }
            }
            return Json(true);
        }
        #endregion
    }
}