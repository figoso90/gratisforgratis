using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GratisForGratis.Models.ViewModels.Email
{
    public class PagamentoOffertaViewModel
    {
        #region PROPRIETA
        public string NominativoDestinatario { get; set; }

        public string NomeAnnuncio { get; set; }

        public decimal? Moneta { get; set; }

        public List<string> Baratti { get; set; }

        public decimal? SoldiSpedizione { get; set; }
        #endregion
    }
}