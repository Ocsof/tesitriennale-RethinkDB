using prova.Connection;
using prova.Model;
using prova.ReactiveExtension;
using prova.Test;
using RethinkDb.Driver;
using RethinkDb.Driver.Ast;
using RethinkDb.Driver.Model;
using RethinkDb.Driver.Net;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace prova
{
    class Start
    {
        private static RethinkDB R = RethinkDB.R;
        //static void Main(string[] args)
        static async Task Main(string[] args)
        {
           

            IList<DbOptions> listNodiCluster = new List<DbOptions>() { new DbOptions {Database = "test", HostPort = "192.168.7.47:28016", Timeout = 60 } ,
                                                                              new DbOptions {Database = "test", HostPort = "192.168.7.47:28017", Timeout = 60 } ,
                                                                              new DbOptions {Database = "test", HostPort = "192.168.7.47:28018", Timeout = 60 } ,
                                                                              new DbOptions {Database = "test", HostPort = "192.168.7.47:28019", Timeout = 60 } ,
                                                                              new DbOptions {Database = "test", HostPort = "192.168.7.47:28020", Timeout = 60 }
            };

            IList<DbOptions> listTwoNodi = new List<DbOptions>() { new DbOptions {Database = "test", HostPort = "192.168.7.47:28016", Timeout = 60 } ,
                                                                              new DbOptions {Database = "test", HostPort = "192.168.7.47:28017", Timeout = 60 } 
            };

            IList<DbOptions> listOneNodo = new List<DbOptions>() { new DbOptions { Database = "test", HostPort = "192.168.7.47:28016", Timeout = 60 } };



            //Test Connettività 
            HttpClient client = new HttpClient();
            var resp = await client.GetAsync("http://192.168.7.47:8081");
            Console.WriteLine(resp.StatusCode);

            /*******   Scegliere in base al numero di nodi in esecuzione sul Cluster  *************/
            //IConnectionNodes rethinkDbConnection = new ConnectionNodes(listNodiCluster);
            IConnectionNodes rethinkDbConnection = new ConnectionNodes(listTwoNodi);
            //IConnectionNodes rethinkDbConnection = new ConnectionNodes(listOneNodo);


            IDbStore rethinkDbStore = new DbStore(rethinkDbConnection); 

            rethinkDbStore.InitializeDatabase();

            /*******   Scegliere in base al numero di nodi in esecuzione sul Cluster  *************/

            IRXTest rxTest = new RXTest(rethinkDbStore, rethinkDbConnection);
            //IRXTest rxTest = new RXSingleNodeTests(rethinkDbStore, rethinkDbConnection);

            /************************ da provare *************************/

            //Test query join Author e Posts
            ITestQuery testQuery = new TestQuery(rethinkDbStore);
            testQuery.PrintAuthorStatus();
        

            /******* Multi Insert e Multi Delete ******/
            
          
           rethinkDbStore.MultiInsertPosts();

           rethinkDbStore.MultiDeletePosts();
          



            /******** prova reactiveExtension *************/
            rxTest.basic_change_feed_with_reactive_extensions();


            Console.ReadLine();

            rethinkDbConnection.CloseConnection();

        }




    }
}
