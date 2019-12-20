using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;

using ComponentSpace.SAML2;
using ComponentSpace.SAML2.Assertions;
using ComponentSpace.SAML2.Protocols;
using ComponentSpace.SAML2.Bindings;
using ComponentSpace.SAML2.Profiles.ArtifactResolution;
using ComponentSpace.SAML2.Profiles.SSOBrowser;
using System.Web.Configuration;

namespace SAML2SP.SAML
{
    public partial class AssertionConsumerService : System.Web.UI.Page
    {
        private const string errorQueryParameter = "error";

        private string CreateAbsoluteURL(string relativeURL)
        {
            return new Uri(Request.Url, ResolveUrl(relativeURL)).ToString();
        }

        private void ReceiveSAMLResponse(ref SAMLResponse samlResponse, ref string relayState)
        {
            Trace.Write("SP", "Receiving SAML response");

            XmlElement samlResponseXml = null;
            HTTPArtifact httpArtifact = null;
            ServiceProvider.ReceiveArtifactByHTTPArtifact(Request, false, out httpArtifact, out relayState);
            ArtifactResolve artifactResolve = new ArtifactResolve();

            artifactResolve.Issuer = new Issuer(CreateAbsoluteURL("~/"));

            artifactResolve.Artifact = new Artifact(httpArtifact.ToString());
            XmlElement artifactResolveXml = artifactResolve.ToXml();
            string idpArtifactResponderURL = WebConfigurationManager.AppSettings["idpArtifactResponderURL"];

            //signing the Artifact resolve as per SP/CP specs

            X509Certificate2 x509Certificate_sp = (X509Certificate2)Application[Global.SPX509Certificate];
            SAMLMessageSignature.Generate(artifactResolveXml, x509Certificate_sp.PrivateKey, x509Certificate_sp);

            //calling to SP/CP with artifact resolve.
            XmlElement artifactResponseXml = ArtifactResolver.SendRequestReceiveResponse(idpArtifactResponderURL, artifactResolveXml);

            ArtifactResponse artifactResponse = new ArtifactResponse(artifactResponseXml);
            samlResponseXml = artifactResponse.SAMLMessage;

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
            var targetUrl = relayState;
            RelayState cachedRelayState = RelayStateCache.Remove(relayState);
            FormsAuthentication.SetAuthCookie(userName, false);
            Response.Redirect(targetUrl, false);
        }

        // Process an error SAML response.
        private void ProcessErrorSAMLResponse(SAMLResponse samlResponse)
        {
            string errorMessage = null;
            if ((samlResponse.Status.StatusMessage != null))
            {
                errorMessage = samlResponse.Status.StatusMessage.Message;
            }
            string redirectURL = string.Format("~/Login.aspx?{0}={1}", errorQueryParameter, HttpUtility.UrlEncode(errorMessage));
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