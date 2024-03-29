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
    
    public partial class PAYPAL
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PAYPAL()
        {
            this.PAYPAL_DETTAGLIO = new HashSet<PAYPAL_DETTAGLIO>();
            this.TRANSAZIONE = new HashSet<TRANSAZIONE>();
        }
    
        public int ID { get; set; }
        public string KEY_PAYPAL { get; set; }
        public string NUMERO_FATTURA { get; set; }
        public string NOME { get; set; }
        public decimal IMPORTO { get; set; }
        public int ID_TIPO_VALUTA { get; set; }
        public System.DateTime DATA_INSERIMENTO { get; set; }
        public Nullable<System.DateTime> DATA_MODIFICA { get; set; }
        public int STATO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PAYPAL_DETTAGLIO> PAYPAL_DETTAGLIO { get; set; }
        public virtual TIPO_VALUTA TIPO_VALUTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRANSAZIONE> TRANSAZIONE { get; set; }
    }
}
