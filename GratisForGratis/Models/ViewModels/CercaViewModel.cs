using GratisForGratis.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace GratisForGratis.Models
{
    #region FILTRI RICERCA

    public interface IRicercaViewModel
    {
        void SetRicercaByCookie(HttpCookie cookie);

        HttpCookie GetCookieRicerca(HttpCookie cookie);

        bool Save(DatabaseContext db, System.Web.Mvc.ControllerContext controller);

        void ResetCookie();

        void SendMail(System.Web.Mvc.ControllerContext controller);
    }

    public class RicercaViewModel : IRicercaViewModel
    {
        #region COSTRUTTORI
        public RicercaViewModel()
        {
            Cerca_IDCategoria = 1;
            Pagina = 1;
            Cerca_PuntiMin = 0;
            Cerca_PuntiMax = Convert.ToInt32(WebConfigurationManager.AppSettings["MaxPunti"]);
            ActionSalvataggio = "SaveRicercaVendite";
        }

        public RicercaViewModel(int numeroRecordTrovati = 0)
        {
            Cerca_IDCategoria = 1;
            Pagina = 1;
            Cerca_PuntiMin = 0;
            Cerca_PuntiMax = Convert.ToInt32(WebConfigurationManager.AppSettings["MaxPunti"]);
            NumeroRecordTrovati = numeroRecordTrovati;
            ActionSalvataggio = "SaveRicercaVendite";
        }

        public RicercaViewModel(HttpCookie ricerca, HttpCookie filtro)
        {
            Cerca_IDCategoria = Convert.ToInt32(ricerca["IDCategoria"]);
            Cerca_PuntiMin = Convert.ToInt32(ricerca["PuntiMin"]);
            Cerca_PuntiMax = Convert.ToInt32(ricerca["PuntiMax"]);
            if (ricerca["Nome"] != null)
                Cerca_Nome = ricerca["Nome"];
            if (ricerca["TipoPagamento"] != null)
                Cerca_TipoPagamento = (TipoPagamento)Convert.ToInt32(ricerca["TipoPagamento"]);
            if (ricerca["IDCitta"] != null)
                Cerca_IDCitta = Convert.ToInt32(ricerca["IDCitta"]);
            Cerca_NonPersonale = Convert.ToBoolean(ricerca["NonPersonale"]);
        }
        #endregion

        #region PROPRIETA
        public string NomeFiltro { get; set; }

        public int Id { get; set; }

        public int NumeroRecordTrovati { get; private set; }

        [Display(Name = "PaymentMethods", ResourceType = typeof(App_GlobalResources.Language))]
        public TipoPagamento? Cerca_TipoPagamento { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Place", ResourceType = typeof(App_GlobalResources.Language))]
        public string Cerca_Citta { get; set; }

        [Range(1, int.MaxValue)]
        public int? Cerca_IDCitta { get; set; }

        [Display(Name = "Min", ResourceType = typeof(App_GlobalResources.Language))]
        [RangeIntAdvanced("MinPunti", "MaxPunti", ErrorMessageResourceName = "ErrorPoints", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public int Cerca_PuntiMin { get; set; }

        [Display(Name = "Max", ResourceType = typeof(App_GlobalResources.Language))]
        [RangeIntAdvanced("MinPunti", "MaxPunti", ErrorMessageResourceName = "ErrorPoints", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public int Cerca_PuntiMax { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Category", ResourceType = typeof(App_GlobalResources.Language))]
        public string Cerca_Categoria { get; set; }

        public int Cerca_IDCategoria { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "NameObject", ResourceType = typeof(App_GlobalResources.Language))]
        public string Cerca_Nome { get; set; }

        public string Cerca_Submit { get; set; }

        public int Pagina { get; set; }

        public bool AttivaRicercaAvanzata { get; private set; }

        public string ActionSalvataggio { get; protected set; }

        [Display(Name = "AdOther", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Cerca_NonPersonale { get; set; }

        #endregion

        #region METODI PROTECTED
        // recupera valore di una chiave dal cookie o mette il valore di default
        protected object GetValueCookie(HttpCookie cookie, string key, object defaultValue = null) {
            if (cookie == null || string.IsNullOrEmpty(cookie[key]))
                return defaultValue;

            return cookie[key];
        }
        #endregion

        #region METODI PUBBLICI
        // setta la ricerca in base ai cookie
        public virtual void SetRicercaByCookie(HttpCookie cookie) {
            this.AttivaRicercaAvanzata = false;
            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
            {
                object valoreDefault = this.GetType().GetProperty(propertyInfo.Name).GetValue(this);
                string chiave = System.Text.RegularExpressions.Regex.Replace(propertyInfo.Name, "Cerca_", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // lascio come valore di default null, così se non trova una chiave nei cookie non setta risultati errati
                // nella proprietà della classe
                object valoreCookie = GetValueCookie(cookie, chiave);
                if (valoreCookie != null)
                {
                    Type tipoProprieta = propertyInfo.PropertyType;
                    if (tipoProprieta.IsGenericType && tipoProprieta.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        tipoProprieta = tipoProprieta.GetGenericArguments().First();
                    }
                    if (tipoProprieta.IsEnum)
                    {
                        valoreCookie = Enum.Parse(tipoProprieta, valoreCookie.ToString()) as Enum;
                    }
                    else
                    {
                        valoreCookie = Convert.ChangeType(valoreCookie, tipoProprieta);
                    }
                    this.GetType().GetProperty(propertyInfo.Name).SetValue(this, valoreCookie);
                    this.AttivaRicercaAvanzata = true;
                }
            }
        }

        // recupera i dati della ricerca in formato cookie sostituendo quelli passati
        public virtual HttpCookie GetCookieRicerca(HttpCookie cookie) {
            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties().Where(item => item.SetMethod != null && item.SetMethod.IsPublic))
            {
                //if (propertyInfo.GetValue(this) != null)
                string nomeParametro = System.Text.RegularExpressions.Regex.Replace(propertyInfo.Name, "Cerca_", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                cookie[nomeParametro] = (propertyInfo.GetValue(this) != null) ? propertyInfo.GetValue(this).ToString() : null;
            }
            return cookie;
        }

        public virtual bool Save(DatabaseContext db, System.Web.Mvc.ControllerContext controller)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("ricerca");
            PersonaModel utente = (HttpContext.Current.Request.IsAuthenticated) ? (HttpContext.Current.Session["utente"] as PersonaModel) : (HttpContext.Current.Session["utenteRicerca"] as PersonaModel);
            PERSONA_RICERCA personaRicerca = new PERSONA_RICERCA();
            RICERCA ricerca = new RICERCA();
            //model.UTENTE1 = utente;
            personaRicerca.ID_PERSONA = utente.Persona.ID;
            ricerca.ID_CATEGORIA = Convert.ToInt32(cookie["IDCategoria"]);
            ricerca.NOME = cookie["Nome"];
            ricerca.DATA_INSERIMENTO = DateTime.Now;
            ricerca.DATA_MODIFICA = ricerca.DATA_INSERIMENTO;
            ricerca.STATO = (int)Stato.ATTIVO;
            db.RICERCA.Add(ricerca);
            if (db.SaveChanges() > 0)
            {
                personaRicerca.ID_RICERCA = ricerca.ID;
                db.PERSONA_RICERCA.Add(personaRicerca);
                if (db.SaveChanges() > 0)
                {
                    this.Id = ricerca.ID;
                    return true;
                    //ResetFiltriRicerca();
                    //SendMailRicercaSalvata(utente, personaRicerca, controller);
                }
            }
            return false;
        }

        public void ResetCookie()
        {
            HttpCookie ricerca = new HttpCookie("ricerca");
            ricerca["IDCategoria"] = "1";
            ricerca["Categoria"] = "Tutti";
            HttpCookie filtro = new HttpCookie("filtro");
            HttpContext.Current.Response.SetCookie(ricerca);
            HttpContext.Current.Response.SetCookie(filtro);
        }

        public void SendMail(System.Web.Mvc.ControllerContext controller)
        {
            PersonaModel utente = (HttpContext.Current.Request.IsAuthenticated) ? (HttpContext.Current.Session["utente"] as PersonaModel) : (HttpContext.Current.Session["utenteRicerca"] as PersonaModel);
            // invio email salvataggio ricerca
            EmailModel email = new EmailModel(controller);
            email.To.Add(new System.Net.Mail.MailAddress(utente.Email.FirstOrDefault(item => item.TIPO == (int)TipoEmail.Registrazione).EMAIL, utente.Persona.NOME + ' ' + utente.Persona.COGNOME));
            email.Subject = string.Format(App_GlobalResources.Email.SearchSaveSubject, this.Cerca_Nome) + " - " + WebConfigurationManager.AppSettings["nomeSito"];
            email.Body = "SalvataggioRicerca";
            email.DatiEmail = this;
            new Controllers.EmailController().SendEmail(email);
        }
        #endregion

        #region METODI STATICI
        public static RicercaViewModel GetViewModelByModel(RICERCA model)
        {
            RicercaViewModel viewModel = null;
            if (model != null)
            {
                viewModel = new RicercaViewModel();
                viewModel.Id = model.ID;
                viewModel.Cerca_Categoria = model.CATEGORIA.NOME;
                if (model.COMUNE != null)
                    viewModel.Cerca_Citta = model.COMUNE.NOME;
                if (model.PUNTI_MAX != null)
                    viewModel.Cerca_PuntiMax = (int)model.PUNTI_MAX;
                if (model.PUNTI_MIN != null)
                    viewModel.Cerca_PuntiMin = (int)model.PUNTI_MIN;
                viewModel.Cerca_Nome = model.NOME;
            }

            return viewModel;
        }

        public static IRicercaViewModel GetViewModelByCookie()
        {
            IRicercaViewModel viewModel = null;
            HttpCookie ricerca = HttpContext.Current.Request.Cookies.Get("ricerca");
            HttpCookie filtro = HttpContext.Current.Request.Cookies.Get("filtro");
            int idCategoria = Convert.ToInt32(ricerca["IDCategoria"]);

            if (idCategoria >= 2 && idCategoria <= 11)
            {
                // macro categorie oggetto
                viewModel = new RicercaOggettoViewModel(ricerca, filtro);
            }
            else if (idCategoria >= 102 && idCategoria <= 126)
            {
                // macro categorie servizio
                viewModel = new RicercaServizioViewModel(ricerca, filtro);
            }
            else if (idCategoria == 12)
            {
                viewModel = new RicercaTelefonoViewModel(ricerca, filtro);
            }
            else if (idCategoria == 64)
            {
                viewModel = new RicercaConsoleViewModel(ricerca, filtro);
            }
            else if (idCategoria == 13 || (idCategoria >= 62 && idCategoria <= 63) || idCategoria == 65)
            {
                viewModel = new RicercaModelloViewModel(ricerca, filtro);
            }
            else if (idCategoria == 14)
            {
                viewModel = new RicercaPcViewModel(ricerca, filtro);
            }
            else if (idCategoria == 26)
            {
                viewModel = new RicercaModelloViewModel(ricerca, filtro);
            }
            else if ((idCategoria >= 28 && idCategoria <= 39) || idCategoria == 41)
            {
                viewModel = new RicercaMusicaViewModel(ricerca, filtro);
            }
            else if (idCategoria == 40)
            {
                viewModel = new RicercaModelloViewModel(ricerca, filtro);
            }
            else if (idCategoria == 45)
            {
                viewModel = new RicercaVideogamesViewModel(ricerca, filtro);
            }
            else if (idCategoria >= 42 && idCategoria <= 47)
            {
                viewModel = new RicercaModelloViewModel(ricerca, filtro);
            }
            else if (idCategoria >= 50 && idCategoria <= 61)
            {
                viewModel = new RicercaModelloViewModel(ricerca, filtro);
            }
            else if (idCategoria >= 67 && idCategoria <= 80)
            {
                viewModel = new RicercaVideoViewModel(ricerca, filtro);
            }
            else if (idCategoria >= 81 && idCategoria <= 85)
            {
                viewModel = new RicercaLibroViewModel(ricerca, filtro);
            }
            else if (idCategoria >= 89 && idCategoria <= 93)
            {
                viewModel = new RicercaVeicoloViewModel(ricerca, filtro);
            }
            else if (idCategoria >= 127 && idCategoria <= 170 && idCategoria != 161 && idCategoria != 152 && idCategoria != 141 && idCategoria != 127)
            {
                viewModel = new RicercaVestitoViewModel(ricerca, filtro);
            }
            else
            {
                viewModel = new RicercaViewModel(ricerca, filtro);
            }
            return viewModel;
        }
        #endregion
    }

    #region OGGETTI
    public class RicercaOggettoViewModel : RicercaViewModel
        {
            #region COSTRUTTORI
            public RicercaOggettoViewModel() : base()
            {
                this.Cerca_AnnoMin = 0;
                this.Cerca_AnnoMax = DateTime.Now.Year;
                ActionSalvataggio = "SaveRicercaOggetto";
            }

            public RicercaOggettoViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) 
            {
                this.Cerca_AnnoMin = 0;
                this.Cerca_AnnoMax = DateTime.Now.Year;
                ActionSalvataggio = "SaveRicercaOggetto";
            }

            public RicercaOggettoViewModel(RicercaViewModel ricerca)
            {
                this.Cerca_Nome = ricerca.Cerca_Nome;
                this.Cerca_Categoria = ricerca.Cerca_Categoria;
                this.Cerca_IDCategoria = ricerca.Cerca_IDCategoria;
                this.Cerca_PuntiMax = Convert.ToInt32(WebConfigurationManager.AppSettings["MaxPunti"]);
                this.Cerca_AnnoMin = 0;
                this.Cerca_AnnoMax = DateTime.Now.Year;
                ActionSalvataggio = "SaveRicercaOggetto";
            }

            public RicercaOggettoViewModel(HttpCookie ricerca, HttpCookie filtro) : base(ricerca, filtro)
            {
                if (ricerca["MarcaID"] != null)
                    Cerca_MarcaID = Convert.ToInt32(ricerca["MarcaID"]);
                if (ricerca["StatoOggetto"] != null)
                    Cerca_StatoOggetto = (CondizioneOggetto)Convert.ToInt32(ricerca["StatoOggetto"]);
                if (ricerca["AnnoMin"] != null)
                    Cerca_AnnoMin = Convert.ToInt32(ricerca["AnnoMin"]);
                if (ricerca["AnnoMax"] != null)
                    Cerca_AnnoMax = Convert.ToInt32(ricerca["AnnoMax"]);
                if (ricerca["TipoScambio"] != null)
                    Cerca_TipoScambio = (TipoScambio)Convert.ToInt32(ricerca["TipoScambio"]);
                //if (ricerca["Colore"] != null)
                //    Cerca_Colore = Convert.ToInt32(ricerca["Colore"]);
            }
            #endregion

            #region PROPRIETA
            public int IdRicercaOggetto { get; set; }

            [Display(Name = "StateObject", ResourceType = typeof(App_GlobalResources.Language))]
            public CondizioneOggetto? Cerca_StatoOggetto { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Brand", ResourceType = typeof(App_GlobalResources.Language))]
            public string Cerca_Marca { get; set; }

            [DataType(DataType.Text)]
            [StringLength(100, MinimumLength = 16)]
            public int? Cerca_MarcaID { get; set; }

            [Display(Name = "Min", ResourceType = typeof(App_GlobalResources.Language))]
            //[RangeDate(ErrorMessageResourceName = "ErrorMinYear", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
            [Range(int.MinValue, int.MaxValue, ErrorMessageResourceName = "ErrorMinYear", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
            public int Cerca_AnnoMin { get; set; }

            [Display(Name = "Max", ResourceType = typeof(App_GlobalResources.Language))]
            [Range(int.MinValue, int.MaxValue, ErrorMessageResourceName = "ErrorMaxYear", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
            //[RangeDate(ErrorMessageResourceName = "ErrorMaxYear", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
            public int Cerca_AnnoMax { get; set; }

            public List<Color> Cerca_Colore { get; set; }

            [Display(Name = "KidExchange", ResourceType = typeof(App_GlobalResources.Language))]
            public TipoScambio? Cerca_TipoScambio { get; set; }
            #endregion

            #region METODI PUBBLICI
            public override bool Save(DatabaseContext db, System.Web.Mvc.ControllerContext controller)
            {
                if (base.Save(db, controller))
                {
                    RICERCA_OGGETTO ricercaOggetto = new RICERCA_OGGETTO();
                    ricercaOggetto.ID_RICERCA = this.Id;
                    ricercaOggetto.ID_MARCA = this.Cerca_MarcaID;
                    ricercaOggetto.STATO_OGGETTO = (int?)this.Cerca_StatoOggetto;
                    ricercaOggetto.ANNO_MASSIMO = this.Cerca_AnnoMax;
                    ricercaOggetto.ANNO_MINIMO = this.Cerca_AnnoMin;
                    //ricercaOggetto.TIPO_SCAMBIO = this.Cerca_TipoScambio;
                    if (this.Cerca_Colore != null && this.Cerca_Colore.Count > 0)
                    {
                        ricercaOggetto.COLORE = string.Join(",", this.Cerca_Colore);
                    }
                    db.RICERCA_OGGETTO.Add(ricercaOggetto);
                    bool risultato = db.SaveChanges() > 0;
                    this.IdRicercaOggetto = ricercaOggetto.ID;
                    return risultato;
                }
                return false;
            }
            #endregion
        }

        public class RicercaModelloViewModel : RicercaOggettoViewModel
        {
            #region COSTRUTTORI
            public RicercaModelloViewModel() : base() {
                ActionSalvataggio = "SaveRicercaModello";
            }

            public RicercaModelloViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) {
                ActionSalvataggio = "SaveRicercaModello";
            }

            public RicercaModelloViewModel(HttpCookie ricerca, HttpCookie filtro) : base(ricerca, filtro)
            {
                if (ricerca["modelloID"] != null)
                    cerca_modelloID = Convert.ToInt32(ricerca["modelloID"]);
            }
            #endregion

            #region PROPRIETA
            [Range(1, int.MaxValue)]
            public int? cerca_modelloID { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Model", ResourceType = typeof(App_GlobalResources.Language))]
            public string cerca_modello { get; set; }
            #endregion

            #region METODI PUBBLICI
            public override bool Save(DatabaseContext db, System.Web.Mvc.ControllerContext controller)
            {
                if (base.Save(db, controller))
                {
                    RICERCA_OGGETTO_MODELLO model = new RICERCA_OGGETTO_MODELLO();
                    model.ID_RICERCA_OGGETTO = this.IdRicercaOggetto;
                    model.ID_MODELLO = this.cerca_modelloID;
                    db.RICERCA_OGGETTO_MODELLO.Add(model);
                    return db.SaveChanges() > 0;
                }
                return false;
            }
            #endregion
        }

        public class RicercaTelefonoViewModel : RicercaOggettoViewModel
        {
            #region COSTRUTTORI
            public RicercaTelefonoViewModel() : base() {
                ActionSalvataggio = "SaveRicercaTelefono";
            }

            public RicercaTelefonoViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) {
                ActionSalvataggio = "SaveRicercaTelefono";
            }

            public RicercaTelefonoViewModel(HttpCookie ricerca, HttpCookie filtro) : base(ricerca, filtro)
            {
                if (ricerca["modelloID"] != null)
                    cerca_modelloID = Convert.ToInt32(ricerca["modelloID"]);
                if (ricerca["sistemaOperativoID"] != null)
                    cerca_sistemaOperativoID = Convert.ToInt32(ricerca["sistemaOperativoID"]);
            }
            #endregion

            #region PROPRIETA
            [Range(1, int.MaxValue)]
            public int? cerca_modelloID { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Model", ResourceType = typeof(App_GlobalResources.Language))]
            public string cerca_modello { get; set; }

            [Range(1, int.MaxValue)]
            public int? cerca_sistemaOperativoID { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "SystemOperating", ResourceType = typeof(App_GlobalResources.Language))]
            public string cerca_sistemaOperativo { get; set; }
            #endregion

            #region METODI PUBBLICI
            public override bool Save(DatabaseContext db, System.Web.Mvc.ControllerContext controller)
            {
                if (base.Save(db, controller))
                {
                    RICERCA_OGGETTO_TELEFONO model = new RICERCA_OGGETTO_TELEFONO();
                    model.ID_RICERCA_OGGETTO = this.IdRicercaOggetto;
                    model.ID_MODELLO = this.cerca_modelloID;
                    model.ID_SISTEMA_OPERATIVO = this.cerca_sistemaOperativoID;
                    db.RICERCA_OGGETTO_TELEFONO.Add(model);
                    return db.SaveChanges() > 0;
                }
                return false;
            }
            #endregion
        }

        public class RicercaAudioHiFiViewModel : RicercaOggettoViewModel
        {
            #region COSTRUTTORI
            public RicercaAudioHiFiViewModel() : base() {
                ActionSalvataggio = "SaveRicercaAudioHiFi";
            }

            public RicercaAudioHiFiViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { ActionSalvataggio = "SaveRicercaAudioHiFi"; }

            public RicercaAudioHiFiViewModel(HttpCookie ricerca, HttpCookie filtro) : base(ricerca, filtro)
            {
                if (ricerca["modelloID"] != null)
                    cerca_modelloID = Convert.ToInt32(ricerca["modelloID"]);
            }
            #endregion

            #region PROPRIETA
            [Range(1, int.MaxValue)]
            public int? cerca_modelloID { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Model", ResourceType = typeof(App_GlobalResources.Language))]
            public string cerca_modello { get; set; }
            #endregion

            #region METODI PUBBLICI
            public override bool Save(DatabaseContext db, System.Web.Mvc.ControllerContext controller)
            {
                if (base.Save(db, controller))
                {
                    RICERCA_OGGETTO_TECNOLOGIA model = new RICERCA_OGGETTO_TECNOLOGIA();
                    model.ID_RICERCA_OGGETTO = this.IdRicercaOggetto;
                    model.ID_MODELLO = this.cerca_modelloID;
                    db.RICERCA_OGGETTO_TECNOLOGIA.Add(model);
                    return db.SaveChanges() > 0;
                }
                return false;
            }
            #endregion
        }

        public class RicercaPcViewModel : RicercaOggettoViewModel
        {
            #region COSTRUTTORI
            public RicercaPcViewModel() : base() { ActionSalvataggio = "SaveRicercaPc"; }

            public RicercaPcViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { ActionSalvataggio = "SaveRicercaPc"; }

            public RicercaPcViewModel(HttpCookie ricerca, HttpCookie filtro) : base(ricerca, filtro)
            {
                if (ricerca["modelloID"] != null)
                    cerca_modelloID = Convert.ToInt32(ricerca["modelloID"]);
                if (ricerca["sistemaOperativoID"] != null)
                    cerca_sistemaOperativoID = Convert.ToInt32(ricerca["sistemaOperativoID"]);
            }
            #endregion

            #region PROPRIETA
            [Range(1, int.MaxValue)]
            public int? cerca_modelloID { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Model", ResourceType = typeof(App_GlobalResources.Language))]
            public string cerca_modello { get; set; }

            [Range(1, int.MaxValue)]
            public int? cerca_sistemaOperativoID { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "SystemOperating", ResourceType = typeof(App_GlobalResources.Language))]
            public string cerca_sistemaOperativo { get; set; }
            #endregion

            #region METODI PUBBLICI
            public override bool Save(DatabaseContext db, System.Web.Mvc.ControllerContext controller)
            {
                if (base.Save(db, controller))
                {
                    RICERCA_OGGETTO_PC model = new RICERCA_OGGETTO_PC();
                    model.ID_RICERCA_OGGETTO = this.IdRicercaOggetto;
                    model.ID_MODELLO = this.cerca_modelloID;
                    model.ID_SISTEMA_OPERATIVO = this.cerca_sistemaOperativoID;
                    db.RICERCA_OGGETTO_PC.Add(model);
                    return db.SaveChanges() > 0;
                }
                return false;
            }
            #endregion
        }

        public class RicercaElettrodomesticoViewModel : RicercaOggettoViewModel
        {
            #region COSTRUTTORI
            public RicercaElettrodomesticoViewModel() : base() { ActionSalvataggio = "SaveRicercaElettrodomestico"; }

            public RicercaElettrodomesticoViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { ActionSalvataggio = "SaveRicercaElettrodomestico"; }

            public RicercaElettrodomesticoViewModel(HttpCookie ricerca, HttpCookie filtro) : base(ricerca, filtro)
            {
                if (ricerca["modelloID"] != null)
                    cerca_modelloID = Convert.ToInt32(ricerca["modelloID"]);
            }
            #endregion

            #region PROPRIETA
            [Range(1, int.MaxValue)]
            public int? cerca_modelloID { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Model", ResourceType = typeof(App_GlobalResources.Language))]
            public string cerca_modello { get; set; }
            #endregion

            #region METODI PUBBLICI
            public override bool Save(DatabaseContext db, System.Web.Mvc.ControllerContext controller)
            {
                if (base.Save(db, controller))
                {
                    RICERCA_OGGETTO_ELETTRODOMESTICO model = new RICERCA_OGGETTO_ELETTRODOMESTICO();
                    model.ID_RICERCA_OGGETTO = this.IdRicercaOggetto;
                    model.ID_MODELLO = this.cerca_modelloID;
                    db.RICERCA_OGGETTO_ELETTRODOMESTICO.Add(model);
                    return db.SaveChanges() > 0;
                }
                return false;
            }
            #endregion
        }

        public class RicercaStrumentoViewModel : RicercaOggettoViewModel
        {
            #region COSTRUTTORI
            public RicercaStrumentoViewModel() : base() { ActionSalvataggio = "SaveRicercaStrumento"; }

            public RicercaStrumentoViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { ActionSalvataggio = "SaveRicercaStrumento"; }

            public RicercaStrumentoViewModel(HttpCookie ricerca, HttpCookie filtro) : base(ricerca, filtro)
            {
                if (ricerca["modelloID"] != null)
                    cerca_modelloID = Convert.ToInt32(ricerca["modelloID"]);
            }
            #endregion

            #region PROPRIETA
            [Range(1, int.MaxValue)]
            public int? cerca_modelloID { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Model", ResourceType = typeof(App_GlobalResources.Language))]
            public string cerca_modello { get; set; }
            #endregion

            #region METODI PUBBLICI
            public override bool Save(DatabaseContext db, System.Web.Mvc.ControllerContext controller)
            {
                if (base.Save(db, controller))
                {
                    RICERCA_OGGETTO_STRUMENTI model = new RICERCA_OGGETTO_STRUMENTI();
                    model.ID_RICERCA_OGGETTO = this.IdRicercaOggetto;
                    model.ID_MODELLO = this.cerca_modelloID;
                    db.RICERCA_OGGETTO_STRUMENTI.Add(model);
                    return db.SaveChanges() > 0;
                }
                return false;
            }
            #endregion
        }

        public class RicercaMusicaViewModel : RicercaOggettoViewModel
        {
            #region COSTRUTTORI
            public RicercaMusicaViewModel() : base() { ActionSalvataggio = "SaveRicercaMusica"; }

            public RicercaMusicaViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { ActionSalvataggio = "SaveRicercaMusica"; }

            public RicercaMusicaViewModel(HttpCookie ricerca, HttpCookie filtro) : base(ricerca, filtro)
            {
                if (ricerca["formatoID"] != null)
                    cerca_formatoID = Convert.ToInt32(ricerca["formatoID"]);
                if (ricerca["artistaID"] != null)
                    cerca_artistaID = Convert.ToInt32(ricerca["artistaID"]);
            }
            #endregion

            #region PROPRIETA
            [Range(1, int.MaxValue)]
            public int? cerca_formatoID { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Format", ResourceType = typeof(App_GlobalResources.Language))]
            public string cerca_formato { get; set; }

            [Range(1, int.MaxValue)]
            public int? cerca_artistaID { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Artist", ResourceType = typeof(App_GlobalResources.Language))]
            public string cerca_artista { get; set; }
            #endregion

            #region METODI PUBBLICI
            public override bool Save(DatabaseContext db, System.Web.Mvc.ControllerContext controller)
            {
                if (base.Save(db, controller))
                {
                    RICERCA_OGGETTO_MUSICA model = new RICERCA_OGGETTO_MUSICA();
                    model.ID_RICERCA_OGGETTO = this.IdRicercaOggetto;
                    model.ID_ARTISTA = this.cerca_artistaID;
                    model.ID_FORMATO = this.cerca_formatoID;
                    db.RICERCA_OGGETTO_MUSICA.Add(model);
                    return db.SaveChanges() > 0;
                }
                return false;
            }
            #endregion
        }

        public class RicercaGiocoViewModel : RicercaOggettoViewModel
        {
            #region COSTRUTTORI
            public RicercaGiocoViewModel() : base() { ActionSalvataggio = "SaveRicercaGioco"; }

            public RicercaGiocoViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { ActionSalvataggio = "SaveRicercaGioco"; }

            public RicercaGiocoViewModel(HttpCookie ricerca, HttpCookie filtro) : base(ricerca, filtro)
            {
                if (ricerca["modelloID"] != null)
                    cerca_modelloID = Convert.ToInt32(ricerca["modelloID"]);
            }
            #endregion

            #region PROPRIETA
            [Range(1, int.MaxValue)]
            public int? cerca_modelloID { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Model", ResourceType = typeof(App_GlobalResources.Language))]
            public string cerca_modello { get; set; }
            #endregion

            #region METODI PUBBLICI
            public override bool Save(DatabaseContext db, System.Web.Mvc.ControllerContext controller)
            {
                if (base.Save(db, controller))
                {
                    RICERCA_OGGETTO_GIOCO model = new RICERCA_OGGETTO_GIOCO();
                    model.ID_RICERCA_OGGETTO = this.IdRicercaOggetto;
                    model.ID_MODELLO = this.cerca_modelloID;
                    db.RICERCA_OGGETTO_GIOCO.Add(model);
                    return db.SaveChanges() > 0;
                }
                return false;
            }
            #endregion
        }

        public class RicercaVideogamesViewModel : RicercaOggettoViewModel
        {
            #region COSTRUTTORI
            public RicercaVideogamesViewModel() : base() { ActionSalvataggio = "SaveRicercaVideogames"; }

            public RicercaVideogamesViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { ActionSalvataggio = "SaveRicercaVideogames"; }

            public RicercaVideogamesViewModel(HttpCookie ricerca, HttpCookie filtro) : base(ricerca, filtro)
            {
                if (ricerca["piattaformaID"] != null)
                    cerca_piattaformaID = Convert.ToInt32(ricerca["piattaformaID"]);
                if (ricerca["genereID"] != null)
                    cerca_genereID = Convert.ToInt32(ricerca["genereID"]);
            }
            #endregion

            #region PROPRIETA
            [Range(1, int.MaxValue)]
            public int? cerca_piattaformaID { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Platform", ResourceType = typeof(App_GlobalResources.Language))]
            public string cerca_piattaforma { get; set; }

            [Range(1, int.MaxValue)]
            public int? cerca_genereID { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Kind", ResourceType = typeof(App_GlobalResources.Language))]
            public string cerca_genere { get; set; }
            #endregion

            #region METODI PUBBLICI
            public override bool Save(DatabaseContext db, System.Web.Mvc.ControllerContext controller)
            {
                if (base.Save(db, controller))
                {
                    RICERCA_OGGETTO_VIDEOGAMES model = new RICERCA_OGGETTO_VIDEOGAMES();
                    model.ID_RICERCA_OGGETTO = this.IdRicercaOggetto;
                    model.ID_PIATTAFORMA = this.cerca_piattaformaID;
                    model.ID_GENERE = this.cerca_genereID;
                    db.RICERCA_OGGETTO_VIDEOGAMES.Add(model);
                    return (db.SaveChanges() > 0);
                }
                return false;
            }
            #endregion
        }

        public class RicercaSportViewModel : RicercaOggettoViewModel
        {
            #region COSTRUTTORI
            public RicercaSportViewModel() : base() { ActionSalvataggio = "SaveRicercaSport"; }

            public RicercaSportViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { ActionSalvataggio = "SaveRicercaSport"; }

            public RicercaSportViewModel(HttpCookie ricerca, HttpCookie filtro) : base(ricerca, filtro)
            {
                if (ricerca["modelloID"] != null)
                    cerca_modelloID = Convert.ToInt32(ricerca["modelloID"]);
            }
            #endregion

            #region PROPRIETA
            [Range(1, int.MaxValue)]
            public int? cerca_modelloID { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Model", ResourceType = typeof(App_GlobalResources.Language))]
            public string cerca_modello { get; set; }
            #endregion

            #region METODI PUBBLICI
            public override bool Save(DatabaseContext db, System.Web.Mvc.ControllerContext controller)
            {
                if (base.Save(db, controller))
                {
                    RICERCA_OGGETTO_SPORT model = new RICERCA_OGGETTO_SPORT();
                    model.ID_RICERCA_OGGETTO = this.IdRicercaOggetto;
                    model.ID_MODELLO = this.cerca_modelloID;
                    db.RICERCA_OGGETTO_SPORT.Add(model);
                    return db.SaveChanges() > 0;
                }
                return false;
            }
            #endregion
        }

        public class RicercaTecnologiaViewModel : RicercaOggettoViewModel
        {
            #region COSTRUTTORI
            public RicercaTecnologiaViewModel() : base() { ActionSalvataggio = "SaveRicercaTecnologia"; }

            public RicercaTecnologiaViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { ActionSalvataggio = "SaveRicercaTecnologia"; }

            public RicercaTecnologiaViewModel(HttpCookie ricerca, HttpCookie filtro) : base(ricerca, filtro)
            {
                if (ricerca["modelloID"] != null)
                    cerca_modelloID = Convert.ToInt32(ricerca["modelloID"]);
            }
            #endregion

            #region PROPRIETA
            [Range(1, int.MaxValue)]
            public int? cerca_modelloID { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Model", ResourceType = typeof(App_GlobalResources.Language))]
            public string cerca_modello { get; set; }
            #endregion

            #region METODI PUBBLICI
            public override bool Save(DatabaseContext db, System.Web.Mvc.ControllerContext controller)
            {
                if (base.Save(db, controller))
                {
                    RICERCA_OGGETTO_TECNOLOGIA model = new RICERCA_OGGETTO_TECNOLOGIA();
                    model.ID_RICERCA_OGGETTO = this.IdRicercaOggetto;
                    model.ID_MODELLO = this.cerca_modelloID;
                    db.RICERCA_OGGETTO_TECNOLOGIA.Add(model);
                    return db.SaveChanges() > 0;
                }
                return false;
            }
            #endregion
        }

        public class RicercaConsoleViewModel : RicercaOggettoViewModel
        {
            #region COSTRUTTORI
            public RicercaConsoleViewModel() : base() { ActionSalvataggio = "SaveRicercaConsole"; }

            public RicercaConsoleViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { ActionSalvataggio = "SaveRicercaConsole"; }

            public RicercaConsoleViewModel(HttpCookie ricerca, HttpCookie filtro) : base(ricerca, filtro)
            {
                if (ricerca["piattaformaID"] != null)
                    cerca_piattaformaID = Convert.ToInt32(ricerca["piattaformaID"]);
            }
            #endregion

            #region PROPRIETA
            [Range(1, int.MaxValue)]
            public int? cerca_piattaformaID { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Platform", ResourceType = typeof(App_GlobalResources.Language))]
            public string cerca_piattaforma { get; set; }
            #endregion

            #region METODI PUBBLICI
            public override bool Save(DatabaseContext db, System.Web.Mvc.ControllerContext controller)
            {
                if (base.Save(db, controller))
                {
                    RICERCA_OGGETTO_CONSOLE model = new RICERCA_OGGETTO_CONSOLE();
                    model.ID_RICERCA_OGGETTO = this.IdRicercaOggetto;
                    model.ID_PIATTAFORMA = this.cerca_piattaformaID;
                    db.RICERCA_OGGETTO_CONSOLE.Add(model);
                    return db.SaveChanges() > 0;
                }
                return false;
            }
            #endregion
        }

        public class RicercaVideoViewModel : RicercaOggettoViewModel
        {
            #region COSTRUTTORI
            public RicercaVideoViewModel() : base() { ActionSalvataggio = "SaveRicercaVideo"; }

            public RicercaVideoViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { ActionSalvataggio = "SaveRicercaVideo"; }

            public RicercaVideoViewModel(HttpCookie ricerca, HttpCookie filtro) : base(ricerca, filtro)
            {
                if (ricerca["formatoID"] != null)
                    cerca_formatoID = Convert.ToInt32(ricerca["formatoID"]);
                if (ricerca["registaID"] != null)
                    cerca_registaID = Convert.ToInt32(ricerca["registaID"]);
            }
            #endregion

            #region PROPRIETA
            [Range(1, int.MaxValue)]
            public int? cerca_formatoID { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Support", ResourceType = typeof(App_GlobalResources.Language))]
            public string cerca_formato { get; set; }

            [Range(1, int.MaxValue)]
            public int? cerca_registaID { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Director", ResourceType = typeof(App_GlobalResources.Language))]
            public string cerca_regista { get; set; }
            #endregion

            #region METODI PUBBLICI
            public override bool Save(DatabaseContext db, System.Web.Mvc.ControllerContext controller)
            {
                if (base.Save(db, controller))
                {
                    RICERCA_OGGETTO_VIDEO model = new RICERCA_OGGETTO_VIDEO();
                    model.ID_RICERCA_OGGETTO = this.IdRicercaOggetto;
                    model.ID_REGISTA = this.cerca_registaID;
                    model.ID_FORMATO = this.cerca_formatoID;
                    db.RICERCA_OGGETTO_VIDEO.Add(model);
                    return db.SaveChanges() > 0;
                }
                return false;
            }
            #endregion
        }

        public class RicercaLibroViewModel : RicercaOggettoViewModel
        {
            #region COSTRUTTORI
            public RicercaLibroViewModel() : base() { ActionSalvataggio = "SaveRicercaLibro"; }

            public RicercaLibroViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { ActionSalvataggio = "SaveRicercaLibro"; }

            public RicercaLibroViewModel(HttpCookie ricerca, HttpCookie filtro) : base(ricerca, filtro)
            {
                if (ricerca["autoreID"] != null)
                    cerca_autoreID = Convert.ToInt32(ricerca["autoreID"]);
            }
            #endregion

            #region PROPRIETA
            [Range(1, int.MaxValue)]
            public int? cerca_autoreID { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Author", ResourceType = typeof(App_GlobalResources.Language))]
            public string cerca_autore { get; set; }
            #endregion

            #region METODI PUBBLICI
            public override bool Save(DatabaseContext db, System.Web.Mvc.ControllerContext controller)
            {
                if (base.Save(db, controller))
                {
                    RICERCA_OGGETTO_AUTORE model = new RICERCA_OGGETTO_AUTORE();
                    model.ID_RICERCA_OGGETTO = this.IdRicercaOggetto;
                    model.ID_AUTORE = this.cerca_autoreID;
                    db.RICERCA_OGGETTO_AUTORE.Add(model);
                    return db.SaveChanges() > 0;
                }
                return false;
            }
            #endregion
        }

        public class RicercaVeicoloViewModel : RicercaOggettoViewModel
        {
            #region COSTRUTTORI
            public RicercaVeicoloViewModel() : base() { ActionSalvataggio = "SaveRicercaVeicolo"; }

            public RicercaVeicoloViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { ActionSalvataggio = "SaveRicercaVeicolo"; }

            public RicercaVeicoloViewModel(HttpCookie ricerca, HttpCookie filtro) : base(ricerca, filtro)
            {
                if (ricerca["modelloID"] != null)
                    cerca_modelloID = Convert.ToInt32(ricerca["modelloID"]);
                if (ricerca["alimentazioneID"] != null)
                    cerca_alimentazioneID = Convert.ToInt32(ricerca["alimentazioneID"]);
            }
            #endregion

            #region PROPRIETA
            [Range(1, int.MaxValue)]
            public int? cerca_modelloID { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Model", ResourceType = typeof(App_GlobalResources.Language))]
            public string cerca_modello { get; set; }

            [Range(1, int.MaxValue)]
            public int? cerca_alimentazioneID { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Feed", ResourceType = typeof(App_GlobalResources.Language))]
            public string cerca_alimentazione { get; set; }
            #endregion

            #region METODI PUBBLICI
            public override bool Save(DatabaseContext db, System.Web.Mvc.ControllerContext controller)
            {
                if (base.Save(db, controller))
                {
                    RICERCA_OGGETTO_VEICOLO model = new RICERCA_OGGETTO_VEICOLO();
                    model.ID_RICERCA_OGGETTO = this.IdRicercaOggetto;
                    model.ID_MODELLO = this.cerca_modelloID;
                    model.ID_ALIMENTAZIONE = this.cerca_alimentazioneID;
                    db.RICERCA_OGGETTO_VEICOLO.Add(model);
                    return db.SaveChanges() > 0;
                }
                return false;
            }
            #endregion
        }

        public class RicercaVestitoViewModel : RicercaOggettoViewModel
        {
            #region COSTRUTTORI
            public RicercaVestitoViewModel() : base() { ActionSalvataggio = "SaveRicercaVestito"; }

            public RicercaVestitoViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { ActionSalvataggio = "SaveRicercaVestito"; }

            public RicercaVestitoViewModel(HttpCookie ricerca, HttpCookie filtro) : base(ricerca, filtro)
            {
                if (ricerca["taglia"] != null)
                    cerca_taglia = ricerca["taglia"];
            }
            #endregion

            #region PROPRIETA
            [DataType(DataType.Text)]
            [Display(Name = "Size", ResourceType = typeof(App_GlobalResources.Language))]
            public string cerca_taglia { get; set; }
            #endregion

            #region METODI PUBBLICI
            public override bool Save(DatabaseContext db, System.Web.Mvc.ControllerContext controller)
            {
                if (base.Save(db, controller))
                {
                    RICERCA_OGGETTO_VESTITO model = new RICERCA_OGGETTO_VESTITO();
                    model.ID_RICERCA_OGGETTO = this.IdRicercaOggetto;
                    model.TAGLIA = this.cerca_taglia;
                    db.RICERCA_OGGETTO_VESTITO.Add(model);
                    return db.SaveChanges() > 0;
                }
                return false;
            }
            #endregion
        }
        #endregion

    #region SERVIZI
    public class RicercaServizioViewModel : RicercaViewModel
    {
        #region COSTRUTTORI
        public RicercaServizioViewModel() { ActionSalvataggio = "SaveRicercaServizio"; }

        public RicercaServizioViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { ActionSalvataggio = "SaveRicercaServizio"; }

        public RicercaServizioViewModel(RicercaViewModel ricerca)
        {
            this.Cerca_Nome = ricerca.Cerca_Nome;
            this.Cerca_Categoria = ricerca.Cerca_Categoria;
            this.Cerca_IDCategoria = ricerca.Cerca_IDCategoria;
            ActionSalvataggio = "SaveRicercaServizio";
        }

        public RicercaServizioViewModel(HttpCookie ricerca, HttpCookie filtro) : base(ricerca,filtro)
        {
            if (ricerca["Lunedi"] != null)
                Cerca_Lunedi = Convert.ToBoolean(ricerca["Lunedi"]);
            if (ricerca["Martedi"] != null)
                Cerca_Martedi = Convert.ToBoolean(ricerca["Martedi"]);
            if (ricerca["Mercoledi"] != null)
                Cerca_Mercoledi = Convert.ToBoolean(ricerca["Mercoledi"]);
            if (ricerca["Giovedi"] != null)
                Cerca_Giovedi = Convert.ToBoolean(ricerca["Giovedi"]);
            if (ricerca["Venerdi"] != null)
                Cerca_Venerdi = Convert.ToBoolean(ricerca["Venerdi"]);
            if (ricerca["Sabato"] != null)
                Cerca_Sabato = Convert.ToBoolean(ricerca["Sabato"]);
            if (ricerca["Domenica"] != null)
                Cerca_Domenica = Convert.ToBoolean(ricerca["Domenica"]);
            if (ricerca["Tutti"] != null)
                Cerca_Tutti = Convert.ToBoolean(ricerca["Tutti"]);
            if (ricerca["Tariffa"] != null)
                Tariffa = (Tariffa)Convert.ToInt32(ricerca["Tariffa"]);
        }
        #endregion

        #region PROPRIETA
        public int IdRicercaServizio { get; set; }

        [Display(Name = "Monday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Cerca_Lunedi { get; set; }

        [Display(Name = "Tuesday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Cerca_Martedi { get; set; }

        [Display(Name = "Wednesday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Cerca_Mercoledi { get; set; }

        [Display(Name = "Thursday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Cerca_Giovedi { get; set; }

        [Display(Name = "Friday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Cerca_Venerdi { get; set; }

        [Display(Name = "Saturday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Cerca_Sabato { get; set; }

        [Display(Name = "Sunday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Cerca_Domenica { get; set; }

        [Display(Name = "AllDays", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Cerca_Tutti { get; set; }

        [Display(Name = "Rate", ResourceType = typeof(App_GlobalResources.Language))]
        public Tariffa Tariffa { get; set; }
        /*
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "StartTime", ResourceType = typeof(App_GlobalResources.Language))]
        public TimeSpan? Cerca_OraInizio { get; set; }

        //[DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "EndTime", ResourceType = typeof(App_GlobalResources.Language))]
        public TimeSpan? Cerca_OraFine { get; set; }

        [Display(Name = "Holidays", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Cerca_Festivi { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Cerca_OraFine < Cerca_OraInizio)
            {
                yield return new ValidationResult(App_GlobalResources.Language.ErrorEndTime);
            }
        }
        */
        #endregion

        #region METODI PUBBLICI
        public override bool Save(DatabaseContext db, System.Web.Mvc.ControllerContext controller)
        {
            if (base.Save(db, controller))
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("ricerca");
                PersonaModel utente = (HttpContext.Current.Request.IsAuthenticated) ? (HttpContext.Current.Session["utente"] as PersonaModel) : (HttpContext.Current.Session["utenteRicerca"] as PersonaModel);
            }
            return false;
        }
        #endregion
    }
    #endregion

    #endregion

    #region ELENCHI OGGETTI/SERVIZI

    public interface IListaOggetti<T> : IList<T>
    {
        List<T> List { get; set; }

        int PageCount { get; set; }

        int PageNumber { get; set; }
    }

    public class ListaOggetti
    {
        public virtual List<OggettoViewModel> List { get; set; }

        public int PageCount { get; set; }

        public int PageNumber { get; set; }

        public int TotalNumber { get; set; }
    }

    public class ListaServizi
    {
        public virtual List<ServizioViewModel> List { get; set; }

        public int PageCount { get; set; }

        public int PageNumber { get; set; }

        public int TotalNumber { get; set; }
    }

    public class ListaVendite
    {
        public ListaVendite(int idCategoria, string categoria)
        {
            this.IdCategoria = idCategoria;
            this.Categoria = categoria;
        }

        public virtual List<AnnuncioViewModel> List { get; set; }

        public int PageCount { get; set; }

        public int PageNumber { get; set; }

        public int TotalNumber { get; set; }

        public int IdCategoria { get; set; }

        public string Categoria { get; set; }
    }

    #endregion

    public class RegistrazioneRicercaViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessageResourceName = "ErrorFormatEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [StringLength(200, ErrorMessageResourceName = "ErrorLengthEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public string Email { get; set; }
    }
}
