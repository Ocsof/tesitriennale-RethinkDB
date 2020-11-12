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
                //notification.Type = typeof(T).Name;
                // insert
                var result = R.Db(this.dbName).Table(this.tableName)
                    .Insert(notification)
                    .RunWrite(conn);
            }
        }

        public void DeleteNotification(Guid id)
        {
            var conn = this.connection.GetConnection();
            R.Db(this.dbName).Table(this.tableName).Get(id).Delete().Run(conn);
        }


        
        public T GetNotificationOrNull<T>(Guid id) where T : Notification
        {
            var conn = this.connection.GetConnection();
            Cursor<T> notification;
            try
            {
                notification = R.Db(this.dbName).Table(this.tableName)
                           .Filter(notification => notification.G("Type").Eq(typeof(T).Name).And(notification.G("id").Eq(id)))
                           .Run<T>(conn);

                return notification.First();
            }
            catch (System.InvalidOperationException)
            {
                return null;
            }
        }

        public IList<T> GetNotificationsOrNull<T>(DateTime date) where T : Notification
        {
            var conn = this.connection.GetConnection();
            Cursor<T> notification;
            Console.WriteLine(date);
            Date data = new Date(date);
            try
            {
                /*
                notification = R.Db(this.dbName).Table(this.tableName)
                           .Filter(notification => notification.G("Type").Eq(typeof(T).Name).And(notification.G("Date").Date().Eq(date)))
                           .Run<T>(conn);
                */
                notification = R.Db(this.dbName).Table(this.tableName)
                           .Filter(notification => notification.G("Type").Eq(typeof(T).Name).And(notification.G("Date").Date().Eq(data)))
                           .Run<T>(conn);

                IList<T> list = notification.ToList();
                return list;
            }
            catch (System.InvalidOperationException)
            {
                return null;
            }

            /*
            Cursor<T> notifications = R.Db(this.dbName).Table(this.tableName).Filter(
                                        notification => notification.G("Date").Date().Eq(R.Now().Date())
                                      ).Run(conn);
            */
            /*
            //versione con indice date (su campo date)
            Cursor<T> notificationss = R.Db(this.dbName).Table(nameof(Notification)).GetAll(date).OptArg("index", "date").Run(conn); ;
            */
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

        public IList<T> GetNotificationsOrNull<T>(string text) where T : Notification
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
