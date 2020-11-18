using Newtonsoft.Json.Linq;
using Rethink.Connection;
using Rethink.Model;
using RethinkDb.Driver;
using RethinkDb.Driver.Ast;
using RethinkDb.Driver.Model;
using RethinkDb.Driver.Net;
using RethinkDbApp.Exception;
using RethinkDbApp.ReactiveExtension;
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
        //private  Cursor<Change<T>> changes;
        private readonly ConcurrentDictionary<Guid, Cursor<Change<T>>> changesDict;  //dizionario Thread safe

        public RXNotifier(IConnectionNodes rethinkDbConnection) //IConnectionPooling
        {
            this.rethinkDbConnection = rethinkDbConnection;
            this.dbName = this.rethinkDbConnection.GetNodi().ElementAt(0).Database;
            changesDict = new ConcurrentDictionary<Guid, Cursor<Change<T>>>();
        }


        public NotificationSubscription<T> ListenWithOneOfTheArguments(params string[]  argsList)
        {      
            var conn = this.rethinkDbConnection.GetConnection();
            
            var changes = R.Db(dbName).Table("Notifications")
             .Filter(notification => 
                R.Expr(R.Array(argsList.ToArray())).Contains(notification.G("Arg"))
             )
             .Changes()
             .RunChanges<T>(conn);

            changes.ToObservable();

            Guid guid = Guid.NewGuid();
            NotificationSubscription<T> pair = new NotificationSubscription<T>(guid, changes.ToObservable());
            if(changesDict.TryAdd(guid, changes))
            {
                return pair;
            }
            //se non riesce ad aggiungere al dizionario perchè Guid già presente:
            throw new NewGuidException();
        }


        public void StopListening(Guid guid)
        {
            if(this.changesDict.TryGetValue(guid, out Cursor<Change<T>> change))
            {
                change.Close(); //chiude la listening
                Thread.Sleep(3000);
            }
            //valutare se tirare eccezione se non entra nell'if
            throw new GetGuidException();
        }

        public void StopListening(NotificationSubscription<T> pair)
        {
            if (this.changesDict.TryGetValue(pair.Guid, out Cursor<Change<T>> change))
            {
                change.Close(); //chiude la listening
                Thread.Sleep(3000);
            }
            //valutare se tirare eccezione se non entra nell'if
        }
    }
     
}
