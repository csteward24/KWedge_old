﻿using System;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;
using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing;


namespace KWedge
{
    public partial class Form1 : Form
    {
        public delegate void DataStateEvent(string data);
        public DataStateEvent datastate;
        private SerialHandler serial;
        private Thread serialThread;
        private bool serialRunning = false;
        public Form1()
        {
            InitializeComponent();
            EnumeratePorts();
            datastate = new DataStateEvent(GetData);
            //AcceptButton = btnConnect;
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(HandleSessionSwitch);
        }
        ~Form1()
        {
            SystemEvents.SessionSwitch -= HandleSessionSwitch;
        }
        private void Form1_Load(object sender, EventArgs e) { }
        private void Form1_FormClosed(Object sender, FormClosedEventArgs e)
        {
            EndSerialThread();
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            //if the form is minimized  
            //hide it from the task bar  
            //and show the system tray icon (represented by the NotifyIcon control)  
            Debug.WriteLine("Resize detected");
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(1000);
            }
        }
        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }
        private void GetData(String str)
        {
            //lblData.Text = str;
            try
            {
                if (this != Form.ActiveForm)
                {
                    SendKeys.Send(str);
                }
                else
                {
                    Debug.WriteLine("Form has focus, supressing keystrokes");
                }
            }
            catch (System.ComponentModel.Win32Exception)
            {
                Debug.WriteLine("Could not send keystrokes");
            }
        }
        private void EnumeratePorts()
        {
            lstPorts.DataSource = SerialPort.GetPortNames();
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            if (!serialRunning)
            {
                StartSerialThread();
                
            }
            else
            {
                EndSerialThread();
                
            }
        }
        private void StartSerialThread()
        {
            serial = new SerialHandler(this, lstPorts.Text);
            serialThread = new Thread(serial.Serial);
            serialThread.Start();
            serialRunning = true;
            btnConnect.BackColor = Color.Crimson;
            btnConnect.Text = "Disconnect";
            lstPorts.Enabled = false;
        }
        private void EndSerialThread()
        {
            serial.ClosePort();
            serialThread.Join();
            serialRunning = false;
            btnConnect.BackColor = Color.Gainsboro;
            btnConnect.Text = "Connect";
            lstPorts.Enabled = true;
        }
        void HandleSessionSwitch(object sender, SessionSwitchEventArgs a)
        {
            if (a.Reason == SessionSwitchReason.SessionLock){
                Debug.WriteLine("Lock detected, closing port");
                EndSerialThread();

            }
            if (a.Reason == SessionSwitchReason.SessionUnlock)
            {
                Debug.WriteLine("Unlock detected, starting serial thread");
                if (!serialRunning)
                {
                    StartSerialThread();
                    btnConnect.Enabled = false;
                    serialRunning = true;
                }

            }
        }

        private void LogToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
