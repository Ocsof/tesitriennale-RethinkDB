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
        private readonly static RethinkDB R = RethinkDB.R;
        private readonly IConnectionNodes connection;
        private readonly IDbManager dbStore;
        private readonly INotificationsManager manageNotifications;
        private readonly IRXNotifier rxNotifier;


        /// <summary>
        /// Connettere l'app al cluster Rethinkdb in esecuzione
        /// </summary>
        /// <param name="dbName">Nome del Db a cui ci si vuole connettere, se non esiste viene creato</param>
        /// <param name="hostsPorts">Lista di stringhe del tipo: "indirizzoip:porta"</param>
        public UtilityRethink(string dbName, IList<String> hostsPorts)
        {

            IList<DbOptions> listNodi = new List<DbOptions>();
            foreach (String hostPort in hostsPorts)
            {
                listNodi.Add(new DbOptions { Database = dbName, HostPort = hostPort, Timeout = 60 });
            }
            this.connection = new ConnectionNodes(listNodi);
            this.dbStore = new DbManager(this.connection);
            this.manageNotifications = new NotificationsManager(this.connection);
            this.rxNotifier = new RXNotifier(this.dbStore, this.connection);
            this.CreateDb(dbName);
        }

        private void CreateDb(string dbName)
        {
            var conn = this.connection.GetConnection();
            var exists = R.DbList().Contains(db => db == dbName).Run(conn);
            if (!exists)
            {
                R.DbCreate(dbName).Run(conn);
                R.Db(dbName).Wait_().Run(conn);
            }
        }

        public IDbManager GetDbManager()
        {
            return this.dbStore;
        }

        public INotificationsManager GetNotificationsManager()
        {
            return this.manageNotifications;
        }

        public IRXNotifier GetNotifier()
        {
            return this.rxNotifier;
        }
   
        public void CloseConnection()
        {
            this.connection.CloseConnection();
        }

    }
}
