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
        public bool WriteFile(string str, string filepath = @"~\Files\instUsernames.txt")
        {
            string path = HostingEnvironment.MapPath(filepath);

            if (File.Exists(path))
            {
                string text = File.ReadAllText(path);
                string[] usernames = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
                File.SetAttributes(path, FileAttributes.Normal);
                if (text.Contains(str))
                { 
                    File.WriteAllText(path, text.Replace(str+" ", ""));
                    return false;
                }
                File.WriteAllText(path, text + str + " ");
                return true;
            }
            return true;
        }
        public string[] ReadInstUsernames(string filepath = @"~\Files\instUsernames.txt")
        {
            string path = HostingEnvironment.MapPath(filepath);
            string[] usernames = null;
            if (File.Exists(path))
            {
                usernames = File.ReadAllText(path).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
            }
            return usernames;
        }
    }
}