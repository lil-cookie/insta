using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace insta_001.Models
{
    public partial class InstModel
    {
      /*  public InstModel(InstPostModel info, List<InstCommModel> comments)
        {

            this.info = info;
            this.comments = comments;
        }*/

        public InstModel() { }
        public string postLink { get; set; }
        public string picSrc { get; set; }
        public DateTime created { get; set; }
        public String postAuthor { get; set; }
        public string postId { get; set; }

        public IEnumerable<InstCommModel> comments { get; set; }


    }
   
}