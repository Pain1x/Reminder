using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Reminder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// TODO:Method for notifications
    public partial class MainWindow : Window
    {
        #region Private Members
        Timer timer;
        ObservableCollection<RemindElement> TODOCollection;
        delegate void UpdateLabel(Label label, string message);
        delegate void UpdateCollection(ObservableCollection<RemindElement> collection);
        #endregion
        #region UI Events
        /// <summary>
        /// Updates the status label
        /// </summary>
        /// <param name="label">The label to update</param>
        /// <param name="message">The update message</param>
        void UpdateStatusLabel(Label label, string message)
        {
            label.Content = message;
        }
        /// <summary>
        /// Removes the element from collection
        /// </summary>
        /// <param name="collection">The collection to remove from</param>
        void RemoveFromCollection(ObservableCollection<RemindElement> collection)
        {
            collection.RemoveAt(0);
        }
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Allows to move the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        /// <summary>
        /// Occurs when button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            RemindMe();
        }
        /// <summary>
        /// Minimizes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        /// <summary>
        /// Closes the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dateTimePicker.Width = 100;
            TODOCollection = new ObservableCollection<RemindElement>();
            TODODataGrid.ItemsSource = TODOCollection;
        }

        private void txtBoxRemindMe_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtBoxRemindMe.Text == "Write here what I have to remind")
            {
                txtBoxRemindMe.Text = "";
            }
        }

        private void txtBoxRemindMe_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBoxRemindMe.Text))
            {
                txtBoxRemindMe.Text = "Write here what I have to remind";
            }
        }
    #endregion
        #region Private Methods
    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            if (TODOCollection.Count > 0)
            {
                RemindElement compareTime = TODOCollection.ElementAtOrDefault(0);
                if (currentTime.Hour == compareTime.Time.Hour && currentTime.Minute == compareTime.Time.Minute && currentTime.Second == compareTime.Time.Second)
                {
                    timer.Stop();
                    try
                    {
                        UpdateCollection upd = RemoveFromCollection;
                        if (!Dispatcher.CheckAccess())
                            Dispatcher.Invoke(upd, TODOCollection);
                        Notify(compareTime);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                UpdateLabel upd = UpdateStatusLabel;
                if (!Dispatcher.CheckAccess())
                    Dispatcher.Invoke(upd, lblInfo, "I have reminded you everything you wanted");
            }
        }
        private void RemindMe()
        {
            CreateNewTimer();
            RemindElement element = new RemindElement(txtBoxRemindMe.Text, dateTimePicker.Value);
            TODOCollection.Add(element);
        }
        private void Notify(RemindElement element)
        {
            string title = element.Notification + " at " + element.Time.Hour + " " + element.Time.Minute;

            string toastXmlString =
            $@"<toast><visual>
            <binding template='ToastGeneric'>
            <text>{title}</text>
            </binding>
        </visual></toast>";

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(toastXmlString);

            var toastNotification = new ToastNotification(xmlDoc);

            var toastNotifier = ToastNotificationManager.CreateToastNotifier();
            toastNotifier.Show(toastNotification);
        }
        private void CreateNewTimer()
        {
            timer = new Timer
            {
                Interval = 1000
            };
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }
    }
    #endregion
}
