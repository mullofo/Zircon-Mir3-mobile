using Client.Controls;
using Client.Models;
using Client.UserModels;
using System.Drawing;
using Library;
using Client.Extentions;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using Client.Envir;
using System;
namespace Client.Scenes.Views
{
    public class LeftBuffBox: DXWindow
    {
        #region Properties
        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        DXLabel BuffInfo;
        #endregion

        /// <summary>
        /// 聊天信息左边透明显示框
        /// </summary>
        public LeftBuffBox()
        {
            Movable = false;
            Opacity = 0F;
            HasTitle = false;
            HasTopBorder = false;
            CloseButton.Visible = false;
            Border = false;
            PassThrough = true;
            Size = new Size(200, 100);
            BuffInfo = new DXLabel
            {
                Parent = this,
                ForeColour = Color.Cyan
            };
        }

        public override void Process()
        {
            var list = MapObject.User.Buffs;
            var text = string.Empty;
            if (!Enum.TryParse(Envir.Config.Language, out Language type))
            {
                type = Language.SimplifiedChinese;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var buff = list[i];
                switch (buff.Type)
                {
                    case BuffType.BloodLust:
                    case BuffType.ElementalSuperiority:
                    case BuffType.Resilience:
                    case BuffType.MagicResistance:
                        var stats = buff.Stats;
                        foreach (KeyValuePair<Stat, int> pair in stats.Values)
                        {
                            if (pair.Key == Stat.Duration) continue;

                            string temp = stats.GetShortDisplay(pair.Key);

                            if (temp == null) continue;
                            var len = type == Language.SimplifiedChinese ? temp.Length * 2 : temp.Length;
                            temp = temp + "".PadLeft(20 - len);
                            text += $"\n{temp}";
                        }
                        
                        
                        text += $"{(int)buff.RemainingTime.TotalSeconds}";
                        break;
                }
            }
            BuffInfo.Text = text;
        }


    }
}
