using Rethink.Connection;
using Rethink.Model;
using Rethink.ReactiveExtension;
using RethinkDb.Driver.Ast;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rethink
{
    public class UtilityRethink : IUtilityRethink
    {
        private readonly IList<DbOptions> listNodi;
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
            this.rethinkDbStore.CreateDb(dbName);
        }

        public void CreateTable(String table)
        {
            this.rethinkDbStore.CreateTable(table);
        }
    
        public void RegisterToNotifications()
        {        
            this.rxTest.basic_change_feed_with_reactive_extensions();
        }

        public int GetIdLastNotification()
        {
            return this.manageNotifications.GetIdLastNotification();
        }

        public int GetLastNotificationExecution()
        {
            return this.manageNotifications.GetIdLastNotificationExecution();
        }

        public void NewNotification<T>(T notification) where T : Notification
        {
            this.manageNotifications.NewNotification(notification);
        }

        public T GetNotification<T>(int id) where T : Notification
        {
            return this.manageNotifications.GetNotification(id);
        }

        public IList<T> GetNotifications<T>(Date date) where T : Notification
        {
            throw new NotImplementedException();
        }

        public IList<T> GetNotifications<T>(string text) where T : Notification
        {
            throw new NotImplementedException();
        }

        public void CloseConnection()
        {
            rethinkDbConnection.CloseConnection();
        }

    }
}
