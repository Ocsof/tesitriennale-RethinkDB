using Rethink.Connection;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;  //namespace per il timer 
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Rethink.Model
{
    class DbStore : IDbStore
    {
        private readonly IConnectionNodes rethinkDbConnection;
        private readonly static RethinkDB R = RethinkDB.R;
        private readonly string dbName;

        public DbStore(IConnectionNodes rethinkDbConnection)  //IConnectionPooling connectionFactory  // ---> per connessione con un cluster + nodi
        {
            this.rethinkDbConnection = rethinkDbConnection;
            this.dbName = rethinkDbConnection.GetNodi().ElementAt(0).Database;
            this.CreateDb(this.dbName);
        }

        public void CreateDb(string dbName)
        {
            var conn = rethinkDbConnection.GetConnection();
            var exists = R.DbList().Contains(db => db == dbName).Run(conn);
            if (!exists)
            {
                R.DbCreate(dbName).Run(conn);
                R.Db(dbName).Wait_().Run(conn);
            }
        }

        public void CreateTable(string tableName)
        {
            var conn = this.rethinkDbConnection.GetConnection();
            var exists = R.Db(dbName).TableList().Contains(t => t == tableName).Run(conn);
            if (!exists)
            {
                R.Db(dbName).TableCreate(tableName).Run(conn);
                R.Db(dbName).Table(tableName).Wait_().Run(conn);
            }
        }

        public void CreateIndex(string tableName, string indexName)
        {
            var conn = rethinkDbConnection.GetConnection();
            var exists = R.Db(dbName).Table(tableName).IndexList().Contains(t => t == indexName).Run(conn);
            if (!exists)
            {
                R.Db(dbName).Table(tableName).IndexCreate(indexName).Run(conn);
                R.Db(dbName).Table(tableName).IndexWait(indexName).Run(conn);
            }
        }

        public void Reconfigure(int shards, int replicas)
        {
            var conn = rethinkDbConnection.GetConnection();
            var tables = R.Db(this.dbName).TableList().Run(conn);
            foreach (string table in tables)
            {
                R.Db(this.dbName).Table(table).Reconfigure().OptArg("shards", shards).OptArg("replicas", replicas).Run(conn);
                R.Db(this.dbName).Table(table).Wait_().Run(conn);
            }
        }

       

      

        public void MultiInsertNotifications()
        {
            var conn = rethinkDbConnection.GetConnection();
            int author_id = 0;

            var id = R.Db(this.dbName).Table(nameof(Notification)).Count().Run(conn) + 1; //id ultimo elem + 1

            for (var i = 0; i < 50; i++)
            {
                Notification notification = new Notification
                {
                    Id = id,
                    Text = this.createRandomString()
                };
                this.InsertOrUpdateNotification(notification.Id, notification.Text);
                id++;
                author_id++;
            }
        }

        public void MultiDeleteNotifications()
        {         
            var conn = rethinkDbConnection.GetConnection();
            var id = R.Db(this.dbName).Table(nameof(Notification)).Count().Run(conn);  //id dell'ultimo elemento
          
            for (var i = 0; i < 50; i++)
            {
                
                var result = R.Db(this.dbName).Table(nameof(Notification))
                .Get(id).Delete().Run(conn);
                id--;
            }
        }


        /***Generatore di stringhe usato dalla MultiInsert ***/

        private String createRandomString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars); 
        }

    }
}
