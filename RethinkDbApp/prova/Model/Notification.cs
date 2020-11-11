using Newtonsoft.Json;
using RethinkDb.Driver.Ast;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rethink.Model
{
    public abstract class Notification
    {
        protected Notification()
        {
            
        }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]  //penso sia qualcosa relativo al fatto che id no null
        public int Id { get; set; }

        /// <summary>
        /// Data generazione notifica
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Descrizione della notifica
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Tipo di notifica, 1 se è di esecuzione, 2 se è di NewDate
        /// </summary>
        public string Type { get; set; }

        public override String ToString()
        {
            string result = "Id: " + this.Id.ToString() + " Date: " + this.Date.ToString() + " Text: " + this.Text + " Type: " + this.Type.ToString();
            return result;
        }
    }
}
