using Rethink.Model;
using Rethink.ReactiveExtension;
using RethinkDb.Driver.Ast;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rethink
{
    /// <summary>
    /// Libreria per la gestione del Db, della tabella "Notifiche" e per rimanere in ascolto sui cambiamenti della tabella "Notifications"
    /// </summary>
    public interface IUtilityRethink
    {       
        /// <summary>
        /// Metodo per gestire il db precedentemente specificato nel costruttore di Utility Rethink
        /// </summary>
        /// <returns>Oggetto che gestisce il database, permette di creare tabelle nuove, indici, riconfigurare il numero di shards e repliche</returns>
        public IDbManager GetDbManager();

        /// <summary>
        /// Metodo per gestire le notifiche sul Db
        /// </summary>
        /// <returns>Oggetto di gestione delle notifiche presenti sul db</returns>
        public INotificationsManager GetNotificationsManager();

        /// <summary>
        /// Metodo per ottenere un notificatore di notifiche di un certo tipo specificato 
        /// </summary>
        /// <returns>Notificatore</returns>
        public IRXNotifier<T> GetNotifier<T>() where T : Notification;


        /// <summary>
        /// Da chiamare ogni volta che si termina una sessione con Rethink per chiudere in sicurezza la connessione
        /// </summary>
        public void CloseConnection();
    }
}
