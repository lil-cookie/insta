using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace insta_001.Models
{
    public class UserModel
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public InstPostModel info { get; set; }
        public IEnumerable<InstCommModel> comments { get; set; }
    }
}