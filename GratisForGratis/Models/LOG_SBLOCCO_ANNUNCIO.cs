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
    
    public partial class LOG_SBLOCCO_ANNUNCIO
    {
        public int ID { get; set; }
        public int ID_ANNUNCIO { get; set; }
        public int ID_UTENTE_SBLOCCO { get; set; }
        public int TIPO { get; set; }
        public System.DateTime DATA_AVVIO { get; set; }
        public Nullable<System.DateTime> DATA_FINE { get; set; }
        public int NUMERO_SBLOCCO { get; set; }
    
        public virtual ANNUNCIO ANNUNCIO { get; set; }
        public virtual PERSONA PERSONA { get; set; }
    }
}
