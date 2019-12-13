/**
 * This example demonstrates reading and writing SAML metadata including the following:
 * 
 * 1. Creating an IdP entity descriptor.
 * 2. Creating an SP entity descriptor.
 * 3. Reading an IdP entity descriptor.
 * 4. Reading an SP entity descriptor.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

using ComponentSpace.SAML2;
using ComponentSpace.SAML2.Metadata;
using ComponentSpace.SAML2.Utility;

namespace MetadataExample
{
    class Program
    {
        private const string idpPrivateKeyFileName = "idp.pfx";
        private const string spPrivateKeyFileName = "sp.pfx";

        private const string privateKeyFilePassword = "password";

        private const string idpCertificateFileName = "idp.cer";
        private const string spCertificateFileName = "sp.cer";

        // Creates a KeyInfo from the supplied X.509 certificate
        private static KeyInfo CreateKeyInfo(X509Certificate2 x509Certificate)
        {
            KeyInfoX509Data keyInfoX509Data = new KeyInfoX509Data();
            keyInfoX509Data.AddCertificate(x509Certificate);

            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(keyInfoX509Data);

            return keyInfo;
        }

        // Creates a KeyDescriptor from the supplied X.509 certificate
        private static KeyDescriptor CreateKeyDescriptor(X509Certificate2 x509Certificate)
        {
            KeyDescriptor keyDescriptor = new KeyDescriptor();
            KeyInfo keyInfo = CreateKeyInfo(x509Certificate);
            keyDescriptor.KeyInfo = keyInfo.GetXml();

            // Set the encryption method by specifying the entire XML.
            //XmlDocument xmlDocument = new XmlDocument();
            //xmlDocument.PreserveWhitespace = true;
            //xmlDocument.LoadXml("<md:EncryptionMethod xmlns:md=\"urn:oasis:names:tc:SAML:2.0:metadata\" Algorithm=\"http://www.w3.org/2001/04/xmlenc#aes256-cbc\"/>");
            //keyDescriptor.EncryptionMethods.Add(xmlDocument.DocumentElement);

            // Set the encryption method by specifying just the algorithm.
            //keyDescriptor.AddEncryptionMethod("http://www.w3.org/2001/04/xmlenc#aes256-cbc");

            return keyDescriptor;
        }

        // Creates an IdP SSO descriptor
        private static IDPSSODescriptor CreateIDPSSODescriptor()
        {
            IDPSSODescriptor idpSSODescriptor = new IDPSSODescriptor();
            idpSSODescriptor.WantAuthnRequestsSigned = true;
            idpSSODescriptor.ProtocolSupportEnumeration = SAML.NamespaceURIs.Protocol;

            X509Certificate2 x509Certificate = new X509Certificate2(idpCertificateFileName);
            idpSSODescriptor.KeyDescriptors.Add(CreateKeyDescriptor(x509Certificate));

            IndexedEndpointType artifactResolutionService = new IndexedEndpointType(1, true);
            artifactResolutionService.Binding = SAMLIdentifiers.BindingURIs.SOAP;
            artifactResolutionService.Location = "https://www.idp.com/ArtifactResolutionService";

            idpSSODescriptor.ArtifactResolutionServices.Add(artifactResolutionService);

            idpSSODescriptor.NameIDFormats.Add(SAMLIdentifiers.NameIdentifierFormats.Transient);

            EndpointType singleSignOnService = new EndpointType(SAMLIdentifiers.BindingURIs.HTTPRedirect, "https://www.idp.com/SSOService", null);
            idpSSODescriptor.SingleSignOnServices.Add(singleSignOnService);

            return idpSSODescriptor;
        }

        // Creates an IdP entity descriptor
        private static EntityDescriptor CreateIDPEntityDescriptor()
        {
            EntityDescriptor entityDescriptor = new EntityDescriptor();
            entityDescriptor.EntityID = new EntityIDType("http://www.idp.com");
            entityDescriptor.IDPSSODescriptors.Add(CreateIDPSSODescriptor());

            Organization organization = new Organization();
            organization.OrganizationNames.Add(new OrganizationName("IdP", "en"));
            organization.OrganizationDisplayNames.Add(new OrganizationDisplayName("IdP", "en"));
            organization.OrganizationURLs.Add(new OrganizationURL("www.idp.com", "en"));
            entityDescriptor.Organization = organization;

            ContactPerson contactPerson = new ContactPerson();
            contactPerson.ContactTypeValue = "technical";
            contactPerson.GivenName = "Joe";
            contactPerson.Surname = "User";
            contactPerson.EmailAddresses.Add("joe.user@idp.com");
            entityDescriptor.ContactPeople.Add(contactPerson);

            return entityDescriptor;
        }

        // Creates an SP SSO descriptor
        private static SPSSODescriptor CreateSPSSODescriptor()
        {
            SPSSODescriptor spSSODescriptor = new SPSSODescriptor();
            spSSODescriptor.ProtocolSupportEnumeration = SAML.NamespaceURIs.Protocol;

            X509Certificate2 x509Certificate = new X509Certificate2(spCertificateFileName);
            spSSODescriptor.KeyDescriptors.Add(CreateKeyDescriptor(x509Certificate));

            IndexedEndpointType assertionConsumerService1 = new IndexedEndpointType(1, true);
            assertionConsumerService1.Binding = SAMLIdentifiers.BindingURIs.HTTPPost;
            assertionConsumerService1.Location = "https://www.idp.com/AssertionConsumerService/POST";

            spSSODescriptor.AssertionConsumerServices.Add(assertionConsumerService1);

            IndexedEndpointType assertionConsumerService2 = new IndexedEndpointType(2, false);
            assertionConsumerService2.Binding = SAMLIdentifiers.BindingURIs.HTTPArtifact;
            assertionConsumerService2.Location = "https://www.idp.com/AssertionConsumerService/Artifact";

            spSSODescriptor.AssertionConsumerServices.Add(assertionConsumerService2);

            spSSODescriptor.NameIDFormats.Add(SAMLIdentifiers.NameIdentifierFormats.Transient);

            return spSSODescriptor;
        }

        // Creates an SP entity descriptor
        private static EntityDescriptor CreateSPEntityDescriptor()
        {
            EntityDescriptor entityDescriptor = new EntityDescriptor();
            entityDescriptor.EntityID = new EntityIDType("http://www.sp.com");
            entityDescriptor.SPSSODescriptors.Add(CreateSPSSODescriptor());

            Organization organization = new Organization();
            organization.OrganizationNames.Add(new OrganizationName("SP", "en"));
            organization.OrganizationDisplayNames.Add(new OrganizationDisplayName("SP", "en"));
            organization.OrganizationURLs.Add(new OrganizationURL("www.sp.com", "en"));
            entityDescriptor.Organization = organization;

            ContactPerson contactPerson = new ContactPerson();
            contactPerson.ContactTypeValue = "technical";
            contactPerson.GivenName = "Jane";
            contactPerson.Surname = "User";
            contactPerson.EmailAddresses.Add("jane.user@sp.com");
            entityDescriptor.ContactPeople.Add(contactPerson);

            return entityDescriptor;
        }

        // Reads the X.509 certificates contained within an IdP or SP SSO descriptor
        private static void ReadX509Certificates(RoleDescriptorType roleDescriptor)
        {
            foreach (KeyDescriptor keyDescriptor in roleDescriptor.KeyDescriptors)
            {
                KeyInfo keyInfo = new KeyInfo();
                keyInfo.LoadXml(keyDescriptor.KeyInfo);

                IEnumerator enumerator = keyInfo.GetEnumerator(typeof(KeyInfoX509Data));

                while (enumerator.MoveNext())
                {
                    KeyInfoX509Data keyInfoX509Data = (KeyInfoX509Data)enumerator.Current;

                    foreach (X509Certificate2 x509Certificate in keyInfoX509Data.Certificates)
                    {
                        Console.WriteLine("X509 certificate: " + x509Certificate.ToString());
                    }
                }

                foreach (XmlElement xmlElement in keyDescriptor.EncryptionMethods)
                {
                    Console.WriteLine("Encryption method: " + KeyDescriptor.GetEncryptionMethodAlgorithm(xmlElement));
                }
            }
        }

        // Reads an entities descriptor
        private static void ReadEntitiesDescriptor(EntitiesDescriptor entitiesDescriptor)
        {
            foreach (EntityDescriptor entityDescriptor in entitiesDescriptor.EntityDescriptors)
            {
                ReadEntityDescriptor(entityDescriptor);
            }
        }

        // Reads an entity descriptor
        private static void ReadEntityDescriptor(EntityDescriptor entityDescriptor)
        {
            foreach (IDPSSODescriptor idpSSODescriptor in entityDescriptor.IDPSSODescriptors)
            {
                foreach (EndpointType singleSignOnService in idpSSODescriptor.SingleSignOnServices)
                {
                    Console.WriteLine("Binding: " + singleSignOnService.Binding);
                    Console.WriteLine("Location: " + singleSignOnService.Location);
                }

                ReadX509Certificates(idpSSODescriptor);
            }

            foreach (SPSSODescriptor spSSODescriptor in entityDescriptor.SPSSODescriptors)
            {
                foreach (EndpointType assertionConsumerService in spSSODescriptor.AssertionConsumerServices)
                {
                    Console.WriteLine("Binding: " + assertionConsumerService.Binding);
                    Console.WriteLine("Location: " + assertionConsumerService.Location);
                }

                ReadX509Certificates(spSSODescriptor);
            }

            foreach (OrganizationDisplayName organizationDisplayName in entityDescriptor.Organization.OrganizationDisplayNames)
            {
                Console.WriteLine("Organization: " + organizationDisplayName.Name);
            }
        }

        // Reads SAML v2.0 metadata
        private static void ReadMetadata(XmlElement xmlElement)
        {
            if (EntitiesDescriptor.IsValid(xmlElement))
            {
                ReadEntitiesDescriptor(new EntitiesDescriptor(xmlElement));
            }
            else if (EntityDescriptor.IsValid(xmlElement))
            {
                ReadEntityDescriptor(new EntityDescriptor(xmlElement));
            }
            else
            {
                throw new ArgumentException("Expecting entities descriptor or entity descriptor");
            }
        }

        static void Main(string[] args)
        {
            // Create an IdP entity descriptor
            EntityDescriptor idpEntityDescriptor = CreateIDPEntityDescriptor();

            // Convert the IdP entity descriptor to XML
            XmlElement xmlElement = idpEntityDescriptor.ToXml();
            string s = xmlElement.OuterXml;
            Console.WriteLine(s);

            // Sign the IdP entity descriptor
            X509Certificate2 x509Certificate = new X509Certificate2(idpPrivateKeyFileName, privateKeyFilePassword);
            SAMLMetadataSignature.Generate(xmlElement, x509Certificate.PrivateKey, x509Certificate);
            Console.WriteLine(xmlElement.OuterXml);

            // Verify the IdP entity descriptor signature
            if (!SAMLMetadataSignature.Verify(xmlElement))
            {
                throw new ArgumentException("The IdP entity descriptor signature failed to verify");
            }

            // Read the IdP entity descriptor
            ReadMetadata(xmlElement);

            // Create an SP entity descriptor
            EntityDescriptor spEntityDescriptor = CreateSPEntityDescriptor();

            // Convert the SP entity descriptor to XML
            xmlElement = spEntityDescriptor.ToXml();
            Console.WriteLine(xmlElement.OuterXml);

            // Sign the SP entity descriptor
            x509Certificate = new X509Certificate2(spPrivateKeyFileName, privateKeyFilePassword);
            SAMLMetadataSignature.Generate(xmlElement, x509Certificate.PrivateKey, x509Certificate);
            Console.WriteLine(xmlElement.OuterXml);

            // Verify the SP entity descriptor signature
            if (!SAMLMetadataSignature.Verify(xmlElement))
            {
                throw new ArgumentException("The SP entity descriptor signature failed to verify");
            }

            // Read the SP entity descriptor
            ReadMetadata(xmlElement);
        }
    }
}
