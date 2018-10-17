using GratisForGratis.App_GlobalResources;
using GratisForGratis.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace GratisForGratis.Controllers
{
    [Authorize]
    [HandleError]
    public class AdvancedController : Controller
    {
        #region ACTION
        [HttpPost]
        [Authorize]
        public ActionResult SuggestAdActivation(string chiave1, string idAttivita = null)
        {
            string backUrl = Request.UrlReferrer.PathAndQuery;
            using (DatabaseContext db = new DatabaseContext())
            {
                //string tokenAnnuncio = id.Substring(3, id.Length - 6);
                db.Database.Connection.Open();
                ANNUNCIO annuncio = db.ANNUNCIO.Single(m => m.TOKEN.ToString() == chiave1);
                int idAnnuncio = annuncio.ID;
                PERSONA persona = (Session["utente"] as PersonaModel).Persona;
                int idUtente = persona.ID;
                int? keyAttivita = null;
                // se è stata selezionata una attività commerciale dell'utente
                List<AttivitaModel> listaAttivita = (Session["utente"] as PersonaModel).Attivita;
                if (listaAttivita != null && listaAttivita.Count > 0)
                    keyAttivita = listaAttivita.SingleOrDefault(m => m.Attivita.TOKEN.ToString() == idAttivita).ID;
                // notifica già inviata.
                if (db.ANNUNCIO_NOTIFICA.Count(m => m.ID_ANNUNCIO == idAnnuncio && (m.NOTIFICA.ID_PERSONA == idUtente || (keyAttivita != null && m.NOTIFICA.ID_ATTIVITA == keyAttivita))) > 0)
                {
                    TempData["MESSAGGIO"] = Language.SuggestAdActivationError;
                    //return RedirectToAction("", "Cerca");
                    return Redirect(backUrl);
                }
                Guid portale = Guid.Parse(System.Configuration.ConfigurationManager.AppSettings["portaleweb"]);
                int bonus = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["bonusSuggerimentoAttivazioneAnnuncio"]);
                this.AddBonus(db, persona, portale, bonus, TipoTransazione.BonusSuggerimentoAttivazioneAnnuncio, Bonus.SuggestAdActivation);

                NOTIFICA notifica = new NOTIFICA();
                notifica.ID_PERSONA = idUtente;
                notifica.ID_ATTIVITA = keyAttivita;
                notifica.ID_PERSONA_DESTINATARIO = annuncio.ID_PERSONA;
                notifica.ID_ATTIVITA_DESTINATARIO = annuncio.ID_ATTIVITA;
                notifica.MESSAGGIO = (int)TipoNotifica.AttivaAnnuncio;
                notifica.DATA_INSERIMENTO = DateTime.Now;
                notifica.STATO = (int)Stato.ATTIVO;
                db.NOTIFICA.Add(notifica);
                if (db.SaveChanges() > 0)
                {
                    db.ANNUNCIO_NOTIFICA.Add(new ANNUNCIO_NOTIFICA()
                    {
                        ID_ANNUNCIO = idAnnuncio,
                        ID_NOTIFICA = notifica.ID
                    });
                    if (db.SaveChanges() > 0)
                    {
                        TempData["MESSAGGIO"] = Language.SuggestAdActivationOK;
                        //return RedirectToAction("", "Cerca");
                        return Redirect(backUrl);
                    }
                }
            }
            TempData["MESSAGGIO"] = Language.SuggestAdActivationKO;
            //return RedirectToAction("", "Cerca");
            return Redirect(backUrl);
        }
        /*
        [HttpPost]
        [Authorize]
        [Filters.ValidateAjax]
        public JsonResult SuggestAdActivation(string id, string idAttivita = null)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                //string tokenAnnuncio = id.Substring(3, id.Length - 6);
                db.Database.Connection.Open();
                ANNUNCIO annuncio = db.ANNUNCIO.Single(m => m.TOKEN.ToString() == id);
                int idAnnuncio = annuncio.ID;
                int idUtente = (Session["utente"] as PersonaModel).Persona.ID;
                int? keyAttivita = null;
                // se è stata selezionata una attività commerciale dell'utente
                List<AttivitaModel> listaAttivita = (Session["utente"] as PersonaModel).Attivita;
                if (listaAttivita != null && listaAttivita.Count > 0)
                    keyAttivita = listaAttivita.SingleOrDefault(m => m.Attivita.TOKEN.ToString() == idAttivita).ID;
                // notifica già inviata.
                if (db.ANNUNCIO_NOTIFICA.Count(m => m.ID_ANNUNCIO == idAnnuncio && (m.NOTIFICA.ID_PERSONA == idUtente || (keyAttivita != null && m.NOTIFICA.ID_ATTIVITA == keyAttivita))) > 0)
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                    return Json(App_GlobalResources.Language.SuggestAdActivationError);
                }

                NOTIFICA notifica = new NOTIFICA();
                notifica.ID_PERSONA = idUtente;
                notifica.ID_ATTIVITA = keyAttivita;
                notifica.ID_PERSONA_DESTINATARIO = annuncio.ID_PERSONA;
                notifica.ID_ATTIVITA_DESTINATARIO = annuncio.ID_ATTIVITA;
                notifica.MESSAGGIO = (int)TipoNotifica.AttivaAnnuncio;
                notifica.DATA_INSERIMENTO = DateTime.Now;
                notifica.STATO = (int)Stato.ATTIVO;
                db.NOTIFICA.Add(notifica);
                if (db.SaveChanges() > 0)
                {
                    db.ANNUNCIO_NOTIFICA.Add(new ANNUNCIO_NOTIFICA()
                    {
                        ID_ANNUNCIO = idAnnuncio,
                        ID_NOTIFICA = notifica.ID
                    });
                    if (db.SaveChanges() > 0)
                    {
                        return Json(App_GlobalResources.Language.SuggestAdActivationOK);
                    }
                }
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return Json(App_GlobalResources.Language.SuggestAdActivationKO);
        }
        */
        #endregion

        #region METODI PUBBLICI

        [AllowAnonymous]
        public void setSessioneUtente(HttpSessionStateBase sessione, DatabaseContext db, int utente, bool ricordaLogin)
        {
            PERSONA model = db.PERSONA
                .Include("ABBONAMENTO")
                .Include("PERSONA_PRIVACY")
                .Include("PERSONA_EMAIL")
                .Include("PERSONA_TELEFONO")
                .Include("PERSONA_METODO_PAGAMENTO")
                .Include("PERSONA_INDIRIZZO")
                .Include("PERSONA_FOTO")
                .Include("PERSONA_ATTIVITA")
                .SingleOrDefault(m => m.ID == utente);
            
            // login effettuata con successo, aggiungo i punti se ha il profilo attivo completamente e se è un nuovo accesso giornaliero
            DateTime dataCorrente = DateTime.Now;
            if (model.STATO == (int)Stato.ATTIVO && model.STATO == (int)Stato.ATTIVO && (model.DATA_ACCESSO == null || model.DATA_ACCESSO.Value.Year < dataCorrente.Year || (model.DATA_ACCESSO.Value.Year == dataCorrente.Year && dataCorrente.DayOfYear > model.DATA_ACCESSO.Value.DayOfYear)))
                AddPuntiLogin(db, model);

            PersonaModel persona = new PersonaModel(model);
            persona.Email = model.PERSONA_EMAIL.Where(m => m.ID_PERSONA == utente).ToList();
            persona.Telefono = model.PERSONA_TELEFONO.Where(m => m.ID_PERSONA == utente).ToList();
            persona.MetodoPagamento = model.PERSONA_METODO_PAGAMENTO.Where(m => m.ID_PERSONA == utente).ToList();
            persona.Indirizzo = db.PERSONA_INDIRIZZO
                .Include("INDIRIZZO")
                .Include("INDIRIZZO.COMUNE")
                .Include("PERSONA_INDIRIZZO_SPEDIZIONE")
                .Where(m => m.ID_PERSONA == utente)
                .ToList();
            persona.Foto = model.PERSONA_FOTO.Where(m => m.ID_PERSONA == utente).OrderByDescending(m => m.ORDINE)
                .AsEnumerable().Select(m => new FotoModel(m.ALLEGATO)).ToList();
            model.PERSONA_ATTIVITA.Where(m => m.ID_PERSONA == utente).ToList().ForEach(m => {
                persona.Attivita.Add(new AttivitaModel(m));
            });
            persona.ContoCorrente = db.CONTO_CORRENTE_MONETA.Where(m => m.ID_CONTO_CORRENTE == persona.Persona.ID_CONTO_CORRENTE).ToList();
            persona.NomeVisibile = (string.IsNullOrWhiteSpace(persona.Persona.NOME + persona.Persona.COGNOME) ? persona.Email
                .Where(m => m.TIPO == (int)TipoEmail.Registrazione).SingleOrDefault().EMAIL: string.Concat(persona.Persona.NOME, " ", persona.Persona.COGNOME));
            persona.NumeroMessaggiDaLeggere = db.CHAT.Count(m => m.ID_DESTINATARIO == utente && m.STATO == (int)StatoChat.INVIATO);
            persona.NumeroNotificheDaLeggere = db.NOTIFICA.Count(m => m.ID_PERSONA_DESTINATARIO == utente && m.STATO == (int)StatoNotifica.ATTIVA);

            sessione["utente"] = persona;
            if (persona.Attivita != null && persona.Attivita.Count > 0)
                sessione["portaleweb"] = persona.Attivita.Select(item =>
                    new PortaleWebViewModel(item,
                    item.ATTIVITA.ATTIVITA_EMAIL.Where(e => e.ID_ATTIVITA == item.ID_ATTIVITA).ToList(),
                    item.ATTIVITA.ATTIVITA_TELEFONO.Where(t => t.ID_ATTIVITA == item.ID_ATTIVITA).ToList()
                )).ToList();
            /*
            sessione["portaleweb"] = persona.Attivita.Select(item => 
                new PortaleWebViewModel(item, 
                item.Attivita.ATTIVITA_EMAIL.Where(e => e.ID_ATTIVITA==item.ID_ATTIVITA).ToList(), 
                item.Attivita.ATTIVITA_TELEFONO.Where(t => t.ID_ATTIVITA == item.ID_ATTIVITA).ToList()
            )).ToList();
            */

            FormsAuthentication.SetAuthCookie(persona.Persona.CONTO_CORRENTE.TOKEN.ToString(), ricordaLogin);
            //FormsAuthentication.RedirectFromLoginPage(utente.EMAIL, ricordaLogin);
        }

        public int AddUtenteFacebook(string accessToken, string tokenPermanente, dynamic paramsFB, DatabaseContext db)
        {
            string email = paramsFB.email;
            PERSONA persona = db.PERSONA.SingleOrDefault(m => m.PERSONA_EMAIL.FirstOrDefault(m2 => 
                m2.EMAIL == email && m2.TIPO == (int)TipoEmail.Registrazione) != null
            );
            if (persona != null)
            {
                if (persona.FACEBOOK_TOKEN_SESSIONE != accessToken)
                {
                    persona.FACEBOOK_TOKEN_SESSIONE = accessToken;
                    if (persona.FACEBOOK_TOKEN_PERMANENTE != tokenPermanente)
                        persona.FACEBOOK_TOKEN_PERMANENTE = tokenPermanente;
                    db.PERSONA.Attach(persona);
                    var entry = db.Entry(persona);
                    entry.Property(e => e.FACEBOOK_TOKEN_SESSIONE).IsModified = true;
                    entry.Property(e => e.FACEBOOK_TOKEN_PERMANENTE).IsModified = true;
                    db.SaveChanges();
                }
            }
            else
            {
                UtenteLoginVeloceViewModel loginVeloce = new UtenteLoginVeloceViewModel();
                loginVeloce.Email = email;
                loginVeloce.Password = Utils.RandomString(20);
                loginVeloce.Nome = paramsFB.first_name;
                loginVeloce.Cognome = paramsFB.last_name;
                loginVeloce.FacebookToken = accessToken;
                loginVeloce.FacebookTokenPermanente = tokenPermanente;
                loginVeloce.RicordaLogin = false;
                loginVeloce.SalvaRegistrazione(ControllerContext, db);
                persona = db.PERSONA.SingleOrDefault(m => m.PERSONA_EMAIL.FirstOrDefault(m2 => 
                    m2.EMAIL == email && m2.TIPO == (int)TipoEmail.Registrazione) != null
                );
            }
            return persona.ID;
        }

        public int CountOfferteNonConfermate(DatabaseContext db)
        {
            PersonaModel utente = Session["utente"] as PersonaModel;
            return db.OFFERTA.Where(item => item.ID_PERSONA == utente.Persona.ID && item.STATO == (int)StatoOfferta.ATTIVA).Count();
        }

        // su tutte le vendite 
        public int CountOfferteRicevute(DatabaseContext db)
        {
            PersonaModel utente = Session["utente"] as PersonaModel;
            return db.OFFERTA.Where(item => item.ANNUNCIO.ID_PERSONA == utente.Persona.ID && item.LETTA == 0 && item.STATO == (int)StatoOfferta.ATTIVA).Count();
        }

        // sulla vendita singola
        public int CountOfferteRicevute(DatabaseContext db, int vendita)
        {
            PersonaModel utente = Session["utente"] as PersonaModel;
            return db.OFFERTA.Where(item => item.ID_ANNUNCIO == vendita && item.ID_PERSONA == utente.Persona.ID && item.STATO == (int)StatoOfferta.ATTIVA).Count();
        }

        public void RefreshUtente(DatabaseContext db)
        {
            PersonaModel utente = Session["utente"] as PersonaModel;
            utente.Persona = db.PERSONA.Where(u => u.ID == utente.Persona.ID).SingleOrDefault();
            Session["utente"] = utente;
            //setSessioneUtente(Session, db, utente.Persona.ID, FormsAuthentication.)
        }

        public void RefreshPunteggioUtente(DatabaseContext db)
        {
            // VERIFICARLA
            if (Session != null)
            {
                PersonaModel utente = Session["utente"] as PersonaModel;
                utente.ContoCorrente = db.CONTO_CORRENTE_MONETA.Where(m => m.ID_CONTO_CORRENTE == utente.Persona.ID_CONTO_CORRENTE).ToList();
                utente.NumeroMessaggiDaLeggere = db.CHAT.Count(m => m.ID_DESTINATARIO == utente.Persona.ID && m.STATO == (int)StatoChat.INVIATO);
                utente.NumeroNotificheDaLeggere = db.NOTIFICA.Count(m => m.ID_PERSONA_DESTINATARIO == utente.Persona.ID && m.STATO == (int)StatoNotifica.ATTIVA);
                Session["utente"] = utente;
            }
        }

        [AllowAnonymous]
        public string GetCurrentDomain()
        {
            return Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host +
                (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
        }

        // VERIFICARE CHE L'ASSEGNAZIONE DELLA MONETA VADA A BUON FINE E CHE QUINDI LA TRANSAZIONE
        // ABBIA EFFETTO
        public void AddBonus(DatabaseContext db, PERSONA persona, Guid tokenPortale, int punti, TipoTransazione tipo, string nomeTransazione, int? idAnnuncio = null)
        {
            TRANSAZIONE model = new TRANSAZIONE();
            model.ID_CONTO_MITTENTE = db.ATTIVITA.Where(p => p.TOKEN == tokenPortale).SingleOrDefault().ID_CONTO_CORRENTE;
            model.ID_CONTO_DESTINATARIO = persona.ID_CONTO_CORRENTE;
            model.TIPO = (int)tipo;
            model.NOME = nomeTransazione;
            model.PUNTI = punti;
            model.DATA_INSERIMENTO = DateTime.Now;
            model.STATO = (int)StatoPagamento.ACCETTATO;
            db.TRANSAZIONE.Add(model);
            db.SaveChanges();

            if (idAnnuncio != null) { 
                TRANSAZIONE_ANNUNCIO transazioneAnnuncio = new Models.TRANSAZIONE_ANNUNCIO();
                transazioneAnnuncio.ID_TRANSAZIONE = model.ID;
                transazioneAnnuncio.ID_ANNUNCIO = (int)idAnnuncio;
                transazioneAnnuncio.PUNTI = punti;
                transazioneAnnuncio.SOLDI = 0;
                transazioneAnnuncio.DATA_INSERIMENTO = DateTime.Now;
                transazioneAnnuncio.STATO = (int)StatoPagamento.ACCETTATO;
                db.TRANSAZIONE_ANNUNCIO.Add(transazioneAnnuncio);
                db.SaveChanges();
            }

            // genero la moneta ogni volta che offro un bonus, in modo da mantenere la concorrenza dei dati
            for (int i = 0; i < punti; i++)
            {
                MONETA moneta = db.MONETA.Create();
                moneta.VALORE = 1;
                moneta.TOKEN = Guid.NewGuid();
                moneta.DATA_INSERIMENTO = DateTime.Now;
                moneta.STATO = (int)Stato.ATTIVO;
                db.MONETA.Add(moneta);
                db.SaveChanges();
                CONTO_CORRENTE_MONETA conto = new CONTO_CORRENTE_MONETA();
                conto.ID_CONTO_CORRENTE = persona.ID_CONTO_CORRENTE;
                conto.ID_MONETA = moneta.ID;
                conto.ID_TRANSAZIONE = model.ID;
                conto.DATA_INSERIMENTO = DateTime.Now;
                conto.STATO = (int)StatoMoneta.ASSEGNATA;
                db.CONTO_CORRENTE_MONETA.Add(conto);
                db.SaveChanges();
            }

            SendNotifica(persona, TipoNotifica.Bonus, "bonusRicevuto", model, db);
            TempData["BONUS"] = string.Format(Bonus.YouWin, punti, Language.Moneta);

            if (tipo != TipoTransazione.BonusLogin)
                RefreshPunteggioUtente(db);
        }
        
        // RAZOR
        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                         viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public bool SendNotifica(PERSONA destinatario, TipoNotifica messaggio, string view, object datiNotifica, DatabaseContext db = null)
        {
            bool nuovaConnessione = true;
            try
            {
                if (db != null && db.Database.Connection.State == System.Data.ConnectionState.Open)
                    nuovaConnessione = false;

                PersonaModel utente = Session["utente"] as PersonaModel;

                NOTIFICA notifica = new NOTIFICA();
                notifica.ID_PERSONA = utente.Persona.ID;
                notifica.ID_PERSONA_DESTINATARIO = destinatario.ID;
                notifica.MESSAGGIO = (int)messaggio;
                notifica.STATO = (int)StatoNotifica.ATTIVA;
                notifica.DATA_INSERIMENTO = DateTime.Now;
                if (nuovaConnessione)
                    db = new DatabaseContext();

                db.NOTIFICA.Add(notifica);
                return db.SaveChanges() > 0;
            }
            catch(Exception eccezione)
            {

            }
            finally
            {
                if (nuovaConnessione && db != null)
                    db.Database.Connection.Close();

                try
                {
                    ControllerContext controller = new ControllerContext();
                    string indirizzoEmail = destinatario.PERSONA_EMAIL.SingleOrDefault(e => e.TIPO == (int)TipoEmail.Registrazione).EMAIL;
                    // modificare oggetto recuperando dal tipo notifica la stringa
                    string oggetto = Components.EnumHelper<TipoNotifica>.GetDisplayValue(messaggio);
                    SendEmail(indirizzoEmail, oggetto, controller, view, datiNotifica);
                    SendChat("",oggetto);
                }
                catch
                {

                }
            }
            return false;
        }

        #endregion

        #region METODI PROTETTI
        // MVC2 .ascx
        protected string RenderViewToString<T>(string viewPath, T model)
        {
            ViewData.Model = model;
            using (var writer = new System.IO.StringWriter())
            {
                var view = new WebFormView(ControllerContext, viewPath);
                var vdd = new ViewDataDictionary<T>(model);
                var viewCxt = new ViewContext(ControllerContext, view, vdd,
                                            new TempDataDictionary(), writer);
                viewCxt.View.Render(viewCxt, writer);
                return writer.ToString();
            }
        }

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

        protected async System.Threading.Tasks.Task SendCodeTelegram(string numeroTelefono)
        {
            var client = new TLSharp.Core.TelegramClient(106191, "2a5c0da8fcb3acfdf501c3f17fb0fa5f");
            await client.ConnectAsync();
            var hash = await client.SendCodeRequestAsync(numeroTelefono);
        }

        protected async System.Threading.Tasks.Task SendMessaggioTelegram(string numeroTelefono, string code, string messaggio)
        {
            var client = new TLSharp.Core.TelegramClient(106191, "2a5c0da8fcb3acfdf501c3f17fb0fa5f");
            await client.ConnectAsync();
            var hash = await client.SendCodeRequestAsync(numeroTelefono);

            var user = await client.MakeAuthAsync(numeroTelefono, hash, code);
            //get available contacts
            var result = await client.GetContactsAsync();

            //find recipient in contacts
            var customer = result.Users
                .Where(x => x.GetType() == typeof(TeleSharp.TL.TLUser))
                .Cast<TeleSharp.TL.TLUser>()
                .FirstOrDefault(x => x.Phone == numeroTelefono);
            /*if (user.ToList().Count != 0)
            {
                foreach (var u in user)
                {
                    if (u.phone.Contains("3965604"))
                    {
                        //send message
                        await client.SendMessageAsync(new TLInputPeerUser() { user_id = u.id }, textBox3.Text);
                    }
                }
            }*/
            // @channelname or chat_id, messaggio da inviare
            await client.SendMessageAsync(new TeleSharp.TL.TLInputPeerUser() { UserId = customer.Id }, messaggio);
        }

        protected string SendCodeWhatsApp()
        {
            string messaggio = "";
            string password;
            if (WhatsAppApi.Register.WhatsRegisterV2.RequestCode("+39 3492240520", out password, "sms"))
            {
                if (!string.IsNullOrEmpty(password))
                {

                }
                else
                {
                    
                }
            }
            else
            {
                messaggio = "Generazione codice fallita";
            }
            return messaggio;
        }

        protected void SendMessaggioWhatsApp(string messaggio)
        {
            WhatsAppApi.WhatsApp wa = new WhatsAppApi.WhatsApp("+39 3492240520", "", "GratisForGratis", false, false);
            wa.OnConnectSuccess += () =>
            {
                Response.Write("connect");
                wa.OnLoginSuccess += (phno, data) =>
                {
                    wa.SendMessage("+39 3492240520", messaggio);
                };

                wa.OnLoginFailed += (data) =>
                {
                    Response.Write("login failed" + data);
                };
                wa.Login();
            };
            wa.OnConnectFailed += (ex) =>
            {
                Response.Write("connection failed");
            };
        }

        #endregion

        protected FileUploadifive UploadImmagine(string path, HttpPostedFileBase file)
        {
            FileUploadifive fileUpload = new FileUploadifive();
            if (file != null && file.ContentLength > 0)
            {
                FileInfo informazioniFile = new FileInfo(Path.GetFileName(file.FileName));
                string estensione = informazioniFile.Extension;
                fileUpload.NomeOriginale = informazioniFile.Name;
                fileUpload.Nome = System.Guid.NewGuid().ToString() + estensione;

                fileUpload.PathOriginale = Request.Url.GetLeftPart(UriPartial.Authority) + path + "/Original";
                fileUpload.PathMedia = Request.Url.GetLeftPart(UriPartial.Authority) + path + "/Normal";
                fileUpload.PathPiccola = Request.Url.GetLeftPart(UriPartial.Authority) + path + "/Little";
                string directoryOriginale = Server.MapPath("~" + path + "/Original");
                string directoryMedia = Server.MapPath("~" + path + "/Normal");
                string directoryPiccola = Server.MapPath("~" + path + "/Little");

                Directory.CreateDirectory(directoryOriginale);
                Directory.CreateDirectory(directoryMedia);
                Directory.CreateDirectory(directoryPiccola);

                file.SaveAs(Path.Combine(directoryOriginale, fileUpload.Nome));
                using (Image img = Image.FromFile(Path.Combine(directoryOriginale, fileUpload.Nome), true))
                {
                    int widthMedia = 300;
                    int heightMedia = 300;
                    int widthPiccola = 100;
                    int heightPiccola = 100;
                    // se orizzontale setto l'altezza altrimenti la larghezza
                    if (img.Width > img.Height)
                    {
                        //setto altezza img media
                        decimal ratioMedia = (decimal)widthMedia / img.Width;
                        decimal tempMedia = img.Height * ratioMedia;
                        heightMedia = (int)tempMedia;
                        //setto altezza img piccola
                        decimal ratioPiccola = (decimal)widthPiccola / img.Width;
                        decimal tempPiccola = img.Height * ratioPiccola;
                        heightPiccola = (int)tempPiccola;
                    }
                    else
                    {
                        //setto larghezza img media
                        decimal ratioMedia = (decimal)heightMedia / img.Height;
                        decimal tempMedia = img.Width * ratioMedia;
                        widthMedia = (int)tempMedia;
                        //setto larghezza img piccola
                        decimal ratioPiccola = (decimal)heightPiccola / img.Height;
                        decimal tempPiccola = img.Width * ratioPiccola;
                        widthPiccola = (int)tempPiccola;
                    }
                    using (Image imgMedia = new Bitmap(img, widthMedia, heightMedia))
                    {
                        imgMedia.Save(Path.Combine(directoryMedia, fileUpload.Nome));
                    }

                    using (Image imgPiccola = new Bitmap(img, widthPiccola, heightPiccola))
                    {
                        imgPiccola.Save(Path.Combine(directoryPiccola, fileUpload.Nome));
                    }
                }
                return fileUpload;
            }
            return null;
        }

        protected FileUploadifive UploadFile(HttpPostedFileBase file, TipoUpload tipo, string tokenUtente)
        {
            FileUploadifive fileUpload = new FileUploadifive();
            if (file != null && file.ContentLength > 0)
            {
                string estensione = new FileInfo(Path.GetFileName(file.FileName)).Extension;
                fileUpload.NomeOriginale = file.FileName;
                fileUpload.Nome = System.Guid.NewGuid().ToString() + estensione;

                fileUpload.PathOriginale = Request.Url.GetLeftPart(UriPartial.Authority) + "/Uploads/Text/" + tokenUtente + "/" + DateTime.Now.Year.ToString() + "/Original/";
                string directoryOriginale = Server.MapPath("~/Uploads/Text/" + tokenUtente + "/" + DateTime.Now.Year.ToString() + "/Original/");
                
                Directory.CreateDirectory(directoryOriginale);
                
                file.SaveAs(Path.Combine(directoryOriginale, fileUpload.Nome));
                return fileUpload;
            }
            return null;
        }
        #endregion

        #region METODI PRIVATI
        private void AddPuntiLogin(DatabaseContext db, PERSONA utente)
        {
            int puntiAccesso = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["bonusAccesso"]);
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