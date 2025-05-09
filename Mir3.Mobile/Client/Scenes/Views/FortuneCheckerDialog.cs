﻿using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.UserModels;
using Library;
using Library.SystemModels;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 财富查询功能
    /// </summary>
    public sealed class FortuneCheckerDialog : DXWindow
    {
        public DXTextBox ItemNameBox;
        public DXComboBox ItemTypeBox;
        public DXVScrollBar SearchScrollBar;
        public DXButton SearchButton;

        public FortuneCheckerRow[] SearchRows;   //财富查询 行

        public List<ItemInfo> SearchResults;  //列表<项目信息>搜索结果

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        /// <summary>
        /// 财富查询界面
        /// </summary>
        public FortuneCheckerDialog()
        {
            //HasFooter = true;
            TitleLabel.Text = "财富查询".Lang();
            SetClientSize(new Size(485, 551));
            IgnoreMoveBounds = true;

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
                Text = "名称".Lang(),
            };

            ItemNameBox = new DXTextBox   //道具名字
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
                Text = "物品".Lang(),
            };

            ItemTypeBox = new DXComboBox   //道具类别
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

            SearchRows = new FortuneCheckerRow[9];

            SearchScrollBar = new DXVScrollBar
            {
                Parent = this,
                Location = new Point(ClientArea.Size.Width - 14 + ClientArea.Left, ClientArea.Y + filterPanel.Size.Height + 5),
                Size = new Size(14, ClientArea.Height - 5 - filterPanel.Size.Height),
                VisibleSize = SearchRows.Length,
                Change = 3,
            };
            SearchScrollBar.ValueChanged += SearchScrollBar_ValueChanged;

            for (int i = 0; i < SearchRows.Length; i++)
            {
                int index = i;
                SearchRows[index] = new FortuneCheckerRow
                {
                    Parent = this,
                    Location = new Point(ClientArea.X, ClientArea.Y + filterPanel.Size.Height + 5 + i * 58),
                };
                //   SearchRows[index].MouseClick += (o, e) => { SelectedRow = SearchRows[index]; };
                SearchRows[index].MouseWheel += SearchScrollBar.DoMouseWheel;
            }
            #endregion
        }
        /// <summary>
        /// 文本输入按键过程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            e.Handled = true;

            if (SearchButton.Enabled)
                Search();
        }
        /// <summary>
        /// 搜索
        /// </summary>
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
        /// <summary>
        /// 刷新列表
        /// </summary>
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
            }
        }
        /// <summary>
        /// 搜索滚动条变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchScrollBar_ValueChanged(object sender, EventArgs e)
        {
            RefreshList();
        }
    }

    /// <summary>
    /// 财富检查行
    /// </summary>
    public sealed class FortuneCheckerRow : DXControl
    {
        #region Properties

        #region Selected
        /// <summary>
        /// 选定
        /// </summary>
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
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);   //背景颜色
            ItemCell.BorderColour = Selected ? Color.FromArgb(198, 166, 99) : Color.FromArgb(74, 56, 41);  //项目单元格 边框颜色

            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region ItemInfo
        /// <summary>
        /// 道具信息
        /// </summary>
        public ItemInfo ItemInfo
        {
            get { return _ItemInfo; }
            set
            {

                ItemInfo oldValue = _ItemInfo;
                _ItemInfo = value;

                OnItemInfoChanged(oldValue, value);     //道具信息更改时
            }
        }
        private ItemInfo _ItemInfo;
        public event EventHandler<EventArgs> ItemInfoChanged;
        public void OnItemInfoChanged(ItemInfo oValue, ItemInfo nValue)
        {
            ItemInfoChanged?.Invoke(this, EventArgs.Empty);  //道具信息更改
            Visible = ItemInfo != null;    //可见
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
        /// <summary>
        /// 更新信息
        /// </summary>
        private void UpdateInfo()
        {
            if (Fortune == null)
            {
                CountLabel.Text = "未查询".Lang();
                ProgressLabel.Text = "未查询".Lang();
                DateLabel.Text = "未查询".Lang();
                return;
            }

            CountLabel.Text = Fortune.DropCount.ToString("#,##0");

            string format = "#,##0";

            if (Fortune.Progress < 10000)
                format += ".#####%";
            else
                format += ".##%";

            ProgressLabel.Text = (1 + Fortune.DropCount - Fortune.Progress).ToString(format);
            DateLabel.Text = (CEnvir.Now - Fortune.CheckDate).Lang(true);
        }

        #endregion

        private ClientFortuneInfo Fortune;

        public DXItemCell ItemCell;
        public DXLabel NameLabel, CountLabelLabel, CountLabel, ProgressLabelLabel, ProgressLabel, DateLabel, TogoLabel, DateLabelLabel;
        public DXButton CheckButton;
        #endregion


        /// <summary>
        /// 财富检查行
        /// </summary>
        public FortuneCheckerRow()
        {
            Size = new Size(465, 55);

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

            CountLabelLabel = new DXLabel
            {
                Parent = this,
                Text = "掉落数量".Lang(),
                ForeColour = Color.White,
                IsControl = false,

            };
            CountLabelLabel.Location = new Point(320 - CountLabelLabel.Size.Width, 5);

            CountLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(320, 5),
                IsControl = false,
            };

            ProgressLabelLabel = new DXLabel
            {
                Parent = this,
                Text = "物品爆率".Lang(),
                ForeColour = Color.White,
                IsControl = false,

            };
            ProgressLabelLabel.Location = new Point(320 - ProgressLabelLabel.Size.Width, 20);

            ProgressLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(320, 20),
                IsControl = false,
            };

            DateLabelLabel = new DXLabel
            {
                Parent = this,
                Text = "查询时间".Lang(),
                ForeColour = Color.White,
                IsControl = false,

            };
            DateLabelLabel.Location = new Point(320 - DateLabelLabel.Size.Width, 35);

            DateLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(320, 35),
                IsControl = false,
            };

            CheckButton = new DXButton
            {
                Parent = this,
                Label = { Text = "查询".Lang() },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(50, SmallButtonHeight),
                Location = new Point(Size.Width - 55, 34)

            };

            CheckButton.MouseClick += CheckButton_MouseClick;
        }
        /// <summary>
        /// 鼠标点击查询时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (GameScene.Game.Observer) return;

            new DXConfirmWindow("你确定要查询你的财富记录吗".Lang(), DXMessageBoxButtons.YesNo, () =>
            {
                CEnvir.Enqueue(new C.FortuneCheck { ItemIndex = ItemInfo.Index });
            });
        }
        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            if (Fortune == null) return;

            DateLabel.Text = (CEnvir.Now - Fortune.CheckDate).Lang(true);
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

                if (CountLabelLabel != null)
                {
                    if (!CountLabelLabel.IsDisposed)
                        CountLabelLabel.Dispose();

                    CountLabelLabel = null;
                }

                if (CountLabel != null)
                {
                    if (!CountLabel.IsDisposed)
                        CountLabel.Dispose();

                    CountLabel = null;
                }

                if (ProgressLabelLabel != null)
                {
                    if (!ProgressLabelLabel.IsDisposed)
                        ProgressLabelLabel.Dispose();

                    ProgressLabelLabel = null;
                }

                if (ProgressLabel != null)
                {
                    if (!ProgressLabel.IsDisposed)
                        ProgressLabel.Dispose();

                    ProgressLabel = null;
                }
            }
        }
        #endregion
    }
}
