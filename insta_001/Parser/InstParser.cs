using insta_001.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace insta_001.Parser
{
    public class InstParser : Parser
    {
        private string[] usernames; /* = new string[] "wood_cotton", "babyfurniture1", "sp.baby_name","mojjevelovaya_busina","mozhevelinki","mirbusinok","fonerkin"};*/
                                    /*new string[] { "wood_cotton", "babyfurniture1", "sp.baby_name", "mojjevelovaya_busina","mozhevelinki","mirbusinok","fonerkin"};*/
                                    //wood_cotton babyfurniture1 sp.baby_name mojjevelovaya_busina mozhevelinki mirbusinok fonerkin

        private int deep = 6;
        string lastPostId = "0";

        public List<InstModel> Main(bool isMyAcc = false)
        {
            if (isMyAcc == true)
            {
                usernames = new string[] { "eco_bysinki" };
            }
            else
            {
                usernames = ReadInstUsernames();
            }

            if (usernames != null)
            {
                List<InstModel> data = CreateThreadPool();
                return data;
            }
            else return null;
        }


        private List<InstModel> CreateThreadPool()
        {
            Thread[] _workers = new Thread[usernames.Length];
            InstParser[] _objInst = new InstParser[usernames.Length];
            for (int i = 0; i < _objInst.Length; i++)
            {
                _objInst[i] = new InstParser();
            }
            List<InstModel> data = new List<InstModel>();
            for (int i = 0; i < _workers.Length; i++)
            {
                int copy = i;
                _workers[i] = new Thread(() => { data.AddRange(_objInst[copy].threadPool(usernames[copy])); });
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

        private List<InstModel> threadPool(object objusername)
        {
            List<InstModel> data = new List<InstModel>();
            //string lastPostId = "0";
            List<InstModel> _20posts = new List<InstModel>();
            int i = 0;
            do
            {
                _20posts = GetNext20Posts((string)objusername, lastPostId);
                if (_20posts.Count != 0)
                {
                    data.AddRange(_20posts);
                    lastPostId = _20posts[_20posts.Count - 1].postId;
                }
                else break;

                i++;
            }
            while (i < deep);

            return data;
        }

        private string GetLastCommId(JObject jnode)
        {
            string lastId = Convert.ToString(jnode.SelectTokens("text"));

            return lastId;
        }

        private List<InstModel> GetNext20Posts(String username, string lastPostId)
        {
            // string htmlStr = ReadHtmlFile(commonUrl + username + @"/media", Encoding.UTF8);
            List<InstModel> data = new List<InstModel>();
            try
            {
                string htmlStr = ReadHtmlFile(commonUrl + username + @"/media/?max_id=" + lastPostId, Encoding.UTF8);
                Thread.Sleep(150);
                if (htmlStr != null)
                {
                    string node = null;

                    JObject jo = JObject.Parse(htmlStr);
                    node = Convert.ToString(jo.SelectToken("items"));

                    JArray jarr = JArray.Parse(node);
                    foreach (JObject jnode in jarr)
                    {
                        //проверяем, что у данного поста есть комментарии
                        string numComments = Convert.ToString(jnode.SelectToken("comments.count"));
                        if (numComments != "0")
                        {
                            //заполняем информацию о посте в объект post
                            InstModel post = getPostInfo(jnode, username);
                            // InstPostModel post = getPostInfo(jnode, username);

                            //получаем массив json-объектов, описывающих комментарии к посту
                            string nodecomm = Convert.ToString(jnode.SelectToken("comments.data"));
                            JArray jarrcomm = JArray.Parse(nodecomm);

                            //заполняем информацию о комментариях к данному посту в список объектов coms
                            post.comments = getCommentsList(jarrcomm);

                            data.Add(post);
                        }
                    }
                    string lastId = getPostId((JObject)jarr.Last);
                }
            }
            catch (Exception e) { }
            return data;
        }





        private List<InstCommModel> getCommentsList(JArray jarrcomm)
        {
            List<InstCommModel> coms = new List<InstCommModel>();
            foreach (JObject jc in jarrcomm)
            {
                try
                {
                    //заполняем информацию о, одном сомментарии к данному посту в объект comment
                    InstCommModel comment = getCommentInfo(jc);
                    coms.Add(comment);
                }
                catch (Exception e) { }
            }
            return coms;
        }

        private InstCommModel getCommentInfo(JObject jnode)
        {
            InstCommModel comment = new InstCommModel();
            comment.text = Convert.ToString(jnode.SelectToken("text"));
            comment.author = Convert.ToString(jnode.SelectToken("from.username"));
            comment.created = getCreatedTime(jnode);
            return comment;
        }

        private InstModel getPostInfo(JObject jnode, string username)
        {
            InstModel post = new InstModel();
            // InstPostModel post = new InstPostModel();
            post.postLink = getPostLink(jnode, username);
            post.picSrc = getPostPicSrc(jnode);
            post.created = getCreatedTime(jnode);
            post.postId = getPostId(jnode);
            post.postAuthor = username;
            return post;
        }

        private DateTime getCreatedTime(JObject jnode)
        {
            Int64 unixTime = Convert.ToInt64(jnode.SelectToken("created_time"));
            // unixTime = unixTime * 1000;
             return (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(unixTime);
           // return unixTime;
        }
        private string getPostId(JObject jnode)
        {
            return Convert.ToString(jnode.SelectToken("id"));
        }
        private string getPostPicSrc(JObject jnode)
        {
            return Convert.ToString(jnode.SelectToken("images.standard_resolution.url"));
        }
        private string getPostLink(JObject jnode, string username)
        {
            String postLink = Convert.ToString(jnode.SelectToken("code"));
            return (@"https://www.instagram.com/p/" + postLink + "/?taken-by=" + username);
        }

    }
}
