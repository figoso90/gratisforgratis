using GratisForGratis.App_GlobalResources;
using GratisForGratis.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace GratisForGratis.Models
{
    public class PagamentoModel
    {
        #region PROPRIETA
        public ErrorePagamento Errore { get; set; }
        #endregion

        #region METODI PUBBLICI
        public int? eseguiFromStoredProcedure(string offerta, PersonaModel utente)
        {
            //string token = acquisto.Trim().Substring(3, acquisto.Trim().Length - 6);
            int idOfferta = Utility.DecodeToInt(offerta.Trim().Substring(3, offerta.Trim().Length - 6));
            using (DatabaseContext db = new DatabaseContext())
            {
                System.Data.Entity.Core.Objects.ObjectParameter errore = new System.Data.Entity.Core.Objects.ObjectParameter("Errore", typeof(ErrorePagamento));
                errore.Value = ErrorePagamento.Nessuno;
                Guid portaleWeb = Guid.Parse(System.Configuration.ConfigurationManager.AppSettings["portaleweb"]);

                // DEVO CAMBIARE E FARMI TORNARE IL TRANSAZIONE EFFETTUATO
                int? idPagamento = db.BENE_SAVE_PAGAMENTO(idOfferta, utente.Persona.ID_CONTO_CORRENTE, errore).FirstOrDefault();
                this.Errore = (ErrorePagamento)errore.Value;

                if (this.Errore != ErrorePagamento.Nessuno)
                    return null;

                if (idPagamento == null)
                {
                    this.Errore = ErrorePagamento.Proprietario;
                }



                return idPagamento;
            }
        }
        public void SendEmail(System.Web.Mvc.ControllerContext controller, TRANSAZIONE pagamento, PersonaModel utente)
        {
            PERSONA venditore = pagamento.CONTO_CORRENTE.PERSONA.SingleOrDefault();

            // impostare invio email pagamento effettuato
            EmailModel email = new EmailModel(controller);
            email.To.Add(new System.Net.Mail.MailAddress(venditore.PERSONA_EMAIL.SingleOrDefault(e => e.TIPO == (int)TipoEmail.Registrazione).EMAIL));
            string nominativo = utente.Persona.NOME + " " + utente.Persona.COGNOME;
            email.Subject = String.Format(Email.PaymentSubject, pagamento.NOME, nominativo) + " - " + WebConfigurationManager.AppSettings["nomeSito"];
            email.Body = "Pagamento";
            email.DatiEmail = new SchedaPagamentoViewModel()
            {
                Nome = pagamento.NOME,
                Compratore = nominativo,
                Venditore = venditore.NOME + " " + venditore.COGNOME,
                Punti = (int)pagamento.PUNTI,
                Soldi = (int)pagamento.SOLDI,
                Data = pagamento.DATA_INSERIMENTO,
            };
            new EmailController().SendEmail(email);
        }
        #endregion
    }
}
