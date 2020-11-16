using Newtonsoft.Json.Linq;
using Rethink.Connection;
using Rethink.Model;
using RethinkDb.Driver;
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

        public void Listen()
        {
            var onCompleted = 0;
            var onError = 0;
            var onNext = 0;
            
            var conn = this.rethinkDbConnection.GetConnection();

            this.changes = R.Db(dbName).Table("Notifications")
            .Changes()
            .RunChanges<T>(conn);

            var observable = changes.ToObservable();

            //use a new thread if you want to continue,
            //otherwise, subscription will block.
            observable.SubscribeOn(NewThreadScheduler.Default) 
                .Subscribe(
                    x => OnNext(x, ref onNext),
                    e => OnError(e, ref onError),
                    () => OnCompleted(ref onCompleted)
                );
        }

        public void ListenWithArg(string arg)
        {            
            var onCompleted = 0;
            var onError = 0;
            var onNext = 0;

            var conn = this.rethinkDbConnection.GetConnection();

            this.changes = R.Db(dbName).Table("Notifications")
             .Filter( notification => notification.G("Arg").Eq(arg))
             .Changes()
             .RunChanges<T>(conn);

            var observable = changes.ToObservable();

            //use a new thread if you want to continue,
            //otherwise, subscription will block.
            observable.SubscribeOn(NewThreadScheduler.Default)
                .Subscribe(
                    x => OnNext(x, ref onNext),
                    e => OnError(e, ref onError),
                    () => OnCompleted(ref onCompleted)
                );
        }

        public void ListenWithOneOfTheArguments(IList<string> argsList)
        {
            IList<string> list = new List<string>();
            var onCompleted = 0;
            var onError = 0;
            var onNext = 0;

            var conn = this.rethinkDbConnection.GetConnection();

            this.changes = R.Db(dbName).Table("Notifications")
             //.Filter(notification => notification.G("Arg").Eq(arg))
             //.Filter()
             .Changes()
             .RunChanges<T>(conn);

            var observable = changes.ToObservable();

            //use a new thread if you want to continue,
            //otherwise, subscription will block.
            observable.SubscribeOn(NewThreadScheduler.Default)
                .Subscribe(
                    x => OnNext(x, ref onNext),
                    e => OnError(e, ref onError),
                    () => OnCompleted(ref onCompleted)
                );
        }

        public void StopListening()
        {
            this.changes.Close(); //chiude la listening
            Console.WriteLine("Stop Listening");
            Thread.Sleep(3000);
        }

        private void OnCompleted(ref int onCompleted)
        {
            Console.WriteLine("On Completed.");
            onCompleted++;
        }

        private void OnError(Exception obj, ref int onError)
        {
            Console.WriteLine("On Error");
            Console.WriteLine(obj.Message);
            onError++;
        }

        private void OnNext<T>(Change<T> obj, ref int onNext) where T : Notification
        {
            Console.WriteLine("On Next");
            var oldValue = obj.OldValue;

            onNext++;
            Console.WriteLine("New Value: " + obj.NewValue.ToString());
            if (oldValue != null)
            { //nel caso di un update
                Console.WriteLine("Old Value: " + oldValue.ToString());
            }

        }

    }
     
}
