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
    
    public partial class ANNUNCIO_CARRELLO
    {
        public int ID { get; set; }
        public int ID_ANNUNCIO { get; set; }
        public int ID_UTENTE { get; set; }
        public Nullable<int> ID_ATTIVITA { get; set; }
        public System.DateTime DATA_INSERIMENTO { get; set; }
        public Nullable<System.DateTime> DATA_MODIFICA { get; set; }
        public int STATO { get; set; }
    
        public virtual ATTIVITA ATTIVITA { get; set; }
        public virtual PERSONA PERSONA { get; set; }
        public virtual ANNUNCIO ANNUNCIO { get; set; }
    }
}
