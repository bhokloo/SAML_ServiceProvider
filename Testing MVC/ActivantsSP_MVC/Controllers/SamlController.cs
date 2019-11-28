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
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using System.Data.Entity;
using System.Configuration;
using ComponentSpace.SAML2.Configuration;

namespace ActivantsSP.Controllers
{
    public class SamlController : Controller
    {
        public ActionResult InitiateSingleSignOn(string relayState = null)
        {
            try
            {
                var path = Server.MapPath("~/Certificates/sp.pfx");
                new X509Certificate(path, "activants", X509KeyStorageFlags.MachineKeySet);
                var partnerName = WebConfigurationManager.AppSettings["PartnerName"];
                var serviceId = "";
                if (Request.QueryString.ToString().Length > 0)
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    dictionary["returnURLs"] = Request.UrlReferrer.GetLeftPart(UriPartial.Authority);
                    dictionary["returnClass"] = Request.QueryString["returnClass"];
                    dictionary["returnFunction"] = Request.QueryString["returnFunction"];
                    dictionary["returnError"] = Request.QueryString["returnError"];
                    relayState = string.Join(";", dictionary);
                    serviceId = Request.QueryString["serviceId"];
                }

                var samlPath = Server.MapPath($"~/{serviceId}.config");
                SAMLConfigurationFile.Validate(samlPath);

                new SamlSpConfigurationController().ServiceProviderconfiguration(serviceId);
                SAMLServiceProvider.InitiateSSO(Response, relayState, partnerName, new SSOOptions() { ForceAuthn = true });
                return new EmptyResult();
            }
            catch (Exception e)
            {
                if (Request.QueryString.ToString().Length > 0)
                {
                    var ReturnUrl = Request.UrlReferrer.AbsoluteUri;
                    var returnClass = Request.QueryString["returnClass"];
                    var returnErrorFunction = Request.QueryString["returnError"];
                    return Redirect(ReturnUrl + "/" +returnClass + "/" + returnErrorFunction);
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

                    var accessToken = "";
                    
                    if(Request.Cookies["SAML_SessionId"] != null)
                    {
                        accessToken = Request.Cookies["SAML_SessionId"].Value;
                    }
                    var clientAccessToken = accessToken + Guid.NewGuid().ToString();

                    if (!string.IsNullOrEmpty(relayState))
                    {
                        
                        var clientQueryParameters = relayState.Replace("[","").Replace("]","").Replace(" ","").Split(';');
                        var returnURL = clientQueryParameters[0].Split(',');
                        var returnClass = clientQueryParameters[1].Split(',');
                        var returnFunction = clientQueryParameters[2].Split(',');
                        var returnError = clientQueryParameters[3].Split(',');
                        applicationUser.Logins.Add(new IdentityUserLogin() { LoginProvider = returnURL[1], ProviderKey = clientAccessToken, UserId = applicationUser.Id });
                        applicationUserManager.Update(applicationUser);

                        if (IsAutorizedUrl(returnURL[1]))
                        {
                            return Redirect(returnURL[1] + "/" +returnClass[1] + "/" +returnFunction[1] + "?access_token=" + clientAccessToken);
                        }
                        else
                        {
                            return Redirect(returnURL[1] + "/" + returnClass[1] + "/" + returnError[1]);
                        }
                    }
                    Session["attr"] = attributes;
                    return RedirectToAction("About", "Home");
                }
                catch (Exception e)
                {
                    if (!string.IsNullOrEmpty(relayState))
                    {
                        var clientQueryParameters = relayState.Replace("[", "").Replace("]", "").Replace(" ", "").Split(';');
                        var returnURL = clientQueryParameters[0].Split(',');
                        var returnClass = clientQueryParameters[1].Split(',');
                        var returnError = clientQueryParameters[3].Split(',');

                        return Redirect(returnURL[1] + "/" +returnClass[1] + "/" + returnError[1]);
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
            try
            {
                var partnerName = WebConfigurationManager.AppSettings["PartnerIdP"];
               
                if (Request.QueryString.ToString().Length > 0)
                {
                    if (IsAutorizedUrl(HttpContext.Request.UrlReferrer.ToString()))
                    {
                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        dict["returnURLs"] = Request.UrlReferrer.GetLeftPart(UriPartial.Authority);
                        dict["returnClass"] = Request.QueryString["returnClass"];
                        dict["returnFunction"] = Request.QueryString["returnFunction"];
                        dict["returnError"] = Request.QueryString["returnError"];

                        relayState = string.Join(";", dict);

                        if (Request.Cookies["SAML_SessionId"] != null)
                        {
                            var browserSamlSessionId = Request.Cookies["SAML_SessionId"].Value;
                            SamlManageController.deleteClientTokens(browserSamlSessionId, Request.UrlReferrer.GetLeftPart(UriPartial.Authority));
                        }
                    }
                }

                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                SAMLServiceProvider.InitiateSLO(Response, null, relayState, partnerName);
                return new EmptyResult();
            }
            catch (Exception e)
            {
                if (Request.QueryString.ToString().Length > 0)
                {
                    var ReturnUrl = Request.UrlReferrer.GetLeftPart(UriPartial.Authority);
                    var returnClass = Request.QueryString["returnClass"];
                    var returnErrorFunction = Request.QueryString["returnError"];
                    return Redirect(ReturnUrl + "/" +returnClass + "/" + returnErrorFunction);
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
                        var clientQueryParameters = relayState.Replace("[", "").Replace("]", "").Replace(" ", "").Split(';');
                        var returnURL = clientQueryParameters[0].Split(',');
                        var returnClass = clientQueryParameters[1].Split(',');
                        var returnFunction = clientQueryParameters[2].Split(',');
                        var returnError = clientQueryParameters[3].Split(',');

                        if (IsAutorizedUrl(returnURL[1]))
                        {
                            return Redirect(returnURL[1] + "/" +returnClass[1] + "/" + returnFunction[1]);
                        }
                        else
                        {
                            return Redirect(returnURL[1] + "/" +returnClass[1] + "/" + returnError[1]);
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
                    var clientQueryParameters = relayState.Replace("[", "").Replace("]", "").Replace(" ", "").Split(';');
                    var returnURL = clientQueryParameters[0].Split(',');
                    var returnClass = clientQueryParameters[1].Split(',');
                    var returnError = clientQueryParameters[3].Split(',');

                    return Redirect(returnURL[1] + "/" +returnClass[1] + "/" + returnError[1]);
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
            if (url.Contains("http://118.201.3.45:302/") || url.Contains("https://localhost:44313"))
                return true;
            else
                return false;
        }
    }
}