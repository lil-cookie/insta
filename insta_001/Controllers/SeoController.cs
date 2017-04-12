using insta_001.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace insta_001.Controllers
{
    public class SeoController : Controller
    {
        // GET: Seo
        public ActionResult Index()
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Parser/urls.txt");
            List<string> redirectUrls =  System.IO.File.ReadLines(path).ToList();

            return View(redirectUrls);
        }
    }
}