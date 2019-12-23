using System;

using ComponentSpace.SAML2;
using ComponentSpace.SAML2.Configuration;
using ComponentSpace.SAML2.Configuration.Resolver;
using ComponentSpace.SAML2.Data;

namespace ExampleServiceProvider
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
        public class ExampleServiceProviderConfigurationResolver : AbstractSAMLConfigurationResolver
        {
            /// <summary>
            /// Gets the <c>LocalServiceProviderConfiguration</c>.
            /// </summary>
            /// <param name="configurationID">The configuration ID or <c>null</c> if none.</param>
            /// <returns>The local service provider configuration.</returns>
            /// <exception cref="SAMLException">
            /// Thrown when the local service provider configuration cannot be found.
            /// </exception>
            public override LocalServiceProviderConfiguration GetLocalServiceProviderConfiguration(string configurationID)
            {
                return new LocalServiceProviderConfiguration()
                {
                    Name = "http://ExampleServiceProvider",
                    AssertionConsumerServiceUrl = "~/SAML/AssertionConsumerService.aspx",
                    LocalCertificateFile = @"certificates\sp.pfx",
                    LocalCertificatePassword = "password"
                };
            }

            /// <summary>
            /// Gets the <c>PartnerIdentityProviderConfiguration</c> given the partner name.
            /// </summary>
            /// <param name="configurationID">The configuration ID or <c>null</c> if none.</param>
            /// <param name="partnerName">The partner name.</param>
            /// <returns>The partner identity provider configuration.</returns>
            /// <exception cref="SAMLException">
            /// Thrown when the partner identity provider configuration cannot be found.
            /// </exception>
            public override PartnerIdentityProviderConfiguration GetPartnerIdentityProviderConfiguration(string configurationID, string partnerName)
            {
                return new PartnerIdentityProviderConfiguration()
                {
                    Name = "http://ExampleIdentityProvider",
                    SignAuthnRequest = true,
                    SingleSignOnServiceUrl = "http://localhost:51801/SAML/SSOService.aspx",
                    SingleLogoutServiceUrl = "http://localhost:51801/SAML/SLOService.aspx",
                    PartnerCertificateFile = @"certificates\idp.cer"
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
                LocalServiceProviderConfiguration = new LocalServiceProviderConfiguration()
                {
                    Name = "http://ExampleServiceProvider",
                    AssertionConsumerServiceUrl = "~/SAML/AssertionConsumerService.aspx",
                    LocalCertificateFile = @"certificates\sp.pfx",
                    LocalCertificatePassword = "password"
                }
            };

            samlConfiguration.AddPartnerIdentityProvider(
                new PartnerIdentityProviderConfiguration()
                {
                    Name = "http://ExampleIdentityProvider",
                    SignAuthnRequest = true,
                    SingleSignOnServiceUrl = "http://localhost:51801/SAML/SSOService.aspx",
                    SingleLogoutServiceUrl = "http://localhost:51801/SAML/SLOService.aspx",
                    PartnerCertificateFile = @"certificates\idp.cer"
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

                LocalServiceProviderConfiguration = new LocalServiceProviderConfiguration()
                {
                    Name = "http://ExampleServiceProvider",
                    AssertionConsumerServiceUrl = "~/SAML/AssertionConsumerService.aspx",
                    LocalCertificateFile = @"certificates\sp.pfx",
                    LocalCertificatePassword = "password"
                }
            };

            samlConfiguration.AddPartnerIdentityProvider(
                new PartnerIdentityProviderConfiguration()
                {
                    Name = "http://ExampleIdentityProvider",
                    SignAuthnRequest = true,
                    SingleSignOnServiceUrl = "http://localhost:51801/SAML/SSOService.aspx",
                    SingleLogoutServiceUrl = "http://localhost:51801/SAML/SLOService.aspx",
                    PartnerCertificateFile = @"certificates\idp.cer"
                });

            samlConfigurations.AddConfiguration(samlConfiguration);

            samlConfiguration = new SAMLConfiguration()
            {
                ID = "tenant2",

                LocalServiceProviderConfiguration = new LocalServiceProviderConfiguration()
                {
                    Name = "http://ExampleServiceProvider2",
                    AssertionConsumerServiceUrl = "~/SAML/AssertionConsumerService.aspx",
                    LocalCertificateFile = @"certificates\sp2.pfx",
                    LocalCertificatePassword = "password"
                }
            };

            samlConfiguration.AddPartnerIdentityProvider(
                new PartnerIdentityProviderConfiguration()
                {
                    Name = "http://ExampleIdentityProvider2",
                    SignAuthnRequest = true,
                    SingleSignOnServiceUrl = "http://localhost:51802/SAML/SSOService.aspx",
                    SingleLogoutServiceUrl = "http://localhost:51802/SAML/SLOService.aspx",
                    PartnerCertificateFile = @"certificates\idp2.cer"
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
            //SAMLController.ConfigurationResolver = new ExampleServiceProviderConfigurationResolver();
            //LoadSAMLConfigurationProgrammatically();
            //LoadMultiTenantedSAMLConfigurationProgrammatically();
            //ConfigureSAMLDatabase();
            //ConfigureSAMLDatabaseUsingAnonymousIDSessionIDDelegate();
            //ConfigureSAMLDatabaseUsingCustomSessionIDDelegate();
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}