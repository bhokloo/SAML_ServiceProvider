using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ActivantsSamlServiceProvider.Utility
{
    public class SamlAuthorizedDomains
    {
        public static bool IsAutorizedUrl(string url)
        {
            if (url.Contains("http://118.201.3.45:302/") || url.Contains("https://localhost:44313"))
                return true;
            else
                return false;
        }
    }
}