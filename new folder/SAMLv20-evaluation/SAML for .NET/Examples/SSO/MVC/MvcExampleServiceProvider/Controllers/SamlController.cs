using ComponentSpace.SAML2;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using MvcExampleServiceProvider.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace MvcExampleServiceProvider.Controllers
{
    public class SamlController : Controller
    {
        public ActionResult InitiateSingleSignOn(string returnUrl = null)
        {
            var partnerName = WebConfigurationManager.AppSettings["PartnerName"];

            // To login automatically at the service provider, 
            // initiate single sign-on to the identity provider (SP-initiated SSO).            
            // The return URL is remembered as SAML relay state.
            SAMLServiceProvider.InitiateSSO(Response, partnerName, returnUrl);

            return new EmptyResult();
        }

        public ActionResult AssertionConsumerService()
        {
            // Receive and process the SAML assertion contained in the SAML response.
            // The SAML response is received either as part of IdP-initiated or SP-initiated SSO.
            SAMLServiceProvider.ReceiveSSO(
                Request, 
                out var isInResponseTo, 
                out var partnerName, 
                out var authnContext, 
                out var userName, 
                out IDictionary<string, string> attributes, 
                out var relayState);

            // Automatically provision the user.
            // If the user doesn't exist locally then create the user.
            // Automatic provisioning is an optional step.
            var applicationUserManager = HttpContext.GetOwinContext().Get<ApplicationUserManager>();
            var applicationUser = applicationUserManager.FindByName(userName);

            if (applicationUser == null)
            {
                applicationUser = new ApplicationUser();

                applicationUser.UserName = userName;
                applicationUser.Email = userName;
                applicationUser.EmailConfirmed = true;

                if (attributes.ContainsKey(ClaimTypes.GivenName))
                {
                    applicationUser.Claims.Add(new IdentityUserClaim() { ClaimType = ClaimTypes.GivenName, ClaimValue = attributes[ClaimTypes.GivenName], UserId = applicationUser.Id });
                }

                if (attributes.ContainsKey(ClaimTypes.Surname))
                {
                    applicationUser.Claims.Add(new IdentityUserClaim() { ClaimType = ClaimTypes.Surname, ClaimValue = attributes[ClaimTypes.Surname], UserId = applicationUser.Id });
                }

                var identityResult = applicationUserManager.Create(applicationUser);

                if (!identityResult.Succeeded)
                {
                    throw new Exception(string.Format("The user {0} couldn't be created.\n{1}", userName, identityResult));
                }
            }

            // Automatically login using the asserted identity.
            var applicationSignInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            applicationSignInManager.SignIn(applicationUser, false, false);

            // Redirect to the target URL if any.
            if (!string.IsNullOrEmpty(relayState) && Url.IsLocalUrl(relayState))
            {
                return Redirect(relayState);
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult SingleLogoutService()
        {
            // Receive the single logout request or response.
            // If a request is received then single logout is being initiated by the identity provider.
            // If a response is received then this is in response to single logout having been initiated by the service provider.
            SAMLServiceProvider.ReceiveSLO(
                Request, 
                out var isRequest, 
                out var logoutReason, 
                out var partnerName, 
                out var relayState);

            if (isRequest)
            {
                // Logout locally.
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                // Respond to the IdP-initiated SLO request indicating successful logout.
                SAMLServiceProvider.SendSLO(Response, null);
            }
            else
            {
                // SP-initiated SLO has completed.
                if (!string.IsNullOrEmpty(relayState) && Url.IsLocalUrl(relayState))
                {
                    return Redirect(relayState);
                }

                return RedirectToAction("Index", "Home");
            }

            return new EmptyResult();
        }
    }
}