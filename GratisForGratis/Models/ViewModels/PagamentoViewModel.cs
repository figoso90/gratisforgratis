using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GratisForGratis.Models
{

    public abstract class PagamentoAbstractModel
    {
        public PagamentoAbstractModel() { }

        // se crei l'oggetto partendo da un altro tipo di oggetto, 
        // questo costruttore copia direttamente gli attributi in comune
        public PagamentoAbstractModel(PagamentoAbstractModel model)
        {
            this.CopyAttributes(model);
        }

        [Required]
        [Url]
        [Display(Name = "Sito web")]
        public string WebSite { get; set; }

        public int Test { get; set; }

        [Url]
        [Display(Name = "Url richiesta")]
        public string UrlRequest { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Token")]
        public string Token { get; set; }

        [Required]
        public int TypePayment { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(200, ErrorMessage = "Email troppo lunga (Max 200 caratteri)")]
        [Display(Name = "Email destinatario")]
        public string EmailReceivent { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 5)]
        [Display(Name = "Descrizione pagamento")]
        public string DescriptionPayment { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Costo totale")]
        public decimal TotalPrice { get; set; }

        [Url]
        [Display(Name = "Url di ritorno")]
        public string ReturnUrlForSuccess { get; set; }

        [Url]
        [Display(Name = "Url di ritorno")]
        public string ReturnUrlForFailed { get; set; }

        // per copiare gli attributi comuni
        public void CopyAttributes(PagamentoAbstractModel model)
        {
            /*
            this.DescriptionPayment = model.DescriptionPayment;
            this.EmailReceivent = model.EmailReceivent;
            this.ReturnUrlForFailed = model.ReturnUrlForFailed;
            this.ReturnUrlForSuccess = model.ReturnUrlForSuccess;
            this.Token = model.Token;
            this.TotalPrice = model.TotalPrice;
            this.TypePayment = model.TypePayment;
            this.UrlRequest = model.UrlRequest;
            this.WebSite = model.WebSite;
            this.Test = model.Test;*/
            foreach (System.Reflection.PropertyInfo propertyInfo in model.GetType().GetProperties())
            {
                this.GetType().GetProperty(propertyInfo.Name).SetValue(this, propertyInfo.GetValue(model));
            }
        }
    }

    public class PagamentoViewModel : PagamentoAbstractModel
    {
        public PagamentoViewModel() : base() { }
        public PagamentoViewModel(PagamentoAbstractModel model) : base(model){ }
    }

    public class SalvataggioPagamentoViewModel : PagamentoAbstractModel
    {
        public SalvataggioPagamentoViewModel() : base() { }
        public SalvataggioPagamentoViewModel(PagamentoAbstractModel model) : base(model){   }

        [Required]
        public Guid PortaleWebID { get; set; }

        public Guid UtentePagatoID { get; set; }

        public string Nominativo { get; set; }

    }

    public class SchedaPagamentoViewModel /*: EncoderViewModel*/
    {

        public int Id { get; set; }

        public string Token { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Name", ResourceType = typeof(App_GlobalResources.Language))]
        public string Nome { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Name", ResourceType = typeof(App_GlobalResources.Language))]
        public string Compratore { get; set; }

        [DataType(DataType.Text)]
        public int CompratoreId { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Name", ResourceType = typeof(App_GlobalResources.Language))]
        public string Venditore { get; set; }

        [DataType(DataType.Text)]
        public int VenditoreId { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Name", ResourceType = typeof(App_GlobalResources.Language))]
        public string Portale { get; set; }

        [DataType(DataType.Text)]
        public int PortaleId { get; set; }

        [Range(int.MinValue, int.MaxValue)]
        public int Punti { get; set; }

        [Range(int.MinValue,int.MaxValue)]
        public int Soldi { get; set; }

        // offerti o ricevuti
        public List<AnnuncioViewModel> Baratti { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Date", ResourceType = typeof(App_GlobalResources.Language))]
        public DateTime? Data { get; set; }
    }

    public class BonificoViewModel
    {
        [Required]
        public TipoTransazione TipoTransazione { get; set; }

        [Required] // email o token conto corrente
        [DataType(DataType.Text)]
        [Display(Name = "Token / Email destinatario")]
        public string Destinatario { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 5)]
        [Display(Name = "Descrizione pagamento")]
        public string DescrizionePagamento { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Costo totale")]
        public decimal Punti { get; set; }

        [Url]
        [Display(Name = "Url di ritorno")]
        public string UrlOk { get; set; }

        [Url]
        [Display(Name = "Url di ritorno")]
        public string UrlKo { get; set; }
    }
}
