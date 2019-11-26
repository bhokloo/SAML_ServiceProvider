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
        public ActionResult index()
        {
            return View();
        }

        public void ActivantsSP()
        {
            Response.Redirect("https://localhost:44309/Saml/InitiateSingleSignOn?returnClass=ActivantsSP&returnFunction=getAccessTokenFromSp&returnError=errorPage");
        }

        public ActionResult getAccessTokenFromSp()
        {
            if (Request.QueryString["access_token"] != null)
            {
                Session["access_token"] = Request.QueryString["access_token"];
                Session["username"] = Request.QueryString["username"];
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

        [Authorize]
        public ActionResult sd()
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("AuthenticateUserWithAccessToken");
            }
            return null;
        }

        public ActionResult errorPage()
        {
            return View();
        }
    }
}