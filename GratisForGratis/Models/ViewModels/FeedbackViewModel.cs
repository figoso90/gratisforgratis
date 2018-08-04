using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GratisForGratis.Models.ViewModels
{
    public class FeedbackViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        //public string AcquistoID { get; set; }
        public int AcquistoID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "PaymentName", ResourceType = typeof(App_GlobalResources.Language))]
        public string Nome { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Range(0, 10, ErrorMessageResourceName = "ErrorRangeVote", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "FeedbackVote", ResourceType = typeof(App_GlobalResources.Language))]
        public int Voto { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "FeedbackComment", ResourceType = typeof(App_GlobalResources.Language))]
        [MaxLength(8000, ErrorMessageResourceName = "ErrorRangeOpinion", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public string Opinione { get; set; }

        public TipoFeedback Tipo { get; set; }

        public string Ricevente { get; set; }

        public DateTime DataInvio { get; set; }

        public int PuntiBonus { get; set; }
    }
}
