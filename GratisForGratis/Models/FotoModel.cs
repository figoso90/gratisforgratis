using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GratisForGratis.Models
{
    public class FotoModel : ALLEGATO
    {
        #region COSTRUTTORI
        public FotoModel() { }

        public FotoModel(ALLEGATO model)
        {
            this.ID = model.ID;
            this.NOME = model.NOME;
            this.ESTENSIONE = model.ESTENSIONE;
            this.DATA_INSERIMENTO = model.DATA_INSERIMENTO;
            this.DATA_MODIFICA = model.DATA_MODIFICA;
            this.STATO = model.STATO;
            this.TESTO = model.TESTO;
        }
        #endregion

        #region METODI PUBBLICI
        public int Add(DatabaseContext db, string nome)
        {
            ALLEGATO foto = new ALLEGATO();
            foto.NOME = nome;
            foto.DATA_INSERIMENTO = DateTime.Now;
            foto.ESTENSIONE = (int)EstensioneFile.Jpg;
            foto.STATO = (int)Stato.ATTIVO;
            db.ALLEGATO.Add(foto);
            db.SaveChanges();
            return foto.ID;
        }
        #endregion
    }
}
