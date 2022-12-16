using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace DatePicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string INI_FILENAME = "time.ini";

        public string iniFilePath = System.IO.Directory.GetCurrentDirectory() + INI_FILENAME;
        public MainWindow()
        {
            InitializeComponent();
            textbox.Text = iniFIle.Read(iniFilePath, "Time Section", "Text1");
        }

        private void input_Clicks_SQL(List<DateTime> sortedDates)
        {
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\BUGS\\Github\\DatePicker\\clicks.mdf;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Open the connection
                connection.Open();

                string query = $"INSERT INTO Counts (NumberDates) VALUES ({sortedDates.Count()});";

                SqlCommand command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();

                query = "SELECT SCOPE_IDENTITY() as lastClickId";
                command = new SqlCommand(query, connection);
                int lastClickId = 0;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    lastClickId = int.Parse(reader["lastClickId"].ToString());
                }
                foreach (DateTime dt in sortedDates)
                {
                    TimeSpan duration = dt.Subtract(DateTime.Now.Date);

                    query = $"INSERT INTO Clicks (ClickId, Distance, WeekOfDay) VALUES "
                            + $"({lastClickId},"
                            + $"{duration.TotalDays},"
                            + $"{(int)dt.DayOfWeek})";
                    System.Diagnostics.Debug.WriteLine(query);
                    command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();
                }

                // Close the connection
                connection.Close();
            }
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            List<DateTime> dates = calendar.SelectedDates.ToList<DateTime>();
            dates.Sort();
            List<string> dateStrings = dates.Select(date => date.ToString("d")).ToList();
            string toClipboard = String.Join(" " + textbox.Text + Environment.NewLine, dateStrings) + 
                " " + textbox.Text;
            input_Clicks_SQL(dates);
            save_inifile(textbox.Text);
            Clipboard.SetText(toClipboard);
            this.Close();
        }

        void save_inifile(string value)
        {
            iniFIle.Write(iniFilePath, "Time Section", "Text1", value);
        }
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                button_Click(sender, e);
            }
        }
        private void calendar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            if (Mouse.Captured is System.Windows.Controls.Calendar || Mouse.Captured is System.Windows.Controls.Primitives.CalendarItem)
            {
                Mouse.Capture(null);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            calendar.BlackoutDates.AddDatesInPast();
        }

        private void calendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.RemovedDate);

        }
    }
}
