using System;
using System.Diagnostics;
using System.Web.Security;

using ComponentSpace.SAML2;

namespace SAMLSPSample.SAMLSP
{
    public partial class SLOService : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("**********************************SLOService");
            bool isRequest = false;
            string logoutReason = null;
            string partnerIdP = null;
            string relayState = null;

            SAMLServiceProvider.ReceiveSLO(Request, out isRequest, out logoutReason, out partnerIdP, out relayState);
            if (isRequest)
            {
                FormsAuthentication.SignOut();
                SAMLServiceProvider.SendSLO(Response, null);
            }
            else
            {           
                Response.Redirect("~/loginSP.aspx");  
            }
        }
    }
}