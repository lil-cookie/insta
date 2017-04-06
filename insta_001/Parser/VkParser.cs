using insta_001.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace insta_001.Parser
{
    public class VkParser : Parser
    {
        string url = "https://vk.com/photos-95557674?act=comments";
        public List<VkModel> Main()
        {
            string htmlStr = ReadHtmlFile(url, Encoding.GetEncoding(1251));
            List<VkModel> posts = GetComments(htmlStr);
            return posts;
        }

        private List<VkModel> GetComments(string html)
        {

            List<string> nodes = ReadAllNodes(html, "//div[@class='reply reply_dived clear  _post']");
            List<VkModel> res = new List<VkModel>();
            foreach (string n in nodes)
            {
                VkModel com = new VkModel();

                com.authorId = ReadOneNodeAtr(n, "//a[@class='author']", "data-from-id");
                com.authorName = ReadOneNode(n, "//a[@class='author']");
                com.authorHref = "https://www.vk.com" + ReadOneNodeAtr(n, "//a[@class='author']", "href");
                com.comment = ReadOneNode(n, "//div[@class='wall_reply_text']");
                com.created = ReadOneNode(n, "//div[@class='reply_date']");
                com.postHref = "https://www.vk.com" + ReadOneNodeAtr(n, "//a[@class='reply_thumb']", "href");
                com.postPhotoHref = ReadOneNodeAtr(n, "//img[@class='reply_thumb_img']", "src");
                res.Add(com);
            }
            return res;
        }
    }
}