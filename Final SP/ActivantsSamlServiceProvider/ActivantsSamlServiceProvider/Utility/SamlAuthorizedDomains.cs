using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace ActivantsSamlServiceProvider.Utility
{
    public class SamlAuthorizedDomains
    {
        public static bool IsAutorizedUrl(string url)
        {
            bool value = false;
            value = bool.Parse(WebConfigurationManager.AppSettings[url]);
            if(value)
                return true;
            else
                return false;
        }
    }
}