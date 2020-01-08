using ComponentSpace.SAML2.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ActivantsSamlServiceProvider.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
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
            Debug.WriteLine("*************************", licenseType);

            return View();
        }

        public ActionResult error()
        {
            return View();
        }
    }
}
