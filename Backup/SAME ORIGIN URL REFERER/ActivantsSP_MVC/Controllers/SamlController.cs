using ComponentSpace.SAML2;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using ActivantsSP.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;


namespace ActivantsSP.Controllers
{
    public class SamlController : Controller
    {
        public ActionResult InitiateSingleSignOn(string relayState = null)
        {
            string ClientUrl = null;
            if(HttpContext.Request.UrlReferrer != null)
            {
                var Url = HttpContext.Request.UrlReferrer.OriginalString;
                string[] newURL = Url.Split('/');
                ClientUrl = newURL[0] + "//" + newURL[2] + "/";
            }
            try {
                var partnerName = WebConfigurationManager.AppSettings["PartnerName"];
                
                if (Request.QueryString["responseFromSp"] != null)
                {
                    relayState = ClientUrl + "ActivantsSP" + "&/"+ Request.QueryString["responseFromSp"] + "&/" + Request.QueryString["errorPage"];
                }
                SAMLServiceProvider.InitiateSSO(Response, relayState, partnerName, new SSOOptions() { ForceAuthn = true });
                return new EmptyResult();
            }
            catch (Exception e)
            {
                if (Request.QueryString["errorPage"] != null)
                {
                    var errPage = Request.QueryString["errorPage"];
                    return Redirect(ClientUrl + "ActivantsSP/" + errPage);
                }
                else
                {
                    TempData["err"] = e;
                    return RedirectToAction("About", "Home");
                }
                   
            }

        }

        public ActionResult AssertionConsumerService()
        {
            string relayState = null;
            if (Response != null)
            {
                try
                {
                    SAMLServiceProvider.ReceiveSSO(
                   Request,
                   out var isInResponseTo,
                   out var partnerName,
                   out var authnContext,
                   out var userName,
                   out IDictionary<string, string> attributes,
                   out relayState);

                    var applicationUserManager = HttpContext.GetOwinContext().Get<ApplicationUserManager>();
                    var applicationUser = applicationUserManager.FindByName(userName);


                    if (applicationUser == null)
                    {
                        applicationUser = new ApplicationUser();

                        applicationUser.UserName = userName;
                        applicationUser.Email = userName;
                        applicationUser.EmailConfirmed = true;

                        foreach(var a in attributes)
                        {
                            applicationUser.Claims.Add(new IdentityUserClaim() { ClaimType = a.Key, ClaimValue = a.Value, UserId = applicationUser.Id });

                        }
                        var identityResult = applicationUserManager.Create(applicationUser);

                        if (!identityResult.Succeeded)
                        {
                            throw new Exception(string.Format("The user {0} couldn't be created.\n{1}", userName, identityResult));
                        }
                    }
                    var applicationSignInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                    applicationSignInManager.SignIn(applicationUser, false, false);
                    if (!string.IsNullOrEmpty(relayState))
                    {
                        string[] Url = relayState.Split(new string[] { "&" }, StringSplitOptions.None);
                        var ClientUrl =Url[0];
                        var responseFromSp = Url[1];
                        var errorPage = Url[2];
                        if (IsAutorizedUrl(ClientUrl))
                        {
                            //var ck = Request.Cookies["SAML_SessionId"];
                            return Redirect(ClientUrl + responseFromSp);
                        }
                        else
                        {
                            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            Session.Clear();
                            Session.Abandon();
                            return Redirect(ClientUrl + errorPage);
                        }
                    }
                    Session["attr"] = attributes;
                    return RedirectToAction("About", "Home");
                }
                catch (Exception e)
                {
                    if (!string.IsNullOrEmpty(relayState))
                    {
                        string[] Url = relayState.Split(new string[] { "&" }, StringSplitOptions.None);
                        var ClientUrl = Url[0];
                        var errorPage = Url[2];
                        return Redirect(ClientUrl + errorPage);
                    }
                    else
                    {
                        TempData["err"] = e;
                        return RedirectToAction("About", "Home");
                    }

                }
            }
            else
            {
                return RedirectToAction("About", "Home");
            }

        }

        public ActionResult InitiateSingleLogout(string relayState = null)
        {
            string ClientUrl = null;
            if (HttpContext.Request.UrlReferrer != null)
            {
                var Url = HttpContext.Request.UrlReferrer.OriginalString;
                string[] newURL = Url.Split('/');
                ClientUrl = newURL[0] + "//" + newURL[2] + "/";
            }
            try
            {
                var partnerName = WebConfigurationManager.AppSettings["PartnerName"];
                if (Request.QueryString["index"] != null)
                {
                    relayState = ClientUrl + "ActivantsSP" + "&/"+ Request.QueryString["index"] + "&/" + Request.QueryString["errorPage"];
                }
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                SAMLServiceProvider.InitiateSLO(Response, null, relayState, partnerName);
                return new EmptyResult();
            }
            catch (Exception e)
            {
                if (Request.QueryString["errPage"] != null)
                {
                    var errPage = Request.QueryString["errorPage"];
                    return Redirect(ClientUrl + "ActivantsSP/" + errPage);
                }
                else
                {
                    TempData["err"] = e;
                    return RedirectToAction("About", "Home");
                }

            }
        }

        public ActionResult SingleLogoutService()
        {
            string relayState = null;
            try
            {
                bool isRequest = false;
                string logoutReason = null;
                string partnerIdP = null;

                SAMLServiceProvider.ReceiveSLO(Request, out isRequest, out logoutReason, out partnerIdP, out relayState);
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                Session.Clear();
                Session.Abandon();
                if (isRequest)
                {
                    SAMLServiceProvider.SendSLO(Response, null);
                }
                else
                {
                    if (!string.IsNullOrEmpty(relayState))
                    {
                        string[] Url = relayState.Split(new string[] { "&" }, StringSplitOptions.None);
                        var ClientUrl = Url[0];
                        var index = Url[1];
                        var errorPage = Url[2];
                        if (IsAutorizedUrl(ClientUrl))
                        {
                            return Redirect(ClientUrl + index);
                        }
                        else
                        {
                            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            Session.Clear();
                            Session.Abandon();
                            return Redirect(ClientUrl + errorPage);
                        }
                    }
                }
                return RedirectToAction("About", "Home");
            }
            catch (Exception e)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                Session.Clear();
                Session.Abandon();

                if (!string.IsNullOrEmpty(relayState))
                {
                    string[] Url = relayState.Split(new string[] { "&" }, StringSplitOptions.None);
                    var ClientUrl = Url[0];
                    var errorPage = Url[2];
                    return Redirect(ClientUrl + errorPage);
                }
                else
                {
                    TempData["err"] = e;
                    return RedirectToAction("About", "Home");
                }
            }

        }

        private bool IsAutorizedUrl(string url)
        {
            if (url.Contains("https://localhost:44313/") || url.Contains("https://192.168.1.172:44312"))
                return true;
            else
                return false;
        }
    }
}