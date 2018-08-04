using System.ComponentModel.DataAnnotations;

namespace GratisForGratis.Models
{
    public enum CondizioneOggetto
    {
        [Display(Name = "SecondHand", ResourceType = typeof(App_GlobalResources.Language))]
        Usato = 1,
        [Display(Name = "New", ResourceType = typeof(App_GlobalResources.Language))]
        Nuovo = 2,
        [Display(Name = "Broken", ResourceType = typeof(App_GlobalResources.Language))]
        Rotto = 3,
        [Display(Name = "Fix", ResourceType = typeof(App_GlobalResources.Language))]
        DaSistemare = 4
    }

    public enum Distanza
    {
        Cm = 0,
        M = 1
    }

    public enum Peso
    {
        G = 0,
        Kg = 1
    }

    public enum Tariffa
    {
        [Display(Name = "HourlyRate", ResourceType = typeof(App_GlobalResources.Language))]
        Oraria = 0,
        [Display(Name = "DailyRate", ResourceType = typeof(App_GlobalResources.Language))]
        Giornaliera = 1,
        [Display(Name = "MounthlyRate", ResourceType = typeof(App_GlobalResources.Language))]
        Mensile = 2,
        [Display(Name = "CompleteService", ResourceType = typeof(App_GlobalResources.Language))]
        ServizioCompleto = 3
    }

    public enum MessaggioNotifica
    {
        [Display(Name = "NewsAdActive", ResourceType = typeof(App_GlobalResources.Language))]
        AttivaAnnuncio = 0,
        [Display(Name = "BidPayment", ResourceType = typeof(App_GlobalResources.Language))]
        PagaOfferta = 1,
        [Display(Name = "Bonus", ResourceType = typeof(App_GlobalResources.Language))]
        Bonus = 1,
    }

    public enum DurataAnnuncio
    {
        [Display(Name = "LifeAdInfinity", ResourceType = typeof(App_GlobalResources.Enum))]
        Infinito = -1,
        [Display(Name = "LifeAd1", ResourceType = typeof(App_GlobalResources.Enum))]
        UnMese = 1,
        [Display(Name = "LifeAd2", ResourceType = typeof(App_GlobalResources.Enum))]
        DueMesi = 2,
        [Display(Name = "LifeAd3", ResourceType = typeof(App_GlobalResources.Enum))]
        TreMesi = 3,
        [Display(Name = "LifeAd4", ResourceType = typeof(App_GlobalResources.Enum))]
        QuattroMesi = 4,
        [Display(Name = "LifeAd5", ResourceType = typeof(App_GlobalResources.Enum))]
        CinqueMesi = 5,
        [Display(Name = "LifeAd6", ResourceType = typeof(App_GlobalResources.Enum))]
        SeiMesi = 6,
        [Display(Name = "LifeAd12", ResourceType = typeof(App_GlobalResources.Enum))]
        UnAnno = 12
    }

    public enum TempoImballaggio
    {
        [Display(Name = "TimePackage1", ResourceType = typeof(App_GlobalResources.Enum))]
        UnGiorno = 0,
        [Display(Name = "TimePackage2", ResourceType = typeof(App_GlobalResources.Enum))]
        DueGiorni = 1,
        [Display(Name = "TimePackage3", ResourceType = typeof(App_GlobalResources.Enum))]
        TreGiorni = 2
    }

    public enum GiorniSpedizione
    {
        [Display(Name = "ShipDay1", ResourceType = typeof(App_GlobalResources.Enum))]
        Lunedì = 0,
        [Display(Name = "ShipDay2", ResourceType = typeof(App_GlobalResources.Enum))]
        Martedì = 1,
        [Display(Name = "ShipDay3", ResourceType = typeof(App_GlobalResources.Enum))]
        Mercoledì = 2,
        [Display(Name = "ShipDay4", ResourceType = typeof(App_GlobalResources.Enum))]
        Giovedì = 3,
        [Display(Name = "ShipDay5", ResourceType = typeof(App_GlobalResources.Enum))]
        Venerdì = 4,
        [Display(Name = "ShipDay6", ResourceType = typeof(App_GlobalResources.Enum))]
        Sabato = 5
    }

    public enum OrariSpedizione
    {
        [Display(Name = "ShipHour9", ResourceType = typeof(App_GlobalResources.Enum))]
        Nove = 9,
        [Display(Name = "ShipHour10", ResourceType = typeof(App_GlobalResources.Enum))]
        Dieci = 10,
        [Display(Name = "ShipHour11", ResourceType = typeof(App_GlobalResources.Enum))]
        Undici = 11,
        [Display(Name = "ShipHour12", ResourceType = typeof(App_GlobalResources.Enum))]
        Dodici = 12,
        [Display(Name = "ShipHour13", ResourceType = typeof(App_GlobalResources.Enum))]
        Tredici = 13,
        [Display(Name = "ShipHour14", ResourceType = typeof(App_GlobalResources.Enum))]
        Quattordici = 14,
        [Display(Name = "ShipHour15", ResourceType = typeof(App_GlobalResources.Enum))]
        Quindici = 15,
        [Display(Name = "ShipHour16", ResourceType = typeof(App_GlobalResources.Enum))]
        Sedici = 16,
        [Display(Name = "ShipHour17", ResourceType = typeof(App_GlobalResources.Enum))]
        Diciassette = 17,
        [Display(Name = "ShipHour18", ResourceType = typeof(App_GlobalResources.Enum))]
        Diciotto = 18,
        [Display(Name = "ShipHour19", ResourceType = typeof(App_GlobalResources.Enum))]
        Diciannove = 19
    }

    public enum Month
    {
        [Display(Name = "January", ResourceType = typeof(App_GlobalResources.Enum))]
        January = 1,
        [Display(Name = "February", ResourceType = typeof(App_GlobalResources.Enum))]
        February = 2,
        [Display(Name = "March", ResourceType = typeof(App_GlobalResources.Enum))]
        March = 3,
        [Display(Name = "April", ResourceType = typeof(App_GlobalResources.Enum))]
        April = 4,
        [Display(Name = "May", ResourceType = typeof(App_GlobalResources.Enum))]
        May = 5,
        [Display(Name = "June", ResourceType = typeof(App_GlobalResources.Enum))]
        June = 6,
        [Display(Name = "July", ResourceType = typeof(App_GlobalResources.Enum))]
        July = 7,
        [Display(Name = "August", ResourceType = typeof(App_GlobalResources.Enum))]
        August = 8,
        [Display(Name = "September", ResourceType = typeof(App_GlobalResources.Enum))]
        September = 9,
        [Display(Name = "October", ResourceType = typeof(App_GlobalResources.Enum))]
        October = 10,
        [Display(Name = "November", ResourceType = typeof(App_GlobalResources.Enum))]
        November = 11,
        [Display(Name = "December", ResourceType = typeof(App_GlobalResources.Enum))]
        December = 12
    }

    public enum AzionePayPal
    {
        Acquisto = 0,
        Offerta = 1,
    }

    public enum EstensioneFile
    {
        Jpg = 0,
        Pdf = 1
    }
}
