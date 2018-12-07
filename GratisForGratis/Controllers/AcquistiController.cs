using GratisForGratis.App_GlobalResources;
using GratisForGratis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GratisForGratis.Controllers
{
    public class AcquistiController : AdvancedController
    {
        [HttpGet]
        public ActionResult Index(int pagina = 1)
        {
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    int utente = ((PersonaModel)Session["utente"]).Persona.ID;
                    // verifica stato offerta se attivo o non accettata, identità utente e stato di attivazione, presenza bene o servizio -- vechia
                    // verifica identità utente e stato di attivazione, presenza bene o servizio
                    return View(GetListaOfferte(db, utente, pagina));
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(string token, int pagina = 1)
        {
            AnnuncioModel model = null;
            using (DatabaseContext db = new DatabaseContext())
            {
                using (System.Data.Entity.DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        int id = Utils.DecodeToInt(token);
                        OFFERTA offerta = db.OFFERTA.SingleOrDefault(m => m.ID == id);
                        if (offerta == null)
                        {
                            throw new HttpException(404, ExceptionMessage.AdNotFound);
                        }
                        model = new AnnuncioModel(offerta.ANNUNCIO);
                        if (model.TOKEN == null)
                        {
                            throw new HttpException(404, ExceptionMessage.AdNotFound);
                        }

                        AcquistoViewModel viewModel = new AcquistoViewModel(offerta);
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
                    catch (HttpException eccezione)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
                        throw new HttpException(404, eccezione.Message);
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
                }
                int utente = ((PersonaModel)Session["utente"]).Persona.ID;
                return View(GetListaOfferte(db, utente, pagina));
            }
        }

        [HttpGet]
        public ActionResult OfferteRifiutate(int pagina = 1)
        {
            List<OffertaViewModel> offerte = new List<OffertaViewModel>();
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    int utente = ((PersonaModel)Session["utente"]).Persona.ID;
                    // verifica stato offerta se attivo o non accettata, identità utente e stato di attivazione, presenza bene o servizio -- vechia
                    // verifica identità utente e stato di attivazione, presenza bene o servizio
                    var query = db.OFFERTA.Where(item => item.PERSONA.ID == utente && item.PERSONA.STATO == (int)Stato.ATTIVO
                        && (item.STATO == (int)StatoOfferta.ANNULLATA)
                        && (item.ANNUNCIO.ID_OGGETTO != null || item.ANNUNCIO.ID_SERVIZIO != null));
                    int numeroElementi = Convert.ToInt32(WebConfigurationManager.AppSettings["numeroElementi"]);
                    ViewData["TotalePagine"] = (int)Math.Ceiling((decimal)query.Count() / (decimal)numeroElementi);
                    ViewData["Pagina"] = pagina;
                    pagina -= 1;
                    string randomString = Utils.RandomString(3);
                    List<OFFERTA> lista = query
                        .OrderByDescending(item => item.DATA_INSERIMENTO)
                        .Skip(pagina * numeroElementi)
                        .Take(numeroElementi).ToList();

                    foreach (OFFERTA item in lista)
                    {
                        OffertaViewModel offertaEffettuata = new OffertaViewModel(db, item);
                        offerte.Add(offertaEffettuata);
                    }
                    if (offerte.Count > 0)
                        RefreshPunteggioUtente(db);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View(offerte);
        }

        [HttpGet]
        public ActionResult Acquisto(string acquisto = null, string baratto = null)
        {
            if (String.IsNullOrEmpty(acquisto) && String.IsNullOrEmpty(baratto))
                return RedirectToAction("", "Home");

            OffertaViewModel viewModel = new OffertaViewModel();
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    int numeroElementi = Convert.ToInt32(WebConfigurationManager.AppSettings["numeroElementi"]);
                    OFFERTA offerta = new OFFERTA();
                    int idUtente = (Session["utente"] as PersonaModel).Persona.ID;
                    // fare un if e fare ricerca o per acquisto direttamente o per baratto
                    if (!String.IsNullOrEmpty(acquisto))
                    {
                        //Guid tokenAcquisto = Guid.Parse(Utils.DecodeToString(acquisto.Trim().Substring(3, acquisto.Trim().Length - 6)));
                        Guid tokenAcquisto = Guid.Parse(acquisto);
                        offerta = db.OFFERTA.SingleOrDefault(c => c.ANNUNCIO.TOKEN == tokenAcquisto && c.ANNUNCIO.ID_PERSONA == idUtente && c.PERSONA.STATO == (int)Stato.ATTIVO);
                    }
                    else
                    {
                        //Guid tokenBaratto = Guid.Parse(Utils.DecodeToString(baratto.Trim().Substring(3, baratto.Trim().Length - 6)));
                        Guid tokenBaratto = Guid.Parse(baratto);
                        offerta = db.OFFERTA.SingleOrDefault(c => c.OFFERTA_BARATTO.Count(b => b.ANNUNCIO.TOKEN == tokenBaratto && b.ANNUNCIO.ID_PERSONA == idUtente) > 0);
                    }
                    viewModel = new OffertaViewModel(db, offerta);
                    /*
                    viewModel = new OffertaViewModel()
                    {
                        Id = offerta.ID,
                        Token = Utils.Encode(offerta.ID),
                        Annuncio = new AnnuncioViewModel()
                        {
                            Nome = offerta.ANNUNCIO.NOME,
                            Categoria = offerta.ANNUNCIO.CATEGORIA.NOME,
                            TipoSpedizione = (TipoSpedizione)offerta.TIPO_TRATTATIVA,
                            StatoVendita = (StatoVendita)offerta.ANNUNCIO.STATO,
                            Foto = offerta.ANNUNCIO.ANNUNCIO_FOTO.Select(f => new AnnuncioFoto()
                            {
                                ID_ANNUNCIO = f.ID_ANNUNCIO,
                                FOTO = f.FOTO,
                                DATA_INSERIMENTO = f.DATA_INSERIMENTO,
                                DATA_MODIFICA = f.DATA_MODIFICA
                            }).ToList(),
                            Venditore = new UtenteVenditaViewModel() {
                                Nominativo = (offerta.ANNUNCIO.ID_ATTIVITA != null) ? offerta.ANNUNCIO.ATTIVITA.NOME : offerta.ANNUNCIO.PERSONA.NOME + ' ' + offerta.ANNUNCIO.PERSONA.COGNOME,
                                Email = (offerta.ANNUNCIO.ID_ATTIVITA != null) ? offerta.ANNUNCIO.ATTIVITA.ATTIVITA_EMAIL.SingleOrDefault(e => e.TIPO == (int)TipoEmail.Registrazione).EMAIL : offerta.ANNUNCIO.PERSONA.PERSONA_EMAIL.SingleOrDefault(e => e.TIPO == (int)TipoEmail.Registrazione).EMAIL,
                                VenditoreToken = offerta.ANNUNCIO.PERSONA.TOKEN,
                                Telefono = (offerta.ANNUNCIO.ID_ATTIVITA != null) ? offerta.ANNUNCIO.ATTIVITA.ATTIVITA_TELEFONO.SingleOrDefault(t => t.TIPO == (int)TipoTelefono.Privato).TELEFONO : offerta.ANNUNCIO.PERSONA.PERSONA_TELEFONO.SingleOrDefault(t => t.TIPO == (int)TipoTelefono.Privato).TELEFONO,
                            }
                        },
                        Punti = (int)offerta.PUNTI,
                        Soldi = (int)offerta.SOLDI,
                        Baratti = db.OFFERTA_BARATTO.Where(b => b.ID_OFFERTA == offerta.ID && b.ANNUNCIO != null).Select(b =>
                                    new AnnuncioViewModel()
                                    {
                                        Token = b.ANNUNCIO.TOKEN.ToString(),
                                        TipoAcquisto = b.ANNUNCIO.SERVIZIO != null ? TipoAcquisto.Servizio : TipoAcquisto.Oggetto,
                                        Nome = b.ANNUNCIO.NOME,
                                        Punti = b.ANNUNCIO.PUNTI,
                                        Soldi = b.ANNUNCIO.SOLDI,
                                    }).ToList(),
                        TipoOfferta = (TipoPagamento)offerta.TIPO_OFFERTA,
                        TipoPagamento = (TipoPagamento)offerta.ANNUNCIO.TIPO_PAGAMENTO,
                        StatoOfferta = (StatoOfferta)offerta.STATO,
                        DataInserimento = (DateTime)offerta.DATA_INSERIMENTO,
                        //PuntiCompratore = offerta.PERSONA.CONTO_CORRENTE.CONTO_CORRENTE_MONETA.Count(m => m.STATO == (int)StatoMoneta.ASSEGNATA),
                        //PuntiSospesiCompratore = offerta.PERSONA.CONTO_CORRENTE.CONTO_CORRENTE_MONETA.Count(m => m.STATO == (int)StatoMoneta.SOSPESA)
                    };*/
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Conclusi(int pagina = 1)
        {
            List<AnnuncioViewModel> acquistiConclusi = new List<AnnuncioViewModel>();
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    int utente = ((PersonaModel)Session["utente"]).Persona.ID;
                    var query = db.ANNUNCIO.Where(item => item.ID_PERSONA != utente && item.ID_COMPRATORE == utente 
                        && item.TRANSAZIONE_ANNUNCIO.Count(m => m.STATO == (int)StatoPagamento.ATTIVO || m.STATO == (int)StatoPagamento.ACCETTATO) > 0
                        && (item.STATO == (int)StatoVendita.VENDUTO || item.STATO == (int)StatoVendita.ELIMINATO || item.STATO == (int)StatoVendita.BARATTATO)
                        && (item.ID_OGGETTO != null || item.ID_SERVIZIO != null));
                    int numeroElementi = Convert.ToInt32(WebConfigurationManager.AppSettings["numeroElementi"]);
                    ViewData["TotalePagine"] = (int)Math.Ceiling((decimal)query.Count() / (decimal)numeroElementi);
                    ViewData["Pagina"] = pagina;
                    pagina -= 1;
                    string randomString = Utils.RandomString(3);
                    List<ANNUNCIO> lista = query
                        .OrderByDescending(item => item.DATA_VENDITA)
                        .Skip(pagina * numeroElementi)
                        .Take(numeroElementi).ToList();
                    foreach (ANNUNCIO m in lista)
                    {
                        AnnuncioModel annuncioModel = new AnnuncioModel();
                        AnnuncioViewModel viewModel = annuncioModel.GetViewModel(db, m);
                        OFFERTA offerta = m.OFFERTA.SingleOrDefault(o => o.STATO == (int)StatoOfferta.ACCETTATA);
                        if (offerta != null)
                            viewModel.Offerta = new OffertaViewModel(db, offerta);
                        acquistiConclusi.Add(viewModel);
                    }
                        
                    if (acquistiConclusi.Count > 0)
                        RefreshPunteggioUtente(db);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View(acquistiConclusi);
        }
        
        [HttpGet]
        public ActionResult Vorrei(int pagina = 1)
        {
            if (pagina == 1)
                pagina = 0;
            ViewData["Pagina"] = pagina;
            List<AnnuncioViewModel> model = new List<AnnuncioViewModel>();
            using (DatabaseContext db = new DatabaseContext())
            {
                int utente = (Session["utente"] as PersonaModel).Persona.ID;

                int numeroAnnunci = db.ANNUNCIO_DESIDERATO.Count(item => item.ID_PERSONA == utente);
                int numeroElementi = Convert.ToInt32(WebConfigurationManager.AppSettings["numeroElementi"]);
                ViewData["TotalePagine"] = (int)Math.Ceiling((decimal)numeroAnnunci / (decimal)numeroElementi);

                db.ANNUNCIO_DESIDERATO
                    .OrderByDescending(item => item.ANNUNCIO.DATA_INSERIMENTO)
                    .Skip(pagina * numeroElementi)
                    .Take(numeroElementi)
                    .Where(item => item.ID_PERSONA == utente && (item.ANNUNCIO.STATO == (int)StatoVendita.INATTIVO 
                        || item.ANNUNCIO.STATO == (int)StatoVendita.ATTIVO) && (item.ANNUNCIO.DATA_FINE == null ||
                        item.ANNUNCIO.DATA_FINE >= DateTime.Now))
                    .ToList().ForEach(m => model.Add(
                            new AnnuncioViewModel(db, m.ANNUNCIO)
                        )
                    );
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Suggeriti(int pagina = 1)
        {
            //List<ProposteAnnuncio> viewModel = new List<ProposteAnnuncio>();
            List<OffertaViewModel> viewModel = new List<OffertaViewModel>();
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    db.Database.Connection.Open();
                    PersonaModel utente = (PersonaModel)Session["utente"];
                    int idUtente = utente.Persona.ID;
                    int contoAttuale = utente.Punti;
                    /*
                    var query = db.ANNUNCIO_DESIDERATO.Where(m => m.ID_PERSONA == idUtente && // sugli annunci che desidero
                            (m.ANNUNCIO.PUNTI <= contoAttuale || // gli annunci che mi posso permettere
                                m.PERSONA.ANNUNCIO.Count(a => a.ANNUNCIO_DESIDERATO.Count(ad => ad.ID_PERSONA == m.ANNUNCIO.ID_PERSONA) > 0) > 0)); // o di cui desiderano almeno un mio annuncio
                    */
                    var query = db.ANNUNCIO_DESIDERATO.Where(m => m.ID_PERSONA == idUtente && (// sugli annunci che desidero
                                    (m.ANNUNCIO.STATO != (int)StatoVendita.ELIMINATO && m.ANNUNCIO.STATO != (int)StatoVendita.BARATTATO && m.ANNUNCIO.STATO != (int)StatoVendita.VENDUTO) &&
                                    m.PERSONA.CONTO_CORRENTE.CONTO_CORRENTE_MONETA.Count() >= m.ANNUNCIO.PUNTI || // se ho abbastanza crediti
                                    db.ANNUNCIO_DESIDERATO.Count(ad => ad.ID_PERSONA == m.ANNUNCIO.ID_PERSONA && ad.ANNUNCIO.ID_PERSONA == idUtente && // o di cui desiderano almeno un mio annuncio
                                        (ad.ANNUNCIO.STATO != (int)StatoVendita.ELIMINATO && ad.ANNUNCIO.STATO != (int)StatoVendita.BARATTATO && ad.ANNUNCIO.STATO != (int)StatoVendita.VENDUTO)
                                    ) > 0)
                                );
                    int numeroElementi = Convert.ToInt32(WebConfigurationManager.AppSettings["numeroElementi"]);
                    ViewData["TotalePagine"] = (int)Math.Ceiling((decimal)query.Count() / (decimal)numeroElementi);
                    ViewData["Pagina"] = pagina;
                    pagina -= 1;
                    /*
                    query.OrderByDescending(item => item.ANNUNCIO.DATA_INSERIMENTO).OrderByDescending(item => item.ID) // ordinato per inserimento annuncio e poi per inserimento desiderio
                        .OrderByDescending(item => item.PERSONA.ANNUNCIO.Count(a => a.ANNUNCIO_DESIDERATO.Count(ad => ad.ID_PERSONA == item.ANNUNCIO.ID_PERSONA) > 0)) // ordinato per numeri annunci desiderati da altri
                    */
                    query.OrderByDescending(item => item.ID) // ordinato per inserimento annuncio e poi per inserimento desiderio
                        .OrderByDescending(item => db.ANNUNCIO_DESIDERATO.Count(ad => ad.ID_PERSONA == item.ANNUNCIO.ID_PERSONA && ad.ANNUNCIO.ID_PERSONA == idUtente && // o di cui desiderano almeno un mio annuncio
                                        (ad.ANNUNCIO.STATO != (int)StatoVendita.ELIMINATO && ad.ANNUNCIO.STATO != (int)StatoVendita.BARATTATO && ad.ANNUNCIO.STATO != (int)StatoVendita.VENDUTO)
                                    )) // ordinato per numeri annunci desiderati da altri
                        .Skip(pagina * numeroElementi)
                        .Take(numeroElementi)
                        .ToList().ForEach(m =>
                        {
                            /*ProposteAnnuncio proposta = new ProposteAnnuncio(db, m.ANNUNCIO);
                            db.ANNUNCIO.Where(a => a.ID_PERSONA == idUtente && (a.ANNUNCIO_DESIDERATO.Count(ad => ad.ID_PERSONA == m.ANNUNCIO.ID_PERSONA) > 0)).ToList().ForEach(a =>
                            {
                                proposta.AnnunciDesiderati.Add(new AnnuncioViewModel(db, a));
                            });

                            viewModel.Add(proposta);*/
                            OffertaViewModel offerta = new OffertaViewModel();
                            offerta.Annuncio = new AnnuncioViewModel(db, m.ANNUNCIO);
                            db.ANNUNCIO.Where(a => a.ID_PERSONA == idUtente && (a.ANNUNCIO_DESIDERATO.Count(ad => ad.ID_PERSONA == m.ANNUNCIO.ID_PERSONA) > 0)).ToList().ForEach(a =>
                            {
                                offerta.Baratti.Add(new AnnuncioViewModel(db, a));
                            });

                            viewModel.Add(offerta);
                        });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Futuri(int pagina = 1)
        {
            //List<ProposteAnnuncio> viewModel = new List<ProposteAnnuncio>();
            //try
            //{
            //    using (DatabaseContext db = new DatabaseContext())
            //    {
            //        db.Database.Connection.Open();
            //        PersonaModel utente = (PersonaModel)Session["utente"];
            //        int idUtente = utente.Persona.ID;
            //        int contoAttuale = utente.Punti;

            //        var query = db.ANNUNCIO_DESIDERATO.Where(m => m.ID_PERSONA == idUtente && // sugli annunci che desidero
            //                (m.ANNUNCIO.PUNTI >= contoAttuale && // gli annunci che non mi posso permettere
            //                    // verificare come calcolare le persone a cui non interessa manco un mio annuncio!!!!
            //                    m.PERSONA.ANNUNCIO.Count(a => a.ANNUNCIO_DESIDERATO.Count(ad => ad.ID_PERSONA == m.ANNUNCIO.ID_PERSONA) <= 0) > 0)); // e di cui non desiderano nemmeno un mio annuncio
            //        int numeroElementi = Convert.ToInt32(WebConfigurationManager.AppSettings["numeroElementi"]);
            //        ViewData["TotalePagine"] = (int)Math.Ceiling((decimal)query.Count() / (decimal)numeroElementi);
            //        ViewData["Pagina"] = pagina;
            //        pagina -= 1;
            //        query.OrderByDescending(item => item.ANNUNCIO.DATA_INSERIMENTO).OrderByDescending(item => item.ID) // ordinato per inserimento annuncio e poi per inserimento desiderio
            //            .Skip(pagina * numeroElementi)
            //            .Take(numeroElementi)
            //            .ToList().ForEach(m =>
            //            {
            //                ProposteAnnuncio proposta = new ProposteAnnuncio(db, m.ANNUNCIO);
            //                db.ANNUNCIO.Where(a => a.ID_PERSONA == idUtente && (a.ANNUNCIO_DESIDERATO.Count(ad => ad.ID_PERSONA == m.ANNUNCIO.ID_PERSONA) <= 0)).ToList().ForEach(a =>
            //                {
            //                    proposta.AnnunciDesiderati.Add(new AnnuncioViewModel(db, a));
            //                });

            //                viewModel.Add(proposta);
            //            });
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            //}
            //return View(viewModel);
            return View();
        }

        #region METODI PRIVATI
        private List<OffertaViewModel> GetListaOfferte(DatabaseContext db, int utente, int pagina)
        {
            List<OffertaViewModel> offerte = new List<OffertaViewModel>();
            var query = db.OFFERTA.Where(item => item.PERSONA.ID == utente && item.PERSONA.STATO == (int)Stato.ATTIVO
                        //&& (item.STATO != (int)StatoOfferta.ACCETTATA && item.STATO != (int)StatoOfferta.ANNULLATA)
                        && (
                            item.STATO != (int)StatoOfferta.ANNULLATA
                            && (item.ANNUNCIO.STATO != (int)StatoVendita.BARATTATO && item.ANNUNCIO.STATO != (int)StatoVendita.ELIMINATO
                            && item.ANNUNCIO.STATO != (int)StatoVendita.INATTIVO && item.ANNUNCIO.STATO != (int)StatoVendita.VENDUTO)
                        )
                        && (item.ANNUNCIO.ID_OGGETTO != null || item.ANNUNCIO.ID_SERVIZIO != null));
            int numeroElementi = Convert.ToInt32(WebConfigurationManager.AppSettings["numeroElementi"]);
            ViewData["TotalePagine"] = (int)Math.Ceiling((decimal)query.Count() / (decimal)numeroElementi);
            ViewData["Pagina"] = pagina;
            pagina -= 1;
            string randomString = Utils.RandomString(3);
            List<OFFERTA> lista = query
                .OrderByDescending(item => item.DATA_INSERIMENTO)
                .Skip(pagina * numeroElementi)
                .Take(numeroElementi).ToList();

            foreach (OFFERTA item in lista)
            {
                OffertaViewModel offertaEffettuata = new OffertaViewModel(db, item);
                offerte.Add(offertaEffettuata);
            }
            if (offerte.Count > 0)
                RefreshPunteggioUtente(db);

            return offerte;
        }
        #endregion

    }
}