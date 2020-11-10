using RethinkDb.Driver.Ast;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rethink.Model
{
    public interface INotificationsManager
    {
        /// <summary>
        /// Ritorno l'id più alto delle notifiche presenti sul db
        /// </summary>
        /// <returns>Id dell'ultima notifica</returns>
        public int GetIdLastNotification();

        /// <summary>
        /// Ritorna l'id più alto delle notifiche di esecuzione presenti sul db
        /// </summary>
        /// <returns>Id dell'ultima notifica di esecuzione</returns>
        public int GetIdLastNotificationExecution();

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
        public T GetNotification<T>(int id) where T : Notification;

        /// <summary>
        /// Elimina la notifica con l'id passato in input
        /// </summary>
        /// <param name="id">id notifica</param>
        public void DeleteNotification(int id);

        /// <summary>
        /// Richiede al db tutte le notifiche avvenute in una certa data
        /// </summary>
        /// <typeparam name="T">Deve essere una classe che eredita dalla classe astratta "Notification"</typeparam>
        /// <param name="date">data presa in considerazione</param>
        /// <returns>Lista delle notifiche in quella data</returns>
        public IList<T> GetNotifications<T>(Date date) where T : Notification;

        /// <summary>
        /// Richiede al db tutte le notifiche con un certo testo
        /// </summary>
        /// <typeparam name="T">Deve essere una classe che eredita dalla classe astratta "Notification"</typeparam>
        /// <param name="text">Testo preso in considerazione</param>
        /// <returns>Lista di notifiche con quel testo</returns>
        public IList<T> GetNotifications<T>(String text) where T : Notification;
    }
}
