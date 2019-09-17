using GratisForGratis.Models;
using GratisForGratis.Models.ViewModels;
using System;
using System.Collections.Generic;
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
                    .Where(m => m.STATO != (int)StatoChat.ELIMINATO &&
                        m.ID_MITTENTE == utente.Persona.ID)
                    .AsEnumerable()
                    .Select(m => new ChatViewModel()
                    {
                        Id = m.ID
                        ,MittenteId = m.ID_MITTENTE
                        ,Mittente = new PersonaModel(m.PERSONA)
                        ,DestinatarioId = m.ID_DESTINATARIO
                        ,Destinatario= new PersonaModel(m.PERSONA1)
                        ,DestinatarioIdAttivita = m.ID_DESTINATARIO_ATTIVITA
                        ,DestinatarioAttivita = new AttivitaModel(m.ATTIVITA1)
                        ,Testo = m.TESTO
                        ,DataInserimento = m.DATA_INSERIMENTO
                        ,DataModifica = m.DATA_MODIFICA
                        ,Stato = (StatoChat)m.STATO
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
                        ,DestinatarioIdAttivita = m.ID_MITTENTE_ATTIVITA
                        ,DestinatarioAttivita = new AttivitaModel(m.ATTIVITA)
                        ,Testo = m.TESTO
                        ,DataInserimento = m.DATA_INSERIMENTO
                        ,DataModifica = m.DATA_MODIFICA
                        ,Stato = (StatoChat)m.STATO
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

                    chat.Mittente.Foto = chat.Mittente.Persona.PERSONA_FOTO.OrderByDescending(m => m.ORDINE).AsEnumerable()
                        .Select(m => new FotoModel(m.ALLEGATO)).ToList();
                    if (chat.DestinatarioId != null)
                    {
                        chat.Destinatario.Foto = chat.Destinatario.Persona.PERSONA_FOTO.OrderByDescending(m => m.ORDINE).AsEnumerable()
                        .Select(m => new FotoModel(m.ALLEGATO)).ToList();
                    }
                    if (chat.DestinatarioIdAttivita != null)
                    {
                        chat.DestinatarioAttivita.Foto = chat.DestinatarioAttivita.Attivita.ATTIVITA_FOTO.OrderByDescending(m => m.ORDINE).AsEnumerable()
                            .Select(m => new FotoModel(m.ALLEGATO)).ToList();
                    }
                    UtenteProfiloViewModel soggettoChat = null;
                    if (chat.MittenteId == utente.Persona.ID)
                    {
                        if (chat.DestinatarioIdAttivita != null)
                        {
                            soggettoChat = new UtenteProfiloViewModel(chat.DestinatarioAttivita);
                        }
                        else
                        {
                            soggettoChat = new UtenteProfiloViewModel(chat.Destinatario);
                        }
                    }
                    else
                    {
                        if (chat.MittenteIdAttivita != null)
                        {
                            soggettoChat = new UtenteProfiloViewModel(chat.MittenteAttivita);
                        }
                        else
                        {
                            soggettoChat = new UtenteProfiloViewModel(chat.Mittente);
                        }
                    }

                    if (i == 0)
                    {
                        viewModel.UltimaChat = chat;
                        viewModel.listaChat = new List<UtenteProfiloViewModel>();
                    }
                    viewModel.listaChat.Add(soggettoChat);
                }

                RefreshPunteggioUtente(db);
            }
            return View(viewModel);
        }

        public ActionResult IndexPortaleWeb(string token)
        {
            ChatIndexViewModel viewModel = new ChatIndexViewModel();
            PersonaModel utente = Session["utente"] as PersonaModel;
            var attivita = utente.Attivita.Where(m => m.ATTIVITA.TOKEN.ToString() == token).SingleOrDefault();
            if (attivita == null)
                return RedirectToAction("", "utente");
            // riepilogo chat, con ultima chat per utente in evidenza
            using (DatabaseContext db = new DatabaseContext())
            {
                db.Database.Connection.Open();
                var a = db.CHAT
                    .Where(m => m.STATO != (int)StatoChat.ELIMINATO &&
                        m.ID_MITTENTE_ATTIVITA == attivita.ID)
                    .AsEnumerable()
                    .Select(m => new ChatViewModel()
                    {
                        Id = m.ID
                        ,MittenteId = m.ID_MITTENTE
                        ,Mittente = new PersonaModel(m.PERSONA)
                        ,MittenteIdAttivita = m.ID_MITTENTE
                        ,MittenteAttivita = attivita
                        ,DestinatarioId = m.ID_DESTINATARIO
                        ,Destinatario = new PersonaModel(m.PERSONA1)
                        ,DestinatarioIdAttivita = m.ID_DESTINATARIO_ATTIVITA
                        ,DestinatarioAttivita = new AttivitaModel(m.ATTIVITA1)
                        ,Testo = m.TESTO
                        ,DataInserimento = m.DATA_INSERIMENTO
                        ,DataModifica = m.DATA_MODIFICA
                        ,Stato = (StatoChat)m.STATO
                    });
                var b = db.CHAT
                .Where(m => m.STATO == (int)Stato.ATTIVO &&
                    m.ID_DESTINATARIO_ATTIVITA == attivita.ID)
                    .AsEnumerable()
                    .Select(m => new ChatViewModel()
                    {
                        Id = m.ID
                        ,MittenteId = m.ID_DESTINATARIO
                        ,Mittente = new PersonaModel(m.PERSONA1)
                        ,MittenteIdAttivita = m.ID_DESTINATARIO_ATTIVITA
                        ,MittenteAttivita = attivita
                        ,DestinatarioId = m.ID_MITTENTE
                        ,Destinatario = new PersonaModel(m.PERSONA)
                        ,DestinatarioIdAttivita = m.ID_MITTENTE_ATTIVITA
                        ,DestinatarioAttivita = new AttivitaModel(m.ATTIVITA)
                        ,Testo = m.TESTO
                        ,DataInserimento = m.DATA_INSERIMENTO
                        ,DataModifica = m.DATA_MODIFICA
                        ,Stato = (StatoChat)m.STATO
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

                    chat.Mittente.Foto = chat.Mittente.Persona.PERSONA_FOTO.OrderByDescending(m => m.ORDINE).AsEnumerable()
                        .Select(m => new FotoModel(m.ALLEGATO)).ToList();
                    chat.MittenteAttivita.Foto = chat.MittenteAttivita.Attivita.ATTIVITA_FOTO.OrderByDescending(m => m.ORDINE).AsEnumerable()
                        .Select(m => new FotoModel(m.ALLEGATO)).ToList();
                    if (chat.DestinatarioId != null)
                    {
                        chat.Destinatario.Foto = chat.Destinatario.Persona.PERSONA_FOTO.OrderByDescending(m => m.ORDINE).AsEnumerable()
                        .Select(m => new FotoModel(m.ALLEGATO)).ToList();
                    }
                    if (chat.DestinatarioIdAttivita != null)
                    {
                        chat.DestinatarioAttivita.Foto = chat.DestinatarioAttivita.Attivita.ATTIVITA_FOTO.OrderByDescending(m => m.ORDINE).AsEnumerable()
                        .Select(m => new FotoModel(m.ALLEGATO)).ToList();
                    }
                    //UtenteProfiloViewModel soggettoChat = new UtenteProfiloViewModel((chat.MittenteIdAttivita == attivita.ID) ? chat.DestinatarioAttivita : chat.MittenteAttivita);
                    UtenteProfiloViewModel soggettoChat = null;
                    if (chat.MittenteIdAttivita == attivita.ID)
                    {
                        if (chat.DestinatarioIdAttivita != null)
                        {
                            soggettoChat = new UtenteProfiloViewModel(chat.DestinatarioAttivita);
                        }
                        else
                        {
                            soggettoChat = new UtenteProfiloViewModel(chat.Destinatario);
                        }
                    }
                    else
                    {
                        if (chat.MittenteIdAttivita != null)
                        {
                            soggettoChat = new UtenteProfiloViewModel(chat.MittenteAttivita);
                        }
                        else
                        {
                            soggettoChat = new UtenteProfiloViewModel(chat.Mittente);
                        }
                    }

                    if (i == 0)
                    {
                        viewModel.UltimaChat = chat;
                        viewModel.listaChat = new List<UtenteProfiloViewModel>();
                    }
                    viewModel.listaChat.Add(soggettoChat);
                }

                RefreshPunteggioUtente(db);
            }
            return View("Index", viewModel);
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
                    viewModel.Utente.Foto = personaChat.PERSONA_FOTO.OrderByDescending(m => m.ORDINE).AsEnumerable()
                        .Select(m => new FotoModel(m.ALLEGATO)).ToList();
                    viewModel.Messaggi = ChatViewModel.GetListaChat(db, utente.Persona.ID, personaChat.ID);
                    RefreshPunteggioUtente(db);
                }
            }
            // apre gli ultimi 30 messaggi con l'utente se selezionato, altrimenti ti fa selezionare la persona
            return View(viewModel);
        }

        public ActionResult PortaleWeb(string token = "")
        {
            ChatPortaleWebViewModel viewModel = new ChatPortaleWebViewModel();
            PersonaModel utente = Session["utente"] as PersonaModel;
            if (!string.IsNullOrWhiteSpace(token))
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    ATTIVITA attivitaChat = db.ATTIVITA.SingleOrDefault(m => m.TOKEN.ToString() == token
                        && m.STATO != (int)Stato.ELIMINATO);
                    viewModel.Attivita = new AttivitaModel(attivitaChat);
                    viewModel.Attivita.Foto = attivitaChat.ATTIVITA_FOTO.OrderByDescending(m => m.ORDINE)
                        .AsEnumerable().Select(m => new FotoModel(m.ALLEGATO)).ToList();
                    viewModel.Messaggi = ChatViewModel.GetListaChatAttivita(db, utente.Persona.ID, attivitaChat.ID);
                    RefreshPunteggioUtente(db);
                }
            }
            // apre gli ultimi 30 messaggi con l'utente se selezionato, altrimenti ti fa selezionare la persona
            return View(viewModel);
        }
        #endregion

        #region AJAX

        [HttpGet]
        [Filters.ValidateAjax]
        public ActionResult FormInserimento(int destinatarioId, int? attivitaId)
        {
            ChatViewModel viewModel = new ChatViewModel();
            viewModel.DestinatarioId = destinatarioId;
            viewModel.DestinatarioIdAttivita = attivitaId;
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
                    model.STATO = (int)StatoChat.INVIATO;
                    db.CHAT.Add(model);
                    if (db.SaveChanges() > 0)
                    {
                        // aggiornare tramite javascript o lato server la pagina (return PartialView)
                        //return Json(true);
                        //return PartialView("PartialPages/_FormMessaggio", new ChatViewModel());
                        return PartialView("PartialPages/_ListaChat", ChatViewModel.GetListaChat(db, model.ID_MITTENTE, (int)model.ID_DESTINATARIO));
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
                            return PartialView("PartialPages/_ListaChat", ChatViewModel.GetListaChat(db, model.ID_MITTENTE, (int)model.ID_DESTINATARIO));
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
                        return PartialView("PartialPages/_ListaChat", ChatViewModel.GetListaChat(db, model.ID_MITTENTE, (int)model.ID_DESTINATARIO));
                    }
                }
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return null;
        }
        #endregion
    }
}