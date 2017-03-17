using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace insta_001.Models
{
    public partial class Data
    {
        public Data(string link, List<Comment> comments)
        {
            this.link = link;
            this.comments = comments;
        }
            public string link { get; set; }
            public List<Comment> comments { get; set; }

    }
}