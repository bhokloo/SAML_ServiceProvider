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
using System.Windows;
using System.Web.UI;
using ComponentSpace.SAML2.Profiles.SSOBrowser;
using ComponentSpace.SAML2.Bindings;
using System.Xml;

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
                    dictionary["AuthorityURL"] = Request.UrlReferrer.GetLeftPart(UriPartial.Authority);
                    dictionary["returnURL"] = Request.QueryString["returnURL"];
                    relayState = string.Join(";", dictionary);
                    var ClientAuthorityUrl = Request.UrlReferrer.GetLeftPart(UriPartial.Authority);
                    var ClientReturnUrl = Request.QueryString["returnURL"];

                    relayState = RelayStateCache.Add(new RelayState(ClientAuthorityUrl, null));
                    relayState = RelayStateCache.Add(new RelayState(ClientReturnUrl, null));

                    serviceId = Request.QueryString["samlConfigurationId"];
                }
                if (serviceId == "")
                {
                    partnerName = WebConfigurationManager.AppSettings["ActivantsSAMLSP1IDPName"];
                    SAMLController.ConfigurationID = "ActivantsSAMLSP1";
                    XmlElement authnRequestXml = SAMLController.ConfigurationID;
                    HTTPArtifactState httpArtifactState = new HTTPArtifactState(SAMLController.ConfigurationID, null);
                    bool value = SamlAuthorizedDomains.IsAutorizedUrl(Request.Url.GetLeftPart(UriPartial.Authority));
                    if (value)
                        //string idpURL = CreateSSOServiceURL();
                        //ServiceProvider.SendArtifactByHTTPArtifact(Response, idpURL, httpArtifact, relayState, false);
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
                    var ReturnUrl = Request.UrlReferrer.GetLeftPart(UriPartial.Authority);
                    TempData["error"] = e;
                    TempData["ReturnURL"] = ReturnUrl;
                    ViewBag.JavaScriptFunction = ReturnUrl;
                    return RedirectToAction("error", "Home");
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
                var serviceId = "";
                var partnerName = "";
                if (Request.QueryString.ToString().Length > 0)
                {
                    relayState = Request.UrlReferrer.GetLeftPart(UriPartial.Authority);
                    serviceId = Request.QueryString["samlConfigurationId"];
                }
                if (serviceId == "")
                {
                    partnerName = WebConfigurationManager.AppSettings["ActivantsSAMLSP1IDPName"];
                    SAMLController.ConfigurationID = "ActivantsSAMLSP1";
                    bool value = SamlAuthorizedDomains.IsAutorizedUrl(Request.Url.GetLeftPart(UriPartial.Authority));
                    if (value)
                        SAMLServiceProvider.InitiateSLO(Response, null, relayState, partnerName);
                }
                else
                {
                    var partnerId = serviceId + "IDPName";
                    partnerName = WebConfigurationManager.AppSettings[partnerId];
                    SAMLController.ConfigurationID = serviceId;
                    bool value = SamlAuthorizedDomains.IsAutorizedUrl(Request.UrlReferrer.GetLeftPart(UriPartial.Authority));
                    if (value)
                        SAMLServiceProvider.InitiateSLO(Response, null, relayState, partnerName);
                }
                return new EmptyResult();
            }
            catch (Exception e)
            {
                if (Request.QueryString.ToString().Length > 0)
                {
                    var ReturnUrl = Request.UrlReferrer.GetLeftPart(UriPartial.Authority);
                    TempData["error"] = e;
                    TempData["ReturnURL"] = ReturnUrl;
                    return RedirectToAction("error", "Home");
                }
                else
                {
                    TempData["err"] = e;
                    return RedirectToAction("index", "Home");
                }
            }
        }

        private string CreateSSOServiceURL()
        {
            return string.Format("{0}?{1}={2}", WebConfigurationManager.AppSettings["idpssoURL"], bindingQueryParameter, HttpUtility.UrlEncode(spToIdPBindingRadioButtonList.SelectedValue));
        }
    }
}