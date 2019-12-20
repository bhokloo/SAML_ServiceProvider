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
using ComponentSpace.SAML2.Utility;

namespace SAML2SP
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string licenseType;
            if (License.IsLicensed)
            {
                licenseType = "Licensed";
            }
            else
            {
                licenseType = $"Evaluation (Expires {License.Expires.ToShortDateString()})";
            }
            Trace.Write("*************************", licenseType);

            if (!string.IsNullOrEmpty(Request.QueryString["error"]))
            {
                errorMessageLabel.Text = Request.QueryString["error"];
            }
            else
            {
                errorMessageLabel.Text = string.Empty;
            }
        }

        protected void continueButton_Click(object sender, EventArgs e)
        {
            var username = "indrajit";
            var password = "password";

            if (FormsAuthentication.Authenticate(username, password))
            {
                FormsAuthentication.RedirectFromLoginPage(username, false);
            }
            else
            {
                errorMessageLabel.Text = "Invalid credentials. The user name and password should be \"sp-user\" and \"password\".";
            }
        }
    }
}
