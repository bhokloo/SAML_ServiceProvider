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
using System.Linq;
using System.Web.Security;
using System.Text;
using System.Diagnostics;

namespace ActivantsSP.Controllers
{
    public class SamlController : Controller
    {
        public ActionResult InitiateSingleSignOn(string relayState = null)
        {
            try {
                var partnerName = WebConfigurationManager.AppSettings["PartnerName"];
                if (Request.Cookies["relayStateForSP"] != null)
                {
                    var relayStatesFromClient = Request.Cookies["relayStateForSP"].Value;
                    relayState = Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(relayStatesFromClient)));
                }
                SAMLServiceProvider.InitiateSSO(Response, relayState, partnerName, new SSOOptions() { ForceAuthn = true });
                return new EmptyResult();
            }
            catch (Exception e)
            {
                if (Request.Cookies["errorPageForClient"] != null)
                {
                    var errorPage = Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(Request.Cookies["errorPageForClient"].Value)));
                    return Redirect(errorPage);
                }
                   
                else
                {
                    TempData["err"] = e;
                    return RedirectToAction("About", "Home");
                }
            }

        }

        public ActionResult InitiateSingleLogout(string relayState = null)
        {
            try
            {
                var partnerName = WebConfigurationManager.AppSettings["PartnerName"];
                if (Request.Cookies["relayStateForSP"] != null)
                {
                    var relayStatesFromClient = Request.Cookies["relayStateForSP"].Value;
                    relayState = Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(relayStatesFromClient)));
                }
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                SAMLServiceProvider.InitiateSLO(Response, null, relayState, partnerName);
                return new EmptyResult();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                if (Request.Cookies["errorPageForClient"] != null)
                {
                    var errorPage = Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(Request.Cookies["errorPageForClient"].Value)));
                    return Redirect(errorPage);
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
            if(Response != null)
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

                        foreach (var a in attributes)
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
                        if(IsNotLocalUrl(relayState))
                        {
                            return Redirect(relayState);
                        }
                        else
                        {
                            var errorPage = Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(Request.Cookies["errorPageForClient"].Value)));
                            return Redirect(errorPage);
                        }
                    }
                    // Response.ContentType = "text/xml";
                    // Response.ContentEncoding = System.Text.Encoding.UTF8;

                    //var json = attributes.ToArray();
                    //var jsonData = Json(json, JsonRequestBehavior.AllowGet);
                    Session["attr"] = attributes;
                    return RedirectToAction("About", "Home");
                }
                catch (Exception e)
                {
                    if (Request.Cookies["errorPageForClient"] != null)
                    {
                        var errorPage = Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(Request.Cookies["errorPageForClient"].Value)));
                        return Redirect(errorPage);
                    }
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

      

        public ActionResult SingleLogoutService()
        {
            string relayState = null;
            try
            {
                bool isRequest = false;
                string logoutReason = null;
                string partnerIdP = null;

                SAMLServiceProvider.ReceiveSLO(Request, out isRequest, out logoutReason, out partnerIdP, out relayState);

                if (isRequest)
                {
                    HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    SAMLServiceProvider.SendSLO(Response, null);
                }
                else
                {
                    if (!string.IsNullOrEmpty(relayState) && IsNotLocalUrl(relayState))
                    {
                        HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        Session.Clear();
                        Session.Abandon();
                        return Redirect(relayState);
                    }
                }
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("About", "Home");
            }
            catch (Exception e)
            {
                if (Request.Cookies["errorPageForClient"] != null)
                {
                    var errorPage = Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(Request.Cookies["errorPageForClient"].Value)));
                    HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    Session.Clear();
                    Session.Abandon();
                    return Redirect(errorPage);
                }
                else
                {
                    HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    Session.Clear();
                    Session.Abandon();
                    TempData["err"] = e;
                    return RedirectToAction("About", "Home");
                }
                    
            }

        }

        private bool IsNotLocalUrl(string relayState)
        {
            if (relayState.Contains("https://localhost:44313/"))
                return true;
            else
                return false;
        }

    }
}