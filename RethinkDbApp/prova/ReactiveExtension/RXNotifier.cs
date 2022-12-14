using Rethink.Connection;
using Rethink.Model;
using RethinkDb.Driver;
using RethinkDb.Driver.Model;
using RethinkDb.Driver.Net;
using RethinkDbApp.Exception;
using RethinkDbApp.Model;
using RethinkDbApp.ReactiveExtension;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

namespace Rethink.ReactiveExtension
{
    /// <summary>
    /// Implementazione di IRXNotifier che interroga la tabella di sistema "INotificationsManager.TABLE";
    /// </summary>
    class RXNotifier<T> : IRXNotifier<T> where T: Notification
    {
        private readonly IConnectionNodes rethinkDbConnection;
        private readonly string dbName;
        private readonly static RethinkDB R = RethinkDB.R;
        private readonly ConcurrentDictionary<Guid, Cursor<Change<T>>> changesDict;  //dizionario Thread safe
        private readonly string tableName;

        public RXNotifier(IConnectionNodes rethinkDbConnection) //IConnectionPooling
        {
            this.rethinkDbConnection = rethinkDbConnection;
            this.dbName = this.rethinkDbConnection.GetNodi().ElementAt(0).Database;
            changesDict = new ConcurrentDictionary<Guid, Cursor<Change<T>>>();
            this.tableName = INotificationsManager.TABLE;
        }


        public NotificationSubscription<T> ListenWithOneOfTheArguments(params string[]  argsList)
        {      
            var conn = this.rethinkDbConnection.GetConnection();
            
            var changes = R.Db(dbName).Table(this.tableName)
             .Filter(notification => 
                R.Expr(R.Array(argsList.ToArray())).Contains(notification.G("Arg"))
             )
             .Changes()
             .RunChanges<T>(conn);

            NotificationSubscription<T> pair = new NotificationSubscription<T>(Guid.NewGuid(), changes.ToObservable());
            if(changesDict.TryAdd(pair.Guid, changes))
            {
                return pair;
            }
            //se non riesce ad aggiungere al dizionario perchè Guid già presente:
            throw new NewGuidException();
        }


        public void StopListening(Guid guid)
        {           
            if (this.changesDict.TryGetValue(guid, out Cursor<Change<T>> change))
            {
                change.Close(); //chiude la listening
                Thread.Sleep(3000);
            }
            else //se non entra non trova il change con guid specificato:
            {
                throw new GetGuidException();
            }          
        }

        public void StopListening(NotificationSubscription<T> pair)
        {
            if (this.changesDict.TryGetValue(pair.Guid, out Cursor<Change<T>> change))
            {
                change.Close(); //chiude la listening
                Thread.Sleep(3000);
            }
            else //se non entra non trova il change con guid specificato:
            {               
                throw new GetGuidException();
            }          
        }
    }
     
}
