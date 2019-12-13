using ActivantsSamlServiceProvider.Utility;
using ComponentSpace.SAML2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ActivantsSamlServiceProvider.Controllers
{
    public class SamlServicesController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AssertionConsumerService()
        {
            string relayState = null;
            if (Response != null)
            {
                try
                {
                   SAMLServiceProvider.ReceiveSSO(
                   Request,
                   out var isInResponseTo,
                   out var partnerName,
                   out var authnContext,
                   out var userName,
                   out IDictionary<string, string> attributes,
                   out relayState);

                    var accessToken = "";
                    if (Request.Cookies["SAML_SessionId"] != null)
                    {
                        accessToken = Request.Cookies["SAML_SessionId"].Value;
                    }

                    var clientAccessToken = accessToken + Guid.NewGuid().ToString();

                    if (!string.IsNullOrEmpty(relayState))
                    {
                        var clientQueryParameters = relayState.Replace("[", "").Replace("]", "").Replace(" ", "").Split(';');
                        var returnURL = clientQueryParameters[0].Split(',');
                        var returnClass = clientQueryParameters[1].Split(',');
                        var returnFunction = clientQueryParameters[2].Split(',');
                        var returnError = clientQueryParameters[3].Split(',');

                        if (SamlAuthorizedDomains.IsAutorizedUrl(returnURL[1]))
                        {
                            return Redirect(returnURL[1] + "/" + returnClass[1] + "/" + returnFunction[1] + "?access_token=" + clientAccessToken);
                        }
                        else
                        {
                            return Redirect(returnURL[1] + "/" + returnClass[1] + "/" + returnError[1]);
                        }
                    }
                    Session["attr"] = attributes;
                    return RedirectToAction("About", "Home");
                }
                catch (Exception e)
                {
                    if (!string.IsNullOrEmpty(relayState))
                    {
                        var clientQueryParameters = relayState.Replace("[", "").Replace("]", "").Replace(" ", "").Split(';');
                        var returnURL = clientQueryParameters[0].Split(',');
                        var returnClass = clientQueryParameters[1].Split(',');
                        var returnError = clientQueryParameters[3].Split(',');

                        return Redirect(returnURL[1] + "/" + returnClass[1] + "/" + returnError[1]);
                    }
                    else
                    {
                        TempData["err"] = e;
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult SingleLogoutService()
        {
            string relayState = null;
            try
            {
                bool isRequest = false;
                string logoutReason = null;
                string partnerIdP = null;
                SAMLServiceProvider.ReceiveSLO(Request, out isRequest, out logoutReason, out partnerIdP, out relayState);
                if (isRequest)
                {
                    SAMLServiceProvider.SendSLO(Response, null);
                }
                else
                {
                    if (!string.IsNullOrEmpty(relayState))
                    {
                        var clientQueryParameters = relayState.Replace("[", "").Replace("]", "").Replace(" ", "").Split(';');
                        var returnURL = clientQueryParameters[0].Split(',');
                        var returnClass = clientQueryParameters[1].Split(',');
                        var returnFunction = clientQueryParameters[2].Split(',');
                        var returnError = clientQueryParameters[3].Split(',');
                        if (SamlAuthorizedDomains.IsAutorizedUrl(returnURL[1]))
                        {
                            return Redirect(returnURL[1] + "/" + returnClass[1] + "/" + returnFunction[1]);
                        }
                        else
                        {
                            return Redirect(returnURL[1] + "/" + returnClass[1] + "/" + returnError[1]);
                        }
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                if (!string.IsNullOrEmpty(relayState))
                {
                    var clientQueryParameters = relayState.Replace("[", "").Replace("]", "").Replace(" ", "").Split(';');
                    var returnURL = clientQueryParameters[0].Split(',');
                    var returnClass = clientQueryParameters[1].Split(',');
                    var returnError = clientQueryParameters[3].Split(',');

                    return Redirect(returnURL[1] + "/" + returnClass[1] + "/" + returnError[1]);
                }
                else
                {
                    TempData["err"] = e;
                    return RedirectToAction("Index", "Home");
                }
            }

        }
    }
}