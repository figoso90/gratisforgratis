using GratisForGratis.Controllers;
using Recaptcha.Web;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Web.Hosting;

namespace GratisForGratis.Models
{
    /// <summary>
    /// DEPRECATED: Sostituita dalla view dei contatti
    /// </summary>
    public class SegnalazioneViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessageResourceName = "ErrorFormatEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [StringLength(200, ErrorMessageResourceName = "ErrorLengthEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "AnswerEmail", ResourceType = typeof(App_GlobalResources.Language))]
        public string EmailRisposta { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "MailObject", ResourceType = typeof(App_GlobalResources.Language))]
        public string Oggetto { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(4000, MinimumLength = 10)]
        [Display(Name = "Text", ResourceType = typeof(App_GlobalResources.Language))]
        public string Testo { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Attachment", ResourceType = typeof(App_GlobalResources.Language))]
        public System.Web.HttpPostedFileBase Allegato { get; set; }

        [Required]
        public string Controller { get; set; }

        [Required]
        public string Vista { get; set; }

        public string MacAddress { get; set; }

        [Required]
        public TipoSegnalazione Tipologia { get; set; }
        #region METODI PUBBLICI
        public String UploadFile(System.Web.HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0 && Utils.CheckFormatoFile(file, TipoMedia.TESTO))
            {
                string estensione = new System.IO.FileInfo(System.IO.Path.GetFileName(file.FileName)).Extension;
                string nomeFileUnivoco = System.Guid.NewGuid().ToString() + estensione;

                string path = HostingEnvironment.MapPath("~/Uploads/Segnalazioni/");

                System.IO.Directory.CreateDirectory(path);

                file.SaveAs(System.IO.Path.Combine(path, nomeFileUnivoco));
                return nomeFileUnivoco;
            }
            return null;
        }
        #endregion
    }

    /// <summary>
    /// Form usata per l'inserimento dati e l'invio della mail
    /// Riferimento: https://github.com/tanveery/recaptcha-net
    /// </summary>
    public class ContattiViewModel
    {
        #region ATTRIBUTI
        private int _Id;
        #endregion

        #region PROPRIETA
        [Required]
        public string Nominativo { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Oggetto { get; set; }
        [Required]
        public string Testo { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Attachment", ResourceType = typeof(App_GlobalResources.Language))]
        public System.Web.HttpPostedFileBase Allegato { get; set; }
        public string Controller { get; set; }
        public string Vista { get; set; }
        public string IP { get; set; }
        public string MacAddress { get; set; }
        [Required]
        public TipoSegnalazione Tipo { get; set; }
        #endregion

        #region COSTRUTTORI
        public ContattiViewModel() { }
        public ContattiViewModel(TipoSegnalazione tipo = TipoSegnalazione.Info, string action = "Contatti", string controller = "Home") 
        {
            Tipo = tipo;
            Vista = action;
            Controller = controller;
        }
        #endregion

        #region METODI PUBBLICI
        public bool SalvaSuDB(DatabaseContext db, PersonaModel utente = null)
        {
            // salvare su database
            PERSONA_SEGNALAZIONE model = new PERSONA_SEGNALAZIONE();
            if (utente != null)
                model.ID_PERSONA = utente.Persona.ID;
            model.IP = IP;
            model.EMAIL_RISPOSTA = Email;
            model.OGGETTO = Tipo.ToString() + ": " + Oggetto;
            model.TESTO = Testo;
            if (Allegato != null)
                model.ALLEGATO = UploadFile(Allegato);
            model.CONTROLLER = Controller;
            model.VISTA = Vista;
            model.DATA_INVIO = DateTime.Now;
            model.STATO = (int)Stato.ATTIVO;
            db.PERSONA_SEGNALAZIONE.Add(model);
            bool risultato = db.SaveChanges() > 0;
            _Id = model.ID;
            return risultato;
        }
        public void InviaEmail()
        {
            using (var mail = new MailMessage())
            {
                mail.To.Add(new MailAddress(System.Configuration.ConfigurationManager.AppSettings["emailContatti" + (int)Tipo]));
                mail.From = new MailAddress(Email);
                mail.Subject = Tipo.ToString() + " - ticket " + _Id + ": " + Oggetto;
                mail.Body = Testo;
                mail.IsBodyHtml = false;

                try
                {
                    using (var smtpClient = new SmtpClient())
                    {
                        smtpClient.Send(mail);
                    }

                }
                finally
                {
                    //dispose the client
                    mail.Dispose();
                }
            }
        }
        #endregion

        #region METODI PRIVATI

        public bool CheckCaptcha(RecaptchaVerificationHelper recaptchaHelper, ref string errore)
        {
            if (String.IsNullOrEmpty(recaptchaHelper.Response))
            {
                errore = App_GlobalResources.ErrorResource.ContactsCaptchaEmpty;
                return false;
            }
            RecaptchaVerificationResult recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();
            if (recaptchaResult != RecaptchaVerificationResult.Success)
            {
                errore = App_GlobalResources.ErrorResource.ContactsCaptchaError;
                return false;
            }
            return true;
        }

        private String UploadFile(System.Web.HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0 && Utils.CheckFormatoFile(file, TipoMedia.TESTO))
            {
                string estensione = new System.IO.FileInfo(System.IO.Path.GetFileName(file.FileName)).Extension;
                string nomeFileUnivoco = System.Guid.NewGuid().ToString() + estensione;

                string path = HostingEnvironment.MapPath("~/Uploads/Segnalazioni/");

                System.IO.Directory.CreateDirectory(path);

                file.SaveAs(System.IO.Path.Combine(path, nomeFileUnivoco));
                
                return nomeFileUnivoco;
            }
            return null;
        }
        #endregion
    }
}
