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
    
    public partial class INDIRIZZO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public INDIRIZZO()
        {
            this.ANNUNCIO_SPEDIZIONE = new HashSet<ANNUNCIO_SPEDIZIONE>();
            this.ANNUNCIO_SPEDIZIONE1 = new HashSet<ANNUNCIO_SPEDIZIONE>();
            this.ATTIVITA_INDIRIZZO = new HashSet<ATTIVITA_INDIRIZZO>();
            this.PERSONA_INDIRIZZO = new HashSet<PERSONA_INDIRIZZO>();
        }
    
        public int ID { get; set; }
        public int ID_COMUNE { get; set; }
        public string INDIRIZZO1 { get; set; }
        public int CIVICO { get; set; }
        public string SEZIONE { get; set; }
        public int TIPOLOGIA { get; set; }
        public System.DateTime DATA_INSERIMENTO { get; set; }
        public Nullable<System.DateTime> DATA_MODIFICA { get; set; }
        public int STATO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANNUNCIO_SPEDIZIONE> ANNUNCIO_SPEDIZIONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANNUNCIO_SPEDIZIONE> ANNUNCIO_SPEDIZIONE1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTIVITA_INDIRIZZO> ATTIVITA_INDIRIZZO { get; set; }
        public virtual COMUNE COMUNE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERSONA_INDIRIZZO> PERSONA_INDIRIZZO { get; set; }
    }
}