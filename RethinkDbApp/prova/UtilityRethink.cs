using Rethink.Connection;
using Rethink.Model;
using Rethink.ReactiveExtension;
using RethinkDb.Driver.Ast;
using RethinkDb.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rethink
{
    public class UtilityRethink : IUtilityRethink
    {
        private readonly IList<DbOptions> listNodi;
        private readonly static RethinkDB R = RethinkDB.R;
        private readonly IConnectionNodes rethinkDbConnection;
        private readonly IDbStore rethinkDbStore;
        private readonly IManageNotifications manageNotifications;
        private readonly IRXTest rxTest;


        /// <summary>
        /// Connettere l'app al cluster Rethinkdb in esecuzione
        /// </summary>
        /// <param name="dbName">Nome del Db a cui ci si vuole connettere, se non esiste viene creato</param>
        /// <param name="hostsPorts">Lista di stringhe del tipo: "indirizzoip:porta"</param>
        public UtilityRethink(string dbName, IList<String> hostsPorts)
        {
            
            
            foreach (String hostPort in hostsPorts)
            {
                this.listNodi.Add(new DbOptions { Database = dbName, HostPort = hostPort, Timeout = 60 });
            }
            this.rethinkDbConnection = new ConnectionNodes(listNodi);
            this.rethinkDbStore = new DbStore(rethinkDbConnection);
            this.manageNotifications = new ManageNotifications(rethinkDbConnection);
            this.rxTest = new RXTest(rethinkDbStore, rethinkDbConnection);
            this.CreateDb(dbName);
        }

        private void CreateDb(string dbName)
        {
            var conn = rethinkDbConnection.GetConnection();
            var exists = R.DbList().Contains(db => db == dbName).Run(conn);
            if (!exists)
            {
                R.DbCreate(dbName).Run(conn);
                R.Db(dbName).Wait_().Run(conn);
            }
        }

        public IDbStore ManageDb()
        {
            return this.rethinkDbStore;
        }

        public IManageNotifications ManageNotifications()
        {
            return this.ManageNotifications();
        }

        public void RegisterToNotifications()
        {        
            this.rxTest.basic_change_feed_with_reactive_extensions();
        }
   
        public void CloseConnection()
        {
            rethinkDbConnection.CloseConnection();
        }

    }
}
