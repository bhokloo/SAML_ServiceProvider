using System;
using System.Web.Mvc;

namespace ClientApplication.Controllers
{
    internal class BasicAuthenticationAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            throw new NotImplementedException();
        }
    }
}