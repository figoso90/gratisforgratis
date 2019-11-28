using System;
using System.Configuration;
using System.Linq;

namespace GratisForGratis.Models
{
    public class ContoCorrenteCreditoModel : CONTO_CORRENTE_CREDITO
    {
        #region ATTRIBUTI
        private DatabaseContext _db;
        #endregion

        #region COSTRUTTORI
        public ContoCorrenteCreditoModel(DatabaseContext db, Guid idContoCorrente)
        {
            _db = db;
            ID_CONTO_CORRENTE = idContoCorrente;
        }
        #endregion

        #region METODI PUBBLICI
        public void Earn(int idTransazione, decimal crediti)
        {
            decimal puntiCreditoRimanente = RimuovereDebito(crediti);

            if (puntiCreditoRimanente > 0)
            {
                AddCreditoSuDB(idTransazione, puntiCreditoRimanente);
            }
        }

        public void Suspend(int idOffertaUscita, decimal crediti)
        {
            decimal puntiRimanenti = 0;
            decimal punti = crediti;
            // finchè ho punti da sospendere li tolgo da quelli utilizzabili
            while (punti > 0)
            {
                // recupero il primo credito disponibile
                CONTO_CORRENTE_CREDITO credito = _db.CONTO_CORRENTE_CREDITO
                    .Where(m => m.ID_CONTO_CORRENTE == this.ID_CONTO_CORRENTE
                        && m.STATO == (int)StatoCredito.ASSEGNATO && m.PUNTI > 0 && m.DATA_SCADENZA > DateTime.Now)
                    .OrderBy(m => m.DATA_SCADENZA)
                    .FirstOrDefault();
                // hai punti da togliere tolto quelli recuperati
                if ((punti - credito.PUNTI) < 0)
                {
                    // se i punti da togliere vanno sotto 0 allora sottraggo i punti a quelli che possedevo e azzero quelli da togliere
                    puntiRimanenti = credito.PUNTI - punti;
                    punti = 0;
                }
                else
                {
                    // altrimenti tolgo i punti recuperati a quelli da togliere e vado avanti
                    punti = punti - credito.PUNTI;
                }

                // se i punti da togliere sono azzerati e i punti rimanenti
                // sono maggiori di zero allora aggiorno il credito dei punti da sospendere
                if (punti <= 0 && puntiRimanenti > 0)
                {
                    credito.PUNTI -= puntiRimanenti;
                }
                credito.DATA_MODIFICA = DateTime.Now;
                credito.ID_OFFERTA_USCITA = idOffertaUscita;
                credito.STATO = (int)StatoCredito.SOSPESO;
                _db.SaveChanges();

                // se avanzano punti dal credito sottratto li aggiungo per lasciarli attivi
                if (punti <= 0 && puntiRimanenti > 0)
                {
                    CONTO_CORRENTE_CREDITO creditoCompratore = new CONTO_CORRENTE_CREDITO();
                    creditoCompratore.ID_CONTO_CORRENTE = this.ID_CONTO_CORRENTE;
                    creditoCompratore.ID_TRANSAZIONE_ENTRATA = credito.ID_TRANSAZIONE_ENTRATA;
                    creditoCompratore.PUNTI = puntiRimanenti;
                    creditoCompratore.SOLDI = Utility.cambioValuta(creditoCompratore.PUNTI);
                    creditoCompratore.GIORNI_SCADENZA = credito.GIORNI_SCADENZA;
                    creditoCompratore.DATA_SCADENZA = credito.DATA_SCADENZA;
                    creditoCompratore.DATA_INSERIMENTO = DateTime.Now;
                    creditoCompratore.STATO = (int)StatoCredito.ASSEGNATO;
                    _db.CONTO_CORRENTE_CREDITO.Add(creditoCompratore);
                    _db.SaveChanges();
                }
                puntiRimanenti = 0;
            }
        }

        // NON ANCORA USATO
        public void PaySystem(int idTransazione, decimal debito)
        {
            decimal debitoRimanente = RimuovereCredito(-debito);

            if (debitoRimanente < 0)
            {
                AddDebitoSuDB(idTransazione, debitoRimanente);
            }
        }

        public void PayUser()
        {

        }

        public void PayUserForBid(Guid idContoDestinatario, int idOfferta, int idTransazione)
        {
            var listaCrediti = _db.CONTO_CORRENTE_CREDITO.Where(m => m.ID_CONTO_CORRENTE == this.ID_CONTO_CORRENTE &&
                    m.ID_OFFERTA_USCITA == idOfferta && m.STATO == (int)StatoMoneta.SOSPESA).ToList();
            listaCrediti.ForEach(m =>
            {
                m.ID_TRANSAZIONE_USCITA = idTransazione;
                m.DATA_SCADENZA = DateTime.Now;
                m.STATO = (int)StatoCredito.CEDUTO;
            });
            if (_db.SaveChanges() <= 0)
                throw new Exception(string.Format(App_GlobalResources.ExceptionMessage.NotSavedMoney, idOfferta, 
                    String.Join(", ", listaCrediti.Select(m => m.ID))));

            ContoCorrenteCreditoModel creditoDestinatario = new ContoCorrenteCreditoModel(_db, idContoDestinatario);
            creditoDestinatario.AddCreditoSuDB(idTransazione, listaCrediti.Sum(m => m.PUNTI));
        }
        #endregion

        #region METODI PRIVATI
        private decimal RimuovereDebito(decimal crediti)
        {
            // verifica se ho del credito negativo da estinguere
            var creditoNegativo = _db.CONTO_CORRENTE_CREDITO.Where(m => m.ID_CONTO_CORRENTE == this.ID_CONTO_CORRENTE &&
                m.STATO == (int)StatoCredito.ASSEGNATO && m.PUNTI < 0).ToList();
            decimal puntiCreditoRimanente = crediti;
            foreach (CONTO_CORRENTE_CREDITO m in creditoNegativo)
            {
                decimal totale = puntiCreditoRimanente + m.PUNTI;
                m.DATA_MODIFICA = DateTime.Now;
                m.DATA_SCADENZA = DateTime.Now;
                m.STATO = (int)StatoCredito.ELIMINATO;
                _db.SaveChanges();
                if (totale < 0)
                {
                    // rimango con credito zero e segno il debito rimasto
                    CONTO_CORRENTE_CREDITO debitoRimanente = new CONTO_CORRENTE_CREDITO();
                    debitoRimanente.ID_CONTO_CORRENTE = this.ID_CONTO_CORRENTE;
                    debitoRimanente.ID_TRANSAZIONE_ENTRATA = m.ID_TRANSAZIONE_ENTRATA;
                    debitoRimanente.PUNTI = totale;
                    debitoRimanente.SOLDI = Utility.cambioValuta(debitoRimanente.PUNTI);
                    debitoRimanente.GIORNI_SCADENZA = m.GIORNI_SCADENZA;
                    debitoRimanente.DATA_SCADENZA = m.DATA_SCADENZA;
                    debitoRimanente.DATA_INSERIMENTO = m.DATA_INSERIMENTO;
                    debitoRimanente.DATA_MODIFICA = DateTime.Now;
                    debitoRimanente.STATO = (int)StatoCredito.ASSEGNATO;
                    _db.CONTO_CORRENTE_CREDITO.Add(debitoRimanente);
                    _db.SaveChanges();
                    puntiCreditoRimanente = 0;
                    break;
                }
                else if (totale == 0)
                {
                    // rimango con credito zero
                    puntiCreditoRimanente = 0;
                    break;
                }
                else
                {
                    // continuo ad eliminare il debito rimanente
                    puntiCreditoRimanente = totale;
                    continue;
                }
            }
            return puntiCreditoRimanente;
        }

        private decimal RimuovereCredito(decimal debito)
        {
            // verifica se ho del credito positivo da rimuovere
            var creditoPositivo = _db.CONTO_CORRENTE_CREDITO.Where(m => m.ID_CONTO_CORRENTE == this.ID_CONTO_CORRENTE &&
                m.STATO == (int)StatoCredito.ASSEGNATO && m.PUNTI > 0).ToList();
            decimal puntiDebitoRimanente = debito;
            foreach (CONTO_CORRENTE_CREDITO m in creditoPositivo)
            {
                decimal totale = m.PUNTI + puntiDebitoRimanente;
                m.DATA_MODIFICA = DateTime.Now;
                m.DATA_SCADENZA = DateTime.Now;
                m.STATO = (int)StatoCredito.ELIMINATO;
                _db.SaveChanges();
                if (totale < 0)
                {
                    // se rimane del debito, continuo a sottrarlo ai prossimi crediti rimasti
                    puntiDebitoRimanente = totale;
                    continue;
                }
                else if (totale == 0)
                {
                    // rimango con debito zero
                    puntiDebitoRimanente = 0;
                    break;
                }
                else
                {
                    // rimango con debito zero e segno il credito rimasto
                    CONTO_CORRENTE_CREDITO debitoRimanente = new CONTO_CORRENTE_CREDITO();
                    debitoRimanente.ID_CONTO_CORRENTE = this.ID_CONTO_CORRENTE;
                    debitoRimanente.ID_TRANSAZIONE_ENTRATA = m.ID_TRANSAZIONE_ENTRATA;
                    debitoRimanente.PUNTI = totale;
                    debitoRimanente.SOLDI = Utility.cambioValuta(debitoRimanente.PUNTI);
                    debitoRimanente.GIORNI_SCADENZA = m.GIORNI_SCADENZA;
                    debitoRimanente.DATA_SCADENZA = m.DATA_SCADENZA;
                    debitoRimanente.DATA_INSERIMENTO = m.DATA_INSERIMENTO;
                    debitoRimanente.DATA_MODIFICA = DateTime.Now;
                    debitoRimanente.STATO = (int)StatoCredito.ASSEGNATO;
                    _db.CONTO_CORRENTE_CREDITO.Add(debitoRimanente);
                    _db.SaveChanges();
                    puntiDebitoRimanente = 0;
                    break;
                }
            }
            return puntiDebitoRimanente;
        }

        private bool AddCreditoSuDB(int idTransazione, decimal crediti)
        {
            CONTO_CORRENTE_CREDITO contoCorrenteCredito = new CONTO_CORRENTE_CREDITO();
            contoCorrenteCredito.ID_CONTO_CORRENTE = this.ID_CONTO_CORRENTE;
            contoCorrenteCredito.ID_TRANSAZIONE_ENTRATA = idTransazione;
            contoCorrenteCredito.PUNTI = crediti;
            contoCorrenteCredito.SOLDI = Utility.cambioValuta(contoCorrenteCredito.PUNTI);
            contoCorrenteCredito.GIORNI_SCADENZA = Convert.ToInt32(ConfigurationManager.AppSettings["GiorniScadenzaCredito"]);
            contoCorrenteCredito.DATA_SCADENZA = DateTime.Now.AddDays(contoCorrenteCredito.GIORNI_SCADENZA);
            contoCorrenteCredito.DATA_INSERIMENTO = DateTime.Now;
            contoCorrenteCredito.STATO = (int)StatoCredito.ASSEGNATO;
            _db.CONTO_CORRENTE_CREDITO.Add(contoCorrenteCredito);
            return _db.SaveChanges() > 0;
        }

        // NON ANCORA USATO
        private bool AddDebitoSuDB(int idTransazione, decimal crediti)
        {
            CONTO_CORRENTE_CREDITO contoCorrenteCredito = new CONTO_CORRENTE_CREDITO();
            contoCorrenteCredito.ID_CONTO_CORRENTE = this.ID_CONTO_CORRENTE;
            contoCorrenteCredito.ID_TRANSAZIONE_ENTRATA = idTransazione;
            contoCorrenteCredito.PUNTI = crediti;
            contoCorrenteCredito.SOLDI = Utility.cambioValuta(contoCorrenteCredito.PUNTI);
            // scadenza infinita (valutare se tenere un debito al massimo per 1 anno)
            contoCorrenteCredito.GIORNI_SCADENZA = -1;
            contoCorrenteCredito.DATA_SCADENZA = DateTime.MaxValue;
            contoCorrenteCredito.DATA_INSERIMENTO = DateTime.Now;
            contoCorrenteCredito.STATO = (int)StatoCredito.ASSEGNATO;
            _db.CONTO_CORRENTE_CREDITO.Add(contoCorrenteCredito);
            return _db.SaveChanges() > 0;
        }
        #endregion
    }
}