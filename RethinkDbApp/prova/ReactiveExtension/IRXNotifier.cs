using Rethink.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rethink.ReactiveExtension
{
    /// <summary>
    /// Notificatore di eventi su tabella "Notifications"
    /// </summary>
    public interface IRXNotifier
    {
        /// <summary>
        /// Test per le notifiche
        /// </summary>
        public void ListenNotifications<T>() where T : Notification;
    }
}
