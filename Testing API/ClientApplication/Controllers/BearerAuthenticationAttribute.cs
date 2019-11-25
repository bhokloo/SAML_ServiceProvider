using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;
using System.Web.Security;

namespace ClientApplication.Controllers
{
    public class BearerAuthenticationAttribute : ActionFilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(filterContext.HttpContext.Session["access_token"])))
            {
                var access_token = HttpContext.Current.Session["access_token"];

                

                ClaimsIdentity oAuthIdentity = new ClaimsIdentity(
                        new[] 
                        {
                            new Claim(ClaimTypes.NameIdentifier, access_token.ToString()),
                            new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string")
                            
                        },
                        DefaultAuthenticationTypes.ApplicationCookie
                     );

                HttpContext.Current.GetOwinContext().Authentication.SignIn( new AuthenticationProperties { IsPersistent = false }, oAuthIdentity);
                var identity = new GenericIdentity(access_token.ToString());
                var principal = new GenericPrincipal(identity, new string[0]);
                HttpContext.Current.User = principal;
                Thread.CurrentPrincipal = principal;
            }
            else
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            { 
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                     { "controller", "ActivantsSP" },
                     { "action", "index" }
                });
            }
        }

    }
}