using Rethink.Connection;
using RethinkDb.Driver;
using RethinkDb.Driver.Ast;
using RethinkDb.Driver.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rethink.Model
{
    class ManageNotifications: IManageNotifications
    {
        private readonly IConnectionNodes rethinkDbConnection;
        private readonly static RethinkDB R = RethinkDB.R;
        private readonly string dbName;

        public ManageNotifications(IConnectionNodes rethinkDbConnection)  //IConnectionPooling connectionFactory  // ---> per connessione con un cluster + nodi
        {
            this.rethinkDbConnection = rethinkDbConnection;
            this.dbName = rethinkDbConnection.GetNodi().ElementAt(0).Database;
        }

        public int GetIdLastNotification()
        {
            var conn = this.rethinkDbConnection.GetConnection();
            var id = R.Db(this.dbName).Table(nameof(Notification)).Count().Run(conn);

            return id;
        }

        public int GetIdLastNotificationExecution()
        {
            var conn = this.rethinkDbConnection.GetConnection();
            var id = R.Db(this.dbName).Table(nameof(Notification)).Filter(
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
            var conn = this.rethinkDbConnection.GetConnection();

            Cursor<T> all = R.Db(this.dbName).Table(nameof(Notification))
                .GetAll(notification.Id)//[new { index = nameof(Post.title) }]
                .Run<T>(conn);

            var notifications = all.ToList();
            if (notifications.Count > 0)
            {
                // update
                R.Db(this.dbName).Table(nameof(Notification)).Get(notifications.First().Id).Update(notification).RunWrite(conn);
            }
            else
            {
                // insert
                var result = R.Db(this.dbName).Table(nameof(Notification))
                    .Insert(notification)
                    .RunWrite(conn);
            }
        }

        public T GetNotification<T>(int id) where T : Notification
        {
            var conn = this.rethinkDbConnection.GetConnection();

            T notification = R.Db(this.dbName).Table(nameof(Notification))
                            .GetAll(id)
                            .Run<T>(conn);

            return notification;
        }

        public IList<T> GetNotifications<T>(Date date) where T : Notification
        {
            var conn = this.rethinkDbConnection.GetConnection();

            Cursor<T> notifications = R.Db(this.dbName).Table(nameof(Notification)).Filter(
                                        notification => notification.G("Date").Date().Eq(R.Now().Date())
                                      ).Run(conn);

            /*
            //versione con indice date (su campo date)
            Cursor<T> notificationss = R.Db(this.dbName).Table(nameof(Notification)).GetAll(date).OptArg("index", "date").Run(conn); ;
            */
            return notifications.ToList();
        }

        public IList<T> GetNotifications<T>(string text) where T : Notification
        {
            var conn = this.rethinkDbConnection.GetConnection();

            Cursor<T> notifications = R.Db(this.dbName).Table(nameof(Notification)).Filter(
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
