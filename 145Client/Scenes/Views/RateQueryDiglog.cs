using Client.Controls;
using Client.Extentions;
using Client.UserModels;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 爆率查询
    /// </summary>
    public sealed class RateQueryDiglog : DXWindow
    {
        public DXTextBox ItemNameBox;
        public DXComboBox ItemTypeBox;
        public DXVScrollBar SearchScrollBar;
        public DXButton SearchButton;

        public NewFortuneCheckerRow[] SearchRows;

        public List<ItemInfo> SearchResults;

        public DropInfoTree Tree;

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;


        public RateQueryDiglog()
        {
            TitleLabel.Text = "暴率查询".Lang();
            AllowDragOut = true;           //允许拖动
            Movable = true;                //可以移动

            SetClientSize(new Size(485, 321));

            #region Search

            DXControl filterPanel = new DXControl
            {
                Parent = this,
                Size = new Size(ClientArea.Width, 26),
                Location = new Point(ClientArea.Left, ClientArea.Top),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            DXLabel label = new DXLabel
            {
                Parent = filterPanel,
                Location = new Point(5, 5),
                Text = "名字".Lang(),
            };

            ItemNameBox = new DXTextBox
            {
                Parent = filterPanel,
                Size = new Size(180, 20),
                Location = new Point(label.Location.X + label.Size.Width + 5, label.Location.Y),
            };
            ItemNameBox.TextBox.KeyPress += TextBox_KeyPress;

            label = new DXLabel
            {
                Parent = filterPanel,
                Location = new Point(ItemNameBox.Location.X + ItemNameBox.Size.Width + 10, 5),
                Text = "物品类别".Lang(),
            };

            ItemTypeBox = new DXComboBox
            {
                Parent = filterPanel,
                Location = new Point(label.Location.X + label.Size.Width + 5, label.Location.Y),
                Size = new Size(95, DXComboBox.DefaultNormalHeight),
                DropDownHeight = 198
            };

            new DXListBoxItem
            {
                Parent = ItemTypeBox.ListBox,
                Label = { Text = $"全部".Lang() },
                Item = null
            };

            Type itemType = typeof(ItemType);

            for (ItemType i = ItemType.Nothing; i <= ItemType.ItemPart; i++)
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

            SearchButton = new DXButton
            {
                Size = new Size(80, SmallButtonHeight),
                Location = new Point(ItemTypeBox.Location.X + ItemTypeBox.Size.Width + 15, label.Location.Y - 1),
                Parent = filterPanel,
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "搜索".Lang() }
            };
            SearchButton.MouseClick += (o, e) => Search();

            DXControl ItemPanel = new DXControl
            {
                Parent = this,
                Size = new Size(258, 290),
                Location = new Point(ClientArea.Left, ClientArea.Top + 30),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            SearchRows = new NewFortuneCheckerRow[5];

            SearchScrollBar = new DXVScrollBar
            {
                Parent = ItemPanel,
                Location = new Point(248 - 7, 1),
                Size = new Size(14, ClientArea.Height - 5 - filterPanel.Size.Height),
                VisibleSize = SearchRows.Length,
                Change = 3,
            };
            SearchScrollBar.ValueChanged += SearchScrollBar_ValueChanged;

            for (int i = 0; i < SearchRows.Length; i++)
            {
                int index = i;
                SearchRows[index] = new NewFortuneCheckerRow
                {
                    Parent = ItemPanel,
                    Location = new Point(1, 1 + i * 58),
                };
                SearchRows[index].MouseClick += (o, e) =>
                {   //这里增加判断函数，处理选中的物品信息
                    RefreshList();
                    SearchRows[index].Selected = true;
                    Tree.CurItem = SearchRows[index].ItemInfo;
                    Tree.ListChanged();
                };
                SearchRows[index].MouseWheel += SearchScrollBar.DoMouseWheel;
            }

            DXControl DropInfoPanel = new DXControl
            {
                Parent = this,
                Size = new Size(220, 290),
                Location = new Point(ClientArea.Left + ItemPanel.Size.Width + 5, ClientArea.Top + 30),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            Tree = new DropInfoTree
            {
                Parent = DropInfoPanel,
                Location = new Point(1, 1),
                Size = new Size(215, 280),
            };

            #endregion

        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            e.Handled = true;

            if (SearchButton.Enabled)
                Search();
        }

        public void Search()
        {
            SearchResults = new List<ItemInfo>();

            SearchScrollBar.MaxValue = 0;

            foreach (var row in SearchRows)
                row.Visible = true;

            ItemType filter = (ItemType?)ItemTypeBox.SelectedItem ?? 0;
            bool useFilter = ItemTypeBox.SelectedItem != null;

            foreach (ItemInfo info in Globals.ItemInfoList.Binding)
            {
                if (info.Drops.Count == 0) continue;

                if (useFilter && info.ItemType != filter) continue;

                if (!string.IsNullOrEmpty(ItemNameBox.TextBox.Text) && info.ItemName.IndexOf(ItemNameBox.TextBox.Text, StringComparison.OrdinalIgnoreCase) < 0) continue;

                SearchResults.Add(info);
            }

            RefreshList();
        }

        public void RefreshList()
        {
            if (SearchResults == null) return;

            SearchScrollBar.MaxValue = SearchResults.Count;

            for (int i = 0; i < SearchRows.Length; i++)
            {
                if (i + SearchScrollBar.Value >= SearchResults.Count)
                {
                    SearchRows[i].ItemInfo = null;
                    SearchRows[i].Visible = false;
                    continue;
                }

                SearchRows[i].ItemInfo = SearchResults[i + SearchScrollBar.Value];
                SearchRows[i].Selected = false;
            }
        }

        private void SearchScrollBar_ValueChanged(object sender, EventArgs e)
        {
            RefreshList();
        }
    }

    public sealed class NewFortuneCheckerRow : DXControl
    {
        #region Properties

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
        public event EventHandler<EventArgs> SelectedChanged;
        public void OnSelectedChanged(bool oValue, bool nValue)
        {
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);
            ItemCell.BorderColour = Selected ? Color.FromArgb(198, 166, 99) : Color.FromArgb(74, 56, 41);

            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region ItemInfo

        public ItemInfo ItemInfo
        {
            get { return _ItemInfo; }
            set
            {
                ItemInfo oldValue = _ItemInfo;
                _ItemInfo = value;

                OnItemInfoChanged(oldValue, value);
            }
        }
        private ItemInfo _ItemInfo;
        public event EventHandler<EventArgs> ItemInfoChanged;
        public void OnItemInfoChanged(ItemInfo oValue, ItemInfo nValue)
        {
            ItemInfoChanged?.Invoke(this, EventArgs.Empty);
            Visible = ItemInfo != null;
            Fortune = null;

            if (ItemInfo == null)
            {
                return;
            }

            ItemCell.Item = new ClientUserItem(ItemInfo, 1);
            ItemCell.RefreshItem();

            NameLabel.Text = ItemInfo.Lang(p => p.ItemName);

            NameLabel.ForeColour = Color.FromArgb(198, 166, 99);

            GameScene.Game.FortuneDictionary.TryGetValue(ItemInfo, out Fortune);

            UpdateInfo();

            ItemInfoChanged?.Invoke(this, EventArgs.Empty);
        }
        private void UpdateInfo()
        {
            if (Fortune == null)
            {
                return;
            }
        }

        #endregion

        private ClientFortuneInfo Fortune;

        public DXItemCell ItemCell;
        public DXLabel NameLabel;
        #endregion

        public NewFortuneCheckerRow()
        {
            Size = new Size(236, 55);

            DrawTexture = true;
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);

            Visible = false;

            ItemCell = new DXItemCell
            {
                Parent = this,
                Location = new Point((Size.Height - DXItemCell.CellHeight) / 2, (Size.Height - DXItemCell.CellHeight) / 2),
                FixedBorder = true,
                Border = true,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                FixedBorderColour = true,
            };

            NameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(ItemCell.Location.X + ItemCell.Size.Width, 22),
                IsControl = false,
            };
        }

        public override void Process()
        {
            base.Process();

            if (Fortune == null) return;
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Selected = false;
                SelectedChanged = null;

                _ItemInfo = null;
                ItemInfoChanged = null;

                if (ItemCell != null)
                {
                    if (!ItemCell.IsDisposed)
                        ItemCell.Dispose();

                    ItemCell = null;
                }

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }
            }
        }

        #endregion
    }

    public class DropInfoTree : DXControl
    {
        #region Properties

        public ItemInfo CurItem;
        private DXVScrollBar ScrollBar;

        public List<DXControl> Lines = new List<DXControl>();

        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            ScrollBar.Size = new Size(14, Size.Height);
            ScrollBar.Location = new Point(Size.Width - 14, 0);
            ScrollBar.VisibleSize = Size.Height;
        }
        #endregion

        public DropInfoTree()
        {
            Border = true;
            BorderColour = Color.FromArgb(198, 166, 99);

            ScrollBar = new DXVScrollBar
            {
                Parent = this,
                Change = 22,
            };
            ScrollBar.ValueChanged += (o, e) => UpdateScrollBar();

            MouseWheel += ScrollBar.DoMouseWheel;
        }

        #region Methods
        public void UpdateScrollBar()
        {
            ScrollBar.MaxValue = Lines.Count * 22;

            for (int i = 0; i < Lines.Count; i++)
                Lines[i].Location = new Point(Lines[i].Location.X, i * 22 - ScrollBar.Value);
        }

        public void ListChanged()
        {
            List<string> MonList = new List<string>();
            List<string> MapList = new List<string>();
            if (CurItem == null) return;
            ScrollBar.Value = 0;
            foreach (DXControl control in Lines)
                control.Dispose();

            Lines.Clear();
            DropInfoTreeHeader Itemheader = new DropInfoTreeHeader
            {
                Parent = this,
                Location = new Point(1, Lines.Count * 22),
                Size = new Size(Size.Width - 17, 20),
                TitleName = "物品信息".Lang(),
            };
            Lines.Add(Itemheader);

            DropInfoTreeEntry ItemName = new DropInfoTreeEntry
            {
                Parent = this,
                Location = new Point(1, Lines.Count * 22),
                Size = new Size(Size.Width - 17, 20),
                Name = CurItem.Lang(p => p.ItemName),
                StrColor = Color.Cyan,
            };
            Lines.Add(ItemName);

            DropInfoTreeHeader Monheader = new DropInfoTreeHeader
            {
                Parent = this,
                Location = new Point(1, Lines.Count * 22),
                Size = new Size(Size.Width - 17, 20),
                TitleName = "怪物信息".Lang(),
            };
            Lines.Add(Monheader);

            for (int i = 0; i < CurItem.Drops.Count; i++)
            {
                if (!MonList.Contains(CurItem.Drops[i].Monster.MonsterName))
                {
                    MonList.Add(CurItem.Drops[i].Monster.MonsterName);

                    DropInfoTreeEntry DropMon = new DropInfoTreeEntry
                    {
                        Parent = this,
                        Location = new Point(1, Lines.Count * 22),
                        Size = new Size(Size.Width - 17, 20),
                        Name = CurItem.Drops[i].Monster.MonsterName,
                        StrColor = Color.White,
                        IsBoss = CurItem.Drops[i].Monster.IsBoss,
                    };
                    for (int j = 0; j < CurItem.Drops[i].Monster.Respawns.Count; j++)
                    {
                        var map = CurItem.Drops[i].Monster.Respawns[j].Region != null ? CurItem.Drops[i].Monster.Respawns[j].Region.Map : Globals.MapInfoList.Binding.First(p => p.Index == CurItem.Drops[i].Monster.Respawns[j].MapID);
                        if (!MapList.Contains(map.Description))
                        {
                            MapList.Add(map.Lang(p => p.Description));
                        }
                    }
                    Lines.Add(DropMon);
                }
            }

            DropInfoTreeHeader Mapheader = new DropInfoTreeHeader
            {
                Parent = this,
                Location = new Point(1, Lines.Count * 22),
                Size = new Size(Size.Width - 17, 20),
                TitleName = "地图信息".Lang(),
            };
            Lines.Add(Mapheader);

            for (int i = 0; i < MapList.Count; i++)
            {
                DropInfoTreeEntry MonMap = new DropInfoTreeEntry
                {
                    Parent = this,
                    Location = new Point(1, Lines.Count * 22),
                    Size = new Size(Size.Width - 17, 20),
                    Name = MapList[i],
                    StrColor = Color.White,
                };
                Lines.Add(MonMap);
            }
            UpdateScrollBar();
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                if (Lines != null)
                {
                    for (int i = 0; i < Lines.Count; i++)
                    {
                        if (Lines[i] != null)
                        {
                            if (!Lines[i].IsDisposed)
                                Lines[i].Dispose();

                            Lines[i] = null;
                        }

                    }

                    Lines.Clear();
                    Lines = null;
                }
            }
        }

        #endregion
    }

    public sealed class DropInfoTreeHeader : DXControl
    {
        #region Properties    

        #region TitleName

        public string TitleName
        {
            get => _TitleName;
            set
            {
                if (_TitleName == value) return;

                string oldValue = _TitleName;
                _TitleName = value;

                OnMapChanged(oldValue, value);
            }
        }
        private string _TitleName;

        public void OnMapChanged(string oValue, string nValue)
        {
            NameLabel.Text = "▼" + nValue;
        }

        #endregion

        public DXLabel NameLabel;
        #endregion

        public DropInfoTreeHeader()
        {
            NameLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.RoyalBlue,
                IsControl = false,
                Location = new Point(2, 2)
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }
            }
        }

        #endregion
    }

    public sealed class DropInfoTreeEntry : DXControl
    {
        #region Properties

        #region Name

        public string Name
        {
            get => _Name;
            set
            {
                if (_Name == value) return;

                string oldValue = _Name;
                _Name = value;

                OnNameChanged(oldValue, value);
            }
        }
        private string _Name;

        public void OnNameChanged(string oValue, string nValue)
        {
            NameLabel.Text = nValue;
        }

        #endregion

        #region Color

        public Color StrColor
        {
            get => _StrColor;
            set
            {
                if (_StrColor == value) return;

                Color oldValue = _StrColor;
                _StrColor = value;

                OnStrColorChanged(oldValue, value);
            }
        }
        private Color _StrColor;

        public void OnStrColorChanged(Color oValue, Color nValue)
        {
            NameLabel.ForeColour = nValue;
        }

        #endregion

        #region IsBoss

        public bool IsBoss
        {
            get => _IsBoss;
            set
            {
                if (_IsBoss == value) return;

                bool oldValue = _IsBoss;
                _IsBoss = value;

                OnIsBossChanged(oldValue, value);
            }
        }
        private bool _IsBoss;

        public void OnIsBossChanged(bool oValue, bool nValue)
        {
            NameLabel.ForeColour = nValue ? Color.Red : Color.White;
        }

        #endregion

        public DXLabel NameLabel;

        #endregion

        public DropInfoTreeEntry()
        {
            NameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(22, 2),
                IsControl = false,
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

            }

        }

        #endregion
    }
}
