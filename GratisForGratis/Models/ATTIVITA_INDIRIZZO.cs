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
    
    public partial class ATTIVITA_INDIRIZZO
    {
        public int ID { get; set; }
        public int ID_ATTIVITA { get; set; }
        public int ID_INDIRIZZO { get; set; }
        public int TIPO { get; set; }
        public int ORDINE { get; set; }
        public System.DateTime DATA_INSERIMENTO { get; set; }
        public Nullable<System.DateTime> DATA_MODIFICA { get; set; }
        public int STATO { get; set; }
    
        public virtual ATTIVITA ATTIVITA { get; set; }
        public virtual INDIRIZZO INDIRIZZO { get; set; }
    }
}
