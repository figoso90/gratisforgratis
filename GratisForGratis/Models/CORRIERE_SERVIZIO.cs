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
    
    public partial class CORRIERE_SERVIZIO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CORRIERE_SERVIZIO()
        {
            this.CORRIERE_SERVIZIO_PREZZO_STIMATO = new HashSet<CORRIERE_SERVIZIO_PREZZO_STIMATO>();
            this.CORRIERE_SERVIZIO_SPEDIZIONE = new HashSet<CORRIERE_SERVIZIO_SPEDIZIONE>();
        }
    
        public int ID { get; set; }
        public int ID_CORRIERE { get; set; }
        public string CODICE { get; set; }
        public string NOME { get; set; }
        public string DESCRIZIONE { get; set; }
        public System.DateTime DATA_INSERIMENTO { get; set; }
        public Nullable<System.DateTime> DATA_MODIFICA { get; set; }
        public int STATO { get; set; }
    
        public virtual CORRIERE CORRIERE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CORRIERE_SERVIZIO_PREZZO_STIMATO> CORRIERE_SERVIZIO_PREZZO_STIMATO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CORRIERE_SERVIZIO_SPEDIZIONE> CORRIERE_SERVIZIO_SPEDIZIONE { get; set; }
    }
}
