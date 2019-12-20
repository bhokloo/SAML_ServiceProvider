using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DemoApp
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var targetURL = "";
            Response.Redirect("https://stg-saml.singpass.gov.sg/FIM/sps/SingpassIDPFed/saml20/logininitial?RequestBinding=HTTPArtifact&ResponseBinding=HTTPArtifact&PartnerId=https://samlsppilot.imda.gov.sg/SAMLSP/SAML/AssertionConsumerService.aspx&Target=https://schemes-uat.imda.gov.sg/SingPassAuth/verify.aspx?function=&NameIdFormat=Email&esrvcID=OSAM&param1=abc&param2=def");

        }
    }
}