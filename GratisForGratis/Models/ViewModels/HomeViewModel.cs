using Recaptcha.Web;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;

namespace GratisForGratis.Models
{
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
    }

    /// <summary>
    /// Form usata per l'inserimento dati e l'invio della mail
    /// Riferimento: https://github.com/tanveery/recaptcha-net
    /// </summary>
    public class ContattiViewModel
    {
        #region PROPRIETA
        public string Nominativo { get; set; }
        public string Email { get; set; }
        public string Oggetto { get; set; }
        public string Testo { get; set; }
        #endregion

        #region METODI PUBBLICI
        public void InviaEmail(ref string errore)
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    mail.To.Add(new MailAddress(System.Configuration.ConfigurationManager.AppSettings["emailContatti"]));
                    mail.From = new MailAddress(Email);
                    mail.Subject = Oggetto;
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
            catch (SmtpFailedRecipientsException ex)
            {
                foreach (SmtpFailedRecipientException t in ex.InnerExceptions)
                {
                    var status = t.StatusCode;
                    if (status == SmtpStatusCode.MailboxBusy ||
                        status == SmtpStatusCode.MailboxUnavailable)
                    {
                        errore = "Invio fallito - riprova tra 5 secondi";
                        System.Threading.Thread.Sleep(5000);
                        //resend
                        //smtpClient.Send(message);
                    }
                    else
                    {
                        errore = string.Format("Invio fallito per {0}", t.FailedRecipient);
                    }
                }
            }
            catch (SmtpException Se)
            {
                // handle exception here
                errore = Se.ToString();
            }

            catch (Exception ex)
            {
                errore = ex.ToString();
            }
        }
        #endregion
    }
}
