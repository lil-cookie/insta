using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace insta_001.Models
{
    public class InstPostModel
    {
        public InstPostModel(string postLink, string picSrc, DateTime created, string postAuthor)
        {
            this.created = created;
            this.postLink = postLink;
            this.picSrc = picSrc;
            this.postAuthor = postAuthor;
        }
        public string postLink { get; set; }
        public string picSrc { get; set; }
        public DateTime created { get; set; }
        public String postAuthor { get; set; }
    }
}