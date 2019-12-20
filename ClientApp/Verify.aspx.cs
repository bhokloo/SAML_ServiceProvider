using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClientApp
{
    public partial class Verify : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void logoutButton_Click(object sender, EventArgs e)
        {
            try
            {
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