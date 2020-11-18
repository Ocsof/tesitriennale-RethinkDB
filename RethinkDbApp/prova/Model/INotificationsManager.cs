using Rethink.Model;
using Rethink.ReactiveExtension;

namespace RethinkDbApp.Model
{
    /// <summary>
    /// 
    /// </summary>
    public interface INotificationsManager
    {
        /// <summary>
        /// Metodo per gestire le notifiche sul Db
        /// </summary>
        /// <returns>Oggetto di gestione delle notifiche presenti sul db, permette di effettuare query</returns>
        public IQueryNotifications GetQueryService();

        /// <summary>
        /// Metodo per ottenere un notificatore di notifiche di un certo tipo specificato 
        /// </summary>
        /// <returns>Notificatore</returns>
        public IRXNotifier<T> GetNotifier<T>() where T : Notification;
    }
}
