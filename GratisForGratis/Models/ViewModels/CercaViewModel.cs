using GratisForGratis.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace GratisForGratis.Models
{
    #region FILTRI RICERCA
    
    public class RicercaViewModel
    {
        #region COSTRUTTORI
        public RicercaViewModel()
        {
            Cerca_IDCategoria = 1;
            Pagina = 1;
            Cerca_PuntiMin = 0;
            Cerca_PuntiMax = Convert.ToInt32(WebConfigurationManager.AppSettings["MaxPunti"]);
        }

        public RicercaViewModel(int numeroRecordTrovati = 0)
        {
            Cerca_IDCategoria = 1;
            Pagina = 1;
            Cerca_PuntiMin = 0;
            Cerca_PuntiMax = Convert.ToInt32(WebConfigurationManager.AppSettings["MaxPunti"]);
            NumeroRecordTrovati = numeroRecordTrovati;
        }
        #endregion

        #region PROPRIETA

        public int NumeroRecordTrovati { get; private set; }

        [Display(Name = "PaymentMethods", ResourceType = typeof(App_GlobalResources.Language))]
        public TipoPagamento? Cerca_TipoPagamento { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Place", ResourceType = typeof(App_GlobalResources.Language))]
        public string Cerca_Citta { get; set; }

        [Range(1, int.MaxValue)]
        public int? Cerca_IDCitta { get; set; }

        [Display(Name = "Min", ResourceType = typeof(App_GlobalResources.Language))]
        [RangeIntAdvanced("MinPunti", "MaxPunti", ErrorMessageResourceName = "ErrorPoints", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public int Cerca_PuntiMin { get; set; }

        [Display(Name = "Max", ResourceType = typeof(App_GlobalResources.Language))]
        [RangeIntAdvanced("MinPunti", "MaxPunti", ErrorMessageResourceName = "ErrorPoints", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public int Cerca_PuntiMax { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Category", ResourceType = typeof(App_GlobalResources.Language))]
        public string Cerca_Categoria { get; set; }

        public int Cerca_IDCategoria { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "NameObject", ResourceType = typeof(App_GlobalResources.Language))]
        public string Cerca_Nome { get; set; }

        public string Cerca_Submit { get; set; }

        public int Pagina { get; set; }

        public bool AttivaRicercaAvanzata { get; private set; }

        #endregion

        #region METODI PROTECTED
        // recupera valore di una chiave dal cookie o mette il valore di default
        protected object GetValueCookie(HttpCookie cookie, string key, object defaultValue = null) {
            if (cookie == null || string.IsNullOrEmpty(cookie[key]))
                return defaultValue;

            return cookie[key];
        }
        #endregion

        #region METODI PUBBLICI
        // setta la ricerca in base ai cookie
        public virtual void SetRicercaByCookie(HttpCookie cookie) {
            this.AttivaRicercaAvanzata = false;
            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
            {
                object valoreDefault = this.GetType().GetProperty(propertyInfo.Name).GetValue(this);
                string chiave = System.Text.RegularExpressions.Regex.Replace(propertyInfo.Name, "Cerca_", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // lascio come valore di default null, così se non trova una chiave nei cookie non setta risultati errati
                // nella proprietà della classe
                object valoreCookie = GetValueCookie(cookie, chiave);
                if (valoreCookie != null)
                {
                    Type tipoProprieta = propertyInfo.PropertyType;
                    if (tipoProprieta.IsGenericType && tipoProprieta.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        tipoProprieta = tipoProprieta.GetGenericArguments().First();
                    }
                    if (tipoProprieta.IsEnum)
                    {
                        valoreCookie = Enum.Parse(tipoProprieta, valoreCookie.ToString()) as Enum;
                    }
                    else
                    {
                        valoreCookie = Convert.ChangeType(valoreCookie, tipoProprieta);
                    }
                    this.GetType().GetProperty(propertyInfo.Name).SetValue(this, valoreCookie);
                    this.AttivaRicercaAvanzata = true;
                }
            }
        }

        // recupera i dati della ricerca in formato cookie sostituendo quelli passati
        public virtual HttpCookie GetCookieRicerca(HttpCookie cookie) {
            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties().Where(item => item.SetMethod != null && item.SetMethod.IsPublic))
            {
                //if (propertyInfo.GetValue(this) != null)
                string nomeParametro = System.Text.RegularExpressions.Regex.Replace(propertyInfo.Name, "Cerca_", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                cookie[nomeParametro] = (propertyInfo.GetValue(this) != null)?propertyInfo.GetValue(this).ToString():null;
            }
            return cookie;
        }
        #endregion
    }

    #region OGGETTI
    public class RicercaOggettoViewModel : RicercaViewModel
    {
        public RicercaOggettoViewModel() : base()
        {
            this.Cerca_AnnoMin = 0;
            this.Cerca_AnnoMax = DateTime.Now.Year;
        }

        public RicercaOggettoViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) 
        {
            this.Cerca_AnnoMin = 0;
            this.Cerca_AnnoMax = DateTime.Now.Year;
        }

        public RicercaOggettoViewModel(RicercaViewModel ricerca)
        {
            this.Cerca_Nome = ricerca.Cerca_Nome;
            this.Cerca_Categoria = ricerca.Cerca_Categoria;
            this.Cerca_IDCategoria = ricerca.Cerca_IDCategoria;
            this.Cerca_PuntiMax = Convert.ToInt32(WebConfigurationManager.AppSettings["MaxPunti"]);
            this.Cerca_AnnoMin = 0;
            this.Cerca_AnnoMax = DateTime.Now.Year;
        }

        [Display(Name = "StateObject", ResourceType = typeof(App_GlobalResources.Language))]
        public CondizioneOggetto? Cerca_StatoOggetto { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Brand", ResourceType = typeof(App_GlobalResources.Language))]
        public string Cerca_Marca { get; set; }

        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 16)]
        public string Cerca_MarcaID { get; set; }

        [Display(Name = "Min", ResourceType = typeof(App_GlobalResources.Language))]
        //[RangeDate(ErrorMessageResourceName = "ErrorMinYear", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Range(int.MinValue, int.MaxValue, ErrorMessageResourceName = "ErrorMinYear", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public int Cerca_AnnoMin { get; set; }

        [Display(Name = "Max", ResourceType = typeof(App_GlobalResources.Language))]
        [Range(int.MinValue, int.MaxValue, ErrorMessageResourceName = "ErrorMaxYear", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        //[RangeDate(ErrorMessageResourceName = "ErrorMaxYear", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public int Cerca_AnnoMax { get; set; }

        public List<Color> Cerca_Colore { get; set; }
    }

    public class RicercaModelloViewModel : RicercaOggettoViewModel
    {
        public RicercaModelloViewModel() : base() { }

        public RicercaModelloViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { }

        [Range(1, int.MaxValue)]
        public int? cerca_modelloID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Model", ResourceType = typeof(App_GlobalResources.Language))]
        public string cerca_modello { get; set; }
    }

    public class RicercaTelefonoViewModel : RicercaOggettoViewModel
    {
        public RicercaTelefonoViewModel() : base() { }

        public RicercaTelefonoViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { }

        [Range(1, int.MaxValue)]
        public int? cerca_modelloID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Model", ResourceType = typeof(App_GlobalResources.Language))]
        public string cerca_modello { get; set; }

        [Range(1, int.MaxValue)]
        public int? cerca_sistemaOperativoID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "SystemOperating", ResourceType = typeof(App_GlobalResources.Language))]
        public string cerca_sistemaOperativo { get; set; }
    }

    public class RicercaAudioHiFiViewModel : RicercaOggettoViewModel
    {
        public RicercaAudioHiFiViewModel() : base() { }

        public RicercaAudioHiFiViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { }

        [Range(1, int.MaxValue)]
        public int? cerca_modelloID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Model", ResourceType = typeof(App_GlobalResources.Language))]
        public string cerca_modello { get; set; }
    }

    public class RicercaPcViewModel : RicercaOggettoViewModel
    {
        public RicercaPcViewModel() : base() { }

        public RicercaPcViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { }

        [Range(1, int.MaxValue)]
        public int? cerca_modelloID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Model", ResourceType = typeof(App_GlobalResources.Language))]
        public string cerca_modello { get; set; }

        [Range(1, int.MaxValue)]
        public int? cerca_sistemaOperativoID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "SystemOperating", ResourceType = typeof(App_GlobalResources.Language))]
        public string cerca_sistemaOperativo { get; set; }
    }

    public class RicercaElettrodomesticoViewModel : RicercaOggettoViewModel
    {
        public RicercaElettrodomesticoViewModel() : base() { }

        public RicercaElettrodomesticoViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { }

        [Range(1, int.MaxValue)]
        public int? cerca_modelloID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Model", ResourceType = typeof(App_GlobalResources.Language))]
        public string cerca_modello { get; set; }
    }

    public class RicercaStrumentiViewModel : RicercaOggettoViewModel
    {
        public RicercaStrumentiViewModel() : base() { }

        public RicercaStrumentiViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { }

        [Range(1, int.MaxValue)]
        public int? cerca_modelloID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Model", ResourceType = typeof(App_GlobalResources.Language))]
        public string cerca_modello { get; set; }
    }

    public class RicercaMusicaViewModel : RicercaOggettoViewModel
    {
        public RicercaMusicaViewModel() : base() { }

        public RicercaMusicaViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { }

        [Range(1, int.MaxValue)]
        public int? cerca_formatoID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Format", ResourceType = typeof(App_GlobalResources.Language))]
        public string cerca_formato { get; set; }

        [Range(1, int.MaxValue)]
        public int? cerca_artistaID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Artist", ResourceType = typeof(App_GlobalResources.Language))]
        public string cerca_artista { get; set; }
    }
    
    public class RicercaGiocoViewModel : RicercaOggettoViewModel
    {
        public RicercaGiocoViewModel() : base() { }

        public RicercaGiocoViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { }

        [Range(1, int.MaxValue)]
        public int? cerca_modelloID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Model", ResourceType = typeof(App_GlobalResources.Language))]
        public string cerca_modello { get; set; }
    }

    public class RicercaVideogamesViewModel : RicercaOggettoViewModel
    {
        public RicercaVideogamesViewModel() : base() { }

        public RicercaVideogamesViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { }

        [Range(1, int.MaxValue)]
        public int? cerca_piattaformaID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Platform", ResourceType = typeof(App_GlobalResources.Language))]
        public string cerca_piattaforma { get; set; }

        [Range(1, int.MaxValue)]
        public int? cerca_genereID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Kind", ResourceType = typeof(App_GlobalResources.Language))]
        public string cerca_genere { get; set; }
    }

    public class RicercaSportViewModel : RicercaOggettoViewModel
    {
        public RicercaSportViewModel() : base() { }

        public RicercaSportViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { }

        [Range(1, int.MaxValue)]
        public int? cerca_modelloID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Model", ResourceType = typeof(App_GlobalResources.Language))]
        public string cerca_modello { get; set; }
    }

    public class RicercaTecnologiaViewModel : RicercaOggettoViewModel
    {
        public RicercaTecnologiaViewModel() : base() { }

        public RicercaTecnologiaViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { }

        [Range(1, int.MaxValue)]
        public int? cerca_modelloID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Model", ResourceType = typeof(App_GlobalResources.Language))]
        public string cerca_modello { get; set; }
    }

    public class RicercaConsoleViewModel : RicercaOggettoViewModel
    {
        public RicercaConsoleViewModel() : base() { }

        public RicercaConsoleViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { }

        [Range(1, int.MaxValue)]
        public int? cerca_piattaformaID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Platform", ResourceType = typeof(App_GlobalResources.Language))]
        public string cerca_piattaforma { get; set; }
    }

    public class RicercaVideoViewModel : RicercaOggettoViewModel
    {
        public RicercaVideoViewModel() : base() { }

        public RicercaVideoViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { }

        [Range(1, int.MaxValue)]
        public int? cerca_formatoID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Support", ResourceType = typeof(App_GlobalResources.Language))]
        public string cerca_formato { get; set; }

        [Range(1, int.MaxValue)]
        public int? cerca_registaID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Director", ResourceType = typeof(App_GlobalResources.Language))]
        public string cerca_regista { get; set; }
    }

    public class RicercaLibroViewModel : RicercaOggettoViewModel
    {
        public RicercaLibroViewModel() : base() { }

        public RicercaLibroViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { }

        [Range(1, int.MaxValue)]
        public int? cerca_autoreID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Author", ResourceType = typeof(App_GlobalResources.Language))]
        public string cerca_autore { get; set; }
    }

    public class RicercaVeicoloViewModel : RicercaOggettoViewModel
    {
        public RicercaVeicoloViewModel() : base() { }

        public RicercaVeicoloViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { }

        [Range(1, int.MaxValue)]
        public int? cerca_modelloID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Model", ResourceType = typeof(App_GlobalResources.Language))]
        public string cerca_modello { get; set; }

        [Range(1, int.MaxValue)]
        public int? cerca_alimentazioneID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Feed", ResourceType = typeof(App_GlobalResources.Language))]
        public string cerca_alimentazione { get; set; }
    }

    public class RicercaVestitoViewModel : RicercaOggettoViewModel
    {
        public RicercaVestitoViewModel() : base() { }

        public RicercaVestitoViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { }

        [DataType(DataType.Text)]
        [Display(Name = "Size", ResourceType = typeof(App_GlobalResources.Language))]
        public string cerca_taglia { get; set; }
    }
    #endregion

    #region SERVIZI
    public class RicercaServizioViewModel : RicercaViewModel
    {
        public RicercaServizioViewModel() { }

        public RicercaServizioViewModel(int numeroRecordTrovati = 0) : base(numeroRecordTrovati) { }

        public RicercaServizioViewModel(RicercaViewModel ricerca)
        {
            this.Cerca_Nome = ricerca.Cerca_Nome;
            this.Cerca_Categoria = ricerca.Cerca_Categoria;
            this.Cerca_IDCategoria = ricerca.Cerca_IDCategoria;
        }

        [Display(Name = "Monday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Cerca_Lunedi { get; set; }

        [Display(Name = "Tuesday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Cerca_Martedi { get; set; }

        [Display(Name = "Wednesday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Cerca_Mercoledi { get; set; }

        [Display(Name = "Thursday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Cerca_Giovedi { get; set; }

        [Display(Name = "Friday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Cerca_Venerdi { get; set; }

        [Display(Name = "Saturday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Cerca_Sabato { get; set; }

        [Display(Name = "Sunday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Cerca_Domenica { get; set; }

        [Display(Name = "AllDays", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Cerca_Tutti { get; set; }

        [Display(Name = "Rate", ResourceType = typeof(App_GlobalResources.Language))]
        public Tariffa Tariffa { get; set; }
        /*
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "StartTime", ResourceType = typeof(App_GlobalResources.Language))]
        public TimeSpan? Cerca_OraInizio { get; set; }

        //[DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "EndTime", ResourceType = typeof(App_GlobalResources.Language))]
        public TimeSpan? Cerca_OraFine { get; set; }

        [Display(Name = "Holidays", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Cerca_Festivi { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Cerca_OraFine < Cerca_OraInizio)
            {
                yield return new ValidationResult(App_GlobalResources.Language.ErrorEndTime);
            }
        }
        */
    }
    #endregion

    #endregion

    #region ELENCHI OGGETTI/SERVIZI

    public interface IListaOggetti<T> : IList<T>
    {
        List<T> List { get; set; }

        int PageCount { get; set; }

        int PageNumber { get; set; }
    }

    public class ListaOggetti
    {
        public virtual List<OggettoViewModel> List { get; set; }

        public int PageCount { get; set; }

        public int PageNumber { get; set; }

        public int TotalNumber { get; set; }
    }

    public class ListaServizi
    {
        public virtual List<ServizioViewModel> List { get; set; }

        public int PageCount { get; set; }

        public int PageNumber { get; set; }

        public int TotalNumber { get; set; }
    }

    public class ListaVendite
    {
        public ListaVendite(int idCategoria, string categoria)
        {
            this.IdCategoria = idCategoria;
            this.Categoria = categoria;
        }

        public virtual List<AnnuncioViewModel> List { get; set; }

        public int PageCount { get; set; }

        public int PageNumber { get; set; }

        public int TotalNumber { get; set; }

        public int IdCategoria { get; set; }

        public string Categoria { get; set; }
    }

    #endregion

    public class RegistrazioneRicercaViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessageResourceName = "ErrorFormatEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [StringLength(200, ErrorMessageResourceName = "ErrorLengthEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public string Email { get; set; }
    }
}
