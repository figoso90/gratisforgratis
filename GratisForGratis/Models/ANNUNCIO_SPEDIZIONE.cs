//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GratisForGratis.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ANNUNCIO_SPEDIZIONE
    {
        public int ID { get; set; }
        public int ID_ANNUNCIO { get; set; }
        public Nullable<int> ID_CORRIERE_NAZIONE { get; set; }
        public int ID_INDIRIZZO_MITTENTE { get; set; }
        public int ID_INDIRIZZO_DESTINATARIO { get; set; }
        public string NOTE { get; set; }
        public decimal ALTEZZA { get; set; }
        public decimal LARGHEZZA { get; set; }
        public decimal LUNGHEZZA { get; set; }
        public decimal PESO { get; set; }
        public decimal PREZZO { get; set; }
        public System.DateTime DATA_INSERIMENTO { get; set; }
        public Nullable<System.DateTime> DATA_MODIFICA { get; set; }
        public int STATO { get; set; }
    
        public virtual CORRIERE_NAZIONE CORRIERE_NAZIONE { get; set; }
        public virtual INDIRIZZO INDIRIZZO { get; set; }
        public virtual INDIRIZZO INDIRIZZO1 { get; set; }
        public virtual ANNUNCIO ANNUNCIO { get; set; }
    }
}