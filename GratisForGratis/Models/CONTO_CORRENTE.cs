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
    
    public partial class CONTO_CORRENTE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CONTO_CORRENTE()
        {
            this.ATTIVITA = new HashSet<ATTIVITA>();
            this.CONTO_CORRENTE_MONETA = new HashSet<CONTO_CORRENTE_MONETA>();
            this.CONTO_CORRENTE_CREDITO = new HashSet<CONTO_CORRENTE_CREDITO>();
            this.TRANSAZIONE = new HashSet<TRANSAZIONE>();
            this.TRANSAZIONE1 = new HashSet<TRANSAZIONE>();
            this.PERSONA = new HashSet<PERSONA>();
        }
    
        public System.Guid ID { get; set; }
        public System.Guid TOKEN { get; set; }
        public decimal PUNTI { get; set; }
        public decimal PUNTI_SOSPESI { get; set; }
        public System.DateTime DATA_INSERIMENTO { get; set; }
        public Nullable<System.DateTime> DATA_MODIFICA { get; set; }
        public int STATO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTIVITA> ATTIVITA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTO_CORRENTE_MONETA> CONTO_CORRENTE_MONETA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTO_CORRENTE_CREDITO> CONTO_CORRENTE_CREDITO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRANSAZIONE> TRANSAZIONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRANSAZIONE> TRANSAZIONE1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERSONA> PERSONA { get; set; }
    }
}
