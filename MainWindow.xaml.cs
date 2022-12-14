using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        public const bool SORT_SELECTED_DATES = true;
        public const string INI_FILENAME = "time.ini";

        public string iniFilePath = System.IO.Directory.GetCurrentDirectory() + INI_FILENAME;
        public MainWindow()
        {
            InitializeComponent();
            textbox.Text = iniFIle.Read(iniFilePath, "Time Section", "Text1");
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            List<DateTime> dates = calendar.SelectedDates.ToList<DateTime>();
            if (SORT_SELECTED_DATES) {dates.Sort();}
            List<string> dateStrings = dates.Select(date => date.ToString("d")).ToList();
            string toClipboard = String.Join(" " + textbox.Text + Environment.NewLine, dateStrings) + 
                " " + textbox.Text;
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
            if (Mouse.Captured is Calendar || Mouse.Captured is System.Windows.Controls.Primitives.CalendarItem)
            {
                Mouse.Capture(null);
            }
        }
    }
}
