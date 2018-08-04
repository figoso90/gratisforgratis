using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GratisForGratis.Models.ViewModels.Email
{
    public class BonusRicevutoViewModel
    {
        #region PROPRIETA
        public string NominativoDestinatario { get; set; }

        public string Nome { get; set; }

        public int? Bonus { get; set; }
        #endregion
    }
}