using GratisForGratis.Models;
using System;
using System.Net.Mail;
using System.Web.Mvc;

namespace GratisForGratis.Controllers
{
    public class EmailController : Controller
    {
        // GET: Email
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ViewResult Index(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                if (SendEmail(model))
                    return View("Index", model);
            }
            return View();
        }

        public bool SendEmail(EmailModel model)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    model.To.ForEach(to => {
                        mail.To.Add(to);
                    });
                    mail.Subject = model.Subject;

                    mail.Body = RenderRazorViewToString(model.ControllerContext, "Email/" + model.Body, model.Layout, model.DatiEmail);
                    //mail.Body = View("Email/" + model.Body, model.Layout, model.DatiEmail).ToString();
                    // aggiunge allegati
                    model.Attachments.ForEach(m => {
                        mail.Attachments.Add(new Attachment(m.InputStream, m.FileName));
                    });
                    mail.Priority = model.Priority;
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    // verifico se inviare in modo asincrono la mail
                    if (model.SendAsync)
                        smtp.SendMailAsync(mail);
                    else
                        smtp.Send(mail);
                }
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return false;
        }

        public bool SendEmailByThread(EmailModel model)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    model.To.ForEach(to => {
                        mail.To.Add(to);
                    });
                    mail.Subject = model.Subject;

                    mail.Body = model.Body;
                    // aggiunge allegati
                    model.Attachments.ForEach(m => {
                        mail.Attachments.Add(new Attachment(m.InputStream, m.FileName));
                    });
                    mail.Priority = model.Priority;
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    // verifico se inviare in modo asincrono la mail
                    if (model.SendAsync)
                        smtp.SendMailAsync(mail);
                    else
                        smtp.Send(mail);
                }
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return false;
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

        protected string RenderViewToString<T>(string viewPath, T model)
        {
            ViewData.Model = model;
            using (var writer = new System.IO.StringWriter())
            {
                var view = new WebFormView(ControllerContext, viewPath);
                var vdd = new ViewDataDictionary<T>(model);
                var viewCxt = new ViewContext(ControllerContext, view, vdd,
                                            new TempDataDictionary(), writer);
                viewCxt.View.Render(viewCxt, writer);
                return writer.ToString();
            }
        }
    }
}