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

namespace insta_001.Controllers
{
    public class GrabOneUserPosts
    {
        private string commonUrl = "https://www.instagram.com/";

        public List<Data> threadPool(object objusername)
        {
            String username = (String)objusername;
            string htmlStr = ReadHtmlFile(commonUrl + username);
            List<PostInfo> posts = GetLinks(htmlStr, username);
            Thread.Sleep(150);


            List<Data> data = new List<Data>();
            string[] htmlPosts = new string[posts.Count];
            List<Comment>[] comOnePost = new List<Comment>[posts.Count];

            Thread[] _workers = new Thread[posts.Count];
            
            for (int i = 0; i < _workers.Length; i++)
            {
                int copy = i;
                _workers[i] = new Thread(() =>
                {
                    //считываем html-страницы постов 1 пользователя
                    htmlPosts[copy] = ReadHtmlFile(posts[copy].postLink);
                    //считываем с 1 html-страницы поста все комментарии
                    comOnePost[copy] = GetCommentsOnePost(htmlPosts[copy]);
                    data.Add(new Data(username, posts[copy], comOnePost[copy]));
                });
                _workers[i].Start();
                Thread.Sleep(150);
            }
            foreach (Thread thread in _workers)
            {
                thread.Join();//синхронизация потоков
            }

            return data;
        }


        private List<Comment> GetCommentsOnePost(String htmlString)
        {
            String json = ReadOneNode(htmlString, "//body/script[3]");
            json = json.Substring(21);
            json = json.Remove(json.Length - 1);
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
                        //   Int32 ctreated = Convert.ToInt32(jnode.SelectToken("created_at"));
                        Int32 unixTime = Convert.ToInt32(jnode.SelectToken("created_at"));
                        DateTime created = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(unixTime);
                        coms.Add(new Comment(author, text, created));
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

        private List<PostInfo> GetLinks(String htmlStr, String username)
        {
            String json = ReadOneNode(htmlStr, "//body/script[3]");
            json = json.Substring(21);
            json = json.Remove(json.Length - 1);
            List<PostInfo> hrefs = GetLinksFromJson(json, username);
            return hrefs;
        }

        private List<PostInfo> GetLinksFromJson(string JsonNode, string username)
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
                   // int created = Convert.ToInt32(jnode.SelectToken("date"));
                    Int32 unixTime = Convert.ToInt32(jnode.SelectToken("date"));
                    DateTime created = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(unixTime);
                    posts.Add(new PostInfo(postLink, picSrc, created, username));
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

        //считать html-стриницу из интернета и вернуть строку 
        private static String ReadHtmlFile(object objurl)
        {
            try
            {
                string url = objurl.ToString();
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