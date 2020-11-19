using System;
using System.Collections.Generic;
using System.Text;

namespace RethinkDbApp.Exception
{
    [Serializable]
    class DeleteTableSystemException : System.Exception
    {
        private readonly static string message = "Impossibile eliminare la tabella di sistema ";
        public DeleteTableSystemException(string table) : base(message + table)
        {

        }
    }
}
