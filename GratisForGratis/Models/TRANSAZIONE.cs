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
    
    public partial class TRANSAZIONE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TRANSAZIONE()
        {
            this.CONTO_CORRENTE_MONETA = new HashSet<CONTO_CORRENTE_MONETA>();
            this.OFFERTA = new HashSet<OFFERTA>();
            this.TRANSAZIONE_ANNUNCIO_SPEDIZIONE = new HashSet<TRANSAZIONE_ANNUNCIO_SPEDIZIONE>();
            this.TRANSAZIONE_ANNUNCIO = new HashSet<TRANSAZIONE_ANNUNCIO>();
        }
    
        public int ID { get; set; }
        public int TEST { get; set; }
        public string NOME { get; set; }
        public int TIPO { get; set; }
        public System.Guid ID_CONTO_MITTENTE { get; set; }
        public System.Guid ID_CONTO_DESTINATARIO { get; set; }
        public Nullable<int> SOLDI { get; set; }
        public Nullable<int> PUNTI { get; set; }
        public Nullable<System.DateTime> DATA_INSERIMENTO { get; set; }
        public Nullable<System.DateTime> DATA_MODIFICA { get; set; }
        public int STATO { get; set; }
        public string EXTERNAL_ID { get; set; }
    
        public virtual CONTO_CORRENTE CONTO_CORRENTE { get; set; }
        public virtual CONTO_CORRENTE CONTO_CORRENTE1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTO_CORRENTE_MONETA> CONTO_CORRENTE_MONETA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OFFERTA> OFFERTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRANSAZIONE_ANNUNCIO_SPEDIZIONE> TRANSAZIONE_ANNUNCIO_SPEDIZIONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRANSAZIONE_ANNUNCIO> TRANSAZIONE_ANNUNCIO { get; set; }
    }
}
