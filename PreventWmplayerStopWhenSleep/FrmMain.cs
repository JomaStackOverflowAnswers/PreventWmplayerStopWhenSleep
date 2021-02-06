using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PreventSleep
{
    public partial class FrmMain : Form
    {
        const int REFRESH_MILLISECONDS = 30000;//Chance this value. 30000ms = 30s.

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
        private Timer timerRefresh;

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

            timerRefresh = new Timer();
            timerRefresh.Enabled = true;
            timerRefresh.Interval = 1000;
            timerRefresh.Tick += new EventHandler(this.timerRefresh_Tick);
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            if((stopwatch.ElapsedMilliseconds / REFRESH_MILLISECONDS) == 1 )
            {
                stopwatch.Restart();
                SetThreadExecutionState(ExecutionState.ES_CONTINUOUS | ExecutionState.ES_AWAYMODE_REQUIRED ); //Display is OFF but the system is active.
                //SetThreadExecutionState(ExecutionState.ES_CONTINUOUS | ExecutionState.ES_AWAYMODE_REQUIRED | ExecutionState.ES_DISPLAY_REQUIRED); //Display is allways ON.
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopwatch.Stop();
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            SetThreadExecutionState(ExecutionState.ES_CONTINUOUS);
        }
    }
}

/*
 References:

https://docs.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-setthreadexecutionstate
 */
