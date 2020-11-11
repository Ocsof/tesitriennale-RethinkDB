using Rethink.Connection;
using RethinkDb.Driver;
using RethinkDb.Driver.Ast;
using RethinkDb.Driver.Net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Random = System.Random;

namespace Rethink.Model
{
    class NotificationsManager: INotificationsManager
    {
        private readonly IConnectionNodes connection;
        private readonly static RethinkDB R = RethinkDB.R;
        private readonly string dbName;
        private readonly string tableName;

        public NotificationsManager(IConnectionNodes connection)  //IConnectionPooling connectionFactory  // ---> per connessione con un cluster + nodi
        {
            this.connection = connection;
            this.dbName = connection.GetNodi().ElementAt(0).Database;
            this.tableName = "Notifications";
        }

        public int GetIdLastNotification()
        {
            var conn = this.connection.GetConnection();
            var id = (int)R.Db(this.dbName).Table(this.tableName).Count().Run(conn);

            return id;
        }

        public int GetIdLastNotificationExecution()
        {
            var conn = this.connection.GetConnection();
            var id = (int)R.Db(this.dbName).Table(this.tableName).Filter(
                                        notification => notification.G("Type").Eq(1) //tipo=1 è una NotificationExec
                                      ).Count().Run(conn);

            /*
            //versione con indice id_exec (su campo id_exec)
            var id = R.Db(this.dbName).Table(nameof(Notification)).GetAll(1).OptArg("index", "id_exec").Count().Run(conn);  
            */

            return id;
        }

        public void NewNotification<T>(T notification) where T : Notification
        {
            var conn = this.connection.GetConnection();

            Cursor<T> all = R.Db(this.dbName).Table(this.tableName)
                .GetAll(notification.Id)//[new { index = nameof(Post.title) }]
                .Run<T>(conn);

            var notifications = all.ToList();
            if (notifications.Count > 0)
            {
                // update
                R.Db(this.dbName).Table(this.tableName).Get(notifications.First().Id).Update(notification).RunWrite(conn);
            }
            else
            {
                notification.Type = typeof(T).Name;
                // insert
                var result = R.Db(this.dbName).Table(this.tableName)
                    .Insert(notification)
                    .RunWrite(conn);
            }
        }

        public void DeleteNotification(int id)
        {
            var conn = this.connection.GetConnection();
            R.Db(this.dbName).Table(this.tableName).Get(id).Delete().Run(conn);
        }


        
        public T GetNotification<T>(int id) where T : Notification
        {
            var conn = this.connection.GetConnection();
            
            var notification = R.Db(this.dbName).Table(this.tableName)
                              .Filter(notification => notification.G("Type").Eq(typeof(T).Name) )
                              .Get(id)
                              .Run<T>(conn);

            return notification;
        }
        
        /*
        public string GetNotification(int id, int type)   
        {
            var conn = this.connection.GetConnection();
            string text = "err";
            if(type == 1)
            {
                var notification = R.Db(this.dbName).Table(this.tableName)
                              .GetAll(id)
                              .Run<NotificationExec>(conn);
                text = notification.Text;
            }
            else
            {
                try
                {
                    var notification = R.Db(this.dbName).Table(this.tableName)
                              .Get(id)
                              .Run<NotificationNewDate>(conn);
                    Console.WriteLine(notification.ToString());
                    text = notification.Text;
                }
                
                catch(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
                {
                    throw new Microsoft.CSharp.RuntimeBinder.RuntimeBinderException();
                    //Console.WriteLine("Non trovata nessuna notifica di tipo" + id.ToString() + " con questo id: " + type.ToString());
                }
                
                
            }
            return text;
        }*/

        public IList<T> GetNotifications<T>(DateTime date) where T : Notification
        {
            var conn = this.connection.GetConnection();

            Cursor<T> notifications = R.Db(this.dbName).Table(this.tableName).Filter(
                                        notification => notification.G("Date").Date().Eq(R.Now().Date())
                                      ).Run(conn);

            /*
            //versione con indice date (su campo date)
            Cursor<T> notificationss = R.Db(this.dbName).Table(nameof(Notification)).GetAll(date).OptArg("index", "date").Run(conn); ;
            */
            return notifications.ToList();
        }

        /*
        public IList<String> GetNotifications(DateTime date)
        {
            var conn = this.connection.GetConnection();

            Cursor<T> notifications = R.Db(this.dbName).Table(this.tableName).Filter(
                                        notification => notification.G("Date").Date().Eq(R.Now().Date())
                                      ).Run(conn);
        
            
            //versione con indice date (su campo date)
            Cursor<T> notificationss = R.Db(this.dbName).Table(nameof(Notification)).GetAll(date).OptArg("index", "date").Run(conn); ;
            
            return notifications.ToList();
        }
        */

        public IList<T> GetNotifications<T>(string text) where T : Notification
        {
            var conn = this.connection.GetConnection();

            Cursor<T> notifications = R.Db(this.dbName).Table(this.tableName).Filter(
                                        notification => notification.G("Text").Eq(text)
                                      ).Run(conn);

            /*
            //versione con indice text (su campo text)
            Cursor<T> notificationss = R.Db(this.dbName).Table(nameof(Notification)).GetAll(text).OptArg("index", "text").Run(conn); ;
            */

            return notifications.ToList();
        }

        
    }
}
