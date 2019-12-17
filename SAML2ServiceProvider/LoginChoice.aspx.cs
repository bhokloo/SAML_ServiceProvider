using System;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Threading;
using System.Xml;
using ComponentSpace.SAML2;
using ComponentSpace.SAML2.Assertions;
using ComponentSpace.SAML2.Protocols;
using ComponentSpace.SAML2.Bindings;
using ComponentSpace.SAML2.Profiles.SSOBrowser;
using ComponentSpace.SAML2.Utility;

namespace SAML2ServiceProvider
{
    public partial class LoginChoice : System.Web.UI.Page
    {
        private class LoginLocations
        {
            public const string ServiceProvider = "SP";
        }

        private const string bindingQueryParameter = "binding";

        private const string errorQueryParameter = "error";

        private string CreateAbsoluteURL(string relativeURL)
        {
            return new Uri(Request.Url, ResolveUrl(relativeURL)).ToString();
        }

        private string CreateAssertionConsumerServiceURL()
        {
            return string.Format("{0}?{1}={2}", CreateAbsoluteURL("~/SAML/AssertionConsumerService.aspx"), bindingQueryParameter, HttpUtility.UrlEncode("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Artifact"));
        }

        private string CreateSSOServiceURL()
        {
            return string.Format("{0}?{1}={2}", WebConfigurationManager.AppSettings["idpssoURL"], bindingQueryParameter, HttpUtility.UrlEncode("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Artifact"));
        }

        private XmlElement CreateAuthnRequest()
        {
            string issuerURL = CreateAbsoluteURL("~/");
            string assertionConsumerServiceURL = CreateAssertionConsumerServiceURL();

            AuthnRequest authnRequest = new AuthnRequest();
            authnRequest.Destination = WebConfigurationManager.AppSettings["idpssoURL"];
            authnRequest.Issuer = new Issuer(issuerURL);
            authnRequest.ForceAuthn = true;
            authnRequest.NameIDPolicy = new NameIDPolicy(null, null, true);
            authnRequest.ProtocolBinding = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Artifact";
            authnRequest.AssertionConsumerServiceURL = assertionConsumerServiceURL;
            XmlElement authnRequestXml = authnRequest.ToXml();
            X509Certificate2 x509Certificate = (X509Certificate2)Application[Global.SPX509Certificate];
            SAMLMessageSignature.Generate(authnRequestXml, x509Certificate.PrivateKey, x509Certificate);
            return authnRequestXml;
        }

        private void RequestLoginAtIdentityProvider()
        {
            XmlElement authnRequestXml = CreateAuthnRequest();
            string spResourceURL = CreateAbsoluteURL(FormsAuthentication.GetRedirectUrl("", false));
            string relayState = RelayStateCache.Add(new RelayState(spResourceURL, null));
            string idpURL = CreateSSOServiceURL();

            string identificationURL = CreateAbsoluteURL("~/");
            HTTPArtifactType4 httpArtifact = new HTTPArtifactType4(HTTPArtifactType4.CreateSourceId(identificationURL), HTTPArtifactType4.CreateMessageHandle());
            HTTPArtifactState httpArtifactState = new HTTPArtifactState(authnRequestXml, null);
            HTTPArtifactStateCache.Add(httpArtifact, httpArtifactState);
            ServiceProvider.SendArtifactByHTTPArtifact(Response, idpURL, httpArtifact, relayState, false);
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string licenseType;
            if (License.IsLicensed)
            {
                licenseType = "Licensed";
            }
            else
            {
                licenseType = $"Evaluation (Expires {License.Expires.ToShortDateString()})";
            }
            Trace.Write("*************************", licenseType);

            if (!string.IsNullOrEmpty(Request.QueryString[errorQueryParameter]))
            {
                errorMessageLabel.Text = Request.QueryString[errorQueryParameter];
            }
            else
            {
                errorMessageLabel.Text = string.Empty;
            }
        }

        protected void continueButton_Click(object sender, EventArgs e)
        {
            try
            {
                 RequestLoginAtIdentityProvider();
            }

            catch (ThreadAbortException)
            {
            }

            catch (Exception exception)
            {
                Trace.Write("SP", "Error on login choice page", exception);
            }
        }
    }
}
