using ActivantsSP.Models;
using ComponentSpace.SAML2.Utility;
using System.Diagnostics;
using System.Web.Mvc;


namespace ActivantsSP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult About()
        {
            //8th
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

            return View(new AboutViewModel()
            {
                ProductInformation = $"ComponentSpace.Saml2.Net, Version={License.Version}, {licenseType}"
            });


        }
    }
}