using Newtonsoft.Json.Linq;
using prova.Connection;
using prova.Model;
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


namespace prova.ReactiveExtension
{
    class RXSingleNodeTests : IRXTest
    {
        private static ISingleConnection _connectionFactory;
        private string dbName;
        private Stopwatch stopWatch;
        private IDbStore rethinkDbStore;
        private static RethinkDB R = RethinkDB.R;

        public RXSingleNodeTests(IDbStore rethinkDbStore, ISingleConnection _connectionF) //IConnectionPooling
        {
            this.rethinkDbStore = rethinkDbStore;
            _connectionFactory = _connectionF;
            this.dbName = _connectionFactory.GetNodi().ElementAt(0).Database;
            this.stopWatch = new Stopwatch();

        }

        public void basic_change_feed_with_reactive_extensions()
        {
            var onCompleted = 0;
            var onError = 0;
            var onNext = 0;

            var conn = _connectionFactory.CreateConnection();

            var changes = R.Db(dbName).Table(nameof(Author))
                //.changes()[new {include_states = true, include_initial = true}]
                .Changes()[new { include_states = true }]
                .RunChanges<Author>(conn);

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


            //Next simulate 3 inserts into Author table.
            Thread.Sleep(3000);

            Task.Run(() =>
            {
                rethinkDbStore.InsertOrUpdateAuthor(new Author
                {
                    id = 101,
                    name = "Gogeta",
                    hobby = "Pallamano",
                    age = 22
                });
            });

            Thread.Sleep(3000);

            Task.Run(() =>
            {
                rethinkDbStore.InsertOrUpdateAuthor(new Author
                {
                    id = 102,
                    name = "Gogetaho",
                    hobby = "Pallamano",
                    age = 22
                });
            });

            Thread.Sleep(3000);

            Task.Run(() =>
            {
                rethinkDbStore.InsertOrUpdateAuthor(new Author
                {
                    id = 101,
                    name = "Gogeta",
                    hobby = "Pallaman",
                    age = 22
                });
            });

            Thread.Sleep(3000);

            changes.Close();

            Thread.Sleep(3000);

            /*
            onCompleted.Should().Be(1);
            onNext.Should().Be(3);
            onError.Should().Be(0);
            */
            Console.WriteLine($"Next: {onNext} Completed: {onCompleted}");
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

        private void OnNext(Change<Author> obj, ref int onNext)
        {
            Console.WriteLine("On Next");
            //obj.Dump();
            onNext++;
            /*
            Console.WriteLine(obj.NewValue.ToString());
            Console.WriteLine(obj.State.ToString());
            Console.WriteLine(obj.ExtraData.ToString());
            */

        }
    }
}
