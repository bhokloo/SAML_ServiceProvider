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

namespace SAML2ServiceProvider.SAML
{
    public partial class ArtifactResponder : System.Web.UI.Page
    {
        private string CreateAbsoluteURL(string relativeURL)
        {
            return new Uri(Request.Url, ResolveUrl(relativeURL)).ToString();
        }

        private void ProcessArtifactResolve()
        {
            Trace.Write("SP", "Processing artifact resolve request");

            XmlElement artifactResolveXml = ArtifactResolver.ReceiveArtifactResolve(Request);
            ArtifactResolve artifactResolve = new ArtifactResolve(artifactResolveXml);
            HTTPArtifactType4 httpArtifact = new HTTPArtifactType4(artifactResolve.Artifact.ArtifactValue);
            HTTPArtifactState httpArtifactState = HTTPArtifactStateCache.Remove(httpArtifact);

            if (httpArtifactState == null)
                return;

            ArtifactResponse artifactResponse = new ArtifactResponse();
            artifactResponse.Issuer = new Issuer(CreateAbsoluteURL("~/"));
            artifactResponse.SAMLMessage = httpArtifactState.SAMLMessage;
            XmlElement artifactResponseXml = artifactResponse.ToXml();
            ArtifactResolver.SendArtifactResponse(Response, artifactResponseXml);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                ProcessArtifactResolve();
            }

            catch (Exception exception)
            {
                Trace.Write("SP", "Error in artifact responder", exception);
            }
        }
    }
}
