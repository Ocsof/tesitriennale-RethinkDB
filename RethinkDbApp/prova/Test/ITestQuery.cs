using System;
using System.Collections.Generic;
using System.Text;

namespace Rethink.Test
{
    interface ITestQuery
    {
        /// <summary>
        /// Stampa il numero di post fatti da ogni autore
        /// </summary>
        public void PrintAuthorStatus();
    }
}
