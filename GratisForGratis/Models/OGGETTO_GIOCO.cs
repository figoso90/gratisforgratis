//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GratisForGratis.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class OGGETTO_GIOCO
    {
        public int ID { get; set; }
        public int ID_OGGETTO { get; set; }
        public int ID_MODELLO { get; set; }
    
        public virtual MODELLO MODELLO { get; set; }
        public virtual OGGETTO OGGETTO { get; set; }
    }
}
