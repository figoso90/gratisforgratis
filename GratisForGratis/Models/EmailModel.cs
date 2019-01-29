using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GratisForGratis.Models
{
    /* 
    Default:
        Mittente => Sito Web
        Allegati => Vuoti
        Priorità di invio => Normale
        Tipo di invio => Asincrono
    */
    public class EmailModel
    {
        //private string body;

        public EmailModel(System.Web.Mvc.ControllerContext ControllerContext)
        {
            To = new List<MailAddress>();
            //From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["fromEmail"], WebConfigurationManager.AppSettings["nomeSito"]);
            Layout = "~/Views/Shared/_PaginaEmail.cshtml";
            Attachments = new List<System.Web.HttpPostedFileBase>();
            Priority = MailPriority.Normal;
            SendAsync = false;
            this.ControllerContext = ControllerContext;
        }

        public System.Web.Mvc.ControllerContext ControllerContext { get; private set; }

        [Required]
        public MailAddress From { get; set; }

        [Required]
        public List<MailAddress> To { get; set; }

        public List<MailAddress> Cc { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Layout { get; set; }

        [Required]
        public string Body { get; set; }

        public object DatiEmail { get; set; }

        public List<System.Web.HttpPostedFileBase> Attachments { get; set; }

        [Required]
        public MailPriority Priority { get; set; }

        [Required]
        public bool SendAsync { get; set; }
    }
}
