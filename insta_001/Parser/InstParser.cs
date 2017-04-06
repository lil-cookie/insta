﻿using HtmlAgilityPack;
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
        private List<String> usernames = new List<string>() { "wood_cotton", "babyfurniture1", "sp.baby_name",
        "mojjevelovaya_busina","mozhevelinki","mirbusinok","fonerkin"};

        public List<InstModel> Main()
        {
            Thread[] _workers = new Thread[usernames.Count];
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
            string htmlStr = ReadHtmlFile(commonUrl + username, Encoding.UTF8);
            List<InstPostModel> posts = GetLinks(htmlStr, username);
            Thread.Sleep(150);


            List<InstModel> data = new List<InstModel>();
            string[] htmlPosts = new string[posts.Count];
            List<InstCommModel>[] comOnePost = new List<InstCommModel>[posts.Count];

            Thread[] _workers = new Thread[posts.Count];

            for (int i = 0; i < _workers.Length; i++)
            {
                int copy = i;
                _workers[i] = new Thread(() =>
                {
                    //считываем html-страницы постов 1 пользователя
                    htmlPosts[copy] = ReadHtmlFile(posts[copy].postLink, Encoding.UTF8);
                    //считываем с 1 html-страницы поста все комментарии
                    comOnePost[copy] = GetCommentsOnePost(htmlPosts[copy]);
                    data.Add(new InstModel(username, posts[copy], comOnePost[copy]));
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


        private List<InstCommModel> GetCommentsOnePost(String htmlString)
        {
            String json = ReadOneNode(htmlString, "//body/script[3]");
            json = json.Substring(21);
            json = json.Remove(json.Length - 1);
            List<InstCommModel> coms = GetCommentsFromJson(json);
            return coms;
        }
        private List<InstCommModel> GetCommentsFromJson(string JsonNode)
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

                List<InstCommModel> coms = new List<InstCommModel>();
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
                        coms.Add(new InstCommModel(author, text, created));
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

        private List<InstPostModel> GetLinks(String htmlStr, String username)
        {
            String json = ReadOneNode(htmlStr, "//body/script[3]");
            json = json.Substring(21);
            json = json.Remove(json.Length - 1);
            List<InstPostModel> hrefs = GetLinksFromJson(json, username);
            return hrefs;
        }

        private List<InstPostModel> GetLinksFromJson(string JsonNode, string username)
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

                List<InstPostModel> posts = new List<InstPostModel>();
                jarr = JArray.Parse(node);
                foreach (JObject jnode in jarr)
                {
                    String postLink = Convert.ToString(jnode.SelectToken("code"));
                    postLink = @"https://www.instagram.com/p/" + postLink + "/?taken-by=" + username;
                    String picSrc = Convert.ToString(jnode.SelectToken("thumbnail_src"));
                    // int created = Convert.ToInt32(jnode.SelectToken("date"));
                    Int32 unixTime = Convert.ToInt32(jnode.SelectToken("date"));
                    DateTime created = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(unixTime);
                    posts.Add(new InstPostModel(postLink, picSrc, created, username));
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

    }
}