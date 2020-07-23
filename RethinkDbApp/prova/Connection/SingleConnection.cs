using prova.Model;
using RethinkDb.Driver.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace prova.Connection
{
    class SingleConnection : ISingleConnection
    {
        //private RethinkDB R = RethinkDB.R;
        private RethinkDb.Driver.Net.Connection conn;
        private IList<DbOptions> listNodi;


        public SingleConnection(IList<DbOptions> listNodi)
        {
            this.listNodi = listNodi;
        }

        public RethinkDb.Driver.Net.Connection CreateConnection()
        {
            if (conn == null)
            {
                var R = RethinkDb.Driver.RethinkDB.R; 
     

                this.conn  = R.Connection()
                             .Hostname(this.listNodi.ElementAt(0).Host) // Hostnames and IP addresses work.
                             .Port(this.listNodi.ElementAt(0).Port) // .Port() is optional. Default driver port number.
                             .Timeout(60)
                             .Connect();
                
                
               
                var result = R.Now().Run<DateTimeOffset>(conn);  // forse è da togliere
            }

            if (!conn.Open)
            {
                //conn.Reconnect();
                conn = null;
                this.CreateConnection();
            }

            return conn;
        }

        public void CloseConnection()
        {
            if (conn != null && conn.Open)
            {

                // conn.Close(false);
                conn.Close();
            }
        }

        public IList<DbOptions> GetNodi()
        {
            return this.listNodi;
        }


    }
}
