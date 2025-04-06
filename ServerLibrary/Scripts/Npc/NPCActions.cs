using System.Collections.Generic;

namespace Server.Scripts.Npc
{
    public class NPCActions
    {
        public ActionType Type;
        public List<string> Params = new List<string>();

        public NPCActions(ActionType action, params string[] p)
        {
            Type = action;

            Params.AddRange(p);
        }
    }

    public enum ActionType
    {
        /// <summary>
        /// 执行Chsarp脚本
        /// </summary>
        ExecCS,
        Move,
        InstanceMove,
        GiveGold,
        TakeGold,
        TakeGameGold,
        GiveGuildGold,
        TakeGuildGold,
        GiveCredit,
        TakeCredit,
        GiveItem,
        TakeItem,
        GiveExp,
        /// <summary>
        /// 获得马匹
        /// </summary>
        GiveHorse,
        /// <summary>
        /// 卖马
        /// </summary>
        TakeHorse,
        GivePet,
        ClearPets,
        AddNameList,
        DelNameList,
        ClearNameList,
        GiveHP,
        GiveMP,
        ChangeLevel,
        SetPkPoint,
        ReducePkPoint,
        IncreasePkPoint,
        ChangeGender,
        ChangeClass,
        LocalMessage,
        Goto,
        Call,
        GiveSkill,
        RemoveSkill,
        Set,
        Param1,
        Param2,
        Param3,
        Mongen,
        GiveMon,
        TimeRecall,
        TimeRecallGroup,
        BreakTimeRecall,
        MonClear,
        GroupRecall,
        GroupTeleport,
        DelayGoto,
        Mov,
        Calc,
        GiveBuff,
        RemoveBuff,
        AddToGuild,
        RemoveFromGuild,
        RefreshEffects,
        ChangeHair,
        CanGainExp,
        ComposeMail,
        AddMailItem,
        AddMailGold,
        SendMail,
        GroupGoto,
        EnterMap,
        GivePearls,
        TakePearls,
        MakeWeddingRing,
        ForceDivorce,
        GlobalMessage,
        LoadValue,
        SaveValue,
        RemovePet,
        ConquestGuard,
        ConquestGate,
        ConquestWall,
        ConquestSiege,
        TakeConquestGold,
        SetConquestRate,
        StartConquest,
        ScheduleConquest,
        OpenGate,
        CloseGate,
        Break,
        AddGuildNameList,
        DelGuildNameList,
        ClearGuildNameList,
        OpenBrowser,
        GetRandomText,
        PlaySound,
        SetTimer,
        ExpireTimer,
        HideNPC,
        ShowNPC
    }
}
