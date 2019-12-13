using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Xml;

using ComponentSpace.SAML2;
using ComponentSpace.SAML2.Assertions;
using ComponentSpace.SAML2.Protocols;
using ComponentSpace.SAML2.Bindings;

namespace ParseHttpRedirectUrl
{
    /// <summary>
    /// Parse the HTTP Redirect URL, checking the signature, and extracting the SAML message and relay states.
    /// 
    /// Usage: ParseHttpRedirectUrl [-c <certificateFileName>] <filename>
    /// 
    /// where the file contains the URL.
    /// 
    /// The SAML message is written to standard output.
    /// </summary>
    static class Program
    {
        private static class QueryNames
        {
            public const string SAMLRequest = "SAMLRequest";
            public const string SAMLResponse = "SAMLResponse";
        }

        private const int expectedArgCount = 1;

        private static string certificateFileName;
        private static string fileName;

        private static X509Certificate2 x509Certificate;

        private static void ParseArguments(String[] args)
        {
            if (args.Length < expectedArgCount)
            {
                throw new ArgumentException("Wrong number of arguments.");
            }

            if (args[0] == "-c")
            {
                certificateFileName = args[1];

                if (args.Length < expectedArgCount + 2)
                {
                    throw new ArgumentException("Wrong number of arguments.");
                }

                fileName = args[2];
            }
            else
            {
                fileName = args[0];
            }
        }

        private static void ShowUsage()
        {
            Console.Error.WriteLine("ParseHttpRedirectUrl [-c <certificateFileName>] <filename>");
        }

        private static void LoadCertificate()
        {
            if (certificateFileName != null)
            {
                Console.Error.WriteLine("Loading certificate " + certificateFileName);

                if (!File.Exists(certificateFileName))
                {
                    throw new ArgumentException("The certificate file " + certificateFileName + " doesn't exist.");
                }

                x509Certificate = new X509Certificate2(certificateFileName);
            }
        }

        private static string DetermineSAMLMessageType(string url)
        {
            if (url.Contains(QueryNames.SAMLRequest + "="))
            {
                return QueryNames.SAMLRequest;
            }
            else if (url.Contains(QueryNames.SAMLResponse + "="))
            {
                return QueryNames.SAMLResponse;
            }
            else
            {
                throw new ArgumentException("The URL doesn't contain a SAMLRequest or SAMLResponse parameter");
            }
        }

        static void Main(string[] args)
        {
            try
            {
                ParseArguments(args);
                LoadCertificate();

                XmlElement samlMessage = null;
                string relayState = null;
                bool signed = false;
                AsymmetricAlgorithm key = null;

                if (x509Certificate != null)
                {
                    key = x509Certificate.PublicKey.Key;
                }

                string url = File.ReadAllText(fileName);

                switch (DetermineSAMLMessageType(url))
                {
                    case QueryNames.SAMLRequest:
                        HTTPRedirectBinding.GetRequestFromRedirectURL(url, out samlMessage, out relayState, out signed, key);
                        break;

                    case QueryNames.SAMLResponse:
                        HTTPRedirectBinding.GetResponseFromRedirectURL(url, out samlMessage, out relayState, out signed, key);
                        break;
                }

                Console.Error.WriteLine("Relay state: " + relayState);
                Console.Error.WriteLine("Signed: " + signed);

                Console.WriteLine(samlMessage.OuterXml);
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
