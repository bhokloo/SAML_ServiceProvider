using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http;

namespace ActivantsSamlServiceProvider.Controllers
{
    public class ValuesController : ApiController
    {
        [HttpGet]
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

        public IHttpActionResult Get()
        {
            var identity = (ClaimsIdentity)User.Identity;
            return Ok("helo " + identity.Name +"  "+ identity.Claims.Where(f => f.Type == "id").Select(f => f.Value).SingleOrDefault()[0]);
        }

        //// GET api/values/5
        //public string Get(int id)
        //{
        //    return "value";
        //}


        //// PUT api/values/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //public void Delete(int id)
        //{
        //}
    }
}
