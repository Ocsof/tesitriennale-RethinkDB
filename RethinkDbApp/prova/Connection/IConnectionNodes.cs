using Rethink.Model;
using RethinkDb.Driver.Net;
using System.Collections.Generic;

namespace Rethink.Connection
{
    interface IConnectionNodes
    {
        /// <summary>
        /// Crea nuova connessione verso il server, se dopo 20 secondi non si è riuscito a connettere viene segnalato un'errore
        /// </summary>
        /// <returns></returns>
        public IConnection GetConnection();

        /// <summary>
        /// Chiusura connessione
        /// </summary>
        public void CloseConnection();

        /// <summary>
        /// Ritorna i Nodi Del Cluster
        /// </summary>
        /// <returns></returns>
        public IList<DbOptions> GetNodi();
     
    }
}
