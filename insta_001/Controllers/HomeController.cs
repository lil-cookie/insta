using insta_001.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace insta_001.Controllers
{
    public class HomeController : Controller
    {


        // GET: Home
        public ActionResult Index()
        {
            //  ViewBag.Collection = new string[2,3]{ { "dsfds", "sdfds", "sfdf" }, { "sdfds", "sfdf", "sdfds" } };

            Parser p = new Parser();
            List<Data> comms = p.Main();
            comms = comms.Where(ev => ev.comments.Count > 0).OrderByDescending(ev => ev.info.created).ToList();
            // IEnumerable<Data> data = comms.Where(ev => ev.comments.Count > 0).OrderByDescending(ev => ev.info.createdAt);//.OrderBy(ev => ev.postAuthor)

            //  comms.Remove(comms.Where(ev => ev.comments.Count == 0));// = (List<Data>)comms.Where);

               for (int i = 0; i < comms.Count(); i++)
               {
                   for (int j = 0; j < comms.Count() - i - 1; j++)
                   {
                       if (comms[j].comments.Last().created < comms[j+1].comments.Last().created)
                       {
                           var temp = comms[j];
                           comms[j] = comms[j+1];
                           comms[j + 1] = temp;
                       }
                   }
               }
               
            return View(comms);
        }
    }



}