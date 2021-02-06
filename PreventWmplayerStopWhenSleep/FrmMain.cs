using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PreventSleep
{
    public partial class FrmMain : Form
    {
        const int REFRESH_MILLISECONDS = 10000;

        [Flags]
        public enum ExecutionState : uint 
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
        }

        
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)] 
        static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);



        Stopwatch stopwatch;
        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
            axWindowsMediaPlayer1.URL = Directory.GetCurrentDirectory() + "\\Music.mp3";
            axWindowsMediaPlayer1.settings.setMode("loop", true);
            axWindowsMediaPlayer1.Ctlcontrols.play();
            axWindowsMediaPlayer1.uiMode = "none";
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            labelElapsed.Text = stopwatch.ElapsedMilliseconds / 1000 + " seconds.";
            if((stopwatch.ElapsedMilliseconds / REFRESH_MILLISECONDS) == 1 )
            {
                labelElapsed.Text = "SetThreadExecutionState is called.";
                stopwatch.Restart();
                SetThreadExecutionState(ExecutionState.ES_CONTINUOUS | ExecutionState.ES_AWAYMODE_REQUIRED);
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            SetThreadExecutionState(ExecutionState.ES_CONTINUOUS);
        }
    }
}

/*
 References:

https://docs.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-setthreadexecutionstate
 */
