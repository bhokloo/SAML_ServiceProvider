using ComponentSpace.SAML2;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace MvcExampleIdentityProvider.Controllers
{
    public class SamlController : Controller
    {
        [Authorize]
        public ActionResult InitiateSingleSignOn()
        {
            // Get the name of the logged in user.
            var userName = User.Identity.Name;

            // For demonstration purposes only, include all the claims as SAML attributes.
            // To include a specific claim: ((ClaimsIdentity) User.Identity).FindFirst(ClaimTypes.GivenName).
            var attributes = new Dictionary<string, string>();

            foreach (var claim in ((ClaimsIdentity)User.Identity).Claims)
            {
                attributes[claim.Type] = claim.Value;
            }

            var partnerName = WebConfigurationManager.AppSettings["PartnerName"];
            var relayState = WebConfigurationManager.AppSettings["TargetUrl"];

            // Initiate single sign-on to the service provider (IdP-initiated SSO)
            // by sending a SAML response containing a SAML assertion to the SP.
            // The optional relay state normally specifies the target URL once SSO completes.
            SAMLIdentityProvider.InitiateSSO(Response, userName, attributes, relayState, partnerName);

            return new EmptyResult();
        }

        public ActionResult SingleSignOnService()
        {
            // Receive the authn request from the service provider (SP-initiated SSO).
            SAMLIdentityProvider.ReceiveSSO(Request, out var partnerName);

            // If the user is logged in at the identity provider, complete SSO immediately.
            // Otherwise have the user login before completing SSO.
            if (User.Identity.IsAuthenticated)
            {
                CompleteSingleSignOn();

                return new EmptyResult();
            }
            else
            {
                return RedirectToAction("SingleSignOnServiceCompletion");
            }
        }

        [Authorize]
        public ActionResult SingleSignOnServiceCompletion()
        {
            CompleteSingleSignOn();

            return new EmptyResult();
        }

        public ActionResult SingleLogoutService()
        {
            // Receive the single logout request or response.
            // If a request is received then single logout is being initiated by a partner service provider.
            // If a response is received then this is in response to single logout having been initiated by the identity provider.
            SAMLIdentityProvider.ReceiveSLO(
                Request, 
                Response, 
                out var isRequest, 
                out var hasCompleted, 
                out var logoutReason, 
                out var partnerName, 
                out var relayState);

            if (isRequest)
            {
                // Logout locally.
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                // Respond to the SP-initiated SLO request indicating successful logout.
                SAMLIdentityProvider.SendSLO(Response, null);
            }
            else
            {
                if (hasCompleted)
                {
                    // IdP-initiated SLO has completed.
                    if (!string.IsNullOrEmpty(relayState) && Url.IsLocalUrl(relayState))
                    {
                        return Redirect(relayState);
                    }

                    return RedirectToAction("Index", "Home");
                }
            }

            return new EmptyResult();
        }

        private void CompleteSingleSignOn()
        {
            // Get the name of the logged in user.
            var userName = User.Identity.Name;

            // For demonstration purposes, include some claims.
            var attributes = new Dictionary<string, string>();

            foreach (var claim in ((ClaimsIdentity)User.Identity).Claims)
            {
                attributes[claim.Type] = claim.Value;
            }

            // The user is logged in at the identity provider.
            // Respond to the authn request by sending a SAML response containing a SAML assertion to the SP.
            SAMLIdentityProvider.SendSSO(Response, userName, attributes);
        }
    }
}