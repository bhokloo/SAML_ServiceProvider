using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Xml;
using ComponentSpace.SAML2;
using ComponentSpace.SAML2.Assertions;
using ComponentSpace.SAML2.Protocols;
using ComponentSpace.SAML2.Bindings;
using ComponentSpace.SAML2.Profiles.ArtifactResolution;
using ComponentSpace.SAML2.Profiles.SSOBrowser;

namespace SAML2ServiceProvider.SAML
{
    public partial class AssertionConsumerService : System.Web.UI.Page
    {
        private const string bindingQueryParameter = "binding";

        private const string errorQueryParameter = "error";

        private string CreateAbsoluteURL(string relativeURL)
        {
            return new Uri(Request.Url, ResolveUrl(relativeURL)).ToString();
        }

        private void ReceiveSAMLResponse(ref SAMLResponse samlResponse, ref string relayState)
        {
            Trace.Write("SP", "Receiving SAML response");
            string bindingType = Request.QueryString[bindingQueryParameter];
            XmlElement samlResponseXml = null;

            switch (bindingType)
            {
                case SAMLIdentifiers.BindingURIs.HTTPArtifact:
                    HTTPArtifact httpArtifact = null;
                    ServiceProvider.ReceiveArtifactByHTTPArtifact(Request, false, out httpArtifact, out relayState);
                    ArtifactResolve artifactResolve = new ArtifactResolve();
                    artifactResolve.Issuer = new Issuer(CreateAbsoluteURL("~/"));
                    artifactResolve.Artifact = new Artifact(httpArtifact.ToString());
                    XmlElement artifactResolveXml = artifactResolve.ToXml();
                    string spArtifactResponderURL = WebConfigurationManager.AppSettings["idpArtifactResponderURL"];
                    XmlElement artifactResponseXml = ArtifactResolver.SendRequestReceiveResponse(spArtifactResponderURL, artifactResolveXml);
                    ArtifactResponse artifactResponse = new ArtifactResponse(artifactResponseXml);
                    samlResponseXml = artifactResponse.SAMLMessage;
                    break;

                default:
                    return;
            }

            X509Certificate2 x509Certificate = (X509Certificate2)Application[Global.IdPX509Certificate];
            if (!SAMLMessageSignature.Verify(samlResponseXml, x509Certificate))
            {
                throw new ArgumentException("The SAML response signature failed to verify.");
            }
            samlResponse = new SAMLResponse(samlResponseXml);
        }

        private void ProcessSuccessSAMLResponse(SAMLResponse samlResponse, string relayState)
        {
            SAMLAssertion samlAssertion = (SAMLAssertion)samlResponse.Assertions[0];
            string userName = samlAssertion.Subject.NameID.NameIdentifier;
            RelayState cachedRelayState = RelayStateCache.Remove(relayState);

            if (cachedRelayState == null)
            {
                return;
            }
            FormsAuthentication.SetAuthCookie(userName, false);
            Response.Redirect(cachedRelayState.ResourceURL, false);
        }

        // Process an error SAML response.
        private void ProcessErrorSAMLResponse(SAMLResponse samlResponse)
        {
            string errorMessage = null;
            if ((samlResponse.Status.StatusMessage != null))
            {
                errorMessage = samlResponse.Status.StatusMessage.Message;
            }
            string redirectURL = string.Format("~/LoginChoice.aspx?{0}={1}", errorQueryParameter, HttpUtility.UrlEncode(errorMessage));
            Response.Redirect(redirectURL, false);
        }

        private void ProcessSAMLResponse()
        {
            SAMLResponse samlResponse = null;
            string relayState = null;
            ReceiveSAMLResponse(ref samlResponse, ref relayState);

            if (samlResponse == null)
                return;

            if (samlResponse.IsSuccess())
            {
                ProcessSuccessSAMLResponse(samlResponse, relayState);
            }
            else
            {
                ProcessErrorSAMLResponse(samlResponse);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                ProcessSAMLResponse();
            }

            catch (Exception exception)
            {
                Trace.Write("SP", "Error in assertion consumer service", exception);
            }
        }
    }
}
