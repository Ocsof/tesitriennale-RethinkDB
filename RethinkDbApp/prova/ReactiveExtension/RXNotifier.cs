using Newtonsoft.Json.Linq;
using Rethink.Connection;
using Rethink.Model;
using RethinkDb.Driver;
using RethinkDb.Driver.Ast;
using RethinkDb.Driver.Model;
using RethinkDb.Driver.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rethink.ReactiveExtension
{
    class RXNotifier<T> : IRXNotifier<T> where T: Notification
    {
        private readonly IConnectionNodes rethinkDbConnection;
        private readonly string dbName;
        private readonly static RethinkDB R = RethinkDB.R;
        private  Cursor<Change<T>> changes;

        public RXNotifier(IConnectionNodes rethinkDbConnection) //IConnectionPooling
        {
            this.rethinkDbConnection = rethinkDbConnection;
            this.dbName = this.rethinkDbConnection.GetNodi().ElementAt(0).Database;
        }

        public IObservable<Change<T>> Listen()
        {           
            var conn = this.rethinkDbConnection.GetConnection();

            this.changes = R.Db(dbName).Table("Notifications")
            .Changes()
            .RunChanges<T>(conn);

            return changes.ToObservable(); //observable          
        }

        public IObservable<Change<T>> ListenWithArg(string arg)
        {            
            var conn = this.rethinkDbConnection.GetConnection();

            this.changes = R.Db(dbName).Table("Notifications")
             .Filter( notification => notification.G("Arg").Eq(arg))
             .Changes()
             .RunChanges<T>(conn);

            return changes.ToObservable();            
        }

        public void ListenWithOneOfTheArguments(IList<string> argsList)
        {
            IList<string> list = new List<string>();

            var conn = this.rethinkDbConnection.GetConnection();
            

            this.changes = R.Db(dbName).Table("Notifications")
             //.Filter(notification => notification.G("Arg").Eq(arg))
             .Filter(notification => 
                R.Expr(R.Array(argsList.ToArray())).Contains(notification.G("Args"))
             )
             .Changes()
             .RunChanges<T>(conn);

            var observable = changes.ToObservable();

        }


        public void StopListening()
        {
            this.changes.Close(); //chiude la listening
            Console.WriteLine("Stop Listening");
            Thread.Sleep(3000);
        }

    }
     
}
