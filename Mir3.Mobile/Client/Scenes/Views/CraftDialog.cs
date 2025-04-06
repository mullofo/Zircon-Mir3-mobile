using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.UserModels;
using Library;
using Library.SystemModels;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C = Library.Network.ClientPackets;
using Font = MonoGame.Extended.Font;
using FontStyle = MonoGame.Extended.FontStyle;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 制作功能
    /// </summary>
    public sealed class CraftDialog : DXWindow
    {
        #region Properites
        private DXImageControl CraftBackground;   //底图
        private DXButton WeaponTab, ArmourTab, JewelryTab, ConsumableTab, MaterialTab, Blueprint1, Blueprint2, Blueprint3;
        private DXButton PreviousPageButton, NextPageButton, BookmarkButton, MakeButton;
        private DXLabel PageNumber; // 页码
        private DXLabel TimeCostLabel; // 需要的时间
        private int CurrentPage; // 当前页面

        private DXCheckBox ShowMakeableOnlyBox; //只显示可制作物品 勾选框
        //private IList<CraftItemInfo> AllMakeableItems = Globals.CraftItemInfoList.Binding; //所有可制作的物品信息

        //可制作武器类别
        private readonly IList<CraftItemInfo> MakeableWeaponList = Globals.CraftItemInfoList.Binding.Where(x => (x.TargetItemType == ItemType.Weapon)
                                                                                                             && x.Blueprint == 1).OrderBy(y => y.SortNumber).ToList();

        //可制作衣服类别
        private readonly IList<CraftItemInfo> MakeableArmourList = Globals.CraftItemInfoList.Binding.Where(x => (x.TargetItemType == ItemType.Armour ||
                                                                                                                 x.TargetItemType == ItemType.Helmet ||
                                                                                                                 x.TargetItemType == ItemType.Shoes ||
                                                                                                                 x.TargetItemType == ItemType.Belt ||
                                                                                                                 x.TargetItemType == ItemType.Shield ||
                                                                                                                 x.TargetItemType == ItemType.Fashion)
                                                                                                              && x.Blueprint == 1).OrderBy(y => y.SortNumber).ToList();

        //可制作首饰类别
        private readonly IList<CraftItemInfo> MakeableJewelryList = Globals.CraftItemInfoList.Binding.Where(x => (x.TargetItemType == ItemType.Necklace ||
                                                                                                                  x.TargetItemType == ItemType.Bracelet ||
                                                                                                                  x.TargetItemType == ItemType.Ring)
                                                                                                               && x.Blueprint == 1).OrderBy(y => y.SortNumber).ToList();

        //可制作消耗品类别
        private readonly IList<CraftItemInfo> MakeableConsumableList = Globals.CraftItemInfoList.Binding.Where(x => x.TargetItemType == ItemType.Consumable ||
                                                                                                                    x.TargetItemType == ItemType.Torch ||
                                                                                                                    x.TargetItemType == ItemType.Poison ||
                                                                                                                    x.TargetItemType == ItemType.Amulet ||
                                                                                                                    x.TargetItemType == ItemType.Scroll
                                                                                                                 && x.Blueprint == 1).OrderBy(y => y.SortNumber).ToList();

        //可制作其他类别
        private readonly IList<CraftItemInfo> MakeableMaterialList = Globals.CraftItemInfoList.Binding.Where(x => (x.TargetItemType == ItemType.Nothing ||
                                                                                                                   x.TargetItemType == ItemType.Emblem ||
                                                                                                                   x.TargetItemType == ItemType.DarkStone ||
                                                                                                                   x.TargetItemType == ItemType.Wing ||
                                                                                                                   x.TargetItemType == ItemType.Gem ||
                                                                                                                   x.TargetItemType == ItemType.Material ||
                                                                                                                   x.TargetItemType == ItemType.HorseArmour ||
                                                                                                                   x.TargetItemType == ItemType.Flower ||
                                                                                                                   x.TargetItemType == ItemType.Book ||
                                                                                                                   x.TargetItemType == ItemType.Drill ||
                                                                                                                   x.TargetItemType == ItemType.FameTitle ||
                                                                                                                   x.TargetItemType == ItemType.Medicament
                                                                                                                   )
                                                                                                               && x.Blueprint == 1).OrderBy(y => y.SortNumber).ToList();

        public IList<CraftItemInfo> RowsWithBlueprint2, RowsWithBlueprint3;

        private DXButton ActiveButton;
        private IList<CraftRow> ActiveRows;
        private IList<CraftItemInfo> ActiveLists;
        /// <summary>
        /// 选定行
        /// </summary>
        public CraftRow SelectedRow
        {
            get => _SelectedRow;
            set
            {
                if (_SelectedRow == value) return;

                CraftRow oldValue = _SelectedRow;
                _SelectedRow = value;

                OnSelectedRowChanged(oldValue, value);
            }
        }
        private CraftRow _SelectedRow;
        public event EventHandler<EventArgs> SelectedRowChanged;
        /// <summary>
        /// 选定制作道具信息
        /// </summary>
        public CraftItemInfo SelectedCraftItemInfo
        {
            get => _SelectedCraftItemInfo;
            set
            {
                if (_SelectedCraftItemInfo == value) return;

                CraftItemInfo oldValue = _SelectedCraftItemInfo;
                _SelectedCraftItemInfo = value;

                OnSelectedCraftItemInfoChanged(oldValue, value);
            }
        }
        private CraftItemInfo _SelectedCraftItemInfo;
        public event EventHandler<EventArgs> SelectedCraftItemInfoChanged;

        private CraftTargetDetail TargetDetail1, TargetDetail2, TargetDetail3;
        public CraftIngredientDetail IngredientDetail1, IngredientDetail2, IngredientDetail3;


        public override WindowType Type => WindowType.CraftBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// 制作功能面板
        /// </summary>
        public CraftDialog()
        {
            HasTitle = false;                   //不显示标题
            HasFooter = false;                  //不显示页脚
            HasTopBorder = false;               //不显示边框
            TitleLabel.Visible = false;         //标题标签不用
            CloseButton.Visible = false;        //关闭按钮不用
            IgnoreMoveBounds = true;
            Opacity = 0F;
            SetClientSize(new Size(486, 492));

            CurrentPage = 1;

            //图纸2和图纸3的row
            RowsWithBlueprint2 = Globals.CraftItemInfoList.Binding.Where(x => x.Blueprint == 2).OrderBy(y => y.SortNumber).ToList();
            RowsWithBlueprint3 = Globals.CraftItemInfoList.Binding.Where(x => x.Blueprint == 3).OrderBy(y => y.SortNumber).ToList();

            #region 按钮
            CraftBackground = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 3701,
                Opacity = 0.7F,
                Location = new Point(0, 0),
                IsControl = true,
                PassThrough = true,
            };

            CloseButton = new DXButton       //关闭按钮
            {
                Parent = this,
                Index = 113,
                LibraryFile = LibraryFile.Interface,
                Hint = "关闭".Lang(),
            };
            CloseButton.MouseEnter += (o, e) => CloseButton.Index = 112;
            CloseButton.MouseLeave += (o, e) => CloseButton.Index = 113;
            CloseButton.MouseClick += (o, e) => Visible = false;

            WeaponTab = new DXButton
            {
                Index = 3710,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(10, 40),
                Parent = this,
                Opacity = 1,
                Text = "武器".Lang(),
            };
            ActiveButton = WeaponTab;
            WeaponTab.MouseClick += SelectedTabChanged;

            ArmourTab = new DXButton
            {
                Index = 3711,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(80, 40),
                Parent = this,
                Opacity = 0,
                Text = "衣服".Lang(),
            };
            ArmourTab.MouseClick += SelectedTabChanged;

            JewelryTab = new DXButton
            {
                Index = 3712,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(149, 40),
                Parent = this,
                Opacity = 0,
                Text = "首饰".Lang(),
            };
            JewelryTab.MouseClick += SelectedTabChanged;

            ConsumableTab = new DXButton
            {
                Index = 3713,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(218, 40),
                Parent = this,
                Opacity = 0,
                Text = "消耗品".Lang(),
            };
            ConsumableTab.MouseClick += SelectedTabChanged;

            MaterialTab = new DXButton
            {
                Index = 3715,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(287, 40),
                Parent = this,
                Opacity = 0,
                Text = "其他".Lang(),
            };
            MaterialTab.MouseClick += SelectedTabChanged;

            ShowMakeableOnlyBox = new DXCheckBox
            {
                Parent = this,
                ReadOnly = true,
                Location = new Point(12, 95),
            };
            ShowMakeableOnlyBox.MouseClick += (o, e) => { ShowMakeableOnlyBox.Checked = !ShowMakeableOnlyBox.Checked; };

            Blueprint1 = new DXButton
            {
                Index = 3720,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(230, 224),
                Parent = this,
            };
            Blueprint1.MouseClick += (o, e) =>
            {
                if (SelectedCraftItemInfo == null) return;
                Blueprint1.Opacity = 1;
                Blueprint2.Opacity = 0.5f;
                Blueprint3.Opacity = 0.5f;

                if (TargetDetail1 != null) TargetDetail1.Visible = true;
                if (TargetDetail2 != null) TargetDetail2.Visible = false;
                if (TargetDetail3 != null) TargetDetail3.Visible = false;

                IngredientDetail1.Visible = true;
                SelectedCraftItemInfo = IngredientDetail1.CraftItem;
                if (IngredientDetail2 != null) IngredientDetail2.Visible = false;
                if (IngredientDetail3 != null) IngredientDetail3.Visible = false;
            };

            Blueprint2 = new DXButton
            {
                Index = 3721,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(272, 224),
                Parent = this,
                Opacity = 0.5f,
            };
            Blueprint2.MouseClick += (o, e) =>
            {
                if (SelectedCraftItemInfo == null) return;
                Blueprint1.Opacity = 0;
                Blueprint2.Opacity = 1;
                Blueprint3.Opacity = 0.5f;

                if (TargetDetail1 != null) TargetDetail1.Visible = false;
                if (TargetDetail2 != null) TargetDetail2.Visible = true;
                if (TargetDetail3 != null) TargetDetail3.Visible = false;

                IngredientDetail1.Visible = false;
                if (IngredientDetail2 != null)
                {
                    IngredientDetail2.Visible = true;
                    SelectedCraftItemInfo = IngredientDetail2.CraftItem;
                }
                if (IngredientDetail3 != null) IngredientDetail3.Visible = false;
            };

            Blueprint3 = new DXButton
            {
                Index = 3722,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(312, 224),
                Parent = this,
                Opacity = 0.5f,
            };
            Blueprint3.MouseClick += (o, e) =>
            {
                if (SelectedCraftItemInfo == null) return;
                Blueprint1.Opacity = 0;
                Blueprint2.Opacity = 0.5f;
                Blueprint3.Opacity = 1;

                if (TargetDetail1 != null) TargetDetail1.Visible = false;
                if (TargetDetail2 != null) TargetDetail2.Visible = false;
                if (TargetDetail3 != null) TargetDetail3.Visible = true;

                IngredientDetail1.Visible = false;
                if (IngredientDetail2 != null) IngredientDetail2.Visible = false;
                if (IngredientDetail3 != null)
                {
                    IngredientDetail3.Visible = true;
                    SelectedCraftItemInfo = IngredientDetail3.CraftItem;
                }
            };

            PreviousPageButton = new DXButton
            {
                Index = 3730,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(40, 470),
                IsControl = true,
                Parent = this,
            };
            PreviousPageButton.MouseClick += (o, e) =>
            {
                TogglePageVisible(ActiveLists, CurrentPage - 1);
            };

            NextPageButton = new DXButton
            {
                Index = 3732,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(170, 470),
                IsControl = true,
                PassThrough = false,
                Parent = this,
            };
            NextPageButton.MouseClick += (o, e) =>
            {
                TogglePageVisible(ActiveLists, CurrentPage + 1);
            };

            BookmarkButton = new DXButton
            {
                Index = 3767,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(305, 465),
                PassThrough = false,
                Parent = this,
            };
            BookmarkButton.MouseClick += (o, e) =>
            {
                if (SelectedCraftItemInfo != null && GameScene.Game.CharacterBox != null)
                {
                    GameScene.Game.User.BookmarkedCraftItemInfo = SelectedCraftItemInfo;
                    GameScene.Game.CharacterBox.UpdateBookmarkedCraftItem();
                    GameScene.Game.CraftResultBox.UpdateCraftTarget(SelectedCraftItemInfo);
                }
            };

            MakeButton = new DXButton   //制作按钮
            {
                Index = 3762,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(415, 465),
                Parent = this,
                Enabled = false,
            };
            MakeButton.MouseClick += (sender, args) =>
            {
                GameScene.Game.CraftResultBox.UpdateCraftTarget(SelectedCraftItemInfo);
                GameScene.Game.CraftResultBox.Visible = true;
                CEnvir.Enqueue(new C.CraftStart { TargetItemIndex = SelectedCraftItemInfo.Index });
            };

            TimeCostLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(375, 225),
                ForeColour = Color.DeepSkyBlue,
                IsControl = false,
                Text = "消耗时间".Lang() + ": 00:00:00",
                AutoSize = true,
                Visible = true,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
            };

            #endregion

            //初始化页码
            PageNumber = new DXLabel
            {
                Parent = this,
                Location = new Point(96, 468),
                ForeColour = Color.White,
                IsControl = false,
                Text = "1 / 1",
                Font = new Font("SimSun", CEnvir.FontSize(11F), FontStyle.Regular),
                AutoSize = true,
                Visible = true,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
            };

            //默认选择武器页
            SelectedTabChanged(WeaponTab, EventArgs.Empty);
        }
        /// <summary>
        /// 选定的行改变时
        /// </summary>
        /// <param name="prevRow"></param>
        /// <param name="currRow"></param>
        private void OnSelectedRowChanged(CraftRow prevRow, CraftRow currRow)
        {
            //旧的row变暗
            if (prevRow?.CraftItem != null)
                prevRow.BackgroundImage.Index = 3740;
            //选中的row高亮
            if (currRow != null)
                currRow.BackgroundImage.Index = 3741;
            //更新右侧合成信息
            TargetDetail1?.Dispose();
            TargetDetail1 = new CraftTargetDetail(currRow.CraftItem)
            {
                Parent = this,
                Visible = true,
                ForeColour = currRow.CraftItem.SuccessRate > 50 ? Color.LawnGreen : Color.Red
            };

            IngredientDetail1?.Dispose();
            IngredientDetail1 = new CraftIngredientDetail(currRow.CraftItem)
            {
                Parent = this,
                Visible = true,
            };
            IngredientDetail1.AfterDraw += SelectedCraftItemInfoChanged;

            #region 图纸2图纸3
            CraftItemInfo b2 = RowsWithBlueprint2.FirstOrDefault(x => x.Item == currRow.CraftItem.Item);
            CraftItemInfo b3 = RowsWithBlueprint3.FirstOrDefault(x => x.Item == currRow.CraftItem.Item);

            IngredientDetail2?.Dispose();
            TargetDetail2?.Dispose();
            if (b2 != null)
            {
                TargetDetail2 = new CraftTargetDetail(b2)
                {
                    Parent = this,
                    Visible = false,
                    ForeColour = b2.SuccessRate > 50 ? Color.LawnGreen : Color.Red
                };

                IngredientDetail2 = new CraftIngredientDetail(b2)
                {
                    Parent = this,
                    Visible = false,
                };
            }

            IngredientDetail3?.Dispose();
            TargetDetail3?.Dispose();
            if (b3 != null)
            {
                TargetDetail3 = new CraftTargetDetail(b3)
                {
                    Parent = this,
                    Visible = false,
                    ForeColour = b3.SuccessRate > 50 ? Color.LawnGreen : Color.Red
                };

                IngredientDetail3 = new CraftIngredientDetail(b3)
                {
                    Parent = this,
                    Visible = false,
                };
            }
            #endregion

            SelectedCraftItemInfo = currRow.CraftItem;
            SelectedRowChanged?.Invoke(this, EventArgs.Empty);
            Blueprint1.Opacity = 1;
            Blueprint2.Opacity = 0.5f;
            Blueprint3.Opacity = 0.5f;
        }
        /// <summary>
        /// 切换制作按钮
        /// </summary>
        /// <param name="info"></param>
        private void ToggleMakeButton(CraftItemInfo info)
        {
            //如果包裹里的物品足够, 使制作按钮可用
            if ((info.Item1 != null && GameScene.Game.CountItemInventory(info.Item1) < info.Amount1) ||
                (info.Item2 != null && GameScene.Game.CountItemInventory(info.Item2) < info.Amount2) ||
                (info.Item3 != null && GameScene.Game.CountItemInventory(info.Item3) < info.Amount3) ||
                (info.Item4 != null && GameScene.Game.CountItemInventory(info.Item4) < info.Amount4) ||
                (info.Item5 != null && GameScene.Game.CountItemInventory(info.Item5) < info.Amount5))
            {
                MakeButton.Index = 3763;
            }
            else
            {
                MakeButton.Index = 3762;
                MakeButton.Enabled = true;
            }
        }
        /// <summary>
        /// 更新制作道具信息
        /// </summary>
        public void UpdateCraftItemInfo()
        {
            if (SelectedCraftItemInfo == null) return;
            OnSelectedCraftItemInfoChanged(null, SelectedCraftItemInfo);
        }
        /// <summary>
        /// 选定的物品信息改变时
        /// </summary>
        /// <param name="prevInfo"></param>
        /// <param name="currInfo"></param>
        private void OnSelectedCraftItemInfoChanged(CraftItemInfo prevInfo, CraftItemInfo currInfo)
        {
            ToggleMakeButton(currInfo);
            TimeCostLabel.Text = "消耗时间" + ": " + TimeSpan.FromSeconds(currInfo.TimeCost).ToString(@"hh\:mm\:ss");
            SelectedCraftItemInfoChanged?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// 切换页面显示
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        private void TogglePageVisible(IList<CraftItemInfo> rows, int page)
        {
            if (page < 1) return;

            int startIndex = 12 * (page - 1);
            int endIndex = startIndex + 11;
            if (startIndex >= rows.Count) return;

            if (ActiveRows != null)
            {
                foreach (CraftRow row in ActiveRows)
                {
                    if (row == null) continue;
                    if (row.IsDisposed) continue;

                    row.Dispose();
                }
                ActiveRows.Clear();
            }

            ActiveRows = new List<CraftRow>();
            for (int i = 0; i < 12; i++)
            {
                if (startIndex + i >= rows.Count) continue;
                CraftRow row = new CraftRow(rows[startIndex + i])
                {
                    Location = new Point(16, 120 + 28 * i),
                    Parent = this,
                    Visible = true,
                };
                row.MouseClick += (o, e) => { SelectedRow = row; };
                ActiveRows.Add(row);
            }

            CurrentPage = page;

            if (PageNumber != null)
            {
                PageNumber.Text = CurrentPage + "/" + Math.Ceiling(rows.Count / 12F);
            }
        }
        /// <summary>
        /// 所选选项卡改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedTabChanged(object sender, EventArgs e)
        {
            DXButton clickedButton = sender as DXButton;
            if (clickedButton == null) return;

            //全体透明
            WeaponTab.Opacity = 0;
            ArmourTab.Opacity = 0;
            JewelryTab.Opacity = 0;
            ConsumableTab.Opacity = 0;
            MaterialTab.Opacity = 0;

            //选中的改不透明
            switch (clickedButton.Text)
            {
                case "武器":
                case "Weapon":
                    WeaponTab.Opacity = 1;
                    ActiveButton = WeaponTab;
                    ActiveLists = MakeableWeaponList;
                    TogglePageVisible(MakeableWeaponList, 1);
                    break;
                case "衣服":
                case "Armour":
                    ArmourTab.Opacity = 1;
                    ActiveButton = ArmourTab;
                    ActiveLists = MakeableArmourList;
                    TogglePageVisible(MakeableArmourList, 1);
                    break;
                case "首饰":
                case "Jewelry":
                    JewelryTab.Opacity = 1;
                    ActiveButton = JewelryTab;
                    ActiveLists = MakeableJewelryList;
                    TogglePageVisible(MakeableJewelryList, 1);
                    break;
                case "消耗品":
                case "Consumable":
                    ConsumableTab.Opacity = 1;
                    ActiveButton = ConsumableTab;
                    ActiveLists = MakeableConsumableList;
                    TogglePageVisible(MakeableConsumableList, 1);
                    break;
                case "其他":
                case "Material":
                    MaterialTab.Opacity = 1;
                    ActiveButton = MaterialTab;
                    ActiveLists = MakeableMaterialList;
                    TogglePageVisible(MakeableMaterialList, 1);
                    break;
            }
        }

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _SelectedRow = null;
                SelectedRowChanged = null;

                _SelectedCraftItemInfo = null;
                SelectedCraftItemInfoChanged = null;

                CurrentPage = 1;

                if (CraftBackground != null)
                {
                    if (!CraftBackground.IsDisposed)
                        CraftBackground.Dispose();

                    CraftBackground = null;
                }

                if (WeaponTab != null)
                {
                    if (!WeaponTab.IsDisposed)
                        WeaponTab.Dispose();

                    WeaponTab = null;
                }

                if (ArmourTab != null)
                {
                    if (!ArmourTab.IsDisposed)
                        ArmourTab.Dispose();

                    ArmourTab = null;
                }

                if (JewelryTab != null)
                {
                    if (!JewelryTab.IsDisposed)
                        JewelryTab.Dispose();

                    JewelryTab = null;
                }

                if (ConsumableTab != null)
                {
                    if (!ConsumableTab.IsDisposed)
                        ConsumableTab.Dispose();

                    ConsumableTab = null;
                }

                if (MaterialTab != null)
                {
                    if (!MaterialTab.IsDisposed)
                        MaterialTab.Dispose();

                    MaterialTab = null;
                }

                if (Blueprint1 != null)
                {
                    if (!Blueprint1.IsDisposed)
                        Blueprint1.Dispose();

                    Blueprint1 = null;
                }

                if (Blueprint2 != null)
                {
                    if (!Blueprint2.IsDisposed)
                        Blueprint2.Dispose();

                    Blueprint2 = null;
                }

                if (Blueprint3 != null)
                {
                    if (!Blueprint3.IsDisposed)
                        Blueprint3.Dispose();

                    Blueprint3 = null;
                }

                if (PreviousPageButton != null)
                {
                    if (!PreviousPageButton.IsDisposed)
                        PreviousPageButton.Dispose();

                    PreviousPageButton = null;
                }

                if (NextPageButton != null)
                {
                    if (!NextPageButton.IsDisposed)
                        NextPageButton.Dispose();

                    NextPageButton = null;
                }

                if (BookmarkButton != null)
                {
                    if (!BookmarkButton.IsDisposed)
                        BookmarkButton.Dispose();

                    BookmarkButton = null;
                }

                if (MakeButton != null)
                {
                    if (!MakeButton.IsDisposed)
                        MakeButton.Dispose();

                    MakeButton = null;
                }

                if (PageNumber != null)
                {
                    if (!PageNumber.IsDisposed)
                        PageNumber.Dispose();

                    PageNumber = null;
                }

                if (ShowMakeableOnlyBox != null)
                {
                    if (!ShowMakeableOnlyBox.IsDisposed)
                        ShowMakeableOnlyBox.Dispose();

                    ShowMakeableOnlyBox = null;
                }

                MakeableWeaponList?.Clear();
                MakeableArmourList?.Clear();
                MakeableJewelryList?.Clear();
                MakeableConsumableList?.Clear();
                MakeableMaterialList?.Clear();
                ActiveLists?.Clear();

                if (ActiveRows != null)
                {
                    foreach (CraftRow row in ActiveRows)
                    {
                        if (row == null) continue;
                        if (row.IsDisposed) continue;

                        row.Dispose();
                    }
                    ActiveRows.Clear();
                }

                RowsWithBlueprint2?.Clear();
                RowsWithBlueprint3?.Clear();

                if (ActiveButton != null)
                {
                    if (!ActiveButton.IsDisposed)
                        ActiveButton.Dispose();

                    ActiveButton = null;
                }

                if (_SelectedRow != null)
                {
                    if (!_SelectedRow.IsDisposed)
                        _SelectedRow.Dispose();

                    _SelectedRow = null;
                }

                if (_SelectedCraftItemInfo != null)
                {
                    _SelectedCraftItemInfo = null;
                }

                if (TargetDetail1 != null)
                {
                    if (!TargetDetail1.IsDisposed)
                        TargetDetail1.Dispose();

                    TargetDetail1 = null;
                }

                if (TargetDetail2 != null)
                {
                    if (!TargetDetail2.IsDisposed)
                        TargetDetail2.Dispose();

                    TargetDetail2 = null;
                }

                if (TargetDetail3 != null)
                {
                    if (!TargetDetail3.IsDisposed)
                        TargetDetail3.Dispose();

                    TargetDetail3 = null;
                }

                if (IngredientDetail1 != null)
                {
                    if (!IngredientDetail1.IsDisposed)
                        IngredientDetail1.Dispose();

                    IngredientDetail1 = null;
                }

                if (IngredientDetail2 != null)
                {
                    if (!IngredientDetail2.IsDisposed)
                        IngredientDetail2.Dispose();

                    IngredientDetail2 = null;
                }

                if (IngredientDetail3 != null)
                {
                    if (!IngredientDetail3.IsDisposed)
                        IngredientDetail3.Dispose();

                    IngredientDetail3 = null;
                }
            }
        }
        #endregion
    }
    /// <summary>
    /// 制作界面行
    /// </summary>
    public sealed class CraftRow : DXControl
    {
        #region Properties
        public DXImageControl BackgroundImage;
        public CraftItemInfo CraftItem;
        private DXLabel ItemNameLabel;
        #endregion

        /// <summary>
        /// 制作界面行
        /// </summary>
        /// <param name="craftItem"></param>
        public CraftRow(CraftItemInfo craftItem)
        {
            CraftItem = craftItem;

            Location = new Point(12, 112);
            Size = new Size(200, 24);
            IsControl = true;

            BackgroundImage = new DXImageControl //背景图
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 3740,
                IsControl = false,
                //Visible = false,
            };

            ItemNameLabel = new DXLabel   //道具名字标签
            {
                Parent = BackgroundImage,
                ForeColour = Color.White,
                Location = new Point(5, 4),
                IsControl = true,
                Text = craftItem.Item.Lang(p => p.ItemName),
                AutoSize = true,
                //Visible = false,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
            };
        }

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                CraftItem = null;

                if (BackgroundImage != null)
                {
                    if (!BackgroundImage.IsDisposed)
                        BackgroundImage.Dispose();

                    BackgroundImage = null;
                }

                if (ItemNameLabel != null)
                {
                    if (!ItemNameLabel.IsDisposed)
                        ItemNameLabel.Dispose();

                    ItemNameLabel = null;
                }
            }
        }
        #endregion
    }
    /// <summary>
    /// 制作目标细节
    /// </summary>
    public sealed class CraftTargetDetail : DXControl
    {
        public DXItemCell TargetItemCell;
        public DXLabel TargetItemNameLabel, SuccessRateLabel, LevelNeededLabel, CostLabel, GainExpLabel;

        /// <summary>
        /// 制作目标细节
        /// </summary>
        /// <param name="craftItem"></param>
        public CraftTargetDetail(CraftItemInfo craftItem)
        {
            Location = new Point(220, 60);
            Size = new Size(280, 140);

            TargetItemCell = new DXItemCell
            {
                Parent = this,
                Location = new Point(10, 32),
                //FixedBorder = true,
                //Border = true,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                //FixedBorderColour = true,
                Visible = true,
                Item = new ClientUserItem(craftItem.Item, craftItem.TargetAmount),
            };
            TargetItemCell.RefreshItem();

            TargetItemNameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(70, 35),
                ForeColour = Color.Yellow,
                IsControl = false,
                Text = craftItem.Item.Lang(p => p.ItemName),
                Font = new Font("SimSun", CEnvir.FontSize(11F), FontStyle.Regular),
                AutoSize = true,
                Visible = true,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
            };

            SuccessRateLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(70, 55),
                ForeColour = Color.LawnGreen,
                IsControl = false,
                Text = "制作成功率" + ": " + craftItem.SuccessRate.ToString() + " %",
                AutoSize = true,
                Visible = true,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
            };

            LevelNeededLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(198, 82),
                ForeColour = Color.White,
                IsControl = false,
                Text = craftItem.LevelNeeded.ToString(),
                AutoSize = true,
                Visible = true,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            };

            CostLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(198, 102),
                ForeColour = Color.White,
                IsControl = false,
                Text = craftItem.GoldCost.ToString(),
                AutoSize = true,
                Visible = true,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Right,
            };

            GainExpLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(198, 122),
                ForeColour = Color.White,
                IsControl = false,
                Text = craftItem.GainExp.ToString(),
                AutoSize = true,
                Visible = true,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (TargetItemCell != null)
                {
                    if (!TargetItemCell.IsDisposed)
                        TargetItemCell.Dispose();

                    TargetItemCell = null;
                }

                if (TargetItemNameLabel != null)
                {
                    if (!TargetItemNameLabel.IsDisposed)
                        TargetItemNameLabel.Dispose();

                    TargetItemNameLabel = null;
                }

                if (SuccessRateLabel != null)
                {
                    if (!SuccessRateLabel.IsDisposed)
                        SuccessRateLabel.Dispose();

                    SuccessRateLabel = null;
                }

                if (LevelNeededLabel != null)
                {
                    if (!LevelNeededLabel.IsDisposed)
                        LevelNeededLabel.Dispose();

                    LevelNeededLabel = null;
                }

                if (CostLabel != null)
                {
                    if (!CostLabel.IsDisposed)
                        CostLabel.Dispose();

                    CostLabel = null;
                }

                if (GainExpLabel != null)
                {
                    if (!GainExpLabel.IsDisposed)
                        GainExpLabel.Dispose();

                    GainExpLabel = null;
                }
            }
        }
        #endregion
    }
    /// <summary>
    /// 制作道具成分详图
    /// </summary>
    public sealed class CraftIngredientDetail : DXControl
    {
        public DXItemCell Ingredient1, Ingredient2, Ingredient3, Ingredient4, Ingredient5;
        public DXLabel Name1, Name2, Name3, Name4, Name5;
        public DXLabel Amount1, Amount2, Amount3, Amount4, Amount5;

        public CraftItemInfo CraftItem;

        /// <summary>
        /// 制作道具成分详图
        /// </summary>
        /// <param name="craftItem"></param>
        public CraftIngredientDetail(CraftItemInfo craftItem)
        {
            Location = new Point(226, 246);
            Size = new Size(272, 220);
            CraftItem = craftItem;

            #region 原材料

            if (craftItem.Item1 != null)
            {
                Ingredient1 = new DXItemCell
                {
                    Parent = this,
                    Location = new Point(3, 3),
                    //FixedBorder = true,
                    //Border = true,
                    ReadOnly = true,
                    ItemGrid = new ClientUserItem[1],
                    Slot = 0,
                    //FixedBorderColour = true,
                    Visible = true,
                    Item = new ClientUserItem(craftItem.Item1, craftItem.Amount1),
                };
                Ingredient1.RefreshItem();

                Name1 = new DXLabel
                {
                    Parent = this,
                    Location = new Point(66, 14),
                    ForeColour = Color.White,
                    IsControl = true,
                    Text = craftItem.Item1.Lang(p => p.ItemName),
                    AutoSize = true,
                    Font = new Font("SimSun", CEnvir.FontSize(11F), FontStyle.Regular),
                    //Visible = false,
                    DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                };

                Amount1 = new DXLabel
                {
                    Parent = this,
                    Location = new Point(Math.Max(Name1.Size.Width + 80, 146), 14),
                    ForeColour = Color.White,
                    IsControl = true,
                    Text = GameScene.Game.CountItemInventory(craftItem.Item1) + "/" + craftItem.Amount1.ToString(),
                    Font = new Font("SimSun", CEnvir.FontSize(11F), FontStyle.Regular),
                    AutoSize = true,
                    //Visible = false,
                    DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                };
            }

            if (craftItem.Item2 != null)
            {
                Ingredient2 = new DXItemCell
                {
                    Parent = this,
                    Location = new Point(3, 45),
                    //FixedBorder = true,
                    //Border = true,
                    ReadOnly = true,
                    ItemGrid = new ClientUserItem[1],
                    Slot = 0,
                    //FixedBorderColour = true,
                    Visible = true,
                    Item = new ClientUserItem(craftItem.Item2, craftItem.Amount2),
                };
                Ingredient2.RefreshItem();

                Name2 = new DXLabel
                {
                    Parent = this,
                    Location = new Point(66, 60),
                    ForeColour = Color.White,
                    IsControl = true,
                    Text = craftItem.Item2.Lang(p => p.ItemName),
                    AutoSize = true,
                    Font = new Font("SimSun", CEnvir.FontSize(11F), FontStyle.Regular),
                    //Visible = false,
                    DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                };

                Amount2 = new DXLabel
                {
                    Parent = this,
                    Location = new Point(Math.Max(Name2.Size.Width + 80, 146), 60),
                    ForeColour = Color.White,
                    IsControl = true,
                    Text = GameScene.Game.CountItemInventory(craftItem.Item2) + "/" + craftItem.Amount2.ToString(),
                    Font = new Font("SimSun", CEnvir.FontSize(11F), FontStyle.Regular),
                    AutoSize = true,
                    //Visible = false,
                    DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                };
            }

            if (craftItem.Item3 != null)
            {
                Ingredient3 = new DXItemCell
                {
                    Parent = this,
                    Location = new Point(3, 88),
                    //FixedBorder = true,
                    //Border = true,
                    ReadOnly = true,
                    ItemGrid = new ClientUserItem[1],
                    Slot = 0,
                    FixedBorderColour = true,
                    Visible = true,
                    Item = new ClientUserItem(craftItem.Item3, craftItem.Amount3),
                };
                Ingredient3.RefreshItem();

                Name3 = new DXLabel
                {
                    Parent = this,
                    Location = new Point(66, 101),
                    ForeColour = Color.White,
                    IsControl = true,
                    Text = craftItem.Item3.Lang(p => p.ItemName),
                    AutoSize = true,
                    Font = new Font("SimSun", CEnvir.FontSize(11F), FontStyle.Regular),
                    //Visible = false,
                    DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                };

                Amount3 = new DXLabel
                {
                    Parent = this,
                    Location = new Point(Math.Max(Name3.Size.Width + 80, 146), 101),
                    ForeColour = Color.White,
                    IsControl = true,
                    Text = GameScene.Game.CountItemInventory(craftItem.Item3) + "/" + craftItem.Amount3.ToString(),
                    Font = new Font("SimSun", CEnvir.FontSize(11F), FontStyle.Regular),
                    AutoSize = true,
                    //Visible = false,
                    DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                };
            }

            if (craftItem.Item4 != null)
            {
                Ingredient4 = new DXItemCell
                {
                    Parent = this,
                    Location = new Point(3, 131),
                    //FixedBorder = true,
                    //Border = true,
                    ReadOnly = true,
                    ItemGrid = new ClientUserItem[1],
                    Slot = 0,
                    //FixedBorderColour = true,
                    Visible = true,
                    Item = new ClientUserItem(craftItem.Item4, craftItem.Amount4),
                };
                Ingredient4.RefreshItem();

                Name4 = new DXLabel
                {
                    Parent = this,
                    Location = new Point(66, 141),
                    ForeColour = Color.White,
                    IsControl = true,
                    Text = craftItem.Item4.Lang(p => p.ItemName),
                    AutoSize = true,
                    Font = new Font("SimSun", CEnvir.FontSize(11F), FontStyle.Regular),
                    //Visible = false,
                    DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                };

                Amount4 = new DXLabel
                {
                    Parent = this,
                    Location = new Point(Math.Max(Name4.Size.Width + 80, 146), 141),
                    ForeColour = Color.White,
                    IsControl = true,
                    Text = GameScene.Game.CountItemInventory(craftItem.Item4) + "/" + craftItem.Amount4.ToString(),
                    Font = new Font("SimSun", CEnvir.FontSize(11F), FontStyle.Regular),
                    AutoSize = true,
                    //Visible = false,
                    DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                };
            }

            if (craftItem.Item5 != null)
            {
                Ingredient5 = new DXItemCell
                {
                    Parent = this,
                    Location = new Point(3, 174),
                    //FixedBorder = true,
                    //Border = true,
                    ReadOnly = true,
                    ItemGrid = new ClientUserItem[1],
                    Slot = 0,
                    //FixedBorderColour = true,
                    Visible = true,
                    Item = new ClientUserItem(craftItem.Item5, craftItem.Amount5),
                };
                Ingredient5.RefreshItem();

                Name5 = new DXLabel
                {
                    Parent = this,
                    Location = new Point(66, 186),
                    ForeColour = Color.White,
                    IsControl = true,
                    Text = craftItem.Item5.Lang(p => p.ItemName),
                    AutoSize = true,
                    Font = new Font("SimSun", CEnvir.FontSize(11F), FontStyle.Regular),
                    //Visible = false,
                    DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                };
                Amount5 = new DXLabel
                {
                    Parent = this,
                    Location = new Point(Math.Max(Name5.Size.Width + 80, 146), 186),
                    ForeColour = Color.White,
                    IsControl = true,
                    Text = GameScene.Game.CountItemInventory(craftItem.Item5) + "/" + craftItem.Amount5.ToString(),
                    Font = new Font("SimSun", CEnvir.FontSize(11F), FontStyle.Regular),
                    AutoSize = true,
                    //Visible = false,
                    DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                };
            }
            #endregion
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Ingredient1 != null)
                {
                    if (!Ingredient1.IsDisposed)
                        Ingredient1.Dispose();

                    Ingredient1 = null;
                }

                if (Ingredient2 != null)
                {
                    if (!Ingredient2.IsDisposed)
                        Ingredient2.Dispose();

                    Ingredient2 = null;
                }

                if (Ingredient3 != null)
                {
                    if (!Ingredient3.IsDisposed)
                        Ingredient3.Dispose();

                    Ingredient3 = null;
                }

                if (Ingredient4 != null)
                {
                    if (!Ingredient4.IsDisposed)
                        Ingredient4.Dispose();

                    Ingredient4 = null;
                }

                if (Ingredient5 != null)
                {
                    if (!Ingredient5.IsDisposed)
                        Ingredient5.Dispose();

                    Ingredient5 = null;
                }

                if (Name1 != null)
                {
                    if (!Name1.IsDisposed)
                        Name1.Dispose();

                    Name1 = null;
                }

                if (Name2 != null)
                {
                    if (!Name2.IsDisposed)
                        Name2.Dispose();

                    Name2 = null;
                }

                if (Name3 != null)
                {
                    if (!Name3.IsDisposed)
                        Name3.Dispose();

                    Name3 = null;
                }

                if (Name4 != null)
                {
                    if (!Name4.IsDisposed)
                        Name4.Dispose();

                    Name4 = null;
                }

                if (Name5 != null)
                {
                    if (!Name5.IsDisposed)
                        Name5.Dispose();

                    Name5 = null;
                }


                if (Amount1 != null)
                {
                    if (!Amount1.IsDisposed)
                        Amount1.Dispose();

                    Amount1 = null;
                }

                if (Amount2 != null)
                {
                    if (!Amount2.IsDisposed)
                        Amount2.Dispose();

                    Amount2 = null;
                }

                if (Amount3 != null)
                {
                    if (!Amount3.IsDisposed)
                        Amount3.Dispose();

                    Amount3 = null;
                }

                if (Amount4 != null)
                {
                    if (!Amount4.IsDisposed)
                        Amount4.Dispose();

                    Amount4 = null;
                }

                if (Amount5 != null)
                {
                    if (!Amount5.IsDisposed)
                        Amount5.Dispose();

                    Amount5 = null;
                }
            }
        }
        #endregion
    }
}
