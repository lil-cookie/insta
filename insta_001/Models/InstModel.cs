﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace insta_001.Models
{
    public partial class InstModel
    {
        public InstModel(InstPostModel info, List<InstCommModel> comments)
        {

            this.info = info;
            this.comments = comments;
        }
        public InstModel() { }
        public InstPostModel info { get; set; }
        public IEnumerable<InstCommModel> comments { get; set; }


    }
   
}