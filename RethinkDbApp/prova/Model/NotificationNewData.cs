using System;
using System.Collections.Generic;
using System.Text;

namespace Rethink.Model
{
    public class NotificationNewData: Notification
    {
        /// <summary>
        /// table
        /// </summary>
        public string Table { get; set; }

        public override String ToString()
        {
            string result = "Id: " + this.Id.ToString() + " Date: " + this.Date.ToString() + " Text: " + this.Text + 
                            " Type: " + this.Type + " Table: " + this.Table;
            return result;
        }
    }
}
