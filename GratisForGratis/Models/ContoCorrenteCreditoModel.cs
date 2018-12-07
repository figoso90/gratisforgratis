using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GratisForGratis.Models
{
    public class ContoCorrenteCreditoModel : CONTO_CORRENTE_CREDITO
    {
        #region ATTRIBUTI
        private DatabaseContext _db;
        #endregion

        #region COSTRUTTORI
        public ContoCorrenteCreditoModel(DatabaseContext db)
        {
            _db = db;
        }
        #endregion

        #region METODI PUBBLICI
        public void Add()
        {

        }

        public void Remove()
        {

        }
        #endregion
    }
}