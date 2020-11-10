﻿using Newtonsoft.Json.Linq;
using Rethink.Connection;
using Rethink.Model;
using RethinkDb.Driver;
using RethinkDb.Driver.Model;
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
    class RXNotifier : IRXNotifier
    {
        private readonly IConnectionNodes rethinkDbConnection;
        private readonly string dbName;
        private readonly IDbManager rethinkDbStore;
        private readonly static RethinkDB R = RethinkDB.R;

        public RXNotifier(IDbManager rethinkDbStore, IConnectionNodes rethinkDbConnection) //IConnectionPooling
        {
            this.rethinkDbStore = rethinkDbStore;
            this.rethinkDbConnection = rethinkDbConnection;
            this.dbName = this.rethinkDbConnection.GetNodi().ElementAt(0).Database;
        }

        public void ListenNotifications<T>() where T : Notification
        {
            var onCompleted = 0;
            var onError = 0;
            var onNext = 0;
            

            var conn = this.rethinkDbConnection.GetConnection();

            var changes = R.Db(dbName).Table(nameof(Notification))
                //.changes()[new {include_states = true, include_initial = true}]
                .Changes()//[new { include_states = true }]
                .RunChanges<T>(conn); //Notification

            //changes.IsFeed.Should().BeTrue();

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
            //Author? oldValue = obj.OldValue;
            //T? oldValue = obj.OldValue;
            var oldValue = obj.OldValue;
         
            //obj.Dump();
            onNext++;
            Console.WriteLine("New Value: " + obj.NewValue.ToString());
            if(oldValue != null) { 
                Console.WriteLine("Old Value: " + oldValue.ToString());
            }

        }
    }
}