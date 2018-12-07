using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GratisForGratis.Models
{
    public class ContoCorrenteMonetaModel : CONTO_CORRENTE_MONETA
    {
        #region METODI PUBBLICI
        //public int AddCredito(DatabaseContext db, string nomeTransazione, TipoTransazione tipo, int punti)
        //{
        //    using (DbContextTransaction transazione = db.Database.BeginTransaction())
        //    {
        //        TRANSAZIONE model = new TRANSAZIONE();
        //        //model.ID_COMPRATORE = this.CONTO_CORRENTE.; >>> mettere conto corrente
        //        model.TIPO = (int)tipo;
        //        model.NOME = nomeTransazione;
        //        model.PUNTI = punti;
        //        model.DATA_INSERIMENTO = DateTime.Now;
        //        model.STATO = (int)StatoPagamento.ACCETTATO;
        //        db.TRANSAZIONE.Add(model);
        //        db.SaveChanges();
        //        CONTO_CORRENTE_MONETA moneta = new CONTO_CORRENTE_MONETA();
        //        moneta.ID_CONTO_CORRENTE = this.ID_CONTO_CORRENTE;
        //        for (int i=0;i<punti;i++)
        //        {
        //            try
        //            {
        //                moneta.ID_MONETA = db.MONETA.FirstOrDefault(m => m.CONTO_CORRENTE_MONETA.Count(item => item.ID_MONETA == m.ID) <= 0).ID;
        //                moneta.DATA_INSERIMENTO = DateTime.Now;
        //                moneta.STATO = (int)StatoMoneta.ATTIVA;
        //                db.CONTO_CORRENTE_MONETA.Add(moneta);
        //                db.SaveChanges();
        //            }
        //            catch (Exception eccezione)
        //            {
        //                i -= 1;
        //                Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
        //            }
        //        }
        //        transazione.Commit();
        //        return db.CONTO_CORRENTE_MONETA.Count(item => item.ID_CONTO_CORRENTE == this.ID_CONTO_CORRENTE);
        //    }
        //}

        //public TRANSAZIONE Pay(DatabaseContext db, Guid mittente, Guid destinatario, string nomeTransazione, TipoTransazione tipo, int punti)
        //{
        //    using (DbContextTransaction transazione = db.Database.BeginTransaction())
        //    {
        //        List<CONTO_CORRENTE_MONETA> list = db.CONTO_CORRENTE_MONETA.Where(m => m.ID_CONTO_CORRENTE == mittente && m.STATO == (int)StatoMoneta.ASSEGNATA).Take(punti).ToList();
        //        if (list.Count < punti)
        //            return null;

        //        TRANSAZIONE model = new TRANSAZIONE();
        //        model.ID_CONTO_MITTENTE = mittente;
        //        model.ID_CONTO_DESTINATARIO = destinatario;
        //        model.TIPO = (int)tipo;
        //        model.NOME = nomeTransazione;
        //        model.PUNTI = punti;
        //        model.DATA_INSERIMENTO = DateTime.Now;
        //        model.STATO = (int)StatoPagamento.ACCETTATO;
        //        db.TRANSAZIONE.Add(model);
        //        db.SaveChanges();

        //        foreach (CONTO_CORRENTE_MONETA moneta in list)
        //        {
        //            moneta.DATA_MODIFICA = DateTime.Now;
        //            moneta.STATO = (int)StatoMoneta.CEDUTA;
        //            db.SaveChanges();
        //            db.Entry(moneta).State = EntityState.Added;
        //            moneta.STATO = (int)StatoMoneta.ASSEGNATA;
        //            moneta.ID_CONTO_CORRENTE = destinatario;
        //            moneta.DATA_INSERIMENTO = DateTime.Now;
        //            moneta.DATA_MODIFICA = null;
        //            db.SaveChanges();

        //        }
        //        transazione.Commit();
        //        return model;
        //    }
        //}
        #endregion
    }
}