using Library;
using NLog;
using Server.DBModels;
using Server.Envir;
using System;

namespace Server.Utils.Logging
{
    public sealed class CurrencyLogEntry
    {
        public CurrencyLogEntry()
        {

        }
        public CurrencyLogEntry(LogLevel logLevel, string component, DateTime time, CharacterInfo character, CurrencyType currency, CurrencyAction action, CurrencySource source, decimal amount, string extraInfo)
        {
            LogLevel = logLevel;
            Component = component;
            Time = time;
            Character = character;
            Currency = currency;
            Action = action;
            Source = source;
            Amount = amount;
            ExtraInfo = extraInfo;
        }

        public LogLevel LogLevel { get; set; }
        public string Component { get; set; } = "不适用";
        public DateTime Time { get; set; } = SEnvir.Now;
        public CharacterInfo Character { get; set; }
        public CurrencyType Currency { get; set; }
        public CurrencyAction Action { get; set; }
        public CurrencySource Source { get; set; }

        public decimal Amount
        {
            get { return _Amount; }
            set
            {
                if (Math.Abs(value) > Config.GoldChangeAlert)
                    SEnvir.Log($"预警：{Character} ，金币值变化 {value} ");
                _Amount = value;
            }
        }

        private decimal _Amount;
        public string ExtraInfo { get; set; } = string.Empty;
    }
}