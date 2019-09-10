using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GratisForGratis.Models
{
    public class PortaleWebViewModel
    {
        #region COSTRUTTORI

        public PortaleWebViewModel() { }

        public PortaleWebViewModel(PERSONA_ATTIVITA model, List<ATTIVITA_EMAIL> modelEmail, List<ATTIVITA_TELEFONO> modelTelefono)
        {
            this.CopyModel(model, modelEmail, modelTelefono);
        }

        #endregion

        #region PROPRIETA

        public string Id { get; private set; }

        [Required]
        [DataType(DataType.EmailAddress, ErrorMessageResourceName = "ErrorFormatEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [StringLength(200, ErrorMessageResourceName = "ErrorLengthEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Name", ResourceType = typeof(App_GlobalResources.Language))]
        public string Nome { get; set; }

        [Required]
        [DataType(DataType.Url, ErrorMessageResourceName = "ErrorFormat", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "Domain", ResourceType = typeof(App_GlobalResources.Language))]
        public string Dominio { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 16)]
        public string Token { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [StringLength(12, MinimumLength = 9, ErrorMessageResourceName = "ErrorPhoneNumber", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "Telephone", ResourceType = typeof(App_GlobalResources.Language))]
        public string Telefono { get; set; }

        [Required]
        [Display(Name = "Subscription", ResourceType = typeof(App_GlobalResources.Language))]
        public AbbonamentoModel Abbonamento { get; set; }

        //// annuale
        //[Display(Name = "BonusForUser", ResourceType = typeof(App_GlobalResources.Language))]
        //public int BonusPerUtente { get; set; }

        ////mesi
        //[Display(Name = "DurationSubscription", ResourceType = typeof(App_GlobalResources.Language))]
        //public int DurataAbbonamento { get; set; }

        [Required]
        [Display(Name = "AcceptConditions", ResourceType = typeof(App_GlobalResources.Language))]
        public bool AccettaCondizioni { get; set; }

        [Display(Name = "RoleUser", ResourceType = typeof(App_GlobalResources.Language))]
        public RuoloProfilo Ruolo { get; set; }

        [Display(Name = "BonusNow", ResourceType = typeof(App_GlobalResources.Language))]
        public decimal Bonus { get; set; }

        // annuale
        [Display(Name = "BonusUsed", ResourceType = typeof(App_GlobalResources.Language))]
        public decimal? BonusSpeso { get; set; }

        [Display(Name = "DateSubscription", ResourceType = typeof(App_GlobalResources.Language))]
        public DateTime DataIscrizione { get; set; }

        public List<FotoModel> Foto { get; set; }

        #endregion

        #region METODI PUBBLICI


        public void CopyModel(PERSONA_ATTIVITA model, List<ATTIVITA_EMAIL> modelEmail, List<ATTIVITA_TELEFONO> modelTelefono)
        {
            this.Id = model.ATTIVITA.ID.ToString();
            this.Ruolo = (RuoloProfilo)model.RUOLO;
            this.Email = modelEmail.Find(item => item.TIPO == (int)TipoEmail.Registrazione).EMAIL;
            this.Nome = model.ATTIVITA.NOME;
            this.Dominio = model.ATTIVITA.DOMINIO;
            this.Token = model.ATTIVITA.TOKEN.ToString();
            this.Telefono = modelTelefono.Find(item => item.TIPO == (int)TipoTelefono.Privato).TELEFONO;
            /*this.Abbonamento = model.ATTIVITA.ABBONAMENTO1.NOME;
            this.BonusPerUtente = model.ATTIVITA.ABBONAMENTO1.BONUS_PERUTENTE;
            this.DurataAbbonamento = model.ATTIVITA.ABBONAMENTO1.DURATA;*/
            // fare count punti sul conto corrente
            //this.Bonus = model.ATTIVITA.BONUS;
        }

        public void CopyModel(ATTIVITA model, List<ATTIVITA_EMAIL> modelEmail, List<ATTIVITA_TELEFONO> modelTelefono)
        {
            this.Email = modelEmail.Find(item => item.TIPO == (int)TipoEmail.Registrazione).EMAIL;
            this.Nome = model.NOME;
            this.Dominio = model.DOMINIO;
            this.Token = model.TOKEN.ToString();
            this.Telefono = modelTelefono.Find(item => item.TIPO == (int)TipoTelefono.Privato).TELEFONO;
            //this.Abbonamento = new AbbonamentoModel(model)
            this.Bonus = model.CONTO_CORRENTE.CONTO_CORRENTE_MONETA.Count;
            this.DataIscrizione = (DateTime)model.DATA_INSERIMENTO;
        }

        public void SetImmagineProfilo(DatabaseContext db, int idAllegato)
        {
            ATTIVITA_FOTO foto = db.ATTIVITA_FOTO.Create();
            foto.ID_FOTO = idAllegato;
            foto.ID_ATTIVITA = Convert.ToInt32(this.Id);
            foto.TIPO = (int)TipoMedia.FOTO;
            int idPortale = Convert.ToInt32(this.Id);
            int numeroFoto = db.ATTIVITA_FOTO.Count(m => m.ID_ATTIVITA == idPortale);
            foto.ORDINE = numeroFoto + 1;
            foto.DATA_INSERIMENTO = DateTime.Now;
            foto.STATO = (int)Stato.ATTIVO;
            db.ATTIVITA_FOTO.Add(foto);
            db.SaveChanges();
            //this.Foto.Add(foto);
        }

        public void RemoveImmagineProfilo(DatabaseContext db, int idAllegato)
        {
            int idPortale = Convert.ToInt32(this.Id);
            ATTIVITA_FOTO foto = db.ATTIVITA_FOTO.SingleOrDefault(m => m.ID_ATTIVITA == idPortale && m.ID_FOTO == idAllegato);
            if (foto != null)
            {
                string pathBase = System.Web.Hosting.HostingEnvironment.MapPath(System.IO.Path.Combine("/Uploads/Images/", this.Token, DateTime.Now.Year.ToString()));
                string pathImgOriginale = System.IO.Path.Combine(pathBase, "Original", foto.ALLEGATO.NOME);
                string pathImgMedia = System.IO.Path.Combine(pathBase, "Normal", foto.ALLEGATO.NOME);
                string pathImgPiccola = System.IO.Path.Combine(pathBase, "Little", foto.ALLEGATO.NOME);

                System.IO.File.Delete(pathImgOriginale);
                System.IO.File.Delete(pathImgMedia);
                System.IO.File.Delete(pathImgPiccola);
                db.ATTIVITA_FOTO.Remove(foto);
                db.SaveChanges();

                this.Foto = db.ATTIVITA_FOTO.Where(m => m.ID_ATTIVITA == idPortale)
                    .Select(m => new FotoModel(m.ALLEGATO)).ToList();
            }
            //this.Foto.Add(foto);
        }

        #endregion
    }

    public class PortaleWebProfiloViewModel : PortaleWebViewModel
    {
        #region PROPRIETA
        public List<FotoModel> Foto { get; set; }

        public List<AnnuncioViewModel> ListaAcquisti { get; set; }

        public List<AnnuncioViewModel> ListaVendite { get; set; }

        public List<AnnuncioViewModel> ListaDesideri { get; set; }
        #endregion

        #region COSTRUTTORI
        public PortaleWebProfiloViewModel() { }

        public PortaleWebProfiloViewModel(PERSONA_ATTIVITA model, List<ATTIVITA_EMAIL> modelEmail, List<ATTIVITA_TELEFONO> modelTelefono) : base(model, modelEmail, modelTelefono) { }

        public PortaleWebProfiloViewModel(PortaleWebViewModel model)
        {
            foreach (System.Reflection.PropertyInfo propertyInfo in model.GetType().GetProperties())
            {
                if (this.GetType().GetProperty(propertyInfo.Name).SetMethod != null)
                    this.GetType().GetProperty(propertyInfo.Name).SetValue(this, propertyInfo.GetValue(model));
            }
        }
        #endregion

        #region METODI PUBBLICI
        public void LoadExtra(DatabaseContext db, ATTIVITA attivita)
        {
            Foto = attivita.ATTIVITA_FOTO.Select(m => new FotoModel(m.ALLEGATO)).ToList();

            ListaAcquisti = new List<AnnuncioViewModel>();
            ListaVendite = new List<AnnuncioViewModel>();
            ListaDesideri = new List<AnnuncioViewModel>();
            // far vedere top n acquisti con link
            var query = db.ANNUNCIO.Where(item => item.ID_COMPRATORE == attivita.ID
                    && item.TRANSAZIONE_ANNUNCIO.Count(m => m.STATO == (int)StatoPagamento.ATTIVO || m.STATO == (int)StatoPagamento.ACCETTATO) > 0
                    && (item.STATO == (int)StatoVendita.VENDUTO || item.STATO == (int)StatoVendita.BARATTATO)
                    && (item.ID_OGGETTO != null || item.ID_SERVIZIO != null));
            List<ANNUNCIO> lista = query
                .OrderByDescending(item => item.DATA_INSERIMENTO)
                .Take(4).ToList();
            foreach (ANNUNCIO m in lista)
            {
                AnnuncioModel annuncioModel = new AnnuncioModel();
                ListaAcquisti.Add(annuncioModel.GetViewModel(db, m));
            }
            // far vedere vendite recenti con link
            var queryVendite = db.ANNUNCIO.Where(item => item.ID_PERSONA == attivita.ID
                    && (item.STATO != (int)StatoVendita.ELIMINATO && item.STATO != (int)StatoVendita.BARATTATO && item.STATO != (int)StatoVendita.VENDUTO)
                    && (item.ID_OGGETTO != null || item.ID_SERVIZIO != null));
            List<ANNUNCIO> lista2 = queryVendite
                .OrderByDescending(item => item.DATA_INSERIMENTO)
                .Take(4).ToList();
            foreach (ANNUNCIO m in lista2)
            {
                AnnuncioModel annuncioModel = new AnnuncioModel();
                ListaVendite.Add(annuncioModel.GetViewModel(db, m));
            }
            // far vedere top n desideri con link
            List<ANNUNCIO_DESIDERATO> lista3 = db.ANNUNCIO_DESIDERATO
                .Where(item => item.ID_PERSONA == attivita.ID && (item.ANNUNCIO.STATO == (int)StatoVendita.INATTIVO
                    || item.ANNUNCIO.STATO == (int)StatoVendita.ATTIVO) && (item.ANNUNCIO.DATA_FINE == null ||
                    item.ANNUNCIO.DATA_FINE >= DateTime.Now))
                .OrderByDescending(item => item.ANNUNCIO.DATA_INSERIMENTO)
                .Take(4)
                .ToList();
            lista3.ForEach(m =>
                ListaDesideri.Add(
                    new AnnuncioViewModel(db, m.ANNUNCIO)
                )
            );
        }
        #endregion
    }
}
