using GratisForGratis.Models;
using GratisForGratis.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace GratisForGratis.Controllers
{
    public class ChatController : AdvancedController
    {
        #region ACTION
        // GET: Chat
        public ActionResult Index()
        {
            ChatIndexViewModel viewModel = new ChatIndexViewModel();
            PersonaModel utente = Session["utente"] as PersonaModel;
            // riepilogo chat, con ultima chat per utente in evidenza
            using (DatabaseContext db = new DatabaseContext())
            {
                db.Database.Connection.Open();
                var a = db.CHAT
                    .Where(m => m.STATO == (int)Stato.ATTIVO &&
                        m.ID_MITTENTE == utente.Persona.ID)
                    .AsEnumerable()
                    .Select(m => new ChatViewModel()
                    {
                        Id = m.ID
                        ,MittenteId = m.ID_MITTENTE
                        ,Mittente = new PersonaModel(m.PERSONA)
                        ,DestinatarioId = m.ID_DESTINATARIO
                        ,Destinatario= new PersonaModel(m.PERSONA1)
                        ,Testo = m.TESTO
                        ,DataInserimento = m.DATA_INSERIMENTO
                        ,DataModifica = m.DATA_MODIFICA
                        ,Stato = (Stato)m.STATO
                    });
                var b = db.CHAT
                .Where(m => m.STATO == (int)Stato.ATTIVO &&
                    m.ID_DESTINATARIO == utente.Persona.ID)
                    .AsEnumerable()
                    .Select(m => new ChatViewModel()
                    {
                        Id = m.ID
                        ,MittenteId = m.ID_DESTINATARIO
                        ,Mittente = new PersonaModel(m.PERSONA1)
                        ,DestinatarioId = m.ID_MITTENTE
                        ,Destinatario= new PersonaModel(m.PERSONA)
                        ,Testo = m.TESTO
                        ,DataInserimento = m.DATA_INSERIMENTO
                        ,DataModifica = m.DATA_MODIFICA
                        ,Stato = (Stato)m.STATO
                    });
                var lista = a.Concat(b)
                    .OrderByDescending(m => m.DataModifica)
                    .OrderByDescending(m => m.DataInserimento)
                    .GroupBy(m => new { m.MittenteId, m.DestinatarioId })
                    .Select(m => m.FirstOrDefault())
                    .ToList();

                for (int i = 0; i < lista.Count; i++)
                {
                    ChatViewModel chat = lista[i];
                    PersonaModel personaChat = (chat.MittenteId == utente.Persona.ID) ? chat.Destinatario : chat.Mittente;
                    if (i == 0)
                    {
                        viewModel.UltimaChat = chat;
                        viewModel.listaChat = new List<PersonaModel>();
                    }
                    viewModel.listaChat.Add(personaChat);
                }

                //List<CHAT> lista = db.CHAT
                //    .Where(m => m.STATO == (int)Stato.ATTIVO &&
                //        m.ID_MITTENTE == utente.Persona.ID || m.ID_DESTINATARIO == utente.Persona.ID)
                //    .OrderByDescending(m => m.DATA_MODIFICA)
                //    .OrderByDescending(m => m.DATA_INSERIMENTO)
                //    .GroupBy(m => new { m.ID_MITTENTE, m.ID_DESTINATARIO })
                //    .Select(m => m.FirstOrDefault())
                //    .ToList();
                //for (int i = 0; i < lista.Count; i++)
                //{
                //    CHAT chat = lista[i];
                //    PERSONA personaChat = (chat.ID_MITTENTE == utente.Persona.ID) ? chat.PERSONA1 : chat.PERSONA;
                //    if (i == 0)
                //    {
                //        viewModel.UltimaChat = new ChatViewModel(chat);
                //        viewModel.listaChat = new List<PersonaModel>();
                //    }
                //    PersonaModel personaModel = new PersonaModel(personaChat);
                //    viewModel.listaChat.Add(personaModel);
                //}
            }
            return View(viewModel);
        }

        public ActionResult Utente(string token = "")
        {
            ChatUtenteViewModel viewModel = new ChatUtenteViewModel();
            PersonaModel utente = Session["utente"] as PersonaModel;
            if (!string.IsNullOrWhiteSpace(token))
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    PERSONA personaChat = db.PERSONA.SingleOrDefault(m => m.TOKEN.ToString() == token
                        && m.STATO != (int)Stato.ELIMINATO);
                    viewModel.Utente = new PersonaModel(personaChat);
                    viewModel.listaChat = GetListaChat(db, utente.Persona.ID, personaChat.ID);
                }
            }
            // apre gli ultimi 30 messaggi con l'utente se selezionato, altrimenti ti fa selezionare la persona
            return View(viewModel);
        }
        #endregion

        #region AJAX
        
        [HttpGet]
        [Filters.ValidateAjax]
        public ActionResult FormInserimento(int destinatarioId)
        {
            ChatViewModel viewModel = new ChatViewModel();
            viewModel.DestinatarioId = destinatarioId;
            return PartialView("PartialPages/_FormMessaggio", viewModel);
        }

        [HttpGet]
        [Filters.ValidateAjax]
        public ActionResult FormModifica(int id)
        {
            ChatViewModel viewModel = new ChatViewModel();
            using (DatabaseContext db = new DatabaseContext())
            {
                CHAT model = db.CHAT.SingleOrDefault(m => m.ID == id);
                if (model != null)
                {
                    viewModel.SetModel(model);
                }
            }
            return PartialView("PartialPages/_FormMessaggio", viewModel);
        }

        [HttpPost]
        [Filters.ValidateAjax]
        public ActionResult Invia(ChatViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    CHAT model = viewModel.GetModel();
                    PersonaModel utente = Session["utente"] as PersonaModel;
                    model.ID_MITTENTE = utente.Persona.ID;
                    model.DATA_INSERIMENTO = DateTime.Now;
                    model.STATO = (int)Stato.ATTIVO;
                    db.CHAT.Add(model);
                    if (db.SaveChanges() > 0)
                    {
                        // aggiornare tramite javascript o lato server la pagina (return PartialView)
                        //return Json(true);
                        //return PartialView("PartialPages/_FormMessaggio", new ChatViewModel());
                        return PartialView("PartialPages/_ListaChat", GetListaChat(db, model.ID_MITTENTE, model.ID_DESTINATARIO));
                    }
                }
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            //return PartialView("PartialPages/_FormMessaggio", viewModel);
            return null;
        }

        [HttpPost]
        [Filters.ValidateAjax]
        public ActionResult Modifica(ChatViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    PersonaModel utente = Session["utente"] as PersonaModel;
                    CHAT model = db.CHAT.SingleOrDefault(m => m.ID == viewModel.Id && m.ID_MITTENTE == utente.Persona.ID);
                    if (model != null)
                    {
                        model.TESTO = viewModel.Testo;
                        db.CHAT.Attach(model);
                        var entry = db.Entry(model);
                        entry.State = System.Data.Entity.EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            //return PartialView("PartialPages/_FormMessaggio", new ChatViewModel());
                            return PartialView("PartialPages/_ListaChat", GetListaChat(db, model.ID_MITTENTE, model.ID_DESTINATARIO));
                        }
                    }
                }
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            //return PartialView("PartialPages/_FormMessaggio",viewModel);
            return null;
        }

        [HttpDelete]
        [Filters.ValidateAjax]
        public ActionResult Elimina(int id)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                PersonaModel utente = Session["utente"] as PersonaModel;
                CHAT model = db.CHAT.SingleOrDefault(m => m.ID == id && m.ID_MITTENTE == utente.Persona.ID);
                if (model != null)
                {
                    db.CHAT.Attach(model);
                    db.CHAT.Remove(model);
                    if (db.SaveChanges() > 0)
                    {
                        return PartialView("PartialPages/_ListaChat", GetListaChat(db, model.ID_MITTENTE, model.ID_DESTINATARIO));
                    }
                }
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return null;
        }
        #endregion

        #region METODI PRIVATI
        private List<ChatViewModel> GetListaChat(DatabaseContext db, int idUtente, int idUtente2)
        {
            List<ChatViewModel> listaChat = new List<ChatViewModel>();

            List<CHAT> lista = db.CHAT
                .Include(m => m.PERSONA)
                .Include(m => m.PERSONA1)
                .Where(m => m.STATO == (int)Stato.ATTIVO &&
                (m.ID_MITTENTE == idUtente && m.PERSONA1.ID == idUtente2) ||
                (m.ID_DESTINATARIO == idUtente && m.PERSONA.ID == idUtente2))
            .OrderByDescending(m => m.DATA_MODIFICA)
            .ToList();

            for (int i = 0; i < lista.Count; i++)
            {
                CHAT chat = lista[i];
                ChatViewModel chatViewModel = new ChatViewModel(chat);
                listaChat.Add(chatViewModel);
            }
            return listaChat;
        }
        #endregion
    }
}