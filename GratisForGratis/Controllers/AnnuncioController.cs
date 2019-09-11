using GratisForGratis.App_GlobalResources;
using GratisForGratis.Filters;
using GratisForGratis.Models;
using GratisForGratis.Models.ExtensionMethods;
using System;
using PayPal.Api;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;

namespace GratisForGratis.Controllers
{
    public class AnnuncioController : AdvancedController
    {
        // GET: Annuncio
        [HttpGet]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Index(string token, string azione = "compra")
        {
            try
            {
                Guid tokenDecriptato = getTokenDecodificato(token);
                using (DatabaseContext db = new DatabaseContext())
                {
                    AnnuncioModel model = new AnnuncioModel();
                    AnnuncioViewModel viewModel = model.GetViewModel(db, tokenDecriptato);
                    viewModel.Azione = azione;
                    if (TempData["esito"] != null)
                        ViewBag.Esito = TempData["esito"];
                    else if (TempData["errore"] != null)
                        ModelState.AddModelError("", TempData["errore"] as string);
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                LoggatoreModel.Errore(ex);
            }
            return View();
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Compra(AcquistoViewModel viewModel)
        {
            AnnuncioModel model = null;
            using (DatabaseContext db = new DatabaseContext())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        Guid token = getTokenDecodificato(viewModel.Token);
                        model = new AnnuncioModel(token, db);
                        if (model.TOKEN == null)
                        {
                            throw new System.Web.HttpException(404, ExceptionMessage.AdNotFound);
                        }

                        if (ModelState.IsValid)
                        {
                            if (!Utils.IsUtenteAttivo(1, TempData))
                            {
                                ModelState.AddModelError("", ErrorResource.UserEnabled);
                            }
                            else
                            {
                                Models.Enumerators.VerificaAcquisto verifica = model.Acquisto(db, viewModel);
                                if (verifica == Models.Enumerators.VerificaAcquisto.Ok)
                                {
                                    if (model.CompletaAcquisto(db, viewModel))
                                    {
                                        TempData["Esito"] = Language.JsonBuyAd;
                                        TempData["pagamentoEffettuato"] = true;
                                        transaction.Commit();
                                        this.RefreshPunteggioUtente(db);
                                        this.SendNotifica(model.PERSONA, model.PERSONA1, TipoNotifica.AnnuncioAcquistato, ControllerContext, "annuncioAcquistato", model);
                                        this.SendNotifica(model.PERSONA1, model.PERSONA, TipoNotifica.AnnuncioVenduto, ControllerContext, "annuncioVenduto", model);
                                        return RedirectToAction("Index", "Annuncio", new { token = viewModel.Token });
                                    }
                                    // altrimenti pagamento fallito!
                                }
                                else if (verifica == Models.Enumerators.VerificaAcquisto.VerificaCartaCredito)
                                {
                                    string actionPagamento = "Payment";
                                    if (viewModel.TipoCarta != TipoCartaCredito.PayPal)
                                        actionPagamento = "PaymentWithCreditCard";
                                    transaction.Commit();
                                    Session["PayPalCompra"] = viewModel;
                                    Session["PayPalAnnuncio"] = model;
                                    return RedirectToAction(actionPagamento, "PayPal", new { Id = model.ID, Token = viewModel.Token, Azione = AzionePayPal.Acquisto });
                                }
                                else
                                {
                                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(string.Format("Messaggio {0} per l'errore {1}", ErrorResource.AdBuyFailed, verifica.ToString())));
                                    ModelState.AddModelError("", ErrorResource.AdBuyFailed);
                                }
                            }
                        }
                        //transaction.Rollback();
                    }
                    catch (System.Web.HttpException eccezione)
                    {
                        //Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
                        LoggatoreModel.Errore(eccezione);
                        throw new System.Web.HttpException(404, eccezione.Message);
                    }
                    catch (Exception eccezione)
                    {
                        ModelState.AddModelError("", eccezione.Message);
                        //Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
                        LoggatoreModel.Errore(eccezione);
                    }
                    finally
                    {
                        if (db.Database.CurrentTransaction != null)
                            transaction.Rollback();
                    }
                    viewModel.Annuncio = model.GetViewModel(db);
                    viewModel.Annuncio.Azione = "compra";
                }
            }
            
            ViewData["acquistoViewModel"] = viewModel;
            return View("Index", viewModel.Annuncio);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult InviaOfferta(OffertaViewModel viewModel)
        {
            AnnuncioModel model = new AnnuncioModel();
            string messaggio = ErrorResource.BidAd;
            if (ModelState.IsValid)
            {
                if (!Utils.IsUtenteAttivo(1, TempData))
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                    return Json(ErrorResource.UserEnabled);
                }

                using (DatabaseContext db = new DatabaseContext())
                {
                    using (DbContextTransaction transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (viewModel.Save(db, ref messaggio))
                            {
                                // aggiungere nella lista dei desideri
                                string tokenDecodificato = HttpContext.Server.UrlDecode(viewModel.Annuncio.Token);
                                Guid tokenGuid = Guid.Parse(tokenDecodificato);
                                PersonaModel utente = (PersonaModel)HttpContext.Session["utente"];
                                AnnuncioViewModel annuncio = null;
                                addDesiderio(db, tokenGuid, utente.Persona.ID, ref annuncio);
                                // salvare transazione
                                transaction.Commit();
                                this.RefreshPunteggioUtente(db);
                                // invia e-mail al venditore
                                this.SendNotifica(utente.Persona, annuncio.Venditore.Persona, TipoNotifica.OffertaRicevuta, ControllerContext, "offerta", viewModel);
                                return Json(new { Messaggio = Language.JsonSendBid });
                            }

                            transaction.Rollback();
                        }
                        catch (Exception eccezione)
                        {
                            //Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
                            transaction.Rollback();
                            LoggatoreModel.Errore(eccezione);
                            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                            return Json(eccezione.Message);
                        }
                    }
                }
            }
            // acquisto generico
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return Json(messaggio);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult AccettaOfferta(string token)
        {
            int idOfferta = Utils.DecodeToInt(token);
            string messaggio = "";
            //OffertaModel offerta = new OffertaModel(idOfferta, (Session["utente"] as PersonaModel).Persona);
            using (DatabaseContext db = new DatabaseContext())
            {
                using (var transazioneDb = db.Database.BeginTransaction())
                {
                    try
                    {
                        PersonaModel utente = ((PersonaModel)System.Web.HttpContext.Current.Session["utente"]);
                        OFFERTA offerta = db.OFFERTA.Include(m => m.PERSONA)
                            .Where(o => o.ID == idOfferta && o.ANNUNCIO.ID_PERSONA == utente.Persona.ID
                            && (o.STATO == (int)StatoOfferta.ATTIVA || 
                                o.STATO == (int)StatoOfferta.ACCETTATA_ATTESO_PAGAMENTO)).SingleOrDefault();

                        OffertaModel offertaModel = new OffertaModel(offerta);
                        Models.Enumerators.VerificaOfferta verifica = offertaModel.CheckAccettaOfferta(utente, offerta);
                        // se bisogna pagare la spedizione reindirizzo su paypal
                        if (verifica == Models.Enumerators.VerificaOfferta.VerificaCartaDiCredito)
                        {
                            // prima setto la sessione per il pagamento
                            offerta.STATO = (int)StatoOfferta.ACCETTATA_ATTESO_PAGAMENTO;
                            offerta.SESSIONE_COMPRATORE = HttpContext.Session.SessionID + "§" + Guid.NewGuid().ToString();
                            db.OFFERTA.Attach(offerta);
                            db.Entry(offerta).State = EntityState.Modified;
                            if (db.SaveChanges() > 0)
                            {
                                // metto l'annuncio dell'offerta come baratto in corso
                                offertaModel.ANNUNCIO.STATO = (int)StatoVendita.BARATTOINCORSO;
                                db.ANNUNCIO.Attach(offertaModel.ANNUNCIO);
                                db.Entry(offertaModel.ANNUNCIO).State = EntityState.Modified;
                                if (db.SaveChanges() > 0)
                                {
                                    transazioneDb.Commit();
                                    //Session["PayPalCompra"] = viewModel;
                                    Session["PayPalOfferta"] = new OffertaModel(offertaModel.OffertaOriginale);
                                    return RedirectToAction("Payment", "PayPal", new { Id = offerta.ID, Token = token, Azione = AzionePayPal.Offerta });
                                }
                            }
                        }
                        else if (verifica == Models.Enumerators.VerificaOfferta.Ok)
                        {
                            if (offertaModel.Accetta(db, utente.Persona, null, ref messaggio))
                            {
                                // se offerta dev'essere pagata, invio notifica e reindirizzo a pagina pagamento
                                // se venditore annulla pagamento, potrà sempre pagare più avanti, sennò feedback negativo e annullo transazioni
                                if (offertaModel.ANNUNCIO.STATO == (int)StatoVendita.BARATTOINCORSO)
                                {
                                    Models.ViewModels.Email.PagamentoOffertaViewModel offertaAccettata = new Models.ViewModels.Email.PagamentoOffertaViewModel();
                                    offertaAccettata.NominativoDestinatario = offertaModel.PERSONA.NOME + " " + offertaModel.PERSONA.COGNOME;
                                    offertaAccettata.NomeAnnuncio = offertaModel.ANNUNCIO.NOME;
                                    offertaAccettata.Moneta = offertaModel.PUNTI;
                                    offertaAccettata.SoldiSpedizione = offertaModel.SOLDI;
                                    offertaAccettata.Baratti = offertaModel.OFFERTA_BARATTO.Select(m => m.ANNUNCIO.NOME).ToList();
                                    this.SendNotifica(utente.Persona,offertaModel.PERSONA, TipoNotifica.OffertaAccettata, ControllerContext, "offertaAccettata", offertaAccettata);
                                }
                                transazioneDb.Commit();
                                ViewBag.Message = Language.AcceptedBid;
                                return Redirect(System.Web.HttpContext.Current.Request.UrlReferrer.ToString());
                            }
                        }
                    }
                    catch (Exception eccezione)
                    {
                        //Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
                        LoggatoreModel.Errore(eccezione);
                    }
                    transazioneDb.Rollback();
                }
            }

            TempData["MESSAGGIO"] = Language.ErrorAcceptBid;
            return Redirect(System.Web.HttpContext.Current.Request.UrlReferrer.ToString());
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult CompletaOfferta(string token)
        {
            int idOfferta = Utils.DecodeToInt(token);
            using (DatabaseContext db = new DatabaseContext())
            {
                db.Database.Connection.Open();
                PersonaModel compratore = ((PersonaModel)System.Web.HttpContext.Current.Session["utente"]);
                // verifico: 
                // se è stata accettata l'offerta 
                // se l'offerta è stata fatta dall'utente in sessione
                // se l'annuncio sta aspettando il pagamento
                OFFERTA offerta = db.OFFERTA.Include(m => m.PERSONA)
                            .Where(o => o.ID == idOfferta && o.ID_PERSONA == compratore.Persona.ID
                            && (o.STATO == (int)StatoOfferta.ACCETTATA) 
                            && o.ANNUNCIO.STATO == (int)StatoVendita.BARATTOINCORSO).SingleOrDefault();

                if (offerta!=null)
                {
                    offerta.ANNUNCIO.DATA_MODIFICA = DateTime.Now;
                    offerta.ANNUNCIO.STATO = (int)StatoVendita.SOSPESOPEROFFERTA;
                    offerta.ANNUNCIO.SESSIONE_COMPRATORE = Session.SessionID + "§" + Guid.NewGuid().ToString();
                    db.ANNUNCIO.Attach(offerta.ANNUNCIO);
                    db.Entry(offerta.ANNUNCIO).State = EntityState.Modified;
                    if (db.SaveChanges() <= 0)
                    {
                        Session["PayPalOfferta"] = new OffertaModel(offerta);
                        return RedirectToAction("Payment", "PayPal", new { Id = offerta.ID, Token = token, Azione = AzionePayPal.OffertaOK });
                    }
                }
            }
            ViewBag.Message = ErrorResource.BidComplete;
            return Redirect(System.Web.HttpContext.Current.Request.UrlReferrer.ToString());
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        //[ValidateAjax]
        //public JsonResult RifiutaOfferta(string token)
        public ActionResult RifiutaOfferta(string token)
        {
            int idOfferta = Utils.DecodeToInt(token);
            PERSONA mittente = (Session["utente"] as PersonaModel).Persona;
            OffertaModel offerta = new OffertaModel(idOfferta, mittente);
            if (offerta.Rifiuta())
            {
                //return Json(new { Messaggio = Language.StateBidDelete });
                ViewBag.Message = Language.StateBidDelete;
                this.SendNotifica(mittente, offerta.PERSONA, TipoNotifica.OffertaRifiutata, ControllerContext, "offertaRifiutata", offerta);
                return Redirect(System.Web.HttpContext.Current.Request.UrlReferrer.ToString());
            }
            //Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            //return Json(Language.ErrorRefuseBid);
            ViewBag.Message = Language.ErrorRefuseBid;
            return Redirect(System.Web.HttpContext.Current.Request.UrlReferrer.ToString());
        }

        #region AJAX

        [HttpDelete]
        [ValidateAjax]
        public JsonResult AnnullaVendita(string token)
        {
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    db.Database.Connection.Open();
                    using (var transazione = db.Database.BeginTransaction())
                    {
                        if (annullaVenditaSuDatabase(db, token))
                        {
                            transazione.Commit();
                            return Json(new { Messaggio = Language.StateSellDelete });
                        }
                        transazione.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                LoggatoreModel.Errore(ex);
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return Json(Language.ErrorRefuseBid);
        }

        [HttpDelete]
        [ValidateAjax]
        public JsonResult AnnullaBaratto(string token)
        {
            DatabaseContext db = new DatabaseContext();
            try
            {
                Guid tokenDecriptato = getTokenDecodificato(token);
                PersonaModel utente = (Session["utente"] as PersonaModel);
                //using (DatabaseContext db = new DatabaseContext())
                //{
                db.Database.Connection.Open();
                using (var transazione = db.Database.BeginTransaction())
                {
                    OFFERTA_BARATTO model = db.OFFERTA_BARATTO.Where(b => b.ANNUNCIO.TOKEN == tokenDecriptato && b.OFFERTA.ID_PERSONA == utente.Persona.ID && b.OFFERTA.STATO == (int)StatoOfferta.ATTIVA).SingleOrDefault();
                    if (model != null)
                    {
                        // cambio stato alla vendita
                        model.ANNUNCIO.STATO = (int)StatoVendita.ATTIVO;
                        model.ANNUNCIO.DATA_MODIFICA = DateTime.Now;
                        // cambio stato offerta
                        model.OFFERTA.DATA_MODIFICA = DateTime.Now;
                        model.OFFERTA.STATO = (int)StatoOfferta.ANNULLATA;
                        // restituisco eventuali punti sospesi
                        OffertaContoCorrenteMoneta offertaMoneta = new OffertaContoCorrenteMoneta();
                        offertaMoneta.RemoveCrediti(db, model.OFFERTA.ID, (int)model.OFFERTA.PUNTI, utente);

                        int numeroRecordModificati = db.SaveChanges();
                        if (numeroRecordModificati > 2)
                        {
                            transazione.Commit();
                            return Json(new { Messaggio = App_GlobalResources.Language.StateCanceledBarter });
                        }
                    }
                    transazione.Rollback();
                }
                //}
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                LoggatoreModel.Errore(ex);
            }
            finally
            {
                if (db.Database.Connection.State != System.Data.ConnectionState.Closed)
                {
                    db.Database.Connection.Close();
                    db.Database.Connection.Dispose();
                }
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return Json(App_GlobalResources.Language.ErrorCancelBarter);
        }

        [HttpPost]
        [ValidateAjax]
        public JsonResult AttivaVendita(string token)
        {
            try
            {
                Guid tokenDecriptato = getTokenDecodificato(token);
                int idUtente = (Session["utente"] as PersonaModel).Persona.ID;
                using (DatabaseContext db = new DatabaseContext())
                {
                    ANNUNCIO model = db.ANNUNCIO.Where(v => v.TOKEN == tokenDecriptato && v.ID_PERSONA == idUtente
                        && (v.STATO == (int)StatoVendita.INATTIVO || v.STATO != (int)StatoVendita.ATTIVO)).SingleOrDefault();
                    if (model != null)
                    {
                        model.STATO = (int)StatoVendita.ATTIVO;
                        model.DATA_MODIFICA = DateTime.Now;
                        if (db.SaveChanges() > 0)
                        {
                            return Json(true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                LoggatoreModel.Errore(ex);
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return Json(App_GlobalResources.Language.EnableSellFailed);
        }

        [HttpPost]
        [ValidateAjax]
        public JsonResult Sblocca(string token)
        {
            try
            {
                Guid tokenDecriptato = getTokenDecodificato(token);
                int idUtente = (Session["utente"] as PersonaModel).Persona.ID;
                using (DatabaseContext db = new DatabaseContext())
                {
                    ANNUNCIO model = db.ANNUNCIO.Where(v => v.TOKEN == tokenDecriptato && v.ID_PERSONA == idUtente)
                        .SingleOrDefault();
                    // verifico se l'annuncio è stato sospeso in automatico o per via di un acquisto non concluso da più di 1 giorno
                    if (model != null)
                    {
                        if (model.STATO == (int)StatoVendita.SOSPESO && (model.DATA_MODIFICA == null || DateTime.Now > model.DATA_MODIFICA.Value.AddDays(1)))
                        {
                            model.STATO = (int)StatoVendita.ATTIVO;
                            model.DATA_MODIFICA = DateTime.Now;
                            if (db.SaveChanges() > 0)
                            {
                                LOG_SBLOCCO_ANNUNCIO log = new LOG_SBLOCCO_ANNUNCIO();
                                log.ID_ANNUNCIO = model.ID;
                                log.ID_UTENTE_SBLOCCO = idUtente;
                                log.TIPO = (int)TipoSbloccoAnnuncio.SospensioneAnnuncio;
                                log.DATA_AVVIO = (DateTime)model.DATA_MODIFICA;
                                db.LOG_SBLOCCO_ANNUNCIO.Add(log);
                                db.SaveChanges();
                                return Json(true);
                            }
                        }
                        else if (model.STATO == (int)StatoVendita.ATTIVO && model.DATA_FINE != null && model.DATA_FINE <= DateTime.Now)
                        {
                            TimeSpan span = Convert.ToDateTime(model.DATA_FINE).Subtract(model.DATA_AVVIO);
                            model.DATA_FINE = DateTime.Now.AddDays(span.Days);
                            model.DATA_MODIFICA = DateTime.Now;
                            if (db.SaveChanges() > 0)
                            {
                                LOG_SBLOCCO_ANNUNCIO log = new LOG_SBLOCCO_ANNUNCIO();
                                log.ID_ANNUNCIO = model.ID;
                                log.ID_UTENTE_SBLOCCO = idUtente;
                                log.TIPO = (int)TipoSbloccoAnnuncio.SospensioneAnnuncio;
                                log.DATA_AVVIO = model.DATA_AVVIO;
                                log.DATA_FINE = model.DATA_FINE;
                                db.LOG_SBLOCCO_ANNUNCIO.Add(log);
                                db.SaveChanges();
                                return Json(true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                LoggatoreModel.Errore(ex);
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return Json(ExceptionMessage.UnblockFailed);
        }

        // vuol dire che me ne manca uno
        [HttpPost]
        [ValidateAjax]
        public string Desidero(string token)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                int idUtente = (Session["utente"] as PersonaModel).Persona.ID;
                db.Database.Connection.Open();
                Guid tokenGuid = getTokenDecodificato(token);
                ANNUNCIO annuncio = null;
                if (addDesiderio(db, tokenGuid, idUtente, ref annuncio))
                {
                    AnnuncioViewModel viewModelAnnuncio = new AnnuncioViewModel(db, annuncio);
                    viewModelAnnuncio.Desidero = true;
                    return RenderRazorViewToString("PartialPages/_Desidero", viewModelAnnuncio);
                }
            }
            // desiderio non registrato
            throw new Exception(ExceptionMessage.WishNotSave);
        }

        [HttpPost]
        [ValidateAjax]
        public string NonDesidero(string token)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                int idUtente = (Session["utente"] as PersonaModel).Persona.ID;
                db.Database.Connection.Open();
                Guid tokenGuid = getTokenDecodificato(token);
                IQueryable<ANNUNCIO_DESIDERATO> model = db.ANNUNCIO_DESIDERATO.Where(m => m.ANNUNCIO.TOKEN == tokenGuid && m.ID_PERSONA == idUtente);
                AnnuncioViewModel viewModelAnnuncio = new AnnuncioViewModel(db, model.FirstOrDefault().ANNUNCIO);
                viewModelAnnuncio.Desidero = false;
                db.ANNUNCIO_DESIDERATO.RemoveRange(model);
                if (db.SaveChanges() > 0)
                    return RenderRazorViewToString("PartialPages/_Desidero", viewModelAnnuncio);
            }
            // desiderio non registrato
            throw new Exception(ExceptionMessage.NotWishNotSave);
        }

        [HttpDelete]
        [ValidateAjax]
        public string NonPossiedo(string tokenAnnuncio ,string tokenAnnuncioUtente)
        {
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    db.Database.Connection.Open();
                    using (var transazione = db.Database.BeginTransaction())
                    {
                        if (annullaVenditaSuDatabase(db, tokenAnnuncioUtente))
                        {
                            transazione.Commit();
                            int idUtente = (Session["utente"] as PersonaModel).Persona.ID;
                            Guid tokenGuid = getTokenDecodificato(tokenAnnuncio);
                            ANNUNCIO model = db.ANNUNCIO.FirstOrDefault(m => m.TOKEN == tokenGuid);
                            AnnuncioViewModel viewModelAnnuncio = new AnnuncioViewModel(db, model);
                            return RenderRazorViewToString("PartialPages/_Possiedo", viewModelAnnuncio);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                LoggatoreModel.Errore(ex);
            }
            // annuncio non in possesso
            throw new Exception(ExceptionMessage.YouDontHaveThisAd);
        }
        #endregion

        #region METODI PRIVATI
        private bool annullaVenditaSuDatabase(DatabaseContext db, string token)
        {
            Guid tokenDecriptato = getTokenDecodificato(token);
            AnnuncioModel model = new AnnuncioModel(tokenDecriptato, db);
            return model.Elimina(db);
            //int idUtente = (Session["utente"] as PersonaModel).Persona.ID;
            //ANNUNCIO model = db.ANNUNCIO.Where(v => v.TOKEN == tokenDecriptato && v.ID_PERSONA == idUtente && v.STATO != (int)StatoVendita.BARATTATO
            //    && v.STATO != (int)StatoVendita.ELIMINATO && v.STATO != (int)StatoVendita.VENDUTO).SingleOrDefault();
            //if (model != null)
            //{
            //    model.STATO = (int)StatoVendita.ELIMINATO;
            //    model.DATA_MODIFICA = DateTime.Now;
            //    if (db.SaveChanges() > 0)
            //    {
            //        OffertaModel.AnnullaOfferteEffettuate(db, model.ID);
            //        OffertaModel.AnnullaOfferteRicevute(db, model.ID);
            //        return true;
            //    }
            //}
            //return false;
        }

        private Guid getTokenDecodificato(string token)
        {
            string tokenDecode = Server.UrlDecode(token);
            return Guid.Parse(tokenDecode);
            //return Guid.Parse(Utils.DecodeToString(tokenDecode.Substring(3).Substring(0, tokenDecode.Length - 6)));
        }

        private bool addDesiderio(DatabaseContext db, Guid tokenGuid, int idUtente, ref AnnuncioViewModel annuncio)
        {
            ANNUNCIO_DESIDERATO model = db.ANNUNCIO_DESIDERATO.Where(m => m.ANNUNCIO.TOKEN == tokenGuid && m.ID_PERSONA == idUtente)
                    .FirstOrDefault();
            if (model == null)
            {
                var modelAnnuncio = db.ANNUNCIO.SingleOrDefault(m => m.TOKEN == tokenGuid && m.ID_PERSONA != idUtente);
                annuncio = new AnnuncioViewModel(db, modelAnnuncio);
                if (annuncio != null)
                {
                    // inserisco l'annuncio tra quelli desiderati
                    model = new ANNUNCIO_DESIDERATO();
                    model.ID_ANNUNCIO = annuncio.Id;
                    model.ID_PERSONA = idUtente;
                    model.STATO = (int)Stato.ATTIVO;
                    db.ANNUNCIO_DESIDERATO.Add(model);
                    return (db.SaveChanges() > 0);
                }
            }
            return false;
        }
        #endregion
    }
}