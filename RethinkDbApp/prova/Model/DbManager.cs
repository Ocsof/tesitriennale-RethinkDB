using Rethink.Connection;
using RethinkDb.Driver;
using System.Linq;

namespace Rethink.Model
{
    class DbManager : IDbManager
    {
        private readonly IConnectionNodes connection;
        private readonly static RethinkDB R = RethinkDB.R;
        private readonly string dbName;

        public DbManager(IConnectionNodes connection)  //IConnectionPooling connectionFactory  // ---> per connessione con un cluster + nodi
        {
            this.connection = connection;
            this.dbName = connection.GetNodi().ElementAt(0).Database;
        }

        public string GetTablesList()
        {
            var conn = this.connection.GetConnection();
            var tableList = R.Db(dbName).TableList().Run(conn);

            return tableList.ToString();
        }

        public void CreateTable(string tableName)
        {
            var conn = this.connection.GetConnection();
            var exists = R.Db(dbName).TableList().Contains(t => t == tableName).Run(conn);
            if (!exists)
            {
                R.Db(this.dbName).TableCreate(tableName).Run(conn);
                R.Db(this.dbName).Table(tableName).Wait_().Run(conn);
            }
        }

        public void DelateTable(string tableName)
        {
            var conn = this.connection.GetConnection();
            var exists = R.Db(this.dbName).TableList().Contains(t => t == tableName).Run(conn);
            if (exists)
            {
                R.Db(this.dbName).TableDrop(tableName).Run(conn);
            }
        }
        public string GetIndexList(string tableName) 
        {
            var conn = this.connection.GetConnection();
            var indexList = R.Db(this.dbName).Table(tableName).IndexList().Run(conn);

            return indexList.ToString();
        }
        public void CreateIndex(string tableName, string indexName)
        {
            var conn = this.connection.GetConnection();
            var exists = R.Db(this.dbName).Table(tableName).IndexList().Contains(t => t == indexName).Run(conn);
            if (!exists)
            {
                R.Db(this.dbName).Table(tableName).IndexCreate(indexName).Run(conn);
                R.Db(this.dbName).Table(tableName).IndexWait(indexName).Run(conn);
            }
        }

        public void DeleteIndex(string tableName, string indexName)
        {
            var conn = this.connection.GetConnection();
            var exists = R.Db(this.dbName).Table(tableName).IndexList().Contains(t => t == indexName).Run(conn);
            if (exists)
            {
                R.Db(this.dbName).Table(tableName).IndexDrop(indexName).Run(conn);
            }
        }

        public void Reconfigure(int shards, int replicas)
        {
            var conn = this.connection.GetConnection();
            var tables = R.Db(this.dbName).TableList().Run(conn);
            foreach (string table in tables)
            {
                R.Db(this.dbName).Table(table).Reconfigure().OptArg("shards", shards).OptArg("replicas", replicas).Run(conn);
                R.Db(this.dbName).Table(table).Wait_().Run(conn);
            }
        }

  

    }
}
