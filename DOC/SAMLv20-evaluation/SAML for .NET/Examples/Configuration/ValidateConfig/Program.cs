using System;
using System.Xml.Schema;

using ComponentSpace.SAML2.Configuration;
using ComponentSpace.SAML2.Exceptions;

namespace ValidateConfig
{
    /// <summary>
    /// Validates the SAML configuration against its schema. 
    /// 
    /// Usage: ValidateConfig <filename>
    /// 
    /// where the file contains the SAML configuration.
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
            Console.Error.WriteLine("ValidateConfig <saml.config>");
        }

        static void Main(string[] args)
        {
            try
            {
                ParseArguments(args);

                Console.Error.WriteLine("Validating {0}.", fileName);
                SAMLConfigurationFile.Validate(fileName);
                Console.Error.WriteLine("The SAML configuration was successfully validated.");
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
                    ShowUsage();
                }
            }
        }
    }
}
