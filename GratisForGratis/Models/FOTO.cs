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
    
    public partial class FOTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FOTO()
        {
            this.ANNUNCIO_FOTO = new HashSet<ANNUNCIO_FOTO>();
            this.ATTIVITA_FOTO = new HashSet<ATTIVITA_FOTO>();
            this.PERSONA_FOTO = new HashSet<PERSONA_FOTO>();
        }
    
        public int ID { get; set; }
        public string FOTO1 { get; set; }
        public string TESTO { get; set; }
        public System.DateTime DATA_INSERIMENTO { get; set; }
        public Nullable<System.DateTime> DATA_MODIFICA { get; set; }
        public int STATO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANNUNCIO_FOTO> ANNUNCIO_FOTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTIVITA_FOTO> ATTIVITA_FOTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERSONA_FOTO> PERSONA_FOTO { get; set; }
    }
}
