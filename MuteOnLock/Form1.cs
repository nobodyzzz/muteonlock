using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace MuteOnLock
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int WM_APPCOMMAND = 0x319;
        private bool runOnStartUp_;

        public Form1()
        {
            InitializeComponent();
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
            this.ShowInTaskbar = false;
            notifyIcon1.Visible = true;
            notifyIcon1.Text = "Mute On Lock";
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            runOnStartUp_ = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run").GetValue(Application.ProductName) != null;
            runOnStartUp.Checked = runOnStartUp_;
        }

        private void SystemEvents_SessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock || e.Reason == SessionSwitchReason.SessionUnlock)
            {
                SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, (IntPtr)APPCOMMAND_VOLUME_MUTE);
            }
        }

        private void RunOnStartUp_Click(object sender, EventArgs e)
        {
            RegistryKey runKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

            if (runOnStartUp_)
            {
                runKey.DeleteValue(Application.ProductName);
                runOnStartUp_ = false;

            }
            else
            {
                runKey.SetValue(Application.ProductName, Application.ExecutablePath);
                runOnStartUp_ = true;
            }
            runOnStartUp.Checked = runOnStartUp_;
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
           Hide();
        }

    }
    
}
