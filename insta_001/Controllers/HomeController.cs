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
        public ActionResult AjaxUpdate(string user = "")
        {
            FileWorker fw = new FileWorker();
            bool? upd = fw.WriteFile(user.Trim());
            string result;

            if (upd == null)
            {
                result = "Unforchenatly, error occurred during a operation. Try again.";
            }
            else if (upd==true)
            {
                result = "User "+ user + " added.";
            }
            else
            {
                result = "User " + user + " deleted.";
            }
            fw = new FileWorker();
            ViewBag.users = fw.ReadInstUsernames();
            /*  if (Request.IsAjaxRequest())
              {
                  user = "rrrrrrrrrrr";
              }
              else user = "eeeeeeeeee";*/
            return PartialView("AjaxUpdate", result);
        }
    }
}