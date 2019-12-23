using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using ComponentSpace.SAML2;

namespace ADFSSP
{
    public static class Configuration
    {
        private static class ConfigurationKeys
        {
            public const string Issuer = "Issuer";
            public const string SingleSignOnServiceBinding = "SingleSignOnServiceBinding";
            public const string HttpPostSingleSignOnServiceURL = "HttpPostSingleSignOnServiceURL";
            public const string HttpRedirectSingleSignOnServiceURL = "HttpRedirectSingleSignOnServiceURL";
        }

        public static string Issuer
        {
            get
            {
                return WebConfigurationManager.AppSettings[ConfigurationKeys.Issuer];
            }
        }

        public static SAMLIdentifiers.Binding SingleSignOnServiceBinding
        {
            get
            {
                return SAMLIdentifiers.BindingURIs.URIToBinding(WebConfigurationManager.AppSettings[ConfigurationKeys.SingleSignOnServiceBinding]);
            }
        }

        public static string SingleSignOnServiceURL
        {
            get
            {
                switch (SingleSignOnServiceBinding)
                {
                    case SAMLIdentifiers.Binding.HTTPPost:
                        return HttpPostSingleSignOnServiceURL;

                    case SAMLIdentifiers.Binding.HTTPRedirect:
                        return HttpRedirectSingleSignOnServiceURL;

                    default:
                        throw new ArgumentException("Invalid single signon service binding");
                }
            }
        }

        public static string HttpPostSingleSignOnServiceURL
        {
            get
            {
                return WebConfigurationManager.AppSettings[ConfigurationKeys.HttpPostSingleSignOnServiceURL];
            }
        }

        public static string HttpRedirectSingleSignOnServiceURL
        {
            get
            {
                return WebConfigurationManager.AppSettings[ConfigurationKeys.HttpRedirectSingleSignOnServiceURL];
            }
        }
    }
}
