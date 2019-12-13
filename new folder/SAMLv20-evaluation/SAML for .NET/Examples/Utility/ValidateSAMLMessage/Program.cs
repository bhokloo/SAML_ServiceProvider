using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;

using ComponentSpace.SAML2;
using ComponentSpace.SAML2.Configuration;
using ComponentSpace.SAML2.Protocols;
using ComponentSpace.SAML2.Utility;

namespace ValidateSAMLMessage
{
    /// <summary>
    /// Validates the SAML message. The level of validation is controlled by the SAML configuration flags.
    /// 
    /// Usage: ValidateSAMLMessage <filename>
    /// 
    /// where the file contains the SAML message.
    /// 
    /// The SAML message may either be XML or a base-64 encoded string.
    /// </summary>
    class Program
    {
        private const int expectedArgCount = 1;

        private static string fileName;

        private static void ParseArguments(String[] args)
        {
            if (args.Length < expectedArgCount)
            {
                throw new ArgumentException("Wrong number of arguments.");
            }

            fileName = args[0];
        }

        private static void ShowUsage()
        {
            Console.Error.WriteLine("ValidateSAMLMessage <samlmessage.xml | samlmessage.base64>");
        }

        private static string DecodeString(string text)
        {
            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(text));
            }

            catch (Exception)
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(HttpUtility.UrlDecode(text)));
            }
        }

        private static XmlElement GetSAMLMessage()
        {
            if (!File.Exists(fileName))
            {
                throw new ArgumentException("The SAML message file " + fileName + " doesn't exist.");
            }

            Console.WriteLine("Reading {0}.", fileName);

            string text = File.ReadAllText(fileName);

            if (!text.Contains("<"))
            {
                text = DecodeString(text);
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = true;
            xmlDocument.LoadXml(text);

            Console.WriteLine("SAML message: {0}", xmlDocument.DocumentElement.OuterXml);

            return xmlDocument.DocumentElement;
        }

        private static bool IsMessageFromIdentityProvider(string issuerName)
        {
            return SAMLController.Configuration.LocalIdentityProviderConfiguration != null &&
                   SAMLController.Configuration.LocalIdentityProviderConfiguration.Name == issuerName;
        }

        private static void ReceiveAuthnRequest(XmlElement xmlElement)
        {
            SAML.HttpContext = new SAMLHttpContext();
            SAMLHttpRequest samlHttpRequest = new SAMLHttpRequest(xmlElement, null, null, null);

            string partnerSP = null;

            SAMLIdentityProvider.ReceiveSSO(samlHttpRequest, out partnerSP);

            Console.WriteLine("Partner SP: {0}", partnerSP);
        }

        private static void ReceiveSAMLResponse(XmlElement xmlElement)
        {
            SAML.HttpContext = new SAMLHttpContext();
            SAMLHttpRequest samlHttpRequest = new SAMLHttpRequest(xmlElement, null, null, null);

            bool isInResponseTo = false;
            string partnerIdP = null;
            string authnContext = null;
            string userName = null;
            IDictionary<string, string> attributes = null;
            string targetUrl = null;

            SAMLServiceProvider.ReceiveSSO(samlHttpRequest, out isInResponseTo, out partnerIdP, out authnContext, out userName, out attributes, out targetUrl);

            Console.WriteLine("SP-Initiated SSO: {0}", isInResponseTo);
            Console.WriteLine("Partner IdP: {0}", partnerIdP);
            Console.WriteLine("User name: {0}", userName);

            if (attributes != null)
            {
                foreach (string attributeName in attributes.Keys)
                {
                    Console.WriteLine("{0}: {1}", attributeName, attributes[attributeName]);
                }
            }

            Console.WriteLine("Target URL: {0}", targetUrl);
        }

        private static void ReceiveLogoutMessageFromIdentityProvider(XmlElement xmlElement)
        {
            SAML.HttpContext = new SAMLHttpContext();
            SAMLHttpRequest samlHttpRequest = new SAMLHttpRequest(xmlElement, null, null, null);

            bool isRequest = false;
            string logoutReason = null;
            string partnerSP = null;
            string relayState = null;

            SAMLServiceProvider.ReceiveSLO(samlHttpRequest, out isRequest, out logoutReason, out partnerSP, out relayState);

            Console.WriteLine("Logout request: {0}", isRequest);
            Console.WriteLine("Logout reason: {0}", logoutReason);
            Console.WriteLine("Partner SP: {0}", partnerSP);
        }

        private static void ReceiveLogoutMessageFromServiceProvider(XmlElement xmlElement)
        {
            SAML.HttpContext = new SAMLHttpContext();
            SAMLHttpRequest samlHttpRequest = new SAMLHttpRequest(xmlElement, null, null, null);
            SAMLHttpResponse samlHttpResponse = new SAMLHttpResponse();

            bool isRequest = false;
            bool hasCompleted = false;
            string logoutReason = null;
            string partnerSP = null;
            string relayState = null;

            SAMLIdentityProvider.ReceiveSLO(samlHttpRequest, samlHttpResponse, out isRequest, out hasCompleted, out logoutReason, out partnerSP, out relayState);

            Console.WriteLine("Logout request: {0}", isRequest);
            Console.WriteLine("Logout completed: {0}", hasCompleted);
            Console.WriteLine("Logout reason: {0}", logoutReason);
            Console.WriteLine("Partner SP: {0}", partnerSP);
        }

        private static void ReceiveLogoutMessage(XmlElement xmlElement, SAMLIdentifiers.MessageType messageType)
        {
            string issuername = null;

            switch (messageType)
            {
                case SAMLIdentifiers.MessageType.LogoutRequest:
                    issuername = LogoutRequest.GetIssuerName(xmlElement);
                    break;

                case SAMLIdentifiers.MessageType.LogoutResponse:
                    issuername = LogoutResponse.GetIssuerName(xmlElement);
                    break;
            }

            if (IsMessageFromIdentityProvider(issuername))
            {
                ReceiveLogoutMessageFromIdentityProvider(xmlElement);
            }
            else
            {
                ReceiveLogoutMessageFromServiceProvider(xmlElement);
            }
        }

        static void Main(string[] args)
        {
            try
            {
                ParseArguments(args);

                SAMLController.Initialize();

                XmlElement xmlElement = GetSAMLMessage();
                SAMLIdentifiers.MessageType messageType = SAML.GetSAMLMessageType(xmlElement);
                Console.WriteLine("The SAML message type is: {0}", messageType);

                switch (messageType)
                {
                    case SAMLIdentifiers.MessageType.AuthnRequest:
                        ReceiveAuthnRequest(xmlElement);
                        break;

                    case SAMLIdentifiers.MessageType.SAMLResponse:
                        ReceiveSAMLResponse(xmlElement);
                        break;

                    case SAMLIdentifiers.MessageType.LogoutRequest:
                        ReceiveLogoutMessage(xmlElement, messageType);
                        break;

                    case SAMLIdentifiers.MessageType.LogoutResponse:
                        ReceiveLogoutMessage(xmlElement, messageType);
                        break;
                }
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
