using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ActivantsSamlServiceProvider
{
    public class WebApiApplication : System.Web.HttpApplication
    {

        private const string spCertificateFileName = "Certificates/sp.pfx";
        private const string spPassword = "password";
        private const string idpCertificateFileName = "Certificates/idp.cer";
        public const string SPX509Certificate = "spX509Certificate";
        public const string IdPX509Certificate = "idpX509Certificate";

        private X509Certificate2 LoadCertificate(string fileName, string password)
        {
            X509Certificate2 functionReturnValue = default(X509Certificate2);

            if (!File.Exists(fileName))
            {
                throw new ArgumentException("The certificate file " + fileName + " doesn't exist.");
            }

            try
            {
                functionReturnValue = new X509Certificate2(fileName, password, X509KeyStorageFlags.MachineKeySet);
            }

            catch (Exception exception)
            {
                throw new ArgumentException("The certificate file " + fileName + " couldn't be loaded - " + exception.Message);
            }

            return functionReturnValue;
        }



        protected void Application_Start()
        {
            //SP Certificate
            string fileName = Path.Combine(HttpRuntime.AppDomainAppPath, spCertificateFileName);
            Application[SPX509Certificate] = LoadCertificate(fileName, spPassword);

            //IdP certificate
            fileName = Path.Combine(HttpRuntime.AppDomainAppPath, idpCertificateFileName);
            Application[IdPX509Certificate] = LoadCertificate(fileName, null);

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        void Application_BeginRequest(object sender, EventArgs e)
        {
            if (ReferenceEquals(null, HttpContext.Current.Request.Headers["Authorization"]))
            {
                var token = HttpContext.Current.Request.Params["access_token"];
                if (!String.IsNullOrEmpty(token))
                {
                    HttpContext.Current.Request.Headers.Add("Authorization", "Bearer " + token);
                }
            }
        }

    }
}
