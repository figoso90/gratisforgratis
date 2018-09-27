using DotNetShipping;
using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Data.Entity;
using System.Linq;

namespace GratisForGratis.Models
{
    #region CLASSI PUBBLICHE
    public class AcquistoViewModel
    {
        #region COSTRUTTORI
        public AcquistoViewModel()
        {
            this.SetDefault();
        }
        public AcquistoViewModel(AnnuncioViewModel annuncio)
        {
            this.SetDefault();
            this.Annuncio = annuncio;
        }
        public AcquistoViewModel(OFFERTA offerta)
        {
            Token = offerta.ANNUNCIO.TOKEN.ToString();
            TipoCarta = TipoCartaCredito.PayPal;
            // recupero dati per spedizione
            TipoScambio = TipoScambio.Spedizione;
            OFFERTA_SPEDIZIONE spedizione = offerta.OFFERTA_SPEDIZIONE.SingleOrDefault();
            NominativoDestinatario = spedizione.NOMINATIVO_DESTINATARIO;
            TelefonoDestinatario = spedizione.TELEFONO_DESTINATARIO;
            InfoExtraDestinatario = spedizione.INFO_EXTRA;
            CapDestinatario = spedizione.INDIRIZZO.COMUNE.CAP;
            IndirizzoDestinatario = spedizione.INDIRIZZO.INDIRIZZO1;
            CivicoDestinatario = spedizione.INDIRIZZO.CIVICO;
        }
        #endregion

        #region PROPRIETA
        //public int Id { get; set; }

        [Required]
        public string Token { get; set; }

        public AnnuncioViewModel Annuncio { get; set; }

        public bool PagamentoFatto { get; set; }

        [Required]
        [Display(Name = "ExchangeMode", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public TipoScambio TipoScambio { get; set; }

        [RequiredIf("TipoScambio", TipoScambio.Spedizione, ErrorMessageResourceName = "RecipientCap", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [Display(Name = "RecipientCap", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public string CapDestinatario { get; set; }

        [RequiredIf("TipoScambio", TipoScambio.Spedizione, ErrorMessageResourceName = "RecipientAddress", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [Display(Name = "RecipientAddress", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public string IndirizzoDestinatario { get; set; }

        [RequiredIf("TipoScambio", TipoScambio.Spedizione, ErrorMessageResourceName = "RecipientCivic", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [Display(Name = "RecipientCivic", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public int CivicoDestinatario { get; set; }

        [RequiredIf("TipoScambio", TipoScambio.Spedizione, ErrorMessageResourceName = "RecipientName", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [Display(Name = "RecipientName", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public string NominativoDestinatario { get; set; }

        [RequiredIf("TipoScambio", TipoScambio.Spedizione, ErrorMessageResourceName = "RecipientTelephone", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [Display(Name = "RecipientTelephone", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public string TelefonoDestinatario { get; set; }

        [Display(Name = "RecipientInfo", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public string InfoExtraDestinatario { get; set; }

        //[RequiredIf("TipoScambio", TipoScambio.Spedizione, ErrorMessageResourceName = "KidCard", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [Display(Name = "KidCard", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public TipoCartaCredito TipoCarta { get; set; }

        // dati carta prepagata
        //[ExpressiveAnnotations.Attributes.RequiredIf("TipoScambio == TipoScambio.Spedizione && TipoCarta != TipoCartaCredito.PayPal", ErrorMessageResourceName = "Cvv2", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [Display(Name = "Cvv2", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public int? Cvv2 { get; set; }

        //[ExpressiveAnnotations.Attributes.RequiredIf("TipoScambio == TipoScambio.Spedizione && TipoCarta != TipoCartaCredito.PayPal", ErrorMessageResourceName = "NumberCard", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [Display(Name = "NumberCard", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public string NumeroCarta { get; set; }

        //[ExpressiveAnnotations.Attributes.RequiredIf("TipoScambio == TipoScambio.Spedizione && TipoCarta != TipoCartaCredito.PayPal", ErrorMessageResourceName = "MounthExpiredCard", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [Display(Name = "MounthExpiredCard", ResourceType = typeof(App_GlobalResources.ViewModel))]
        [Range(1, 12, ErrorMessageResourceName = "MounthExpiredCardRange", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        public Month? MeseScadenzaCarta { get; set; }

        //[ExpressiveAnnotations.Attributes.RequiredIf("TipoScambio == TipoScambio.Spedizione && TipoCarta != TipoCartaCredito.PayPal", ErrorMessageResourceName = "YearExpiredCardRange", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [Display(Name = "YearExpiredCard", ResourceType = typeof(App_GlobalResources.ViewModel))]
        [GratisForGratis.DataAnnotations.RangeYearPaymentCard(ErrorMessageResourceName = "YearExpiredCardRange", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        public int? AnnoScadenzaCarta { get; set; }

        //[ExpressiveAnnotations.Attributes.RequiredIf("TipoScambio == TipoScambio.Spedizione && TipoCarta != TipoCartaCredito.PayPal", ErrorMessageResourceName = "NamePaymentCard", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [Display(Name = "NamePaymentCard", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public string NomeTitolareCarta { get; set; }

        //[ExpressiveAnnotations.Attributes.RequiredIf("TipoScambio == TipoScambio.Spedizione && TipoCarta != TipoCartaCredito.PayPal", ErrorMessageResourceName = "SurnamePaymentCard", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [Display(Name = "SurnamePaymentCard", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public string CognomeTitolareCarta { get; set; }

        #endregion

        #region METODI PRIVATI
        public void SetDefault()
        {
            PersonaModel utente = (HttpContext.Current.Session["utente"] as PersonaModel);
            PERSONA_INDIRIZZO indirizzo = utente.Indirizzo.FirstOrDefault(m => m.TIPO == (int)TipoIndirizzo.Residenza);
            NominativoDestinatario = utente.NomeVisibile;
            PERSONA_TELEFONO telefono = utente.Telefono.FirstOrDefault(m => m.TIPO == (int)TipoTelefono.Privato);
            if (telefono != null) {
                TelefonoDestinatario = telefono.TELEFONO;
            }
            if (indirizzo != null)
            {
                CapDestinatario = indirizzo.INDIRIZZO.COMUNE.CAP;
                IndirizzoDestinatario = indirizzo.INDIRIZZO.INDIRIZZO1;
                CivicoDestinatario = indirizzo.INDIRIZZO.CIVICO;
            }
            PERSONA_INDIRIZZO indirizzoSpedizione = utente.Indirizzo.FirstOrDefault(m => m.TIPO == (int)TipoIndirizzo.Spedizione);
            if (indirizzoSpedizione != null && indirizzoSpedizione.PERSONA_INDIRIZZO_SPEDIZIONE != null && indirizzoSpedizione.PERSONA_INDIRIZZO_SPEDIZIONE.Count() > 0)
            {
                PERSONA_INDIRIZZO_SPEDIZIONE datiSpedizione = indirizzoSpedizione.PERSONA_INDIRIZZO_SPEDIZIONE.FirstOrDefault();
                NominativoDestinatario = datiSpedizione.NOMINATIVO;
                TelefonoDestinatario = datiSpedizione.TELEFONO;
                InfoExtraDestinatario = datiSpedizione.INFO_EXTRA;
                CapDestinatario = indirizzoSpedizione.INDIRIZZO.COMUNE.CAP;
                IndirizzoDestinatario = indirizzoSpedizione.INDIRIZZO.INDIRIZZO1;
                CivicoDestinatario = indirizzoSpedizione.INDIRIZZO.CIVICO;
            }

            NomeTitolareCarta = utente.Persona.NOME;
            CognomeTitolareCarta = utente.Persona.COGNOME;
            // caricare ultimo o i metodi di pagamento inseriti, caricandoli prima nella sessione utente
            PERSONA_METODO_PAGAMENTO metodoPagamento = utente.MetodoPagamento.LastOrDefault();
            if (metodoPagamento != null)
            {
                TipoCarta = (TipoCartaCredito)metodoPagamento.TIPO_CARTA;
                NumeroCarta = metodoPagamento.NUMERO;
                Cvv2 = (int)metodoPagamento.CVV2;
                MeseScadenzaCarta = (Month)metodoPagamento.MESE;
                AnnoScadenzaCarta = (int)metodoPagamento.ANNO;
                NomeTitolareCarta = metodoPagamento.NOME;
                CognomeTitolareCarta = metodoPagamento.COGNOME;
            }
        }
        #endregion
    }

    public class OfferteVenditaViewModel
    {
        #region COSTRUTTORI
        public OfferteVenditaViewModel()
        {
            Offerte = new List<OffertaViewModel>();
        }
        #endregion

        #region PROPRIETA
        /*public int IdInt
        {
            set
            {
                Id = Encode(value);
            }
        }*/
        public int Id { get; set; }

        public string Token { get; set; }

        [Required]
        [Display(Name = "Name", ResourceType = typeof(App_GlobalResources.Language))]
        public string NomeVendita { get; set; }

        [Required]
        [Display(Name = "StateSelling", ResourceType = typeof(App_GlobalResources.Language))]
        public StatoVendita StatoVendita { get; set; }

        [Required]
        [Display(Name = "InsertDate", ResourceType = typeof(App_GlobalResources.Language))]
        public DateTime? DataInserimento { get; set; }

        [Required]
        [Display(Name = "Offers", ResourceType = typeof(App_GlobalResources.Language))]
        public List<OffertaViewModel> Offerte { get; set; }
        #endregion
    }

    public class OffertaViewModel
    {
        #region COSTRUTTORI
        public OffertaViewModel()
        {
            Baratti = new List<AnnuncioViewModel>();
            BarattiToken = new List<string>();
            SetDefault();
            TipoOfferta = TipoPagamento.HAPPY;
        }
        public OffertaViewModel(AnnuncioViewModel annuncio)
        {
            Baratti = new List<AnnuncioViewModel>();
            BarattiToken = new List<string>();
            SetDefault();
            TipoOfferta = TipoPagamento.HAPPY;
            this.Annuncio = annuncio;
        }
        public OffertaViewModel(DatabaseContext db, OFFERTA model)
        {
            Baratti = new List<AnnuncioViewModel>();
            SetDefault();
            TipoOfferta = TipoPagamento.HAPPY;

            Id = model.ID;
            Token = Controllers.Utils.Encode(model.ID);
            Annuncio = new AnnuncioViewModel(db, model.ANNUNCIO);
            Punti = (int)model.PUNTI;
            Soldi = (int)model.SOLDI;
            Annuncio.Categoria = model.ANNUNCIO.CATEGORIA.NOME;
            Annuncio.Foto = model.ANNUNCIO.ANNUNCIO_FOTO.Select(f => new AnnuncioFoto()
            {
                ID_ANNUNCIO = f.ID_ANNUNCIO,
                ALLEGATO = f.ALLEGATO,
                DATA_INSERIMENTO = f.DATA_INSERIMENTO,
                DATA_MODIFICA = f.DATA_MODIFICA
            }).ToList();
            Baratti = model.OFFERTA_BARATTO
                .Where(b => b.ID_OFFERTA == model.ID && b.ANNUNCIO != null)
                .Select(b =>
                    new AnnuncioViewModel()
                    {
                        Token = b.ANNUNCIO.TOKEN.ToString(),
                        TipoAcquisto = b.ANNUNCIO.SERVIZIO != null ? TipoAcquisto.Servizio : TipoAcquisto.Oggetto,
                        Nome = b.ANNUNCIO.NOME,
                        Punti = b.ANNUNCIO.PUNTI,
                        Soldi = Convert.ToDecimal(b.ANNUNCIO.SOLDI).ToString("C"),
                    }).ToList();
            BarattiToken = Baratti.Select(m => m.Token.ToString()).ToList();
            Compratore = new UtenteVenditaViewModel(model.PERSONA);
            TipoOfferta = (TipoPagamento)model.TIPO_OFFERTA;
            TipoPagamento = (TipoPagamento)model.ANNUNCIO.TIPO_PAGAMENTO;
            StatoOfferta = (StatoOfferta)model.STATO;
            DataInserimento = (DateTime)model.DATA_INSERIMENTO;
            OFFERTA_SPEDIZIONE offertaSpedizione = model.OFFERTA_SPEDIZIONE.FirstOrDefault();
            TipoScambio = TipoScambio.AMano;
            if (offertaSpedizione != null) {
                TipoScambio = TipoScambio.Spedizione;
                INDIRIZZO indirizzo = offertaSpedizione.INDIRIZZO;
                CapDestinatario = indirizzo.COMUNE.CAP;
                IndirizzoDestinatario = indirizzo.INDIRIZZO1;
                CivicoDestinatario = indirizzo.CIVICO;
                NominativoDestinatario = offertaSpedizione.NOMINATIVO_DESTINATARIO;
                TelefonoDestinatario = offertaSpedizione.TELEFONO_DESTINATARIO;
                SoldiSpedizione = offertaSpedizione.SOLDI.ToString("C");
                ANNUNCIO_TIPO_SCAMBIO tipoScambio = model.ANNUNCIO.ANNUNCIO_TIPO_SCAMBIO.FirstOrDefault(m => m.TIPO_SCAMBIO == (int)TipoScambio.Spedizione);
                if (tipoScambio != null)
                {
                    NomeCorriere = tipoScambio.ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE.FirstOrDefault()
                        .CORRIERE_SERVIZIO_SPEDIZIONE.CORRIERE_SERVIZIO.CORRIERE.NOME;
                }
            }
        }
        #endregion

        #region PROPRIETA

        public int Id { get; set; }

        public string Token { get; set; }

        public AnnuncioViewModel Annuncio { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "OfferPoints", ResourceType = typeof(App_GlobalResources.Language))]
        public int Punti { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Money", ResourceType = typeof(App_GlobalResources.Language))]
        public int Soldi { get; set; }

        [Display(Name = "OfferKind", ResourceType = typeof(App_GlobalResources.Language))]
        public TipoPagamento TipoOfferta { get; set; }

        //[Required]
        [Display(Name = "PaymentMethods", ResourceType = typeof(App_GlobalResources.Language))]
        public TipoPagamento TipoPagamento { get; set; }

        //[Required]
        [Display(Name = "InsertDate", ResourceType = typeof(App_GlobalResources.Language))]
        public DateTime? DataInserimento { get; set; }

        //[Required]
        [Display(Name = "Seller", ResourceType = typeof(App_GlobalResources.Language))]
        public UtenteVenditaViewModel Compratore { get; set; }

        //[Required]
        [Display(Name = "StateBid", ResourceType = typeof(App_GlobalResources.Language))]
        public StatoOfferta StatoOfferta { get; set; }

        [Required(ErrorMessageResourceName = "FieldEmptyBarters", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        public List<AnnuncioViewModel> Baratti { get; set; }

        [Required(ErrorMessageResourceName = "FieldEmptyBarters", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        public List<string> BarattiToken { get; set; }

        // scelta spedizione per offerta in base alle possibilità messe dal venditore

        [Required]
        [Display(Name = "ExchangeMode", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public TipoScambio TipoScambio { get; set; }

        [RequiredIf("TipoScambio", TipoScambio.Spedizione, ErrorMessageResourceName = "RecipientCap", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [Display(Name = "RecipientCap", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public string CapDestinatario { get; set; }

        [RequiredIf("TipoScambio", TipoScambio.Spedizione, ErrorMessageResourceName = "RecipientAddress", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [Display(Name = "RecipientAddress", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public string IndirizzoDestinatario { get; set; }

        [RequiredIf("TipoScambio", TipoScambio.Spedizione, ErrorMessageResourceName = "RecipientCivic", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [Display(Name = "RecipientCivic", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public int CivicoDestinatario { get; set; }

        [RequiredIf("TipoScambio", TipoScambio.Spedizione, ErrorMessageResourceName = "RecipientName", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [Display(Name = "RecipientName", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public string NominativoDestinatario { get; set; }

        [RequiredIf("TipoScambio", TipoScambio.Spedizione, ErrorMessageResourceName = "RecipientTelephone", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [Display(Name = "RecipientTelephone", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public string TelefonoDestinatario { get; set; }

        [Display(Name = "RecipientInfo", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public string InfoExtraDestinatario { get; set; }

        public string SoldiSpedizione { get; set; }

        public string NomeCorriere { get; set; }
        #endregion

        #region METODI PUBBLICI
        public bool Save(DatabaseContext db)
        {
            string tokenDecodificato = HttpContext.Current.Server.UrlDecode(this.Annuncio.Token);
            Guid tokenGuid = Guid.Parse(tokenDecodificato);
            PersonaModel utente = (PersonaModel)HttpContext.Current.Session["utente"];
            int crediti = utente.ContoCorrente.Count(m => m.STATO == (int)StatoMoneta.ASSEGNATA);
            ANNUNCIO annuncio = db.ANNUNCIO.SingleOrDefault(m => m.TOKEN == tokenGuid && m.ID_PERSONA != utente.Persona.ID && m.STATO == (int)StatoVendita.ATTIVO
                && this.Punti <= crediti && m.OFFERTA.Count(o => o.ID_PERSONA == utente.Persona.ID && o.STATO != (int)StatoOfferta.ANNULLATA) <= 0
                && utente.Persona.STATO == (int)Stato.ATTIVO && ((m.OGGETTO != null && 1 <= m.OGGETTO.NUMERO_PEZZI) || (m.SERVIZIO != null))
                && (this.BarattiToken.Count > 0 || this.Punti > 0 ));

            if (annuncio != null)
            {
                // inserimento offerta
                OFFERTA offerta = new OFFERTA();
                offerta.ID_ANNUNCIO = annuncio.ID;
                offerta.ID_PERSONA = utente.Persona.ID;
                offerta.PUNTI = this.Punti;
                offerta.SOLDI = this.Soldi;
                offerta.TIPO_OFFERTA = (int)this.TipoOfferta;
                offerta.TIPO_TRATTATIVA = (int)this.TipoPagamento; // DA VERIFICARE CHE COSA SERVA
                offerta.DATA_INSERIMENTO = DateTime.Now;
                offerta.STATO = (int)StatoOfferta.ATTIVA;
                db.OFFERTA.Add(offerta);

                if (db.SaveChanges() > 0)
                {
                    // Inserimento dei crediti associati all'offerta
                    if (this.Punti > 0)
                    {
                        List<CONTO_CORRENTE_MONETA> lista = db.CONTO_CORRENTE_MONETA
                            .Where(m => m.ID_CONTO_CORRENTE == utente.Persona.ID_CONTO_CORRENTE && m.STATO == (int)StatoMoneta.ASSEGNATA)
                            .Take(this.Punti)
                            .ToList();

                        foreach (CONTO_CORRENTE_MONETA m in lista)
                        {
                            m.DATA_MODIFICA = DateTime.Now;
                            m.STATO = (int)StatoMoneta.SOSPESA;
                            db.CONTO_CORRENTE_MONETA.Attach(m);
                            db.Entry(m).State = EntityState.Modified;
                            if (db.SaveChanges() <= 0)
                            {
                                return false;
                            }
                        }
                    }
                    
                    // se sto offrendo un baratto, inserisco gli oggetti barattati
                    foreach (string annuncioDiScambio in this.BarattiToken)
                    {
                        OFFERTA_BARATTO offertaBaratto = new OFFERTA_BARATTO();
                        offertaBaratto.ID_OFFERTA = offerta.ID;
                        offertaBaratto.ID_ANNUNCIO = db.ANNUNCIO.SingleOrDefault(m => m.TOKEN.ToString() == annuncioDiScambio
                            && m.STATO == (int)StatoVendita.ATTIVO).ID;
                        offertaBaratto.DATA_INSERIMENTO = DateTime.Now;
                        offertaBaratto.STATO = (int)Stato.ATTIVO;
                        db.OFFERTA_BARATTO.Add(offertaBaratto);
                        if (db.SaveChanges() <= 0)
                            return false;
                    }

                    // aggiungere offerta_spedizione per salvare dati destinazione
                    if (this.TipoScambio == TipoScambio.Spedizione)
                    {
                        OFFERTA_SPEDIZIONE offertaSpedizione = new OFFERTA_SPEDIZIONE();
                        offertaSpedizione.ID_OFFERTA = offerta.ID;
                        INDIRIZZO indirizzo = db.INDIRIZZO.FirstOrDefault(m => m.INDIRIZZO1 == this.IndirizzoDestinatario && m.COMUNE.CAP == this.CapDestinatario && m.CIVICO == this.CivicoDestinatario);
                        if (indirizzo == null)
                        {
                            indirizzo = new INDIRIZZO();
                            indirizzo.INDIRIZZO1 = this.IndirizzoDestinatario;
                            indirizzo.CIVICO = this.CivicoDestinatario;
                            indirizzo.ID_COMUNE = db.COMUNE.FirstOrDefault(m => m.CAP == this.CapDestinatario).ID;
                            indirizzo.TIPOLOGIA = (int)TipoIndirizzo.Residenza;
                            indirizzo.DATA_INSERIMENTO = DateTime.Now;
                            indirizzo.STATO = (int)Stato.ATTIVO;
                            db.INDIRIZZO.Add(indirizzo);
                            db.SaveChanges();
                        }

                        var annuncioSpedizione = db.ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE.SingleOrDefault(m => m.ANNUNCIO_TIPO_SCAMBIO.ID_ANNUNCIO == annuncio.ID && m.ANNUNCIO_TIPO_SCAMBIO.TIPO_SCAMBIO == (int)TipoScambio.Spedizione);
                        if (annuncioSpedizione != null)
                        {
                            offertaSpedizione.SOLDI = annuncioSpedizione.SOLDI;
                        }
                        offertaSpedizione.ID_INDIRIZZO_DESTINATARIO = indirizzo.ID;
                        offertaSpedizione.NOMINATIVO_DESTINATARIO = this.NominativoDestinatario;
                        offertaSpedizione.TELEFONO_DESTINATARIO = this.TelefonoDestinatario;
                        TIPO_VALUTA tipoValuta = (HttpContext.Current.Application["tipoValuta"] as List<TIPO_VALUTA>).SingleOrDefault(m => m.SIMBOLO == System.Globalization.NumberFormatInfo.CurrentInfo.CurrencySymbol);
                        offertaSpedizione.ID_TIPO_VALUTA = tipoValuta.ID;
                        offertaSpedizione.DATA_INSERIMENTO = DateTime.Now;
                        offertaSpedizione.STATO = (int)Stato.ATTIVO;
                        db.OFFERTA_SPEDIZIONE.Add(offertaSpedizione);
                        if (db.SaveChanges() <= 0)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        public void SetDefault()
        {
            PersonaModel utente = (HttpContext.Current.Session["utente"] as PersonaModel);
            if (utente != null)
            {
                PERSONA_INDIRIZZO indirizzo = utente.Indirizzo.FirstOrDefault(m => m.TIPO == (int)TipoIndirizzo.Residenza);
                NominativoDestinatario = utente.NomeVisibile;
                PERSONA_TELEFONO telefono = utente.Telefono.FirstOrDefault(m => m.TIPO == (int)TipoTelefono.Privato);
                if (telefono != null)
                {
                    TelefonoDestinatario = telefono.TELEFONO;
                }
                if (indirizzo != null)
                {
                    CapDestinatario = indirizzo.INDIRIZZO.COMUNE.CAP;
                    IndirizzoDestinatario = indirizzo.INDIRIZZO.INDIRIZZO1;
                    CivicoDestinatario = indirizzo.INDIRIZZO.CIVICO;
                }
                PERSONA_INDIRIZZO indirizzoSpedizione = utente.Indirizzo.FirstOrDefault(m => m.TIPO == (int)TipoIndirizzo.Spedizione);
                if (indirizzoSpedizione != null && indirizzoSpedizione.PERSONA_INDIRIZZO_SPEDIZIONE != null && indirizzoSpedizione.PERSONA_INDIRIZZO_SPEDIZIONE.Count() > 0)
                {
                    PERSONA_INDIRIZZO_SPEDIZIONE datiSpedizione = indirizzoSpedizione.PERSONA_INDIRIZZO_SPEDIZIONE.FirstOrDefault();
                    NominativoDestinatario = datiSpedizione.NOMINATIVO;
                    TelefonoDestinatario = datiSpedizione.TELEFONO;
                    InfoExtraDestinatario = datiSpedizione.INFO_EXTRA;
                    CapDestinatario = indirizzoSpedizione.INDIRIZZO.COMUNE.CAP;
                    IndirizzoDestinatario = indirizzoSpedizione.INDIRIZZO.INDIRIZZO1;
                    CivicoDestinatario = indirizzoSpedizione.INDIRIZZO.CIVICO;
                }
            }
        }
        #endregion
    }
    /*
    public class OffertaAnnuncioViewModel
    {
        #region PROPRIETA
        [Required]
        public string Token { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Points", ResourceType = typeof(App_GlobalResources.Language))]
        public int Punti { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Money", ResourceType = typeof(App_GlobalResources.Language))]
        public int Soldi { get; set; }

        [Required]
        [Display(Name = "Shipment", ResourceType = typeof(App_GlobalResources.Language))]
        public Shipment Spedizione { get; set; }

        [Required]
        [Display(Name = "OfferKind", ResourceType = typeof(App_GlobalResources.Language))]
        public TipoOfferta TipoOfferta { get; set; }

        [Required]
        [Display(Name = "PaymentMethods", ResourceType = typeof(App_GlobalResources.Language))]
        public TipoPagamento TipoPagamento { get; set; }

        [Required]
        [Display(Name = "InsertDate", ResourceType = typeof(App_GlobalResources.Language))]
        public DateTime? DataInserimento { get; set; }

        [Required]
        [Display(Name = "Seller", ResourceType = typeof(App_GlobalResources.Language))]
        public UtenteVenditaViewModel Compratore { get; set; }

        [Required]
        [Display(Name = "StateBid", ResourceType = typeof(App_GlobalResources.Language))]
        public StatoOfferta StatoOfferta { get; set; }

        public List<string> TokenAnnunciBaratto { get; set; }
        #endregion

        #region METODI PUBBLICI
        public bool Save(DatabaseContext db)
        {
            string tokenDecodificato = HttpContext.Current.Server.UrlDecode(this.Token);
            string tokenPulito = tokenDecodificato.Substring(3).Substring(0, tokenDecodificato.Length - 6);
            Guid tokenGuid = Guid.Parse(tokenPulito);
            PersonaModel utente = (PersonaModel)HttpContext.Current.Session["utente"];
            ANNUNCIO annuncio = db.ANNUNCIO.SingleOrDefault(m => m.TOKEN == tokenGuid && m.ID_PERSONA != utente.Persona.ID && m.STATO == (int)StatoVendita.ATTIVO 
                && this.Punti <= utente.ContoCorrente.Count() && m.TIPO_PAGAMENTO == (int)this.TipoPagamento 
                && ((this.TipoPagamento == TipoPagamento.Vendita && m.PUNTI == this.Punti) || (this.TipoPagamento != TipoPagamento.Vendita && this.TokenAnnunciBaratto != null && this.TokenAnnunciBaratto.Count > 0))
                && m.OFFERTA.Count(o => o.ID_PERSONA == utente.Persona.ID && o.STATO != (int)StatoOfferta.ANNULLATA) <= 0
                && utente.Persona.STATO == (int)Stato.ATTIVO && ((m.OGGETTO != null && 1 <= m.OGGETTO.NUMERO_PEZZI) || (m.SERVIZIO != null)));
            if (annuncio!=null)
            {
                // controllo tipo offerta
                if (this.TipoOfferta == TipoOfferta.Punti)
                {
                    this.Punti = annuncio.PUNTI;
                    this.Soldi = annuncio.SOLDI;
                }
                // inserimento offerta
                OFFERTA offerta = new OFFERTA();
                offerta.ID_ANNUNCIO = annuncio.ID;
                offerta.ID_PERSONA = utente.Persona.ID;
                offerta.PUNTI = this.Punti;
                offerta.SOLDI = this.Soldi;
                offerta.TIPO_OFFERTA = (int)this.TipoOfferta;
                offerta.TIPO_TRATTATIVA = (int)this.TipoPagamento;
                offerta.DATA_INSERIMENTO = DateTime.Now;
                offerta.STATO = (int)StatoOfferta.ATTIVA;
                if (this.TipoPagamento == TipoPagamento.Vendita)
                    offerta.STATO = (int)StatoOfferta.ACCETTATA;

                db.OFFERTA.Add(offerta);
                if (db.SaveChanges() > 0)
                {
                    // Inserimento dei crediti associati all'offerta
                    if (this.Punti > 0)
                    {
                        List<CONTO_CORRENTE_MONETA> lista = db.CONTO_CORRENTE_MONETA
                            .Where(m => m.ID_CONTO_CORRENTE == utente.Persona.ID_CONTO_CORRENTE && m.STATO == (int)StatoMoneta.ASSEGNATA)
                            .Take(this.Punti)
                            .ToList();
                        
                        foreach(CONTO_CORRENTE_MONETA m in lista)
                        {
                            m.STATO = (int)StatoMoneta.SOSPESA;
                            db.CONTO_CORRENTE_MONETA.Attach(m);
                            if (db.SaveChanges() > 0)
                            {
                                OFFERTA_CONTO_CORRENTE_MONETA offertaConto = new OFFERTA_CONTO_CORRENTE_MONETA();
                                offertaConto.ID_OFFERTA = offerta.ID;
                                offertaConto.ID_CONTO_CORRENTE_MONETA = m.ID;
                                offertaConto.DATA_INSERIMENTO = DateTime.Now;
                                offertaConto.STATO = (int)Stato.ATTIVO;
                                db.OFFERTA_CONTO_CORRENTE_MONETA.Add(offertaConto);
                                if (db.SaveChanges() <= 0)
                                    return false;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    
                    if (this.TipoPagamento == TipoPagamento.Vendita && this.TipoOfferta == TipoOfferta.Punti) {
                        // se sta acquistando e fa l'offerta corretta in punti, sospendo la vendita dell'oggetto
                        annuncio.DATA_MODIFICA = DateTime.Now;
                        annuncio.STATO = (int)StatoVendita.SOSPESO;
                        db.ANNUNCIO.Attach(annuncio);
                        if (db.SaveChanges() <= 0)
                            return false;
                    }
                    else if (this.TipoOfferta == TipoOfferta.Baratto)
                    {
                        // se sto offrendo un baratto, inserisco gli oggetti barattati
                        foreach(string token in this.TokenAnnunciBaratto)
                        {
                            OFFERTA_BARATTO offertaBaratto = new OFFERTA_BARATTO();
                            offertaBaratto.ID_OFFERTA = offerta.ID;
                            offertaBaratto.ID_ANNUNCIO = db.ANNUNCIO.SingleOrDefault(m => m.TOKEN.ToString() == token
                                && m.STATO == (int)StatoVendita.ATTIVO).ID;
                            offertaBaratto.DATA_INSERIMENTO = DateTime.Now;
                            offertaBaratto.STATO = (int)Stato.ATTIVO;
                        }

                    }
                }
            }
            return false;
        }
        #endregion
    }
    */
    #endregion
}
