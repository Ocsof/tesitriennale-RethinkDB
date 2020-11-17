using Rethink.Model;
using RethinkDb.Driver.Model;
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
        /// Test per le notifiche
        /// </summary>
        public IObservable<Change<T>> Listen();

        /// <summary>
        /// Rimane in ascolto solo per le notifiche con un determinato argomento
        /// </summary>
        /// <param name="arg">argomento della notifica</param>
        public IObservable<Change<T>> ListenWithArg(string arg);

        /// <summary>
        /// Rimane in ascolto solo per le notifiche che hanno uno degli argomenti della lista
        /// </summary>
        /// <param name="arg">argomenti delle notifiche su cui rimanere in ascolto</param>
        public void ListenWithOneOfTheArguments(IList<string> argsList);

        /// <summary>
        /// Si smette di rimanere in ascolto sulla tabella Notifiche
        /// </summary>
        public void StopListening();


    }
}
