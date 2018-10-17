using GratisForGratis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GratisForGratis.Controllers
{
    [Authorize]
    public class OffertaController : AdvancedController
    {
        #region ACTION
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index(string token)
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Crea(string tokenAnnuncio)
        {
            // non recupero l'offerta
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Modifica(string token)
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Elimina(string token)
        {
            return View();
        }
        #endregion

        #region AJAX
        #endregion
    }
}