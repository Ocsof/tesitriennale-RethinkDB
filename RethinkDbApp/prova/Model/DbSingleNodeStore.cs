using prova.Connection;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;  //namespace per il timer 
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace prova.Model
{
    class DbSingleNodeStore: IDbStore
    {
        private static ISingleConnection _connectionFactory;
        private static RethinkDB R = RethinkDB.R;
        private string dbName;
        private Stopwatch stopWatch;

        public DbSingleNodeStore(ISingleConnection connectionFactory)  //IConnectionPooling connectionFactory  // ---> per connessione con un cluster + nodi
        {
            _connectionFactory = connectionFactory;
            this.dbName = connectionFactory.GetNodi().ElementAt(0).Database;
            this.stopWatch = new Stopwatch();
        }

        /***metodo per inizializzare il db, per le insert usare il file .txt nella cartella query ***/
        public void InitializeDatabase()
        {
            // database
            CreateDb(this.dbName);

            // tables
            CreateTable(this.dbName, nameof(Author));
            CreateTable(this.dbName, nameof(Post));

            // indexes
            CreateIndex(this.dbName, nameof(Author), nameof(Author.name));
            CreateIndex(this.dbName, nameof(Post), nameof(Post.author_id));

        }

        public void CreateDb(string dbName)
        {
            this.stopWatch.Start();

            var conn = _connectionFactory.CreateConnection();

            this.stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("Time to connect = " + elapsedTime);

            var exists = R.DbList().Contains(db => db == dbName).Run(conn);
            Console.WriteLine(exists);

            if (!exists)
            {
                R.DbCreate(dbName).Run(conn);
                R.Db(dbName).Wait_().Run(conn);
            }
        }

        public void CreateTable(string dbName, string tableName)
        {
            var conn = _connectionFactory.CreateConnection();
            var exists = R.Db(dbName).TableList().Contains(t => t == tableName).Run(conn);
            if (!exists)
            {
                R.Db(dbName).TableCreate(tableName).Run(conn);
                R.Db(dbName).Table(tableName).Wait_().Run(conn);
            }
        }

        public void CreateIndex(string dbName, string tableName, string indexName)
        {
            var conn = _connectionFactory.CreateConnection();
            var exists = R.Db(dbName).Table(tableName).IndexList().Contains(t => t == indexName).Run(conn);
            if (!exists)
            {
                R.Db(dbName).Table(tableName).IndexCreate(indexName).Run(conn);
                R.Db(dbName).Table(tableName).IndexWait(indexName).Run(conn);
            }
        }

        public void Reconfigure(int shards, int replicas)
        {
            var conn = _connectionFactory.CreateConnection();
            var tables = R.Db(this.dbName).TableList().Run(conn);
            foreach (string table in tables)
            {
                R.Db(this.dbName).Table(table).Reconfigure().OptArg("shards", shards).OptArg("replicas", replicas).Run(conn);
                R.Db(this.dbName).Table(table).Wait_().Run(conn);
            }
        }

        /*** Sfrutto l'indice che ho costruito su name per effettuare ricerche veloci, se cè gia' lo stesso name allora fa update ***/
        /**Possibile Bug ----> invece che RunResult mi ha fatto mettere RunWrite perchè è obsoleto **/
        public void InsertOrUpdateAuthor(Author author)
        {
            var conn = _connectionFactory.CreateConnection();

            Cursor<Author> all = R.Db(this.dbName).Table(nameof(Author))
                .GetAll(author.name)[new { index = nameof(author.name) }]
                .Run<Author>(conn);

            var authors = all.ToList();

            if (authors.Count > 0)
            {
                // update
                R.Db(this.dbName).Table(nameof(Author)).Get(authors.First().id).Update(author).RunWrite(conn);
                //return authors.First().id.ToString();
            }
            else
            {
                // insert
                var result = R.Db(this.dbName).Table(nameof(Author))
                    .Insert(author)
                    .RunWrite(conn);
                //return result.GeneratedKeys.First().ToString();
            }
        }

        //valutare se far fare l'update or not ---> dettagli :)
        public void InsertOrUpdatePost(Post post)
        {
            var conn = _connectionFactory.CreateConnection();
            Cursor<Post> all = R.Db(this.dbName).Table(nameof(Post))
                .GetAll(post.id)//[new { index = nameof(Post.title) }]
                .Run<Post>(conn);

            var posts = all.ToList();

            if (posts.Count > 0)
            {
                // update
                R.Db(this.dbName).Table(nameof(Post)).Get(posts.First().id).Update(post).RunWrite(conn);
                //return posts.First().id.ToString();
            }
            else
            {
                // insert
                var result = R.Db(this.dbName).Table(nameof(Post))
                    .Insert(post)
                    .RunWrite(conn);
                //return result.GeneratedKeys.First().ToString();
            }
        }

        public List<AuthorStatus> GetAuthorsStatus()
        {
            var conn = _connectionFactory.CreateConnection();
            Cursor<Author> all = R.Db(dbName).Table(nameof(Author)).RunCursor<Author>(conn);

            this.stopWatch.Start();
            var list = all.OrderBy(f => f.id)
                .Select(f => new AuthorStatus
                {
                    name = f.name,
                    age = f.age,
                    id = f.id, //è quello che uso qui sotto per trovare i post che ha fatto
                    hobby = f.hobby,
                    totalPostsMade = R.Db(dbName).Table(nameof(Post))
                            .GetAll(f.id)[new { index = nameof(Post.author_id) }]  //è per questo che gli creo un indice --> per velocizzare, Testare se è effettivamente + veloce
                            .Count()
                            .Run<long>(conn)
                }).ToList();

            this.stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("Time to get authors status = " + elapsedTime);

            return list;
        }


        public void MultiInsertPosts()
        {
            var conn = _connectionFactory.CreateConnection();
            int author_id = 0;

            var id = R.Db(this.dbName).Table(nameof(Post)).Count().Run(conn) + 1; //id ultimo elem + 1

            this.stopWatch.Start();
            for (var i = 0; i < 50; i++)
            {
                Post post = new Post
                {
                    id = id,
                    author_id = author_id,
                    title = this.createRandomString(),
                    content = this.createRandomString()
                };
                this.InsertOrUpdatePost(post);
                id++;
                author_id++;
            }
            this.stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("Time to multi insert = " + elapsedTime);
        }

        public void MultiDeletePosts()
        {
            var conn = _connectionFactory.CreateConnection();
            var id = R.Db(this.dbName).Table(nameof(Post)).Count().Run(conn);  //id dell'ultimo elemento

            this.stopWatch.Start();
            for (var i = 0; i < 50; i++)
            {

                var result = R.Db(this.dbName).Table(nameof(Post))
                .Get(id).Delete().Run(conn);
                id--;
            }
            this.stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("Time to multi delete = " + elapsedTime);
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
