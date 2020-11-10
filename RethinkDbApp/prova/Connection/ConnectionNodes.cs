using Rethink.Model;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;
using RethinkDb.Driver.Net.Clustering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Rethink.Connection
{
    class ConnectionNodes : IConnectionNodes
    {
        //private RethinkDB R = RethinkDB.R;
        private ConnectionPool conn;  //IConnection
        private readonly IList<DbOptions> listNodi;


        public ConnectionNodes(IList<DbOptions> listNodi)
        {
            this.listNodi = listNodi;
        }

        public virtual IConnection GetConnection()
        {
            if (conn == null)
            {
                //Ok per single connection
                var R = RethinkDb.Driver.RethinkDB.R;

                string[] nodi = new string[this.listNodi.Count];
                int position = 0;
                foreach(DbOptions node in listNodi)
                {
                    nodi[position] = node.HostPort;
                    position++;
                }
                Console.WriteLine(nodi.ToString());
                this.conn = R.ConnectionPool()
                        /* .Seed(new[] { this.listNodi.ElementAt(0).HostPort, this.listNodi.ElementAt(1).HostPort, listNodi.ElementAt(2).HostPort, listNodi.ElementAt(3).HostPort, listNodi.ElementAt(4).HostPort 
                                    })
                        */
                        .Seed(nodi)
                        .PoolingStrategy(new RoundRobinHostPool())
                        .Discover(true)
                        .Connect();
         

                var result = R.Now().Run<DateTimeOffset>(conn);  // forse è da togliere
            }

            if (!conn.AnyOpen)
            {
                //conn.Reconnect();
                conn = null;
                this.GetConnection();
            }
            
            return conn;
        }

        public void CloseConnection()
        {
            if (conn != null && conn.AnyOpen)
            {

                // conn.Close(false);
                conn.Shutdown();
            }
        }

        public IList<DbOptions> GetNodi()
        {
            return this.listNodi;
        }


    }
}
