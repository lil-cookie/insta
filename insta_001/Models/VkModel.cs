using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace insta_001.Models
{
    public class VkModel
    {
        public string authorId { get; set; }
        public string authorName { get; set; }
        public string comment { get; set; }
        public string created { get; set; }
        public string authorHref { get; set; }
        public string postHref { get; set; }
        public string postPhotoHref { get; set; }
    }
}