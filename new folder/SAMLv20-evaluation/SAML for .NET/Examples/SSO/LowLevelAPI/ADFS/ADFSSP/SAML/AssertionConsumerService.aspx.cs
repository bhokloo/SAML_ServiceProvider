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
using ComponentSpace.SAML2.Profiles.SSOBrowser;

namespace ADFSSP.SAML
{
    public partial class AssertionConsumerService : System.Web.UI.Page
    {
        // The query string parameter indicating an error occurred.
        private const string errorQueryParameter = "error";

        // Create an absolute URL from an application relative URL.
        private string CreateAbsoluteURL(string relativeURL)
        {
            return new Uri(Request.Url, ResolveUrl(relativeURL)).ToString();
        }

        // Receive the SAML response from the identity provider.
        private void ReceiveSAMLResponse(out SAMLResponse samlResponse, out string relayState)
        {
            Trace.Write("SP", "Receiving SAML response.");

            // Receive the SAML response.
            XmlElement samlResponseXml = null;

            ServiceProvider.ReceiveSAMLResponseByHTTPPost(Request, out samlResponseXml, out relayState);

            // Deserialize the XML.
            samlResponse = new SAMLResponse(samlResponseXml);

            Trace.Write("SP", "Received SAML response");
        }

        // Process a successful SAML response.
        private void ProcessSuccessSAMLResponse(SAMLResponse samlResponse, string relayState)
        {
            Trace.Write("SP", "Processing successful SAML response");

            // Extract the asserted identity from the SAML response.
            // The SAML assertion may be signed or encrypted and signed.
            SAMLAssertion samlAssertion = null;

            if (samlResponse.GetUnsignedAssertions().Count > 0)
            {
                samlAssertion = samlResponse.GetUnsignedAssertions()[0];
            }
            else if (samlResponse.GetSignedAssertions().Count > 0)
            {
                Trace.Write("SP", "Verifying assertion signature");

                XmlElement samlAssertionXml = samlResponse.GetSignedAssertions()[0];

                // Verify the assertion signature. The embedded signing certificate is used.
                if (!SAMLAssertionSignature.Verify(samlAssertionXml))
                {
                    throw new ArgumentException("The SAML assertion signature failed to verify.");
                }

                samlAssertion = new SAMLAssertion(samlAssertionXml);
            }
            else if (samlResponse.GetEncryptedAssertions().Count > 0)
            {
                Trace.Write("SP", "Decrypting assertion");

                // Load the decryption key.
                X509Certificate2 x509Certificate = (X509Certificate2)Application[Global.SPX509Certificate];

                // Decrypt the encrypted assertion.
                XmlElement samlAssertionXml = samlResponse.GetEncryptedAssertions()[0].DecryptToXml(x509Certificate.PrivateKey, null, null);

                if (SAMLAssertionSignature.IsSigned(samlAssertionXml))
                {
                    Trace.Write("SP", "Verifying assertion signature");

                    // Verify the assertion signature. The embedded signing certificate is used.
                    if (!SAMLAssertionSignature.Verify(samlAssertionXml))
                    {
                        throw new ArgumentException("The SAML assertion signature failed to verify.");
                    }
                }

                samlAssertion = new SAMLAssertion(samlAssertionXml);
            }
            else
            {
                throw new ArgumentException("No assertions in response");
            }

            // Get the subject name identifier.
            string userName = null;

            if (samlAssertion.Subject.NameID != null)
            {
                userName = samlAssertion.Subject.NameID.NameIdentifier;
            }

            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("The SAML assertion doesn't contain a subject name.");
            }

            // Create a login context for the asserted identity.
            Trace.Write("SP", "Automatically logging in user " + userName);
            FormsAuthentication.SetAuthCookie(userName, false);

            // Get the originally requested resource URL from the relay state, if any.
            string redirectURL = "~/";

            RelayState cachedRelayState = RelayStateCache.Remove(relayState);

            if (cachedRelayState != null)
            {
                redirectURL = cachedRelayState.ResourceURL;
            }

            // Redirect to the originally requested resource URL, if any, or the default page.
            Trace.Write("SP", "Redirecting to " + redirectURL);
            Response.Redirect(redirectURL, false);

            Trace.Write("SP", "Processed successful SAML response");
        }

        // Process an error SAML response.
        private void ProcessErrorSAMLResponse(SAMLResponse samlResponse)
        {
            Trace.Write("SP", "Processing error SAML response");

            string errorMessage = null;

            if (samlResponse.Status.StatusMessage != null)
            {
                errorMessage = samlResponse.Status.StatusMessage.Message;
            }

            string redirectURL = String.Format("~/LoginChoice.aspx?{0}={1}", errorQueryParameter, HttpUtility.UrlEncode(errorMessage));

            Response.Redirect(redirectURL, false);

            Trace.Write("SP", "Processed error SAML response");
        }

        // Process the SAML response returned by the identity provider in response
        // to the authentication request sent by the service provider.
        private void ProcessSAMLResponse()
        {
            // Receive the SAML response.
            SAMLResponse samlResponse = null;
            string relayState = null;

            ReceiveSAMLResponse(out samlResponse, out relayState);

            // Check whether the SAML response indicates success or an error and process accordingly.
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
                Trace.Write("SP", "Assertion consumer service");

                ProcessSAMLResponse();
            }

            catch (Exception exception)
            {
                Trace.Write("SP", "Error in assertion consumer service", exception);
                Response.Write(exception.Message);
            }
        }
    }
}
