using System.Collections.Generic;

namespace Server.Scripts.Npc
{
    /// <summary>
    /// NPC判断函数
    /// </summary>
    public class NPCChecks
    {
        public CheckType Type;
        public List<string> Params = new List<string>();

        public NPCChecks(CheckType check, params string[] p)
        {
            Type = check;

            for (int i = 0; i < p.Length; i++)
                Params.Add(p[i]);
        }
    }

    public enum CheckType
    {
        /// <summary>
        /// csharp 脚本判断方法
        /// </summary>
        CheckCS,
        /// <summary>
        /// 检测是否GM
        /// </summary>
        IsAdmin,
        /// <summary>
        /// 检测玩家的等级 CheckLevel 0 可返回D0 %P9
        /// </summary>
        CheckLevel,
        /// <summary>
        /// 检测物品有多少数量
        /// </summary>
        CheckItem,
        /// <summary>
        /// 检测金币数量
        /// </summary>
        CheckGold,
        /// <summary>
        /// 检测元宝数量
        /// </summary>
        CheckGameGold,
        /// <summary>
        /// 检测行会金币(新)
        /// </summary>
        CheckGuildGold,

        CheckCredit,
        CheckGender,
        CheckClass,
        CheckDay,
        /// <summary>
        /// 检测小时
        /// </summary>
        CheckHour,
        CheckMinute,
        CheckNameList,
        /// <summary>
        /// 检测PK点数
        /// </summary>
        CheckPkPoint,
        CheckRange,
        Check,
        CheckHum,
        CheckMon,
        CheckMonName,
        CheckExactMon,
        Random,
        Groupleader,
        GroupCount,
        GroupCheckNearby,
        PetLevel,
        PetCount,
        CheckCalc,
        InGuild,
        CheckMap,
        CheckQuest,
        CheckRelationship,
        CheckWeddingRing,
        CheckPet,
        HasBagSpace,
        IsNewHuman,
        CheckConquest,
        AffordGuard,
        AffordGate,
        AffordWall,
        AffordSiege,
        CheckPermission,
        ConquestAvailable,
        ConquestOwner,
        CheckGuildNameList,
        CheckHorse,
        CheckNPShow,
        CheckNPHide
    }
}
