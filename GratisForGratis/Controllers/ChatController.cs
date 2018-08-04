using GratisForGratis.Models;
using GratisForGratis.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GratisForGratis.Controllers
{
    public class ChatController : Controller
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
                List<CHAT> lista = db.CHAT.Where(m => m.STATO == (int)Stato.ATTIVO && 
                    (m.ID_MITTENTE == utente.Persona.ID || m.ID_DESTINATARIO == utente.Persona.ID))
                    .OrderByDescending(m => m.DATA_MODIFICA)
                    .GroupBy(m => new { m.ID_MITTENTE, m.ID_DESTINATARIO })
                    .Select(m => m.FirstOrDefault())
                    .ToList();
                for(int i = 0; i < lista.Count; i++)
                {
                    CHAT chat = lista[i];
                    PERSONA personaChat = (chat.ID_MITTENTE == utente.Persona.ID) ? chat.PERSONA : chat.PERSONA1;
                    if (i == 0)
                    {
                        viewModel.UltimaChat = new ChatViewModel(chat);
                        viewModel.listaChat = new List<PersonaModel>();
                    }
                    PersonaModel personaModel = new PersonaModel(personaChat);
                    viewModel.listaChat.Add(personaModel);
                }
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

                    List<CHAT> lista = db.CHAT.Where(m => m.STATO == (int)Stato.ATTIVO &&
                    (m.ID_MITTENTE == utente.Persona.ID && m.PERSONA1.TOKEN.ToString() == token) || 
                    (m.ID_DESTINATARIO == utente.Persona.ID && m.PERSONA.TOKEN.ToString() == token))
                    .OrderByDescending(m => m.DATA_MODIFICA)
                    .ToList();

                    for (int i = 0; i < lista.Count; i++)
                    {
                        CHAT chat = lista[i];
                        if (i == 0)
                        {
                            viewModel.listaChat = new List<ChatViewModel>();
                        }
                        ChatViewModel chatViewModel = new ChatViewModel(chat);
                        viewModel.listaChat.Add(chatViewModel);
                    }
                }
            }
            // apre gli ultimi 30 messaggi con l'utente se selezionato, altrimenti ti fa selezionare la persona
            return View(viewModel);
        }
        #endregion

        #region AJAX
        
        [HttpGet]
        [Filters.ValidateAjax]
        public ActionResult FormInserimento()
        {
            return View("PartialPages/_FormMessaggio");
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
            return View("PartialPages/_FormMessaggio", viewModel);
        }

        [HttpPost]
        [Filters.ValidateAjax]
        public ActionResult Invia(ChatViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    db.CHAT.Add(viewModel.GetModel());
                    if (db.SaveChanges() > 0)
                    {
                        // aggiornare tramite javascript o lato server la pagina (return PartialView)
                        //return Json(true);
                        return View("PartialPages/_FormMessaggio");
                    }
                }
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return View("PartialPages/_FormMessaggio", viewModel);
            //return Json(false);
        }

        [HttpPost]
        [Filters.ValidateAjax]
        public ActionResult Modifica(ChatViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    CHAT model = viewModel.GetModel();
                    db.Entry<CHAT>(model).State = System.Data.Entity.EntityState.Modified;
                    db.CHAT.Attach(model);
                    if (db.SaveChanges() > 0)
                    {
                        // aggiornare tramite javascript o lato server la pagina (return PartialView)
                        //return Json(true);
                        return View("PartialPages/_FormMessaggio");
                    }
                }
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return View("PartialPages/_FormMessaggio",viewModel);
            //return Json(false);
        }

        [HttpDelete]
        [Filters.ValidateAjax]
        public JsonResult Elimina(int id)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                PersonaModel utente = Session["utente"] as PersonaModel;
                CHAT chat = db.CHAT.SingleOrDefault(m => m.ID == id);
                if (chat != null)
                {
                    db.CHAT.Attach(chat);
                    db.CHAT.Remove(chat);
                    if (db.SaveChanges() > 0)
                    {
                        // aggiornare tramite javascript o lato server la pagina (return PartialView)
                        return Json(true);
                    }
                }
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return Json(false);
        }
        #endregion
    }
}