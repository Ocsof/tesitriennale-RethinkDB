using RethinkDb.Driver.Ast;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rethink.Model
{
    public interface IDbManager
    {
        
        /// <summary>
        /// Restituisce le tabelle presenti sul db
        /// </summary>
        /// <returns>Tabelle presenti sul db</returns>
        public string GetTablesList();

        /// <summary>
        /// Crea la tabella sul db precedentemente specificato.
        /// </summary>
        /// <param name="tableName">Nome tabella da creare sul db</param>
        public void CreateTable(string tableName);

        /// <summary>
        /// Elimina la tabella sul db specificato inizialmente
        /// </summary>
        /// <param name="tableName">Nome tabella da eliminare</param>
        public void DelateTable(string tableName);

        /// <summary>
        /// Restituisce gli indici della tabella richiesta
        /// </summary>
        /// <param name="tableName">Nome tabella</param>
        /// <returns>Indici della tabella</returns>
        public string GetIndexList(string tableName);

        /// <summary>
        /// Crea Indice su un campo di una tabella di un db
        /// </summary>
        /// <param name="dbName">Nome db</param>
        /// <param name="tableName">Nome tabella</param>
        /// <param name="indexName">Nome Indice da creare</param>
        public void CreateIndex(string tableName, string indexName);

        /// <summary>
        /// Elimina indice secondario costruito su un campo della tabella specificata
        /// </summary>
        /// <param name="tableName">Nome tabella</param>
        /// <param name="indexName">Nome dell'indice</param>
        public void DeleteIndex(string tableName, string indexName);

        /// <summary>
        /// Riconfigura il numero di shard e di repliche per ogni tabella sul db
        /// </summary>
        /// <param name="shards"></param>
        /// <param name="replicas"></param>
        public void Reconfigure(int shards, int replicas);

        
        

    }
}
