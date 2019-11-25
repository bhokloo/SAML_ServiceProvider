using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Text;
using System.Security.Cryptography;
using System.IO;

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
            string relayState = HttpUtility.UrlEncode(Cryptography.Encrypt("https://localhost:44313/Home/ResponseFromSp"));
            string errPage = HttpUtility.UrlEncode(Cryptography.Encrypt("https://localhost:44313/Home/errorPage"));
            Response.Redirect(string.Format("https://localhost:44308/saml/InitiateSingleSignOn?relayState={0}&errPage={1}", relayState, errPage));
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