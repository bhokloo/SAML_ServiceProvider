using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Xml;

using ComponentSpace.SAML2.Assertions;
using ComponentSpace.SAML2.Protocols;
using ComponentSpace.SAML2.Metadata;
using ComponentSpace.SAML2.Utility;

namespace GenerateSignature
{
    /// <summary>
    /// Signs SAML v2.0 assertions, requests, responses and metadata using XML digital signatures.
    /// 
    /// Usage: GenerateSignature -c <certificateFileName> -p <password> <filename>
    /// 
    /// where the file contains a SAML assertion, request, response or metadata.
    /// 
    /// The signed SAML is written to standard output.
    /// </summary>
    static class Program
    {
        private const int expectedArgCount = 5;

        private static string certificateFileName;
        private static string password;
        private static string fileName;

        private static X509Certificate2 x509Certificate;

        private static void ParseArguments(String[] args)
        {
            if (args.Length < expectedArgCount)
            {
                throw new ArgumentException("Wrong number of arguments.");
            }

            if (args[0] != "-c")
            {
                throw new ArgumentException("Missing certificate file.");
            }

            certificateFileName = args[1];

            if (args[2] != "-p")
            {
                throw new ArgumentException("Missing certificate password.");
            }

            password = args[3];
            fileName = args[4];
        }

        private static void ShowUsage()
        {
            Console.Error.WriteLine("Usage: GenerateSignature -c <certificateFileName> -p <password> <filename>");
        }

        private static void LoadKeyAndCertificate()
        {
            Console.Error.WriteLine("Loading certificate and key from " + certificateFileName);

            if (!File.Exists(certificateFileName))
            {
                throw new ArgumentException("The certificate file " + certificateFileName + " doesn't exist.");
            }

            x509Certificate = new X509Certificate2(certificateFileName, password);
        }

        private static XmlDocument LoadXmlDocument()
        {
            Console.Error.WriteLine("Loading " + fileName);

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = true;
            xmlDocument.Load(fileName);

            return xmlDocument;
        }

        private static void SignAssertion(XmlElement xmlElement)
        {
            Console.Error.WriteLine("Signing SAML assertion");
            SAMLAssertionSignature.Generate(xmlElement, x509Certificate.PrivateKey, x509Certificate);
        }

        private static void SignMessage(XmlElement xmlElement)
        {
            Console.Error.WriteLine("Signing SAML message");
            SAMLMessageSignature.Generate(xmlElement, x509Certificate.PrivateKey, x509Certificate);
        }

        private static void SignMetadata(XmlElement xmlElement)
        {
            Console.Error.WriteLine("Signing SAML metadata");
            SAMLMetadataSignature.Generate(xmlElement, x509Certificate.PrivateKey, x509Certificate);
        }

        static void Main(string[] args)
        {
            try
            {
                ParseArguments(args);
                LoadKeyAndCertificate();

                XmlDocument xmlDocument = LoadXmlDocument();

                switch (xmlDocument.DocumentElement.NamespaceURI)
                {
                    case SAML.NamespaceURIs.Assertion:
                        SignAssertion(xmlDocument.DocumentElement);
                        break;

                    case SAML.NamespaceURIs.Protocol:
                        SignMessage(xmlDocument.DocumentElement);
                        break;

                    case SAML.NamespaceURIs.Metadata:
                        SignMetadata(xmlDocument.DocumentElement);
                        break;

                    default:
                        throw new ArgumentException("Unexpected namespace URI " + xmlDocument.DocumentElement.NamespaceURI);
                }

                Console.WriteLine(xmlDocument.OuterXml);
            }

            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.ToString());

                if (exception is ArgumentException)
                {
                    ShowUsage();
                }
            }
        }
    }
}
