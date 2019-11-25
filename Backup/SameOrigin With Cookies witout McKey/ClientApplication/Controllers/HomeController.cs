using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using ClientApplication.Models;
using System.Xml;
using System.Collections.Specialized;
using Microsoft.AspNet.Identity;
using System.Net;
using System.Web.Security;
using System.Text;

namespace ClientApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public void About()
        {
            HttpCookie relayStateForSP = new HttpCookie("relayStateForSP");
            HttpCookie errorPageForClient = new HttpCookie("errorPageForClient");
            relayStateForSP.Value = Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes("https://localhost:44313/Home/ResponseFromSp")));
            errorPageForClient.Value = Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes("https://localhost:44313/Home/errorPage")));
            Response.Cookies.Add(relayStateForSP);
            Response.Cookies.Add(errorPageForClient);
            Response.Redirect("https://localhost:44307/saml/InitiateSingleSignOn");
        }

        [Authorize]
        public ActionResult ResponseFromSp()
        {
            IDictionary<string, string> attributes = (IDictionary<string, string>)Session["attributes"];
            var userClaims = HttpContext.GetOwinContext().Authentication.User.Claims;
            return View();
        }
        public ActionResult errorPage()
        {
            return View();
        }
    }
}







//ExternalLoginListViewModel model = new ExternalLoginListViewModel();
//return RedirectToAction("ExternalLogin", "Account", new { provider = "https://localhost:44307/Saml/InitiateSingleSignOn"});