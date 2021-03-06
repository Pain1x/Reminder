using Reminder.Commands;
using Reminder.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace Reminder.ViewModels
{
    class DeedViewModel : INotifyPropertyChanged
    {
        delegate void UpdateCollection(ObservableCollection<Deed> collection);
        /// <summary>
        /// Implementation of INotifyPropertyChanged interface
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Implementation of INotifyPropertyChanged interface
        /// </summary>
        /// <param name="prop"></param>
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private RelayCommand addCommand;
        private Deed deed;
        private Dispatcher dispatcher;
        private Timer timer;
        public DeedViewModel()
        {
            Affairs = new ObservableCollection<Deed>();
            deed = new Deed();
            dispatcher = Dispatcher.CurrentDispatcher;
        }
        #region Public Properties
        public ObservableCollection<Deed> Affairs { get; set; }
        /// <summary>
        /// Saves the notification message
        /// </summary>
        public string Notification
        {
            get
            {
                return deed.Notification;
            }
            set
            {
                deed.Notification = value;
                OnPropertyChanged("Notification");
            }
        }
        /// <summary>
        /// Saves the time when to remind
        /// </summary>
        public DateTime Time
        {
            get
            {
                return deed.Time;
            }
            set
            {
                deed.Time = value;
                OnPropertyChanged("Time");
            }
        }
        #endregion
        #region Commands
        /// <summary>
        /// Adds an deed to the collection of affairs
        /// </summary>
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand(obj =>
                  {
                      CreateNewTimer();
                      Deed element = new Deed
                      {
                          Notification = Notification,
                          Time = Time
                  };                      
                      Affairs.Add(element);
                      SaveTheListOfActs(Affairs);
                  }));

            }
        }
        #endregion
        #region Private Methods
        /// <summary>
        /// Creates new timer
        /// </summary>
        private void CreateNewTimer()
        {
            timer = new Timer()
            {
                Interval = 1000
            };
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }
        /// <summary>
        /// Removes an item from collection of affairs
        /// </summary>
        /// <param name="collection"></param>
        private void RemoveFromCollection(ObservableCollection<Deed> collection)
        {
            collection.RemoveAt(0);
        }

        /// <summary>
        /// Occurs when the time has passed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            if (Affairs.Count > 0) 
            {
                Deed compareTime = Affairs.ElementAtOrDefault(0);
                if (currentTime.Hour == compareTime.Time.Hour && currentTime.Minute == compareTime.Time.Minute && currentTime.Second == compareTime.Time.Second)
                {
                    timer.Stop();
                    try
                    {
                        UpdateCollection upd = RemoveFromCollection;
                        if (!dispatcher.CheckAccess())
                            dispatcher.Invoke(upd, Affairs);
                        Notify(compareTime);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        /// <summary>
        /// Saves the collection of affairs in xml file
        /// </summary>
        /// <param name="collection"></param>
        private void SaveTheListOfActs(ObservableCollection<Deed> collection)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(ObservableCollection<Deed>));
            string path = ResolvePath(@".\SerializedObjects\ListOfActs.xml");
            TextWriter txtWriter = new StreamWriter(path);
            formatter.Serialize(txtWriter, collection);
            txtWriter.Close();
        }

        /// <summary>
        /// Gets the path of a file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string ResolvePath(string filePath)
        {
            if (Path.IsPathRooted(filePath))
            {
                return filePath;
            }
            else
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
            }
        }

        /// <summary>
        /// Shows the notification
        /// </summary>
        /// <param name="element"></param>
        private void Notify(Deed element)
        {
            var notification = element.Notification + " at " + element.Time.Hour + " " + element.Time.Minute;
            MessageBox.Show(notification);
        }
    }
    #endregion
}

