using Microsoft.Owin.Security.OAuth;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace ActivantsSamlServiceProvider.Security
{
    public class MyAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        IDictionary<string, string> samlData = new Dictionary<string, string>();

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if(context.Parameters != null)
            {
                foreach (var value in context.Parameters)
                {
                    string[] ssd= value.Value;
                    if(!value.Key.Equals("grant_type"))
                        samlData.Add(value.Key, ssd[0]);
                }
            }
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            if (context.UserName != null && context.UserName != "")
            {
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, context.UserName));
                foreach(var value in samlData)
                {
                    identity.AddClaim(new Claim(value.Key, value.Value));
                }
                context.Validated(identity);
            }
            else
            {
                context.SetError("invalid grant", "incorrect credentials");
                return;
            }
        }

    }
}