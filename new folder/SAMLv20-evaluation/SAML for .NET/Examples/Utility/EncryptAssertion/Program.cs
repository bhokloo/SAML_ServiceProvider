using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.IO;
using System.Xml;

using ComponentSpace.SAML2;
using ComponentSpace.SAML2.Assertions;
using ComponentSpace.SAML2.Protocols;

namespace EncryptAssertion
{
    /// <summary>
    /// Encrypts SAML v2.0 assertions.
    /// 
    /// Usage: EncryptAssertion [-a <algorithm>] -c <certificateFileName> <filename>
    /// 
    /// where the file contains a SAML assertion
    /// and the algorithm defaults to "http://www.w3.org/2001/04/xmlenc#aes256-cbc".
    /// 
    /// The encrypted SAML assertion is written to standard output.
    /// 
    /// See System.Security.Cryptography.Xml.EncryptedXml for algorithm types.
    /// </summary>
    static class Program
    {
        private const int expectedArgCount = 3;

        private static string algorithm = EncryptedXml.XmlEncAES256Url;
        private static string certificateFileName;
        private static string fileName;

        private static X509Certificate2 x509Certificate;

        private static void ParseArguments(String[] args)
        {
            if (args.Length < expectedArgCount)
            {
                throw new ArgumentException("Wrong number of arguments.");
            }

            int index = 0;

            if (args.Length >= expectedArgCount + 2)
            {
                if (args[index++] != "-a")
                {
                    throw new ArgumentException("Missing algorithm.");
                }

                algorithm = args[index++];
            }

            if (args[index++] != "-c")
            {
                throw new ArgumentException("Missing certificate.");
            }

            certificateFileName = args[index++];

            fileName = args[index++];
        }

        private static void ShowUsage()
        {
            Console.Error.WriteLine("EncryptAssertion [-a <algorithm>] -c <certificateFileName> <filename>");
        }

        private static void LoadCertificate()
        {
            Console.Error.WriteLine("Loading certificate " + certificateFileName);

            if (!File.Exists(certificateFileName))
            {
                throw new ArgumentException("The certificate file " + certificateFileName + " doesn't exist.");
            }

            x509Certificate = new X509Certificate2(certificateFileName);
        }

        private static XmlDocument LoadXmlDocument()
        {
            Console.Error.WriteLine("Loading " + fileName);

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = true;
            xmlDocument.Load(fileName);

            return xmlDocument;
        }

        private static XmlElement EncryptAssertion(XmlElement xmlElement)
        {
            Console.Error.WriteLine("Encrypting SAML assertion");

            return new EncryptedAssertion(xmlElement, x509Certificate, new EncryptionMethod(algorithm)).ToXml();
        }

        static void Main(string[] args)
        {
            try
            {
                ParseArguments(args);
                LoadCertificate();

                XmlDocument xmlDocument = LoadXmlDocument();

                Console.WriteLine(EncryptAssertion(xmlDocument.DocumentElement).OuterXml);
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
