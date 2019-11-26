using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Security.Claims;
using System.Linq;
using System.Diagnostics;

namespace ClientApplication.Controllers
{
    public class ActivantsSPController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public void ActivantsSP()
        {
            string encryptedReturnUrl = HttpUtility.UrlEncode(Cryptography.Encrypt("https://localhost:44313/ActivantsSP&/getAccessTokenFromSp&/errorPage&/index"));
            Response.Redirect(string.Format("https://localhost:44309/Saml/InitiateSingleSignOn?responseFromSp={0}", encryptedReturnUrl));
        }

        public ActionResult getAccessTokenFromSp()
        {
            if (Request.QueryString["access_token"] != null)
            {
                Session["access_token"] = Request.QueryString["access_token"];
                return RedirectToAction("AuthenticateUserWithAccessToken");
            }
            return RedirectToAction("Index");

        }

        [BearerAuthentication]
        [Authorize]
        public ActionResult AuthenticateUserWithAccessToken()
        {
            return View();
        }

        public ActionResult errorPage()
        {
            return View();
        }
    }
}