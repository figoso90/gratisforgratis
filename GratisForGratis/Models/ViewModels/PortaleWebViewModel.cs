using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GratisForGratis.Models
{
    public class PortaleWebViewModel
    {
        public PortaleWebViewModel() { }

        public PortaleWebViewModel(PERSONA_ATTIVITA model, List<ATTIVITA_EMAIL> modelEmail, List<ATTIVITA_TELEFONO> modelTelefono)
        {
            this.CopyModel(model, modelEmail, modelTelefono);
        }

        public void CopyModel(PERSONA_ATTIVITA model, List<ATTIVITA_EMAIL> modelEmail, List<ATTIVITA_TELEFONO> modelTelefono)
        {
            this.Id = model.ATTIVITA.ID.ToString();
            this.Ruolo = (RuoloProfilo)model.RUOLO;
            this.Email = modelEmail.Find(item => item.TIPO == (int)TipoEmail.Registrazione).EMAIL;
            this.Nome = model.ATTIVITA.NOME;
            this.Dominio = model.ATTIVITA.DOMINIO;
            this.Token = model.ATTIVITA.TOKEN.ToString();
            this.Telefono = modelTelefono.Find(item => item.TIPO == (int)TipoTelefono.Privato).TELEFONO;
            /*this.Abbonamento = model.ATTIVITA.ABBONAMENTO1.NOME;
            this.BonusPerUtente = model.ATTIVITA.ABBONAMENTO1.BONUS_PERUTENTE;
            this.DurataAbbonamento = model.ATTIVITA.ABBONAMENTO1.DURATA;*/
            // fare count punti sul conto corrente
            //this.Bonus = model.ATTIVITA.BONUS;
        }

        public void CopyModel(ATTIVITA model, List<ATTIVITA_EMAIL> modelEmail, List<ATTIVITA_TELEFONO> modelTelefono)
        {
            this.Email = modelEmail.Find(item => item.TIPO == (int)TipoEmail.Registrazione).EMAIL;
            this.Nome = model.NOME;
            this.Dominio = model.DOMINIO;
            this.Token = model.TOKEN.ToString();
            this.Telefono = modelTelefono.Find(item => item.TIPO == (int)TipoTelefono.Privato).TELEFONO;
            //this.Abbonamento = new AbbonamentoModel(model)
            this.Bonus = model.CONTO_CORRENTE.CONTO_CORRENTE_MONETA.Count;
            this.DataIscrizione = (DateTime)model.DATA_INSERIMENTO;
        }

        public string Id { get; private set; }

        [Required]
        [DataType(DataType.EmailAddress, ErrorMessageResourceName = "ErrorFormatEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [StringLength(200, ErrorMessageResourceName = "ErrorLengthEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Name", ResourceType = typeof(App_GlobalResources.Language))]
        public string Nome { get; set; }

        [Required]
        [DataType(DataType.Url, ErrorMessageResourceName = "ErrorFormat", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "Domain", ResourceType = typeof(App_GlobalResources.Language))]
        public string Dominio { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 16)]
        public string Token { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [StringLength(12, MinimumLength = 9, ErrorMessageResourceName = "ErrorPhoneNumber", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "Telephone", ResourceType = typeof(App_GlobalResources.Language))]
        public string Telefono { get; set; }

        [Required]
        [Display(Name = "Subscription", ResourceType = typeof(App_GlobalResources.Language))]
        public AbbonamentoModel Abbonamento { get; set; }

        //// annuale
        //[Display(Name = "BonusForUser", ResourceType = typeof(App_GlobalResources.Language))]
        //public int BonusPerUtente { get; set; }

        ////mesi
        //[Display(Name = "DurationSubscription", ResourceType = typeof(App_GlobalResources.Language))]
        //public int DurataAbbonamento { get; set; }

        [Required]
        [Display(Name = "AcceptConditions", ResourceType = typeof(App_GlobalResources.Language))]
        public bool AccettaCondizioni { get; set; }

        [Display(Name = "RoleUser", ResourceType = typeof(App_GlobalResources.Language))]
        public RuoloProfilo Ruolo { get; set; }

        [Display(Name = "BonusNow", ResourceType = typeof(App_GlobalResources.Language))]
        public decimal Bonus { get; set; }

        // annuale
        [Display(Name = "BonusUsed", ResourceType = typeof(App_GlobalResources.Language))]
        public decimal? BonusSpeso { get; set; }

        [Display(Name = "DateSubscription", ResourceType = typeof(App_GlobalResources.Language))]
        public DateTime DataIscrizione { get; set; }
    }

    public class PortaleWebProfiloViewModel : PortaleWebViewModel
    {
        public PortaleWebProfiloViewModel() { }

        public PortaleWebProfiloViewModel(PERSONA_ATTIVITA model, List<ATTIVITA_EMAIL> modelEmail, List<ATTIVITA_TELEFONO> modelTelefono) : base(model, modelEmail, modelTelefono) { }

        public PortaleWebProfiloViewModel(PortaleWebViewModel model)
        {

            foreach (System.Reflection.PropertyInfo propertyInfo in model.GetType().GetProperties())
            {
                if (this.GetType().GetProperty(propertyInfo.Name).SetMethod != null)
                    this.GetType().GetProperty(propertyInfo.Name).SetValue(this, propertyInfo.GetValue(model));
            }
        }
    }
}
