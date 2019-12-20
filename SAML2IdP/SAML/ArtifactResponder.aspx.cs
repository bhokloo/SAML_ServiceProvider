using System;
using System.Data;
using System.Configuration;
using System.Collections;
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
using System.Security.Cryptography.X509Certificates;

namespace SAML2IdP.SAML
{
    public partial class ArtifactResponder : System.Web.UI.Page
    {
        private string CreateAbsoluteURL(string relativeURL)
        {
            return new Uri(Request.Url, ResolveUrl(relativeURL)).ToString();
        }

        private void ProcessArtifactResolve()
        {
            Trace.Write("IdP", "Processing artifact resolve request");

            X509Certificate2 x509Certificate = (X509Certificate2)Application[Global.SPX509Certificate];
            

            XmlElement artifactResolveXml = ArtifactResolver.ReceiveArtifactResolve(Request);

            if (!SAMLMessageSignature.Verify(artifactResolveXml, x509Certificate))
            {
                throw new ArgumentException("The SAML response signature failed to verify.");
            }

            ArtifactResolve artifactResolve = new ArtifactResolve(artifactResolveXml);

            HTTPArtifactType4 httpArtifact = new HTTPArtifactType4(artifactResolve.Artifact.ArtifactValue);

            HTTPArtifactState httpArtifactState = HTTPArtifactStateCache.Remove(httpArtifact);

            if (httpArtifactState == null)
            {
                Trace.Write("IdP", string.Format("The artifact {0} is not recognized.", artifactResolve.Artifact.ArtifactValue));
                return;
            }

            ArtifactResponse artifactResponse = new ArtifactResponse();

            artifactResponse.Issuer = new Issuer(CreateAbsoluteURL("~/"));

            artifactResponse.SAMLMessage = httpArtifactState.SAMLMessage;

            XmlElement artifactResponseXml = artifactResponse.ToXml();

            ArtifactResolver.SendArtifactResponse(Response, artifactResponseXml);

            Trace.Write("IdP", "Processed artifact resolve request");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Trace.Write("IdP", "Artifact responder");

                ProcessArtifactResolve();

            }
            catch (Exception exception)
            {
                Trace.Write("IdP", "Error in artifact responder", exception);
            }
        }
    }
}
