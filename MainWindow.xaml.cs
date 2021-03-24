using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Xml;
using System.Diagnostics;

namespace ChampionLock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string XmlLockFileReader()
        {
            XmlTextReader xmlReader = new XmlTextReader("ChampionLock\\app_stuff.xaml");
            while (xmlReader.Read())
            {
                if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "name"))
                {
                    string s1 = xmlReader.ReadElementString();
                    MessageBox.Show(s1);
                }
                else
                {
                    return "None";
                }
            }
            return "None";
        }

        private bool ProgramIsRunning(string FullPath)
        {
            string FilePath = System.IO.Path.GetDirectoryName(FullPath);
            string FileName = System.IO.Path.GetFileNameWithoutExtension(FullPath).ToLower();
            bool isRunning = false;

            System.Diagnostics.Process[] pList = System.Diagnostics.Process.GetProcessesByName(FileName);

            foreach (System.Diagnostics.Process p in pList)
            {
                if (p.MainModule.FileName.StartsWith(FilePath, StringComparison.InvariantCultureIgnoreCase))
                {
                    isRunning = true;
                    break;
                }
            }

            return isRunning;
        }

        public MainWindow()
        {
            InitializeComponent();
            //string Text = XmlLockFileReader();
            //MessageBox.Show(Text);

        }

        private void closeApp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void minimizeApp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.WindowState = WindowState.Minimized;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void titleBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = openFileDlg.ShowDialog();
            // Get the selected file name and display in a TextBox.
            // Load content of file in a TextBlock
            if (result == true)
            {
                //FileNameTextBox.Text = openFileDlg.FileName;
                //TextBlock1.Text = System.IO.File.ReadAllText(openFileDlg.FileName);
            }
        }

        private void ViewListButton_Click(object sender, RoutedEventArgs e)
        {
            XmlLockFileReader();
        }

        private void ForceKillbutton_Click(object sender, RoutedEventArgs e)
        {
            Process[] plist = Process.GetProcesses();
            foreach (Process procss in plist)
            {
                string appname = procss.ProcessName;
                appname = appname.ToLower();
                if (appname.CompareTo("jcpicker") == 0)
                {
                    procss.Kill();
                    MessageBox.Show("jcpicker is a blacklisted app. >:)");
                }


            }
        }
    }
}
