using GratisForGratis.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GratisForGratis.Models.Authenticates
{
    public class SessioneAuthenticate
    {
        #region METODI PUBBLICI
        public bool SendNotifica(PERSONA mittente, PERSONA destinatario, TipoNotifica messaggio, ControllerContext controller, string view, object datiNotifica, ATTIVITA attivitaMittente = null, DatabaseContext db = null)
        {
            bool nuovaConnessione = true;
            try
            {
                if (db != null && db.Database.Connection.State == System.Data.ConnectionState.Open)
                    nuovaConnessione = false;

                NOTIFICA notifica = new NOTIFICA();
                notifica.ID_PERSONA = mittente.ID;
                if (attivitaMittente != null)
                    notifica.ID_ATTIVITA = attivitaMittente.ID;
                notifica.ID_PERSONA_DESTINATARIO = destinatario.ID;
                notifica.MESSAGGIO = (int)messaggio;
                notifica.STATO = (int)StatoNotifica.ATTIVA;
                notifica.DATA_INSERIMENTO = DateTime.Now;
                if (nuovaConnessione)
                    db = new DatabaseContext();

                db.NOTIFICA.Add(notifica);
                return db.SaveChanges() > 0;
            }
            catch (Exception eccezione)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
            }
            finally
            {
                if (nuovaConnessione && db != null)
                    db.Database.Connection.Close();

                try
                {
                    string indirizzoEmail = destinatario.PERSONA_EMAIL.SingleOrDefault(e => e.TIPO == (int)TipoEmail.Registrazione).EMAIL;
                    // modificare oggetto recuperando dal tipo notifica la stringa
                    string oggetto = Components.EnumHelper<TipoNotifica>.GetDisplayValue(messaggio);
                    SendEmail(indirizzoEmail, oggetto, controller, view, datiNotifica);
                    SendChat("", oggetto);
                }
                catch (Exception eccezione)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
                }
            }
            return false;
        }

        public void setSessioneUtente(HttpSessionStateBase sessione, DatabaseContext db, int utente, bool ricordaLogin)
        {
            PersonaModel persona = new PersonaModel(db, utente);

            // login effettuata con successo, aggiungo i punti se ha il profilo attivo completamente e se è un nuovo accesso giornaliero
            DateTime dataCorrente = DateTime.Now;
            //if (persona.Persona.STATO == (int)Stato.ATTIVO && persona.Persona.STATO == (int)Stato.ATTIVO && (persona.Persona.DATA_ACCESSO == null || persona.Persona.DATA_ACCESSO.Value.Year < dataCorrente.Year || (persona.Persona.DATA_ACCESSO.Value.Year == dataCorrente.Year && dataCorrente.DayOfYear > persona.Persona.DATA_ACCESSO.Value.DayOfYear)))
            //    AddPuntiLogin(db, persona.Persona);

            sessione["utente"] = persona;
            if (persona.Attivita != null && persona.Attivita.Count > 0)
                sessione["portaleweb"] = persona.Attivita.Select(item =>
                    new PortaleWebViewModel(item,
                    item.ATTIVITA.ATTIVITA_EMAIL.Where(e => e.ID_ATTIVITA == item.ID_ATTIVITA).ToList(),
                    item.ATTIVITA.ATTIVITA_TELEFONO.Where(t => t.ID_ATTIVITA == item.ID_ATTIVITA).ToList()
                )).ToList();

            //FormsAuthentication.SetAuthCookie(persona.Persona.CONTO_CORRENTE.TOKEN.ToString(), ricordaLogin);
        }
        #endregion

        #region METODI PROTETTI
        protected bool SendEmail(string indirizzoEmail, string oggetto, ControllerContext controller, string nomeView, object datiEmail)
        {
            EmailModel email = new EmailModel(controller);
            email.To.Add(new System.Net.Mail.MailAddress(indirizzoEmail));
            email.Subject = oggetto + " - " + System.Web.Configuration.WebConfigurationManager.AppSettings["nomeSito"];
            email.Body = nomeView;
            email.DatiEmail = datiEmail;
            EmailController emailer = new EmailController();
            return emailer.SendEmail(email);
        }

        #region INVIO MESSAGGIO CHAT

        protected bool SendChat(string telefono, string oggetto)
        {
            return false;
        }

        //protected async System.Threading.Tasks.Task SendCodeTelegram(string numeroTelefono)
        //{
        //    var client = new TLSharp.Core.TelegramClient(106191, "2a5c0da8fcb3acfdf501c3f17fb0fa5f");
        //    await client.ConnectAsync();
        //    var hash = await client.SendCodeRequestAsync(numeroTelefono);
        //}

        //protected async System.Threading.Tasks.Task SendMessaggioTelegram(string numeroTelefono, string code, string messaggio)
        //{
        //    var client = new TLSharp.Core.TelegramClient(106191, "2a5c0da8fcb3acfdf501c3f17fb0fa5f");
        //    await client.ConnectAsync();
        //    var hash = await client.SendCodeRequestAsync(numeroTelefono);

        //    var user = await client.MakeAuthAsync(numeroTelefono, hash, code);
        //    //get available contacts
        //    var result = await client.GetContactsAsync();

        //    //find recipient in contacts
        //    var customer = result.Users
        //        .Where(x => x.GetType() == typeof(TeleSharp.TL.TLUser))
        //        .Cast<TeleSharp.TL.TLUser>()
        //        .FirstOrDefault(x => x.Phone == numeroTelefono);
        //    /*if (user.ToList().Count != 0)
        //    {
        //        foreach (var u in user)
        //        {
        //            if (u.phone.Contains("3965604"))
        //            {
        //                //send message
        //                await client.SendMessageAsync(new TLInputPeerUser() { user_id = u.id }, textBox3.Text);
        //            }
        //        }
        //    }*/
        //    // @channelname or chat_id, messaggio da inviare
        //    await client.SendMessageAsync(new TeleSharp.TL.TLInputPeerUser() { UserId = customer.Id }, messaggio);
        //}

        //protected string SendCodeWhatsApp()
        //{
        //    string messaggio = "";
        //    string password;
        //    if (WhatsAppApi.Register.WhatsRegisterV2.RequestCode("+39 3492240520", out password, "sms"))
        //    {
        //        if (!string.IsNullOrEmpty(password))
        //        {

        //        }
        //        else
        //        {

        //        }
        //    }
        //    else
        //    {
        //        messaggio = "Generazione codice fallita";
        //    }
        //    return messaggio;
        //}

        //protected void SendMessaggioWhatsApp(string messaggio)
        //{
        //    WhatsAppApi.WhatsApp wa = new WhatsAppApi.WhatsApp("+39 3492240520", "", "GratisForGratis", false, false);
        //    wa.OnConnectSuccess += () =>
        //    {
        //        Response.Write("connect");
        //        wa.OnLoginSuccess += (phno, data) =>
        //        {
        //            wa.SendMessage("+39 3492240520", messaggio);
        //        };

        //        wa.OnLoginFailed += (data) =>
        //        {
        //            Response.Write("login failed" + data);
        //        };
        //        wa.Login();
        //    };
        //    wa.OnConnectFailed += (ex) =>
        //    {
        //        Response.Write("connection failed");
        //    };
        //}

        #endregion
        #endregion
    }
}