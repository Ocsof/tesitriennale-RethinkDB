using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace RethinkDbApp.Exception
{
    [Serializable]
    public class ConnectionFailureException : System.Exception
    {
        private readonly static string message = "Connessione fallita, server non trovato";
        public ConnectionFailureException() : base(message)
        {

        }
    }
}
