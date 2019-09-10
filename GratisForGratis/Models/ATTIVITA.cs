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
    
    public partial class ATTIVITA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ATTIVITA()
        {
            this.ANNUNCIO_CARRELLO = new HashSet<ANNUNCIO_CARRELLO>();
            this.ANNUNCIO_DESIDERATO = new HashSet<ANNUNCIO_DESIDERATO>();
            this.ATTIVITA_EMAIL = new HashSet<ATTIVITA_EMAIL>();
            this.ATTIVITA_FOTO = new HashSet<ATTIVITA_FOTO>();
            this.ATTIVITA_INDIRIZZO = new HashSet<ATTIVITA_INDIRIZZO>();
            this.ATTIVITA_TELEFONO = new HashSet<ATTIVITA_TELEFONO>();
            this.NOTIFICA = new HashSet<NOTIFICA>();
            this.NOTIFICA1 = new HashSet<NOTIFICA>();
            this.OGGETTO_APPARTENENZA = new HashSet<OGGETTO_APPARTENENZA>();
            this.PERSONA_ATTIVITA = new HashSet<PERSONA_ATTIVITA>();
            this.OFFERTA = new HashSet<OFFERTA>();
            this.ANNUNCIO = new HashSet<ANNUNCIO>();
            this.CHAT = new HashSet<CHAT>();
            this.CHAT1 = new HashSet<CHAT>();
        }
    
        public int ID { get; set; }
        public System.Guid TOKEN { get; set; }
        public System.Guid ID_CONTO_CORRENTE { get; set; }
        public string PASSWORD { get; set; }
        public string TOKEN_PASSWORD { get; set; }
        public string NOME { get; set; }
        public string DOMINIO { get; set; }
        public System.DateTime DATA_INSERIMENTO { get; set; }
        public Nullable<System.DateTime> DATA_MODIFICA { get; set; }
        public int STATO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANNUNCIO_CARRELLO> ANNUNCIO_CARRELLO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANNUNCIO_DESIDERATO> ANNUNCIO_DESIDERATO { get; set; }
        public virtual CONTO_CORRENTE CONTO_CORRENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTIVITA_EMAIL> ATTIVITA_EMAIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTIVITA_FOTO> ATTIVITA_FOTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTIVITA_INDIRIZZO> ATTIVITA_INDIRIZZO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTIVITA_TELEFONO> ATTIVITA_TELEFONO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTIFICA> NOTIFICA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTIFICA> NOTIFICA1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OGGETTO_APPARTENENZA> OGGETTO_APPARTENENZA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERSONA_ATTIVITA> PERSONA_ATTIVITA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OFFERTA> OFFERTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANNUNCIO> ANNUNCIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CHAT> CHAT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CHAT> CHAT1 { get; set; }
    }
}
