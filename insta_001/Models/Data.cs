using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace insta_001.Models
{
    public partial class Data
    {
        public Data(String postAuthor, PostInfo info, List<Comment> comments)
        {

            this.info = info;
            this.comments = comments;
        }
        public PostInfo info { get; set; }
        public List<Comment> comments { get; set; }


    }
   
}