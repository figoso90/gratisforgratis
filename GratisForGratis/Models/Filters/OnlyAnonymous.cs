using System.Web.Mvc;

namespace GratisForGratis.Filters
{
    public class OnlyAnonymous : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                if (filterContext.HttpContext.Request.UrlReferrer != null)
                    filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.UrlReferrer.AbsolutePath);
                else
                    filterContext.Result = new RedirectResult(System.Web.Security.FormsAuthentication.DefaultUrl);
            }
                
            //filterContext.Result = new RedirectResult(System.Web.Security.FormsAuthentication.DefaultUrl);
            base.OnAuthorization(filterContext);
        }
    }
}
