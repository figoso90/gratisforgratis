using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GratisForGratis.Models.ViewModels
{
    public class ChatViewModel
    {
        #region PROPRIETA
        public int MittenteId { get; set; }

        public PersonaModel Mittente { get; set; }

        [Required]
        public int DestinatarioId { get; set; }

        public PersonaModel Destinatario { get; set; }

        [Required]
        public string Testo { get; set; }

        public DateTime DataInserimento { get; set; }

        public Stato Stato { get; set; }
        #endregion

        #region COSTRUTTORI
        public ChatViewModel() { }

        public ChatViewModel(CHAT model)
        {
            SetModel(model);
        }
        #endregion

        #region METODI PUBBLICI

        public void SetModel(CHAT model)
        {
            Mittente = new PersonaModel(model.PERSONA);
            MittenteId = model.PERSONA.ID;
            Destinatario = new PersonaModel(model.PERSONA1);
            DestinatarioId = model.PERSONA1.ID;
            Testo = model.TESTO;
            DataInserimento = model.DATA_INSERIMENTO;
            Stato = (Stato)model.STATO;
        }

        public CHAT GetModel()
        {
            PersonaModel utente = HttpContext.Current.Session["utente"] as PersonaModel;
            CHAT model = new CHAT();
            model.ID_MITTENTE = utente.Persona.ID;
            model.DATA_INSERIMENTO = DateTime.Now;
            model.ID_DESTINATARIO = this.DestinatarioId;
            model.TESTO = this.Testo;
            model.STATO = (int)this.Stato;
            return model;
        }
        #endregion
    }

    public class ChatIndexViewModel
    {
        #region PROPRIETA
        public List<PersonaModel> listaChat { get; set; }

        public ChatViewModel UltimaChat { get; set; }
        #endregion

        #region COSTRUTTORI
        public ChatIndexViewModel()
        {
            listaChat = new List<PersonaModel>();
        }
        #endregion
    }

    public class ChatUtenteViewModel
    {
        #region PROPRIETA
        public List<ChatViewModel> listaChat { get; set; }

        public PersonaModel Utente { get; set; }
        #endregion

        #region COSTRUTTORI
        public ChatUtenteViewModel()
        {
            listaChat = new List<ChatViewModel>();
        }

        public ChatUtenteViewModel(PERSONA model)
        {
            listaChat = new List<ChatViewModel>();
        }
        #endregion
    }

}