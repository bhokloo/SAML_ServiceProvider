using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

using ComponentSpace.SAML2.Configuration;
using ComponentSpace.SAML2.Metadata;

namespace ExportMetadata
{
    /// <summary>
    /// Exports the saml.config configuration file as SAML metadata.
    /// 
    /// Usage: ExportMetadata
    /// </summary>
    class Program
    {
        private static SAMLConfiguration LoadSAMLConfiguration()
        {
            Console.Write("SAML configuration file to export [saml.config]: ");
            string fileName = Console.ReadLine();

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "saml.config";
            }

            if (!File.Exists(fileName))
            {
                throw new ArgumentException(string.Format("The configuration file {0} doesn't exist.", fileName));
            }

            SAMLConfigurations samlConfigurations = SAMLConfigurationFile.Load(fileName);

            if (samlConfigurations.Configurations.Count == 1)
            {
                return samlConfigurations.Configurations[0];
            }

            Console.Write("SAML configuration ID: ");
            string configurationID = Console.ReadLine();

            return samlConfigurations.GetConfiguration(configurationID);
        }

        private static X509Certificate2 LoadCertificate(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            if (!File.Exists(fileName))
            {
                throw new ArgumentException(string.Format("The X.509 certificate file {0} doesn't exist.", fileName));
            }

            return new X509Certificate2(fileName);
        }

        private static EntityDescriptor ExportIdentityProviderMetadata(SAMLConfiguration samlConfiguration)
        {
            Console.Write("X.509 certificate .CER file [None]: ");
            string fileName = Console.ReadLine();
            X509Certificate2 x509Certificate = LoadCertificate(fileName);

            Console.Write("Single Sign-On Service URL: ");
            string singleSignOnServiceURL = Console.ReadLine();

            if (string.IsNullOrEmpty(singleSignOnServiceURL))
            {
                throw new ArgumentException("A single sign-on service URL must be specified.");
            }

            Console.Write("Single Logout Service URL [None]: ");
            string singleLogoutServiceURL = Console.ReadLine();

            Console.Write("Partner Service Provider Name [None]: ");
            string partnerName = Console.ReadLine();

            return MetadataExporter.Export(samlConfiguration, x509Certificate, singleSignOnServiceURL, singleLogoutServiceURL, partnerName);
        }

        private static EntityDescriptor ExportServiceProviderMetadata(SAMLConfiguration samlConfiguration)
        {
            Console.Write("X.509 signature certificate .CER file [None]: ");
            string fileName = Console.ReadLine();
            X509Certificate2 signatureCertificate = LoadCertificate(fileName);

            Console.Write("X.509 encryption certificate .CER file [None]: ");
            fileName = Console.ReadLine();
            X509Certificate2 encryptionCertificate = LoadCertificate(fileName);

            Console.Write("Assertion Consumer Service URL: ");
            string assertionConsumerServiceURL = Console.ReadLine();

            if (string.IsNullOrEmpty(assertionConsumerServiceURL))
            {
                throw new ArgumentException("An assertion consumer service URL must be specified.");
            }

            Console.Write("Single Logout Service URL [None]: ");
            string singleLogoutServiceURL = Console.ReadLine();

            Console.Write("Partner Identity Provider Name [None]: ");
            string partnerName = Console.ReadLine();

            return MetadataExporter.Export(samlConfiguration, signatureCertificate, encryptionCertificate, assertionConsumerServiceURL, singleLogoutServiceURL, partnerName);
        }

        private static void SaveMetadata(EntityDescriptor entityDescriptor)
        {
            Console.Write("SAML metadata file [metadata.xml]: ");
            string fileName = Console.ReadLine();

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "metadata.xml";
            }

            using (XmlTextWriter xmlTextWriter = new XmlTextWriter(fileName, null))
            {
                xmlTextWriter.Formatting = Formatting.Indented;
                entityDescriptor.ToXml().OwnerDocument.Save(xmlTextWriter);
            }
        }

        static void Main(string[] args)
        {
            try
            {
                SAMLConfiguration samlConfiguration = LoadSAMLConfiguration();

                EntityDescriptor entityDescriptor = null;

                if (samlConfiguration.LocalIdentityProviderConfiguration != null)
                {
                    entityDescriptor = ExportIdentityProviderMetadata(samlConfiguration);
                }
                else if (samlConfiguration.LocalServiceProviderConfiguration != null)
                {
                    entityDescriptor = ExportServiceProviderMetadata(samlConfiguration);
                }

                SaveMetadata(entityDescriptor);
            }

            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.ToString());
            }
        }
    }
}
