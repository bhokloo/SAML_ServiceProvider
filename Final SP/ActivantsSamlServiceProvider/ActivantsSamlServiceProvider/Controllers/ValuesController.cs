using ActivantsSamlServiceProvider.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace ActivantsSamlServiceProvider.Controllers
{
    public class ValuesController : ApiController
    {
        [HttpGet]
        public async System.Threading.Tasks.Task<string> Get(string username)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44364/");
            HttpResponseMessage response = client.PostAsync("Token", new StringContent(
                      string.Format("grant_type=password&username={0}&id=123456789", HttpUtility.UrlEncode(username), Encoding.UTF8,
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
                return null;
            }
        }

        public IHttpActionResult GetSamlData()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IDictionary<string, string> SamlData = new Dictionary<string, string>();
            foreach(var value in identity.Claims)
            {
                SamlData.Add(value.Type.Split('/').Where(x => !string.IsNullOrWhiteSpace(x)).LastOrDefault(), value.Value);
            }

            if (SamlData.Count <= 0)
                return null;

            try
            {
                string SPJsonSamlData = (new JavaScriptSerializer()).Serialize(SamlData);
                var EncryptedSamlData = HttpUtility.UrlEncode(Cryptography.Encrypt(SPJsonSamlData));
                return Ok(EncryptedSamlData);
            }
            catch (Exception)
            {
                return null;
            }
        }

        //client side
        public IHttpActionResult GetMachineKey(string encryptedSamlData)
        {
            var DecryptedSamlData = new Cryptography().Decrypt(HttpUtility.UrlDecode(encryptedSamlData));
            var ClientJsonSaml = JsonConvert.DeserializeObject(DecryptedSamlData);

            JObject obj = JObject.Parse(ClientJsonSaml.ToString());
            //{"nameidentifier":"indrajit","username":"indrajit","id":"123456789"}
            string username = (string)obj["username"];

            return Ok(ClientJsonSaml);
        }
    }
}
