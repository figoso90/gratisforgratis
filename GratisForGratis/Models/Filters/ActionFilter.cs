using Elmah;
using GratisForGratis.Controllers;
using GratisForGratis.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace GratisForGratis.Filters
{
    internal class ActionFilter : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpRequestBase richiesta = filterContext.RequestContext.HttpContext.Request;
            HttpResponseBase risposta = filterContext.RequestContext.HttpContext.Response;
            if ((risposta.Cookies.Get("ricerca") == null ? true : !risposta.Cookies.Get("ricerca").HasKeys))
            {
                risposta.Cookies.Set(richiesta.Cookies.Get("ricerca"));
            }
            if ((risposta.Cookies.Get("filtro") == null ? true : !risposta.Cookies.Get("filtro").HasKeys))
            {
                risposta.Cookies.Set(richiesta.Cookies.Get("filtro"));
            }
            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // recupero la pubblicità per il sito se non è chiamata AJAX
            /*if (!richiesta.IsAjaxRequest())
            {
                ServizioADV.ServizioWebControllerService servizio = new ServizioADV.ServizioWebControllerService();
                if (filterContext.RouteData.Values["controller"].ToString() == "Home" && filterContext.RouteData.Values["action"].ToString() == "Index")
                    filterContext.Controller.TempData["advertising"] = servizio.getBannerByToken("4eb492b5-3010-11e6-8202-000c29be38d2", "4eb492b5-3010-11e6-8202-000c29be38d2", "1");
                else
                    filterContext.Controller.TempData["advertising"] = servizio.getBannerRandom("4eb492b5-3010-11e6-8202-000c29be38d2", "1");
            }*/
            /*
            HttpRequestBase richiesta = filterContext.RequestContext.HttpContext.Request;
            try
            {
                HttpResponseBase risposta = filterContext.RequestContext.HttpContext.Response;
                //if (risposta.Cookies.Get("ricerca") == null || risposta.Cookies.Get("ricerca").HasKeys))
                //{
                //    risposta.Cookies.Set(richiesta.Cookies.Get("ricerca"));
                //}
                if ((risposta.Cookies.Get("ricerca") == null ? true : !risposta.Cookies.Get("ricerca").HasKeys))
                {
                    risposta.Cookies.Set(richiesta.Cookies.Get("ricerca"));
                }
                if ((risposta.Cookies.Get("filtro") == null ? true : !risposta.Cookies.Get("filtro").HasKeys))
                {
                    risposta.Cookies.Set(richiesta.Cookies.Get("filtro"));
                }
            }
            catch (Exception exception)
            {
                ErrorSignal.FromCurrentContext().Raise(exception);
            }
            */
                base.OnResultExecuting(filterContext);
            }
        }
    }
 