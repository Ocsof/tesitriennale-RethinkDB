using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rethink.Model
{
    class Post
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]  //penso sia qualcosa relativo al fatto che id no null
        public long id { get; set; }
        public string title { get; set; }
        public int author_id { get; set; }
        public string content { get; set; }

        public override String ToString()
        {
            string result = id.ToString() + "  " + title + "  " + author_id.ToString() + "  " + content;
            return result;
        }
    }
}
