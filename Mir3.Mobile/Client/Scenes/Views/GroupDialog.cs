using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.Scenes.Configs;
using Client.UserModels;
using Library;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C = Library.Network.ClientPackets;
using Font = MonoGame.Extended.Font;
using FontStyle = MonoGame.Extended.FontStyle;

//Cleaned
namespace Client.Scenes.Views
{
    /// <summary>
    /// 组队界面
    /// </summary>
    public sealed class GroupDialog : DXWindow
    {
        #region Properties

        public DXImageControl GroupGround;
        public DXButton Close1Button, AllowGroupButton, AddButton, Add1Button, RemoveButton;
        public DXCheckBox ShowPartyList;
        public DXLabel Captain, AllowGroupLabel;

        #region AllowGroup
        /// <summary>
        /// 允许组队开关
        /// </summary>
        public bool AllowGroup
        {
            get => _AllowGroup;
            set
            {
                if (_AllowGroup == value) return;

                bool oldValue = _AllowGroup;
                _AllowGroup = value;

                OnAllowGroupChanged(oldValue, value);
            }
        }
        private bool _AllowGroup;
        public event EventHandler<EventArgs> AllowGroupChanged;
        public void OnAllowGroupChanged(bool oValue, bool nValue)
        {
            AllowGroupChanged?.Invoke(this, EventArgs.Empty);

            if (AllowGroup && !BigPatchConfig.ChkAutoUnAcceptGroup)
            {
                AllowGroupButton.Index = 1370;
                AllowGroupButton.Hint = "AllowGroup.On".Lang();
                AllowGroupLabel.Text = "[允许]".Lang();
            }
            else
            {
                AllowGroupButton.Index = 1371;
                AllowGroupButton.Hint = "AllowGroup.Off".Lang();
                AllowGroupLabel.Text = "[拒绝]".Lang();
            }
        }

        #endregion

        public DXTab MemberTab;

        public List<ClientPlayerInfo> Members = new List<ClientPlayerInfo>();

        public List<DXLabel> Labels = new List<DXLabel>();

        #region SelectedLabel
        /// <summary>
        /// 选定的标签
        /// </summary>
        public DXLabel SelectedLabel
        {
            get => _SelectedLabel;
            set
            {
                if (_SelectedLabel == value) return;

                DXLabel oldValue = _SelectedLabel;
                _SelectedLabel = value;

                OnSelectedLabelChanged(oldValue, value);
            }
        }
        private DXLabel _SelectedLabel;
        public event EventHandler<EventArgs> SelectedLabelChanged;
        public void OnSelectedLabelChanged(DXLabel oValue, DXLabel nValue)
        {
            if (oValue != null)
            {
                oValue.ForeColour = Color.FromArgb(198, 166, 99);
                oValue.BackColour = Color.Empty;
            }

            if (nValue != null)
            {
                nValue.ForeColour = Color.White;
                nValue.BackColour = Color.FromArgb(24, 16, 16);
            }

            RemoveButton.Enabled = nValue != null && Members[0].ObjectID == GameScene.Game.User.ObjectID;

            SelectedLabelChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        public override WindowType Type => WindowType.GroupBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        #endregion

        /// <summary>
        /// 组队界面
        /// </summary>
        public GroupDialog()
        {
            HasTitle = false;  //不显示标题
            HasFooter = false;  //不显示页脚
            HasTopBorder = false;  //不显示边框
            TitleLabel.Visible = false;  //标题标签不用
            IgnoreMoveBounds = true;
            Opacity = 0F;

            Size s;
            s = UI1Library.GetSize(1360);
            Size = s;
            Location = ClientArea.Location;

            GroupGround = new DXImageControl  //组队容器
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1360,
                ImageOpacity = 0.85F,
                Location = new Point(0, 0),
                IsControl = true,
                PassThrough = true,
                Visible = true
            };

            Close1Button = new DXButton       //关闭按钮
            {
                Parent = this,
                Index = 1221,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(217, 293)
            };
            Close1Button.TouchUp += (o, e) => Visible = false;

            AllowGroupButton = new DXButton    //允许组队按钮
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1371,
                Parent = this,
                Hint = "AllowGroup.Off".Lang(),
                Location = new Point(24, 44)
            };
            AllowGroupButton.TouchUp += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.GroupSwitch { Allow = !AllowGroup });
            };

            Captain = new DXLabel
            {
                Parent = this,
                Size = new Size(40, 15),
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Location = new Point(110, 12),
                Text = "组队".Lang(),
            };

            AllowGroupLabel = new DXLabel
            {
                Parent = this,
                Size = new Size(40, 15),
                ForeColour = Color.White,
                Location = new Point(110, 44),
                Text = "[拒绝]".Lang(),
            };

            DXTabControl members = new DXTabControl
            {
                Parent = this,
                Size = new Size(200, 200),
                Location = new Point(28, 66),
            };

            MemberTab = new DXTab   //队员选项卡
            {
                TabButton =
                {
                    IsControl = false,
                    Visible = false,
                },
                Parent = members,
                Border = false,
                Opacity = 0F,
            };

            //ShowPartyList = new DXCheckBox      //组队界面开关
            //{
            //    Label = { Text = "组队界面".Lang() },
            //    Parent = this,
            //    Visible = true,
            //};
            //ShowPartyList.Location = new Point(ClientArea.Right - 80, 40);
            //ShowPartyList.CheckedChanged += (o, e) =>
            //{
            //    Config.PartyListVisible = ShowPartyList.Checked;
            //    GameScene.Game.GroupMemberBox.Visible = Config.PartyListVisible;
            //};

            AddButton = new DXButton   //邀请按钮
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1362,
                Hint = "邀请组队".Lang(),
                Location = new Point(ClientArea.Right - 210, Size.Height - 50),
                Parent = this,
            };
            AddButton.TouchUp += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                if (GameScene.Game.MapControl.MapInfo.CanPlayName == true) return;

                if (Members.Count >= Globals.GroupLimit)
                {
                    GameScene.Game.ReceiveChat("组队人数已达到上限".Lang(), MessageType.System);
                    return;
                }

                if (Members.Count >= Globals.GroupLimit)
                {
                    GameScene.Game.ReceiveChat("你不是队长无权限操作".Lang(), MessageType.System);
                    return;
                }

                new DXInputWindow((str) =>
                {
                    return Globals.CharacterReg.IsMatch(str);
                }, (str) =>
                {
                    CEnvir.Enqueue(new C.GroupInvite { Name = str });
                }, "请输入你要邀请组队的玩家名字".Lang());
            };

            Add1Button = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1365,
                Hint = "添加队员".Lang(),
                Location = new Point(ClientArea.Right - 150, Size.Height - 50),
                Parent = this,
            };
            Add1Button.TouchUp += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                if (GameScene.Game.MapControl.MapInfo.CanPlayName == true) return;

                if (Members.Count >= Globals.GroupLimit)
                {
                    GameScene.Game.ReceiveChat("组队人数已达到上限".Lang(), MessageType.System);
                    return;
                }

                if (Members.Count >= Globals.GroupLimit)
                {
                    GameScene.Game.ReceiveChat("你不是队长无权限操作".Lang(), MessageType.System);
                    return;
                }
                //Invite Group Member
                new DXInputWindow((str) =>
                {
                    return Globals.CharacterReg.IsMatch(str);
                }, (str) =>
                {
                    CEnvir.Enqueue(new C.GroupInvite { Name = str });
                }, "请输入你要邀请组队的玩家名字".Lang());
            };

            RemoveButton = new DXButton   //删除队员按钮
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1368,
                Hint = "移除队员".Lang(),
                Location = new Point(ClientArea.Right - 90, Size.Height - 50),
                Parent = this,
                //Enabled = false,
            };
            RemoveButton.TouchUp += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.GroupRemove { Name = SelectedLabel.Text });
            };
        }

        #region Methods
        /// <summary>
        /// 更新组队成员
        /// </summary>
        public void UpdateMembers()
        {
            SelectedLabel = null;

            foreach (DXLabel label in Labels)  //遍历 标签
                label.Dispose();  //标签处理

            Labels.Clear();  //标签清除

            for (int i = 0; i < Members.Count; i++)
            {
                ClientPlayerInfo member = Members[i];

                DXLabel label = new DXLabel
                {
                    Parent = MemberTab,
                    Location = new Point(10 + 100 * (i % 2), 10 + 20 * (i / 2)),
                    Text = member.Name,
                };
                label.MouseClick += (o, e) =>
                {
                    if (e.Button == MouseButtons.Left)
                        SelectedLabel = label;
                    //else if (e.Button == MouseButtons.Right)
                    //{
                    //    GameScene.Game.BigMapBox.Visible = true;
                    //    GameScene.Game.BigMapBox.Opacity = 1F;


                    //    if (!GameScene.Game.DataDictionary.TryGetValue(member.ObjectID, out ClientObjectData data)) return;

                    //    GameScene.Game.BigMapBox.SelectedInfo = Globals.MapInfoList.Binding.FirstOrDefault(x => x.Index == data.MapIndex);
                    //}
                };

                Labels.Add(label);
            }

            AddButton.Enabled = Members.Count == 0 || Members[0].ObjectID == GameScene.Game.User.ObjectID;
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _AllowGroup = false;
                AllowGroupChanged = null;

                if (AllowGroupButton != null)
                {
                    if (!AllowGroupButton.IsDisposed)
                        AllowGroupButton.Dispose();

                    AllowGroupButton = null;
                }

                if (AddButton != null)
                {
                    if (!AddButton.IsDisposed)
                        AddButton.Dispose();

                    AddButton = null;
                }

                if (RemoveButton != null)
                {
                    if (!RemoveButton.IsDisposed)
                        RemoveButton.Dispose();

                    RemoveButton = null;
                }

                if (MemberTab != null)
                {
                    if (!MemberTab.IsDisposed)
                        MemberTab.Dispose();

                    MemberTab = null;
                }

                for (int i = 0; i < Labels.Count; i++)
                {
                    if (Labels[i] != null)
                    {
                        if (!Labels[i].IsDisposed)
                            Labels[i].Dispose();

                        Labels[i] = null;
                    }
                }
                Labels.Clear();
                Labels = null;

                if (_SelectedLabel != null)
                {
                    if (!_SelectedLabel.IsDisposed)
                        _SelectedLabel.Dispose();

                    _SelectedLabel = null;
                }

                SelectedLabelChanged = null;

                Members.Clear();
                Members = null;
            }
        }
        #endregion
    }

    /// <summary>
    /// 左边组队列表信息框
    /// </summary>
    public sealed class GroupMemberDialog : DXWindow
    {
        public DXControl MemberContainer;

        public List<GroupMember> GroupMembers = new List<GroupMember>();

        private DXButton VisibleButton;

        public override bool CustomSize => false;

        public override bool AutomaticVisibility => false;

        public override WindowType Type => WindowType.None;

        public GroupMemberDialog()
        {
            //TitleLabel.Text = "Group";
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            Border = false;
            TitleLabel.Visible = false;
            CloseButton.Visible = false;
            //AllowResize = false;
            PassThrough = true;
            Opacity = 0;
            Size = new Size(110 + 48, 212);
            MemberContainer = new DXControl
            {
                Size = new Size(110, 35),
                Location = new Point(24, 0),
                Parent = this,
                PassThrough = true,
            };

            VisibleButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 1162,
            };
            VisibleButton.Location = new Point(Size.Width - VisibleButton.Size.Width, 180);
            VisibleButton.TouchUp += (o, e) =>
            {
                //if (GameScene.Game.Observer) return;

                VisibleButton.Index = VisibleButton.Index == 1162 ? 1163 : 1162;
                ResizeWindow(GroupMembers.Count);
            };
        }

        /// <summary>
        /// 调整窗口大小
        /// </summary>
        /// <param name="count"></param>
        public void ResizeWindow(int count)
        {
            Visible = count >= 2;
            int width = 110 + 110 * ((count - 1) / 10);
            int height = 35 * Math.Min(10, count);
            MemberContainer.Size = new Size(width, height + 20);
            if (VisibleButton.Index == 1163)
                Size = new Size(24, 212);
            else
                Size = new Size(width + 48, Math.Max(212, height));
            VisibleButton.Location = new Point(Size.Width - VisibleButton.Size.Width, 180);
        }

        /// <summary>
        /// 填充成员
        /// </summary>
        /// <param name="currentMembers"></param>
        public void PopulateMembers(List<ClientPlayerInfo> currentMembers)
        {
            new List<ClientPlayerInfo>(currentMembers);
            ResizeWindow(0);
            foreach (GroupMember groupMember2 in GroupMembers)
            {
                groupMember2.Dispose();
            }
            if (currentMembers.Count <= 0)
            {
                return;
            }
            GroupMembers.Clear();
            for (int i = 0; i < currentMembers.Count; i++)
            {
                ClientPlayerInfo playerInfo = currentMembers[i];
                GroupMember currentGroupMember = new GroupMember(playerInfo);
                int num = i % 10;
                int x2 = 1 + Math.Abs(i / 10) * (currentGroupMember.Size.Width + 10);
                int y = 1 + ((num != 0) ? (num * currentGroupMember.Size.Height + num * 10) : 0);
                GroupMembers.Add(currentGroupMember);
                currentGroupMember.Parent = MemberContainer;
                currentGroupMember.Location = new Point(x2, y);
                //GroupMember groupMember = currentGroupMember;
                //groupMember.ProcessAction = (Action)Delegate.Combine(groupMember.ProcessAction, (Action)delegate
                //{
                //    if (MouseControl == currentGroupMember)
                //    {
                //        MapObject mapObject = GameScene.Game.MapControl.Objects?.FirstOrDefault((MapObject x) => x.ObjectID == currentGroupMember.ObjectID);
                //        if (mapObject != null)
                //        {
                //            GameScene.Game.MouseObject = mapObject;
                //        }
                //    }
                //});
            }
            ResizeWindow(currentMembers.Count);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
            {
                return;
            }
            foreach (GroupMember groupMember in GroupMembers)
            {
                if (!groupMember.IsDisposed)
                {
                    groupMember.Dispose();
                }
            }
            GroupMembers.Clear();
            GroupMembers = null;
            //if (!MemberContainer.IsDisposed)
            //{
            //    MemberContainer.Dispose();
            //}
            //MemberContainer = null;
        }
    }

    public sealed class GroupMember : DXControl
    {
        public DXLabel MemberName;

        public DXControl HealthBar, HealthBarFrame;

        //public DXControl ManaBar;

        private DXButton SelectedButton;

        public uint ObjectID;

        public GroupMember(ClientPlayerInfo playerInfo)
        {
            Size = new Size(103, 24);
            DrawTexture = false;
            //BackColour = Color.FromArgb(16, 8, 8);
            //Opacity = 0.5f;
            //Border = true;
            //BorderColour = Color.FromArgb(82, 65, 57);
            Location = new Point(1, 1);
            //PassThrough = true;
            ObjectID = playerInfo.ObjectID;

            SelectedButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 1160,
            };

            MemberName = new DXLabel
            {
                //Location = new Point(17, 3),
                //Font = new Font(Config.FontName, CEnvir.FontSize(10f), FontStyle.Regular),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                //Size = new Size(100, 15),
                Parent = this,
                //ForeColour = Color.Yellow,
                Outline = true,
                //OutlineColour = Color.Black,
                IsControl = false,
                PassThrough = true,
                Text = playerInfo.Name
            };
            MemberName.Location = new Point(SelectedButton.Location.X + SelectedButton.Size.Width + 5, 0);

            HealthBarFrame = new DXControl
            {
                Parent = this,
                Size = new Size(70, 3),
                Location = new Point(MemberName.Location.X, MemberName.Location.Y + MemberName.Size.Height + 3),
                DrawTexture = true,
                BackColour = Color.FromArgb(250, 215, 195),
                Border = true,
                BorderColour = Color.Black,
                Visible = true,
                PassThrough = true
            };

            HealthBar = new DXControl
            {
                Parent = this,
                Size = new Size(70, 3),
                Location = new Point(MemberName.Location.X, MemberName.Location.Y + MemberName.Size.Height + 3),
                DrawTexture = true,
                BackColour = Color.FromArgb(255, 93, 57),
                Border = true,
                BorderColour = Color.Black,
                Visible = true,
                PassThrough = true
            };

            //ManaBar = new DXControl
            //{
            //    Parent = this,
            //    Size = new Size(100, 3),
            //    Location = new Point(7, HealthBar.Location.Y + 1 + HealthBar.Size.Height),
            //    DrawTexture = true,
            //    BackColour = Color.FromArgb(55, 55, 230),
            //    Visible = true,
            //    PassThrough = true
            //};

            TouchUp += (o, e) =>
            {
                //SelectedButton.Index = SelectedButton.Index == 1160 ? 1161 : 1160;
                if (ObjectID == 0) return;
                MapObject mapObject = GameScene.Game.MapControl.Objects?.FirstOrDefault(x => x.ObjectID == ObjectID);
                if (mapObject != null)
                {
                    GameScene.Game.MouseObject = mapObject;
                    GameScene.Game.MagicObject = mapObject;
                    SelectedButton.Index = 1161;
                }
                else
                    SelectedButton.Index = 1160;
            };
        }

        public override void Process()
        {
            base.Process();

            if (!Visible) return;

            //没有玩家对象数据直接跳出
            ClientObjectData data;
            if (!GameScene.Game.DataDictionary.TryGetValue(ObjectID, out data)) return;

            int currentHealth = Math.Max(0, data.Health);
            int maxHealth = data.MaxHealth;
            MemberName.ForeColour = (currentHealth <= 0) ? Color.Red : Color.White;
            HealthBar.Size = new Size(currentHealth * 70 / maxHealth, 3);
            //ManaBar.Size = new Size(currentMana * 70 / maxMana, 3);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
            {
                return;
            }
            SelectedButton.TryDispose();
            MemberName.TryDispose();
            HealthBar.TryDispose();
            HealthBarFrame.TryDispose();
            //ManaBar = null;

            ObjectID = 0;
        }
    }

    /// <summary>
    /// 右边敌对列表信息框
    /// </summary>
    public sealed class EnemyMemberDialog : DXWindow
    {
        public List<EnemyMember> EnemyMembers = new List<EnemyMember>();

        public HashSet<uint> AttackedMembers = new HashSet<uint>();

        public DXControl Background;
        private DXButton SwitchButton, VisibleButton;
        private const int columerows = 7;  //每列7行

        public override bool CustomSize => false;

        public override bool AutomaticVisibility => false;

        public override WindowType Type => WindowType.None;

        public EnemyMemberDialog()
        {
            //TitleLabel.Text = "Group";
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            Border = false;
            TitleLabel.Visible = false;
            CloseButton.Visible = false;
            //AllowResize = false;
            PassThrough = true;
            Opacity = 0;

            Background = new DXControl
            {
                Parent = this,
                DrawTexture = false,
                Size = new Size(120 * 2 + 10, 300),
                Visible = false,
                PassThrough = true,
            };
            Background.VisibleChanged += (o, e) =>
            {
                if (Background.Visible)
                    Location = new Point(GameScene.Game.MiniMapBox.Location.X - Background.Size.Width - 10, Location.Y);
                else
                    Location = new Point(GameScene.Game.Size.Width - Size.Width - 24 - Mir3.Mobile.Game1.Native.UI.ScreenOffset.Y, Location.Y);

                VisibleButton.Index = Background.Visible ? 1163 : 1162;
            };

            Size = new Size(30 + Background.Size.Width, Background.Size.Height);

            SwitchButton = new DXButton
            {
                Parent = Background,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 1167,
            };
            SwitchButton.Location = new Point(Background.Size.Width - SwitchButton.Size.Width, 0);
            SwitchButton.TouchUp += (o, e) =>
            {
                //if (GameScene.Game.Observer) return;

                SwitchButton.Index = SwitchButton.Index == 1167 ? 1166 : 1167;

                GameScene.Game.SelectPlayerType = SwitchButton.Index == 1167 ? SelectPlayerType.NeerBy : SelectPlayerType.MinBlood;
            };

            for (int i = 0; i < columerows * 2; i++)  //2列
            {
                EnemyMember enemyMember = new EnemyMember()
                {
                    Parent = Background,
                    Visible = false,
                };
                int x = Math.Abs(i / columerows) * (enemyMember.Size.Width + 10);
                int y = ((i % columerows) * enemyMember.Size.Height + (i % columerows) * 10) + SwitchButton.Size.Height;
                enemyMember.Location = new Point(x, y);
                EnemyMembers.Add(enemyMember);
            }

            VisibleButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 1162,
            };
            VisibleButton.Location = new Point(Size.Width - VisibleButton.Size.Width, 200);
            VisibleButton.TouchUp += (o, e) =>
            {
                //if (GameScene.Game.Observer) return;

                VisibleButton.Index = VisibleButton.Index == 1162 ? 1163 : 1162;
                Background.Visible = VisibleButton.Index == 1163;
            };
        }

        private DateTime refreshTime;
        public override void Process()
        {
            if (!Background.Visible) return;
            if (GameScene.Game.MapControl.MapInfo.CanPlayName)
            {
                Background.Visible = false;
                return;
            }

            base.Process();

            if (CEnvir.Now >= refreshTime)
            {
                refreshTime = CEnvir.Now.AddSeconds(1);

                //先按 附近 血量 2个模式排序
                List<MapObject> objects = null;
                switch (GameScene.Game.SelectPlayerType)
                {
                    case SelectPlayerType.NeerBy:
                        objects = GameScene.Game.MapControl.Objects?.Where
                            (x => Functions.InRange(x.CurrentLocation, MapControl.User.CurrentLocation, Globals.MagicRange)
                            && (x.Race == ObjectType.Player || (x.Race == ObjectType.Monster && ((MonsterObject)x).MonsterInfo.IsBoss))
                            && !x.Dead && x.ObjectID != MapControl.User.ObjectID)?.OrderBy
                            (x => (x.CurrentLocation.X - MapControl.User.CurrentLocation.X) * (x.CurrentLocation.X - MapControl.User.CurrentLocation.X) +
                            (x.CurrentLocation.Y - MapControl.User.CurrentLocation.Y) * (x.CurrentLocation.Y - MapControl.User.CurrentLocation.Y)).ToList();
                        break;
                    case SelectPlayerType.MinBlood:
                        objects = GameScene.Game.MapControl.Objects?.Where
                            (x => Functions.InRange(x.CurrentLocation, MapControl.User.CurrentLocation, Globals.MagicRange)
                            && (x.Race == ObjectType.Player || (x.Race == ObjectType.Monster && ((MonsterObject)x).MonsterInfo.IsBoss))
                            && !x.Dead && x.ObjectID != MapControl.User.ObjectID)?.OrderBy(x => x.CurrentHP).ToList();
                        break;
                }
                if (objects == null) return;

                //敌对列表赋值
                //初始化敌对列表
                for (int i = 0; i < EnemyMembers.Count; i++)
                {
                    EnemyMembers[i].ObjectID = 0;
                }

                //优先显示攻击者 不考虑其他（队友，行会等）都显示
                int num = 0;
                int row = 0;
                for (int i = 0; i < objects.Count; i++)
                {
                    if (num >= EnemyMembers.Count) break;

                    MapObject obj = objects[i];
                    if (obj == null) continue;
                    //if (obj.Race != ObjectType.Player) continue;
                    //if (!Functions.InRange(obj.CurrentLocation, MapControl.User.CurrentLocation, Globals.MagicRange)) continue;
                    //if (obj.Dead) continue;

                    if (AttackedMembers.Any(x => x == obj.ObjectID))
                    {
                        if (num < columerows)
                            row = num + columerows;
                        else
                            row = num - columerows;

                        var enemyMember = EnemyMembers[row];
                        enemyMember.ObjectID = obj.ObjectID;
                        enemyMember.Attacked.Visible = true;
                        if (GameScene.Game.MagicObject == obj || GameScene.Game.TargetObject == obj)
                            enemyMember.Selected.Visible = true;
                        num++;
                        continue;
                    }
                }

                //boss
                for (int i = 0; i < objects.Count; i++)
                {
                    if (num >= EnemyMembers.Count) break;

                    MapObject obj = objects[i];
                    if (obj == null) continue;
                    //if (obj.Race != ObjectType.Player) continue;
                    //if (!Functions.InRange(obj.CurrentLocation, MapControl.User.CurrentLocation, Globals.MagicRange)) continue;
                    //if (obj.Dead) continue;

                    if (obj.Race == ObjectType.Monster)
                    {
                        if (num < columerows)
                            row = num + columerows;
                        else
                            row = num - columerows;

                        var enemyMember = EnemyMembers[row];
                        enemyMember.ObjectID = obj.ObjectID;
                        enemyMember.Boss.Visible = true;
                        if (GameScene.Game.MagicObject == obj || GameScene.Game.TargetObject == obj)
                            enemyMember.Selected.Visible = true;
                        num++;
                        continue;
                    }
                }

                //其次在填充其他
                for (int i = 0; i < objects.Count; i++)
                {
                    if (num >= EnemyMembers.Count) break;

                    MapObject obj = objects[i];
                    if (obj == null) continue;
                    //if (obj.Race != ObjectType.Player) continue;
                    //if (!Functions.InRange(obj.CurrentLocation, MapControl.User.CurrentLocation, Globals.MagicRange)) continue;
                    //if (obj.Dead) continue;

                    if (AttackedMembers.Any(x => x == obj.ObjectID)) continue;  //攻击者，上面已经显示
                    //if (obj.ObjectID == MapControl.User.ObjectID) continue; //自己不显示
                    if (obj.Race == ObjectType.Monster) continue;

                    //根据攻击模式判断是否显示
                    switch (MapControl.User.AttackMode)
                    {
                        case AttackMode.Group:
                            if (GameScene.Game.GroupMemberBox.GroupMembers == null || GameScene.Game.GroupMemberBox.GroupMembers.Any(x => x.ObjectID == obj.ObjectID))
                                continue;
                            break;
                        case AttackMode.Guild:
                            if (GameScene.Game.GuildBox.GuildInfo == null || GameScene.Game.GuildBox.GuildInfo.Members == null || GameScene.Game.GuildBox.GuildInfo.Members.Any(x => x.ObjectID == obj.ObjectID))
                                continue;
                            break;
                        case AttackMode.Peace:
                            return;
                        case AttackMode.WarRedBrown:
                            if (GameScene.Game.GroupMemberBox.GroupMembers != null && GameScene.Game.GroupMemberBox.GroupMembers.Any(x => x.ObjectID == obj.ObjectID))
                                continue;
                            if (GameScene.Game.GuildBox.GuildInfo != null && GameScene.Game.GuildBox.GuildInfo.Members != null && GameScene.Game.GuildBox.GuildInfo.Members.Any(x => x.ObjectID == obj.ObjectID))
                                continue;
                            if (obj.NameColour != Globals.RedNameColour && obj.NameColour != Globals.BrownNameColour && obj.NameColour != Color.Yellow)
                                continue;
                            break;
                    }

                    if (num < columerows)
                        row = num + columerows;
                    else
                        row = num - columerows;

                    var enemyMember = EnemyMembers[row];
                    enemyMember.ObjectID = obj.ObjectID;
                    if (GameScene.Game.MagicObject == obj || GameScene.Game.TargetObject == obj)
                        enemyMember.Selected.Visible = true;
                    num++;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
            {
                return;
            }
            foreach (EnemyMember enemyMember in EnemyMembers)
            {
                if (!enemyMember.IsDisposed)
                {
                    enemyMember.Dispose();
                }
            }
            EnemyMembers.Clear();
            EnemyMembers = null;
            AttackedMembers.Clear();
            AttackedMembers = null;

            Background.TryDispose();
            SwitchButton.TryDispose();
            VisibleButton.TryDispose();
        }
    }

    public sealed class EnemyMember : DXControl
    {
        public DXLabel MemberName;

        private DXControl HealthBar, HealthBarBackground;

        public DXImageControl Selected, Attacked, Boss;

        public uint ObjectID
        {
            get => _ObjectID;
            set
            {
                if (_ObjectID == value) return;

                uint oldValue = _ObjectID;
                _ObjectID = value;

                OnObjectIDChanged(oldValue, value);
            }
        }
        private uint _ObjectID;
        public void OnObjectIDChanged(uint oValue, uint nValue)
        {
            Visible = nValue > 0;

            if (nValue == 0)
            {
                Selected.Visible = false;
                Attacked.Visible = false;
                Boss.Visible = false;
            }
            else
                MemberName.Text = GameScene.Game.MapControl.Objects?.FirstOrDefault(x => x.ObjectID == nValue)?.Name;
        }

        public EnemyMember()
        {
            Size = new Size(120, 28);
            DrawTexture = false;
            //BackColour = Color.FromArgb(16, 8, 8);
            //Opacity = 0.5f;
            //Border = true;
            //BorderColour = Color.FromArgb(82, 65, 57);
            Location = new Point(1, 1);
            //PassThrough = true;

            Selected = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 1165,
                IsControl = false,
                Visible = false,
            };

            MemberName = new DXLabel
            {
                //Location = new Point(17, 3),
                //Font = new Font(Config.FontName, CEnvir.FontSize(10f), FontStyle.Regular),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                //Size = new Size(100, 15),
                Parent = this,
                ForeColour = Color.White,
                Outline = true,
                //OutlineColour = Color.Black,
                IsControl = false,
                PassThrough = true,
                Text = "名字"
            };
            MemberName.Location = new Point(Selected.Location.X + Selected.Size.Width + 5, 0);

            HealthBarBackground = new DXControl
            {
                Parent = this,
                Size = new Size(70, 3),
                Location = new Point(MemberName.Location.X, MemberName.Location.Y + MemberName.Size.Height + 3),
                DrawTexture = true,
                BackColour = Color.FromArgb(250, 215, 195),
                Border = true,
                BorderColour = Color.Black,
                Visible = true,
                IsControl = false,
                PassThrough = true
            };

            HealthBar = new DXControl
            {
                Parent = this,
                Size = new Size(70, 3),
                Location = new Point(MemberName.Location.X, MemberName.Location.Y + MemberName.Size.Height + 3),
                DrawTexture = true,
                BackColour = Color.FromArgb(255, 93, 57),
                Border = true,
                BorderColour = Color.Black,
                Visible = true,
                IsControl = false,
                PassThrough = true
            };

            Attacked = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 1164,
                Visible = false,
                IsControl = false,
                Location = new Point(HealthBarBackground.Location.X + HealthBarBackground.Size.Width, 0),
            };
            Attacked.VisibleChanged += (o, e) =>
            {
                if (Attacked.Visible)
                    Boss.Visible = false;
            };

            Boss = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 1168,
                Visible = false,
                IsControl = false,
                Location = Attacked.Location,
            };
            Boss.VisibleChanged += (o, e) =>
            {
                if (Boss.Visible)
                    Attacked.Visible = false;
            };

            TouchUp += (o, e) =>
            {
                if (ObjectID == 0) return;
                MapObject mapObject = GameScene.Game.MapControl.Objects?.FirstOrDefault(x => x.ObjectID == ObjectID);
                if (mapObject != null)
                {
                    GameScene.Game.MouseObject = mapObject;
                    GameScene.Game.MagicObject = mapObject;
                    Selected.Visible = true;
                }
                else
                    Selected.Visible = false;
            };
        }

        public override void Process()
        {
            if (!Visible) return;

            base.Process();

            //没有玩家对象数据直接跳出
            ClientObjectData data;
            if (!GameScene.Game.DataDictionary.TryGetValue(ObjectID, out data)) return;

            int currentHealth = Math.Max(0, data.Health);
            int maxHealth = data.MaxHealth;
            MemberName.ForeColour = (currentHealth <= 0) ? Color.Red : Color.White;
            HealthBar.Size = new Size(currentHealth * 70 / maxHealth, 3);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
            {
                return;
            }
            Selected.TryDispose();
            Attacked.TryDispose();
            Boss.TryDispose();
            MemberName.TryDispose();
            HealthBar.TryDispose();
            HealthBarBackground.TryDispose();

            _ObjectID = 0;
        }
    }
}
