using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GratisForGratis.Models
{
    public class AnnuncioNotificaModel
    {
        #region PROPRIETA
        public AnnuncioViewModel Annuncio { get; set; }

        public NotificaModel Notifica { get; set; }
        #endregion
    }
}