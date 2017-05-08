﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;

namespace insta_001.Parser
{
    public class FileWorker
    {
        private static Mutex mut = new Mutex();
        public bool? WriteFile(string str, string filepath = @"~\Files\instUsernames.txt")
        {
            string path = HostingEnvironment.MapPath(filepath);
            if (mut.WaitOne(1000))
            {
                try
                {
                    if (File.Exists(path))
                    {
                        string text = File.ReadAllText(path);
                        string[] usernames = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
                        File.SetAttributes(path, FileAttributes.Normal);
                        if (text.Contains(str))
                        {
                            File.WriteAllText(path, text.Replace(str + " ", ""));
                            mut.ReleaseMutex();
                            return false;
                        }
                        File.WriteAllText(path, text + str + " ");
                        mut.ReleaseMutex();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    mut.ReleaseMutex();
                    return null;
                }
            }
            mut.ReleaseMutex();
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