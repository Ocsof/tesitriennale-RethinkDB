using Newtonsoft.Json.Linq;
using Rethink.Connection;
using Rethink.Model;
using RethinkDb.Driver;
using RethinkDb.Driver.Ast;
using RethinkDb.Driver.Model;
using RethinkDb.Driver.Net;
using System;
using System.Collections.Concurrent;
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
        ConcurrentDictionary<Guid, Cursor<Change<T>>> changesDict;

        public RXNotifier(IConnectionNodes rethinkDbConnection) //IConnectionPooling
        {
            this.rethinkDbConnection = rethinkDbConnection;
            this.dbName = this.rethinkDbConnection.GetNodi().ElementAt(0).Database;
        }


        public IObservable<Change<T>> ListenWithOneOfTheArguments(params string[]  argsList)
        {      
            var conn = this.rethinkDbConnection.GetConnection();
            
            var changes = R.Db(dbName).Table("Notifications")
             .Filter(notification => 
                R.Expr(R.Array(argsList.ToArray())).Contains(notification.G("Arg"))
             )
             .Changes()
             .RunChanges<T>(conn);

            Guid guid = Guid.NewGuid();
            changesDict.TryAdd(guid, changes);  //mettere l'if        
          
            return changes.ToObservable();
        }


        public void StopListening(Guid guid)
        {
            if(this.changesDict.TryGetValue(guid, out IObservable<Change<T>> change))
            {
                change.Close(); //chiude la listening
                Thread.Sleep(3000);
            }

        }

    }
     
}
