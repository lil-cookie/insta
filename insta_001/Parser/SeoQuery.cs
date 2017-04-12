using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace insta_001.Parser
{
    public class SeoQuery : Parser
    {
        private List<string> redirectUrls = new List<string>();

        public void Main()
        {

            var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Parser/urls.txt");
            redirectUrls = File.ReadLines(path).ToList();

            foreach (var url in redirectUrls)
            {
                ReadHtmlFile(url, Encoding.UTF8);
                //System.Diagnostics.Process.Start(url);
                WebRequest wrGETURL = WebRequest.Create(url);
                wrGETURL.GetResponse();
            }

        }
    }
}