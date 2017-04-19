using HtmlAgilityPack;
using insta_001.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace insta_001.Parser
{
    public class InstParser : Parser
    {
        private string[] usernames; //new string[] { "wood_cotton" }; 
                                    /*{ "wood_cotton", "babyfurniture1", "sp.baby_name",
                                "mojjevelovaya_busina","mozhevelinki","mirbusinok","fonerkin"};*/

        public List<InstModel> Main()
        {
            usernames = ReadInstUsernames();

            Thread[] _workers = new Thread[usernames.Length];
            List<InstModel> data = new List<InstModel>();
            for (int i = 0; i < _workers.Length; i++)
            {
                int copy = i;
                _workers[i] = new Thread(() => { data.AddRange(threadPool(usernames[copy])); });
                _workers[i].Name = string.Format("Thread {0} :", i + 1);
                _workers[i].Start();
            }
            foreach (Thread thread in _workers)
            {
                thread.Join();//синхронизация потоков
            }

            return data;
        }

        private string commonUrl = "https://www.instagram.com/";

        public List<InstModel> threadPool(object objusername)
        {
            String username = (String)objusername;
            string htmlStr = ReadHtmlFile(commonUrl + username + @"/media/", Encoding.UTF8);
            Thread.Sleep(150);
            List<InstModel> data = new List<InstModel>();
            if (htmlStr != "")
            {
                string node = null;
                string JsonNode = null;

                JObject jo = JObject.Parse(htmlStr);
                node = Convert.ToString(jo.SelectToken("items"));

                JArray jarr = JArray.Parse(node);
                foreach (JObject jnode in jarr)
                {
                    string numComments = Convert.ToString(jnode.SelectToken("comments.count"));
                    if (numComments != "0")
                    {
                        String postLink = Convert.ToString(jnode.SelectToken("code"));
                        postLink = @"https://www.instagram.com/p/" + postLink + "/?taken-by=" + username;
                        String picSrc = Convert.ToString(jnode.SelectToken("images.standard_resolution.url"));
                        Int32 unixTime = Convert.ToInt32(jnode.SelectToken("created_time"));
                        DateTime created = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(unixTime);
                        InstPostModel post = new InstPostModel(postLink, picSrc, created, username);

                        string nodecomm = Convert.ToString(jnode.SelectToken("comments.data"));
                        JArray jarrcomm = JArray.Parse(nodecomm);

                        List<InstCommModel> coms = new List<InstCommModel>();
                        foreach (JObject jc in jarrcomm)
                        {
                            try
                            {
                                String text = Convert.ToString(jc.SelectToken("text"));
                                String author = Convert.ToString(jc.SelectToken("from.full_name"));
                                Int32 unixTimeComm = Convert.ToInt32(jc.SelectToken("created_time"));
                                DateTime createdComm = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(unixTimeComm);
                                coms.Add(new InstCommModel(author, text, createdComm));
                            }
                            catch (Exception e) { }
                        }
                        data.Add(new InstModel(post, coms));
                    }
                }
            }
            return data;
        }
    }
}
