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
    
    public partial class ANNUNCIO_CLICK
    {
        public int ID { get; set; }
        public int ID_ANNUNCIO { get; set; }
        public Nullable<int> ID_PERSONA { get; set; }
        public string IP { get; set; }
        public string MAC_ADDRESS { get; set; }
        public int ID_SISTEMA_OPERATIVO { get; set; }
        public int ID_MODELLO_BROWSER { get; set; }
        public System.DateTime DATA_INSERIMENTO { get; set; }
        public Nullable<System.DateTime> DATA_MODIFICA { get; set; }
        public int STATO { get; set; }
    
        public virtual ANNUNCIO ANNUNCIO { get; set; }
        public virtual BROWSER_MODELLO BROWSER_MODELLO { get; set; }
        public virtual SISTEMA_OPERATIVO SISTEMA_OPERATIVO { get; set; }
        public virtual PERSONA PERSONA { get; set; }
    }
}
