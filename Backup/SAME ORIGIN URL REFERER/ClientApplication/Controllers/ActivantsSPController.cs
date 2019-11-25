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

        public void ActivantsSp()
        {
            Response.Redirect("https://localhost:44308/saml/InitiateSingleSignOn?responseFromSp=responseFromSp&errorPage=errorPage");
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