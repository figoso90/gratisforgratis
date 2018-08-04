using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GratisForGratis.Models
{
    public class MetodoPagamento
    {
        #region PROPRIETA
        public string TipoCarta { get; set; }
        public string Numero { get; set; }
        public string Cvv2 { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public int MeseScadenza { get; set; }
        public int AnnoScadenza { get; set; }
        #endregion
    }
}