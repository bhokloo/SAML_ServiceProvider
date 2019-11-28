using ComponentSpace.SAML2.Configuration;
using ComponentSpace.SAML2.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Schema;

namespace ActivantsSP.Controllers
{
    public class SamlSpConfigurationController : Controller
    {
        public void ServiceProviderconfiguration(string serviceId)
        {
            try
            {
                //var samlConfiguration = new SAMLConfiguration();
                //var SAMLConfiguration = (NameValueCollection)WebConfigurationManager.GetSection(serviceId);
                var samlPath = Server.MapPath("~/saml.config");
                SAMLConfigurationFile.Validate(samlPath);
                //samlConfiguration.ToXml().OwnerDocument.Load("saml.config");
                //samlConfiguration.LocalServiceProviderConfiguration = CreateServiceProviderConfiguration(serviceId);
                //SaveConfiguration(samlConfiguration,serviceId);
            }
            catch (SAMLSchemaValidationException exception)
            {
                Console.Error.WriteLine(exception.Message);

                foreach (XmlSchemaException error in exception.Errors)
                {
                    Console.Error.WriteLine("Line {0}, Column {1}: {2}", error.LineNumber, error.LinePosition, error.Message);
                }

                foreach (XmlSchemaException warning in exception.Warnings)
                {
                    Console.Error.WriteLine("Line {0}, Column {1}: {2}", warning.LineNumber, warning.LinePosition, warning.Message);
                }
            }

            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.ToString());

                if (exception is ArgumentException)
                {
                    //ShowUsage();
                }
            }
        }

        private static LocalServiceProviderConfiguration CreateServiceProviderConfiguration(string serviceId)
        {
            var SAMLConfiguration = (NameValueCollection)WebConfigurationManager.GetSection(serviceId);

            var localServiceProviderConfiguration = new LocalServiceProviderConfiguration();
            var partnerIdentityProviderConfiguration = new PartnerIdentityProviderConfiguration();

            //SERVICE PROVIDER
            localServiceProviderConfiguration.Name = SAMLConfiguration["serviceIdName"];
            localServiceProviderConfiguration.AssertionConsumerServiceUrl = SAMLConfiguration["AssertionConsumerServiceUrl"];
            localServiceProviderConfiguration.Description = SAMLConfiguration["Description"];
            localServiceProviderConfiguration.SingleLogoutServiceUrl = SAMLConfiguration["SingleLogoutServiceUrl"];
            localServiceProviderConfiguration.LocalCertificateFile = SAMLConfiguration["LocalCertificateFile"];
            localServiceProviderConfiguration.LocalCertificatePassword = SAMLConfiguration["LocalCertificatePassword"];

            //IDENTITY PROVIDER
            partnerIdentityProviderConfiguration.Name = SAMLConfiguration["PartnerIdP"];
            partnerIdentityProviderConfiguration.Description = SAMLConfiguration["Description"];
            partnerIdentityProviderConfiguration.SignAuthnRequest = bool.Parse(SAMLConfiguration["SignAuthnRequest"]);
            partnerIdentityProviderConfiguration.WantSAMLResponseSigned = bool.Parse(SAMLConfiguration["WantSAMLResponseSigned"]);
            partnerIdentityProviderConfiguration.WantAssertionSigned = bool.Parse(SAMLConfiguration["WantAssertionSigned"]);
            partnerIdentityProviderConfiguration.SignLogoutRequest = bool.Parse(SAMLConfiguration["SignLogoutRequest"]);
            partnerIdentityProviderConfiguration.WantLogoutResponseSigned = bool.Parse(SAMLConfiguration["WantLogoutResponseSigned"]);
            partnerIdentityProviderConfiguration.SingleSignOnServiceBinding = SAMLConfiguration["SingleSignOnServiceBinding"];
            partnerIdentityProviderConfiguration.SingleLogoutServiceBinding = SAMLConfiguration["SingleLogoutServiceBinding"];
            partnerIdentityProviderConfiguration.SignatureMethod = SAMLConfiguration["SignatureMethod"];
            partnerIdentityProviderConfiguration.SingleSignOnServiceUrl = SAMLConfiguration["SingleSignOnServiceUrl"];
            partnerIdentityProviderConfiguration.SingleLogoutServiceUrl = SAMLConfiguration["SingleLogoutServiceUrl"];
            partnerIdentityProviderConfiguration.PartnerCertificateFile = SAMLConfiguration["PartnerCertificateFile"];

            return localServiceProviderConfiguration;
        }

        private static void SaveConfiguration(SAMLConfiguration samlConfiguration, string serviceConfig)
        {
            
            var fileName = "saml.config";
            using (XmlTextWriter xmlTextWriter = new XmlTextWriter(fileName, null))
            {
                xmlTextWriter.Formatting = Formatting.Indented;
                samlConfiguration.ToXml().OwnerDocument.Save(xmlTextWriter);
            }
        }
    }
}

//<? xml version="1.0"?>
//<SAMLConfiguration>

//  <add key = "serviceIdName" value="123456789"/>
//  <add key = "Description" value="Activants Service Provider"/>
//  <add key = "AssertionConsumerServiceUrl" value="https://localhost:44309/SAML/AssertionConsumerService"/>
//  <add key = "SingleLogoutServiceUrl" value="https://localhost:44309/SAML/SingleLogoutService"/>
//  <add key = "LocalCertificateFile" value="Certificates\sp.pfx"/>
//  <add key = "LocalCertificatePassword" value="activants"/>


//  <add key = "PartnerIdP" value="https://login.xecurify.com/moas/198052/8ab7a98c-0cce-11ea-a703-02c931e36dd8"/>
//  <add key = "Description=" value="Activants IDP"/>
//  <add key = "SignAuthnRequest" value="true"/>
//  <add key = "WantSAMLResponseSigned" value="true"/>
//  <add key = "WantAssertionSigned" value="true"/>
//  <add key = "SignLogoutRequest" value="true"/>
//  <add key = "WantLogoutResponseSigned" value="false"/>
//  <add key = "SingleSignOnServiceBinding" value="urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect"/>
//  <add key = "SingleLogoutServiceBinding" value="urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect"/>
//  <add key = "SignatureMethod" value="http://www.w3.org/2001/04/xmldsig-more#rsa-sha256"/>
//  <add key = "SingleSignOnServiceUrl" value="https://login.xecurify.com/moas/idp/samlsso/8ab7a98c-0cce-11ea-a703-02c931e36dd8"/>
//  <add key = "SingleLogoutServiceUrl" value="https://login.xecurify.com/moas/idp/samllogout/8ab7a98c-0cce-11ea-a703-02c931e36dd8?cid=198052"/>
//  <add key = "PartnerCertificateFile" value="Certificates\idp.cer"/>
  
//</SAMLConfiguration>