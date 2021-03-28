using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
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
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.ComponentModel;

namespace ChampionLock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public string GlobalPassword = File.ReadAllText(@"data\password.txt").Trim().Replace("\n","");
        public static bool active = false;
        public static string hi = "stupid";
        DispatcherTimer timer;
        // XML editiing
        public static void XmlLockFileAdder(string filename, string name)
        {
            var app = new XElement("app", new XElement("name", name));
            var doc = new XDocument();

            if (File.Exists(filename))
            {
                doc = XDocument.Load(filename);
                doc.Element("applist").Add(app);
            }
            else
            {
                doc = new XDocument(new XElement("applist", app));
            }
            doc.Save(filename);
        }
        private static void XmlLockFileDeleter(string filename, string name)
        {
            XDocument doc = XDocument.Load(filename);
            foreach (var app in doc.Element("applist").Elements("app"))
            {
                if (app.Element("name").Value == name)
                {
                    app.Remove();
                }
            }
            doc.Save(filename);
        }
        private List<string> XmlLockFileReader()
        {
            List<string> Blacklist = new List<string>();
            var applist = XElement.Load("data\\app_stuff.xml");
            foreach (var app in applist.Elements("app"))
            {
                Blacklist.Add(app.Element("name").Value.ToString());
            }
            return Blacklist;
        }

        // Not in use anymore, TBD
        private bool ValueNameExists(string[] valueNames, string valueName)
        {
            foreach (string s in valueNames)
            {
                if (s.ToLower() == valueName.ToLower()) return true;
            }

            return false;
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

        // Window
        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += timer_Tick;
        }

        // XAML interactions

        // App controls
        private void closeApp(object sender, MouseButtonEventArgs e)
        {
            if (active == false)
            {
                try
                {
                    Close();
                    this.closeButton.Cursor = Cursors.None;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            } else
            {
                this.closeButton.ToolTip = "This app cannot be closed. Minimize instead.";
                this.closeButton.Cursor = Cursors.No;
            }
        }
        private void minimizeApp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.WindowState = WindowState.Minimized;
                Hide();
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
        private void ViewListButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> blacklist = XmlLockFileReader();
            string messagelist = "";
            foreach (string app in blacklist)
            {
                messagelist = messagelist + app + "\n";
            }
            MessageBox.Show(messagelist);
        }
        private void ForceKillbutton_Click(object sender, RoutedEventArgs e)
        {
            Process[] plist = Process.GetProcesses();
            foreach (Process procss in plist)
            {
                Console.WriteLine(procss.ProcessName.ToString());
                var blacklist = XmlLockFileReader();
                string appname = procss.ProcessName;
                appname = appname.ToLower();
                foreach (string lockedapp in blacklist)
                {
                    if (appname.CompareTo(lockedapp) == 0)
                    {
                        try
                        {
                            procss.Kill();
                        } catch (Exception f)
                        {
                            Console.WriteLine("Something happened, but your app was most likely locked :)\n");
                            Console.WriteLine(f.Message);
                        }

                    }
                }
            }
            MessageBox.Show("No blocked apps should be open anymore.");
        }
        private void LookAtList_Click(object sender, RoutedEventArgs e)
        {
            Process[] plist = Process.GetProcesses();
            List<string> messagelist = new List<string>();
            string fileName = @"ProcessList.txt";
            foreach (Process process in plist)
            {
                messagelist.Add(process.ProcessName);
            }
            string[] lines = messagelist.ToArray<string>();
            File.WriteAllLines(fileName, lines);
            Process.Start(fileName);
        }
        private void PasswordEnterButton_Click(object sender, RoutedEventArgs e)
        {
            // stop
            if (active == true)
            {
                if (this.PasswordInputBox.Password == File.ReadAllText(@"data\password.txt").Trim().Replace("\n", ""))
                {
                    active = false;
                    this.PasswordEnterButton.Content = "Lock";
                    this.ActiveBar.Background = new SolidColorBrush(Color.FromRgb(153,238,255));
                    this.ActiveBar.Text = "The locker is not active";
                    this.PasswordInputBox.Password = "";
                    timer.Stop();
                }
                else
                {
                    MessageBox.Show("Password is incorrect", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            // start
            else if (active == false)
            {
                if (this.PasswordInputBox.Password != string.Empty && this.PasswordInputBox.Password.Length >= 4)
                {
                    string pass = this.PasswordInputBox.Password.ToString();
                    File.WriteAllText(@"data\password.txt", pass);
                    active = true;
                    this.PasswordEnterButton.Content = "Unlock";
                    this.ActiveBar.Background = new SolidColorBrush(Colors.Red);
                    this.ActiveBar.Text = "The locker is now active";
                    this.PasswordInputBox.Password = "";
                    timer.Start();
                } else if (this.PasswordInputBox.Password == string.Empty)
                {
                    MessageBox.Show("Please put a password!", "Password Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                } else if (this.PasswordInputBox.Password.Length < 4)
                {
                    MessageBox.Show("Pleas emake your password 4 or more characters", "Password Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        // Blacklisting controls
        private void LockButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.AppTextBox.Text.ToLower() != "type here")
            {
                string apptext = this.AppTextBox.Text.ToString().ToLower();
                XmlLockFileAdder("data\\app_stuff.xml", apptext);
            }
        }
        private void AppTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DisplayAppBlock != null)
            {
                string apptext = this.AppTextBox.Text.ToString();
                Process[] plist = Process.GetProcesses();
                foreach (Process process in plist)
                {
                    if (process.ProcessName.ToLower() == apptext.ToLower())
                    {
                        this.DisplayAppBlock.Text = "This app is lockable. Press Lock to add it to the blacklist.";
                    }  else
                    {
                        this.DisplayAppBlock.Text = "Make sure the app you want to lock is running.";
                    }
                }
            }
        }
        private void UnlockButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.UnLockAppTextBox.Text.ToLower() != "type here")
            {
                XmlLockFileDeleter(@"data\app_stuff.xml", this.UnLockAppTextBox.Text.ToString().ToLower());
            }
        }
        private void UnLockAppTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.UnLockDisplayAppBlock != null)
            {
                this.UnLockDisplayAppBlock.Text = "Press 'View Locked Apps' to see what can be unlocked.";
            }
        }
        
        // ACTIVE LOCK >:)
        void timer_Tick(object sender, EventArgs e)
        {
            string open = File.ReadAllText(@"data\counter.txt").Replace("\n", "").Trim();
            int counter = Convert.ToInt32(open);
            List<string> blacklist = XmlLockFileReader();
            Process[] plist = Process.GetProcesses();
            foreach (Process process in plist)
            {
                foreach (string app in blacklist) 
                {
                    if (process.ProcessName.ToLower() == app)
                    {
                        try
                        {
                            process.Kill();
                            MessageBox.Show($"Haha no >:( {process.ProcessName} is not allowed");
                            counter += 1;
                            File.WriteAllText(@"data\counter.txt", counter.ToString());
                        }
                        catch (Exception f)
                        {
                            Console.WriteLine(f.Message);
                        }
                    } else if (process.ProcessName == "Taskmgr")
                    {
                        try
                        {
                            process.Kill();
                            MessageBox.Show("Haha no >:(, the task manager is locked by default.");
                            counter += 1;
                            File.WriteAllText(@"data\counter.txt", counter.ToString());
                        } 
                        catch (Exception f)
                        {
                            Console.WriteLine(f.Message);
                        }
                    }
                }
            }
        }

    }
}
