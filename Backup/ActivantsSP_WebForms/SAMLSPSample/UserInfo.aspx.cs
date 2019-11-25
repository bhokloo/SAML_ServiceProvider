using ComponentSpace.SAML2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace SAMLSPSample
{
    public partial class UserInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("**********************************UserInfoPageLoad");
            IDictionary<string, string> attributes = (IDictionary<string, string>)Session[SAMLSP.AssertionConsumerService.AttributesSessionKey];
            Debug.WriteLine(attributes);
            Debug.WriteLine(attributes);
            if(attributes == null)
            {
                return;
            }
            string htmlTable = "<table class='table table-bordered' id='myTable'>" +
                               "<thead><tr> <th> Attribute </th> <th> Values </th></tr></thead><tbody>";

            
            foreach (KeyValuePair<string, string> entry in attributes)
            {
                htmlTable += "<tr><td>" + entry.Key + "</td><td>" + entry.Value + "</td></tr>";
            }

            htmlTable += "</tbody></table>";
            lblTable.Text = htmlTable;
        }

        private class AttributeDataSource
        {
            private string attributeName;
            private string attributeValue;

            public static IList<AttributeDataSource> Get(IDictionary<string, string> attributes)
            {
                Debug.WriteLine("**********************************AttributeDataSource");

                IList<AttributeDataSource> attributeDataSources = new List<AttributeDataSource>();

                foreach (string attributeName in attributes.Keys)
                {
                    attributeDataSources.Add(new AttributeDataSource(attributeName, HttpUtility.HtmlEncode(attributes[attributeName])));
                }

                return attributeDataSources;
            }

            private AttributeDataSource(string attributeName, string attributeValue)
            {
                this.attributeName = attributeName;
                this.attributeValue = attributeValue;
            }

            public string AttributeName
            {
                get
                {
                    return attributeName;
                }
            }

            public string AttributeValue
            {
                get
                {
                    return attributeValue;
                }
            }
        }

        protected void logoutButton_Click(object sender, EventArgs e)
        {
            // Logout locally.
            FormsAuthentication.SignOut();

            if (SAMLServiceProvider.CanSLO(WebConfigurationManager.AppSettings[AppSettings.PartnerIdP]))
            {
                // Request logout at the identity provider.
                string partnerIdP = WebConfigurationManager.AppSettings[AppSettings.PartnerIdP];
                SAMLServiceProvider.InitiateSLO(Response, null, null, partnerIdP);
            }
            else
            {
                Response.Redirect("loginSP.aspx");
            }

        }
    }
}