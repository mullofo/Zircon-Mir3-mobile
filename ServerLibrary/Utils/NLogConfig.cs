using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using System;

namespace Server.Utils
{
    public static class NLogConfig
    {
        public static LoggingConfiguration GetCsvLoggerConfig(string logName)
        {
            LoggingConfiguration config = new LoggingConfiguration();
            FileTarget fileTarget = GetCsvLoggerFileTarget(logName);

            config.AddTarget(fileTarget);
            config.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget);

            return config;
        }

        private static CsvLayout GetCsvLoggerLayout()
        {
            CsvLayout csvLayout = new CsvLayout
            {
                Delimiter = CsvColumnDelimiterMode.Comma,
                WithHeader = true,
            };
            csvLayout.Columns.Add(new CsvColumn { Name = "日志等级", Layout = "${level:upperCase=true}", Quoting = CsvQuotingMode.Nothing });
            csvLayout.Columns.Add(new CsvColumn { Name = "模块", Layout = "${event-properties:item=component", Quoting = CsvQuotingMode.Nothing });

            csvLayout.Columns.Add(new CsvColumn { Name = "时间", Layout = "${date:format=yyyy年MM月dd日 hh时mm分ss秒}", Quoting = CsvQuotingMode.Nothing });
            csvLayout.Columns.Add(new CsvColumn { Name = "玩家名", Layout = "${event-properties:item=name}", Quoting = CsvQuotingMode.Nothing });
            csvLayout.Columns.Add(new CsvColumn { Name = "玩家账号", Layout = "${event-properties:item=account}", Quoting = CsvQuotingMode.Auto });
            csvLayout.Columns.Add(new CsvColumn { Name = "上次登录IP", Layout = "${event-properties:item=ip}", Quoting = CsvQuotingMode.Nothing });

            csvLayout.Columns.Add(new CsvColumn { Name = "货币", Layout = "${event-properties:item=currency}", Quoting = CsvQuotingMode.Auto });
            csvLayout.Columns.Add(new CsvColumn { Name = "动作", Layout = "${event-properties:item=action}", Quoting = CsvQuotingMode.Auto });
            csvLayout.Columns.Add(new CsvColumn { Name = "数额", Layout = "${event-properties:item=amount}", Quoting = CsvQuotingMode.Nothing });
            csvLayout.Columns.Add(new CsvColumn { Name = "来源", Layout = "${event-properties:item=source}", Quoting = CsvQuotingMode.Auto });
            csvLayout.Columns.Add(new CsvColumn { Name = "其他信息", Layout = "${event-properties:item=extra}", Quoting = CsvQuotingMode.Auto });

            return csvLayout;
        }

        private static FileTarget GetCsvLoggerFileTarget(string logName)
        {
            FileTarget fileTarget = new FileTarget(logName);
            CsvLayout csvLayout = GetCsvLoggerLayout();

            fileTarget.FileName = "${basedir}/Log/" + logName + $"{DateTime.Now.Month}_{DateTime.Now.Day}.csv";
            fileTarget.Layout = csvLayout;

            fileTarget.ArchiveEvery = FileArchivePeriod.Day;
            fileTarget.ArchiveDateFormat = "yyyyMMdd";
            fileTarget.ArchiveNumbering = ArchiveNumberingMode.Date;
            fileTarget.MaxArchiveFiles = 0;
            fileTarget.CreateDirs = true;
            fileTarget.ArchiveFileName = "${basedir}/Log/${longdate}" + logName + ".csv";
            fileTarget.KeepFileOpen = false;

            return fileTarget;
        }

    }
}