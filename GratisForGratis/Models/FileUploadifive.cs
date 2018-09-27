using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GratisForGratis.Models
{
    public class FileUploadifive
    {
        #region PROPRIETA
        public string Id { get; set; }

        public string PathOriginale { get; set; }

        public string PathMedia { get; set; }

        public string PathPiccola { get; set; }

        public string Nome { get; set; }

        public string NomeOriginale { get; set; }

        public string TipoFileEffettivo { get; set; }
        #endregion
    }
}