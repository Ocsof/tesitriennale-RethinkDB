using Rethink.Model;
using RethinkDb.Driver.Model;
using RethinkDbApp.ReactiveExtension;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rethink.ReactiveExtension
{
    /// <summary>
    /// Notificatore di eventi su tabella "Notifications"
    /// </summary>
    public interface IRXNotifier<T> where T : Notification
    {

        /// <summary>
        /// Rimane in ascolto solo per le notifiche che hanno uno degli argomenti della lista
        /// </summary>
        /// <param name="arg">argomenti delle notifiche su cui rimanere in ascolto</param>
        public NotificationSubscription<T> ListenWithOneOfTheArguments(params string[] argsList);

        /// <summary>
        /// Si smette di rimanere in ascolto sulla tabella Notifiche
        /// </summary>
        public void StopListening(Guid id);

        public void StopListening(NotificationSubscription<T> pair);//coppia


    }
}
