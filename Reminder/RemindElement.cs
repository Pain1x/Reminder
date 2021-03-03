using System;

namespace Reminder
{
    [Serializable]
    public class RemindElement 
    {
        public RemindElement()
        {

        }
        public RemindElement(string notification, DateTime time)
        {
            Notification = notification;
            Time = time;
        }

        public string Notification { get; set; }
        public DateTime Time { get; set; }
    }
}
