using System;
using System.IO;
using System.Xml;

using ComponentSpace.SAML2.Configuration;
using ComponentSpace.SAML2.Metadata;

namespace ImportMetadata
{
    /// <summary>
    /// Imports SAML metadata into the saml.config configuration file.
    /// 
    /// Usage: ImportMetadata
    /// </summary>
    class Program
    {
        private static string configurationFileName;
        private static string configurationID;

        private static SAMLConfigurations samlConfigurations;

        private static EntityDescriptor LoadMetadata()
        {
            Console.Write("SAML metadata file to import: ");
            string fileName = Console.ReadLine();

            if (!File.Exists(fileName))
            {
                throw new ArgumentException(string.Format("The metadata file {0} doesn't exist.", fileName));
            }

            XmlDocument xmlDocument = new XmlDocument()
            {
                PreserveWhitespace = true
            };

            xmlDocument.Load(fileName);

            return new EntityDescriptor(xmlDocument.DocumentElement);
        }

        private static SAMLConfiguration LoadSAMLConfiguration()
        {
            Console.Write("SAML configuration file [saml.config]: ");
            configurationFileName = Console.ReadLine();

            if (string.IsNullOrEmpty(configurationFileName))
            {
                configurationFileName = "saml.config";
            }

            if (!File.Exists(configurationFileName))
            {
                throw new ArgumentException(string.Format("The configuration file {0} doesn't exist.", configurationFileName));
            }

            samlConfigurations = SAMLConfigurationFile.Load(configurationFileName);

            if (samlConfigurations.Configurations.Count == 1)
            {
                return samlConfigurations.Configurations[0];
            }

            Console.Write("SAML configuration ID: ");
            configurationID = Console.ReadLine();

            return samlConfigurations.GetConfiguration(configurationID);
        }

        private static void SaveSAMLConfiguration(SAMLConfiguration samlConfiguration)
        {
            Console.Write("Save SAML configuration to [{0}]: ", configurationFileName);
            string fileName = Console.ReadLine();

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = configurationFileName;
            }

            XmlDocument xmlDocument = samlConfiguration.ToXml().OwnerDocument;

            using (XmlTextWriter xmlTextWriter = new XmlTextWriter(fileName, null))
            {
                xmlTextWriter.Formatting = Formatting.Indented;
                xmlDocument.Save(xmlTextWriter);
            }
        }

        static void Main(string[] args)
        {
            try
            {
                EntityDescriptor entityDescriptor = LoadMetadata();

                SAMLConfiguration samlConfiguration = LoadSAMLConfiguration();

                Console.Write("Certificate folder for saving certificates [Certificates]: ");
                string certificateFolder = Console.ReadLine();

                if (string.IsNullOrEmpty(certificateFolder))
                {
                    certificateFolder = "Certificates";
                }

                if (samlConfiguration.LocalIdentityProviderConfiguration != null)
                {
                    MetadataImporter.ImportServiceProviders(entityDescriptor, samlConfiguration, certificateFolder);
                }

                if (samlConfiguration.LocalServiceProviderConfiguration != null)
                {
                    MetadataImporter.ImportIdentityProviders(entityDescriptor, samlConfiguration, certificateFolder);
                }

                SaveSAMLConfiguration(samlConfiguration);
            }

            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.ToString());
            }
        }
    }
}
