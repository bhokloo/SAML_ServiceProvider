using ComponentSpace.SAML2;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using ActivantsSamlServiceProvider.Utility;

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
                var serviceId = "";
                var partnerName = "";
                if (Request.QueryString.ToString().Length > 0)
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    dictionary["returnURLs"] = Request.UrlReferrer.GetLeftPart(UriPartial.Authority);
                    dictionary["returnClass"] = Request.QueryString["returnClass"];
                    dictionary["returnFunction"] = Request.QueryString["returnFunction"];
                    dictionary["returnError"] = Request.QueryString["returnError"];
                    relayState = string.Join(";", dictionary);
                    serviceId = Request.QueryString["samlConfigurationId"];
                }
                if (serviceId == "")
                {
                    partnerName = WebConfigurationManager.AppSettings["ActivantsSAMLSP1IDPName"];
                    SAMLController.ConfigurationID = "ActivantsSAMLSP1";
                    bool value = SamlAuthorizedDomains.IsAutorizedUrl(Request.Url.GetLeftPart(UriPartial.Authority));
                    if(value)
                        SAMLServiceProvider.InitiateSSO(Response, relayState, partnerName, new SSOOptions() { ForceAuthn = true });
                   
                }
                else
                {
                    var partnerId = serviceId + "IDPName";
                    partnerName = WebConfigurationManager.AppSettings[partnerId];
                    SAMLController.ConfigurationID = serviceId;
                    bool value = SamlAuthorizedDomains.IsAutorizedUrl(Request.UrlReferrer.GetLeftPart(UriPartial.Authority));
                    if (value)
                        SAMLServiceProvider.InitiateSSO(Response, relayState, partnerName, new SSOOptions() { ForceAuthn = true });
                }
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
                    return RedirectToAction("index", "Home");
                }

            }

        }


        public ActionResult InitiateSingleLogout(string relayState = null)
        {
            try
            {
                var partnerName = WebConfigurationManager.AppSettings["PartnerIdP"];
               
                if (Request.QueryString.ToString().Length > 0)
                {
                    if (SamlAuthorizedDomains.IsAutorizedUrl(HttpContext.Request.UrlReferrer.ToString()))
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
                        }
                    }
                }
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
    }
}