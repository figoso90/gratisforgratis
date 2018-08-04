using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GratisForGratis.Models.ViewModels
{
    public class SpedizioneViewModel
    {
        #region PROPRIETA
        [Required]
        public int Id { get; set; }

        public string NomeAnnuncio { get; set; }

        public string Destinatario { get; set; }

        public decimal Prezzo { get; set; }

        public Stato Stato { get; set; }

        [Required]
        public HttpPostedFileBase LDV { get; set; }

        public HttpPostedFileBase Borderò { get; set; }
        #endregion
    }
}