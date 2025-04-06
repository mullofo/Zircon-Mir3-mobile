using System;
using System.Text;

namespace Patch
{
    public class PatchViewModel
    {
        public Action<int> CurrentUpdated;

        public Action<int> TotalUpdated;

        public Action<string> LoadTextChange;

        public Action<string> DownloadTextChange;

        /// <summary>
        /// 进度条长度
        /// </summary>
        public int MaxWidth { get; set; } = 150;

        DateTime LastSpeedCheck { get; set; } = DateTime.Now;

        long LastDownloadProcess { get; set; }

        public bool Game145 { get; set; } = true;
        public bool GameKorean { get; set; } = false;


        //public bool IsComplate => TotalWidth >= MaxWidth;
        public double TotalPercent => TotalWidth == 0 ? 0 : Math.Round(TotalWidth * 1f / MaxWidth * 100, 2);

        private string _downloadText;
        public string DownloadText
        {
            get => _downloadText;
            set
            {
                _downloadText = value;
                DownloadTextChange?.Invoke(_downloadText);
            }
        }

        private string _loadText;
        public string LoadText
        {
            get => _loadText;
            set
            {
                _loadText = value;
                LoadTextChange?.Invoke(_loadText);
            }
        }
        private string _speedText;
        public string SpeedText
        {
            get => _speedText;
            set
            {
                _speedText = value;
            }
        }

        private long _totalDownload;
        public long TotalDownload
        {
            get => _totalDownload;
            set
            {
                _totalDownload = value;
            }
        }

        private long _currentProgress;
        public long CurrentProgress
        {
            get => _currentProgress;
            set
            {
                _currentProgress = value;
            }
        }

        private long _totalProgress;
        public long TotalProgress
        {
            get => _totalProgress;
            set
            {
                _totalProgress = value;
            }
        }

        private int _currentWidth;
        public int CurrentWidth
        {
            get => _currentWidth;
            set
            {
                _currentWidth = value;
                CurrentUpdated?.Invoke(_currentWidth);
            }
        }

        private int _totalWidth;
        public int TotalWidth
        {
            get => _totalWidth;
            set
            {
                _totalWidth = value;
                TotalUpdated?.Invoke(_totalWidth);
            }
        }

        public void Update()
        {
            const decimal KB = 1024;
            const decimal MB = KB * 1024;
            const decimal GB = MB * 1024;

            long progress = TotalProgress + CurrentProgress;

            StringBuilder text = new StringBuilder();

            if (progress > GB)
                text.Append($"{progress / GB:#,##0.0}GB");
            else if (progress > MB)
                text.Append($"{progress / MB:#,##0.0}MB");
            else if (progress > KB)
                text.Append($"{progress / KB:#,##0}KB");
            else
                text.Append($"{progress:#,##0}B");

            if (TotalDownload > GB)
                text.Append($" / {TotalDownload / GB:#,##0.0}GB");
            else if (TotalDownload > MB)
                text.Append($" / {TotalDownload / MB:#,##0.0}MB");
            else if (TotalDownload > KB)
                text.Append($" / {TotalDownload / KB:#,##0}KB");
            else
                text.Append($" / {TotalDownload:#,##0}B");

            var downloadSizeStr = text.ToString();

            if (TotalDownload > 0)
                TotalWidth = (int)(progress * 1F / TotalDownload * MaxWidth);

            //long speed = (progress - LastDownloadProcess) * TimeSpan.TicksPerSecond / (DateTime.Now.Ticks - LastSpeedCheck.Ticks); //May cause errors?
            LastDownloadProcess = progress;

            /*if (speed > GB)
            {
                speedText.Content = $"{speed / GB:#,##0.0} GB/S";

            }
            else if (speed > MB)
            {
                speedText.Content = $"{speed / MB:#,##0.0} MB/s";
            }
            else if (speed > KB)
            {
                speedText.Content = $"{speed / KB:#,##0} KB/s";
            }
            else
            {
                speedText.Content = $"{speed:#,##0} Byte/s";

            }*/
            DownloadText = downloadSizeStr;
            LastSpeedCheck = DateTime.Now;
        }
    }
}
