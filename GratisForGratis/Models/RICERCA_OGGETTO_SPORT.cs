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
    
    public partial class RICERCA_OGGETTO_SPORT
    {
        public int ID { get; set; }
        public int ID_RICERCA_OGGETTO { get; set; }
        public Nullable<int> ID_MODELLO { get; set; }
    
        public virtual MODELLO MODELLO { get; set; }
        public virtual RICERCA_OGGETTO RICERCA_OGGETTO { get; set; }
    }
}
