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
    public class ActivantsSPController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        //Passing QueryString with parameters
        public void ActivantsSP()
        {
            //with encryptio
            string responseFromSp = HttpUtility.UrlEncode(Cryptography.Encrypt("https://localhost:44313/ActivantsSP&/responseFromSp&/errorPage&/index"));
            //without encrytion
            //var responseFromSp = "https://localhost:44313/ActivantsSP&/responseFromSp&/errorPage&/index";
            Response.Redirect(string.Format("https://192.168.1.172:44300/Saml/InitiateSingleSignOn?responseFromSp={0}", responseFromSp));
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