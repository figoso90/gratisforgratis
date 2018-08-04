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
    
    public partial class PERSONA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PERSONA()
        {
            this.ANNUNCIO = new HashSet<ANNUNCIO>();
            this.ANNUNCIO_CARRELLO = new HashSet<ANNUNCIO_CARRELLO>();
            this.ANNUNCIO_CLICK = new HashSet<ANNUNCIO_CLICK>();
            this.ANNUNCIO_DESIDERATO = new HashSet<ANNUNCIO_DESIDERATO>();
            this.ANNUNCIO_FEEDBACK = new HashSet<ANNUNCIO_FEEDBACK>();
            this.ANNUNCIO_VISUALIZZAZIONE = new HashSet<ANNUNCIO_VISUALIZZAZIONE>();
            this.NOTIFICA = new HashSet<NOTIFICA>();
            this.NOTIFICA1 = new HashSet<NOTIFICA>();
            this.OFFERTA = new HashSet<OFFERTA>();
            this.OGGETTO_APPARTENENZA = new HashSet<OGGETTO_APPARTENENZA>();
            this.PERSONA_ATTIVITA = new HashSet<PERSONA_ATTIVITA>();
            this.PERSONA_EMAIL = new HashSet<PERSONA_EMAIL>();
            this.PERSONA_FOTO = new HashSet<PERSONA_FOTO>();
            this.PERSONA_INDIRIZZO = new HashSet<PERSONA_INDIRIZZO>();
            this.PERSONA_PRIVACY = new HashSet<PERSONA_PRIVACY>();
            this.PERSONA_RICERCA = new HashSet<PERSONA_RICERCA>();
            this.PERSONA_SEGNALAZIONE = new HashSet<PERSONA_SEGNALAZIONE>();
            this.PERSONA_TELEFONO = new HashSet<PERSONA_TELEFONO>();
            this.PERSONA_METODO_PAGAMENTO = new HashSet<PERSONA_METODO_PAGAMENTO>();
            this.LOG_SBLOCCO_ANNUNCIO = new HashSet<LOG_SBLOCCO_ANNUNCIO>();
            this.ANNUNCIO1 = new HashSet<ANNUNCIO>();
            this.CHAT = new HashSet<CHAT>();
            this.CHAT1 = new HashSet<CHAT>();
        }
    
        public int ID { get; set; }
        public System.Guid TOKEN { get; set; }
        public System.Guid ID_CONTO_CORRENTE { get; set; }
        public string PASSWORD { get; set; }
        public string TOKEN_PASSWORD { get; set; }
        public string FACEBOOK_TOKEN_PERMANENTE { get; set; }
        public string FACEBOOK_TOKEN_SESSIONE { get; set; }
        public string NOME { get; set; }
        public string COGNOME { get; set; }
        public int ID_ABBONAMENTO { get; set; }
        public System.DateTime DATA_INSERIMENTO { get; set; }
        public Nullable<System.DateTime> DATA_ACCESSO { get; set; }
        public Nullable<System.DateTime> DATA_MODIFICA { get; set; }
        public int STATO { get; set; }
    
        public virtual ABBONAMENTO ABBONAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANNUNCIO> ANNUNCIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANNUNCIO_CARRELLO> ANNUNCIO_CARRELLO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANNUNCIO_CLICK> ANNUNCIO_CLICK { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANNUNCIO_DESIDERATO> ANNUNCIO_DESIDERATO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANNUNCIO_FEEDBACK> ANNUNCIO_FEEDBACK { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANNUNCIO_VISUALIZZAZIONE> ANNUNCIO_VISUALIZZAZIONE { get; set; }
        public virtual CONTO_CORRENTE CONTO_CORRENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTIFICA> NOTIFICA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTIFICA> NOTIFICA1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OFFERTA> OFFERTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OGGETTO_APPARTENENZA> OGGETTO_APPARTENENZA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERSONA_ATTIVITA> PERSONA_ATTIVITA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERSONA_EMAIL> PERSONA_EMAIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERSONA_FOTO> PERSONA_FOTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERSONA_INDIRIZZO> PERSONA_INDIRIZZO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERSONA_PRIVACY> PERSONA_PRIVACY { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERSONA_RICERCA> PERSONA_RICERCA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERSONA_SEGNALAZIONE> PERSONA_SEGNALAZIONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERSONA_TELEFONO> PERSONA_TELEFONO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERSONA_METODO_PAGAMENTO> PERSONA_METODO_PAGAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOG_SBLOCCO_ANNUNCIO> LOG_SBLOCCO_ANNUNCIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANNUNCIO> ANNUNCIO1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CHAT> CHAT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CHAT> CHAT1 { get; set; }
    }
}
