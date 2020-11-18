using Rethink.Model;
using RethinkDb.Driver;
using RethinkDb.Driver.Model;
using RethinkDbApp.ReactiveExtension;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
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

            
            Guid id2 = new Guid("f903aa68-864c-4b97-8f53-9ef191017001");
            notificationsManager.DeleteNotification(id2);
            id2 = new Guid("f1a17ce5-4d6a-4825-a145-c09e62b25c22");
            notificationsManager.DeleteNotification(id2);
            id2 = new Guid("8d440084-42d3-4153-82ff-70671ecb6f00");
            notificationsManager.DeleteNotification(id2);
            id2 = new Guid("5a2266b7-7003-4fac-aa15-402a9fbcd5d1");
            notificationsManager.DeleteNotification(id2);
            id2 = new Guid("9e5b5586-1c7e-4903-8424-2f5c8cfa4328");
            notificationsManager.DeleteNotification(id2);
            id2 = new Guid("ca14e408-bd83-4154-b815-9e321a0b9970");
            notificationsManager.DeleteNotification(id2);
            id2 = new Guid("dd5b1215-7f39-42ab-bc4b-103fea7b8ef9");
            notificationsManager.DeleteNotification(id2);
            

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

            /*** prova ****/

            
            Console.WriteLine("****************** Test Notifier ***************");
            Console.WriteLine();

            var notificatorsExec = utilityRethink.GetNotifier<NotificationExec>();
            var notificatorsNewData = utilityRethink.GetNotifier<NotificationNewData>();

            var onCompleted = 0;
            var onError = 0;
            var onNext = 0;


            NotificationSubscription<NotificationExec> pair = notificatorsExec.ListenWithOneOfTheArguments("ciao", "ciuppa");
            IObservable<Change<NotificationExec>> observervableExecForArgs = pair.Observable;
            observervableExecForArgs.SubscribeOn(NewThreadScheduler.Default)
                .Subscribe(
                    x => OnNext(x, ref onNext),
                    e => OnError(e, ref onError),
                    () => OnCompleted(ref onCompleted)
                );

            //Next simulate 3 inserts into Notification table.
            Thread.Sleep(3000);

            Task.Run(() =>
            {
                notificationsManager.NewNotification<NotificationExec>(new NotificationExec
                {
                    Id = Guid.NewGuid(),
                    Date = DateTime.Now,
                    Text = CreateRandomString(),
                    Arg = "ciao",
                    IdExec = Guid.NewGuid()
                });
            });

            Console.WriteLine();
            Thread.Sleep(10000);

            Task.Run(() =>
            {
                notificationsManager.NewNotification<NotificationExec>(new NotificationExec
                {
                    Id = Guid.NewGuid(),
                    Date = DateTime.Now,
                    Text = CreateRandomString(),
                    Arg = "ciuppa",
                    IdExec = Guid.NewGuid()
                });
            });

            Console.WriteLine();
            Thread.Sleep(10000);

            //qui non entra nella onNext perchè l'argomento "pappappero" non è nella lista 
            Task.Run(() =>
            {
                notificationsManager.NewNotification<NotificationExec>(new NotificationExec
                {
                    Id = Guid.NewGuid(),
                    Date = DateTime.Now,
                    Text = CreateRandomString(),
                    Arg = "pappappero",
                    IdExec = Guid.NewGuid()
                });
            });

            Console.WriteLine();
            Thread.Sleep(10000);

            notificatorsExec.StopListening(pair.Guid);
            //notificatorsExec.StopListening(pair);
            Console.WriteLine();

            //notificatorsNewData.StopListening();


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

        private static void OnCompleted(ref int onCompleted)
        {
            Console.WriteLine("Stop listening");
            onCompleted++;
        }

        private static void OnError(Exception obj, ref int onError)
        {
            Console.WriteLine("On Error");
            Console.WriteLine(obj.Message);
            onError++;
        }

        private static void OnNext<T>(Change<T> obj, ref int onNext) where T : Notification
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
