using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace GPTChatBot
{
    class APIHelper
    {
        public static bool CallAPI(out object oReturn, HttpVerbs verb, Type type, string controller, string id = null, string action = null, object body = null, List<WSParams> liParams = null)
        {
            string url = Program.configuration["APIURL"];
            url = $"{(url.EndsWith("/") ? url : url + "/")}{controller + "/"}{(string.IsNullOrEmpty(id) ? "" : id + "/")}{(string.IsNullOrEmpty(action) ? "" : action + "/")}";
            if (liParams != null && liParams.Count > 0)
            {
                url += "?";
                for (int i = 0; i < liParams.Count; i++)
                {
                    if (i > 0)
                        url += "&";
                    url += liParams[i].URLString;
                }
            }
            string key = Program.configuration["APIKEY"];
            oReturn = null;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("AuthID", key);
                HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(verb.ToString()), url);
                if(verb != HttpVerbs.Get && body != null)
                    requestMessage.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                HttpResponseMessage resp = client.SendAsync(requestMessage).Result;
                if (!resp.IsSuccessStatusCode)
                {
                    Console.WriteLine(resp.Content.ReadAsAsync<HttpError>().Result?.Message);
                    return false;
                }
                if (!string.IsNullOrEmpty(resp.Content?.ReadAsStringAsync().Result))
                {
                    try
                    {
                        oReturn = JsonConvert.DeserializeObject(resp.Content?.ReadAsStringAsync().Result, type);
                        Convert.ChangeType(oReturn, type);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + "\r\n" + e.InnerException);
                        return false;
                    }
                }
            }
            return true;
        }
    }
    public class WSParams
    {
        string Name { get; set; }
        public string Value { get; set; }
        public string URLString { get { return $"{Name}={Value}"; } }
    }
}
