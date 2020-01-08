using ComponentSpace.SAML2.Assertions;
using ComponentSpace.SAML2.Profiles.SingleLogout;
using ComponentSpace.SAML2.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace SAML2SP
{
    public partial class logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                LogoutRequest logoutRequest = new LogoutRequest();
                logoutRequest.Issuer = new Issuer(CreateAbsoluteURL("~/"));
                logoutRequest.NameID = new NameID(Context.User.Identity.Name);
                XmlElement logoutRequestXml = logoutRequest.ToXml();
                string logoutURL = null;
                string relayState = null;
                if(Request.QueryString["idpLogoutURL"] != null)
                {
                    logoutURL = Request.QueryString["idpLogoutURL"];
                }
                if (Request.QueryString["target"] != null)
                {
                    relayState = Request.QueryString["target"];
                }
                X509Certificate2 x509Certificate = (X509Certificate2)Application[Global.SPX509Certificate];
                SingleLogoutService.SendLogoutRequestByHTTPRedirect(Response, logoutURL, logoutRequestXml, relayState, x509Certificate.PrivateKey, null);
                FormsAuthentication.SignOut();
                Session.Abandon();
            }

            catch (Exception exception)
            {
                Trace.Write("SP", "Error on logout page", exception);
            }
        }

        private string CreateAbsoluteURL(string relativeURL)
        {
            return new Uri(Request.Url, ResolveUrl(relativeURL)).ToString();
        }
    }
}