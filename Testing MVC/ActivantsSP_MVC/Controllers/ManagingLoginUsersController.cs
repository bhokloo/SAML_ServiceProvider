using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ActivantsSP.Utility
{
    public class ManagingLoginUsersController : Controller
    {
        public void deleteClientTokens()
        {
            if(User.Identity.IsAuthenticated)
            {
                var browserProviderKey = "";
                if (Request.Cookies["SAML_SessionId"] != null)
                {
                    browserProviderKey = Request.Cookies["SAML_SessionId"].Value;
                }
                string commandText = "Delete from AspNetUserLogins where ProviderKey = @browserProviderKey";
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("ProviderKey", browserProviderKey);

                using (var context = new Context())
                {
                    const string query = "DELETE FROM [dbo].[Customers] WHERE [id]={0}";
                    var rows = context.Database.ExecuteSqlCommand(query, id);
                    // rows >= 1 - count of deleted rows,
                    // rows = 0 - nothing to delete.
                }

                //return _database.Execute(commandText, parameters);
            }

           
            //parameters.Add("UserId", userId);

            //return _database.Execute(commandText, parameters);

            //context.Database.ExecuteSqlCommand()

        }
    }
}