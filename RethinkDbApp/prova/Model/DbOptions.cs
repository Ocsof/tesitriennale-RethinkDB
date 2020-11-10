using System;
using System.Collections.Generic;
using System.Text;

namespace Rethink.Model
{
    class DbOptions
    {
        /// <summary>
        /// Stringa del tipo: "indirizzoip:porta"
        /// </summary>
        public string HostPort { get; set; }

        /// <summary>
        /// Nome database
        /// </summary>
        public string Database { get; set; }
        public int Timeout { get; set; }
    }
}
