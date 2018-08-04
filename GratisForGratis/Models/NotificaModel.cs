using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GratisForGratis.Models
{
    public class NotificaModel : NOTIFICA
    {
        #region PROPRIETA
        public int Id { get; set; }

        public PersonaModel Persona { get; set; }

        public AttivitaModel Attivita { get; set; }

        public string Messaggio { get; set; }

        public DateTime DataInserimento { get; set; }

        public DateTime? DataModifica { get; set; }

        public DateTime? DataLettura { get; set; }

        public StatoNotifica Stato { get; set; }
        #endregion
    }
}