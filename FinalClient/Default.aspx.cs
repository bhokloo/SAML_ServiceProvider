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
using System.Web.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace FinalClient
{
    public partial class _Default : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void logoutButton_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("http://localhost:64712/logout.aspx?idpLogoutURL=http://localhost:64614/SAML/SLOService.aspx&target=http://localhost:64723");
                FormsAuthentication.SignOut();
                Session.Abandon();
            }

            catch (Exception exception)
            {
                Trace.Write("SP", "Error on logout page", exception);
            }
        }
    }
}
