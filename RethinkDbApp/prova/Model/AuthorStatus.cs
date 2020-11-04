using System;
using System.Collections.Generic;
using System.Text;

namespace Rethink
{
    class AuthorStatus
    {
        public string name { get; set; }
        public int id { get; set; }
        public int age { get; set; }
        public string hobby { get; set; }
        public long totalPostsMade { get; set; }
        public override String ToString()
        {
            string result = id.ToString() + "  " + name + "  " + age.ToString() + "  " + hobby + "  " + totalPostsMade;
            return result;
        }
            
    }
}
