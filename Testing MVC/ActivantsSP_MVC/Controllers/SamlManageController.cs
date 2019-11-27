using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ActivantsSP.Controllers
{
    public class SamlManageController : Controller
    {
        public JsonResult getTokens()
        {
            string[] jsonObjects = new string[] { };
            var context = new IdentityDbContext();
            return null;
        }


        public static bool deleteClientTokens(string value, string url)
        {
            var browserSamCookie = value + '%';
            var ClientUrl = url;

            using (var context = new IdentityDbContext())
            {
                try
                {
                    var trustedSamlSessionId = context.Database.SqlQuery<IdentityUserLogin>("SELECT * FROM [AspNetUserLogins] WHERE ProviderKey LIKE @browserSamCookie", new SqlParameter("@browserSamCookie", browserSamCookie)).ToList();
                    if(trustedSamlSessionId.Count > 0)
                    {

                        foreach (var entry in trustedSamlSessionId)
                        {
                            if (entry.LoginProvider.Contains(ClientUrl) && entry.ProviderKey.Contains(value))
                            {
                                context.Database.ExecuteSqlCommand("DELETE FROM [AspNetUserLogins] WHERE ProviderKey = @ProviderKey", new SqlParameter("@ProviderKey", entry.ProviderKey));
                                
                            }
                        }
                        context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    
                }
                catch(Exception e)
                {
                    return false;
                }
            }
        }
    }
}