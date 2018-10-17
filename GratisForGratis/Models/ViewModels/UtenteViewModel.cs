using GratisForGratis.Controllers;
using GratisForGratis.DataAnnotations;
using SimpleCrypto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GratisForGratis.Models
{
    public class UtenteLoginViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessageResourceName = "ErrorFormatEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [StringLength(200, ErrorMessageResourceName = "ErrorLengthEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(150,MinimumLength =6)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "StoresUser", ResourceType = typeof(App_GlobalResources.Language))]
        public bool RicordaLogin { get; set; }

        public string ReturnUrl { get; set; }

        public string ErroreLogin;
    }

    public class UtenteRegistrazioneViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessageResourceName = "ErrorFormatEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [StringLength(200, ErrorMessageResourceName = "ErrorLengthEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(150, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(150, MinimumLength = 6)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(App_GlobalResources.Language))]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessageResourceName = "ErrorComparePassword", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public string ConfermaPassword { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Name", ResourceType = typeof(App_GlobalResources.Language))]
        public string Nome { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Surname", ResourceType = typeof(App_GlobalResources.Language))]
        public string Cognome { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequiredField", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [DataType(DataType.PhoneNumber)]
        [StringLength(12, MinimumLength = 9, ErrorMessageResourceName = "ErrorPhoneNumber", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessageResourceName = "ErrorPhoneNumber", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "Telephone", ResourceType = typeof(App_GlobalResources.Language))]
        public string Telefono { get; set; }

        [Required]
        [Range(typeof(bool), "true", "true", ErrorMessageResourceName = "ErrorTermsAndConditions", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "AcceptConditions", ResourceType = typeof(App_GlobalResources.Language))]
        public bool AccettaCondizioni { get; set; }

        public string ReturnUrl { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequiredField", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [DataType(DataType.PostalCode, ErrorMessageResourceName = "ErrorCity", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "ResidenceCity", ResourceType = typeof(App_GlobalResources.Language))]
        public string Citta { get; set; }

        public int? IDCitta { get; set; }

        [Required( ErrorMessageResourceName = "ErrorRequiredField", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 5)]
        [Display(Name = "ResidenceAddress", ResourceType = typeof(App_GlobalResources.Language))]
        public string Indirizzo { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequiredField", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]     
        [Display(Name = "Civic", ResourceType = typeof(App_GlobalResources.Language))]
        public int? Civico { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequiredField", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [DataType(DataType.PostalCode, ErrorMessageResourceName = "ErrorCity", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "ShippingCity", ResourceType = typeof(App_GlobalResources.Language))]
        public string CittaSpedizione { get; set; }

        public int? IDCittaSpedizione { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequiredField", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 5)]
        [Display(Name = "ShippingAddress", ResourceType = typeof(App_GlobalResources.Language))]
        public string IndirizzoSpedizione { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequiredField", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "ShippingCivic", ResourceType = typeof(App_GlobalResources.Language))]
        public int? CivicoSpedizione { get; set; }

    }

    public class UtenteLoginVeloceViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessageResourceName = "ErrorFormatEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [StringLength(200, ErrorMessageResourceName = "ErrorLengthEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(150, MinimumLength = 6)]
        public string Password { get; set; }

        public string Nome { get; set; }

        public string Cognome { get; set; }

        public string FacebookToken { get; set; }

        public string FacebookTokenPermanente { get; set; }

        [Required]
        [Display(Name = "StoresUser", ResourceType = typeof(App_GlobalResources.Language))]
        public bool RicordaLogin { get; set; }

        public string ReturnUrl { get; set; }

        public string ErroreLogin;

        public bool SalvaRegistrazione(ControllerContext controller, DatabaseContext db)
        {
            CONTO_CORRENTE conto = db.CONTO_CORRENTE.Create();
            conto.ID = Guid.NewGuid();
            conto.TOKEN = Guid.NewGuid();
            conto.DATA_INSERIMENTO = DateTime.Now;
            conto.STATO = (int)Stato.ATTIVO;
            db.CONTO_CORRENTE.Add(conto);
            db.SaveChanges();
            PBKDF2 crypto = new PBKDF2();
            PERSONA persona = db.PERSONA.Create();
            persona.TOKEN = Guid.NewGuid();
            persona.TOKEN_PASSWORD = crypto.GenerateSalt(1, 20);
            persona.PASSWORD = crypto.Compute(this.Password.Trim(), persona.TOKEN_PASSWORD);
            persona.ID_CONTO_CORRENTE = conto.ID;
            persona.ID_ABBONAMENTO = db.ABBONAMENTO.SingleOrDefault(item => item.NOME == "BASE").ID;
            persona.DATA_INSERIMENTO = DateTime.Now;
            persona.STATO = (int)Stato.INATTIVO;

            // solo in caso di accesso con FB
            persona.FACEBOOK_TOKEN_SESSIONE = FacebookToken;
            persona.FACEBOOK_TOKEN_PERMANENTE = FacebookTokenPermanente;
            if (!string.IsNullOrWhiteSpace(this.Nome))
                persona.NOME = this.Nome.Trim();
            if (!string.IsNullOrWhiteSpace(this.Cognome))
                persona.COGNOME = this.Cognome.Trim();

            db.PERSONA.Add(persona);
            if (db.SaveChanges() > 0)
            {
                PERSONA_EMAIL personaEmail = db.PERSONA_EMAIL.Create();
                personaEmail.ID_PERSONA = persona.ID;
                personaEmail.EMAIL = this.Email.Trim();
                personaEmail.TIPO = (int)TipoEmail.Registrazione;
                personaEmail.DATA_INSERIMENTO = DateTime.Now;
                personaEmail.STATO = (int)Stato.INATTIVO;
                db.PERSONA_EMAIL.Add(personaEmail);

                if (db.SaveChanges() > 0)
                {
                    InvioEmail(controller, persona, personaEmail);
                    return true;
                }
            }
            return false;
        }

        public void InvioEmail(ControllerContext controller, PERSONA persona, PERSONA_EMAIL personaEmail)
        {
            EmailModel email = new EmailModel(controller);
            email.To.Add(new System.Net.Mail.MailAddress(personaEmail.EMAIL, personaEmail.EMAIL));
            email.Subject = App_GlobalResources.Email.RegistrationSubject + " - " + WebConfigurationManager.AppSettings["nomeSito"];
            email.Body = "RegistrazioneUtente";
            email.DatiEmail = new RegistrazioneEmailModel(this)
            {
                PasswordCodificata = persona.PASSWORD
            };
            new EmailController().SendEmail(email);
        }

    }

    public class UtentePasswordDimenticataViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessageResourceName = "ErrorFormatEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [StringLength(200, ErrorMessageResourceName = "ErrorLengthEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string NuovaPassword { get; set; }
    }

    public class UtenteImpostazioniViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessageResourceName = "ErrorFormatEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [StringLength(200, ErrorMessageResourceName =  "ErrorLengthEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Name", ResourceType = typeof(App_GlobalResources.Language))]
        public string Nome { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Surname", ResourceType = typeof(App_GlobalResources.Language))]
        public string Cognome { get; set; }

        [RequiredIfNotNull(new string[] { "Civico", "Indirizzo" }, ErrorMessageResourceName = "ErrorRequiredField", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [DataType(DataType.PostalCode, ErrorMessageResourceName = "ErrorCity", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "ResidenceCity", ResourceType = typeof(App_GlobalResources.Language))]
        public string Citta { get; set; }

        public int? IDCitta { get; set; }

        [RequiredIfNotNull(new string[] { "Citta", "Civico" }, ErrorMessageResourceName = "ErrorRequiredField", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 5)]
        [Display(Name = "ResidenceAddress", ResourceType = typeof(App_GlobalResources.Language))]
        public string Indirizzo { get; set; }

        [RequiredIfNotNull(new string[] { "Citta", "Indirizzo" }, ErrorMessageResourceName = "ErrorRequiredField", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        //[DataType(DataType.Text)]
        //[StringLength(50, MinimumLength = 1)]
        [Display(Name = "Civic", ResourceType = typeof(App_GlobalResources.Language))]
        public int? Civico { get; set; }

        [DataType(DataType.PostalCode, ErrorMessageResourceName = "ErrorCity", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "ShippingCity", ResourceType = typeof(App_GlobalResources.Language))]
        public string CittaSpedizione { get; set; }

        public int? IDCittaSpedizione { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 5)]
        [Display(Name = "ShippingAddress", ResourceType = typeof(App_GlobalResources.Language))]
        public string IndirizzoSpedizione { get; set; }

        //[DataType(DataType.Text)]
        //[StringLength(50, MinimumLength = 1)]
        [Display(Name = "ShippingCivic", ResourceType = typeof(App_GlobalResources.Language))]
        public int? CivicoSpedizione { get; set; }

        [DataType(DataType.PhoneNumber)]
        [StringLength(12, MinimumLength = 9, ErrorMessageResourceName = "ErrorPhoneNumber", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessageResourceName = "ErrorPhoneNumber", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "Telephone", ResourceType = typeof(App_GlobalResources.Language))]
        public string Telefono { get; set; }

        [Required]
        [Display(Name = "AcceptConditions", ResourceType = typeof(App_GlobalResources.Language))]
        public bool AccettaCondizioni { get; set; }

        public string ImmagineProfilo { get; set; }

        public bool HasLoginFacebook { get; set; }
    }

    public class UtenteCambioPasswordViewModel
    {

        [DataType(DataType.Password)]
        [StringLength(150, MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [StringLength(150, MinimumLength = 6)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(App_GlobalResources.Language))]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessageResourceName = "ErrorComparePassword", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public string ConfermaPassword { get; set; }

    }

    public class RegistrazioneEmailModel : UtenteRegistrazioneViewModel
    {
        public RegistrazioneEmailModel(UtenteRegistrazioneViewModel model)
        {
            foreach (System.Reflection.PropertyInfo propertyInfo in model.GetType().GetProperties())
            {
                this.GetType().GetProperty(propertyInfo.Name).SetValue(this, propertyInfo.GetValue(model));
            }
        }

        public RegistrazioneEmailModel(UtenteLoginVeloceViewModel model)
        {
            foreach (System.Reflection.PropertyInfo propertyInfo in model.GetType().GetProperties())
            {
                if (this.GetType().GetProperty(propertyInfo.Name)!=null)
                    this.GetType().GetProperty(propertyInfo.Name).SetValue(this, propertyInfo.GetValue(model));
            }
        }

        public string PasswordCodificata { get; set; }
    }

    public class UtenteVenditaViewModel
    {
        public UtenteVenditaViewModel() { }
        public UtenteVenditaViewModel(PERSONA model)
        {
            /*
            foreach (System.Reflection.PropertyInfo propertyInfo in model.GetType().GetProperties())
            {
                // verifico l'esistenza della proprietà
                if (this.GetType().GetProperty(propertyInfo.Name.First().ToString() + propertyInfo.Name.ToLower().Substring(1)) != null)
                    this.GetType().GetProperty(propertyInfo.Name.First().ToString() + propertyInfo.Name.ToLower().Substring(1)).SetValue(this, propertyInfo.GetValue(model));
            }*/
            Id = model.ID;
            Nominativo = model.NOME + " " + model.COGNOME;
            Email = model.PERSONA_EMAIL.FirstOrDefault(m => m.TIPO == (int)TipoEmail.Registrazione).EMAIL;
            PERSONA_TELEFONO telefono = model.PERSONA_TELEFONO.FirstOrDefault(m => m.TIPO == (int)TipoTelefono.Privato);
            if (telefono!=null)
                Telefono = telefono.TELEFONO;
            VenditoreToken = model.TOKEN;
        }

        //[DataType(DataType.Text)]
        [Display(Name = "Seller", ResourceType = typeof(App_GlobalResources.Language))]
        public int Id { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Nominative", ResourceType = typeof(App_GlobalResources.Language))]
        public string Nominativo { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessageResourceName = "ErrorEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "Email", ResourceType = typeof(App_GlobalResources.Language))]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber, ErrorMessageResourceName = "ErrorPhoneNumber", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessageResourceName = "ErrorPhoneNumber", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "Telephone", ResourceType = typeof(App_GlobalResources.Language))]
        public string Telefono { get; set; }

        public Guid VenditoreToken { get; set; }
    }

    public class UtenteRicercaViewModel /*: EncoderViewModel*/
    {
        #region COSTRUTTORI
        public UtenteRicercaViewModel() { }

        public UtenteRicercaViewModel(PERSONA_RICERCA model)
        {
            LoadProprieta(model);
        }
        #endregion

        #region PROPRIETA
        public int Id { get; set; }

        public string Token { get; set; }

        public string Testo { get; set; }

        public string Categoria { get; set; }

        public DateTime DataInserimento { get; set; }

        public DateTime DataModifica { get; set; }

        public Stato Stato { get; set; }
        #endregion

        #region METODI PUBBLICI
        public bool SaveRicerca(DatabaseContext db, ControllerContext controller)
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
                    ResetFiltriRicerca();
                    LoadProprieta(personaRicerca);
                    SendMailRicercaSalvata(utente, personaRicerca, controller);
                }
            }
            return false;
        }
        #endregion

        #region METODI PRIVATI
        private void LoadProprieta(PERSONA_RICERCA model)
        {
            List<FINDSOTTOCATEGORIE_Result> categorie = (HttpContext.Current.Application["categorie"] as List<FINDSOTTOCATEGORIE_Result>);
            FINDSOTTOCATEGORIE_Result categoria = categorie.SingleOrDefault(item => item.ID == model.RICERCA.ID_CATEGORIA);
            this.Id = model.ID;
            this.Categoria = categoria.NOME;
            this.Testo = model.RICERCA.NOME;
            this.DataInserimento = model.RICERCA.DATA_INSERIMENTO;
            this.DataModifica = (DateTime)model.RICERCA.DATA_MODIFICA;
            this.Stato = (Stato)model.RICERCA.STATO;
        }

        private void ResetFiltriRicerca()
        {
            HttpCookie ricerca = new HttpCookie("ricerca");
            ricerca["IDCategoria"] = "1";
            ricerca["Categoria"] = "Tutti";
            HttpCookie filtro = new HttpCookie("filtro");
            HttpContext.Current.Response.SetCookie(ricerca);
            HttpContext.Current.Response.SetCookie(filtro);
        }

        private void SendMailRicercaSalvata(PersonaModel utente, PERSONA_RICERCA model, ControllerContext controller)
        {
            // invio email salvataggio ricerca
            EmailModel email = new EmailModel(controller);
            email.To.Add(new System.Net.Mail.MailAddress(utente.Email.FirstOrDefault(item => item.TIPO == (int)TipoEmail.Registrazione).EMAIL, utente.Persona.NOME + ' ' + utente.Persona.COGNOME));
            email.Subject = string.Format(App_GlobalResources.Email.SearchSaveSubject, model.RICERCA.NOME) + " - " + WebConfigurationManager.AppSettings["nomeSito"];
            email.Body = "SalvataggioRicerca";
            email.DatiEmail = model;
            new EmailController().SendEmail(email);
        }
        #endregion
    }

    public class UtenteNotificaViewModel
    {
        #region PROPRIETA
        public int Id { get; set; }

        public string Persona { get; set; }

        public string Attivita { get; set; }

        public string Messaggio { get; set; }

        public DateTime DataInserimento { get; set; }

        public DateTime? DataModifica { get; set; }

        public DateTime? DataLettura { get; set; }

        public StatoNotifica Stato { get; set; }

        public TipoNotifica Tipo { get; set; }

        public AnnuncioNotificaModel AnnuncioNotifica { get; set; }
        #endregion

        #region METODI PUBBLICI
        public void getTipoNotifica(DatabaseContext db, NOTIFICA model)
        {
            Id = model.ID;
            Persona = model.PERSONA.NOME + " " + model.PERSONA.COGNOME;
            if (model.ID_ATTIVITA != null)
                Attivita = model.ATTIVITA.NOME;
            Messaggio = Components.EnumHelper<TipoNotifica>.GetDisplayValue((TipoNotifica)model.MESSAGGIO);
            DataInserimento = model.DATA_INSERIMENTO;
            DataModifica = model.DATA_MODIFICA;
            DataLettura = model.DATA_LETTURA;
            Stato = (StatoNotifica)model.STATO;
            Tipo = (TipoNotifica)model.MESSAGGIO;
            if (model.ANNUNCIO_NOTIFICA.Count > 0)
            {
                this.AnnuncioNotifica = new AnnuncioNotificaModel();
                this.AnnuncioNotifica.Annuncio = new AnnuncioViewModel(db, model.ANNUNCIO_NOTIFICA.SingleOrDefault().ANNUNCIO);
            }
        }
        #endregion
    }

    public class UtentePrivacyViewModel
    {
        #region PROPRIETA
        public bool AccettaCondizioni { get; set; }

        public bool Comunicazioni { get; set; }

        public bool? NotificaEmail { get; set; }

        public bool? NotificaChat { get; set; }

        public TipoChat? Chat { get; set; }
        #endregion

        #region COSTRUTTORI
        public UtentePrivacyViewModel()
        {

        }

        public UtentePrivacyViewModel(PERSONA_PRIVACY model)
        {
            AccettaCondizioni = model.ACCETTA_CONDIZIONE;
            Comunicazioni = model.COMUNICAZIONI;
            NotificaEmail = model.NOTIFICA_EMAIL;
            NotificaChat = model.NOTIFICA_CHAT;
            Chat = (TipoChat?)model.CHAT;
        }
        #endregion
    }

    public class UtenteProfiloViewModel
    {
        public string Token { get; set; }
        
        public string Email { get; set; }

        [Display(Name = "Name", ResourceType = typeof(App_GlobalResources.Language))]
        public string Nome { get; set; }

        [Display(Name = "Surname", ResourceType = typeof(App_GlobalResources.Language))]
        public string Cognome { get; set; }

        [Display(Name = "Telephone", ResourceType = typeof(App_GlobalResources.Language))]
        public string Telefono { get; set; }

        [Display(Name = "ResidenceCity", ResourceType = typeof(App_GlobalResources.Language))]
        public string Citta { get; set; }
        
        [Display(Name = "ResidenceAddress", ResourceType = typeof(App_GlobalResources.Language))]
        public string Indirizzo { get; set; }

        [Display(Name = "Civic", ResourceType = typeof(App_GlobalResources.Language))]
        public int? Civico { get; set; }

        [Display(Name = "ShippingCity", ResourceType = typeof(App_GlobalResources.Language))]
        public string CittaSpedizione { get; set; }

        [Display(Name = "ShippingAddress", ResourceType = typeof(App_GlobalResources.Language))]
        public string IndirizzoSpedizione { get; set; }

        [Display(Name = "ShippingCivic", ResourceType = typeof(App_GlobalResources.Language))]
        public int? CivicoSpedizione { get; set; }

        // AGGIUNGERE LISTA ACQUISTI UTENTE
        public List<AnnuncioViewModel> listaAcquisti { get; set; }
        // AGGIUNGERE LISTA DESIDERI UTENTE
        public List<AnnuncioViewModel> listaDesideri { get; set; }
    }

}
