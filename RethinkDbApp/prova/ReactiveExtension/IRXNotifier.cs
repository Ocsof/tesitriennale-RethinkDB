using Rethink.Model;
using RethinkDbApp.ReactiveExtension;
using System;

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
        /// Smette di rimanere in ascolto sull'observervable con id passato in input
        /// </summary>
        /// <param name="id">id dell'observable</param>
        public void StopListening(Guid id);

        /// <summary>
        /// Smette di rimanere in ascolto sull'observervable
        /// </summary>
        /// <param name="notificationSubscription">coppia Guid-observable</param>
        public void StopListening(NotificationSubscription<T> notificationSubscription);


    }
}
