using Elmah;
using Emotion.Exceptions;
using Emotion.Request;
using Emotion.Type;
using Foolproof;
using GratisForGratis.App_GlobalResources;
using GratisForGratis.Components;
using GratisForGratis.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GratisForGratis.Models
{

    public abstract class PubblicaViewModel
    {
        #region PROPRIETA
        public DatabaseContext DbContext { get; set; }

        [Display(Name = "Category", ResourceType = typeof(Language))]
        [Range(1, 2147483647, ErrorMessageResourceName = "ErrorCategory", ErrorMessageResourceType = typeof(Language))]
        [Required(ErrorMessageResourceName = "ErrorRequiredCategory", ErrorMessageResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public int CategoriaId { get; set; }

        public string CategoriaNome { get; set; }

        [DataType(DataType.Text)]
        [Required]
        public string Citta { get; set; }

        [DataType(DataType.Text)]
        //[ListValidator(ErrorMessageResourceName = "ErrorRequiredPhote", ErrorMessageResourceType = typeof(Language))]
        [Required(ErrorMessageResourceName = "ErrorRequiredPhote", ErrorMessageResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public List<string> Foto { get; set; }

        [Range(1, 2147483647, ErrorMessageResourceName = "ErrorCity", ErrorMessageResourceType = typeof(Language))]
        [Required(ErrorMessageResourceName = "ErrorCity", ErrorMessageResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public int IDCitta { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "LblTitleAd", ResourceType = typeof(Language))]
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessageResourceName = "TextTooLong", ErrorMessageResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public virtual string Nome { get; set; }

        //[Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "OptionalNote", ResourceType = typeof(Language))]
        [StringLength(2000, MinimumLength = 5, ErrorMessageResourceName = "ErrorDescriptionPost", ErrorMessageResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public string NoteAggiuntive { get; set; }

        [Range(0, double.MaxValue, ErrorMessageResourceName = "ErrorPoints", ErrorMessageResourceType = typeof(Language))]
        //[Required]
        public decimal? Punti { get; set; }

        [Display(Name = "WebAddress", ResourceType = typeof(Language))]
        [Url(ErrorMessageResourceName = "ErrorAddress", ErrorMessageResourceType = typeof(Language))]
        public string SchedaProdotto { get; set; }

        [Display(Name = "LblRealCoin", ResourceType = typeof(Language))]
        [DataType(DataType.Currency, ErrorMessageResourceName = "ErrorMoney", ErrorMessageResourceType = typeof(Language))]
        [Range(0, double.MaxValue, ErrorMessageResourceName = "ErrorValueCoin", ErrorMessageResourceType = typeof(Language))]
        [Required]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public decimal Soldi { get; set; }

        [Display(Name = "PaymentMethodsAccepted", ResourceType = typeof(Language))]
        [Range(0, 2147483647, ErrorMessageResourceName = "ErrorTypePayment", ErrorMessageResourceType = typeof(Language))]
        [Required]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public TipoPagamento TipoPagamento { get; set; }

        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public int IdTipoValuta { get; set; }

        [Required]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public TipoAcquisto TipoPubblicazione { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Token", ResourceType = typeof(Language))]
        [StringLength(100, MinimumLength = 16, ErrorMessageResourceName = "TextTooLong", ErrorMessageResourceType = typeof(Language))]
        public string TokenOK { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "ModePublication", ResourceType = typeof(Language))]
        public Guid? Partner { get; set; }

        public int? IdAnnuncioPadre { get; set; }

        public int? IdAnnuncioOriginale { get; set; }

        [Display(Name = "NoBid", ResourceType = typeof(ViewModel))]
        public bool NoOfferte { get; set; }

        [Display(Name = "Resell", ResourceType = typeof(ViewModel))]
        public bool RimettiInVendita { get; set; }

        public IEnumerable<SelectListItem> ListaDurataInserzione { get; set; }

        [Display(Name = "LifeAdvertisment", ResourceType = typeof(ViewModel))]
        [Required(ErrorMessageResourceName = "LifeAdvertismentRequired", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public DurataAnnuncio DurataInserzione { get; set; }

        public Guid TokenUploadFoto { get; set; }
        #endregion

        #region COSTRUTTORI
        public PubblicaViewModel()
        {
            SetProprietaDefault();
        }

        public PubblicaViewModel(ANNUNCIO model)
        {
            SetProprietaDefault();
            this.IdAnnuncioPadre = model.ID;
            this.IdAnnuncioOriginale = (model.ID_ORIGINE!=null)?model.ID_ORIGINE:model.ID;
            this.CategoriaId = model.ID_CATEGORIA;
            this.CategoriaNome = model.CATEGORIA.NOME;
            //this.Citta = model.id
            this.Nome = model.NOME;
            this.NoteAggiuntive = model.NOTE_AGGIUNTIVE;
            this.Punti = model.PUNTI;
            this.SchedaProdotto = model.URL_SCHEDA;
            this.TipoPagamento = (TipoPagamento) model.TIPO_PAGAMENTO;
            this.TokenOK = model.TOKEN.ToString();
            try {
                this.Foto = model.ANNUNCIO_FOTO.Where(m => m.ID_ANNUNCIO == model.ID)
                    .Select(m => m.ALLEGATO.NOME).ToList();
            }
            catch (Exception eccezione)
            {
                ErrorSignal.FromCurrentContext().Raise(eccezione);
            }
            GetDatiDettaglio(model);

        }

        public PubblicaViewModel(PubblicaViewModel model)
        {
            SetProprietaDefault();
            this.CopyAttributes(model);
        }
        #endregion

        #region METODI PUBBLICI
        public void CopyAttributes<T>(T model) where T : PubblicaViewModel
        {
            PropertyInfo[] properties = model.GetType().GetProperties();
            for (int i = 0; i < (int)properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                this.GetType().GetProperty(propertyInfo.Name).SetValue(this, propertyInfo.GetValue(model));
            }
        }
        public void Update<T>(T model) where T : PubblicaCopiaViewModel
        {
            PropertyInfo[] properties = model.GetType().GetProperties();
            for (int i = 0; i < (int)properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                if (this.GetType().GetProperty(propertyInfo.Name)!=null)
                    this.GetType().GetProperty(propertyInfo.Name).SetValue(this, propertyInfo.GetValue(model));
            }
        }
        //public void CopyAttributes(PubblicaViewModel model)
        //{
        //    PropertyInfo[] properties = model.GetType().GetProperties();
        //    for (int i = 0; i < (int)properties.Length; i++)
        //    {
        //        PropertyInfo propertyInfo = properties[i];
        //        this.GetType().GetProperty(propertyInfo.Name).SetValue(this, propertyInfo.GetValue(model));
        //    }
        //}
        public void GetDatiDaDatabase(ANNUNCIO model)
        {
            CategoriaId = model.ID_CATEGORIA;
            CategoriaNome = model.CATEGORIA.NOME;

            this.GetDatiDettaglio(model);
        }
        #endregion

        #region METODI ASTRATTI
        public abstract void GetDatiDettaglio(ANNUNCIO model);
        public abstract bool SalvaAnnuncio(ControllerContext controller, ANNUNCIO vendita = null);
        protected abstract bool SalvaDettaglioPerTipologiaAnnuncio(DatabaseContext db, ANNUNCIO model);
        protected abstract bool SalvaDettaglioPerCategoriaAnnuncio(DatabaseContext db, ANNUNCIO model);
        //public abstract void Update(PubblicaCopiaViewModel viewModel);
        #endregion

        #region METODI PRIVATI
        private void SetProprietaDefault()
        {
            this.TipoPagamento = TipoPagamento.HAPPY;

            ListaDurataInserzione = System.Enum.GetValues(typeof(DurataAnnuncio)).Cast<DurataAnnuncio>().Select(v => new SelectListItem
            {
                Text = Components.EnumHelper<DurataAnnuncio>.GetDisplayValue(v),
                Value = ((int)v).ToString()
            }).ToList();

            PersonaModel persona = (HttpContext.Current.Session["utente"] as PersonaModel);
            PERSONA_INDIRIZZO indirizzo = persona.Indirizzo.Where(m => m.TIPO == (int)TipoIndirizzo.Residenza).SingleOrDefault();
            if (indirizzo != null)
            {
                this.Citta = indirizzo.INDIRIZZO.COMUNE.NOME;
                this.IDCitta = persona.Indirizzo.Where(m => m.TIPO == (int)TipoIndirizzo.Residenza).SingleOrDefault().INDIRIZZO.ID_COMUNE;
            }
            this.TokenUploadFoto = Guid.NewGuid();
        }
        #endregion
    }

    // pubblicazione rapida
    public class PubblicazioneViewModel : PubblicaViewModel
    {
        #region ATTRIBUTI
        private delegate bool SaveDettaglio(DatabaseContext db, PubblicaOggettoViewModel iModel, ANNUNCIO vendita);
        #endregion

        #region COSTRUTTORI
        public PubblicazioneViewModel() : base() { }
        public PubblicazioneViewModel(ANNUNCIO model) : base(model) { }
        public PubblicazioneViewModel(PubblicazioneViewModel model)
        {
            this.CopyAttributes(model);
        }
        #endregion

        #region METODI PUBBLICI
        public override void GetDatiDettaglio(ANNUNCIO model) { }
        public override bool SalvaAnnuncio(ControllerContext controller, ANNUNCIO vendita = null)
        {
            PersonaModel utente = ((PersonaModel)HttpContext.Current.Session["utente"]);
            if (vendita == null)
                vendita = new ANNUNCIO();
            if (HttpContext.Current.Session["portaleweb"] != null)
            {
                PortaleWebViewModel portale = (HttpContext.Current.Session["portaleweb"] as List<PortaleWebViewModel>).Where(p => p.Token == this.Partner.ToString()).SingleOrDefault();
                if (portale != null)
                    vendita.ID_ATTIVITA = Convert.ToInt32(portale.Id);
            }
            var ri = new System.Globalization.RegionInfo(System.Threading.Thread.CurrentThread.CurrentUICulture.LCID);
            TIPO_VALUTA tipoValuta = (HttpContext.Current.Application["tipoValuta"] as List<TIPO_VALUTA>)
                .SingleOrDefault(m => m.SIMBOLO.ToUpper() == ri.CurrencySymbol.ToUpper());
            //.SingleOrDefault(m => m.CODICE.ToUpper() == this.TipoPagamento.ToString().ToUpper());

            vendita.ID_PERSONA = utente.Persona.ID;
            vendita.ID_CATEGORIA = this.CategoriaId;
            vendita.NOME = this.Nome;
            vendita.ID_COMUNE = this.IDCitta;
            vendita.NOTE_AGGIUNTIVE = this.NoteAggiuntive;
            // se ho inserito il campo soldi, uso quello con la conversione attuale
            decimal creditoInserito = this.Soldi;
            if (this.Soldi>1)
                creditoInserito = Decimal.Divide(this.Soldi, Convert.ToInt32(ConfigurationManager.AppSettings["Conversione" + tipoValuta.CODICE.ToUpper()]));
            vendita.PUNTI = creditoInserito;
            vendita.TIPO_PAGAMENTO = (int)this.TipoPagamento;
            vendita.SOLDI = Utils.cambioValuta(vendita.PUNTI, tipoValuta.CODICE.ToUpper());
            this.IdTipoValuta = tipoValuta.ID;
            vendita.ID_TIPO_VALUTA = tipoValuta.ID;
            vendita.TOKEN = Guid.NewGuid();
            vendita.ID_PADRE = this.IdAnnuncioPadre;
            vendita.ID_ORIGINE = this.IdAnnuncioOriginale;
            vendita.DATA_INSERIMENTO = DateTime.Now;
            // salvataggio nuovi campi. Il campo spedizione salvarlo come bit, quindi convertire i vari campi bool in un unico bit
            vendita.DATA_AVVIO = DateTime.Now;
            vendita.DATA_FINE = DateTime.Now.AddMonths((int)this.DurataInserzione);
            vendita.NO_OFFERTE = Convert.ToInt32(this.NoOfferte);
            vendita.RIMETTI_IN_VENDITA = Convert.ToInt32(this.RimettiInVendita);
            if (Utils.IsUtenteAttivo(0))
                vendita.STATO = (int)StatoVendita.ATTIVO;
            else
                vendita.STATO = (int)StatoVendita.INATTIVO;

            if (this.TipoPagamento != TipoPagamento.HAPPY)
            {
                vendita.ID_COMMISSIONE = GetCommissioneAnnuncio(DbContext, (decimal)vendita.SOLDI);
            }

            // non funziona più perchè deve salvare prima il campo id_oggetto o id_servizio
            // per me la soluzione è salvare invece l'id annuncio nelle altre due tabelle e cambiare il metodo di visualizzazione
            DbContext.ANNUNCIO.Add(vendita);
            if (DbContext.SaveChanges() > 0)
            {
                if (SalvaDettaglioPerTipologiaAnnuncio(DbContext, vendita) && SalvaDettaglioPerCategoriaAnnuncio(DbContext, vendita))
                {
                    foreach (string nomeFoto in this.Foto)
                    {
                        // può avvenire bug inserimento senza foto, perchè non c'è controllo in caso di errore generico
                        // bisognerebbe portare il try e catch esternamente
                        AnnuncioFoto foto = new AnnuncioFoto();
                        foto.Add(DbContext, utente.Persona.TOKEN, vendita.ID, nomeFoto, TokenUploadFoto);
                    }
                    return true;
                }
            }
            return false;
        }
        public void InviaEmail(ControllerContext controller, ANNUNCIO model, PersonaModel utente)
        {
            try
            {
                string fotoNotifica = "/Uploads/Images/" + utente.Persona.TOKEN + "/" + DateTime.Now.Year.ToString() + "/Normal/" + this.Foto[0];
                EmailModel email = new EmailModel(controller);
                EmailController emailController = new EmailController();
                email.Body = emailController.RenderRazorViewToString(controller, "Email/NotificaRicerca", email.Layout, null);

                string nomeVendita = model.NOME;
                string tokenVendita = Utils.RandomString(3) + Utils.Encode(model.TOKEN.ToString()) + Utils.RandomString(3);

                System.Threading.Tasks.Task.Run(() => this.InviaNotifichePubblicazione(utente.Persona.ID, nomeVendita, tokenVendita, model.ID_CATEGORIA, "Oggetto", email));
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
        public bool IsAnnuncioCompleto()
        {
            PulisciListe();
            PropertyInfo[] properties = this.GetType().GetProperties()
                .Where(m => m.CustomAttributes.Count(m2 => 
                    m2.AttributeType == typeof(GratisForGratis.DataAnnotations.AnnuncioCompletoAttribute)) > 0
                ).ToArray();
            for (int i = 0; i < (int)properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];

                if (propertyInfo.GetValue(this) == null)
                {
                    return false;
                }
                else
                {
                    if (typeof(System.Collections.ICollection).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        System.Collections.ICollection lista = (System.Collections.ICollection)propertyInfo.GetValue(this);
                        if (lista.Count <= 0)
                            return false;
                    }
                }
            }
            return true;
        }
        public void CancelUploadFoto(ANNUNCIO vendita)
        {
            PersonaModel utente = ((PersonaModel)HttpContext.Current.Session["utente"]);
            foreach (string nomeFoto in this.Foto)
            {
                AnnuncioFoto foto = new AnnuncioFoto();
                foto.Cancel(DbContext, utente.Persona.TOKEN, vendita.ID, nomeFoto, TokenUploadFoto);
            }
        }
        #endregion

        #region METODI PROTETTI
        protected override bool SalvaDettaglioPerTipologiaAnnuncio(DatabaseContext db, ANNUNCIO model)
        {
            return true;
        }
        protected override bool SalvaDettaglioPerCategoriaAnnuncio(DatabaseContext db, ANNUNCIO model)
        {
            return true;
        }

        protected virtual void PulisciListe()
        {
            Foto.RemoveAll(m => string.IsNullOrWhiteSpace(m));
        }
        #endregion

        #region METODI PRIVATI
        private async void InviaNotifichePubblicazione(int idUtente, string nomeVendita, string tokenVendita, int categoria, string tipologia, EmailModel email)
        {
            await System.Threading.Tasks.Task.Delay(10);
            using (DatabaseContext db2 = new DatabaseContext())
            {
                double maxGiorniRicerca = Convert.ToInt32(
                        System.Web.Configuration.WebConfigurationManager.AppSettings["maxGiorniRicerca"]
                        );
                string tokenVenditaCodificato = HttpUtility.UrlEncode(tokenVendita);

                db2.PERSONA_RICERCA.Where(r => (r.RICERCA.ID_CATEGORIA == categoria || r.RICERCA.CATEGORIA.ID_PADRE == categoria || r.RICERCA.ID_CATEGORIA == 1) && (r.RICERCA.NOME.Contains(nomeVendita) || nomeVendita.Contains(r.RICERCA.NOME)) &&
                    r.ID_PERSONA != idUtente && DbFunctions.DiffDays(DateTime.Now, r.RICERCA.DATA_INSERIMENTO) < maxGiorniRicerca)
                    .ToList().ForEach(r =>
                    {
                        string tokenRicerca = Utils.RandomString(3) + Utils.Encode(r.ID.ToString()) + Utils.RandomString(3);
                        string tokenRicercaCodificato = HttpUtility.UrlEncode(tokenRicerca);
                        string indirizzoEmail = r.PERSONA.PERSONA_EMAIL.SingleOrDefault(item => item.TIPO == (int)TipoEmail.Registrazione).EMAIL;
                        string emailCodificata = HttpUtility.UrlEncode(indirizzoEmail);
                        email.To.Add(new System.Net.Mail.MailAddress(indirizzoEmail, r.PERSONA.NOME + ' ' + r.PERSONA.COGNOME));
                        email.Subject = string.Format(Email.SearchNotifySubject, r.RICERCA.NOME) + " - " + WebConfigurationManager.AppSettings["nomeSito"];
                        email.Body = string.Format(HttpUtility.UrlDecode(email.Body), r.RICERCA.NOME, tipologia, tokenVenditaCodificato, tokenRicercaCodificato, nomeVendita, emailCodificata);
                        new EmailController().SendEmailByThread(email);
                    });
            }
        }

        /*DEPRECATED*/
        private int SaveBonus(DatabaseContext db, PersonaModel utente)
        {
            bool risultato = false;
            TRANSAZIONE bonusAnnuncioCompleto = null;
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
                bonus = new TRANSAZIONE()
                {
                    ID_CONTO_MITTENTE = idContoCorrente,
                    ID_CONTO_DESTINATARIO = utente.Persona.ID_CONTO_CORRENTE,
                    TIPO = (int)TipoBonus.PubblicazioneIniziale,
                    NOME = Bonus.InitialPubblication,
                    PUNTI = Convert.ToInt32(ConfigurationManager.AppSettings["bonusPubblicazioniIniziali"]),
                    DATA_INSERIMENTO = DateTime.Now,
                    STATO = (int)Stato.ATTIVO,
                };
                db.TRANSAZIONE.Add(bonus);
                if (db.SaveChanges() > 0)
                {
                    numeroPuntiGuadagnati += (int)bonus.PUNTI;
                    risultato = risultato | true;                    
                }
            }

            // aggiunge bonus se l'annuncio è completo di tutti i dati
            if (this.IsAnnuncioCompleto())
            {
                bonusAnnuncioCompleto = new TRANSAZIONE()
                {
                    ID_CONTO_MITTENTE = idContoCorrente,
                    ID_CONTO_DESTINATARIO = utente.Persona.ID_CONTO_CORRENTE,
                    TIPO = (int)TipoBonus.AnnuncioCompleto,
                    NOME = Bonus.FullAnnouncement,
                    PUNTI = Convert.ToInt32(ConfigurationManager.AppSettings["bonusAnnuncioCompleto"]),
                    DATA_INSERIMENTO = DateTime.Now,
                    STATO = (int)Stato.ATTIVO,
                };
                db.TRANSAZIONE.Add(bonusAnnuncioCompleto);
                //return (db.SaveChanges() > 0);
                if (db.SaveChanges() > 0)
                {
                    numeroPuntiGuadagnati += (int)bonusAnnuncioCompleto.PUNTI;
                    risultato = risultato | true;
                }
            }

            return ((risultato)? numeroPuntiGuadagnati : 0);
            //return ((risultato) ? (int)bonus.PUNTI : 0);
        }

        private int GetCommissioneAnnuncio(DatabaseContext db, decimal importo)
        {
            decimal percentuale = Convert.ToDecimal(ConfigurationManager.AppSettings["annuncioMonetaRealePercentuale"]);
            COMMISSIONE commissione = new COMMISSIONE();
            commissione.PERCENTUALE = percentuale;
            commissione.TIPO = (int)TipoCommissione.Annuncio;
            commissione.DATA_INSERIMENTO = DateTime.Now;
            commissione.STATO = (int)Stato.ATTIVO;
            db.COMMISSIONE.Add(commissione);
            db.SaveChanges();
            return commissione.ID;
        }
        #endregion
    }

    public abstract class PubblicaCopiaViewModel
    {
        #region PROPRIETA
        [DataType(DataType.Text)]
        [Required]
        public string Citta { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessageResourceName = "ErrorRequiredPhote", ErrorMessageResourceType = typeof(Language))]
        public List<string> Foto { get; set; }

        [Range(1, 2147483647, ErrorMessageResourceName = "ErrorCity", ErrorMessageResourceType = typeof(Language))]
        [Required(ErrorMessageResourceName = "ErrorCity", ErrorMessageResourceType = typeof(Language))]
        public int IDCitta { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "OptionalNote", ResourceType = typeof(Language))]
        [StringLength(2000, MinimumLength = 5, ErrorMessageResourceName = "ErrorDescriptionPost", ErrorMessageResourceType = typeof(Language))]
        public string NoteAggiuntive { get; set; }

        [Display(Name = "LblRealCoin", ResourceType = typeof(Language))]
        [DataType(DataType.Currency, ErrorMessageResourceName = "ErrorMoney", ErrorMessageResourceType = typeof(Language))]
        [Range(0, 2147483647, ErrorMessageResourceName = "ErrorValueCoin", ErrorMessageResourceType = typeof(Language))]
        [Required]
        public decimal Soldi { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Token", ResourceType = typeof(Language))]
        [StringLength(100, MinimumLength = 16, ErrorMessageResourceName = "TextTooLong", ErrorMessageResourceType = typeof(Language))]
        public string TokenOK { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "ModePublication", ResourceType = typeof(Language))]
        public Guid? Partner { get; set; }

        public UtenteVenditaViewModel Venditore { get; set; }

        [DataType(DataType.DateTime, ErrorMessageResourceName = "ErrorDate", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public DateTime? DataInserimento { get; set; }

        public Guid TokenUploadFoto { get; set; }

        [Display(Name = "NoBid", ResourceType = typeof(ViewModel))]
        public bool NoOfferte { get; set; }

        [Display(Name = "Resell", ResourceType = typeof(ViewModel))]
        public bool RimettiInVendita { get; set; }

        public IEnumerable<SelectListItem> ListaDurataInserzione { get; set; }

        [Display(Name = "LifeAdvertisment", ResourceType = typeof(ViewModel))]
        [Required(ErrorMessageResourceName = "LifeAdvertismentRequired", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        public DurataAnnuncio DurataInserzione { get; set; }
        #endregion

        #region COSTRUTTORI
        public PubblicaCopiaViewModel()
        {
            ListaDurataInserzione = System.Enum.GetValues(typeof(DurataAnnuncio)).Cast<DurataAnnuncio>().Select(v => new SelectListItem
            {
                Text = Components.EnumHelper<DurataAnnuncio>.GetDisplayValue(v),
                Value = ((int)v).ToString()
            }).ToList();
            this.TokenUploadFoto = Guid.NewGuid();
        }
        public PubblicaCopiaViewModel(ANNUNCIO model)
        {
            ListaDurataInserzione = System.Enum.GetValues(typeof(DurataAnnuncio)).Cast<DurataAnnuncio>().Select(v => new SelectListItem
            {
                Text = Components.EnumHelper<DurataAnnuncio>.GetDisplayValue(v),
                Value = ((int)v).ToString()
            }).ToList();
            this.TokenUploadFoto = Guid.NewGuid();
            this.IDCitta = (int)model.ID_COMUNE;
            this.Citta = model.COMUNE.NOME;
            this.NoteAggiuntive = model.NOTE_AGGIUNTIVE;
            this.TokenOK = model.TOKEN.ToString();
            this.DataInserimento = model.DATA_INSERIMENTO;
            this.Venditore = new UtenteVenditaViewModel();
            this.Venditore.Id = model.ID_PERSONA;
            this.Venditore.Nominativo = model.PERSONA.NOME + ' ' + model.PERSONA.COGNOME;
            this.Venditore.VenditoreToken = model.PERSONA.TOKEN;
            try
            {
                this.Foto = model.ANNUNCIO_FOTO.Where(m => m.ID_ANNUNCIO == model.ID)
                    .Select(m => m.ALLEGATO.NOME).ToList();
            }
            catch (Exception eccezione)
            {
                ErrorSignal.FromCurrentContext().Raise(eccezione);
            }
            GetDatiDettaglio(model);
        }
        #endregion

        #region METODI ASTRATTI
        public abstract void GetDatiDettaglio(ANNUNCIO model);
        #endregion
    }

    public class PubblicaOggettoViewModel : PubblicazioneViewModel
    {
        #region ATTRIBUTI
        private List<string> _materiali = new List<string>();
        private List<string> _componenti = new List<string>();
        #endregion

        #region PROPRIETA
        [Range(0, 2147483647, ErrorMessageResourceName = "ErrorRange", ErrorMessageResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public int? Anno { get; set; }

        [Range(0, 2147483647, ErrorMessageResourceName = "ErrorRange", ErrorMessageResourceType = typeof(Language))]
        public decimal? Altezza { get; set; }

        [Range(0, 2147483647, ErrorMessageResourceName = "ErrorRange", ErrorMessageResourceType = typeof(Language))]
        public decimal? Lunghezza { get; set; }

        [Range(0, 2147483647, ErrorMessageResourceName = "ErrorRange", ErrorMessageResourceType = typeof(Language))]
        public decimal? Larghezza { get; set; }

        [Range(0, 2147483647, ErrorMessageResourceName = "ErrorRange", ErrorMessageResourceType = typeof(Language))]
        public decimal? Peso { get; set; }

        public string Colore { get; set; }

        [Display(Name = "StateObject", ResourceType = typeof(Language))]
        [Required]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public CondizioneOggetto CondizioneOggetto { get; set; }

        //[Required]
        public virtual string Marca { get; set; }

        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public int? MarcaID { get; set; }

        [Display(Name = "Quantity", ResourceType = typeof(Language))]
        [Range(1, 2147483647, ErrorMessageResourceName = "ErrorQuantity", ErrorMessageResourceType = typeof(Language))]
        [Required]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public int Quantità { get; set; }

        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public List<string> Materiali
        {
            get
            {
                return _materiali;
            }
            set
            {
                _materiali = value;
            }
        }

        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public List<string> Componenti
        {
            get
            {
                return _componenti;
            }
            set
            {
                _componenti = value;
            }
        }

        // nuovi campi
        [Display(Name = "ExchangeHand", ResourceType = typeof(ViewModel))]
        [Required(ErrorMessageResourceName = "ExchangeModeRequired", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public bool ScambioAMano { get; set; }

        [Display(Name = "ExchangeShip", ResourceType = typeof(ViewModel))]
        [Required(ErrorMessageResourceName = "ExchangeModeRequired", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public bool ScambioConSpedizione { get; set; }

        public List<SelectListItem> ListaTipoSpedizione { get; set; }

        [Display(Name = "ShipmentMode", ResourceType = typeof(ViewModel))]
        [Required(ErrorMessageResourceName = "ShipmentModeRequired", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        public int TipoSpedizione { get; set; }
        /*
        [Display(Name = "ShipmentHand", ResourceType = typeof(ViewModel))]
        [Required(ErrorMessageResourceName = "ShipmentRequired", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        public bool SpedizioneAMano { get; set; }

        [Display(Name = "ShipmentPrivate", ResourceType = typeof(ViewModel))]
        [Required(ErrorMessageResourceName = "ShipmentRequired", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        public bool SpedizionePrivata { get; set; }

        [Display(Name = "ShipmentOnline", ResourceType = typeof(ViewModel))]
        [Required(ErrorMessageResourceName = "ShipmentRequired", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        public bool SpedizioneOnLine { get; set; }
        */
        [Display(Name = "ShipmentPrice", ResourceType = typeof(ViewModel))]
        [DataType(DataType.Currency)]
        [DisplayFormat(NullDisplayText = "n/a", ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        [RequiredIf("ScambioConSpedizione", Operator.EqualTo, true, ErrorMessageResourceName = "ShipmentPriceRequired", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [Range(0, double.MaxValue, ErrorMessageResourceName = "ShipmentPriceNotValid", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        public decimal? PrezzoSpedizione { get; set; }

        public IEnumerable<SelectListItem> ListaTempoImballaggio { get; set; }

        [Display(Name = "TimePackage", ResourceType = typeof(ViewModel))]
        [RequiredIf("ScambioConSpedizione", Operator.EqualTo, true, ErrorMessageResourceName = "TimePackageRequired", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        public int? TempoImballaggio { get; set; }

        [Display(Name = "ShipmentDescription", ResourceType = typeof(ViewModel))]
        public string UlterioriInformazioniSpedizione { get; set; }

        public List<SelectListItem> ServiziSpedizione { get; set; }

        [Display(Name = "CourierList", ResourceType = typeof(ViewModel))]
        [RequiredIf("ScambioConSpedizione", Operator.EqualTo, true, ErrorMessageResourceName = "ShipmentCourierRequired", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        public int? ServizioSpedizioneScelto { get; set; }

        public IEnumerable<SelectListItem> GiorniSpedizione { get; set; }

        [Display(Name = "ShipDays", ResourceType = typeof(ViewModel))]
        [RequiredIf("TipoSpedizione", Operator.EqualTo, Spedizione.Online, ErrorMessageResourceName = "ShipDaysRequired", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        public List<int> GiorniSpedizioneScelti { get; set; }

        public IEnumerable<SelectListItem> OrariSpedizione { get; set; }

        [Display(Name = "ShipHours", ResourceType = typeof(ViewModel))]
        [RequiredIf("TipoSpedizione", Operator.EqualTo, Spedizione.Online, ErrorMessageResourceName = "ShipHoursRequired", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        public List<int> OrariSpedizioneScelti { get; set; }

        [RequiredIf("TipoSpedizione", Operator.EqualTo, Spedizione.Online, ErrorMessageResourceName = "ShipHoursRequired", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        public int? IdIndirizzoMittente { get; set; }

        [RequiredIf("TipoSpedizione", Operator.EqualTo, Spedizione.Online, ErrorMessageResourceName = "DepartureCityRequired", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [Display(Name = "DepartureCity", ResourceType = typeof(ViewModel))]
        public string CittaMittente { get; set; }

        public int? IDCittaMittente { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 5)]
        [RequiredIf("TipoSpedizione", Operator.EqualTo, Spedizione.Online, ErrorMessageResourceName = "DepartureAddressRequired", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        [Display(Name = "DepartureAddress", ResourceType = typeof(ViewModel))]
        public string IndirizzoMittente { get; set; }

        [Display(Name = "DepartureCivic", ResourceType = typeof(ViewModel))]
        public int? CivicoMittente { get; set; }

        [Display(Name = "SellerName", ResourceType = typeof(ViewModel))]
        [RequiredIf("TipoSpedizione", Operator.EqualTo, Spedizione.Online, ErrorMessageResourceName = "SellerNameRequired", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        public string NominativoMittente { get; set; }

        [Display(Name = "SellerTelephone", ResourceType = typeof(ViewModel))]
        [DataType(DataType.PhoneNumber)]
        [StringLength(12, MinimumLength = 9, ErrorMessageResourceName = "ErrorPhoneNumber", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessageResourceName = "ErrorPhoneNumber", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [RequiredIf("TipoSpedizione", Operator.EqualTo, Spedizione.Online, ErrorMessageResourceName = "SellerTelephoneRequired", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        public string TelefonoMittente { get; set; }
        #endregion

        #region COSTRUTTORI

        public PubblicaOggettoViewModel() : base()
        {
            LoadProprietaDefault();
        }

        public PubblicaOggettoViewModel(PubblicazioneViewModel model) : base(model)
        {
            LoadProprietaDefault();
            //this.CopyAttributes(model);
        }

        public PubblicaOggettoViewModel(PubblicaOggettoViewModel model) : base(model)
        {
            LoadProprietaDefault();
            this.CopyAttributes(model);
        }

        public PubblicaOggettoViewModel(ANNUNCIO model) : base(model) { }

        #endregion

        #region METODI PUBBLICI
        public override void GetDatiDettaglio(ANNUNCIO model)
        {
            base.GetDatiDettaglio(model);
            this.Anno = model.OGGETTO.ANNO;
            this.Colore = model.OGGETTO.COLORE;
            this.CondizioneOggetto = (CondizioneOggetto)model.OGGETTO.STATO;
            if (model.OGGETTO.MARCA != null)
            {
                this.Marca = model.OGGETTO.MARCA.NOME;
            }
            this.MarcaID = model.OGGETTO.ID_MARCA;
            this.Quantità = model.OGGETTO.NUMERO_PEZZI;
            if (model.OGGETTO.OGGETTO_MATERIALE != null && model.OGGETTO.OGGETTO_MATERIALE.Count() > 0)
            {
                this.Materiali = model.OGGETTO.OGGETTO_MATERIALE.Select(m => m.MATERIALE.NOME).ToList();
            }
            if (model.OGGETTO.OGGETTO_COMPONENTE != null && model.OGGETTO.OGGETTO_COMPONENTE.Count() > 0)
            {
                this.Componenti = model.OGGETTO.OGGETTO_COMPONENTE.Select(m => m.COMPONENTE.NOME).ToList();
            }
            this.Altezza = Convert.ToInt32(model.OGGETTO.ALTEZZA);
            this.Lunghezza = Convert.ToInt32(model.OGGETTO.LUNGHEZZA);
            this.Larghezza = Convert.ToInt32(model.OGGETTO.LARGHEZZA);
            this.Peso = Convert.ToInt32(model.OGGETTO.PESO);
        }
        #endregion

        #region METODI PROTETTI
        protected override bool SalvaDettaglioPerTipologiaAnnuncio(DatabaseContext db, ANNUNCIO model)
        {
            OGGETTO oggetto = db.OGGETTO.Create();
            //oggetto.ID_COMUNE = this.IDCitta;
            oggetto.NUMERO_PEZZI = (this.Quantità <= 0) ? 1 : this.Quantità;

            // verifica se esiste id o marca nel DB
            if (this.MarcaID == null && !string.IsNullOrWhiteSpace(this.Marca))
            {
                MARCA marca;
                string nomeMarca = this.Marca.Trim();
                marca = db.MARCA.Where(item => item.NOME.StartsWith(nomeMarca)).FirstOrDefault();
                if (marca == null)
                {
                    marca = new MARCA();
                    marca.NOME = nomeMarca;
                    marca.DESCRIZIONE = marca.NOME;
                    marca.ID_CATEGORIA = this.CategoriaId;
                    marca.DATA_INSERIMENTO = DateTime.Now;
                    marca.DATA_MODIFICA = marca.DATA_INSERIMENTO;
                    marca.STATO = (int)Stato.ATTIVO;
                    db.MARCA.Add(marca);
                    db.SaveChanges();
                }

                this.MarcaID = marca.ID;
            }
            oggetto.ID_MARCA = this.MarcaID;

            oggetto.COLORE = this.Colore;
            oggetto.CONDIZIONE = (int)this.CondizioneOggetto;
            oggetto.ANNO = this.Anno;
            oggetto.ALTEZZA = this.Altezza;
            oggetto.LUNGHEZZA = this.Lunghezza;
            oggetto.LARGHEZZA = this.Larghezza;
            oggetto.PESO = this.Peso;

            // conversione punti a soldi
            oggetto.DATA_INSERIMENTO = DateTime.Now;
            oggetto.DATA_MODIFICA = oggetto.DATA_INSERIMENTO;

            oggetto.STATO = (int)Stato.ATTIVO; // attiva oggetto

            db.OGGETTO.Add(oggetto);
            if (db.SaveChanges() > 0)
            {
                SaveMateriali(db, oggetto.ID, this.Materiali);
                SaveComponenti(db, oggetto.ID, this.Componenti);

                model.OGGETTO = oggetto;
                if (SaveScambio(db, model))
                {
                    return true;
                }
            }

            return false;
        }

        protected override void PulisciListe()
        {
            base.PulisciListe();

            Componenti.RemoveAll(m => string.IsNullOrWhiteSpace(m));
            Materiali.RemoveAll(m => string.IsNullOrWhiteSpace(m));
        }
        #region VERIFICA DATI

        protected int? checkModello(DatabaseContext db, int? id, string nome, int? marca)
        {
            // verifica se esiste id o modello nel DB
            if (id == null && !string.IsNullOrWhiteSpace(nome))
            {
                MODELLO modello;
                string nomeModello = nome.Trim();
                modello = db.MODELLO.Where(item => item.NOME.StartsWith(nomeModello) && item.ID_MARCA == marca).FirstOrDefault();
                if (modello == null)
                {
                    modello = new MODELLO();
                    modello.NOME = nomeModello;
                    modello.DESCRIZIONE = modello.NOME;
                    modello.ID_MARCA = (int)marca;
                    modello.DATA_INSERIMENTO = DateTime.Now;
                    modello.DATA_MODIFICA = modello.DATA_INSERIMENTO;
                    modello.STATO = (int)Stato.ATTIVO;
                    db.MODELLO.Add(modello);
                    db.SaveChanges();
                }

                id = modello.ID;
            }
            return id;
        }

        protected int? checkSistemaOperativo(DatabaseContext db, int? id, string nome, int categoria)
        {
            // verifica se esiste id o modello nel DB
            if (id == null && !string.IsNullOrWhiteSpace(nome))
            {
                SISTEMA_OPERATIVO modello;
                string nomeModello = nome.Trim();
                modello = db.SISTEMA_OPERATIVO.Where(item => item.NOME.StartsWith(nomeModello) && item.ID_CATEGORIA == categoria).FirstOrDefault();
                if (modello == null)
                {
                    modello = new SISTEMA_OPERATIVO();
                    modello.NOME = nomeModello;
                    modello.DESCRIZIONE = modello.NOME;
                    modello.ID_CATEGORIA = categoria;
                    modello.DATA_INSERIMENTO = DateTime.Now;
                    modello.DATA_MODIFICA = modello.DATA_INSERIMENTO;
                    modello.STATO = (int)Stato.ATTIVO;
                    db.SISTEMA_OPERATIVO.Add(modello);
                    db.SaveChanges();
                }

                id = modello.ID;
            }
            return id;
        }

        protected int? checkAlimentazione(DatabaseContext db, int? id, string nome, int categoria)
        {
            // verifica se esiste id o modello nel DB
            if (id == null && !string.IsNullOrWhiteSpace(nome))
            {
                ALIMENTAZIONE modello;
                string nomeModello = nome.Trim();
                modello = db.ALIMENTAZIONE.Where(item => item.NOME.StartsWith(nomeModello) && item.ID_CATEGORIA == categoria).FirstOrDefault();
                if (modello == null)
                {
                    modello = new ALIMENTAZIONE();
                    modello.NOME = nomeModello;
                    modello.DESCRIZIONE = modello.NOME;
                    modello.ID_CATEGORIA = categoria;
                    modello.DATA_INSERIMENTO = DateTime.Now;
                    modello.DATA_MODIFICA = modello.DATA_INSERIMENTO;
                    modello.STATO = (int)Stato.ATTIVO;
                    db.ALIMENTAZIONE.Add(modello);
                    db.SaveChanges();
                }

                id = modello.ID;
            }
            return id;
        }

        protected int? checkArtista(DatabaseContext db, int? id, string nome, int categoria)
        {
            // verifica se esiste id o modello nel DB
            if (id == null && !string.IsNullOrWhiteSpace(nome))
            {
                ARTISTA modello;
                string nomeModello = nome.Trim();
                modello = db.ARTISTA.Where(item => item.NOME.StartsWith(nomeModello) && item.ID_CATEGORIA == categoria).FirstOrDefault();
                if (modello == null)
                {
                    modello = new ARTISTA();
                    modello.NOME = nomeModello;
                    modello.DESCRIZIONE = modello.NOME;
                    modello.ID_CATEGORIA = categoria;
                    modello.DATA_INSERIMENTO = DateTime.Now;
                    modello.DATA_MODIFICA = modello.DATA_INSERIMENTO;
                    modello.STATO = (int)Stato.ATTIVO;
                    db.ARTISTA.Add(modello);
                    db.SaveChanges();
                }

                id = modello.ID;
            }
            return id;
        }

        protected int? checkAutore(DatabaseContext db, int? id, string nome, int categoria)
        {
            // verifica se esiste id o modello nel DB
            if (id == null && !string.IsNullOrWhiteSpace(nome))
            {
                AUTORE modello;
                string nomeModello = nome.Trim();
                modello = db.AUTORE.Where(item => item.NOME.StartsWith(nomeModello) && item.ID_CATEGORIA == categoria).FirstOrDefault();
                if (modello == null)
                {
                    modello = new AUTORE();
                    modello.NOME = nomeModello;
                    modello.DESCRIZIONE = modello.NOME;
                    modello.ID_CATEGORIA = categoria;
                    modello.DATA_INSERIMENTO = DateTime.Now;
                    modello.DATA_MODIFICA = modello.DATA_INSERIMENTO;
                    modello.STATO = (int)Stato.ATTIVO;
                    db.AUTORE.Add(modello);
                    db.SaveChanges();
                }

                id = modello.ID;
            }
            return id;
        }

        protected int? checkFormato(DatabaseContext db, int? id, string nome, int categoria)
        {
            // verifica se esiste id o modello nel DB
            if (id == null && !string.IsNullOrWhiteSpace(nome))
            {
                FORMATO modello;
                string nomeModello = nome.Trim();
                modello = db.FORMATO.Where(item => item.NOME.StartsWith(nomeModello) && item.ID_CATEGORIA == categoria).FirstOrDefault();
                if (modello == null)
                {
                    modello = new FORMATO();
                    modello.NOME = nomeModello;
                    modello.DESCRIZIONE = modello.NOME;
                    modello.ID_CATEGORIA = categoria;
                    modello.DATA_INSERIMENTO = DateTime.Now;
                    modello.DATA_MODIFICA = modello.DATA_INSERIMENTO;
                    modello.STATO = (int)Stato.ATTIVO;
                    db.FORMATO.Add(modello);
                    db.SaveChanges();
                }

                id = modello.ID;
            }
            return id;
        }

        protected int? checkGenere(DatabaseContext db, int? id, string nome, int categoria)
        {
            // verifica se esiste id o modello nel DB
            if (id == null && !string.IsNullOrWhiteSpace(nome))
            {
                GENERE modello;
                string nomeModello = nome.Trim();
                modello = db.GENERE.Where(item => item.NOME.StartsWith(nomeModello) && item.ID_CATEGORIA == categoria).FirstOrDefault();
                if (modello == null)
                {
                    modello = new GENERE();
                    modello.NOME = nomeModello;
                    modello.DESCRIZIONE = modello.NOME;
                    modello.ID_CATEGORIA = categoria;
                    modello.DATA_INSERIMENTO = DateTime.Now;
                    modello.DATA_MODIFICA = modello.DATA_INSERIMENTO;
                    modello.STATO = (int)Stato.ATTIVO;
                    db.GENERE.Add(modello);
                    db.SaveChanges();
                }

                id = modello.ID;
            }
            return id;
        }

        protected int? checkPiattaforma(DatabaseContext db, int? id, string nome, int categoria)
        {
            // verifica se esiste id o modello nel DB
            if (id == null && !string.IsNullOrWhiteSpace(nome))
            {
                PIATTAFORMA modello;
                string nomeModello = nome.Trim();
                modello = db.PIATTAFORMA.Where(item => item.NOME.StartsWith(nomeModello) && item.ID_CATEGORIA == categoria).FirstOrDefault();
                if (modello == null)
                {
                    modello = new PIATTAFORMA();
                    modello.NOME = nomeModello;
                    modello.DESCRIZIONE = modello.NOME;
                    modello.ID_CATEGORIA = categoria;
                    modello.DATA_INSERIMENTO = DateTime.Now;
                    modello.DATA_MODIFICA = modello.DATA_INSERIMENTO;
                    modello.STATO = (int)Stato.ATTIVO;
                    db.PIATTAFORMA.Add(modello);
                    db.SaveChanges();
                }

                id = modello.ID;
            }
            return id;
        }

        protected int? checkRegista(DatabaseContext db, int? id, string nome, int categoria)
        {
            // verifica se esiste id o modello nel DB
            if (id == null && !string.IsNullOrWhiteSpace(nome))
            {
                REGISTA modello;
                string nomeModello = nome.Trim();
                modello = db.REGISTA.Where(item => item.NOME.StartsWith(nomeModello) && item.ID_CATEGORIA == categoria).FirstOrDefault();
                if (modello == null)
                {
                    modello = new REGISTA();
                    modello.NOME = nomeModello;
                    modello.DESCRIZIONE = modello.NOME;
                    modello.ID_CATEGORIA = categoria;
                    modello.DATA_INSERIMENTO = DateTime.Now;
                    modello.DATA_MODIFICA = modello.DATA_INSERIMENTO;
                    modello.STATO = (int)Stato.ATTIVO;
                    db.REGISTA.Add(modello);
                    db.SaveChanges();
                }

                id = modello.ID;
            }
            return id;
        }

        #endregion
        #endregion

        #region METODI PRIVATI
        private void LoadProprietaDefault()
        {
            base.TipoPubblicazione = TipoAcquisto.Oggetto;
            this.Quantità = 1;
            this.CondizioneOggetto = CondizioneOggetto.Usato;
            this.Anno = new int?(DateTime.Now.Year);
            this.Materiali.Add("");
            this.Componenti.Add("");
            this.PrezzoSpedizione = 0;
            this.ScambioAMano = true;
            // recupero di default la sped online
            TipoSpedizione = (int)Spedizione.Privata;
            ListaTempoImballaggio = System.Enum.GetValues(typeof(TempoImballaggio)).Cast<TempoImballaggio>().Select(v => new SelectListItem
            {
                Text = Components.EnumHelper<TempoImballaggio>.GetDisplayValue(v),
                Value = ((int)v).ToString()
            }).ToList();
            GiorniSpedizione = System.Enum.GetValues(typeof(GiorniSpedizione)).Cast<GiorniSpedizione>().Select(v => new SelectListItem
            {
                Text = Components.EnumHelper<GiorniSpedizione>.GetDisplayValue(v),
                Value = ((int)v).ToString()
            }).ToList();
            OrariSpedizione = System.Enum.GetValues(typeof(OrariSpedizione)).Cast<OrariSpedizione>().Select(v => new SelectListItem
            {
                Text = Components.EnumHelper<OrariSpedizione>.GetDisplayValue(v),
                Value = ((int)v).ToString()
            }).ToList();
            ServiziSpedizione = new List<SelectListItem>();
            (HttpContext.Current.Application["serviziSpedizione"] as List<CORRIERE_SERVIZIO>).GroupBy(m => m.CORRIERE.ID_TIPO_SPEDIZIONE).ToList().ForEach(m =>
            {
                SelectListGroup group = new SelectListGroup();
                group.Name = EnumHelper<Spedizione>.GetDisplayValue((Spedizione)m.Key);
                (HttpContext.Current.Application["serviziSpedizione"] as List<CORRIERE_SERVIZIO>).Where(s => s.CORRIERE.ID_TIPO_SPEDIZIONE == m.Key).ToList().ForEach(s =>
                {
                    ServiziSpedizione.Add(new SelectListItem()
                    {
                        Text = s.CORRIERE.NOME + " - " + s.NOME,
                        Value = s.ID.ToString(),
                        Group = group
                    });
                });
            });
            ListaTipoSpedizione = new List<SelectListItem>();
            foreach (TIPO_SPEDIZIONE m in HttpContext.Current.Application["tipoSpedizione"] as List<TIPO_SPEDIZIONE>)
            {
                var testo = typeof(App_GlobalResources.Enum).InvokeMember(m.NOME, BindingFlags.GetProperty, null, null, null);
                SelectListItem selectItem = new SelectListItem()
                {
                    Text = testo.ToString(),
                    Value = m.ID.ToString()
                };
                ListaTipoSpedizione.Add(selectItem);
            }
            PersonaModel utente = (HttpContext.Current.Session["utente"] as PersonaModel);
            NominativoMittente = utente.NomeVisibile;
            PERSONA_TELEFONO telefono = utente.Telefono.SingleOrDefault(m => m.TIPO == (int)TipoTelefono.Privato);
            if (telefono!=null)
                TelefonoMittente = telefono.TELEFONO;
            PERSONA_INDIRIZZO indirizzoSpedizione = utente.Indirizzo.SingleOrDefault(m => m.TIPO == (int)TipoIndirizzo.Spedizione);
            if (indirizzoSpedizione != null)
            {
                INDIRIZZO indirizzo = indirizzoSpedizione.INDIRIZZO;
                CittaMittente = indirizzo.COMUNE.NOME;
                IDCittaMittente = indirizzo.ID_COMUNE;
                IndirizzoMittente = indirizzo.INDIRIZZO1;
                IdIndirizzoMittente = indirizzo.ID;
                CivicoMittente = indirizzo.CIVICO;
            }

        }

        private bool SaveMateriali(DatabaseContext db, int idOggetto, List<string> materiali)
        {
            // salvataggio materiali, cercando prima l'esistenza in anagrafica, tramite fulltext
            foreach (string m in materiali.Where(m => !string.IsNullOrWhiteSpace(m)))
            {
                SqlParameter param1 = new SqlParameter("materiale", m);
                MATERIALE materialeDaCercare = db.MATERIALE.SqlQuery(@"
                    SELECT M.* FROM MATERIALE AS M INNER JOIN
                        FREETEXTTABLE(MATERIALE, *, @materiale) AS MF ON M.ID = MF.[KEY]
                        ORDER BY MF.RANK DESC
                    ", new object[] { param1 }).FirstOrDefault();
                if (materialeDaCercare == null)
                {
                    // inserire nuovo materiale
                    materialeDaCercare = new MATERIALE();
                    materialeDaCercare.NOME = m;
                    db.MATERIALE.Add(materialeDaCercare);
                    if (db.SaveChanges() <= 0)
                        return false;
                }
                // salvataggio associazione con oggetto
                OGGETTO_MATERIALE associazioneMaterialeOggetto = new OGGETTO_MATERIALE();
                associazioneMaterialeOggetto.ID_MATERIALE = materialeDaCercare.ID;
                associazioneMaterialeOggetto.ID_OGGETTO = idOggetto;
                db.OGGETTO_MATERIALE.Add(associazioneMaterialeOggetto);
                // non controllo salvataggio, male che vada faccio salvare oggetto senza questi dati
                db.SaveChanges();
            };
            return true;
        }

        private bool SaveComponenti(DatabaseContext db, int idOggetto, List<string> componenti)
        {
            // salvataggio componenti, cercando prima l'esistenza in anagrafica, tramite fulltext
            foreach (string m in componenti.Where(m => !string.IsNullOrWhiteSpace(m)))
            {
                SqlParameter param1 = new SqlParameter("componente", m);
                COMPONENTE componenteDaCercare = db.COMPONENTE.SqlQuery(@"
                    SELECT M.* FROM COMPONENTE AS M INNER JOIN
                        FREETEXTTABLE(COMPONENTE, *, @componente) AS MF ON M.ID = MF.[KEY]
                        ORDER BY MF.RANK DESC
                    ", new object[] { param1 }).FirstOrDefault();
                if (componenteDaCercare == null)
                {
                    // inserire nuovo componente
                    componenteDaCercare = new COMPONENTE();
                    componenteDaCercare.NOME = m;
                    db.COMPONENTE.Add(componenteDaCercare);
                    if (db.SaveChanges() <= 0)
                        return false;
                }
                // salvataggio associazione con oggetto
                OGGETTO_COMPONENTE associazioneComponenteOggetto = new OGGETTO_COMPONENTE();
                associazioneComponenteOggetto.ID_COMPONENTE = componenteDaCercare.ID;
                associazioneComponenteOggetto.ID_OGGETTO = idOggetto;
                db.OGGETTO_COMPONENTE.Add(associazioneComponenteOggetto);
                // non controllo salvataggio, male che vada faccio salvare oggetto senza questi dati
                db.SaveChanges();

            };
            return true;
        }

        private bool SaveScambio(DatabaseContext db, ANNUNCIO annuncio)
        {
            try
            {
                if (this.ScambioAMano)
                {
                    ANNUNCIO_TIPO_SCAMBIO tipoScambio = new ANNUNCIO_TIPO_SCAMBIO();
                    tipoScambio.ID_ANNUNCIO = annuncio.ID;
                    tipoScambio.TIPO_SCAMBIO = (int)TipoScambio.AMano;
                    tipoScambio.DATA_INSERIMENTO = DateTime.Now;
                    tipoScambio.STATO = (int)Stato.ATTIVO;
                    db.ANNUNCIO_TIPO_SCAMBIO.Add(tipoScambio);
                    if (db.SaveChanges() <= 0)
                        return false;
                }
                if (this.ScambioConSpedizione)
                {
                    ANNUNCIO_TIPO_SCAMBIO tipoScambio = new ANNUNCIO_TIPO_SCAMBIO();
                    tipoScambio.ID_ANNUNCIO = annuncio.ID;
                    tipoScambio.TIPO_SCAMBIO = (int)TipoScambio.Spedizione;
                    tipoScambio.DATA_INSERIMENTO = DateTime.Now;
                    tipoScambio.STATO = (int)Stato.ATTIVO;
                    db.ANNUNCIO_TIPO_SCAMBIO.Add(tipoScambio);
                    if (db.SaveChanges() <= 0)
                        return false;

                    // se si vorranno le spedizioni comulative, si recupererà dati mittente e destinatario dal centro spedizione
                    // e i clienti ritireranno i pacchi andando al centro con il nome registrato sul portale
                    CORRIERE_SERVIZIO_SPEDIZIONE spedizione = new CORRIERE_SERVIZIO_SPEDIZIONE();
                    spedizione.TEMPO_IMBALLAGGIO = this.TempoImballaggio;
                    spedizione.ID_TIPO_PACCO = 1;
                    spedizione.ID_CORRIERE_SERVIZIO = (int)this.ServizioSpedizioneScelto;
                    spedizione.PUNTI = 0;
                    spedizione.SOLDI = Convert.ToDecimal(this.PrezzoSpedizione);
                    spedizione.ID_TIPO_VALUTA = this.IdTipoValuta;
                    spedizione.INFO_EXTRA_MITTENTE = this.UlterioriInformazioniSpedizione;
                    spedizione.DATA_INSERIMENTO = DateTime.Now;
                    spedizione.STATO = (int)Stato.SOSPESO;

                    // 25-04-2018 Aggiunto gestione di prezzo fisso in base al tipo di servizio se si sceglie ONLINE!
                    if (this.TipoSpedizione == (int)Spedizione.Online)
                    {
                        decimal pesoVolumetrico = 0;
                        // calcolo peso volumetrico! (lunghezza x altezza x larghezza) / 5000
                        pesoVolumetrico = (decimal)(this.Lunghezza * this.Altezza * this.Larghezza) / 5000;
                        var prezzoStimato = db.CORRIERE_SERVIZIO_PREZZO_STIMATO.SingleOrDefault(m => m.ID_CORRIERE_SERVIZIO == spedizione.ID_CORRIERE_SERVIZIO
                        && (m.PESO_MINIMO <= pesoVolumetrico && (m.PESO_MASSIMO > pesoVolumetrico || m.PESO_MASSIMO == null)) && m.STATO == (int)Stato.ATTIVO);
                        // settaggio prezzo servizio
                        if (prezzoStimato != null)
                        {
                            spedizione.SOLDI = prezzoStimato.PREZZO_STIMATO;
                        }
                    }

                    if (db.CORRIERE_SERVIZIO.Count(m => m.ID == this.ServizioSpedizioneScelto && m.CORRIERE.ID_TIPO_SPEDIZIONE == (int)Spedizione.Online) > 0)
                    {
                        // gestione SPEDIZIONE ONLINE con TNT o E-MOTION
                        PersonaModel mittente = ((PersonaModel)HttpContext.Current.Session["utente"]);
                        spedizione.GIORNI_DISPONIBILITA_SPEDIZIONE = (GiorniSpedizioneScelti != null && GiorniSpedizioneScelti.Count() > 0) ? string.Join(";", GiorniSpedizioneScelti) : string.Join(";", GiorniSpedizione.Select(m => m.Value));
                        spedizione.ORARI_DISPONIBILITA_SPEDIZIONE = (OrariSpedizioneScelti != null && OrariSpedizioneScelti.Count() > 0) ? string.Join(";", OrariSpedizioneScelti) : string.Join(";", OrariSpedizione.Select(m => m.Value));
                        spedizione.NOMINATIVO_MITTENTE = this.NominativoMittente;
                        spedizione.TELEFONO_MITTENTE = this.TelefonoMittente;

                        // aggiornamento se nuovo indirizzo SISTEMARE GESTIONE INDIRIZZO SPEDIZIONE E PUNTO DI SPEDIZIONE
                        //CORRIERE_PUNTO_SPEDIZIONE puntoSpedizione = null;
                        INDIRIZZO indirizzo = db.INDIRIZZO.SingleOrDefault(m => m.CIVICO == this.CivicoMittente && m.INDIRIZZO1 == this.IndirizzoMittente && m.ID_COMUNE == this.IDCittaMittente);
                        if (indirizzo == null)
                        {
                            indirizzo = new INDIRIZZO();
                            indirizzo.ID_COMUNE = (int)this.IDCittaMittente;
                            indirizzo.INDIRIZZO1 = this.IndirizzoMittente;
                            indirizzo.CIVICO = (int)this.CivicoMittente;
                            indirizzo.DATA_INSERIMENTO = DateTime.Now;
                            indirizzo.STATO = (int)Stato.ATTIVO;
                            db.INDIRIZZO.Add(indirizzo);
                            if (db.SaveChanges() <= 0)
                                return false;
                        }
                        /* 25-04-2018 Commentato perchè non abbiamo più e-motion
                        int idCorriere = db.CORRIERE_SERVIZIO.SingleOrDefault(n => n.ID == spedizione.ID_CORRIERE_SERVIZIO).ID_CORRIERE;
                        puntoSpedizione = db.CORRIERE_PUNTO_SPEDIZIONE.SingleOrDefault(m => m.ID_CORRIERE == idCorriere && m.ID_INDIRIZZO == indirizzo.ID);
                        if (puntoSpedizione == null)
                        {
                            puntoSpedizione = new CORRIERE_PUNTO_SPEDIZIONE();
                            puntoSpedizione.ID_CORRIERE = idCorriere;
                            puntoSpedizione.ID_INDIRIZZO = indirizzo.ID;
                            puntoSpedizione.ID_PUNTO_SPEDIZIONE = InserisciPuntoSpedizione(db, mittente.Email.SingleOrDefault(m => m.TIPO == (int)TipoEmail.Registrazione).EMAIL,
                                puntoSpedizione.ID_CORRIERE);
                            puntoSpedizione.DATA_INSERIMENTO = DateTime.Now;
                            puntoSpedizione.STATO = (int)Stato.ATTIVO;
                            db.CORRIERE_PUNTO_SPEDIZIONE.Add(puntoSpedizione);
                            if (db.SaveChanges() <= 0)
                                return false;
                        }
                        */

                        spedizione.ID_INDIRIZZO_MITTENTE = indirizzo.ID;
                        // 25-04-2018 Commentato perchè non abbiamo più e-motion
                        //spedizione.SOLDI = CalcolaPrezzo(annuncio, puntoSpedizione.ID_PUNTO_SPEDIZIONE);
                    }
                    else
                    {
                        // nella gestione privata salvo comunque i dati di spedizione in caso di offerta da fare o ricevere
                        spedizione.GIORNI_DISPONIBILITA_SPEDIZIONE = (GiorniSpedizioneScelti != null && GiorniSpedizioneScelti.Count() > 0) ? string.Join(";", GiorniSpedizioneScelti) : string.Join(";", GiorniSpedizione.Select(m => m.Value));
                        spedizione.ORARI_DISPONIBILITA_SPEDIZIONE = (OrariSpedizioneScelti != null && OrariSpedizioneScelti.Count() > 0) ? string.Join(";", OrariSpedizioneScelti) : string.Join(";", OrariSpedizione.Select(m => m.Value));
                        spedizione.NOMINATIVO_MITTENTE = this.NominativoMittente;
                        spedizione.TELEFONO_MITTENTE = this.TelefonoMittente;
                        
                        INDIRIZZO indirizzo = db.INDIRIZZO.SingleOrDefault(m => m.CIVICO == this.CivicoMittente && m.INDIRIZZO1 == this.IndirizzoMittente && m.ID_COMUNE == this.IDCittaMittente);
                        if (indirizzo == null)
                        {
                            indirizzo = new INDIRIZZO();
                            indirizzo.ID_COMUNE = (int)this.IDCittaMittente;
                            indirizzo.INDIRIZZO1 = this.IndirizzoMittente;
                            indirizzo.CIVICO = (int)this.CivicoMittente;
                            indirizzo.DATA_INSERIMENTO = DateTime.Now;
                            indirizzo.STATO = (int)Stato.ATTIVO;
                            db.INDIRIZZO.Add(indirizzo);
                            if (db.SaveChanges() <= 0)
                                return false;
                        }
                        spedizione.ID_INDIRIZZO_MITTENTE = indirizzo.ID;
                    }
                    db.CORRIERE_SERVIZIO_SPEDIZIONE.Add(spedizione);
                    if (db.SaveChanges() <= 0)
                        return false;

                    ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE annuncioSpedizione = new ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE();
                    annuncioSpedizione.ID_ANNUNCIO_TIPO_SCAMBIO = tipoScambio.ID;
                    annuncioSpedizione.ID_CORRIERE_SERVIZIO_SPEDIZIONE = spedizione.ID;
                    annuncioSpedizione.ID_TIPO_OGGETTO = 1;
                    annuncioSpedizione.PUNTI = spedizione.PUNTI;
                    annuncioSpedizione.SOLDI = spedizione.SOLDI;
                    annuncioSpedizione.DATA_INSERIMENTO = DateTime.Now;
                    annuncioSpedizione.STATO = (int)Stato.ATTIVO;
                    annuncioSpedizione.ID_COMMISSIONE = GetCommissioneSpedizione(db, spedizione.SOLDI);
                    db.ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE.Add(annuncioSpedizione);
                    if (db.SaveChanges() <= 0)
                        return false;

                }
            }
            catch (ShipmentException eccezione)
            {
                throw eccezione;
            }
            catch (PackageException eccezione)
            {
                throw eccezione;
            }
            catch (Exception eccezione)
            {
                ErrorSignal.FromCurrentContext().Raise(eccezione);
                throw new Exception(ExceptionMessage.ShipmentServiceFailed);
            }
            return true;
        }

        private int GetCommissioneSpedizione(DatabaseContext db, decimal importo)
        {
            decimal percentuale = Convert.ToDecimal(ConfigurationManager.AppSettings["spedizionePercentuale"]);
            COMMISSIONE commissione = new COMMISSIONE();
            commissione.PERCENTUALE = percentuale;
            commissione.TIPO = (int)TipoCommissione.Spedizione;
            commissione.DATA_INSERIMENTO = DateTime.Now;
            commissione.STATO = (int)Stato.ATTIVO;
            db.COMMISSIONE.Add(commissione);
            db.SaveChanges();
            return commissione.ID;
        }

        #region METODI PER E-MOTION
        private string InserisciPuntoSpedizione(DatabaseContext db, string emailMittente, int idCorriere)
        {
            COMUNE comune = db.COMUNE.SingleOrDefault(m => m.ID == this.IDCittaMittente);
            DeparturePointRequest newDeparture = new DeparturePointRequest();
            newDeparture.firstname = this.NominativoMittente.Split(' ')[0];
            newDeparture.lastname = ((this.NominativoMittente.Split(' ').Count() > 1) ? this.NominativoMittente.Split(' ')[1] : "");
            newDeparture.address1 = this.IndirizzoMittente + " " + this.CivicoMittente.ToString();
            newDeparture.city = comune.NOME;
            newDeparture.company = string.Empty;
            newDeparture.departurename = Guid.NewGuid().ToString().Replace("-", "");
            newDeparture.email = emailMittente;
            newDeparture.iso_country = GetCountryISO(db, comune, idCorriere);
            newDeparture.iso_state = GetStateISO(db, comune, idCorriere, newDeparture.iso_country);
            newDeparture.phone = this.TelefonoMittente;
            newDeparture.postcode = comune.CAP;
            newDeparture.other = this.UlterioriInformazioniSpedizione;
            
            Emotion.Service emotion = new Emotion.Service();
            Emotion.Response.NewDeparturePointResponse queryResult = emotion.NewDeparturePoint(newDeparture);
            if (queryResult.result != "OK")
                throw new ShipmentException(queryResult.errorMessage);
            else
                return queryResult.departureid;
        }

        private decimal CalcolaPrezzo(ANNUNCIO annuncio, string idPuntoSpedizione)
        {
            decimal prezzo = 0;
            //try
            //{

            DepartureRequest departure = new DepartureRequest();
            departure.departureid = idPuntoSpedizione;

            GetShippingCostRequest shipping = new GetShippingCostRequest();
            shipping.shippingserviceid = "SA0031";
            shipping.deliveryaddress = new AddressRequest()
            {
                countryiso = "IT"
            };
            shipping.departureid = departure.departureid;
            shipping.goodstype = GoodType.GENERICO;
            PackagesRequest packages = new PackagesRequest();
            packages.SetHeightDecimal((decimal)annuncio.OGGETTO.ALTEZZA);
            packages.SetLengthDecimal((decimal)annuncio.OGGETTO.LUNGHEZZA);
            packages.SetWidthDecimal((decimal)annuncio.OGGETTO.LARGHEZZA);
            packages.SetWeightDecimal((decimal)annuncio.OGGETTO.PESO);
            shipping.packages = new List<PackagesRequest>()
            {
                packages
            };
            shipping.packagingtype = PackType.PACCO;
            Emotion.Service servizioSpedizione = new Emotion.Service();
            Emotion.Response.GetShippingCostResponse risposta = servizioSpedizione.GetShippingCost(shipping);
            if (risposta.data != null)
            {
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("it-IT");
                if (risposta.data.SA0031 != null)
                {
                    Decimal.TryParse(risposta.data.SA0031.totalcost, System.Globalization.NumberStyles.Currency, System.Globalization.CultureInfo.InvariantCulture, out prezzo);
                    return prezzo;
                }
                else if (risposta.data.SA0032 != null)
                {
                    Decimal.TryParse(risposta.data.SA0031.totalcost, System.Globalization.NumberStyles.Currency, System.Globalization.CultureInfo.InvariantCulture, out prezzo);
                    return prezzo;
                }
            }
            /*}
            catch (ShipmentException eccezione)
            {
                throw new ShipmentException(eccezione.Message);
            }
            catch (Exception eccezione)
            {
                // mettere log
            }*/
            return prezzo;
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
                comune2.ISO = states.states.First(m => m.name.ToUpper()==comune.PROVINCIA.NOME.ToUpper()).iso_code;
                db.CORRIERE_COMUNE.Add(comune2);
                if (db.SaveChanges() > 0)
                    return comune2.ISO;
            }
            return string.Empty;
        }
        #endregion
        #endregion
    }

    public class PubblicaOggettoCopiaViewModel : PubblicaCopiaViewModel
    {
        #region PROPRIETA
        [Range(0, 2147483647, ErrorMessageResourceName = "ErrorRange", ErrorMessageResourceType = typeof(Language))]
        public int? Anno { get; set; }

        public string Colore { get; set; }

        [Display(Name = "StateObject", ResourceType = typeof(Language))]
        [Required]
        public CondizioneOggetto CondizioneOggetto { get; set; }
        #endregion

        #region COSTRUTTORI
        public PubblicaOggettoCopiaViewModel() : base() { }
        public PubblicaOggettoCopiaViewModel(ANNUNCIO model) : base(model) { }
        #endregion

        #region METODI PUBBLICI
        public override void GetDatiDettaglio(ANNUNCIO model)
        {
            //this.Citta = model.COMUNE.NOME;
            //this.IDCitta = model.OGGETTO.ID_COMUNE;
            this.Anno = model.OGGETTO.ANNO;
            this.Colore = model.OGGETTO.COLORE;
            this.CondizioneOggetto = (CondizioneOggetto)model.OGGETTO.CONDIZIONE;
        }
        #endregion
    }

    public class PubblicaServizioViewModel : PubblicazioneViewModel
    {
        #region PROPRIETA
        [DataType(DataType.Text)]
        [Display(Name = "NameService", ResourceType = typeof(Language))]
        //[Required]
        [StringLength(50, MinimumLength = 3, ErrorMessageResourceName = "TextTooLong", ErrorMessageResourceType = typeof(Language))]
        public override string Nome { get; set; }

        //[Required]
        [Display(Name = "Sunday", ResourceType = typeof(Language))]
        public bool Domenica { get; set; }

        //[Required]
        [Display(Name = "Thursday", ResourceType = typeof(Language))]
        public bool Giovedi { get; set; }

        //[Required]
        [Display(Name = "Monday", ResourceType = typeof(Language))]
        public bool Lunedi { get; set; }

        //[Required]
        [Display(Name = "Tuesday", ResourceType = typeof(Language))]
        public bool Martedi { get; set; }

        //[Required]
        [Display(Name = "Wednesday", ResourceType = typeof(Language))]
        public bool Mercoledi { get; set; }

        //[Required]
        [Display(Name = "Bid", ResourceType = typeof(Language))]
        public string Offerta { get; set; }

        //[Required]
        [DataType(DataType.Time, ErrorMessageResourceName = "ErrorEndTime", ErrorMessageResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.RangeTime(ErrorMessageResourceName = "ErrorFormat", ErrorMessageResourceType = typeof(Language))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}", ConvertEmptyStringToNull = true, NullDisplayText = "18:00")]
        [Display(Name = "EndTime", ResourceType = typeof(Language))]
        public DateTime? OraFineFeriali { get; set; }

        [DataType(DataType.Time, ErrorMessageResourceName = "ErrorEndTime", ErrorMessageResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.RangeTime(ErrorMessageResourceName = "ErrorFormat", ErrorMessageResourceType = typeof(Language))]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:HH:mm}", ConvertEmptyStringToNull = true, NullDisplayText = "18:00")]
        [Display(Name = "EndTime", ResourceType = typeof(Language))]
        public DateTime? OraFineFestivi { get; set; }

        //[Required]
        [DataType(DataType.Time, ErrorMessageResourceName = "ErrorStartTime", ErrorMessageResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.RangeTime("OraFineFeriali", ErrorMessageResourceName = "ErrorFormat", ErrorMessageResourceType = typeof(Language))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}", ConvertEmptyStringToNull = true, NullDisplayText = "09:00")]
        [Display(Name = "StartTime", ResourceType = typeof(Language))]
        public DateTime? OraInizioFeriali { get; set; }

        [DataType(DataType.Time, ErrorMessageResourceName = "ErrorStartTime", ErrorMessageResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.RangeTime("OraFineFestivi", ErrorMessageResourceName = "ErrorFormat", ErrorMessageResourceType = typeof(Language))]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:HH:mm}", ConvertEmptyStringToNull = true, NullDisplayText = "09:00")]
        [Display(Name = "StartTime", ResourceType = typeof(Language))]
        public DateTime? OraInizioFestivi { get; set; }

        //[Required]
        [Display(Name = "Results", ResourceType = typeof(Language))]
        public string Risultati { get; set; }

        //[Required]
        [Display(Name = "Saturday", ResourceType = typeof(Language))]
        public bool Sabato { get; set; }

        //[Required]
        [Display(Name = "AllDays", ResourceType = typeof(Language))]
        public bool Tutti { get; set; }

        //[Required]
        [Display(Name = "Friday", ResourceType = typeof(Language))]
        public bool Venerdi { get; set; }

        [Required]
        [Display(Name = "Rate", ResourceType = typeof(Language))]
        public Tariffa Tariffa { get; set; }
        #endregion

        #region COSTRUTTORI
        public PubblicaServizioViewModel() : base()
        {
            base.TipoPubblicazione = TipoAcquisto.Servizio;
            Tutti = true;
        }

        public PubblicaServizioViewModel(PubblicazioneViewModel model) : base(model)
        {
            base.TipoPubblicazione = TipoAcquisto.Servizio;
            Tutti = true;
        }

        public PubblicaServizioViewModel(ANNUNCIO model) : base(model) { }
        #endregion

        #region METODI PUBBLICI
        public override void GetDatiDettaglio(ANNUNCIO model)
        {
            base.GetDatiDettaglio(model);
            if (model.SERVIZIO.LUNEDI != null)
                this.Lunedi = (bool)model.SERVIZIO.LUNEDI;
            if (model.SERVIZIO.MARTEDI != null)
                this.Martedi = (bool)model.SERVIZIO.MARTEDI;
            if (model.SERVIZIO.MERCOLEDI != null)
                this.Mercoledi = (bool)model.SERVIZIO.MERCOLEDI;
            if (model.SERVIZIO.GIOVEDI != null)
                this.Giovedi = (bool)model.SERVIZIO.GIOVEDI;
            if (model.SERVIZIO.VENERDI != null)
                this.Venerdi = (bool)model.SERVIZIO.VENERDI;
            if (model.SERVIZIO.SABATO != null)
                this.Sabato = (bool)model.SERVIZIO.SABATO;
            if (model.SERVIZIO.DOMENICA != null)
                this.Domenica = (bool)model.SERVIZIO.DOMENICA;
            if (model.SERVIZIO.TUTTI!=null)
                this.Tutti = (bool)model.SERVIZIO.TUTTI;
            this.Offerta = model.SERVIZIO.SERVIZI_OFFERTI;
            if (model.SERVIZIO.ORA_INIZIO_FERIALI != null)
                this.OraInizioFeriali = Convert.ToDateTime(model.SERVIZIO.ORA_INIZIO_FERIALI.ToString());
            if (model.SERVIZIO.ORA_FINE_FERIALI != null)
                this.OraFineFeriali = Convert.ToDateTime(model.SERVIZIO.ORA_FINE_FERIALI.ToString());
            if (model.SERVIZIO.ORA_INIZIO_FESTIVI != null)
                this.OraInizioFestivi = Convert.ToDateTime(model.SERVIZIO.ORA_INIZIO_FESTIVI.ToString());
            if (model.SERVIZIO.ORA_FINE_FESTIVI != null)
                this.OraFineFestivi = Convert.ToDateTime(model.SERVIZIO.ORA_FINE_FESTIVI.ToString());
            this.Risultati = model.SERVIZIO.RISULTATI_FINALI;
            this.Tariffa = (Tariffa)model.SERVIZIO.TARIFFA;

        }
        #endregion

        #region METODI PROTETTI
        protected override bool SalvaDettaglioPerTipologiaAnnuncio(DatabaseContext db, ANNUNCIO model)
        {
            SERVIZIO servizio = db.SERVIZIO.Create();
            //servizio.ID_COMUNE = this.IDCitta;
            if (this.Tutti)
            {
                servizio.LUNEDI = true;
                servizio.MARTEDI = true;
                servizio.MERCOLEDI = true;
                servizio.GIOVEDI = true;
                servizio.VENERDI = true;
                servizio.SABATO = true;
                servizio.DOMENICA = true;
                servizio.TUTTI = true;
            }
            else
            {
                servizio.LUNEDI = this.Lunedi;
                servizio.MARTEDI = this.Martedi;
                servizio.MERCOLEDI = this.Mercoledi;
                servizio.GIOVEDI = this.Giovedi;
                servizio.VENERDI = this.Venerdi;
                servizio.SABATO = this.Sabato;
                servizio.DOMENICA = this.Domenica;
            }
            if (this.OraInizioFeriali != null)
                servizio.ORA_INIZIO_FERIALI = ((DateTime)this.OraInizioFeriali).TimeOfDay;
            if (this.OraFineFeriali != null)
                servizio.ORA_FINE_FERIALI = ((DateTime)this.OraFineFeriali).TimeOfDay;
            if (this.OraInizioFestivi != null)
                servizio.ORA_INIZIO_FESTIVI = ((DateTime)this.OraInizioFestivi).TimeOfDay;
            if (this.OraFineFestivi != null)
                servizio.ORA_FINE_FESTIVI = ((DateTime)this.OraFineFestivi).TimeOfDay;
            servizio.SERVIZI_OFFERTI = this.Offerta;
            servizio.RISULTATI_FINALI = this.Risultati;
            servizio.TARIFFA = (int)this.Tariffa;
            servizio.DATA_INSERIMENTO = DateTime.Now;
            servizio.DATA_MODIFICA = servizio.DATA_INSERIMENTO;
            servizio.STATO = (int)Stato.ATTIVO; // attiva oggetto

            db.SERVIZIO.Add(servizio);
            if (db.SaveChanges() > 0)
            {
                model.SERVIZIO = servizio;
                return true;
            }
            return false;
        }
        #endregion
    }

    public class PubblicaServizioCopiaViewModel : PubblicaCopiaViewModel
    {
        #region PROPRIETA
        [Required]
        [Display(Name = "Sunday", ResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public bool Domenica { get; set; }

        [Required]
        [Display(Name = "Thursday", ResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public bool Giovedi { get; set; }

        [Required]
        [Display(Name = "Monday", ResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public bool Lunedi { get; set; }

        [Required]
        [Display(Name = "Tuesday", ResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public bool Martedi { get; set; }

        [Required]
        [Display(Name = "Wednesday", ResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public bool Mercoledi { get; set; }

        [Display(Name = "Bid", ResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public string Offerta { get; set; }

        [DataType(DataType.Time, ErrorMessageResourceName = "ErrorEndTime", ErrorMessageResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.RangeTime(ErrorMessageResourceName = "ErrorFormat", ErrorMessageResourceType = typeof(Language))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        [Display(Name = "EndTime", ResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public DateTime? OraFineFeriali { get; set; }

        [DataType(DataType.Time, ErrorMessageResourceName = "ErrorEndTime", ErrorMessageResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.RangeTime(ErrorMessageResourceName = "ErrorFormat", ErrorMessageResourceType = typeof(Language))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        [Display(Name = "EndTime", ResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public DateTime? OraFineFestivi { get; set; }

        [DataType(DataType.Time, ErrorMessageResourceName = "ErrorStartTime", ErrorMessageResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.RangeTime(ErrorMessageResourceName = "ErrorFormat", ErrorMessageResourceType = typeof(Language))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        [Display(Name = "StartTime", ResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public DateTime? OraInizioFeriali { get; set; }

        [DataType(DataType.Time, ErrorMessageResourceName = "ErrorStartTime", ErrorMessageResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.RangeTime(ErrorMessageResourceName = "ErrorFormat", ErrorMessageResourceType = typeof(Language))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        [Display(Name = "StartTime", ResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public DateTime? OraInizioFestivi { get; set; }

        [Display(Name = "Results", ResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public string Risultati { get; set; }

        [Required]
        [Display(Name = "Saturday", ResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public bool Sabato { get; set; }

        [Required]
        [Display(Name = "AllDays", ResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public bool Tutti { get; set; }

        [Required]
        [Display(Name = "Friday", ResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public bool Venerdi { get; set; }

        [Required]
        [Display(Name = "Rate", ResourceType = typeof(Language))]
        [GratisForGratis.DataAnnotations.AnnuncioCompleto]
        public Tariffa Tariffa { get; set; }
        #endregion    

        #region COSTRUTTORI
        public PubblicaServizioCopiaViewModel() : base() { }
        public PubblicaServizioCopiaViewModel(ANNUNCIO model) : base(model) { }
        #endregion

        #region METODI PUBBLICI
        public override void GetDatiDettaglio(ANNUNCIO model)
        {
            this.Lunedi = (bool)model.SERVIZIO.LUNEDI;
            this.Martedi = (bool)model.SERVIZIO.MARTEDI;
            this.Mercoledi = (bool)model.SERVIZIO.MERCOLEDI;
            this.Giovedi = (bool)model.SERVIZIO.GIOVEDI;
            this.Venerdi = (bool)model.SERVIZIO.VENERDI;
            this.Sabato = (bool)model.SERVIZIO.SABATO;
            this.Domenica = (bool)model.SERVIZIO.DOMENICA;
            if (model.SERVIZIO.TUTTI != null)
                this.Tutti = (bool)model.SERVIZIO.TUTTI;
            this.Offerta = model.SERVIZIO.SERVIZI_OFFERTI;
            if (model.SERVIZIO.ORA_INIZIO_FERIALI != null)
                this.OraInizioFeriali = Convert.ToDateTime(model.SERVIZIO.ORA_INIZIO_FERIALI.ToString());
            if (model.SERVIZIO.ORA_FINE_FERIALI != null)
                this.OraFineFeriali = Convert.ToDateTime(model.SERVIZIO.ORA_FINE_FERIALI.ToString());
            if (model.SERVIZIO.ORA_INIZIO_FESTIVI != null)
                this.OraInizioFestivi = Convert.ToDateTime(model.SERVIZIO.ORA_INIZIO_FESTIVI.ToString());
            if (model.SERVIZIO.ORA_FINE_FESTIVI != null)
                this.OraFineFestivi = Convert.ToDateTime(model.SERVIZIO.ORA_FINE_FESTIVI.ToString());
            this.Risultati = model.SERVIZIO.RISULTATI_FINALI;
            this.Tariffa = (Tariffa)model.SERVIZIO.TARIFFA;
            //this.IDCitta = (int)model.SERVIZIO.ID_COMUNE;
            //this.Citta = model.SERVIZIO.COMUNE.NOME;
        }
        #endregion
    }

    public class PubblicaSpedizioneViewModel
    {
        #region PROPRIETA
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessageResourceName = "ErrorRange", ErrorMessageResourceType = typeof(Language))]
        public int IdCorriereNazione { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string CorriereNazione { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string NomeMittente { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string TelefonoMittente { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailMittente { get; set; }

        [Range(1, Int32.MaxValue, ErrorMessageResourceName = "ErrorRange", ErrorMessageResourceType = typeof(Language))]
        public int IdIndirizzoMittente { get; set; }

        [Required]
        [Range(1, Int32.MaxValue, ErrorMessageResourceName = "ErrorRange", ErrorMessageResourceType = typeof(Language))]
        public int IdComuneMittente { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string ComuneMittente { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string IndirizzoMittente { get; set; }

        [Required]
        [Range(1, Int32.MaxValue, ErrorMessageResourceName = "ErrorRange", ErrorMessageResourceType = typeof(Language))]
        public int CivicoMittente { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string SezioneMittente { get; set; }
        #endregion
    }

    public class NotificaRicercaViewModel
    {
        public PERSONA_RICERCA RICERCA { get; set; }

        public ANNUNCIO ANNUNCIO { get; set; }

        public string FOTO { get; set; }
    }

    public class SessioneFoto
    {
        public string NomeOriginaleFoto { get; set; }

        public string NomeUnivocoFoto { get; set; }
    }

    public class PrezzoSpedizioneViewModel
    {
        public string Prezzo { get; set; }

        public bool Visibile { get; set; }
    }

    public class PubblicaListaFotoViewModel
    {
        #region PROPRIETA
        public List<string> Foto { get; set; }

        public string TokenUploadFoto { get; set; }
        #endregion
    }

    #region DETTAGLI
    public class PubblicaTecnologiaViewModel : PubblicaOggettoViewModel
    {
        #region COSTRUTTORI
        public PubblicaTecnologiaViewModel() : base() { }
        public PubblicaTecnologiaViewModel(ANNUNCIO model) : base(model) { }
        #endregion
        #region PROPRIETA
        //[Required]
        [DataType(DataType.Text)]
        public string Modello { get; set; }

        public int? ModelloID { get; set; }
        #endregion
        #region METODI PUBBLICI
        public override void GetDatiDettaglio(ANNUNCIO model)
        {
            base.GetDatiDettaglio(model);
            OGGETTO_TECNOLOGIA modelDettaglio = model.OGGETTO.OGGETTO_TECNOLOGIA.FirstOrDefault();
            if (modelDettaglio != null)
            {
                this.ModelloID = modelDettaglio.ID_MODELLO;
                if (this.ModelloID != null)
                    this.Modello = modelDettaglio.MODELLO.NOME;
            }
        }
        #endregion
        #region METODI PROTETTI
        protected override bool SalvaDettaglioPerCategoriaAnnuncio(DatabaseContext db, ANNUNCIO model)
        {
            OGGETTO_TECNOLOGIA oggetto = new OGGETTO_TECNOLOGIA();
            oggetto.ID_MODELLO = checkModello(db, this.ModelloID, this.Modello, model.OGGETTO.ID_MARCA);
            oggetto.ID_OGGETTO = (int)model.OGGETTO.ID;
            db.OGGETTO_TECNOLOGIA.Add(oggetto);
            return db.SaveChanges() > 0;
        }
        #endregion
    }

    public class PubblicaElettrodomesticoViewModel : PubblicaOggettoViewModel
    {
        #region COSTRUTTORI
        public PubblicaElettrodomesticoViewModel() : base() { }
        public PubblicaElettrodomesticoViewModel(ANNUNCIO model) : base(model) { }
        #endregion
        #region PROPRIETA
        //[Required]
        [DataType(DataType.Text)]
        public string Modello { get; set; }

        public int? ModelloID { get; set; }
        #endregion
        #region METODI PUBBLICI
        public override void GetDatiDettaglio(ANNUNCIO model)
        {
            base.GetDatiDettaglio(model);
            OGGETTO_ELETTRODOMESTICO modelDettaglio = model.OGGETTO.OGGETTO_ELETTRODOMESTICO.FirstOrDefault();
            if (modelDettaglio != null)
            {
                this.ModelloID = modelDettaglio.ID_MODELLO;
                if (this.ModelloID != null)
                    this.Modello = modelDettaglio.MODELLO.NOME;
            }
        }
        #endregion
        #region METODI PROTETTI
        protected override bool SalvaDettaglioPerCategoriaAnnuncio(DatabaseContext db, ANNUNCIO model)
        {
            OGGETTO_ELETTRODOMESTICO oggetto = new OGGETTO_ELETTRODOMESTICO();
            oggetto.ID_MODELLO = checkModello(db, this.ModelloID, this.Modello, model.OGGETTO.ID_MARCA);
            oggetto.ID_OGGETTO = (int)model.OGGETTO.ID;
            db.OGGETTO_ELETTRODOMESTICO.Add(oggetto);
            return db.SaveChanges() > 0;
        }
        #endregion
    }

    public class PubblicaGiocoViewModel : PubblicaOggettoViewModel
    {
        #region COSTRUTTORI
        public PubblicaGiocoViewModel() : base() { }
        public PubblicaGiocoViewModel(ANNUNCIO model) : base(model) { }
        #endregion
        #region PROPRIETA
        //[Required]
        [DataType(DataType.Text)]
        public string Modello { get; set; }

        public int? ModelloID { get; set; }
        #endregion
        #region METODI PUBBLICI
        public override void GetDatiDettaglio(ANNUNCIO model)
        {
            base.GetDatiDettaglio(model);
            OGGETTO_GIOCO modelDettaglio = model.OGGETTO.OGGETTO_GIOCO.FirstOrDefault();
            if (modelDettaglio != null)
            {
                this.ModelloID = modelDettaglio.ID_MODELLO;
                if (this.ModelloID != null)
                    this.Modello = modelDettaglio.MODELLO.NOME;
            }
        }
        #endregion
        #region METODI PROTETTI
        protected override bool SalvaDettaglioPerCategoriaAnnuncio(DatabaseContext db, ANNUNCIO model)
        {
            OGGETTO_GIOCO oggetto = new OGGETTO_GIOCO();
            oggetto.ID_MODELLO = checkModello(db, this.ModelloID, this.Modello, model.OGGETTO.ID_MARCA);
            oggetto.ID_OGGETTO = (int)model.OGGETTO.ID;
            db.OGGETTO_GIOCO.Add(oggetto);
            return db.SaveChanges() > 0;
        }
        #endregion
    }

    public class PubblicaSportViewModel : PubblicaOggettoViewModel
    {
        #region COSTRUTTORI
        public PubblicaSportViewModel() : base() { }
        public PubblicaSportViewModel(ANNUNCIO model) : base(model) { }
        #endregion
        #region PROPRIETA
        //[Required]
        [DataType(DataType.Text)]
        public string Modello { get; set; }

        public int? ModelloID { get; set; }
        #endregion
        #region METODI PUBBLICI
        public override void GetDatiDettaglio(ANNUNCIO model)
        {
            base.GetDatiDettaglio(model);
            OGGETTO_SPORT modelDettaglio = model.OGGETTO.OGGETTO_SPORT.FirstOrDefault();
            if (modelDettaglio != null)
            {
                this.ModelloID = modelDettaglio.ID_MODELLO;
                if (this.ModelloID != null)
                    this.Modello = modelDettaglio.MODELLO.NOME;
            }
        }
        #endregion
        #region METODI PROTETTI
        protected override bool SalvaDettaglioPerCategoriaAnnuncio(DatabaseContext db, ANNUNCIO model)
        {
            OGGETTO_SPORT oggetto = new OGGETTO_SPORT();
            oggetto.ID_MODELLO = checkModello(db, this.ModelloID, this.Modello, model.OGGETTO.ID_MARCA);
            oggetto.ID_OGGETTO = (int)model.OGGETTO.ID;
            db.OGGETTO_SPORT.Add(oggetto);
            return db.SaveChanges() > 0;
        }
        #endregion
    }

    public class PubblicaStrumentoViewModel : PubblicaOggettoViewModel
    {
        #region COSTRUTTORI
        public PubblicaStrumentoViewModel() : base() { }
        public PubblicaStrumentoViewModel(ANNUNCIO model) : base(model) { }
        #endregion
        #region PROPRIETA
        //[Required]
        [DataType(DataType.Text)]
        public string Modello { get; set; }

        public int? ModelloID { get; set; }
        #endregion
        #region METODI PUBBLICI
        public override void GetDatiDettaglio(ANNUNCIO model)
        {
            base.GetDatiDettaglio(model);
            OGGETTO_STRUMENTO modelDettaglio = model.OGGETTO.OGGETTO_STRUMENTO.FirstOrDefault();
            if (modelDettaglio != null)
            {
                this.ModelloID = modelDettaglio.ID_MODELLO;
                if (this.ModelloID != null)
                    this.Modello = modelDettaglio.MODELLO.NOME;
            }
        }
        #endregion
        #region METODI PROTETTI
        protected override bool SalvaDettaglioPerCategoriaAnnuncio(DatabaseContext db, ANNUNCIO model)
        {
            OGGETTO_STRUMENTO oggetto = new OGGETTO_STRUMENTO();
            oggetto.ID_MODELLO = checkModello(db, this.ModelloID, this.Modello, model.OGGETTO.ID_MARCA);
            oggetto.ID_OGGETTO = (int)model.OGGETTO.ID;
            db.OGGETTO_STRUMENTO.Add(oggetto);
            return db.SaveChanges() > 0;
        }
        #endregion
    }

    public class PubblicaConsoleViewModel : PubblicaOggettoViewModel
    {
        #region COSTRUTTORI
        public PubblicaConsoleViewModel() : base() { }
        public PubblicaConsoleViewModel(ANNUNCIO model) : base(model) { }
        #endregion
        #region PROPRIETA
        [DataType(DataType.Text)]
        //[Required]
        public string Piattaforma
        {
            get;
            set;
        }

        public int? PiattaformaID
        {
            get;
            set;
        }
        #endregion
        #region METODI PUBBLICI
        public override void GetDatiDettaglio(ANNUNCIO model)
        {
            base.GetDatiDettaglio(model);
            OGGETTO_CONSOLE modelDettaglio = model.OGGETTO.OGGETTO_CONSOLE.FirstOrDefault();
            if (modelDettaglio != null)
            {
                this.PiattaformaID = modelDettaglio.ID_PIATTAFORMA;
                if (this.PiattaformaID != null)
                    this.Piattaforma = modelDettaglio.PIATTAFORMA.NOME;
            }
        }
        #endregion
        #region METODI PROTETTI
        protected override bool SalvaDettaglioPerCategoriaAnnuncio(DatabaseContext db, ANNUNCIO model)
        {
            OGGETTO_CONSOLE oggetto = new OGGETTO_CONSOLE();
            oggetto.ID_PIATTAFORMA = checkPiattaforma(db, this.PiattaformaID, this.Piattaforma, model.ID_CATEGORIA);
            oggetto.ID_OGGETTO = (int)model.OGGETTO.ID;
            db.OGGETTO_CONSOLE.Add(oggetto);
            return db.SaveChanges() > 0;
        }
        #endregion
    }

    public class PubblicaLibroViewModel : PubblicaOggettoViewModel
    {
        #region COSTRUTTORI
        public PubblicaLibroViewModel() : base() { }
        public PubblicaLibroViewModel(ANNUNCIO model) : base(model) { }
        #endregion
        #region PROPRIETA
        [DataType(DataType.Text)]
        //[Required]
        public string Autore
        {
            get;
            set;
        }

        public int? AutoreID
        {
            get;
            set;
        }
        #endregion
        #region METODI PUBBLICI
        public override void GetDatiDettaglio(ANNUNCIO model)
        {
            base.GetDatiDettaglio(model);
            OGGETTO_LIBRO modelDettaglio = model.OGGETTO.OGGETTO_LIBRO.FirstOrDefault();
            if (modelDettaglio != null)
            {
                this.AutoreID = modelDettaglio.ID_AUTORE;
                if (this.AutoreID != null)
                    this.Autore = modelDettaglio.AUTORE.NOME;
            }
        }
        #endregion
        #region METODI PROTETTI
        protected override bool SalvaDettaglioPerCategoriaAnnuncio(DatabaseContext db, ANNUNCIO model)
        {
            OGGETTO_LIBRO oggetto = new OGGETTO_LIBRO();
            oggetto.ID_AUTORE = checkAutore(db, this.AutoreID, this.Autore, model.ID_CATEGORIA);
            oggetto.ID_OGGETTO = (int)model.OGGETTO.ID;
            db.OGGETTO_LIBRO.Add(oggetto);
            return db.SaveChanges() > 0;
        }
        #endregion
    }

    public class PubblicaMusicaViewModel : PubblicaOggettoViewModel
    {
        #region COSTRUTTORI
        public PubblicaMusicaViewModel() : base() { }
        public PubblicaMusicaViewModel(ANNUNCIO model) : base(model) { }
        #endregion
        #region PROPRIETA
        [DataType(DataType.Text)]
        //[Required]
        public string Artista
        {
            get;
            set;
        }

        public int? ArtistaID
        {
            get;
            set;
        }

        [DataType(DataType.Text)]
        //[Required]
        public string Formato
        {
            get;
            set;
        }

        public int? FormatoID
        {
            get;
            set;
        }
        #endregion
        #region METODI PUBBLICI
        public override void GetDatiDettaglio(ANNUNCIO model)
        {
            base.GetDatiDettaglio(model);
            OGGETTO_MUSICA modelDettaglio = model.OGGETTO.OGGETTO_MUSICA.FirstOrDefault();
            if (modelDettaglio != null)
            {
                this.ArtistaID = modelDettaglio.ID_ARTISTA;
                if (this.ArtistaID != null)
                    this.Artista = modelDettaglio.ARTISTA.NOME;
                this.FormatoID = modelDettaglio.ID_FORMATO;
                if (this.FormatoID != null)
                    this.Formato = modelDettaglio.FORMATO.NOME;
            }
        }
        #endregion
        #region METODI PROTETTI
        protected override bool SalvaDettaglioPerCategoriaAnnuncio(DatabaseContext db, ANNUNCIO model)
        {
            OGGETTO_MUSICA oggetto = new OGGETTO_MUSICA();
            oggetto.ID_FORMATO = checkFormato(db, this.FormatoID, this.Formato, model.ID_CATEGORIA);
            oggetto.ID_ARTISTA = checkArtista(db, this.ArtistaID, this.Artista, model.ID_CATEGORIA);
            oggetto.ID_OGGETTO = (int)model.OGGETTO.ID;
            db.OGGETTO_MUSICA.Add(oggetto);
            return db.SaveChanges() > 0;
        }
        #endregion
    }

    public class PubblicaPcViewModel : PubblicaOggettoViewModel
    {
        #region COSTRUTTORI
        public PubblicaPcViewModel() : base() { }
        public PubblicaPcViewModel(ANNUNCIO model) : base(model) { }
        #endregion
        #region PROPRIETA
        //[Required]
        [DataType(DataType.Text)]
        public string Modello { get; set; }

        public int? ModelloID { get; set; }
        [DataType(DataType.Text)]
        [Display(Name = "OperatingSystem", ResourceType = typeof(Language))]
        //[Required]
        public string SistemaOperativo { get; set; }

        public int? SistemaOperativoID { get; set; }
        #endregion
        #region METODI PUBBLICI
        public override void GetDatiDettaglio(ANNUNCIO model)
        {
            base.GetDatiDettaglio(model);
            OGGETTO_COMPUTER modelDettaglio = model.OGGETTO.OGGETTO_COMPUTER.FirstOrDefault();
            if (modelDettaglio != null)
            {
                this.ModelloID = modelDettaglio.ID_MODELLO;
                if (this.ModelloID != null)
                    this.Modello = modelDettaglio.MODELLO.NOME;
                this.SistemaOperativoID = modelDettaglio.ID_SISTEMA_OPERATIVO;
                if (this.SistemaOperativoID != null)
                    this.SistemaOperativo = modelDettaglio.SISTEMA_OPERATIVO.NOME;
            }
        }
        #endregion
        #region METODI PROTETTI
        protected override bool SalvaDettaglioPerCategoriaAnnuncio(DatabaseContext db, ANNUNCIO model)
        {
            OGGETTO_COMPUTER oggetto = new OGGETTO_COMPUTER();
            oggetto.ID_MODELLO = checkModello(db, this.ModelloID, this.Modello, model.OGGETTO.ID_MARCA);
            oggetto.ID_SISTEMA_OPERATIVO = checkSistemaOperativo(db, this.SistemaOperativoID, this.SistemaOperativo, model.ID_CATEGORIA);
            oggetto.ID_OGGETTO = (int)model.OGGETTO.ID;
            db.OGGETTO_COMPUTER.Add(oggetto);
            return db.SaveChanges() > 0;
        }
        #endregion
    }

    public class PubblicaTelefoniSmartphoneViewModel : PubblicaOggettoViewModel
    {
        #region COSTRUTTORI
        public PubblicaTelefoniSmartphoneViewModel() : base() { }
        public PubblicaTelefoniSmartphoneViewModel(PubblicaTelefoniSmartphoneViewModel model)
        {
            this.CopyAttributes(model);
        }
        public PubblicaTelefoniSmartphoneViewModel(ANNUNCIO model) : base(model) { }
        #endregion
        #region PROPRIETA
        //[Required]
        [DataType(DataType.Text)]
        public string Modello { get; set; }

        public int? ModelloID { get; set; }
        [DataType(DataType.Text)]
        [Display(Name = "OperatingSystem", ResourceType = typeof(Language))]
        //[Required]
        public string SistemaOperativo { get; set; }

        public int? SistemaOperativoID { get; set; }
        #endregion
        #region METODI PUBBLICI
        public override void GetDatiDettaglio(ANNUNCIO model)
        {
            base.GetDatiDettaglio(model);
            OGGETTO_TELEFONO modelDettaglio = model.OGGETTO.OGGETTO_TELEFONO.FirstOrDefault();
            if (modelDettaglio != null)
            {
                this.ModelloID = modelDettaglio.ID_MODELLO;
                if (this.ModelloID != null)
                    this.Modello = modelDettaglio.MODELLO.NOME;
                this.SistemaOperativoID = modelDettaglio.ID_SISTEMA_OPERATIVO;
                if (this.SistemaOperativoID != null)
                    this.SistemaOperativo = modelDettaglio.SISTEMA_OPERATIVO.NOME;
            }
        }
        #endregion
        #region METODI PROTETTI
        protected override bool SalvaDettaglioPerCategoriaAnnuncio(DatabaseContext db, ANNUNCIO model)
        {
            OGGETTO_TELEFONO oggetto = new OGGETTO_TELEFONO();
            oggetto.ID_MODELLO = checkModello(db, this.ModelloID, this.Modello, model.OGGETTO.ID_MARCA);
            oggetto.ID_SISTEMA_OPERATIVO = checkSistemaOperativo(db, this.SistemaOperativoID, this.SistemaOperativo, model.ID_CATEGORIA);
            oggetto.ID_OGGETTO = (int)model.OGGETTO.ID;
            db.OGGETTO_TELEFONO.Add(oggetto);
            return db.SaveChanges() > 0;
        }
        #endregion
    }

    public class PubblicaVeicoloViewModel : PubblicaOggettoViewModel
    {
        #region COSTRUTTORI
        public PubblicaVeicoloViewModel() : base() { }
        public PubblicaVeicoloViewModel(ANNUNCIO model) : base(model) { }
        #endregion
        #region PROPRIETA
        //[Required]
        [DataType(DataType.Text)]
        public string Modello { get; set; }

        public int? ModelloID { get; set; }

        [DataType(DataType.Text)]
        //[Required]
        [Display(Name = "Power", ResourceType = typeof(Language))]
        public string Alimentazione { get; set; }

        public int? AlimentazioneID { get; set; }
        #endregion        
        #region METODI PUBBLICI
        public override void GetDatiDettaglio(ANNUNCIO model)
        {
            base.GetDatiDettaglio(model);
            OGGETTO_VEICOLO modelDettaglio = model.OGGETTO.OGGETTO_VEICOLO.FirstOrDefault();
            if (modelDettaglio != null)
            {
                this.ModelloID = modelDettaglio.ID_MODELLO;
                if (this.ModelloID != null)
                    this.Modello = modelDettaglio.MODELLO.NOME;
                this.AlimentazioneID = modelDettaglio.ID_ALIMENTAZIONE;
                if (this.AlimentazioneID != null)
                    this.Alimentazione = modelDettaglio.ALIMENTAZIONE.NOME;
            }
        }
        #endregion
        #region METODI PROTETTI
        protected override bool SalvaDettaglioPerCategoriaAnnuncio(DatabaseContext db, ANNUNCIO model)
        {
            OGGETTO_VEICOLO oggetto = new OGGETTO_VEICOLO();
            oggetto.ID_MODELLO = checkModello(db, this.ModelloID, this.Modello, model.OGGETTO.ID_MARCA);
            oggetto.ID_ALIMENTAZIONE = checkAlimentazione(db, this.AlimentazioneID, this.Alimentazione, model.ID_CATEGORIA);
            oggetto.ID_OGGETTO = (int)model.OGGETTO.ID;
            db.OGGETTO_VEICOLO.Add(oggetto);
            return db.SaveChanges() > 0;
        }
        #endregion
    }

    public class PubblicaVestitoViewModel : PubblicaOggettoViewModel
    {
        #region COSTRUTTORI
        public PubblicaVestitoViewModel() : base() { }
        public PubblicaVestitoViewModel(ANNUNCIO model) : base(model) { }
        #endregion
        #region PROPRIETA
        [DataType(DataType.Text)]
        //[Required]
        public string Taglia
        {
            get;
            set;
        }        
        #endregion
        #region METODI PUBBLICI
        public override void GetDatiDettaglio(ANNUNCIO model)
        {
            base.GetDatiDettaglio(model);
            OGGETTO_VESTITO modelDettaglio = model.OGGETTO.OGGETTO_VESTITO.FirstOrDefault();
            if (modelDettaglio != null)
            {
                this.Taglia = modelDettaglio.TAGLIA;
            }
        }
        #endregion
        #region METODI PROTETTI
        protected override bool SalvaDettaglioPerCategoriaAnnuncio(DatabaseContext db, ANNUNCIO model)
        {
            OGGETTO_VESTITO oggetto = new OGGETTO_VESTITO();
            oggetto.ID_OGGETTO = (int)model.OGGETTO.ID;
            oggetto.TAGLIA = this.Taglia;
            db.OGGETTO_VESTITO.Add(oggetto);
            return db.SaveChanges() > 0;
        }
        #endregion
    }

    public class PubblicaVideogamesViewModel : PubblicaOggettoViewModel
    {
        #region COSTRUTTORI
        public PubblicaVideogamesViewModel() : base() { }
        public PubblicaVideogamesViewModel(ANNUNCIO model) : base(model) { }
        #endregion
        #region PROPRIETA
        [DataType(DataType.Text)]
        //[Required]
        public string Genere
        {
            get;
            set;
        }

        public int? GenereID
        {
            get;
            set;
        }

        [DataType(DataType.Text)]
        //[Required]
        public string Piattaforma
        {
            get;
            set;
        }

        public int? PiattaformaID
        {
            get;
            set;
        }
        #endregion
        #region METODI PUBBLICI
        public override void GetDatiDettaglio(ANNUNCIO model)
        {
            base.GetDatiDettaglio(model);
            OGGETTO_VIDEOGAMES modelDettaglio = model.OGGETTO.OGGETTO_VIDEOGAMES.FirstOrDefault();
            if (modelDettaglio != null)
            {
                this.GenereID = modelDettaglio.ID_GENERE;
                if (this.GenereID != null)
                    this.Genere = modelDettaglio.GENERE.NOME;
                this.PiattaformaID = modelDettaglio.ID_PIATTAFORMA;
                if (this.PiattaformaID != null)
                    this.Piattaforma = modelDettaglio.PIATTAFORMA.NOME;
            }
        }
        #endregion
        #region METODI PROTETTI
        protected override bool SalvaDettaglioPerCategoriaAnnuncio(DatabaseContext db, ANNUNCIO model)
        {
            OGGETTO_VIDEOGAMES oggetto = new OGGETTO_VIDEOGAMES();
            oggetto.ID_PIATTAFORMA = checkPiattaforma(db, this.PiattaformaID, this.Piattaforma, model.ID_CATEGORIA);
            oggetto.ID_GENERE = checkGenere(db, this.GenereID, this.Genere, model.ID_CATEGORIA);
            oggetto.ID_OGGETTO = (int)model.OGGETTO.ID;
            db.OGGETTO_VIDEOGAMES.Add(oggetto);
            return db.SaveChanges() > 0;
        }
        #endregion
    }

    public class PubblicaVideoViewModel : PubblicaOggettoViewModel
    {
        #region COSTRUTTORI
        public PubblicaVideoViewModel() : base() { }
        public PubblicaVideoViewModel(ANNUNCIO model) : base(model) { }
        #endregion
        #region PROPRIETA
        [DataType(DataType.Text)]
        //[Required]
        public string Formato
        {
            get;
            set;
        }

        public int? FormatoID
        {
            get;
            set;
        }

        [DataType(DataType.Text)]
        //[Required]
        public string Regista
        {
            get;
            set;
        }

        public int? RegistaID
        {
            get;
            set;
        }
        #endregion
        #region METODI PUBBLICI
        public override void GetDatiDettaglio(ANNUNCIO model)
        {
            base.GetDatiDettaglio(model);
            OGGETTO_VIDEO modelDettaglio = model.OGGETTO.OGGETTO_VIDEO.FirstOrDefault();
            if (modelDettaglio != null)
            {
                this.FormatoID = modelDettaglio.ID_FORMATO;
                if (this.FormatoID!=null)
                    this.Formato = modelDettaglio.FORMATO.NOME;
                this.RegistaID = modelDettaglio.ID_REGISTA;
                if (this.RegistaID != null)
                    this.Regista = modelDettaglio.REGISTA.NOME;
            }
        }
        #endregion
        #region METODI PROTETTI
        protected override bool SalvaDettaglioPerCategoriaAnnuncio(DatabaseContext db, ANNUNCIO model)
        {
            OGGETTO_VIDEO oggetto = new OGGETTO_VIDEO();
            oggetto.ID_FORMATO = checkFormato(db, this.FormatoID, this.Formato, model.ID_CATEGORIA);
            oggetto.ID_REGISTA = checkRegista(db, this.RegistaID, this.Regista, model.ID_CATEGORIA);
            oggetto.ID_OGGETTO = (int)model.OGGETTO.ID;
            db.OGGETTO_VIDEO.Add(oggetto);
            return db.SaveChanges() > 0;
        }
        #endregion
    }

    #endregion
}