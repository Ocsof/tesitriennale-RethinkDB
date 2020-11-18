using Rethink.Model;
using RethinkDb.Driver.Model;
using RethinkDb.Driver.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace RethinkDbApp.ReactiveExtension
{
    /// <summary>
    /// Classe che accoppia un observable con un Guid
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NotificationSubscription<T> where T: Notification
    {
        /// <summary>
        /// id
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// observable
        /// </summary>
        public IObservable<Change<T>> Observable { get; }

        public NotificationSubscription(Guid guid, IObservable<Change<T>> observable)
        {
            this.Guid = guid;
            this.Observable = observable;
        }


    }
}
