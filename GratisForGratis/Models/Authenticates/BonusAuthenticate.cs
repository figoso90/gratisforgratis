using GratisForGratis.App_GlobalResources;
using GratisForGratis.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GratisForGratis.Models.Authenticates
{
    public class BonusAuthenticate
    {
        #region METODI PUBBLICI
        public void AddBonus(DatabaseContext db, PERSONA persona, Guid tokenPortale, decimal punti, TipoTransazione tipo, string nomeTransazione, int? idAnnuncio = null)
        {
            ATTIVITA attivita = db.ATTIVITA.Where(p => p.TOKEN == tokenPortale).SingleOrDefault();
            PERSONA_ATTIVITA proprietario = attivita.PERSONA_ATTIVITA.SingleOrDefault(m => m.RUOLO == (int)RuoloProfilo.Proprietario && m.STATO == (int)Stato.ATTIVO);
            PERSONA mittente = null;
            if (proprietario != null)
                mittente = proprietario.PERSONA;
            TRANSAZIONE model = new TRANSAZIONE();
            model.ID_CONTO_MITTENTE = attivita.ID_CONTO_CORRENTE;
            model.ID_CONTO_DESTINATARIO = persona.ID_CONTO_CORRENTE;
            model.TIPO = (int)tipo;
            model.NOME = nomeTransazione;
            model.PUNTI = punti;
            model.DATA_INSERIMENTO = DateTime.Now;
            model.STATO = (int)StatoPagamento.ACCETTATO;
            db.TRANSAZIONE.Add(model);
            db.SaveChanges();

            if (idAnnuncio != null)
            {
                TRANSAZIONE_ANNUNCIO transazioneAnnuncio = new Models.TRANSAZIONE_ANNUNCIO();
                transazioneAnnuncio.ID_TRANSAZIONE = model.ID;
                transazioneAnnuncio.ID_ANNUNCIO = (int)idAnnuncio;
                transazioneAnnuncio.PUNTI = punti;
                transazioneAnnuncio.SOLDI = Utility.cambioValuta(transazioneAnnuncio.PUNTI);
                transazioneAnnuncio.DATA_INSERIMENTO = DateTime.Now;
                transazioneAnnuncio.STATO = (int)StatoPagamento.ACCETTATO;
                db.TRANSAZIONE_ANNUNCIO.Add(transazioneAnnuncio);
                db.SaveChanges();
            }

            // aggiunta credito
            ContoCorrenteCreditoModel credito = new ContoCorrenteCreditoModel(db, persona.ID_CONTO_CORRENTE);
            credito.Earn(model.ID, punti);

            //SendNotifica(mittente, persona, TipoNotifica.Bonus, ControllerContext, "bonusRicevuto", model, attivita, db);
            //TempData["BONUS"] = string.Format(Bonus.YouWin, punti, Language.Moneta);

            //if (tipo != TipoTransazione.BonusLogin)
                //RefreshPunteggioUtente(db);
        }
        #endregion

        #region METODI PRIVATI
        private void AddPuntiLogin(DatabaseContext db, PERSONA utente)
        {
            decimal puntiAccesso = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["bonusAccesso"]);
            utente.DATA_ACCESSO = DateTime.Now;
            db.Entry(utente).State = System.Data.Entity.EntityState.Modified;
            if (db.SaveChanges() > 0)
            {
                Guid tokenPortale = Guid.Parse(System.Configuration.ConfigurationManager.AppSettings["portaleweb"]);
                this.AddBonus(db, utente, tokenPortale, puntiAccesso, TipoTransazione.BonusLogin, Bonus.Login);
                //db.SaveChanges();
            }
        }
        #endregion
    }
}