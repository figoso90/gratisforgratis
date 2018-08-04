using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GratisForGratis.Controllers
{
    public class TestController : AdvancedController
    {
        // GET: Test
        public ActionResult Index()
        {
            Models.ViewModels.Email.PagamentoOffertaViewModel pagamentoOfferta = new Models.ViewModels.Email.PagamentoOffertaViewModel();
            pagamentoOfferta.NominativoDestinatario = "Prova Davide";
            pagamentoOfferta.Moneta = 0;
            Models.PersonaModel destinatario = Session["utente"] as Models.PersonaModel;
            string indirizzoEmail = destinatario.Email.SingleOrDefault(e => e.TIPO == (int)Models.TipoEmail.Registrazione).EMAIL;

            if (this.SendEmail(indirizzoEmail, Models.MessaggioNotifica.PagaOfferta.ToString(), ControllerContext, "test", pagamentoOfferta))
                ViewData["test"] = "Funziona";
            else
                ViewData["test"] = "Non funziona";
            return View();
        }

        [HttpGet]
        public ActionResult RegistrazioneTelegram()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegistrazioneTelegram(string telefono = "+393492240520")
        {
            if (ModelState.IsValid)
            {
                telefono = "+393492240520";
                SendCodeTelegram(telefono);
            }
            return View();
        }

        [HttpGet]
        public ActionResult MessaggioTelegram()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MessaggioTelegram(string telefono = "+39 3283932533", string code = "", string messaggio = "Prova di integrazione")
        {
            if (ModelState.IsValid)
            {
                SendMessaggioTelegram(telefono, code, messaggio);
            }
            return View();
        }

        public string RenderRazorViewToString(ControllerContext context, string viewName, string masterView, object model)
        {
            ViewData.Model = model;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindView(context, viewName, masterView);
                var viewContext = new ViewContext(context, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(context, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}