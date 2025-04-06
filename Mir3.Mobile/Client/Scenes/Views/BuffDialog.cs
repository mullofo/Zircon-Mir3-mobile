using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace Client.Scenes.Views
{
    /// <summary>
    /// BUFF显示框
    /// </summary>
    public sealed class BuffDialog : DXWindow
    {
        #region Properties
        //private DXLabel FPSLabel, PINLabel;
        //private DateTime FPSTime;
        private Dictionary<ClientBuffInfo, DXImageControl> Icons = new Dictionary<ClientBuffInfo, DXImageControl>();

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => true;
        #endregion

        /// <summary>
        /// BUFF界面
        /// </summary>
        public BuffDialog()
        {
            HasTitle = false;               //不显示标题
            HasFooter = false;              //不显示页脚
            HasTopBorder = false;           //不显示上边框
            TitleLabel.Visible = false;     //不显示标签文本
            CloseButton.Visible = false;    //不显示关闭按钮
            Opacity = 0F;                   //完全透明

            Size s = MonoGame.Extended.TextRenderer.MeasureText(null, "F", new MonoGame.Extended.Font(Config.FontName, CEnvir.FontSize(9F)));
            Size = new Size(65, 30 + s.Height + s.Height);        //固定大小

            //FPSLabel = new DXLabel
            //{
            //    Parent = this,
            //    BackColour = Color.FromArgb(125, 50, 50, 50),
            //    Size = new Size(65, s.Height),
            //    AutoSize = false,
            //    Outline = false,
            //    ForeColour = Color.Yellow,
            //    IsControl = false,
            //};

            //PINLabel = new DXLabel
            //{
            //    Parent = this,
            //    BackColour = Color.FromArgb(125, 50, 50, 50),
            //    Size = new Size(65, s.Height),
            //    AutoSize = false,
            //    Outline = false,
            //    ForeColour = Color.SkyBlue,
            //    IsControl = false,
            //};
        }

        #region Methods
        /// <summary>
        /// BUFF改变时
        /// </summary>
        public void BuffsChanged()
        {
            foreach (DXImageControl control in Icons.Values)
                control.Dispose();

            Icons.Clear();

            List<ClientBuffInfo> buffs = MapObject.User.Buffs.ToList();

            Stats permStats = new Stats();

            for (int i = buffs.Count - 1; i >= 0; i--)
            {
                ClientBuffInfo buff = buffs[i];

                switch (buff.Type)
                {
                    case BuffType.ItemBuff:
                        if (buff.RemainingTime != TimeSpan.MaxValue) continue;

                        permStats.Add(Globals.ItemInfoList.Binding.First(x => x.Index == buff.ItemIndex).Stats);

                        buffs.Remove(buff);
                        break;
                    case BuffType.Ranking:
                    case BuffType.Developer:
                        buffs.Remove(buff);
                        break;
                    case BuffType.HuntGold:
                        if (buff.Stats[Stat.AvailableHuntGoldCap] <= 0 || buff.RemainingTime >= TimeSpan.FromSeconds(99999))
                        {
                            buffs.Remove(buff);
                        }
                        break;
                    case BuffType.RewardPool:
                        buffs.Remove(buff);
                        break;
                }
            }

            if (permStats.Count > 0)
                buffs.Add(new ClientBuffInfo { Index = 0, Stats = permStats, Type = BuffType.ItemBuffPermanent, RemainingTime = TimeSpan.MaxValue });

            buffs.Sort((x1, x2) => x2.RemainingTime.CompareTo(x1.RemainingTime));

            foreach (ClientBuffInfo buff in buffs)
            {
                if (!CEnvir.ClientControl.BufferMapEffectShow && buff.Type == BuffType.MapEffect) continue;

                DXImageControl icon;
                Icons[buff] = icon = new DXImageControl
                {
                    Parent = this,
                    LibraryFile = LibraryFile.CBIcon,
                };

                switch (buff.Type)  //BUFF对应显示的图标
                {
                    case BuffType.Heal:
                        icon.Index = 78;
                        break;
                    case BuffType.Invisibility:
                        icon.Index = 74;
                        break;
                    case BuffType.MagicResistance:
                        icon.Index = 92;
                        break;
                    case BuffType.Resilience:
                        icon.Index = 91;
                        break;
                    case BuffType.PoisonousCloud:
                        icon.Index = 98;
                        break;
                    case BuffType.Castle:
                        icon.Index = 242;
                        break;
                    case BuffType.FullBloom:
                        icon.Index = 162;
                        break;
                    case BuffType.WhiteLotus:
                        icon.Index = 163;
                        break;
                    case BuffType.RedLotus:
                        icon.Index = 164;
                        break;
                    case BuffType.MagicShield:
                        icon.Index = 100;
                        break;
                    case BuffType.FrostBite:
                        icon.Index = 221;
                        break;
                    case BuffType.ElementalSuperiority:
                        icon.Index = 93;
                        break;
                    case BuffType.BloodLust:
                        icon.Index = 90;
                        break;
                    case BuffType.Cloak:
                        icon.Index = 160;
                        break;
                    case BuffType.GhostWalk:
                        icon.Index = 160;
                        break;
                    case BuffType.Observable:
                        icon.Index = 172;
                        break;
                    case BuffType.TheNewBeginning:
                        icon.Index = 166;
                        break;
                    case BuffType.Veteran:
                        icon.Index = 171;
                        break;
                    case BuffType.Brown:
                        icon.Index = 229;
                        break;
                    case BuffType.PKPoint:
                        icon.Index = 266;
                        break;
                    case BuffType.Redemption:
                        icon.Index = 258;
                        break;
                    case BuffType.Renounce:
                        icon.Index = 94;
                        break;
                    case BuffType.Defiance:
                        icon.Index = 97;
                        break;
                    case BuffType.Might:
                        icon.Index = 96;
                        break;
                    case BuffType.ReflectDamage:
                        icon.Index = 98;
                        break;
                    case BuffType.Endurance:
                        icon.Index = 95;
                        break;
                    case BuffType.JudgementOfHeaven:
                        icon.Index = 99;
                        break;
                    case BuffType.StrengthOfFaith:
                        icon.Index = 141;
                        break;
                    case BuffType.CelestialLight:
                        icon.Index = 142;
                        break;
                    case BuffType.Transparency:
                        icon.Index = 160;
                        break;
                    case BuffType.LifeSteal:
                        icon.Index = 98;
                        break;
                    case BuffType.DarkConversion:
                        icon.Index = 166;
                        break;
                    case BuffType.DragonRepulse:
                        icon.Index = 165;
                        break;
                    case BuffType.Evasion:
                        icon.Index = 167;
                        break;
                    case BuffType.RagingWind:
                        icon.Index = 168;
                        break;
                    case BuffType.MagicWeakness:
                        icon.Index = 182;
                        break;
                    case BuffType.ItemBuff:
                        icon.Index = Globals.ItemInfoList.Binding.First(x => x.Index == buff.ItemIndex).BuffIcon;
                        break;
                    case BuffType.PvPCurse:
                        icon.Index = 241;
                        break;
                    case BuffType.ItemBuffPermanent:
                        icon.Index = 81;
                        break;
                    case BuffType.HuntGold:
                        icon.Index = 264;
                        break;
                    case BuffType.Companion:
                        switch (GameScene.Game.Companion.CompanionInfo.MonsterInfo.Image)
                        {
                            case MonsterImage.Companion_Pig:
                                icon.Index = 137;
                                break;
                            case MonsterImage.Companion_TuskLord:
                                icon.Index = 146;
                                break;
                            case MonsterImage.Companion_SkeletonLord:
                                icon.Index = 147;
                                break;
                            case MonsterImage.Companion_Dragon:
                                icon.Index = 158;
                                break;
                            case MonsterImage.Companion_Donkey:
                                icon.Index = 159;
                                break;
                            case MonsterImage.Companion_Sheep:
                                icon.Index = 285;
                                break;
                            case MonsterImage.Companion_Griffin:
                            case MonsterImage.Companion_BanyoLordGuzak:
                                icon.Index = 173;
                                break;
                            case MonsterImage.Companion_Panda:
                                icon.Index = 291;
                                break;
                            case MonsterImage.Companion_Rabbit:
                                icon.Index = 174;
                                break;
                            default:
                                icon.Index = 137;
                                break;
                        }
                        break;
                    case BuffType.MapEffect:
                        icon.Index = 76;
                        break;
                    case BuffType.Guild:
                        icon.Index = 140;
                        break;
                    case BuffType.Group:
                        icon.Index = 141;
                        break;
                    case BuffType.Invincibility:
                        icon.Index = 143;
                        break;
                    case BuffType.ElementalHurricane:
                        icon.Index = 98;
                        break;
                    case BuffType.SuperiorMagicShield:
                        icon.Index = 161;
                        break;
                    case BuffType.Concentration:
                        icon.Index = 201;
                        break;
                    case BuffType.SuperTransparency:
                        icon.Index = 203;
                        break;
                    case BuffType.FishingMaster: //姜太公buff图标
                        icon.Index = 170;
                        break;
                    case BuffType.AfterImages:
                        icon.Index = 80;
                        break;
                    case BuffType.CustomBuff:   //自定义BUFF图标=序号
                    case BuffType.TarzanBuff:
                    case BuffType.EventBuff:
                        CustomBuffInfo customBuff = Globals.CustomBuffInfoList.Binding.FirstOrDefault(x => x.Index == buff.FromCustomBuff);
                        if (customBuff != null)
                        {
                            icon.Index = customBuff.SmallBuffIcon;
                        }
                        break;
                    case BuffType.PierceBuff:
                        icon.Index = 90;
                        break;
                    case BuffType.BurnBuff:
                        icon.Index = 95;
                        break;
                    default:
                        icon.Index = 73;
                        break;
                }

                icon.ProcessAction = () =>
                {
                    if (MouseControl == icon)
                        icon.Hint = GetBuffHint(buff);
                };
            }

            const int iconsPerRow = 6;
            int count = 0;
            for (int i = 0; i < buffs.Count; i++)
            {
                //位置 倒序 5个图标 每行6个一排 最多6行 图标距离25
                if (!CEnvir.ClientControl.BufferMapEffectShow && buffs[i].Type == BuffType.MapEffect) continue;
                Icons[buffs[i]].Location = new Point(3 + (Math.Min(iconsPerRow - 1, Icons.Count - 1) - (count % iconsPerRow)) * 25, 3 + (count / iconsPerRow) * 25);
                count++;
            }

            //buf 位置算法不对

            //新算法
            int Dvalue = 0;
            int IconWidth = 27;
            if (Icons.Count >= iconsPerRow) Dvalue = IconWidth * iconsPerRow;
            else Dvalue = IconWidth * Icons.Count;
            //Point Pos = new Point(GameScene.Game.MiniMapBox.Location.X - Dvalue - 3 - 5, GameScene.Game.Location.Y);
            //Location = Pos;

            //Size = new Size(Math.Max(FPSLabel.Size.Width, 3 + Math.Min(iconsPerRow, Math.Max(1, Icons.Count)) * 25), 3 + Math.Max(1, 1 + (Icons.Count - 1) / iconsPerRow) * 25 + FPSLabel.Size.Height * 2 + 3 + 1);
            Size = new Size(3 + Math.Min(iconsPerRow, Math.Max(1, Icons.Count)) * 25, 3 + Math.Max(1, 1 + (Icons.Count - 1) / iconsPerRow) * 25);

            //FPS和Ping标签
            //FPSLabel.Location = new Point(Size.Width - FPSLabel.Size.Width, Size.Height - PINLabel.Size.Height - FPSLabel.Size.Height - 1);
            //PINLabel.Location = new Point(Size.Width - PINLabel.Size.Width, Size.Height - PINLabel.Size.Height);

#if Mobile
#else
            if (GameScene.Game.MiniMapBox != null)
            {
                GameScene.Game.MiniMapBox.UpdateBuffBoxLocation();
            }
#endif
        }
        /// <summary>
        /// 设置BUFF提示信息
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        private string GetBuffHint(ClientBuffInfo buff)
        {
            string text = string.Empty;

            Stats stats = buff.Stats;
            //BUFF效果说明
            switch (buff.Type)
            {
                case BuffType.Observable:
                    text = buff.Type.Lang() + "\n\n" +
                           $"允许他人观看你游戏".Lang() + "\n";
                    break;
                case BuffType.Invisibility:
                    text = buff.Type.Lang() + "\n\n";

                    text += $"最显眼的地方也是最佳藏身之处".Lang() + "\n";
                    break;
                case BuffType.GhostWalk:
                    text = buff.Type.Lang() + "\n\n" +
                           $"让你在隐形的情况下移动得更快".Lang();
                    break;
                case BuffType.MagicWeakness:
                    text = buff.Type.Lang() + "\n\n" +
                           $"你的魔法抗性大大降低".Lang() + "\n";
                    break;
                case BuffType.ElementalHurricane:
                    text = buff.Type.Lang() + "\n\n" +
                           $"施法过程中无法移动".Lang() + "\n";
                    break;
                case BuffType.Companion:
                    text = buff.Type.Lang() + "\n\n" +
                           $"自动捡取物品".Lang() + "\n";
                    break;
                case BuffType.ItemBuff:
                    ItemInfo info = Globals.ItemInfoList.Binding.First(x => x.Index == buff.ItemIndex);
                    text = info.Lang(p => p.ItemName) + "\n";
                    stats = info.Stats;
                    break;
                case BuffType.CustomBuff:             //自定义BUFF名字=说明名字
                case BuffType.TarzanBuff:
                case BuffType.EventBuff:
                    CustomBuffInfo customBuff = Globals.CustomBuffInfoList.Binding.FirstOrDefault(x => x.Index == buff.FromCustomBuff);
                    if (customBuff != null)
                    {
                        text = customBuff.Lang(p => p.BuffName) + "\n";
                    }
                    break;
                case BuffType.PierceBuff:
                    text = buff.Type.Lang() + "\n\n" +
                           $"你的强元素大大降低".Lang();
                    break;
                case BuffType.BurnBuff:
                    text = buff.Type.Lang() + "\n\n" +
                           $"你的攻击大大降低".Lang();
                    break;
                default:
                    text = buff.Type.Lang() + "\n";
                    break;
            }

            if (stats != null && stats.Count > 0)
            {
                foreach (KeyValuePair<Stat, int> pair in stats.Values)
                {
                    if (pair.Key == Stat.Duration) continue;

                    string temp = stats.GetDisplay(pair.Key);

                    if (temp == null) continue;
                    text += $"\n{temp}";
                }

                if (buff.RemainingTime != TimeSpan.MaxValue)
                    text += $"\n";
            }

            if (buff.RemainingTime != TimeSpan.MaxValue)
                text += $"\n" + "剩余时间".Lang() + ":" + buff.RemainingTime.Lang(true);

            if (buff.Pause) text += "\n" + "暂停".Lang() + "（" + "无效".Lang() + "）";

            return text;
        }
        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            foreach (KeyValuePair<ClientBuffInfo, DXImageControl> pair in Icons)
            {
                if (pair.Key.Pause)
                {
                    pair.Value.ForeColour = Color.IndianRed;
                    continue;
                }
                if (pair.Key.RemainingTime == TimeSpan.MaxValue) continue;

                if (pair.Key.RemainingTime.TotalSeconds >= 10)
                {
                    pair.Value.ForeColour = Color.White;
                    continue;
                }

                float rate = pair.Key.RemainingTime.Milliseconds / (float)1000;

                pair.Value.ForeColour = Functions.Lerp(Color.White, Color.CadetBlue, rate);
            }

            Hint = Icons.Count > 0 ? null : "Buff区域".Lang();

            //if (Time.Now >= FPSTime)
            //{
            //    FPSLabel.Text = $"FPS:" + $"{CEnvir.FPSCount}";
            //    PINLabel.Text = $"PIN:" + $"{CEnvir.Connection?.Ping}";
            //    FPSTime = Time.Now.AddSeconds(1);
            //}
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                foreach (KeyValuePair<ClientBuffInfo, DXImageControl> pair in Icons)
                {
                    if (pair.Value == null) continue;

                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }

                //if (FPSLabel != null)
                //{
                //    if (!FPSLabel.IsDisposed)
                //        FPSLabel.Dispose();

                //    FPSLabel = null;
                //}

                //if (PINLabel != null)
                //{
                //    if (!PINLabel.IsDisposed)
                //        PINLabel.Dispose();

                //    PINLabel = null;
                //}

                //FPSTime = DateTime.MinValue;
                Icons.Clear();
                Icons = null;
            }
        }
        #endregion
    }
}