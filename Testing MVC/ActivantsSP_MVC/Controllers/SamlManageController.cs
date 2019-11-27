using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            var client_access_token = Request.Headers["access_token"];
            using (var context = new IdentityDbContext())
            {
                try
                {
                    var trustedAccessToken = context.Database.SqlQuery<IdentityUserLogin>("SELECT * FROM [AspNetUserLogins] WHERE ProviderKey = @client_access_token", new SqlParameter("@client_access_token", client_access_token)).FirstOrDefault();
                    if (trustedAccessToken != null)
                    {
                        var clientAttributes = context.Database.SqlQuery<IdentityUserClaim>("SELECT * FROM [AspNetUserClaims] WHERE UserId = @userId", new SqlParameter("@userId", trustedAccessToken.UserId)).ToList();
                        if(clientAttributes != null)
                        {
                            Dictionary<string, string> values = new Dictionary<string, string>();
                            foreach (var claimsOfClient in clientAttributes)
                            {
                                values.Add(claimsOfClient.ClaimType, claimsOfClient.ClaimValue);
                            }
                            Response.ContentType = "application/json";
                            Response.ContentEncoding = System.Text.Encoding.UTF8;
                            var jsonData = Json(values, JsonRequestBehavior.AllowGet);
                            return jsonData;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    return null;
                }
            }
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