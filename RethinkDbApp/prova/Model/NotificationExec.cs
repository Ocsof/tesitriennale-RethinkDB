using System;

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
        public int IdExec { get; set; }

        public override String ToString()
        {
            string result = "Id: " + this.Id.ToString() + " Date: " + this.Date.ToString() + " Text: " + this.Text + 
                            " Type: " + this.Type.ToString() + " IdExec: " + this.IdExec.ToString();
            return result;
        }
    }
}
