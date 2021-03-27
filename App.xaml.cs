using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Forms = System.Windows.Forms;
using System.Drawing;

namespace ChampionLock
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Forms.NotifyIcon _notifyIcon;
        public App()
        {
            _notifyIcon = new Forms.NotifyIcon();
        }
        protected override void OnStartup(StartupEventArgs e)
        {

            _notifyIcon.Icon = new Icon(@"Icons\ChampionLock_icon3.ico");
            _notifyIcon.Text = "ChampionLock";
            _notifyIcon.MouseClick += NotifyIcon_Click;

            _notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Status", Image.FromFile(@"Icons\ChampionLock_icon1.ico"), OnStatusClicked);
            _notifyIcon.ContextMenuStrip.Items.Add(new Forms.ToolStripLabel("Status: Running"));
            _notifyIcon.ContextMenuStrip.Items.Add(new Forms.ToolStripButton("Status: Running"));
            _notifyIcon.ContextMenuStrip.Items.Add(new Forms.ToolStripDropDownButton("Status: Running", 
                null, 
                new Forms.ToolStripLabel("Hello"),
                new Forms.ToolStripLabel("There")));
            
            
            MainWindow = new MainWindow();
            MainWindow.Show();


            _notifyIcon.Visible = true;

            base.OnStartup(e);
        }
        private void NotifyIcon_Click(object sender, Forms.MouseEventArgs e)
        {
            if (e.Button == Forms.MouseButtons.Left)
            {
                MainWindow.WindowState = WindowState.Normal;
                MainWindow.Show();
                MainWindow.Activate();
            } else if (e.Button == Forms.MouseButtons.Right)
            {
                _notifyIcon.ContextMenuStrip.Show();
            }
        }

        private void OnStatusClicked(object sender, EventArgs e)
        {
            MessageBox.Show("Lock is running", "Status", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        protected override void OnExit(ExitEventArgs e)
        {

            _notifyIcon.Dispose();
            base.OnExit(e);
        }
    }
}
