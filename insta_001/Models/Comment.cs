using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace insta_001.Models
{
    public partial  class Comment
    {
        public Comment(string author, string text, string cteated)
        {

            this.author = author;
            this.text = text;
            this.cteated = cteated;
        }

        public string author { get; set; }
        public string text { get; set; }
        public string cteated { get; set; }
    }
}