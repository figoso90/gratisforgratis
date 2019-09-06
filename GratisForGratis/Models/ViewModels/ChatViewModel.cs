using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GratisForGratis.Models.ViewModels
{
    public class ChatViewModel
    {
        #region PROPRIETA

        public int Id { get; set; }

        [Display(Name = "ChatSender", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public int MittenteId { get; set; }

        public int? MittenteIdAttivita { get; set; }

        public PersonaModel Mittente { get; set; }

        [Required]
        [Display(Name = "ChatRecipient", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public int DestinatarioId { get; set; }

        public int? DestinatarioIdAttivita { get; set; }

        public PersonaModel Destinatario { get; set; }

        [Required]
        [Display(Name ="ChatMessage", ResourceType = typeof(App_GlobalResources.ViewModel))]
        [StringLength(10000, MinimumLength = 1, ErrorMessageResourceName = "ChatLengthMessage", ErrorMessageResourceType = typeof(App_GlobalResources.ErrorResource))]
        public string Testo { get; set; }

        [Display(Name = "ChatDate", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public DateTime DataInserimento { get; set; }

        [Display(Name = "ChatDateUpdate", ResourceType = typeof(App_GlobalResources.ViewModel))]
        public DateTime? DataModifica { get; set; }

        public StatoChat Stato { get; set; }
        #endregion

        #region COSTRUTTORI
        public ChatViewModel() { }

        public ChatViewModel(int destinatarioId)
        {
            this.DestinatarioId = destinatarioId;
        }

        public ChatViewModel(CHAT model)
        {
            SetModel(model);
        }
        #endregion

        #region METODI PUBBLICI

        public void SetModel(CHAT model)
        {
            Id = model.ID;
            Mittente = new PersonaModel(model.PERSONA);
            MittenteId = model.PERSONA.ID;
            Destinatario = new PersonaModel(model.PERSONA1);
            DestinatarioId = model.PERSONA1.ID;
            Testo = model.TESTO;
            DataInserimento = model.DATA_INSERIMENTO;
            Stato = (StatoChat)model.STATO;
        }

        public CHAT GetModel()
        {
            CHAT model = new CHAT();
            model.ID = this.Id;
            model.ID_MITTENTE = this.MittenteId;
            model.ID_DESTINATARIO = this.DestinatarioId;
            model.TESTO = this.Testo;
            model.DATA_INSERIMENTO = (DateTime)this.DataInserimento;
            model.DATA_MODIFICA = null;
            model.STATO = (int)this.Stato;
            return model;
        }
        
        public static List<ChatViewModel> GetListaChat(DatabaseContext db, int idUtente, int idUtente2)
        {
            List<ChatViewModel> listaChat = new List<ChatViewModel>();

            db.CHAT
                .Include(m => m.PERSONA)
                .Include(m => m.PERSONA1)
                .Where(m => m.STATO != (int)StatoChat.ELIMINATO &&
                (m.ID_MITTENTE == idUtente && m.PERSONA1.ID == idUtente2) ||
                (m.ID_DESTINATARIO == idUtente && m.PERSONA.ID == idUtente2))
            .OrderByDescending(m => m.DATA_MODIFICA)
            .ToList().ForEach(m =>
            {
                m.DATA_MODIFICA = DateTime.Now;
                m.STATO = (int)StatoChat.LETTO;
                db.CHAT.Attach(m);
                var entry = db.Entry(m);
                entry.State = EntityState.Modified;
                db.SaveChanges();

                ChatViewModel chatViewModel = new ChatViewModel(m);
                listaChat.Add(chatViewModel);
            });

            return listaChat;
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

    public class ChatPortaleWebViewModel
    {
        #region PROPRIETA
        public List<ChatViewModel> listaChat { get; set; }

        public AttivitaModel Attivita { get; set; }
        #endregion

        #region COSTRUTTORI
        public ChatPortaleWebViewModel()
        {
            listaChat = new List<ChatViewModel>();
        }

        public ChatPortaleWebViewModel(ATTIVITA model)
        {
            listaChat = new List<ChatViewModel>();
        }
        #endregion
    }

}