﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GratisForGratis.Models.File
{
    public class Ogm:FileMedia
    {
        #region FIELDS

        #endregion FIELDS

        #region PROPRIETà

        #endregion PROPRIETà

        #region METODI

        public Ogm()
            : base(new String[] { "4F676753" }, TipoMedia.VIDEO, 4)
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