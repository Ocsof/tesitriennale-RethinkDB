using System;
using System.Collections.Generic;
using System.Text;

namespace Rethink.Model
{
    class DbOptions
    {
        //Per single connection 
        /*
        public string Host { get; set; }
        public int Port { get; set; }
        */
        public string HostPort { get; set; }

        //public string Host { get; set; }

        //public int Port { get; set; }
        public string Database { get; set; }
        public int Timeout { get; set; }
    }
}
