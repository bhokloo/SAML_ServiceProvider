using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using ComponentSpace.SAML2.Notifications;
using ComponentSpace.SAML2.Protocols;
namespace SAMLSPSample.SAMLSP
{
    public partial class SAMLObserver : AbstractSAMLObserver
    {
        public override void OnLogoutResponseReceived(LogoutResponse logoutResponse)
        {
            Debug.WriteLine("**********************************SAMLObserver");
            if (logoutResponse.IsSuccess())
                System.Diagnostics.Debug.WriteLine("********************************************logoutResponse is Success");
            else
                System.Diagnostics.Debug.WriteLine("*********************************************logoutResponse is NOT Success");
        }
    }
}