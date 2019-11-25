using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace ActivantsSP.Controllers
{
    public class ActivantsSpController : ApiController
    {
        [HttpGet]
        [Authorize]
        public async System.Threading.Tasks.Task<string> Get(string username)
        {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:44309/");
                HttpResponseMessage response = client.PostAsync("Token", new StringContent(
                          string.Format("grant_type=password&username={0}", HttpUtility.UrlEncode(username), Encoding.UTF8,
                          "application/x-www-form-urlencoded"))).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    string[] tokenExtraction = result.Split(new char[] { '\\', '/', ':', '{', '}', '"' });
                    string accessToken = tokenExtraction[5];
                    return accessToken;
                }
                else
                {
                    return "Fail";
                }
        }
    }
}

//var ss = Newtonsoft.Json.JsonConvert.DeserializeObject(result);

//for cheking the access token...
//    [Authorize]
//    [HttpGet]
//    [Route("api/ActivantsSp/getresource")]
//    public IHttpActionResult GetResource()
//    {
//        if(User.Identity.IsAuthenticated)
//        {
//            var identity = (ClaimsIdentity)User.Identity;
//            return Ok("Hello: " + identity.Claims.FirstOrDefault().Value);
//        }
//        return null;
//    }
//}
//}