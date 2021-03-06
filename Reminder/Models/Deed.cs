using System;

namespace Reminder.Models
{
    public class Deed
    {
        public string Notification { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
    }
}
