using System;
using System.Collections.Generic;
using System.Text;

namespace Rethink.Model
{
    interface IDbStore
    {
        /// <summary>
        /// Inizializzazione db: crea db, tabelle e indici se non già presenti sul server
        /// </summary>
        public void InitializeDatabase();

        /// <summary>
        /// Crea Db sul server
        /// </summary>
        /// <param name="dbName"> Nome del db da creare</param>
        public void CreateDb(string dbName);

        /// <summary>
        /// Crea tabella su un db
        /// </summary>
        /// <param name="dbName">Nome db su cui creare tabella</param>
        /// <param name="tableName">Nome tabella da creare</param>

        public void CreateTable(string dbName, string tableName);

        /// <summary>
        /// Crea Indice su un campo di una tabella di un db
        /// </summary>
        /// <param name="dbName">Nome db</param>
        /// <param name="tableName">Nome tabella</param>
        /// <param name="indexName">Nome Indice da creare</param>
        public void CreateIndex(string dbName, string tableName, string indexName);

        /// <summary>
        /// Inserisci nuovo author nel db
        /// </summary>
        /// <param name="author">Author da inserire</param>
        public void InsertOrUpdateAuthor(Author author);

        /// <summary>
        /// Inserisci nuovo post nel db
        /// </summary>
        /// <param name="post">Post da inserire</param>
        public void InsertOrUpdatePost(Post post);
        
        /// <summary>
        /// Query che ritorna per ogn autore il numero di post che ha scritto
        /// </summary>
        /// <returns></returns>
        public List<AuthorStatus> GetAuthorsStatus();

        /// <summary>
        /// 50 inserimenti su tabella Post
        /// </summary>
        public void MultiInsertPosts();

        /// <summary>
        /// 50 cancellazioni su Post
        /// </summary>
        public void MultiDeletePosts();


    }
}
