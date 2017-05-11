using insta_001.Models;
using insta_001.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace insta_001.Controllers
{
    public class InstController : Controller
    {
        // GET: Inst
        public ActionResult Index(bool isMyAcc = false)
        {
            InstParser p = new InstParser();
            List<InstModel> comms = new List<InstModel>();

            if (isMyAcc == true)
            {
                comms = p.Main(true);
            }
            else
            {
                comms = p.Main();
            }

            if (comms != null)
            {
                comms = SortByCommCreated(comms);
                return PartialView("Index", comms);
            }

            else return PartialView(new List<InstModel>());
        }

        [HttpPost]
        public ActionResult AjaxSortByPostCreated(List<InstModel> comms = null)
        {
            var val2 = Request.UrlReferrer.Query;

            InstParser p = new InstParser();
            comms = new List<InstModel>();

            if (val2 == "? isMyAcc = True")
            {
                comms = p.Main(true);
            }
            else
            {
                comms = p.Main();
            }
            if (comms != null)
            {
                comms = comms.Where(ev => ev.comments.Count() > 0).OrderByDescending(ev => ev.info.created).ToList();
            }

            return PartialView("AjaxUpdate", comms);
        }

        [HttpPost]
        public ActionResult AjaxSortByCommCreated(List<InstModel> comms = null)
        {
            var val2 = Request.UrlReferrer.Query;

            InstParser p = new InstParser();
            comms = new List<InstModel>();

            if (val2 == "? isMyAcc = True")
            {
                comms = p.Main(true);
            }
            else
            {
                comms = p.Main();
            }
            comms = SortByCommCreated(comms);
            return PartialView("AjaxUpdate", comms);
        }
        public ActionResult GetInstDataJson(InstModel model)
        {
            return Json(model);
            // return Json(new { success = true });
        }


        public List<InstModel> SortByCommCreated(List<InstModel> comms)
        {
            //comms = comms.Where(ev => ev.comments.Count > 0).ToList();
            for (int i = 0; i < comms.Count(); i++)
            {
                for (int j = 0; j < comms.Count() - i - 1; j++)
                {
                    if (comms[j].comments.Last().created < comms[j + 1].comments.Last().created)
                    {
                        var temp = comms[j];
                        comms[j] = comms[j + 1];
                        comms[j + 1] = temp;
                    }
                }
            }
            return comms;
        }
    }
}