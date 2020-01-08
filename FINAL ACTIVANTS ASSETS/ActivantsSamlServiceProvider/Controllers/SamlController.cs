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
using ComponentSpace.SAML2.Protocols;
using ComponentSpace.SAML2.Assertions;
using System.Web.Security;

namespace ActivantsSP.Controllers
{
    public class SamlController : Controller
    {
        public ActionResult InitiateSingleSignOn(string relayState = null)
        {
            try
            {
               // RequestLoginAtIdentityProvider();

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
                   // XmlElement authnRequestXml = SAMLController.ConfigurationID;
                    //HTTPArtifactState httpArtifactState = new HTTPArtifactState(SAMLController.ConfigurationID, null);
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

        //private string CreateAbsoluteURL(string relativeURL)
        //{
        //    return new Uri(Request.Url.AbsoluteUri, ResolveUrl(relativeURL));
        //}

        //private void RequestLoginAtIdentityProvider()
        //{
        //    XmlElement authnRequestXml = CreateAuthnRequest();
        //    string spResourceURL = CreateAbsoluteURL(FormsAuthentication.GetRedirectUrl("", false));
        //    string relayState = RelayStateCache.Add(new RelayState(spResourceURL, null));

        //    // Send the authentication request to the identity provider over the selected binding.
        //    string idpURL = CreateSSOServiceURL();

        //    switch (spToIdPBindingRadioButtonList.SelectedValue)
        //    {
        //        case SAMLIdentifiers.BindingURIs.HTTPRedirect:
        //            X509Certificate2 x509Certificate = (X509Certificate2)Application[Global.SPX509Certificate];

        //            ServiceProvider.SendAuthnRequestByHTTPRedirect(Response, idpURL, authnRequestXml, relayState, x509Certificate.PrivateKey);

        //            break;
        //        case SAMLIdentifiers.BindingURIs.HTTPPost:
        //            ServiceProvider.SendAuthnRequestByHTTPPost(Response, idpURL, authnRequestXml, relayState);

        //            // Don't send this form.
        //            Response.End();

        //            break;
        //        case SAMLIdentifiers.BindingURIs.HTTPArtifact:
        //            // Create the artifact.
        //            string identificationURL = CreateAbsoluteURL("~/");
        //            HTTPArtifactType4 httpArtifact = new HTTPArtifactType4(HTTPArtifactType4.CreateSourceId(identificationURL), HTTPArtifactType4.CreateMessageHandle());

        //            // Cache the authentication request for subsequent sending using the artifact resolution protocol.
        //            HTTPArtifactState httpArtifactState = new HTTPArtifactState(authnRequestXml, null);
        //            HTTPArtifactStateCache.Add(httpArtifact, httpArtifactState);

        //            // Send the artifact.
        //            ServiceProvider.SendArtifactByHTTPArtifact(Response, idpURL, httpArtifact, relayState, false);
        //            break;
        //    }
        //}

        //private XmlElement CreateAuthnRequest()
        //{
        //    // Create some URLs to identify the service provider to the identity provider.
        //    // As we're using the same endpoint for the different bindings, add a query string parameter
        //    // to identify the binding.
        //    string issuerURL = CreateAbsoluteURL("~/");
        //    string assertionConsumerServiceURL = CreateAssertionConsumerServiceURL();

        //    // Create the authentication request.
        //    AuthnRequest authnRequest = new AuthnRequest();
        //    authnRequest.Destination = WebConfigurationManager.AppSettings["idpssoURL"];
        //    authnRequest.Issuer = new Issuer(issuerURL);
        //    authnRequest.ForceAuthn = true;
        //    authnRequest.NameIDPolicy = new NameIDPolicy(null, null, true);
        //    authnRequest.ProtocolBinding = idpToSPBindingRadioButtonList.SelectedValue;
        //    authnRequest.AssertionConsumerServiceURL = assertionConsumerServiceURL;

        //    // Serialize the authentication request to XML for transmission.
        //    XmlElement authnRequestXml = authnRequest.ToXml();

        //    // Don't sign if using HTTP redirect as the generated query string is too long for most browsers.
        //    if (spToIdPBindingRadioButtonList.SelectedValue != SAMLIdentifiers.BindingURIs.HTTPRedirect)
        //    {
        //        // Sign the authentication request.
        //        X509Certificate2 x509Certificate = (X509Certificate2)Application[Global.SPX509Certificate];

        //        SAMLMessageSignature.Generate(authnRequestXml, x509Certificate.PrivateKey, x509Certificate);
        //    }

        //    return authnRequestXml;
        //}

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

        //private string CreateSSOServiceURL()
        //{
        //    return string.Format("{0}?{1}={2}", WebConfigurationManager.AppSettings["idpssoURL"], bindingQueryParameter, HttpUtility.UrlEncode(spToIdPBindingRadioButtonList.SelectedValue));
        //}
    }
}