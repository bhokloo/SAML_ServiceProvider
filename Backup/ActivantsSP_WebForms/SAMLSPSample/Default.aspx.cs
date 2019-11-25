using System;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;

using ComponentSpace.SAML2;
using System.Web.UI;
using System.Collections.Generic;
using System.Diagnostics;

namespace SAMLSPSample
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("**********************************_Default");
        }
    }
}