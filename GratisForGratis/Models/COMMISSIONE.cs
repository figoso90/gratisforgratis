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
    
    public partial class COMMISSIONE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public COMMISSIONE()
        {
            this.ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE = new HashSet<ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE>();
            this.OFFERTA_SPEDIZIONE = new HashSet<OFFERTA_SPEDIZIONE>();
            this.ANNUNCIO = new HashSet<ANNUNCIO>();
        }
    
        public int ID { get; set; }
        public decimal PERCENTUALE { get; set; }
        public int TIPO { get; set; }
        public System.DateTime DATA_INSERIMENTO { get; set; }
        public Nullable<System.DateTime> DATA_MODIFICA { get; set; }
        public int STATO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE> ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OFFERTA_SPEDIZIONE> OFFERTA_SPEDIZIONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANNUNCIO> ANNUNCIO { get; set; }
    }
}
