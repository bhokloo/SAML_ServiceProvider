using System;

using ComponentSpace.SAML2;
using ComponentSpace.SAML2.Configuration;
using ComponentSpace.SAML2.Configuration.Resolver;
using ComponentSpace.SAML2.Data;

namespace ExampleIdentityProvider
{
    public class Global : System.Web.HttpApplication
    {
        // SAML configuration may be specified using one of the following approaches:
        //
        // (1) using a saml.config file in the application's directory that's loaded automatically
        // (2) programmatically by calling the SAML configuration API
        // (3) programmatically by implementing the ISAMLConfigurationResolver interface
        //
        // The saml.config file is the simplest approach and requires no additional coding.
        //
        // If SAML configuration information is stored in a database, it must be set programmatically.
        //
        // If the SAML configuration changes infrequently, it may be set using the SAML configuration API, 
        // typically at application start-up.
        //
        // If the SAML configuration changes frequently, it's better to implement the ISAMLConfigurationResolver interface
        // for the on-demand retrieval of SAML configuration information.
        //
        // The following code demonstrates the two approaches to setting the configuration programmatically.



        // This class demonstrates loading configuration programmatically by implementing the ISAMLConfigurationResolver interface.
        // This interface supports the on-demand retrieval of SAML configuration information.
        // Alternatively, configuration may be loaded programmatically by calling the SAML configuration API.
        // Either of these approaches may be used if you wish to store configuration in a custom database, for example.
        // If not configured programmatically, configuration is loaded automatically from the saml.config file 
        // in the application's directory.
        public class ExampleIdentityProviderConfigurationResolver : AbstractSAMLConfigurationResolver
        {
            /// <summary>
            /// Gets the <c>LocalIdentityProviderConfiguration</c>.
            /// </summary>
            /// <param name="configurationID">The configuration ID or <c>null</c> if none.</param>
            /// <returns>The local identity provider configuration.</returns>
            /// <exception cref="SAMLException">
            /// Thrown when the local identity provider configuration cannot be found.
            /// </exception>
            public override LocalIdentityProviderConfiguration GetLocalIdentityProviderConfiguration(string configurationID)
            {
                return new LocalIdentityProviderConfiguration()
                {
                    Name = "http://ExampleIdentityProvider",
                    LocalCertificateFile = @"certificates\idp.pfx",
                    LocalCertificatePassword = "password"
                };
            }

            /// <summary>
            /// Gets the <c>PartnerServiceProviderConfiguration</c> given the partner name.
            /// </summary>
            /// <param name="configurationID">The configuration ID or <c>null</c> if none.</param>
            /// <param name="partnerName">The partner name.</param>
            /// <returns>The partner service provider configuration.</returns>
            /// <exception cref="SAMLException">
            /// Thrown when the partner service provider configuration cannot be found.
            /// </exception>
            public override PartnerServiceProviderConfiguration GetPartnerServiceProviderConfiguration(string configurationID, string partnerName)
            {
                return new PartnerServiceProviderConfiguration()
                {
                    Name = "http://ExampleServiceProvider",
                    WantAuthnRequestSigned = true,
                    SignSAMLResponse = true,
                    AssertionConsumerServiceUrl = "http://localhost:51901/SAML/AssertionConsumerService.aspx",
                    SingleLogoutServiceUrl = "http://localhost:51901/SAML/SLOService.aspx",
                    PartnerCertificateFile = @"certificates\sp.cer"
                };
            }
        }

        // This method demonstrates loading configuration programmatically by calling the SAML configuration API.
        // Alternatively, configuration may be loaded programmatically by implementing the ISAMLConfigurationResolver interface.
        // Either of these approaches may be used if you wish to store configuration in a custom database, for example.
        // If not configured programmatically, configuration is loaded automatically from the saml.config file 
        // in the application's directory.
        private static void LoadSAMLConfigurationProgrammatically()
        {
            SAMLConfiguration samlConfiguration = new SAMLConfiguration()
            {
                LocalIdentityProviderConfiguration = new LocalIdentityProviderConfiguration()
                {
                    Name = "http://ExampleIdentityProvider",
                    LocalCertificateFile = @"certificates\idp.pfx",
                    LocalCertificatePassword = "password"
                }
            };

            samlConfiguration.AddPartnerServiceProvider(
                new PartnerServiceProviderConfiguration()
                {
                    Name = "http://ExampleServiceProvider",
                    WantAuthnRequestSigned = true,
                    SignSAMLResponse = true,
                    AssertionConsumerServiceUrl = "http://localhost:51901/SAML/AssertionConsumerService.aspx",
                    SingleLogoutServiceUrl = "http://localhost:51901/SAML/SLOService.aspx",
                    PartnerCertificateFile = @"certificates\sp.cer"
                });

            SAMLController.Configuration = samlConfiguration;
        }

        // This method demonstrates loading multi-tenanted configuration programmatically by calling the SAML configuration API.
        // Alternatively, configuration is loaded automatically from the multi-tenanted saml.config file in the application's directory.
        private static void LoadMultiTenantedSAMLConfigurationProgrammatically()
        {
            SAMLConfigurations samlConfigurations = new SAMLConfigurations();

            SAMLConfiguration samlConfiguration = new SAMLConfiguration()
            {
                ID = "tenant1",

                LocalIdentityProviderConfiguration = new LocalIdentityProviderConfiguration()
                {
                    Name = "http://ExampleIdentityProvider",
                    LocalCertificateFile = @"certificates\idp.pfx",
                    LocalCertificatePassword = "password"
                }
            };

            samlConfiguration.AddPartnerServiceProvider(
                new PartnerServiceProviderConfiguration()
                {
                    Name = "http://ExampleServiceProvider",
                    WantAuthnRequestSigned = true,
                    SignSAMLResponse = true,
                    AssertionConsumerServiceUrl = "http://localhost:51901/SAML/AssertionConsumerService.aspx",
                    SingleLogoutServiceUrl = "http://localhost:51901/SAML/SLOService.aspx",
                    PartnerCertificateFile = @"certificates\sp.cer"
                });

            samlConfigurations.AddConfiguration(samlConfiguration);

            samlConfiguration = new SAMLConfiguration()
            {
                ID = "tenant2",

                LocalIdentityProviderConfiguration = new LocalIdentityProviderConfiguration()
                {
                    Name = "http://ExampleIdentityProvider2",
                    LocalCertificateFile = @"certificates\idp2.pfx",
                    LocalCertificatePassword = "password"
                }
            };

            samlConfiguration.AddPartnerServiceProvider(
                new PartnerServiceProviderConfiguration()
                {
                    Name = "http://ExampleServiceProvider2",
                    WantAuthnRequestSigned = true,
                    SignSAMLResponse = true,
                    AssertionConsumerServiceUrl = "http://localhost:51902/SAML/AssertionConsumerService.aspx",
                    SingleLogoutServiceUrl = "http://localhost:51902/SAML/SLOService.aspx",
                    PartnerCertificateFile = @"certificates\sp2.cer"
                });

            samlConfigurations.AddConfiguration(samlConfiguration);

            SAMLController.Configurations = samlConfigurations;
        }

        // This method demonstrates using a database to store SAML identifiers and session data in a database.
        // This may be required in a web farm deployment when ASP.NET sessions are not stored in a database.
        // ASP.NET session cookies are used to uniquely identify SSO sessions.
        // ASP.NET session cookies must be enabled in web.config.
        private static void ConfigureSAMLDatabase()
        {
            SAMLController.SSOSessionStore = new DatabaseSSOSessionStore();
            SAMLController.IDCache = new DatabaseIDCache();
        }

        // This method demonstrates using a database to store SAML identifiers and session data in a database.
        // This may be required in a web farm deployment when ASP.NET sessions are not stored in a database.
        // SAML session cookies are used to uniquely identify SSO sessions.
        private static void ConfigureSAMLDatabaseUsingSAMLCookie()
        {
            SAMLController.SSOSessionStore = new DatabaseSSOSessionStore()
            {
                SessionIDDelegate = new SessionIDDelegate(SessionIDDelegates.GetSessionIDFromSAMLCookie)
            };

            SAMLController.IDCache = new DatabaseIDCache();
        }

        // This method demonstrates using a database to store SAML identifiers and session data in a database.
        // This may be required in a web farm deployment when ASP.NET sessions are not stored in a database.
        // A custom session ID delegate is used to uniquely identify SSO sessions.
        private static void ConfigureSAMLDatabaseUsingCustomSessionIDDelegate()
        {
            SAMLController.SSOSessionStore = new DatabaseSSOSessionStore()
            {
                SessionIDDelegate = delegate ()
                {
                    // Return an identifier that uniquely identifies the user's SSO session.
                    // The implementation details are not shown.
                    return null;
                }
            };

            SAMLController.IDCache = new DatabaseIDCache();
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            //SAMLController.ConfigurationResolver = new ExampleIdentityProviderConfigurationResolver();
            //LoadSAMLConfigurationProgrammatically();
            //LoadMultiTenantedSAMLConfigurationProgrammatically();
            //ConfigureSAMLDatabase();
            //ConfigureSAMLDatabaseUsingSAMLCookie();
            //ConfigureSAMLDatabaseUsingCustomSessionIDDelegate();
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}