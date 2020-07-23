using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace prova
{
    class Author
    {

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]  //penso sia qualcosa relativo al fatto che id no null
        public int id { get; set; }
        public string name { get; set; }
        public int age { get; set; }
        public string hobby { get; set; }

        public override String ToString()
        {
            string result = id.ToString() + "  " + name + "  " + age.ToString() + "  " + hobby;
            return result;
        }
    }
}
