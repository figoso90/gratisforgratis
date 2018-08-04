using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GratisForGratis.Models
{
    public class PdfModel : ALLEGATO
    {
        #region METODI PUBBLICI
        public int Add(DatabaseContext db, string nome)
        {
            ALLEGATO allegato = new ALLEGATO();
            allegato.NOME = nome;
            allegato.DATA_INSERIMENTO = DateTime.Now;
            allegato.ESTENSIONE = (int)EstensioneFile.Pdf;
            allegato.STATO = (int)Stato.ATTIVO;
            db.ALLEGATO.Add(allegato);
            db.SaveChanges();
            return allegato.ID;
        }
        #endregion
    }
}
