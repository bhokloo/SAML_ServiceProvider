using System;
using System.IO;
using System.Xml;

using ComponentSpace.SAML2;
using ComponentSpace.SAML2.Metadata;

namespace VerifySAML
{
    /// <summary>
    /// Reads the SAML v2.0 metadata.
    /// 
    /// Usage: ReadMetadata <filename>
    /// 
    /// where the file contains a SAML entities descriptor or entity descriptor.
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
            Console.Error.WriteLine("ReadMetadata <filename>");
        }

        private static XmlDocument LoadXmlDocument()
        {
            Console.Error.WriteLine("Loading " + fileName);

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = true;
            xmlDocument.Load(fileName);

            return xmlDocument;
        }

        private static void ReadMetadata(XmlElement xmlElement)
        {
            if (EntitiesDescriptor.IsValid(xmlElement))
            {
                Console.Error.WriteLine("Reading SAML entities descriptor metadata");
                EntitiesDescriptor entitiesDescriptor = new EntitiesDescriptor(xmlElement);
                Console.WriteLine(entitiesDescriptor.ToXml().OuterXml);
            }
            else if (EntityDescriptor.IsValid(xmlElement))
            {
                Console.Error.WriteLine("Reading SAML entity descriptor metadata");
                EntityDescriptor entityDescriptor = new EntityDescriptor(xmlElement);
                Console.WriteLine(entityDescriptor.ToXml().OuterXml);
            }
            else
            {
                throw new ArgumentException("Expecting entities descriptor or entity descriptor");
            }
        }

        static void Main(string[] args)
        {
            try
            {
                ParseArguments(args);

                XmlDocument xmlDocument = LoadXmlDocument();

                ReadMetadata(xmlDocument.DocumentElement);
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
