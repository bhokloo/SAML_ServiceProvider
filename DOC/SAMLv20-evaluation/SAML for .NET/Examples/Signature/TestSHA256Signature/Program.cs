/*
 * This example demonstrates XML signature generation and verification using SHA-1, SHA-256, SHA-384 and SHA-512 digest and signature algorithms.
 * SHA-256, SHA-384 and SHA-512 XML signature support requires the .NET framework v4.0 or above.
 * There are no external dependencies.
 * 
 * SHA-256, SHA-384 and SHA-512 XML signatures support also requires the use of an X.509 certificate that supports these algorithms.
 * Specifically, the private key must specify the Microsoft Enhanced RSA and AES Cryptographic Provider (ie CSP type 24).
 * If it doesn't, openssl may be used to specify the correct CSP.
 * For more information, refer to the following forum topics.
 * 
 * http://componentspace.com/Forums/1565/SHA256-and-Cryptographic-Service-Provider-Types
 * http://componentspace.com/Forums/1578/SHA256-and-Converting-the-Cryptographic-Service-Provider-Type
 * 
 * The supplied test certificates support these algorithms.
 * 
 * Usage: TestSHA256Signature <certificate-filename> <certificate-password>
 *
 * where the certificate file is a PFX (it must include a private key)
 * and the password protects the certificate file.
 */
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

using ComponentSpace.SAML2;
using ComponentSpace.SAML2.Bindings;
using ComponentSpace.SAML2.Assertions;
using ComponentSpace.SAML2.Protocols;

namespace TestSHA256Signature
{
    class Program
    {
        private static string certificateFileName;
        private static string password;

        private static void ParseArguments(String[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("Wrong number of arguments.");
            }

            certificateFileName = args[0];
            password = args[1];
        }

        private static void ShowUsage()
        {
            Console.Error.WriteLine("TestSHA256Signature <certificate-filename> <certificate-password>");
            Console.Error.WriteLine("For example: SHA256Signature idp.pfx password");
        }

        private static void DisplayCertificateInformation(X509Certificate2 x509Certificate)
        {
            Console.WriteLine("Testing SHA-256 signature support using the certificate issued to \"{0}\".", x509Certificate.Subject);

            if (!x509Certificate.HasPrivateKey)
            {
                Console.WriteLine("A private key is required to generate signatures.");
            }
            else
            {
                if (x509Certificate.PrivateKey is RSACryptoServiceProvider)
                {
                    RSACryptoServiceProvider rsaCryptoServiceProvider = (RSACryptoServiceProvider)x509Certificate.PrivateKey;

                    if (rsaCryptoServiceProvider.CspKeyContainerInfo.ProviderType == SAMLIdentifiers.CryptographicServiceProviderTypes.PROV_RSA_AES)
                    {
                        Console.WriteLine("The private key's associated cryptographic service provider supports SHA-256 signatures.");
                    }
                    else
                    {
                        Console.WriteLine("The private key's associated cryptographic service provider, \"{0}\", doesn't support SHA-256 signatures.", rsaCryptoServiceProvider.CspKeyContainerInfo.ProviderName);
                        Console.WriteLine(x509Certificate.ToString(true));
                    }
                }
            }
        }

        private static void SignAndVerify(X509Certificate2 x509Certificate, string digestMethod, string signatureMethod)
        {
            try
            {
                Console.WriteLine("Testing signature generation and verification using \"{0}\".", signatureMethod);

                // Create a basic SAML assertion and serialize it to XML.
                SAMLAssertion samlAssertion = new SAMLAssertion();
                samlAssertion.Issuer = new Issuer("test");
                XmlElement samlAssertionElement = samlAssertion.ToXml();

                // Sign the SAML assertion using the specified digest and signature methods.
                SAMLAssertionSignature.Generate(samlAssertionElement, x509Certificate.PrivateKey, x509Certificate, null, digestMethod, signatureMethod);

                // Verify the signature.
                bool verified = SAMLAssertionSignature.Verify(samlAssertionElement);

                if (!verified)
                {
                    throw new Exception("The XML signature failed to verify.");
                }

                // The HTTP-redirect doesn't use XML signatures so check it separately.
                // Create a basic authn request and serialize it to XML.
                AuthnRequest authnRequest = new AuthnRequest();
                authnRequest.Issuer = new Issuer("test");
                XmlElement authnRequestElement = authnRequest.ToXml();

                // Create the HTTP-redirect URL included the signature.
                string url = HTTPRedirectBinding.CreateRequestRedirectURL("http://www.test.com", authnRequestElement, null, x509Certificate.PrivateKey, signatureMethod);

                string relayState = null;
                bool signed = false;

                // Retrieve the authn request from the HTTP-redirect URL and verify the signature.
                HTTPRedirectBinding.GetRequestFromRedirectURL(url, out authnRequestElement, out relayState, out signed, x509Certificate.PublicKey.Key);

                Console.WriteLine("Signature generation and verification using \"{0}\" was successful.", signatureMethod);
            }

            catch (Exception exception)
            {
                Console.WriteLine("Signature generation and verification using \"{0}\" failed.", signatureMethod);
                Console.WriteLine(exception.ToString());
            }
        }

        static void Main(string[] args)
        {
            try
            {
                ParseArguments(args);

                // Load the certificate and private key for signature generation.
                X509Certificate2 x509Certificate = new X509Certificate2(certificateFileName, password);

                DisplayCertificateInformation(x509Certificate);

                // Sign and verify using SHA-1, SHA-256, SHA-384 and SHA-512.
                SignAndVerify(x509Certificate, SAMLIdentifiers.DigestMethods.SHA1, SAMLIdentifiers.SignatureMethods.RSA_SHA1);
                SignAndVerify(x509Certificate, SAMLIdentifiers.DigestMethods.SHA256, SAMLIdentifiers.SignatureMethods.RSA_SHA256);
                SignAndVerify(x509Certificate, SAMLIdentifiers.DigestMethods.SHA384, SAMLIdentifiers.SignatureMethods.RSA_SHA384);
                SignAndVerify(x509Certificate, SAMLIdentifiers.DigestMethods.SHA512, SAMLIdentifiers.SignatureMethods.RSA_SHA512);
            }

            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());

                if (exception is ArgumentException)
                {
                    ShowUsage();
                }
            }
        }
    }
}
