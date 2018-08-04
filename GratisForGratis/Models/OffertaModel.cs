using GratisForGratis.App_GlobalResources;
using GratisForGratis.Models.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GratisForGratis.Models
{
    class OffertaModel : OFFERTA
    {
        #region PROPRIETA
        public OFFERTA OffertaOriginale { get; set; }
        public AnnuncioModel AnnuncioModel { get; set; }
        #endregion

        #region COSTRUTTORI
        public OffertaModel(int id, PERSONA venditore)
        {
            this.ID = id;
            this.ANNUNCIO = new ANNUNCIO();
            this.ANNUNCIO.ID_PERSONA = venditore.ID;
        }

        public OffertaModel(OFFERTA model)
        {
            OffertaOriginale = model;
            AnnuncioModel = new AnnuncioModel(model.ANNUNCIO);
            CopyAttributes<OFFERTA>(model);
        }
        #endregion

        #region METODI PUBBLICI

        public VerificaAcquisto CheckOfferta(PersonaModel utente, OFFERTA model)
        {
            int credito = utente.ContoCorrente.Count(m => m.STATO == (int)StatoMoneta.ASSEGNATA);

            if (model.ID_PERSONA == utente.Persona.ID)
                return VerificaAcquisto.AnnuncioPersonale;

            if (!(model.STATO == (int)StatoOfferta.ATTIVA || model.STATO == (int)StatoOfferta.SOSPESA || model.STATO == (int)StatoOfferta.ACCETTATA_ATTESO_PAGAMENTO))
                return VerificaAcquisto.AnnuncioNonDisponibile;

            if (model.PUNTI > credito)
                return VerificaAcquisto.CreditiNonSufficienti;

            if (utente.Persona.STATO != (int)Stato.ATTIVO)
                return VerificaAcquisto.VenditoreNonAttivo;

            //ANNUNCIO_TIPO_SCAMBIO tipoScambio = model.ANNUNCIO.ANNUNCIO_TIPO_SCAMBIO.FirstOrDefault(m => m.TIPO_SCAMBIO == (int)TipoScambio.Spedizione);
            //if (model.ANNUNCIO.ID_OGGETTO != null && tipoScambio != null && model.OFFERTA_SPEDIZIONE.Count() > 0)
            //{
            //    ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE spedizione = tipoScambio.ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE.FirstOrDefault();

            //    if (spedizione != null)
            //    {
            //        return VerificaAcquisto.VerificaCartaCredito;
            //    }
            //}
            if (model.OFFERTA_SPEDIZIONE.Count() > 0)
            {
                OFFERTA_SPEDIZIONE spedizione = model.OFFERTA_SPEDIZIONE.FirstOrDefault();

                if (spedizione != null && spedizione.SOLDI > 0)
                {
                    return VerificaAcquisto.VerificaCartaCredito;
                }
            }

            if ((int)this.SOLDI > 0 && model.ANNUNCIO.TIPO_PAGAMENTO != (int)TipoPagamento.HAPPY)
                return VerificaAcquisto.VerificaCartaCredito;

            return VerificaAcquisto.Ok;
        }

        public bool Accetta(DatabaseContext db, PersonaModel utente, ref string messaggio)
        {            
            DateTime dataModifica = DateTime.Now;

            TipoScambio tipoScambio = TipoScambio.AMano;
            if (this.OFFERTA_SPEDIZIONE.Count() > 0)
                tipoScambio = TipoScambio.Spedizione;

            VerificaAcquisto statoAcquisto = AnnuncioModel.CheckAcquisto(utente, tipoScambio, false);
            if (statoAcquisto == VerificaAcquisto.Ok)
            {
                // se viene accettata l'offerta allora l'annuncio risulta pagato e venduto
                if (this.OFFERTA_BARATTO.Count() > 0)
                    this.ANNUNCIO.STATO = (int)StatoVendita.BARATTATO;
                else
                    this.ANNUNCIO.STATO = (int)StatoVendita.VENDUTO;
            }
            this.ANNUNCIO.DATA_MODIFICA = dataModifica;
            this.ANNUNCIO.ID_COMPRATORE = utente.Persona.ID;
            this.ANNUNCIO.DATA_VENDITA = dataModifica;
            this.DATA_MODIFICA = dataModifica;
            this.STATO = (int)StatoOfferta.ACCETTATA;

            // salvataggio dati su db
            this.OffertaOriginale.STATO = this.STATO;
            this.OffertaOriginale.DATA_MODIFICA = this.DATA_MODIFICA;

            // SALVATAGGIO MODIFICHE RECUPERANDO OFFERTA DA DB O PROVANDO A SETTARE COME MODIFICABILE L'ATTUALE OGGETTO
            int salvataggi = db.SaveChanges();
            if (salvataggi > 1)
            {
                List<CONTO_CORRENTE_MONETA> listaCrediti = db.CONTO_CORRENTE_MONETA.Where(m => this.PUNTI > 0 && m.STATO == (int)StatoMoneta.SOSPESA).Take((int)this.PUNTI).ToList();

                TRANSAZIONE transazione = new TRANSAZIONE();
                if (listaCrediti != null)
                {
                    transazione.ID_CONTO_DESTINATARIO = this.ANNUNCIO.PERSONA.ID_CONTO_CORRENTE;
                    transazione.ID_CONTO_MITTENTE = utente.Persona.ID_CONTO_CORRENTE;
                    transazione.NOME = this.ANNUNCIO.NOME;
                    transazione.PUNTI = this.PUNTI;
                    transazione.SOLDI = (this.SOLDI == null) ? 0 : (int)this.SOLDI;
                    transazione.TIPO = (int)TipoPagamento.HAPPY;
                    transazione.DATA_INSERIMENTO = DateTime.Now;
                    transazione.TEST = 0;
                    transazione.STATO = (int)Stato.ATTIVO;
                    db.TRANSAZIONE.Add(transazione);
                    if (db.SaveChanges() <= 0)
                        throw new Exception(string.Format(ExceptionMessage.NotSavedBidTransaction, this.ID));

                    TRANSAZIONE_ANNUNCIO transazioneAnnuncio = new Models.TRANSAZIONE_ANNUNCIO();
                    transazioneAnnuncio.ID_TRANSAZIONE = transazione.ID;
                    transazioneAnnuncio.ID_ANNUNCIO = this.ID_ANNUNCIO;
                    transazioneAnnuncio.PUNTI = (int)this.PUNTI;
                    transazioneAnnuncio.SOLDI = (int)this.SOLDI;
                    transazioneAnnuncio.DATA_INSERIMENTO = DateTime.Now;
                    transazioneAnnuncio.STATO = (int)StatoPagamento.ACCETTATO;
                    if (db.SaveChanges() > 0)
                        throw new Exception(string.Format(ExceptionMessage.NotSavedBidTransaction, this.ID));

                    // transazione dei punti
                    foreach (CONTO_CORRENTE_MONETA m in listaCrediti)
                    {
                        m.ID_TRANSAZIONE = transazione.ID;
                        m.DATA_MODIFICA = DateTime.Now;
                        m.STATO = (int)StatoMoneta.CEDUTA;
                        db.Entry(m).State = System.Data.Entity.EntityState.Modified;
                        if (db.SaveChanges() <= 0)
                            throw new Exception(string.Format(ExceptionMessage.NotSavedMoney, this.ID, m.ID));

                        // aggiungo i crediti al venditore
                        CONTO_CORRENTE_MONETA contoCorrente = new CONTO_CORRENTE_MONETA();
                        contoCorrente.ID_CONTO_CORRENTE = this.ANNUNCIO.PERSONA.ID_CONTO_CORRENTE;
                        contoCorrente.ID_MONETA = m.ID_MONETA;
                        contoCorrente.ID_TRANSAZIONE = transazione.ID;
                        contoCorrente.DATA_INSERIMENTO = DateTime.Now;
                        contoCorrente.STATO = (int)StatoMoneta.ASSEGNATA;
                        db.CONTO_CORRENTE_MONETA.Add(contoCorrente);
                        if (db.SaveChanges() <= 0)
                            throw new Exception(string.Format(ExceptionMessage.NotSavedMoneyForSeller, this.ID, m.ID));
                    }
                }
                // cambio stato dei baratti offerti
                foreach (OFFERTA_BARATTO baratto in this.OFFERTA_BARATTO.ToList())
                {
                    baratto.DATA_MODIFICA = DateTime.Now;
                    baratto.STATO = (int)StatoVendita.BARATTATO;
                    db.Entry(baratto).State = System.Data.Entity.EntityState.Modified;
                    if (db.SaveChanges() <= 0)
                        throw new Exception(string.Format(ExceptionMessage.NotSavedBidBarter, this.ID, baratto.ID));
                }

                AnnullaOfferteEffettuate(db, this.ID_ANNUNCIO);

                AnnullaOfferteRicevute(db, this.ID_ANNUNCIO, this.ID);

                this.STATO = this.STATO;
                return true;
            }
            return false;
        }

        public bool Rifiuta()
        {
            //using (DatabaseContext db = new DatabaseContext())
            //{
            DatabaseContext db = new DatabaseContext();
            try {
                db.Database.Connection.Open();
                using (var transazione = db.Database.BeginTransaction())
                {
                    OFFERTA offerta = db.OFFERTA.Where(o => o.ID == this.ID && o.ANNUNCIO.ID_PERSONA == this.ANNUNCIO.ID_PERSONA && o.STATO == (int)StatoOfferta.ATTIVA).SingleOrDefault();
                    offerta.DATA_MODIFICA = DateTime.Now;
                    offerta.STATO = (int)StatoOfferta.ANNULLATA;
                    OffertaContoCorrenteMoneta offertaMoneta = new OffertaContoCorrenteMoneta();
                    offertaMoneta.RemoveCrediti(db, (int)offerta.PUNTI, new PersonaModel(offerta.PERSONA));
                    offerta.PERSONA.DATA_MODIFICA = DateTime.Now;
                    db.Entry(offerta).State = System.Data.Entity.EntityState.Modified;
                    if (db.SaveChanges() > 1)
                    {
                        transazione.Commit();
                        return true;
                    }
                    transazione.Rollback();
                }
            }
            catch (Exception eccezione)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
            }
            finally
            {
                if (db.Database.Connection.State != System.Data.ConnectionState.Closed)
                {
                    db.Database.Connection.Close();
                    db.Database.Connection.Dispose();
                }
            }
            //}
            return false;
        }

        public static void AnnullaOfferteEffettuate(DatabaseContext db,int vendita)
        {
            // annullo offerte di baratto effettuate
            foreach(OFFERTA_BARATTO b in db.OFFERTA_BARATTO.Where(b => b.ID_ANNUNCIO == vendita && (b.OFFERTA.STATO != (int)StatoOfferta.ACCETTATA && b.OFFERTA.STATO != (int)StatoOfferta.ANNULLATA)).ToList())
            {
                b.OFFERTA.DATA_MODIFICA = DateTime.Now;
                b.OFFERTA.STATO = (int)StatoOfferta.ANNULLATA;
                OffertaContoCorrenteMoneta offertaMoneta = new OffertaContoCorrenteMoneta();
                offertaMoneta.RemoveCrediti(db, (int)b.OFFERTA.PUNTI, new PersonaModel(b.OFFERTA.PERSONA));
                b.OFFERTA.PERSONA.DATA_MODIFICA = DateTime.Now;
                db.Entry(b.OFFERTA).State = System.Data.Entity.EntityState.Modified;
                db.Entry(b.OFFERTA.PERSONA).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void AnnullaOfferteRicevute(DatabaseContext db, int vendita, int offerta = 0)
        {
            // anullo le altre offerte ricevute
            foreach (OFFERTA o in db.OFFERTA.Where(o => (offerta == 0 || o.ID != offerta) 
                && (o.STATO != (int)StatoOfferta.ACCETTATA && o.STATO != (int)StatoOfferta.ANNULLATA) 
                && o.ID_ANNUNCIO == vendita).ToList()) { 
                o.DATA_MODIFICA = DateTime.Now;
                o.STATO = (int)StatoOfferta.ANNULLATA;
                o.PERSONA.DATA_MODIFICA = DateTime.Now;
                db.Entry(o.PERSONA).State = System.Data.Entity.EntityState.Modified;
                db.Entry(o).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                // restituzione crediti sospesi
                OffertaContoCorrenteMoneta offertaMoneta = new OffertaContoCorrenteMoneta();
                offertaMoneta.RemoveCrediti(db, (int)o.PUNTI, new PersonaModel(o.PERSONA));
            }
        }

        #endregion

        #region METODI PRIVATI
        private void CopyAttributes<T>(T model) where T : OFFERTA
        {
            PropertyInfo[] properties = model.GetType().GetProperties();
            for (int i = 0; i < (int)properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                this.GetType().GetProperty(propertyInfo.Name).SetValue(this, propertyInfo.GetValue(model));
            }
        }
        #endregion

    }
}
