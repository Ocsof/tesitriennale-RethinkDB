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
        static async Task Main(string[] args)
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
            IUtilityRethink utilityRethink = new UtilityRethink("test", hostPortsNodiCluster);

            var dbManager = utilityRethink.GetDbManager();
            var notificationsManager = utilityRethink.GetNotificationsManager();
            var notificators = utilityRethink.GetNotificationsManager();

            /********** Test Connettività **********/
            HttpClient client = new HttpClient();
            var resp = await client.GetAsync("http://192.168.7.47:8081");
            Console.WriteLine(resp.StatusCode);

            /************************************************************************************************************************************
            *********************************************Test DbManager   ********************************************************************
            **********************************************************************************************************************************/

            dbManager.CreateTable("Notifications");
            Console.WriteLine(dbManager.GetTablesList());
            //store.DelateTable("Notifications"); 
            dbManager.CreateIndex("Notifications", "Date");
            Console.WriteLine(dbManager.GetIndexList("Notifications"));
            dbManager.DeleteIndex("Notifications", "Date");
            Console.Write(dbManager.GetIndexList("Notifications"));
            //dbManager.Reconfigure(2, 3);
            Console.WriteLine();

            /***********************************************************************************************************************************
            ******************************************* Test NotificationsManager **********************************************************
            **********************************************************************************************************************************/

            //********Test di NewNotification(notification), DeleteNotification(id) *******/

            //per eliminare una notifica in particolare
            
            Guid id = new Guid("03c0f735-0a30-4101-a116-bf29b4b364e9");
            Console.WriteLine(id.ToString());
            notificationsManager.DeleteNotification(id);  //in questo caso non esiste quindi non te lo fa
            
            IList<Guid> idList = MultiInsertNotifications(notificationsManager);
            MultiDeleteNotifications(notificationsManager, idList);

            /*************Creazione e inserimento nel db di una notifica di New Date e una di Execution **********************************/
            Guid idNewData = Guid.NewGuid();
            Console.Write(id);
            NotificationNewData notificationNewData = new NotificationNewData
            {
                Id = idNewData,
                Date = DateTime.Now,
                Text = CreateRandomString(),
                Table = CreateRandomString()
            };

            Guid idExecution = Guid.NewGuid();
            Console.Write(id);
            Guid idExec = Guid.NewGuid();
            NotificationExec notificationExecution = new NotificationExec
            {
                Id = idExecution,
                Date = DateTime.Now,//new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                Text = CreateRandomString(),
                IdExec = idExec
            };
            Console.WriteLine(notificationNewData.Type);
            Console.WriteLine(notificationExecution.Type);

            notificationsManager.NewNotification(notificationNewData);
            notificationsManager.NewNotification(notificationExecution);
            notificationsManager.DeleteNotification(idNewData);
            notificationsManager.DeleteNotification(idExecution);

            Console.ReadLine();

            /************************** Get di notifiche -----> ricerca per Id ************************************************/
            notificationsManager.NewNotification(notificationNewData);
            notificationsManager.NewNotification(notificationExecution);

            NotificationNewData not = notificationsManager.GetNotificationOrNull<NotificationNewData>(idNewdate);
            //Console.WriteLine(not.ToString());
            //caso di errore ---> la notifica con id 1 sarebbe di tipo NewDate
            //NotificationExec notE = notificationsManager.GetNotificationOrNull<NotificationExec>(1);
            //Console.WriteLine(notE.ToString());
            //caso di errore ---> la notifica con id 2 sarebbe di tipo Execution
            //not = notificationsManager.GetNotificationOrNull<NotificationNewData>(1);
            //Console.WriteLine(not.ToString());

            //come 
            //Console.WriteLine("Text della notifica richiesta: " + notificationsManager.GetNotification<NotificationExec>(5, 2));
            //notificationsManager.DeleteNotification(55); //es di Delete che non worka perchè non esistono ancora notifiche con id = 55
            //MultiDeleteNotifications(notificationsManager);

            MultiInsertNotifications(notificationsManager);
            //MultiDeleteNotifications(notificationsManager);
            //notificationsManager.GetNotifications(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));


            /******** prova reactiveExtension *************/
            //utilityRethink.GetNotifier();


            Console.ReadLine();

            utilityRethink.CloseConnection();

        }


        private static IList<Guid> MultiInsertNotifications(INotificationsManager notificationsManager)
        {
            IList<Guid> idList = new List<Guid>();
            Guid id;
            Guid idExec;
            int typeNotification = 1;
            for (int i = 0; i < 50; i++)
            {
                id = Guid.NewGuid(); 
                idExec = Guid.NewGuid();
                idList.Add(id);
                if (typeNotification == 1)
                {
                    NotificationExec notification = new NotificationExec
                    {
                        Id = id,
                        Date = DateTime.Now, //new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                        Text = CreateRandomString(),
                        IdExec = idExec
                    };
                    notificationsManager.NewNotification(notification);
                    typeNotification++;
                }
                else
                {
                    NotificationNewData notification = new NotificationNewData
                    {
                        Id = id,
                        Date = DateTime.Now, //new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                        Text = CreateRandomString(),
                        Table = CreateRandomString()
                    };
                    notificationsManager.NewNotification(notification);
                    typeNotification--;
                }
            }
            return idList;
        }

        private static void MultiDeleteNotifications(INotificationsManager notificationsManager, IList<Guid> idList)
        {
            foreach(Guid id in idList)
            {
                notificationsManager.DeleteNotification(id);
            }
        }

        private static String CreateRandomString()
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
