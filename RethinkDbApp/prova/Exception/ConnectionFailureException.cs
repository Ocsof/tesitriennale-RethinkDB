using System;

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
