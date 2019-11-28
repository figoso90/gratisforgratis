using GratisForGratis.App_GlobalResources;
using GratisForGratis.Filters;
using GratisForGratis.Models;
using GratisForGratis.Models.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace GratisForGratis.Controllers
{
    [Authorize]
    public class FeedbackController : AdvancedController
    {
        #region ACTION
        [HttpGet]
        public ActionResult Index(int id, TipoFeedback tipo)
        {
            string nomeView = "";
            // verificare come visualizzare soltanto
            // verificare come differenziare tra compratore e venditore
            FeedbackViewModel viewModel = new FeedbackViewModel();
            try
            {
                if (ModelState.IsValid)
                {
                    using (DatabaseContext db = new DatabaseContext())
                    {
                        //string acquistoDecodificato = Uri.UnescapeDataString(id);
                        //string acquistoPulito = acquistoDecodificato.Trim().Substring(3, acquistoDecodificato.Trim().Length - 6);
                        //int idAcquisto = Utility.DecodeToInt(acquistoPulito);
                        int idUtente = (Session["utente"] as PersonaModel).Persona.ID;
                        ANNUNCIO_FEEDBACK model = db.ANNUNCIO_FEEDBACK.Where(f => f.ID_ANNUNCIO == id && f.ID_VOTANTE == idUtente).SingleOrDefault();
                        if (model != null)
                        {
                            TempData["feedback"] = model;
                            return RedirectToAction("Inviato", new { id = model.ID });
                        }

                        // se è un nuovo voto, recupero i dati del pagamento
                        ANNUNCIO model2 = null;
                        if (tipo == TipoFeedback.Acquirente)
                        {
                            model2 = db.ANNUNCIO.Where(p => p.ID == id && p.ID_PERSONA != idUtente).SingleOrDefault();
                            viewModel.Nome = model2.NOME;
                            viewModel.AcquistoID = id;
                            viewModel.Tipo = tipo;
                        }
                        else if (tipo == TipoFeedback.Venditore)
                        {
                            model2 = db.ANNUNCIO.Where(p => p.ID == id && p.ID_PERSONA == idUtente).SingleOrDefault();
                            viewModel.Nome = model2.NOME;
                            viewModel.AcquistoID = id;
                            viewModel.Tipo = tipo;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                LoggatoreModel.Errore(exception);
                // se ha un errore generico o semplicemente sta cercando di fare un feedback
                return Redirect(System.Web.Security.FormsAuthentication.DefaultUrl);
            }
            return View(nomeView, viewModel);
        }

        [HttpPost]
        public ActionResult Index(FeedbackViewModel viewModel)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        db.Database.Connection.Open();
                        //string acquistoDecodificato = Uri.UnescapeDataString(viewModel.AcquistoID);
                        //string acquistoPulito = acquistoDecodificato.Trim().Substring(3, acquistoDecodificato.Trim().Length - 6);
                        //int idAcquisto = Utility.DecodeToInt(acquistoPulito);
                        PersonaModel utente = (Session["utente"] as PersonaModel);
                        ANNUNCIO_FEEDBACK model = db.ANNUNCIO_FEEDBACK.Include("Annuncio.Persona").Where(f => f.ID_VOTANTE == utente.Persona.ID && f.ID_ANNUNCIO == viewModel.AcquistoID).SingleOrDefault();
                        if (model != null)
                        {
                            TempData["feedback"] = model;
                            TempData["salvato"] = Language.SavedFeedback;
                            return RedirectToAction("Inviato", new { id = model.ID });
                        }

                        model = new ANNUNCIO_FEEDBACK();
                        ANNUNCIO model2 = null;
                        if (viewModel.Tipo == TipoFeedback.Acquirente)
                        {
                            model2 = db.ANNUNCIO.Where(p => p.ID == viewModel.AcquistoID && p.ID_PERSONA != utente.Persona.ID).SingleOrDefault();
                        }
                        else if (viewModel.Tipo == TipoFeedback.Venditore)
                        {
                            model2 = db.ANNUNCIO.Where(p => p.ID == viewModel.AcquistoID && p.ID_PERSONA == utente.Persona.ID).SingleOrDefault();
                        }

                        if (model2 != null)
                        {
                            model.ID_ANNUNCIO = model2.ID;
                            model.ID_VOTANTE = utente.Persona.ID;
                            model.VOTO = viewModel.Voto;
                            model.COMMENTO = viewModel.Opinione;
                            model.DATA_INSERIMENTO = DateTime.Now;
                            model.DATA_MODIFICA = model.DATA_INSERIMENTO;
                            model.STATO = (int)Stato.ATTIVO;
                            db.ANNUNCIO_FEEDBACK.Add(model);
                            if (db.SaveChanges() > 0)
                            {
                                // feedback salvato
                                AddBonusFeedback(utente.Persona, db, Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["bonusFeedback"]), model.ID_ANNUNCIO);
                                return RedirectToAction("Inviato", new { id = model.ID, nuovo = true });
                            }
                        }
                        ModelState.AddModelError("Errore", Language.ErrorFeedback);
                    }
                }
                catch (Exception exception)
                {
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                    LoggatoreModel.Errore(exception);
                    // se ha un errore generico o semplicemente sta cercando di fare un feedback
                    return Redirect(System.Web.Security.FormsAuthentication.DefaultUrl);
                }
                finally
                {
                    if (db.Database.Connection.State != System.Data.ConnectionState.Closed)
                    {
                        db.Database.Connection.Close();
                        db.Database.Connection.Dispose();
                    }
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult Inviato(int id, bool nuovo = false)
        {
            FeedbackViewModel viewModel = new FeedbackViewModel();
            using (DatabaseContext db = new DatabaseContext())
            {
                try
                {
                    db.Database.Connection.Open();
                    PersonaModel utente = (Session["utente"] as PersonaModel);
                    ANNUNCIO_FEEDBACK model = db.ANNUNCIO_FEEDBACK.Include("Annuncio.Persona").Where(f => f.ID == id && f.ID_VOTANTE == utente.Persona.ID).SingleOrDefault();
                    viewModel.Ricevente = model.ANNUNCIO.PERSONA.NOME + ' ' + model.ANNUNCIO.PERSONA.COGNOME;
                    // AGGIUNGERE SALVATAGGIO ATTIVITà IN CASO SIA UN'AZIENDA A RILASCIARE IL FEEDBACK
                    viewModel.Voto = model.VOTO;
                    viewModel.Opinione = model.COMMENTO;
                    viewModel.Nome = model.ANNUNCIO.NOME;
                    viewModel.DataInvio = model.DATA_INSERIMENTO;
                    if (nuovo)
                        ViewBag.Title = string.Format(Language.TitleSendFeedback, viewModel.Ricevente);
                    else
                        ViewBag.Title = string.Format(Language.TitleSendFeedback2, viewModel.Ricevente);
                    TRANSAZIONE bonusRicevuti = db.TRANSAZIONE.SingleOrDefault(item =>
                        item.ID_CONTO_MITTENTE == utente.Persona.ID_CONTO_CORRENTE &&
                        item.TRANSAZIONE_ANNUNCIO.Count(m => m.ID_ANNUNCIO == model.ID_ANNUNCIO) > 0
                        && item.TIPO == (int)TipoTransazione.BonusFeedback);
                    if (bonusRicevuti != null)
                    {
                        viewModel.PuntiBonus = (int)bonusRicevuti.PUNTI;
                    }
                }
                catch (Exception eccezione)
                {
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
                    LoggatoreModel.Errore(eccezione);
                    // se ha un errore generico o semplicemente sta cercando di fare un feedback
                    return Redirect(System.Web.Security.FormsAuthentication.DefaultUrl);
                }
                finally
                {
                    if (db.Database.Connection.State != System.Data.ConnectionState.Closed)
                    {
                        db.Database.Connection.Close();
                        db.Database.Connection.Dispose();
                    }
                }
            }
            return View(viewModel);
        }
        #endregion

        #region METODI PRIVATI
        private void AddBonusFeedback(PERSONA utente, DatabaseContext db, int punti, int idAnnuncio)
        {
            Guid tokenPortale = Guid.Parse(System.Configuration.ConfigurationManager.AppSettings["portaleweb"]);
            AddBonus(db, ControllerContext, utente, tokenPortale, punti, TipoTransazione.BonusFeedback, Bonus.Feedback, idAnnuncio);
        }
        #endregion
    }
}