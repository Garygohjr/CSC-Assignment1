using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Task7.Models;

namespace Task7.Controllers
{
    public class VerifyController : Controller
    {
        [HttpGet]
        [Route("api/v1/verify")]
        public string verifi(string base64)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://" + Keys.ENVIRONMENT_URL + "/api/v7/partner/documents/");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", "apikey " + Keys.USERNAME + ":" + Keys.API_KEY);
            httpWebRequest.Headers.Add("Client-id", Keys.CLIENT_ID);
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"file_name\":\"receipt.jpg\"," +
                               "\"file_data\":\"" + base64 + "\"}";
                streamWriter.Write(json);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var jsonResponse = streamReader.ReadToEnd();
                return (String.Format("Response: {0}", jsonResponse));
            }
        }
    }
}