using GratisForGratis.Models;
using GratisForGratis.Models.ViewModels;
using GratisForGratis.Utilities.PayPal;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GratisForGratis.Models.ExtensionMethods;

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
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Payment(PayPalIndexViewModel viewModel)
        {
            string urlCancel = GetUrlCancel(viewModel.Azione, viewModel.Token);
            int? id = null;

            try
            {
                //getting the apiContext as earlier
                APIContext apiContext = Configuration.GetAPIContext();

                if ((Session["PayPalCompra"] != null && Session["PayPalAnnuncio"] != null) || Session["PayPalOfferta"] != null)
                {
                    id = GetIdFromSession();
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
                        System.Web.Routing.RouteValueDictionary dataCancelPayPal = new System.Web.Routing.RouteValueDictionary(new {
                            azione = viewModel.Azione,
                            id = id
                        });
                        string urlCancelPaypal = Url.Action("CancelPayment", "PayPal", dataCancelPayPal, this.Request.Url.Scheme, this.Request.Url.Host);
                        //CreatePayment function gives us the payment approval url
                        //on which payer is redirected for paypal account payment
                        var createdPayment = this.CreatePayment(apiContext, url, urlCancelPaypal, GetListTransaction(viewModel.Azione, viewModel.Guid));

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
                    else if (!string.IsNullOrEmpty(viewModel.PayerID) || !string.IsNullOrWhiteSpace(viewModel.Guid))
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
                else
                {
                    TempData["errore"] = "Sessione pagamento scaduta!";
                }
            }
            catch (Exception ex)
            {
                //Logger.log("Error" + ex.Message);
                LoggatoreModel.Errore(ex);
                // redirect errore nell'acquisto
                TempData["errore"] = "Errore grave durante l'acquisto: " + ex.Message;
            }
            // mettere qui l'annullo dell'acquisto in modo da tornare indietro
            if (id != null)
            {
                AnnullaPagamento(viewModel.Azione, (int)id);
            }
            
            return Redirect(urlCancel);
        }

        public ActionResult CancelPayment(AzionePayPal azione, int id)
        {
            AnnullaPagamento(azione, id);
            string token = "";
            if (azione == AzionePayPal.Acquisto)
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    token = db.ANNUNCIO.SingleOrDefault(m => m.ID == (int)id).TOKEN.ToString();
                }
            }
            string urlCancel = GetUrlCancel(azione, token);
            return Redirect(urlCancel);
        }

        #region ATTRIBUTI
        private PayPal.Api.Payment payment;

        private string _UrlFinePagamento;
        #endregion

        #region METODI PRIVATI

        private int? GetIdFromSession()
        {
            if (Session["PayPalAnnuncio"] != null) {
                var annuncio = Session["PayPalAnnuncio"] as AnnuncioModel;
                return annuncio.ID;
            } else if (Session["PayPalOfferta"] != null)
            {
                var offerta = Session["PayPalOfferta"] as OffertaModel;
                return offerta.ID;
            }
            return null;
        }

        private bool CanPagamento(AzionePayPal azione)
        {
            switch (azione)
            {
                case AzionePayPal.Acquisto:
                    using (DatabaseContext db = new DatabaseContext())
                    {
                        AnnuncioModel annuncio = Session["PayPalAnnuncio"] as AnnuncioModel;
                        ANNUNCIO model = db.ANNUNCIO.SingleOrDefault(m => m.ID == annuncio.ID 
                            && m.SESSIONE_COMPRATORE == annuncio.SESSIONE_COMPRATORE);
                        if (model != null)
                        {
                            annuncio = new AnnuncioModel(model);
                            return true;
                        }
                    }
                    break;
                case AzionePayPal.Offerta:
                    using (DatabaseContext db = new DatabaseContext())
                    {
                        OffertaModel offerta = Session["PayPalOfferta"] as OffertaModel;
                        OFFERTA model = db.OFFERTA.SingleOrDefault(m => m.ID == offerta.ID && 
                            m.SESSIONE_COMPRATORE == offerta.SESSIONE_COMPRATORE);
                        if (model != null)
                        {
                            //offerta = new OffertaModel(model);
                            return true;
                        }
                    }
                    break;
                case AzionePayPal.OffertaOK:
                    using (DatabaseContext db = new DatabaseContext())
                    {
                        AnnuncioModel annuncio = Session["PayPalAnnuncio"] as AnnuncioModel;
                        ANNUNCIO model = db.ANNUNCIO.SingleOrDefault(m => m.ID == annuncio.ID
                            && m.SESSIONE_COMPRATORE == annuncio.SESSIONE_COMPRATORE);
                        if (model != null)
                        {
                            annuncio = new AnnuncioModel(model);
                            return true;
                        }
                    }
                    break;
            }
            //AnnullaPagamento(azione);
            return false;
        }

        private void AnnullaPagamento(AzionePayPal azione, int id)
        {
            try
            {
                switch (azione)
                {
                    case AzionePayPal.Acquisto:
                        using (DatabaseContext db = new DatabaseContext())
                        {
                            // se non metto la sessione_compratore torno indietro dall'acquisto e annullo tutto.
                            ANNUNCIO model = db.ANNUNCIO.SingleOrDefault(m => m.ID == id && m.SESSIONE_COMPRATORE!=null);
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
                        using (DatabaseContext db = new DatabaseContext())
                        {
                            OFFERTA model = db.OFFERTA.SingleOrDefault(m => m.ID == id && m.SESSIONE_COMPRATORE!=null);
                            if (model != null)
                            {
                                model.ANNUNCIO.STATO = (int)StatoVendita.ATTIVO;
                                model.SESSIONE_COMPRATORE = null;
                                model.STATO = (int)StatoOfferta.ATTIVA;
                                db.SaveChanges();
                            }
                        }
                        break;
                    case AzionePayPal.OffertaOK:
                        using (DatabaseContext db = new DatabaseContext())
                        {
                            ANNUNCIO model = db.ANNUNCIO.SingleOrDefault(m => m.ID == id && m.SESSIONE_COMPRATORE!=null);
                            if (model != null)
                            {
                                model.STATO = (int)StatoVendita.BARATTOINCORSO;
                                model.SESSIONE_COMPRATORE = null;
                                model.DATA_MODIFICA = DateTime.Now;
                                db.SaveChanges();
                            }
                        }
                        break;
                }
            }
            catch (Exception eccezione)
            {
                LoggatoreModel.Errore(eccezione);
            }
        }

        private List<Transaction> GetListTransaction(AzionePayPal azione, string guid)
        {
            using(DatabaseContext db = new DatabaseContext())
            {
                switch (azione)
                {
                    case AzionePayPal.Acquisto:
                        AcquistoViewModel acquisto = Session["PayPalCompra"] as AcquistoViewModel;
                        //AnnuncioModel annuncio = Session["PayPalAnnuncio"] as AnnuncioModel;
                        AnnuncioModel annuncio = new AnnuncioModel((Session["PayPalAnnuncio"] as AnnuncioModel).TOKEN, db);
                        return GetListTransactionFromAcquisto(acquisto, annuncio, guid);
                    case AzionePayPal.Offerta:
                        //OffertaModel offerta = Session["PayPalOfferta"] as OffertaModel;
                        int idOfferta = (Session["PayPalOfferta"] as OffertaModel).ID;
                        var model = db.OFFERTA.SingleOrDefault(m => m.ID == idOfferta);
                        OffertaModel offerta = new OffertaModel(model);
                        return GetListTransactionFromOfferta(offerta, guid);
                    case AzionePayPal.OffertaOK:
                        AcquistoViewModel acquistoOfferta = Session["PayPalCompra"] as AcquistoViewModel;
                        //AnnuncioModel annuncioOfferta = Session["PayPalAnnuncio"] as AnnuncioModel;
                        AnnuncioModel annuncioOfferta = new AnnuncioModel((Session["PayPalAnnuncio"] as AnnuncioModel).TOKEN, db);
                        return GetListTransactionFromAcquisto(acquistoOfferta, annuncioOfferta, guid);
                }
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
                if (annuncio.SOLDI!=null && annuncio.SOLDI > 0)
                {
                    itemList.items.Add(new Item()
                    {
                        name = annuncio.NOME,
                        currency = tipoValuta.CODICE,
                        price = ConvertDecimalToString(annuncio.SOLDI),
                        quantity = "1",
                        sku = "sku"
                    });
                    //decimal percentualeAnnuncio = Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["annuncioMonetaRealePercentuale"]);
                    //decimal commissioneAnnuncio = (((decimal)annuncio.SOLDI / 100) * percentualeAnnuncio);
                    decimal commissioneAnnuncio = (((decimal)annuncio.SOLDI / 100) * annuncio.COMMISSIONE.PERCENTUALE);
                    itemList.items.Add(new Item()
                    {
                        name = App_GlobalResources.Language.PayPalAd,
                        currency = tipoValuta.CODICE,
                        price = ConvertDecimalToString(commissioneAnnuncio),
                        quantity = "1",
                        sku = "sku"
                    });
                }
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
                    //decimal percentualeSpedizione = Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["spedizionePercentuale"]);
                    //decimal commissioneSpedizione = (((decimal)spedizione.SOLDI / 100) * percentualeSpedizione);
                    decimal commissioneSpedizione = (((decimal)spedizione.SOLDI / 100) * spedizione.COMMISSIONE.PERCENTUALE);
                    itemList.items.Add(new Item()
                    {
                        name = App_GlobalResources.Language.PayPalShipment,
                        currency = tipoValuta.CODICE,
                        price = ConvertDecimalToString(commissioneSpedizione),
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

                //decimal percentualeSpedizione = Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["spedizionePercentuale"]);
                //decimal commissioneSpedizione = (((decimal)spedizione.SOLDI / 100) * percentualeSpedizione);
                decimal commissioneSpedizione = (((decimal)spedizione.SOLDI / 100) * spedizione.COMMISSIONE.PERCENTUALE);
                itemList.items.Add(new Item()
                {
                    name = App_GlobalResources.Language.PayPalShipment,
                    currency = tipoValuta.CODICE,
                    price = ConvertDecimalToString(commissioneSpedizione),
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

            if (offerta.ANNUNCIO.TIPO_PAGAMENTO != (int)TipoPagamento.HAPPY)
            {
                if (offerta.SOLDI!=null && offerta.SOLDI > 0)
                {
                    itemList.items.Add(new Item()
                    {
                        name = "Pagamento offerta per annuncio: " + offerta.ANNUNCIO.NOME,
                        currency = tipoValuta.CODICE,
                        price = ConvertDecimalToString((decimal)offerta.SOLDI),
                        quantity = "1",
                        sku = "sku"
                    });
                    //decimal percentualeAnnuncio = Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["annuncioMonetaRealePercentuale"]);
                    //decimal commissioneAnnuncio = (((decimal)offerta.SOLDI / 100) * percentualeAnnuncio);
                    //itemList.items.Add(new Item()
                    //{
                    //    name = App_GlobalResources.Language.PayPalAd,
                    //    currency = tipoValuta.CODICE,
                    //    price = ConvertDecimalToString(commissioneAnnuncio),
                    //    quantity = "1",
                    //    sku = "sku"
                    //});
                }
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

                    decimal percentualeSpedizione = Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["spedizionePercentuale"]);
                    decimal commissioneSpedizione = (((decimal)spedizione.SOLDI / 100) * percentualeSpedizione);
                    itemList.items.Add(new Item()
                    {
                        name = App_GlobalResources.Language.PayPalShipment,
                        currency = tipoValuta.CODICE,
                        price = ConvertDecimalToString(commissioneSpedizione),
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
                    return SaveOfferta(payment);
                case AzionePayPal.OffertaOK:
                    return SaveOffertaCompleta(viewModel, payment);
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
                int idPaypal = SavePayPal(db, payment);
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    viewModel.PagamentoFatto = true;
                    AnnuncioModel annuncioModel = new AnnuncioModel(annuncio);
                    Models.Enumerators.VerificaAcquisto verifica = annuncioModel.Acquisto(db, viewModel);
                    if (verifica == Models.Enumerators.VerificaAcquisto.Ok)
                    {
                        if (model.CompletaAcquisto(db, viewModel, idPaypal))
                        {
                            transaction.Commit();
                            this.RefreshPunteggioUtente(db);
                            this.SendNotifica(annuncioModel.PERSONA, annuncioModel.PERSONA1, TipoNotifica.AnnuncioAcquistato, ControllerContext, "annuncioAcquistato", annuncioModel);
                            this.SendNotifica(annuncioModel.PERSONA1, annuncioModel.PERSONA, TipoNotifica.AnnuncioVenduto, ControllerContext, "annuncioVenduto", annuncioModel);
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

        private bool SaveOfferta(Payment payment)
        {
            OffertaModel offerta = Session["PayPalOfferta"] as OffertaModel;
            using (DatabaseContext db = new DatabaseContext())
            {
                db.Database.Connection.Open();
                // AGGIUNGERE TRANSAZIONE NELLA TABELLA DEI LOG
                LOG_PAGAMENTO log = new LOG_PAGAMENTO();
                OFFERTA model = db.OFFERTA.Include(m => m.PERSONA)
                    .Include(m => m.ANNUNCIO.ANNUNCIO_TIPO_SCAMBIO)
                    .SingleOrDefault(m => m.ID == offerta.ID);
                log.ID_ANNUNCIO = model.ID_ANNUNCIO;
                log.ID_COMPRATORE = model.ID_PERSONA;
                log.ID_PAGAMENTO = model.ID.ToString();
                log.ISTITUTO_PAGAMENTO = "PAYPAL";
                log.NOME_ANNUNCIO = "Offerta per l'annuncio " + model.ANNUNCIO.NOME;
                log.DATA_INSERIMENTO = DateTime.Now;
                db.LOG_PAGAMENTO.Add(log);
                db.SaveChanges();
                // cambia stato spedizione in pagata
                OFFERTA_SPEDIZIONE spedizione = model.OFFERTA_SPEDIZIONE.FirstOrDefault();
                spedizione.DATA_MODIFICA = DateTime.Now;
                spedizione.STATO = (int)StatoSpedizione.PAGATA;
                db.OFFERTA_SPEDIZIONE.Attach(spedizione);
                db.Entry(spedizione).State = EntityState.Modified;
                db.SaveChanges();
                int idPayPal = SavePayPal(db, payment);
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var utente = (Session["utente"] as PersonaModel);
                        string messaggio = string.Empty;
                        // ripopolo classe offerta per calcoli nella funzione ACCETTA
                        offerta.OffertaOriginale = model;
                        offerta.ANNUNCIO.ANNUNCIO_TIPO_SCAMBIO = db.ANNUNCIO_TIPO_SCAMBIO
                            .Include(m => m.ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE)
                            .Where(m => m.ID_ANNUNCIO == offerta.ID_ANNUNCIO).ToList();
                        offerta.AnnuncioModel.ANNUNCIO_TIPO_SCAMBIO = offerta.ANNUNCIO.ANNUNCIO_TIPO_SCAMBIO;

                        //if (offerta.Accetta(db, utente.Persona, utente.Credito, ref messaggio)) => non passa compratore nell'offerta
                        if (offerta.Accetta(db, utente.Persona, idPayPal, ref messaggio))
                        {
                            transaction.Commit();
                            Models.ViewModels.Email.PagamentoOffertaViewModel offertaAccettata = new Models.ViewModels.Email.PagamentoOffertaViewModel();
                            offertaAccettata.NominativoDestinatario = offerta.PERSONA.NOME + " " + offerta.PERSONA.COGNOME;
                            offertaAccettata.NomeAnnuncio = offerta.ANNUNCIO.NOME;
                            offertaAccettata.Moneta = offerta.PUNTI;
                            offertaAccettata.SoldiSpedizione = offerta.SOLDI;
                            offertaAccettata.Baratti = offerta.OFFERTA_BARATTO.Select(m => m.ANNUNCIO.NOME).ToList();
                            this.SendNotifica(utente.Persona, offerta.PERSONA, TipoNotifica.OffertaAccettata, ControllerContext, "offertaAccettata", offertaAccettata);
                            //System.Web.Routing.RouteValueDictionary data = new System.Web.Routing.RouteValueDictionary(new { token = offerta.ID });
                            _UrlFinePagamento = Url.Action("OfferteRicevute", "Vendite", null, this.Request.Url.Scheme, this.Request.Url.Host);
                            //return (db.SaveChanges() > 0);
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        LoggatoreModel.Errore(ex);
                        throw ex;
                    }
                }
            }
            return false;
        }

        private bool SaveOffertaCompleta(PayPalIndexViewModel paypal, Payment payment)
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
                log.NOME_ANNUNCIO = "Pagamento spedizione per " + annuncio.NOME;
                log.DATA_INSERIMENTO = DateTime.Now;
                db.LOG_PAGAMENTO.Add(log);
                db.SaveChanges();
                int idPayPal = SavePayPal(db, payment);

                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    viewModel.PagamentoFatto = true;
                    AnnuncioModel annuncioModel = new AnnuncioModel(annuncio);
                    Models.Enumerators.VerificaAcquisto verifica = annuncioModel.Acquisto(db, viewModel, true);
                    if (verifica == Models.Enumerators.VerificaAcquisto.Ok)
                    {
                        OFFERTA offerta = db.OFFERTA.SingleOrDefault(m => m.ID == paypal.Id);
                        if (model.CompletaAcquistoOfferta(db, offerta, idPayPal))
                        {
                            transaction.Commit();
                            this.RefreshPunteggioUtente(db);
                            Models.ViewModels.Email.PagamentoOffertaViewModel pagamentoOfferta = new Models.ViewModels.Email.PagamentoOffertaViewModel();
                            pagamentoOfferta.NominativoDestinatario = offerta.ANNUNCIO.PERSONA.NOME + " " + offerta.ANNUNCIO.PERSONA.COGNOME;
                            pagamentoOfferta.NomeAnnuncio = offerta.ANNUNCIO.NOME;
                            pagamentoOfferta.Moneta = offerta.PUNTI;
                            pagamentoOfferta.SoldiSpedizione = offerta.SOLDI;
                            pagamentoOfferta.Baratti = offerta.OFFERTA_BARATTO.Select(m => m.ANNUNCIO.NOME).ToList();
                            this.SendNotifica((Session["utente"] as PersonaModel).Persona, offerta.ANNUNCIO.PERSONA, TipoNotifica.OffertaPagata, ControllerContext, "pagamentoOfferta", pagamentoOfferta);
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

        private string GetUrlCancel(AzionePayPal azione, string token)
        {
            string nomeController = "Annuncio";
            if (azione == AzionePayPal.Offerta)
            {
                nomeController = "Vendite";
                return Url.Action("OfferteRicevute", nomeController, null, this.Request.Url.Scheme, this.Request.Url.Host);
            }
            else if (azione == AzionePayPal.OffertaOK)
            {
                nomeController = "Acquisti";
                return Url.Action("", nomeController, null, this.Request.Url.Scheme, this.Request.Url.Host);
            }
            else
            {
                System.Web.Routing.RouteValueDictionary data = new System.Web.Routing.RouteValueDictionary(new
                    {
                        token
                    });
                return Url.Action("Index", nomeController, data, this.Request.Url.Scheme, this.Request.Url.Host);
            }
        }

        private int SavePayPal(DatabaseContext db, Payment payment)
        {
            var transazione = payment.transactions.FirstOrDefault();
            TIPO_VALUTA tipoValuta = (HttpContext.Application["tipoValuta"] as List<TIPO_VALUTA>).SingleOrDefault(m => m.CODICE == transazione.amount.currency);

            PAYPAL paypal = new PAYPAL();
            paypal.KEY_PAYPAL = payment.id;
            paypal.NUMERO_FATTURA = transazione.invoice_number;
            paypal.NOME = transazione.description;
            paypal.IMPORTO = transazione.amount.total.ParseFromPayPal();
            paypal.ID_TIPO_VALUTA = tipoValuta.ID;
            paypal.DATA_INSERIMENTO = DateTime.Now;
            paypal.STATO = (int)Stato.ATTIVO;
            db.PAYPAL.Add(paypal);
            db.SaveChanges();
            foreach(var item in transazione.item_list.items)
            {
                PAYPAL_DETTAGLIO dettaglio = new PAYPAL_DETTAGLIO();
                dettaglio.ID_PAYPAL = paypal.ID;
                dettaglio.NOME = item.name;
                dettaglio.IMPORTO = item.price.ParseFromPayPal();
                dettaglio.QUANTITA = Convert.ToInt32(item.quantity);
                dettaglio.STATO = (int)Stato.ATTIVO;
                db.PAYPAL_DETTAGLIO.Add(dettaglio);
                db.SaveChanges();
            }
            return paypal.ID;
        }

        // metodo inutilizzato, viene usato direttamente paypal
        //private CreditCard GetCreditCard(AzionePayPal azione)
        //{
        //    switch (azione)
        //    {
        //        case AzionePayPal.Acquisto:
        //            CreditCard creditCard = new CreditCard();
        //            AcquistoViewModel viewModel = Session["PayPalCompra"] as AcquistoViewModel;
        //            //crdtCard.billing_address = billingAddress;
        //            creditCard.type = viewModel.TipoCarta.ToString();
        //            creditCard.number = viewModel.NumeroCarta;
        //            creditCard.cvv2 = ((int)viewModel.Cvv2).ToString();
        //            creditCard.first_name = viewModel.NomeTitolareCarta;
        //            creditCard.last_name = viewModel.CognomeTitolareCarta;
        //            creditCard.expire_month = (int)viewModel.MeseScadenzaCarta;
        //            creditCard.expire_year = (int)viewModel.AnnoScadenzaCarta;
        //            return creditCard;
        //        case AzionePayPal.Offerta:
        //            break;
        //    }
        //    return null;
        //}
        #endregion
    }
}