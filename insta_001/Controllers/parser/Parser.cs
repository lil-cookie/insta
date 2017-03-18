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
using System.Runtime.InteropServices;

namespace insta_001
{
    public class Parser
    {
        private List<String> usernames = new List<string>() { "wood_cotton", "babyfurniture1", "sp.baby_name",
        "mojjevelovaya_busina","mozhevelinki","mirbusinok","fonerkin"};
        private string commonUrl = "https://www.instagram.com/";

/*
        public delegate String ExampleCallback(object parameter);
        ExampleCallback callback = new ExampleCallback(ReadHtmlFile);

        // private String userPostsHtml = null;
        public void ResultCallback(object parameter)
        {
            callback.BeginInvoke(parameter, new AsyncCallback(TaskFinished), null);
        }
        public void TaskFinished(IAsyncResult result)
        {
            usersPostsHtml.ElementAt(callback.EndInvoke(result));
        }
        */

        private List<String> usersPostsHtml = new List<String>();

        public List<Data> Main()
        {
            Thread[] _workers = new Thread[usernames.Count];
            /*
            //получили html-страницы юзеров с последними 12 фото
            for (int i=0;i<_workers.Length;i++)
            {
                _workers[i] = new Thread(new ParameterizedThreadStart(ResultCallback));
                _workers[i].Name = string.Format("Thread {0} :", i + 1);
                _workers[i].Start(commonUrl + usernames[i]);
                Thread.Sleep(50);
            }

            foreach (Thread thread in _workers)
            {
                thread.Join();//синхронизация потоков
            }
           */
           
            for (int i = 0; i < _workers.Length; i++)
            {
                _workers[i] = new Thread(new ParameterizedThreadStart(threadPool));
                _workers[i].Name = string.Format("Thread {0} :", i + 1);
                _workers[i].Start(usernames[i]);
                Thread.Sleep(50);
            }
            foreach (Thread thread in _workers)
            {
                thread.Join();//синхронизация потоков
            }
            return data;
        }

        List<Data> data = new List<Data>();
        private void threadPool(object objusername)
        {
            String username= (String)objusername;
            string htmlStr = ReadHtmlFile(commonUrl + username);
            List<PostInfo> posts = GetLinks(htmlStr, username);

            foreach (PostInfo post in posts)
            {
                Thread.Sleep(500);
                htmlStr = ReadHtmlFile(post.postLink);
                List<Comment> comOnePost = GetCommentsOnePost(htmlStr);
                data.Add(new Data(username, post, comOnePost));
            }
          //  return data;
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
                    String createdAt = Convert.ToString(jnode.SelectToken("date"));
                    posts.Add(new PostInfo(postLink, picSrc, createdAt));
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
                        coms.Add(new Comment(author, text, ctreated));
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

       // private static Mutex mutex = new Mutex();
        //считать html-стриницу из интернета и вернуть строку 
        private static String ReadHtmlFile(object objurl)
        {
            try
            {
              //  mutex.WaitOne();
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
          /*  finally
            {
                mutex.ReleaseMutex();
            }*/
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