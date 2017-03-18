using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace insta_001.Models
{
    public class PostInfo
    {
        public PostInfo(string postLink, string picSrc, String createdAt)
        {
            this.createdAt = createdAt;
            this.postLink = postLink;
            this.picSrc = picSrc;
        }
        public string postLink { get; set; }
        public string picSrc { get; set; }
        public String createdAt { get; set; }
    }
}