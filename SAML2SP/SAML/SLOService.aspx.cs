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
using ComponentSpace.SAML2.Profiles.SingleLogout;

namespace SAML2SP.SAML
{
    public partial class SLOService : System.Web.UI.Page
    {
        private string CreateAbsoluteURL(string relativeURL)
        {
            return new Uri(Request.Url, ResolveUrl(relativeURL)).ToString();
        }
        private LogoutResponse CreateLogoutResponse()
        {
            LogoutResponse logoutResponse = new LogoutResponse();
            logoutResponse.Status = new Status(SAMLIdentifiers.PrimaryStatusCodes.Success, null);
            logoutResponse.Issuer = new Issuer(CreateAbsoluteURL("~/"));
            return logoutResponse;
        }

        private void SendLogoutResponse(ref LogoutResponse logoutResponse)
        {
            XmlElement logoutResponseXml = logoutResponse.ToXml();
            //X509Certificate2 x509Certificate = (X509Certificate2)Application[Global.SPX509Certificate];
           // SingleLogoutService.SendLogoutResponseByHTTPRedirect(Response, WebConfigurationManager.AppSettings["idpLogoutURL"], logoutResponseXml, null, x509Certificate.PrivateKey, null);
        }

        private void ProcessLogoutRequest(LogoutRequest logoutRequest, string relayState)
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            LogoutResponse logoutResponse = CreateLogoutResponse();
            SendLogoutResponse(ref logoutResponse);
        }

        private void ProcessLogoutResponse(LogoutResponse logoutResponse, string relayState)
        {
            Response.Redirect("~/", false);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                XmlElement logoutMessage = null;
                string relayState = null;
                bool isRequest = false;
                bool signed = false;
                X509Certificate2 x509Certificate = (X509Certificate2)Application[Global.IdPX509Certificate];

                SingleLogoutService.ReceiveLogoutMessageByHTTPRedirect(Request, out logoutMessage, out relayState, out isRequest, out signed, x509Certificate.PublicKey.Key);
                if (isRequest)
                {
                    ProcessLogoutRequest(new LogoutRequest(logoutMessage), relayState);
                }
                else
                {
                    ProcessLogoutResponse(new LogoutResponse(logoutMessage), relayState);
                }
            }

            catch (Exception exception)
            {
                Trace.Write("SP", "Error in single logout service.", exception);
            }
        }
    }
}
