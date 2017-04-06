using insta_001.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace insta_001.Controllers
{
    public class Parser
    {
        private List<String> usernames = new List<string>() { "wood_cotton", "babyfurniture1", "sp.baby_name",
        "mojjevelovaya_busina","mozhevelinki","mirbusinok","fonerkin"};
        public List<Data> Main()
        {
            Thread[] _workers = new Thread[usernames.Count];
            List<Data> data = new List<Data>();
            GrabOneUserPosts o = new GrabOneUserPosts();
            for (int i = 0; i < _workers.Length; i++)
            {
                int copy = i;
                _workers[i] = new Thread(() => { data.AddRange(o.threadPool(usernames[copy])); });
                _workers[i].Name = string.Format("Thread {0} :", i + 1);
                _workers[i].Start();
            }
            foreach (Thread thread in _workers)
            {
                thread.Join();//синхронизация потоков
            }

            return data;
        }

    }
}