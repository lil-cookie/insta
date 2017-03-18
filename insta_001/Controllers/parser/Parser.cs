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

namespace insta_001
{
    public class Parser
    {
        public List<Data> Main()
        {
            string htmlStr = ReadHtmlFile(commonUrl + username);
            List<Data> data = new List<Data>();
            List<PostInfo> posts = GetLinks(htmlStr);

            foreach (PostInfo post in posts)
            {
                Thread.Sleep(1000);
                htmlStr = ReadHtmlFile(post.postLink);
                List<Comment> comOnePost = GetCommentsOnePost(htmlStr);
                data.Add(new Data(post, comOnePost));
            }
           // htmlStr = ReadHtmlFile(posts[0].postLink);
           // coms.AddRange(GetCommentsOnePost(htmlStr));
           // data.Add(new Data(posts[0], coms));
            return data;
        }

        private string commonUrl = "https://www.instagram.com/";
        private string username = "wood_cotton";

        private List<PostInfo> GetLinks(String htmlString)
        {
            String json = ReadOneNode(htmlString, "//body/script[3]");
            json = json.Substring(21);
            json = json.Remove(json.Length - 1);
            //  MessageBox.Show(json);
            List<PostInfo> hrefs = GetLinksFromJson(json);
            /*     string message = "";
                 foreach (string hr in hrefs)
                 {
                     message += hr + "\n";
                 }
              */
            return hrefs;
        }
        private List<PostInfo> GetLinksFromJson(string JsonNode)
        {
            string node = null;
            try
            {
                JObject jo = JObject.Parse(JsonNode);
                node = Convert.ToString(jo.SelectToken("entry_data.ProfilePage"));

                JArray jarr = JArray.Parse(node);
                JsonNode = jarr.First.ToString();
                jo = JObject.Parse(JsonNode);
                node = Convert.ToString(jo.SelectToken("user.media.nodes"));

                // string username = Convert.ToString(jo.SelectToken("user.username"));

                List<PostInfo> posts = new List<PostInfo>();
                jarr = JArray.Parse(node);
                foreach (JObject jnode in jarr)
                {
                    String postLink = Convert.ToString(jnode.SelectToken("code"));
                    postLink = @"https://www.instagram.com/p/" + postLink + "/?taken-by=" + username;
                    String picSrc = Convert.ToString(jnode.SelectToken("thumbnail_src"));
                    posts.Add(new PostInfo(postLink, picSrc));
                }

                if (node == string.Empty) node = null;
                return posts;
            }
            catch (Exception ex)
            {
                //  MessageBox.Show(ex.Message.ToString());
                return null;
            }

        }

        private List<Comment> GetCommentsOnePost(String htmlString)
        {
            String json = ReadOneNode(htmlString, "//body/script[3]");
            json = json.Substring(21);
            json = json.Remove(json.Length - 1);
            //  MessageBox.Show(json);
            List<Comment> coms = GetCommentsFromJson(json);
            return coms;
        }
        private List<Comment> GetCommentsFromJson(string JsonNode)
        {
            string node = null;
            try
            {
                JObject jo = JObject.Parse(JsonNode);
                node = Convert.ToString(jo.SelectToken("entry_data.PostPage"));

                JArray jarr = JArray.Parse(node);
                JsonNode = jarr.First.ToString();
                jo = JObject.Parse(JsonNode);
                node = Convert.ToString(jo.SelectToken("media.comments.nodes"));

                List<Comment> coms = new List<Comment>();
                jarr = JArray.Parse(node);
                foreach (JObject jnode in jarr)
                {
                    try
                    {
                        String text = Convert.ToString(jnode.SelectToken("text"));
                        String author = Convert.ToString(jnode.SelectToken("user.username"));
                        String ctreated = Convert.ToString(jnode.SelectToken("created_at"));
                        coms.Add(new Comment( author, text, ctreated));
                    }
                    catch (Exception e) { }
                }

                if (node == string.Empty) return null;
                return coms;
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message.ToString());
                return null;
            }

        }

        //считать html-стриницу из интернета и вернуть строку 
        private string ReadHtmlFile(string url)
        {
            try
            {
                var wc = new WebClient();
                wc.Encoding = Encoding.UTF8;
                string html = wc.DownloadString(url);
                return html;
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
                return null;
            }
        }
        //считать одну ноду и вернуть только текст (без тегов)
        private string ReadOneNode(string htmlString, string xPathExpression)//переопределяем абстрактный метод класса Parser 
        {
            string str = null;
            try
            {
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(htmlString);
                // Извлекаем всё текстовое, что есть внутри тега из выражения xPathExpression
                HtmlNode node = htmlDoc.DocumentNode.SelectSingleNode(xPathExpression);
                str = node.InnerText;
            }
            catch (Exception) { }
            // Возвращаем результат работы парсера
            return str;
        }

    }
}