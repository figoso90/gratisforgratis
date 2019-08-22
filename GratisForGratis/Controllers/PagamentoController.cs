using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GratisForGratis.Models;
using GratisForGratis.Filters;
using DotNetShipping;
using GratisForGratis.App_GlobalResources;
using System.Web.Configuration;

namespace GratisForGratis.Controllers
{
    [Authorize]
    public class PagamentoController : AdvancedController
    {
        // GET: Pagamento
        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult Index(BonificoViewModel viewModel)
        //{
        //    PersonaModel sessioneUtente = Session["utente"] as PersonaModel;

        //    try
        //    {
        //        using (DatabaseContext db = new DatabaseContext())
        //        {
        //            // se esiste una sessione pagamento aperta e la mail ricevente dei soldi corrisponde a quella della login, 
        //            // allora blocco la login e scatta l'errore
        //            CONTO_CORRENTE conto = db.CONTO_CORRENTE.Include("Persona").Include("Attivita").SingleOrDefault(m => m.TOKEN.ToString() == viewModel.Destinatario);

        //            if (conto == null)
        //            {
        //                ModelState.AddModelError("Error", string.Format(Language.ErrorPaymentUser, viewModel.Destinatario));
        //                return View(viewModel);
        //            }

        //            ContoCorrenteMonetaModel model = new ContoCorrenteMonetaModel();
        //            TRANSAZIONE pagamento = model.Pay(db, sessioneUtente.Persona.ID_CONTO_CORRENTE, conto.ID, viewModel.DescrizionePagamento, viewModel.TipoTransazione, (int)viewModel.Punti);
                    
        //            this.RefreshPunteggioUtente(db);

        //            // impostare invio email pagamento effettuato
        //            try {
        //                EmailModel email = new EmailModel(ControllerContext);
        //                string indirizzoMail = string.Empty;
        //                string nominativo = string.Empty;
        //                // verifico se il destinatario è una persona o attività
        //                if (conto.PERSONA.Count > 0)
        //                {
        //                    PERSONA persona = conto.PERSONA.SingleOrDefault(p => p.ID_CONTO_CORRENTE == conto.ID);
        //                    if (persona != null)
        //                    {
        //                        indirizzoMail = db.PERSONA_EMAIL
        //                            .SingleOrDefault(m => m.ID_PERSONA == persona.ID &&
        //                                m.TIPO == (int)TipoEmail.Registrazione).EMAIL;
        //                    }
        //                } else
        //                {
        //                    ATTIVITA attivita = conto.ATTIVITA.SingleOrDefault(p => p.ID_CONTO_CORRENTE == conto.ID);
        //                    if (attivita != null)
        //                    {
        //                        indirizzoMail = db.ATTIVITA_EMAIL
        //                            .SingleOrDefault(m => m.ID_ATTIVITA == attivita.ID &&
        //                                m.TIPO == (int)TipoEmail.Registrazione).EMAIL;
        //                    }
        //                }
        //                // se ho trovato l'indirizzo mail
        //                if (!string.IsNullOrWhiteSpace(indirizzoMail))
        //                {
        //                    email.To.Add(new System.Net.Mail.MailAddress(indirizzoMail, nominativo));
        //                    email.Subject = string.Format(Email.PaymentFromPartnersSubject, pagamento.NOME, (Session["utente"] as PersonaModel).Persona.NOME + ' ' + (Session["utente"] as PersonaModel).Persona.COGNOME, nominativo) + " - " + WebConfigurationManager.AppSettings["nomeSito"];
        //                    email.Body = "PagamentoDaPartners";
        //                    email.DatiEmail = new SchedaPagamentoViewModel()
        //                    {
        //                        Nome = pagamento.NOME,
        //                        Compratore = (Session["utente"] as PersonaModel).Persona.NOME + ' ' + (Session["utente"] as PersonaModel).Persona.COGNOME,
        //                        Venditore = pagamento.CONTO_CORRENTE1.PERSONA.Select(item => item.NOME + ' ' + item.COGNOME).SingleOrDefault(),
        //                        Punti = (int)pagamento.PUNTI,
        //                        //Soldi = (int)pagamento.SOLDI,
        //                        Data = pagamento.DATA_INSERIMENTO,
        //                        Portale = nominativo,
        //                    };
        //                    new EmailController().SendEmail(email);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        //            }
        //            //RemoveSessionPayment();
        //            if (!String.IsNullOrEmpty(viewModel.UrlOk))
        //            {
        //                return Redirect(Request.Url.PathAndQuery);
        //            }
        //            else if (!String.IsNullOrEmpty(viewModel.UrlKo))
        //            {
        //                return Redirect(viewModel.UrlKo);
        //            }
        //            ViewData["messaggio"] = Language.TransactionOK;
        //            return View(new BonificoViewModel());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        //        ModelState.AddModelError("Error", ex);
        //    }

        //    return View(viewModel);
        //}

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Test()
        {
            System.Collections.Specialized.NameValueCollection appSettings = System.Configuration.ConfigurationManager.AppSettings;

            // You will need a license #, userid and password to utilize the UPS provider.
            string upsLicenseNumber = appSettings["UPSLicenseNumber"];
            string upsUserId = appSettings["UPSUserId"];
            string upsPassword = appSettings["UPSPassword"];

            // You will need an account # and meter # to utilize the FedEx provider.
            string fedexKey = appSettings["FedExKey"];
            string fedexPassword = appSettings["FedExPassword"];
            string fedexAccountNumber = appSettings["FedExAccountNumber"];
            string fedexMeterNumber = appSettings["FedExMeterNumber"];

            // You will need a userId to use the USPS provider. Your account will also need access to the production servers.
            string uspsUserId = appSettings["USPSUserId"];

            // Setup package and destination/origin addresses
            var packages = new List<Package>();
            packages.Add(new Package(12, 12, 12, 35, 150));
            packages.Add(new Package(4, 4, 6, 15, 250));

            var origin = new Address("", "", "06405", "US");
            var destination = new Address("", "", "20852", "US"); // US Address

            // Create RateManager
            var rateManager = new RateManager();

            // Add desired DotNetShippingProviders
            rateManager.AddProvider(new DotNetShipping.ShippingProviders.UPSProvider(upsLicenseNumber, upsUserId, upsPassword));
            //rateManager.AddProvider(new FedExProvider(fedexKey, fedexPassword, fedexAccountNumber, fedexMeterNumber));
            //rateManager.AddProvider(new USPSProvider(uspsUserId));

            // (Optional) Add RateAdjusters
            rateManager.AddRateAdjuster(new PercentageRateAdjuster(.9M));

            // Call GetRates()
            Shipment shipment = rateManager.GetRates(origin, destination, packages);

            // Iterate through the rates returned
            foreach (Rate rate in shipment.Rates)
            {
                Console.WriteLine(rate);
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult TestPayment()
        {
            Session["testPaymentFromLocalHost"] = true;
            if (Session["pagamento"] != null)
                return View(new PagamentoViewModel((PagamentoAbstractModel)Session["pagamento"]));

            return View();
        }

        [HttpGet]
        public ActionResult Payment()
        {
            if (Session["pagamento"] == null)
                return RedirectToAction("Index", "Home");

            return View(new PagamentoViewModel((PagamentoAbstractModel)Session["pagamento"]));
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Payment(PagamentoViewModel model)
        {
            model.WebSite = Request.UrlReferrer.Host;
            model.UrlRequest = Request.UrlReferrer.OriginalString;
            try
            {
                // verifico se non è un test locale
                if (Session["testPaymentFromLocalHost"] != null)
                {
                    model.WebSite = "http://www.gratisforgratis.com";
                    model.UrlRequest = "/Home/Index";
                    model.Token = System.Configuration.ConfigurationManager.AppSettings["test"];
                    model.Test = 1;
                }

                using (DatabaseContext db = new DatabaseContext())
                {
                    // verifica autenticità del portale web tramite dominio e token
                    ATTIVITA sito = db.ATTIVITA.FirstOrDefault(m => m.DOMINIO == model.WebSite && m.TOKEN == new Guid(model.Token));
                    if (sito != null)
                    {
                        SalvataggioPagamentoViewModel salvataggioPagamento = new SalvataggioPagamentoViewModel(model);

                        salvataggioPagamento.PortaleWebID = sito.ID_CONTO_CORRENTE;
                        // verifica se l'utente è registrato
                        PERSONA utentePagato = db.PERSONA.FirstOrDefault(m => m.PERSONA_EMAIL.SingleOrDefault(item => item.TIPO == (int)TipoEmail.Registrazione).EMAIL == model.EmailReceivent);
                        if (utentePagato != null)
                        {
                            salvataggioPagamento.UtentePagatoID = utentePagato.ID_CONTO_CORRENTE;
                            Session["pagamento"] = salvataggioPagamento;
                        }
                        else
                        {
                            ModelState.AddModelError("Error", Language.ErrorNotUser);
                            if (!String.IsNullOrEmpty(model.ReturnUrlForFailed))
                                return Redirect(model.ReturnUrlForFailed);
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("Error", Language.ErrorNotPartner);
                        if (!String.IsNullOrEmpty(model.ReturnUrlForFailed))
                            return Redirect(model.ReturnUrlForFailed);
                    }
                }
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                ModelState.AddModelError("Error", ex.Message);
            }
            
            return View(model);
        }

        // pagamento da sito partners
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePayment()
        {
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    SalvataggioPagamentoViewModel datiPagamento = (SalvataggioPagamentoViewModel)Session["pagamento"];
                    if (!ModelState.IsValid)
                    {
                        return View("Payment", datiPagamento);
                    }

                    // se esiste una sessione pagamento aperta e la mail ricevente dei soldi corrisponde a quella della login, 
                    // allora blocco la login e scatta l'errore

                    if (Session["pagamento"] != null && datiPagamento.EmailReceivent.Equals(((PersonaModel)Session["utente"]).Email.FirstOrDefault(item => item.TIPO == (int)TipoEmail.Registrazione)))
                    {
                        ModelState.AddModelError("Error", string.Format(App_GlobalResources.Language.ErrorPaymentUser,datiPagamento.EmailReceivent));
                        return View("Payment", datiPagamento);
                    }

                    // effettuare il salvataggio
                    TRANSAZIONE pagamento = new TRANSAZIONE();
                    pagamento.NOME = datiPagamento.DescriptionPayment;
                    //pagamento.ID_ATTIVITA = datiPagamento.PortaleWebID;
                    pagamento.SOLDI = (int)datiPagamento.TotalPrice;
                    pagamento.TIPO = datiPagamento.TypePayment;
                    pagamento.ID_CONTO_DESTINATARIO = (datiPagamento.PortaleWebID != null)? ((ATTIVITA)Session["utente"]).ID_CONTO_CORRENTE : ((PersonaModel)Session["utente"]).Persona.ID_CONTO_CORRENTE;
                    pagamento.ID_CONTO_MITTENTE = datiPagamento.UtentePagatoID;
                    pagamento.TEST = datiPagamento.Test;

                    db.TRANSAZIONE.Add(pagamento);
                    db.SaveChanges();
                    // impostare invio email pagamento effettuato
                    EmailModel email = new EmailModel(ControllerContext);
                    email.To.Add(new System.Net.Mail.MailAddress(datiPagamento.EmailReceivent, pagamento.CONTO_CORRENTE1.PERSONA.Select(item => item.NOME + ' ' + item.COGNOME).SingleOrDefault()));
                    email.Subject = string.Format(Email.PaymentFromPartnersSubject,pagamento.NOME, (Session["utente"] as PersonaModel).Persona.NOME + ' ' + (Session["utente"] as PersonaModel).Persona.COGNOME, datiPagamento.Nominativo) + " - " + WebConfigurationManager.AppSettings["nomeSito"];
                    email.Body = "PagamentoDaPartners";
                    email.DatiEmail = new SchedaPagamentoViewModel()
                    {
                        Nome = pagamento.NOME,
                        Compratore = (Session["utente"] as PersonaModel).Persona.NOME + ' ' + (Session["utente"] as PersonaModel).Persona.COGNOME,
                        Venditore = pagamento.CONTO_CORRENTE1.PERSONA.Select(item => item.NOME + ' ' + item.COGNOME).SingleOrDefault(),
                        Punti = (int)pagamento.PUNTI,
                        Soldi = (int)pagamento.SOLDI,
                        Data = pagamento.DATA_INSERIMENTO,
                        Portale = datiPagamento.Nominativo,
                    };
                    new EmailController().SendEmail(email);
                    RemoveSessionPayment();
                    if (String.IsNullOrEmpty(datiPagamento.ReturnUrlForSuccess))
                    {
                        return Redirect(datiPagamento.UrlRequest);
                    }
                    else
                    {
                        return Redirect(datiPagamento.ReturnUrlForSuccess);
                    }
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                LoggatoreModel.Errore(ex);
                ModelState.AddModelError("Error", ex);
            }
                
            return View("Payment", new PagamentoViewModel((PagamentoAbstractModel)Session["pagamento"]));
        }

        [HttpPost]
        public ActionResult SavePayment(SchedaPagamentoViewModel scheda)
        {
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    db.TRANSAZIONE.Add(new TRANSAZIONE() {
                        NOME = scheda.Nome,
                        SOLDI = scheda.Soldi,
                        TIPO = 0,
                        ID_CONTO_DESTINATARIO = ((PersonaModel)Session["utente"]).Persona.ID_CONTO_CORRENTE,
                        ID_CONTO_MITTENTE = db.PERSONA.SingleOrDefault(item => item.ID == scheda.VenditoreId).ID_CONTO_CORRENTE,
                    });
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                LoggatoreModel.Errore(ex);
            }
            return View();
        }

        // pagamento dal portale
        [HttpPost]
        [ValidateAjax]
        public ActionResult Definitivo_OLD(List<string> acquisti)
        {
            try
            {
                foreach (string acquisto in acquisti)
                {
                    //string token = acquisto.Trim().Substring(3, acquisto.Trim().Length - 6);
                    int idOfferta = Utils.DecodeToInt(acquisto.Trim().Substring(3, acquisto.Trim().Length - 6));
                    using (DatabaseContext db = new DatabaseContext())
                    {
                        PersonaModel utente = Session["utente"] as PersonaModel;
                        System.Data.Entity.Core.Objects.ObjectParameter errore = new System.Data.Entity.Core.Objects.ObjectParameter("Errore", typeof(ErrorePagamento));
                        errore.Value = ErrorePagamento.Nessuno;
                        Guid portaleWeb = Guid.Parse(System.Configuration.ConfigurationManager.AppSettings["portaleweb"]);
                        
                        // DEVO CAMBIARE E FARMI TORNARE IL TRANSAZIONE EFFETTUATO
                        int? idPagamento = db.BENE_SAVE_PAGAMENTO(idOfferta, utente.Persona.ID_CONTO_CORRENTE, errore).FirstOrDefault();
                        if ((ErrorePagamento)errore.Value != ErrorePagamento.Nessuno)
                            return Json(errore.Value.ToString());
                        if (idPagamento == null)
                            return Json(Language.ErrorPayment);

                        TRANSAZIONE pagamento = db.TRANSAZIONE.Include("CONTO_CORRENTE.PERSONA.PERSONA_EMAIL").Where(p => p.ID == idPagamento).SingleOrDefault();
                        // aggiorno punti attuali utente
                        Session["utente"] = new PersonaModel(db.PERSONA.Where(u => u.ID == utente.Persona.ID).FirstOrDefault());
                        PERSONA venditore = pagamento.CONTO_CORRENTE.PERSONA.SingleOrDefault();
                        // impostare invio email pagamento effettuato
                        EmailModel email = new EmailModel(ControllerContext);
                        email.To.Add(new System.Net.Mail.MailAddress(venditore.PERSONA_EMAIL.SingleOrDefault(e => e.TIPO == (int)TipoEmail.Registrazione).EMAIL));
                        string nominativo = (Session["utente"] as PersonaModel).Persona.NOME + " " + (Session["utente"] as PersonaModel).Persona.COGNOME;
                        email.Subject = String.Format(Email.PaymentSubject, pagamento.NOME, nominativo)  + " - " + WebConfigurationManager.AppSettings["nomeSito"];
                        email.Body = "Pagamento";
                        email.DatiEmail = new SchedaPagamentoViewModel()
                        {
                            Nome = pagamento.NOME,
                            Compratore = nominativo,
                            Venditore = venditore.NOME + " " + venditore.COGNOME,
                            Punti = (int)pagamento.PUNTI,
                            Soldi = (int)pagamento.SOLDI,
                            Data = pagamento.DATA_INSERIMENTO,
                        };
                        new EmailController().SendEmail(email);
                        
                        return Json(new { Messaggio = Language.CompletedPurchase });
                    }
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                LoggatoreModel.Errore(ex);
                // log errore, invio mail
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                return Json(ex.ToString());
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return Json(Language.ErrorPayment);
        }

        // pagamento dal portale
        [HttpPost]
        [ValidateAjax]
        public ActionResult Definitivo(List<string> acquisti)
        {

            using (DatabaseContext db = new DatabaseContext())
            {
                try
                {
                    db.Database.Connection.Open();
                    foreach (string acquisto in acquisti)
                    {
                        PagamentoModel model = new PagamentoModel();
                        PersonaModel utente = Session["utente"] as PersonaModel;
                        int? idPagamento = model.eseguiFromStoredProcedure(acquisto, utente);
                        if (idPagamento != null)
                        {
                            TRANSAZIONE pagamento = db.TRANSAZIONE.Include("CONTO_CORRENTE.PERSONA.PERSONA_EMAIL").Where(p => p.ID == idPagamento).SingleOrDefault();
                            // aggiorno punti attuali utente
                            Session["utente"] = new PersonaModel(db.PERSONA.Where(u => u.ID == utente.Persona.ID).FirstOrDefault());
                            model.SendEmail(ControllerContext, pagamento, utente);
                            return Json(new { Messaggio = Language.CompletedPurchase });
                        }
                        else
                        {
                            return Json(model.Errore.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    LoggatoreModel.Errore(ex);
                    // log errore, invio mail
                    Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                    return Json(ex.ToString());
                }
                finally
                {
                    if (db.Database.Connection.State != System.Data.ConnectionState.Closed)
                        db.Database.Connection.Close();
                }
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return Json(Language.ErrorPayment);
        }

        private Shipment CreateSpedizione(System.Collections.Specialized.NameValueCollection datiAccesso)
        {
            // You will need a license #, userid and password to utilize the UPS provider.
            string upsLicenseNumber = datiAccesso["UPSLicenseNumber"];
            string upsUserId = datiAccesso["UPSUserId"];
            string upsPassword = datiAccesso["UPSPassword"];

            // You will need an account # and meter # to utilize the FedEx provider.
            string fedexKey = datiAccesso["FedExKey"];
            string fedexPassword = datiAccesso["FedExPassword"];
            string fedexAccountNumber = datiAccesso["FedExAccountNumber"];
            string fedexMeterNumber = datiAccesso["FedExMeterNumber"];

            // You will need a userId to use the USPS provider. Your account will also need access to the production servers.
            string uspsUserId = datiAccesso["USPSUserId"];

            // Setup package and destination/origin addresses
            var packages = new List<Package>();
            packages.Add(new Package(12, 12, 12, 35, 150));
            //packages.Add(new Package(4, 4, 6, 15, 250));

            var origin = new Address("", "", "20026", "IT");
            var destination = new Address("", "", "20024", "IT"); // US Address

            // Create RateManager
            var rateManager = new RateManager();

            // Add desired DotNetShippingProviders
            rateManager.AddProvider(new DotNetShipping.ShippingProviders.UPSProvider(upsLicenseNumber, upsUserId, upsPassword));
            //rateManager.AddProvider(new FedExProvider(fedexKey, fedexPassword, fedexAccountNumber, fedexMeterNumber));
            //rateManager.AddProvider(new USPSProvider(uspsUserId));

            // (Optional) Add RateAdjusters
            //rateManager.AddRateAdjuster(new PercentageRateAdjuster(.9M));

            // Call GetRates()
            return rateManager.GetRates(origin, destination, packages);
        }

        private void RemoveSessionPayment()
        {
            Session.Remove("testPaymentFromLocalHost");
            Session.Remove("pagamento");
        }
    }
}