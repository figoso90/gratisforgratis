//------------------------------------------------------------------------------
// <auto-generated>
//     Codice generato da un modello.
//
//     Le modifiche manuali a questo file potrebbero causare un comportamento imprevisto dell'applicazione.
//     Se il codice viene rigenerato, le modifiche manuali al file verranno sovrascritte.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GratisForGratis.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CONTO_CORRENTE_CREDITO
    {
        public int ID { get; set; }
        public System.Guid ID_CONTO_CORRENTE { get; set; }
        public int ID_TRANSAZIONE_ENTRATA { get; set; }
        public Nullable<int> ID_TRANSAZIONE_USCITA { get; set; }
        public Nullable<int> ID_OFFERTA_USCITA { get; set; }
        public decimal PUNTI { get; set; }
        public decimal SOLDI { get; set; }
        public System.DateTime DATA_SCADENZA { get; set; }
        public int GIORNI_SCADENZA { get; set; }
        public System.DateTime DATA_INSERIMENTO { get; set; }
        public Nullable<System.DateTime> DATA_MODIFICA { get; set; }
        public int STATO { get; set; }
    
        public virtual CONTO_CORRENTE CONTO_CORRENTE { get; set; }
        public virtual OFFERTA OFFERTA { get; set; }
        public virtual TRANSAZIONE TRANSAZIONE { get; set; }
        public virtual TRANSAZIONE TRANSAZIONE1 { get; set; }
    }
}
