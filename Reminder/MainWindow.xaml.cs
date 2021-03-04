using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;

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

        /// <summary>
        /// Realization of placeholder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBoxRemindMe_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtBoxActs.Text == "Write here what I have to remind")
            {
                txtBoxActs.Text = "";
            }
        }

        /// <summary>
        /// Realization of placeholder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBoxRemindMe_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBoxActs.Text))
            {
                txtBoxActs.Text = "Write here what I have to remind";
            }
        }
    #endregion
        #region Private Methods
        /// <summary>
        /// Occurs when the time has passed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Creates the timer with the business to be done
        /// </summary>
        private void RemindMe()
        {
            CreateNewTimer();
            RemindElement element = new RemindElement(txtBoxActs.Text, dateTimePicker.Value);
            TODOCollection.Add(element);
            SaveTheListOfActs(TODOCollection);
        }
        /// <summary>
        /// Shows the Windows notification
        /// </summary>
        /// <param name="element"></param>
        private void Notify(RemindElement element)
        {
            var notification = element.Notification + " at " + element.Time.Hour + " " + element.Time.Minute;
            MessageBox.Show(notification);
        }
        /// <summary>
        /// Creates a new instance of a timer for given business
        /// </summary>
        private void CreateNewTimer()
        {
            timer = new Timer
            {
                Interval = 1000
            };
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        /// <summary>
        /// Saves the list of acts in xml file
        /// </summary>
        /// <param name="element"></param>
        private void SaveTheListOfActs(ObservableCollection<RemindElement> collection)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(ObservableCollection<RemindElement>));
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
    }
    #endregion
}
