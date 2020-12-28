using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace Task1
{
    public partial class Task1CSharp : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            UriBuilder url = new UriBuilder();
            url.Scheme = "https";

            url.Host = "api.data.gov.sg";
            url.Path = "v1/environment/2-hour-weather-forecast";

            HttpWebRequest request = WebRequest.Create(url.ToString()) as HttpWebRequest;
            request.Timeout = 15 * 1000;
            request.KeepAlive = false;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string content = reader.ReadToEnd();

            Response.Write(content);

        }

    }
}
