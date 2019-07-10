using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GratisForGratis.Models
{
    public enum Spedizione
    {
        [Display(Name = "PrivateShipment", ResourceType = typeof(App_GlobalResources.Enum))]
        Privata = 1,
        [Display(Name = "OnLineShipment", ResourceType = typeof(App_GlobalResources.Enum))]
        Online = 2
    }
    
    public enum TipoScambio
    {
        [Display(Name = "HandleShipment", ResourceType = typeof(App_GlobalResources.Enum))]
        AMano = 0,
        [Display(Name = "Shipment", ResourceType = typeof(App_GlobalResources.Enum))]
        Spedizione = 1
    }

    public enum TipoAcquisto
    {
        Oggetto = 0,
        Servizio = 1
    }

    /*
    public enum TipoOfferta
    {
        [Display(Name = "Points", ResourceType = typeof(App_GlobalResources.Language))]
        Punti = 0,
        [Display(Name = "Barter", ResourceType = typeof(App_GlobalResources.Language))]
        Baratto = 1,
    }
    */
    public enum TipoSegnalazione
    {
        [Display(Name = "Bug", ResourceType = typeof(App_GlobalResources.Language))]
        Bug = 0,
        [Display(Name = "Improvement", ResourceType = typeof(App_GlobalResources.Language))]
        Miglioramento = 1
    }

    public enum TipoBonus
    {
        Iscrizione = 0,
        PubblicazioneIniziale = 1,
        IscrizionePartner = 2,
        BonusPartner = 3,
        Login = 4,
        AnnuncioCompleto = 5,
        CanalePubblicitario = 6
    }

    public enum RuoloProfilo
    {
        Proprietario = 0,
        Amministratore = 1,
        Operatore = 2
    }

    // usato nella tabella TRANSAZIONE
    public enum TipoTransazione
    {
        Annuncio = 0,
        Bonifico = 1,
        Gxg = 2,
        BonusIscrizione = 3,
        BonusPubblicazioneIniziale = 4,
        BonusIscrizionePartner = 5,
        BonusPartner = 6,
        BonusLogin = 7,
        BonusFeedback = 8,
        BonusCanalePubblicitario = 9,
        BonusAnnuncioCompleto = 10,
        BonusSegnalazioneErrore = 11,
        BonusSuggerimentoAttivazioneAnnuncio = 12,
        BonusInvitaAmicoFB = 13,
        BonusAttivaHappyPage = 14
    }

    public enum TipoEmail
    {
        Contatto = 0,
        Registrazione = 1
    }

    public enum TipoTelefono
    {
        Privato = 0,
        Ufficio = 1
    }

    public enum TipoIndirizzo
    {
        Residenza = 0,
        Domicilio = 1,
        Spedizione = 2
    }

    public enum TipoVenditore
    {
        Persona = 0,
        Attivita = 1
    }

    public enum TipoNotifica
    {
        [Display(Name = "NewsAdActive", ResourceType = typeof(App_GlobalResources.Language))]
        AttivaAnnuncio = 0,
        [Display(Name = "BidPayment", ResourceType = typeof(App_GlobalResources.Language))]
        OffertaPagata = 1,
        [Display(Name = "Bonus", ResourceType = typeof(App_GlobalResources.Language))]
        Bonus = 2,
        [Display(Name = "NotificationTypeBidReceveid", ResourceType = typeof(App_GlobalResources.Enum))]
        OffertaRicevuta = 3,
        [Display(Name = "NotificationTypeAdSelled", ResourceType = typeof(App_GlobalResources.Enum))]
        AnnuncioVenduto = 4,
        [Display(Name = "NotificationTypeAdPurchase", ResourceType = typeof(App_GlobalResources.Enum))]
        AnnuncioAcquistato = 5,
        [Display(Name = "NotificationTypeBidRefused", ResourceType = typeof(App_GlobalResources.Enum))]
        OffertaRifiutata = 6,
        [Display(Name = "NotificationTypeBidAccepted", ResourceType = typeof(App_GlobalResources.Enum))]
        OffertaAccettata = 7,
    }
    
    /*
    public enum TipoPagamento
    {
        [Display(Name = "All", ResourceType = typeof(App_GlobalResources.Language))]
        Qualunque = 0,
        [Display(Name = "Points", ResourceType = typeof(App_GlobalResources.Language))]
        Vendita = 1,
        [Display(Name = "Barter", ResourceType = typeof(App_GlobalResources.Language))]
        Baratto = 2,
    }
    */
    public enum TipoPagamento
    {
        [Display(Name = "Moneta", ResourceType = typeof(App_GlobalResources.Enum))]
        HAPPY = 0,
        [Display(Name = "RealMoney", ResourceType = typeof(App_GlobalResources.Enum))]
        REALE = 1,
    }

    public enum TipoValuta
    {
        EUR = 1,
        USD = 2
    }

    public enum TipoCartaCredito
    {
        PayPal = -1,
        Visa = 0,
        Mastercard = 1,
        Discover = 2,
        Amex = 3,
    }

    public enum TipoSbloccoAnnuncio
    {
        SospensioneAnnuncio = 0,
        FineVendita = 1,
        DirittiAutore = 2,
        Illegale = 3
    }

    public enum TipoFeedback
    {
        Venditore = 0,
        Acquirente = 1
    }

    public enum TipoChat
    {
        WhatsApp = 0,
        Telegram = 1
    }

    public enum TipoUpload
    {
        Pdf = 0,
        Immagine = 1,
        Video = 2,
        Spedizioni = 3
    }

    public enum TipoCommissione
    {
        Spedizione = 0,
        Annuncio = 1,
        RimborsoSpese = 2
    }
}
