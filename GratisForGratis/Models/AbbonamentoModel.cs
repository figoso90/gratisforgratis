using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GratisForGratis.Models
{
    public class AbbonamentoModel
    {
        #region PROPRIETà
        public int Id { get; set; }

        public string Nome { get; set; }

        public decimal BonusPerUtente { get; set; }

        public int Durata { get; set; }

        public Stato Stato { get; set; }
        #endregion

        #region COSTRUTTORI
        public AbbonamentoModel() { }

        public AbbonamentoModel(ABBONAMENTO model)
        {
            this.Id = model.ID;
            this.Nome = model.NOME;
            this.BonusPerUtente = model.BONUS_PERUTENTE;
            this.Durata = model.DURATA;
            this.Stato = (Stato)model.STATO;
        }
        #endregion
    }
}