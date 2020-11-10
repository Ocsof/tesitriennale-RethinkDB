using Rethink.Model;
using RethinkDb.Driver;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Rethink
{
    class Start
    {

        //static void Main(string[] args)
        async Task Main(string[] args)
        {
           
            /*
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
            */

            IList<string> hostPortsNodiCluster = new List<String>() { "192.168.7.47:28016", "192.168.7.47:28017", "192.168.7.47:28018", "192.168.7.47:28019", "192.168.7.47:28020" };
            IList<string> hostPortsTwoNodi = new List<String>() { "192.168.7.47:28016", "192.168.7.47:28017" };
            IList<string> hostPortsOneNode = new List<String>() { "192.168.7.47:28016" };
            IUtilityRethink utilityRethink = new UtilityRethink("test", hostPortsTwoNodi);
            var dbManager = utilityRethink.GetDbManager();
            var notificationsManager = utilityRethink.GetNotificationsManager();

            /****** test DbStore   **********/
            dbManager.CreateTable("Notifications");
            Console.WriteLine(dbManager.GetTablesList());
            //store.DelateTable("Notifications"); 
            dbManager.CreateIndex("Notifications", "Date");
            Console.WriteLine(dbManager.GetIndexList("Notifications"));
            dbManager.DeleteIndex("Notifications", "Date");
            Console.Write(dbManager.GetIndexList("Notifications"));


            //Test Connettività 
            HttpClient client = new HttpClient();
            var resp = await client.GetAsync("http://192.168.7.47:8081");
            Console.WriteLine(resp.StatusCode);


            /******* Multi Insert e Multi Delete ******/
            this.MultiInsertNotifications(notificationsManager);
            this.MultiDeleteNotifications(notificationsManager);



            /******** prova reactiveExtension *************/
            utilityRethink.GetNotifier();


            Console.ReadLine();

            utilityRethink.CloseConnection();

        }


        public void MultiInsertNotifications(INotificationsManager notificationsManager)
        {
            int typeNotification = 1;
            int id = notificationsManager.GetIdLastNotification();
            int idExec = notificationsManager.GetIdLastNotificationExecution();
            Console.WriteLine(id);
            Console.WriteLine(idExec);

            for (int i = 0; i < 50; i++)
            {
                if (typeNotification == 1)
                {
                    NotificationExec notification = new NotificationExec
                    {
                        Id = id,
                        Date = new DateTime(),
                        Text = this.createRandomString(),
                        Type = typeNotification,
                        IdExec = idExec
                    };
                    notificationsManager.NewNotification(notification);
                    idExec++;
                    typeNotification++;
                }
                else
                {
                    NotificationNewDate notification = new NotificationNewDate
                    {
                        Id = id,
                        Date = new DateTime(),
                        Text = this.createRandomString(),
                        Type = typeNotification,
                        Table = this.createRandomString()
                    };
                    notificationsManager.NewNotification(notification);
                    typeNotification--;
                }
                id++;
            }
        }

        public void MultiDeleteNotifications(INotificationsManager notificationsManager)
        {
            for(int id = 0; id < 50; id++)
            {
                notificationsManager.DeleteNotification(id);
            }
        }

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
