using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace ClientApplication.Controllers
{
     public class BearerAuthenticationAttribute : ActionFilterAttribute, IAuthenticationFilter
        {
            public void OnAuthentication(AuthenticationContext filterContext)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(filterContext.HttpContext.Session["access_token"])))
                {
                    var access_token = HttpContext.Current.Session["access_token"];
                    var username = HttpContext.Current.Session["username"];

                    var url = "https://localhost:44309/saml/getTokens?username=" + username;
                    WebRequest request = HttpWebRequest.Create(url);
                    WebResponse response = request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string responseText = reader.ReadToEnd();

                    if (responseText != null)
                    {
                        
                    }

                     ClaimsIdentity oAuthIdentity = new ClaimsIdentity(
                            new[]
                            {
                            new Claim(ClaimTypes.NameIdentifier, access_token.ToString()),
                            new Claim(ClaimTypes.Name, username.ToString()),
                            new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", access_token.ToString()),
                            new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", access_token.ToString())

                            },
                            DefaultAuthenticationTypes.ApplicationCookie
                         );

                    HttpContext.Current.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = false }, oAuthIdentity);
                    var identity = new GenericIdentity(username.ToString());
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