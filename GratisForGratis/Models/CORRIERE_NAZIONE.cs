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
    
    public partial class CORRIERE_NAZIONE
    {
        public int ID { get; set; }
        public int ID_CORRIERE { get; set; }
        public int ID_NAZIONE { get; set; }
        public string ID_EXTERNAL { get; set; }
        public string ISO { get; set; }
        public string NOME { get; set; }
        public System.DateTime DATA_INSERIMENTO { get; set; }
        public Nullable<System.DateTime> DATA_MODIFICA { get; set; }
        public int STATO { get; set; }
    
        public virtual CORRIERE CORRIERE { get; set; }
        public virtual NAZIONE NAZIONE { get; set; }
    }
}
