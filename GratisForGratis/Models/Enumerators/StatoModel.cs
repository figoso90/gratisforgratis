using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GratisForGratis.Models
{
    public enum Stato
    {
        [Display(Name = "StateInactive", ResourceType = typeof(App_GlobalResources.Language))]
        INATTIVO = 0,
        [Display(Name = "StateActive", ResourceType = typeof(App_GlobalResources.Language))]
        ATTIVO = 1,
        [Display(Name = "StateDelete", ResourceType = typeof(App_GlobalResources.Language))]
        ELIMINATO = 2,
        [Display(Name = "StatePause", ResourceType = typeof(App_GlobalResources.Language))]
        SOSPESO = 3
    }

    public enum StatoVendita
    {
        [Display(Name = "StateSellInactive", ResourceType = typeof(App_GlobalResources.Language))]
        INATTIVO = 0, // non visibile
        [Display(Name = "StateSellActive", ResourceType = typeof(App_GlobalResources.Language))]
        ATTIVO = 1, // in vendita
        [Display(Name = "StateSellDelete", ResourceType = typeof(App_GlobalResources.Language))]
        ELIMINATO = 2, // annullata
        [Display(Name = "StateSellPause", ResourceType = typeof(App_GlobalResources.Language))]
        SOSPESO = 3, // in attesa di pagamento su vendita diretta
        [Display(Name = "StateSellSold", ResourceType = typeof(App_GlobalResources.Language))]
        VENDUTO = 4, // pagamento ricevuto
        [Display(Name = "StateSellProgressBarter", ResourceType = typeof(App_GlobalResources.Language))]
        BARATTOINCORSO = 5, // in corso un baratto
        [Display(Name = "StateSellBarter", ResourceType = typeof(App_GlobalResources.Language))]
        BARATTATO = 6, // baratto effettuato
        [Display(Name = "StateSellPauseForBid", ResourceType = typeof(App_GlobalResources.Enum))]
        SOSPESOPEROFFERTA = 7, // in attesa pagamento per offerta
    }

    public enum StatoOfferta
    {
        [Display(Name = "StateBidInactive", ResourceType = typeof(App_GlobalResources.Language))]
        INATTIVA = 0,
        [Display(Name = "StateBidActive", ResourceType = typeof(App_GlobalResources.Language))]
        ATTIVA = 1,
        [Display(Name = "StateBidDelete", ResourceType = typeof(App_GlobalResources.Language))]
        ANNULLATA = 2,
        [Display(Name = "StateBidPause", ResourceType = typeof(App_GlobalResources.Language))]
        SOSPESA = 3,
        [Display(Name = "StateBidAccepted", ResourceType = typeof(App_GlobalResources.Language))]
        ACCETTATA = 4,
        [Display(Name = "StateWaitingPayment", ResourceType = typeof(App_GlobalResources.Enum))]
        ACCETTATA_ATTESO_PAGAMENTO = 5 // quando sto eseguendo pagamento paypal per offerta e ho una sessione stabilita

}
    // NON PIù IN USO
    public enum StatoBaratto
    {
        [Display(Name = "StateInactive", ResourceType = typeof(App_GlobalResources.Language))]
        INATTIVO = 0,
        [Display(Name = "StateActive", ResourceType = typeof(App_GlobalResources.Language))]
        ATTIVO = 1,
        [Display(Name = "StateCancel", ResourceType = typeof(App_GlobalResources.Language))]
        ANNULLATO = 2,
        [Display(Name = "StatePause", ResourceType = typeof(App_GlobalResources.Language))]
        SOSPESO = 3,
        [Display(Name = "StateAccept", ResourceType = typeof(App_GlobalResources.Language))]
        ACCETTATO = 4
    }

    public enum StatoPagamento
    {
        [Display(Name = "StateInactive", ResourceType = typeof(App_GlobalResources.Language))]
        INATTIVO = 0,
        [Display(Name = "StateActive", ResourceType = typeof(App_GlobalResources.Language))]
        ATTIVO = 1,
        [Display(Name = "StateCancel", ResourceType = typeof(App_GlobalResources.Language))]
        ANNULLATO = 2,
        [Display(Name = "StatePause", ResourceType = typeof(App_GlobalResources.Language))]
        SOSPESO = 3,
        [Display(Name = "StateAccept", ResourceType = typeof(App_GlobalResources.Language))]
        ACCETTATO = 4
    }

    public enum StatoMoneta
    {
        [Display(Name = "StateMoneyInactive", ResourceType = typeof(App_GlobalResources.Language))]
        INATTIVA = 0,
        [Display(Name = "StateMoneyActive", ResourceType = typeof(App_GlobalResources.Language))]
        ATTIVA = 1,
        [Display(Name = "StateMoneyDelete", ResourceType = typeof(App_GlobalResources.Language))]
        ELIMINATA = 2,
        [Display(Name = "StateMoneyPause", ResourceType = typeof(App_GlobalResources.Language))]
        SOSPESA = 3,
        [Display(Name = "StateMoneyAssigned", ResourceType = typeof(App_GlobalResources.Language))]
        ASSEGNATA = 4,
        CEDUTA = 5
    }

    public enum StatoNotifica
    {
        [Display(Name = "StateNewsInactive", ResourceType = typeof(App_GlobalResources.Language))]
        INATTIVA = 0,
        [Display(Name = "StateNewsActive", ResourceType = typeof(App_GlobalResources.Language))]
        ATTIVA = 1,
        [Display(Name = "StateNewsDelete", ResourceType = typeof(App_GlobalResources.Language))]
        ELIMINATA = 2,
        [Display(Name = "StateNewsPause", ResourceType = typeof(App_GlobalResources.Language))]
        SOSPESA = 3,
        [Display(Name = "StateNewsRead", ResourceType = typeof(App_GlobalResources.Language))]
        LETTA = 4
    }

    public enum StatoSpedizione
    {
        [Display(Name = "StateShipmentInactive", ResourceType = typeof(App_GlobalResources.Enum))]
        INATTIVA = 0,
        [Display(Name = "StateShipmentActive", ResourceType = typeof(App_GlobalResources.Enum))]
        ATTIVA = 1,
        [Display(Name = "StateShipmentDelete", ResourceType = typeof(App_GlobalResources.Enum))]
        ELIMINATA = 2,
        [Display(Name = "StateShipmentPause", ResourceType = typeof(App_GlobalResources.Enum))]
        SOSPESA = 3,
        [Display(Name = "StateShipmentLDV", ResourceType = typeof(App_GlobalResources.Enum))]
        LDV = 4,
        [Display(Name = "StateShipmentOK", ResourceType = typeof(App_GlobalResources.Enum))]
        EFFETTUATA = 5,
        [Display(Name = "StateShipmentPay", ResourceType = typeof(App_GlobalResources.Enum))]
        PAGATA = 6
    }

    public enum StatoChat
    {
        [Display(Name = "StateInactive", ResourceType = typeof(App_GlobalResources.Language))]
        BOZZA = 0,
        [Display(Name = "StateActive", ResourceType = typeof(App_GlobalResources.Language))]
        INVIATO = 1,
        [Display(Name = "StateDelete", ResourceType = typeof(App_GlobalResources.Language))]
        ELIMINATO = 2,
        [Display(Name = "StatePause", ResourceType = typeof(App_GlobalResources.Language))]
        ELIMINATO_DESTINATARIO = 3,
        [Display(Name = "StatePause", ResourceType = typeof(App_GlobalResources.Language))]
        LETTO = 4
    }

    public enum StatoCredito
    {
        [Display(Name = "StateCreditInactive", ResourceType = typeof(App_GlobalResources.Language))]
        INATTIVO = 0,
        [Display(Name = "StateCreditActive", ResourceType = typeof(App_GlobalResources.Language))]
        ATTIVO = 1,
        [Display(Name = "StateCreditDelete", ResourceType = typeof(App_GlobalResources.Language))]
        ELIMINATO = 2,
        [Display(Name = "StateCreditPause", ResourceType = typeof(App_GlobalResources.Language))]
        SOSPESO = 3,
        [Display(Name = "StateCreditAssigned", ResourceType = typeof(App_GlobalResources.Language))]
        ASSEGNATO = 4,
        [Display(Name = "StateCreditLost", ResourceType = typeof(App_GlobalResources.Language))]
        CEDUTO = 5,
        [Display(Name = "StateCreditExpire", ResourceType = typeof(App_GlobalResources.Language))]
        SCADUTO = 6
    }
}
