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

namespace ActivantsSP.Controllers
{
    public class SamlController : Controller
    {
        public ActionResult InitiateSingleSignOn(string relayState = null)
        {
            try
            {
                var partnerName = WebConfigurationManager.AppSettings["PartnerName"];
                if (Request.QueryString["responseFromSp"] != null)
                {
                    relayState = Request.QueryString["responseFromSp"];
                }
                SAMLServiceProvider.InitiateSSO(Response, relayState, partnerName, new SSOOptions() { ForceAuthn = true });
                return new EmptyResult();
            }
            catch (Exception e)
            {
                if (Request.QueryString["responseFromSp"] != null)
                {
                    var fullUrl = Decrypt(HttpUtility.UrlDecode(Request.QueryString["responseFromSp"]));
                    string[] Url = fullUrl.Split(new string[] { "&" }, StringSplitOptions.None);
                    var ReturnUrl = Url[0];
                    var errUrl = Url[3];
                    return Redirect(ReturnUrl + errUrl);
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
                    if (!string.IsNullOrEmpty(relayState))
                    {
                        HttpCookie cookie = new HttpCookie("relayState");
                        cookie.Value = relayState;
                        Response.Cookies.Add(cookie);

                        var fullUrl = Decrypt(HttpUtility.UrlDecode(relayState));
                        string[] Url = fullUrl.Split(new string[] { "&" }, StringSplitOptions.None);

                        var ClientUrl = Url[0];
                        var responseFromSp = Url[1];
                        var errorPage = Url[2];
                        new AccountController();
                        if (IsAutorizedUrl(ClientUrl))
                        {
                            return Redirect(ClientUrl + responseFromSp);
                        }
                        else
                        {
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
                        var fullUrl = Decrypt(HttpUtility.UrlDecode(relayState));
                        string[] Url = fullUrl.Split(new string[] { "&" }, StringSplitOptions.None);
                        var ClientUrl = Url[0];
                        var errorPage = Url[2];
                        return RedirectToAction(ClientUrl, errorPage);
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
                var partnerName = WebConfigurationManager.AppSettings["PartnerName"];
                if (Request.Cookies["relayState"] != null)
                {
                    var clientUrl = Request.Cookies["relayState"].Value;
                    relayState = clientUrl;
                }
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                SAMLServiceProvider.InitiateSLO(Response, null, relayState, partnerName);
                return new EmptyResult();
            }
            catch (Exception e)
            {
                if (Request.Cookies["relayState"] != null)
                {
                    var fullUrl = Decrypt(HttpUtility.UrlDecode(Request.QueryString["responseFromSp"]));
                    string[] Url = fullUrl.Split(new string[] { "&" }, StringSplitOptions.None);
                    var ReturnUrl = Url[0];
                    var errUrl = Url[3];
                    return Redirect(ReturnUrl + errUrl);
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
                        var fullUrl = Decrypt(HttpUtility.UrlDecode(relayState));
                        string[] Url = fullUrl.Split(new string[] { "&" }, StringSplitOptions.None);
                        var ClientUrl = Url[0];
                        var errorPage = Url[2];
                        var index = Url[3];
                        if (IsAutorizedUrl(ClientUrl))
                        {
                            return Redirect(ClientUrl + index);
                        }
                        else
                        {
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
                    var fullUrl = Decrypt(HttpUtility.UrlDecode(relayState));
                    string[] Url = fullUrl.Split(new string[] { "&" }, StringSplitOptions.None);
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
            if (url.Contains("https://localhost:44313/"))
                return true;
            else
                return false;
        }

        private string Decrypt(string cipherText)
        {
            string EncryptionKey = "hyddhrii%2moi43Hd5%%";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

    }
}