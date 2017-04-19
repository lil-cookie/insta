using insta_001.Models;
using insta_001.Parser;
using System.Web.Mvc;

namespace insta_001.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(InstUserModel user)
        {
            FileWorker fw = new FileWorker();
            fw.WriteFile(user.username.Trim());
            return View();
        }

    }
}