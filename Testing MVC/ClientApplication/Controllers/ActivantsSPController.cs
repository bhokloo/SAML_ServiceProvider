using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System.Configuration;

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
            var samlConfigurationId = ConfigurationManager.AppSettings["samlConfigurationId"];
            Response.Redirect("https://localhost:44309/Saml/InitiateSingleSignOn?returnClass=ActivantsSP&returnFunction=getAccessTokenFromSp&returnError=errorPage&samlConfigurationId=" + samlConfigurationId);
        }

        public ActionResult getAccessTokenFromSp()
        {
            if (Request.QueryString["access_token"] != null)
            {
                 var access_token = Request.QueryString["access_token"];
                Session["access_token"] = access_token;
                return RedirectToAction("AuthenticateUserWithAccessToken");
            }
            return RedirectToAction("Index");
        }

        [BearerAuthentication]
        [Authorize]
        public ActionResult AuthenticateUserWithAccessToken()
        {
            var ff = Session["indrajit"];
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