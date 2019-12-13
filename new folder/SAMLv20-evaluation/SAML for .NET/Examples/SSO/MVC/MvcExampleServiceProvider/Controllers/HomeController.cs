using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ComponentSpace.SAML2.Utility;
using MvcExampleServiceProvider.Models;

namespace MvcExampleServiceProvider.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            string licenseType;

            if (License.IsLicensed)
            {
                licenseType = "Licensed";
            }
            else
            {
                licenseType = $"Evaluation (Expires {License.Expires.ToShortDateString()})";
            }

            return View(new AboutViewModel()
            {
                ProductInformation = $"ComponentSpace.Saml2.Net, Version={License.Version}, {licenseType}"
            });
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}