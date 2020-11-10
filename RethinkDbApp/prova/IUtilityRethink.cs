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
        /// Crea la tabella sul db precedentemente specificato.
        /// </summary>
        /// <param name="table">tabelle da crearee. Se c'è gia non la crea</param>
        public void CreateTable(String table);

        /// <summary>
        /// Permette di rimanere in ascolto sui cambiamenti nella tabella ed essere informati di tale cambiamento, es: insert, update
        /// </summary>
        /// <param name="table">tabella su cui rimanere in ascolto del db</param>
        public void RegisterToNotifications();

        /// <summary>
        /// Ritorno l'id più alto delle notifiche presenti sul db
        /// </summary>
        /// <returns>Id dell'ultima notifica</returns>
        public int GetIdLastNotification();

        /// <summary>
        /// Ritorna l'id più alto delle notifiche di esecuzione presenti sul db
        /// </summary>
        /// <returns>Id dell'ultima notifica di esecuzione</returns>
        public int GetLastNotificationExecution();

        /// <summary>
        /// Inserisce sul db "Notification" una nuova notifica
        /// </summary>
        /// <typeparam name="T">Deve essere una classe che eredita dalla classe astratta "Notification"</typeparam>
        /// <param name="notification">Notifica che verrà inserita sul db</param>
        public void NewNotification<T>(T notification) where T : Notification;

        /// <summary>
        /// Richiede al db la notifica con l'id passato in input
        /// </summary>
        /// <typeparam name="T">Deve essere una classe che eredita dalla classe astratta "Notification"</typeparam>
        /// <param name="id">id della notifica da ricercare</param>
        /// <returns>La notifica</returns>
        public T GetNotification<T>(int id) where T: Notification;

        /// <summary>
        /// Richiede al db tutte le notifiche avvenute in una certa data
        /// </summary>
        /// <typeparam name="T">Deve essere una classe che eredita dalla classe astratta "Notification"</typeparam>
        /// <param name="date">data presa in considerazione</param>
        /// <returns>Lista delle notifiche in quella data</returns>
        public IList<T> GetNotifications<T>(Date date) where T: Notification;

        /// <summary>
        /// Richiede al db tutte le notifiche con un certo testo
        /// </summary>
        /// <typeparam name="T">Deve essere una classe che eredita dalla classe astratta "Notification"</typeparam>
        /// <param name="text">Testo preso in considerazione</param>
        /// <returns>Lista di notifiche con quel testo</returns>
        public IList<T> GetNotifications<T>(String text) where T : Notification;

        /// <summary>
        /// Da chiamare ogni volta che si termina una sessione con Rethink per chiudere in sicurezza la connessione
        /// </summary>
        public void CloseConnection();
    }
}
