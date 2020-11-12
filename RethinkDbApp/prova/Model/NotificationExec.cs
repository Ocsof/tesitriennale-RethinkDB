﻿using System;

namespace Rethink.Model
{
    public class NotificationExec : Notification
    {
        public NotificationExec() : base()
        {

        }
        /// <summary>
        /// Id di esecuzione
        /// </summary>
        public Guid IdExec { get; set; }

        public override String ToString()
        {
            string result = "Id: " + this.Id.ToString() + " Date: " + this.Date.ToString() + " Text: " + this.Text + 
                            " Type: " + this.Type + " IdExec: " + this.IdExec.ToString();
            return result;
        }
    }
}
