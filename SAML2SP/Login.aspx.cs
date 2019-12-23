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
using System.Web.Configuration;

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
            try
            {
                string idpURL = WebConfigurationManager.AppSettings["idpURL"];
                string ClientURL = WebConfigurationManager.AppSettings["targetURL"];
                
                //Response.Redirect("http://localhost:64614/login.aspx?target=http://localhost:64712");
                Response.Redirect(idpURL + "?target="+ ClientURL);
            }
            catch(Exception ex)
            {
                errorMessageLabel.Text = ex.GetType().ToString();
            }
        }
    }
}
