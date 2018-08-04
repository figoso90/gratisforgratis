using GratisForGratis.App_GlobalResources;
using GratisForGratis.Filters;
using GratisForGratis.Models;
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
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
        }

        [HttpPost]
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
                                    return RedirectToAction(actionPagamento, "PayPal", new { Token = viewModel.Token, Azione = AzionePayPal.Acquisto });
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
                        Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
                        throw new System.Web.HttpException(404, eccezione.Message);
                    }
                    catch (Exception eccezione)
                    {
                        ModelState.AddModelError("", eccezione.Message);
                        Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
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

        //[HttpGet]
        //public ActionResult Errore(AcquistoViewModel viewModel)
        //{
        //    AnnuncioModel model = null;
        //    if (ModelState.IsValid)
        //    {
        //        using (DatabaseContext db = new DatabaseContext())
        //        {
        //            using (DbContextTransaction transaction = db.Database.BeginTransaction())
        //            {
        //                try
        //                {
        //                    Guid token = getTokenDecodificato(viewModel.Token);
        //                    model = new AnnuncioModel(token, db);
        //                    if (model.TOKEN == null)
        //                    {
        //                        throw new System.Web.HttpException(404, ExceptionMessage.AdNotFound);
        //                    }
        //                    if (TempData["primaVolta"] != null && Convert.ToBoolean(TempData["primaVolta"]))
        //                    {
        //                        if (!Utils.IsUtenteAttivo(1, TempData))
        //                        {
        //                            ModelState.AddModelError("", ErrorResource.UserEnabled);
        //                        }
        //                        else
        //                        {
        //                            model.AnnullaAcquisto(db);
        //                            transaction.Commit();
        //                            //ModelState.AddModelError("", viewModel.Messaggio);
        //                            ModelState.AddModelError("", TempData["errore"] as string);
        //                            TempData["primaVolta"] = null;
        //                        }
        //                    }
        //                }
        //                catch (System.Web.HttpException eccezione)
        //                {
        //                    Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
        //                    throw new System.Web.HttpException(404, eccezione.Message);
        //                }
        //                catch (Exception eccezione)
        //                {
        //                    ModelState.AddModelError("", eccezione.Message);
        //                    Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
        //                }
        //                finally
        //                {
        //                    if (db.Database.CurrentTransaction != null)
        //                        transaction.Rollback();
        //                }
        //                viewModel.Annuncio = model.GetViewModel(db);
        //                viewModel.Annuncio.Azione = "compra";
        //            }
        //        }
        //    }

        //    ViewData["acquistoViewModel"] = viewModel;
        //    return View("Index", viewModel.Annuncio);
        //}

        [HttpPost]
        public ActionResult InviaOfferta(OffertaViewModel viewModel)
        {
            AnnuncioModel model = new AnnuncioModel();
            if (ModelState.IsValid)
            {
                if (!Utils.IsUtenteAttivo(1, TempData))
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                    return Json(ErrorResource.UserEnabled);
                }

                if (viewModel.BarattiToken != null && viewModel.BarattiToken.Count > 4)
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                    return Json(Language.ErrorNothingBarter);
                }

                if (viewModel.BarattiToken.Count <= 0 && viewModel.Punti <= 0)
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                    return Json(ErrorResource.BidNotCorrect);
                }

                using (DatabaseContext db = new DatabaseContext())
                {
                    using (DbContextTransaction transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (viewModel.Save(db))
                            {
                                // aggiungere nella lista dei desideri
                                string tokenDecodificato = HttpContext.Server.UrlDecode(viewModel.Annuncio.Token);
                                Guid tokenGuid = Guid.Parse(tokenDecodificato);
                                PersonaModel utente = (PersonaModel)HttpContext.Session["utente"];
                                addDesiderio(db, tokenGuid, utente.Persona.ID);
                                // salvare transazione
                                transaction.Commit();
                                this.RefreshPunteggioUtente(db);
                                return Json(new { Messaggio = Language.JsonSendBid });
                            }

                            transaction.Rollback();
                        }
                        catch (Exception eccezione)
                        {
                            Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
                            transaction.Rollback();
                            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                            return Json(eccezione.Message);
                        }
                    }
                }
            }
            // acquisto generico
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return Json(ErrorResource.BidAd);
        }

        [HttpPost]
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
                        OffertaModel offerta = new OffertaModel(db.OFFERTA.Where(o => o.ID == idOfferta 
                            && o.ANNUNCIO.ID_PERSONA == utente.Persona.ID 
                            && (o.STATO == (int)StatoOfferta.ATTIVA || o.STATO == (int)StatoOfferta.ACCETTATA_ATTESO_PAGAMENTO)).SingleOrDefault());
                        Models.Enumerators.VerificaAcquisto verifica = offerta.CheckOfferta(utente, offerta);
                        if (verifica == Models.Enumerators.VerificaAcquisto.VerificaCartaCredito)
                        {
                            offerta.OffertaOriginale.STATO = (int)StatoOfferta.ACCETTATA_ATTESO_PAGAMENTO;
                            offerta.OffertaOriginale.SESSIONE_COMPRATORE = HttpContext.Session.SessionID + "§" + Guid.NewGuid().ToString();
                            db.OFFERTA.Attach(offerta.OffertaOriginale);
                            db.Entry(offerta.OffertaOriginale).State = EntityState.Modified;
                            if (db.SaveChanges() > 0)
                            {
                                offerta.ANNUNCIO.STATO = (int)StatoVendita.BARATTOINCORSO;
                                db.ANNUNCIO.Attach(offerta.ANNUNCIO);
                                db.Entry(offerta.ANNUNCIO).State = EntityState.Modified;
                                if (db.SaveChanges() > 0)
                                {
                                    transazioneDb.Commit();
                                    //Session["PayPalCompra"] = viewModel;
                                    Session["PayPalOfferta"] = new OffertaModel(offerta.OffertaOriginale);
                                    return RedirectToAction("Payment", "PayPal", new { Token = token, Azione = AzionePayPal.Offerta });
                                }
                            }
                        }
                        else if (verifica == Models.Enumerators.VerificaAcquisto.Ok)
                        {
                            if (offerta.Accetta(db, utente, ref messaggio))
                            {
                                // se offerta dev'essere pagata, invio notifica e reindirizzo a pagina pagamento
                                // se venditore annulla pagamento, potrà sempre pagare più avanti, sennò feedback negativo e annullo transazioni
                                if (offerta.ANNUNCIO.STATO == (int)StatoVendita.BARATTOINCORSO)
                                {
                                    Models.ViewModels.Email.PagamentoOffertaViewModel pagamentoOfferta = new Models.ViewModels.Email.PagamentoOffertaViewModel();
                                    pagamentoOfferta.NominativoDestinatario = offerta.PERSONA.NOME + " " + offerta.PERSONA.COGNOME;
                                    pagamentoOfferta.NomeAnnuncio = offerta.ANNUNCIO.NOME;
                                    pagamentoOfferta.Moneta = offerta.PUNTI;
                                    pagamentoOfferta.SoldiSpedizione = offerta.SOLDI;
                                    pagamentoOfferta.Baratti = offerta.OFFERTA_BARATTO.Select(m => m.ANNUNCIO.NOME).ToList();
                                    this.SendNotifica(offerta.PERSONA, MessaggioNotifica.PagaOfferta, "pagamentoOfferta", pagamentoOfferta);
                                }
                                transazioneDb.Commit();
                                ViewBag.Message = Language.AcceptedBid;
                                return Redirect(System.Web.HttpContext.Current.Request.UrlReferrer.ToString());
                            }
                        }
                    }
                    catch (Exception eccezione)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
                    }
                    transazioneDb.Rollback();
                }
            }

            ViewBag.Message = Language.ErrorAcceptBid;
            return Redirect(System.Web.HttpContext.Current.Request.UrlReferrer.ToString());
        }

        [HttpPost]
        //[ValidateAjax]
        //public JsonResult RifiutaOfferta(string token)
        public ActionResult RifiutaOfferta(string token)
        {
            int idOfferta = Utils.DecodeToInt(token);
            OffertaModel offerta = new OffertaModel(idOfferta, (Session["utente"] as PersonaModel).Persona);
            if (offerta.Rifiuta())
            {
                //return Json(new { Messaggio = Language.StateBidDelete });
                ViewBag.Message = Language.StateBidDelete;
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
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
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
                        offertaMoneta.RemoveCrediti(db, (int)model.OFFERTA.PUNTI, utente);

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
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
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
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
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
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
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
                if (addDesiderio(db, tokenGuid, idUtente))
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
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            // annuncio non in possesso
            throw new Exception(ExceptionMessage.YouDontHaveThisAd);
        }
        #endregion

        #region METODI PRIVATI
        private bool annullaVenditaSuDatabase(DatabaseContext db, string token)
        {
            Guid tokenDecriptato = getTokenDecodificato(token);
            int idUtente = (Session["utente"] as PersonaModel).Persona.ID;
            ANNUNCIO model = db.ANNUNCIO.Where(v => v.TOKEN == tokenDecriptato && v.ID_PERSONA == idUtente && v.STATO != (int)StatoVendita.BARATTATO
                && v.STATO != (int)StatoVendita.ELIMINATO && v.STATO != (int)StatoVendita.VENDUTO).SingleOrDefault();
            if (model != null)
            {
                model.STATO = (int)StatoVendita.ELIMINATO;
                model.DATA_MODIFICA = DateTime.Now;
                if (db.SaveChanges() > 0)
                {
                    OffertaModel.AnnullaOfferteEffettuate(db, model.ID);
                    OffertaModel.AnnullaOfferteRicevute(db, model.ID);
                    return true;
                }
            }
            return false;
        }

        private Guid getTokenDecodificato(string token)
        {
            string tokenDecode = Server.UrlDecode(token);
            return Guid.Parse(tokenDecode);
            //return Guid.Parse(Utils.DecodeToString(tokenDecode.Substring(3).Substring(0, tokenDecode.Length - 6)));
        }

        private bool addDesiderio(DatabaseContext db, Guid tokenGuid, int idUtente, ANNUNCIO annuncio = null)
        {
            ANNUNCIO_DESIDERATO model = db.ANNUNCIO_DESIDERATO.Where(m => m.ANNUNCIO.TOKEN == tokenGuid && m.ID_PERSONA == idUtente)
                    .FirstOrDefault();
            if (model == null)
            {
                annuncio = db.ANNUNCIO.SingleOrDefault(m => m.TOKEN == tokenGuid && m.ID_PERSONA != idUtente);
                if (annuncio != null)
                {
                    // inserisco l'annuncio tra quelli desiderati
                    model = new ANNUNCIO_DESIDERATO();
                    model.ID_ANNUNCIO = annuncio.ID;
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