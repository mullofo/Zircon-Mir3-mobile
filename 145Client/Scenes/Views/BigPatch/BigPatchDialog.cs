using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.Scenes.Configs;
using Client.UserModels;
using Library;
using Library.SystemModels;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using Point = System.Drawing.Point;
using Timer = System.Timers.Timer;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 大补贴辅助功能
    /// </summary>
    public sealed class BigPatchDialog : DXWindow
    {
        public override WindowType Type => WindowType.BigPatchWindow;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #region CreateCheckBox
        /// <summary>
        /// 创建复选框
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="Changed"></param>
        /// <param name="Checked"></param>
        /// <returns></returns>
        public static DXCheckBox CreateCheckBox(DXControl parent, string name, int x, int y, EventHandler<EventArgs> Changed, bool Checked = false)
        {
            DXCheckBox box = new DXCheckBox
            {
                Label = { Text = name },
                Parent = parent,
                Checked = Checked,
                Size = new Size(80, 20),
                bAlignRight = false,
            };
            box.Location = new Point(x, y);
            box.CheckedChanged += Changed;
            return box;
        }
        #endregion

        #region DXGroupBox
        /// <summary>
        /// 分组框
        /// </summary>
        public class DXGroupBox : DXControl
        {
            public DXLabel Name;

            /// <summary>
            /// 分组框
            /// </summary>
            public DXGroupBox()
            {
                Name = new DXLabel
                {
                    Parent = this,
                    Outline = true,
                    ForeColour = Color.FromArgb(0x46, 0x3a, 0x23),
                    Location = new Point(5, 10),
                };
                BorderColour = Color.FromArgb(0x46, 0x3a, 0x23);
                //Opacity = 0.1f;
                Border = true;
            }
            /// <summary>
            /// 更新边框信息
            /// </summary>
            protected internal override void UpdateBorderInformation()
            {
                BorderInformation = null;
                if (!Border || DisplayArea.Width == 0 || DisplayArea.Height == 0) return;

                BorderInformation = new[]
                {
                    new Vector2(Name.Size.Width+Name.Location.X, 20),
                    new Vector2(Size.Width -5, 20),
                    new Vector2(Size.Width -5, Size.Height -5),
                    new Vector2(5, Size.Height -5),
                    new Vector2(5, 20)
                };
            }
        }
        #endregion

        #region DXStaticView
        /// <summary>
        /// 静态视图
        /// </summary>
        public class DXStaticView : DXControl
        {
            public int Vspac = 1;
            public int _First, _Last;
            public DXControl view;
            public List<string> contents;
            public DXVScrollBar VScrollBar;
            public int ItemCount
            {
                get => view.Controls.Count;
            }
            /// <summary>
            /// 静态视图
            /// </summary>
            public DXStaticView()
            {
                _First = 0;
                _Last = 0;
                contents = new List<string>();
                view = new DXControl
                {
                    Parent = this,
                };
            }
            /// <summary>
            /// 更新项目
            /// </summary>
            public void UpdateItems()
            {
                int Y = 0;
                int ScrollValue = VScrollBar.Value;
                int _Scrolled = -ScrollValue;
                if (ItemCount > 0)
                {
                    _First = ScrollValue / VScrollBar.Change;
                }

                for (int i = 0; i < _First; i++)
                {
                    view.Controls[i].Visible = false;
                }

                for (int i = _First; Y < Size.Height && i < ItemCount; i++)
                {
                    DXControl Item = view.Controls[i];
                    Item.Location = new Point(0, Y);
                    Item.Size = new Size(view.Size.Width, Item.Size.Height);
                    Y += Item.Size.Height + Vspac;
                    Item.Visible = true;
                    _Last = i;
                }
                for (int i = _Last + 1; i < ItemCount; i++)
                {
                    view.Controls[i].Visible = false;
                }

                VScrollBar.MaxValue = (int)(ItemCount * VScrollBar.Change);
            }
            /// <summary>
            /// 更新滚动条
            /// </summary>
            public void UpdateScrollBar()
            {
                if (ItemCount == 0)
                {
                    VScrollBar.Visible = false;
                    return;
                }
                VScrollBar.Location = new Point(view.Location.X + view.Size.Width, view.Location.Y + 1);
                VScrollBar.VisibleSize = view.Size.Height;
                VScrollBar.Size = new Size(VScrollBar.Size.Width, view.Size.Height + view.Size.Height + 3);
                VScrollBar.Visible = true;
                VScrollBar.Change = DXLabel.DefaultHeight + Vspac;
                int Mode = VScrollBar.VisibleSize % VScrollBar.Change;
                if (Mode > 0) VScrollBar.VisibleSize -= Mode;
            }
            /// <summary>
            /// 尺寸变化
            /// </summary>
            /// <param name="oValue"></param>
            /// <param name="nValue"></param>
            public override void OnSizeChanged(Size oValue, Size nValue)
            {
                base.OnSizeChanged(oValue, nValue);
                UpdateViewRect();
            }
            /// <summary>
            /// 更新视图矩形
            /// </summary>
            public void UpdateViewRect()
            {
                view.Location = new Point(5, 5);
                view.Size = new Size(Size.Width - VScrollBar.Size.Width - 5 * 2, Size.Height - 10);
                UpdateScrollBar();
                UpdateItems();
            }
            /// <summary>
            /// 插入
            /// </summary>
            /// <param name="str"></param>
            /// <param name="pos"></param>
            public void Insert(string str, int pos = -1)
            {
                if (pos >= contents.Count)
                {
                    contents.Add(str);
                }
                else
                {
                    contents.Insert(pos, str);
                }
                UpdateItems();
            }
        }
        #endregion

        #region DXTextView
        /// <summary>
        /// 文本视图
        /// </summary>
        public class DXTextView : DXTextBox
        {
            /// <summary>
            /// 文本视图
            /// </summary>
            public DXTextView()
            {
                TextBox.Visible = false;
                Border = true;
                Editable = true;
                //AllowResize = true,
                TextBox.AcceptsReturn = true;
                TextBox.Multiline = true;
                TextBox.WordWrap = false;
                TextBox.ForeColor = Color.DarkOrange;
                TextBox.ScrollBars = ScrollBars.Vertical;
            }
        }
        #endregion

        #region 常用
        /// <summary>
        /// 常用选项
        /// </summary>
        public class DXCommonlyTab : DXTab
        {
            /// <summary>
            /// 常用分组
            /// </summary>
            public DXGroupBox GroupNormal;
            /// <summary>
            /// 战斗分组
            /// </summary>
            //public DXGroupBox GroupWar;

            /// <summary>
            /// 物品闪烁
            /// </summary>
            //public DXCheckBox ChkItemObjShining;
            /// <summary>
            /// 物品极品光柱
            /// </summary>
            //public DXCheckBox ChkItemObjBeam;
            /// <summary>
            /// 物品显示
            /// </summary>
            //public DXCheckBox ChkItemObjShow;
            /// <summary>
            /// 自动捡取
            /// </summary>
            //public DXCheckBox ChkAutoPick;
            /// <summary>
            /// TAB捡取
            /// </summary>
            //public DXCheckBox ChkTabPick;
            /// <summary>
            /// 道具分组
            /// </summary>
            //public DXGroupBox GroupItem;

            /// <summary>
            /// 天气分组
            /// </summary>
            public DXGroupBox GroupWeather;
            /// <summary>
            /// 天气状态
            /// </summary>
            public DXComboBox CombWeather;

            /// <summary>
            /// 自动攻击分组
            /// </summary>
            //public DXGroupBox GroupAutoAttack;
            /// <summary>
            /// 自动练技能
            /// </summary>
            //public DXCheckBox ChkAutoFire;
            /// <summary>
            /// 自动技能选择
            /// </summary>
            //public DXComboBox CombAutoFire;
            /// <summary>
            /// 间隔时间
            /// </summary>
            //public DXLabel LabAutoFire;
            /// <summary>
            /// 数字设置间隔
            /// </summary>
            //public DXNumberBox NumberAutoFireInterval;

            /// <summary>
            /// 鼠标中键分组
            /// </summary>
            //public DXGroupBox GroupMouseMiddle;
            /// <summary>
            /// 召唤坐骑
            /// </summary>
            //public DXCheckBox ChkCallMounts;
            /// <summary>
            /// 施法
            /// </summary>
            //public DXCheckBox ChkCastingMagic;
            /// <summary>
            /// 中键快捷
            /// </summary>
            //public DXComboBox CombMiddleMouse;

            /// <summary>
            /// 刷新背包按钮
            /// </summary>
            //public DXButton RefreshBag;
            /// <summary>
            /// 重置配置按钮
            /// </summary>
            public DXButton ReloadConfig;

            /// <summary>
            /// 战士分组
            /// </summary>
            public DXGroupBox Warrior;
            /// <summary>
            /// 自动烈火
            /// </summary>
            public DXCheckBox AutoFlamingSword;
            /// <summary>
            /// 自动翔空
            /// </summary>
            public DXCheckBox AutoDragobRise;
            /// <summary>
            /// 自动莲月
            /// </summary>
            public DXCheckBox AutoBladeStorm;
            /// <summary>
            /// 自动屠龙斩
            /// </summary>
            //public DXCheckBox AutoMaelstromBlade;
            /// <summary>
            /// 自动铁布衫
            /// </summary>
            public DXCheckBox AutoDefiance;
            /// <summary>
            /// 自动破血狂杀
            /// </summary>
            public DXCheckBox AutoMight;
            /// <summary>
            /// 自动连招
            /// </summary>
            public DXCheckBox AutoCombo;
            /// <summary>
            /// 连招类型
            /// </summary>
            public DXComboBox ComboType;
            /// <summary>
            /// 第一招
            /// </summary>
            public DXComboBox Combo1;
            /// <summary>
            /// 第二招
            /// </summary>
            public DXComboBox Combo2;
            /// <summary>
            /// 第三招
            /// </summary>
            public DXComboBox Combo3;
            /// <summary>
            /// 第四招
            /// </summary>
            public DXComboBox Combo4;
            /// <summary>
            /// 第五招
            /// </summary>
            public DXComboBox Combo5;

            /// <summary>
            /// 法师分组
            /// </summary>
            public DXGroupBox Wizard;
            /// <summary>
            /// 自动魔法盾
            /// </summary>
            public DXCheckBox AutoMagicShield;
            /// <summary>
            /// 自动凝血
            /// </summary>
            public DXCheckBox AutoRenounce;
            /// <summary>
            /// 自动天打雷劈
            /// </summary>
            //public DXCheckBox AutoThunder;

            /// <summary>
            /// 道士分组
            /// </summary>
            public DXGroupBox Taoist;
            /// <summary>
            /// 自动换毒
            /// </summary>
            public DXCheckBox AutoPoisonDust;
            /// <summary>
            /// 自动换符
            /// </summary>
            public DXCheckBox AutoAmulet;
            /// <summary>
            /// 自动阴阳盾
            /// </summary>
            //public DXCheckBox AutoCelestial;

            /// <summary>
            /// 刺客分组
            /// </summary>
            public DXGroupBox Assassin;
            /// <summary>
            /// 自动四花
            /// </summary>
            public DXCheckBox AutoFourFlowers;
            /// <summary>
            /// 自动风之闪避
            /// </summary>
            public DXCheckBox AutoEvasion;
            /// <summary>
            /// 自动风之守护
            /// </summary>
            public DXCheckBox AutoRagingWind;

            /// <summary>
            /// 聊天组
            /// </summary>
            public DXGroupBox ChatFrame;
            public DXCheckBox ChkAutoSayWords;
            public DXNumberBox AutoSayInterval;
            public Timer SayWordTimer;
            public DXLabel IntervalLeft, IntervalRight;
            public DXTextView InputBox;

            //自动技能
            public DXGroupBox AutoSkill;
            public DXComboBox CombSkill1;
            public DXNumberBox NumbSkill1;
            public DXCheckBox AutoSkill_1;
            public DXComboBox CombSkill2;
            public DXNumberBox NumbSkill2;
            public DXCheckBox AutoSkill_2;
            public List<DXListBoxItem> SkillListBoxItems = new List<DXListBoxItem>();

            /// <summary>
            /// 检查道具设置
            /// </summary>
            public struct CHK_ITEM_SET
            {
                public string name;
                public bool state;
                public EventHandler<EventArgs> method;
            };

            /// <summary>
            /// 常用选项卡
            /// </summary>
            public DXCommonlyTab()
            {
                Border = true;
                int x = 15, y = 30, Yspac = 25;
                GroupNormal = new DXGroupBox
                {
                    Parent = this,
                    Size = new Size(120, 405),
                    Location = new Point(10, 0),
                    Name = { Text = "常用".Lang(), },
                };
                //GroupWar = new DXGroupBox
                //{
                //    Parent = this,
                //    Size = new Size(120, 405),
                //    Location = new Point(10, 0),
                //    Name = { Text = "战斗".Lang(), },
                //    Visible = false,
                //};
                //GroupItem = new DXGroupBox
                //{
                //    Parent = this,
                //    Size = new Size(120, 405),
                //    Location = new Point(10, 0),
                //    Name = { Text = "物品".Lang(), },
                //    Visible = false,
                //};
                GroupWeather = new DXGroupBox
                {
                    Parent = this,
                    Size = new Size(90, 20),
                    Location = new Point(10, 0),
                    Name = { Text = "天气".Lang() },
                };
                //GroupAutoAttack = new DXGroupBox
                //{
                //    Parent = this,
                //    Size = new Size(120, 60),
                //    Location = new Point(10, 0),
                //    Name = { Text = "自动练技能".Lang() },
                //    Visible = false,
                //};
                //GroupMouseMiddle = new DXGroupBox
                //{
                //    Parent = this,
                //    Size = new Size(120, 32),
                //    Location = new Point(10, 0),
                //    Name = { Text = "鼠标中键".Lang() },
                //    Visible = false,
                //};

                #region _GROUP_BOX
                Warrior = new DXGroupBox
                {
                    Parent = this,
                    Size = new Size(120, 32),
                    Location = new Point(10, 0),
                    Name = { Text = "战士".Lang() },
                    Visible = false,
                };
                Wizard = new DXGroupBox
                {
                    Parent = this,
                    Size = new Size(120, 32),
                    Location = new Point(10, 0),
                    Name = { Text = "法师".Lang() },
                    Visible = false,
                };
                Taoist = new DXGroupBox
                {
                    Parent = this,
                    Size = new Size(120, 32),
                    Location = new Point(10, 0),
                    Name = { Text = "道士".Lang() },
                    Visible = false,
                };
                Assassin = new DXGroupBox
                {
                    Parent = this,
                    Size = new Size(120, 32),
                    Location = new Point(10, 0),
                    Name = { Text = "刺客".Lang() },
                    Visible = false,
                };
                #endregion 

                AutoSkill = new DXGroupBox
                {
                    Parent = this,
                    Size = new Size(120, 60),
                    Location = new Point(10, 0),
                    Name = { Text = "自动技能".Lang() },
                };

                ChatFrame = new DXGroupBox
                {
                    Parent = this,
                    Size = new Size(120, 103),
                    Location = new Point(10, 0),
                    Name = { Text = "聊天".Lang() },
                };

                CHK_ITEM_SET[] ChkNormals = new CHK_ITEM_SET[]
                {
                    //new CHK_ITEM_SET{name = "免助跑".Lang(),state=BigPatchConfig.ChkAvertVerb,method= (o,e)=>
                    //{
                    //    BigPatchConfig.ChkAvertVerb=(o as DXCheckBox)?.Checked??false;
                    //}},
                    //new CHK_ITEM_SET{name = "显血".Lang(),state=BigPatchConfig.ShowHealth,method= (o,e)=>
                    //{
                    //    BigPatchConfig.ShowHealth=(o as DXCheckBox)?.Checked??false;
                    //}},
                    //new CHK_ITEM_SET{name = "数字显血".Lang(),state=BigPatchConfig.ChkShowHPBar,method= (o,e)=>
                    //{
                    //    BigPatchConfig.ChkShowHPBar=(o as DXCheckBox)?.Checked??false;
                    //}},
                    //new CHK_ITEM_SET{name = "免蜡烛".Lang(),state=BigPatchConfig.ChkAvertBright,method= (o,e)=>
                    //{
                    //    if (GameScene.Game.Observer) return;
                    //    BigPatchConfig.ChkAvertBright=(o as DXCheckBox)?.Checked??false;
                    //    GameScene.Game?.MapControl?.LLayer.UpdateLights();
                    //}},
                    //new CHK_ITEM_SET{name = "名字显示".Lang(),state=BigPatchConfig.ShowPlayerNames,method= (o,e)=>
                    //{
                    //    BigPatchConfig.ShowPlayerNames=(o as DXCheckBox)?.Checked??false;
                    //}},
                    //new CHK_ITEM_SET{name = "综合数显".Lang(),state=BigPatchConfig.ChkDisplayOthers,method= (o,e)=>
                    //{
                    //    BigPatchConfig.ChkDisplayOthers=(o as DXCheckBox)?.Checked??false;
                    //}},
                    //new CHK_ITEM_SET{name = "快速小退".Lang(),state=BigPatchConfig.ChkQuickSelect,method= (o,e)=>
                    //{
                    //    BigPatchConfig.ChkQuickSelect=(o as DXCheckBox)?.Checked??false;
                    //}},
                    //new CHK_ITEM_SET{name = "雷达显示".Lang(),state=BigPatchConfig.ChkShowObjects,method= (o,e)=>
                    //{
                        //BigPatchConfig.ChkShowObjects=(o as DXCheckBox)?.Checked??false;
                    //}},
                    //new CHK_ITEM_SET{name = "BOSS提醒".Lang(),state=BigPatchConfig.ChkBossWarrning,method= (o,e)=>
                    //{
                    //    BigPatchConfig.ChkBossWarrning=(o as DXCheckBox)?.Checked??false;
                    //}},
                    //new CHK_ITEM_SET{name = "边跑边砍".Lang(),state=BigPatchConfig.ChkRunningHit,method= (o,e)=>
                    //{
                    //    BigPatchConfig.ChkRunningHit=(o as DXCheckBox)?.Checked??false;
                    //}},
                    //new CHK_ITEM_SET{name = "跑不停".Lang(),state=BigPatchConfig.ChkKeepRunning,method= (o,e)=>
                    //{
                    //    BigPatchConfig.ChkKeepRunning=(o as DXCheckBox)?.Checked??false;
                    //}},
                    //new CHK_ITEM_SET{name = "持久警告".Lang(),state=BigPatchConfig.ChkDurableWarning,method= (o,e)=>
                    //{
                    //    BigPatchConfig.ChkDurableWarning=(o as DXCheckBox)?.Checked??false;
                    //}},
                    //new CHK_ITEM_SET{name = "怪物信息".Lang(),state=BigPatchConfig.ChkMonsterInfo,method= (o,e)=>
                    //{
                    //    BigPatchConfig.ChkMonsterInfo=(o as DXCheckBox)?.Checked??false;
                    //}},
                    //new CHK_ITEM_SET{name = "自动关组".Lang(),state=BigPatchConfig.ChkAutoUnAcceptGroup,method= (o,e)=>
                    //{
                    //    BigPatchConfig.ChkAutoUnAcceptGroup=(o as DXCheckBox)?.Checked??false;
                    //}},
                    new CHK_ITEM_SET{name = "清理尸体".Lang(),state=BigPatchConfig.ChkCleanCorpse,method= (o,e)=>
                    {
                        if (GameScene.Game.Observer) return;
                        BigPatchConfig.ChkCleanCorpse=(o as DXCheckBox)?.Checked??false;
                    }},
                    new CHK_ITEM_SET{name = "免SHIFT".Lang(),state=BigPatchConfig.ChkAvertShift,method= (o,e)=>
                    {
                        BigPatchConfig.ChkAvertShift=(o as DXCheckBox)?.Checked??false;
                    }},
                    //new CHK_ITEM_SET{name = "怪名字显示".Lang(),state=BigPatchConfig.ChkMonsterNameTips,method= (o,e)=>
                    //{
                    //    BigPatchConfig.ChkMonsterNameTips=(o as DXCheckBox)?.Checked??false;
                    //}},
                    //new CHK_ITEM_SET{name = "怪等级显示".Lang(),state=BigPatchConfig.ChkMonsterLevelTips,method= (o,e)=>
                    //{
                    //    BigPatchConfig.ChkMonsterLevelTips=(o as DXCheckBox)?.Checked??false;
                    //}},                 
                };
                for (int i = 0; i < ChkNormals.Length; i++)
                {
                    CreateCheckBox(GroupNormal, ChkNormals[i].name, x, y, ChkNormals[i].method, ChkNormals[i].state);
                    y += Yspac;
                }
                GroupNormal.Size = new Size(GroupNormal.Size.Width, y);

                ///////////////////////////////////////////////////////////////////

                //免Shift
                //CHK_ITEM_SET[] ChkWars = new CHK_ITEM_SET[]
                //{
                //new CHK_ITEM_SET{name = "免SHIFT".Lang(),state=BigPatchConfig.ChkAvertShift,method= (o,e)=>
                //{
                //    BigPatchConfig.ChkAvertShift=(o as DXCheckBox)?.Checked??false;
                //}},
                //new CHK_ITEM_SET{name = "攻击锁定".Lang(),state=BigPatchConfig.ChkLockTarget,method= (o,e)=>
                //{
                //    BigPatchConfig.ChkLockTarget=(o as DXCheckBox)?.Checked??false;
                //}},
                //new CHK_ITEM_SET{name = "数字飘血".Lang(),state=BigPatchConfig.ShowDamageNumbers,method= (o,e)=>
                //{
                //    BigPatchConfig.ShowDamageNumbers=(o as DXCheckBox)?.Checked??false;
                //}},
                //new CHK_ITEM_SET{name = "显示目标".Lang(),state=BigPatchConfig.ChkShowTargetInfo,method= (o,e)=>
                //{
                //    BigPatchConfig.ChkShowTargetInfo=(o as DXCheckBox)?.Checked??false;
                //}},
                //new CHK_ITEM_SET{name = "右键单击取消目标".Lang(),state=BigPatchConfig.RightClickDeTarget,method= (o,e)=>
                //{
                //    BigPatchConfig.RightClickDeTarget=(o as DXCheckBox)?.Checked??false;
                //}},
                //new CHK_ITEM_SET{name = "战斗退出(?)",state=BigPatchConfig.ChkQuitWar,method= (o,e)=>
                //{
                //    BigPatchConfig.ChkQuitWar=(o as DXCheckBox)?.Checked??false;
                //}},
                //new CHK_ITEM_SET{name = "避免石化(?)",state=BigPatchConfig.ChkAvertPetrifaction,method= (o,e)=>
                //{
                //    BigPatchConfig.ChkAvertPetrifaction=(o as DXCheckBox)?.Checked??false;
                //}},
                //new CHK_ITEM_SET{name = "稳如泰山".Lang(),state=BigPatchConfig.ChkAvertShake,method= (o,e)=>
                //{
                //    BigPatchConfig.ChkAvertShake=(o as DXCheckBox)?.Checked??false;
                //}},
                //new CHK_ITEM_SET{name = "攻击着色",state=BigPatchConfig.ChkColourTarget,method= (o,e)=>
                //{
                //BigPatchConfig.ChkColourTarget=(o as DXCheckBox)?.Checked??false;
                //}},
                //new CHK_ITEM_SET{name = "队友着色(?)",state=BigPatchConfig.ChkColourFriend,method= (o,e)=>
                //{
                //BigPatchConfig.ChkColourFriend=(o as DXCheckBox)?.Checked??false;
                //}},
                //new CHK_ITEM_SET{name = "施法下马".Lang(),state=BigPatchConfig.ChkDismountToFireMagic,method= (o,e)=>
                //{
                //    BigPatchConfig.ChkDismountToFireMagic=(o as DXCheckBox)?.Checked??false;
                //}},
                //new CHK_ITEM_SET{name = "防范暗杀(?)",state=BigPatchConfig.ChkDefendAssassination,method= (o,e)=>
                //{
                //    BigPatchConfig.ChkDefendAssassination=(o as DXCheckBox)?.Checked??false;
                //}},
                //new CHK_ITEM_SET{name = "经验提示".Lang(),state=BigPatchConfig.ChkCloseExpTips,method= (o,e)=>
                //{
                //    BigPatchConfig.ChkCloseExpTips=(o as DXCheckBox)?.Checked??false;
                //}},
                //new CHK_ITEM_SET{name = "战斗信息提示".Lang(),state=BigPatchConfig.ChkCloseCombatTips,method= (o,e)=>
                //{
                //    BigPatchConfig.ChkCloseCombatTips=(o as DXCheckBox)?.Checked??false;
                //}},
                //new CHK_ITEM_SET{name = "锁怪效果".Lang(),state=BigPatchConfig.ChkLockMonEffect,method= (o,e)=>
                //{
                //    BigPatchConfig.ChkLockMonEffect=(o as DXCheckBox)?.Checked??false;
                //}},
                //new CHK_ITEM_SET{name = "转生残影".Lang(),state=BigPatchConfig.ChkShowRebirthShow,method= (o,e)=>
                //{
                //    BigPatchConfig.ChkShowRebirthShow=(o as DXCheckBox)?.Checked??false;
                //}},
                //new CHK_ITEM_SET{name = "死亡红屏".Lang(),state=BigPatchConfig.ChkDeathRedScreen,method= (o,e)=>
                //{
                //    BigPatchConfig.ChkDeathRedScreen=(o as DXCheckBox)?.Checked??false;
                //}}
                //};
                //y = 30;
                //for (int i = 0; i < ChkWars.Length; i++)
                //{
                //    CreateCheckBox(GroupWar, ChkWars[i].name, x, y, ChkWars[i].method, ChkWars[i].state);
                //    y += Yspac;
                //}
                //GroupWar.Size = new Size(GroupWar.Size.Width, y);


                ////////////////////////////////////////////////////////////////////

                // 物品闪烁
                //y = 30;
                //ChkItemObjShining = CreateCheckBox(GroupItem, "物品闪烁".Lang(), x, y, (o, e) =>
                // {
                //     BigPatchConfig.ChkItemObjShining = ChkItemObjShining.Checked;
                // }, BigPatchConfig.ChkItemObjShining);

                //物品极品光柱
                //y += Yspac;
                //ChkItemObjBeam = CreateCheckBox(GroupItem, "物品极品光柱".Lang(), x, y, (o, e) =>
                //{
                //    BigPatchConfig.ChkItemObjBeam = ChkItemObjBeam.Checked;
                //}, BigPatchConfig.ChkItemObjBeam);

                ////物品显示
                //y += Yspac;
                //ChkItemObjShow = CreateCheckBox(GroupItem, "物品名称显示".Lang(), x, y, (o, e) =>
                //{
                //    BigPatchConfig.ChkItemObjShow = ChkItemObjShow.Checked;
                //}, BigPatchConfig.ChkItemObjShow);

                ////自动捡取
                //y += Yspac;
                //ChkAutoPick = CreateCheckBox(GroupItem, "自动捡取".Lang(), x, y, (o, e) =>
                //{
                //    //if (ChkAutoPick.Checked)
                //    //{
                //    //    ChkTabPick.Checked = !ChkAutoPick.Checked;
                //    //}
                //    BigPatchConfig.ChkAutoPick = ChkAutoPick.Checked;
                //}, BigPatchConfig.ChkAutoPick);

                //Tab捡取
                //y += Yspac;
                //ChkTabPick = CreateCheckBox(GroupItem, "Tab捡取".Lang(), x, y, (o, e) =>
                //{
                //    if (ChkTabPick.Checked)
                //    {
                //        ChkAutoPick.Checked = !ChkTabPick.Checked;
                //    }
                //    BigPatchConfig.ChkTabPick = ChkTabPick.Checked;
                //}, BigPatchConfig.ChkTabPick);
                //GroupItem.Size = new Size(GroupItem.Size.Width, y + Yspac);

                GroupWeather.Location = new Point(GroupNormal.Location.X, y + Yspac);
                y = 10;
                CombWeather = new DXComboBox
                {
                    Parent = GroupWeather,
                };
                CombWeather.Location = new Point(20, 30);
                y += CombWeather.Size.Height + Yspac;
                new DXListBoxItem
                {
                    Parent = CombWeather.ListBox,
                    Label = { Text = $"未选择".Lang() },
                    Item = WeatherSetting.None,
                };
                new DXListBoxItem
                {
                    Parent = CombWeather.ListBox,
                    Label = { Text = $"晴".Lang() },
                    Item = WeatherSetting.Default,
                };
                new DXListBoxItem
                {
                    Parent = CombWeather.ListBox,
                    Label = { Text = $"雾".Lang() },
                    Item = WeatherSetting.Fog,
                };
                //new DXListBoxItem
                //{
                //Parent = CombWeather.ListBox,
                //Label = { Text = $"燃烧的雾-未开放" },
                //Item = WeatherSetting.BurningFog,
                //};
                new DXListBoxItem
                {
                    Parent = CombWeather.ListBox,
                    Label = { Text = $"雪".Lang() },
                    Item = WeatherSetting.Snow,
                };
                //new DXListBoxItem
                //{
                //Parent = CombWeather.ListBox,
                //Label = { Text = $"花瓣雨-未开放" },
                //Item = WeatherSetting.Everfall,
                //};
                new DXListBoxItem
                {
                    Parent = CombWeather.ListBox,
                    Label = { Text = $"雨".Lang() },
                    Item = WeatherSetting.Rain,
                };
                CombWeather.ListBox.SelectItem(BigPatchConfig.Weather);

                CombWeather.SelectedItemChanged += (o, e) =>
                {
                    DXComboBox comb = o as DXComboBox;
                    BigPatchConfig.Weather = (int)(WeatherSetting)comb.ListBox.SelectedItem.Item;
                    GameScene.Game.MapControl.UpdateWeather();
                };
                CombWeather.Size = new Size(55, 18);
                GroupWeather.Size = new Size(CombWeather.Location.X + CombWeather.Size.Width + 15, y + Yspac);
                //////////////////////////////////////////////////////////////////////

                //GroupAutoAttack.Location = new Point(GroupItem.Location.X, GroupItem.Location.Y + GroupItem.Size.Height + 10);
                //y = 30;
                //ChkAutoFire = CreateCheckBox(GroupAutoAttack, "自动练技能".Lang(), x, y, (o, e) =>
                //{
                //    BigPatchConfig.ChkAutoFire = ChkAutoFire.Checked;
                //}, BigPatchConfig.ChkAutoFire);

                //CombAutoFire = new DXComboBox
                //{
                //    Parent = GroupAutoAttack,
                //};
                //new DXListBoxItem
                //{
                //    Parent = CombAutoFire.ListBox,
                //    Label = { Text = "空".Lang() },
                //    Item = 0,
                //};

                //for (int i = 0; i <= 11; i++)
                //{
                //    new DXListBoxItem
                //    {
                //        Parent = CombAutoFire.ListBox,
                //        Label = { Text = $"F{(i + 1).ToString()}" },
                //        Item = i + 1,
                //    };
                //}
                //CombAutoFire.SelectedItemChanged += (o, e) =>
                //{
                //    BigPatchConfig.AutoFire = (int)CombAutoFire.ListBox.SelectedItem.Item;
                //};
                //CombAutoFire.ListBox.SelectItem(BigPatchConfig.AutoFire);
                //BigPatchConfig.AutoFire = CombAutoFire.ListBox.SelectedItem == null ? 0 : (int)CombAutoFire.ListBox.SelectedItem.Item;
                //CombAutoFire.Size = new Size(50, 18);
                //CombAutoFire.Location = new Point(ChkAutoFire.Location.X + ChkAutoFire.Size.Width + 5, ChkAutoFire.Location.Y);

                //y += Yspac;
                //LabAutoFire = new DXLabel
                //{
                //    Parent = GroupAutoAttack,
                //    Text = "间隔:".Lang(),
                //    Location = new Point(x, y),
                //};
                //NumberAutoFireInterval = new DXNumberBox
                //{
                //    Parent = GroupAutoAttack,
                //    Location = new Point(x + LabAutoFire.Size.Width, y),
                //    Size = new Size(80, 20),
                //    ValueTextBox = { Size = new Size(40, 18) },
                //    MaxValue = 100,
                //    MinValue = 1,
                //    Value = BigPatchConfig.AutoFireInterval,
                //    UpButton = { Location = new Point(63, 1) }
                //};
                //NumberAutoFireInterval.ValueTextBox.ValueChanged += (o, e) =>
                //{
                //    BigPatchConfig.AutoFireInterval = NumberAutoFireInterval.Value;
                //};
                //y += Yspac;
                //GroupAutoAttack.Size = new Size(GroupItem.Size.Width, y + 10);

                ///////////////////////////////////////////////////////////////

                //y = 30 + 50;
                //ChkCallMounts = CreateCheckBox(GroupMouseMiddle, "骑马".Lang(), x, y, (o, e) =>
                //{
                //    if (ChkCallMounts.Checked)
                //    {
                //        ChkCastingMagic.Checked = !ChkCallMounts.Checked;
                //    }
                //    BigPatchConfig.ChkCallMounts = ChkCallMounts.Checked;
                //}, BigPatchConfig.ChkCallMounts);
                //x += 75;
                //ChkCastingMagic = CreateCheckBox(GroupMouseMiddle, "魔法".Lang(), x, y, (o, e) =>
                //{
                //    if (ChkCastingMagic.Checked)
                //    {
                //        ChkCallMounts.Checked = !ChkCastingMagic.Checked;
                //    }
                //    BigPatchConfig.ChkCastingMagic = ChkCastingMagic.Checked;
                //}, BigPatchConfig.ChkCastingMagic);

                //x -= 75;
                //y += Yspac;
                //CombMiddleMouse = new DXComboBox
                //{
                //    Parent = GroupMouseMiddle,
                //};
                //new DXListBoxItem
                //{
                //    Parent = CombMiddleMouse.ListBox,
                //    Label = { Text = "空".Lang() },
                //    Item = 0,
                //};

                //for (int i = 0; i <= 11; i++)
                //{
                //    new DXListBoxItem
                //    {
                //        Parent = CombMiddleMouse.ListBox,
                //        Label = { Text = $"F{(i + 1).ToString()}" },
                //        Item = i + 1,
                //    };
                //}
                //CombMiddleMouse.SelectedItemChanged += (o, e) =>
                //{
                //    BigPatchConfig.MiddleMouse = (int)CombMiddleMouse.ListBox.SelectedItem.Item;
                //};
                //CombMiddleMouse.ListBox.SelectItem(BigPatchConfig.MiddleMouse);
                //CombMiddleMouse.Size = new Size(50, 18);
                //CombMiddleMouse.Location = new Point(x, y);

                //y += Yspac;
                //GroupMouseMiddle.Location = new Point(GroupAutoAttack.Location.X, GroupAutoAttack.Location.Y + GroupAutoAttack.Size.Height + 10);
                //GroupMouseMiddle.Size = new Size(GroupAutoAttack.Size.Width, y + 10);

                #region _AUTO_SKILL
                //////////////////////////////////////////////
                // 战士
                y = 5;

                AutoFlamingSword = CreateCheckBox(Warrior, "自动烈火".Lang(), x, y += 25, (o, e) =>
                {
                    BigPatchConfig.AutoFlamingSword = AutoFlamingSword.Checked;
                    if (AutoFlamingSword.Checked)
                        AutoCombo.Checked = !AutoFlamingSword.Checked;
                }, BigPatchConfig.AutoFlamingSword);
                AutoFlamingSword.CheckedChanged += (o, e) =>
                {
                    if (GameScene.Game.Observer) return;

                    //打开烈火 翔空 连月 辅助自动技能关闭
                    if (AutoFlamingSword.Checked)
                    {
                        AutoDragobRise.Checked = AutoCombo.Checked;
                        AutoBladeStorm.Checked = AutoCombo.Checked;
                    }

                    CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = AutoFlamingSword.Checked, Slot = AutoSetConf.SetFlamingSwordBox });
                };

                AutoDragobRise = CreateCheckBox(Warrior, "自动翔空".Lang(), x + 120, y, (o, e) =>
                {
                    BigPatchConfig.AutoDragobRise = AutoDragobRise.Checked;
                    if (AutoDragobRise.Checked)
                        AutoCombo.Checked = !AutoDragobRise.Checked;
                }, BigPatchConfig.AutoDragobRise);
                AutoDragobRise.CheckedChanged += (o, e) =>
                {
                    if (GameScene.Game.Observer) return;

                    //打开翔空  烈火 连月 辅助自动技能关闭
                    if (AutoDragobRise.Checked)
                    {
                        AutoFlamingSword.Checked = AutoCombo.Checked;
                        AutoBladeStorm.Checked = AutoCombo.Checked;
                    }

                    CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = AutoDragobRise.Checked, Slot = AutoSetConf.SetDragobRiseBox });
                };

                AutoBladeStorm = CreateCheckBox(Warrior, "自动莲月".Lang(), x, y += 25, (o, e) =>
                {
                    BigPatchConfig.AutoBladeStorm = AutoBladeStorm.Checked;
                    if (AutoBladeStorm.Checked)
                        AutoCombo.Checked = !AutoBladeStorm.Checked;
                }, BigPatchConfig.AutoBladeStorm);
                AutoBladeStorm.CheckedChanged += (o, e) =>
                {
                    if (GameScene.Game.Observer) return;

                    //打开连月 烈火 翔空 辅助自动技能关闭
                    if (AutoBladeStorm.Checked)
                    {
                        AutoFlamingSword.Checked = AutoCombo.Checked;
                        AutoDragobRise.Checked = AutoCombo.Checked;
                    }

                    CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = AutoBladeStorm.Checked, Slot = AutoSetConf.SetBladeStormBox });
                };

                AutoDefiance = CreateCheckBox(Warrior, "自动铁布衫".Lang(), x + 120, y, (o, e) =>
                {
                    if (AutoDefiance.Checked)
                    {
                        AutoMight.Checked = !AutoDefiance.Checked;
                    }
                    BigPatchConfig.AutoDefiance = AutoDefiance.Checked;
                }, BigPatchConfig.AutoDefiance);

                AutoMight = CreateCheckBox(Warrior, "自动破血".Lang(), x, y += 25, (o, e) =>
                {
                    if (AutoMight.Checked)
                    {
                        AutoDefiance.Checked = !AutoMight.Checked;
                    }
                    BigPatchConfig.AutoMight = AutoMight.Checked;
                }, BigPatchConfig.AutoMight);

                //AutoMaelstromBlade = CreateCheckBox(Warrior, "自动屠龙斩".Lang(), x + 120, y, (o, e) =>
                //{
                //    BigPatchConfig.AutoMaelstromBlade = AutoMaelstromBlade.Checked;
                //    if (AutoMaelstromBlade.Checked)
                //        AutoCombo.Checked = !AutoMaelstromBlade.Checked;
                //}, BigPatchConfig.AutoMaelstromBlade);
                //AutoMaelstromBlade.CheckedChanged += (o, e) =>
                //{
                //    if (GameScene.Game.Observer) return;

                //    CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = AutoMaelstromBlade.Checked, Slot = AutoSetConf.SetMaelstromBlade });
                //};

                AutoCombo = CreateCheckBox(Warrior, "连招".Lang(), x, y += 25, (o, e) =>
                {
                    BigPatchConfig.AutoCombo = AutoCombo.Checked;
                }, BigPatchConfig.AutoCombo);
                AutoCombo.CheckedChanged += (o, e) =>
                {
                    if (GameScene.Game.Observer) return;
                    //打开自动三联 烈火 翔空 连月 屠龙斩 辅助自动技能关闭
                    if (AutoCombo.Checked)
                    {
                        AutoFlamingSword.Checked = !AutoCombo.Checked;
                        AutoDragobRise.Checked = !AutoCombo.Checked;
                        AutoBladeStorm.Checked = !AutoCombo.Checked;
                        //AutoMaelstromBlade.Checked = !AutoCombo.Checked;
                    }
                };
                ComboType = new DXComboBox
                {
                    Parent = Warrior,
                };
                new DXListBoxItem
                {
                    Parent = ComboType.ListBox,
                    Label = { Text = "空".Lang() },
                    Item = 0,
                };
                string[] ActThree = new string[]
                {
                    "二连".Lang(),"三连".Lang(),//"四连".Lang(),"五连".Lang(),
                };
                for (int idx = 0; idx < ActThree.Length; idx++)
                {
                    new DXListBoxItem
                    {
                        Parent = ComboType.ListBox,
                        Label = { Text = ActThree[idx] },
                        Item = idx + 2,
                    };
                }
                ComboType.SelectedItemChanged += (o, e) =>
                {
                    if (GameScene.Game.Observer) return;

                    BigPatchConfig.ComboType = (int)ComboType.ListBox.SelectedItem.Item;

                    switch (BigPatchConfig.ComboType)
                    {
                        case 2:
                            Combo3.Visible = false;
                            Combo4.Visible = false;
                            Combo5.Visible = false;
                            break;
                        case 3:
                            Combo3.Visible = true;
                            Combo4.Visible = false;
                            Combo5.Visible = false;
                            if (BigPatchConfig.Combo1 == BigPatchConfig.Combo2)
                                Combo2.ListBox.SelectItem(MagicType.None);
                            break;
                        case 4:
                            Combo3.Visible = true;
                            Combo4.Visible = true;
                            Combo5.Visible = false;
                            if (BigPatchConfig.Combo1 == BigPatchConfig.Combo2 || BigPatchConfig.Combo1 == BigPatchConfig.Combo3)
                                Combo1.ListBox.SelectItem(MagicType.None);
                            break;
                        case 5:
                            Combo3.Visible = true;
                            Combo4.Visible = true;
                            Combo5.Visible = true;
                            if (BigPatchConfig.Combo1 == BigPatchConfig.Combo2 || BigPatchConfig.Combo1 == BigPatchConfig.Combo3 || BigPatchConfig.Combo1 == BigPatchConfig.Combo4)
                                Combo1.ListBox.SelectItem(MagicType.None);
                            if (BigPatchConfig.Combo2 == BigPatchConfig.Combo4)
                                Combo4.ListBox.SelectItem(MagicType.None);
                            break;
                    }
                };
                ComboType.Size = new Size(80, 18);
                ComboType.Location = new Point(x + 120, y);

                Combo1 = new DXComboBox
                {
                    Parent = Warrior,
                };
                new DXListBoxItem
                {
                    Parent = Combo1.ListBox,
                    Label = { Text = "空".Lang() },
                    Item = MagicType.None,
                };
                Combo1.SelectedItemChanged += (o, e) =>
                {
                    BigPatchConfig.Combo1 = (MagicType)Combo1.ListBox.SelectedItem.Item;

                    if (BigPatchConfig.Combo1 == MagicType.None) return;

                    if (BigPatchConfig.ComboType == 3)
                    {
                        if (BigPatchConfig.Combo1 == BigPatchConfig.Combo2)
                        {
                            DXMessageBox box = new DXMessageBox("三连招的前两招不能重复".Lang(), "组合错误".Lang(), DXMessageBoxButtons.OK);
                            box.OKButton.MouseClick += (o1, e1) =>
                            {
                                Combo1.ListBox.SelectItem(MagicType.None);
                            };
                        }
                    }
                    else if (BigPatchConfig.ComboType == 4)
                    {
                        if (BigPatchConfig.Combo1 == BigPatchConfig.Combo2 || BigPatchConfig.Combo1 == BigPatchConfig.Combo3)
                        {
                            DXMessageBox box = new DXMessageBox("四连招的前三招不能重复".Lang(), "组合错误".Lang(), DXMessageBoxButtons.OK);
                            box.OKButton.MouseClick += (o1, e1) =>
                            {
                                Combo1.ListBox.SelectItem(MagicType.None);
                            };
                        }
                    }
                    else if (BigPatchConfig.ComboType == 5)
                    {
                        if (BigPatchConfig.Combo1 == BigPatchConfig.Combo2 || BigPatchConfig.Combo1 == BigPatchConfig.Combo3 || BigPatchConfig.Combo1 == BigPatchConfig.Combo4)
                        {
                            DXMessageBox box = new DXMessageBox("五连招的前四招不能重复".Lang(), "组合错误".Lang(), DXMessageBoxButtons.OK);
                            box.OKButton.MouseClick += (o1, e1) =>
                            {
                                Combo1.ListBox.SelectItem(MagicType.None);
                            };
                        }
                    }
                };
                Combo1.Size = new Size(80, 18);
                Combo1.Location = new Point(x, y += 25);

                Combo2 = new DXComboBox
                {
                    Parent = Warrior,
                };
                new DXListBoxItem
                {
                    Parent = Combo2.ListBox,
                    Label = { Text = "空".Lang() },
                    Item = MagicType.None,
                };
                Combo2.SelectedItemChanged += (o, e) =>
                {
                    BigPatchConfig.Combo2 = (MagicType)Combo2.ListBox.SelectedItem.Item;

                    if (BigPatchConfig.Combo2 == MagicType.None) return;

                    if (BigPatchConfig.ComboType == 3)
                    {
                        if (BigPatchConfig.Combo2 == BigPatchConfig.Combo1)
                        {
                            DXMessageBox box = new DXMessageBox("三连招的前两招不能重复".Lang(), "组合错误".Lang(), DXMessageBoxButtons.OK);
                            box.OKButton.MouseClick += (o1, e1) =>
                            {
                                Combo2.ListBox.SelectItem(MagicType.None);
                            };
                        }
                        else if (BigPatchConfig.Combo2 == BigPatchConfig.Combo3)
                        {
                            DXMessageBox box = new DXMessageBox("三连招的第二招不能和第三招重复".Lang(), "组合错误".Lang(), DXMessageBoxButtons.OK);
                            box.OKButton.MouseClick += (o1, e1) =>
                            {
                                Combo2.ListBox.SelectItem(MagicType.None);
                            };
                        }
                    }
                    else if (BigPatchConfig.ComboType == 4)
                    {
                        if (BigPatchConfig.Combo2 == BigPatchConfig.Combo1 || BigPatchConfig.Combo2 == BigPatchConfig.Combo3)
                        {
                            DXMessageBox box = new DXMessageBox("四连招的前三招不能重复".Lang(), "组合错误".Lang(), DXMessageBoxButtons.OK);
                            box.OKButton.MouseClick += (o1, e1) =>
                            {
                                Combo2.ListBox.SelectItem(MagicType.None);
                            };
                        }
                    }
                    else if (BigPatchConfig.ComboType == 5)
                    {
                        if (BigPatchConfig.Combo2 == BigPatchConfig.Combo1 || BigPatchConfig.Combo2 == BigPatchConfig.Combo3 || BigPatchConfig.Combo2 == BigPatchConfig.Combo4)
                        {
                            DXMessageBox box = new DXMessageBox("五连招的前四招不能重复".Lang(), "组合错误".Lang(), DXMessageBoxButtons.OK);
                            box.OKButton.MouseClick += (o1, e1) =>
                            {
                                Combo2.ListBox.SelectItem(MagicType.None);
                            };
                        }
                    }
                };
                Combo2.Size = new Size(80, 18);
                Combo2.Location = new Point(x + 80, y);

                Combo3 = new DXComboBox
                {
                    Parent = Warrior,
                    Visible = false,
                };
                new DXListBoxItem
                {
                    Parent = Combo3.ListBox,
                    Label = { Text = "空".Lang() },
                    Item = MagicType.None,
                };
                Combo3.SelectedItemChanged += (o, e) =>
                {
                    BigPatchConfig.Combo3 = (MagicType)Combo3.ListBox.SelectedItem.Item;

                    if (BigPatchConfig.Combo3 == MagicType.None) return;

                    if (BigPatchConfig.ComboType == 3)
                    {
                        if (BigPatchConfig.Combo3 == BigPatchConfig.Combo2)
                        {
                            DXMessageBox box = new DXMessageBox("三连招的第三招不能和第二招重复".Lang(), "组合错误".Lang(), DXMessageBoxButtons.OK);
                            box.OKButton.MouseClick += (o1, e1) =>
                            {
                                Combo3.ListBox.SelectItem(MagicType.None);
                            };
                        }
                    }
                    else if (BigPatchConfig.ComboType == 4)
                    {
                        if (BigPatchConfig.Combo3 == BigPatchConfig.Combo2 || BigPatchConfig.Combo3 == BigPatchConfig.Combo1)
                        {
                            DXMessageBox box = new DXMessageBox("四连招的前三招不能重复".Lang(), "组合错误".Lang(), DXMessageBoxButtons.OK);
                            box.OKButton.MouseClick += (o1, e1) =>
                            {
                                Combo3.ListBox.SelectItem(MagicType.None);
                            };
                        }
                        else if (BigPatchConfig.Combo3 == BigPatchConfig.Combo4)
                        {
                            DXMessageBox box = new DXMessageBox("四连招的第三招不能和第四招重复".Lang(), "组合错误".Lang(), DXMessageBoxButtons.OK);
                            box.OKButton.MouseClick += (o1, e1) =>
                            {
                                Combo3.ListBox.SelectItem(MagicType.None);
                            };
                        }
                    }
                    else if (BigPatchConfig.ComboType == 5)
                    {
                        if (BigPatchConfig.Combo3 == BigPatchConfig.Combo2 || BigPatchConfig.Combo3 == BigPatchConfig.Combo1 || BigPatchConfig.Combo3 == BigPatchConfig.Combo4)
                        {
                            DXMessageBox box = new DXMessageBox("五连招的前四招不能重复".Lang(), "组合错误".Lang(), DXMessageBoxButtons.OK);
                            box.OKButton.MouseClick += (o1, e1) =>
                            {
                                Combo3.ListBox.SelectItem(MagicType.None);
                            };
                        }
                    }
                };
                Combo3.Size = new Size(80, 18);
                Combo3.Location = new Point(x + 160, y);

                Combo4 = new DXComboBox
                {
                    Parent = Warrior,
                    Visible = false,
                };
                new DXListBoxItem
                {
                    Parent = Combo4.ListBox,
                    Label = { Text = "空".Lang() },
                    Item = MagicType.None,
                };
                Combo4.SelectedItemChanged += (o, e) =>
                {
                    BigPatchConfig.Combo4 = (MagicType)Combo4.ListBox.SelectedItem.Item;

                    if (BigPatchConfig.Combo4 == MagicType.None) return;

                    if (BigPatchConfig.ComboType == 4)
                    {
                        if (BigPatchConfig.Combo4 == BigPatchConfig.Combo3)
                        {
                            DXMessageBox box = new DXMessageBox("四连招的第四招不能和第三招重复".Lang(), "组合错误".Lang(), DXMessageBoxButtons.OK);
                            box.OKButton.MouseClick += (o1, e1) =>
                            {
                                Combo4.ListBox.SelectItem(MagicType.None);
                            };
                        }
                    }
                    else if (BigPatchConfig.ComboType == 5)
                    {
                        if (BigPatchConfig.Combo4 == BigPatchConfig.Combo2 || BigPatchConfig.Combo4 == BigPatchConfig.Combo3 || BigPatchConfig.Combo4 == BigPatchConfig.Combo1)
                        {
                            DXMessageBox box = new DXMessageBox("五连招的前四招不能重复".Lang(), "组合错误".Lang(), DXMessageBoxButtons.OK);
                            box.OKButton.MouseClick += (o1, e1) =>
                            {
                                Combo4.ListBox.SelectItem(MagicType.None);
                            };
                        }
                        else if (BigPatchConfig.Combo4 == BigPatchConfig.Combo5)
                        {
                            DXMessageBox box = new DXMessageBox("五连招的第四招不能和第五招重复".Lang(), "组合错误".Lang(), DXMessageBoxButtons.OK);
                            box.OKButton.MouseClick += (o1, e1) =>
                            {
                                Combo4.ListBox.SelectItem(MagicType.None);
                            };
                        }
                    }
                };
                Combo4.Size = new Size(80, 18);
                Combo4.Location = new Point(x, y += 25);

                Combo5 = new DXComboBox
                {
                    Parent = Warrior,
                    Visible = false,
                };
                new DXListBoxItem
                {
                    Parent = Combo5.ListBox,
                    Label = { Text = "空".Lang() },
                    Item = MagicType.None,
                };
                Combo5.SelectedItemChanged += (o, e) =>
                {
                    BigPatchConfig.Combo5 = (MagicType)Combo5.ListBox.SelectedItem.Item;

                    if (BigPatchConfig.Combo5 == MagicType.None) return;

                    if (BigPatchConfig.ComboType == 5)
                    {
                        if (BigPatchConfig.Combo5 == BigPatchConfig.Combo4)
                        {
                            DXMessageBox box = new DXMessageBox("五连招的第五招不能和第四招重复".Lang(), "组合错误".Lang(), DXMessageBoxButtons.OK);
                            box.OKButton.MouseClick += (o1, e1) =>
                            {
                                Combo5.ListBox.SelectItem(MagicType.None);
                            };
                        }
                    }
                };
                Combo5.Size = new Size(80, 18);
                Combo5.Location = new Point(x + 80, y);


                ////////////////////////////////////////////////
                // 法师
                y = 5;
                AutoMagicShield = CreateCheckBox(Wizard, "自动魔法盾".Lang(), x, y += 25, (o, e) =>
                {
                    BigPatchConfig.AutoMagicShield = AutoMagicShield.Checked;
                }, BigPatchConfig.AutoMagicShield);
                AutoRenounce = CreateCheckBox(Wizard, "自动凝血".Lang(), x, y += 25, (o, e) =>
                {
                    BigPatchConfig.AutoRenounce = AutoRenounce.Checked;
                }, BigPatchConfig.AutoRenounce);
                /*AutoThunder = CreateCheckBox(Wizard, "自动天打雷劈", x, y += 25, (o, e) =>
                {
                    BigPatchConfig.AutoThunder = AutoThunder.Checked;
                }, BigPatchConfig.AutoThunder);*/

                /////////////////////////////////////////////////
                // 道士
                y = 5;
                AutoPoisonDust = CreateCheckBox(Taoist, "自动换毒".Lang(), x, y += 25, (o, e) =>
                {
                    BigPatchConfig.AutoPoisonDust = AutoPoisonDust.Checked;
                }, BigPatchConfig.AutoPoisonDust);
                AutoAmulet = CreateCheckBox(Taoist, "自动换符".Lang(), x, y += 25, (o, e) =>
                {
                    BigPatchConfig.AutoAmulet = AutoAmulet.Checked;
                }, BigPatchConfig.AutoAmulet);
                //AutoCelestial = CreateCheckBox(Taoist, "自动阴阳盾".Lang(), x, y += 25, (o, e) =>
                //{
                //    BigPatchConfig.AutoCelestial = AutoCelestial.Checked;
                //}, BigPatchConfig.AutoCelestial);
                /////////////////////////////////////////////////
                // 刺客
                y = 5;
                AutoFourFlowers = CreateCheckBox(Assassin, "自动四花".Lang(), x, y += 25, (o, e) =>
                {
                    BigPatchConfig.AutoFourFlowers = AutoFourFlowers.Checked;
                }, BigPatchConfig.AutoFourFlowers);
                AutoEvasion = CreateCheckBox(Assassin, "自动风之闪避".Lang(), x, y += 25, (o, e) =>
                {
                    BigPatchConfig.AutoEvasion = AutoEvasion.Checked;
                }, BigPatchConfig.AutoEvasion);
                AutoRagingWind = CreateCheckBox(Assassin, "自动风之守护".Lang(), x, y += 25, (o, e) =>
                {
                    BigPatchConfig.AutoRagingWind = AutoRagingWind.Checked;
                }, BigPatchConfig.AutoRagingWind);

                ////////////////////////////////////////////////
                ///自动技能
                y = 35;
                DXLabel Lab = new DXLabel
                {
                    Parent = AutoSkill,
                    Text = "技能1".Lang(),
                    Hint = "根据定义的时间间隔(秒单位)自动释放技能".Lang(),
                };
                Lab.Location = new Point(x, y);
                CombSkill1 = new DXComboBox
                {
                    Parent = AutoSkill,
                    Size = new Size(90, 18),
                };

                CombSkill1.Location = new Point(Lab.Location.X + Lab.Size.Width + 5, y);
                CombSkill1.SelectedItemChanged += (o, e) =>
                {
                    MagicType m = (MagicType)CombSkill1.SelectedItem;
                    foreach (MagicInfo info in Globals.MagicInfoList.Binding)
                    {
                        if (info.Magic == m)
                        {
                            BigPatchConfig.AutoSkillMagic_1 = m;
                            if (NumbSkill1.Value < info.Delay / 1000)
                            {
                                NumbSkill1.Value = info.Delay / 1000;
                            }
                            break;
                        }
                    }
                };
                NumbSkill1 = new DXNumberBox
                {
                    Parent = AutoSkill,
                    Size = new Size(80, 20),
                    ValueTextBox = { Size = new Size(40, 18) },
                    MaxValue = 50000,
                    MinValue = 1,
                    Value = BigPatchConfig.NumbSkill1,
                    UpButton = { Location = new Point(63, 1) }
                };
                NumbSkill1.ValueTextBox.ValueChanged += (o, e) =>
                {
                    BigPatchConfig.NumbSkill1 = NumbSkill1.Value;
                };
                NumbSkill1.Location = new Point(CombSkill1.Location.X + CombSkill1.Size.Width + 5, CombSkill1.Location.Y);
                AutoSkill_1 = new DXCheckBox
                {
                    Parent = AutoSkill,
                    Checked = BigPatchConfig.AutoMagicSkill_1,
                    Hint = "自动技能①".Lang(),
                    Location = new Point(NumbSkill1.Location.X + NumbSkill1.Size.Width + 5, NumbSkill1.Location.Y + 2),
                };
                AutoSkill_1.CheckedChanged += (o, e) =>
                {
                    BigPatchConfig.AutoMagicSkill_1 = AutoSkill_1.Checked;
                };

                y += 25;
                Lab = new DXLabel
                {
                    Parent = AutoSkill,
                    Text = "技能2".Lang(),
                    Hint = "根据定义的时间间隔(秒单位)自动释放技能".Lang(),
                };
                Lab.Location = new Point(x, y);

                CombSkill2 = new DXComboBox
                {
                    Parent = AutoSkill,
                    Size = new Size(90, 18),
                };
                CombSkill2.Location = new Point(Lab.Location.X + Lab.Size.Width + 5, Lab.Location.Y);
                CombSkill2.SelectedItemChanged += (o, e) =>
                {
                    MagicType m = (MagicType)CombSkill2.SelectedItem;
                    foreach (MagicInfo info in Globals.MagicInfoList.Binding)
                    {
                        if (info.Magic == m)
                        {
                            BigPatchConfig.AutoSkillMagic_2 = m;
                            if (NumbSkill2.Value < info.Delay / 1000)
                            {
                                NumbSkill2.Value = info.Delay / 1000;
                            }
                            break;
                        }
                    }
                };
                NumbSkill2 = new DXNumberBox
                {
                    Parent = AutoSkill,
                    Size = new Size(80, 20),
                    ValueTextBox = { Size = new Size(40, 18) },
                    MaxValue = 50000,
                    MinValue = 1,
                    Value = BigPatchConfig.NumbSkill2,
                    UpButton = { Location = new Point(63, 1) }
                };
                NumbSkill2.ValueTextBox.ValueChanged += (o, e) =>
                {
                    BigPatchConfig.NumbSkill2 = NumbSkill2.Value;
                };
                NumbSkill2.Location = new Point(CombSkill2.Location.X + CombSkill2.Size.Width + 5, CombSkill2.Location.Y);
                AutoSkill_2 = new DXCheckBox
                {
                    Parent = AutoSkill,
                    Checked = BigPatchConfig.AutoMagicSkill_2,
                    Hint = "自动技能②".Lang(),
                    Location = new Point(NumbSkill2.Location.X + NumbSkill2.Size.Width + 5, NumbSkill2.Location.Y + 2),
                };
                AutoSkill_2.CheckedChanged += (o, e) =>
                {
                    BigPatchConfig.AutoMagicSkill_2 = AutoSkill_2.Checked;
                };
                y += 25;

                AutoSkill.Size = new Size(125, y + 5);

                ////////////////////////////////////////////////
                //x = GroupMouseMiddle.Location.X;
                y += 5;
                //RefreshBag = new DXButton
                //{
                //    Parent = this,
                //    Size = new Size(70, 18),
                //    Location = new Point(x, y),
                //    ButtonType = ButtonType.SmallButton,
                //    Label = {
                //    Text = "刷新包裹".Lang(),}
                //};
                //RefreshBag.MouseClick += (o, e) =>
                //{
                //    if (GameScene.Game.Observer) return;
                //    CEnvir.Enqueue(new C.InventoryRefresh { });
                //};
                //y += Yspac;

                ReloadConfig = new DXButton
                {
                    Parent = this,
                    Size = new Size(50, 18),
                    Location = new Point(x, y),
                    ButtonType = ButtonType.SmallButton,
                    Label = {
                    Text = "重置".Lang(), },
                };
                ReloadConfig.MouseClick += (o, e) =>
                {
                    BigPatchConfig.ChkAvertVerb = true;
                    BigPatchConfig.ChkShowHPBar = true;
                    BigPatchConfig.ChkAvertBright = false;
                    BigPatchConfig.ShowPlayerNames = true;
                    BigPatchConfig.ChkDisplayOthers = true;
                    BigPatchConfig.ChkQuickSelect = false;
                    BigPatchConfig.ChkShowObjects = false;
                    BigPatchConfig.ChkBossWarrning = true;
                    BigPatchConfig.ChkRunningHit = false;
                    BigPatchConfig.ChkKeepRunning = false;
                    BigPatchConfig.ChkDurableWarning = true;
                    BigPatchConfig.ChkMonsterInfo = true;
                    BigPatchConfig.ChkAutoUnAcceptGroup = false;
                    BigPatchConfig.ChkCleanCorpse = false;
                    BigPatchConfig.ChkMonsterNameTips = false;
                    BigPatchConfig.ChkMonsterLevelTips = false;
                    BigPatchConfig.ShowHealth = true;
                    BigPatchConfig.ShowMonHealth = true;
                    BigPatchConfig.ChkAvertShift = false;
                    BigPatchConfig.ChkLockTarget = true;
                    BigPatchConfig.ShowDamageNumbers = true;
                    BigPatchConfig.ChkShowTargetInfo = false;
                    BigPatchConfig.RightClickDeTarget = true;
                    BigPatchConfig.ChkQuitWar = false;
                    BigPatchConfig.ChkAvertPetrifaction = false;
                    BigPatchConfig.ChkAvertShake = true;
                    BigPatchConfig.ChkColourTarget = false;
                    BigPatchConfig.ChkColourFriend = false;
                    BigPatchConfig.ChkDismountToFireMagic = false;
                    BigPatchConfig.ChkDefendAssassination = false;
                    //BigPatchConfig.ChkCloseExpTips = true;
                    BigPatchConfig.ChkCloseCombatTips = true;
                    BigPatchConfig.ChkItemObjShining = false;
                    BigPatchConfig.ChkItemObjBeam = true;
                    BigPatchConfig.ChkItemObjShow = true;
                    BigPatchConfig.ChkAutoPick = true;
                    BigPatchConfig.ChkTabPick = false;
                    BigPatchConfig.Weather = 0;
                    BigPatchConfig.ChkAutoFire = false;
                    BigPatchConfig.AutoFire = 0;
                    BigPatchConfig.AutoFireInterval = 10;
                    BigPatchConfig.ChkCallMounts = false;
                    BigPatchConfig.ChkCastingMagic = false;
                    BigPatchConfig.MiddleMouse = 0;
                };

                ///////////////////////////////////
                ChkAutoSayWords = new DXCheckBox
                {
                    Parent = ChatFrame,
                    Label = { Text = "自动喊话".Lang() },
                    bAlignRight = false,
                    Location = new Point(10, 30),
                    Checked = false,
                };
                IntervalLeft = new DXLabel
                {
                    Parent = ChatFrame,
                    Text = "间隔:".Lang(),
                    Location = new Point(90, 30),
                };

                AutoSayInterval = new DXNumberBox
                {
                    Change = 60000,
                    Parent = ChatFrame,
                    ValueTextBox = { Size = new Size(50, 14) },
                    MaxValue = 6000000,
                    Value = BigPatchConfig.AutoSayInterval,
                    MinValue = 60000,
                    Location = new Point(125, 30),
                };

                IntervalRight = new DXLabel
                {
                    Parent = ChatFrame,
                    Text = "毫秒".Lang(),
                    Location = new Point(220, 30),
                };

                SayWordTimer = new Timer
                {
                    Interval = 60000,
                    Enabled = false,
                    AutoReset = true,
                };
                SayWordTimer.Elapsed += OnSayWordsTimer;

                AutoSayInterval.ValueTextBox.ValueChanged += (o, e) =>
                {
                    BigPatchConfig.AutoSayInterval = AutoSayInterval.Value;
                    SayWordTimer.Interval = BigPatchConfig.AutoSayInterval;
                };

                ChkAutoSayWords.CheckedChanged += (o, e) =>
                {
                    SayWordTimer.Enabled = ChkAutoSayWords.Checked;
                    SayWordTimer.Interval = AutoSayInterval.Value;
                    if (SayWordTimer.Enabled)
                    {
                        SayWordTimer.Start();
                    }
                    else
                    {
                        SayWordTimer.Stop();
                    }
                };

                ////////////////////////////
                InputBox = new DXTextView
                {
                    Parent = ChatFrame,
                    ReadOnly = false,
                    Visible = false,
                };
                for (int i = 0; i < BigPatchConfig.AutoSayLines.Count; i++)
                {
                    InputBox.TextBox.Text += BigPatchConfig.AutoSayLines[i] + "\r\n";
                }

                InputBox.TextBox.ScrollBars = ScrollBars.None;
                InputBox.TextBox.MaxLength = 115;//最长这么多 多了发不出去

            }
            /// <summary>
            /// 尺寸变化 坐标位置
            /// </summary>
            /// <param name="oValue"></param>
            /// <param name="nValue"></param>
            public override void OnSizeChanged(Size oValue, Size nValue)
            {
                base.OnSizeChanged(oValue, nValue);

                if (null == InputBox) return;

                if (GroupNormal != null)
                {
                    GroupNormal.Location = new Point(10, 0);
                    GroupNormal.Size = new Size(110, 80);
                }
                //if (GroupWar != null)
                //{
                //    GroupWar.Size = new Size((Size.Width - 40) / 3, GroupWar.Size.Height);
                //    GroupWar.Location = new Point(GroupNormal.Location.X + GroupNormal.Size.Width + 10, 0);
                //}
                //if (GroupItem != null)
                //{
                //    GroupItem.Location = new Point(GroupWar.Location.X + GroupWar.Size.Width + 10, 0);
                //    GroupItem.Size = new Size((Size.Width - 40) / 3, GroupItem.Size.Height);
                //}
                if (GroupWeather != null)
                {
                    GroupWeather.Location = new Point(125, 0);
                    GroupWeather.Size = new Size(90, 65);
                }
                //if (GroupAutoAttack != null)
                //{
                //    GroupAutoAttack.Location = new Point(GroupItem.Location.X, GroupItem.Location.Y + GroupItem.Size.Height + 10);
                //    GroupAutoAttack.Size = new Size(GroupItem.Size.Width, GroupAutoAttack.Size.Height);
                //}
                //if (GroupMouseMiddle != null)
                //{
                //    GroupMouseMiddle.Location = new Point(GroupItem.Location.X, GroupAutoAttack.Location.Y + GroupAutoAttack.Size.Height + 10);
                //    GroupMouseMiddle.Size = new Size(GroupItem.Size.Width, GroupMouseMiddle.Size.Height);
                //}
                //if (null != RefreshBag)
                //{
                //    int x = GroupMouseMiddle.Location.X;
                //    int y = GroupMouseMiddle.Location.Y + GroupMouseMiddle.Size.Height + 10;
                //    RefreshBag.Location = new Point(x + 5, y);
                //}
                if (null != ReloadConfig)
                {
                    //int x = AutoSkill.Location.X;
                    //int y = AutoSkill.Location.Y + AutoSkill.Size.Height + 20;
                    ReloadConfig.Location = new Point(220, 30);
                }

                //战士
                if (Warrior != null)
                {
                    Warrior.Size = new Size(380, 157);
                    Warrior.Location = new Point(10, 68);
                }

                //法师
                if (Wizard != null)
                {
                    Wizard.Size = new Size(380, 157);
                    Wizard.Location = new Point(10, 68);
                }

                //道士
                if (Taoist != null)
                {
                    Taoist.Size = new Size(380, 157);
                    Taoist.Location = new Point(10, 68);
                }

                //刺客
                if (Assassin != null)
                {
                    Assassin.Size = new Size(380, 157);
                    Assassin.Location = new Point(10, 68);
                }

                //自动技能
                if (AutoSkill != null)
                {
                    AutoSkill.Size = new Size(380, 100);
                    AutoSkill.Location = new Point(10, 215);
                }

                if (ChatFrame != null)
                {
                    ChatFrame.Size = new Size(380, 155);
                    ChatFrame.Location = new Point(10, 305);
                }

                InputBox.Size = new Size(360, 90);
                InputBox.Location = new Point(10, 52);
                InputBox.Visible = true;
                InputBox.Opacity = 0.7F;
            }

            public void UpdateMagic()
            {
                ////////////////////////////////////////////////////////////
                //初始化技能窗口
                if (SkillListBoxItems.Count > 0)
                {
                    foreach (DXListBoxItem item in SkillListBoxItems)
                        item.Dispose();
                    SkillListBoxItems.Clear();
                }

                foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in GameScene.Game.User.Magics)
                {
                    ClientUserMagic m = pair.Value;
                    SkillListBoxItems.Add(new DXListBoxItem
                    {
                        Parent = CombSkill1.ListBox,
                        Label = { Text = m.Info.Name },
                        Item = m.Info.Magic,
                    });
                    SkillListBoxItems.Add(new DXListBoxItem
                    {
                        Parent = CombSkill2.ListBox,
                        Label = { Text = m.Info.Name },
                        Item = m.Info.Magic,
                    });

                    string name = null;
                    switch (m.Info.Magic)
                    {
                        case MagicType.FlamingSword:
                            name = "烈火".Lang();
                            break;
                        case MagicType.DragonRise:
                            name = "翔空".Lang();
                            break;
                        case MagicType.BladeStorm:
                            name = "莲月".Lang();
                            break;
                    }

                    if (!string.IsNullOrEmpty(name))
                    {
                        new DXListBoxItem
                        {
                            Parent = Combo1.ListBox,
                            Label = { Text = name },
                            Item = m.Info.Magic,
                        };
                        new DXListBoxItem
                        {
                            Parent = Combo2.ListBox,
                            Label = { Text = name },
                            Item = m.Info.Magic,
                        };
                        new DXListBoxItem
                        {
                            Parent = Combo3.ListBox,
                            Label = { Text = name },
                            Item = m.Info.Magic,
                        };
                        new DXListBoxItem
                        {
                            Parent = Combo4.ListBox,
                            Label = { Text = name },
                            Item = m.Info.Magic,
                        };
                        new DXListBoxItem
                        {
                            Parent = Combo5.ListBox,
                            Label = { Text = name },
                            Item = m.Info.Magic,
                        };
                    }
                }

                CombSkill1.ListBox.SelectItem(BigPatchConfig.AutoSkillMagic_1);
                BigPatchConfig.AutoSkillMagic_1 = CombSkill1.SelectedItem == null ? MagicType.None : (MagicType)CombSkill1.ListBox.SelectedItem.Item;
                CombSkill2.ListBox.SelectItem(BigPatchConfig.AutoSkillMagic_2);
                BigPatchConfig.AutoSkillMagic_2 = CombSkill2.SelectedItem == null ? MagicType.None : (MagicType)CombSkill2.ListBox.SelectedItem.Item;

                Combo1.ListBox.SelectItem(BigPatchConfig.Combo1);
                BigPatchConfig.Combo1 = Combo1.SelectedItem == null ? MagicType.None : (MagicType)Combo1.ListBox.SelectedItem.Item;
                Combo2.ListBox.SelectItem(BigPatchConfig.Combo2);
                BigPatchConfig.Combo2 = Combo2.SelectedItem == null ? MagicType.None : (MagicType)Combo2.ListBox.SelectedItem.Item;
                Combo3.ListBox.SelectItem(BigPatchConfig.Combo3);
                BigPatchConfig.Combo3 = Combo3.SelectedItem == null ? MagicType.None : (MagicType)Combo3.ListBox.SelectedItem.Item;
                Combo4.ListBox.SelectItem(BigPatchConfig.Combo4);
                BigPatchConfig.Combo4 = Combo4.SelectedItem == null ? MagicType.None : (MagicType)Combo4.ListBox.SelectedItem.Item;
                Combo5.ListBox.SelectItem(BigPatchConfig.Combo5);
                BigPatchConfig.Combo5 = Combo5.SelectedItem == null ? MagicType.None : (MagicType)Combo5.ListBox.SelectedItem.Item;
                ComboType.ListBox.SelectItem(BigPatchConfig.ComboType);
            }

            public static void OnSayWordsTimer(object o, ElapsedEventArgs e)
            {
                string str = GameScene.Game?.BigPatchBox?.Commonly?.InputBox?.TextBox?.Text;
                str = str.Replace("\r", "").Replace("\n", "");
                if (null == str || str.Length == 0) return;

                CEnvir.Enqueue(new C.Chat
                {
                    Text = str,
                });
            }
        };
        #endregion

        #endregion

        #region 辅助

        /// <summary>
        /// 玩家辅助选项
        /// </summary>
        public class DXPlayerHelperTab : DXTab
        {
            public DXCheckBox ChkBufTimer;
            public DXCheckBox ChkAutoAddEnemy;
            public DXCheckBox ChkPkMode;
            public DXCheckBox ChkPkDrink;
            //public DXLabel LabCommand;
            //public DXComboBox CombCmdBox;
            //public DXButton BtSubmit;

            public DXGroupBox Android;
            public DXLabel TimeLable;
            public DXCheckBox AndroidPlayer;
            //public DXCheckBox AndroidPickUp;
            public DXCheckBox AndroidPoisonDust;
            public DXCheckBox AndroidEluded;
            public DXCheckBox AndroidBackCastle;
            public DXCheckBox AndroidBossRandom;
            public DXCheckBox AndroidPlayerRandom;
            public DXCheckBox AndroidSingleSkill;
            public DXComboBox AndroidSkills;
            public DXNumberBox AndroidCoordX;
            public DXNumberBox AndroidCoordY;
            public DXNumberBox AndroidCoordRange;
            public DXCheckBox AndroidLockRange;
            public DXNumberBox AndroidBackCastleMinPHValue;
            public DXCheckBox AndroidMinPHBackCastle;
            public DXNumberBox AndroidRandomMinPHValue;
            public DXCheckBox AndroidMinPHRandom;
            public DXNumberBox TimeBoxRandom;       //自动随机时间
            public DXCheckBox ChkAutoRandom;        //自动随机
            public DXNumberBox TargetTimeRandom;    //更换目标时间
            public DXCheckBox ChkChangeTarget;      //更换目标
            public DXCheckBox ChkConsumeRepair;     //自动消耗品修理
            public DXNumberBox ConsumeRepairTime;   //自动消耗品修理时间
            public List<DXListBoxItem> SkillListBoxItems = new List<DXListBoxItem>();

            /// <summary>
            /// 玩家辅助选项
            /// </summary>
            public DXPlayerHelperTab()
            {
                Android = new DXGroupBox
                {
                    Parent = this,
                    Size = new Size(380, 420),
                    Location = new Point(10, 0),
                    Name = { Text = "挂机".Lang() },
                    //Visible = false,
                };
                if (CEnvir.ClientControl?.OnAutoHookTab != true)
                {
                    Android.Visible = false;
                }

                ////////////////////////////////////////////////
                //LabCommand = new DXLabel
                //{
                //    Parent = this,
                //    Text = "特殊命令:"
                //};
                //CombCmdBox = new DXComboBox
                //{
                //    Parent = this,
                //    Size = new Size(180, 18),
                //};
                //new DXListBoxItem
                //{
                //    Parent = CombCmdBox.ListBox,
                //    Label = { Text = "空" },
                //    Item = null,
                //};
                //string[] Cmds = new string[]
                //{
                //    "@复活回城",
                //    "@卡位自救",
                //    "@加入行会",
                //    "@退出行会",
                //    "@允许回生术",
                //    "@允许天地合一",
                //    "@天地合一",
                //    "@添加成员",
                //    "@允许结盟",
                //    "@结盟",
                //    "@拒绝私聊",
                //    "@拒绝所有人聊天",
                //    "@拒绝行会聊天",
                //    "@拒绝交易",
                //};
                //int i = 0;
                //foreach (string cmd in Cmds)
                //{
                //    new DXListBoxItem
                //    {
                //        Parent = CombCmdBox.ListBox,
                //        Label = { Text = cmd },
                //        Item = i++,
                //    };
                //}
                //CombCmdBox.ListBox.SelectItem(null);
                //BtSubmit = new DXButton
                //{
                //    Parent = this,
                //    Size = new Size(60, 18),
                //    ButtonType = ButtonType.SmallButton,
                //    Label = { Text = "执行" },
                //};
                //BtSubmit.MouseClick += (o, e) =>
                //{

                //};

                int x = 15, y = 30;

                //自动挂机
                DXLabel Lab = new DXLabel
                {
                    Parent = Android,
                    Text = "剩余时间".Lang(),
                    Outline = true,
                    Location = new Point(x, y),
                };
                TimeLable = new DXLabel
                {
                    Text = "",
                    Outline = true,
                    Parent = Android,
                    Border = true,
                    BorderColour = Color.Green,
                    Location = new Point(Lab.Location.X + Lab.Size.Width + 4, y),
                    ForeColour = Color.Green,
                };
                AndroidPlayer = CreateCheckBox(Android, "自动挂机".Lang(), x + 150, y, (o, e) =>
                   {
                       BigPatchConfig.AndroidPlayer = AndroidPlayer.Checked;
                   }, BigPatchConfig.AndroidPlayer);
                AndroidPlayer.CheckedChanged += (o, e) =>
                {
                    if (GameScene.Game.Observer) return;  //如果是观察者 返回

                    if (GameScene.Game.MapControl.MapInfo.BanAndroidPlayer == true)
                    {
                        GameScene.Game.ReceiveChat("当前地图禁止挂机".Lang(), MessageType.System);   //提示并跳过
                        AndroidPlayer.Checked = false;   //关闭挂机
                        return;   //如果地图信息禁止挂机 返回
                    }

                    if (AndroidPlayer.Checked == true)   //如果 挂机等开启
                    {
                        if (GameScene.Game.User.AutoTime == 0)   //判断挂机时间=0
                        {
                            AndroidPlayer.Checked = false;   //关闭挂机  返回
                            return;
                        }
                    }
                    CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = AndroidPlayer.Checked, Slot = AutoSetConf.SetAutoOnHookBox });  //发送自动挂机改变
                };

                /*AndroidPickUp = CreateCheckBox(Android, "自动拾取", x, y += 25, (o, e) =>
                  {
                      BigPatchConfig.AndroidPickUp = AndroidPickUp.Checked;
                  }, BigPatchConfig.AndroidPickUp);*/
                AndroidPoisonDust = CreateCheckBox(Android, "自动上毒".Lang(), x, y += 25, (o, e) =>
                {
                    BigPatchConfig.AndroidPoisonDust = AndroidPoisonDust.Checked;
                }, BigPatchConfig.AndroidPoisonDust);
                AndroidEluded = CreateCheckBox(Android, "自动躲避".Lang(), x + 150, y, (o, e) =>
                {
                    BigPatchConfig.AndroidEluded = AndroidEluded.Checked;
                }, BigPatchConfig.AndroidEluded);
                AndroidBackCastle = CreateCheckBox(Android, "自动回城".Lang(), x, y += 25, (o, e) =>
                {
                    BigPatchConfig.AndroidBackCastle = AndroidBackCastle.Checked;
                }, BigPatchConfig.AndroidBackCastle);

                AndroidBossRandom = CreateCheckBox(Android, "Boss随机".Lang(), x, y += 25, (o, e) =>
                {
                    BigPatchConfig.AndroidBossRandom = AndroidBossRandom.Checked;
                }, BigPatchConfig.AndroidBossRandom);
                AndroidPlayerRandom = CreateCheckBox(Android, "遇人随机".Lang(), x + 150, y, (o, e) =>
                {
                    BigPatchConfig.AndroidPlayerRandom = AndroidPlayerRandom.Checked;
                }, BigPatchConfig.AndroidPlayerRandom);

                AndroidSingleSkill = CreateCheckBox(Android, "远程技能".Lang(), x, y += 25, (o, e) =>
                {
                    BigPatchConfig.AndroidSingleSkill = AndroidSingleSkill.Checked;
                }, BigPatchConfig.AndroidSingleSkill);
                AndroidSkills = new DXComboBox
                {
                    Parent = Android,
                    Size = new Size(120, 18),
                    Location = new Point(AndroidSingleSkill.Location.X + AndroidSingleSkill.Size.Width + 5, y),
                };
                AndroidSkills.SelectedItemChanged += (o, e) =>
                {
                    BigPatchConfig.AndroidSkills = (MagicType)AndroidSkills.ListBox.SelectedItem.Item;
                };
                Lab = new DXLabel
                {
                    Parent = Android,
                    Text = "X坐标:".Lang(),
                    Outline = true,
                    Location = new Point(x, y += 40),
                };
                AndroidCoordX = new DXNumberBox
                {
                    Parent = Android,
                    Size = new Size(80, 20),
                    ValueTextBox = { Size = new Size(40, 18) },
                    MaxValue = 1024,
                    MinValue = 1,
                    Value = BigPatchConfig.AndroidCoord.X,
                    UpButton = { Location = new Point(63, 1) },
                    Location = new Point(x + Lab.Size.Width, y),
                };
                AndroidCoordX.ValueTextBox.ValueChanged += (o, e) =>
                {
                    BigPatchConfig.AndroidCoord = new Point((int)AndroidCoordX.Value, BigPatchConfig.AndroidCoord.Y);
                };
                Lab = new DXLabel
                {
                    Parent = Android,
                    Text = "Y坐标:".Lang(),
                    Outline = true,
                    Location = new Point(x + 125, y),
                };
                AndroidCoordY = new DXNumberBox
                {
                    Parent = Android,
                    Size = new Size(80, 20),
                    ValueTextBox = { Size = new Size(40, 18) },
                    MaxValue = 1024,
                    MinValue = 1,
                    Value = BigPatchConfig.AndroidCoord.Y,
                    UpButton = { Location = new Point(63, 1) },
                    Location = new Point(x + 120 + Lab.Size.Width, y),
                };
                AndroidCoordY.ValueTextBox.ValueChanged += (o, e) =>
                {
                    BigPatchConfig.AndroidCoord = new Point(BigPatchConfig.AndroidCoord.X, (int)AndroidCoordY.Value);
                };
                Lab = new DXLabel
                {
                    Parent = Android,
                    Text = "范围:".Lang(),
                    Outline = true,
                    Location = new Point(x, y += 25),
                };
                AndroidCoordRange = new DXNumberBox
                {
                    Parent = Android,
                    Size = new Size(80, 20),
                    ValueTextBox = { Size = new Size(40, 18) },
                    MaxValue = 1024,
                    MinValue = 1,
                    Value = BigPatchConfig.AndroidCoordRange,
                    UpButton = { Location = new Point(63, 1) },
                    Location = new Point(AndroidCoordX.Location.X, y),
                };
                AndroidCoordRange.ValueTextBox.ValueChanged += (o, e) =>
                {
                    BigPatchConfig.AndroidCoordRange = AndroidCoordRange.Value;
                };

                AndroidLockRange = CreateCheckBox(Android, "定点挂机".Lang(), x + 150, y, (o, e) =>
                {
                    BigPatchConfig.AndroidLockRange = AndroidLockRange.Checked;
                }, BigPatchConfig.AndroidLockRange);

                /*Lab = new DXLabel
                {
                    Parent = Android,
                    Text = "血量低于:",
                    Outline = true,
                    Hint = "百分比值",
                    Location = new Point(x, y += 40),
                };
                AndroidBackCastleMinPHValue = new DXNumberBox
                {
                    Parent = Android,
                    Value = BigPatchConfig.AndroidBackCastleMinPHValue,
                    Size = new Size(80, 20),
                    ValueTextBox = { Size = new Size(40, 18) },
                    MaxValue = 100,
                    MinValue = 1,
                    UpButton = { Location = new Point(63, 1) },
                    Location = new Point(x + Lab.Size.Width, y),
                };
                AndroidBackCastleMinPHValue.ValueTextBox.ValueChanged += (o, e) =>
                {
                    BigPatchConfig.AndroidBackCastleMinPHValue = AndroidBackCastleMinPHValue.Value;
                };
                AndroidMinPHBackCastle = CreateCheckBox(Android, "回城保护", x + 150, y, (o, e) =>
                {
                    BigPatchConfig.AndroidMinPHBackCastle = AndroidMinPHBackCastle.Checked;
                }, BigPatchConfig.AndroidMinPHBackCastle);

                Lab = new DXLabel
                {
                    Parent = Android,
                    Text = "血量低于:",
                    Outline = true,
                    Hint = "百分比值",
                    Location = new Point(x, y += 25),
                };
                AndroidRandomMinPHValue = new DXNumberBox
                {
                    Parent = Android,
                    Value = BigPatchConfig.AndroidRandomMinPHValue,
                    Size = new Size(80, 20),
                    ValueTextBox = { Size = new Size(40, 18) },
                    MaxValue = 100,
                    MinValue = 1,
                    UpButton = { Location = new Point(63, 1) },
                    Location = new Point(x + Lab.Size.Width, y),
                };
                AndroidRandomMinPHValue.ValueTextBox.ValueChanged += (o, e) =>
                {
                    BigPatchConfig.AndroidRandomMinPHValue = AndroidRandomMinPHValue.Value;
                };
                AndroidMinPHRandom = CreateCheckBox(Android, "随机保护", x + 150, y, (o, e) =>
                {
                    BigPatchConfig.AndroidMinPHRandom = AndroidMinPHRandom.Checked;
                }, BigPatchConfig.AndroidMinPHRandom);*/

                Lab = new DXLabel
                {
                    Parent = Android,
                    Text = "间隔(秒):".Lang(),
                    Outline = true,
                    Location = new Point(x, y += 40),
                };
                TimeBoxRandom = new DXNumberBox
                {
                    Parent = Android,
                    Size = new Size(80, 20),
                    ValueTextBox = { Size = new Size(40, 18) },
                    MaxValue = 100000,
                    MinValue = 1,
                    Value = BigPatchConfig.TimeAutoRandom,
                    UpButton = { Location = new Point(63, 1) },
                    Location = new Point(x + Lab.Size.Width, y),
                };
                TimeBoxRandom.ValueTextBox.ValueChanged += (o, e) =>
                {
                    BigPatchConfig.TimeAutoRandom = TimeBoxRandom.Value;
                };
                ChkAutoRandom = CreateCheckBox(Android, "自动随机".Lang(), x + 150, y, (o, e) =>
                {
                    BigPatchConfig.ChkAutoRandom = ChkAutoRandom.Checked;
                }, BigPatchConfig.ChkAutoRandom);

                Lab = new DXLabel
                {
                    Parent = Android,
                    Text = "间隔(秒):".Lang(),
                    Outline = true,
                    Hint = "被怪围困".Lang(),
                    Location = new Point(x, y += 25),
                };
                TargetTimeRandom = new DXNumberBox
                {
                    Parent = Android,
                    Size = new Size(80, 20),
                    ValueTextBox = { Size = new Size(40, 18) },
                    MaxValue = 100000,
                    MinValue = 1,
                    Value = BigPatchConfig.TargetTimeRandom,
                    UpButton = { Location = new Point(63, 1) },
                    Location = new Point(x + Lab.Size.Width, y),
                };
                TargetTimeRandom.ValueTextBox.ValueChanged += (o, e) =>
                {
                    BigPatchConfig.TargetTimeRandom = TargetTimeRandom.Value;
                };
                ChkChangeTarget = CreateCheckBox(Android, "更换目标".Lang(), x + 150, y, (o, e) =>
                {
                    BigPatchConfig.ChkChangeTarget = ChkChangeTarget.Checked;
                }, BigPatchConfig.ChkChangeTarget);

                Lab = new DXLabel
                {
                    Parent = Android,
                    Text = "间隔(分):".Lang(),
                    Outline = true,
                    Hint = "用消耗品特修".Lang(),
                    Location = new Point(x, y += 25),
                };
                ConsumeRepairTime = new DXNumberBox
                {
                    Parent = Android,
                    Size = new Size(80, 20),
                    ValueTextBox = { Size = new Size(40, 18) },
                    MaxValue = 100000,
                    MinValue = 1,
                    Value = BigPatchConfig.ConsumeRepairTime,
                    UpButton = { Location = new Point(63, 1) },
                    Location = new Point(x + Lab.Size.Width, y),
                };
                ConsumeRepairTime.ValueTextBox.ValueChanged += (o, e) =>
                {
                    BigPatchConfig.ConsumeRepairTime = ConsumeRepairTime.Value;
                };
                ChkConsumeRepair = CreateCheckBox(Android, "自动特修".Lang(), x + 150, y, (o, e) =>
                {
                    BigPatchConfig.ChkConsumeRepair = ChkConsumeRepair.Checked;
                }, BigPatchConfig.ChkConsumeRepair);
            }
            public void UpdateMagic()
            {
                //初始化技能窗口
                if (SkillListBoxItems.Count > 0)
                {
                    foreach (DXListBoxItem item in SkillListBoxItems)
                        item.Dispose();
                    SkillListBoxItems.Clear();
                }

                foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in GameScene.Game.User.Magics)
                {
                    ClientUserMagic m = pair.Value;
                    SkillListBoxItems.Add(new DXListBoxItem
                    {
                        Parent = AndroidSkills.ListBox,
                        Label = { Text = m.Info.Name },
                        Item = m.Info.Magic,
                    });

                }
            }
        };
        #endregion

        #region 新版喝药保护

        public class DXAutoPotionTab : DXTab
        {
            /// <summary>
            /// 智能红分组
            /// </summary>
            public DXGroupBox GroupMindHP;
            /// <summary>
            /// 自定义红分组
            /// </summary>
            public DXGroupBox GroupCustomHP;
            /// <summary>
            /// 自定义蓝分组
            /// </summary>
            public DXGroupBox GroupCustomMP;
            /// <summary>
            /// 智能蓝分组
            /// </summary>
            public DXGroupBox GroupMindMP;
            /// <summary>
            /// 自定义蓝分组
            /// </summary>
            public DXGroupBox Other;
            /// <summary>
            /// 智能喝红
            /// </summary>
            public DXCheckBox chkMindHP;
            /// <summary>
            /// 同步HP
            /// </summary>
            public DXCheckBox chkHPSynchro;
            /// <summary>
            /// HP保持值
            /// </summary>
            public DXNumberBox NumberMindHP;
            /// <summary>
            /// 自动喝药HP 1-8,13-14,MP 9-12,
            /// </summary>
            public DXCheckBox[] ChkCustomHMPs = new DXCheckBox[16];
            /// <summary>
            /// 自动喝药值HP 1-8,13-14,MP 9-12,
            /// </summary>
            public DXNumberBox[] NumberCustomHMPs = new DXNumberBox[16];

            /// <summary>
            /// 自动消耗品HP 1-8,13-14,MP 9-12,
            /// </summary>
            public DXComboBox[] CombConsumables = new DXComboBox[14];
            /// <summary>
            /// 智能喝蓝
            /// </summary>
            public DXCheckBox chkMindMP;
            /// <summary>
            /// 同步HP
            /// </summary>
            public DXCheckBox chkMPSynchro;
            /// <summary>
            /// HP保持值
            /// </summary>
            public DXNumberBox NumberMindMP;
            /// <summary>
            /// 自动下线
            /// </summary>
            public DXCheckBox chkOutoOffline;
            /// <summary>
            /// 自动下线值
            /// </summary>
            public DXNumberBox NumberOutoOffline;
            /// <summary>
            /// 自动治愈术
            /// </summary>
            public DXCheckBox chkOutoCure;
            /// <summary>
            /// 自动治愈术值
            /// </summary>
            public DXNumberBox NumberOutoCure;

            public DXAutoPotionTab()
            {
                var potList = Globals.ItemInfoList.Binding.Where(p => p.CanAutoPot);
                var healthList = potList.Where(p => p.Stats.Values.Any(c => c.Key == Stat.Health));
                var manaList = potList.Where(p => p.Stats.Values.Any(c => c.Key == Stat.Mana));
                //回城卷、随机卷、特修水
                var rollList = Globals.ItemInfoList.Binding.Where(p => p.ItemType == ItemType.Consumable && (p.Shape == 2 || p.Shape == 3));
                Border = true;
                int x = 25, y = 35, Yspac = 25;

                GroupMindHP = new DXGroupBox
                {
                    Parent = this,
                    Size = new Size(380, 95 + 152 - 28),
                    Location = new Point(10, 0),
                    Name = { Text = "PK模式喝红(优先使用万年)".Lang(), },
                };
                GroupCustomHP = new DXGroupBox
                {
                    Parent = this,
                    Size = new Size(380, 177 - 28),
                    Location = new Point(10, 70),
                    Name = { Text = "自动喝红药".Lang(), },
                };
                GroupMindMP = new DXGroupBox
                {
                    Parent = this,
                    Size = new Size(380, 220 - 29),
                    Location = new Point(10, 215),
                    Name = { Text = "PK模式喝蓝(优先使用万年)".Lang(), },
                };
                GroupCustomMP = new DXGroupBox
                {
                    Parent = this,
                    Size = new Size(380, 150 - 29),
                    Location = new Point(10, 285),
                    Name = { Text = "自动喝蓝药".Lang(), },
                };
                Other = new DXGroupBox
                {
                    Parent = this,
                    Size = new Size(380, 185),
                    Location = new Point(275, 215),
                    Name = { Text = "其他".Lang() },
                    Visible = false,
                };

                chkMindHP = CreateCheckBox(GroupMindHP, "HP保持值:".Lang(), x, y, (o, e) =>
                {
                    BigPatchConfig.HPAuto = chkMindHP.Checked;
                    HookHelper.SaveAutoPotionConfig();
                }, BigPatchConfig.HPAuto);

                NumberMindHP = new DXNumberBox
                {
                    Parent = GroupMindHP,
                    Location = new Point(x + 80, y),
                    Size = new Size(100, 20),
                    ValueTextBox = { Size = new Size(60, 18) },
                    MaxValue = 99999999,
                    MinValue = 0,
                    Value = BigPatchConfig.HP,
                    UpButton = { Location = new Point(83, 1) }
                };
                NumberMindHP.ValueTextBox.ValueChanged += (o, e) =>
                {
                    BigPatchConfig.HP = NumberMindHP.Value;
                    HookHelper.SaveAutoPotionConfig();
                };

                y += Yspac;
                chkHPSynchro = CreateCheckBox(GroupMindHP, "智能调整最大血量%".Lang(), x, y, (o, e) =>
                {
                    BigPatchConfig.HPSync = chkHPSynchro.Checked;
                    if (BigPatchConfig.HPSync)
                    {
                        NumberMindHP.ValueTextBox.Value = GameScene.Game.User.Stats[Stat.Health];// - Math.Min(200, GameScene.Game.User.Level * 10);
                    }
                    HookHelper.SaveAutoPotionConfig();

                }, BigPatchConfig.HPSync);

                #region 自动喝红药
                y = 10;
                //DXLabel Lab = new DXLabel
                //{
                //    Parent = GroupCustomHP,
                //    Text = "HP低于设置值".Lang(),
                //    Location = new Point(x + 25, y),
                //};

                //Lab = new DXLabel
                //{
                //    Parent = GroupCustomHP,
                //    Text = "使用消耗道具".Lang(),
                //    Location = new Point(x + 120, y),
                //};

                for (var i = 0; i < 8; i++)
                {
                    y += Yspac + 3;

                    ChkCustomHMPs[i] = new DXCheckBox
                    {
                        Parent = GroupCustomHP,
                        Size = new Size(80, 20),
                        bAlignRight = false,
                        Location = new Point(x, y),
                        Tag = i
                    };
                    ChkCustomHMPs[i].CheckedChanged += AutoPotionChecked;
                    NumberCustomHMPs[i] = new DXNumberBox
                    {
                        Parent = GroupCustomHP,
                        Location = new Point(x + 20, y),
                        Size = new Size(80, 20),
                        ValueTextBox = { Size = new Size(40, 18) },
                        MaxValue = 99999999,
                        MinValue = 0,
                        UpButton = { Location = new Point(63, 1) },
                        Tag = i
                    };
                    NumberCustomHMPs[i].ValueTextBox.ValueChanged += AutoPotionChange;
                    CombConsumables[i] = new DXComboBox
                    {
                        Parent = GroupCustomHP,
                        Location = new Point(x + 105, y),
                        Size = new Size(110, 18),
                        Tag = i
                    };
                    foreach (var item in healthList)
                    {
                        new DXListBoxItem
                        {
                            Parent = CombConsumables[i].ListBox,
                            Label = { Text = item.ItemName },
                            Item = item
                        };
                    }
                    CombConsumables[i].SelectedItemChanged += AutoPotionSelectChange;
                }
                #endregion


                #region 自动喝蓝药
                x = 25;
                y = 37;
                chkMindMP = CreateCheckBox(GroupMindMP, "MP保持值:".Lang(), x, y, (o, e) =>
                {
                    BigPatchConfig.MPAuto = chkMindMP.Checked;
                    HookHelper.SaveAutoPotionConfig();
                }, BigPatchConfig.MPAuto);

                NumberMindMP = new DXNumberBox
                {
                    Parent = GroupMindMP,
                    Location = new Point(x + 80, y),
                    Size = new Size(100, 20),
                    ValueTextBox = { Size = new Size(60, 18) },
                    MaxValue = 99999999,
                    MinValue = 0,
                    Value = BigPatchConfig.MP,
                    UpButton = { Location = new Point(83, 1) },
                };
                NumberMindMP.ValueTextBox.ValueChanged += (o, e) =>
                {
                    BigPatchConfig.MP = NumberMindMP.Value;
                    HookHelper.SaveAutoPotionConfig();
                };

                y += Yspac;
                chkMPSynchro = CreateCheckBox(GroupMindMP, "智能调整最大蓝量%".Lang(), x, y, (o, e) =>
                {
                    BigPatchConfig.MPSync = chkMPSynchro.Checked;
                    if (BigPatchConfig.MPSync)
                    {
                        NumberMindMP.ValueTextBox.Value = GameScene.Game.User.Stats[Stat.Mana]; // - Math.Min(200, GameScene.Game.User.Level * 10);
                    }
                    HookHelper.SaveAutoPotionConfig();
                }, BigPatchConfig.MPSync);

                y = 10;

                for (var i = 8; i < 12; i++)
                {
                    y += Yspac + 3;

                    ChkCustomHMPs[i] = new DXCheckBox
                    {
                        Parent = GroupCustomMP,
                        Size = new Size(80, 20),
                        bAlignRight = false,
                        Location = new Point(x, y),
                        Tag = i
                    };
                    ChkCustomHMPs[i].CheckedChanged += AutoPotionChecked;
                    NumberCustomHMPs[i] = new DXNumberBox
                    {
                        Parent = GroupCustomMP,
                        Location = new Point(x + 20, y),
                        Size = new Size(80, 20),
                        ValueTextBox = { Size = new Size(40, 18) },
                        MaxValue = 99999999,
                        MinValue = 0,
                        UpButton = { Location = new Point(63, 1) },
                        Tag = i
                    };
                    NumberCustomHMPs[i].ValueTextBox.ValueChanged += AutoPotionChange;
                    CombConsumables[i] = new DXComboBox
                    {
                        Parent = GroupCustomMP,
                        Location = new Point(x + 105, y),
                        Size = new Size(110, 18),
                        Tag = i
                    };
                    foreach (var item in manaList)
                    {
                        new DXListBoxItem
                        {
                            Parent = CombConsumables[i].ListBox,
                            Label = { Text = item.ItemName },
                            Item = item
                        };
                    }
                    CombConsumables[i].SelectedItemChanged += AutoPotionSelectChange;
                }
                #endregion

                x = 25;
                y = 10;
                for (var i = 12; i < 14; i++)
                {
                    y += Yspac + 7;

                    ChkCustomHMPs[i] = new DXCheckBox
                    {
                        Parent = Other,
                        Size = new Size(80, 20),
                        bAlignRight = false,
                        Location = new Point(x, y),
                        Tag = i
                    };
                    ChkCustomHMPs[i].CheckedChanged += AutoPotionChecked;
                    NumberCustomHMPs[i] = new DXNumberBox
                    {
                        Parent = Other,
                        Location = new Point(x + 145, y),
                        Size = new Size(80, 20),
                        ValueTextBox = { Size = new Size(40, 18) },
                        MaxValue = 99999999,
                        MinValue = 0,
                        UpButton = { Location = new Point(63, 1) },
                        Tag = i
                    };
                    NumberCustomHMPs[i].ValueTextBox.ValueChanged += AutoPotionChange;
                    CombConsumables[i] = new DXComboBox
                    {
                        Parent = Other,
                        Location = new Point(x + 25, y),
                        Size = new Size(110, 18),
                        Tag = i
                    };
                    foreach (var item in rollList)
                    {
                        new DXListBoxItem
                        {
                            Parent = CombConsumables[i].ListBox,
                            Label = { Text = item.ItemName },
                            Item = item
                        };
                    }
                    CombConsumables[i].SelectedItemChanged += AutoPotionSelectChange;
                }

                y += Yspac + 7;
                ChkCustomHMPs[14] = new DXCheckBox
                {
                    Label = { Text = "自动下线    当HP低于:".Lang() },
                    Parent = Other,
                    Size = new Size(80, 20),
                    bAlignRight = false,
                    Location = new Point(x, y),
                    Tag = 14
                };
                ChkCustomHMPs[14].CheckedChanged += AutoPotionChecked;
                NumberCustomHMPs[14] = new DXNumberBox
                {
                    Parent = Other,
                    Location = new Point(x + 145, y),
                    Size = new Size(80, 20),
                    ValueTextBox = { Size = new Size(40, 18) },
                    MaxValue = 99999999,
                    MinValue = 0,
                    UpButton = { Location = new Point(63, 1) },
                    Tag = 14
                };
                NumberCustomHMPs[14].ValueTextBox.ValueChanged += AutoPotionChange;

                y += Yspac + 7;
                ChkCustomHMPs[15] = new DXCheckBox
                {
                    Label = { Text = "道士加血    当HP低于:".Lang() },
                    Parent = Other,
                    Size = new Size(80, 20),
                    bAlignRight = false,
                    Location = new Point(x, y),
                    Tag = 15
                };
                ChkCustomHMPs[15].CheckedChanged += AutoPotionChecked;
                NumberCustomHMPs[15] = new DXNumberBox
                {
                    Parent = Other,
                    Location = new Point(x + 145, y),
                    Size = new Size(80, 20),
                    ValueTextBox = { Size = new Size(40, 18) },
                    MaxValue = 99999999,
                    MinValue = 0,
                    UpButton = { Location = new Point(63, 1) },
                    Tag = 15
                };
                ChkCustomHMPs[15].CheckedChanged += AutoPotionChecked;
            }

            #region methods
            private void AutoPotionChecked(object sender, EventArgs e)
            {
                if (Updating) return;
                var checkbox = sender as DXCheckBox;
                if (checkbox.Tag != null)
                {
                    var index = Convert.ToInt32(checkbox.Tag);
                    SendUpdate(index);
                }
            }
            private void AutoPotionSelectChange(object sender, EventArgs e)
            {
                if (Updating) return;
                var infoBox = sender as DXComboBox;
                if (infoBox != null)
                {
                    var index = Convert.ToInt32(infoBox.Tag);
                    SendUpdate(index);
                }
            }

            private void AutoPotionChange(object sender, EventArgs e)
            {
                if (Updating) return;
                var potionBox = sender as DXNumberTextBox;
                if (potionBox != null)
                {
                    var index = Convert.ToInt32(potionBox.Parent.Tag);
                    SendUpdate(index);
                }
            }

            public void LoadConfig()
            {
                Updating = true;

                chkMindHP.Checked = BigPatchConfig.HPAuto;
                chkHPSynchro.Checked = BigPatchConfig.HPSync;
                NumberMindHP.ValueTextBox.Value = BigPatchConfig.HP;

                chkMindMP.Checked = BigPatchConfig.MPAuto;
                chkMPSynchro.Checked = BigPatchConfig.MPSync;
                NumberMindMP.ValueTextBox.Value = BigPatchConfig.MP;

                for (var i = 0; i < HookHelper.Links.Length; i++)
                {
                    var link = HookHelper.Links[i];
                    if (link != null)
                    {
                        var index = link.Slot;
                        ChkCustomHMPs[i].Checked = link.Enabled;
                        NumberCustomHMPs[i].ValueTextBox.Value = index >= 8 && index <= 11 ? link.Mana : link.Health;
                        if (i < 14)
                        {
                            CombConsumables[i].ListBox.SelectItem(Globals.ItemInfoList.Binding.FirstOrDefault(x => x.Index == link.LinkInfoIndex));
                        }
                    }
                    else
                    {
                        ChkCustomHMPs[i].Checked = false;
                        NumberCustomHMPs[i].ValueTextBox.Value = 0;
                        if (i < 14)
                        {
                            CombConsumables[i].ListBox.SelectedItem = null;
                        }
                    }
                }

                Updating = false;
            }

            public void UserChanged()
            {
                if (GameScene.Game.User.Stats[Stat.Health] != 0 && BigPatchConfig.HPSync)
                {
                    NumberMindHP.ValueTextBox.Value = GameScene.Game.User.Stats[Stat.Health];
                }
                if (GameScene.Game.User.Stats[Stat.Mana] != 0 && BigPatchConfig.MPSync)
                {
                    NumberMindHP.ValueTextBox.Value = GameScene.Game.User.Stats[Stat.Mana];
                }

            }

            private void SendUpdate(int index)
            {
                if (GameScene.Game.Observer) return;

                var value = NumberCustomHMPs[index].ValueTextBox.Value;
                var str = $"{ChkCustomHMPs[index].Checked}|{value}";
                if (index < 14)
                {
                    var itemInfo = CombConsumables[index].SelectedItem as ItemInfo;
                    str = $"{ChkCustomHMPs[index].Checked}|{value}|{itemInfo?.ItemName ?? ""}";
                }
                HookHelper.UpdateOrAdd(index, str);
                switch (index)
                {
                    case 0:
                        BigPatchConfig.HP1 = str;
                        break;
                    case 1:
                        BigPatchConfig.HP2 = str;
                        break;
                    case 2:
                        BigPatchConfig.HP3 = str;
                        break;
                    case 3:
                        BigPatchConfig.HP4 = str;
                        break;
                    case 4:
                        BigPatchConfig.HP5 = str;
                        break;
                    case 5:
                        BigPatchConfig.HP6 = str;
                        break;
                    case 6:
                        BigPatchConfig.HP7 = str;
                        break;
                    case 7:
                        BigPatchConfig.HP8 = str;
                        break;
                    case 8:
                        BigPatchConfig.MP1 = str;
                        break;
                    case 9:
                        BigPatchConfig.MP2 = str;
                        break;
                    case 10:
                        BigPatchConfig.MP3 = str;
                        break;
                    case 11:
                        BigPatchConfig.MP4 = str;
                        break;
                    case 12:
                        BigPatchConfig.Roll1 = str;
                        break;
                    case 13:
                        BigPatchConfig.Roll2 = str;
                        break;
                    case 14:
                        BigPatchConfig.OffLine = str;
                        break;
                    case 15:
                        BigPatchConfig.AutoHeal = str;
                        break;
                }
                HookHelper.SaveAutoPotionConfig();
            }
            #endregion

        }

        #endregion

        #region 聊天
        public class DXAnsweringTab : DXTab
        {
            public DXCheckBox ChkMsgNotify;
            public DXCheckBox ChkAutoReplay;
            public DXComboBox CombAutoReplayText;
            public DXCheckBox ChkSaveSayRecord;
            public DXCheckBox ChkShieldNpcWords;
            public DXCheckBox ChkShieldMonsterWords;
            public DXAnsweringTab()
            {
                ChkAutoReplay = new DXCheckBox
                {
                    Parent = this,
                    Label = { Text = "自动回复".Lang() },
                    bAlignRight = false,
                    Location = new Point(10, 10),
                    Checked = BigPatchConfig.ChkAutoReplay,
                };
                ChkAutoReplay.CheckedChanged += (o, e) =>
                {
                    BigPatchConfig.ChkAutoReplay = ChkAutoReplay.Checked;
                };

                CombAutoReplayText = new DXComboBox
                {
                    Parent = this,
                    Location = new Point(10, 10),
                    Size = new Size(32, 18),
                };

                CombAutoReplayText.SelectedItemChanged += (o, e) =>
                {
                    BigPatchConfig.AutoReplayItem = (int)CombAutoReplayText.SelectedItem;
                };
                new DXListBoxItem
                {
                    Parent = CombAutoReplayText.ListBox,
                    Label = { Text = $"您好，我有事不在".Lang() },
                    Item = 0
                };
                new DXListBoxItem
                {
                    Parent = CombAutoReplayText.ListBox,
                    Label = { Text = "我去吃饭了....".Lang() },
                    Item = 1
                };
                new DXListBoxItem
                {
                    Parent = CombAutoReplayText.ListBox,
                    Label = { Text = "挂机中.....".Lang() },
                    Item = 2
                };
                new DXListBoxItem
                {
                    Parent = CombAutoReplayText.ListBox,
                    Label = { Text = "挂机练功 请勿打扰".Lang() },
                    Item = 3
                };

                CombAutoReplayText.ListBox.SelectItem(BigPatchConfig.AutoReplayItem);

                ChkMsgNotify = new DXCheckBox
                {
                    Parent = this,
                    Label = { Text = "来消息声音提示".Lang() },
                    bAlignRight = false,
                    Location = new Point(10, 10),
                    Checked = BigPatchConfig.ChkMsgNotify,
                };
                ChkMsgNotify.CheckedChanged += (o, e) =>
                {
                    BigPatchConfig.ChkMsgNotify = ChkMsgNotify.Checked;
                };

                //////////////////////////
                ChkSaveSayRecord = new DXCheckBox
                {
                    Parent = this,
                    Label = { Text = "保存喊话内容".Lang() },
                    bAlignRight = false,
                    Location = new Point(10, 10),
                    Checked = false,//BigPatchConfig.ChkSaveSayRecord,
                };
                ChkSaveSayRecord.CheckedChanged += (o, e) =>
                {
                    // 这个按钮不能记录 因为TextBox销毁无法及时保存文本 所以必须每次手动点击按钮
                    //BigPatchConfig.ChkSaveSayRecord = ChkSaveSayRecord.Checked;
                    if (ChkSaveSayRecord.Checked) SaveSaywords();
                };

                ChkShieldNpcWords = new DXCheckBox
                {
                    Parent = this,
                    Label = { Text = "屏蔽NPC白字".Lang() },
                    bAlignRight = false,
                    Location = new Point(10, 10),
                    Checked = BigPatchConfig.ChkShieldNpcWords,
                    Visible = false,
                };
                ChkShieldNpcWords.CheckedChanged += (o, e) =>
                {
                    BigPatchConfig.ChkShieldNpcWords = ChkShieldNpcWords.Checked;
                };

                ChkShieldMonsterWords = new DXCheckBox
                {
                    Parent = this,
                    Label = { Text = "屏蔽怪物白字".Lang() },
                    bAlignRight = false,
                    Location = new Point(10, 10),
                    Checked = BigPatchConfig.ChkShieldMonsterWords,
                    Visible = false,
                };
                ChkShieldMonsterWords.CheckedChanged += (o, e) =>
                {
                    BigPatchConfig.ChkShieldMonsterWords = ChkShieldMonsterWords.Checked;
                };
            }

            public override void OnSizeChanged(Size oValue, Size nValue)
            {
                base.OnSizeChanged(oValue, nValue);


                ChkAutoReplay.Location = new Point(10, 15);
                CombAutoReplayText.Size = new Size(Size.Width - 20 - ChkAutoReplay.Size.Width - ChkMsgNotify.Size.Width - 40, CombAutoReplayText.Size.Height);
                CombAutoReplayText.Location = new Point(ChkAutoReplay.Location.X + ChkAutoReplay.Size.Width + 20, 15);
                ChkMsgNotify.Location = new Point(Size.Width - 10 - ChkMsgNotify.Size.Width, 15);

                int x, y;
                x = ChkAutoReplay.Location.X;
                y = ChkAutoReplay.Location.Y + ChkAutoReplay.Size.Height + 10;
                //ChkAutoSayWords.Location = new Point(x, y);
                //x += ChkAutoSayWords.Size.Width + 5;
                //IntervalLeft.Location = new Point(x, y);
                //x += IntervalLeft.Size.Width;
                //AutoSayInterval.Location = new Point(x, y);
                //x += AutoSayInterval.Size.Width;
                //IntervalRight.Location = new Point(x, y);
                //x += IntervalRight.Size.Width + 5;
                ChkSaveSayRecord.Location = new Point(x, y);
                x += ChkSaveSayRecord.Size.Width + 5;
                ChkShieldNpcWords.Location = new Point(x, y);
                x += ChkShieldNpcWords.Size.Width + 5;
                ChkShieldMonsterWords.Location = new Point(x, y);

                x = ChkAutoReplay.Location.X;
                y += ChkShieldNpcWords.Size.Height;
                //ChkShieldMonsterWords.Location = new Point(Size.Width - 10 - ChkShieldMonsterWords.Size.Width, y);
                y += ChkShieldMonsterWords.Size.Height;


            }
            public void SaveSaywords()
            {
                BigPatchConfig.AutoSayLines.Clear();

                string str = GameScene.Game?.BigPatchBox?.Commonly?.InputBox?.TextBox?.Text;
                if (str == null) return;

                string[] sep = { "\r\n" };
                string[] lins = str.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < lins.Length; i++) BigPatchConfig.AutoSayLines.Add(lins[i]);
            }
        };
        #endregion

        #region 便签
        public class DXUserNoteBookTab : DXTab
        {
            public DXTextView NoteView;
            public DXUserNoteBookTab()
            {
                NoteView = new DXTextView
                {
                    Parent = this,
                    ReadOnly = false,
                    Visible = false,
                };
                NoteView.TextBox.ScrollBars = ScrollBars.None;
                NoteView.TextBox.MaxLength = 1024;
            }
            public override void OnSizeChanged(Size oValue, Size nValue)
            {
                base.OnSizeChanged(oValue, nValue);
                if (null == NoteView) return;
                NoteView.Size = new Size(Size.Width - 20, Size.Height - 20);
                NoteView.Location = new Point(10, 10);
                NoteView.Visible = true;
            }
        };
        #endregion

        #region 记录
        public class DXSystemMsgRecordTab : DXTab
        {
            public DXListBox LogList;

            [DllImport("kernel32.dll")]
            public static extern int WinExec(string programPath, int operType);

            public DXSystemMsgRecordTab()
            {
                LogList = new DXListBox
                {
                    Parent = this,
                    Size = new Size(120, DXListBox.DefaultHeight),
                    Location = new Point(5, 5),
                };
                DirectoryInfo TheFolder = new DirectoryInfo(".\\SysLogs\\");
                if (!TheFolder.Exists) return;
                foreach (FileInfo NextFile in TheFolder.GetFiles())
                {
                    DXListBoxItem log = new DXListBoxItem
                    {
                        Parent = LogList,
                        Label = { Text = NextFile.Name },
                        Hint = "双击 打开记录文件".Lang(),
                        Tag = NextFile.FullName,
                    };
                    log.MouseDoubleClick += (o, e) =>
                    {
                        DXListBoxItem lab = o as DXListBoxItem;
                        if (lab != null)
                        {
                            WinExec($"notepad.exe {lab.Tag as string}", 5);
                        }
                    };
                }
            }
            public override void OnSizeChanged(Size oValue, Size nValue)  //尺寸变化
            {
                base.OnSizeChanged(oValue, nValue);
                LogList.Size = new Size(Size.Width - 10, Size.Height - 10);
            }
        };
        #endregion

        #region 捡取
        /// <summary>
        /// 道具捡取过滤设置
        /// </summary>
        public class CItemFilterSet
        {
            /// <summary>
            /// 道具序号
            /// </summary>
            public int idx;
            /// <summary>
            /// 道具名字
            /// </summary>
            public string name;
            /// <summary>
            /// 是否极品提示
            /// </summary>
            public bool hint;
            /// <summary>
            /// 是否捡取
            /// </summary>
            public bool pick;
            /// <summary>
            /// 是否显示名字
            /// </summary>
            public bool show;
            /// <summary>
            /// 是否宠物捡取
            /// </summary>
            public bool cpick;
            //public bool buy;

            /// <summary>
            /// 设置值
            /// </summary>
            /// <param name="idx"></param>
            /// <param name="value"></param>
            public void SetValue(int idx, bool value)
            {
                switch (idx)
                {
                    case 0:
                        hint = value;    //极品提示
                        break;
                    case 1:
                        pick = value;    //是否捡取
                        break;
                    case 2:
                        show = value;    //是否显示物品名字
                        break;
                    case 3:
                        cpick = value;  //宠物是否捡取
                        break;
                    /*case 4:
                        buy = value;
                        break;*/
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 道具筛选器 道具过滤
        /// </summary>
        public class CItemFilter
        {
            /// <summary>
            /// 道具名字
            /// </summary>
            public string FileName;
            /// <summary>
            /// 道具筛选器设置列表
            /// </summary>
            public List<CItemFilterSet> Items;
            /// <summary>
            /// 道具初始化关闭
            /// </summary>
            public bool Inited = false;

            /// <summary>
            /// 道具筛选器
            /// </summary>
            public CItemFilter()
            {
                Items = new List<CItemFilterSet>();  //道具列表
                foreach (ItemInfo info in Globals.ItemInfoList.Binding)
                {
                    CItemFilterSet iset = new CItemFilterSet
                    {
                        name = info.ItemName,    //道具名称
                        idx = info.Index,        //道具序号
                    };
                    Items.Add(iset);    //道具列表增加
                }
            }

            /// <summary>
            /// 初始化列表记录在客户端PickupFilter.ini文件里
            /// </summary>
            /// <param name="file"></param>
            public void Initialize()
            {
                //道具名字等列表名字
                string[] lines = File.ReadAllLines(FileName);
                foreach (string line in lines)
                {
                    if (string.IsNullOrEmpty(line) || line[0] == ';') continue;
                    string[] item = line.Split(',');
                    if (item.Length >= 4)
                    {
                        //int idx = 0;
                        //if (!int.TryParse(item[0].Trim(), out idx)) continue;
                        //if (idx == 0 || idx > Items.Count) continue;
                        // CItemFilterSet iset = Items[idx - 1];

                        CItemFilterSet iset = Items.Where(o => o.name == item[0]).FirstOrDefault();

                        bool hint = false;
                        if (bool.TryParse(item[1].Trim(), out hint))
                            iset.hint = hint;
                        bool pick = false;
                        if (bool.TryParse(item[2].Trim(), out pick))
                            iset.pick = pick;
                        bool show = false;
                        if (bool.TryParse(item[3].Trim(), out show))
                            iset.show = show;
                        bool cpick = false;
                        if (bool.TryParse(item[4].Trim(), out cpick))
                            iset.cpick = cpick;
                    }
                }
                Inited = true;
            }
            /// <summary>
            /// 取消初始化
            /// </summary>
            public void Uninitalize()
            {
                if (!File.Exists(FileName))
                {
                    return;
                }
                string[] lines = new string[Items.Count];
                for (int i = 0; i < Items.Count; i++)
                {
                    CItemFilterSet iset = Items[i];
                    if (iset.name == null || iset.idx == 0 || iset.idx > Items.Count) continue;
                    if (!iset.hint && !iset.pick && !iset.show && !iset.cpick) continue;
                    lines[i] = $"{Items[i].name}, {Items[i].hint}, {Items[i].pick}, {Items[i].show}, {Items[i].cpick}";
                }
                File.WriteAllLines(FileName, lines);
            }

            // 宠物拾取过滤 同步到服务器
            public void SendCompanionPickUpFilterUpdates()
            {
                var changeList = Items.Where(x => x.name != null && x.idx != 0 && x.cpick).ToList();
                CEnvir.Enqueue(new C.CompanionPickUpSkipUpdate
                {
                    CompanionPickupSkipList = changeList.Select(y => y.idx).ToList()
                });
            }
        }

        /// <summary>
        /// 自动捡取道具选择
        /// </summary>
        public class DXAutoPickItemTab : DXTab
        {
            /// <summary>
            /// 搜索按钮
            /// </summary>
            public DXButton Search;
            /// <summary>
            /// 搜索类型框
            /// </summary>
            public DXComboBox CombTypeBox;
            /// <summary>
            /// 搜索内容输入框
            /// </summary>
            public DXTextBox sTextBox;
            /// <summary>
            /// 重置按钮
            /// </summary>
            public DXButton AddButton;
            /// <summary>
            /// 清空按钮
            /// </summary>
            public DXButton DelButton;
            /// <summary>
            /// 保存按钮
            /// </summary>
            public DXButton SaveButton;
            /// <summary>
            /// 道具筛选器
            /// </summary>
            public CItemFilter ItemFilter;
            /// <summary>
            /// 自动捡取项目视图
            /// </summary>
            public DXListView ItemView;

            /// <summary>
            /// 自动捡取道具选择
            /// </summary>
            public DXAutoPickItemTab()
            {
                CombTypeBox = new DXComboBox
                {
                    Parent = this,
                    Size = new Size(100, 18),
                    Location = new Point(10, 5),
                    DropDownHeight = 198,
                };
                new DXListBoxItem
                {
                    Parent = CombTypeBox.ListBox,
                    Label = { Text = $"全部".Lang() },
                    Item = null,
                };

                Type itemType = typeof(ItemType);
                for (ItemType i = ItemType.Nothing; i <= ItemType.Medicament; i++)
                {
                    MemberInfo[] infos = itemType.GetMember(i.ToString());

                    DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();

                    new DXListBoxItem
                    {
                        Parent = CombTypeBox.ListBox,
                        Label = { Text = description?.Description ?? i.ToString() },
                        Item = i
                    };
                }
                CombTypeBox.ListBox.SelectItem(null);

                sTextBox = new DXTextBox
                {
                    Parent = this,
                    Size = new Size(120, 20),
                    Location = new Point(CombTypeBox.Location.X + CombTypeBox.Size.Width + 5, CombTypeBox.Location.Y),
                };

                Search = new DXButton
                {
                    Parent = this,
                    Size = new Size(64, 18),
                    Label = { Text = "搜索".Lang() },
                    ButtonType = ButtonType.SmallButton,
                    Location = new Point(sTextBox.Location.X + sTextBox.Size.Width + 5, sTextBox.Location.Y),
                };
                Search.MouseClick += (o, e) =>
                {
                    //if (ItemView.ItemCount == 0) Initialize();

                    string s = sTextBox.TextBox.Text;
                    if (s != null && s.Length > 0)
                    {
                        ItemView.SortByName(s);
                    }
                };

                AddButton = new DXButton
                {
                    Parent = this,
                    Size = new Size(64, 18),
                    Label = { Text = "重置".Lang() },
                    ButtonType = ButtonType.SmallButton,
                    Location = new Point(Search.Location.X + Search.Size.Width + 5, Search.Location.Y),
                };
                AddButton.MouseClick += (o, e) =>
                {
                    if (ItemView.ItemCount == 0)
                    {
                        foreach (ItemInfo info in Globals.ItemInfoList.Binding)
                        {
                            CItemFilterSet iset = new CItemFilterSet
                            {
                                name = info.ItemName,
                            };
                            ItemFilter.Items.Add(iset);
                        }
                        ItemFilter.Inited = true;
                        Initialize();
                    }
                };

                DelButton = new DXButton
                {
                    Parent = this,
                    Size = new Size(64, 18),
                    Label = { Text = "清空".Lang() },
                    ButtonType = ButtonType.SmallButton,
                    Location = new Point(AddButton.Location.X + AddButton.Size.Width + 5, AddButton.Location.Y),
                };
                DelButton.MouseClick += (o, e) =>
                {
                    ItemView.RemoveAll();
                };

                SaveButton = new DXButton
                {
                    Parent = this,
                    Size = new Size(64, 18),
                    Label = { Text = "保存".Lang() },
                    ButtonType = ButtonType.SmallButton,
                    Location = new Point(DelButton.Location.X + DelButton.Size.Width + 5, DelButton.Location.Y),
                };
                SaveButton.MouseClick += (o, e) =>
                {
                    ItemFilter.Uninitalize();
                    ItemFilter.SendCompanionPickUpFilterUpdates();
                };

                ItemView = new DXListView
                {
                    Parent = this,
                    Size = new Size(410, 405),
                    Location = new Point(5, Search.Location.Y + Search.Size.Height + 5),
                    ItemBorder = false,
                };

                ItemView.InsertColumn(0, "物品名称".Lang(), 180, 24, "物品的名字".Lang());
                ItemView.InsertColumn(1, "勾选极品提示".Lang(), 80, 24, "聊天信息中紫色文字提示".Lang());
                ItemView.InsertColumn(2, "勾选不拾取".Lang(), 70, 24, "勾选后该道具不自动拾取".Lang());
                ItemView.InsertColumn(3, "勾选不显名".Lang(), 70, 24, "勾选后物品在地上不显示名字".Lang());
                ItemView.InsertColumn(4, "勾选宠物不拾取".Lang(), 100, 24, "勾选后该道具宠物不自动拾取".Lang());
                /*ItemView.InsertColumn(4, "自动出售".Lang(), 55, 24, "打开杂货店是否自动出售".Lang());
                ItemView.InsertColumn(5, "自动购买".Lang(), 55, 24, "打开杂货店是否自动购买".Lang());
                ItemView.InsertColumn(6, "保持存量".Lang(), 60, 24, "自动购买时是保持背包存量".Lang());*/

                ItemFilter = new CItemFilter();

            }
            /// <summary>
            /// 初始化
            /// </summary>
            public void Initialize()
            {
                ItemFilter.Initialize();
                // 上线 发送一下拾取过滤列表
                // todo 性能影响不确定
                ItemFilter.SendCompanionPickUpFilterUpdates();

                for (int i = 0; i < ItemFilter.Items.Count; i++)
                {
                    CItemFilterSet item = ItemFilter.Items[i];
                    if (item.name == null) continue;
                    if (item.name.Length == 0) continue;

                    uint idx = ItemView.InsertItem(uint.MaxValue, item.name);

                    DXCheckBox chkhint = new DXCheckBox
                    {
                        AutoSize = false,
                        Checked = item.hint,
                        Tag = i,
                    };
                    chkhint.CheckedChanged += (o, e) =>
                    {
                        DXCheckBox chk = o as DXCheckBox;
                        ItemFilter.Items[(int)chk.Tag]?.SetValue(0, chk.Checked);
                    };
                    ItemView.SetItem(idx, 1, chkhint);

                    DXCheckBox chkpick = new DXCheckBox
                    {
                        AutoSize = false,
                        Checked = item.pick,
                        Tag = i,
                    };
                    chkpick.CheckedChanged += (o, e) =>
                    {
                        DXCheckBox chk = o as DXCheckBox;
                        ItemFilter.Items[(int)chk.Tag]?.SetValue(1, chk.Checked);
                    };
                    ItemView.SetItem(idx, 2, chkpick);

                    DXCheckBox chkshow = new DXCheckBox
                    {
                        AutoSize = false,
                        Checked = item.show,
                        Tag = i,
                    };
                    chkshow.CheckedChanged += (o, e) =>
                    {
                        DXCheckBox chk = o as DXCheckBox;
                        ItemFilter.Items[(int)chk.Tag]?.SetValue(2, chk.Checked);
                    };
                    ItemView.SetItem(idx, 3, chkshow);

                    DXCheckBox chkcpick = new DXCheckBox
                    {
                        AutoSize = false,
                        Checked = item.cpick,
                        Tag = i,
                    };
                    chkcpick.MouseClick += (o, e) =>
                    {
                        if (GameScene.Game.Companion == null || !GameScene.Game.Companion.CompanionInfo.Sorting)
                        {
                            chkcpick.Checked = true;
                        }
                    };
                    chkcpick.CheckedChanged += (o, e) =>
                    {
                        DXCheckBox chk = o as DXCheckBox;
                        ItemFilter.Items[(int)chk.Tag]?.SetValue(3, chk.Checked);
                    };
                    ItemView.SetItem(idx, 4, chkcpick);

                    /*DXCheckBox chkbuy = new DXCheckBox
                    {
                        AutoSize = false,
                        Checked = item.buy,
                        Tag = i,
                    };
                    chkbuy.CheckedChanged += (o, e) =>
                    {
                        DXCheckBox chk = o as DXCheckBox;
                        ItemFilter.Items[(int)chk.Tag]?.SetValue(4, chk.Checked);
                    };
                    ItemView.SetItem(idx, 5, chkbuy);

                    DXCheckBox chknum = new DXCheckBox
                    {
                        AutoSize = false,
                        Checked = item.buy,
                        Enabled = false,
                        Tag = i,
                    };
                    chknum.CheckedChanged += (o, e) =>
                    {
                        DXCheckBox chk = o as DXCheckBox;
                        ItemFilter.Items[(int)chk.Tag]?.SetValue(5, chk.Checked);
                    };
                    ItemView.SetItem(idx, 6, chknum);*/
                }
                ItemView.UpdateItems();
            }
            public override void OnSizeChanged(Size oValue, Size nValue)
            {
                base.OnSizeChanged(oValue, nValue);

                //Search.Location = new Point(10, 5);
                if (!ItemView.Visible) return;

                ItemView.Location = new Point(5, Search.Location.Y + Search.Size.Height);
                ItemView.Size = new Size(Size.Width - 10, Size.Height - ItemView.Location.Y);
            }
        };
        #endregion

        #region 信息
        public class DXViewRangeObjectTab : DXTab
        {
            public List<MapObject> Objects;
            public DXViewRangeObjectTab()
            {
                Objects = new List<MapObject>();
            }
            public override void OnSizeChanged(Size oValue, Size nValue)
            {
                base.OnSizeChanged(oValue, nValue);
            }
        };
        #endregion

        #region 魔法
        public class DXMagicHelperTab : DXTab
        {
            public DXListView MagicView;
            public DXMagicHelperTab()
            {
                MagicView = new DXListView
                {
                    Parent = this,
                    Size = new Size(410, 275),
                    Location = new Point(140, 0),
                };
                MagicView.KeyUp += (o, e) =>
                {
                    MagicHelper hlp = MagicView.HeightLight?.Tag as MagicHelper;
                    if (hlp == null) return;

                    //设置快捷键
                    if (!SetShortcutKey(hlp.obj as ClientUserMagic, e.KeyCode)) return;
                    //更新魔法信息
                    UpdateMagic();
                };
                MagicView.InsertColumn(0, "魔法名称".Lang(), 100, 24, "只显示已学过的技能".Lang());
                //MagicView.InsertColumn(1, "等级".Lang(), 35, 24, "技能等级".Lang());
                //MagicView.InsertColumn(2, "快捷键".Lang(), 80, 24, "选中技能，按下F1-F12注册一个快捷键".Lang());
                //MagicView.InsertColumn(3, "扩展".Lang(), 80, 24, "保留".Lang());
                MagicView.InsertColumn(1, "锁人".Lang(), 50, 24, "设置自动锁定人".Lang());
                MagicView.InsertColumn(2, "锁怪".Lang(), 50, 24, "设置自动锁定怪".Lang());
                MagicView.InsertColumn(3, "毒符设定".Lang(), 100, 24, "道士的自动换符设置".Lang());

            }
            public override void OnSizeChanged(Size oValue, Size nValue)
            {
                base.OnSizeChanged(oValue, nValue);
                MagicView.Size = new Size(Size.Width - 20, Size.Height - 20);
                MagicView.Location = new Point(10, 10);
            }
            public bool SetShortcutKey(ClientUserMagic magic, Keys KeyCode)
            {
                SpellKey key = SpellKey.None;
                foreach (KeyBindAction action in CEnvir.GetKeyAction(KeyCode))
                {
                    switch (action)
                    {
                        case KeyBindAction.SpellUse01:
                            key = SpellKey.Spell01;
                            break;
                        case KeyBindAction.SpellUse02:
                            key = SpellKey.Spell02;
                            break;
                        case KeyBindAction.SpellUse03:
                            key = SpellKey.Spell03;
                            break;
                        case KeyBindAction.SpellUse04:
                            key = SpellKey.Spell04;
                            break;
                        case KeyBindAction.SpellUse05:
                            key = SpellKey.Spell05;
                            break;
                        case KeyBindAction.SpellUse06:
                            key = SpellKey.Spell06;
                            break;
                        case KeyBindAction.SpellUse07:
                            key = SpellKey.Spell07;
                            break;
                        case KeyBindAction.SpellUse08:
                            key = SpellKey.Spell08;
                            break;
                        case KeyBindAction.SpellUse09:
                            key = SpellKey.Spell09;
                            break;
                        case KeyBindAction.SpellUse10:
                            key = SpellKey.Spell10;
                            break;
                        case KeyBindAction.SpellUse11:
                            key = SpellKey.Spell11;
                            break;
                        case KeyBindAction.SpellUse12:
                            key = SpellKey.Spell12;
                            break;
                        case KeyBindAction.SpellUse13:
                            key = SpellKey.Spell13;
                            break;
                        case KeyBindAction.SpellUse14:
                            key = SpellKey.Spell14;
                            break;
                        case KeyBindAction.SpellUse15:
                            key = SpellKey.Spell15;
                            break;
                        case KeyBindAction.SpellUse16:
                            key = SpellKey.Spell16;
                            break;
                        case KeyBindAction.SpellUse17:
                            key = SpellKey.Spell17;
                            break;
                        case KeyBindAction.SpellUse18:
                            key = SpellKey.Spell18;
                            break;
                        case KeyBindAction.SpellUse19:
                            key = SpellKey.Spell19;
                            break;
                        case KeyBindAction.SpellUse20:
                            key = SpellKey.Spell20;
                            break;
                        case KeyBindAction.SpellUse21:
                            key = SpellKey.Spell21;
                            break;
                        case KeyBindAction.SpellUse22:
                            key = SpellKey.Spell22;
                            break;
                        case KeyBindAction.SpellUse23:
                            key = SpellKey.Spell23;
                            break;
                        case KeyBindAction.SpellUse24:
                            key = SpellKey.Spell24;
                            break;
                        default:
                            continue;
                    }
                }

                if (key == SpellKey.None) return false;

                switch (GameScene.Game.MagicBarBox.SpellSet)
                {
                    case 1:
                        magic.Set1Key = key;
                        break;
                    case 2:
                        magic.Set2Key = key;
                        break;
                    case 3:
                        magic.Set3Key = key;
                        break;
                    case 4:
                        magic.Set4Key = key;
                        break;
                }

                foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in GameScene.Game.User.Magics)
                {
                    if (pair.Key == magic.Info)
                    {
                        GameScene.Game.MagicBox.Magics[pair.Key].Refresh();
                        continue;
                    }
                    if (pair.Value.Set1Key == magic.Set1Key && magic.Set1Key != SpellKey.None)
                    {
                        pair.Value.Set1Key = SpellKey.None;
                        GameScene.Game.MagicBox.Magics[pair.Key].Refresh();
                        continue;
                    }

                    if (pair.Value.Set2Key == magic.Set2Key && magic.Set2Key != SpellKey.None)
                    {
                        pair.Value.Set2Key = SpellKey.None;
                        GameScene.Game.MagicBox.Magics[pair.Key].Refresh();
                        continue;
                    }

                    if (pair.Value.Set3Key == magic.Set3Key && magic.Set3Key != SpellKey.None)
                    {
                        pair.Value.Set3Key = SpellKey.None;
                        GameScene.Game.MagicBox.Magics[pair.Key].Refresh();
                        continue;
                    }

                    if (pair.Value.Set4Key == magic.Set4Key && magic.Set4Key != SpellKey.None)
                    {
                        pair.Value.Set4Key = SpellKey.None;
                        GameScene.Game.MagicBox.Magics[pair.Key].Refresh();
                        continue;
                    }

                }

                CEnvir.Enqueue(new C.MagicKey { Magic = magic.Info.Magic, Set1Key = magic.Set1Key, Set2Key = magic.Set2Key, Set3Key = magic.Set3Key, Set4Key = magic.Set4Key });

                GameScene.Game.MagicBarBox.UpdateIcons();

                return true;
            }

            public void UpdateMagic()
            {
                if (GameScene.Game.User == null) return;

                uint item = 0;
                MagicView.RemoveAll();
                string[] AmuletName = new string[]
                {
                    "灵魂护身符".Lang(),
                    "狂风护身符".Lang(),
                    "霹雷护身符".Lang(),
                    "幻影护身符".Lang(),
                    "寒气护身符".Lang(),
                    "神圣护身符".Lang(),
                    "火焰护身符".Lang(),
                    "暗黑护身符".Lang(),
                    "护身符".Lang()
                };
                Type SpellKeyType = typeof(SpellKey);
                foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in GameScene.Game.User.Magics)
                {
                    ClientUserMagic magic = pair.Value;
                    if (magic.Info.Magic == MagicType.None) continue;

                    MagicHelper helper = null;
                    for (int n = 0; n < BigPatchConfig.magics.Count; n++)
                    {
                        if (BigPatchConfig.magics[n].TypeID == magic.Info.Magic)
                        {
                            helper = BigPatchConfig.magics[n];
                            break;
                        }
                    }
                    if (helper == null)
                    {
                        helper = new MagicHelper
                        {
                            TypeID = magic.Info.Magic,
                            Name = magic.Info.Name,
                            Key = magic.Set1Key,
                            LockPlayer = true,
                            LockMonster = true,
                            Amulet = -1,
                        };
                        BigPatchConfig.magics.Add(helper);
                    }
                    helper.obj = magic;
                    helper.Name = magic.Info.Name;

                    DXControl line = null;

                    item = MagicView.InsertItem(item, magic.Info.Name);
                    line = MagicView.Items.Controls[(int)item];

                    //保存魔法信息
                    line.Tag = helper;

                    //MagicView.SetItem(item, 1, $"{magic.Level.ToString()}");

                    //查询快捷键
                    //string keyName = "";
                    //if (magic.Set1Key != SpellKey.None)
                    //{
                    //    MemberInfo[] infos = SpellKeyType.GetMember(magic.Set1Key.ToString());
                    //    DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();
                    //    keyName = description.Description;
                    //}

                    //显示快捷键
                    //MagicView.SetItem(item, 2, keyName);
                    //line.KeyUp += (o, e) =>
                    // {
                    //     DXControl ob = o as DXControl;
                    //     if (ob == null) return;

                    //     MagicHelper hlp = ob.Tag as MagicHelper;
                    //     if (hlp == null) return;

                    //     Type KeyType = typeof(SpellKey);
                    //     MemberInfo[] infos = KeyType.GetMember(hlp.Key.ToString());
                    //     DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();

                    //     ob.Text = $"{description?.Description}";
                    //     //设置快捷键
                    //     SetShortcutKey(hlp.obj as ClientUserMagic, e.KeyCode);
                    //     e.Handled = true;

                    // };

                    //显示扩展
                    //keyName = "";
                    //MagicView.SetItem(item, 3, keyName);

                    //锁定
                    MagicView.SetItem(item, 1, CreateCheckBox(line, "", 0, 0, (o, e) =>
                        {
                            DXCheckBox ob = o as DXCheckBox;
                            if (ob == null) return;

                            MagicHelper hlp = ob.Parent.Tag as MagicHelper;
                            if (hlp == null) return;

                            hlp.LockPlayer = ob.Checked;
                            //if (!ob.Checked) MapObject.MagicObject = null;
                        }, helper.LockPlayer));
                    //锁定
                    MagicView.SetItem(item, 2, CreateCheckBox(line, "", 0, 0, (o, e) =>
                    {
                        DXCheckBox ob = o as DXCheckBox;
                        if (ob == null) return;

                        MagicHelper hlp = ob.Parent.Tag as MagicHelper;
                        if (hlp == null) return;

                        hlp.LockMonster = ob.Checked;
                        //if (!ob.Checked) MapObject.MagicObject = null;
                    }, helper.LockMonster));

                    //显示换符设置
                    DXComboBox b = new DXComboBox();
                    MagicView.SetItem(item, 3, b);
                    new DXListBoxItem
                    {
                        Parent = b.ListBox,
                        Label = { Text = $"未选择".Lang() },
                        Item = -1
                    };

                    b.SelectedItemChanged += (o, e) =>
                    {
                        if (GameScene.Game.Observer) return;
                        DXComboBox ob = o as DXComboBox;
                        if (ob == null) return;

                        DXControl ln = ob.Parent as DXControl;
                        if (ln == null) return;

                        MagicHelper hlp = ln.Tag as MagicHelper;
                        if (hlp == null) return;

                        hlp.Amulet = (int)ob.SelectedItem;
                    };

                    if (magic.Info.Magic == MagicType.PoisonDust)
                    {
                        new DXListBoxItem
                        {
                            Parent = b.ListBox,
                            Label =
                        {
                            Text = "红毒".Lang(),
                        },
                            Item = 0,
                        };
                        new DXListBoxItem
                        {
                            Parent = b.ListBox,
                            Label =
                        {
                            Text = "绿毒".Lang(),
                        },
                            Item = 1,
                        };
                        new DXListBoxItem
                        {
                            Parent = b.ListBox,
                            Label =
                        {
                            Text = "红毒绿毒交换".Lang(),
                        },
                            Item = 2,
                        };
                    }
                    else
                    {
                        switch (magic.Info.Magic)
                        {
                            case MagicType.DragonRise://翔空剑法
                            case MagicType.ExplosiveTalisman://灵魂火符
                            case MagicType.ImprovedExplosiveTalisman://灭魂火符
                            case MagicType.GreaterEvilSlayer://月魂凌波
                            case MagicType.EvilSlayer://月魂断玉
                            case MagicType.Purification://云寂术
                            case MagicType.CelestialLight://阴阳法环
                            case MagicType.Resurrection://回生术
                            case MagicType.StrengthOfFaith://移花接玉
                            case MagicType.Invisibility://隐身术
                            case MagicType.SummonSkeleton://召唤骷髅
                            case MagicType.SummonShinsu://召唤神兽
                            case MagicType.SummonJinSkeleton://召唤超强骷髅
                            case MagicType.SummonDemonicCreature://焰魔召唤术
                            case MagicType.DemonExplosion://魔焰强解术
                            case MagicType.Transparency://秒影无踪
                            case MagicType.MagicResistance://幽灵盾
                            case MagicType.ElementalSuperiority://强魔震法
                            case MagicType.MassTransparency://隐魂术
                            case MagicType.MassInvisibility://集体隐身术
                            case MagicType.TrapOctagon://困魔咒
                            case MagicType.BloodLust://猛虎强势
                            case MagicType.Resilience://神圣战甲术
                                {
                                    for (int i = 0; i < AmuletName.Length; i++)
                                    {
                                        new DXListBoxItem
                                        {
                                            Parent = b.ListBox,
                                            Label = { Text = AmuletName[i] },
                                            Item = i,
                                        };
                                    }
                                }
                                break;
                        }
                    }
                    b.ListBox.SelectItem(helper.Amulet);
                }
                MagicView.UpdateItems();
            }
        };
        #endregion

        #region Boss列表
        /// <summary>
        /// BOSS设置
        /// </summary>
        public class CBossFilterSet
        {
            /// <summary>
            /// Boss序号
            /// </summary>
            public int idx;
            /// <summary>
            /// 名字
            /// </summary>
            public string name;

            public int nameHash;
            /// <summary>
            /// 是否提示
            /// </summary>
            public bool remind;
            /// <summary>
            /// 设置值
            /// </summary>
            /// <param name="idx"></param>
            /// <param name="value"></param>
            public void SetValue(int idx, bool value)
            {
                switch (idx)
                {
                    case 0:
                        remind = value;    //提示
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Boss筛选器 Boss过滤
        /// </summary>
        public class CBossFilter
        {
            /// <summary>
            /// Boss名字
            /// </summary>
            public string FileName;
            /// <summary>
            /// 道具筛选器设置列表
            /// </summary>
            public List<CBossFilterSet> Boss;

            /// <summary>
            /// 道具初始化关闭
            /// </summary>
            public bool Inited = false;

            /// <summary>
            /// 道具筛选器
            /// </summary>
            public CBossFilter()
            {
                Boss = new List<CBossFilterSet>();  //Boss列表
            }

            /// <summary>
            /// 初始化列表记录在客户端文件里
            /// </summary>
            /// <param name="file"></param>
            public void Initialize()
            {
                //Boss.Clear(); 

                if (HookHelper.BossFilterLines == null || HookHelper.BossFilterLines.Count == 0)
                    HookHelper.CreateBossFilterFile();

                foreach (string line in HookHelper.BossFilterLines)
                {
                    if (string.IsNullOrEmpty(line) || line[0] == ';') continue;
                    string[] item = line.Split(',');
                    if (item.Length >= 2)
                    {
                        CBossFilterSet iset = new CBossFilterSet
                        {
                            name = item[0],
                            nameHash = item[0].GetHashCode(),
                        };

                        bool remind = false;
                        if (bool.TryParse(item[1].Trim(), out remind))
                            iset.remind = remind;
                        Boss.Add(iset);
                    }
                }
                Inited = true;
            }
            /// <summary>
            /// 取消初始化
            /// </summary>
            public void Uninitalize()
            {
                if (Boss.Count == 0) return;

                string[] lines = new string[Boss.Count];
                for (int i = 0; i < Boss.Count; i++)
                {
                    CBossFilterSet iset = Boss[i];
                    if (iset.name == null) continue;
                    lines[i] = $"{Boss[i].name}, {Boss[i].remind},";
                }
                File.WriteAllLines(FileName, lines);
            }

        }

        public class DXMonBossListTab : DXTab
        {
            /// <summary>
            /// BOSS格子
            /// </summary>
            public DXGroupBox BossGroup;
            /// <summary>
            /// BOSS列表控件
            /// </summary>
            public DXListView BossView;
            /// <summary>
            /// Boss筛选器
            /// </summary>
            public CBossFilter BossFilter;
            public DXTextBox NameTextBox;
            public DXButton AddButton, DelButton;
            public DXControl SelectItem;

            public DXMonBossListTab()
            {
                Border = true;
                BossGroup = new DXGroupBox
                {
                    Parent = this,
                    Size = new Size(380, 420),
                    Location = new Point(10, 0),
                    Name = { Text = "Boss列表".Lang(), },
                };
                BossView = new DXListView
                {
                    Parent = this,
                    Size = new Size(360, 325),
                    Location = new Point(15, 25),
                    ItemBorder = false,
                };

                BossView.InsertColumn(0, "怪物名称".Lang(), 160, 24, "怪物的名字".Lang());
                BossView.InsertColumn(1, "提醒".Lang(), 50, 24, "勾选BOSS提醒".Lang());
                BossView.ItemMouseClick += (s, e) =>
                {
                    SelectItem = s as DXControl;
                    NameTextBox.TextBox.Text = SelectItem.Text;
                };

                NameTextBox = new DXTextBox
                {
                    Location = new Point(25, 367),
                    Parent = this,
                    BackColour = Color.Empty,
                    Border = true,
                    Size = new Size(150, 18),
                };

                AddButton = new DXButton
                {
                    LibraryFile = LibraryFile.GameInter,
                    Index = 1010,
                    Parent = this,
                    Location = new Point(185, 367),
                    Hint = "新增BOSS提醒",
                };
                AddButton.MouseClick += (o, e) =>
                {
                    string name = NameTextBox.TextBox.Text.Trim();
                    if (BossFilter.Boss.Exists(d => d.nameHash == name.GetHashCode())) return;
                    CBossFilterSet iset = new CBossFilterSet
                    {
                        name = name,
                        nameHash = name.GetHashCode(),
                        remind = true,
                    };
                    BossFilter.Boss.Add(iset);
                    UpdateFilterItem();
                };

                DelButton = new DXButton
                {
                    LibraryFile = LibraryFile.GameInter,
                    Index = 1011,
                    Parent = this,
                    Location = new Point(206, 367),
                    Hint = "删除现有BOSS提醒",
                };
                DelButton.MouseClick += (o, e) =>
                {
                    var index = BossFilter.Boss.FindIndex(d => d.name == SelectItem.Text);
                    if (index == -1) return;
                    BossFilter.Boss.RemoveAt(index);
                    BossFilter.Uninitalize();
                    BossView.DeleteItem((uint)index);
                };

                BossFilter = new CBossFilter();
            }


            public void UpdateFilterItem()
            {

                for (int i = 0; i < BossFilter.Boss.Count; i++)
                {
                    CBossFilterSet item = BossFilter.Boss[i];
                    if (item.name == null) continue;
                    if (item.name.Length == 0) continue;

                    var line = BossView.GetItem((uint)i, 0);
                    if (line == null)
                    {
                        uint idx = BossView.InsertItem((uint)i, item.name);

                        DXCheckBox chkremind = new DXCheckBox
                        {
                            AutoSize = false,
                            Checked = item.remind,
                            Tag = i,
                        };
                        chkremind.CheckedChanged += (o, e) =>
                        {
                            DXCheckBox chk = o as DXCheckBox;
                            var index = (int)chk.Tag;
                            if (BossFilter.Boss.Count < index || index < 0) return;
                            BossFilter.Boss[index]?.SetValue(0, chk.Checked);
                            BossFilter.Uninitalize();
                        };
                        BossView.SetItem(idx, 1, chkremind);
                    }
                    else
                    {
                        var chk = BossView.GetItem((uint)i, 1);
                        if (chk != null)
                        {
                            chk.Tag = i;
                        }
                    }
                }
                BossView.UpdateItems();
            }
            /// <summary>
            /// 初始化
            /// </summary>
            public void Initialize()
            {
                BossFilter.Initialize();

                for (int i = 0; i < BossFilter.Boss.Count; i++)
                {
                    CBossFilterSet item = BossFilter.Boss[i];
                    if (item.name == null) continue;
                    if (item.name.Length == 0) continue;

                    uint idx = BossView.InsertItem(uint.MaxValue, item.name);

                    DXCheckBox chkremind = new DXCheckBox
                    {
                        AutoSize = false,
                        Checked = item.remind,
                        Tag = i,
                    };
                    chkremind.CheckedChanged += (o, e) =>
                    {
                        DXCheckBox chk = o as DXCheckBox;
                        BossFilter.Boss[(int)chk.Tag]?.SetValue(0, chk.Checked);
                        BossFilter.Uninitalize();
                    };
                    BossView.SetItem(idx, 1, chkremind);
                }
                BossView.UpdateItems();
            }

            //public override void OnSizeChanged(Size oValue, Size nValue)
            //{
            //    base.OnSizeChanged(oValue, nValue);
            //    if (!BossView.Visible) return;

            //    BossView.Size = new Size(Size.Width - 35, Size.Height - 82);
            //    BossView.Location = new Point(15, 25);
            //}
        }
        #endregion

        #region 主界面
        /// <summary>
        /// 底图
        /// </summary>
        public DXImageControl TabBackGround;
        /// <summary>
        /// 顶部标签
        /// </summary>
        public DXLabel Title1Label;
        /// <summary>
        /// 分页
        /// </summary>
        public DXTabControl TabControl;
        /// <summary>
        /// 常用基础
        /// </summary>
        public DXCommonlyTab Commonly;
        /// <summary>
        /// 辅助挂机
        /// </summary>
        public DXPlayerHelperTab Helper;
        /// <summary>
        /// 自动喝药
        /// </summary>
        public DXAutoPotionTab AutoPotionPage;
        /// <summary>
        /// 聊天记录
        /// </summary>
        public DXAnsweringTab Answering;
        /// <summary>
        /// 用户标签
        /// </summary>
        public DXUserNoteBookTab NoteBook;
        /// <summary>
        /// 系统消息记录
        /// </summary>
        public DXSystemMsgRecordTab MsgRecord;
        /// <summary>
        /// 自动捡取
        /// </summary>
        public DXAutoPickItemTab AutoPick;
        /// <summary>
        /// 可见对象
        /// </summary>
        public DXViewRangeObjectTab ViewRange;
        /// <summary>
        /// 魔法技能辅助
        /// </summary>
        public DXMagicHelperTab Magic;
        /// <summary>
        /// Boss列表
        /// </summary>
        public DXMonBossListTab MonBoss;
        /// <summary>
        /// 保护时间
        /// </summary>
        public DateTime ChkAutoRandomTime;
        /// <summary>
        /// 保护时间
        /// </summary>
        public DateTime ChkConsumeRepairTime;
        /// <summary>
        /// 保护时间
        /// </summary>
        public DateTime ChkRandomTime;
        /// <summary>
        /// 技能时间
        /// </summary>
        private DateTime skillTime1;
        /// <summary>
        /// 技能时间
        /// </summary>
        private DateTime skillTime2;
        /// <summary>
        /// 技能时间
        /// </summary>
        private DateTime skillTime3;

        public int AutoActsStep = 1;
        private DateTime AutoActsTime;

        /// <summary>
        /// 大补贴功能主界面
        /// </summary>
        public BigPatchDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            IgnoreMoveBounds = true;
            Opacity = 0F;

            Location = ClientArea.Location;

            TabBackGround = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 700,
                IsControl = true,
                PassThrough = true,
                ImageOpacity = 0.85F,
                Location = new Point(0, 0)
            };
            Size = TabBackGround.Size;

            CloseButton.Parent = TabBackGround;

            Title1Label = new DXLabel
            {
                Text = "辅助",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            Title1Label.Location = new Point((Size.Width - Title1Label.Size.Width) / 2, 15);

            TabControl = new DXTabControl
            {
                Parent = TabBackGround,
                Location = new Point(14, 42),
                Size = new Size(397, 484),
            };

            Commonly = new DXCommonlyTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "常用".Lang(), Hint = "一些常用功能设置".Lang() } },
                Opacity = 0.7F,
            };

            Helper = new DXPlayerHelperTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "辅助".Lang(), Hint = "挂机功能自动技能".Lang() } },
            };

            AutoPotionPage = new DXAutoPotionTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "保护".Lang(), Hint = "自动使用药品".Lang() } },
                Opacity = 0.7F,
            };

            //Answering = new DXAnsweringTab
            //{
            //    Parent = TabControl,
            //    Border = true,
            //    TabButton = { Label = { Text = "聊天".Lang(), Hint = "自动回复自动喊话".Lang() } },
            //};

            //NoteBook = new DXUserNoteBookTab
            //{
            //    Parent = TabControl,
            //    Border = true,
            //    TabButton = { Label = { Text = "便签".Lang(), Hint = "方便用户记录一些文本".Lang() } },
            //};

            //MsgRecord = new DXSystemMsgRecordTab
            //{
            //    Parent = TabControl,
            //    Border = true,
            //    TabButton = { Label = { Text = "记录".Lang(), Hint = "系统消息记录".Lang() } },
            //};

            //AutoPick = new DXAutoPickItemTab
            //{
            //    Parent = TabControl,
            //    Border = true,
            //    TabButton = { Label = { Text = "拾取".Lang(), Hint = "物品自动拾取设置".Lang() } },
            //};

            //ViewRange = new DXViewRangeObjectTab
            //{
            //    Parent = TabControl,
            //    Border = true,
            //    TabButton = { Label = { Text = "信息".Lang(), Hint = "可见范围内的怪物,人物,以及NPC".Lang() } },
            //};

            Magic = new DXMagicHelperTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "魔法".Lang(), Hint = "魔法技能锁定符更换".Lang() } },
                Opacity = 0.7F,
            };

            MonBoss = new DXMonBossListTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "怪物".Lang(), Hint = "Boss列表自增提醒".Lang() } },
                Opacity = 0.7F,
            };
        }
        #endregion

        /// <summary>
        /// 更新链接
        /// </summary>
        /// <param name="info"></param>
        public void UpdateLinks(StartInformation info)
        {
            if (Commonly != null)
            {
                switch (info.Class)
                {
                    case MirClass.Warrior:
                        Commonly.Warrior.Visible = true;
                        break;
                    case MirClass.Wizard:
                        Commonly.Wizard.Visible = true;
                        break;
                    case MirClass.Taoist:
                        Commonly.Taoist.Visible = true;
                        break;
                    case MirClass.Assassin:
                        Commonly.Assassin.Visible = true;
                        break;
                }
            }
        }

        /// <summary>
        /// 挂机时地图信息更改时
        /// </summary>
        public void OnMapInfoChanged()
        {
            if (Helper != null) Helper.AndroidPlayer.Checked = false;  //停止挂机
        }
        /// <summary>
        /// 挂机时间改变时
        /// </summary>
        /// <param name="AutoTime"></param>
        public void OnTimerChanged(long AutoTime)
        {
            TimeSpan d = new TimeSpan(0, 0, (int)AutoTime);
            if (Helper != null)
            {
                Helper.TimeLable.Text = (d.Hours + d.Days * 24) + ":" + d.Minutes + ":" + d.Seconds;  //修正了挂机时间清零的问题
                if (AutoTime == 0)
                {
                    Helper.AndroidPlayer.Checked = false;   //如果时间为0，停止挂机
                }
            }
        }

        #region 自动技能
        /// <summary>
        /// 自动技能
        /// </summary>
        /// 
        DateTime MagicShieldDelay;
        public void AutoSkills()
        {
            //下面的自动技能每使用一次魔法就直接跳出，因为不可能是同一时间调用多个魔法。
            //所以释放有优先级 从上到下判断
            //if (CEnvir.Now < MapObject.User.NextActionTime || MapObject.User.ActionQueue.Count != 0) return;
            //if (CEnvir.Now < MapObject.User.ServerTime) return;
            if (CEnvir.Now < MapObject.User.NextMagicTime) return;
            if (MapObject.User.MagicAction != null) return;

            Dictionary<string, bool> magicDics = new Dictionary<string, bool>();
            string[] magicstr = { "风之闪避", "风之守护", "魔法盾", "凝血离魂", "阴阳法环", "铁布衫", "破血狂杀" };
            foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in MapObject.User.Magics)
            {
                MagicInfo magic = pair.Key;
                if (Array.IndexOf(magicstr, magic.Name) > -1)
                {
                    magicDics[magic.Name] = true;
                }
                else
                    magicDics[magic.Name] = false;
            }

            #region 各职业buffer技能
            //刺客
            if (GameScene.Game.User.Class == MirClass.Assassin)
            {
                if (BigPatchConfig.AutoEvasion && magicDics.TryGetValue("风之闪避", out bool value))  //自动风之闪避
                {
                    if (GameScene.Game.User.Buffs.All(x => x.Type != BuffType.Evasion))
                    {
                        GameScene.Game.UseMagic(MagicType.Evasion);
                        return;
                    }
                }

                if (BigPatchConfig.AutoRagingWind && magicDics.TryGetValue("风之守护", out value))  //自动风之守护
                {
                    if (GameScene.Game.User.Buffs.All(x => x.Type != BuffType.RagingWind))
                    {
                        GameScene.Game.UseMagic(MagicType.RagingWind);
                        return;
                    }
                }
            }

            //法师
            else if (GameScene.Game.User.Class == MirClass.Wizard)
            {
                if (BigPatchConfig.AutoMagicShield && MagicShieldDelay < CEnvir.Now && magicDics.TryGetValue("魔法盾", out bool value))  //自动魔法盾
                {
                    if (GameScene.Game.User.Buffs.All(x => x.Type != BuffType.MagicShield))
                    {
                        MagicShieldDelay = CEnvir.Now.AddSeconds(2);
                        GameScene.Game.UseMagic(MagicType.MagicShield);
                        return;
                    }
                }

                if (BigPatchConfig.AutoRenounce && magicDics.TryGetValue("凝血离魂", out value))  //自动凝血
                {
                    if (GameScene.Game.User.Buffs.All(x => x.Type != BuffType.Renounce))
                    {
                        GameScene.Game.UseMagic(MagicType.Renounce);
                        return;
                    }
                }
            }

            //道士
            else if (GameScene.Game.User.Class == MirClass.Taoist)
            {
                if (BigPatchConfig.AutoCelestial && magicDics.TryGetValue("阴阳法环", out bool value))//自动阴阳法环
                {
                    if (GameScene.Game.User.Buffs.All(x => x.Type != BuffType.CelestialLight))
                    {
                        GameScene.Game.UseMagic(MagicType.CelestialLight);
                        return;
                    }
                }
            }

            //战士
            else if (GameScene.Game.User.Class == MirClass.Warrior)
            {
                if (BigPatchConfig.AutoDefiance && magicDics.TryGetValue("铁布衫", out bool value))   //自动铁布衫
                {
                    if (GameScene.Game.User.Buffs.All(x => x.Type != BuffType.Defiance))
                    {
                        GameScene.Game.UseMagic(MagicType.Defiance);
                        return;
                    }
                }

                if (BigPatchConfig.AutoMight && magicDics.TryGetValue("破血狂杀", out value))   //自动破血狂杀
                {
                    if (GameScene.Game.User.Buffs.All(x => x.Type != BuffType.Might))
                    {
                        GameScene.Game.UseMagic(MagicType.Might);
                        return;
                    }
                }
            }
            #endregion

            #region 自动技能
            //如果不挂机，持续施法才能使用
            if (!BigPatchConfig.AndroidPlayer && GameScene.Game.ContinuouslyMagic && MapObject.MagicObject != null && !MapObject.MagicObject.Dead)
            {
                if (GameScene.Game.ContinuouslyMagicType != MagicType.None
                    && GameScene.Game.ContinuouslyMagicType != MagicType.FrozenEarth
                    && GameScene.Game.ContinuouslyMagicType != MagicType.ScortchedEarth
                    && GameScene.Game.ContinuouslyMagicType != MagicType.LightningBeam
                    && GameScene.Game.ContinuouslyMagicType != MagicType.GreaterFrozenEarth
                    && GameScene.Game.ContinuouslyMagicType != MagicType.BlowEarth
                    && GameScene.Game.ContinuouslyMagicType != MagicType.Teleportation
                    && GameScene.Game.ContinuouslyMagicType != MagicType.GeoManipulation
                    && GameScene.Game.ContinuouslyMagicType != MagicType.FireWall
                    && GameScene.Game.ContinuouslyMagicType != MagicType.Repulsion
                    && GameScene.Game.ContinuouslyMagicType != MagicType.Heal
                    && GameScene.Game.ContinuouslyMagicType != MagicType.PoisonDust
                    && GameScene.Game.ContinuouslyMagicType != MagicType.Invisibility
                    && GameScene.Game.ContinuouslyMagicType != MagicType.MagicResistance
                    && GameScene.Game.ContinuouslyMagicType != MagicType.MassInvisibility
                    && GameScene.Game.ContinuouslyMagicType != MagicType.Resilience
                    && GameScene.Game.ContinuouslyMagicType != MagicType.TrapOctagon
                    && GameScene.Game.ContinuouslyMagicType != MagicType.ElementalSuperiority
                    && GameScene.Game.ContinuouslyMagicType != MagicType.MassHeal
                    && GameScene.Game.ContinuouslyMagicType != MagicType.BloodLust
                    && GameScene.Game.ContinuouslyMagicType != MagicType.Resurrection
                    && GameScene.Game.ContinuouslyMagicType != MagicType.Purification
                    && GameScene.Game.ContinuouslyMagicType != MagicType.Transparency
                    && GameScene.Game.ContinuouslyMagicType != MagicType.CelestialLight
                    && GameScene.Game.ContinuouslyMagicType != MagicType.SummonSkeleton
                    && GameScene.Game.ContinuouslyMagicType != MagicType.SummonShinsu
                    && GameScene.Game.ContinuouslyMagicType != MagicType.SummonJinSkeleton
                    && GameScene.Game.ContinuouslyMagicType != MagicType.StrengthOfFaith
                    && GameScene.Game.ContinuouslyMagicType != MagicType.TaoistCombatKick
                    && GameScene.Game.ContinuouslyMagicType != MagicType.ShoulderDash
                    && GameScene.Game.ContinuouslyMagicType != MagicType.HalfMoon
                    && GameScene.Game.ContinuouslyMagicType != MagicType.Thrusting
                    && GameScene.Game.ContinuouslyMagicType != MagicType.FlamingSword
                    && GameScene.Game.ContinuouslyMagicType != MagicType.DragonRise
                    && GameScene.Game.ContinuouslyMagicType != MagicType.BladeStorm
                    && GameScene.Game.ContinuouslyMagicType != MagicType.MagicShield)
                {
                    //如果目标在魔法范围，使用魔法
                    if (Functions.InRange(MapObject.MagicObject.CurrentLocation, GameScene.Game.User.CurrentLocation, Globals.MagicRange))
                    {
                        GameScene.Game.UseMagic(GameScene.Game.ContinuouslyMagicType);
                        return;
                    }
                }
            }
            //如果不持续施法，下面的自动技能才可以用，避免冲突
            else
            {
                if (BigPatchConfig.AutoMagicSkill_1 && CEnvir.Now >= skillTime1)
                {
                    if (BigPatchConfig.AutoSkillMagic_1 != MagicType.None)
                    {
                        GameScene.Game.UseMagic(BigPatchConfig.AutoSkillMagic_1);
                        skillTime1 = CEnvir.Now + TimeSpan.FromSeconds(BigPatchConfig.NumbSkill1 > 0 ? BigPatchConfig.NumbSkill1 : 10);
                        return;
                    }
                }
                if (BigPatchConfig.AutoMagicSkill_2 && CEnvir.Now >= skillTime2)
                {
                    if (BigPatchConfig.AutoSkillMagic_2 != MagicType.None)
                    {
                        GameScene.Game.UseMagic(BigPatchConfig.AutoSkillMagic_2);
                        skillTime2 = CEnvir.Now + TimeSpan.FromSeconds(BigPatchConfig.NumbSkill2 > 0 ? BigPatchConfig.NumbSkill2 : 10);
                        return;
                    }
                }
                if (BigPatchConfig.ChkAutoFire && CEnvir.Now >= skillTime3)
                {
                    if (BigPatchConfig.AutoFire == 0) return;
                    GameScene.Game.UseMagic((SpellKey)BigPatchConfig.AutoFire);
                    skillTime3 = CEnvir.Now + TimeSpan.FromSeconds(BigPatchConfig.AutoFireInterval);
                    return;
                }
            }
            #endregion

        }
        #endregion

        #region 自动连招
        /// <summary>
        /// 自动连招
        /// </summary>
        public void AutoActs()
        {
            if (CEnvir.Now >= AutoActsTime)
            {
                AutoActsTime = CEnvir.Now.AddSeconds(1);
                if (GameScene.Game.User.Class == MirClass.Warrior)
                {
                    //没配置连招技能返回
                    switch (BigPatchConfig.ComboType)
                    {
                        case 2:
                            if (BigPatchConfig.Combo1 == MagicType.None || BigPatchConfig.Combo2 == MagicType.None)
                                return;
                            break;
                        case 3:
                            if (BigPatchConfig.Combo1 == MagicType.None || BigPatchConfig.Combo2 == MagicType.None || BigPatchConfig.Combo3 == MagicType.None)
                                return;
                            break;
                        case 4:
                            if (BigPatchConfig.Combo1 == MagicType.None || BigPatchConfig.Combo2 == MagicType.None || BigPatchConfig.Combo3 == MagicType.None || BigPatchConfig.Combo4 == MagicType.None)
                                return;
                            break;
                        case 5:
                            if (BigPatchConfig.Combo1 == MagicType.None || BigPatchConfig.Combo2 == MagicType.None || BigPatchConfig.Combo3 == MagicType.None || BigPatchConfig.Combo4 == MagicType.None || BigPatchConfig.Combo5 == MagicType.None)
                                return;
                            break;
                    }

                    bool canUse1 = GameScene.Game.User.canCombo1,
                        canUse2 = GameScene.Game.User.canCombo2,
                        canUse3 = GameScene.Game.User.canCombo3,
                        canUse4 = GameScene.Game.User.canCombo4,
                        canUse5 = GameScene.Game.User.canCombo5;

                    DateTime magic1Delay = DateTime.MinValue, magic2Delay = DateTime.MinValue,
                        magic3Delay = DateTime.MinValue, magic4Delay = DateTime.MinValue, magic5Delay = DateTime.MinValue;
                    foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in GameScene.Game.User.Magics)
                    {
                        ClientUserMagic m = pair.Value;

                        MagicType type = m.Info.Magic;
                        if (type == BigPatchConfig.Combo1)
                            magic1Delay = m.NextCast;
                        else if (type == BigPatchConfig.Combo2)
                            magic2Delay = m.NextCast;
                        else if (type == BigPatchConfig.Combo3)
                            magic3Delay = m.NextCast;
                        else if (type == BigPatchConfig.Combo4)
                            magic4Delay = m.NextCast;
                        else if (type == BigPatchConfig.Combo5)
                            magic5Delay = m.NextCast;
                    }

                    //////////////////////////////////////
                    // 连招说明：
                    // 二连：可以随意搭配技能组合。
                    // 三连：前两招不能重复，第二招和第三招不能重复，否则可能接不上。
                    // 四连：前三招不能重复，第三招和第四招不能重复，否则可能接不上。
                    // 五连：前四招不能重复，第四招和第五招不能重复，否则可能接不上。
                    //////////////////////////////////////

                    //开始连招
                    if (AutoActsStep == 1)
                    {
                        //二连 释放第一招
                        if (BigPatchConfig.ComboType == 2)
                        {
                            if (!canUse1 && CEnvir.Now >= magic1Delay)
                            {
                                GameScene.Game.UseMagic(BigPatchConfig.Combo1);
                                AutoActsStep = 11;
                            }
                        }
                        //三连 按顺序释放前两招
                        else if (BigPatchConfig.ComboType == 3)
                        {
                            if (!canUse1 && !canUse2 && CEnvir.Now >= magic1Delay)
                            {
                                GameScene.Game.UseMagic(BigPatchConfig.Combo1);
                            }
                            else if (canUse1 && !canUse2 && CEnvir.Now >= magic2Delay)
                            {
                                GameScene.Game.UseMagic(BigPatchConfig.Combo2);
                            }
                            if (canUse1 && canUse2)
                                AutoActsStep = 11;
                        }
                        //四连 按顺序释放前三招
                        else if (BigPatchConfig.ComboType == 4)
                        {
                            if (!canUse1 && !canUse2 && !canUse3 && CEnvir.Now >= magic1Delay)
                            {
                                GameScene.Game.UseMagic(BigPatchConfig.Combo1);
                            }
                            else if (canUse1 && !canUse2 && CEnvir.Now >= magic2Delay)
                            {
                                GameScene.Game.UseMagic(BigPatchConfig.Combo2);
                            }
                            else if (canUse2 && !canUse3 && CEnvir.Now >= magic3Delay)
                            {
                                GameScene.Game.UseMagic(BigPatchConfig.Combo3);
                            }
                            if (canUse1 && canUse2 && canUse3)
                                AutoActsStep = 11;
                        }
                        //五连 按顺序释放前四招
                        else if (BigPatchConfig.ComboType == 5)
                        {
                            if (!canUse1 && !canUse2 && !canUse3 && !canUse4 && CEnvir.Now >= magic1Delay)
                            {
                                GameScene.Game.UseMagic(BigPatchConfig.Combo1);
                            }
                            else if (canUse1 && !canUse2 && CEnvir.Now >= magic2Delay)
                            {
                                GameScene.Game.UseMagic(BigPatchConfig.Combo2);
                            }
                            else if (canUse2 && !canUse3 && CEnvir.Now >= magic3Delay)
                            {
                                GameScene.Game.UseMagic(BigPatchConfig.Combo3);
                            }
                            else if (canUse3 && !canUse4 && CEnvir.Now >= magic4Delay)
                            {
                                GameScene.Game.UseMagic(BigPatchConfig.Combo4);
                            }
                            if (canUse1 && canUse2 && canUse3 && canUse4)
                                AutoActsStep = 11;
                        }

                    }
                    //连招准备好出提示
                    else if (AutoActsStep == 11)
                    {
                        if (BigPatchConfig.ComboType == 2)
                        {
                            //在漏气时间内出提示  服务端代码里面漏气的时间是24秒内
                            if (canUse1 && CEnvir.Now >= magic1Delay && CEnvir.Now <= magic1Delay.AddSeconds(1))
                            {
                                GameScene.Game.ReceiveChat(">>> 提示：二连招准备就绪！".Lang(), MessageType.Hint);
                                GameScene.Game.User.ComboActive = true;
                            }
                            //漏气就重来
                            else if (!canUse1)
                            {
                                AutoActsStep = 1;
                                GameScene.Game.User.ComboActive = false;
                            }
                        }
                        else if (BigPatchConfig.ComboType == 3)
                        {
                            //在漏气时间内出提示  服务端代码里面漏气的时间是26秒内
                            if (canUse1 && canUse2 && CEnvir.Now >= magic1Delay && CEnvir.Now <= magic1Delay.AddSeconds(1))
                            {
                                GameScene.Game.ReceiveChat(">>> 提示：三连招准备就绪！".Lang(), MessageType.Hint);
                                GameScene.Game.User.ComboActive = true;
                            }
                            //漏气就重来
                            else if (CEnvir.Now > magic1Delay.AddSeconds(26) || CEnvir.Now > magic2Delay.AddSeconds(26))
                            {
                                AutoActsStep = 1;
                                GameScene.Game.User.ComboActive = false;
                            }
                        }
                        else if (BigPatchConfig.ComboType == 4)
                        {
                            //在漏气时间内出提示  服务端代码里面漏气的时间是24秒内
                            if (canUse1 && canUse2 && canUse3 && CEnvir.Now >= magic1Delay && CEnvir.Now <= magic1Delay.AddSeconds(1))
                            {
                                GameScene.Game.ReceiveChat(">>> 提示：四连招准备就绪！".Lang(), MessageType.Hint);
                                GameScene.Game.User.ComboActive = true;
                            }
                            //漏气就重来
                            else if (CEnvir.Now > magic1Delay.AddSeconds(24) || CEnvir.Now > magic2Delay.AddSeconds(24) || CEnvir.Now > magic3Delay.AddSeconds(24))
                            {
                                AutoActsStep = 1;
                                GameScene.Game.User.ComboActive = false;
                            }
                        }
                        else if (BigPatchConfig.ComboType == 5)
                        {
                            //在漏气时间内出提示  服务端代码里面漏气的时间是24秒内
                            if (canUse1 && canUse2 && canUse3 && canUse4 && CEnvir.Now >= magic1Delay && CEnvir.Now <= magic1Delay.AddSeconds(1))
                            {
                                GameScene.Game.ReceiveChat(">>> 提示：五连招准备就绪！".Lang(), MessageType.Hint);
                                GameScene.Game.User.ComboActive = true;
                            }
                            //漏气就重来
                            else if (CEnvir.Now > magic1Delay.AddSeconds(24) || CEnvir.Now > magic2Delay.AddSeconds(24) || CEnvir.Now > magic3Delay.AddSeconds(24) || CEnvir.Now > magic4Delay.AddSeconds(24))
                            {
                                AutoActsStep = 1;
                                GameScene.Game.User.ComboActive = false;
                            }
                        }

                    }
                    //开始最后一招
                    if (AutoActsStep == 2)
                    {
                        if (BigPatchConfig.ComboType == 2)
                        {
                            if (!canUse2 && CEnvir.Now >= magic2Delay)
                            {
                                GameScene.Game.UseMagic(BigPatchConfig.Combo2);
                                AutoActsStep = 21;
                            }
                        }
                        else if (BigPatchConfig.ComboType == 3)
                        {
                            if (!canUse3 && CEnvir.Now >= magic3Delay)
                            {
                                GameScene.Game.UseMagic(BigPatchConfig.Combo3);
                                AutoActsStep = 21;
                            }
                        }
                        else if (BigPatchConfig.ComboType == 4)
                        {
                            if (!canUse4 && CEnvir.Now >= magic4Delay)
                            {
                                GameScene.Game.UseMagic(BigPatchConfig.Combo4);
                                AutoActsStep = 21;
                            }
                        }
                        else if (BigPatchConfig.ComboType == 5)
                        {
                            if (!canUse5 && CEnvir.Now >= magic5Delay)
                            {
                                GameScene.Game.UseMagic(BigPatchConfig.Combo5);
                                AutoActsStep = 21;
                            }
                        }

                    }
                    else if (AutoActsStep == 21)
                    {
                        bool can = false;
                        if (BigPatchConfig.ComboType == 2)
                            can = canUse2;
                        else if (BigPatchConfig.ComboType == 3)
                            can = canUse3;
                        else if (BigPatchConfig.ComboType == 4)
                            can = canUse4;
                        else if (BigPatchConfig.ComboType == 5)
                            can = canUse5;

                        //重来
                        if (!can)
                        {
                            AutoActsStep = 1;
                            GameScene.Game.User.ComboActive = false;
                        }

                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// 更新自动辅助
        /// </summary>
        public void UpdateAutoAssist()
        {
            //自动连招
            if (BigPatchConfig.AutoCombo && BigPatchConfig.ComboType > 1)
                AutoActs();

            //自动技能 
            AutoSkills();

            if (BigPatchConfig.ChkAutoPick) PickupItems();  //自动拾取

            if (BigPatchConfig.AndroidPlayer)
            {
                if (GameScene.Game.User.Dead)
                {
                    if (BigPatchConfig.AndroidBackCastle)
                    {
                        CEnvir.Enqueue(new C.TownRevive());
                    }
                }

                if (BigPatchConfig.ChkAutoRandom)  //自动随机
                {
                    if (CEnvir.Now > ChkAutoRandomTime)
                    {
                        DXItemCell cell;
                        cell = GameScene.Game.InventoryBox.Grid.Grid.FirstOrDefault(x => x?.Item?.Info.ItemType == ItemType.Consumable && x?.Item?.Info.Shape == 3);
                        if (cell?.UseItem() == true)
                        {
                            ChkAutoRandomTime = CEnvir.Now.AddSeconds(BigPatchConfig.TimeAutoRandom);//使用延时的秒数
                        }
                    }
                }

                //if (BigPatchConfig.AndroidMinPHRandom)  //血少飞随机
                //{
                //    float protecthp = 0.30f;
                //    protecthp = BigPatchConfig.AndroidRandomMinPHValue / 100f;
                //    if (GameScene.Game.User.CurrentHP < GameScene.Game.User.Stats[Stat.Health] * protecthp)
                //    {
                //        if (CEnvir.Now > _ProtectTime)
                //        {
                //            DXItemCell cell;
                //            cell = GameScene.Game.InventoryBox.Grid.Grid.FirstOrDefault(x => x?.Item?.Info.Shape == 3);
                //            //if (cell?.UseItem() == true)
                //            //{
                //            _ProtectTime = CEnvir.Now.AddSeconds(BigPatchConfig.TimeAutoRandom);//使用延时的秒数
                //            //}
                //        }
                //    }
                //}

                //if (BigPatchConfig.AndroidMinPHBackCastle)  //血少回城
                //{
                //    float protecthp = 0.30f;
                //    protecthp = BigPatchConfig.AndroidBackCastleMinPHValue / 100f;
                //    if (GameScene.Game.User.CurrentHP < GameScene.Game.User.Stats[Stat.Health] * protecthp)
                //    {
                //        if (CEnvir.Now > _ProtectTime)
                //        {
                //            DXItemCell cell;
                //            cell = GameScene.Game.InventoryBox.Grid.Grid.FirstOrDefault(x => x?.Item?.Info.Shape == 2);
                //            //if (cell?.UseItem() == true)
                //            //{
                //            //BigPatchConfig.AndroidMinPHBackCastle = false;
                //            _ProtectTime = CEnvir.Now.AddSeconds(BigPatchConfig.TimeAutoRandom);//使用延时的秒数
                //        }
                //    }
                //}

                if (BigPatchConfig.ChkConsumeRepair)  //自动修复
                {
                    if (CEnvir.Now > ChkConsumeRepairTime)
                    {
                        DXItemCell cell;
                        cell = GameScene.Game.InventoryBox.Grid.Grid.FirstOrDefault(x => x?.Item?.Info.ItemType == ItemType.Consumable && x?.Item?.Info.Shape == 11);
                        if (cell?.UseItem() == true)
                        {
                            ChkConsumeRepairTime = CEnvir.Now.AddMinutes(BigPatchConfig.ConsumeRepairTime); //使用延时的分钟数
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 发现boss及人物就随机
        /// </summary>
        public void ChkRandom(ClientObjectData ob)
        {
            if (!BigPatchConfig.AndroidPlayer) return;
            if (GameScene.Game.MapControl.MapInfo == null) return;
            if (ob.MapIndex != GameScene.Game.MapControl.MapInfo.Index || ob.Dead || ob.ItemInfo != null) return;

            if (ob.MonsterInfo != null)
            {
                if (!BigPatchConfig.AndroidBossRandom) return;
                if (!ob.MonsterInfo.IsBoss) return;
            }
            else
            {
                if (!BigPatchConfig.AndroidPlayerRandom) return;
                if (MapObject.User.ObjectID == ob.ObjectID) return;
                else if (GameScene.Game.Observer) return;
                else if (GameScene.Game.GroupBox.Members.Any(p => p.ObjectID == ob.ObjectID)) return;
                else if (GameScene.Game.Partner != null && GameScene.Game.Partner.ObjectID == ob.ObjectID) return;
                else if (GameScene.Game.GuildBox.GuildInfo != null && GameScene.Game.GuildBox.GuildInfo.Members.Any(p => p.ObjectID == ob.ObjectID)) return;
            }
            int dis = Functions.Distance(MapControl.User.CurrentLocation, ob.Location);
            if (dis > Globals.MagicRange) return;

            if (CEnvir.Now > ChkRandomTime)
            {
                DXItemCell cell;
                cell = GameScene.Game.InventoryBox.Grid.Grid.FirstOrDefault(x => x?.Item?.Info.ItemType == ItemType.Consumable && x?.Item?.Info.Shape == 3);
                if (cell?.UseItem() == true)
                {
                    ChkRandomTime = CEnvir.Now.AddSeconds(1);
                }
            }
        }

        /// <summary>
        /// 用户角色改变
        /// </summary>
        public void UserChanged()
        {
            Magic?.UpdateMagic();
            Commonly?.UpdateMagic();
            Helper?.UpdateMagic();
            AutoPotionPage?.UserChanged();
        }
        /// <summary>
        /// 接受聊天信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public void ReceiveChat(string message, MessageType type)
        {
            if (type == MessageType.WhisperIn || MessageType.GMWhisperIn == type)
            {
                string str = Answering?.CombAutoReplayText?.SelectedLabel?.Text;
                if (str != null)
                {
                    string[] parts = message.Split('=', '>');
                    if (parts.Length == 0) return;
                    CEnvir.Enqueue(new C.Chat
                    {
                        Text = '/' + parts[0] + ' ' + str,
                    });
                }
            }
        }

        #region 自动捡取
        /// <summary>
        /// 自动捡取道具
        /// </summary>
        /// <returns></returns>
        public bool PickupItems()
        {
            bool rt = false;
            do
            {
                //if (null == AutoPick) break;

                if (GameScene.Game.Observer) break;   //如果是观察者模式 跳出

                if (GameScene.Game.User.Dead) break;   //如果角色死亡 跳出

                if (CEnvir.Now < GameScene.Game.PickUpTime) break;   //小于可捡取时间跳出

                if (GameScene.Game.User.Horse != HorseType.None) break;  //骑马不允许捡取

                GameScene.Game.PickUpTime = CEnvir.Now.AddMilliseconds(250);  //捡取时间延迟

                CEnvir.Enqueue(new C.PickUp());
                rt = true;

                //int range = GameScene.Game.User.Stats[Stat.PickUpRadius];

                //int userPosX = GameScene.Game.User.CurrentLocation.X;
                //int userPosY = GameScene.Game.User.CurrentLocation.Y;

                //for (int d = 0; d <= range; d++)
                //{
                //    for (int y = userPosY - d; y <= userPosY + d; y++)
                //    {
                //        if (y < 0) continue;
                //        if (y >= GameScene.Game.MapControl.Height) break;

                //        for (int x = userPosX - d; x <= userPosX + d; x += Math.Abs(y - userPosY) == d ? 1 : d * 2)
                //        {
                //            if (x < 0) continue;
                //            if (x >= GameScene.Game.MapControl.Width) break;

                //            Cell cell = GameScene.Game.MapControl.Cells[x, y]; //直接访问我们已经检查了界限

                //            if (cell?.Objects == null) continue;

                //            foreach (MapObject cellObject in cell.Objects)
                //            {
                //                if (cellObject.Race != ObjectType.Item) continue;

                //                ItemObject item = (ItemObject)cellObject;

                //                string name = item.Item.Info.ItemName;
                //                int itemIdx = item.Item.Info.Index;
                //                if (item.Item.Info.Effect == ItemEffect.ItemPart)
                //                {
                //                    name = item.Name;
                //                    itemIdx = item.Item.AddedStats[Stat.ItemIndex];
                //                }

                //                if (itemIdx > AutoPick.ItemFilter.Items.Count) continue;

                //                CItemFilterSet iset = AutoPick.ItemFilter.Items[itemIdx - 1];

                //                if (!iset.pick)
                //                {
                //                    CEnvir.Enqueue(new C.PickUp
                //                    {
                //                        ItemIdx = item.Item.Info.Index,
                //                        Xpos = x,
                //                        Ypos = y,
                //                    });
                //                    rt = true;
                //                }
                //            }
                //        }
                //    }
                //}
            } while (false);
            return rt;
        }
        /// <summary>
        /// 获取自动捡取筛选项 通过索引
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public CItemFilterSet GetFilterItem(int idx)
        {
            CItemFilterSet iset = null;
            do
            {
                if (AutoPick == null) break;
                if (AutoPick.ItemFilter == null) break;
                if (AutoPick.ItemFilter.Items == null) break;
                if (AutoPick.ItemFilter.Items.Count == 0) break;
                if (idx < 1) break;
                if (idx > AutoPick.ItemFilter.Items.Count) break;

                iset = AutoPick.ItemFilter.Items[idx];
            } while (false);
            return iset;
        }

        /// <summary>
        /// 获取自动捡取筛选项 通过名字
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CItemFilterSet GetFilterItem(string name)
        {
            CItemFilterSet iset = null;
            do
            {
                if (AutoPick == null) break;
                if (AutoPick.ItemFilter == null) break;
                if (AutoPick.ItemFilter.Items == null) break;
                if (AutoPick.ItemFilter.Items.Count == 0) break;

                iset = AutoPick.ItemFilter.Items.Where(o => o.name == name).FirstOrDefault();
            } while (false);
            return iset;
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            AutoPick?.ItemFilter.Uninitalize();
            MonBoss?.BossFilter.Uninitalize();
            //Answering?.SaveSaywords();//这个时候保存配置太迟了，ConfigReader 已经把内存刷到文件了。
            if (disposing)
            {
                Commonly?.Dispose();
                Helper?.Dispose();
                Answering?.Dispose();
                NoteBook?.Dispose();
                MsgRecord?.Dispose();
                AutoPick?.Dispose();
                ViewRange?.Dispose();
                Magic?.Dispose();
            }
        }
    }
}
