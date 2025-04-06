using Library;
using System;
using System.Runtime.InteropServices;

namespace Server.Envir
{
    public class AccurateTimer
    {
        private delegate void TimerEventDel(int id, int msg, IntPtr user, int dw1, int dw2);

        private const int TIME_PERIODIC = 1;

        private const int EVENT_TYPE = 1;

        private Action mAction;

        private int mTimerId;

        private int delay;

        private TimerEventDel mHandler;

        //private Thread taskThread = null;

        [DllImport("winmm.dll")]
        private static extern int timeBeginPeriod(int msec);

        [DllImport("winmm.dll")]
        private static extern int timeEndPeriod(int msec);

        [DllImport("winmm.dll")]
        private static extern int timeSetEvent(int delay, int resolution, TimerEventDel handler, IntPtr user, int eventType);

        [DllImport("winmm.dll")]
        private static extern int timeKillEvent(int id);

        public AccurateTimer(Action action, TimeSpan delay)
        {
            int calculatedDelay = Convert.ToInt32(delay.TotalMilliseconds);
            if (calculatedDelay <= 0)
            {
                calculatedDelay = 1;
            }
            InitTimer(action, calculatedDelay);
        }

        public AccurateTimer(Action action, int delay)
        {
            InitTimer(action, delay);
        }

        private void InitTimer(Action action, int delay)
        {
            if (delay <= 0)
            {
                delay = 1;
            }
            this.delay = delay;
            mAction = action;
            timeBeginPeriod(1);
            mHandler = TimerCallback;
            //taskThread = new Thread(InternalCallAction)
            //{
            //	IsBackground = true
            //};
            mTimerId = timeSetEvent(delay, 0, mHandler, IntPtr.Zero, 1);
            //SEnvir.Log($"加载计时器线程 {mAction.Method.Name} 间隔 {delay}ms.");
        }

        public void Stop()
        {
            int err = timeKillEvent(mTimerId);
            timeEndPeriod(1);
        }

        private void TimerCallback(int id, int msg, IntPtr user, int dw1, int dw2)
        {
            if (mTimerId != 0)
            {
                //if (taskThread.ThreadState == ThreadState.Stopped)
                //{
                //	taskThread = new Thread(InternalCallAction)
                //	{
                //		IsBackground = true
                //	};
                //	taskThread.Start();
                //}
                //else if (taskThread.ThreadState == ThreadState.Unstarted || taskThread.ThreadState == (ThreadState.Background | ThreadState.Unstarted))
                //{
                //	taskThread.Start();
                //}
                InternalCallAction();
            }
        }

        private void InternalCallAction()
        {
            SEnvir.Now = Time.Now;
            mAction();
        }
    }
}
