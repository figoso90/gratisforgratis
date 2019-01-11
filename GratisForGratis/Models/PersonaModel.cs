using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;

namespace GratisForGratis.Models
{
    public class PersonaModel
    {
        #region PROPRIETA

        public int Punti { get; set; }

        public int PuntiSospesi { get; set; }

        public PERSONA Persona { get; set; }

        public string NomeVisibile { get; set; }

        public List<PERSONA_EMAIL> Email { get; set; }

        public List<PERSONA_TELEFONO> Telefono { get; set; }

        public List<PERSONA_INDIRIZZO> Indirizzo { get; set; }

        public List<AttivitaModel> Attivita { get; set; }

        public List<FotoModel> Foto { get; set; }

        //public List<CONTO_CORRENTE_MONETA> ContoCorrente { get; set; }

        public List<CONTO_CORRENTE_CREDITO> Credito { get; set; }

        public List<PERSONA_METODO_PAGAMENTO> MetodoPagamento { get; set; }

        public string PaginaSelezionata { get; set; }

        public int NumeroMessaggi { get; set; }

        public int NumeroMessaggiDaLeggere { get; set; }

        public int NumeroNotifiche { get; set; }

        public int NumeroNotificheDaLeggere { get; set; }

        #endregion

        #region COSTRUTTORI

        public PersonaModel()
        {
            this.SetValoriBase();
        }

        public PersonaModel(DatabaseContext db, int idPersona)
        {
            this.Persona = db.PERSONA
                .Include("ABBONAMENTO")
                .Include("PERSONA_PRIVACY")
                .Include("PERSONA_EMAIL")
                .Include("PERSONA_TELEFONO")
                .Include("PERSONA_METODO_PAGAMENTO")
                .Include("PERSONA_INDIRIZZO")
                .Include("PERSONA_FOTO")
                .Include("PERSONA_ATTIVITA")
                .SingleOrDefault(m => m.ID == idPersona);
            this.SetValoriBase();
            
            this.Email = this.Persona.PERSONA_EMAIL.Where(m => m.ID_PERSONA == idPersona).ToList();
            this.Telefono = this.Persona.PERSONA_TELEFONO.Where(m => m.ID_PERSONA == idPersona).ToList();
            this.MetodoPagamento = this.Persona.PERSONA_METODO_PAGAMENTO.Where(m => m.ID_PERSONA == idPersona).ToList();
            this.Indirizzo = db.PERSONA_INDIRIZZO
                .Include("INDIRIZZO")
                .Include("INDIRIZZO.COMUNE")
                .Include("PERSONA_INDIRIZZO_SPEDIZIONE")
                .Where(m => m.ID_PERSONA == idPersona)
                .ToList();
            this.Foto = this.Persona.PERSONA_FOTO.Where(m => m.ID_PERSONA == idPersona).OrderByDescending(m => m.ORDINE)
                .AsEnumerable().Select(m => new FotoModel(m.ALLEGATO)).ToList();
            this.Persona.PERSONA_ATTIVITA.Where(m => m.ID_PERSONA == idPersona).ToList().ForEach(m => {
                this.Attivita.Add(new AttivitaModel(m));
            });
            this.Credito = db.CONTO_CORRENTE_CREDITO.Where(m => m.ID_CONTO_CORRENTE == this.Persona.ID_CONTO_CORRENTE).ToList();
            this.NomeVisibile = (string.IsNullOrWhiteSpace(this.Persona.NOME + this.Persona.COGNOME) ? this.Email
                .Where(m => m.TIPO == (int)TipoEmail.Registrazione).SingleOrDefault().EMAIL : string.Concat(this.Persona.NOME, " ", this.Persona.COGNOME));
            this.NumeroMessaggiDaLeggere = db.CHAT.Count(m => m.ID_DESTINATARIO == idPersona && m.STATO == (int)StatoChat.INVIATO);
            this.NumeroNotificheDaLeggere = db.NOTIFICA.Count(m => m.ID_PERSONA_DESTINATARIO == idPersona && m.STATO == (int)StatoNotifica.ATTIVA);
            
        }

        public PersonaModel(PERSONA model)
        {
            this.Persona = model;
            this.SetValoriBase();
        }
        #endregion

        #region METODI PRIVATI

        private void SetValoriBase()
        {
            this.Punti = 0;
            this.PuntiSospesi = 0;
            this.Email = new List<PERSONA_EMAIL>();
            this.Telefono = new List<PERSONA_TELEFONO>();
            this.Indirizzo = new List<PERSONA_INDIRIZZO>();
            this.Attivita = new List<AttivitaModel>();
            this.Foto = new List<FotoModel>();
            //this.ContoCorrente = new List<CONTO_CORRENTE_MONETA>();
            this.Credito = new List<CONTO_CORRENTE_CREDITO>();
            this.MetodoPagamento = new List<PERSONA_METODO_PAGAMENTO>();
        }

        #endregion

        #region METODI PUBBLICI

        public void SetEmail(DatabaseContext db, string email, Stato stato = Stato.ATTIVO)
        {
            PERSONA_EMAIL model = this.Email.SingleOrDefault(m => m.TIPO == (int)TipoEmail.Registrazione);
            //this.Email.Remove(model);
            if (model == null)
            {
                if (!string.IsNullOrWhiteSpace(email))
                {
                    model = new PERSONA_EMAIL();
                    model.ID_PERSONA = this.Persona.ID;
                    model.DATA_INSERIMENTO = DateTime.Now;
                    model.TIPO = (int)TipoEmail.Registrazione;
                    model.STATO = (int)stato;
                    model.EMAIL = email;
                    db.PERSONA_EMAIL.Add(model);
                    this.Email.Add(model);
                }
            }
            else if (model.EMAIL != email)
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    db.Entry(model).State = EntityState.Deleted;
                    this.Email.Remove(model);
                }
                else
                {
                    model.EMAIL = email;
                    db.Entry(model).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
        }

        public void SetTelefono(DatabaseContext db, string telefono)
        {
            PERSONA_TELEFONO model = this.Telefono.SingleOrDefault(m => m.TIPO == (int)TipoTelefono.Privato);
            //this.Telefono.Remove(model);
            if (model == null)
            {
                if (!string.IsNullOrWhiteSpace(telefono))
                {
                    model = new PERSONA_TELEFONO();
                    model.ID_PERSONA = this.Persona.ID;
                    model.DATA_INSERIMENTO = DateTime.Now;
                    model.TIPO = (int)TipoTelefono.Privato;
                    model.STATO = (int)Stato.ATTIVO;
                    model.TELEFONO = telefono;
                    db.PERSONA_TELEFONO.Add(model);
                    this.Telefono.Add(model);
                }
            }
            else if (model.TELEFONO != telefono)
            {
                if (string.IsNullOrWhiteSpace(telefono))
                {
                    db.Entry(model).State = EntityState.Deleted;
                    this.Telefono.Remove(model);
                }
                else
                {
                    model.TELEFONO = telefono;
                    db.Entry(model).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
            /*
            if (!string.IsNullOrWhiteSpace(telefono))
                this.Telefono.Add(model);
            */
        }

        public void SetIndirizzo(DatabaseContext db, int? comune, string indirizzo, int? civico, int tipoIndirizzo)
        {
            PERSONA_INDIRIZZO model = this.Indirizzo.SingleOrDefault(m => m.TIPO == tipoIndirizzo
                && m.STATO == (int)Stato.ATTIVO);
            bool modificato = false;
            if (model == null)
            {
                // se l'utente non aveva un indirizzo e ne ha inserito uno
                if (!string.IsNullOrWhiteSpace(indirizzo))
                {
                    model = new PERSONA_INDIRIZZO();
                    model.ID_PERSONA = this.Persona.ID;
                    model.DATA_INSERIMENTO = DateTime.Now;
                    model.TIPO = tipoIndirizzo;
                    model.STATO = (int)Stato.ATTIVO;
                    model.INDIRIZZO = db.INDIRIZZO.Include("Comune").SingleOrDefault(m => m.INDIRIZZO1 == indirizzo && m.ID_COMUNE == comune && m.CIVICO == civico);
                    if (model.INDIRIZZO == null)
                    {
                        // se l'indirizzo inserito non è stato trovato a db
                        model.INDIRIZZO = new INDIRIZZO();
                        model.INDIRIZZO.DATA_INSERIMENTO = DateTime.Now;
                        model.INDIRIZZO.STATO = (int)Stato.ATTIVO;
                        model.INDIRIZZO.ID_COMUNE = (int)comune;
                        model.INDIRIZZO.INDIRIZZO1 = indirizzo;
                        model.INDIRIZZO.CIVICO = (int)civico;
                        db.INDIRIZZO.Add(model.INDIRIZZO);
                    }
                    // aggiungo l'indirizzo all'utente
                    model.ID_INDIRIZZO = model.INDIRIZZO.ID;
                    db.PERSONA_INDIRIZZO.Add(model);
                    this.Indirizzo.Add(model);
                }
                modificato = db.SaveChanges() > 0;
            }
            else if (model.INDIRIZZO != null && model.INDIRIZZO.ID_COMUNE != comune || model.INDIRIZZO.INDIRIZZO1 != indirizzo || model.INDIRIZZO.CIVICO != civico)
            {
                // se l'utente aveva un indirizzo ed è stato modificato o cancellato
                db.Entry(model).State = EntityState.Deleted;
                this.Indirizzo.Remove(model);
                db.PERSONA_INDIRIZZO.Remove(model);
                
                if (!string.IsNullOrWhiteSpace(indirizzo))
                {
                    PERSONA_INDIRIZZO nuovoIndirizzo = new PERSONA_INDIRIZZO();
                    nuovoIndirizzo.ID_PERSONA = model.ID_PERSONA;
                    nuovoIndirizzo.ORDINE = model.ORDINE;
                    nuovoIndirizzo.TIPO = model.TIPO;
                    nuovoIndirizzo.DATA_INSERIMENTO = DateTime.Now;
                    nuovoIndirizzo.STATO = (int)Stato.ATTIVO;
                    // se l'indirizzo è stato modificato allora associo il nuovo
                    nuovoIndirizzo.INDIRIZZO = db.INDIRIZZO.Include("Comune").SingleOrDefault(m => m.INDIRIZZO1 == indirizzo && m.ID_COMUNE == comune && m.CIVICO == civico);
                    if (nuovoIndirizzo.INDIRIZZO == null)
                    {
                        // se non trovo l'indirizzo nuovo su db lo inserisco
                        nuovoIndirizzo.INDIRIZZO = new INDIRIZZO();
                        nuovoIndirizzo.INDIRIZZO.DATA_INSERIMENTO = DateTime.Now;
                        nuovoIndirizzo.INDIRIZZO.STATO = (int)Stato.ATTIVO;
                        nuovoIndirizzo.INDIRIZZO.ID_COMUNE = (int)comune;
                        nuovoIndirizzo.INDIRIZZO.INDIRIZZO1 = indirizzo;
                        nuovoIndirizzo.INDIRIZZO.CIVICO = (int)civico;
                        db.INDIRIZZO.Add(nuovoIndirizzo.INDIRIZZO);
                        db.SaveChanges();
                    }
                    nuovoIndirizzo.ID_INDIRIZZO = nuovoIndirizzo.INDIRIZZO.ID;
                    db.PERSONA_INDIRIZZO.Add(nuovoIndirizzo);
                }
                modificato = db.SaveChanges() > 0;
            }
        }

        public bool SetPrivacy(DatabaseContext db, bool accettaCondizioni)
        {
            PERSONA_PRIVACY personaPrivacy = db.PERSONA_PRIVACY.Create();
            personaPrivacy.ID_PERSONA = this.Persona.ID;
            personaPrivacy.ACCETTA_CONDIZIONE = accettaCondizioni;
            personaPrivacy.DATA_INSERIMENTO = DateTime.Now;
            personaPrivacy.STATO = (int)Stato.ATTIVO;
            db.PERSONA_PRIVACY.Add(personaPrivacy);

            return (db.SaveChanges() > 0);
        }

        public bool AddBonusCanalePubblicitario(DatabaseContext db, string promo)
        {
            int valorePromo = Convert.ToInt32(ConfigurationManager.AppSettings["bonusPromozione" + promo]);
            this.Persona.DATA_ACCESSO = DateTime.Now;
            db.Entry(this.Persona).State = EntityState.Modified;
            if (db.SaveChanges() > 0)
            {
                Guid tokenPortale = Guid.Parse(ConfigurationManager.AppSettings["portaleweb"]);
                TRANSAZIONE model = new TRANSAZIONE();
                model.ID_CONTO_MITTENTE = db.ATTIVITA.Where(p => p.TOKEN == tokenPortale).SingleOrDefault().ID_CONTO_CORRENTE;
                model.ID_CONTO_DESTINATARIO = this.Persona.ID_CONTO_CORRENTE;
                model.TIPO = (int)TipoTransazione.BonusCanalePubblicitario;
                model.NOME = string.Format(App_GlobalResources.Bonus.AdChannel, promo);
                model.PUNTI = valorePromo;
                model.DATA_INSERIMENTO = DateTime.Now;
                model.STATO = (int)StatoPagamento.ACCETTATO;
                db.TRANSAZIONE.Add(model);
                db.SaveChanges();
                if (model.PUNTI != null && model.PUNTI > 0)
                {
                    ContoCorrenteCreditoModel credito = new ContoCorrenteCreditoModel(db, this.Persona.ID_CONTO_CORRENTE);
                    credito.Earn(model.ID, (decimal)model.PUNTI);
                }
                return true;
            }
            return false;
        }

        public void SetImmagineProfilo(DatabaseContext db, int idAllegato)
        {
            PERSONA_FOTO foto = db.PERSONA_FOTO.Create();
            foto.ID_FOTO = idAllegato;
            foto.ID_PERSONA = this.Persona.ID;
            foto.TIPO = (int)TipoMedia.FOTO;
            int numeroFoto = db.PERSONA_FOTO.Count(m => m.ID_PERSONA == this.Persona.ID);
            foto.ORDINE = numeroFoto + 1;
            foto.DATA_INSERIMENTO = DateTime.Now;
            foto.STATO = (int)Stato.ATTIVO;
            db.PERSONA_FOTO.Add(foto);
            db.SaveChanges();
            //this.Foto.Add(foto);
        }

        public void RemoveImmagineProfilo(DatabaseContext db, int idAllegato)
        {
            PERSONA_FOTO foto = db.PERSONA_FOTO.SingleOrDefault(m => m.ID_PERSONA == this.Persona.ID && m.ID_FOTO == idAllegato);
            if (foto != null)
            {
                string pathBase = System.Web.Hosting.HostingEnvironment.MapPath(System.IO.Path.Combine("/Uploads/Images/", this.Persona.TOKEN.ToString(), DateTime.Now.Year.ToString()));
                string pathImgOriginale = System.IO.Path.Combine(pathBase, "Original", foto.ALLEGATO.NOME);
                string pathImgMedia = System.IO.Path.Combine(pathBase, "Normal", foto.ALLEGATO.NOME);
                string pathImgPiccola = System.IO.Path.Combine(pathBase, "Little", foto.ALLEGATO.NOME);

                System.IO.File.Delete(pathImgOriginale);
                System.IO.File.Delete(pathImgMedia);
                System.IO.File.Delete(pathImgPiccola);
                db.PERSONA_FOTO.Remove(foto);
                db.SaveChanges();
            }
            //this.Foto.Add(foto);
        }

        #endregion
    }
}