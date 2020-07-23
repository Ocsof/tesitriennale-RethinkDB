using System;
using System.Collections.Generic;
using System.Text;
using prova.Model;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;

namespace prova.Connection
{
    interface ISingleConnection
    {
        /// <summary>
        /// Crea nuova connessione verso il server
        /// </summary>
        /// <returns></returns>
        public RethinkDb.Driver.Net.Connection CreateConnection();

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
