using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Security;

using ComponentSpace.SAML2;
using ComponentSpace.SAML2.Data;
using System.Diagnostics;

namespace SAMLSPSample.SAMLSP
{
    public partial class AssertionConsumerService : System.Web.UI.Page
    {
        public const string AttributesSessionKey = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("***********************************AssertionConsumerService");
            bool isInResponseTo = false;
            string partnerIdP = null;
            string authnContext = null;
            string userName = null;
            IDictionary<string, string> attributes = null;
            string targetUrl = null;
            SAMLServiceProvider.ReceiveSSO(Request, out isInResponseTo, out partnerIdP, out authnContext, out userName, out attributes, out targetUrl);
            if (targetUrl == null)
            {
                targetUrl = "~/UserInfo";
            }
            FormsAuthentication.SetAuthCookie(userName, false);
            Session[AttributesSessionKey] = attributes;
            Response.Redirect(targetUrl, false);
        }
    }
}
