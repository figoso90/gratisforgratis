﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GratisForGratis.Models.File
{
    public class Mp4:FileMedia
    {
        #region FIELDS

        #endregion FIELDS

        #region PROPRIETà

        #endregion PROPRIETà

        #region METODI

        public Mp4()
            : base(new String[] { "000000" }, TipoMedia.VIDEO, 3)
        {

        }

        public override bool checkFormato(String esadecimaleFile)
        {
            if (esadecimaleFile.StartsWith(idEsadecimale[0]))
            {
                return true;
            }
            return false;
        }

        #endregion METODI
    }
}