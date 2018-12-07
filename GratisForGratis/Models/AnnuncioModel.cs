using Emotion.Request;
using GratisForGratis.Controllers;
using GratisForGratis.Models.Enumerators;
using GratisForGratis.Models.ExtensionMethods;
using GratisForGratis.Utilities.PayPal;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;

namespace GratisForGratis.Models
{
    public class AnnuncioModel : ANNUNCIO
    {
        #region ATTRIBUTI
        private ANNUNCIO _AnnuncioOriginale;
        #endregion

        #region COSTRUTTORI
        public AnnuncioModel() : base() {
            _AnnuncioOriginale = new ANNUNCIO();
        }

        public AnnuncioModel(Guid token, DatabaseContext db) : base()
        {
            this._AnnuncioOriginale = db.ANNUNCIO.SingleOrDefault(m => m.TOKEN == token);
            CopyAttributes<ANNUNCIO>(_AnnuncioOriginale);
        }

        public AnnuncioModel(ANNUNCIO annuncio) : base()
        {
            _AnnuncioOriginale = annuncio;
            CopyAttributes<ANNUNCIO>(_AnnuncioOriginale);
        }
        #endregion

        #region PROPRIETà
        public string PayerId { get; set; }
        public string ExternalPaymentId { get; set; }
        #endregion

        #region METODI PUBBLICI

        public AnnuncioViewModel GetViewModel(DatabaseContext db)
        {
            AnnuncioViewModel viewModel = new AnnuncioViewModel();
            SetAnnuncioViewModel(db, viewModel, this);
            if (viewModel.TipoAcquisto == TipoAcquisto.Oggetto)
            {
                viewModel = SetOggettoViewModel(db, viewModel, this);
                viewModel = SetInfoCategoriaOggetto(db, new OggettoViewModel(viewModel));
            }
            else
            {
                viewModel = SetServizioViewModel(db, viewModel, this);
                viewModel = SetInfoCategoriaServizio(db, new ServizioViewModel(viewModel));
            }
            SetFeedbackVenditore(db, viewModel);
            return viewModel;
        }

        public AnnuncioViewModel GetViewModel(DatabaseContext db, Guid token)
        {
            AnnuncioViewModel viewModel = new AnnuncioViewModel();
            ANNUNCIO item = db.ANNUNCIO.Include(m => m.ANNUNCIO_FOTO).SingleOrDefault(m => m.TOKEN == token);
            SetAnnuncioViewModel(db, viewModel, item);
            if (viewModel.TipoAcquisto == TipoAcquisto.Oggetto)
            {
                viewModel = SetOggettoViewModel(db, viewModel, item);
                viewModel = SetInfoCategoriaOggetto(db, new OggettoViewModel(viewModel));
            }
            else
            {
                viewModel = SetServizioViewModel(db, viewModel, item);
                viewModel = SetInfoCategoriaServizio(db, new ServizioViewModel(viewModel));
            }
            SetFeedbackVenditore(db, viewModel);
            return viewModel;
        }

        public AnnuncioViewModel GetViewModel(DatabaseContext db, ANNUNCIO item)
        {
            AnnuncioViewModel viewModel = new AnnuncioViewModel();
            SetAnnuncioViewModel(db, viewModel, item);
            if (viewModel.TipoAcquisto == TipoAcquisto.Oggetto)
            {
                viewModel = SetOggettoViewModel(db, viewModel, item);
                viewModel = SetInfoCategoriaOggetto(db, new OggettoViewModel(viewModel));
            }
            else
            {
                viewModel = SetServizioViewModel(db, viewModel, item);
                viewModel = SetInfoCategoriaServizio(db, new ServizioViewModel(viewModel));
            }
            SetFeedbackVenditore(db, viewModel);
            return viewModel;
        }

        public ANNUNCIO GetAnnuncio()
        {
            ANNUNCIO annuncio = new ANNUNCIO();
            PropertyInfo[] properties = this.GetType().GetProperties();
            for (int i = 0; i < (int)properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                if (annuncio.GetType().GetProperty(propertyInfo.Name)!=null)
                    annuncio.GetType().GetProperty(propertyInfo.Name).SetValue(annuncio, propertyInfo.GetValue(this));
            }
            return annuncio;
        }

        public VerificaAcquisto CheckAcquisto(PERSONA compratore, List<CONTO_CORRENTE_CREDITO> listaCreditoCompratore, 
            TipoScambio scambio, bool pagamentoEseguito, bool offerta = false)
        {
            decimal credito = listaCreditoCompratore.Where(m => m.STATO == (int)StatoCredito.ASSEGNATO).Sum(m => m.PUNTI);
            // profilo compratore e venditore attivi
            // stato vendita in corso
            // credito compratore disponibile
            if (this.ID_PERSONA == compratore.ID)
                return VerificaAcquisto.AnnuncioPersonale;

            if (!(this.STATO == (int)StatoVendita.ATTIVO || this.STATO == (int)StatoVendita.BARATTOINCORSO 
                || this.STATO == (int)StatoVendita.SOSPESO || this.STATO == (int)StatoVendita.SOSPESOPEROFFERTA))
                return VerificaAcquisto.AnnuncioNonDisponibile;

            if (this.PUNTI > credito && !offerta)
                return VerificaAcquisto.CreditiNonSufficienti;

            if (compratore.STATO != (int)Stato.ATTIVO)
                return VerificaAcquisto.CompratoreNonAttivo;

            ANNUNCIO_TIPO_SCAMBIO tipoScambio = this.ANNUNCIO_TIPO_SCAMBIO.FirstOrDefault(m => m.TIPO_SCAMBIO == (int)TipoScambio.Spedizione);
            if (this.ID_OGGETTO != null && tipoScambio != null && scambio != TipoScambio.AMano)
            {
                ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE spedizione = tipoScambio.ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE.FirstOrDefault();

                if (!pagamentoEseguito && spedizione != null)
                {
                    return VerificaAcquisto.VerificaCartaCredito;
                }
            }

            if (!pagamentoEseguito && (int)this.SOLDI > 0 && this.TIPO_PAGAMENTO != (int)TipoPagamento.HAPPY)
                return VerificaAcquisto.VerificaCartaCredito;

            return VerificaAcquisto.Ok;
        }

        public VerificaAcquisto Acquisto(DatabaseContext db, AcquistoViewModel viewModel)
        {
            PersonaModel utente = (PersonaModel)HttpContext.Current.Session["utente"];
            VerificaAcquisto verifica = CheckAcquisto(utente.Persona, utente.Credito, viewModel.TipoScambio, viewModel.PagamentoFatto);
            switch (verifica)
            {
                case VerificaAcquisto.Ok:
                    //if (!CompletaAcquisto(db, viewModel, utente))
                    //    return VerificaAcquisto.PagamentoHappyFallito;
                    break;
                case VerificaAcquisto.VerificaCartaCredito:
                    _AnnuncioOriginale.DATA_MODIFICA = DateTime.Now;
                    if (_AnnuncioOriginale.STATO == (int)StatoVendita.BARATTOINCORSO)
                    {
                        _AnnuncioOriginale.STATO = (int)StatoVendita.SOSPESOPEROFFERTA;
                    }
                    else
                    {
                        _AnnuncioOriginale.STATO = (int)StatoVendita.SOSPESO;
                    }
                    _AnnuncioOriginale.SESSIONE_COMPRATORE = HttpContext.Current.Session.SessionID + "§" + Guid.NewGuid().ToString();
                    db.ANNUNCIO.Attach(_AnnuncioOriginale);
                    db.Entry(_AnnuncioOriginale).State = EntityState.Modified;
                    if (db.SaveChanges() <= 0)
                    {
                        return VerificaAcquisto.VerificaCartaFallita;
                    }
                    break;
            }
            CopyAttributes<ANNUNCIO>(_AnnuncioOriginale);
            return verifica;
        }

        public bool CompletaAcquisto(DatabaseContext db, AcquistoViewModel viewModel)
        {
            PersonaModel utente = (PersonaModel)HttpContext.Current.Session["utente"];
            ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE spedizione = null;
            CORRIERE_SERVIZIO_SPEDIZIONE datiSpedizione = null;
            INDIRIZZO indirizzo = null;
            // creazione transazione
            TRANSAZIONE transazione = new TRANSAZIONE();
            transazione.ID_CONTO_MITTENTE = utente.Persona.ID_CONTO_CORRENTE;
            transazione.ID_CONTO_DESTINATARIO = this.PERSONA.ID_CONTO_CORRENTE;
            transazione.NOME = this.NOME;
            transazione.PUNTI = this.PUNTI;
            transazione.SOLDI = Utils.cambioValuta(transazione.PUNTI);
            transazione.TEST = 0;
            transazione.TIPO = this.TIPO_PAGAMENTO;
            transazione.DATA_INSERIMENTO = DateTime.Now;
            transazione.STATO = (int)StatoPagamento.ACCETTATO;
            if (!string.IsNullOrWhiteSpace(ExternalPaymentId))
                transazione.EXTERNAL_ID = ExternalPaymentId;
            db.TRANSAZIONE.Add(transazione);
            if (db.SaveChanges() > 0)
            {

                TRANSAZIONE_ANNUNCIO transazioneAnnuncio = new TRANSAZIONE_ANNUNCIO();
                transazioneAnnuncio.ID_TRANSAZIONE = transazione.ID;
                transazioneAnnuncio.ID_ANNUNCIO = this.ID;
                transazioneAnnuncio.PUNTI = (decimal)transazione.PUNTI;
                transazioneAnnuncio.SOLDI = (decimal)transazione.SOLDI;
                transazioneAnnuncio.DATA_INSERIMENTO = DateTime.Now;
                transazioneAnnuncio.STATO = (int)StatoPagamento.ACCETTATO;

                ANNUNCIO_TIPO_SCAMBIO tipoScambio = this.ANNUNCIO_TIPO_SCAMBIO.FirstOrDefault(m => m.TIPO_SCAMBIO == (int)TipoScambio.Spedizione);
                if (this.ID_OGGETTO != null && tipoScambio != null && viewModel.TipoScambio != TipoScambio.AMano)
                {
                    spedizione = tipoScambio.ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE.FirstOrDefault();
                    if (spedizione != null)
                    {
                        datiSpedizione = db.CORRIERE_SERVIZIO_SPEDIZIONE.SingleOrDefault(m => m.ID == spedizione.ID_CORRIERE_SERVIZIO_SPEDIZIONE);

                        indirizzo = db.INDIRIZZO.FirstOrDefault(m => m.INDIRIZZO1 == viewModel.IndirizzoDestinatario && m.COMUNE.CAP == viewModel.CapDestinatario && m.CIVICO == viewModel.CivicoDestinatario);
                        if (indirizzo == null)
                        {
                            indirizzo = new INDIRIZZO();
                            indirizzo.INDIRIZZO1 = viewModel.IndirizzoDestinatario;
                            indirizzo.CIVICO = viewModel.CivicoDestinatario;
                            indirizzo.ID_COMUNE = db.COMUNE.FirstOrDefault(m => m.CAP == viewModel.CapDestinatario).ID;
                            indirizzo.TIPOLOGIA = (int)TipoIndirizzo.Residenza;
                            indirizzo.DATA_INSERIMENTO = DateTime.Now;
                            indirizzo.STATO = (int)Stato.ATTIVO;
                            db.INDIRIZZO.Add(indirizzo);
                            db.SaveChanges();
                        }
                        datiSpedizione.ID_INDIRIZZO_DESTINATARIO = indirizzo.ID;
                        datiSpedizione.NOMINATIVO_DESTINATARIO = viewModel.NominativoDestinatario;
                        datiSpedizione.TELEFONO_DESTINATARIO = viewModel.TelefonoDestinatario;
                        datiSpedizione.INFO_EXTRA_DESTINATARIO = viewModel.InfoExtraDestinatario;
                        db.CORRIERE_SERVIZIO_SPEDIZIONE.Attach(datiSpedizione);
                        db.Entry(datiSpedizione).State = EntityState.Modified;
                        // transazione in sospeso per la spedizione
                        if (db.SaveChanges() > 0)
                        {
                            transazioneAnnuncio.PUNTI_SPEDIZIONE = spedizione.PUNTI;
                            transazioneAnnuncio.SOLDI_SPEDIZIONE = spedizione.SOLDI;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                db.TRANSAZIONE_ANNUNCIO.Add(transazioneAnnuncio);
                if (db.SaveChanges() > 0)
                {
                    if ((decimal)this.SOLDI > 0 && this.TIPO_PAGAMENTO != (int)TipoPagamento.HAPPY)
                        return false;

                    this.TRANSAZIONE_ANNUNCIO = new List<TRANSAZIONE_ANNUNCIO>();
                    this.TRANSAZIONE_ANNUNCIO.Add(transazioneAnnuncio);

                    // spostamento crediti al venditore
                    if (this.PUNTI > 0)
                    {
                        decimal puntiRimanenti = 0;
                        decimal punti = this.PUNTI;
                        while (punti > 0)
                        {
                            CONTO_CORRENTE_CREDITO credito = db.CONTO_CORRENTE_CREDITO
                                .Where(m => m.ID_CONTO_CORRENTE == utente.Persona.ID_CONTO_CORRENTE
                                    && m.STATO == (int)StatoCredito.ASSEGNATO && m.DATA_SCADENZA > DateTime.Now)
                                .OrderBy(m => m.DATA_SCADENZA)
                                .FirstOrDefault();
                            if ((punti - credito.PUNTI) < 0)
                            {
                                puntiRimanenti = credito.PUNTI - punti;
                                punti = 0;
                            }
                            else
                            {
                                punti = punti - credito.PUNTI;
                            }

                            if (punti <= 0 && puntiRimanenti > 0)
                            {
                                credito.PUNTI -= puntiRimanenti;
                            }
                            credito.ID_TRANSAZIONE_USCITA = transazione.ID;
                            credito.STATO = (int)StatoCredito.CEDUTO;
                            db.SaveChanges();

                            if (punti <= 0 && puntiRimanenti > 0)
                            {
                                CONTO_CORRENTE_CREDITO creditoCompratore = new CONTO_CORRENTE_CREDITO();
                                creditoCompratore.ID_CONTO_CORRENTE = transazione.ID_CONTO_MITTENTE;
                                creditoCompratore.ID_TRANSAZIONE_ENTRATA = transazione.ID;
                                creditoCompratore.PUNTI = puntiRimanenti;
                                creditoCompratore.SOLDI = Utils.cambioValuta(creditoCompratore.PUNTI);
                                creditoCompratore.GIORNI_SCADENZA = credito.GIORNI_SCADENZA;
                                creditoCompratore.DATA_SCADENZA = credito.DATA_SCADENZA;
                                creditoCompratore.DATA_INSERIMENTO = DateTime.Now;
                                creditoCompratore.STATO = (int)StatoCredito.ASSEGNATO;
                                db.CONTO_CORRENTE_CREDITO.Add(creditoCompratore);
                                db.SaveChanges();
                            }
                        }

                        CONTO_CORRENTE_CREDITO creditoVenditore = new CONTO_CORRENTE_CREDITO();
                        creditoVenditore.ID_CONTO_CORRENTE = transazione.ID_CONTO_DESTINATARIO;
                        creditoVenditore.ID_TRANSAZIONE_ENTRATA = transazione.ID;
                        creditoVenditore.PUNTI = this.PUNTI;
                        creditoVenditore.SOLDI = Utils.cambioValuta(creditoVenditore.PUNTI);
                        creditoVenditore.GIORNI_SCADENZA = Convert.ToInt32(ConfigurationManager.AppSettings["GiorniScadenzaCredito"]);
                        creditoVenditore.DATA_SCADENZA = DateTime.Now.AddDays(creditoVenditore.GIORNI_SCADENZA);
                        creditoVenditore.DATA_INSERIMENTO = DateTime.Now;
                        creditoVenditore.STATO = (int)StatoCredito.ASSEGNATO;
                        db.CONTO_CORRENTE_CREDITO.Add(creditoVenditore);
                        db.SaveChanges();

                        //List<CONTO_CORRENTE_MONETA> lista = db.CONTO_CORRENTE_MONETA
                        //    .Where(m => m.ID_CONTO_CORRENTE == utente.Persona.ID_CONTO_CORRENTE && m.STATO == (int)StatoMoneta.ASSEGNATA)
                        //    .Take((int)this.PUNTI)
                        //    .ToList();

                        //foreach (CONTO_CORRENTE_MONETA m in lista)
                        //{
                        //    m.STATO = (int)StatoMoneta.CEDUTA;
                        //    m.DATA_MODIFICA = DateTime.Now;
                        //    db.CONTO_CORRENTE_MONETA.Attach(m);
                        //    db.Entry(m).State = EntityState.Modified;
                        //    if (db.SaveChanges() > 0)
                        //    {
                        //        // aggiungo i crediti al venditore
                        //        CONTO_CORRENTE_MONETA contoCorrente = new CONTO_CORRENTE_MONETA();
                        //        contoCorrente.ID_CONTO_CORRENTE = this.PERSONA.ID_CONTO_CORRENTE;
                        //        contoCorrente.ID_MONETA = m.ID_MONETA;
                        //        contoCorrente.ID_TRANSAZIONE = transazione.ID;
                        //        contoCorrente.DATA_INSERIMENTO = DateTime.Now;
                        //        contoCorrente.STATO = (int)StatoMoneta.ASSEGNATA;
                        //        db.CONTO_CORRENTE_MONETA.Add(contoCorrente);
                        //        if (db.SaveChanges() > 0)
                        //        {
                        //        }
                        //    }
                        //    else
                        //    {
                        //        return false;
                        //    }
                        //}
                    }
                }
                ANNUNCIO annuncioAcquistato = db.ANNUNCIO.SingleOrDefault(m => m.ID == this.ID);
                annuncioAcquistato.DATA_MODIFICA = DateTime.Now;
                annuncioAcquistato.STATO = (int)StatoVendita.VENDUTO;
                annuncioAcquistato.ID_COMPRATORE = utente.Persona.ID;
                annuncioAcquistato.DATA_VENDITA = DateTime.Now;
                db.ANNUNCIO.Attach(annuncioAcquistato);
                db.Entry(annuncioAcquistato).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    /*
                    if (spedizione != null)
                    {
                        AvviaSpedizione(db, datiSpedizione, indirizzo, utente);
                    }
                    */
                    OffertaModel.AnnullaOfferteEffettuate(db, this.ID);
                    OffertaModel.AnnullaOfferteRicevute(db, this.ID);
                    _AnnuncioOriginale = annuncioAcquistato;
                    return true;
                }
            }
            return false;
        }

        public void AnnullaAcquisto(DatabaseContext db)
        {
            _AnnuncioOriginale.DATA_MODIFICA = DateTime.Now;
            if (_AnnuncioOriginale.STATO == (int)StatoVendita.SOSPESOPEROFFERTA)
                _AnnuncioOriginale.STATO = (int)StatoVendita.BARATTOINCORSO;
            else
                _AnnuncioOriginale.STATO = (int)StatoVendita.ATTIVO;

            _AnnuncioOriginale.SESSIONE_COMPRATORE = null;
            db.ANNUNCIO.Attach(_AnnuncioOriginale);
            db.Entry(_AnnuncioOriginale).State = EntityState.Modified;
            db.SaveChanges();
            CopyAttributes<ANNUNCIO>(_AnnuncioOriginale);
        }

        public void CopyAttributes<T>(T model) where T : ANNUNCIO
        {
            PropertyInfo[] properties = model.GetType().GetProperties();
            for (int i = 0; i < (int)properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                //if (!(propertyInfo.GetType() == typeof(T) || propertyInfo.GetType().BaseType == typeof(T)))
                this.GetType().GetProperty(propertyInfo.Name).SetValue(this, propertyInfo.GetValue(model));
            }
            _AnnuncioOriginale = (ANNUNCIO)model;
        }
        /*
        public bool Compra(DatabaseContext db, AcquistoViewModel viewModel, ref string messaggio)
        {
            messaggio = App_GlobalResources.ErrorResource.AdNotValid;
            PersonaModel utente = (PersonaModel)HttpContext.Current.Session["utente"];
            int credito = utente.ContoCorrente.Count(m => m.STATO == (int)StatoMoneta.ASSEGNATA);
            ANNUNCIO annuncio = db.ANNUNCIO.SingleOrDefault(m => m.TOKEN == this.TOKEN);
            if (annuncio == null)
                throw new HttpException(404, App_GlobalResources.ExceptionMessage.AdNotFound);

            if (annuncio.ID_PERSONA == utente.Persona.ID
                        || annuncio.STATO != (int)StatoVendita.ATTIVO || annuncio.PUNTI > credito
                        || utente.Persona.STATO != (int)Stato.ATTIVO)
            {
                return false;
            }
            else {

                // creazione transazione
                TRANSAZIONE transazione = new TRANSAZIONE();
                transazione.ID_CONTO_MITTENTE = utente.Persona.ID_CONTO_CORRENTE;
                transazione.ID_CONTO_DESTINATARIO = annuncio.PERSONA.ID_CONTO_CORRENTE;
                transazione.NOME = annuncio.NOME;
                transazione.PUNTI = annuncio.PUNTI;
                transazione.SOLDI = annuncio.SOLDI;
                transazione.TEST = 0;
                transazione.TIPO = annuncio.TIPO_PAGAMENTO;
                transazione.DATA_INSERIMENTO = DateTime.Now;
                transazione.STATO = (int)StatoPagamento.ACCETTATO;
                db.TRANSAZIONE.Add(transazione);
                if (db.SaveChanges() > 0)
                {
                    TRANSAZIONE_ANNUNCIO transazioneAnnuncio = new TRANSAZIONE_ANNUNCIO();
                    transazioneAnnuncio.ID_TRANSAZIONE = transazione.ID;
                    transazioneAnnuncio.ID_ANNUNCIO = annuncio.ID;
                    transazioneAnnuncio.PUNTI = annuncio.PUNTI;
                    transazioneAnnuncio.SOLDI = (int)annuncio.SOLDI;
                    transazioneAnnuncio.DATA_INSERIMENTO = DateTime.Now;
                    transazioneAnnuncio.STATO = (int)StatoPagamento.ATTIVO;
                    db.TRANSAZIONE_ANNUNCIO.Add(transazioneAnnuncio);
                    if (db.SaveChanges() > 0)
                    {
                        if ((int)annuncio.SOLDI > 0)
                            this.PagamentoSoldiReali = true;

                        ANNUNCIO_TIPO_SCAMBIO tipoScambio = annuncio.ANNUNCIO_TIPO_SCAMBIO.FirstOrDefault(m => m.TIPO_SCAMBIO == (int)TipoScambio.Spedizione);
                        if (annuncio.ID_OGGETTO != null && tipoScambio != null && viewModel.TipoScambio != TipoScambio.AMano)
                        {
                            ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE spedizione = tipoScambio.ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE.FirstOrDefault();
                            if (spedizione != null)
                            {
                                CORRIERE_SERVIZIO_SPEDIZIONE datiSpedizione = db.CORRIERE_SERVIZIO_SPEDIZIONE.SingleOrDefault(m => m.ID == spedizione.ID_CORRIERE_SERVIZIO_SPEDIZIONE);

                                INDIRIZZO indirizzo = db.INDIRIZZO.FirstOrDefault(m => m.INDIRIZZO1 == viewModel.IndirizzoDestinatario && m.COMUNE.CAP == viewModel.CapDestinatario && m.CIVICO == viewModel.CivicoDestinatario);
                                if (indirizzo == null)
                                {
                                    indirizzo = new INDIRIZZO();
                                    indirizzo.INDIRIZZO1 = viewModel.IndirizzoDestinatario;
                                    indirizzo.CIVICO = viewModel.CivicoDestinatario;
                                    indirizzo.ID_COMUNE = db.COMUNE.FirstOrDefault(m => m.CAP == viewModel.CapDestinatario).ID;
                                    indirizzo.TIPOLOGIA = (int)TipoIndirizzo.Residenza;
                                    indirizzo.DATA_INSERIMENTO = DateTime.Now;
                                    indirizzo.STATO = (int)Stato.ATTIVO;
                                    db.INDIRIZZO.Add(indirizzo);
                                    db.SaveChanges();
                                }
                                datiSpedizione.ID_INDIRIZZO_DESTINATARIO = indirizzo.ID;
                                datiSpedizione.NOMINATIVO_DESTINATARIO = viewModel.NominativoDestinatario;
                                datiSpedizione.TELEFONO_DESTINATARIO = viewModel.TelefonoDestinatario;
                                datiSpedizione.INFO_EXTRA_DESTINATARIO = viewModel.InfoExtraDestinatario;
                                db.CORRIERE_SERVIZIO_SPEDIZIONE.Attach(datiSpedizione);
                                db.Entry(datiSpedizione).State = EntityState.Modified;
                                if (db.SaveChanges() > 0)
                                {
                                    // transazione in sospeso per la spedizione
                                    TRANSAZIONE_ANNUNCIO_SPEDIZIONE transazioneAnnuncioSpedizione = new TRANSAZIONE_ANNUNCIO_SPEDIZIONE();
                                    transazioneAnnuncioSpedizione.ID_ANNUNCIO_SPEDIZIONE = spedizione.ID;
                                    transazioneAnnuncioSpedizione.ID_TRANSAZIONE = transazione.ID;
                                    transazioneAnnuncioSpedizione.DATA_INSERIMENTO = DateTime.Now;
                                    transazioneAnnuncioSpedizione.STATO = (int)Stato.SOSPESO;
                                    db.TRANSAZIONE_ANNUNCIO_SPEDIZIONE.Add(transazioneAnnuncioSpedizione);
                                    if (db.SaveChanges() <= 0)
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        this.PagamentoSoldiReali = true;
                                    }
                                }
                            }
                        }
                        // spostamento crediti al venditore
                        if (annuncio.PUNTI > 0)
                        {
                            List<CONTO_CORRENTE_MONETA> lista = db.CONTO_CORRENTE_MONETA
                                .Where(m => m.ID_CONTO_CORRENTE == utente.Persona.ID_CONTO_CORRENTE && m.STATO == (int)StatoMoneta.ASSEGNATA)
                                .Take(annuncio.PUNTI)
                                .ToList();

                            foreach (CONTO_CORRENTE_MONETA m in lista)
                            {
                                m.STATO = (int)StatoMoneta.CEDUTA;
                                m.DATA_MODIFICA = DateTime.Now;
                                db.CONTO_CORRENTE_MONETA.Attach(m);
                                db.Entry(m).State = EntityState.Modified;
                                if (db.SaveChanges() > 0)
                                {
                                    // aggiungo i crediti al venditore
                                    CONTO_CORRENTE_MONETA contoCorrente = new CONTO_CORRENTE_MONETA();
                                    contoCorrente.ID_CONTO_CORRENTE = annuncio.PERSONA.ID_CONTO_CORRENTE;
                                    contoCorrente.ID_MONETA = m.ID_MONETA;
                                    contoCorrente.ID_TRANSAZIONE = transazione.ID;
                                    contoCorrente.DATA_INSERIMENTO = DateTime.Now;
                                    contoCorrente.STATO = (int)StatoMoneta.ASSEGNATA;
                                    db.CONTO_CORRENTE_MONETA.Add(contoCorrente);
                                    if (db.SaveChanges() > 0)
                                    {
                                        // associazione moneta ceduta alla transazione
                                        //TRANSAZIONE_CONTO_CORRENTE_MONETA offertaConto = new TRANSAZIONE_CONTO_CORRENTE_MONETA();
                                        //offertaConto.ID_TRANSAZIONE = transazione.ID;
                                        //offertaConto.ID_CONTO_CORRENTE_MONETA = m.ID;
                                        //offertaConto.DATA_INSERIMENTO = DateTime.Now;
                                        //offertaConto.STATO = (int)Stato.ATTIVO;
                                        //db.TRANSAZIONE_CONTO_CORRENTE_MONETA.Add(offertaConto);
                                        //if (db.SaveChanges() <= 0)
                                        //    return false;
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    annuncio.DATA_MODIFICA = DateTime.Now;
                    annuncio.STATO = (int)StatoVendita.SOSPESO;

                    db.ANNUNCIO.Attach(annuncio);
                    db.Entry(annuncio).State = EntityState.Modified;
                    if (db.SaveChanges() > 0)
                    {
                        messaggio = string.Empty;
                        return true;
                    }
                }
            }
            return false;
        }
        */
        #endregion

        #region METODI PRIVATI

        private void SetAnnuncioViewModel(DatabaseContext db, AnnuncioViewModel annuncio, ANNUNCIO vendita)
        {

            int utente = 0;
            if (HttpContext.Current.Session["utente"] != null)
                utente = ((PersonaModel)HttpContext.Current.Session["utente"]).Persona.ID;
            annuncio.Id = vendita.ID;
            annuncio.Token = vendita.TOKEN.ToString();
            annuncio.Nome = vendita.NOME;
            annuncio.Citta = vendita.COMUNE.NOME;
            annuncio.Punti = vendita.PUNTI.ToHappyCoin();
            annuncio.Soldi = Convert.ToDecimal(vendita.SOLDI).ToString("C");
            annuncio.TipoPagamento = (TipoPagamento)vendita.TIPO_PAGAMENTO;
            annuncio.TipoAcquisto = (vendita.ID_SERVIZIO != null) ? TipoAcquisto.Servizio : TipoAcquisto.Oggetto;
            annuncio.CategoriaID = vendita.ID_CATEGORIA;
            annuncio.Categoria = vendita.CATEGORIA.NOME;
            annuncio.Foto = vendita.ANNUNCIO_FOTO.Where(f => f.ID_ANNUNCIO == vendita.ID).Select(f => new AnnuncioFoto()
            {
                ID_ANNUNCIO = f.ID_ANNUNCIO,
                ALLEGATO= f.ALLEGATO,
                DATA_INSERIMENTO = f.DATA_INSERIMENTO,
                DATA_MODIFICA = f.DATA_MODIFICA
            }).ToList();
            annuncio.DataInserimento = vendita.DATA_INSERIMENTO;
            annuncio.DataModifica = vendita.DATA_MODIFICA;
            annuncio.StatoVendita = (StatoVendita)vendita.STATO;
            annuncio.Notificato = (vendita.ANNUNCIO_NOTIFICA.Count(m => m.ID_ANNUNCIO == vendita.ID && m.NOTIFICA.ID_PERSONA == utente) > 0) ? true : false;
            IQueryable<ANNUNCIO_DESIDERATO> listaInteressati = db.ANNUNCIO_DESIDERATO.Where(f => f.ID_ANNUNCIO == annuncio.Id);
            annuncio.NumeroInteressati = listaInteressati.Count();
            annuncio.Desidero = listaInteressati.FirstOrDefault(m => m.ID_PERSONA == utente) != null;
            // controllo se l'utente ha già proposto lo stesso annuncio
            int? idAnnuncioOriginale = null;
            if (vendita.ID_ORIGINE != null)
            {
                idAnnuncioOriginale = (int)vendita.ID_ORIGINE;
            }
            // verifica se è una prima copia o una copia di seconda mano
            ANNUNCIO copiaAnnuncio = db.ANNUNCIO.SingleOrDefault(m => m.ID_PERSONA == utente
                    && (m.STATO == (int)StatoVendita.ATTIVO || m.STATO == (int)StatoVendita.INATTIVO)
                    && ((idAnnuncioOriginale != null && (m.ID_ORIGINE == idAnnuncioOriginale || m.ID == idAnnuncioOriginale))
                        || m.ID_ORIGINE == annuncio.Id));
            if (copiaAnnuncio != null)
                annuncio.TokenAnnuncioCopiato = Utils.RandomString(3) + copiaAnnuncio.TOKEN + Utils.RandomString(3);
            annuncio.Venditore = new UtenteVenditaViewModel(vendita.PERSONA);

            if (vendita.ID_COMPRATORE != null)
            {
                annuncio.Compratore = new UtenteVenditaViewModel(vendita.PERSONA1);
            }
            annuncio.NoteAggiuntive = vendita.NOTE_AGGIUNTIVE;
            annuncio.DataAvvio = vendita.DATA_AVVIO;
            annuncio.DataFine = vendita.DATA_FINE;
            annuncio.DataVendita = vendita.DATA_VENDITA;
        }

        private AnnuncioViewModel SetOggettoViewModel(DatabaseContext db, AnnuncioViewModel oggetto, ANNUNCIO vendita)
        {
            OggettoViewModel viewModel = new OggettoViewModel(oggetto);
            viewModel.OggettoId = (int)vendita.ID_OGGETTO;

            if (vendita.OGGETTO.ID_MARCA != null)
                viewModel.Marca = vendita.OGGETTO.MARCA.NOME;
            if (vendita.OGGETTO.OGGETTO_MATERIALE != null)
            {
                viewModel.Materiali = vendita.OGGETTO.OGGETTO_MATERIALE.Select(m => m.MATERIALE.NOME).ToList();
            }
            if (vendita.OGGETTO.OGGETTO_COMPONENTE != null)
            {
                viewModel.Componenti = vendita.OGGETTO.OGGETTO_COMPONENTE.Select(m => m.COMPONENTE.NOME).ToList();
            }
            viewModel.StatoOggetto = (CondizioneOggetto)vendita.OGGETTO.CONDIZIONE;
            viewModel.Quantità = vendita.OGGETTO.NUMERO_PEZZI;

            var listaTipoScambio = db.ANNUNCIO_TIPO_SCAMBIO.Where(m => m.ID_ANNUNCIO == oggetto.Id);
            if (listaTipoScambio != null)
            {
                viewModel.TipoScambio = listaTipoScambio.Select(m => (TipoScambio)m.TIPO_SCAMBIO).ToArray();
                if (viewModel.TipoScambio.Contains(TipoScambio.Spedizione))
                {
                    ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE spedizione = db.ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE
                        .FirstOrDefault(m => m.ID_ANNUNCIO_TIPO_SCAMBIO == listaTipoScambio.FirstOrDefault(n => n.TIPO_SCAMBIO == (int)TipoScambio.Spedizione).ID);
                    if (spedizione != null)
                    {
                        CORRIERE_SERVIZIO_SPEDIZIONE corriereSpedizione = spedizione.CORRIERE_SERVIZIO_SPEDIZIONE;
                        if (corriereSpedizione != null) {
                            viewModel.NomeCorriere = corriereSpedizione.CORRIERE_SERVIZIO.CORRIERE.NOME;
                            viewModel.PuntiSpedizione = spedizione.PUNTI.ToHappyCoin();
                            viewModel.SoldiSpedizione = spedizione.SOLDI.ToString("C");
                            // se è stato generato LDV e caricato su G4G
                            ALLEGATO ldv = corriereSpedizione.ALLEGATO;
                            if (ldv != null)
                            {
                                viewModel.LDVNome = ldv.NOME;
                                
                                PersonaModel utente = (HttpContext.Current.Session["utente"] as PersonaModel);
                                if (utente != null)
                                {
                                    viewModel.LDVFile = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) +
                                        "/Uploads/Text/" + utente.Persona.TOKEN.ToString() + "/" + DateTime.Now.Year.ToString() +
                                        "/Original/" + ldv.NOME;
                                }
                            }
                        }
                    }
                }
            }
            return viewModel;
        }

        private AnnuncioViewModel SetInfoCategoriaOggetto(DatabaseContext db, OggettoViewModel oggettoView)
        {
            // gestito cosi, perchè nel caso faccio macroricerche, recupero lo stesso i dati personalizzati
            // sulla specifica sottocategoria.
            // new List<int> { 14 }.IndexOf(oggetto.CategoriaID) != 1
            if (oggettoView.CategoriaID == 12)
            {
                TelefonoViewModel viewModel = new TelefonoViewModel(oggettoView);
                OGGETTO_TELEFONO model = db.OGGETTO_TELEFONO.Where(m => m.ID_OGGETTO == oggettoView.OggettoId).FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                if (viewModel.modelloID != null)
                    viewModel.modelloNome = model.MODELLO.NOME;
                viewModel.sistemaOperativoID = model.ID_SISTEMA_OPERATIVO;
                if (viewModel.sistemaOperativoID != null)
                    viewModel.sistemaOperativoNome = model.SISTEMA_OPERATIVO.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID == 64)
            {
                ConsoleViewModel viewModel = new ConsoleViewModel(oggettoView);
                OGGETTO_CONSOLE model = db.OGGETTO_CONSOLE.Where(m => m.ID_OGGETTO == oggettoView.OggettoId).FirstOrDefault();
                viewModel.piattaformaID = model.ID_PIATTAFORMA;
                if (viewModel.piattaformaID != null)
                    viewModel.piattaformaNome = model.PIATTAFORMA.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID == 13 || (oggettoView.CategoriaID >= 62 && oggettoView.CategoriaID <= 63) || oggettoView.CategoriaID == 65)
            {
                ModelloViewModel viewModel = new ModelloViewModel(oggettoView);
                OGGETTO_TECNOLOGIA model = db.OGGETTO_TECNOLOGIA.Where(m => m.ID_OGGETTO == oggettoView.OggettoId).FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                if (viewModel.modelloID != null)
                    viewModel.modelloNome = model.MODELLO.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID == 14)
            {
                ComputerViewModel viewModel = new ComputerViewModel(oggettoView);
                OGGETTO_COMPUTER model = db.OGGETTO_COMPUTER.Where(m => m.ID_OGGETTO == oggettoView.OggettoId).FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                if (viewModel.modelloID != null)
                    viewModel.modelloNome = model.MODELLO.NOME;
                viewModel.sistemaOperativoID = model.ID_SISTEMA_OPERATIVO;
                if (viewModel.sistemaOperativoID != null)
                    viewModel.sistemaOperativoNome = model.SISTEMA_OPERATIVO.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID == 26)
            {
                ModelloViewModel viewModel = new ModelloViewModel(oggettoView);
                OGGETTO_ELETTRODOMESTICO model = db.OGGETTO_ELETTRODOMESTICO.Where(m => m.ID_OGGETTO == oggettoView.OggettoId).FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                if (viewModel.modelloID != null)
                    viewModel.modelloNome = model.MODELLO.NOME;
                return viewModel;
            }
            else if ((oggettoView.CategoriaID >= 28 && oggettoView.CategoriaID <= 39) || oggettoView.CategoriaID == 41)
            {
                MusicaViewModel viewModel = new MusicaViewModel(oggettoView);
                OGGETTO_MUSICA model = db.OGGETTO_MUSICA.Where(m => m.ID_OGGETTO == oggettoView.OggettoId).FirstOrDefault();
                viewModel.formatoID = model.ID_FORMATO;
                if (viewModel.formatoID != null)
                    viewModel.formatoNome = model.FORMATO.NOME;
                viewModel.artistaID = model.ID_ARTISTA;
                if (viewModel.artistaID != null)
                    viewModel.artistaNome = model.ARTISTA.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID == 40)
            {
                ModelloViewModel viewModel = new ModelloViewModel(oggettoView);
                OGGETTO_STRUMENTO model = db.OGGETTO_STRUMENTO.Where(m => m.ID_OGGETTO == oggettoView.OggettoId).FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                if (viewModel.modelloID != null)
                    viewModel.modelloNome = model.MODELLO.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID == 45)
            {
                VideogamesViewModel viewModel = new VideogamesViewModel(oggettoView);
                OGGETTO_VIDEOGAMES model = db.OGGETTO_VIDEOGAMES.Where(m => m.ID_OGGETTO == oggettoView.OggettoId).FirstOrDefault();
                viewModel.piattaformaID = model.ID_PIATTAFORMA;
                if (viewModel.piattaformaID != null)
                    viewModel.piattaformaNome = model.PIATTAFORMA.NOME;
                viewModel.genereID = model.ID_GENERE;
                if (viewModel.genereID != null)
                    viewModel.genereNome = model.GENERE.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID >= 42 && oggettoView.CategoriaID <= 47)
            {
                ModelloViewModel viewModel = new ModelloViewModel(oggettoView);
                OGGETTO_GIOCO model = db.OGGETTO_GIOCO.Where(m => m.ID_OGGETTO == oggettoView.OggettoId).FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                if (viewModel.modelloID != null)
                    viewModel.modelloNome = model.MODELLO.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID >= 50 && oggettoView.CategoriaID < 61)
            {
                ModelloViewModel viewModel = new ModelloViewModel(oggettoView);
                OGGETTO_SPORT model = db.OGGETTO_SPORT.Where(m => m.ID_OGGETTO == oggettoView.OggettoId).FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                if (viewModel.modelloID != null)
                    viewModel.modelloNome = model.MODELLO.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID >= 67 && oggettoView.CategoriaID < 80)
            {
                VideoViewModel viewModel = new VideoViewModel(oggettoView);
                OGGETTO_VIDEO model = db.OGGETTO_VIDEO.Where(m => m.ID_OGGETTO == oggettoView.OggettoId).FirstOrDefault();
                viewModel.formatoID = model.ID_FORMATO;
                if (viewModel.formatoID != null)
                    viewModel.formatoNome = model.FORMATO.NOME;
                viewModel.registaID = model.ID_REGISTA;
                if (viewModel.registaID != null)
                    viewModel.registaNome = model.REGISTA.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID >= 81 && oggettoView.CategoriaID <= 85)
            {
                LibroViewModel viewModel = new LibroViewModel(oggettoView);
                OGGETTO_LIBRO model = db.OGGETTO_LIBRO.Where(m => m.ID_OGGETTO == oggettoView.OggettoId).FirstOrDefault();
                viewModel.autoreID = model.ID_AUTORE;
                if (viewModel.autoreID != null)
                    viewModel.autoreNome = model.AUTORE.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID >= 89 && oggettoView.CategoriaID < 93)
            {
                VeicoloViewModel viewModel = new VeicoloViewModel(oggettoView);
                OGGETTO_VEICOLO model = db.OGGETTO_VEICOLO.Where(m => m.ID_OGGETTO == oggettoView.OggettoId).FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                if (viewModel.modelloID != null)
                    viewModel.modelloNome = model.MODELLO.NOME;
                viewModel.alimentazioneID = model.ID_ALIMENTAZIONE;
                if (viewModel.alimentazioneID != null)
                    viewModel.alimentazioneNome = model.ALIMENTAZIONE.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID >= 127 && oggettoView.CategoriaID <= 170 && oggettoView.CategoriaID != 161 && oggettoView.CategoriaID != 152 && oggettoView.CategoriaID != 141 && oggettoView.CategoriaID != 127)
            {
                VestitoViewModel viewModel = new VestitoViewModel(oggettoView);
                OGGETTO_VESTITO model = db.OGGETTO_VESTITO.Where(m => m.ID_OGGETTO == oggettoView.OggettoId).FirstOrDefault();
                viewModel.taglia = model.TAGLIA;
                return viewModel;
            }

            return oggettoView;
        }

        private AnnuncioViewModel SetServizioViewModel(DatabaseContext db, AnnuncioViewModel servizio, ANNUNCIO vendita)
        {
            ServizioViewModel viewModel = new ServizioViewModel(servizio);
            viewModel.Lunedi = vendita.SERVIZIO.LUNEDI;
            viewModel.Martedi = vendita.SERVIZIO.MARTEDI;
            viewModel.Mercoledi = vendita.SERVIZIO.MERCOLEDI;
            viewModel.Giovedi = vendita.SERVIZIO.GIOVEDI;
            viewModel.Venerdi = vendita.SERVIZIO.VENERDI;
            viewModel.Sabato = vendita.SERVIZIO.SABATO;
            viewModel.Domenica = vendita.SERVIZIO.DOMENICA;
            if (vendita.SERVIZIO.TUTTI != null)
                viewModel.Tutti = (bool)vendita.SERVIZIO.TUTTI;
            viewModel.OraInizio = vendita.SERVIZIO.ORA_INIZIO_FERIALI;
            viewModel.OraFine = vendita.SERVIZIO.ORA_FINE_FERIALI;
            if (vendita.SERVIZIO.ORA_INIZIO_FESTIVI != null)
                viewModel.OraInizioFestivita = (TimeSpan)vendita.SERVIZIO.ORA_INIZIO_FESTIVI;
            if (vendita.SERVIZIO.ORA_FINE_FESTIVI != null)
                viewModel.OraFineFestivita = (TimeSpan)vendita.SERVIZIO.ORA_FINE_FESTIVI;
            viewModel.RisultatiFinali = vendita.SERVIZIO.RISULTATI_FINALI;
            viewModel.ServiziOfferti = vendita.SERVIZIO.SERVIZI_OFFERTI;
            viewModel.Tariffa = (Tariffa)vendita.SERVIZIO.TARIFFA;
            return viewModel;
        }

        // praticamente non usata
        private AnnuncioViewModel SetInfoCategoriaServizio(DatabaseContext db, ServizioViewModel servizio)
        {
            // gestito cosi, perchè nel caso faccio macroricerche, recupero lo stesso i dati personalizzati
            // sulla specifica sottocategoria.
            switch (servizio.CategoriaID)
            {
                default:
                    break;
            }
            return servizio;
        }

        private void SetFeedbackVenditore(DatabaseContext db, AnnuncioViewModel viewModel)
        {
            try
            {
                List<int> voti = db.ANNUNCIO_FEEDBACK
                                .Where(item => (item.ANNUNCIO.ID_ATTIVITA == viewModel.Venditore.Id) ||
                                    (item.ANNUNCIO.ID_ATTIVITA == null && item.ANNUNCIO.ID_PERSONA == viewModel.Venditore.Id)
                                ).Select(item => item.VOTO).ToList();

                int votoMassimo = voti.Count * 10;
                if (voti.Count <= 0)
                {
                    viewModel.VenditoreFeedback = -1;
                }
                else
                {
                    int x = voti.Sum() / votoMassimo;
                    viewModel.VenditoreFeedback = x * 100;
                }
            }
            catch (Exception eccezione)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
                viewModel.VenditoreFeedback = -1;
            }
        }

        private bool SalvaCartaCredito(DatabaseContext db, AcquistoViewModel viewModel, PersonaModel utente)
        {
            PERSONA_METODO_PAGAMENTO metodoPagamento = db.PERSONA_METODO_PAGAMENTO
                .OrderByDescending(m => m.DATA_INSERIMENTO)
                .FirstOrDefault(m => m.TIPO_CARTA == (int)viewModel.TipoCarta && m.NUMERO.Equals(viewModel.NumeroCarta));
            if (metodoPagamento == null)
            {
                metodoPagamento = new PERSONA_METODO_PAGAMENTO();
                metodoPagamento.ID_PERSONA = utente.Persona.ID;
                metodoPagamento.NUMERO = viewModel.NumeroCarta;
                metodoPagamento.CVV2 = viewModel.Cvv2;
                metodoPagamento.MESE = (int)viewModel.MeseScadenzaCarta;
                metodoPagamento.ANNO = viewModel.AnnoScadenzaCarta;
                metodoPagamento.NOME = viewModel.NomeTitolareCarta;
                metodoPagamento.COGNOME = viewModel.CognomeTitolareCarta;
                metodoPagamento.TIPO_CARTA = (int)viewModel.TipoCarta;
                metodoPagamento.DATA_INSERIMENTO = DateTime.Now;
                metodoPagamento.STATO = (int)Stato.ATTIVO;
                metodoPagamento.PAYER_ID = this.PayerId;
                db.PERSONA_METODO_PAGAMENTO.Add(metodoPagamento);
            }
            else if (metodoPagamento.ID_PERSONA == utente.Persona.ID)
            {
                metodoPagamento.CVV2 = viewModel.Cvv2;
                metodoPagamento.MESE = (int)viewModel.MeseScadenzaCarta;
                metodoPagamento.ANNO = viewModel.AnnoScadenzaCarta;
                metodoPagamento.NOME = viewModel.NomeTitolareCarta;
                metodoPagamento.COGNOME = viewModel.CognomeTitolareCarta;
                metodoPagamento.TIPO_CARTA = (int)viewModel.TipoCarta;
                metodoPagamento.DATA_MODIFICA = DateTime.Now;
                metodoPagamento.PAYER_ID = this.PayerId;
                db.PERSONA_METODO_PAGAMENTO.Attach(metodoPagamento);
                db.Entry(metodoPagamento).State = EntityState.Modified;
            }
            return (db.SaveChanges() > 0);
        }

        private void AvviaSpedizione(DatabaseContext db, CORRIERE_SERVIZIO_SPEDIZIONE datiSpedizione, INDIRIZZO indirizzo, PersonaModel utente, int numTentativo = 1)
        {
            // effettua spedizione realmente!
            var puntoSpedizione = db.CORRIERE_PUNTO_SPEDIZIONE.FirstOrDefault(m => m.ID_INDIRIZZO == datiSpedizione.ID_INDIRIZZO_MITTENTE && m.ID_CORRIERE == datiSpedizione.CORRIERE_SERVIZIO.ID_CORRIERE);
            if (puntoSpedizione == null)
            {
                if (!InserisciPuntoSpedizione(db, datiSpedizione, ref puntoSpedizione))
                {
                    throw new Emotion.Exceptions.ShipmentException(App_GlobalResources.ExceptionMessage.ShipmentDepartureError);
                }
            }
            var corriereComune = indirizzo.COMUNE.CORRIERE_COMUNE.FirstOrDefault(m => m.ID_CORRIERE == puntoSpedizione.ID_CORRIERE);
            var corriereNazione = indirizzo.COMUNE.PROVINCIA.REGIONE.NAZIONE.CORRIERE_NAZIONE.FirstOrDefault(m => m.ID_CORRIERE == puntoSpedizione.ID_CORRIERE);

            Emotion.Service emotion = new Emotion.Service();
            ShippingRequest shipping = new ShippingRequest();
            shipping.shippingserviceid = string.Empty;
            shipping.departureid = puntoSpedizione.ID_PUNTO_SPEDIZIONE;
            string countryIso = GetCountryISO(db, indirizzo.COMUNE, puntoSpedizione.ID_CORRIERE);
            shipping.deliveryaddress = new AddressRequest()
            {
                address = indirizzo.INDIRIZZO1,
                city = indirizzo.COMUNE.NOME,
                company = "",
                countryiso = countryIso,
                cityiso = GetStateISO(db, indirizzo.COMUNE, puntoSpedizione.ID_CORRIERE, countryIso),
                customsdescription = _AnnuncioOriginale.NOME,
                customsvalue = _AnnuncioOriginale.NOME,
                email = utente.Email.FirstOrDefault(m => m.TIPO == (int)TipoEmail.Registrazione).EMAIL,
                firstname = datiSpedizione.NOMINATIVO_DESTINATARIO.Split(' ')[0],
                lastname = datiSpedizione.NOMINATIVO_DESTINATARIO.Split(' ')[1],
                other = datiSpedizione.INFO_EXTRA_DESTINATARIO,
                phone = datiSpedizione.TELEFONO_DESTINATARIO,
                postcode = indirizzo.COMUNE.CAP
            };
            shipping.goodstype = Emotion.Type.GoodType.GENERICO;
            shipping.packagingtype = Emotion.Type.PackType.PACCO;
            PackagesRequest packages = new PackagesRequest();
            packages.SetHeightDecimal(Convert.ToDecimal(this.OGGETTO.ALTEZZA));
            packages.SetLengthDecimal(Convert.ToDecimal(this.OGGETTO.LUNGHEZZA));
            packages.SetWidthDecimal(Convert.ToDecimal(this.OGGETTO.LARGHEZZA));
            packages.SetWeightDecimal(Convert.ToDecimal(this.OGGETTO.PESO));
            shipping.packages = new List<PackagesRequest>()
            {
                packages
            };
            Emotion.Response.ShippingRequestResponse response = emotion.ShippingRequest(shipping);
            if (response.result.Equals("OK"))
            {
                // salvataggio id spedizione emotion
                datiSpedizione.STATO = (int)Stato.ATTIVO;
                datiSpedizione.EXTERNAL_ID_SPEDIZIONE = response.emotionreference;
                db.CORRIERE_SERVIZIO_SPEDIZIONE.Attach(datiSpedizione);
                db.Entry(datiSpedizione).State = EntityState.Modified;
                db.SaveChanges();
            }
            else if (response.result.Equals("KO"))
            {
                throw new Emotion.Exceptions.ShipmentException(response.errormessage);
            }
            else if (response.errorcode.Equals("500"))
            {
                InserisciPuntoSpedizione(db, datiSpedizione, ref puntoSpedizione);
                if (numTentativo == 3)
                {
                    throw new Emotion.Exceptions.ShipmentException(App_GlobalResources.ExceptionMessage.ShipmentFailed);
                }
                else
                {
                    AvviaSpedizione(db, datiSpedizione, indirizzo, utente, numTentativo + 1);
                }
            }
            else
            {
                throw new Emotion.Exceptions.ShipmentException(App_GlobalResources.ExceptionMessage.ShipmentError);
            }
        }

        private bool InserisciPuntoSpedizione(DatabaseContext db, CORRIERE_SERVIZIO_SPEDIZIONE datiSpedizione, ref CORRIERE_PUNTO_SPEDIZIONE puntoSpedizione)
        {
            DeparturePointRequest newDeparture = new DeparturePointRequest();
            newDeparture.firstname = datiSpedizione.NOMINATIVO_MITTENTE.Split(' ')[0];
            newDeparture.lastname = ((datiSpedizione.NOMINATIVO_MITTENTE.Split(' ').Count() > 1) ? datiSpedizione.NOMINATIVO_MITTENTE.Split(' ')[1] : "");
            newDeparture.address1 = datiSpedizione.INDIRIZZO.INDIRIZZO1 + " " + datiSpedizione.INDIRIZZO.CIVICO.ToString();
            newDeparture.city = datiSpedizione.INDIRIZZO.COMUNE.NOME;
            newDeparture.company = string.Empty;
            newDeparture.departurename = Guid.NewGuid().ToString().Replace("-", "");
            newDeparture.email = this.PERSONA.PERSONA_EMAIL.FirstOrDefault(m => m.TIPO == (int)TipoEmail.Registrazione && m.STATO == (int)Stato.ATTIVO).EMAIL;
            newDeparture.iso_country = GetCountryISO(db, datiSpedizione.INDIRIZZO.COMUNE, datiSpedizione.CORRIERE_SERVIZIO.ID_CORRIERE);
            newDeparture.iso_state = GetStateISO(db, datiSpedizione.INDIRIZZO.COMUNE, datiSpedizione.CORRIERE_SERVIZIO.ID_CORRIERE, newDeparture.iso_country);
            newDeparture.phone = datiSpedizione.TELEFONO_MITTENTE;
            newDeparture.postcode = datiSpedizione.INDIRIZZO.COMUNE.CAP;
            newDeparture.other = datiSpedizione.INFO_EXTRA_MITTENTE;

            Emotion.Service emotion = new Emotion.Service();
            Emotion.Response.NewDeparturePointResponse queryResult = emotion.NewDeparturePoint(newDeparture);

            if (queryResult.result != "OK")
            {
                throw new Emotion.Exceptions.ShipmentException(queryResult.errorMessage);
            }
            else if (puntoSpedizione == null)
            {
                puntoSpedizione = new CORRIERE_PUNTO_SPEDIZIONE();
                puntoSpedizione.ID_CORRIERE = datiSpedizione.CORRIERE_SERVIZIO.ID_CORRIERE;
                puntoSpedizione.ID_INDIRIZZO = (int)datiSpedizione.ID_INDIRIZZO_MITTENTE;
                puntoSpedizione.ID_PUNTO_SPEDIZIONE = queryResult.departureid;
                puntoSpedizione.DATA_INSERIMENTO = DateTime.Now;
                puntoSpedizione.STATO = (int)Stato.ATTIVO;
                db.CORRIERE_PUNTO_SPEDIZIONE.Add(puntoSpedizione);
                return (db.SaveChanges() > 0);
            }
            else
            {
                puntoSpedizione.ID_PUNTO_SPEDIZIONE = queryResult.departureid;
                db.CORRIERE_PUNTO_SPEDIZIONE.Attach(puntoSpedizione);
                db.Entry(puntoSpedizione).State = EntityState.Modified;
                return (db.SaveChanges() > 0);
            }
        }

        private string GetCountryISO(DatabaseContext db, COMUNE comune, int idCorriere)
        {
            var corriereNazione = comune.PROVINCIA.REGIONE.NAZIONE.CORRIERE_NAZIONE;
            if (corriereNazione != null && corriereNazione.Count() > 0)
            {
                return corriereNazione.FirstOrDefault(m => m.ID_CORRIERE == idCorriere).ISO;
            }
            else
            {
                Emotion.Service emotion = new Emotion.Service();
                Emotion.Response.CountriesResponse countries = emotion.Countries();
                CORRIERE_NAZIONE nazione = new CORRIERE_NAZIONE();
                nazione.ID_NAZIONE = (int)comune.PROVINCIA.REGIONE.ID_NAZIONE;
                nazione.ID_CORRIERE = idCorriere;
                nazione.NOME = comune.PROVINCIA.REGIONE.NAZIONE.NOME;
                nazione.STATO = (int)Stato.ATTIVO;
                nazione.DATA_INSERIMENTO = DateTime.Now;
                nazione.ISO = countries.country.First(m => m.Value.name.ToUpper() == nazione.NOME.ToUpper()).Value.iso_code;
                db.CORRIERE_NAZIONE.Add(nazione);
                if (db.SaveChanges() > 0)
                    return nazione.ISO;
            }
            return string.Empty;
        }

        private string GetStateISO(DatabaseContext db, COMUNE comune, int idCorriere, string country)
        {
            var corriereComune = comune.CORRIERE_COMUNE;
            if (corriereComune != null && corriereComune.Count() > 0)
            {
                return corriereComune.FirstOrDefault(m => m.ID_CORRIERE == idCorriere).ISO;
            }
            else
            {
                Emotion.Service emotion = new Emotion.Service();
                CountryIsoRequest countryIso = new CountryIsoRequest();
                countryIso.countryiso = country;
                Emotion.Response.StatesResponse states = emotion.States(countryIso);
                CORRIERE_COMUNE comune2 = new CORRIERE_COMUNE();
                comune2.ID_COMUNE = comune.ID;
                comune2.ID_CORRIERE = idCorriere;
                comune2.NOME = comune.PROVINCIA.NOME;
                comune2.STATO = (int)Stato.ATTIVO;
                comune2.DATA_INSERIMENTO = DateTime.Now;
                comune2.ISO = states.states.First(m => m.name.ToUpper() == comune.PROVINCIA.NOME.ToUpper()).iso_code;
                db.CORRIERE_COMUNE.Add(comune2);
                if (db.SaveChanges() > 0)
                    return comune2.ISO;
            }
            return string.Empty;
        }
        #endregion
    }
}