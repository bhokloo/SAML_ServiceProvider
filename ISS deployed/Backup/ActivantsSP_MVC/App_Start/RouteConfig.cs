using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ActivantsSP
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "About", id = UrlParameter.Optional }
            );
        }
    }
}

    // public class CustonJsonFormatter : JsonMediaTypeFormatter
    //    {
    //        public CustonJsonFormatter()
    //        {
    //            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
    //        }

    //        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
    //        {
    //            base.SetDefaultContentHeaders(type, headers, mediaType);
    //            headers.ContentType = new MediaTypeHeaderValue("application/json");
    //        }
    //    }
    //}
