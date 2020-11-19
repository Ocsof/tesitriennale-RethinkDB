using Rethink.Connection;
using Rethink.Model;
using Rethink.ReactiveExtension;

namespace RethinkDbApp.Model
{
    class NotificationsManager : INotificationsManager
    {
        private readonly IConnectionNodes connection;
        private readonly IQueryNotifications queryToNotifications;
        

        public NotificationsManager(IConnectionNodes connection)
        {
            this.connection = connection;
            this.queryToNotifications = new QueryNotifications(connection);
        }
        
        public string GetWellKnownTable()
        {
            return INotificationsManager.TABLE;
        }
        
        public IQueryNotifications GetQueryService()
        {
            return this.queryToNotifications;
        }

        public IRXNotifier<T> GetNotifier<T>() where T : Notification
        {
            IRXNotifier<T> rxNotifier = new RXNotifier<T>(this.connection);
            return rxNotifier;
        }

        
    }
}
