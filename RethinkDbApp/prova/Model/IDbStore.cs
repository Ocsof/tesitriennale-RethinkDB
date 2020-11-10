using System;
using System.Collections.Generic;
using System.Text;

namespace Rethink.Model
{
    public interface IDbStore
    {


        /// <summary>
        /// Crea la tabella sul db precedentemente specificato.
        /// </summary>
        /// <param name="tableName">Nome tabella da creare sul db</param>
        public void CreateTable(string tableName);


        /// <summary>
        /// Crea Indice su un campo di una tabella di un db
        /// </summary>
        /// <param name="dbName">Nome db</param>
        /// <param name="tableName">Nome tabella</param>
        /// <param name="indexName">Nome Indice da creare</param>
        public void CreateIndex(string tableName, string indexName);

        /// <summary>
        /// Riconfigura il numero di shard e di repliche per ogni tabella sul db
        /// </summary>
        /// <param name="shards"></param>
        /// <param name="replicas"></param>
        public void Reconfigure(int shards, int replicas);

        
        

    }
}
