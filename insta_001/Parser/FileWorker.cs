using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace insta_001.Parser
{
    public class FileWorker
    {
        public void WriteFile(string str)
        {
            string path = HostingEnvironment.MapPath(@"~\Parser\instUsernames.txt");
            StreamWriter sw = new StreamWriter(path, true);
            sw.Write(str + " ");
            sw.Close();
        }
        public string[] ReadInstUsernames()
        {
            string path = HostingEnvironment.MapPath(@"~\Parser\instUsernames.txt");
            string[] usernames = null;
            if (File.Exists(path))
            {
                usernames = File.ReadAllText(path).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
            }
            return usernames;
        }
    }
}