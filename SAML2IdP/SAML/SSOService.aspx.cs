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
using System.Security.Cryptography.Xml;

namespace SAML2IdP.SAML
{
    public partial class SSOService : System.Web.UI.Page
    {
        private const string targetQueryParameter = "target";

        private string CreateAbsoluteURL(string relativeURL)
        {
            return new Uri(Request.Url, ResolveUrl(relativeURL)).ToString();
        }

        private SAMLResponse CreateSAMLResponse()
        {
            Trace.Write("IdP", "Creating SAML response");

            SAMLResponse samlResponse = new SAMLResponse();

            samlResponse.Destination = Configuration.AssertionConsumerServiceURL;

            Issuer issuer = new Issuer(CreateAbsoluteURL("~/"));

            samlResponse.Issuer = issuer;

            samlResponse.Status = new Status(SAMLIdentifiers.PrimaryStatusCodes.Success, null);

            SAMLAssertion samlAssertion = new SAMLAssertion();

            samlAssertion.Issuer = issuer;

            Subject subject = new Subject(new NameID(Context.User.Identity.Name));

            SubjectConfirmation subjectConfirmation = new SubjectConfirmation(SAMLIdentifiers.SubjectConfirmationMethods.Bearer);

            SubjectConfirmationData subjectConfirmationData = new SubjectConfirmationData();

            subjectConfirmationData.Recipient = Configuration.AssertionConsumerServiceURL;

            subjectConfirmation.SubjectConfirmationData = subjectConfirmationData;

            subject.SubjectConfirmations.Add(subjectConfirmation);

            samlAssertion.Subject = subject;

            AuthnStatement authnStatement = new AuthnStatement();

            authnStatement.AuthnContext = new AuthnContext();

            authnStatement.AuthnContext.AuthnContextClassRef = new AuthnContextClassRef(SAMLIdentifiers.AuthnContextClasses.Password);

            samlAssertion.Statements.Add(authnStatement);

            //samlResponse.Assertions.Add(samlAssertion);

            var KeyEncryptionMethod = EncryptedXml.XmlEncRSA15Url;
            var DataEncryptionMethod = EncryptedXml.XmlEncAES256Url;

            X509Certificate2 x509Certificate_sp = (X509Certificate2)Application[Global.SPX509Certificate];
            EncryptedAssertion encryptedAssertion = new EncryptedAssertion(samlAssertion, x509Certificate_sp, new System.Security.Cryptography.Xml.EncryptionMethod(KeyEncryptionMethod), new System.Security.Cryptography.Xml.EncryptionMethod(DataEncryptionMethod));
            samlResponse.Assertions.Add(encryptedAssertion);

            Trace.Write("IdP", "Created SAML response");

            return samlResponse;
        }

        private void SendSAMLResponse(SAMLResponse samlResponse, string relayState)
        {
            Trace.Write("IdP", "Sending SAML response");

            XmlElement samlResponseXml = samlResponse.ToXml();

            X509Certificate2 x509Certificate = (X509Certificate2)Application[Global.IdPX509Certificate];
            //X509Certificate2 x509Certificate_sp = (X509Certificate2)Application[Global.SPX509Certificate];
            SAMLMessageSignature.Generate(samlResponseXml, x509Certificate.PrivateKey, x509Certificate);

            string identificationURL = CreateAbsoluteURL("~/");

            HTTPArtifactType4 httpArtifact = new HTTPArtifactType4(HTTPArtifactType4.CreateSourceId(identificationURL), HTTPArtifactType4.CreateMessageHandle());
            
            HTTPArtifactState httpArtifactState = new HTTPArtifactState(samlResponseXml, null);
            
            HTTPArtifactStateCache.Add(httpArtifact, httpArtifactState);
           
            IdentityProvider.SendArtifactByHTTPArtifact(Response, Configuration.AssertionConsumerServiceURL, httpArtifact, relayState, false);

            Trace.Write("IdP", "Sent SAML response");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string targetURL = Request.QueryString["target"];
               

                if (string.IsNullOrEmpty(targetURL))
                {
                    return;
                }
                Trace.Write("IdP", "Target URL: " + targetURL);

                SAMLResponse samlResponse = CreateSAMLResponse();

                SendSAMLResponse(samlResponse, targetURL);
            }

            catch (Exception exception)
            {
                Trace.Write("IdP", "Error in SSO service", exception);
            }
        }
    }
}
