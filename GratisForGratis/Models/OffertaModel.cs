using GratisForGratis.App_GlobalResources;
using GratisForGratis.Models.Enumerators;
using System;
using System.Collections.Generic;
using System.Configuration;
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

        public VerificaOfferta CheckOfferta(PersonaModel utente, OFFERTA model)
        {
            decimal credito = model.PERSONA.CONTO_CORRENTE.CONTO_CORRENTE_CREDITO
                .Where(m => m.STATO == (int)StatoCredito.SOSPESO).Sum(m => m.PUNTI);
            //int credito = utente.Credito.Count(m => m.STATO == (int)StatoCredito.ASSEGNATO);

            if (model.PERSONA.STATO != (int)Stato.ATTIVO)
            {
                return VerificaOfferta.CompratoreNonAttivo;
            }

            if (model.ID_PERSONA == utente.Persona.ID)
                return VerificaOfferta.OffertaNonValida;

            if (!(model.STATO == (int)StatoOfferta.ATTIVA || model.STATO == (int)StatoOfferta.SOSPESA || model.STATO == (int)StatoOfferta.ACCETTATA_ATTESO_PAGAMENTO))
                return VerificaOfferta.OffertaNonDisponibile;

            if (model.PUNTI > credito)
                return VerificaOfferta.CreditiNonSufficienti;

            if (utente.Persona.STATO != (int)Stato.ATTIVO)
                return VerificaOfferta.VenditoreNonAttivo;

            if (model.OFFERTA_SPEDIZIONE.Count() > 0)
            {
                OFFERTA_SPEDIZIONE spedizione = model.OFFERTA_SPEDIZIONE.FirstOrDefault();

                if (spedizione != null && spedizione.SOLDI > 0)
                {
                    return VerificaOfferta.VerificaCartaDiCredito;
                }
            }

            if ((int)this.SOLDI > 0 && model.ANNUNCIO.TIPO_PAGAMENTO != (int)TipoPagamento.HAPPY)
                return VerificaOfferta.VerificaCartaDiCredito;

            return VerificaOfferta.Ok;
        }

        public bool Accetta(DatabaseContext db, PERSONA compratore, List<CONTO_CORRENTE_CREDITO> listaCreditoCompratore, ref string messaggio)
        {            
            DateTime dataModifica = DateTime.Now;

            TipoScambio tipoScambio = TipoScambio.AMano;
            if (this.OFFERTA_SPEDIZIONE.Count() > 0)
                tipoScambio = TipoScambio.Spedizione;

            VerificaAcquisto statoAcquisto = AnnuncioModel.CheckAcquisto(compratore, listaCreditoCompratore, tipoScambio, false, true);

            // SE VIENE ACCETTA L'OFFERTA E NON C'è DA PAGARE LA SPEDIZIONE
            // PER IL PROPRIO ANNUNCIO, ALLORA CAMBIO STATO ALL'ANNUNCIO,
            // ALTRIMENTI LASCIO IN STATO SOSPESO L'ANNUNCIO
            if (statoAcquisto == VerificaAcquisto.Ok)
            {
                if (this.OFFERTA_BARATTO.Count() > 0)
                    this.ANNUNCIO.STATO = (int)StatoVendita.BARATTATO;
                else
                    this.ANNUNCIO.STATO = (int)StatoVendita.VENDUTO;
            }
            else if (statoAcquisto != VerificaAcquisto.VerificaCartaCredito && statoAcquisto != VerificaAcquisto.SpedizioneDaPagare)
            {
                // se non deve farsi pagare la spedizione ma c'è un errore differente
                // allora non accetto l'offerta
                return false;
            }

            this.ANNUNCIO.DATA_MODIFICA = dataModifica;
            this.ANNUNCIO.ID_COMPRATORE = compratore.ID;
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
                TRANSAZIONE transazione = new TRANSAZIONE();
                transazione.ID_CONTO_DESTINATARIO = this.ANNUNCIO.PERSONA.ID_CONTO_CORRENTE;
                transazione.ID_CONTO_MITTENTE = compratore.ID_CONTO_CORRENTE;
                transazione.NOME = this.ANNUNCIO.NOME;
                transazione.PUNTI = this.PUNTI;
                transazione.SOLDI = Controllers.Utils.cambioValuta(this.PUNTI);
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
                transazioneAnnuncio.PUNTI = (decimal)transazione.PUNTI;
                transazioneAnnuncio.SOLDI = (decimal)transazione.SOLDI;
                transazioneAnnuncio.DATA_INSERIMENTO = DateTime.Now;
                transazioneAnnuncio.STATO = (int)StatoPagamento.ACCETTATO;
                db.TRANSAZIONE_ANNUNCIO.Add(transazioneAnnuncio);
                if (db.SaveChanges() <= 0)
                    throw new Exception(string.Format(ExceptionMessage.NotSavedBidTransaction, this.ID));

                // tolgo i punti al mittente                
                if (transazione.PUNTI != null && transazione.PUNTI > 0)
                {
                    var listaCrediti = db.CONTO_CORRENTE_CREDITO.Where(m => m.ID_CONTO_CORRENTE == transazione.ID_CONTO_MITTENTE &&
                    m.ID_OFFERTA_USCITA == this.ID && m.STATO == (int)StatoMoneta.SOSPESA).ToList();
                    listaCrediti.ForEach(m =>
                    {
                        m.ID_TRANSAZIONE_USCITA = transazione.ID;
                        m.DATA_SCADENZA = DateTime.Now;
                        m.STATO = (int)StatoCredito.CEDUTO;
                    });
                    if (db.SaveChanges() <= 0)
                        throw new Exception(string.Format(ExceptionMessage.NotSavedMoney, this.ID, String.Join(", ", listaCrediti.Select(m => m.ID))));

                    // passo i punti al destinatario
                    CONTO_CORRENTE_CREDITO creditoDestinatario = new CONTO_CORRENTE_CREDITO();
                    creditoDestinatario.ID_CONTO_CORRENTE = transazione.ID_CONTO_DESTINATARIO;
                    creditoDestinatario.ID_TRANSAZIONE_ENTRATA = transazione.ID;
                    creditoDestinatario.PUNTI = (decimal)transazione.PUNTI;
                    creditoDestinatario.SOLDI = Controllers.Utils.cambioValuta(transazione.PUNTI);
                    creditoDestinatario.GIORNI_SCADENZA = Convert.ToInt32(ConfigurationManager.AppSettings["GiorniScadenzaCredito"]);
                    creditoDestinatario.DATA_SCADENZA = DateTime.Now.AddDays(creditoDestinatario.GIORNI_SCADENZA);
                    creditoDestinatario.DATA_INSERIMENTO = DateTime.Now;
                    creditoDestinatario.STATO = (int)StatoCredito.ASSEGNATO;
                    db.CONTO_CORRENTE_CREDITO.Add(creditoDestinatario);
                    if (db.SaveChanges() <= 0)
                        throw new Exception(string.Format(ExceptionMessage.NotSavedMoneyForSeller, this.ID, creditoDestinatario.ID));
                }
                
                // cambio stato dei baratti offerti
                foreach (OFFERTA_BARATTO baratto in this.OFFERTA_BARATTO.ToList())
                {
                    baratto.DATA_MODIFICA = DateTime.Now;
                    baratto.STATO = (int)StatoOfferta.ACCETTATA;
                    db.Entry(baratto).State = System.Data.Entity.EntityState.Modified;
                    if (db.SaveChanges() <= 0)
                        throw new Exception(string.Format(ExceptionMessage.NotSavedBidBarter, this.ID, baratto.ID));

                    baratto.ANNUNCIO.STATO = (int)StatoVendita.BARATTATO;
                    baratto.ANNUNCIO.DATA_MODIFICA = dataModifica;
                    baratto.ANNUNCIO.ID_COMPRATORE = this.ANNUNCIO.ID_PERSONA;
                    baratto.ANNUNCIO.DATA_VENDITA = dataModifica;
                    db.Entry(baratto.ANNUNCIO).State = System.Data.Entity.EntityState.Modified;
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
                    offertaMoneta.RemoveCrediti(db, offerta.ID, (int)offerta.PUNTI, new PersonaModel(offerta.PERSONA));
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
                offertaMoneta.RemoveCrediti(db, b.OFFERTA.ID, (int)b.OFFERTA.PUNTI, new PersonaModel(b.OFFERTA.PERSONA));
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
                offertaMoneta.RemoveCrediti(db, o.ID, (int)o.PUNTI, new PersonaModel(o.PERSONA));
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
