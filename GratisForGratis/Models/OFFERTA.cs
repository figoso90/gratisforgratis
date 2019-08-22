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
    
    public partial class OFFERTA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OFFERTA()
        {
            this.CONTO_CORRENTE_CREDITO = new HashSet<CONTO_CORRENTE_CREDITO>();
            this.OFFERTA_BARATTO = new HashSet<OFFERTA_BARATTO>();
            this.OFFERTA_SPEDIZIONE = new HashSet<OFFERTA_SPEDIZIONE>();
        }
    
        public int ID { get; set; }
        public int ID_ANNUNCIO { get; set; }
        public Nullable<int> ID_PERSONA { get; set; }
        public Nullable<int> ID_ATTIVITA { get; set; }
        public int TIPO_OFFERTA { get; set; }
        public int TIPO_TRATTATIVA { get; set; }
        public Nullable<decimal> SOLDI { get; set; }
        public Nullable<decimal> PUNTI { get; set; }
        public string NOTE { get; set; }
        public Nullable<int> ID_TRANSAZIONE { get; set; }
        public string SESSIONE_COMPRATORE { get; set; }
        public System.DateTime DATA_INSERIMENTO { get; set; }
        public Nullable<System.DateTime> DATA_MODIFICA { get; set; }
        public int LETTA { get; set; }
        public int STATO { get; set; }
    
        public virtual ANNUNCIO ANNUNCIO { get; set; }
        public virtual ATTIVITA ATTIVITA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTO_CORRENTE_CREDITO> CONTO_CORRENTE_CREDITO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OFFERTA_BARATTO> OFFERTA_BARATTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OFFERTA_SPEDIZIONE> OFFERTA_SPEDIZIONE { get; set; }
        public virtual TRANSAZIONE TRANSAZIONE { get; set; }
        public virtual PERSONA PERSONA { get; set; }
    }
}
