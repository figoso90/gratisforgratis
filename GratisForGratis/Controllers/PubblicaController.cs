using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GratisForGratis.Models;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using Elmah;
using Facebook;
using System.Configuration;
using System.Data.Entity;
using System.Web.Configuration;
using GratisForGratis.App_GlobalResources;
using effts;
using System.Data.SqlClient;

namespace GratisForGratis.Controllers
{
    public class PubblicaController : AdvancedController
    {
        #region ACTION

        [HttpGet]
        public ActionResult Index()
        {
            PubblicazioneViewModel viewModel = new PubblicazioneViewModel();
            // se ho selezionato già una categoria dalla ricerca, carico di default la pubblicazione per quella categoria
            HttpCookie cookie = HttpContext.Request.Cookies.Get("ricerca");
            int idCategoria = Convert.ToInt32(cookie["IDCategoria"]);
            if (idCategoria > 1)
            {
                string nomeVistaDettaglio = GetNomeVistaDettagliAnnuncio(idCategoria);
                if (!string.IsNullOrWhiteSpace(nomeVistaDettaglio))
                {
                    viewModel.CategoriaId = idCategoria;
                    viewModel.CategoriaNome = cookie["Categoria"];
                    ViewData["infoAggiuntive"] = RenderRazorViewToString(nomeVistaDettaglio, null);
                }
            }
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Completato(int id)
        {
            // se ancora la registrazione è incompleta, lo obbligo a concluderla
            if (!Utils.IsUtenteAttivo(0, TempData))
                return RedirectToAction("Impostazioni", "Utente");

            AnnuncioViewModel viewModel = null;
            try
            {
                ANNUNCIO model = new ANNUNCIO();
                using (DatabaseContext db = new DatabaseContext())
                {
                    model = db.ANNUNCIO.Include(m => m.ANNUNCIO_FOTO)
                        .Include(m => m.CATEGORIA).Include("ANNUNCIO_FOTO.FOTO").SingleOrDefault(m => m.ID == id);
                    viewModel = new AnnuncioViewModel(db, model);
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index", "Utente");
            }
            //if (!HttpContext.IsDebuggingEnabled)
                SendPostFacebook(viewModel.Nome + " GRATIS con " + viewModel.Punti + Language.Moneta, GetCurrentDomain() + "/Uploads/Images/" + (Session["utente"] as PersonaModel).Email.FirstOrDefault(item => item.TIPO == (int)TipoEmail.Registrazione) + "/" + DateTime.Now.Year + "/Normal/" + viewModel.Foto[0], GetCurrentDomain());
            return View(viewModel);
        }
        
        #region STEP3 - PAGINE DETTAGLIO ANNUNCIO DA PUBBLICARE

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TelefoniSmartphone(PubblicaTelefoniSmartphoneViewModel model)
        {
            return SaveAnnuncio(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Pc(PubblicaPcViewModel model)
        {
            return SaveAnnuncio(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AudioHiFi(PubblicaTecnologiaViewModel model)
        {
            return SaveAnnuncio(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Console(PubblicaConsoleViewModel model)
        {
            return SaveAnnuncio(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Elettrodomestico(PubblicaElettrodomesticoViewModel model)
        {
            return SaveAnnuncio(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Gioco(PubblicaGiocoViewModel model)
        {
            return SaveAnnuncio(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Libro(PubblicaLibroViewModel model)
        {
            return SaveAnnuncio(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Musica(PubblicaMusicaViewModel model)
        {
            return SaveAnnuncio(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sport(PubblicaSportViewModel model)
        {
            return SaveAnnuncio(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Strumento(PubblicaStrumentoViewModel model)
        {
            return SaveAnnuncio(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Tecnologia(PubblicaTecnologiaViewModel model)
        {
            return SaveAnnuncio(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Veicolo(PubblicaVeicoloViewModel model)
        {
            return SaveAnnuncio(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vestito(PubblicaVestitoViewModel model)
        {
            return SaveAnnuncio(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Video(PubblicaVideoViewModel model)
        {
            return SaveAnnuncio(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Videogames(PubblicaVideogamesViewModel model)
        {
            return SaveAnnuncio(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Oggetto(PubblicaOggettoViewModel model)
        {
            return SaveAnnuncio(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Servizio(PubblicaServizioViewModel model)
        {
            return SaveAnnuncio(model);
        }

        #endregion

        #endregion

        #region SERVIZI

        [HttpPost]
        [Filters.ValidateAjax]
        public ActionResult LoadInfoAggiuntive(int categoria)
        {
            // Modificare questa sezione. Il nome del dettaglio metterlo in una variabile viewdata
            // Richiamare o oggetto o servizio view che si occuperà di richiamare il dettaglio
            // nel dettaglio ricevere oggetto generico, effettuare controllo e trasformarlo in oggetto categoria {tutto questo non funziona, perchè torna i dati in maniera diversa da come li aspetta}
            // Cambiare anche la index e caricare la vista parziale oggetto e servizio
            string nomeVistaDettaglio = GetNomeVistaDettagliAnnuncio(categoria);
            if (!string.IsNullOrWhiteSpace(nomeVistaDettaglio))
                return PartialView(GetNomeVistaDettagliAnnuncio(categoria));
            else
                return PartialView(GetNomeVistaTipologia(categoria), TempData["modelloVista"]);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UploadImmagine(HttpPostedFileBase file)
        {
            if (file != null && Utils.CheckFormatoFile(file))
            {
                string antiForgeryToken = Request.Form.Get("__TokenUploadFoto");
                FileUploadifive foto = UploadImmagine("/Temp/Images/" + Session.SessionID + "/" + antiForgeryToken, file);
                foto.Id = foto.Nome;
                string pathBase = Path.Combine(Server.MapPath("~/Temp/Images/"), Session.SessionID, antiForgeryToken, "Little");
                string[] listaFotoUpload = Directory.GetFiles(pathBase);
                PubblicaListaFotoViewModel model = new PubblicaListaFotoViewModel();
                model.TokenUploadFoto = antiForgeryToken;
                if (listaFotoUpload != null && listaFotoUpload.Length > 0) {
                    model.Foto = listaFotoUpload.Select(m => new FileInfo(m).Name).ToList();
                }
                string htmlGalleriaFotoAnnuncio = RenderRazorViewToString("PartialPages/_GalleriaFotoAnnuncio", model);
                return Json(new { Success = true, responseText = htmlGalleriaFotoAnnuncio, Foto = foto });
                //return Json(new { Success = true, responseText = foto });
            }
            //messaggio di errore
            return Json(new { Success = false, responseText = Language.ErrorFormatFile });
        }

        [HttpPost]
        public ActionResult AnnullaUploadImmagine(string nome)
        {
            try
            {
                string antiForgeryToken = Request.Form.Get("__TokenUploadFoto");
                string pathBase = Path.Combine(Server.MapPath("~/Temp/Images/"), Session.SessionID, antiForgeryToken);
                string pathImgOriginale = Path.Combine(pathBase, "Original", nome);
                string pathImgMedia = Path.Combine(pathBase, "Normal", nome);
                string pathImgPiccola = Path.Combine(pathBase, "Little", nome);

                System.IO.File.Delete(pathImgOriginale);
                System.IO.File.Delete(pathImgMedia);
                System.IO.File.Delete(pathImgPiccola);
                //return Json(new { Success = true });
                string[] listaFotoUpload = Directory.GetFiles(Path.Combine(pathBase, "Little"));
                PubblicaListaFotoViewModel model = new PubblicaListaFotoViewModel();
                model.TokenUploadFoto = antiForgeryToken;
                if (listaFotoUpload != null && listaFotoUpload.Length > 0)
                {
                    model.Foto = listaFotoUpload.Select(m => new FileInfo(m).Name).ToList();
                }
                return PartialView("PartialPages/_GalleriaFotoAnnuncio", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            //return Json(new { Success = false, responseText = Language.ErrorFormatFile });
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return null;
        }

        // devi essere loggato
        [HttpPost]
        public string AddBaratto(BarattoOggettoViewModel model)
        {
            if (ModelState.IsValid && model.File != null)
            {
                List<FileUploadifive> fotoBaratto = new List<FileUploadifive>();
                foreach (HttpPostedFileBase file in model.File)
                {
                    fotoBaratto.Add(UploadImmagine("/Temp/Images/" + Session.SessionID, file));
                }

                if (fotoBaratto.Count <= 0)
                    ModelState.AddModelError("Errore", "Inserire almeno una foto!");

                PubblicaOggettoViewModel oggetto = model;
                /*if (ModelState.IsValid && SaveOggetto(db, oggetto, fotoBaratto))
                {
                    
                    return "Baratto aggiunto con successo!"; // ritornare id oggetto
                }*/
            }
            throw new Exception("Verificare dati oggetto!!");
        }

        [HttpPost]
        [Filters.ValidateAjax]
        public ActionResult GetFormCopiaAnnuncio(string token)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                int idUtente = (Session["utente"] as PersonaModel).Persona.ID;
                string tokenDecodificato = Server.UrlDecode(token);
                //string tokenPulito = tokenDecodificato.Substring(3).Substring(0, tokenDecodificato.Length - 6);
                //Guid tokenGuid = Guid.Parse(tokenPulito);
                Guid tokenGuid = Guid.Parse(tokenDecodificato);
                ANNUNCIO annuncio = db.ANNUNCIO.SingleOrDefault(m => m.TOKEN == tokenGuid && m.ID_PERSONA != idUtente);
                if (annuncio != null)
                {
                    PubblicaCopiaViewModel copia = null;
                    if (annuncio.ID_OGGETTO != null)
                    {
                        copia = new PubblicaOggettoCopiaViewModel(annuncio);
                        return PartialView("PartialPages/_CopiaOggetto", copia);
                    }
                    else
                    {
                        copia = new PubblicaServizioCopiaViewModel(annuncio);
                        return PartialView("PartialPages/_CopiaServizio", copia);
                    }
                }
            }
            throw new Exception(ExceptionMessage.OpenCopyForm);
        }

        [HttpPost]
        [Filters.ValidateAjax]
        public string CopiaOggetto(PubblicaOggettoCopiaViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    using (DbContextTransaction transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            PERSONA persona = (Session["utente"] as PersonaModel).Persona;
                            Guid token = Guid.Parse(viewModel.TokenOK);
                            ANNUNCIO annuncio = db.ANNUNCIO.SingleOrDefault(m => m.TOKEN == token && m.ID_PERSONA != persona.ID);
                            if (annuncio != null)
                            {
                                // controllo se ha già quell'annuncio
                                ANNUNCIO annuncioInPossesso = db.ANNUNCIO.FirstOrDefault(m =>  m.ID_PERSONA == persona.ID 
                                    && (m.STATO == (int)StatoVendita.ATTIVO || m.STATO == (int)StatoVendita.INATTIVO) 
                                    && (m.ID == annuncio.ID || m.ID_ORIGINE == annuncio.ID));
                                if (annuncioInPossesso == null)
                                {
                                    ANNUNCIO_DESIDERATO annuncioDesiderato = db.ANNUNCIO_DESIDERATO.Where(m => m.ANNUNCIO.TOKEN == token && m.ID_PERSONA == persona.ID).SingleOrDefault();
                                    if (annuncioDesiderato != null)
                                    {
                                        db.ANNUNCIO_DESIDERATO.Remove(annuncioDesiderato);
                                        db.SaveChanges();
                                    }
                                    // copiare e salvare annuncio
                                    PubblicazioneViewModel viewModelAnnuncio = UpdateOggetto(annuncio, viewModel);
                                    // copia foto
                                    List<string> fotoEsistenti = annuncio.ANNUNCIO_FOTO
                                        .Where(m => viewModelAnnuncio.Foto.Contains(m.ALLEGATO.NOME) == true)
                                        .Select(m => m.ALLEGATO.NOME).ToList();
                                    for (int i=0; i < fotoEsistenti.Count; i++)
                                    {
                                        string nomeFileOriginale = Server.MapPath("~/Uploads/Images/" + annuncio.PERSONA.TOKEN + "/" + annuncio.DATA_INSERIMENTO.Year + "/Original/" + fotoEsistenti[i]);
                                        HttpFile fileOriginale = new HttpFile(nomeFileOriginale);
                                        FileUploadifive fileSalvatato = UploadImmagine("/Temp/Images/" + Session.SessionID + "/" + viewModel.TokenUploadFoto, fileOriginale);
                                        if (fileSalvatato != null)
                                        {
                                            string[] array = viewModelAnnuncio.Foto.ToArray();
                                            int indiceArray = Array.IndexOf(array, fileSalvatato.NomeOriginale);
                                            viewModelAnnuncio.Foto[indiceArray] = fileSalvatato.Nome;
                                        }
                                    }
                                    viewModelAnnuncio.DbContext = db;
                                    annuncioInPossesso = new ANNUNCIO();
                                    if (viewModelAnnuncio.SalvaAnnuncio(ControllerContext, annuncioInPossesso))
                                    {
                                        int? idAnnuncio = annuncioInPossesso.ID;
                                        if (idAnnuncio != null)
                                        {
                                            // ASSEGNAZIONE CREDITI
                                            PersonaModel utente = ((PersonaModel)Session["utente"]);
                                            viewModelAnnuncio.InviaEmail(ControllerContext, annuncio, utente);
                                            int numeroCreditiBonus = AddBonus(db, utente, viewModelAnnuncio);
                                            TempData["BONUS"] = string.Format(Bonus.YouWin, numeroCreditiBonus, Language.Moneta);

                                            annuncioInPossesso.PERSONA = persona; // perchè sennò non riesce a recuperare l'associazione
                                            AnnuncioViewModel nuovoAnnuncio = new AnnuncioViewModel(db, annuncioInPossesso);
                                            transaction.Commit();
                                            return RenderRazorViewToString("PartialPages/_Possiedo", nuovoAnnuncio);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception eccezione)
                        {
                            transaction.Rollback();
                            ErrorSignal.FromCurrentContext().Raise(eccezione);
                        }
                    }
                }
            }
            // desiderio non registrato
            throw new Exception(ExceptionMessage.CopyAdvertising);
        }

        [HttpPost]
        [Filters.ValidateAjax]
        public string CopiaServizio(PubblicaServizioCopiaViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    using (DbContextTransaction transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            PERSONA persona = (Session["utente"] as PersonaModel).Persona;
                            Guid token = Guid.Parse(viewModel.TokenOK);
                            ANNUNCIO annuncio = db.ANNUNCIO.SingleOrDefault(m => m.TOKEN == token && m.ID_PERSONA != persona.ID);
                            if (annuncio != null)
                            {
                                // controllo se ha già quell'annuncio
                                ANNUNCIO annuncioInPossesso = db.ANNUNCIO.FirstOrDefault(m => m.ID_PERSONA == persona.ID
                                    && (m.STATO == (int)StatoVendita.ATTIVO || m.STATO == (int)StatoVendita.INATTIVO)
                                    && (m.ID == annuncio.ID || m.ID_ORIGINE == annuncio.ID));
                                if (annuncioInPossesso == null)
                                {
                                    ANNUNCIO_DESIDERATO annuncioDesiderato = db.ANNUNCIO_DESIDERATO.Where(m => m.ANNUNCIO.TOKEN == token && m.ID_PERSONA == persona.ID).SingleOrDefault();
                                    if (annuncioDesiderato != null)
                                    {
                                        db.ANNUNCIO_DESIDERATO.Remove(annuncioDesiderato);
                                        db.SaveChanges();
                                    }
                                    // copiare e salvare annuncio
                                    PubblicazioneViewModel viewModelAnnuncio = UpdateServizio(annuncio, viewModel);
                                    // copia foto
                                    for (int i=0; i<viewModelAnnuncio.Foto.Count; i++)
                                    {
                                        ANNUNCIO_FOTO annuncioFoto = annuncio.ANNUNCIO_FOTO.SingleOrDefault(f => f.ALLEGATO.NOME == viewModelAnnuncio.Foto[i]);
                                        if (annuncioFoto != null)
                                        {
                                            string nomeFileOriginale = Server.MapPath("~/Uploads/Images/" + annuncio.PERSONA.TOKEN + "/" + annuncioFoto.DATA_INSERIMENTO.Year + "/Original/" + annuncioFoto.ALLEGATO.NOME);
                                            HttpFile fileOriginale = new HttpFile(nomeFileOriginale);
                                            FileUploadifive fileSalvatato = UploadImmagine("/Temp/Images/" + Session.SessionID + "/" + viewModel.TokenUploadFoto, fileOriginale);
                                            if (fileSalvatato != null)
                                            {
                                                string[] array = viewModelAnnuncio.Foto.ToArray();
                                                int indiceArray = Array.IndexOf(array, fileSalvatato.NomeOriginale);
                                                viewModelAnnuncio.Foto[indiceArray] = fileSalvatato.Nome;
                                            }
                                        }
                                    }
                                    viewModelAnnuncio.DbContext = db;
                                    annuncioInPossesso = new ANNUNCIO();
                                    if (viewModelAnnuncio.SalvaAnnuncio(ControllerContext, annuncioInPossesso))
                                    {
                                        int? idAnnuncio = annuncioInPossesso.ID;
                                        if (idAnnuncio != null)
                                        {
                                            // ASSEGNAZIONE CREDITI
                                            PersonaModel utente = ((PersonaModel)Session["utente"]);
                                            viewModelAnnuncio.InviaEmail(ControllerContext, annuncio, utente);
                                            int numeroCreditiBonus = AddBonus(db, utente, viewModelAnnuncio);
                                            TempData["BONUS"] = string.Format(Bonus.YouWin, numeroCreditiBonus, Language.Moneta);

                                            annuncioInPossesso.PERSONA = persona; // perchè sennò non riesce a recuperare l'associazione
                                            AnnuncioViewModel nuovoAnnuncio = new AnnuncioViewModel(db, annuncioInPossesso);
                                            transaction.Commit();
                                            return RenderRazorViewToString("PartialPages/_Possiedo", nuovoAnnuncio);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception eccezione)
                        {
                            transaction.Rollback();
                            ErrorSignal.FromCurrentContext().Raise(eccezione);
                        }
                    }
                }
            }
            // desiderio non registrato
            throw new Exception(ExceptionMessage.CopyAdvertising);
        }

        [HttpPost]
        [Filters.ValidateAjax]
        public JsonResult GetPrezzoSpedizione(int servizioSpedizione, decimal altezza, decimal larghezza, decimal lunghezza)
        {
            PrezzoSpedizioneViewModel prezzoViewModel = new PrezzoSpedizioneViewModel();
            prezzoViewModel.Prezzo = Convert.ToInt32(ConfigurationManager.AppSettings["spedizionePrezzoMax"]);
            prezzoViewModel.Visibile = true;
            decimal pesoVolumetrico = 0;
            // calcolo peso volumetrico! (lunghezza x altezza x larghezza) / 5000
            pesoVolumetrico = (lunghezza * altezza * larghezza) / 5000;
            using (DatabaseContext db = new DatabaseContext())
            {
                // gestire peso minimo di un servizio con lo 0 e peso massimo con NULL
                var prezzoStimato = db.CORRIERE_SERVIZIO_PREZZO_STIMATO.SingleOrDefault(m => m.ID_CORRIERE_SERVIZIO == servizioSpedizione
                    && (m.PESO_MINIMO <= pesoVolumetrico && (m.PESO_MASSIMO > pesoVolumetrico || m.PESO_MASSIMO == null)) && m.STATO == (int)Stato.ATTIVO);
                // settaggio prezzo servizio
                if (prezzoStimato != null)
                {
                    prezzoViewModel.Prezzo = prezzoStimato.PREZZO_STIMATO;
                    prezzoViewModel.Visibile = true;
                }
            }
            return Json(prezzoViewModel);
        }

        #endregion

        #region METODI PRIVATI

        private ActionResult SaveAnnuncio(PubblicazioneViewModel viewModel)
        {
            ANNUNCIO annuncio = new ANNUNCIO();
            using (DatabaseContext db = new DatabaseContext())
            {
                db.Database.Connection.Open();
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try {
                        viewModel.DbContext = db;
                        // verificare come aprire la transazione sul salvataggio, effettuare commit o rollback
                        if (ModelState.IsValid && viewModel.SalvaAnnuncio(ControllerContext, annuncio))
                        {
                            int? idAnnuncio = annuncio.ID;
                            if (idAnnuncio != null)
                            {
                                PersonaModel utente = ((PersonaModel)Session["utente"]);
                                viewModel.InviaEmail(ControllerContext, annuncio, utente);
                                
                                int numeroCreditiBonus = AddBonus(db, utente, viewModel);
                                if (numeroCreditiBonus > 0)
                                    TempData["BONUS"] = string.Format(Bonus.YouWin, numeroCreditiBonus, Language.Moneta);

                                transaction.Commit();
                                return RedirectToAction("Completato", new { id = idAnnuncio });
                            }
                        }
                        transaction.Rollback();
                    }
                    catch (Exception eccezione)
                    {
                        transaction.Rollback();
                        viewModel.CancelUploadFoto(annuncio);
                        ModelState.AddModelError("Error", eccezione.Message);
                        ErrorSignal.FromCurrentContext().Raise(eccezione);
                    }
                }
            }
            // se ha già scelto una categoria ricarico i campi
            string nomeVistaDettaglio = GetNomeVistaDettagliAnnuncio(viewModel.CategoriaId);
            if (!string.IsNullOrWhiteSpace(nomeVistaDettaglio))
            {
                ViewData["infoAggiuntive"] = RenderRazorViewToString(nomeVistaDettaglio, viewModel);
            }
            else
            {
                string nomeTipologiaDettaglio = GetNomeVistaTipologia(viewModel.CategoriaId);
                if (!string.IsNullOrWhiteSpace(nomeTipologiaDettaglio))
                    ViewData["infoAggiuntive"] = RenderRazorViewToString(nomeTipologiaDettaglio, viewModel);
            }
            return View("Index", viewModel);
        }

        private string SendPostFacebook(string message, string picture, string link)
        {
            try
            {
                FacebookClient app = new FacebookClient(ConfigurationManager.AppSettings["TokenPermanente"]);
                Dictionary<string, object> feed = new Dictionary<string, object>() {
                    { "message", message },
                    { "picture", picture },
                    { "link", link }
                };
                var isSend = app.Post("/" + ConfigurationManager.AppSettings["FanPageID"] + "/feed", feed);
                return (string)isSend;
            }
            catch (FacebookOAuthException ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            catch (FacebookApiException ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return string.Empty;
        }

        private string GetNomeVistaDettagliAnnuncio(int categoria)
        {
            List<FINDSOTTOCATEGORIE_Result> listaCategorie = (HttpContext.Application["categorie"] as List<FINDSOTTOCATEGORIE_Result>);
            FINDSOTTOCATEGORIE_Result model = listaCategorie.SingleOrDefault(item => item.ID == categoria);
            if (model != null)
            {
                string tipoAnnuncio = ((TipoAcquisto)model.TIPO_VENDITA).ToString();
                string paginaAltreInfo = string.Concat(tipoAnnuncio, "/", model.DESCRIZIONE);
                if (ViewEngines.Engines.FindView(ControllerContext, paginaAltreInfo, null).View != null)
                    return paginaAltreInfo;
            }
            return null;
        }

        private string GetNomeVistaTipologia(int categoria)
        {
            List<FINDSOTTOCATEGORIE_Result> listaCategorie = (HttpContext.Application["categorie"] as List<FINDSOTTOCATEGORIE_Result>);
            FINDSOTTOCATEGORIE_Result model = listaCategorie.SingleOrDefault(item => item.ID == categoria);
            if (model != null)
            {
                TipoAcquisto tipoAcquisto = ((TipoAcquisto)model.TIPO_VENDITA);
                string tipoAnnuncio = tipoAcquisto.ToString();
                ViewData["ActionTipologia"] = "/Pubblica/" + tipoAnnuncio;
                if (tipoAcquisto == TipoAcquisto.Servizio)
                    TempData["modelloVista"] = new PubblicaServizioViewModel();
                else
                    TempData["modelloVista"] = new PubblicaOggettoViewModel();
                return tipoAnnuncio;
            }
            return null;
        }

        private PubblicazioneViewModel UpdateOggetto(ANNUNCIO model, PubblicaOggettoCopiaViewModel viewModelCopia)
        {
            PubblicazioneViewModel viewModel = null;
            if (model.ID_CATEGORIA == 12)
            {
                viewModel = new PubblicaTelefoniSmartphoneViewModel(model);
            }
            else if (model.ID_CATEGORIA == 64)
            {
                viewModel = new PubblicaConsoleViewModel(model);
            }
            else if (model.ID_CATEGORIA == 13 || (model.ID_CATEGORIA >= 62 && model.ID_CATEGORIA <= 63) || model.ID_CATEGORIA == 65)
            {
                viewModel = new PubblicaTecnologiaViewModel(model);
            }
            else if (model.ID_CATEGORIA == 14)
            {
                viewModel = new PubblicaPcViewModel(model);                
            }
            else if (model.ID_CATEGORIA == 26)
            {
                viewModel = new PubblicaElettrodomesticoViewModel(model);                
            }
            else if ((model.ID_CATEGORIA >= 28 && model.ID_CATEGORIA <= 39) || model.ID_CATEGORIA == 41)
            {
                viewModel = new PubblicaMusicaViewModel(model);
            }
            else if (model.ID_CATEGORIA == 40)
            {
                viewModel = new PubblicaStrumentoViewModel(model);
            }
            else if (model.ID_CATEGORIA == 45)
            {
                viewModel = new PubblicaVideogamesViewModel(model);
            }
            else if (model.ID_CATEGORIA >= 42 && model.ID_CATEGORIA <= 47)
            {
                viewModel = new PubblicaGiocoViewModel(model);                
            }
            else if (model.ID_CATEGORIA >= 50 && model.ID_CATEGORIA <= 61)
            {
                viewModel = new PubblicaSportViewModel(model);
            }
            else if (model.ID_CATEGORIA >= 67 && model.ID_CATEGORIA <= 80)
            {
                viewModel = new PubblicaVideoViewModel(model);
            }
            else if (model.ID_CATEGORIA >= 81 && model.ID_CATEGORIA <= 85)
            {
                viewModel = new PubblicaLibroViewModel(model);                
            }
            else if (model.ID_CATEGORIA >= 89 && model.ID_CATEGORIA <= 93)
            {
                viewModel = new PubblicaVeicoloViewModel(model);
            }
            else if (model.ID_CATEGORIA >= 127 && model.ID_CATEGORIA <= 170 && model.ID_CATEGORIA != 161 && model.ID_CATEGORIA != 152 && model.ID_CATEGORIA != 141 && model.ID_CATEGORIA != 127)
            {
                viewModel = new PubblicaVestitoViewModel(model);
            }
            // se è stato copiato l'annuncio, allora riporto le modifiche
            if (viewModel != null)
                viewModel.Update(viewModelCopia);

            return viewModel;
        }

        private PubblicazioneViewModel UpdateServizio(ANNUNCIO model, PubblicaServizioCopiaViewModel viewModelCopia)
        {
            PubblicazioneViewModel viewModel = null;
            viewModel = new PubblicaServizioViewModel(model);
            // se è stato copiato l'annuncio, allora riporto le modifiche
            if (viewModel != null)
                viewModel.Update(viewModelCopia);

            return viewModel;
        }

        private int AddBonus(DatabaseContext db, PersonaModel utente, PubblicazioneViewModel viewModel)
        {
            bool risultato = false;
            int numeroPuntiGuadagnati = 0;

            // verifico se dare un bonus dopo un certo numero di pubblicazioni
            Guid portale = Guid.Parse(ConfigurationManager.AppSettings["portaleweb"]);
            Guid idContoCorrente = db.ATTIVITA.SingleOrDefault(p => p.TOKEN == portale).ID_CONTO_CORRENTE;
            int numeroVendite = db.ANNUNCIO.Where(v => v.ID_PERSONA == utente.Persona.ID).GroupBy(v => v.ID_CATEGORIA).Count();

            // aggiunge il bonus sui primi tot. annunci iniziali
            TRANSAZIONE bonus = db.TRANSAZIONE.Where(b => b.ID_CONTO_MITTENTE == idContoCorrente
                && b.ID_CONTO_DESTINATARIO == utente.Persona.ID_CONTO_CORRENTE && b.TIPO == (int)TipoTransazione.BonusPubblicazioneIniziale).FirstOrDefault();
            if (numeroVendite == Convert.ToInt32(ConfigurationManager.AppSettings["numeroPubblicazioniBonus"])
                && bonus == null)
            {
                int puntiBonusIniziali = Convert.ToInt32(ConfigurationManager.AppSettings["bonusPubblicazioniIniziali"]);
                this.AddBonus(db, utente.Persona, portale, puntiBonusIniziali, 
                    TipoTransazione.BonusPubblicazioneIniziale, Bonus.InitialPubblication);
                numeroPuntiGuadagnati += (int)puntiBonusIniziali;
                risultato = risultato | true;
            }

            // aggiunge bonus se l'annuncio è completo di tutti i dati
            if (viewModel.IsAnnuncioCompleto())
            {
                int puntiAnnuncioCompleto = Convert.ToInt32(ConfigurationManager.AppSettings["bonusAnnuncioCompleto"]);
                this.AddBonus(db, utente.Persona, portale, puntiAnnuncioCompleto,
                    TipoTransazione.BonusAnnuncioCompleto, Bonus.FullAnnouncement);
                numeroPuntiGuadagnati += puntiAnnuncioCompleto;
                risultato = risultato | true;
            }

            return ((risultato) ? numeroPuntiGuadagnati : 0);
            //return ((risultato) ? (int)bonus.PUNTI : 0);
        }

        #endregion

    }
}