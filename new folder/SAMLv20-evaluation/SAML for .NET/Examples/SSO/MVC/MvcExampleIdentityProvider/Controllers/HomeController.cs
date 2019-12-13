using ComponentSpace.SAML2.Utility;
using MvcExampleIdentityProvider.Models;
using System.Web.Mvc;

namespace MvcExampleIdentityProvider.Controllers
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