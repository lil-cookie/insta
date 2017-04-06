using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace insta_001.Parser
{
    public class Parser
    {
        //считать html-стриницу из интернета и вернуть строку 
        protected static String ReadHtmlFile(object objurl, Encoding enc)
        {
            try
            {
                string url = objurl.ToString();
                var wc = new WebClient();
                wc.Encoding = enc;
                string html = wc.DownloadString(url);
                return html;
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
                return null;
            }
        }

        //считать все ноды и вернуть с тегами
        protected List<string> ReadAllNodes(string htmlString, string xPathExpression)
        {
            List<string> res = new List<string>();
            try
            {
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(htmlString);
                // Извлекаем всё текстовое, что есть внутри тега из выражения xPathExpression
                HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes(xPathExpression);
                foreach (HtmlNode node in nodes)
                {
                    res.Add(node.InnerHtml);
                }

            }
            catch (Exception) { }
            // Возвращаем результат работы парсера
            return res;
        }

        //считать одну ноду и вернуть только текст (без тегов)
        protected string ReadOneNode(string htmlString, string xPathExpression)
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

        //считать одну ноду и вернуть только текст (без тегов)
        protected string ReadOneNodeAtr(string htmlString, string xPathExpression, string atrName)
        {
            string str = null;
            try
            {
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(htmlString);
                // Извлекаем всё текстовое, что есть внутри тега из выражения xPathExpression
                HtmlNode node = htmlDoc.DocumentNode.SelectSingleNode(xPathExpression);
                str = node.Attributes[atrName].Value;
            }
            catch (Exception) { }
            // Возвращаем результат работы парсера
            return str;
        }
    }
}