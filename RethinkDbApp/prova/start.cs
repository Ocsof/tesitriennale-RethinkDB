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
            Console.WriteLine("************ Test Connettività *************");
            Console.WriteLine();

            HttpClient client = new HttpClient();
            var resp = await client.GetAsync("http://192.168.7.47:8081");
            Console.WriteLine(resp.StatusCode);

            Console.WriteLine();

            /************************************************************************************************************************************
            *********************************************Test DbManager   ********************************************************************
            **********************************************************************************************************************************/
            Console.WriteLine("************ Test DbManager *************");
            Console.WriteLine();

            dbManager.CreateTable("Notifications");
            dbManager.CreateTable("Notifications");
            Console.WriteLine("Table List: " + dbManager.GetTablesList());
            //store.DelateTable("Notifications"); 
            dbManager.CreateIndex("Notifications", "Date");
            Console.WriteLine("Index List: " + dbManager.GetIndexList("Notifications"));
            dbManager.DeleteIndex("Notifications", "Date");
            Console.WriteLine("Index List: " + dbManager.GetIndexList("Notifications"));
            //dbManager.Reconfigure(2, 3);

            Console.WriteLine();

            /***********************************************************************************************************************************
            ******************************************* Test NotificationsManager **********************************************************
            **********************************************************************************************************************************/

            Console.WriteLine("****************** Test NotificationsManager ***************");
            Console.WriteLine();

            //per eliminare una notifica in particolare
            Guid id = new Guid("03c0f735-0a30-4101-a116-bf29b4b364e9");
            notificationsManager.DeleteNotification(id);  //in questo caso non esiste quindi non te lo fa

            //********Test di NewNotification(notification), DeleteNotification(id) *******/
            IList<Guid> idList = MultiInsertNotifications(notificationsManager);
            MultiDeleteNotifications(notificationsManager, idList);

            /****************** Test di New e Delete su una nuova notifica di execution e un'altra di NewData **********************************/

            Console.WriteLine("-------- New e Delete su due nuove notifiche ----------");
            Console.WriteLine();

            Guid idNewData = Guid.NewGuid();
            NotificationNewData notificationNewData = new NotificationNewData
            {
                Id = idNewData,
                Date = DateTime.Now,
                Text = CreateRandomString(),
                Table = CreateRandomString()
            };

            Guid idExecution = Guid.NewGuid();
            Guid idExec = Guid.NewGuid();
            NotificationExec notificationExecution = new NotificationExec
            {
                Id = idExecution,
                Date = DateTime.Now,//new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                Text = CreateRandomString(),
                IdExec = idExec
            };
            Console.WriteLine("Notifica 1: " + notificationNewData.ToString());
            Console.WriteLine("Notifica 2: " + notificationExecution.ToString());

            notificationsManager.NewNotification(notificationNewData);
            notificationsManager.NewNotification(notificationExecution);
            notificationsManager.DeleteNotification(idNewData);
            notificationsManager.DeleteNotification(idExecution);

            Console.WriteLine();

            /************************** Get di notifiche -----> ricerca per Id ************************************************/

            Console.WriteLine("-------- Get di notifiche -----> ricerca per Id ----------");
            Console.WriteLine();

            notificationsManager.NewNotification(notificationNewData);
            notificationsManager.NewNotification(notificationExecution);

            notificationNewData = notificationsManager.GetNotificationOrNull<NotificationNewData>(idNewData);
            if(notificationNewData != null)
            {
                Console.WriteLine("Notifica: " + notificationNewData.ToString());
            }  
            //la notifica con id "NewDate" sarebbe di tipo NewDate quindi la variabile restituita è null
            notificationExecution = notificationsManager.GetNotificationOrNull<NotificationExec>(idNewData);
            if(notificationExecution != null)
            {
                Console.WriteLine("Notifica: " + notificationExecution.ToString()); //qui non ci entra perchè è null
            }
            //non esiste notifica con questo id casuale per ora
            notificationExecution = notificationsManager.GetNotificationOrNull<NotificationExec>(new Guid());
            if(notificationExecution != null)
            {
                Console.WriteLine("Notifica: " + notificationExecution.ToString()); //qui non ci entra perchè è null
            }
            Console.WriteLine();
            notificationsManager.DeleteNotification(idNewData);
            notificationsManager.DeleteNotification(idExecution);

            /************************** Get di notifiche -----> ricerca per Data ************************************************/

            Console.WriteLine("-------- Get di notifiche -----> ricerca per Data ----------");
            Console.WriteLine();

            notificationsManager.NewNotification(notificationNewData);
            //notificationsManager.NewNotification(notificationExecution);
            notificationNewData.Id = Guid.NewGuid();
            notificationsManager.NewNotification(notificationNewData);
            DateTime newDataDate = notificationNewData.Date;
            Console.WriteLine(newDataDate.ToString() == notificationNewData.Date.ToString());
            IList<NotificationNewData> listNotificationNewData = notificationsManager.GetNotificationsOrNull<NotificationNewData>(newDataDate);
            if(listNotificationNewData.Count != 0)
            {
                foreach(NotificationNewData not in listNotificationNewData)
                {
                    Console.WriteLine("Notifica: " + not.ToString());
                }
            }
            //new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)

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
