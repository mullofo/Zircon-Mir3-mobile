using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.UserModels;
using Library;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using C = Library.Network.ClientPackets;
using Font = MonoGame.Extended.Font;
using FontStyle = MonoGame.Extended.FontStyle;


namespace Client.Scenes.Views
{
    /// <summary>
    /// 记忆传送功能
    /// </summary>
    public sealed class FixedPointDialog : DXWindow
    {
        #region Properties
        public DXImageControl BackGround;
        private DXTextBox SearchTextBox;
        private DXButton SearchButton, SignButton, MoveButton, Close1Button;
        public DXListBox ListBox;
        private List<DXListBoxItem> ListBoxItems = new List<DXListBoxItem>();
        private RightHandDialog RightDialog;

        public override WindowType Type => WindowType.FixedPointBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        #endregion

        /// <summary>
        /// 记忆传送功能界面
        /// </summary>
        public FixedPointDialog()
        {
            HasTitle = false;              //不显示标题
            HasFooter = false;             //不显示页脚
            HasTopBorder = false;          //不显示边框
            TitleLabel.Visible = false;    //标题标签不用
            CloseButton.Visible = false;
            Movable = true;                //可以移动
            IgnoreMoveBounds = true;
            Opacity = 0F;

            SetClientSize(new Size(232, 342));

            BackGround = new DXImageControl   //背景图
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 3200,
                ImageOpacity = 0.85F,
                IsControl = true,
                PassThrough = true,
            };

            TitleLabel = new DXLabel  //标签文本
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                Location = new Point((Size.Width - TitleLabel.Size.Width) / 2, 1),
            };

            Close1Button = new DXButton       //关闭按钮
            {
                Parent = this,
                Index = 113,
                LibraryFile = LibraryFile.Interface,
                Hint = "关闭".Lang(),
                Location = new Point(200, 0),
            };
            Close1Button.MouseEnter += (o, e) => CloseButton.Index = 112;
            Close1Button.MouseLeave += (o, e) => CloseButton.Index = 113;
            Close1Button.MouseClick += (o, e) => Visible = false;

            SearchTextBox = new DXTextBox  //搜索输入框
            {
                Parent = this,
                Size = new Size(115, 15),
                Location = new Point(ClientArea.Left + 5, ClientArea.Y + 38),
                Border = false,
                Opacity = 0F,
            };

            //性能原因 没必要这样刷新
            //SearchTextBox.TextBox.TextChanged += (o, e) => ApplySearchFilter();

            SearchButton = new DXButton  //搜索按钮
            {
                Location = new Point(140, ClientArea.Y + 35),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = @"搜索".Lang() },
            };
            SearchButton.MouseClick += (o, e) =>
            {
                RefreshList();
            };

            SignButton = new DXButton  //位置记忆按钮
            {
                Location = new Point(20, ClientArea.Y + 292),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = @"位置记忆".Lang() },
            };
            SignButton.MouseClick += (o, e) =>
            {
                FixedPointSet();
            };

            MoveButton = new DXButton  //位置移动按钮
            {
                Location = new Point(135, ClientArea.Y + 292),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = @"位置移动".Lang() },
            };
            MoveButton.MouseClick += (o, e) =>
            {
                if (null != ListBox.SelectedItem)
                {
                    CEnvir.Enqueue(new C.cs_FixedPointMove { Uind = ListBox.SelectedItem.Item as FixeUnit });
                }
            };

            ListBox = new DXListBox  //列表框
            {
                Parent = this,
                Location = new Point(ClientArea.Left + 5, ClientArea.Y + 77),
                Size = new Size(ClientArea.Width - 25, ClientArea.Height - 145),
                Border = false,
                Opacity = 0F,
            };
            RefreshList();
            ListBox.selectedItemChanged += (o, e) =>       //所选项目更改
            {
                MoveButton.IsEnabled = (null != ListBox.SelectedItem);
                RightDialog.Visible = false;
            };

            RightDialog = new RightHandDialog   //副对话框
            {
                Parent = this,
                Visible = false,
                Size = new Size(70, DefaultHeight * 2 - 10),
            };
            RightDialog.BringToFront();
        }

        #region Method
        /// <summary>
        /// 可见改变时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);
            if (nValue == true)
            {
                CEnvir.Enqueue(new C.GetFixedPointinfo { });
            }
        }
        /// <summary>
        /// 刷新列表
        /// </summary>
        public void RefreshList()
        {
            TitleLabel.Text = @"位置记忆栏".Lang() + "(" + CEnvir.FixedPointList.Count + "/" + CEnvir.FixedPointTCount + ")";
            ListBox.SelectedItem = null;

            foreach (DXListBoxItem item in ListBoxItems)
                item.Dispose();

            ListBoxItems.Clear();
            if (null != RightDialog) RightDialog.Visible = false;
            foreach (ClientFixedPointInfo info in CEnvir.FixedPointList)
            {
                if (!string.IsNullOrEmpty(SearchTextBox.TextBox.Text) &&
                    (info.Name.IndexOf(SearchTextBox.TextBox.Text, StringComparison.OrdinalIgnoreCase) < 0))
                {
                    continue;
                }
                var item = new DXListBoxItem
                {
                    Parent = ListBox,
                    Label = { Text = info.Name, ForeColour = info.NameColour },
                    Item = info.Uind,
                    isForeColourUpdate = false,
                };
                ListBoxItems.Add(item);
                item.MouseUp += (o, e) =>
                 {
                     if (e.Button == MouseButtons.Right)
                     {
                         RightDialog.Visible = true;
                         RightDialog.Location = new Point(e.X - Location.X, e.Y - Location.Y);
                     }
                 };

            }
        }
        /// <summary>
        /// 搜索
        /// </summary>
        public void ApplySearchFilter()
        {
            TitleLabel.Text = @"搜索".Lang();
            RefreshList();
        }
        /// <summary>
        /// unit 为空的时候表示新增
        /// unit 不为空的时候  isRemove = false 时 表示修改 unit
        /// unit 不为空的时候  isRemove = true  时 表示删除 unit
        /// </summary>
        public void FixedPointSet(FixeUnit unit = null, bool isRemove = false)
        {
            ClientFixedPointInfo info = null;
            if (null != unit)
            {
                foreach (var list in CEnvir.FixedPointList)
                {
                    if (list.Uind == unit)
                    {
                        info = list;
                        break;
                    }
                }
            }
            else
            {
                info = new ClientFixedPointInfo();
                info.NameColour = Color.White;
            }
            if (isRemove)
            {
                int opt = 2;
                CEnvir.Enqueue(new C.cs_FixedPoint { Opt = opt, Info = info });
            }
            else
            {
                DXInputWindow window = new DXInputWindow(null, (value, color) =>
                {
                    if ("" == value)
                    {
                        new DXConfirmWindow("请输入标记的名字".Lang());
                        //MessageBox.Show(@"请输入标记的名字");
                        return;
                    }
                    if (null == unit)
                    {
                        info.Uind = new FixeUnit(GameScene.Game.MapControl.MapInfo.Index, GameScene.Game.User.CurrentLocation);
                    }
                    else
                    {
                        info.Uind = unit;
                    }
                    info.Name = value;
                    info.NameColour = color;
                    int opt = (null == unit ? 0 : 1);
                    CEnvir.Enqueue(new C.cs_FixedPoint { Opt = opt, Info = info });

                }, "请在这里输入要标记的名字".Lang())
                {
                    Value = null == unit ? (GameScene.Game.MapControl.MapInfo.Description + "(" + GameScene.Game.User.CurrentLocation.X + "," + GameScene.Game.User.CurrentLocation.Y + ")") : info.Name,
                    ValueColor = info.NameColour,
                };
            }

        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (SearchButton != null)
                {
                    if (!SearchButton.IsDisposed)
                        SearchButton.Dispose();
                    SearchButton = null;
                }
                if (SignButton != null)
                {
                    if (!SignButton.IsDisposed)
                        SignButton.Dispose();
                    SignButton = null;
                }
                if (MoveButton != null)
                {
                    if (!MoveButton.IsDisposed)
                        MoveButton.Dispose();
                    MoveButton = null;
                }
                if (SearchTextBox != null)
                {
                    if (!SearchTextBox.IsDisposed)
                        SearchTextBox.Dispose();
                    SearchTextBox = null;
                }
                if (ListBox != null)
                {
                    if (!ListBox.IsDisposed)
                        ListBox.Dispose();
                    ListBox = null;
                }
                if (ListBoxItems != null)
                {
                    for (int i = 0; i < ListBoxItems.Count; i++)
                    {
                        if (ListBoxItems[i] != null)
                        {
                            if (!ListBoxItems[i].IsDisposed)
                                ListBoxItems[i].Dispose();

                            ListBoxItems[i] = null;
                        }
                    }

                    ListBoxItems.Clear();
                    ListBoxItems = null;
                }
                if (RightDialog != null)
                {
                    if (!RightDialog.IsDisposed)
                        RightDialog.Dispose();
                    RightDialog = null;
                }
            }
        }
        #endregion
    }
    /// <summary>
    /// 鼠标右键菜单
    /// </summary>
    public sealed class RightHandDialog : DXControl
    {
        #region Properties
        private DXButton AlterButton, RemoveButton;
        private FixedPointDialog fixedPointDialog = null;
        #endregion

        /// <summary>
        /// 鼠标右键菜单
        /// </summary>
        public RightHandDialog()
        {
            new DXLabel
            {
                Parent = this,
                IsControl = false,
                Location = new Point(0, 0),
                ForeColour = Color.White,
            };
            AlterButton = new DXButton
            {
                Label = { Text = @"修改名字".Lang() },
                Location = new Point(0, 0),
                Size = new Size(70, SmallButtonHeight),
                Parent = this,
                ButtonType = ButtonType.SmallButton,
                CanBePressed = false,
                Border = true
            };
            AlterButton.MouseClick += (o, e) =>
            {
                Visible = false;
                var Uind = GetSelectFixeUnit();
                if (null != Uind)
                {
                    if (null != fixedPointDialog)
                    {
                        fixedPointDialog.FixedPointSet(Uind, false);
                    }
                }
            };

            RemoveButton = new DXButton
            {
                Label = { Text = @"删除坐标".Lang() },
                Location = new Point(0, SmallButtonHeight),
                Size = new Size(70, SmallButtonHeight),
                Parent = this,
                ButtonType = ButtonType.SmallButton,
                CanBePressed = false,
                Border = true
            };
            RemoveButton.MouseClick += (o, e) =>
            {
                Visible = false;
                var Uind = GetSelectFixeUnit();
                if (null != Uind)
                {
                    if (null != fixedPointDialog)
                    {
                        fixedPointDialog.FixedPointSet(Uind, true);
                    }
                }
            };

        }
        #region Method
        /// <summary>
        /// 获取选择固定的单元行
        /// </summary>
        /// <returns></returns>
        public FixeUnit GetSelectFixeUnit()
        {
            fixedPointDialog = Parent as FixedPointDialog;
            if (null != fixedPointDialog)
            {
                if (0 != CEnvir.FixedPointList.Count && null != fixedPointDialog.ListBox.SelectedItem && null != fixedPointDialog.ListBox.SelectedItem.Item)
                {
                    foreach (var list in CEnvir.FixedPointList)
                    {
                        if (fixedPointDialog.ListBox.SelectedItem.Item as FixeUnit == list.Uind)
                        {
                            return list.Uind;
                        }
                    }
                }
            }
            return null;
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (AlterButton != null)
                {
                    if (!AlterButton.IsDisposed)
                        AlterButton.Dispose();
                    AlterButton = null;
                }
                if (RemoveButton != null)
                {
                    if (!RemoveButton.IsDisposed)
                        RemoveButton.Dispose();
                    RemoveButton = null;
                }
            }
        }
        #endregion
    }
}
