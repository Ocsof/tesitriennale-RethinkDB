using Rethink.Model;
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
        public IDbStore ManageDb();

        /// <summary>
        /// Metodo per gestire le notifiche sul Db
        /// </summary>
        /// <returns>Oggetto di gestione delle notifiche presenti sul db</returns>
        public IManageNotifications ManageNotifications();

        /// <summary>
        /// Permette di rimanere in ascolto sui cambiamenti nella tabella ed essere informati di tale cambiamento, es: insert, update
        /// </summary>
        /// <param name="table">tabella su cui rimanere in ascolto del db</param>
        public void RegisterToNotifications();


        /// <summary>
        /// Da chiamare ogni volta che si termina una sessione con Rethink per chiudere in sicurezza la connessione
        /// </summary>
        public void CloseConnection();
    }
}
