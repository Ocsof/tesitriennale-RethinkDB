using Rethink.Model;
using RethinkDb.Driver.Model;
using RethinkDb.Driver.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace RethinkDbApp.ReactiveExtension
{
    public class NotificationSubscription<T> where T: Notification
    {
        public Guid Guid { get; }
        public IObservable<Change<T>> Observable { get; }

        public NotificationSubscription(Guid guid, IObservable<Change<T>> observable)
        {
            this.Guid = guid;
            this.Observable = observable;
        }


    }
}
