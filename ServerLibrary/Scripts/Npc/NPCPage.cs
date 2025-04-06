using Library;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Server.Scripts.Npc
{

    public class NPCPage : INPCPage
    {

        public NPCDialogType DialogType { get; set; }

        public string Key;
        public List<INPCSegment> SegmentList { get; set; } = new List<INPCSegment>();
        /// <summary>
        /// 无用
        /// </summary>
        [Obsolete]
        public List<ClientNPCGood> Goods { get; set; }
        /// <summary>
        /// 无用
        /// </summary>
        [Obsolete]
        public List<ItemType> Types { get; set; }

        public List<string> Args = new List<string>();
        public List<string> Buttons = new List<string>();

        public List<int> ScriptCalls = new List<int>();

        public bool BreakFromSegments = false;

        public NPCPage(string key)
        {
            Key = key;
        }

        public string ArgumentParse(string key)
        {
            if (key.StartsWith("[@_")) return key; //默认的NPC页面，所以不以这种方式使用参数

            Regex r = new Regex(@"\((.*)\)");

            Match match = r.Match(key);
            if (!match.Success) return key;

            key = Regex.Replace(key, r.ToString(), "()");

            string strValues = match.Groups[1].Value;
            string[] arrValues = strValues.Split(',');

            Args = new List<string>();

            foreach (var t in arrValues)
                Args.Add(t);

            return key;
        }
    }
}