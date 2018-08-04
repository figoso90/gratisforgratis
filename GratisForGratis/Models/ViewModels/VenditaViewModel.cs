using GratisForGratis.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GratisForGratis.Models
{
    #region GENERICO
    public class AnnuncioViewModel
    {
        #region COSTRUTTORI
        public AnnuncioViewModel()
        {
            this.SetProprietaIniziali();
        }
        public AnnuncioViewModel(DatabaseContext db, ANNUNCIO model)
        {
            this.SetProprietaIniziali();
            Id = model.ID;
            Venditore = new UtenteVenditaViewModel();
            if (model.ID_ATTIVITA != null)
            {
                Venditore.Id = (int)model.ID_ATTIVITA;
                Venditore.Nominativo = model.ATTIVITA.NOME;
                Venditore.Email = model.ATTIVITA.ATTIVITA_EMAIL.FirstOrDefault(m => m.TIPO == (int)TipoEmail.Registrazione).EMAIL;
                Venditore.Telefono = model.ATTIVITA.ATTIVITA_TELEFONO.FirstOrDefault(m => m.TIPO == (int)TipoTelefono.Privato).TELEFONO;
                SetFeedbackVenditore(model, TipoVenditore.Attivita);
            }
            else
            {
                Venditore.Id = model.ID_PERSONA;
                Venditore.Nominativo = model.PERSONA.NOME + " " + model.PERSONA.COGNOME;
                Venditore.Email = model.PERSONA.PERSONA_EMAIL.FirstOrDefault(m => m.TIPO == (int)TipoEmail.Registrazione).EMAIL;
                PERSONA_TELEFONO telefono = model.PERSONA.PERSONA_TELEFONO.FirstOrDefault(m => m.TIPO == (int)TipoTelefono.Privato);
                if (telefono!=null)
                    Venditore.Telefono = telefono.TELEFONO;
                SetFeedbackVenditore(model, TipoVenditore.Persona);
            }
            Venditore.VenditoreToken = model.PERSONA.TOKEN;
            Token = model.TOKEN.ToString();
            Nome = model.NOME;
            TipoPagamento = (TipoPagamento)model.TIPO_PAGAMENTO;
            Punti = model.PUNTI;
            Soldi = Convert.ToDecimal(model.SOLDI).ToString("C");
            DataInserimento = (DateTime)model.DATA_INSERIMENTO;
            CategoriaID = model.ID_CATEGORIA;
            Categoria = model.CATEGORIA.NOME;
            //TipoSpedizione = (Spedizione)model.TIPO_SPEDIZIONE;
            int venditaId = model.ID;
            Foto = model.ANNUNCIO_FOTO.Select(f => new AnnuncioFoto()
            {
                ID_ANNUNCIO = f.ID_ANNUNCIO,
                ALLEGATO = f.ALLEGATO,
                DATA_INSERIMENTO = f.DATA_INSERIMENTO,
                DATA_MODIFICA = f.DATA_MODIFICA
            }).ToList();
            StatoVendita = (StatoVendita)model.STATO;
            Notificato = (model.ANNUNCIO_NOTIFICA.Count > 0) ? true : false;
            var listaInteressati = model.ANNUNCIO_DESIDERATO.Where(f => f.ID_ANNUNCIO == model.ID);
            NumeroInteressati = listaInteressati.Count();
            int idUtente = (HttpContext.Current.Session["utente"] as PersonaModel).Persona.ID;
            Desidero = listaInteressati.FirstOrDefault(m => m.ID_PERSONA == idUtente) != null;
            // controllo se l'utente ha già proposto lo stesso annuncio
            int? idAnnuncioOriginale = model.ID_ORIGINE;
            ANNUNCIO copiaAnnuncio = db.ANNUNCIO.SingleOrDefault(m => m.ID_PERSONA == idUtente
                && (m.STATO == (int)StatoVendita.ATTIVO || m.STATO == (int)StatoVendita.INATTIVO)
                && ((m.ID_ORIGINE == idAnnuncioOriginale && idAnnuncioOriginale != null) ||
                m.ID_ORIGINE == model.ID));
            if (copiaAnnuncio != null)
                TokenAnnuncioCopiato = Utils.RandomString(3) + copiaAnnuncio.TOKEN + Utils.RandomString(3);
        }
        public AnnuncioViewModel(AnnuncioViewModel model)
        {
            this.SetProprietaIniziali();
            foreach (PropertyInfo propertyInfo in model.GetType().GetProperties())
            {
                this.GetType().GetProperty(propertyInfo.Name).SetValue(this, propertyInfo.GetValue(model));
            }
        }
        #endregion

        #region PROPRIETA
        // aggiungere partner venditore
        public int Id { get; set; }

        //public int VenditaID { get; set; }

        public string Token { get; set; }

        public TipoAcquisto TipoAcquisto { get; set; }

        public UtenteVenditaViewModel Venditore { get; set; }

        public UtenteVenditaViewModel Compratore { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Name", ResourceType = typeof(App_GlobalResources.Language))]
        public string Nome { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "OptionalNote", ResourceType = typeof(App_GlobalResources.Language))]
        public string NoteAggiuntive { get; set; }

        [Display(Name = "PaymentMethods", ResourceType = typeof(App_GlobalResources.Language))]
        public TipoPagamento TipoPagamento { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Category", ResourceType = typeof(App_GlobalResources.Language))]
        public int CategoriaID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Category", ResourceType = typeof(App_GlobalResources.Language))]
        public string Categoria { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "City", ResourceType = typeof(App_GlobalResources.Language))]
        public string Citta { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Points", ResourceType = typeof(App_GlobalResources.Language))]
        public int? Punti { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Money", ResourceType = typeof(App_GlobalResources.Language))]
        public string Soldi { get; set; }

        public int IdTipoValuta { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Phote", ResourceType = typeof(App_GlobalResources.Language))]
        public List<AnnuncioFoto> Foto { get; set; }

        [DataType(DataType.DateTime, ErrorMessageResourceName = "ErrorDate", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public DateTime? DataInserimento { get; set; }

        [DataType(DataType.DateTime, ErrorMessageResourceName = "ErrorDate", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public DateTime? DataModifica { get; set; }

        [Display(Name = "Feedback", ResourceType = typeof(App_GlobalResources.Language))]
        public double VenditoreFeedback { get; set; }

        [Display(Name = "StateSelling", ResourceType = typeof(App_GlobalResources.Language))]
        public StatoVendita StatoVendita { get; set; }

        public bool Notificato { get; set; }

        public bool Desidero { get; set; }

        public int NumeroInteressati { get; set; }

        public string TokenAnnuncioCopiato { get; set; }

        public OffertaViewModel Offerta { get; set; }

        public string NomeVistaDettaglio { get { return string.Empty; } protected set { } }
        // nuovi campi

        [DataType(DataType.DateTime, ErrorMessageResourceName = "ErrorDate", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public DateTime DataAvvio { get; set; }

        [DataType(DataType.DateTime, ErrorMessageResourceName = "ErrorDate", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public DateTime? DataFine { get; set; }

        public bool NoOfferte { get; set; }

        public bool RimettiInVendita { get; set; }

        public string Azione { get; set; }
        /*
        public bool SpedizioneAMano { get; set; }

        public bool SpedizionePrivata { get; set; }

        public bool SpedizioneOnline { get; set; }
        */
        #endregion

        #region METODI PRIVATI
        private void SetProprietaIniziali()
        {
            Foto = new List<AnnuncioFoto>();
            Offerta = new OffertaViewModel();
        }
        private void SetFeedbackVenditore(ANNUNCIO model, TipoVenditore tipoVenditore)
        {
            try
            {
                List<int> voti = model.ANNUNCIO_FEEDBACK
                                .Where(item => (tipoVenditore == TipoVenditore.Persona && item.ANNUNCIO.ID_PERSONA == Venditore.Id) ||
                                (tipoVenditore == TipoVenditore.Attivita && item.ANNUNCIO.ID_ATTIVITA == Venditore.Id)).Select(item => item.VOTO).ToList();

                int votoMassimo = voti.Count * 10;
                if (voti.Count <= 0)
                {
                    VenditoreFeedback = -1;
                }
                else
                {
                    int x = voti.Sum() / votoMassimo;
                    VenditoreFeedback = x * 100;
                }
            }
            catch (Exception eccezione)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
                VenditoreFeedback = -1;
            }
        }
        #endregion
    }

    public class OggettoViewModel : AnnuncioViewModel
    {
        #region COSTRUTTORI
        public OggettoViewModel() : base() { }

        public OggettoViewModel(OggettoViewModel model) : base(model)
        {
            foreach (PropertyInfo propertyInfo in model.GetType().GetProperties())
            {
                this.GetType().GetProperty(propertyInfo.Name).SetValue(this, propertyInfo.GetValue(model));
            }
        }

        public OggettoViewModel(AnnuncioViewModel model) : base(model) { }
        #endregion

        #region PROPRIETA
        [Required]
        public int OggettoId { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int NumeroOggetto { get; set; }

        [Display(Name = "StateObject", ResourceType = typeof(App_GlobalResources.Language))]
        public CondizioneOggetto StatoOggetto { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Brand", ResourceType = typeof(App_GlobalResources.Language))]
        public string Marca { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Year", ResourceType = typeof(App_GlobalResources.Language))]
        public int? Anno { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "High", ResourceType = typeof(App_GlobalResources.Language))]
        public decimal? Altezza { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Width", ResourceType = typeof(App_GlobalResources.Language))]
        public decimal? Larghezza { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Length", ResourceType = typeof(App_GlobalResources.Language))]
        public decimal? Lunghezza { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Weight", ResourceType = typeof(App_GlobalResources.Language))]
        public decimal? Peso { get; set; }

        [Range(1, int.MaxValue)]
        [Display(Name = "Quantity", ResourceType = typeof(App_GlobalResources.Language))]
        public int? Quantità { get; set; }

        public string Note { get; set; }

        public List<string> Materiali { get; set; }

        public List<string> Componenti { get; set; }

        public TipoScambio[] TipoScambio { get; set; }

        public string NomeCorriere { get; set; }

        public int PuntiSpedizione { get; set; }

        public string SoldiSpedizione { get; set; }

        public string LDVNome { get; set; }

        public string LDVFile { get; set; }
        #endregion
    }

    public class ServizioViewModel : AnnuncioViewModel
    {
        #region COSTRUTTORI
        public ServizioViewModel() : base()
        {
            TipoAcquisto = TipoAcquisto.Servizio;
        }

        public ServizioViewModel(ServizioViewModel model) : base(model)
        {
            foreach (PropertyInfo propertyInfo in model.GetType().GetProperties())
            {
                this.GetType().GetProperty(propertyInfo.Name).SetValue(this, propertyInfo.GetValue(model));
            }
            TipoAcquisto = TipoAcquisto.Servizio;
        }

        public ServizioViewModel(AnnuncioViewModel model) : base(model)
        {
            TipoAcquisto = TipoAcquisto.Servizio;
        }
        #endregion

        #region PROPRIETA
        [Required]
        public int ServizioId { get; set; }
        public string Compra { get; set; }

        public string Note { get; set; }

        [Display(Name = "Monday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool? Lunedi { get; set; }

        [Display(Name = "Tuesday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool? Martedi { get; set; }

        [Display(Name = "Wednesday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool? Mercoledi { get; set; }

        [Display(Name = "Thursday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool? Giovedi { get; set; }

        [Display(Name = "Friday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool? Venerdi { get; set; }

        [Display(Name = "Saturday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool? Sabato { get; set; }

        [Display(Name = "Sunday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool? Domenica { get; set; }

        [Display(Name = "AllDays", ResourceType = typeof(App_GlobalResources.Language))]
        public bool? Tutti { get; set; }

        [Display(Name = "Holidays", ResourceType = typeof(App_GlobalResources.Language))]
        public bool? Festivita { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "StartTime", ResourceType = typeof(App_GlobalResources.Language))]
        public TimeSpan? OraInizio { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "EndTime", ResourceType = typeof(App_GlobalResources.Language))]
        public TimeSpan? OraFine { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "StartTime", ResourceType = typeof(App_GlobalResources.Language))]
        public TimeSpan? OraInizioFestivita { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "EndTime", ResourceType = typeof(App_GlobalResources.Language))]
        public TimeSpan? OraFineFestivita { get; set; }

        [Display(Name = "Bid", ResourceType = typeof(App_GlobalResources.Language))]
        public string ServiziOfferti { get; set; }

        [Display(Name = "Results", ResourceType = typeof(App_GlobalResources.Language))]
        public string RisultatiFinali { get; set; }

        [Display(Name = "Rate", ResourceType = typeof(App_GlobalResources.Language))]
        public Tariffa Tariffa { get; set; }
        #endregion
    }

    #endregion

    #region DETTAGLI OGGETTO

    public class ModelloViewModel : OggettoViewModel
    {

        public ModelloViewModel() : base() { }

        public ModelloViewModel(OggettoViewModel model) : base(model) { }

        //[Required]
        [Range(1, int.MaxValue)]
        public int? modelloID { get; set; }

        //[Required]
        [DataType(DataType.Text)]
        public string modelloNome { get; set; }

        public new string NomeVistaDettaglio { get { return "Modello"; } protected set { } }

    }

    public class TelefonoViewModel : ModelloViewModel
    {

        public TelefonoViewModel() : base() { }

        public TelefonoViewModel(OggettoViewModel model) : base(model) { }

        //[Required]
        [Range(1, int.MaxValue)]
        public int? sistemaOperativoID { get; set; }

        //[Required]
        [DataType(DataType.Text)]
        public string sistemaOperativoNome { get; set; }

        public new string NomeVistaDettaglio { get { return "Telefono"; } protected set { } }
    }

    public class ComputerViewModel : ModelloViewModel
    {
        public ComputerViewModel() : base() { }

        public ComputerViewModel(OggettoViewModel model) : base(model) { }

        //[Required]
        [Range(1, int.MaxValue)]
        public int? sistemaOperativoID { get; set; }

        //[Required]
        [DataType(DataType.Text)]
        public string sistemaOperativoNome { get; set; }

        public new string NomeVistaDettaglio { get { return "Computer"; } protected set { } }
    }

    public class MusicaViewModel : OggettoViewModel
    {
        public MusicaViewModel() : base() { }

        public MusicaViewModel(OggettoViewModel model) : base(model) { }

        //[Required]
        [Range(1, int.MaxValue)]
        public int? formatoID { get; set; }

        //[Required]
        [DataType(DataType.Text)]
        public string formatoNome { get; set; }

        //[Required]
        [Range(1, int.MaxValue)]
        public int? artistaID { get; set; }

        //[Required]
        [DataType(DataType.Text)]
        public string artistaNome { get; set; }

        public new string NomeVistaDettaglio { get { return "Musica"; } protected set { } }
    }

    public class VideogamesViewModel : OggettoViewModel
    {
        public VideogamesViewModel() : base() { }

        public VideogamesViewModel(OggettoViewModel model) : base(model) { }

        //[Required]
        [Range(1, int.MaxValue)]
        public int? piattaformaID { get; set; }

        //[Required]
        [DataType(DataType.Text)]
        public string piattaformaNome { get; set; }

        //[Required]
        [Range(1, int.MaxValue)]
        public int? genereID { get; set; }

        //[Required]
        [DataType(DataType.Text)]
        public string genereNome { get; set; }

        public new string NomeVistaDettaglio { get { return "Videogames"; } protected set { } }
    }

    public class ConsoleViewModel : OggettoViewModel
    {
        public ConsoleViewModel() : base() { }

        public ConsoleViewModel(OggettoViewModel model) : base(model) { }

        //[Required]
        [Range(1, int.MaxValue)]
        public int? piattaformaID { get; set; }

        //[Required]
        [DataType(DataType.Text)]
        public string piattaformaNome { get; set; }

        public new string NomeVistaDettaglio { get { return "Console"; } protected set { } }
    }

    public class VideoViewModel : OggettoViewModel
    {
        public VideoViewModel() : base() { }

        public VideoViewModel(OggettoViewModel model) : base(model) { }

        //[Required]
        [Range(1, int.MaxValue)]
        public int? formatoID { get; set; }

        //[Required]
        [DataType(DataType.Text)]
        [Display(Name = "Format", ResourceType = typeof(App_GlobalResources.Language))]
        public string formatoNome { get; set; }

        //[Required]
        [Range(1, int.MaxValue)]
        public int? registaID { get; set; }

        //[Required]
        [DataType(DataType.Text)]
        [Display(Name = "Director", ResourceType = typeof(App_GlobalResources.Language))]
        public string registaNome { get; set; }

        public new string NomeVistaDettaglio { get { return "Video"; } protected set { } }
    }

    public class LibroViewModel : OggettoViewModel
    {
        public LibroViewModel() : base() { }

        public LibroViewModel(OggettoViewModel model) : base(model) { }

        //[Required]
        [Range(1, int.MaxValue)]
        public int? autoreID { get; set; }

        //[Required]
        [DataType(DataType.Text)]
        public string autoreNome { get; set; }

        public new string NomeVistaDettaglio { get { return "Libro"; } protected set { } }
    }

    public class VeicoloViewModel : ModelloViewModel
    {
        public VeicoloViewModel() : base() { }

        public VeicoloViewModel(OggettoViewModel model) : base(model) { }

        //[Required]
        [Range(1, int.MaxValue)]
        public int? alimentazioneID { get; set; }

        //[Required]
        [DataType(DataType.Text)]
        [Display(Name = "Power", ResourceType = typeof(App_GlobalResources.Language))]
        public string alimentazioneNome { get; set; }

        public new string NomeVistaDettaglio { get { return "Veicolo"; } protected set { } }
    }

    public class VestitoViewModel : OggettoViewModel
    {
        public VestitoViewModel() : base() { }

        public VestitoViewModel(OggettoViewModel model) : base(model) { }

        [Required]
        [DataType(DataType.Text)]
        public string taglia { get; set; }

        public new string NomeVistaDettaglio { get { return "Vestito"; } protected set { } }
    }

    #endregion

    #region BARATTO

    public class BarattoOggettoViewModel : PubblicaOggettoViewModel
    {
        public HttpPostedFileBase[] File
        {
            get;
            set;
        }

        public BarattoOggettoViewModel()
        {
        }
    }

    public class BarattoServizioViewModel : PubblicaOggettoViewModel
    {
        public HttpPostedFileBase[] File
        {
            get;
            set;
        }

        public BarattoServizioViewModel()
        {
        }
    }

    #endregion
}
