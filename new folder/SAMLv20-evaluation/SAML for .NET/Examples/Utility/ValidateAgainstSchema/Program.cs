using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

using ComponentSpace.SAML2.Schemas;

namespace ValidateAgainstSchema
{
    /// <summary>
    /// Validates the SAML assertion, protocol or metadata XML 
    /// against the SAML, XML signature and XML encryption schemas.
    /// 
    /// Usage: ValidateAgainstSchema <filename>
    /// 
    /// where the file contains a SAML assertion, request, response or metadata.
    /// </summary>
    static class Program
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
            Console.Error.WriteLine("ValidateAgainstSchema <filename>");
        }

        private static XmlDocument LoadXmlDocument()
        {
            Console.Error.WriteLine("Loading " + fileName);

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = true;
            xmlDocument.Load(fileName);

            return xmlDocument;
        }

        private static void Validate(XmlDocument xmlDocument)
        {
            Console.Error.WriteLine("Validating against XML schemas");

            DateTime startDateTime = DateTime.Now;

            SAMLValidator samlValidator = new SAMLValidator();
            bool validated = samlValidator.Validate(xmlDocument);

            TimeSpan elapsedTimeSpan = DateTime.Now - startDateTime;

            Console.Error.WriteLine("Validated: {0}", validated);

            if (!validated)
            {
                foreach (XmlSchemaException warning in samlValidator.Warnings)
                {
                    Console.WriteLine(warning.Message);
                }

                foreach (XmlSchemaException error in samlValidator.Errors)
                {
                    Console.WriteLine(error.Message);
                }
            }

            Console.Error.WriteLine("Elapsed time: {0} ms", elapsedTimeSpan.TotalMilliseconds);
        }

        static void Main(string[] args)
        {
            try
            {
                ParseArguments(args);

                XmlDocument xmlDocument = LoadXmlDocument();
                Validate(xmlDocument);
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
