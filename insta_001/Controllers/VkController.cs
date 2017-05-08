using insta_001.Models;
using insta_001.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace insta_001.Controllers
{
    public class VkController : Controller
    {
        // GET: Vk
        public ActionResult Index()
        {
            VkParser v = new VkParser();
            List<VkModel> vk = v.Main();
            vk = vk.OrderByDescending(ev => ev.created).ToList();
            return View(vk);
        }
    }
}