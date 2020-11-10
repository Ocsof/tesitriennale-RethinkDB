using Rethink.Model;
using Rethink.ReactiveExtension;
using RethinkDb.Driver.Ast;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rethink
{
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

        
        public IRXNotifier GetNotifier();


        /// <summary>
        /// Da chiamare ogni volta che si termina una sessione con Rethink per chiudere in sicurezza la connessione
        /// </summary>
        public void CloseConnection();
    }
}
