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
            
            IList<string> hostPortsNodiCluster = new List<String>() { "192.168.7.47:28016", "192.168.7.47:28017", "192.168.7.47:28018", "192.168.7.47:28019", "192.168.7.47:28020" };
            IList<string> hostPortsTwoNodi = new List<String>() { "192.168.7.47:28016", "192.168.7.47:28017" };
            IList<string> hostPortsOneNode = new List<String>() { "192.168.7.47:28016" };
            IUtilityRethink utilityRethink = new UtilityRethink("test", hostPortsNodiCluster);

            var dbManager = utilityRethink.GetDbManager();
            var notificationsManager = utilityRethink.GetNotificationsManager();
            

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

            /*
            Guid id2 = new Guid("335d6447-c1b6-4d7a-920a-ae5e31228f0e");
            notificationsManager.DeleteNotification(id2);
            id2 = new Guid("ba2be3d5-71c3-4461-9ec6-8345ac82a16b");
            notificationsManager.DeleteNotification(id2);
            */
            

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
                Arg = CreateRandomString(),
                Table = CreateRandomString()
            };

            Guid idExecution = Guid.NewGuid();
            Guid idExec = Guid.NewGuid();
            NotificationExec notificationExecution = new NotificationExec
            {
                Id = idExecution,
                Date = DateTime.Now,//new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                Text = CreateRandomString(),
                Arg = CreateRandomString(),
                IdExec = idExec
            };
            Console.WriteLine("Notifica di new data inserita: ");
            Console.WriteLine(notificationNewData.ToString());
            Console.WriteLine("Notifica di esecuzione inserita: ");
            Console.WriteLine(notificationExecution.ToString());

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
 
            //la notifica con id "NewDate" sarebbe di tipo NewDate quindi la variabile restituita è null
            notificationExecution = notificationsManager.GetNotificationOrNull<NotificationExec>(idNewData);
            if(notificationExecution != null)
            {
                Console.WriteLine("Notifica : " + notificationExecution.ToString()); //qui non ci entra perchè è null
            }

            //non esiste notifica con questo id casuale per ora
            notificationExecution = notificationsManager.GetNotificationOrNull<NotificationExec>(new Guid());
            if(notificationExecution != null)
            {
                Console.WriteLine("Notifica: " + notificationExecution.ToString()); //qui non ci entra perchè è null
            }

            //qui è ok quindi entra nell'if
            notificationNewData = notificationsManager.GetNotificationOrNull<NotificationNewData>(idNewData);
            if (notificationNewData != null)
            {
                Console.WriteLine("Notifica con id: " + idNewData.ToString() + " : ");
                Console.WriteLine(notificationNewData.ToString());
            }

            //qui tutto ok ed entra nell'if
            notificationExecution = notificationsManager.GetNotificationOrNull<NotificationExec>(idExecution);
            if (notificationExecution != null)
            {
                Console.WriteLine("Notifica con id: " + idExecution.ToString() + " : ");
                Console.WriteLine(notificationExecution.ToString()); 
            }

            Console.WriteLine();
            notificationsManager.DeleteNotification(idNewData);
            notificationsManager.DeleteNotification(idExecution);

            /************************** Get di notifiche -----> ricerca per Data ************************************************/

            Console.WriteLine("-------- Get di notifiche -----> ricerca per Data ----------");
            Console.WriteLine();

            notificationsManager.NewNotification(notificationNewData);
            notificationsManager.NewNotification(notificationExecution);
            notificationNewData.Id = Guid.NewGuid();
            DateTime newDataDate = notificationNewData.Date;
            IList<NotificationNewData> listNotificationNewData = notificationsManager.GetNotificationsOrNull<NotificationNewData>(newDataDate);
            if(listNotificationNewData.Count != 0)
            {
                Console.WriteLine("Notifiche di new data in data: " + newDataDate.ToString() + ": ");
                foreach (NotificationNewData not in listNotificationNewData)
                {
                    Console.WriteLine(not.ToString());
                    Console.WriteLine();
                }
            }
            IList<NotificationExec> listNotificationExecution = notificationsManager.GetNotificationsOrNull<NotificationExec>(newDataDate);
            if (listNotificationExecution.Count != 0)
            {
                Console.WriteLine("Notifiche di exec in data: " + newDataDate.ToString() + ": ");
                foreach (NotificationExec not in listNotificationExecution)
                {
                    Console.WriteLine(not.ToString());
                    Console.WriteLine();
                }
            }
            notificationsManager.DeleteNotification(idNewData);
            notificationsManager.DeleteNotification(idExecution);

            /************************** Get di notifiche -----> ricerca per Text ************************************************/

            Console.WriteLine("-------- Get di notifiche -----> ricerca per Text ----------");
            Console.WriteLine();

            notificationsManager.NewNotification(notificationNewData);
            notificationsManager.NewNotification(notificationExecution);
            string textNewData = notificationNewData.Text;
            string textExec = notificationExecution.Text;
            listNotificationNewData = notificationsManager.GetNotificationsWithTextOrNull<NotificationNewData>(textNewData);
            if (listNotificationNewData.Count != 0)
            {
                Console.WriteLine("Notifiche di new data con text: " + textNewData);
                foreach (NotificationNewData not in listNotificationNewData)
                {
                    Console.WriteLine(not.ToString());
                    Console.WriteLine();
                }
            }
            listNotificationExecution = notificationsManager.GetNotificationsWithTextOrNull<NotificationExec>(textExec);
            if (listNotificationExecution.Count != 0)
            {
                Console.WriteLine("Notifiche di exec con text: " + textExec);
                foreach (NotificationExec not in listNotificationExecution)
                {
                    Console.WriteLine("Notifica: " + not.ToString());
                    Console.WriteLine();
                }
            }
            notificationsManager.DeleteNotification(idNewData);
            notificationsManager.DeleteNotification(idExecution);

            /************************** Get di notifiche -----> ricerca per Arg ************************************************/

            Console.WriteLine("-------- Get di notifiche -----> ricerca per Arg ----------");
            Console.WriteLine();

            notificationsManager.NewNotification(notificationNewData);
            notificationsManager.NewNotification(notificationExecution);
            string argNewData = notificationNewData.Arg;
            string argExec = notificationExecution.Arg;
            listNotificationNewData = notificationsManager.GetNotificationsWithArgOrNull<NotificationNewData>(argNewData);
            if (listNotificationNewData.Count != 0)
            {
                Console.WriteLine("Notifica di new data con Arg: " + argNewData);
                foreach (NotificationNewData not in listNotificationNewData)
                {
                    Console.WriteLine(not.ToString());
                    Console.WriteLine();
                }
            }
            listNotificationExecution = notificationsManager.GetNotificationsWithArgOrNull<NotificationExec>(argExec);
            if (listNotificationExecution.Count != 0)
            {
                Console.WriteLine("Notifica di exec con Arg: " + argExec);
                foreach (NotificationExec not in listNotificationExecution)
                {
                    Console.WriteLine(not.ToString());
                    Console.WriteLine();
                }
            }
            notificationsManager.DeleteNotification(idNewData);
            notificationsManager.DeleteNotification(idExecution);


            /************************** Test Notifier ************************************************/

            /*
            Console.WriteLine("****************** Test Notifier ***************");
            Console.WriteLine();

            var notificatorsExec = utilityRethink.GetNotifier<NotificationExec>();
            var notificatorsNewData = utilityRethink.GetNotifier<NotificationNewData>();

            Console.WriteLine("-------------- Listen normale -------------");
            Console.WriteLine();

            notificatorsExec.Listen();
            notificatorsNewData.Listen();

            //Next simulate 2 inserts into Notification table.
            Thread.Sleep(3000);

            Task.Run(() =>
            {
                notificationsManager.NewNotification<NotificationExec>(new NotificationExec
                {
                    Id = Guid.NewGuid(),
                    Date = DateTime.Now,
                    Text = CreateRandomString(),
                    Arg = CreateRandomString(),
                    IdExec = Guid.NewGuid()
                });
            });

            Console.WriteLine();
            Thread.Sleep(10000);

            Task.Run(() =>
            {
                notificationsManager.NewNotification<NotificationNewData>(new NotificationNewData
                {
                    Id = Guid.NewGuid(),
                    Date = DateTime.Now,
                    Text = CreateRandomString(),
                    Arg = CreateRandomString(),
                    Table = CreateRandomString()
                });
            });

            Console.WriteLine();
            Thread.Sleep(10000);

            notificatorsExec.StopListening();
            notificatorsNewData.StopListening();

            Console.WriteLine("-------------- Listen Per argomento -------------");
            Console.WriteLine();



            Console.WriteLine("-------------- Listen per lista di argomenti -------------");
            Console.WriteLine();

            notificatorsExec.StopListening();
            notificatorsNewData.StopListening();

            */

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
