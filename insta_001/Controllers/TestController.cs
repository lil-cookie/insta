using insta_001.Models;
using insta_001.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace insta_001.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Update(InstModel model)
        {
            // do something with the model

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AjaxSortByPostCreated(List<InstModel> comms)
        {
            var val = Request.Form;
            var val2 = Request.Url;


            if (comms != null)
            {
                //comms = comms.Where(ev => ev.comments.Count > 0).OrderByDescending(ev => ev.info.created).ToList();
            }
            return PartialView("AjaxUpdate", comms);
        }


        public JsonResult GetAllPosts()
        {
            InstParser p = new InstParser();
            List<InstModel> comms = new List<InstModel>();
            comms = p.Main(true);

            return Json(comms, JsonRequestBehavior.AllowGet);
        }
    }
}