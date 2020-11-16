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
        public Guid IdExec { get; set; }

        public override String ToString()
        {
            string result = base.ToString() + " IdExec: " + this.IdExec.ToString();
            return result;
        }
    }
}
