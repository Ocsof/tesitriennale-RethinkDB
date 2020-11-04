using Rethink.Model;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;
using RethinkDb.Driver.Net.Clustering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rethink.Connection
{
    interface IConnectionNodes
    {
        /// <summary>
        /// Crea nuova connessione verso il server
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
