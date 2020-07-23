using Microsoft.Extensions.Options;
using prova.Model;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;
using RethinkDb.Driver.Net.Clustering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace prova.Connection
{
    class ConnectionCluster : IConnectionPooling
    {
        //private RethinkDB R = RethinkDB.R;
        private ConnectionPool conn;
        private IList<DbOptions> listNodi;


        public ConnectionCluster(IList<DbOptions> listNodi)
        {
            this.listNodi = listNodi;
        }

        public virtual ConnectionPool CreateConnection()
        {
            if (conn == null)
            {
                //Ok per single connection
                var R = RethinkDb.Driver.RethinkDB.R; 
                
                this.conn = R.ConnectionPool()
                        .Seed(new[] { this.listNodi.ElementAt(0).HostPort, this.listNodi.ElementAt(1).HostPort, listNodi.ElementAt(2).HostPort, listNodi.ElementAt(3).HostPort, listNodi.ElementAt(4).HostPort })
                        .PoolingStrategy(new RoundRobinHostPool())
                        .Discover(true)
                        .Connect();
         

                var result = R.Now().Run<DateTimeOffset>(conn);  // forse è da togliere
            }

            if (!conn.AnyOpen)
            {
                //conn.Reconnect();
                conn = null;
                this.CreateConnection();
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
