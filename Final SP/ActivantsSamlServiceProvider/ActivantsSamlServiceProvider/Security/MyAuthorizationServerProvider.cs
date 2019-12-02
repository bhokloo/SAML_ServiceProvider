using Microsoft.Owin.Security.OAuth;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace ActivantsSamlServiceProvider.Security
{
    public class MyAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string id = context.Parameters.Where(f => f.Key == "id").Select(f => f.Value).SingleOrDefault()[0];
            context.OwinContext.Set<string>("id", id);
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            if (context.UserName != null && context.UserName != "")
            {
                identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                identity.AddClaim(new Claim(ClaimTypes.SerialNumber, context.OwinContext.Get<string>("id")));
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