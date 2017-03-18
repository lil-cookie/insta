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
            IEnumerable<Data> data = comms.Where(ev => ev.comments.Count > 0).OrderByDescending(ev => ev.info.createdAt);//.OrderBy(ev => ev.postAuthor)
            return View(data);
        }
    }



}