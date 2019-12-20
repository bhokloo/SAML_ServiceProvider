using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace SAML2SP
{
    public class Global : System.Web.HttpApplication
    {
        private const string spCertificateFileName = "sp.pfx";
        private const string spPassword = "password";
        private const string idpCertificateFileName = "idp.cer";
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

        protected void Application_Start(object sender, EventArgs e)
        {
            string fileName = Path.Combine(HttpRuntime.AppDomainAppPath, spCertificateFileName);
            Application[SPX509Certificate] = LoadCertificate(fileName, spPassword);
            fileName = Path.Combine(HttpRuntime.AppDomainAppPath, idpCertificateFileName);
            Application[IdPX509Certificate] = LoadCertificate(fileName, null);
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}