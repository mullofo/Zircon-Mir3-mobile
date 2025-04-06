using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;
using C = Library.Network.ClientPackets;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class AutoPotionDialog : DXWindow                      //辅助
    {
        #region Properties
        public int StartIndex
        {
            get => _StartIndex;
            set
            {
                //if (_StartIndex == value) return;

                int oldValue = _StartIndex;
                _StartIndex = value;

                OnStartIndexChanged(oldValue, value);
            }
        }
        private int _StartIndex;
        public event EventHandler<EventArgs> StartIndexChanged;
        public void OnStartIndexChanged(int oValue, int nValue)
        {

            if (seacherList !=null)
            {
                for (int i = 0; i < Lines.Length; i++)
                {
                    if (i + nValue - 1 < seacherList.Count)
                        Lines[i].Item = seacherList[nValue + i - 1];
                    else Lines[i].Item = null;
                }
                return;
            }
            int item = 0;
            if (ItemTypeBox.SelectedItem is ItemType)
            {
                ItemType idx =(ItemType)ItemTypeBox.SelectedItem;
                item = (int)idx;

            }else
            {
                item = (int)ItemType.Shield + 1;
            }

            for (int i = 0; i < Lines.Length; i++)
            {
                if (i + nValue - 1 < singleFiterList[item].Count)
                    Lines[i].Item = singleFiterList[item][nValue + i - 1];
                else Lines[i].Item = null;
            }

           
            /*
            if (nValue > oValue)
                for (int i = 0; i < Lines.Length; i++)
                {
                    if (nValue - oValue + i < Lines.Length)
                    {
                        if (Lines[i + nValue - oValue].Item != null)
                        {
                            Lines[i].Item = Lines[i + nValue - oValue].Item;
                        }else
                            Lines[i].Item = FilterItems[nValue + i];

                    }
                    else
                    {
                        Lines[i].Item = FilterItems[nValue + i];
                    }


                }
            else
                for (int i = Lines.Length - 1; i >= 0; i--)
                {
                    if (nValue - oValue + i >= 0)
                    {
                        if (Lines[i + nValue - oValue].Item != null)
                            Lines[i].Item = Lines[i + nValue - oValue].Item;
                        else
                            Lines[i].Item = FilterItems[nValue + i];

                    }
                    else
                        Lines[i].Item = FilterItems[nValue + i];
                }

            */

            StartIndexChanged?.Invoke(this, EventArgs.Empty);

        }

        private DXTabControl TabControl;
        private DXTab UsedTab, AutoPotionTab, AutoFightTab, AutoPickTab, AutoHookTab, ChatTab, RecordTab, BossTab, MagicTab;
        public DXTextBox ChatTextBox;
        public DXVScrollBar ChatTextScrollBar;

        //辅助部分
        public DXCheckBox SetRunCheckBox;       //助跑
        public DXCheckBox SetShiftBox;          //Shift        
        public DXCheckBox SetDisplayBox;        //综合数显
        public DXCheckBox SetBrightBox;         //免蜡
        public DXCheckBox SetCorpseBox;         //清理尸体
        public DXCheckBox SetShowHealth;        //显血
        public DXCheckBox SetFlamingSwordBox;   //自动烈火
        public DXCheckBox SetDragobRiseBox;     //自动翔空
        public DXCheckBox SetBladeStormBox;     //自动莲月
        public DXCheckBox SetDefianceBox;       //自动铁布衫
        public DXCheckBox SetMightBox;          //自动破血狂杀
        public DXCheckBox SetMagicShieldBox;    //自动魔法盾
        public DXCheckBox SetRenounceBox;       //自动凝血
        public DXCheckBox SetPoisonDustBox;     //自动换毒
        public DXCheckBox SetCelestialBox;      //自动阴阳盾
        public DXCheckBox SetFourFlowersBox;    //自动四花
        public DXCheckBox SetEvasionBox;        //自动风之闪避
        public DXCheckBox SerRagingWindBox;     //自动风之守护

        public DXComboBox SetMagicskillsComboBox;
        public DXNumberBox TimeBox1, TimeBox2;
        public DXCheckBox SetMagicskillsBox;    //自动技能
        public DXComboBox SetMagicskills1ComboBox;
        public DXCheckBox SetMagicskills1Box;   //自动技能1

        //自动挂机部分
        public DXCheckBox SetAutoOnHookBox;     //启动自动挂机
        public DXLabel AutoTimeLable;
        public DXCheckBox SetPickUpBox;         //自动捡取
        public DXCheckBox SetAutoPoisonBox;     //自动上毒
        public DXCheckBox SetAutoAvoidBox;      //自动躲避
        public DXCheckBox SetDeathResurrectionBox;    //死亡复活

        public DXNumberBox TimeBoxRandom;       //自动随机
        public DXCheckBox ChkAutoRandom;        //自动随机
        public DXCheckBox SetSingleHookSkillsBox;     //单技能挂机
        public DXComboBox SetSingleHookSkillsComboBox;//单技能挂机框

        public DXCheckBox SetGroupHookSkillsBox;      //群体技能挂机
        public DXComboBox SetGroupHookSkillsComboBox; //群体技能挂机框

        public DXCheckBox SetSummoningSkillsBox;      //召唤技能
        public DXComboBox SetSummoningSkillsComboBox; //召唤技能框

        public DXNumberBox TimeBox3, TimeBox4, XBox, YBox, RBox;  //保护值定义
        public DXCheckBox FixedComBox;   //定点挂机框

        public DXCheckBox SetRandomItemBox;     //随机保护
        public DXComboBox SetRandomItemComboBox;//随机设置框

        public DXCheckBox SetHomeItemBox;      //回城保护
        public DXComboBox SetHomeItemComboBox; //回城设置框

        public DXComboBox ItemTypeBox;   //道具分类
        public DXTextBox ItemNameBox;    //搜索框
        public DXButton SearchButton;    //搜索按钮

        public ClientAutoPotionLink[] Links;
        public ClientAutoFightLink[] FightLinks;

        public AutoPotionRow[] Rows;//自动喝药
        public DXVScrollBar ScrollBar;
        public DXVScrollBar PickScrollBar;

        public override WindowType Type => WindowType.AutoPotionBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => true;
        private PickupItem[] Lines;

        public DXLabel label, label1, label2, label3, label4, label5, label6, label7, label8, label9, label10, label11, label12, label13, label14, label15, label16, label17, label18, label19;
        #endregion
        
        public DXCheckBox CreateCheckBox(DXControl parent,string name, int x, int y, EventHandler<EventArgs> Changed)
        {
            DXCheckBox box = new DXCheckBox
            {
                Label = { Text = name },
                Parent = parent,
                Checked = true,
            };
            box.Location = new Point(x - box.Size.Width, y); ;
            box.CheckedChanged += Changed;
            return box;
        }
        public AutoPotionDialog()
        {
            TitleLabel.Text = "大补贴1.0";
            HasFooter = false;  //不显示页脚

            SetClientSize(new Size(550, 428));
            TabControl = new DXTabControl
            {
                Parent = this,
                Location = ClientArea.Location,
                Size = ClientArea.Size,
            };

            UsedTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "常用" } },
            };

            AutoFightTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "辅助" } },
            };

            AutoPotionTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "保护" } },
            };

            ChatTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "聊天" } },
            };

            ChatTextBox = new DXTextBox
            {
                Parent = ChatTab,
                Size = new Size(ClientArea.Size.Width - 10-28,395),
                Location = new Point(5, 5),
            };

            ChatTextScrollBar = new DXVScrollBar
            {
                Parent = ChatTab,
                Size = new Size(14, 395),
                Location = new Point(ClientArea.Right - 28, 6),
                VisibleSize = ClientArea.Height,
                MaxValue = 100,

            };
            
            //ChatTextBox.MouseWheel += ChatTextScrollBar.DoMouseWheel;
            ChatTextBox.TextBox.LocationChanged += (o, e) =>
            {
                ChatTextScrollBar.Location = new Point(ChatTextBox.TextBox.Location.X + ChatTextBox.TextBox.Size.Width, ChatTextBox.TextBox.Location.Y);
            };
            ChatTextBox.TextBox.MouseWheel += ChatTextScrollBar.DoMouseWheel;
            ChatTextBox.TextBox.TextChanged += (o, e) =>
            {
                ChatTextScrollBar.MaxValue = ChatTextBox.TextBox.Lines.Length * ChatTextBox.TextBox.Font.Height;
            };
            RecordTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "记录" } },
            };

            AutoPickTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "物品" } },
            };

            BossTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "怪物" } },
            };

            MagicTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "魔法" } },
            };

            label = new DXLabel
            {
                Text = "┌─常规───",
                Outline = true,
                Parent = AutoFightTab,         //放在不同的框架内
                Location=new Point (0,10)
            };

            label1 = new DXLabel
            {
                Text = "┌─战斗───",
                Outline = true,
                Parent = AutoFightTab,
                Location = new Point(0, 110)
            };

            label2 = new DXLabel
            {
                Text = "┌─自动───",
                Outline = true,
                Parent = AutoFightTab,
                Location = new Point(0, 290)
            };

            SetDisplayBox = CreateCheckBox(AutoFightTab, "综合数显", 270, 80, AutoSetConf.SetFlamingSwordBox);
            //自动烈火
            SetFlamingSwordBox = CreateCheckBox(AutoFightTab, "自动烈火", 80, 140, AutoSetConf.SetFlamingSwordBox);
            //SetFlamingSwordBox = new DXCheckBox
            //{
            //     Label = { Text = "自动烈火" },
            //     Parent = AutoFightTab,
            //     Checked = false,
            //     Visible = false,
            //};
            //SetFlamingSwordBox.Location = new Point(80 - SetFlamingSwordBox.Size.Width, 140);
            //SetFlamingSwordBox.CheckedChanged += (o, e) => 
            //{
            //    if (GameScene.Game.Observer) return;

            //    if (GameScene.Game.AutoPotionBox.Updating) return;
            //    CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetFlamingSwordBox.Checked, Slot = AutoSetConf.SetFlamingSwordBox });
            //};

            //自动翔空
            SetDragobRiseBox = new DXCheckBox
            {
                Label = { Text = "自动翔空" },
                Parent = AutoFightTab,
                Checked = false,
                Visible = false,
            };
                    SetDragobRiseBox.Location = new Point(180 - SetDragobRiseBox.Size.Width, 140);
            SetDragobRiseBox.CheckedChanged += (o, e) => 
            {
                if (GameScene.Game.Observer) return;

                if (GameScene.Game.AutoPotionBox.Updating) return;
                CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetDragobRiseBox.Checked, Slot = AutoSetConf.SetDragobRiseBox });
            };

            //自动莲月
            SetBladeStormBox = new DXCheckBox
            {
                Label = { Text = "自动莲月" },
                Parent = AutoFightTab,
                Checked = false,
                Visible = false,
            };
                    SetBladeStormBox.Location = new Point(270 - SetBladeStormBox.Size.Width, 140);
            SetBladeStormBox.CheckedChanged += (o, e) => 
            {
                if (GameScene.Game.Observer) return;

                if (GameScene.Game.AutoPotionBox.Updating) return;
                CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetBladeStormBox.Checked, Slot = AutoSetConf.SetBladeStormBox });
            };

            //自动铁布衫
            SetDefianceBox = new DXCheckBox
            {
                Label = { Text = "自动铁布衫" },
                Parent = AutoFightTab,
                Checked = false,
                Visible = false,
            };
            SetDefianceBox.Location = new Point(180 - SetDefianceBox.Size.Width, 180);
            SetDefianceBox.CheckedChanged += (o, e) => 
            {
                if (GameScene.Game.Observer) return;

                if (GameScene.Game.AutoPotionBox.Updating) return;
                CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetDefianceBox.Checked, Slot = AutoSetConf.SetDefianceBox });
            };

            //自动破血狂杀
            SetMightBox = new DXCheckBox
            {
                Label = { Text = "自动破血" },
                Parent = AutoFightTab,
                Checked = false,
                Visible = false,
            };
            SetMightBox.Location = new Point(80 - SetMightBox.Size.Width, 180);
            SetMightBox.CheckedChanged += (o, e) => 
            {
                if (GameScene.Game.Observer) return;

                if (GameScene.Game.AutoPotionBox.Updating) return;
                CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetMightBox.Checked, Slot = AutoSetConf.SetMightBox });
            };

            //自动魔法盾
            SetMagicShieldBox = new DXCheckBox
            {
                Label = { Text = "自动魔法盾" },
                Parent = AutoFightTab,
                Checked = false,
                Visible = false,
            };
            SetMagicShieldBox.Location = new Point(180 - SetMagicShieldBox.Size.Width, 140);
            SetMagicShieldBox.CheckedChanged += (o, e) => 
            {
                if (GameScene.Game.Observer) return;

                if (GameScene.Game.AutoPotionBox.Updating) return;
                CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetMagicShieldBox.Checked, Slot = AutoSetConf.SetMagicShieldBox });
            };
            //自动凝血
            SetRenounceBox = new DXCheckBox
            {
                Label = { Text = "自动凝血" },
                Parent = AutoFightTab,
                Checked = false,
                Visible = false,
            };
            SetRenounceBox.Location = new Point(80 - SetRenounceBox.Size.Width, 140);
            SetRenounceBox.CheckedChanged += (o, e) => 
            {
                if (GameScene.Game.Observer) return;

                if (GameScene.Game.AutoPotionBox.Updating) return;
                CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetRenounceBox.Checked, Slot = AutoSetConf.SetRenounceBox });
            };

            //自动换毒
            SetPoisonDustBox = new DXCheckBox
            {
                Label = { Text = "自动换毒" },
                Parent = AutoFightTab,
                Checked = false,
                Visible = false,
            };
            SetPoisonDustBox.Location = new Point(80 - SetPoisonDustBox.Size.Width, 140);
            SetPoisonDustBox.CheckedChanged += (o, e) => 
            {
                if (GameScene.Game.Observer) return;

                if (GameScene.Game.AutoPotionBox.Updating) return;
                CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetPoisonDustBox.Checked, Slot = AutoSetConf.SetPoisonDustBox });
            };
            //自动阴阳盾
            SetCelestialBox = new DXCheckBox
            {
                Label = { Text = "自动阴阳盾" },
                Parent = AutoFightTab,
                Checked = false,
                Visible = false,
            };
            SetCelestialBox.Location = new Point(180 - SetCelestialBox.Size.Width, 140);
            SetCelestialBox.CheckedChanged += (o, e) => 
            {
                if (GameScene.Game.Observer) return;

                if (GameScene.Game.AutoPotionBox.Updating) return;
                CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetCelestialBox.Checked, Slot = AutoSetConf.SetCelestialBox });
            };

            //自动四花
            SetFourFlowersBox = new DXCheckBox
            {
                Label = { Text = "自动四花" },
                Parent = AutoFightTab,
                Checked = false,
                Visible = false,
            };
            SetFourFlowersBox.Location = new Point(80 - SetFourFlowersBox.Size.Width, 140);
            SetFourFlowersBox.CheckedChanged += (o, e) => 
            {
                if (GameScene.Game.Observer) return;

                if (GameScene.Game.AutoPotionBox.Updating) return;
                CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetFourFlowersBox.Checked, Slot = AutoSetConf.SetFourFlowersBox });
            };

            //自动风之闪避
            SetEvasionBox = new DXCheckBox
                    {
                        Label = { Text = "自动闪避" },
                        Parent = AutoFightTab,
                        Checked = false,
                        Visible = false,
                    };
            SetEvasionBox.Location = new Point(180 - SetEvasionBox.Size.Width, 140);
            SetEvasionBox.CheckedChanged += (o, e) => 
            {
                if (GameScene.Game.Observer) return;

                if (GameScene.Game.AutoPotionBox.Updating) return;
                CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetEvasionBox.Checked, Slot = AutoSetConf.SetEvasionBox });
            };

            //自动风之守护
            SerRagingWindBox = new DXCheckBox
                    {
                        Label = { Text = "自动守护" },
                        Parent = AutoFightTab,
                        Checked = false,
                        Visible = false,
                    };
            SerRagingWindBox.Location = new Point(270 - SerRagingWindBox.Size.Width, 140);
            SerRagingWindBox.CheckedChanged += (o, e) => 
            {
                if (GameScene.Game.Observer) return;

                if (GameScene.Game.AutoPotionBox.Updating) return;
                CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SerRagingWindBox.Checked, Slot = AutoSetConf.SerRagingWindBox });
            };

            //免助跑
            SetRunCheckBox = CreateCheckBox(AutoFightTab, "免助跑", 80, 80, AutoSetConf.SetRunCheckBox);
                //SetRunCheckBox = new DXCheckBox
                //{
                //    Label = { Text = "免助跑" },
                //    Parent = AutoFightTab,
                //    Checked = true,
                //    Visible = CEnvir.OnRunCheck,
                //};
                //SetRunCheckBox.Location = new Point(80 - SetRunCheckBox.Size.Width, 80);
                //SetRunCheckBox.CheckedChanged += (o, e) =>
                //{
                //    if (GameScene.Game.Observer) return;

                //    if (GameScene.Game.AutoPotionBox.Updating) return;
                //    CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetRunCheckBox.Checked, Slot = AutoSetConf.SetRunCheckBox });
                //};
            
            //免SHIFT
            SetShiftBox = new DXCheckBox
            {
                Label = { Text = "免Shift" },
                Parent = AutoFightTab,
                Checked = false,
            };
            SetShiftBox.Location = new Point(80 - SetShiftBox.Size.Width, 40);
            SetShiftBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                if (GameScene.Game.AutoPotionBox.Updating) return;
                CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetShiftBox.Checked, Slot = AutoSetConf.SetShiftBox });
            };
            //显血
            SetShowHealth = new DXCheckBox
            {
                Label = { Text = "数字显血" },
                Parent = AutoFightTab,
                Checked = true,
            };
            SetShowHealth.Location = new Point(180 - SetShowHealth.Size.Width, 40);
            SetShowHealth.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                if (GameScene.Game.AutoPotionBox.Updating) return;
                CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetShowHealth.Checked, Slot = AutoSetConf.SetShowHealth });
            };
            //免蜡
                SetBrightBox = new DXCheckBox
                {
                    Label = { Text = "免蜡烛" },
                    Parent = AutoFightTab,
                    Checked = true,
                    Visible = CEnvir.OnBrightBox,
                };
                SetBrightBox.Location = new Point(180 - SetBrightBox.Size.Width, 80);
                SetBrightBox.CheckedChanged += (o, e) =>
                {
                    if (GameScene.Game.Observer) return;

                    if (GameScene.Game.AutoPotionBox.Updating) return;
                    CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetBrightBox.Checked, Slot = AutoSetConf.SetBrightBox });
                    GameScene.Game?.MapControl?.LLayer.UpdateLights();
                };
            //清理尸体
            SetCorpseBox = new DXCheckBox
            {
                Label = { Text = "清理尸体" },
                Parent = AutoFightTab,
                Checked = false,
            };
            SetCorpseBox.Location = new Point(270 - SetCorpseBox.Size.Width, 40);
            SetCorpseBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                if (GameScene.Game.AutoPotionBox.Updating) return;
                CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetCorpseBox.Checked, Slot = AutoSetConf.SetCorpseBox });
            };
            //自动技能
            SetMagicskillsComboBox = new DXComboBox
            {
                Parent = AutoFightTab,
                Location = new Point(10, 320),
                Size = new Size(100, DXComboBox.DefaultNormalHeight),
            };
            SetMagicskillsComboBox.SelectedItemChanged += (o, e) =>
            {
                    if (GameScene.Game.Observer) return;

                    if (GameScene.Game.AutoPotionBox.Updating) return;
                    if (SetMagicskillsComboBox.SelectedItem is MagicType)
                    {
                        MagicType magicindex = (MagicType)SetMagicskillsComboBox.SelectedItem;
                        CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetMagicskillsBox.Checked, Slot = AutoSetConf.SetMagicskillsBox, MagicIndex = magicindex,TimeCount = (int)TimeBox1.Value });
                    } 
            };
            TimeBox1 = new DXNumberBox
            {
                Parent = AutoFightTab,
                Location = new Point(120, 320),
                Size = new Size(80, 20),
                ValueTextBox = { Size = new Size(40, 18) },
                MaxValue = 50000,
                MinValue = 0,
                UpButton = { Location = new Point(63, 1) }
            };
            TimeBox1.ValueTextBox.ValueChanged += (o, e) => 
            {
                    if (GameScene.Game.Observer) return;

                    if (GameScene.Game.AutoPotionBox.Updating) return;
                    if (SetMagicskillsComboBox.SelectedItem is MagicType)
                    {
                        MagicType magicindex = (MagicType)SetMagicskillsComboBox.SelectedItem;
                        CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetMagicskillsBox.Checked, Slot = AutoSetConf.SetMagicskillsBox, MagicIndex = magicindex, TimeCount = (int)TimeBox1.Value });
                    }              
            };
        
            SetMagicskillsBox = new DXCheckBox
            {
                Label = { Text = "自动技能" },
                Parent = AutoFightTab,
                Checked = false,
            };
            SetMagicskillsBox.Location = new Point(270 - SetMagicskillsBox.Size.Width, 320);
            SetMagicskillsBox.CheckedChanged +=(o,e) =>
            {
                if (GameScene.Game.Observer) return;

                if (GameScene.Game.AutoPotionBox.Updating) return;
                if (SetMagicskillsComboBox.SelectedItem is MagicType)
                {
                    MagicType magicindex = (MagicType)SetMagicskillsComboBox.SelectedItem;
                    CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetMagicskillsBox.Checked, Slot = AutoSetConf.SetMagicskillsBox, MagicIndex = magicindex, TimeCount = (int)TimeBox1.Value });
                }
            };

            //自动技能1
            SetMagicskills1ComboBox = new DXComboBox
            {
                Parent = AutoFightTab,
                Location = new Point(10, 360),
                Size = new Size(100, DXComboBox.DefaultNormalHeight),
            };
            SetMagicskills1ComboBox.SelectedItemChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                if (GameScene.Game.AutoPotionBox.Updating) return;
                if (SetMagicskills1ComboBox.SelectedItem is MagicType)
                {
                    MagicType magicindex = (MagicType)SetMagicskills1ComboBox.SelectedItem;
                    CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetMagicskills1Box.Checked, Slot = AutoSetConf.SetMagicskills1Box, MagicIndex = magicindex, TimeCount = (int)TimeBox2.Value });
                }
            };
            TimeBox2 = new DXNumberBox
            {
                Parent = AutoFightTab,
                Location = new Point(120, 360),
                Size = new Size(80, 20),
                ValueTextBox = { Size = new Size(40, 18) },
                MaxValue = 50000,
                MinValue = 0,
                UpButton = { Location = new Point(63, 1) }
            };
            TimeBox2.ValueTextBox.ValueChanged += (o, e) => 
            {
                if (GameScene.Game.Observer) return;

                if (GameScene.Game.AutoPotionBox.Updating) return;
                if (SetMagicskills1ComboBox.SelectedItem is MagicType)
                {
                    MagicType magicindex = (MagicType)SetMagicskills1ComboBox.SelectedItem;
                    CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetMagicskills1Box.Checked, Slot = AutoSetConf.SetMagicskills1Box, MagicIndex = magicindex, TimeCount = (int)TimeBox2.Value });
                }
            };

            SetMagicskills1Box = new DXCheckBox
            {
                Label = { Text = "自动技能" },
                Parent = AutoFightTab,
                Checked = false,
            };
            SetMagicskills1Box.Location = new Point(270 - SetMagicskills1Box.Size.Width, 360);
            SetMagicskills1Box.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                if (GameScene.Game.AutoPotionBox.Updating) return;
                if (SetMagicskills1ComboBox.SelectedItem is MagicType)
                {
                    MagicType magicindex = (MagicType)SetMagicskills1ComboBox.SelectedItem;
                    CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetMagicskills1Box.Checked, Slot = AutoSetConf.SetMagicskills1Box, MagicIndex = magicindex, TimeCount = (int)TimeBox2.Value });
                }
            };

            Links = new ClientAutoPotionLink[Globals.MaxAutoPotionCount];
            Rows = new AutoPotionRow[Globals.MaxAutoPotionCount];


            ScrollBar = new DXVScrollBar
            {
                Parent = AutoPotionTab,
                Size = new Size(14,395),
                Location = new Point(ClientArea.Right - 28, 6),
                VisibleSize = ClientArea.Height,
                MaxValue = Rows.Length * 50 - 2

            };
            DXControl panel = new DXControl
            {
                Parent = AutoPotionTab,
                Size = new Size(ClientArea.Size.Width -10, ClientArea.Size.Height),
                Location = new Point(5, 5),
            };
            panel.MouseWheel += ScrollBar.DoMouseWheel;

            for (int i = 0; i < Links.Length; i++)
            {
                AutoPotionRow row;
                Rows[i] = row = new AutoPotionRow
                {
                    Parent = panel,
                    Location = new Point(1, 1 + 50*i),
                    Index = i,
                };
                row.MouseWheel += ScrollBar.DoMouseWheel;
            }
            ScrollBar.ValueChanged += (o, e) => UpdateLocations();

            //自动挂机部分
            AutoHookTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "挂机" } },         
            };
            if (!CEnvir.OnAutoHookTab)             //判断服务端设置 未勾选，那么隐藏自动挂机面板
            {
                AutoHookTab.Parent = null;
            }

                label3 = new DXLabel
                {
                    Text = "┌─启动───",
                    Outline = true,
                    Parent = AutoHookTab,         //放在不同的框架内
                    Location = new Point(0, 10)
                };

                label4 = new DXLabel
                {
                    Text = "┌─功能───",
                    Outline = true,
                    Parent = AutoHookTab,
                    Location = new Point(0, 70)
                };

                label5 = new DXLabel
                {
                    Text = "┌─技能───",
                    Outline = true,
                    Parent = AutoHookTab,
                    Location = new Point(0, 170)
                };

                label6 = new DXLabel
                {
                    Text = "┌─保护───",
                    Outline = true,
                    Parent = AutoHookTab,
                    Location = new Point(0, 310)
                };

                label7 = new DXLabel
                {
                    Text = "┌─区域───",
                    Outline = true,
                    Parent = AutoHookTab,
                    Location = new Point(0, 230)
                };

                //自动挂机

                label8 = new DXLabel
                {
                    Text = "剩余时间:",
                    Outline = true,
                    Parent = AutoHookTab,
                    Location = new Point(30, 40)
                };
                AutoTimeLable = new DXLabel
                {
                    Text = "",
                    Outline = true,
                    Parent = AutoHookTab,
                    Location = new Point(100, 40)
                };
                SetAutoOnHookBox = new DXCheckBox
                {
                    Label = { Text = "自动挂机" },
                    Parent = AutoHookTab,
                    Checked = false,
                };
                SetAutoOnHookBox.Location = new Point(250 - SetAutoOnHookBox.Size.Width, 40);
                SetAutoOnHookBox.CheckedChanged += (o, e) =>
                {
                    if (GameScene.Game.Observer) return;

                    if (GameScene.Game.AutoPotionBox.Updating) return;
                    if (SetAutoOnHookBox.Checked == true)
                    {
                        if (GameScene.Game.User.AutoTime == 0)
                        {
                            SetAutoOnHookBox.Checked = false;
                            return;
                        }
                    }
                    CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetAutoOnHookBox.Checked, Slot = AutoSetConf.SetAutoOnHookBox });
                };

                //自动捡取
                SetPickUpBox = new DXCheckBox
                {
                    Label = { Text = "自动捡取" },
                    Parent = AutoHookTab,
                    Checked = false,
                };
                SetPickUpBox.Location = new Point(100 - SetPickUpBox.Size.Width, 100);
                SetPickUpBox.CheckedChanged += (o, e) =>
                {
                    if (GameScene.Game.Observer) return;

                    if (GameScene.Game.AutoPotionBox.Updating) return;
                    CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetPickUpBox.Checked, Slot = AutoSetConf.SetPickUpBox });
                };

                //自动上毒
                SetAutoPoisonBox = new DXCheckBox
                {
                    Label = { Text = "自动上毒" },
                    Parent = AutoHookTab,
                    Checked = false,
                };
                SetAutoPoisonBox.Location = new Point(250 - SetAutoPoisonBox.Size.Width, 100);
                SetAutoPoisonBox.CheckedChanged += (o, e) =>
                {
                    if (GameScene.Game.Observer) return;

                    if (GameScene.Game.AutoPotionBox.Updating) return;
                    CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetAutoPoisonBox.Checked, Slot = AutoSetConf.SetAutoPoisonBox });
                };

                //自动躲避
                SetAutoAvoidBox = new DXCheckBox
                {
                    Label = { Text = "自动躲避" },
                    Parent = AutoHookTab,
                    Checked = false,
                };
                SetAutoAvoidBox.Location = new Point(100 - SetAutoAvoidBox.Size.Width, 125);
                SetAutoAvoidBox.CheckedChanged += (o, e) =>
                {
                    if (GameScene.Game.Observer) return;

                    if (GameScene.Game.AutoPotionBox.Updating) return;
                    CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetAutoAvoidBox.Checked, Slot = AutoSetConf.SetAutoAvoidBox });
                };

                //死亡回城
                SetDeathResurrectionBox = new DXCheckBox
                {
                    Label = { Text = "死亡回城" },
                    Parent = AutoHookTab,
                    Checked = false,
                };
                SetDeathResurrectionBox.Location = new Point(250 - SetDeathResurrectionBox.Size.Width, 125);
                SetDeathResurrectionBox.CheckedChanged += (o, e) =>
                {
                    if (GameScene.Game.Observer) return;

                    if (GameScene.Game.AutoPotionBox.Updating) return;
                    CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetDeathResurrectionBox.Checked, Slot = AutoSetConf.SetDeathResurrectionBox });
                };

                ChkAutoRandom = new DXCheckBox
                {
                    Parent = AutoHookTab,
                    Checked = Config.ChkAutoRandom,
                    Label = { Text = "自动随机" },
                    
                };
                ChkAutoRandom.Location = new Point(100 - ChkAutoRandom.Size.Width, 150);
                ChkAutoRandom.CheckedChanged += (o, e) =>
                {
                    Config.ChkAutoRandom = ChkAutoRandom.Checked;
                };

                DXLabel unnamed = new DXLabel
                {
                    Parent = AutoHookTab,
                    Text = "间隔(秒)",
                };
                unnamed.Location = new Point(ChkAutoRandom.Location.X + ChkAutoRandom.Size.Width + 5, 150);
                
                TimeBoxRandom = new DXNumberBox
                {
                    Parent = AutoHookTab,
                    MaxValue = 10000,
                    MinValue = 10,
                    Value = Config.TimeAutoRandom,
                    Size = new Size(100, DXNumberBox.DefaultHeight),
                };
                TimeBoxRandom.Location = new Point(unnamed.Location.X + unnamed.Size.Width + 5, 150);

                TimeBoxRandom.ValueTextBox.ValueChanged += (o, e) =>
                {
                    Config.TimeAutoRandom = TimeBoxRandom.Value;
                };

                //单技能挂机
                SetSingleHookSkillsBox = new DXCheckBox
                {
                    Label = { Text = "单体技能" },
                    Parent = AutoHookTab,
                    Checked = false,
                };
                SetSingleHookSkillsBox.Location = new Point(100 - SetSingleHookSkillsBox.Size.Width, 200);
                SetSingleHookSkillsBox.CheckedChanged += (o, e) =>
                {
                    if (GameScene.Game.Observer) return;

                    if (GameScene.Game.AutoPotionBox.Updating) return;
                    if (SetSingleHookSkillsComboBox.SelectedItem is MagicType)
                    {
                        MagicType magicindex = (MagicType)SetSingleHookSkillsComboBox.SelectedItem;
                        CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetSingleHookSkillsBox.Checked, Slot = AutoSetConf.SetSingleHookSkillsBox, MagicIndex = magicindex });
                    }
                };

                //单技能挂机框
                SetSingleHookSkillsComboBox = new DXComboBox
                {
                    Parent = AutoHookTab,
                    Location = new Point(125, 200),
                    Size = new Size(100, DXComboBox.DefaultNormalHeight),
                };
                SetSingleHookSkillsComboBox.SelectedItemChanged += (o, e) =>
                {
                    if (GameScene.Game.Observer) return;

                    if (GameScene.Game.AutoPotionBox.Updating) return;
                    if (SetSingleHookSkillsComboBox.SelectedItem is MagicType)
                    {
                        MagicType magicindex = (MagicType)SetSingleHookSkillsComboBox.SelectedItem;
                        CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetSingleHookSkillsBox.Checked, Slot = AutoSetConf.SetSingleHookSkillsBox, MagicIndex = magicindex });
                    }
                };

                /*
                //群体技能挂机
                SetGroupHookSkillsBox = new DXCheckBox
                {
                    Label = { Text = "群体技能" },
                    Parent = AutoHookTab,
                    Checked = false,
                };
                SetGroupHookSkillsBox.Location = new Point(100 - SetGroupHookSkillsBox.Size.Width, 250);
                SetGroupHookSkillsBox.CheckedChanged += (o, e) =>
                {
                    if (GameScene.Game.Observer) return;

                    if (GameScene.Game.AutoPotionBox.Updating) return;
                    CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetGroupHookSkillsBox.Checked, Slot = AutoSetConf.SetGroupHookSkillsBox });
                };
                //群体技能挂机框
                SetGroupHookSkillsComboBox = new DXComboBox
                {
                    Parent = AutoHookTab,
                    Location = new Point(125, 250),
                    Size = new Size(100, DXComboBox.DefaultNormalHeight),
                };
                SetGroupHookSkillsComboBox.SelectedItemChanged += (o, e) =>
                {
                    if (GameScene.Game.Observer) return;

                    if (GameScene.Game.AutoPotionBox.Updating) return;
                    if (SetGroupHookSkillsComboBox.SelectedItem is MagicType)
                    {
                        MagicType magicindex = (MagicType)SetGroupHookSkillsComboBox.SelectedItem;
                        CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetGroupHookSkillsBox.Checked, Slot = AutoSetConf.SetGroupHookSkillsBox, MagicIndex = magicindex });
                    }
                };

                //召唤技能
                SetSummoningSkillsBox = new DXCheckBox
                {
                    Label = { Text = "召唤技能" },
                    Parent = AutoHookTab,
                    Checked = false,
                };
                SetSummoningSkillsBox.Location = new Point(100 - SetSummoningSkillsBox.Size.Width, 280);
                SetSummoningSkillsBox.CheckedChanged += (o, e) =>
                {
                    if (GameScene.Game.Observer) return;

                    if (GameScene.Game.AutoPotionBox.Updating) return;
                    CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetSummoningSkillsBox.Checked, Slot = AutoSetConf.SetSummoningSkillsBox });
                };
                //召唤技能框
                SetSummoningSkillsComboBox = new DXComboBox
                {
                    Parent = AutoHookTab,
                    Location = new Point(125, 280),
                    Size = new Size(100, DXComboBox.DefaultNormalHeight),
                };
                SetSummoningSkillsComboBox.SelectedItemChanged += (o, e) =>
                {
                    if (GameScene.Game.Observer) return;

                    if (GameScene.Game.AutoPotionBox.Updating) return;
                    if (SetSummoningSkillsComboBox.SelectedItem is MagicType)
                    {
                        MagicType magicindex = (MagicType)SetSummoningSkillsComboBox.SelectedItem;
                        CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetSummoningSkillsBox.Checked, Slot = AutoSetConf.SetSummoningSkillsBox, MagicIndex = magicindex });
                    }
                };   */
                //定点坐标挂机
                label9 = new DXLabel
                {
                    Text = "X坐标:",
                    Parent = AutoHookTab,
                    Location = new Point(25, 260)
                };

                XBox = new DXNumberBox
                {
                    Parent = AutoHookTab,
                    Location = new Point(65, 260),
                    Size = new Size(80, 20),
                    ValueTextBox = { Size = new Size(40, 18) },
                    MaxValue = 10000,
                    MinValue = 0,
                    UpButton = { Location = new Point(63, 1) }
                };

                label10 = new DXLabel
                {
                    Text = "Y坐标:",
                    Parent = AutoHookTab,
                    Location = new Point(150, 260)
                };

                YBox = new DXNumberBox
                {
                    Parent = AutoHookTab,
                    Location = new Point(190, 260),
                    Size = new Size(80, 20),
                    ValueTextBox = { Size = new Size(40, 18) },
                    MaxValue = 10000,
                    MinValue = 0,
                    UpButton = { Location = new Point(63, 1) }
                };

                label11 = new DXLabel
                {
                    Text = "范围:",
                    Parent = AutoHookTab,
                    Location = new Point(28, 285)
                };

                RBox = new DXNumberBox
                {
                    Parent = AutoHookTab,
                    Location = new Point(65, 285),
                    Size = new Size(80, 20),
                    ValueTextBox = { Size = new Size(40, 18) },
                    MaxValue = 100,
                    MinValue = 0,
                    UpButton = { Location = new Point(63, 1) }
                };

                FixedComBox = new DXCheckBox
                {
                    Label = { Text = "定点挂机" },
                    Parent = AutoHookTab,
                    Checked = false,
                };
                FixedComBox.Location = new Point(270 - FixedComBox.Size.Width, 285);


                //随机保护
                label12 = new DXLabel
                {
                    Text = "血值低于",
                    Outline = true,
                    Parent = AutoHookTab,
                    Location = new Point(10, 340)
                };

                TimeBox3 = new DXNumberBox
                {
                    Parent = AutoHookTab,
                    Location = new Point(65, 340),
                    Size = new Size(80, 20),
                    ValueTextBox = { Size = new Size(40, 18) },
                    MaxValue = 100,
                    MinValue = 0,
                    UpButton = { Location = new Point(63, 1) }
                };
                TimeBox3.ValueTextBox.ValueChanged += (o, e) =>
                {

                    if (GameScene.Game.Observer) return;

                    if (GameScene.Game.AutoPotionBox.Updating) return;
                    if (TimeBox3.ValueTextBox.Value > 0)
                    {

                        CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetRandomItemBox.Checked, Slot = AutoSetConf.SetRandomItemBox, TimeCount = (int)TimeBox3.Value });
                    }

                };
                label13 = new DXLabel
                {
                    Text = "%",
                    Outline = true,
                    Parent = AutoHookTab,
                    Location = new Point(140, 340)
                };
                SetRandomItemBox = new DXCheckBox
                {
                    Label = { Text = "随机传送卷保护" },
                    Parent = AutoHookTab,
                    Checked = false,
                };
                SetRandomItemBox.Location = new Point(270 - SetRandomItemBox.Size.Width, 340);
                SetRandomItemBox.CheckedChanged += (o, e) =>
                {
                    if (GameScene.Game.Observer) return;

                    if (GameScene.Game.AutoPotionBox.Updating) return;
                    if (TimeBox3.ValueTextBox.Value > 0)
                    {

                        CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetRandomItemBox.Checked, Slot = AutoSetConf.SetRandomItemBox, TimeCount = (int)TimeBox3.Value });
                    }

                };
                /*SetRandomItemComboBox = new DXComboBox
                {
                    Parent = AutoHookTab,
                    Location = new Point(150, 340),
                    Size = new Size(70, DXComboBox.DefaultNormalHeight),
                };
                SetRandomItemComboBox.SelectedItemChanged += (o, e) =>
                {

                    if (GameScene.Game.Observer) return;

                    if (GameScene.Game.AutoPotionBox.Updating) return;

                };*/

                //回城保护
                label14 = new DXLabel
                {
                    Text = "血值低于",
                    Outline = true,
                    Parent = AutoHookTab,
                    Location = new Point(10, 370)
                };

                TimeBox4 = new DXNumberBox
                {
                    Parent = AutoHookTab,
                    Location = new Point(65, 370),
                    Size = new Size(80, 20),
                    ValueTextBox = { Size = new Size(40, 18) },
                    MaxValue = 100,
                    MinValue = 0,
                    UpButton = { Location = new Point(63, 1) }
                };
                TimeBox4.ValueTextBox.ValueChanged += (o, e) =>
                {

                    if (GameScene.Game.Observer) return;

                    if (GameScene.Game.AutoPotionBox.Updating) return;
                    if (TimeBox4.ValueTextBox.Value > 0)
                    {

                        CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetHomeItemBox.Checked, Slot = AutoSetConf.SetHomeItemBox, TimeCount = (int)TimeBox4.Value });
                    }
                };
                label15 = new DXLabel
                {
                    Text = "%",
                    Outline = true,
                    Parent = AutoHookTab,
                    Location = new Point(140, 370)
                };
                SetHomeItemBox = new DXCheckBox
                {
                    Label = { Text = "回城卷保护" },
                    Parent = AutoHookTab,
                    Checked = false,
                };
                SetHomeItemBox.Location = new Point(270 - SetHomeItemBox.Size.Width, 370);
                SetHomeItemBox.CheckedChanged += (o, e) =>
                {
                    if (GameScene.Game.Observer) return;

                    if (GameScene.Game.AutoPotionBox.Updating) return;
                    if (TimeBox4.ValueTextBox.Value > 0)
                    {

                        CEnvir.Enqueue(new C.AutoFightConfChanged { Enabled = SetHomeItemBox.Checked, Slot = AutoSetConf.SetHomeItemBox, TimeCount = (int)TimeBox4.Value });
                    }

                };
            
            /*SetHomeItemComboBox = new DXComboBox
            {
                Parent = AutoHookTab,
                Location = new Point(150, 370),
                Size = new Size(70, DXComboBox.DefaultNormalHeight),
            };
            SetHomeItemComboBox.SelectedItemChanged += (o, e) =>
            {

                if (GameScene.Game.Observer) return;

                if (GameScene.Game.AutoPotionBox.Updating) return;

            };*/

            //物品过滤部分

            label16 = new DXLabel
            {
                Text = "物品名称",
                Outline = true,
                Parent = AutoPickTab,
                Location = new Point(15, 10)
            };

            label17 = new DXLabel
            {
                Text = "极品",
                Outline = true,
                Parent = AutoPickTab,
                Location = new Point(120, 10)
            };

            label18 = new DXLabel
            {
                Text = "捡取",
                Outline = true,
                Parent = AutoPickTab,
                Location = new Point(170, 10)
            };

            label19 = new DXLabel
            {
                Text = "显示",
                Outline = true,
                Parent = AutoPickTab,
                Location = new Point(220, 10)
            };
            Lines = new PickupItem[17];
            //上下滚动条
            PickScrollBar = new DXVScrollBar
            {
                Parent = AutoPickTab,
                Size = new Size(14, 330),
                Location = new Point(ClientArea.Right - 28, 35),
                VisibleSize = Lines.Length,                 //可见尺寸 = 线.长度
                MaxValue = Rows.Length * 50 - 2             //最大值 = 行.长度 * 50 - 2
            };
            PickScrollBar.ValueChanged += (o, e) => StartIndex = PickScrollBar.Value+1;
            MouseWheel += PickScrollBar.DoMouseWheel;       //鼠标滚轮+=选择滚动条.执行鼠标滚轮
            //物品过滤分类
            ItemTypeBox = new DXComboBox
            {
                Parent = AutoPickTab,
                Location = new Point(10, 380),
                Size = new Size(75, DXComboBox.DefaultNormalHeight),
                DropDownHeight = 198
            };
            ItemTypeBox.SelectedItemChanged += (o, e) =>
            {
                if (seacherList != null) { seacherList.Clear(); seacherList = null; }
                if(ItemTypeBox.SelectedItem is ItemType)
                {
                    ItemType idx = (ItemType)ItemTypeBox.SelectedItem;
                    PickScrollBar.MaxValue = singleFiterList[(int)idx].Count;
                }else
                {

                    PickScrollBar.MaxValue = singleFiterList[(int)ItemType.Shield+1].Count;
                }
                if (PickScrollBar.Value != 0 )
                    PickScrollBar.Value = 0;
                else
                    StartIndex = 1;
            };
            new DXListBoxItem
            {
                Parent = ItemTypeBox.ListBox,
                Label = { Text = $"全部" },
                Item = null
            };

            Type itemType = typeof(ItemType);

            for (ItemType i = ItemType.Nothing; i <= ItemType.Shield; i++)
            {
                MemberInfo[] infos = itemType.GetMember(i.ToString());

                DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();

                new DXListBoxItem
                {
                    Parent = ItemTypeBox.ListBox,
                    Label = { Text = description?.Description ?? i.ToString() },
                    Item = i
                };
            }

            ItemTypeBox.ListBox.SelectItem(null);

            //搜索框
            ItemNameBox = new DXTextBox
            {
                Parent = AutoPickTab,
                Size = new Size(120, 20),
                Location = new Point(100, 380),
            };

            //搜索按钮
            SearchButton = new DXButton
            {
                Size = new Size(50, SmallButtonHeight),
                Location = new Point(230, 380),
                Parent = AutoPickTab,
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "搜索" }
            };
            SearchButton.MouseClick += (o, e) => 
            {
                string searchitemname = ItemNameBox.TextBox.Text;

                seacherList = singleFiterList[(int)ItemType.Shield + 1].FindAll(x => x.name.Contains(searchitemname));
                PickScrollBar.MaxValue = seacherList.Count;
                if (PickScrollBar.Value != 0)
                    PickScrollBar.Value = 0;
                else
                    StartIndex = 1;

            };
            //InitPickupFilter();

        }

        #region Methods

        public FilterItem[] FilterItems;


        public List<FilterItem>[] singleFiterList ;
        public List<FilterItem> seacherList;

        private Dictionary<int, string> CompanionMemory = new Dictionary<int, string>();
        private string UpdateAmount = "";
        // 循环2次. 超过5w个可以用map替换数组
        public FilterItem GetFilterItem(string itemName, int itemIdx)
        {
            if (itemName.Length < 1)
                return null;


            // 精准查询
            foreach (var item in FilterItems)
            {
                if (item == null)
                    continue;

                // 非模糊拾取
                if (item.canPickup != 2)
                {
                    //if (item.name.Equals(itemName))
                    if(itemName.StartsWith(item.name))
                    {
                        // 拾取
                        if (item.canPickup != 0)
                        {
                            if (!CompanionMemory.ContainsKey(itemIdx))
                            {
                                CompanionMemory.Add(itemIdx, itemName);
                                UpdateAmount = string.Concat(UpdateAmount, itemIdx, ",1;");
                            }
                        }
                        else
                        {
                            if (CompanionMemory.ContainsKey(itemIdx))
                            {
                                CompanionMemory.Remove(itemIdx);
                                UpdateAmount = string.Concat(UpdateAmount, itemIdx, ",0;");
                            }
                        }

                        return item;
                    }

                }
            }
/*
           // 模糊查询
            foreach (var item in FilterItems)
            {
                if (item == null)
                    continue;

                // 模糊拾取
                if (item.canPickup == 2)
                {
                    if (itemName.Contains(item.name))
                    {
                        if (!CompanionMemory.ContainsKey(itemIdx))
                        {
                            CompanionMemory.Add(itemIdx, itemName);
                            UpdateAmount = string.Concat(UpdateAmount, itemIdx, ",1;");
                        }
                        return item;
                    }
                }
            }
          */  
            return null;
        }

        string filterName = "./PickupFilter.ini";

        private void InitPickupFilter()  //初始化捡取过滤
        {
            //Lines = new PickupItem[17];

            for (int i = 0; i < Lines.Length; i++)
            {
                Lines[i] = new PickupItem()
                {
                    Parent = AutoPickTab,
                    Location = new Point(10, 30 + i * 20),
                };
                Lines[i].MouseClick += (o, e) => 
                {
                    SelectedFilterItem((PickupItem)o);
                };

                Lines[i].MouseWheel += PickScrollBar.DoMouseWheel;
            }

            if (!File.Exists(filterName))
            {
                File.Create(filterName);
            }

            string[] lines = File.ReadAllLines(filterName);
            FilterItems = new FilterItem[lines.Length+1];
            singleFiterList = new List<FilterItem>[(int)ItemType.Shield+2];
            for(var i = 0;i<(int)ItemType.Shield+2;i++)
            {
                singleFiterList[i] = new List<FilterItem>();
            }
            int idx = 1;
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)|| line[0] == ';') continue;
                string[] item = line.Split(',');
                if (item.Length >= 4)
                {
                    ItemInfo info = Globals.ItemInfoList.Binding.FirstOrDefault(x => x.ItemName == item[0].Trim());
                    if (info == null) continue;
                    FilterItems[idx] = new FilterItem();
                    FilterItems[idx].name = item[0].Trim();

                    short log = 0;
                    if (short.TryParse(item[1].Trim(), out log))
                        FilterItems[idx].canlog = log;
                    short pick = 0;
                    if (short.TryParse(item[2].Trim(), out pick))
                        FilterItems[idx].canPickup = pick;
                    short highLight = 0;
                    if (short.TryParse(item[3].Trim(), out highLight))
                        FilterItems[idx].highLight = highLight;
                    FilterItems[idx].itemIndex = idx;
                    singleFiterList[(int)info.ItemType].Add(FilterItems[idx]);
                    singleFiterList[(int)ItemType.Shield + 1].Add(FilterItems[idx]);
                    idx++;
                }
            }
            PickScrollBar.MaxValue = singleFiterList[(int)ItemType.Shield + 1].Count;
            StartIndex = 1;
            //RefereshFilterItem();
        }

        private void SelectedFilterItem(PickupItem item)
        {
            for (int i = 0; i < Lines.Length; i++)
            {
                Lines[i].Selected = false;
            }
            item.Selected = true;
        }
        private DateTime _protecttime;
        private DateTime _RandomeTime;
        // tick
        public void UpdateAutoAssist()
        {
            if (GameScene.Game.Observer)
            {
                return;
            }
            if (SetCelestialBox.Checked)           //自动阴阳盾
            {
                if  (!GameScene.Game.User.Buffs.Any(x => x.Type == BuffType.CelestialLight))
                        GameScene.Game.UseMagic(MagicType.CelestialLight);
            }

            if (SetDefianceBox.Checked)   //自动铁布衫
            {
                if (!GameScene.Game.User.Buffs.Any(x => x.Type == BuffType.Defiance))
                    GameScene.Game.UseMagic(MagicType.Defiance);
            }

            if (SetMightBox.Checked)   //自动破血狂杀
            {
                if (!GameScene.Game.User.Buffs.Any(x => x.Type == BuffType.Might))
                    GameScene.Game.UseMagic(MagicType.Might);
            }

            if (SetMagicShieldBox.Checked)  //自动魔法盾
            {
                if (!GameScene.Game.User.Buffs.Any(x => x.Type == BuffType.MagicShield))
                    GameScene.Game.UseMagic(MagicType.MagicShield);
            }

            if (SetRenounceBox.Checked)  //自动凝血
            {
                if (!GameScene.Game.User.Buffs.Any(x => x.Type == BuffType.Renounce))
                    GameScene.Game.UseMagic(MagicType.Renounce);
            }

            if (SetEvasionBox.Checked)  //自动风之闪避
            {
                if (!GameScene.Game.User.Buffs.Any(x => x.Type == BuffType.Evasion))
                    GameScene.Game.UseMagic(MagicType.Evasion);
            }

            if (SerRagingWindBox.Checked)  //自动风之守护
            {
                if (!GameScene.Game.User.Buffs.Any(x => x.Type == BuffType.RagingWind))
                    GameScene.Game.UseMagic(MagicType.RagingWind);
            }

            if (SetAutoOnHookBox.Checked)
            {
                if (GameScene.Game.User.Dead)
                    if (SetDeathResurrectionBox.Checked)
                    {
                        SetAutoOnHookBox.Checked = false;
                        CEnvir.Enqueue(new C.TownRevive());
                    }
                if (SetPickUpBox.Checked)
                    AutoPickup();

                if(Config.ChkAutoRandom)
                {
                    if(CEnvir.Now > _RandomeTime)
                    {
                        DXItemCell cell;
                        cell = GameScene.Game.InventoryBox.Grid.Grid.FirstOrDefault(x => x?.Item?.Info.ItemName == "随机传送卷");
                        if (cell?.UseItem() == true)
                        {
                            _RandomeTime = CEnvir.Now.AddSeconds(Config.TimeAutoRandom);//随机使用延时的秒数
                        }
                    }
                }

                if (SetRandomItemBox.Checked)
                {
                    float protecthp=0.30f;
                    protecthp = TimeBox3.Value / 100f;
                    if (GameScene.Game.User.CurrentHP < GameScene.Game.User.Stats[Stat.Health] * protecthp)
                    {
                        if (CEnvir.Now > _protecttime)
                        {
                            DXItemCell cell;
                            cell = GameScene.Game.InventoryBox.Grid.Grid.FirstOrDefault(x => x?.Item?.Info.ItemName == "随机传送卷");
                            if (cell?.UseItem() == true)
                            {
                                _protecttime=CEnvir.Now.AddSeconds(5);//随机使用延时的秒数
                            }
                        }
                    }                  
                }

                if (SetHomeItemBox.Checked)
                {
                    float protecthp = 0.30f;
                    protecthp = TimeBox4.Value / 100f;
                    if (GameScene.Game.User.CurrentHP < GameScene.Game.User.Stats[Stat.Health] * protecthp)
                    {
                        DXItemCell cell;
                        cell = GameScene.Game.InventoryBox.Grid.Grid.FirstOrDefault(x => x?.Item?.Info.ItemName == "回城卷");
                        if (cell?.UseItem() == true)
                        {
                            SetAutoOnHookBox.Checked = false;
                        }
                    }
                }            
            }          
        }
        public void AutoPickup()  //自动捡取
        {
            if (GameScene.Game.Observer)
                return;

            if (GameScene.Game.User.Dead)
                return;

            if (CEnvir.Now < GameScene.Game.PickUpTime)
                return;

            GameScene.Game.PickUpTime = CEnvir.Now.AddMilliseconds(250);

            int range = GameScene.Game.User.Stats[Stat.PickUpRadius];

            int userPosX = GameScene.Game.User.CurrentLocation.X;
            int userPosY = GameScene.Game.User.CurrentLocation.Y;

            for (int d = 0; d <= range; d++)
            {
                for (int y = userPosY - d; y <= userPosY + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= GameScene.Game.MapControl.Height) break;

                    for (int x = userPosX - d; x <= userPosX + d; x += Math.Abs(y - userPosY) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= GameScene.Game.MapControl.Width) break;

                        Cell cell = GameScene.Game.MapControl.Cells[x, y]; //直接访问我们已经检查了界限

                        if (cell?.Objects == null) continue;

                        foreach (MapObject cellObject in cell.Objects)
                        {
                            if (cellObject.Race != ObjectType.Item)
                                continue;

                            ItemObject item = (ItemObject)cellObject;

                            string name = item.Item.Info.ItemName;
                            int itemIdx = item.Item.Info.Index;
                           if (item.Item.Info.Effect == ItemEffect.ItemPart)
                           {
                               name = item.Name;
                                itemIdx = item.Item.AddedStats[Stat.ItemIndex];
                           }

                                FilterItem filterItem = GetFilterItem(name, itemIdx);

                            if (filterItem == null)
                            {
                                // 未在列表中, 不拾取
                                continue;
                            }

                            if (filterItem.canPickup != 0)
                            {
                                CEnvir.Enqueue(new C.PickUp
                                {
                                    ItemIdx = item.Item.Info.Index,
                                    Xpos = x,
                                    Ypos = y,
                                });
                            }

                        }

                    }
                }
            }
        }


        public void UpdateLocations()
        {
            int y = -ScrollBar.Value;

            foreach (AutoPotionRow row in Rows)
                row.Location = new Point(1, 1 + 50*row.Index + y);
        }
        //private bool showing = false;
        private void showindex(MirClass Class)
        {
            
            switch (Class)
            {
                case MirClass.Warrior:
                    SetFlamingSwordBox.Visible = true;   //自动烈火
                    SetDragobRiseBox.Visible = true;     //自动翔空
                    SetBladeStormBox.Visible = true;     //自动莲月
                    SetDefianceBox.Visible = true;     //自动铁布衫
                    SetMightBox.Visible = true;       //自动破血狂杀
                    break;
                case MirClass.Wizard:
                    SetMagicShieldBox.Visible = true;    //自动魔法盾
                    SetRenounceBox.Visible = true;       //自动凝血
                    break;
                case MirClass.Taoist:
                    SetPoisonDustBox.Visible = true;     //自动换毒
                    SetCelestialBox.Visible = true;      //自动阴阳盾
                    break;
                case MirClass.Assassin:
                    SetFourFlowersBox.Visible = true;    //自动四花
                    SetEvasionBox.Visible = true;       //自动风之闪避
                    SerRagingWindBox.Visible = true;    //自动风之守护
                    break;
            }
            //showing = true;

        }
        public bool Updating;

        public void UpdateLinks(StartInformation info)
        {

            showindex(info.Class);
            Updating = true;
            foreach (ClientAutoPotionLink link in Links)
            {
                if (link == null || link.Slot < 0 || link.Slot >= Rows.Length) continue;

                Rows[link.Slot].ItemCell.QuickInfo = Globals.ItemInfoList.Binding.FirstOrDefault(x => x.Index == link.LinkInfoIndex);
                Rows[link.Slot].HealthTargetBox.Value = link.Health;
                Rows[link.Slot].ManaTargetBox.Value = link.Mana;
                Rows[link.Slot].EnabledCheckBox.Checked = link.Enabled;
            }
            foreach (ClientUserMagic magic in info.Magics)
            {
                new DXListBoxItem
                {
                    Parent = SetMagicskillsComboBox.ListBox,
                    Label = { Text = magic.Info.Name },
                    Item = magic.Info.Magic,
                };

                new DXListBoxItem
                {
                    Parent = SetMagicskills1ComboBox.ListBox,
                    Label = { Text = magic.Info.Name },
                    Item = magic.Info.Magic,
                };

                new DXListBoxItem
                {
                    Parent = SetSingleHookSkillsComboBox.ListBox,
                    Label = { Text = magic.Info.Name },
                    Item = magic.Info.Magic,
                };


            }
            foreach (ClientAutoFightLink link in info.AutoFightLinks)
            {
                switch(link.Slot)
                {
                case AutoSetConf.SetBladeStormBox:
                    {
                        SetBladeStormBox.Checked = link.Enabled;
                    }
                    break;
                case AutoSetConf.SetBrightBox:
                    {
                        SetBrightBox.Checked = link.Enabled;
                        GameScene.Game?.MapControl?.LLayer.UpdateLights();
                    }
                    break;
                case AutoSetConf.SetCelestialBox:
                    {
                        SetCelestialBox.Checked = link.Enabled;
                    }
                    break;
                case AutoSetConf.SetCorpseBox:
                    {
                        SetCorpseBox.Checked = link.Enabled;
                    }
                    break;
                case AutoSetConf.SetDisplayBox:
                    {
                        SetDisplayBox.Checked = link.Enabled;
                    }
                    break;
                case AutoSetConf.SetDragobRiseBox:
                    {
                        SetDragobRiseBox.Checked = link.Enabled;
                    }
                    break;
                case AutoSetConf.SetFlamingSwordBox:
                    {
                        SetFlamingSwordBox.Checked = link.Enabled;
                    }
                    break;
                case AutoSetConf.SetFourFlowersBox:
                    {
                        SetFourFlowersBox.Checked = link.Enabled;
                    }
                    break;
                case AutoSetConf.SetMagicShieldBox:
                    {
                        SetMagicShieldBox.Checked = link.Enabled;
                    }
                    break;
                case AutoSetConf.SetMagicskills1Box:
                    {
                        SetMagicskills1Box.Checked = link.Enabled;
                        SetMagicskills1ComboBox.ListBox.SelectItem(link.MagicIndex);
                        TimeBox2.Value = link.TimeCount;
                    }
                    break;
                case AutoSetConf.SetMagicskillsBox:
                    {
                        SetMagicskillsBox.Checked = link.Enabled;
                        SetMagicskillsComboBox.ListBox.SelectItem(link.MagicIndex);
                        TimeBox1.Value = link.TimeCount;
                    }
                    break;
                case AutoSetConf.SetPickUpBox:
                    {
                        SetPickUpBox.Checked = link.Enabled;
                    }
                    break;
                case AutoSetConf.SetPoisonDustBox:
                    {
                        SetPoisonDustBox.Checked = link.Enabled;
                    }
                    break;
                case AutoSetConf.SetRenounceBox:
                    {
                        SetRenounceBox.Checked = link.Enabled;
                    }
                    break;
                case AutoSetConf.SetRunCheckBox:
                    {
                        SetRunCheckBox.Checked = link.Enabled;
                    }
                    break;
                case AutoSetConf.SetShiftBox:
                    {
                        SetShiftBox.Checked = link.Enabled;
                    }
                    break;
                case AutoSetConf.SetAutoPoisonBox:
                    {
                        SetAutoPoisonBox.Checked = link.Enabled;
                    }
                    break;
                case AutoSetConf.SetAutoAvoidBox:
                    {
                        SetAutoAvoidBox.Checked = link.Enabled;
                    }
                    break;
                case AutoSetConf.SetDeathResurrectionBox:
                    {
                        SetDeathResurrectionBox.Checked = link.Enabled;
                    }
                    break;
                case AutoSetConf.SetSingleHookSkillsBox:
                    {
                        SetSingleHookSkillsBox.Checked = link.Enabled;
                        SetSingleHookSkillsComboBox.ListBox.SelectItem(link.MagicIndex);
                    }
                    break;
                case AutoSetConf.SetRandomItemBox:
                    {
                        SetRandomItemBox.Checked = link.Enabled;
                        TimeBox3.Value = link.TimeCount;
                    }
                    break;
                case AutoSetConf.SetHomeItemBox:
                    {
                        SetHomeItemBox.Checked = link.Enabled;
                        TimeBox4.Value = link.TimeCount;
                    }
                    break;
                case AutoSetConf.SetDefianceBox:
                    {
                        SetDefianceBox.Checked = link.Enabled;
                    }
                    break;
                case AutoSetConf.SetMightBox:
                    {
                        SetMightBox.Checked = link.Enabled;
                    }
                    break;
                case AutoSetConf.SetShowHealth:
                    {
                        SetShowHealth.Checked = link.Enabled;
                    }
                    break;
                case AutoSetConf.SetEvasionBox:
                    {
                        SetEvasionBox.Checked = link.Enabled;
                    }
                    break;
                case AutoSetConf.SerRagingWindBox:
                    {
                        SerRagingWindBox.Checked = link.Enabled;
                    }
                    break;
                }
                //if (link.Slot == AutoSetConf.SetBladeStormBox) { SetBladeStormBox.Checked = link.Enabled; }
                //if (link.Slot == AutoSetConf.SetBrightBox) { SetBrightBox.Checked = link.Enabled; GameScene.Game?.MapControl?.LLayer.UpdateLights(); }
                //if (link.Slot == AutoSetConf.SetCelestialBox) { SetCelestialBox.Checked = link.Enabled; }
                //if (link.Slot == AutoSetConf.SetCorpseBox) { SetCorpseBox.Checked = link.Enabled; }
                //if (link.Slot == AutoSetConf.SetDisplayBox) { SetDisplayBox.Checked = link.Enabled;  }
                //if (link.Slot == AutoSetConf.SetDragobRiseBox) { SetDragobRiseBox.Checked = link.Enabled; }
                //if (link.Slot == AutoSetConf.SetFlamingSwordBox) { SetFlamingSwordBox.Checked = link.Enabled; }
                //if (link.Slot == AutoSetConf.SetFourFlowersBox) { SetFourFlowersBox.Checked = link.Enabled; }
                //if (link.Slot == AutoSetConf.SetMagicShieldBox) { SetMagicShieldBox.Checked = link.Enabled; }
                //if (link.Slot == AutoSetConf.SetMagicskills1Box) { SetMagicskills1Box.Checked = link.Enabled; SetMagicskills1ComboBox.ListBox.SelectItem(link.MagicIndex); TimeBox2.Value = link.TimeCount; }
                //if (link.Slot == AutoSetConf.SetMagicskillsBox) { SetMagicskillsBox.Checked = link.Enabled;SetMagicskillsComboBox.ListBox.SelectItem(link.MagicIndex);TimeBox1.Value = link.TimeCount; }
                //if (link.Slot == AutoSetConf.SetPickUpBox) { SetPickUpBox.Checked = link.Enabled; }
                //if (link.Slot == AutoSetConf.SetPoisonDustBox) { SetPoisonDustBox.Checked = link.Enabled; }
                //if (link.Slot == AutoSetConf.SetRenounceBox) { SetRenounceBox.Checked = link.Enabled; }
                //if (link.Slot == AutoSetConf.SetRunCheckBox) { SetRunCheckBox.Checked = link.Enabled; }
                //if (link.Slot == AutoSetConf.SetShiftBox) { SetShiftBox.Checked = link.Enabled; }
                //if (link.Slot == AutoSetConf.SetAutoPoisonBox) { SetAutoPoisonBox.Checked = link.Enabled; }
                //if (link.Slot == AutoSetConf.SetAutoAvoidBox) { SetAutoAvoidBox.Checked = link.Enabled; }
                //if (link.Slot == AutoSetConf.SetDeathResurrectionBox) { SetDeathResurrectionBox.Checked = link.Enabled; }
                //if (link.Slot == AutoSetConf.SetSingleHookSkillsBox) { SetSingleHookSkillsBox.Checked = link.Enabled;SetSingleHookSkillsComboBox.ListBox.SelectItem(link.MagicIndex); }
                //if (link.Slot == AutoSetConf.SetRandomItemBox) { SetRandomItemBox.Checked = link.Enabled; TimeBox3.Value = link.TimeCount; }
                //if (link.Slot == AutoSetConf.SetHomeItemBox) { SetHomeItemBox.Checked = link.Enabled; TimeBox4.Value = link.TimeCount; }
                //if (link.Slot == AutoSetConf.SetDefianceBox) { SetDefianceBox.Checked = link.Enabled; }
                //if (link.Slot == AutoSetConf.SetMightBox) { SetMightBox.Checked = link.Enabled; }
                //if (link.Slot == AutoSetConf.SetShowHealth) { SetShowHealth.Checked = link.Enabled; }
                //if (link.Slot == AutoSetConf.SetEvasionBox) { SetEvasionBox.Checked = link.Enabled; }
                //if (link.Slot == AutoSetConf.SerRagingWindBox) { SerRagingWindBox.Checked = link.Enabled; }
            }
                Updating = false;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            //if (FilterItems != null)
            //{
            //    if (!File.Exists(filterName))
            //    {
            //        File.Create(filterName);
            //    }

            //    string[] lines1 = new string[FilterItems.Length];
            //    for (int i = 0; i < FilterItems.Length; i++)
            //    {
            //        if (FilterItems[i] == null) continue;
            //        lines1[i] = $"{FilterItems[i].name},{FilterItems[i].canlog}, {FilterItems[i].canPickup}, {FilterItems[i].highLight}";
            //    }
            //    //File.WriteAllLines(filterName, lines1);
            //}

            if (disposing)
            {
                if (Links != null)
                {
                    for (int i = 0; i < Links.Length; i++)
                        Links[i] = null;

                    Links = null;
                }

                for (int i = 0; i < Rows.Length; i++)
                {
                    if (Rows[i] == null) continue;

                    if (!Rows[i].IsDisposed)
                        Rows[i].Dispose();

                    Rows[i] = null;
                }

                Rows = null;

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                if (PickScrollBar != null)
                {
                    if (!PickScrollBar.IsDisposed)
                        PickScrollBar.Dispose();

                    PickScrollBar = null;
                }
            }

        }

        #endregion
    }

    public sealed class FilterItem
    {
        public int itemIndex { set; get; }
        public string name { set; get; }
        public short canPickup { set; get; }//是否拣取
        public short highLight { set; get; } // 是否显示名称
        public short canlog { set; get; } // 是否提示
        /*
        public static Color GetHighColor(int highLight)
        {
            Color color = Color.Linen;
            switch (highLight)
            {
                case 0:
                    color = Color.Linen;
                    break;
                case 1:
                    color = Color.LimeGreen;
                    break;
                case 2:
                    color = Color.CornflowerBlue;
                    break;
                case 3:
                    color = Color.Purple;
                    break;
                case 4:
                    color = Color.DarkOrange;
                    break;
                default:
                    break;

            }

            return color;
        }*/
    }
    // 过滤表
    public sealed class PickupItem : DXControl
    {


        #region Properties

        public int ItemIndex
        {
            get => _ItemIndex;
            set
            {
                if (_ItemIndex == value) return;

                int oldValue = _ItemIndex;
                _ItemIndex = value;

                OnItemIndexChanged(oldValue, value);
            }
        }
        private int _ItemIndex;
        public event EventHandler<EventArgs> ItemIndexChanged;
        public void OnItemIndexChanged(int oValue, int nValue)
        {
            Item = GameScene.Game.AutoPotionBox.FilterItems[nValue];
            ItemIndexChanged?.Invoke(this, EventArgs.Empty);
        }

        public FilterItem Item
        {
            get => _Item;
            set
            {
                if (_Item == value) return;
                FilterItem oldValue = _Item;
                _Item = value;
                OnItemChanged(oldValue, value);
            }
        }
        private FilterItem _Item;
        public event EventHandler<EventArgs> ItemChanged;
        public void OnItemChanged(FilterItem oValue, FilterItem nValue)
        {
            if (Item == null)
            {
                Visible = false;
            }
            else
            {
                Visible = true;
                NameLabel.Text = Item.name;
                if (Item.canlog == 0)
                    SpecialBox.Checked = false;
                else
                    SpecialBox.Checked = true;
                if (Item.canPickup == 0)
                    PickUpBox.Checked = false;
                else
                    PickUpBox.Checked = true;
                if (Item.highLight == 0)
                    NameVisableBox.Checked = false;
                else
                    NameVisableBox.Checked = true;
            }

            ItemChanged?.Invoke(this, EventArgs.Empty);

        }
        #region Selected

        public bool Selected
        {
            get => _Selected;
            set
            {
                if (_Selected == value) return;

                bool oldValue = _Selected;
                _Selected = value;

                OnSelectedChanged(oldValue, value);
            }
        }
        private bool _Selected;
        //public event EventHandler<EventArgs> SelectedChanged;
        public void OnSelectedChanged(bool oValue, bool nValue)
        {
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);

            //SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXLabel NameLabel, PickupLabel, HeightLightLabel;
        public DXCheckBox SpecialBox, PickUpBox, NameVisableBox;

        #endregion

        public PickupItem()
        {
            Size = new Size(260, 18);

            DrawTexture = true;
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);

            Visible = true;

            NameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(5, 2),
                IsControl = false,
            };

            SpecialBox = new DXCheckBox
            {
                Parent = this,
                Location = new Point(118, 2)  //极品框坐标
            };
            SpecialBox.CheckedChanged += (o, e) => 
            {

                if (GameScene.Game == null || GameScene.Game.Observer || GameScene.Game.AutoPotionBox == null)
                {

                    return;
                }
                GameScene.Game.AutoPotionBox.FilterItems[Item.itemIndex].canlog = (short)(SpecialBox.Checked?1:0);

            };
            PickUpBox = new DXCheckBox
            {
                Parent = this,
                Location = new Point(168, 2)  //捡取框坐标
            };
            PickUpBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game == null || GameScene.Game.Observer || GameScene.Game.AutoPotionBox==null)
                {

                    return;
                }
                GameScene.Game.AutoPotionBox.FilterItems[Item.itemIndex].canPickup = (short)(PickUpBox.Checked ? 1 : 0);
            };
            NameVisableBox = new DXCheckBox
            {
                Parent = this,
                Location = new Point(218, 2)  //显示器框坐标
            };
            NameVisableBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game == null || GameScene.Game.Observer || GameScene.Game.AutoPotionBox == null)
                {

                    return;
                }
                GameScene.Game.AutoPotionBox.FilterItems[Item.itemIndex].highLight = (short)(NameVisableBox.Checked ? 1 : 0);
            };

        }



        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);



            if (disposing)
            {

                _Selected = false;
                //SelectedChanged = null;

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }
                if (SpecialBox != null)
                {
                    if (!SpecialBox.IsDisposed)
                        SpecialBox.Dispose();

                    SpecialBox = null;
                }
                if (PickUpBox != null)
                {
                    if (!PickUpBox.IsDisposed)
                        PickUpBox.Dispose();

                    PickUpBox = null;
                }
                if (NameVisableBox != null)
                {
                    if (!NameVisableBox.IsDisposed)
                        NameVisableBox.Dispose();

                    NameVisableBox = null;
                }

                if (PickupLabel != null)
                {
                    if (!PickupLabel.IsDisposed)
                        PickupLabel.Dispose();

                    PickupLabel = null;
                }

                if (HeightLightLabel != null)
                {
                    if (!HeightLightLabel.IsDisposed)
                        HeightLightLabel.Dispose();

                    HeightLightLabel = null;
                }

            }

        }

        #endregion
    }
    public sealed class AutoPotionRow : DXControl
    {
        #region Properties

        #region UseItem

        public ItemInfo UseItem
        {
            get => _UseItem;
            set
            {
                if (_UseItem == value) return;

                ItemInfo oldValue = _UseItem;
                _UseItem = value;

                OnUseItemChanged(oldValue, value);
            }
        }
        private ItemInfo _UseItem;
        public event EventHandler<EventArgs> UseItemChanged;
        public void OnUseItemChanged(ItemInfo oValue, ItemInfo nValue)
        {
            UseItemChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Index

        public int Index
        {
            get => _Index;
            set
            {
                if (_Index == value) return;

                int oldValue = _Index;
                _Index = value;

                OnIndexChanged(oldValue, value);
            }
        }
        private int _Index;
        public event EventHandler<EventArgs> IndexChanged;
        public void OnIndexChanged(int oValue, int nValue)
        {
            IndexChanged?.Invoke(this, EventArgs.Empty);
            IndexLabel.Text = (Index + 1).ToString();
            ItemCell.Slot = Index;

            UpButton.Enabled = Index > 0;
            DownButton.Enabled = Index < 7;
        }

        #endregion

        public DXLabel IndexLabel, HealthLabel, ManaLabel;
        public DXItemCell ItemCell;
        public DXNumberBox HealthTargetBox, ManaTargetBox;
        public DXCheckBox EnabledCheckBox;
        public DXButton UpButton, DownButton;

        #endregion

        public AutoPotionRow()
        {
            Size = new Size(260, 46);

            Border = true;
            BorderColour = Color.FromArgb(198, 166, 99);

            UpButton =  new DXButton
            {
                Index = 44,
                LibraryFile = LibraryFile.Interface,
                Location = new Point(5, 5),
                Parent = this,
                Enabled = false,
            };
            UpButton.MouseClick += (o, e) =>
            {
                GameScene.Game.AutoPotionBox.Updating = true;

                int hp = (int)HealthTargetBox.Value;
                int mp = (int)ManaTargetBox.Value;
                bool enabled = EnabledCheckBox.Checked;
                ItemInfo info = ItemCell.QuickInfo;

                ItemCell.QuickInfo = GameScene.Game.AutoPotionBox.Rows[Index - 1].ItemCell.QuickInfo;
                HealthTargetBox.Value = GameScene.Game.AutoPotionBox.Rows[Index - 1].HealthTargetBox.Value;
                ManaTargetBox.Value = GameScene.Game.AutoPotionBox.Rows[Index - 1].ManaTargetBox.Value;
                EnabledCheckBox.Checked = GameScene.Game.AutoPotionBox.Rows[Index - 1].EnabledCheckBox.Checked;

                GameScene.Game.AutoPotionBox.Rows[Index - 1].ItemCell.QuickInfo = info;
                GameScene.Game.AutoPotionBox.Rows[Index - 1].HealthTargetBox.Value = hp;
                GameScene.Game.AutoPotionBox.Rows[Index - 1].ManaTargetBox.Value = mp;
                GameScene.Game.AutoPotionBox.Rows[Index - 1].EnabledCheckBox.Checked = enabled;
                
                GameScene.Game.AutoPotionBox.Updating = false;

                SendUpdate();
                GameScene.Game.AutoPotionBox.Rows[Index - 1].SendUpdate();
            };

            DownButton =   new DXButton
            {
                Index = 46,
                LibraryFile = LibraryFile.Interface,
                Location = new Point(5, 29),
                Parent = this,
            };
            DownButton.MouseClick += (o, e) =>
            {
                GameScene.Game.AutoPotionBox.Updating = true;

                int hp = (int)HealthTargetBox.Value;
                int mp = (int)ManaTargetBox.Value;
                bool enabled = EnabledCheckBox.Checked;
                ItemInfo info = ItemCell.QuickInfo;

                ItemCell.QuickInfo = GameScene.Game.AutoPotionBox.Rows[Index + 1].ItemCell.QuickInfo;
                HealthTargetBox.Value = GameScene.Game.AutoPotionBox.Rows[Index + 1].HealthTargetBox.Value;
                ManaTargetBox.Value = GameScene.Game.AutoPotionBox.Rows[Index + 1].ManaTargetBox.Value;
                EnabledCheckBox.Checked = GameScene.Game.AutoPotionBox.Rows[Index + 1].EnabledCheckBox.Checked;

                GameScene.Game.AutoPotionBox.Rows[Index + 1].ItemCell.QuickInfo = info;
                GameScene.Game.AutoPotionBox.Rows[Index + 1].HealthTargetBox.Value = hp;
                GameScene.Game.AutoPotionBox.Rows[Index + 1].ManaTargetBox.Value = mp;
                GameScene.Game.AutoPotionBox.Rows[Index + 1].EnabledCheckBox.Checked = enabled;

                GameScene.Game.AutoPotionBox.Updating = false;

                SendUpdate();
                GameScene.Game.AutoPotionBox.Rows[Index + 1].SendUpdate();
            };

            ItemCell = new DXItemCell
            {
                Parent = this,
                Location = new Point(20, 5),
                AllowLink = true,
                FixedBorder = true,
                Border = true,
                GridType =  GridType.AutoPotion,
            };

            IndexLabel = new DXLabel
            {
                Parent = ItemCell,
                Text = (Index +1).ToString(),
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Italic),
                IsControl = false,
                Location = new Point(-2, -1)
            };
            
            
            HealthTargetBox = new DXNumberBox
            {
                Parent = this,
                Location = new Point(105, 5),
                Size =  new Size(80, 20),
                ValueTextBox = { Size = new Size(40, 18)},
                MaxValue = 50000,
                MinValue = 0,
                UpButton = { Location = new Point(63, 1) }
            };
            HealthTargetBox.ValueTextBox.ValueChanged += (o, e) => SendUpdate();

            ManaTargetBox = new DXNumberBox
            {
                Parent = this,
                Location = new Point(105, 25),
                Size = new Size(80, 20),
                ValueTextBox = { Size = new Size(40, 18) },
                MaxValue = 50000,
                MinValue = 0,
                UpButton = { Location = new Point(63, 1) }
            };
            ManaTargetBox.ValueTextBox.ValueChanged += (o, e) => SendUpdate();

            HealthLabel = new DXLabel
            {
                Parent = this,
                IsControl = false,
                Text = "生命值:"
            };
            HealthLabel.Location = new Point(HealthTargetBox.Location.X - HealthLabel.Size.Width, HealthTargetBox.Location.Y + (HealthTargetBox.Size.Height - HealthLabel.Size.Height) / 2);


            ManaLabel = new DXLabel
            {
                Parent = this,
                IsControl = false,
                Text = "魔法值:"
            };
            ManaLabel.Location = new Point(ManaTargetBox.Location.X - ManaLabel.Size.Width, ManaTargetBox.Location.Y + (ManaTargetBox.Size.Height - ManaLabel.Size.Height) / 2);
            
            EnabledCheckBox = new DXCheckBox
            {
                Label = {Text =  "启动"},
                Parent = this,
            };
            EnabledCheckBox.CheckedChanged += (o, e) => SendUpdate();

            EnabledCheckBox.Location = new Point(Size.Width - EnabledCheckBox.Size.Width - 5,  5);
        }

        #region Methods

        public void SendUpdate()
        {
            if (GameScene.Game.Observer) return;

            if (GameScene.Game.AutoPotionBox.Updating) return;

            CEnvir.Enqueue(new C.AutoPotionLinkChanged { Slot = Index, LinkIndex = ItemCell.Item?.Info.Index ?? -1, Enabled = EnabledCheckBox.Checked, Health = (int)HealthTargetBox.Value, Mana = (int)ManaTargetBox.Value });
        }
        #endregion
        
        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _UseItem = null;
                UseItemChanged = null;

                _Index = 0;
                IndexChanged = null;

                if (IndexLabel != null)
                {
                    if (!IndexLabel.IsDisposed)
                        IndexLabel.Dispose();

                    IndexLabel = null;
                }

                if (HealthLabel != null)
                {
                    if (!HealthLabel.IsDisposed)
                        HealthLabel.Dispose();

                    HealthLabel = null;
                }

                if (ManaLabel != null)
                {
                    if (!ManaLabel.IsDisposed)
                        ManaLabel.Dispose();

                    ManaLabel = null;
                }

                if (ItemCell != null)
                {
                    if (!ItemCell.IsDisposed)
                        ItemCell.Dispose();

                    ItemCell = null;
                }

                if (HealthTargetBox != null)
                {
                    if (!HealthTargetBox.IsDisposed)
                        HealthTargetBox.Dispose();

                    HealthTargetBox = null;
                }

                if (ManaTargetBox != null)
                {
                    if (!ManaTargetBox.IsDisposed)
                        ManaTargetBox.Dispose();

                    ManaTargetBox = null;
                }

                if (EnabledCheckBox != null)
                {
                    if (!EnabledCheckBox.IsDisposed)
                        EnabledCheckBox.Dispose();

                    EnabledCheckBox = null;
                }

                if (UpButton != null)
                {
                    if (!UpButton.IsDisposed)
                        UpButton.Dispose();

                    UpButton = null;
                }

                if (DownButton != null)
                {
                    if (!DownButton.IsDisposed)
                        DownButton.Dispose();

                    DownButton = null;
                }

            }

        }

        #endregion
    }

}
