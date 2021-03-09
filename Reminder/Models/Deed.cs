using System;

namespace Reminder.Models
{
    /// <summary>
    /// Model of deed object
    /// </summary>
    public class Deed
    {
        /// <summary>
        /// Saves the text of notification
        /// </summary>
        public string Notification { get; set; }
        /// <summary>
        /// Saves the time when to remind
        /// </summary>
        public DateTime Time { get; set; } = DateTime.Now;
    }
}
