using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using GratisForGratis.Models;
using System;
using System.Web;
using System.Drawing;
using System.IO;
using System.Web.Configuration;

namespace GratisForGratis.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        #region ACTION

        [HttpGet]
        public ActionResult Index(string promo = null)
        {
            try
            {
                // reset cookie
                HttpCookie currentUserCookie = Request.Cookies["GXG_promo"];
                if (currentUserCookie != null)
                {
                    Response.Cookies.Remove("GXG_promo");
                    currentUserCookie.Expires = DateTime.Now.AddDays(-10);
                    currentUserCookie.Value = null;
                    Response.SetCookie(currentUserCookie);
                }
                // memorizza arrivo da canale pubblicitario
                if (!string.IsNullOrWhiteSpace(promo) && !string.IsNullOrWhiteSpace(WebConfigurationManager.AppSettings["bonusPromozione" + promo]))
                {
                    HttpCookie cookiePromo = new HttpCookie("GXG_promo", promo);
                    cookiePromo.Expires = DateTime.Now.AddMinutes(1);
                    Response.SetCookie(cookiePromo);
                }

                ViewBag.Title = App_GlobalResources.MetaTag.TitleGeneric;
                ViewBag.Description = App_GlobalResources.MetaTag.DescriptionHome;
                ViewBag.Keywords = App_GlobalResources.MetaTag.KeywordsHome;

                HttpCookie cookie = HttpContext.Request.Cookies.Get("ricerca");
                var risultato = (HttpContext.Application["categorie"] as List<FINDSOTTOCATEGORIE_Result>).Where(c => c.LIVELLO == -1).FirstOrDefault();
                // verifico se ho trovato la categoria
                if (risultato != null)
                {
                    cookie["Categoria"] = risultato.DESCRIZIONE;
                    cookie["IDCategoria"] = risultato.ID.ToString();
                    //cookie["TipoAcquisto"] = ((int)cerca.Cerca_TipoAcquisto).ToString();
                    cookie["TipoAcquisto"] = risultato.TIPO_VENDITA.ToString();
                }
                // per verificare se reindirizza alla pagina di errore
                //int numero = Convert.ToInt32("so");
                HttpContext.Response.SetCookie(cookie);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
        }

        [HttpGet]
        public ActionResult Contatti()
        {
            ViewBag.Title = App_GlobalResources.Language.Contacts;
            ViewBag.Description = App_GlobalResources.MetaTag.DescriptionContatti;
            ViewBag.Keywords = App_GlobalResources.MetaTag.KeywordsContatti;

            return View();
        }

        [HttpGet]
        public ActionResult ComeFunziona()
        {
            ViewBag.Title = App_GlobalResources.Language.TitleHowWork;
            ViewBag.Description = string.Format(App_GlobalResources.MetaTag.DescriptionCosaFacciamo, 
                WebConfigurationManager.AppSettings["nomeSito"]);
            ViewBag.Keywords = App_GlobalResources.MetaTag.KeywordsCosaFacciamo;

            return View();
        }

        [HttpGet]
        public ActionResult PercheGratisForGratis()
        {
            ViewBag.Title = string.Format(App_GlobalResources.Language.TitleWhySite, WebConfigurationManager.AppSettings["nomeSito"]);
            ViewBag.Description = string.Format(App_GlobalResources.MetaTag.DescriptionCosaFacciamo,
                WebConfigurationManager.AppSettings["nomeSito"]);
            ViewBag.Keywords = App_GlobalResources.MetaTag.KeywordsCosaFacciamo;

            return View();
        }

        [HttpGet]
        public ActionResult Spedizione()
        {
            ViewBag.Title = App_GlobalResources.Language.TitleHowWork;
            ViewBag.Description = string.Format(App_GlobalResources.MetaTag.DescriptionCosaFacciamo,
                WebConfigurationManager.AppSettings["nomeSito"]);
            ViewBag.Keywords = App_GlobalResources.MetaTag.KeywordsCosaFacciamo;

            return View();
        }

        [HttpGet]
        public ActionResult GuidaSicura()
        {
            ViewBag.Title = App_GlobalResources.Language.TitleGuideSecurity;
            ViewBag.Description = string.Format(App_GlobalResources.MetaTag.DescriptionCosaFacciamo,
                WebConfigurationManager.AppSettings["nomeSito"]);
            ViewBag.Keywords = App_GlobalResources.MetaTag.KeywordsCosaFacciamo;

            return View();
        }

        [HttpGet]
        public ActionResult ServiziExtra()
        {
            ViewBag.Title = App_GlobalResources.Language.TitleExtraServices;
            ViewBag.Description = string.Format(App_GlobalResources.MetaTag.DescriptionCosaFacciamo,
                WebConfigurationManager.AppSettings["nomeSito"]);
            ViewBag.Keywords = App_GlobalResources.MetaTag.KeywordsCosaFacciamo;

            return View();
        }

        [HttpGet]
        public ActionResult Partners()
        {
            ViewBag.Title = App_GlobalResources.Language.TitlePartners;
            ViewBag.Description = App_GlobalResources.MetaTag.DescriptionPartners;
            ViewBag.Keywords = App_GlobalResources.MetaTag.KeywordsPartners;

            return View();
        }

        [HttpGet]
        public ActionResult Privacy()
        {
            ViewBag.Title = App_GlobalResources.Language.TitlePrivacy;
            ViewBag.Description = string.Format(App_GlobalResources.MetaTag.DescriptionPrivacy,
                WebConfigurationManager.AppSettings["nomeSito"]);
            ViewBag.Keywords = App_GlobalResources.MetaTag.KeywordsPrivacy;

            return View();
        }

        [HttpGet]
        public ActionResult VenditaGratis()
        {
            ViewBag.Title = string.Format(App_GlobalResources.MetaTag.TitleSellGratis, App_GlobalResources.Language.Moneta);
            ViewBag.Description = App_GlobalResources.MetaTag.DescriptionGratis;
            ViewBag.Keywords = App_GlobalResources.MetaTag.KeywordsGratis;

            return View();
        }

        [HttpGet]
        public ActionResult Baratto()
        {
            ViewBag.Title = App_GlobalResources.Language.TitleBarter;
            ViewBag.Description = App_GlobalResources.MetaTag.DescriptionGratis;
            ViewBag.Keywords = App_GlobalResources.MetaTag.KeywordsGratis;

            return View();
        }

        [HttpGet]
        public ActionResult Regalo()
        {
            ViewBag.Title = App_GlobalResources.MetaTag.TitleGift;
            ViewBag.Description = App_GlobalResources.MetaTag.DescriptionGift;
            ViewBag.Keywords = App_GlobalResources.MetaTag.KeywordsGift;

            return View();
        }

        [HttpGet]
        public ActionResult MonetaVirtuale()
        {
            ViewBag.Title = App_GlobalResources.MetaTag.TitleOnlineMoney;
            ViewBag.Description = App_GlobalResources.MetaTag.DescriptionGratis;
            ViewBag.Keywords = App_GlobalResources.MetaTag.KeywordsGratis;

            return View();
        }

        [HttpGet]
        public ActionResult SitiUtili()
        {
            ViewBag.Title = App_GlobalResources.MetaTag.TitleSiteWeb;
            ViewBag.Description = App_GlobalResources.MetaTag.DescriptionSiteWeb;
            ViewBag.Keywords = App_GlobalResources.MetaTag.KeywordsSiteWeb;

            return View();
        }

        [HttpGet]
        public ActionResult Team()
        {
            ViewBag.Title = App_GlobalResources.MetaTag.TitleSiteWeb;
            ViewBag.Description = App_GlobalResources.MetaTag.DescriptionSiteWeb;
            ViewBag.Keywords = App_GlobalResources.MetaTag.KeywordsSiteWeb;

            return View();
        }

        [HttpGet]
        public ActionResult BandiRicevuti()
        {
            ViewBag.Title = App_GlobalResources.MetaTag.TitleSiteWeb;
            ViewBag.Description = App_GlobalResources.MetaTag.DescriptionSiteWeb;
            ViewBag.Keywords = App_GlobalResources.MetaTag.KeywordsSiteWeb;

            return View();
        }

        #endregion

        #region SERVIZI

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Filters.ValidateAjax]
        public ActionResult Segnalazione(SegnalazioneViewModel viewModel)
        {
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {

                    // salvare su database
                    PERSONA_SEGNALAZIONE model = new PERSONA_SEGNALAZIONE();
                    if (Session["utente"] != null)
                        model.ID_PERSONA = (Session["utente"] as PersonaModel).Persona.ID;
                    model.IP = Request.UserHostAddress;
                    model.EMAIL_RISPOSTA = viewModel.EmailRisposta;
                    model.OGGETTO = viewModel.Oggetto;
                    model.TESTO = viewModel.Testo;
                    if (viewModel.Allegato!=null)
                        model.ALLEGATO = UploadFile(viewModel.Allegato);
                    model.CONTROLLER = viewModel.Controller;
                    model.VISTA = viewModel.Vista;
                    model.DATA_INVIO = DateTime.Now;
                    model.STATO = (int)Stato.ATTIVO;
                    db.PERSONA_SEGNALAZIONE.Add(model);
                    if (db.SaveChanges() > 0)
                        return Json(App_GlobalResources.Language.SuccessReporting);
                }
            }
            catch (InvalidDataException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                return Json(ex.Message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return Json(App_GlobalResources.Language.ErrorReporting);
        }

        // DA COMPLETARE ANCORA
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Filters.ValidateAjax]
        public ActionResult SuggerimentoAttivazioneAnnuncio(string id)
        {
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    // salvataggio notifica per venditore
                    // da implementare tabella notifica
                    return Json("Suggerimento inviato correttamente!");
                }
            }
            catch (InvalidDataException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                return Json(ex.Message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return Json(App_GlobalResources.Language.ErrorReporting);
        }

        /**
        ** term: inizio parola della categoria da cercare
        ** filtroExtra: nome oggetto da cercare
        **/
        [HttpGet]
        public ActionResult FindCategorie(string term, string filtroExtra = "")
        {
            List<Autocomplete> lista;

            using (DatabaseContext db = new DatabaseContext())
            {
                // recupero ogni parola del nome dell'oggetto, in modo da cercare per ognuna di esse
                string[] paroleNome = filtroExtra.Split(' ');
                lista = db.CATEGORIA.Where(item => item.ID > 0 && (item.NOME.StartsWith(term.Trim()) || (!String.IsNullOrEmpty(filtroExtra) && paroleNome.Contains(item.NOME)))).Select(c => new Autocomplete { Label = c.NOME, Value = c.ID }).ToList();
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        /**
        ** term: inizio parola della categoria da cercare
        ** filtroExtra: nome oggetto da cercare
        **/
        [HttpGet]
        public ActionResult FindSottocategorie(string term, string filtroExtra = "")
        {
            List<Autocomplete> lista;

            using (DatabaseContext db = new DatabaseContext())
            {
                // recupero ogni parola del nome dell'oggetto, in modo da cercare per ognuna di esse
                string[] paroleNome = filtroExtra.Split(' ');
                lista = db.FINDSOTTOCATEGORIE(term.Trim(), 0).Select(c => new Autocomplete { Label = c.NOME, Value = c.ID }).ToList();
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        /**
        ** term: inizio parola della regione da cercare
        ** filtroExtra: id nazione da filtrare
        **/
        [HttpGet]
        public ActionResult FindRegione(string term, int filtroExtra = 0)
        {
            List<Autocomplete> lista;

            using (DatabaseContext db = new DatabaseContext())
            {
                //lista = db.REGIONE.Where(item => item.Nome.StartsWith(term.Trim())).Select(c => new Autocomplete { Label = c.Nome, Value = c.IDRegione }).ToList();
                lista = db.REGIONE.Where(item => item.NOME.StartsWith(term.Trim()) && (filtroExtra <= 0 || (item.ID_NAZIONE.Value == filtroExtra))).Select(c => new Autocomplete { Label = c.NOME, Value = c.ID }).ToList();
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        /**
       ** term: inizio parola della provincia da cercare
       ** filtroExtra: id regione da filtrare
       **/
        [HttpGet]
        public ActionResult FindProvincia(string term, int filtroExtra = 0)
        {
            List<Autocomplete> lista;

            using (DatabaseContext db = new DatabaseContext())
            {
                lista = db.PROVINCIA.Where(item => item.NOME.StartsWith(term.Trim()) && (filtroExtra <= 0 || (item.ID_REGIONE.Value == filtroExtra))).Select(c => new Autocomplete { Label = c.NOME, Value = c.ID }).ToList();
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        /**
        ** term: inizio parola della città da cercare
        ** filtroExtra: id provincia da filtrare
        **/
        [HttpGet]
        public ActionResult FindCitta(string term, int filtroExtra = 0)
        {
            Autocomplete[] lista;

            using (DatabaseContext db = new DatabaseContext())
            {
                lista = db.COMUNE.Where(item => item.NOME.StartsWith(term.Trim()) && (filtroExtra <= 0 || (item.ID_PROVINCIA.Value == filtroExtra)))
                    .Select(item => new Autocomplete { Label = item.NOME + " (" + item.CAP + ")", Value = item.ID }).ToArray();
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        /**
        ** term: inizio parola della città da cercare
        ** filtroExtra: id provincia da filtrare
        **/
        [HttpGet]
        public ActionResult FindCittaSenzaCap(string term, int filtroExtra = 0)
        {
            List<Autocomplete> lista;

            using (DatabaseContext db = new DatabaseContext())
            {
                lista = db.COMUNE.Where(item => item.NOME.StartsWith(term.Trim()) && (filtroExtra <= 0 || (item.ID_PROVINCIA.Value == filtroExtra)))
                   .GroupBy(item => item.NOME)
                   .Select(item => new Autocomplete { Label = item.Max(c => c.NOME), Value = item.Max(c => c.ID) }).ToList();
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        /**
        ** term: inizio parola del cap da cercare
        **/
        [HttpGet]
        public ActionResult FindCap(string term, int filtroExtra = 0)
        {
            List<Autocomplete> lista;

            using (DatabaseContext db = new DatabaseContext())
            {
                lista = db.COMUNE.Where(item => item.CAP.StartsWith(term.Trim()) && (filtroExtra <= 0 || (item.ID_PROVINCIA.Value == filtroExtra)))
                   .Select(item => new Autocomplete { Label = item.NOME, Value = item.ID }).ToList();
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        /**
        ** term: inizio parola della marca da cercare
        ** filtroExtra: id categoria da filtrare
        **/
        [HttpGet]
        public ActionResult FindMarca(string term, int filtroExtra = 0)
        {
            List<Autocomplete> lista;

            // solo nella ricerca avanzata, verifica la marca per categoria
            HttpCookie cookie = Request.Cookies.Get("ricerca");
            if (filtroExtra == 0 && cookie != null && cookie["IDCategoria"] != null)
            {
                filtroExtra = (HttpContext.Application["categorie"] as List<FINDSOTTOCATEGORIE_Result>).Where(c => c.ID.ToString() == cookie["IDCategoria"] && c.STATO == (int)Stato.ATTIVO).FirstOrDefault().ID;
            }

            using (DatabaseContext db = new DatabaseContext())
            {
                lista = db.MARCA.Where(item => item.NOME.StartsWith(term.Trim()) && (filtroExtra <= 0 || (item.ID_CATEGORIA == filtroExtra))).Select(m => new Autocomplete { Label = m.NOME, Value = m.ID }).ToList();
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        /**
        ** term: inizio parola del modello da cercare
        ** filtroExtra: nome marca da filtrare
        **/
        [HttpGet]
        public ActionResult FindModello(string term, string filtroExtra = "")
        {
            List<Autocomplete> lista;
            int categoria = 0;

            if (string.IsNullOrWhiteSpace(filtroExtra) && Session["pubblicazioneoggetto"] != null)
            {
                filtroExtra = (Session["pubblicazioneoggetto"] as PubblicaOggettoViewModel).Marca;
            }
            else if (string.IsNullOrWhiteSpace(filtroExtra))
            {
                // solo nella ricerca avanzata, verifica la marca per categoria
                HttpCookie cookie = Request.Cookies.Get("ricerca");
                if (cookie != null && cookie["IDCategoria"] != null)
                {
                    categoria = (HttpContext.Application["categorie"] as List<FINDSOTTOCATEGORIE_Result>).Where(c => c.ID.ToString() == cookie["IDCategoria"] && c.STATO == (int)Stato.ATTIVO).FirstOrDefault().ID;
                }
            }

            using (DatabaseContext db = new DatabaseContext())
            {

                lista = db.MODELLO.Where(item => item.NOME.StartsWith(term.Trim()) && ((filtroExtra.Trim() != string.Empty && item.MARCA.NOME.StartsWith(filtroExtra)) || item.MARCA.ID_CATEGORIA == categoria)).Select(m => new Autocomplete { Label = m.NOME, Value = m.ID }).ToList();
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult FindAlimentazione(string term)
        {
            List<Autocomplete> lista;
            using (DatabaseContext db = new DatabaseContext())
            {
                int d = 0;
                if (Session["pubblicazioneoggetto"] != null)
                {
                    d = (base.Session["pubblicazioneoggetto"] as PubblicaOggettoViewModel).CategoriaId;
                }
                else
                {
                    // solo nella ricerca avanzata, verifica la marca per categoria
                    HttpCookie cookie = Request.Cookies.Get("ricerca");
                    if (cookie != null && cookie["IDCategoria"] != null)
                    {
                        d = (HttpContext.Application["categorie"] as List<FINDSOTTOCATEGORIE_Result>).Where(c => c.ID.ToString() == cookie["IDCategoria"] && c.STATO == (int)Stato.ATTIVO).FirstOrDefault().ID;
                    }
                }
                lista = (
                    from item in db.ALIMENTAZIONE
                    where item.NOME.StartsWith(term.Trim()) && item.ID_CATEGORIA == d
                    select item into m
                    select new Autocomplete()
                    {
                        Label = m.NOME,
                        Value = m.ID
                    }).ToList<Autocomplete>();
            }
            return base.Json(lista, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult FindArtista(string term)
        {
            List<Autocomplete> lista;
            using (DatabaseContext db = new DatabaseContext())
            {
                int d = 0;
                if (Session["pubblicazioneoggetto"] != null)
                { 
                    d = (base.Session["pubblicazioneoggetto"] as PubblicaOggettoViewModel).CategoriaId;
                }
                else
                {
                    // solo nella ricerca avanzata, verifica la marca per categoria
                    HttpCookie cookie = Request.Cookies.Get("ricerca");
                    if (cookie != null && cookie["IDCategoria"] != null)
                    {
                        d = (HttpContext.Application["categorie"] as List<FINDSOTTOCATEGORIE_Result>).Where(c => c.ID.ToString() == cookie["IDCategoria"] && c.STATO == (int)Stato.ATTIVO).FirstOrDefault().ID;
                    }
                }
                lista = (
                    from item in db.ARTISTA
                    where item.NOME.StartsWith(term.Trim()) && item.ID_CATEGORIA == d
                    select item into m
                    select new Autocomplete()
                    {
                        Label = m.NOME,
                        Value = m.ID
                    }).ToList<Autocomplete>();
            }
            return base.Json(lista, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult FindAutore(string term)
        {
            List<Autocomplete> lista;
            using (DatabaseContext db = new DatabaseContext())
            {
                int d = 0;
                if (Session["pubblicazioneoggetto"] != null)
                { 
                    d = (base.Session["pubblicazioneoggetto"] as PubblicaOggettoViewModel).CategoriaId;
                }
                else
                {
                    // solo nella ricerca avanzata, verifica la marca per categoria
                    HttpCookie cookie = Request.Cookies.Get("ricerca");
                    if (cookie != null && cookie["IDCategoria"] != null)
                    {
                        d = (HttpContext.Application["categorie"] as List<FINDSOTTOCATEGORIE_Result>).Where(c => c.ID.ToString() == cookie["IDCategoria"] && c.STATO == (int)Stato.ATTIVO).FirstOrDefault().ID;
                    }
                }
                lista = (
                    from item in db.AUTORE
                    where item.NOME.StartsWith(term.Trim()) && item.ID_CATEGORIA == d
                    select item into m
                    select new Autocomplete()
                    {
                        Label = m.NOME,
                        Value = m.ID
                    }).ToList<Autocomplete>();
            }
            return base.Json(lista, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult FindFormato(string term)
        {
            List<Autocomplete> lista;
            using (DatabaseContext db = new DatabaseContext())
            {
                int d = 0;
                if (Session["pubblicazioneoggetto"] != null)
                { 
                    d = (base.Session["pubblicazioneoggetto"] as PubblicaOggettoViewModel).CategoriaId;
                }
                else
                {
                    // solo nella ricerca avanzata, verifica la marca per categoria
                    HttpCookie cookie = Request.Cookies.Get("ricerca");
                    if (cookie != null && cookie["IDCategoria"] != null)
                    {
                        d = (HttpContext.Application["categorie"] as List<FINDSOTTOCATEGORIE_Result>).Where(c => c.ID.ToString() == cookie["IDCategoria"] && c.STATO == (int)Stato.ATTIVO).FirstOrDefault().ID;
                    }
                }
                lista = (
                    from item in db.FORMATO
                    where item.NOME.StartsWith(term.Trim()) && item.ID_CATEGORIA == d
                    select item into m
                    select new Autocomplete()
                    {
                        Label = m.NOME,
                        Value = m.ID
                    }).ToList<Autocomplete>();
            }
            return base.Json(lista, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult FindGenere(string term)
        {
            List<Autocomplete> lista;
            using (DatabaseContext db = new DatabaseContext())
            {
                int d = 0;
                if (Session["pubblicazioneoggetto"] != null)
                { 
                    d = (base.Session["pubblicazioneoggetto"] as PubblicaOggettoViewModel).CategoriaId;
                }
                else
                {
                    // solo nella ricerca avanzata, verifica la marca per categoria
                    HttpCookie cookie = Request.Cookies.Get("ricerca");
                    if (cookie != null && cookie["IDCategoria"] != null)
                    {
                        d = (HttpContext.Application["categorie"] as List<FINDSOTTOCATEGORIE_Result>).Where(c => c.ID.ToString() == cookie["IDCategoria"] && c.STATO == (int)Stato.ATTIVO).FirstOrDefault().ID;
                    }
                }
                lista = (
                    from item in db.GENERE
                    where item.NOME.StartsWith(term.Trim()) && item.ID_CATEGORIA == d
                    select item into m
                    select new Autocomplete()
                    {
                        Label = m.NOME,
                        Value = m.ID
                    }).ToList<Autocomplete>();
            }
            return base.Json(lista, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult FindPiattaforma(string term)
        {
            List<Autocomplete> lista;
            using (DatabaseContext db = new DatabaseContext())
            {
                int d = 0;
                if (Session["pubblicazioneoggetto"] != null)
                { 
                    d = (base.Session["pubblicazioneoggetto"] as PubblicaOggettoViewModel).CategoriaId;
                }
                else
                {
                    // solo nella ricerca avanzata, verifica la marca per categoria
                    HttpCookie cookie = Request.Cookies.Get("ricerca");
                    if (cookie != null && cookie["IDCategoria"] != null)
                    {
                        d = (HttpContext.Application["categorie"] as List<FINDSOTTOCATEGORIE_Result>).Where(c => c.ID.ToString() == cookie["IDCategoria"] && c.STATO == (int)Stato.ATTIVO).FirstOrDefault().ID;
                    }
                }
                lista = (
                    from item in db.PIATTAFORMA
                    where item.NOME.StartsWith(term.Trim()) && item.ID_CATEGORIA == d
                    select item into m
                    select new Autocomplete()
                    {
                        Label = m.NOME,
                        Value = m.ID
                    }).ToList<Autocomplete>();
            }
            return base.Json(lista, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult FindRegista(string term)
        {
            List<Autocomplete> lista;
            using (DatabaseContext db = new DatabaseContext())
            {
                int d = 0;
                if (Session["pubblicazioneoggetto"] != null)
                { 
                    d = (base.Session["pubblicazioneoggetto"] as PubblicaOggettoViewModel).CategoriaId;
                }
                else
                {
                    // solo nella ricerca avanzata, verifica la marca per categoria
                    HttpCookie cookie = Request.Cookies.Get("ricerca");
                    if (cookie != null && cookie["IDCategoria"] != null)
                    {
                        d = (HttpContext.Application["categorie"] as List<FINDSOTTOCATEGORIE_Result>).Where(c => c.ID.ToString() == cookie["IDCategoria"] && c.STATO == (int)Stato.ATTIVO).FirstOrDefault().ID;
                    }
                }
                lista = (
                    from item in db.REGISTA
                    where item.NOME.StartsWith(term.Trim()) && item.ID_CATEGORIA == d
                    select item into m
                    select new Autocomplete()
                    {
                        Label = m.NOME,
                        Value = m.ID
                    }).ToList<Autocomplete>();
            }
            return base.Json(lista, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult FindSistemaOperativo(string term)
        {
            List<Autocomplete> lista;
            using (DatabaseContext db = new DatabaseContext())
            {
                int d = 0;
                if (Session["pubblicazioneoggetto"] != null)
                {
                    d = (base.Session["pubblicazioneoggetto"] as PubblicaOggettoViewModel).CategoriaId;
                }
                else
                {
                    // solo nella ricerca avanzata, verifica la marca per categoria
                    HttpCookie cookie = Request.Cookies.Get("ricerca");
                    if (cookie != null && cookie["IDCategoria"] != null)
                    {
                        d = (HttpContext.Application["categorie"] as List<FINDSOTTOCATEGORIE_Result>).Where(c => c.ID.ToString() == cookie["IDCategoria"] && c.STATO == (int)Stato.ATTIVO).FirstOrDefault().ID;
                    }
                }
                lista = (
                    from item in db.SISTEMA_OPERATIVO
                    where item.NOME.StartsWith(term.Trim()) && item.ID_CATEGORIA == d
                    select item into m
                    select new Autocomplete()
                    {
                        Label = m.NOME,
                        Value = m.ID
                    }).ToList<Autocomplete>();
            }
            return base.Json(lista, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region METODI

        private String UploadFile(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0 && Utils.CheckFormatoFile(file,TipoMedia.TESTO))
            {
                string estensione = new System.IO.FileInfo(System.IO.Path.GetFileName(file.FileName)).Extension;
                string nomeFileUnivoco = System.Guid.NewGuid().ToString() + estensione;

                string path = Server.MapPath("~/Uploads/Segnalazioni/");

                System.IO.Directory.CreateDirectory(path);

                file.SaveAs(System.IO.Path.Combine(path, nomeFileUnivoco));
                return nomeFileUnivoco;
            }
            return null;
        }

        #endregion

    }
}