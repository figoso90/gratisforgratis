using Facebook;
using GratisForGratis.App_GlobalResources;
using GratisForGratis.Filters;
using GratisForGratis.Models;
using SimpleCrypto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace GratisForGratis.Controllers
{
    public class UtenteController : AdvancedController
    {
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Index()
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                base.RefreshPunteggioUtente(db);
            }
            return base.View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Profilo(string token)
        {
            UtenteProfiloViewModel viewModel = new UtenteProfiloViewModel();
            using (DatabaseContext db = new DatabaseContext())
            {
                db.Database.Connection.Open();
                PERSONA persona = db.PERSONA.FirstOrDefault(u => u.TOKEN.ToString() == token);
                viewModel.Token = token;
                viewModel.Email = persona.PERSONA_EMAIL.SingleOrDefault(item => item.TIPO == (int)TipoEmail.Registrazione).EMAIL;
                viewModel.Nome = persona.NOME;
                viewModel.Cognome = persona.COGNOME;
                PERSONA_TELEFONO modelTelefono = persona.PERSONA_TELEFONO.SingleOrDefault(item => 
                    item.TIPO == (int)TipoTelefono.Privato);
                if (modelTelefono != null)
                    viewModel.Telefono = modelTelefono.TELEFONO;
                PERSONA_INDIRIZZO modelIndirizzo = persona.PERSONA_INDIRIZZO.SingleOrDefault(item =>
                    item.TIPO == (int)TipoIndirizzo.Residenza);

                if (modelIndirizzo != null && modelIndirizzo.INDIRIZZO != null)
                {
                    viewModel.Citta = modelIndirizzo.INDIRIZZO.COMUNE.NOME;
                    viewModel.Indirizzo = modelIndirizzo.INDIRIZZO.INDIRIZZO1;
                    viewModel.Civico = modelIndirizzo.INDIRIZZO.CIVICO;
                }
                // caricamento indirizzo di spedizione
                PERSONA_INDIRIZZO modelIndirizzoSpedizione = persona.PERSONA_INDIRIZZO.SingleOrDefault(item =>
                    item.TIPO == (int)TipoIndirizzo.Spedizione);

                if (modelIndirizzoSpedizione != null && modelIndirizzoSpedizione.INDIRIZZO != null)
                {
                    viewModel.CittaSpedizione = modelIndirizzoSpedizione.INDIRIZZO.COMUNE.NOME;
                    viewModel.IndirizzoSpedizione = modelIndirizzoSpedizione.INDIRIZZO.INDIRIZZO1;
                    viewModel.CivicoSpedizione = modelIndirizzoSpedizione.INDIRIZZO.CIVICO;
                }

                viewModel.Foto = persona.PERSONA_FOTO.Select(m => new FotoModel(m.ALLEGATO)).ToList();

                viewModel.listaAcquisti = new List<AnnuncioViewModel>();
                viewModel.listaDesideri = new List<AnnuncioViewModel>();
                // far vedere top n acquisti con link
                var query = db.ANNUNCIO.Where(item => item.ID_COMPRATORE != persona.ID && item.TRANSAZIONE_ANNUNCIO.Count(m => m.STATO == (int)StatoPagamento.ATTIVO || m.STATO == (int)StatoPagamento.ACCETTATO) > 0
                        && (item.STATO == (int)StatoVendita.VENDUTO || item.STATO == (int)StatoVendita.ELIMINATO || item.STATO == (int)StatoVendita.BARATTATO)
                        && (item.ID_OGGETTO != null || item.ID_SERVIZIO != null));
                List<ANNUNCIO> lista = query
                    .OrderByDescending(item => item.DATA_INSERIMENTO)
                    .Take(4).ToList();
                foreach (ANNUNCIO m in lista)
                {
                    AnnuncioModel annuncioModel = new AnnuncioModel();
                    viewModel.listaAcquisti.Add(annuncioModel.GetViewModel(db, m));
                }
                // far vedere top n desideri con link
                List<ANNUNCIO_DESIDERATO> listaDesideri = db.ANNUNCIO_DESIDERATO
                    .Where(item => item.ID_PERSONA == persona.ID && (item.ANNUNCIO.STATO == (int)StatoVendita.INATTIVO
                        || item.ANNUNCIO.STATO == (int)StatoVendita.ATTIVO) && (item.ANNUNCIO.DATA_FINE == null ||
                        item.ANNUNCIO.DATA_FINE >= DateTime.Now))
                    .OrderByDescending(item => item.ANNUNCIO.DATA_INSERIMENTO)
                    .Take(4)
                    .ToList();
                listaDesideri.ForEach(m => 
                    viewModel.listaDesideri.Add(
                        new AnnuncioViewModel(db, m.ANNUNCIO)
                    )
                );
            }
            return base.View(viewModel);
        }

        #region LOGIN
        [AllowAnonymous]
        [OnlyAnonymous]
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Login(string ReturnUrl)
        {
            ViewBag.Title = Language.TitleAccess;
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [AllowAnonymous]
        [OnlyAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        // recupera login di portaleweb
        public ActionResult Login(UtenteLoginVeloceViewModel viewModel)
        {
            ViewBag.Title = Language.TitleAccess;
            ViewBag.ReturnUrl = viewModel.ReturnUrl;
            if (base.ModelState.IsValid)
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    using (DbContextTransaction transazione = db.Database.BeginTransaction())
                    {
                        try
                        {
                            PBKDF2 crypto = new PBKDF2();
                            PERSONA_EMAIL model = db.PERSONA_EMAIL.SingleOrDefault(
                                    item =>
                                    item.EMAIL == viewModel.Email
                                    && item.TIPO == (int)TipoEmail.Registrazione);
                            if (model == null)
                            {
                                if (viewModel.SalvaRegistrazione(ControllerContext, db))
                                {
                                    base.TempData["salvato"] = true;
                                    // recupero nuovamente l'utente
                                    model = db.PERSONA_EMAIL.SingleOrDefault(
                                        item =>
                                        item.EMAIL == viewModel.Email
                                        && item.TIPO == (int)TipoEmail.Registrazione);
                                    setSessioneUtente(base.Session, db, model.ID_PERSONA, viewModel.RicordaLogin, ControllerContext);
                                    transazione.Commit();
                                    // sistemare il return, perchè va in conflitto con il allowonlyanonymous
                                    return Redirect((string.IsNullOrWhiteSpace(viewModel.ReturnUrl)) ? FormsAuthentication.DefaultUrl : viewModel.ReturnUrl);
                                }
                                else
                                {
                                    transazione.Rollback();
                                    ModelState.AddModelError("Error", Language.EmailNotExist);
                                }
                            }
                            else if (!model.PERSONA.PASSWORD.Equals(crypto.Compute(viewModel.Password, model.PERSONA.TOKEN_PASSWORD)))
                            {
                                ModelState.AddModelError("Error", Language.ErrorPassword);
                            }
                            else
                            {
                                setSessioneUtente(base.Session, db, model.ID_PERSONA, viewModel.RicordaLogin, ControllerContext);
                                transazione.Commit();
                                // sistemare il return, perchè va in conflitto con il allowonlyanonymous
                                return Redirect((string.IsNullOrWhiteSpace(viewModel.ReturnUrl)) ? FormsAuthentication.DefaultUrl : viewModel.ReturnUrl);
                            }

                        }
                        catch (Exception exception)
                        {
                            transazione.Rollback();
                            Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                        }
                    }
                }
            }

            base.ModelState.AddModelError("Errore", Language.ErrorRegister);
            return View(viewModel);
        }

        [AllowAnonymous]
        [OnlyAnonymous]
        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult LoginForPay(string returnUrl)
        {
            ViewBag.Title = Language.TitleAccess;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [OnlyAnonymous]
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult LoginFacebook(string ReturnUrl)
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FacebookApiId"],
                client_secret = ConfigurationManager.AppSettings["FacebookApiSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email" // Add other permissions as needed
            });

            return Redirect(loginUrl.AbsoluteUri);
        }

        [AllowAnonymous]
        [OnlyAnonymous]
        public ActionResult FacebookCallback(string code)
        {
            try
            {
                // RECUPERA ACCESS TOKEN BREVE
                var fb = new FacebookClient();
                string scope = "user_friends, email, manage_pages, pages_show_list, publish_pages, public_profile, publish_actions";
                dynamic result = fb.Post("oauth/access_token", new
                {
                    client_id = ConfigurationManager.AppSettings["FacebookApiId"],
                    client_secret = ConfigurationManager.AppSettings["FacebookApiSecret"],
                    redirect_uri = RedirectUri.AbsoluteUri,
                    code = code,
                    scope = scope
                });

                var accessToken = result.access_token;

                // Store the access token in the session
                Session["AccessToken"] = accessToken;

                // update the facebook client with the access token so 
                // we can make requests on behalf of the user
                fb.AccessToken = accessToken;

                // Get the user's information
                dynamic me = fb.Get("me?fields=first_name,last_name,id,email");

                // Set the auth cookie
                string email = me.email;
                FormsAuthentication.SetAuthCookie(email, false);

                // RECUPERA ACCESS TOKEN MEDIO
                //FacebookClient app = new FacebookClient(accessToken);

                //dynamic result2 = app.Post("https://graph.facebook.com/oauth/access_token", new
                //{
                //    grant_type = "fb_exchange_token",
                //    client_id = ConfigurationManager.AppSettings["FacebookApiId"],
                //    client_secret = ConfigurationManager.AppSettings["FacebookApiSecret"],
                //    fb_exchange_token = accessToken
                //});

                //// RECUPERA ACCESS TOKEN PERMANENTE
                //JsonObject user = (JsonObject)app.Get("https://graph.facebook.com/me/accounts", new
                //{
                //    access_token = result2[0]
                //});

                //JsonArray listaPagine = (JsonArray)user[0];
                //string tokenPermanenteG4G = null;
                //listaPagine.ForEach(m =>
                //{
                //    var test = (JsonObject)m;
                //    if (test["id"].ToString() == ConfigurationManager.AppSettings["FBPageIDG4G"])
                //    {
                //        // TOKEN PERMANENTE PAGINA GRATISFORGRATIS
                //        tokenPermanenteG4G = test["access_token"].ToString();
                //    }
                //});

                using (DatabaseContext db = new DatabaseContext())
                {
                    int id = this.AddUtenteFacebook(accessToken, accessToken, me, db);
                    this.setSessioneUtente(base.Session, db, id, false, ControllerContext);
                }
                //string tok = ConfigurationManager.AppSettings["FBTokenPermanenteG4G"];
                
                //FacebookClient app2 = new FacebookClient(tokenPermanenteG4G);
                //dynamic result3 = app2.Post("/v3.2/" + ConfigurationManager.AppSettings["FBPageIDG4G"] + "/feed", 
                //    new Dictionary<string, object> {
                //        {
                //            "message", "This Post was made from my website"
                //        }
                //    });
                //return Redirect((string.IsNullOrWhiteSpace(RedirectUri.AbsoluteUri)) ? FormsAuthentication.DefaultUrl : RedirectUri.AbsoluteUri);
                return RedirectToAction("Index");
            }
            catch (FacebookOAuthException eccezione)
            {
                TempData["eccezione"] = eccezione;
                return Redirect("Login");
            }
        }

        // NON USATO
        [HttpGet]
        public ActionResult FBTokenPermanente(string accessToken)
        {
            var fb = new FacebookClient();

            // generate medium lived token (2 mesi)
            FacebookClient app = new FacebookClient(accessToken);
            dynamic result2 = app.Post("https://graph.facebook.com/oauth/access_token", new
            {
                grant_type = "fb_exchange_token",
                client_id = ConfigurationManager.AppSettings["FacebookApiId"],
                client_secret = ConfigurationManager.AppSettings["FacebookApiSecret"],
                fb_exchange_token = accessToken
            });

            // RECUPERA ACCESS TOKEN PERMANENTE
            JsonObject user = (JsonObject)app.Get("https://graph.facebook.com/me/accounts", new
            {
                access_token = result2[0]
            });

            JsonArray listaPagine = (JsonArray)user[0];
            string token = null;
            listaPagine.ForEach(m =>
            {
                var test = (JsonObject)m;
                //if (test["id"].ToString() == ConfigurationManager.AppSettings["FBPageIDG4G"])
                //{
                //    // TOKEN PERMANENTE PAGINA GRATISFORGRATIS
                //    token = test["access_token"].ToString();
                //}
                token += "<p><b>Pagina " + test["name"].ToString() + ":</b> " + test["access_token"].ToString() + "</p>";
            });
            
            TempData["token"] = token;
            return View();
        }
        #endregion

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult CambioPassword()
        {
            PersonaModel utente = base.Session["utente"] as PersonaModel;
            UtenteCambioPasswordViewModel model = new UtenteCambioPasswordViewModel()
            {
                Password = utente.Persona.PASSWORD,
                ConfermaPassword = utente.Persona.PASSWORD
            };
            return base.View(model);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult CambioPassword(UtenteCambioPasswordViewModel model)
        {
            if (base.ModelState.IsValid)
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    PersonaModel utente = base.Session["utente"] as PersonaModel;
                    PBKDF2 crypto = new PBKDF2();
                    utente.Persona.PASSWORD = crypto.Compute(model.Password, utente.Persona.TOKEN_PASSWORD);
                    db.Entry<PERSONA>(utente.Persona).State = EntityState.Modified;
                    if (db.SaveChanges() > 0)
                    {
                        base.Session["utente"] = utente;
                        base.TempData["salvato"] = true;
                    }
                }
            }
            return base.View(model);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Impostazioni()
        {
            PersonaModel utente = base.Session["utente"] as PersonaModel;
            UtenteImpostazioniViewModel model = new UtenteImpostazioniViewModel();
            model.Load(utente);
            return base.View(model);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Impostazioni(UtenteImpostazioniViewModel model)
        {

            if (base.ModelState.IsValid)
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    db.Database.Connection.Open();
                    using (DbContextTransaction transazione = db.Database.BeginTransaction())
                    {
                        PersonaModel utente = base.Session["utente"] as PersonaModel;
                        try
                        {
                            if (db.PERSONA_EMAIL.Count(m => m.ID_PERSONA != utente.Persona.ID && 
                                m.EMAIL == model.Email) > 0)
                            {
                                base.ModelState.AddModelError("Errore", ErrorResource.RegisterExist);
                                base.TempData["salvato"] = false;
                                return base.View(model);
                            }
                            utente.SetEmail(db, model.Email);
                            if (db.PERSONA_TELEFONO.Count(m => m.ID_PERSONA != utente.Persona.ID && 
                                m.TELEFONO == model.Telefono) > 0)
                            {
                                base.ModelState.AddModelError("Errore", ErrorResource.RegisterExist);
                                base.TempData["salvato"] = false;
                                return base.View(model);
                            }
                            utente.SetTelefono(db, model.Telefono);
                            utente.SetIndirizzo(db, model.Citta, model.IDCitta, model.Indirizzo, model.Civico, (int)TipoIndirizzo.Residenza);
                            utente.SetIndirizzo(db, model.Citta, model.IDCittaSpedizione, model.IndirizzoSpedizione, model.CivicoSpedizione, (int)TipoIndirizzo.Spedizione);

                            bool primaVolta = utente.Persona.STATO == (int)Stato.INATTIVO;
                            bool personaModificata = false;

                            if (primaVolta)
                            {
                                utente.Persona.STATO = (int)Stato.ATTIVO;
                                personaModificata = true;
                            }

                            if (utente.Persona.NOME != model.Nome || utente.Persona.COGNOME != model.Cognome)
                            {
                                utente.Persona.NOME = model.Nome;
                                utente.Persona.COGNOME = model.Cognome;
                                personaModificata = true;
                            }

                            if (personaModificata)
                            {
                                utente.Persona.DATA_MODIFICA = DateTime.Now;
                                db.Entry(utente.Persona).State = EntityState.Modified;
                                if (db.SaveChanges() <= 0)
                                    throw new Exception(Language.ImpostazioniErroreSalvaUtente);
                            }

                            if (primaVolta)
                            {
                                // crediti omaggio registrazione completata
                                if (db.TRANSAZIONE.Count(item => item.ID_CONTO_DESTINATARIO == utente.Persona.ID_CONTO_CORRENTE && item.TIPO == (int)TipoTransazione.BonusIscrizione) <= 0)
                                {
                                    Guid tokenPortale = Guid.Parse(ConfigurationManager.AppSettings["portaleweb"]);
                                    int punti = Convert.ToInt32(ConfigurationManager.AppSettings["bonusIscrizione"]);
                                    this.AddBonus(db, ControllerContext, utente.Persona, tokenPortale, punti, TipoTransazione.BonusIscrizione, Bonus.Registration);
                                    this.RefreshPunteggioUtente(db);
                                }

                                // attivo automaticamente annunci già pubblicati
                                db.ANNUNCIO.Where(m => m.ID_PERSONA == utente.Persona.ID && m.STATO == (int)StatoVendita.INATTIVO).ToList().ForEach(m =>
                                {
                                    m.STATO = (int)StatoVendita.ATTIVO;
                                    db.Entry(m).State = EntityState.Modified;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        // non blocco l'attivazione dell'account, abiliterà gli annunci manualmente
                                        Exception eccezione = new Exception(Language.ImpostazioniErroreAttivaAnnunci);
                                        ModelState.AddModelError("", eccezione.Message);
                                        Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
                                    }
                                });
                            }
                            transazione.Commit();
                            //base.Session["utente"] = utente;
                            base.TempData["salvato"] = true;
                        }
                        catch (Exception eccezione)
                        {
                            transazione.Rollback();
                            ModelState.AddModelError("", eccezione.Message);
                            Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
                        }
                        finally
                        {
                            this.RefreshUtente(db);
                            utente = base.Session["utente"] as PersonaModel;
                            model.HasLoginFacebook = utente.Persona.FACEBOOK_TOKEN_PERMANENTE != null;
                            model.Load(utente);
                        }
                    }
                }
            }
            base.TempData["salvato"] = false;
            return base.View(model);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Privacy()
        {
            PersonaModel utente = base.Session["utente"] as PersonaModel;
            UtentePrivacyViewModel model = new UtentePrivacyViewModel(utente.Persona.PERSONA_PRIVACY.FirstOrDefault());
            return base.View(model);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Privacy(UtentePrivacyViewModel viewModel)
        {
            if (base.ModelState.IsValid)
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    try
                    {
                        db.Database.Connection.Open();
                        PersonaModel utente = base.Session["utente"] as PersonaModel;
                        PERSONA_PRIVACY model = utente.Persona.PERSONA_PRIVACY.FirstOrDefault();
                        model.ACCETTA_CONDIZIONE = viewModel.AccettaCondizioni;
                        model.COMUNICAZIONI = viewModel.Comunicazioni;
                        model.NOTIFICA_EMAIL = viewModel.NotificaEmail;
                        model.NOTIFICA_CHAT = viewModel.NotificaChat;
                        model.CHAT = (int?)viewModel.Chat;
                        model.DATA_MODIFICA = DateTime.Now;
                        db.Entry(model).State = EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            RefreshUtente(db);
                            base.TempData["salvato"] = true;
                            return base.View(viewModel);
                        }
                    }
                    catch (Exception eccezione)
                    {
                        ModelState.AddModelError("", eccezione.Message);
                        Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
                    }
                }
            }
            base.TempData["salvato"] = false;
            return base.View(viewModel);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            base.Session.Clear();
            base.Session.Abandon();
            return base.RedirectToAction("Login");
        }

        [AllowAnonymous]
        [OnlyAnonymous]
        [HttpGet]
        public ActionResult PasswordDimenticata()
        {
            ViewBag.Title = Language.TitleForgotPassword;
            return base.View();
        }

        [AllowAnonymous]
        [OnlyAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PasswordDimenticata(UtentePasswordDimenticataViewModel model)
        {
            ViewBag.Title = Language.TitleForgotPassword;
            if (base.ModelState.IsValid)
            {
                model.NuovaPassword = Utils.RandomString(10);
                using (DatabaseContext db = new DatabaseContext())
                {
                    using (DbContextTransaction transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            PERSONA_EMAIL utenteEmail = db.PERSONA_EMAIL
                                .Where(item => item.EMAIL.Equals(model.Email) &&
                                    item.TIPO == (int)TipoEmail.Registrazione &&
                                    item.PERSONA.STATO == (int)Stato.ATTIVO)
                                .SingleOrDefault();
                            if (utenteEmail!= null)
                            {
                                PBKDF2 crypto = new PBKDF2();
                                //utente.TOKEN_PASSWORD = crypto.GenerateSalt(1, 20);
                                utenteEmail.PERSONA.PASSWORD = crypto.Compute(model.NuovaPassword, utenteEmail.PERSONA.TOKEN_PASSWORD);
                                utenteEmail.PERSONA.DATA_MODIFICA = DateTime.Now;

                                if (db.SaveChanges() > 0)
                                {
                                    // invio email nuova password
                                    EmailModel email = new EmailModel(ControllerContext);
                                    email.To.Add(new System.Net.Mail.MailAddress(model.Email));
                                    email.Subject = Email.ForgotPasswordSubject + " - " + WebConfigurationManager.AppSettings["nomeSito"];
                                    email.Body = "PasswordDimenticata";
                                    email.DatiEmail = model;
                                    email.SendAsync = false;
                                    if (new EmailController().SendEmail(email))
                                    {
                                        transaction.Commit();
                                        base.TempData["salvato"] = true;
                                    }
                                    else
                                    {
                                        transaction.Rollback();
                                        base.ModelState.AddModelError("", Language.ErrorSendForgotPassword);
                                    }
                                }
                                else
                                {
                                    base.ModelState.AddModelError("", Language.ErrorForgotPassword);
                                }
                            }
                            else
                            {
                                base.ModelState.AddModelError("", Language.ErrorForgotPassword);
                            }
                            
                        }
                        catch (Exception exception)
                        {
                            transaction.Rollback();
                            base.ModelState.AddModelError("", exception.Message);
                            Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                        }
                    }
                }
            }
            return base.View(model);
        }

        [HttpGet]
        public ActionResult ReclamoOrdine()
        {
            return base.View();
        }

        [AllowAnonymous]
        [OnlyAnonymous]
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Registrazione()
        {
            return base.View();
        }

        [AllowAnonymous]
        [OnlyAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Registrazione(UtenteRegistrazioneViewModel model)
        {
            if (base.ModelState.IsValid)
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    using (DbContextTransaction transazione = db.Database.BeginTransaction())
                    {
                        try
                        {
                            // verificare esistenza e-mail
                            PERSONA checkPersona = db.PERSONA.SingleOrDefault(
                                    item =>
                                    item.PERSONA_EMAIL.Count(m => m.EMAIL == model.Email && m.TIPO == (int)TipoEmail.Registrazione) > 0 ||
                                    item.PERSONA_TELEFONO.Count(m => m.TELEFONO == model.Telefono && m.TIPO == (int)TipoTelefono.Privato) > 0);
                            if (checkPersona == null)
                            {
                                CONTO_CORRENTE conto = db.CONTO_CORRENTE.Create();
                                conto.ID = Guid.NewGuid();
                                conto.TOKEN = Guid.NewGuid();
                                conto.DATA_INSERIMENTO = DateTime.Now;
                                conto.STATO = (int)Stato.ATTIVO;
                                db.CONTO_CORRENTE.Add(conto);
                                db.SaveChanges();
                                PBKDF2 crypto = new PBKDF2();
                                PERSONA persona = db.PERSONA.Create();
                                persona.TOKEN = Guid.NewGuid();
                                persona.TOKEN_PASSWORD = crypto.GenerateSalt(1, 20);
                                persona.PASSWORD = crypto.Compute(model.Password.Trim(), persona.TOKEN_PASSWORD);
                                persona.NOME = model.Nome.Trim();
                                persona.COGNOME = model.Cognome.Trim();
                                persona.ID_CONTO_CORRENTE = conto.ID;
                                persona.ID_ABBONAMENTO = db.ABBONAMENTO.SingleOrDefault(item => item.NOME == "BASE").ID;
                                persona.DATA_INSERIMENTO = DateTime.Now;
                                persona.STATO = (int)Stato.ATTIVO;
                                db.PERSONA.Add(persona);
                                if (db.SaveChanges() > 0)
                                {
                                    PersonaModel utente = new PersonaModel(persona);
                                    utente.SetEmail(db, model.Email, Stato.INATTIVO);
                                    utente.SetTelefono(db, model.Telefono);
                                    utente.SetIndirizzo(db, model.Citta, model.IDCitta, model.Indirizzo, model.Civico, (int)TipoIndirizzo.Residenza);
                                    utente.SetIndirizzo(db, model.CittaSpedizione, model.IDCittaSpedizione, model.IndirizzoSpedizione, model.CivicoSpedizione, (int)TipoIndirizzo.Spedizione);

                                    utente.SetPrivacy(db, model.AccettaCondizioni);
                                    // crediti omaggio registrazione completata
                                    if (db.TRANSAZIONE.Count(item => item.ID_CONTO_DESTINATARIO == utente.Persona.ID_CONTO_CORRENTE && item.TIPO == (int)TipoTransazione.BonusIscrizione) <= 0)
                                    {
                                        Guid tokenPortale = Guid.Parse(ConfigurationManager.AppSettings["portaleweb"]);
                                        int punti = Convert.ToInt32(ConfigurationManager.AppSettings["bonusIscrizione"]);
                                        this.AddBonus(db, ControllerContext, utente.Persona, tokenPortale, punti, TipoTransazione.BonusIscrizione, Bonus.Registration);
                                    }
                                    // assegna bonus canale pubblicitario
                                    if (Request.Cookies.Get("GXG_promo") != null)
                                    {
                                        string promo = Request.Cookies.Get("GXG_promo").Value;
                                        utente.AddBonusCanalePubblicitario(db, promo);
                                        // reset cookie
                                        HttpCookie currentUserCookie = Request.Cookies["GXG_promo"];
                                        if (currentUserCookie != null)
                                        {
                                            Response.Cookies.Remove("GXG_promo");
                                            currentUserCookie.Expires = DateTime.Now.AddDays(-10);
                                            currentUserCookie.Value = null;
                                            Response.SetCookie(currentUserCookie);
                                        }
                                    }

                                    // invio email registrazione
                                    EmailModel email = new EmailModel(ControllerContext);
                                    email.To.Add(new System.Net.Mail.MailAddress(utente.Email.SingleOrDefault(m => m.TIPO == (int)TipoEmail.Registrazione).EMAIL, persona.NOME + " " + persona.COGNOME));
                                    email.Subject = Email.RegistrationSubject + " - " + WebConfigurationManager.AppSettings["nomeSito"];
                                    email.Body = "RegistrazioneUtente";
                                    email.DatiEmail = new RegistrazioneEmailModel(model)
                                    {
                                        PasswordCodificata = persona.PASSWORD
                                    };
                                    new EmailController().SendEmail(email);

                                    transazione.Commit();

                                    setSessioneUtente(base.Session, db, persona.ID, false, ControllerContext);
                                    // sistemare il return, perchè va in conflitto con il allowonlyanonymous
                                    return Redirect((string.IsNullOrWhiteSpace(model.ReturnUrl)) ? FormsAuthentication.DefaultUrl : model.ReturnUrl);
                                }
                            }
                            else
                            {
                                base.ModelState.AddModelError("Errore", ErrorResource.RegisterExist);
                                return View(model);
                            }
                        }
                        catch (Exception exception)
                        {
                            transazione.Rollback();
                            Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                        }
                    }
                }
            }

            base.ModelState.AddModelError("Errore", Language.ErrorRegister);
            return View(model);
        }

        // usata per il reinvio dalla barra in alto
        [AllowAnonymous]
        [HttpGet]
        public ActionResult ReinvioEmailRegistrazione()
        {
            // invio email registrazione
            EmailModel email = new EmailModel(ControllerContext);
            PersonaModel utente = Session["utente"] as PersonaModel;
            email.To.Add(new System.Net.Mail.MailAddress(utente.Email.SingleOrDefault(e => e.TIPO == (int)TipoEmail.Registrazione).EMAIL));
            email.Subject = Email.RegistrationSubject + " - " + WebConfigurationManager.AppSettings["nomeSito"];
            email.Body = "RegistrazioneUtenteReinvio";
            email.DatiEmail = utente;
            EmailController emailer = new EmailController();
            return Json(emailer.SendEmail(email), JsonRequestBehavior.AllowGet);
        }

        // usata per il reinvio dalla pagina di conferma di registrazione
        /*
        [AllowAnonymous]
        [OnlyAnonymous]
        [HttpGet]
        public ActionResult ReinvioEmailRegistrazione(UtenteRegistrazioneViewModel model)
        {
            // invio email registrazione
            EmailModel email = new EmailModel(ControllerContext);
            email.To.Add(new System.Net.Mail.MailAddress(model.Email));
            email.Subject = Email.RegistrationSubject + " - " + WebConfigurationManager.AppSettings["nomeSito"];
            email.Body = "RegistrazioneUtente";
            email.DatiEmail = model;
            EmailController emailer = new EmailController();
            return Json(emailer.SendEmail(email));
        }
        */
        [HttpGet]
        public ActionResult SaldoPunti(int pagina = 1)
        {
            if (pagina == 0)
                pagina = 1;
            ViewData["Pagina"] = pagina;
            return base.View("SaldoPunti", GetListaBonus(pagina-1));
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Iscrizione(string token)
        {
            TempData["Messaggio"] = Language.ErrorActiveUser1;
            string tokenDecodificato = Utils.DecodeToString(HttpUtility.UrlDecode(token));
            // verifica tramite email+password dell'esistenza dell'utente e attivazione dello stesso
            using (DatabaseContext db = new DatabaseContext())
            {
                using (DbContextTransaction transazione = db.Database.BeginTransaction())
                {
                    try
                    {
                        PERSONA_EMAIL email = db.PERSONA_EMAIL.Where(u => (u.EMAIL + u.PERSONA.PASSWORD) == tokenDecodificato && u.STATO == (int)Stato.INATTIVO).SingleOrDefault();
                        if (email != null)
                        {
                            email.STATO = (int)Stato.ATTIVO;
                            email.DATA_MODIFICA = DateTime.Now;
                            if (db.SaveChanges() > 0)
                            {
                                // salva log bonus iscrizione se non è già presente
                                /*if (db.TRANSAZIONE.Count(item => item.ID_CONTO_DESTINATARIO == persona.PERSONA.ID_CONTO_CORRENTE && item.TIPO == (int)TipoTransazione.BonusIscrizione) <= 0)
                                {
                                    Guid tokenPortale = Guid.Parse(ConfigurationManager.AppSettings["portaleweb"]);
                                    this.AddBonus(db, persona.PERSONA, tokenPortale, punti, TipoTransazione.BonusIscrizione, Bonus.Registration);
                                }*/
                                if (Request.IsAuthenticated)
                                    setSessioneUtente(base.Session, db, email.ID_PERSONA, false, ControllerContext);
                                TempData["Messaggio"] = string.Format(Language.ActivatedUser, ConfigurationManager.AppSettings["bonusIscrizione"] + " " + Language.Moneta, ConfigurationManager.AppSettings["bonusPubblicazioniIniziali"] + " " + Language.Moneta);
                                transazione.Commit();
                                return View();
                            }
                            TempData["Messaggio"] = Language.ErrorActiveUser2;
                            transazione.Rollback();
                        }
                    }
                    catch (Exception eccezione)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
                        transazione.Rollback();
                    }
                }
            }
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Disiscrizione(string token)
        {
            TempData["Messaggio"] = Language.ErrorRemoveUser1;
            string tokenDecodificato = Utils.DecodeToString(HttpUtility.UrlDecode(token));
            using (DatabaseContext db = new DatabaseContext())
            {
                PERSONA persona = db.PERSONA_EMAIL.Where(u => (u.EMAIL + u.PERSONA.PASSWORD) == tokenDecodificato).Select(u => u.PERSONA).SingleOrDefault();
                if (persona != null)
                {
                    persona.STATO = (int)Stato.ELIMINATO;
                    persona.DATA_MODIFICA = DateTime.Now;
                    if (db.SaveChanges() > 0)
                    {
                        TempData["Messaggio"] = Language.RemoveUser;
                        return View();
                    }
                    TempData["Messaggio"] = Language.ErrorRemoveUser2;
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult SavePagamento(string idOfferta)
        {
            bool isPagato = false;
            using (DatabaseContext db = new DatabaseContext())
            {
                if (!string.IsNullOrEmpty(idOfferta))
                {
                }
            }
            return base.View(isPagato);
        }

        [HttpGet]
        public ActionResult RicercheSalvate(int pagina = 1)
        {
            ViewBag.Title = MetaTag.TitleSearchSaved;
            if (pagina == 0)
                pagina = 1;
            ViewData["Pagina"] = pagina;
            using (DatabaseContext db = new DatabaseContext())
            {
                int idUtente = (Session["utente"] as PersonaModel).Persona.ID;
                int numeroElementi = Convert.ToInt32(WebConfigurationManager.AppSettings["numeroElementi"]);

                 var query = db.PERSONA_RICERCA
                    .Where(item => item.ID_PERSONA == idUtente && item.RICERCA.STATO == (int)Stato.ATTIVO);

                ViewData["TotalePagine"] = (int)Math.Ceiling((decimal)query.Count() / (decimal)numeroElementi);

                List<UtenteRicercaViewModel> viewModel = query
                    .OrderByDescending(item => item.RICERCA.DATA_INSERIMENTO)
                    .Skip((pagina-1) * numeroElementi)
                    .Take(numeroElementi)
                    .Select(item => new UtenteRicercaViewModel()
                    {
                        Id = item.ID,
                        Testo = item.RICERCA.NOME,
                        Categoria = item.RICERCA.CATEGORIA.NOME,
                        DataInserimento = item.RICERCA.DATA_INSERIMENTO,
                        DataModifica = (DateTime)item.RICERCA.DATA_MODIFICA,
                        Stato = (Stato)item.RICERCA.STATO
                    })
                    .ToList();
                return View(viewModel);
            }
        }

        [HttpDelete]
        [ValidateAjax]
        public ActionResult RicercheSalvate(List<string> ricerca)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                ricerca.ForEach(item =>
                {
                    int id = Convert.ToInt32(Server.UrlDecode(item));
                    PERSONA_RICERCA model = db.PERSONA_RICERCA.SingleOrDefault(m => m.ID == id);
                    db.PERSONA_RICERCA.Remove(model);
                });
                db.SaveChanges();
            }
            return Json(true);
        }

        #region SERVIZI
        [HttpPost]
        [ValidateAjax]
        public JsonResult UploadImmagineProfilo(HttpPostedFileBase file)
        {
            PersonaModel utente = (Session["utente"] as PersonaModel);
            FileUploadifive fileSalvato = UploadImmagine("/Uploads/Images/" + utente.Persona.TOKEN + "/" + DateTime.Now.Year.ToString(), file);
            FotoModel model = new FotoModel();
            using (DatabaseContext db = new DatabaseContext())
            {
                db.Database.Connection.Open();
                int idAllegato = model.Add(db, fileSalvato.Nome);
                if (idAllegato > 0)
                {
                    // salvo allegato come immagine del profilo
                    utente.SetImmagineProfilo(db, idAllegato);
                    // aggiorna sessione utente
                    bool ricordaLogin = (FormsAuthentication.CookieMode == HttpCookieMode.UseCookies);
                    setSessioneUtente(base.Session, db, utente.Persona.ID, ricordaLogin, ControllerContext);
                    //fileSalvato.Id = idAllegato.ToString();
                    //return Json(new { Success = true, responseText = fileSalvato });
                    string htmlGalleriaFotoProfilo = RenderRazorViewToString("PartialPages/_GalleriaFotoProfilo", Session["utente"] as PersonaModel);
                    return Json(new { Success = true, responseText = htmlGalleriaFotoProfilo });
                }
            }
            //Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            //return null;
            return Json(new { Success = false, responseText = Language.ErrorFormatFile });
        }

        [HttpPost]
        [ValidateAjax]
        public ActionResult DeleteImmagineProfilo(int nome)
        {
            PersonaModel utente = (Session["utente"] as PersonaModel);
            using (DatabaseContext db = new DatabaseContext())
            {
                db.Database.Connection.Open();
                if (nome > 0)
                {
                    // salvo allegato come immagine del profilo
                    utente.RemoveImmagineProfilo(db, nome);
                    // aggiorna sessione utente
                    bool ricordaLogin = (FormsAuthentication.CookieMode == HttpCookieMode.UseCookies);
                    setSessioneUtente(base.Session, db, utente.Persona.ID, ricordaLogin, ControllerContext);
                    //return Json(new { Success = true, responseText = true });
                    return PartialView("PartialPages/_GalleriaFotoProfilo", Session["utente"] as PersonaModel);
                }
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return null;
            //return Json(new { Success = false, responseText = Language.ErrorFormatFile });
        }

        [HttpPost]
        [ValidateAjax]
        public JsonResult AddToCarrello(int idAnnuncio)
        {
            // verifico esistenza annuncio
            // salvo tra gli acquisti ma nello stato inattivo (poi andare a verificare procedura di acquisto immediato)
            // lato client riporto nei cookie l'annuncio nel carrello (ogni )
            return Json(true);
        }

        [HttpDelete]
        [ValidateAjax]
        public JsonResult DeleteFromCarrello(int idAnnuncio)
        {
            // verifico esistenza annuncio
            // salvo tra gli acquisti ma nello stato inattivo (poi andare a verificare procedura di acquisto immediato)
            // lato client riporto nei cookie l'annuncio nel carrello (ogni )
            return Json(true);
        }

        [HttpPost]
        [ValidateAjax]
        public JsonResult SaveCarrello(int idAnnuncio)
        {
            // da fare solo se l'utente... boh
            // Aggiungo nel carrello dell'utente gli annunci in lista
            return Json(true);
        }
        #endregion

        #region METODI PRIVATI

        private bool isValid(string email, string password, ref string errore, ref PERSONA utente)
        {
            bool flag;
            PBKDF2 crypto = new PBKDF2();
            using (DatabaseContext db = new DatabaseContext())
            {
                utente = db.PERSONA_EMAIL.Where(u => u.EMAIL == email && u.PERSONA.STATO == (int)Stato.ATTIVO).Select(u => u.PERSONA).SingleOrDefault();
                //utente = db.PERSONA.FirstOrDefault<PERSONA>((PERSONA u) => u.EMAIL == email && u.STATO == 1);
                if (utente == null)
                {
                    errore = Language.EmailNotExist;
                }
                else if (!utente.PASSWORD.Equals(crypto.Compute(password, utente.TOKEN_PASSWORD)))
                {
                    errore = Language.ErrorPassword;
                }
                else
                {
                    flag = true;
                    return flag;
                }
            }
            flag = false;
            return flag;
        }

        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        private List<TRANSAZIONE> GetListaBonus(int pagina)
        {
            List<TRANSAZIONE> model = new List<TRANSAZIONE>();
            using (DatabaseContext db = new DatabaseContext())
            {
                HttpCookie cookie = new HttpCookie("notifiche");
                int num = base.CountOfferteNonConfermate(db);
                cookie["InConferma"] = num.ToString();
                num = base.CountOfferteRicevute(db);
                cookie["Ricevute"] = num.ToString();
                base.RefreshPunteggioUtente(db);

                Guid utente = (Session["utente"] as PersonaModel).Persona.ID_CONTO_CORRENTE;
                var query = db.TRANSAZIONE.Where(item => item.ID_CONTO_DESTINATARIO == utente && 
                    (item.TIPO == (int)TipoTransazione.BonusIscrizione ||
                    item.TIPO == (int)TipoTransazione.BonusIscrizionePartner ||
                    item.TIPO == (int)TipoTransazione.BonusLogin ||
                    item.TIPO == (int)TipoTransazione.BonusPartner ||
                    item.TIPO == (int)TipoTransazione.BonusPubblicazioneIniziale ||
                    item.TIPO == (int)TipoTransazione.BonusCanalePubblicitario ||
                    item.TIPO == (int)TipoTransazione.BonusAnnuncioCompleto ||
                    item.TIPO == (int)TipoTransazione.BonusSuggerimentoAttivazioneAnnuncio ||
                    item.TIPO == (int)TipoTransazione.BonusSegnalazioneErrore ||
                    item.TIPO == (int)TipoTransazione.BonusInvitaAmicoFB ||
                    item.TIPO == (int)TipoTransazione.BonusAttivaHappyPage)
                    && 
                    item.STATO == (int)StatoPagamento.ACCETTATO);
                int numeroElementi = Convert.ToInt32(WebConfigurationManager.AppSettings["numeroElementi"]);
                ViewData["TotalePagine"] = (int)Math.Ceiling((decimal)query.Count() / (decimal)numeroElementi);

                return query
                    .OrderByDescending(item => item.DATA_INSERIMENTO)
                    .Skip(pagina * numeroElementi)
                    .Take(numeroElementi)
                    .ToList();
            }
        }

        #endregion
    }
}