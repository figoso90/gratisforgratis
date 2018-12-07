using GratisForGratis.Models.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GratisForGratis.Models
{
    [Serializable]
    public class TransazioneModel : TRANSAZIONE
    {
        #region ATTRIBUTI
        private DatabaseContext _db;
        #endregion

        #region COSTRUTTORI
        public TransazioneModel(DatabaseContext db)
        {
            _db = db;
        }
        #endregion

        #region METODI PUBBLICI
        public bool AddBonus(int rowCount = 0)
        {
            TRANSAZIONE transazione = this as TRANSAZIONE;
            _db.TRANSAZIONE.Add(transazione);
            return _db.SaveChanges() > rowCount;
        }

        public void SendOfferta()
        {

        }

        public void SendPagamento()
        {

        }
        #endregion
    }
}