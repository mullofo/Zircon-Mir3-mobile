using Library;
using NLog;
using Server.DBModels;
using Server.Envir;
using Server.Utils.Logging;
using System.Linq;

namespace Server.Models
{
    public partial class PlayerObject : MapObject //城堡相关
    {
        public void MakeGoldDeposit(string castleName, long amount)
        {
            if (Character.Account.GuildMember == null) return;
            if (string.IsNullOrEmpty(castleName)) return;
            if (amount <= 0) return;

            CastleFundInfo castle = SEnvir.CastleFundInfoList.Binding.FirstOrDefault(x => x.Castle.Name == castleName);
            if (castle == null) return;

            if (Character.Account.GuildMember.Guild?.Castle?.Name != castleName) return;

            if (amount > Gold)
            {
                // Insufficient fund
                Connection.ReceiveChat("你的资金不足", MessageType.System);
                return;
            }

            ChangeGold(-amount);
            // 记录
            // 构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry()
            {
                LogLevel = LogLevel.Info,
                Component = "城堡系统",
                Time = SEnvir.Now,
                Character = Character,
                Currency = CurrencyType.Gold,
                Action = CurrencyAction.Deduct,
                Source = CurrencySource.ItemAdd,
                Amount = amount,
                ExtraInfo = $"城堡扣除金币"
            };
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry);

            castle.TotalDeposit += amount;
        }

        public void WithdrawCastleFund(string castleName, long amount)
        {
            if (Character.Account.GuildMember == null) return;

            if (string.IsNullOrEmpty(castleName)) return;

            if (amount <= 0) return;

            if ((Character.Account.GuildMember.Permission & GuildPermission.Leader) != GuildPermission.Leader)
            {
                Connection.ReceiveChat("Guild.GuildManagePermission".Lang(Connection.Language), MessageType.System);
                return;
            }

            CastleFundInfo castle = SEnvir.CastleFundInfoList.Binding.FirstOrDefault(x => x.Castle.Name == castleName);
            if (castle == null) return;

            if (Character.Account.GuildMember.Guild?.Castle?.Name != castleName) return;

            if (amount > castle.TotalFund)
            {
                // Insufficient castle fund
                Connection.ReceiveChat("城堡资金不足", MessageType.System);
                return;
            }

            if (castle.TotalTax >= amount)
            {
                castle.TotalTax -= amount;
            }
            else
            {
                castle.TotalDeposit -= amount - castle.TotalTax;
                castle.TotalTax = 0;
            }
            ChangeGold(amount);
            // 记录
            // 构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry()
            {
                LogLevel = LogLevel.Info,
                Component = "城堡系统",
                Time = SEnvir.Now,
                Character = Character,
                Currency = CurrencyType.Gold,
                Action = CurrencyAction.Add,
                Source = CurrencySource.ItemAdd,
                Amount = amount,
                ExtraInfo = $"城堡领袖{castle.Castle.Name}从城堡里提取金币"
            };
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry);

            // Guild leader has withdrawn {amount} gold from castle {castle.Castle.Name}
            foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                member.Account.Connection?.ReceiveChat($"行会领袖从{castle.Castle.Name}提取了{amount}金币", MessageType.System);

        }
    }
}