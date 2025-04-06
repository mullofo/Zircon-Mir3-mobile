using System.Diagnostics;

namespace LibraryEditor
{
    public static class ProcessMonitor
    {
        private static Process ThisApplication = Process.GetCurrentProcess();

        public static void RefreshProcessStats()
        {
            ProcessMonitor.ThisApplication.Refresh();
        }

        public static string GetBytesReadable(long i)
        {
            long num1 = i < 0L ? -i : i;
            string str;
            double num2;
            if (num1 >= 1152921504606846976L)
            {
                str = "EB";
                num2 = (double)(i >> 50);
            }
            else if (num1 >= 1125899906842624L)
            {
                str = "PB";
                num2 = (double)(i >> 40);
            }
            else if (num1 >= 1099511627776L)
            {
                str = "TB";
                num2 = (double)(i >> 30);
            }
            else if (num1 >= 1073741824L)
            {
                str = "GB";
                num2 = (double)(i >> 20);
            }
            else if (num1 >= 1048576L)
            {
                str = "MB";
                num2 = (double)(i >> 10);
            }
            else
            {
                if (num1 < 1024L)
                    return i.ToString("0 B");
                str = "KB";
                num2 = (double)i;
            }
            return (num2 / 1024.0).ToString("0.### ") + str;
        }

        public static string GetPhysicalMemoryUsage(bool Refresh = false)
        {
            if (Refresh)
                ProcessMonitor.RefreshProcessStats();
            return GetBytesReadable(ProcessMonitor.ThisApplication.WorkingSet64);
        }

        public static string GetBasePriority(bool Refresh = false)
        {
            if (Refresh)
                ProcessMonitor.RefreshProcessStats();
            return ProcessMonitor.ThisApplication.BasePriority.ToString();
        }

        public static string GetPriorityClass(bool Refresh = false)
        {
            if (Refresh)
                ProcessMonitor.RefreshProcessStats();
            return ProcessMonitor.ThisApplication.PriorityClass.ToString();
        }

        public static string GetProcessorTime(bool Refresh = false)
        {
            if (Refresh)
                ProcessMonitor.RefreshProcessStats();
            return ProcessMonitor.ThisApplication.UserProcessorTime.ToString();
        }

        public static string GetPrivilegedProcessorTime(bool Refresh = false)
        {
            if (Refresh)
                ProcessMonitor.RefreshProcessStats();
            return ProcessMonitor.ThisApplication.PrivilegedProcessorTime.ToString();
        }

        public static string GetPagedSystemMemorySize64(bool Refresh = false)
        {
            if (Refresh)
                ProcessMonitor.RefreshProcessStats();
            return GetBytesReadable(ProcessMonitor.ThisApplication.PagedSystemMemorySize64);
        }

        public static string GetPagedMemorySize64(bool Refresh = false)
        {
            if (Refresh)
                ProcessMonitor.RefreshProcessStats();
            return GetBytesReadable(ProcessMonitor.ThisApplication.PagedMemorySize64);
        }

        public static string GetHandle(bool Refresh = false)
        {
            if (Refresh)
                ProcessMonitor.RefreshProcessStats();
            return ProcessMonitor.ThisApplication.Handle.ToString();
        }

        public static string GetHandleCount(bool Refresh = false)
        {
            if (Refresh)
                ProcessMonitor.RefreshProcessStats();
            return ProcessMonitor.ThisApplication.HandleCount.ToString();
        }

        public static string GetID(bool Refresh = false)
        {
            if (Refresh)
                ProcessMonitor.RefreshProcessStats();
            return ProcessMonitor.ThisApplication.Id.ToString();
        }

        public static string GetSessionId(bool Refresh = false)
        {
            if (Refresh)
                ProcessMonitor.RefreshProcessStats();
            return ProcessMonitor.ThisApplication.SessionId.ToString();
        }
    }
}

