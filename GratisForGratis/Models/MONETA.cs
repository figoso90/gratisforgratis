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
    
    public partial class MONETA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MONETA()
        {
            this.CONTO_CORRENTE_MONETA = new HashSet<CONTO_CORRENTE_MONETA>();
        }
    
        public int ID { get; set; }
        public System.Guid TOKEN { get; set; }
        public double VALORE { get; set; }
        public System.DateTime DATA_INSERIMENTO { get; set; }
        public Nullable<System.DateTime> DATA_MODIFICA { get; set; }
        public int STATO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTO_CORRENTE_MONETA> CONTO_CORRENTE_MONETA { get; set; }
        public virtual MONETA MONETA1 { get; set; }
        public virtual MONETA MONETA2 { get; set; }
    }
}
