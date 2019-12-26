using System;
using System.Web;


namespace SAML2IdP
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var target = "";
            var partnerId = "";
            if(Request.QueryString["target"] != null || Request.QueryString["partnerId"] != null)
            {
                target = Request.QueryString["target"];
                partnerId = Request.QueryString["partnerId"];
            }

            // Construct a URL to the local SSO service and specifying the target URL of the SP.
            spHyperLink.NavigateUrl = string.Format("~/SAML/SSOService.aspx?target={0}&partnerId={1}", target, partnerId);
        }
    }
}
