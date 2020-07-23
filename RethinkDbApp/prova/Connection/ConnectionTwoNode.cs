using prova.Model;
using RethinkDb.Driver.Net.Clustering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace prova.Connection
{
    /*** classe che eredita dalla RethinkDbClusterConnection ***/
    class ConnectionTwoNode : ConnectionCluster
    {
        private ConnectionPool conn;

        public ConnectionTwoNode(IList<DbOptions> listNodi)
            : base(listNodi)
        {
            
        }

        public override ConnectionPool CreateConnection()
        {
            if (conn == null)
            {
                //Ok per single connection
                var R = RethinkDb.Driver.RethinkDB.R; 
                this.conn = R.ConnectionPool()
                        .Seed(new[] { this.GetNodi().ElementAt(0).HostPort, this.GetNodi().ElementAt(1).HostPort })
                        .PoolingStrategy(new RoundRobinHostPool())
                        .Discover(true)
                        .Connect();

                var result = R.Now().Run<DateTimeOffset>(conn);
            }

            if (!conn.AnyOpen)
            {
                //conn.Reconnect();
                conn = null;
                this.CreateConnection();
            }

            return conn;
        }

    }
}
