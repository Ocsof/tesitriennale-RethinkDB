using System;

namespace RethinkDbApp.Exception
{
    [Serializable]
    class TableNotFoundException : System.Exception
    {
        private readonly static string message = "Tabella ";
        private readonly static string message2 = " non trovata sul db";
        public TableNotFoundException(string table) : base(message + table + message2)
        {

        }
    }
}
