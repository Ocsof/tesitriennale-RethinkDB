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

  

    }
}
