using insta_001.Models;
using insta_001.Parser;
using System.Web.Mvc;
using System;

namespace insta_001.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            FileWorker fw = new FileWorker();
            ViewBag.users = fw.ReadInstUsernames();
            return View();
        }


        [HttpPost]
        public ActionResult Index(string user)
        {
            FileWorker fw = new FileWorker();
            bool upd = fw.WriteFile(user.Trim());
            ViewBag.upd = upd;
            ViewBag.username = user;
            fw = new FileWorker();
            ViewBag.users = fw.ReadInstUsernames();
            return View();
        }
    }
}