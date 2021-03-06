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
    
    public partial class AUTORE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AUTORE()
        {
            this.OGGETTO_LIBRO = new HashSet<OGGETTO_LIBRO>();
            this.RICERCA_OGGETTO_AUTORE = new HashSet<RICERCA_OGGETTO_AUTORE>();
        }
    
        public int ID { get; set; }
        public string NOME { get; set; }
        public string DESCRIZIONE { get; set; }
        public int ID_CATEGORIA { get; set; }
        public System.DateTime DATA_INSERIMENTO { get; set; }
        public Nullable<System.DateTime> DATA_MODIFICA { get; set; }
        public int STATO { get; set; }
    
        public virtual CATEGORIA CATEGORIA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OGGETTO_LIBRO> OGGETTO_LIBRO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RICERCA_OGGETTO_AUTORE> RICERCA_OGGETTO_AUTORE { get; set; }
    }
}
