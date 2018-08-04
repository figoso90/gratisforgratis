using GratisForGratis.Models;
using GratisForGratis.Models.ViewModels;
using GratisForGratis.Utilities.PayPal;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GratisForGratis.Controllers
{
    public class PayPalController : AdvancedController
    {
        public ActionResult Test()
        {
            APIContext apiContext = Configuration.GetAPIContext();
            PayPalIndexViewModel viewModel = new PayPalIndexViewModel();
            viewModel.Azione = AzionePayPal.Acquisto;
            string urlCancel = GetUrlCancel(viewModel.Azione, viewModel.Token);
            viewModel.Guid = Convert.ToString((new Random()).Next(100000));
            System.Web.Routing.RouteValueDictionary data = new System.Web.Routing.RouteValueDictionary(viewModel);
            string url = Url.Action("Payment", "PayPal", data, this.Request.Url.Scheme, this.Request.Url.Host);

            List<Transaction> lista = new List<Transaction>();
            //similar to credit card create itemlist and add item objects to it
            var itemList = new ItemList() { items = new List<Item>() };
            TIPO_VALUTA tipoValuta = (HttpContext.Application["tipoValuta"] as List<TIPO_VALUTA>).SingleOrDefault(m => m.SIMBOLO == NumberFormatInfo.CurrentInfo.CurrencySymbol);
            
            itemList.items.Add(new Item()
            {
                name = "Nome test",
                currency = tipoValuta.CODICE,
                price = ConvertDecimalToString(11),
                quantity = "1",
                sku = "sku"
            });

            itemList.items.Add(new Item()
            {
                name = string.Concat(App_GlobalResources.Language.Shipment, " test"),
                currency = tipoValuta.CODICE,
                price = ConvertDecimalToString(11),
                //price = ConvertDecimalToString(new Decimal(1006.5)), // prova fissa verifica conversione
                quantity = "1",
                sku = "sku"
            });

            decimal subtotal = itemList.items.Sum(m => ConvertStringToDecimal(m.price));

            // similar as we did for credit card, do here and create details object
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = ConvertDecimalToString(subtotal)
            };

            // similar as we did for credit card, do here and create amount object
            var amount = new Amount()
            {
                currency = tipoValuta.CODICE,
                total = ConvertDecimalToString(subtotal), // Total must be equal to sum of shipping, tax and subtotal.
                details = details
            };

            Transaction transazione = new Transaction();

            transazione.description = "Test descrizione";
            transazione.invoice_number = "testami";
            transazione.amount = amount;
            transazione.item_list = itemList;

            lista.Add(transazione);

            var createdPayment = this.CreatePayment(apiContext, url, urlCancel, lista);

            var links = createdPayment.links.GetEnumerator();

            string paypalRedirectUrl = null;

            while (links.MoveNext())
            {
                Links lnk = links.Current;

                if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                {
                    //saving the payapalredirect URL to which user will be redirected for payment
                    paypalRedirectUrl = lnk.href;
                }
            }

            return Redirect(paypalRedirectUrl);
        }
        // GET: PayPal
        public ActionResult Payment(PayPalIndexViewModel viewModel)
        {
            //getting the apiContext as earlier
            APIContext apiContext = Configuration.GetAPIContext();
            string urlCancel = GetUrlCancel(viewModel.Azione, viewModel.Token);

            if ((Session["PayPalCompra"] != null && Session["PayPalAnnuncio"] != null )|| Session["PayPalOfferta"] != null)
            {
                try
                {
                    bool enablePagamento = CanPagamento(viewModel.Azione);
                    if (string.IsNullOrEmpty(viewModel.PayerID) && string.IsNullOrWhiteSpace(viewModel.Guid) && enablePagamento)
                    {
                        //this section will be executed first because PayerID doesn't exist
                        //it is returned by the create function call of the payment class

                        // Creating a payment
                        // baseURL is the url on which paypal sendsback the data.
                        // So we have provided URL of this controller only
                        viewModel.Guid = Convert.ToString((new Random()).Next(100000));
                        System.Web.Routing.RouteValueDictionary data = new System.Web.Routing.RouteValueDictionary(viewModel);
                        string url = Url.Action("Payment", "PayPal", data, this.Request.Url.Scheme, this.Request.Url.Host);

                        //CreatePayment function gives us the payment approval url
                        //on which payer is redirected for paypal account payment
                        var createdPayment = this.CreatePayment(apiContext, url, urlCancel, GetListTransaction(viewModel.Azione, viewModel.Guid));

                        //get links returned from paypal in response to Create function call

                        var links = createdPayment.links.GetEnumerator();

                        string paypalRedirectUrl = null;

                        while (links.MoveNext())
                        {
                            Links lnk = links.Current;

                            if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                            {
                                //saving the payapalredirect URL to which user will be redirected for payment
                                paypalRedirectUrl = lnk.href;
                            }
                        }

                        // saving the paymentID in the key guid
                        Session.Add(viewModel.Guid, createdPayment.id);

                        return Redirect(paypalRedirectUrl);
                    }
                    else if(!string.IsNullOrEmpty(viewModel.PayerID) || !string.IsNullOrWhiteSpace(viewModel.Guid))
                    {
                        // This section is executed when we have received all the payments parameters

                        // from the previous call to the function Create

                        // Executing a payment
                        var executedPayment = ExecutePayment(apiContext, viewModel.PayerID, Session[viewModel.Guid] as string);
                        if (executedPayment.state.ToLower() != "approved")
                        {
                            // redirect errore nell'acquisto
                            TempData["errore"] = "Errore durante l'acquisto";
                        }
                        else
                        {
                            if (SavePayment(viewModel, executedPayment))
                            {
                                TempData["Esito"] = App_GlobalResources.Language.JsonBuyAd;
                                TempData["pagamentoEffettuato"] = true;
                                return Redirect(_UrlFinePagamento);
                            }
                            else
                            {
                                TempData["errore"] = App_GlobalResources.ErrorResource.AdBuyFailed;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Logger.log("Error" + ex.Message);
                    // redirect errore nell'acquisto
                    TempData["errore"] = "Errore grave durante l'acquisto";
                }
                // mettere qui l'annullo dell'acquisto in modo da tornare indietro
                AnnullaPagamento(viewModel.Azione);
            }
            else
            {
                TempData["errore"] = "Sessione pagamento scaduta!";
            }
            return Redirect(urlCancel);
        }
        
        public ActionResult PaymentWithCreditCard(PayPalIndexViewModel viewModel)
        {
            //////create and item for which you are taking payment
            //////if you need to add more items in the list
            //////Then you will need to create multiple item objects or use some loop to instantiate object
            ////Item item = new Item();
            ////item.name = "Demo Item";
            ////item.currency = "USD";
            ////item.price = "5";
            ////item.quantity = "1";
            ////item.sku = "sku";

            //////Now make a List of Item and add the above item to it
            //////you can create as many items as you want and add to this list
            ////List<Item> itms = new List<Item>();
            ////itms.Add(item);
            ////ItemList itemList = new ItemList();
            ////itemList.items = itms;

            //////Address for the payment
            ////Address billingAddress = new Address();
            ////billingAddress.city = "NewYork";
            ////billingAddress.country_code = "US";
            ////billingAddress.line1 = "23rd street kew gardens";
            ////billingAddress.postal_code = "43210";
            ////billingAddress.state = "NY";

            string urlCancel = GetUrlCancel(viewModel.Azione, viewModel.Token);
            if ((Session["PayPalCompra"] != null && Session["PayPalAnnuncio"] != null ) || Session["PayPalOfferta"] != null && CanPagamento(viewModel.Azione))
            {

                //Now Create an object of credit card and add above details to it
                //Please replace your credit card details over here which you got from paypal
                CreditCard crdtCard = GetCreditCard(viewModel.Azione);

                ////// Specify details of your payment amount.
                ////Details details = new Details();
                ////details.shipping = "1";
                ////details.subtotal = "5";
                ////details.tax = "1";

                ////// Specify your total payment amount and assign the details object
                ////Amount amnt = new Amount();
                ////amnt.currency = "USD";
                ////// Total = shipping tax + subtotal.
                ////amnt.total = "7";
                ////amnt.details = details;

                ////// Now make a transaction object and assign the Amount object
                ////Transaction tran = new Transaction();
                ////tran.amount = amnt;
                ////tran.description = "Description about the payment amount.";
                ////tran.item_list = itemList;
                ////tran.invoice_number = "your invoice number which you are generating";

                ////// Now, we have to make a list of transaction and add the transactions object
                ////// to this list. You can create one or more object as per your requirements

                ////List<Transaction> transactions = new List<Transaction>();
                ////transactions.Add(tran);

                // Now we need to specify the FundingInstrument of the Payer
                // for credit card payments, set the CreditCard which we made above

                FundingInstrument fundInstrument = new FundingInstrument();
                fundInstrument.credit_card = crdtCard;

                // The Payment creation API requires a list of FundingIntrument

                List<FundingInstrument> fundingInstrumentList = new List<FundingInstrument>();
                fundingInstrumentList.Add(fundInstrument);

                // Now create Payer object and assign the fundinginstrument list to the object
                Payer payr = new Payer();
                payr.funding_instruments = fundingInstrumentList;
                payr.payment_method = "credit_card";

                // finally create the payment object and assign the payer object & transaction list to it
                Payment pymnt = new Payment();
                pymnt.intent = "sale";
                pymnt.payer = payr;
                viewModel.Guid = Convert.ToString((new Random()).Next(100000));
                pymnt.transactions = GetListTransaction(viewModel.Azione, viewModel.Guid);

                try
                {
                    //getting context from the paypal
                    //basically we are sending the clientID and clientSecret key in this function
                    //to the get the context from the paypal API to make the payment
                    //for which we have created the object above.

                    //Basically, apiContext object has a accesstoken which is sent by the paypal
                    //to authenticate the payment to facilitator account.
                    //An access token could be an alphanumeric string

                    APIContext apiContext = Configuration.GetAPIContext();

                    //Create is a Payment class function which actually sends the payment details
                    //to the paypal API for the payment. The function is passed with the ApiContext
                    //which we received above.

                    Payment createdPayment = pymnt.Create(apiContext);

                    //if the createdPayment.state is "approved" it means the payment was successful else not

                    if (createdPayment.state.ToLower() != "approved")
                    {
                        TempData["errore"] = "Errore durante l'acquisto";
                    }
                    else
                    {
                        if (SavePayment(viewModel, createdPayment))
                        {
                            TempData["Esito"] = App_GlobalResources.Language.JsonBuyAd;
                            TempData["pagamentoEffettuato"] = true;
                            return Redirect(_UrlFinePagamento);
                        }
                        else
                        {
                            TempData["errore"] = App_GlobalResources.ErrorResource.AdBuyFailed;
                        }
                    }
                }
                catch (PayPal.PayPalException ex)
                {
                    //Logger.Log("Error: " + ex.Message);
                    TempData["errore"] = ex.Message;
                }
            }
            else
            {
                TempData["errore"] = "Sessione pagamento scaduta!";
            }

            return Redirect(urlCancel);
        }
        
        #region METODI PRIVATI
        private PayPal.Api.Payment payment;

        private string _UrlFinePagamento;

        private bool CanPagamento(AzionePayPal azione)
        {
            switch (azione)
            {
                case AzionePayPal.Acquisto:
                    AnnuncioModel annuncio = Session["PayPalAnnuncio"] as AnnuncioModel;
                    using (DatabaseContext db = new DatabaseContext())
                    {
                        ANNUNCIO model = db.ANNUNCIO.SingleOrDefault(m => m.ID == annuncio.ID && m.SESSIONE_COMPRATORE == annuncio.SESSIONE_COMPRATORE);
                        if (model != null)
                        {
                            annuncio = new AnnuncioModel(model);
                            return true;
                        }
                    }
                    break;
                case AzionePayPal.Offerta:
                    OffertaModel offerta = Session["PayPalOfferta"] as OffertaModel;
                    using (DatabaseContext db = new DatabaseContext())
                    {
                        OFFERTA model = db.OFFERTA.SingleOrDefault(m => m.ID == offerta.ID && 
                            m.SESSIONE_COMPRATORE == offerta.SESSIONE_COMPRATORE);
                        if (model != null)
                        {
                            //offerta = new OffertaModel(model);
                            return true;
                        }
                    }
                    break;
            }
            //AnnullaPagamento(azione);
            return false;
        }

        private void AnnullaPagamento(AzionePayPal azione)
        {
            try
            {
                switch (azione)
                {
                    case AzionePayPal.Acquisto:
                        AnnuncioModel annuncio = Session["PayPalAnnuncio"] as AnnuncioModel;
                        using (DatabaseContext db = new DatabaseContext())
                        {
                            ANNUNCIO model = db.ANNUNCIO.SingleOrDefault(m => m.ID == annuncio.ID);
                            if (model != null)
                            {
                                if (model.STATO == (int)StatoVendita.SOSPESOPEROFFERTA)
                                {
                                    model.STATO = (int)StatoVendita.BARATTOINCORSO;
                                }
                                else
                                {
                                    model.STATO = (int)StatoVendita.ATTIVO;
                                }
                                model.SESSIONE_COMPRATORE = null;
                                model.DATA_MODIFICA = DateTime.Now;
                                db.SaveChanges();
                            }
                        }
                        break;
                    case AzionePayPal.Offerta:
                        OffertaModel offerta = Session["PayPalOfferta"] as OffertaModel;
                        using (DatabaseContext db = new DatabaseContext())
                        {
                            OFFERTA model = db.OFFERTA.SingleOrDefault(m => m.ID == offerta.ID);
                            if (model != null)
                            {
                                model.ANNUNCIO.STATO = (int)StatoVendita.ATTIVO;
                                model.SESSIONE_COMPRATORE = null;
                                model.STATO = (int)StatoOfferta.ATTIVA;
                                db.SaveChanges();
                            }
                        }
                        break;
                }
            }
            catch { }
        }

        private List<Transaction> GetListTransaction(AzionePayPal azione, string guid)
        {
            switch (azione)
            {
                case AzionePayPal.Acquisto:
                    AcquistoViewModel acquisto = Session["PayPalCompra"] as AcquistoViewModel;
                    AnnuncioModel annuncio = Session["PayPalAnnuncio"] as AnnuncioModel;
                    return GetListTransactionFromAcquisto(acquisto, annuncio, guid);
                case AzionePayPal.Offerta:
                    OffertaModel offerta = Session["PayPalOfferta"] as OffertaModel;
                    return GetListTransactionFromOfferta(offerta, guid);
            }
            return null;
        }

        private List<Transaction> GetListTransactionFromAcquisto(AcquistoViewModel viewModel, AnnuncioModel annuncio, string guid)
        {
            List<Transaction> lista = new List<Transaction>();
            //similar to credit card create itemlist and add item objects to it
            var itemList = new ItemList() { items = new List<Item>() };
            TIPO_VALUTA tipoValuta = (HttpContext.Application["tipoValuta"] as List<TIPO_VALUTA>).SingleOrDefault(m => m.SIMBOLO == NumberFormatInfo.CurrentInfo.CurrencySymbol);

            if (annuncio.TIPO_PAGAMENTO != (int)TipoPagamento.HAPPY)
            {
                itemList.items.Add(new Item()
                {
                    name = annuncio.NOME,
                    currency = tipoValuta.CODICE,
                    price = ConvertDecimalToString(annuncio.SOLDI),
                    quantity = "1",
                    sku = "sku"
                });
            }

            // se presente spedizione
            ANNUNCIO_TIPO_SCAMBIO tipoScambio = annuncio.ANNUNCIO_TIPO_SCAMBIO.FirstOrDefault(m => m.TIPO_SCAMBIO == (int)TipoScambio.Spedizione);
            if (annuncio.ID_OGGETTO != null && tipoScambio != null && viewModel.TipoScambio != TipoScambio.AMano)
            {
                ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE spedizione = tipoScambio.ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE.FirstOrDefault();
                if (spedizione != null)
                {
                    itemList.items.Add(new Item()
                    {
                        name = string.Concat(App_GlobalResources.Language.Shipment, " ", annuncio.NOME),
                        currency = tipoValuta.CODICE,
                        price = ConvertDecimalToString(spedizione.SOLDI),
                        //price = ConvertDecimalToString(new Decimal(1006.5)), // prova fissa verifica conversione
                        quantity = "1",
                        sku = "sku"
                    });
                }
            }

            decimal subtotal = itemList.items.Sum(m => ConvertStringToDecimal(m.price));

            // similar as we did for credit card, do here and create details object
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = ConvertDecimalToString(subtotal)
            };

            // similar as we did for credit card, do here and create amount object
            var amount = new Amount()
            {
                currency = tipoValuta.CODICE,
                total = ConvertDecimalToString(subtotal), // Total must be equal to sum of shipping, tax and subtotal.
                details = details
            };

            Transaction transazione = new Transaction();

            transazione.description = annuncio.NOTE_AGGIUNTIVE;
            transazione.invoice_number = guid + "|&%" + annuncio.TOKEN.ToString();
            transazione.amount = amount;
            transazione.item_list = itemList;

            lista.Add(transazione);
            return lista;
        }

        private List<Transaction> GetListTransactionFromOfferta(OffertaModel offerta, string guid)
        {
            List<Transaction> lista = new List<Transaction>();
            //similar to credit card create itemlist and add item objects to it
            var itemList = new ItemList() { items = new List<Item>() };
            TIPO_VALUTA tipoValuta = (HttpContext.Application["tipoValuta"] as List<TIPO_VALUTA>)
                .SingleOrDefault(m => m.SIMBOLO == NumberFormatInfo.CurrentInfo.CurrencySymbol);

            if (offerta.SOLDI > 0)
            {
                itemList.items.Add(new Item()
                {
                    name = "Pagamento offerta per annuncio: " + offerta.ANNUNCIO.NOME,
                    currency = tipoValuta.CODICE,
                    price = ConvertDecimalToString(offerta.SOLDI),
                    quantity = "1",
                    sku = "sku"
                });
            }

            OFFERTA_SPEDIZIONE spedizione = offerta.OFFERTA_SPEDIZIONE.FirstOrDefault();
            if (spedizione != null)
            {
                itemList.items.Add(new Item()
                {
                    name = string.Concat(App_GlobalResources.Language.Shipment, " per annuncio: ", offerta.ANNUNCIO.NOME),
                    currency = tipoValuta.CODICE,
                    price = ConvertDecimalToString(spedizione.SOLDI),
                    //price = ConvertDecimalToString(new Decimal(1006.5)), // prova fissa verifica conversione
                    quantity = "1",
                    sku = "sku"
                });
            }

            decimal subtotal = itemList.items.Sum(m => ConvertStringToDecimal(m.price));

            // similar as we did for credit card, do here and create details object
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = ConvertDecimalToString(subtotal)
            };

            // similar as we did for credit card, do here and create amount object
            var amount = new Amount()
            {
                currency = tipoValuta.CODICE,
                total = ConvertDecimalToString(subtotal), // Total must be equal to sum of shipping, tax and subtotal.
                details = details
            };

            Transaction transazione = new Transaction();

            transazione.description = offerta.NOTE;
            transazione.invoice_number = guid + "|&%" + offerta.ANNUNCIO.TOKEN.ToString();
            transazione.amount = amount;
            transazione.item_list = itemList;

            lista.Add(transazione);
            return lista;
        }

        private List<Transaction> GetListTransactionFromOffertaOK(OffertaModel offerta, string guid)
        {
            List<Transaction> lista = new List<Transaction>();
            //similar to credit card create itemlist and add item objects to it
            var itemList = new ItemList() { items = new List<Item>() };
            TIPO_VALUTA tipoValuta = (HttpContext.Application["tipoValuta"] as List<TIPO_VALUTA>)
                .SingleOrDefault(m => m.SIMBOLO == NumberFormatInfo.CurrentInfo.CurrencySymbol);

            if (offerta.ANNUNCIO.SOLDI > 0)
            {
                itemList.items.Add(new Item()
                {
                    name = "Pagamento offerta per annuncio: " + offerta.ANNUNCIO.NOME,
                    currency = tipoValuta.CODICE,
                    price = ConvertDecimalToString(offerta.ANNUNCIO.SOLDI),
                    quantity = "1",
                    sku = "sku"
                });
            }

            ANNUNCIO_TIPO_SCAMBIO tipoScambio = offerta.ANNUNCIO.ANNUNCIO_TIPO_SCAMBIO.FirstOrDefault(m => m.TIPO_SCAMBIO == (int)TipoScambio.Spedizione);
            if (offerta.ANNUNCIO.ID_OGGETTO != null && tipoScambio != null)
            {
                ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE spedizione = tipoScambio.ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE.FirstOrDefault();
                if (spedizione != null)
                {
                    itemList.items.Add(new Item()
                    {
                        name = string.Concat(App_GlobalResources.Language.Shipment, " ", offerta.ANNUNCIO.NOME),
                        currency = tipoValuta.CODICE,
                        price = ConvertDecimalToString(spedizione.SOLDI),
                        //price = ConvertDecimalToString(new Decimal(1006.5)), // prova fissa verifica conversione
                        quantity = "1",
                        sku = "sku"
                    });
                }
            }

            decimal subtotal = itemList.items.Sum(m => ConvertStringToDecimal(m.price));

            // similar as we did for credit card, do here and create details object
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = ConvertDecimalToString(subtotal)
            };

            // similar as we did for credit card, do here and create amount object
            var amount = new Amount()
            {
                currency = tipoValuta.CODICE,
                total = ConvertDecimalToString(subtotal), // Total must be equal to sum of shipping, tax and subtotal.
                details = details
            };

            Transaction transazione = new Transaction();

            transazione.description = offerta.NOTE;
            transazione.invoice_number = guid + "|&%" + offerta.ANNUNCIO.TOKEN.ToString();
            transazione.amount = amount;
            transazione.item_list = itemList;

            lista.Add(transazione);
            return lista;
        }

        private string ConvertDecimalToString(decimal? valore)
        {
            if (valore != null)
            {
                var culture = new CultureInfo("en-US").NumberFormat;
                culture.CurrencyDecimalSeparator = ".";
                culture.CurrencyGroupSeparator = ",";
                return Convert.ToDecimal(valore).ToString("N2", culture);
            }

            return null;
        }

        private decimal ConvertStringToDecimal(string valore)
        {
            if (!string.IsNullOrWhiteSpace(valore))
            {
                var culture = new CultureInfo("en-US").NumberFormat;
                culture.CurrencyDecimalSeparator = ".";
                culture.CurrencyGroupSeparator = ",";

                return Convert.ToDecimal(valore, culture);
            }

            return 0;
        }

        private Payment CreatePayment(APIContext apiContext, string redirectUrl, string redirectUrlCancel, List<Transaction> transazioni, string paymentMethod = "paypal")
        {
            var payer = new Payer() { payment_method = paymentMethod };

            // Configure Redirect Urls here with RedirectUrls object
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrlCancel, // URL ANNULLO
                return_url = redirectUrl // URL ACQUISTO CONCLUSO
            };

            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transazioni,
                redirect_urls = redirUrls,
                // id o token per riconoscere
            };

            // Create a payment using a APIContext
            return this.payment.Create(apiContext);
        }
        
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        private bool SavePayment(PayPalIndexViewModel viewModel, Payment payment)
        {
            switch (viewModel.Azione)
            {
                case AzionePayPal.Acquisto:
                    return SaveAcquisto(viewModel, payment);
                case AzionePayPal.Offerta:
                    return SaveOfferta();
            }
            return false;
        }

        private bool SaveAcquisto(PayPalIndexViewModel paypal, Payment payment)
        {
            AcquistoViewModel viewModel = Session["PayPalCompra"] as AcquistoViewModel;
            AnnuncioModel model = Session["PayPalAnnuncio"] as AnnuncioModel;
            using (DatabaseContext db = new DatabaseContext())
            {
                // AGGIUNGERE TRANSAZIONE NELLA TABELLA LOG_PAGAMENTO
                LOG_PAGAMENTO log = new LOG_PAGAMENTO();
                ANNUNCIO annuncio = db.ANNUNCIO.SingleOrDefault(m => m.TOKEN.ToString() == viewModel.Token 
                    && m.SESSIONE_COMPRATORE == model.SESSIONE_COMPRATORE);
                log.ID_ANNUNCIO = annuncio.ID;
                log.ID_COMPRATORE = (Session["utente"] as PersonaModel).Persona.ID;
                log.SESSIONE_COMPRATORE = paypal.Guid;
                log.ID_PAGAMENTO = payment.id;
                //log.ID_PAGAMENTO = "TEST";
                log.ISTITUTO_PAGAMENTO = "PAYPAL";
                log.NOME_ANNUNCIO = annuncio.NOME;
                log.DATA_INSERIMENTO = DateTime.Now;
                db.LOG_PAGAMENTO.Add(log);
                db.SaveChanges();
                using (System.Data.Entity.DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    viewModel.PagamentoFatto = true;
                    AnnuncioModel annuncioModel = new AnnuncioModel(annuncio);
                    Models.Enumerators.VerificaAcquisto verifica = annuncioModel.Acquisto(db, viewModel);
                    if (verifica == Models.Enumerators.VerificaAcquisto.Ok)
                    {
                        if (model.CompletaAcquisto(db, viewModel))
                        {
                            transaction.Commit();
                            this.RefreshPunteggioUtente(db);
                            System.Web.Routing.RouteValueDictionary data = new System.Web.Routing.RouteValueDictionary(new { token = viewModel.Token });
                            _UrlFinePagamento = Url.Action("Index", "Annuncio", data, this.Request.Url.Scheme, this.Request.Url.Host);
                            return true;
                        }
                        // altrimenti acquisto fallito
                    }
                }
            }
            return false;
        }

        private bool SaveOfferta()
        {
            OffertaModel offerta = Session["PayPalOfferta"] as OffertaModel;
            using (DatabaseContext db = new DatabaseContext())
            {
                // AGGIUNGERE TRANSAZIONE NELLA TABELLA LOG_PAGAMENTO
                LOG_PAGAMENTO log = new LOG_PAGAMENTO();
                OFFERTA model = db.OFFERTA.SingleOrDefault(m => m.ID == offerta.ID);
                log.ID_ANNUNCIO = model.ID_ANNUNCIO;
                log.ID_COMPRATORE = model.ID_PERSONA;
                log.ID_PAGAMENTO = model.ID.ToString();
                log.ISTITUTO_PAGAMENTO = "PAYPAL";
                log.NOME_ANNUNCIO = "Offerta per l'annuncio " + model.ANNUNCIO.NOME;
                log.DATA_INSERIMENTO = DateTime.Now;
                db.LOG_PAGAMENTO.Add(log);
                db.SaveChanges();
                using (System.Data.Entity.DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var utente = (Session["utente"] as PersonaModel);
                        string messaggio = string.Empty;
                        offerta.OffertaOriginale = db.OFFERTA.SingleOrDefault(m => m.ID == offerta.ID);
                        offerta.ANNUNCIO = offerta.OffertaOriginale.ANNUNCIO;
                        if (offerta.Accetta(db, utente, ref messaggio))
                        {
                            transaction.Commit();
                            System.Web.Routing.RouteValueDictionary data = new System.Web.Routing.RouteValueDictionary(new { token = offerta.ID });
                            _UrlFinePagamento = Url.Action("Index", "Offerta", data, this.Request.Url.Scheme, this.Request.Url.Host);
                            return (db.SaveChanges() > 0);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            return false;
        }

        private string GetUrlCancel(AzionePayPal azione, string token)
        {
            string nomeController = "Annuncio";
            if (azione == AzionePayPal.Offerta)
            {
                nomeController = "Vendite";
                System.Web.Routing.RouteValueDictionary data = new System.Web.Routing.RouteValueDictionary(new { token = token });
                ViewBag.Message = "Errore durante il pagamento!";
                return Url.Action("OfferteRicevute", nomeController, data, this.Request.Url.Scheme, this.Request.Url.Host);
            }
            else
            {
                AcquistoViewModel acquisto = new AcquistoViewModel();
                acquisto.Token = token;
                if (Session["PayPalCompra"] != null)
                    acquisto = Session["PayPalCompra"] as AcquistoViewModel;
                System.Web.Routing.RouteValueDictionary data = new System.Web.Routing.RouteValueDictionary(acquisto);
                return Url.Action("Index", nomeController, data, this.Request.Url.Scheme, this.Request.Url.Host);
            }
        }

        // metodo inutilizzato, viene usato direttamente paypal
        private CreditCard GetCreditCard(AzionePayPal azione)
        {
            switch (azione)
            {
                case AzionePayPal.Acquisto:
                    CreditCard creditCard = new CreditCard();
                    AcquistoViewModel viewModel = Session["PayPalCompra"] as AcquistoViewModel;
                    //crdtCard.billing_address = billingAddress;
                    creditCard.type = viewModel.TipoCarta.ToString();
                    creditCard.number = viewModel.NumeroCarta;
                    creditCard.cvv2 = ((int)viewModel.Cvv2).ToString();
                    creditCard.first_name = viewModel.NomeTitolareCarta;
                    creditCard.last_name = viewModel.CognomeTitolareCarta;
                    creditCard.expire_month = (int)viewModel.MeseScadenzaCarta;
                    creditCard.expire_year = (int)viewModel.AnnoScadenzaCarta;
                    return creditCard;
                case AzionePayPal.Offerta:
                    break;
            }
            return null;
        }
        #endregion
    }
}