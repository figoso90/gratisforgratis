using System;
using System.Collections.Generic;
using System.Linq;
using GratisForGratis.Models;
using System.Web.Mvc;
using GratisForGratis.Models.ViewModels;
using System.Data.Entity;
using GratisForGratis.Filters;
using System.Web.Security;
using System.Web;
using GratisForGratis.App_GlobalResources;

namespace GratisForGratis.Controllers
{
    [Authorize]
    [HandleError]
    public class PortaleWebController : AdvancedController
    {
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Profilo(string token)
        {
            PortaleWebProfiloViewModel viewModel = null;
            try
            {
                PortaleWebViewModel portale = null;
                if (Session["portaleweb"] != null)
                    portale = (Session["portaleweb"] as List<PortaleWebViewModel>).Where(p => p.Token == token).SingleOrDefault();

                if (portale!=null)
                    viewModel = new PortaleWebProfiloViewModel(portale);
                using (DatabaseContext db = new DatabaseContext())
                {
                    ATTIVITA model = null;
                    if (portale != null)
                    {
                        int id = Convert.ToInt32(portale.Id);
                        model = db.ATTIVITA.SingleOrDefault(p => p.ID == id);
                    }
                    else
                    {
                        model = db.ATTIVITA.SingleOrDefault(p => p.TOKEN.ToString() == token);
                        viewModel = new PortaleWebProfiloViewModel();
                    }
                    // se la pagina non viene trovata
                    if (model == null)
                        return RedirectToAction("Index");

                    viewModel.CopyModel(model, model.ATTIVITA_EMAIL.Where(e => e.ID_ATTIVITA == model.ID).ToList(), model.ATTIVITA_TELEFONO.Where(e => e.ID_ATTIVITA == model.ID).ToList());
                    DateTime unAnnoFa = DateTime.Now.AddYears(-1);
                    var bonus = db.TRANSAZIONE.Where(b => b.ID_CONTO_MITTENTE == model.ID_CONTO_CORRENTE && 
                        b.TIPO == (int)TipoTransazione.BonusFeedback
                        && b.DATA_INSERIMENTO > unAnnoFa).ToList();
                    if (bonus!=null)
                        viewModel.BonusSpeso = bonus.Sum(b => b.PUNTI);
                    viewModel.LoadExtra(db, model);
                }
            }
            catch(Exception exception)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                LoggatoreModel.Errore(exception);
                return RedirectToAction("Index");
            }
            Session["happyPageAperta"] = token;
            return View(viewModel);
        }


        [HttpGet]
        public ActionResult Details(string token)
        {
            PortaleWebProfiloViewModel viewModel = null;
            try
            {
                PortaleWebViewModel portale = null;
                if (Session["portaleweb"] != null)
                    portale = (Session["portaleweb"] as List<PortaleWebViewModel>).Where(p => p.Token == token).SingleOrDefault();

                if (portale != null)
                    viewModel = new PortaleWebProfiloViewModel(portale);
                using (DatabaseContext db = new DatabaseContext())
                {
                    ATTIVITA model = null;
                    if (portale != null)
                    {
                        int id = Convert.ToInt32(portale.Id);
                        model = db.ATTIVITA.SingleOrDefault(p => p.ID == id);
                    }
                    else
                    {
                        model = db.ATTIVITA.SingleOrDefault(p => p.TOKEN.ToString() == token);
                        viewModel = new PortaleWebProfiloViewModel();
                    }
                    // se la pagina non viene trovata
                    if (model == null)
                        return RedirectToAction("Index");

                    viewModel.CopyModel(model, model.ATTIVITA_EMAIL.Where(e => e.ID_ATTIVITA == model.ID).ToList(), model.ATTIVITA_TELEFONO.Where(e => e.ID_ATTIVITA == model.ID).ToList());
                    DateTime unAnnoFa = DateTime.Now.AddYears(-1);
                    var bonus = db.TRANSAZIONE.Where(b => b.ID_CONTO_MITTENTE == model.ID_CONTO_CORRENTE &&
                        b.TIPO == (int)TipoTransazione.BonusFeedback
                        && b.DATA_INSERIMENTO > unAnnoFa).ToList();
                    if (bonus != null)
                        viewModel.BonusSpeso = bonus.Sum(b => b.PUNTI);
                    viewModel.LoadExtra(db, model);
                }
            }
            catch (Exception exception)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                LoggatoreModel.Errore(exception);
                return RedirectToAction("Index");
            }
            Session["happyPageAperta"] = token;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Details(PortaleWebProfiloViewModel viewModel)
        {
            try
            { 
                if (base.ModelState.IsValid)
                {
                    using (DatabaseContext db = new DatabaseContext())
                    {
                        // da modificare
                        PortaleWebViewModel viewModel2 = (Session["portaleweb"] as List<PortaleWebViewModel>).Where(p => p.Token == viewModel.Token).SingleOrDefault();
                        int idPartner = Convert.ToInt32(viewModel2.Id);
                        ATTIVITA model = db.ATTIVITA.Where(p => p.ID == idPartner).SingleOrDefault();
                        model.NOME = viewModel.Nome;
                        model.DOMINIO = viewModel.Dominio;
                        ATTIVITA_EMAIL modelEmail = model.ATTIVITA_EMAIL.SingleOrDefault(item => item.TIPO == (int)TipoEmail.Registrazione);
                        modelEmail.EMAIL = viewModel.Email;
                        db.ATTIVITA_EMAIL.Attach(modelEmail);
                        ATTIVITA_TELEFONO modelTelefono = model.ATTIVITA_TELEFONO.SingleOrDefault(item => item.TIPO == (int)TipoTelefono.Privato);
                        modelTelefono.TELEFONO = viewModel.Telefono;
                        db.ATTIVITA_TELEFONO.Attach(modelTelefono);
                        model.DATA_MODIFICA = DateTime.Now;
                        db.Entry<ATTIVITA>(model).State = System.Data.Entity.EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            // trovare portaleweb modificato e sostituirlo
                            //Session["portaleweb"] = model;
                            Session["portaleweb"] = (Session["utente"] as PersonaModel).Persona.PERSONA_ATTIVITA
                                .Select(item => new PortaleWebViewModel(item, 
                                    item.ATTIVITA.ATTIVITA_EMAIL.Where(e => e.ID_ATTIVITA == item.ID_ATTIVITA).ToList(), 
                                    item.ATTIVITA.ATTIVITA_TELEFONO.Where(t => t.ID_ATTIVITA == item.ID_ATTIVITA).ToList()
                                )).ToList();
                            TempData["salvato"] = true;
                        }
                    }
                }
            }
            catch(Exception exception)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                LoggatoreModel.Errore(exception);
            }
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult SpedizioniInAttesa(int token = 0)
        {
            List<SpedizioneViewModel> lista = new List<SpedizioneViewModel>();
            using (DatabaseContext db = new DatabaseContext())
            {
                lista = db.SPEDIZIONE_INATTESA
                    .Where(m => m.ID == token || token <= 0)
                    .Select(m => new SpedizioneViewModel() {
                        Id = m.ID,
                        NomeAnnuncio = m.NOME_ANNUNCIO,
                        Destinatario = m.NOME + " " + m.COGNOME_RAGSOC,
                        Prezzo = m.PREZZO,
                        Stato = Stato.ATTIVO
                    }).ToList();
            }
            return View(lista);
        }

        [HttpPost]
        public ActionResult SpedizioniInAttesa(SpedizioneViewModel viewModel)
        {
            List<SpedizioneViewModel> lista = new List<SpedizioneViewModel>();
            using (DatabaseContext db = new DatabaseContext())
            {
                db.Database.Connection.Open();
                int token = viewModel.Id;
                if (ModelState.IsValid)
                {
                    // effettuo modifica
                    if (viewModel.LDV != null && Utils.CheckFormatoFile(viewModel.LDV, TipoMedia.TESTO))
                    {
                        CORRIERE_SERVIZIO_SPEDIZIONE spedizione = db.CORRIERE_SERVIZIO_SPEDIZIONE
                            .Include(m => m.INDIRIZZO.PERSONA_INDIRIZZO)
                            .Include(m => m.INDIRIZZO1.PERSONA_INDIRIZZO)
                            .SingleOrDefault(m => m.ID == viewModel.Id);
                        if (spedizione != null)
                        {
                            var annuncio = spedizione.ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE.FirstOrDefault().ANNUNCIO_TIPO_SCAMBIO.ANNUNCIO;
                            string tokenMittente = annuncio.PERSONA.TOKEN.ToString();
                            if (annuncio.ID_ATTIVITA != null)
                            {
                                tokenMittente = annuncio.ATTIVITA.TOKEN.ToString();
                            }
                            // cambiare percorso di salvataggio
                            FileUploadifive allegatoPdf = UploadFile(viewModel.LDV, TipoUpload.Pdf, tokenMittente);
                            PdfModel model = new PdfModel();
                            spedizione.ID_LDV = model.Add(db, allegatoPdf.Nome);
                            spedizione.DATA_MODIFICA = DateTime.Now;
                            spedizione.STATO = (int)StatoSpedizione.LDV;
                            // non modifica
                            db.CORRIERE_SERVIZIO_SPEDIZIONE.Attach(spedizione);
                            db.Entry<CORRIERE_SERVIZIO_SPEDIZIONE>(spedizione).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            var mittente = spedizione.INDIRIZZO.PERSONA_INDIRIZZO.FirstOrDefault().PERSONA;
                            var destinatario = spedizione.INDIRIZZO1.PERSONA_INDIRIZZO.FirstOrDefault().PERSONA;
                            var modelEmail = db.PERSONA_EMAIL.Where(m => m.ID_PERSONA == destinatario.ID && m.TIPO == (int)TipoEmail.Registrazione).FirstOrDefault();
                            // invio email
                            EmailModel email = new EmailModel(ControllerContext);
                            email.To.Add(new System.Net.Mail.MailAddress(modelEmail.EMAIL, mittente.NOME + " " + mittente.COGNOME));
                            email.Subject = string.Format(App_GlobalResources.Email.LDVSubject, viewModel.NomeAnnuncio) + " - " + System.Web.Configuration.WebConfigurationManager.AppSettings["nomeSito"];
                            email.Body = "LDV";
                            email.DatiEmail = viewModel;
                            email.Attachments = new List<System.Web.HttpPostedFileBase>()
                            {
                                viewModel.LDV
                            };
                            new EmailController().SendEmail(email);
                        }
                        else
                        {
                            ViewBag.Message = "Errore nel caricamento dell'LDV! Riprovare più tardi!";
                        }
                    }
                }
                else
                {
                    ViewBag.Message = "Inserire LDV da caricare.";
                }
                token = 0;
                lista = db.SPEDIZIONE_INATTESA
                    .Where(m => m.ID == token || token <= 0)
                    .Select(m => new SpedizioneViewModel()
                    {
                        Id = m.ID,
                        NomeAnnuncio = m.NOME_ANNUNCIO,
                        Destinatario = m.NOME + " " + m.COGNOME_RAGSOC,
                        Prezzo = m.PREZZO,
                        Stato = Stato.ATTIVO
                    }).ToList();
            }
            return View(lista);
        }

        [HttpGet]
        public ActionResult SpedizioniConcluse(int token = 0)
        {
            List<SpedizioneViewModel> lista = new List<SpedizioneViewModel>();
            using (DatabaseContext db = new DatabaseContext())
            {
                lista = db.SPEDIZIONE_CONCLUSA
                    .Where(m => m.ID == token || token <= 0)
                    .Select(m => new SpedizioneViewModel()
                    {
                        Id = m.ID,
                        NomeAnnuncio = m.NOME_ANNUNCIO,
                        Destinatario = m.NOME + " " + m.COGNOME_RAGSOC,
                        Prezzo = m.PREZZO,
                        Stato = Stato.ATTIVO
                    }).ToList();
            }
            return View(lista);
        }

        [HttpPost]
        public ActionResult SpedizioniConcluse(SpedizioneViewModel viewModel)
        {
            List<SpedizioneViewModel> lista = new List<SpedizioneViewModel>();
            using (DatabaseContext db = new DatabaseContext())
            {
                int token = viewModel.Id;
                if (ModelState.IsValid)
                {
                    // effettuo modifica
                    if (viewModel.LDV != null && Utils.CheckFormatoFile(viewModel.LDV, TipoMedia.TESTO))
                    {
                        CORRIERE_SERVIZIO_SPEDIZIONE spedizione = db.CORRIERE_SERVIZIO_SPEDIZIONE.SingleOrDefault(m => m.ID == viewModel.Id);
                        if (spedizione != null)
                        {
                            var annuncio = spedizione.ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE.FirstOrDefault().ANNUNCIO_TIPO_SCAMBIO.ANNUNCIO;
                            string tokenMittente = annuncio.PERSONA.TOKEN.ToString();
                            if (annuncio.ID_ATTIVITA != null)
                            {
                                tokenMittente = annuncio.ATTIVITA.TOKEN.ToString();
                            }
                            // cambiare percorso di salvataggio
                            FileUploadifive allegatoPdf = UploadFile(viewModel.LDV, TipoUpload.Pdf, tokenMittente);
                            PdfModel model = new PdfModel();
                            spedizione.ID_LDV = model.Add(db, allegatoPdf.Nome);
                            spedizione.DATA_MODIFICA = DateTime.Now;
                            spedizione.STATO = (int)StatoSpedizione.LDV;
                            // non modifica
                            db.CORRIERE_SERVIZIO_SPEDIZIONE.Attach(spedizione);
                            db.Entry<CORRIERE_SERVIZIO_SPEDIZIONE>(spedizione).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            ViewBag.Message = "Errore nel caricamento dell'LDV! Riprovare più tardi!";
                        }
                    }
                }
                else
                {
                    ViewBag.Message = "Inserire LDV da caricare.";
                }
                token = 0;
                lista = db.SPEDIZIONE_CONCLUSA
                    .Where(m => m.ID == token || token <= 0)
                    .Select(m => new SpedizioneViewModel()
                    {
                        Id = m.ID,
                        NomeAnnuncio = m.NOME,
                        Prezzo = 0,
                        Stato = Stato.ATTIVO
                    }).ToList();
            }
            return View(lista);
        }

        #region SERVIZI
        [HttpPost]
        [ValidateAjax]
        public JsonResult UploadImmagineProfilo(HttpPostedFileBase file, string token)
        {
            PortaleWebViewModel utente = (Session["portaleweb"] as List<PortaleWebViewModel>).SingleOrDefault(m => m.Token == token);
            if (utente == null)
                return Json(new { Success = false, responseText = ErrorResource.HappyShopNotFound });
            FileUploadifive fileSalvato = UploadImmagine("/Uploads/Images/" + utente.Token + "/" + DateTime.Now.Year.ToString(), file);
            FotoModel model = new FotoModel();
            using (DatabaseContext db = new DatabaseContext())
            {
                db.Database.Connection.Open();
                int idAllegato = model.Add(db, fileSalvato.Nome);
                if (idAllegato > 0)
                {
                    // salvo allegato come immagine del profilo
                    utente.SetImmagineProfilo(db, idAllegato);
                    if (utente.Foto != null && utente.Foto.Count > 0)
                    {
                        string htmlGalleriaFotoProfilo = RenderRazorViewToString("PartialPages/_GalleriaFotoProfilo", new PortaleWebProfiloViewModel(utente));
                        return Json(new { Success = true, responseText = htmlGalleriaFotoProfilo });
                    }
                }
            }
            //Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            //return null;
            return Json(new { Success = false, responseText = Language.ErrorFormatFile });
        }

        [HttpPost]
        [ValidateAjax]
        public ActionResult DeleteImmagineProfilo(int nome, string token)
        {
            PortaleWebViewModel utente = (Session["portaleweb"] as List<PortaleWebViewModel>).SingleOrDefault(m => m.Token == token);
            if (utente == null)
                return Json(new { Success = false, responseText = ErrorResource.HappyShopNotFound });
            using (DatabaseContext db = new DatabaseContext())
            {
                db.Database.Connection.Open();
                if (nome > 0)
                {
                    // salvo allegato come immagine del profilo
                    utente.RemoveImmagineProfilo(db, nome);
                    //return Json(new { Success = true, responseText = true });
                    string htmlGalleriaFotoProfilo = RenderRazorViewToString("PartialPages/_GalleriaFotoProfilo", new PortaleWebProfiloViewModel(utente));
                    return Json(new { Success = true, responseText = htmlGalleriaFotoProfilo });
                }
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return null;
            //return Json(new { Success = false, responseText = Language.ErrorFormatFile });
        }
        #endregion

    }
}