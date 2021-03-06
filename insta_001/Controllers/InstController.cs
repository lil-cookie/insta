﻿using insta_001.Models;
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
                comms = comms.Where(ev => ev.comments.Count > 0).OrderByDescending(ev => ev.info.created).ToList();
                // IEnumerable<Data> data = comms.Where(ev => ev.comments.Count > 0).OrderByDescending(ev => ev.info.createdAt);//.OrderBy(ev => ev.postAuthor)
                //  comms.Remove(comms.Where(ev => ev.comments.Count == 0));// = (List<Data>)comms.Where);

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
                return View(comms);
            }
            else return View(new List<InstModel>());
        }


    }
}