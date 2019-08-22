using GratisForGratis.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GratisForGratis.Controllers
{
    public class VenditeController : AdvancedController
    {

        [HttpGet]
        public ActionResult Index(int pagina = 1)
        {
            List<AnnuncioViewModel> vendite = new List<AnnuncioViewModel>();
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    int utente = ((PersonaModel)Session["utente"]).Persona.ID;
                    var query = db.ANNUNCIO.Where(item => item.ID_PERSONA == utente 
                        && (item.STATO != (int)StatoVendita.ELIMINATO && item.STATO != (int)StatoVendita.BARATTATO && item.STATO != (int)StatoVendita.VENDUTO)
                        && (item.ID_OGGETTO != null || item.ID_SERVIZIO != null));
                    int numeroElementi = Convert.ToInt32(WebConfigurationManager.AppSettings["numeroElementi"]);
                    ViewData["TotalePagine"] = (int)Math.Ceiling((decimal)query.Count() / (decimal)numeroElementi);
                    ViewData["Pagina"] = pagina;
                    pagina -= 1;
                    List<ANNUNCIO> listaAnnunci = query
                        .OrderByDescending(item => item.DATA_INSERIMENTO)
                        .Skip(pagina * numeroElementi)
                        .Take(numeroElementi)
                        .ToList();

                    foreach(ANNUNCIO annuncio in listaAnnunci)
                    {
                        AnnuncioModel annuncioModel = new AnnuncioModel();
                        vendite.Add(annuncioModel.GetViewModel(db, annuncio));
                    }

                    if (vendite.Count > 0)
                        RefreshPunteggioUtente(db);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View(vendite);
        }

        [HttpGet]
        public ActionResult OfferteRicevute(string vendita = "", int pagina = 1)
        {
            List<OffertaViewModel> offerte = new List<OffertaViewModel>();
            try
            {
                Guid? token = null;
                if (!string.IsNullOrWhiteSpace(vendita))
                    token = Guid.Parse(HttpUtility.UrlDecode(vendita));

                int utente = ((PersonaModel)Session["utente"]).Persona.ID;

                using (DatabaseContext db = new DatabaseContext())
                {
                    int numeroElementi = Convert.ToInt32(WebConfigurationManager.AppSettings["numeroElementi"]);
                    //var query = db.OFFERTA.Where(c => c.ANNUNCIO == idVendita && (c.STATO != (int)StatoOfferta.ANNULLATA || c.STATO != (int)StatoOfferta.INATTIVA || c.STATO != (int)StatoOfferta.SOSPESA));
                    var query = db.OFFERTA.Where(c => (token == null || c.ANNUNCIO.TOKEN == token) && c.ANNUNCIO.ID_PERSONA == utente && c.PERSONA.STATO == (int)Stato.ATTIVO);
                    ViewData["TotalePagine"] = (int)Math.Ceiling((decimal)query.Count() / (decimal)numeroElementi);
                    ViewData["Pagina"] = pagina;
                    pagina -= 1;
                    List<OFFERTA> lista = query
                    .OrderByDescending(item => item.DATA_INSERIMENTO)
                    .Skip(pagina * numeroElementi)
                    .Take(numeroElementi)
                    .ToList();
                    // fare update LETTA della lista recuperata
                    //offerte.Id = lista.Select(o => o.ANNUNCIO.ID).FirstOrDefault();
                    //offerte.NomeVendita = lista.Select(o => o.ANNUNCIO.NOME).FirstOrDefault();
                    //offerte.DataInserimento = lista.Select(o => o.ANNUNCIO.DATA_INSERIMENTO).FirstOrDefault();
                    foreach (OFFERTA o in lista)
                    {
                        OffertaViewModel offertaEffettuata = new OffertaViewModel(db, o);
                        offerte.Add(offertaEffettuata);
                    }

                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            
            return View(offerte);
        }

        [HttpGet]
        public ActionResult Concluse(int pagina = 1)
        {
            List<AnnuncioViewModel> vendite = new List<AnnuncioViewModel>();
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    int utente = ((PersonaModel)Session["utente"]).Persona.ID;
                    var query = db.ANNUNCIO.Where(item => item.ID_PERSONA == utente
                        && (item.STATO == (int)StatoVendita.ELIMINATO || item.STATO == (int)StatoVendita.BARATTATO || item.STATO == (int)StatoVendita.VENDUTO)
                        && (item.ID_OGGETTO != null || item.ID_SERVIZIO != null));
                    int numeroElementi = Convert.ToInt32(WebConfigurationManager.AppSettings["numeroElementi"]);
                    ViewData["TotalePagine"] = (int)Math.Ceiling((decimal)query.Count() / (decimal)numeroElementi);
                    ViewData["Pagina"] = pagina;
                    pagina -= 1;
                    List<ANNUNCIO> listaAnnunci = query
                        .OrderByDescending(item => item.DATA_INSERIMENTO)
                        .Skip(pagina * numeroElementi)
                        .Take(numeroElementi)
                        .ToList();

                    foreach (ANNUNCIO annuncio in listaAnnunci)
                    {
                        AnnuncioModel annuncioModel = new AnnuncioModel();
                        vendite.Add(annuncioModel.GetViewModel(db, annuncio));
                    }
                    if (vendite.Count > 0)
                        RefreshPunteggioUtente(db);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View(vendite);
        }
        /* USERò IN CASO DI SISTEMA AUTOMATICO PER SUGGERIRE AD ALTRI UTENTI UN ACQUISTO E RICEVERE UN GUADAGNO
        [HttpGet]
        public ActionResult Suggerite(int pagina = 1)
        {
            List<ProposteAnnuncio> viewModel = new List<ProposteAnnuncio>();
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    db.Database.Connection.Open();
                    PersonaModel utente = (PersonaModel)Session["utente"];
                    int idUtente = utente.Persona.ID;
                    int contoAttuale = utente.Punti;
                    
                    var query = db.ANNUNCIO.Where(m => m.ID_PERSONA == idUtente && (// sui miei annunci
                                    (m.STATO != (int)StatoVendita.ELIMINATO && m.STATO != (int)StatoVendita.BARATTATO && m.STATO != (int)StatoVendita.VENDUTO) &&
                                    db.PERSONA.Count(p => p.CONTO_CORRENTE.CONTO_CORRENTE_MONETA.Count() >= m.PUNTI) > 0 || // gli utenti con abbastanza crediti
                                    db.ANNUNCIO_DESIDERATO.Count(ad => ad.ID_PERSONA == idUtente &&
                                        (ad.ANNUNCIO.STATO != (int)StatoVendita.ELIMINATO && ad.ANNUNCIO.STATO != (int)StatoVendita.BARATTATO && ad.ANNUNCIO.STATO != (int)StatoVendita.VENDUTO) &&
                                    m.ANNUNCIO_DESIDERATO.Count(b => b.ID_PERSONA == ad.ANNUNCIO.ID_PERSONA) > 0) > 0 // o di cui desidero almeno un annuncio
                                ));
                    int numeroElementi = Convert.ToInt32(WebConfigurationManager.AppSettings["numeroElementi"]);
                    ViewData["TotalePagine"] = (int)Math.Ceiling((decimal)query.Count() / (decimal)numeroElementi);
                    ViewData["Pagina"] = pagina;
                    pagina -= 1;

                    query.OrderByDescending(item => item.DATA_INSERIMENTO).OrderByDescending(item => item.ID) // ordinato per inserimento annuncio e poi per inserimento desiderio
                        .OrderByDescending(item => db.PERSONA.Count(p => p.CONTO_CORRENTE.CONTO_CORRENTE_MONETA.Count() >= item.PUNTI)) // ordinato per disponibilità di crediti
                        .OrderByDescending(item => db.ANNUNCIO_DESIDERATO.Count(ad => ad.ID_PERSONA == idUtente &&
                            (ad.ANNUNCIO.STATO != (int)StatoVendita.ELIMINATO && ad.ANNUNCIO.STATO != (int)StatoVendita.BARATTATO && ad.ANNUNCIO.STATO != (int)StatoVendita.VENDUTO) &&
                            item.ANNUNCIO_DESIDERATO.Count(b => b.ID_PERSONA == ad.ANNUNCIO.ID_PERSONA) > 0)) // ordinato per numeri annunci desiderati da te
                        .Skip(pagina * numeroElementi)
                        .Take(numeroElementi)
                        .ToList().ForEach(m =>
                        {
                            ProposteAnnuncio proposta = new ProposteAnnuncio(db, m);

                            // aggiungere persone che possono comprare l'annuncio
                            db.PERSONA.Where(p => p.CONTO_CORRENTE.CONTO_CORRENTE_MONETA.Count() >= m.PUNTI).ToList().ForEach(p =>
                            {
                                proposta.PersonePossibili.Add(p);
                            });

                            // capire come prendere bene tutti gli annunci che mi piacciono delle persone che desiderano il mio annuncio
                            db.ANNUNCIO_DESIDERATO.Where(ad => ad.ID_PERSONA == idUtente &&
                                (ad.ANNUNCIO.STATO != (int)StatoVendita.ELIMINATO && ad.ANNUNCIO.STATO != (int)StatoVendita.BARATTATO && ad.ANNUNCIO.STATO != (int)StatoVendita.VENDUTO) &&
                                db.ANNUNCIO_DESIDERATO.Count(b => b.ID_ANNUNCIO == m.ID && b.ID_PERSONA == ad.ANNUNCIO.ID_PERSONA) > 0).ToList().ForEach(a =>
                            {
                                proposta.AnnunciDesiderati.Add(new AnnuncioViewModel(db, a.ANNUNCIO));
                            });

                            viewModel.Add(proposta);
                        });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View(viewModel);
        }
        */
    }
}