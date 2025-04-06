using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.Scenes;
using Client.Scenes.Views;
using Library;
using Library.SystemModels;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input;
using System;
using System.Drawing;
using System.Linq;
using C = Library.Network.ClientPackets;
using Color = System.Drawing.Color;
using Font = MonoGame.Extended.Font;
using FontStyle = MonoGame.Extended.FontStyle;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

//Cleaned
namespace Client.Controls
{
    /// <summary>
    /// 道具单元格事件 事件参数
    /// </summary>
    public sealed class ItemCellEvent : EventArgs
    {
        public bool AllowMove = true;      //允许移动=真
        public bool Handled = false;       //处理=否
    }

    /// <summary>
    /// 道具单元格控制
    /// </summary>
    public sealed class DXItemCell : DXControl
    {
        #region Static
        /// <summary>
        /// 选择单元格
        /// </summary>
        public static DXItemCell SelectedCell
        {
            get => GameScene.Game.SelectedCell;
            set
            {
                GameScene.Game.SelectedCell = value;
                //if (value == null)
                //    MovingItem = false;
            }
        }

        #region Selected

        /// <summary>
        /// 选择项目
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
            SelectedChanged?.Invoke(this, EventArgs.Empty);

            UpdateBorder();
        }
        #endregion Selected

        /// <summary>
        /// 选择包裹的单元 --用于购买界面的出售 单个修理 的价格判断
        /// </summary>
        public static DXItemCell SelectedInventoryCell
        {
            get => _SelectedInventoryCell;
            set
            {
                if (_SelectedInventoryCell == value) return;

                if (_SelectedInventoryCell != null) _SelectedInventoryCell.Selected = false;

                _SelectedInventoryCell = value;

                if (_SelectedInventoryCell != null) _SelectedInventoryCell.Selected = true;

                GameScene.Game.InventoryBox.ChangePriceDes();
            }
        }
        static DXItemCell _SelectedInventoryCell;

        public const int CellWidth = 39;   //单元格宽
        public const int CellHeight = 39;   //单元格高

        /// <summary>
        /// 选中物品，是否可以出售
        /// </summary>
        /// <returns></returns>
        public bool CanSell()
        {
            var cell = this;
            if (cell == null)
            {
                return false;
            }
            return !(cell.Item.Flags.HasFlag(UserItemFlags.Locked) ||
                    cell.Item.Flags.HasFlag(UserItemFlags.Worthless) ||
                    cell.Item.Flags.HasFlag(UserItemFlags.Bound)
                ) && cell.Item.Info.CanSell;
        }
        #endregion

        #region Properties

        public LibraryFile LibraryFile = LibraryFile.StoreItems;

        #region FixedBorder
        /// <summary>
        /// 固定边界
        /// </summary>
        public bool FixedBorder
        {
            get => _FixedBorder;
            set
            {
                if (_FixedBorder == value) return;

                bool oldValue = _FixedBorder;
                _FixedBorder = value;

                OnFixedBorderChanged(oldValue, value);
            }
        }
        private bool _FixedBorder;
        public event EventHandler<EventArgs> FixedBorderChanged;
        public void OnFixedBorderChanged(bool oValue, bool nValue)
        {
            FixedBorderChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion FixedBorder

        #region FixedBorderColour
        /// <summary>
        /// 固定边框颜色
        /// </summary>
        public bool FixedBorderColour
        {
            get => _FixedBorderColour;
            set
            {
                if (_FixedBorderColour == value) return;

                bool oldValue = _FixedBorderColour;
                _FixedBorderColour = value;

                OnFixedBorderColourChanged(oldValue, value);
            }
        }
        private bool _FixedBorderColour;
        public event EventHandler<EventArgs> FixedBorderColourChanged;
        public void OnFixedBorderColourChanged(bool oValue, bool nValue)
        {
            FixedBorderColourChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion FixedBorderColour

        #region GridType
        /// <summary>
        /// 格子类型
        /// </summary>
        public GridType GridType
        {
            get => _GridType;
            set
            {
                if (_GridType == value) return;

                GridType oldValue = _GridType;
                _GridType = value;

                OnGridTypeChanged(oldValue, value);
            }
        }
        private GridType _GridType;
        public event EventHandler<EventArgs> GridTypeChanged;
        public void OnGridTypeChanged(GridType oValue, GridType nValue)
        {
            GridTypeChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion GridType

        #region HostGrid

        /// <summary>
        /// 主网格
        /// </summary>
        public DXItemGrid HostGrid
        {
            get => _HostGrid;
            set
            {
                if (_HostGrid == value) return;

                DXItemGrid oldValue = _HostGrid;
                _HostGrid = value;

                OnHostGridChanged(oldValue, value);
            }
        }
        private DXItemGrid _HostGrid;
        public event EventHandler<EventArgs> HostGridChanged;
        public void OnHostGridChanged(DXItemGrid oValue, DXItemGrid nValue)
        {
            HostGridChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion HostGrid

        #region Item

        /// <summary>
        /// 客户端用户项 道具
        /// </summary>
        public ClientUserItem Item
        {
            get
            {
                if (GridType == GridType.Belt || GridType == GridType.AutoPotion)  //如果格子类型是物品快捷栏或者自动喝药栏
                {
                    if (QuickInfo != null)   //如果快捷信息为空
                        return QuickInfoItem;  //返回 快捷信息 道具

                    return QuickItem;   //返回 快捷道具
                }

                if (Linked)   //如果链接
                    return Link?.Item;  //返回 链接 道具

                if (ItemGrid == null || Slot >= ItemGrid.Length) return null;   //如果 道具格子为空   Slot>= 道具网格的长度  那么返回空

                return ItemGrid[Slot];  //返回 道具格子的Slot
            }
            set
            {
                if (ItemGrid == null || ItemGrid[Slot] == value || Linked || Slot >= ItemGrid.Length) return;  //如果 道具格子的Slot = value     链接   Slot>= 道具网格的长度  那么返回 

                ClientUserItem oldValue = ItemGrid[Slot];  //客户端用户项 旧值 = 道具格子的Slot
                ItemGrid[Slot] = value;   //道具格子的Slot = value 

                OnItemChanged(oldValue, value);  //项目变更
            }
        }
        public event EventHandler<EventArgs> ItemChanged;
        public void OnItemChanged(ClientUserItem oValue, ClientUserItem nValue)
        {
            ItemChanged?.Invoke(this, EventArgs.Empty);
            RefreshItem();
        }
        #endregion Item

        #region ItemGrid

        /// <summary>
        /// 道具格子
        /// </summary>
        public ClientUserItem[] ItemGrid
        {
            get => _ItemGrid;
            set
            {
                if (_ItemGrid == value) return;

                ClientUserItem[] oldValue = _ItemGrid;
                _ItemGrid = value;

                OnItemGridChanged(oldValue, value);
            }
        }
        private ClientUserItem[] _ItemGrid;
        public event EventHandler<EventArgs> ItemGridChanged;
        public void OnItemGridChanged(ClientUserItem[] oValue, ClientUserItem[] nValue)
        {
            ItemGridChanged?.Invoke(this, EventArgs.Empty);
            ItemChanged?.Invoke(this, EventArgs.Empty);
            RefreshItem();  //刷新项目
        }
        #endregion ItemGrid

        #region Locked

        /// <summary>
        /// 锁定
        /// </summary>
        public bool Locked
        {
            get => _Locked;
            set
            {
                if (_Locked == value) return;

                bool oldValue = _Locked;
                _Locked = value;

                OnLockedChanged(oldValue, value);
            }
        }
        private bool _Locked;
        public event EventHandler<EventArgs> LockedChanged;
        public void OnLockedChanged(bool oValue, bool nValue)
        {
            LockedChanged?.Invoke(this, EventArgs.Empty);

            UpdateBorder();  //更新边界
        }
        #endregion Locked

        #region ReadOnly

        /// <summary>
        /// 只读属性
        /// </summary>
        public bool ReadOnly
        {
            get => _ReadOnly;
            set
            {
                if (_ReadOnly == value) return;

                bool oldValue = _ReadOnly;
                _ReadOnly = value;

                OnReadOnlyChanged(oldValue, value);
            }
        }
        private bool _ReadOnly;
        public event EventHandler<EventArgs> ReadOnlyChanged;
        public void OnReadOnlyChanged(bool oValue, bool nValue)
        {
            ReadOnlyChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion ReadOnly

        #region Slot

        /// <summary>
        /// 槽 口
        /// </summary>
        public int Slot
        {
            get => _Slot;
            set
            {
                if (_Slot == value) return;

                int oldValue = _Slot;
                _Slot = value;

                OnSlotChanged(oldValue, value);
            }
        }
        private int _Slot;
        public event EventHandler<EventArgs> SlotChanged;
        public void OnSlotChanged(int oValue, int nValue)
        {
            SlotChanged?.Invoke(this, EventArgs.Empty);
            ItemChanged?.Invoke(this, EventArgs.Empty);
            RefreshItem();
        }
        #endregion Slot

        #region ShowCountLabel

        /// <summary>
        /// 显示计数标签
        /// </summary>
        public bool ShowCountLabel
        {
            get => _ShowCountLabel;
            set
            {
                if (_ShowCountLabel == value) return;

                bool oldValue = _ShowCountLabel;
                _ShowCountLabel = value;

                OnShowCountLabelChanged(oldValue, value);
            }
        }
        private bool _ShowCountLabel;
        public event EventHandler<EventArgs> ShowCountLabelChanged;
        public void OnShowCountLabelChanged(bool oValue, bool nValue)
        {
            ShowCountLabelChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion ShowCountLabel

        public ClientUserItem QuickInfoItem { get; private set; }

        #region QuickInfo

        /// <summary>
        /// 快捷信息
        /// </summary>
        public ItemInfo QuickInfo
        {
            get => _QuickInfo;
            set
            {
                if (_QuickInfo == value) return;

                ItemInfo oldValue = _QuickInfo;
                _QuickInfo = value;

                OnLinkedInfoChanged(oldValue, value);
            }
        }
        private ItemInfo _QuickInfo;
        public event EventHandler<EventArgs> LinkedInfoChanged;
        public void OnLinkedInfoChanged(ItemInfo oValue, ItemInfo nValue)
        {
            if (nValue != null)
            {
                QuickInfoItem = new ClientUserItem(nValue, 1);
                QuickItem = null;
                if (GridType == GridType.Belt)
                    GameScene.Game.BeltBox.Links[Slot].LinkInfoIndex = nValue.Index;
            }
            else
            {
                QuickInfoItem = null;
                if (GridType == GridType.Belt)
                    GameScene.Game.BeltBox.Links[Slot].LinkInfoIndex = -1;
            }

            RefreshItem();
            LinkedInfoChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion QuickInfo

        #region QuickItem

        /// <summary>
        /// 快捷道具
        /// </summary>
        public ClientUserItem QuickItem
        {
            get => _QuickItem;
            set
            {
                if (_QuickItem == value) return;

                ClientUserItem oldValue = _QuickItem;
                _QuickItem = value;

                OnLinkedItemChanged(oldValue, value);
            }
        }
        private ClientUserItem _QuickItem;
        public event EventHandler<EventArgs> LinkedItemChanged;
        public void OnLinkedItemChanged(ClientUserItem oValue, ClientUserItem nValue)
        {
            if (nValue != null)
            {
                QuickInfo = null;
                GameScene.Game.BeltBox.Links[Slot].LinkItemIndex = nValue.Index;
            }
            else
            {
                GameScene.Game.BeltBox.Links[Slot].LinkItemIndex = -1;
            }

            RefreshItem();
            LinkedItemChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion QuickItem

        #region Link

        /// <summary>
        /// 道具链接
        /// </summary>
        public DXItemCell Link
        {
            get => _Link;
            set
            {
                if (_Link == value) return;

                DXItemCell oldValue = _Link;
                _Link = value;

                OnLinkChanged(oldValue, value);
            }
        }

        private DXItemCell _Link;
        public event EventHandler<EventArgs> LinkChanged;
        public void OnLinkChanged(DXItemCell oValue, DXItemCell nValue)
        {
            if (oValue?.Link == this) oValue.Link = null;

            if (nValue != null && nValue.Link != this) nValue.Link = this;

            RefreshItem();

            UpdateBorder();

            LinkChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion Link

        #region LinkedCount

        /// <summary>
        /// 链接计数
        /// </summary>
        public long LinkedCount
        {
            get => _LinkedCount;
            set
            {
                if (_LinkedCount == value) return;

                long oldValue = _LinkedCount;
                _LinkedCount = value;

                OnLinkedCountChanged(oldValue, value);
            }
        }
        private long _LinkedCount;
        public event EventHandler<EventArgs> LinkedCountChanged;
        public void OnLinkedCountChanged(long oValue, long nValue)
        {
            LinkedCountChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion LinkedCount

        #region Linked

        /// <summary>
        /// 关联
        /// </summary>
        public bool Linked
        {
            get => _Linked;
            set
            {
                if (_Linked == value) return;

                bool oldValue = _Linked;
                _Linked = value;

                OnLinkedChanged(oldValue, value);
            }
        }
        private bool _Linked;
        public event EventHandler<EventArgs> LinkedChanged;
        public void OnLinkedChanged(bool oValue, bool nValue)
        {
            LinkedChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion Linked

        #region AllowLink

        /// <summary>
        /// 允许关联
        /// </summary>
        public bool AllowLink
        {
            get => _AllowLink;
            set
            {
                if (_AllowLink == value) return;

                bool oldValue = _AllowLink;
                _AllowLink = value;

                OnAllowLinkChanged(oldValue, value);
            }
        }
        private bool _AllowLink;
        public event EventHandler<EventArgs> AllowLinkChanged;
        public void OnAllowLinkChanged(bool oValue, bool nValue)
        {
            AllowLinkChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion AllowLink

        public DXLabel CountLabel, LevelLabel;

        /// <summary>
        /// 可见更改时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (HostGrid == null && !IsVisible)
                Link = null;
        }

        /// <summary>
        /// 边框更改时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnBorderChanged(bool oValue, bool nValue)
        {
            base.OnBorderChanged(oValue, nValue);

            TextureValid = false;

            UpdateBorder();
        }

        /// <summary>
        /// 边框颜色更改时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnBorderColourChanged(Color oValue, Color nValue)
        {
            base.OnBorderColourChanged(oValue, nValue);

            TextureValid = false;

            UpdateBorder();
        }

        /// <summary>
        /// 启用更改时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnEnabledChanged(bool oValue, bool nValue)
        {
            base.OnEnabledChanged(oValue, nValue);

            UpdateBorder();
        }

        //public static bool MovingItem;
        #endregion Properties

        /// <summary>
        /// 绘制道具类型
        /// </summary>
        public DrawItemType DrawItemType { get; set; } = DrawItemType.All;

        /// <summary>
        /// 道具单元格
        /// </summary>
        /// <param name="draw"></param>
        public DXItemCell(bool draw = true)
        {
            BackColour = Color.Empty;
            DrawTexture = true;
            ShowCountLabel = true;
            AllowLink = true;

            BorderColour = Color.FromArgb(74, 56, 41);
            Size = new Size(CellWidth, CellHeight);

            CountLabel = new DXLabel   //计数标签
            {
                ForeColour = Color.Yellow,  //背包计数标签颜色
                IsControl = false,
                Parent = this,
            };
            CountLabel.SizeChanged += CountLabel_SizeChanged;

            LevelLabel = new DXLabel    //道具等级
            {
                ForeColour = Color.White,
                IsControl = false,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                Parent = this,
            };
            LevelLabel.SizeChanged += LevelLabel_SizeChanged;
        }

        #region Methods

        /// <summary>
        /// 计数标签大小更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CountLabel_SizeChanged(object sender, EventArgs e)
        {
            CountLabel.Location = new Point(Size.Width - CountLabel.Size.Width - 2, Size.Height - CountLabel.Size.Height - 2);
        }

        private void LevelLabel_SizeChanged(object sender, EventArgs e)
        {
            LevelLabel.Location = new Point(18, 2);
        }

        /// <summary>
        /// 清理纹理
        /// </summary>
        protected override void OnClearTexture()
        {
            base.OnClearTexture();

            if (!Border || BorderInformation == null) return;

            DXManager.Line.Draw(BorderInformation, BorderColour);
        }

        /// <summary>
        /// 更新边框信息
        /// </summary>
        protected internal override void UpdateBorderInformation()
        {
            BorderInformation = null;
            if (!Border || Size.Width == 0 || Size.Height == 0) return;

            BorderInformation = new[]
            {
                new Vector2(0, 0),
                new Vector2((Size.Width - 1) * ZoomRate, 0 ),
                new Vector2((Size.Width - 1)  * ZoomRate, (Size.Height - 1)  * ZoomRate),
                new Vector2(0 , (Size.Height - 1)  * ZoomRate),
                new Vector2(0 , 0 )
            };
            TextureValid = false;
        }

        /// <summary>
        /// 绘制边框
        /// </summary>
        protected override void DrawBorder() { }

        /// <summary>
        /// 绘制控件
        /// </summary>
        protected override void DrawControl()
        {
            //道具被鼠标选中，不画图片
            //if (Selected && SelectedInventoryCell != this)
            //{
            //    CountLabel.Visible = false;
            //    LevelLabel.Visible = false;
            //    return;
            //}
            //道具没被选中，但是有锁，不画图片   目的是解决道具在move的时候会闪现
            //if (!Selected && Locked)
            //{
            //    CountLabel.Visible = false;
            //    LevelLabel.Visible = false;
            //    return;
            //}

            if (DrawItemType == DrawItemType.Null) return;

            CountLabel.Visible = ShowCountLabel && Item?.Info != null && (Item.Info.Effect != ItemEffect.Gold && Item.Info.Effect != ItemEffect.Experience) && (Item.Info.StackSize > 1 || Item.Count > 1);
            LevelLabel.Visible = ShowCountLabel && Item?.Info != null && (Item.Info.Effect != ItemEffect.Gold && Item.Info.Effect != ItemEffect.Experience);
            if (DrawItemType == DrawItemType.All || DrawItemType == DrawItemType.OnlyImage)
            {
                MirLibrary Library;

                CEnvir.LibraryList.TryGetValue(LibraryFile, out Library);

                if (Library != null && Item != null)
                {
                    int drawIndex;
                    int effectIndex = 0, effectCount = 0;
                    Point effectOffset = Point.Empty;
                    MirLibrary effectLibrary = null;
                    //自定义特效首饰
                    ItemDisplayEffect customEffect =
                        Globals.ItemDisplayEffectList.Binding.FirstOrDefault(x => x.Info.Index == Item.Info.Index);

                    if (Item.Info.Effect == ItemEffect.Gold)  //移金币图标变化
                    {
                        if (Item.Count < 100)
                            drawIndex = 120;
                        else if (Item.Count < 200)
                            drawIndex = 121;
                        else if (Item.Count < 500)
                            drawIndex = 122;
                        else if (Item.Count < 1000)
                            drawIndex = 123;
                        else if (Item.Count < 1000000) //1 Million
                            drawIndex = 124;
                        else if (Item.Count < 5000000) //5 Million
                            drawIndex = 125;
                        else if (Item.Count < 10000000) //10 Million
                            drawIndex = 126;
                        else
                            drawIndex = 127;
                    }
                    else if (customEffect != null)
                    {
                        drawIndex = Item.Info.Image;
                        effectIndex = customEffect.InnerImageStartIndex;
                        effectCount = customEffect.InnerImageCount;
                        //effectOffset = new Point(customEffect.InnerX, customEffect.InnerY);
                        //effectOffset = new Point(-41, -41);
                        //effectOffset = new Point(-131, -131);
                        CEnvir.LibraryList.TryGetValue(customEffect.InnerEffectLibrary, out effectLibrary);

                    }
                    else if ((GridType == GridType.Equipment || GridType == GridType.Inspect) && Slot == (int)EquipmentSlot.FameTitle)  //声望称号
                    {
                        switch (Item.Info.Image)
                        {
                            case 4501: //江湖初出
                                effectIndex = 1870;
                                effectCount = 10;
                                effectOffset = new Point(40, 32);
                                break;
                            case 4502: //新进高手
                                effectIndex = 1890;
                                effectCount = 10;
                                effectOffset = new Point(40, 32);
                                break;
                            case 4503: //江湖侠客
                                effectIndex = 1910;
                                effectCount = 10;
                                effectOffset = new Point(40, 32);
                                break;
                            case 4504: //武林名宿
                                effectIndex = 1930;
                                effectCount = 10;
                                effectOffset = new Point(40, 32);
                                break;
                            case 4505: //仁义大侠
                                effectIndex = 1950;
                                effectCount = 10;
                                effectOffset = new Point(40, 32);
                                break;
                            case 4506: //善仁英雄
                                effectIndex = 1970;
                                effectCount = 10;
                                effectOffset = new Point(29, 21);
                                break;
                            case 4507: //尊扬义侠
                                effectIndex = 1990;
                                effectCount = 12;
                                effectOffset = new Point(24, 16);
                                break;
                            case 4508: //英雄豪杰
                                effectIndex = 2270;
                                effectCount = 18;
                                effectOffset = new Point(34, 26);
                                break;
                            case 4509: //武林至尊
                                effectIndex = 2250;
                                effectCount = 18;
                                effectOffset = new Point(34, 26);
                                break;
                        }
                        CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter, out effectLibrary);
                        drawIndex = -1;
                    }
                    else
                    {
                        ItemInfo info = Item.Info;

                        if (info.Effect == ItemEffect.ItemPart && Item.AddedStats[Stat.ItemIndex] > 0)
                            info = Globals.ItemInfoList.Binding.First(x => x.Index == Item.AddedStats[Stat.ItemIndex]);

                        if (Item.Info.Effect != ItemEffect.ItemPart)
                        {
                            drawIndex = CEnvir.GetItemIllusionItemInfo(Item);
                        }
                        else
                            drawIndex = info.Image;

                        switch (drawIndex)   //徽章道具特效
                        {
                            case 79: //八卦徽章
                                effectIndex = 600;
                                effectCount = 6;
                                effectOffset = new Point(25, 15);
                                CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter, out effectLibrary);
                                break;

                            case 328: //至尊徽章
                                effectIndex = 3890;
                                effectCount = 8;
                                CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter, out effectLibrary);
                                effectOffset = new Point(25, 15);
                                break;
                            case 329: //无极徽章
                                effectIndex = 680;
                                effectCount = 8;
                                CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter2, out effectLibrary);
                                effectOffset = new Point(-6, 5);
                                break;
                            case 340: //焰魔石
                                effectIndex = 2020;
                                effectCount = 10;
                                CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter, out effectLibrary);
                                effectOffset = new Point(-6, 2);
                                break;
                            case 341: //冰魔石
                                effectIndex = 2030;
                                effectCount = 10;
                                CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter, out effectLibrary);
                                effectOffset = new Point(-8, 4);
                                break;
                            case 342: //雷魔石
                                effectIndex = 2040;
                                effectCount = 10;
                                CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter, out effectLibrary);
                                effectOffset = new Point(-6, 0);
                                break;
                            case 343: //风魔石
                                effectIndex = 2050;
                                effectCount = 10;
                                CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter, out effectLibrary);
                                effectOffset = new Point(-6, 0);
                                break;
                            case 1870: //泰尚徽章
                                effectIndex = 2915;
                                effectCount = 5;
                                CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_UI, out effectLibrary);
                                effectOffset = new Point(-6, 5);
                                break;
                            case 4000:
                            case 4001:
                            case 4002:
                            case 4003:
                            case 4004:
                            case 4005:
                                effectIndex = 5800;
                                effectCount = 10;
                                CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter, out effectLibrary);
                                effectOffset = new Point(-9, 0);
                                break;
                        }
                    }

                    MirImage image = Library?.CreateImage(drawIndex, ImageType.Image);
                    Point imageOffset = Point.Empty;
                    if (image != null)
                    {
                        Rectangle area = new Rectangle(DisplayArea.X, DisplayArea.Y, image.Width, image.Height);
                        area.Offset((int)(Size.Width - image.Width) / 2, (int)(Size.Height - image.Height) / 2);

                        PresentMirImage(image.Image, this, area, Item.Count > 0 ? Color.White : Color.Gray, this, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);

                        //素材偏移量
                        imageOffset = new Point((Size.Width - image.Width) / 2, (Size.Height - image.Height) / 2);
                    }

                    MirImage effectImage = effectLibrary?.CreateImage(effectIndex + (GameScene.Game.MapControl.Animation) % effectCount, ImageType.Image);
                    if (effectImage != null)
                    {
                        Rectangle area = new Rectangle(DisplayArea.X, DisplayArea.Y, effectImage.Width, effectImage.Height);
                        if (customEffect != null)
                        {
                            //自定义特效偏移量
                            effectOffset = new Point((int)(Size.Width - effectImage.Width) / 2, (int)(Size.Height - effectImage.Height) / 2);
                        }
                        area.Offset(effectImage.OffSetX + effectOffset.X + imageOffset.X, effectImage.OffSetY + effectOffset.Y + imageOffset.Y);

                        PresentMirImage(effectImage.Image, this, area, Color.White, this, 0, 0, blend: (effectImage.ImageTextureType == 1 || effectImage.ImageTextureType == 3 || effectImage.ImageTextureType == 5), zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
                    }

                    //结婚道具外框
                    if (Item != null && (Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage && image != null)
                    {
                        Size = new Size(40, 40);
                        Location.Offset(-2, 0);
                        CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter, out var lib);
                        image = lib.CreateImage(197, ImageType.Image);
                        PresentMirImage(image.Image, this, DisplayArea, Item.Count > 0 ? Color.White : Color.Gray, this, 0, 0, blend: true, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
                    }
                }
            }

            if (DrawItemType == DrawItemType.All)
            {
                if (InterfaceLibrary != null)
                {
                    MirImage image = InterfaceLibrary.CreateImage(47, ImageType.Image);  //新道具图标

                    if (Item != null && Item.New && image != null)
                        PresentMirImage(image.Image, this, new Rectangle(DisplayArea.X + 1, DisplayArea.Y + 1, image.Width, image.Height), Item.Count > 0 ? Color.White : Color.Gray, this, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);

                    image = InterfaceLibrary.CreateImage(48, ImageType.Image);  //加锁道具图标
                    if (Item != null && (Item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked && image != null)
                        PresentMirImage(image.Image, this, new Rectangle(DisplayArea.X + 1, DisplayArea.Y + 1, image.Width, image.Height), Item.Count > 0 ? Color.White : Color.Gray, this, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);


                    image = InterfaceLibrary.CreateImage(49, ImageType.Image);  //无法使用道具图标
                    if (Item != null && GameScene.Game != null && !GameScene.Game.CanUseItem(Item) && image != null)
                        PresentMirImage(image.Image, this, new Rectangle(DisplayArea.Right - 12, DisplayArea.Y + 1, image.Width, image.Height), Item.Count > 0 ? Color.White : Color.Gray, this, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);

                    image = InterfaceLibrary.CreateImage(103, ImageType.Image);  //碎片道具图标
                    if (Item != null && GameScene.Game != null && image != null && Item.Info.Effect == ItemEffect.ItemPart)
                        PresentMirImage(image.Image, this, new Rectangle(DisplayArea.Right - 16, DisplayArea.Y + 1, image.Width, image.Height), Item.Count > 0 ? Color.White : Color.Gray, this, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
                }
            }

            base.DrawControl();
        }

        /// <summary>
        /// 更新边框
        /// </summary>
        public void UpdateBorder()
        {
            BackColour = Color.Empty;

            if (!Enabled)   //不启用
                BackColour = Color.FromArgb(125, 0, 125, 125);
            else if (Locked || Selected || (!Linked && Link != null))
                BackColour = Color.FromArgb(150, 78, 109, 205);
            //else if (MouseControl == this && SelectedCell != null && SelectedCell != this && GridType == GridType.Inventory)
            //    BackColour = Color.FromArgb(200, 48, 108, 56);

            DrawTexture = MouseControl == this || !Enabled || Locked || Selected || FixedBorder || (!Linked && Link != null);

            if (MouseControl == this || Locked || Selected || (!Linked && Link != null))
            //if (Locked || Selected || (!Linked && Link != null))
            {
                if (!FixedBorderColour)  //不固定边框颜色
                    BorderColour = Color.Lime;
                Border = true;
            }
            else
            {
                if (!FixedBorderColour)
                    BorderColour = Color.FromArgb(74, 56, 41);
                Border = FixedBorder;
            }
        }

        /// <summary>
        /// 刷新道具
        /// </summary>
        public void RefreshItem()
        {
            //如果 单元格类型 = 包裹  宠物包裹  碎片包裹 或者 快捷栏为零
            if ((GridType == GridType.Inventory || GridType == GridType.CompanionInventory || GridType == GridType.PatchGrid) && GameScene.Game.BeltBox?.Grid != null)
                foreach (DXItemCell cell in GameScene.Game.BeltBox.Grid.Grid)
                    cell.RefreshItem();

            //背包 宠物背包 碎片包裹
            //if ((GridType == GridType.Inventory || GridType == GridType.CompanionInventory || GridType == GridType.PatchGrid) && GameScene.Game?.BigPatchBox != null)
            //    GameScene.Game?.BigPatchBox?.RefreshItem();

            //药品快捷栏或者自动喝药表格
            if ((GridType == GridType.Belt || GridType == GridType.AutoPotion) && QuickInfo != null)
                QuickInfoItem.Count = GameScene.Game.Inventory.Where(x => x?.Info == QuickInfo).Sum(x => x.Count) + (GameScene.Game.Companion?.InventoryArray.Where(x => x?.Info == QuickInfo).Sum(x => x.Count) ?? 0);

            if (MouseControl == this && GridType != GridType.Belt)
            {
                GameScene.Game.MouseItem = null;
                GameScene.Game.MouseItem = Item;
            }

            CountLabel.Visible = ShowCountLabel && Item?.Info != null && (Item.Info.Effect != ItemEffect.Gold && Item.Info.Effect != ItemEffect.Experience) && (Item.Info.StackSize > 1 || Item.Count > 1);
            CountLabel.Text = Linked ? LinkedCount.ToString() : Item?.Count.ToString();
            //道具等级大于1  道具不是武器
            if (Item?.Level > 1 && Item?.Info.ItemType != ItemType.Weapon && Config.ItemLevelLabel)
            {
                LevelLabel.Visible = true;
                LevelLabel.Text = $"+" + (Item.Level - 1);
            }
        }

        /// <summary>
        /// 角色宝石道具
        /// </summary>
        /// <returns></returns>
        public bool UseGemOnItem()
        {
            if (SelectedCell == null)
            {
                return false;
            }
            if (SelectedCell.Item.Info.ItemType != ItemType.Gem && SelectedCell.Item.Info.ItemType != ItemType.Orb)
            {
                return false;
            }
            if (SelectedCell == this || SelectedCell.Item == null)
            {
                SelectedCell = null;
                return false;
            }
            if (Item == null)
                return false;

            if (Item.Info.Effect == ItemEffect.NoGem)
                return false;

            DXMessageBox dXMessageBox = new DXMessageBox($"DXItemCell.AttachGem".Lang(Item.Info.Lang(p => p.ItemName)), "升级".Lang(), DXMessageBoxButtons.YesNo);
            dXMessageBox.YesButton.MouseClick += delegate
            {
                C.AttachGem packet = new C.AttachGem
                {
                    FromGrid = SelectedCell.GridType,
                    ToGrid = GridType,
                    FromSlot = SelectedCell.Slot,
                    ToSlot = Slot
                };
                Locked = true;
                SelectedCell = null;
                CEnvir.Enqueue(packet);
                Locked = false;
            };
            return true;
        }

        /// <summary>
        /// 开孔或者取下宝石
        /// </summary>
        /// <returns></returns>
        public bool UseDrillOnItem()
        {
            if (SelectedCell == null)
            {
                return false;
            }
            if (SelectedCell.Item.Info.ItemType != ItemType.Drill)
            {
                return false;
            }
            if (SelectedCell == this || SelectedCell.Item == null)
            {
                SelectedCell = null;
                return false;
            }
            if (Item == null)
                return false;

            if (SelectedCell.Item.Info.Effect == ItemEffect.DrillAddHole)
            {
                DXMessageBox dXMessageBox = new DXMessageBox($"DXItemCell.AddHole".Lang(Item.Info.Lang(p => p.ItemName)), "打孔".Lang(), DXMessageBoxButtons.YesNo);
                dXMessageBox.YesButton.MouseClick += delegate
                {
                    C.AddHole packet = new C.AddHole
                    {
                        FromGrid = SelectedCell.GridType,
                        ToGrid = GridType,
                        FromSlot = SelectedCell.Slot,
                        ToSlot = Slot
                    };
                    Locked = true;
                    SelectedCell = null;
                    CEnvir.Enqueue(packet);
                    Locked = false;
                };
            }
            else if (SelectedCell.Item.Info.Effect == ItemEffect.DrillRemoveGem)
            {
                DXMessageBox dXMessageBox = new DXMessageBox($"DXItemCell.RemoveGem".Lang(Item.Info.Lang(p => p.ItemName)), "取下宝石".Lang(), DXMessageBoxButtons.YesNo);
                dXMessageBox.YesButton.MouseClick += delegate
                {
                    C.RemoveGem packet = new C.RemoveGem
                    {
                        FromGrid = SelectedCell.GridType,
                        ToGrid = GridType,
                        FromSlot = SelectedCell.Slot,
                        ToSlot = Slot
                    };
                    Locked = true;
                    SelectedCell = null;
                    CEnvir.Enqueue(packet);
                    Locked = false;
                };
            }
            return true;
        }

        public event EventHandler<ItemCellEvent> PreMoveItem;

        /// <summary>
        /// 移动道具
        /// </summary>
        public void MoveItem()
        {
            //在移动之前做个条件判断
            ItemCellEvent cellEvent = new ItemCellEvent();
            PreMoveItem?.Invoke(this, cellEvent);
            if (cellEvent.Handled && !cellEvent.AllowMove) return;

            if (SelectedCell == null)
            {
                if (Item == null) return;

                if (Linked && Link != null)
                {
                    Link = null;
                    return;
                }

                #region SelectedInventoryCell赋值
                if (GridType == GridType.Inventory)
                {
                    if (GameScene.Game.InventoryBox.Visible)
                    {
                        if (GameScene.Game.InventoryBox.InventoryType == InventoryType.Repair)
                        {
                            if (Item.Info.CanRepair)
                            {
                                SelectedInventoryCell = this;
                                //BackColour = Color.FromArgb(125, 78, 109, 205);
                            }
                            return;
                        }
                    }
                }
                #endregion

                SelectedCell = this;
                return;
            }

            //把物品放回原处
            if (SelectedCell == this || SelectedCell.Item == null)
            {
                SelectedCell = null;
                return;
            }

            switch (SelectedCell.GridType) //从格子里 选定单元格.格子类型
            {
                case GridType.Storage:  //仓库单元格
                    if (GridType == GridType.Belt)  //禁止仓库物品直接放入快捷栏
                    {
                        SelectedCell = null;
                        return;
                    }
                    break;
                case GridType.Equipment:  //装备单元格

                    if (GridType == GridType.Equipment)
                    {
                        return;
                    }

                    if (Item == null || (SelectedCell.Item.Info == Item.Info && SelectedCell.Item.Count < SelectedCell.Item.Info.StackSize))
                        SelectedCell.MoveItem(this);
                    else
                        SelectedCell.MoveItem(HostGrid);

                    SelectedCell = null;
                    return;

                case GridType.CompanionEquipment:  //宠物装备单元格

                    if (GridType == GridType.CompanionEquipment)
                    {
                        return;
                    }

                    if (Item == null || (SelectedCell.Item.Info == Item.Info && SelectedCell.Item.Count < SelectedCell.Item.Info.StackSize))
                        SelectedCell.MoveItem(this);
                    else
                        SelectedCell.MoveItem(HostGrid);

                    SelectedCell = null;
                    return;
                case GridType.FishingEquipment:   //钓鱼装备单元格

                    if (GridType == GridType.FishingEquipment)
                    {
                        return;
                    }

                    if (Item == null || (SelectedCell.Item.Info == Item.Info && SelectedCell.Item.Count < SelectedCell.Item.Info.StackSize))
                        SelectedCell.MoveItem(this);
                    else
                        SelectedCell.MoveItem(HostGrid);

                    SelectedCell = null;
                    return;
            }

            switch (GridType) //从包裹里移动物品放到格子里
            {
                case GridType.Equipment://装备单元格
                    if (!Functions.CorrectSlot(SelectedCell.Item.Info.ItemType, (EquipmentSlot)Slot) || SelectedCell.GridType == GridType.Belt) return;
                    SelectedCell.Item.Info.PlaySound();
                    ToEquipment(SelectedCell);
                    return;

                case GridType.CompanionEquipment://宠物装备单元格
                    if (!Functions.CorrectSlot(SelectedCell.Item.Info.ItemType, (CompanionSlot)Slot) || SelectedCell.GridType == GridType.Belt) return;
                    SelectedCell.Item.Info.PlaySound();
                    ToCompanionEquipment(SelectedCell);
                    return;

                case GridType.FishingEquipment://钓鱼装备单元格
                    if (!Functions.CorrectSlot(SelectedCell.Item.Info.ItemType, (FishingSlot)Slot) || SelectedCell.GridType == GridType.Belt) return;
                    SelectedCell.Item.Info.PlaySound();
                    ToFishingEquipment(SelectedCell);
                    return;

                case GridType.Storage: //到仓库               
                    if (!SelectedCell.Item.Info.CanStore)
                    {
                        GameScene.Game.ReceiveChat("此物品禁止存入仓库".Lang(), MessageType.System);
                        return;
                    }
                    break;

                case GridType.GuildStorage:  //到行会仓库
                    if (!SelectedCell.Item.Info.CanStore)
                    {
                        GameScene.Game.ReceiveChat("此物品禁止存入行会仓库".Lang(), MessageType.System);
                        return;
                    }
                    break;

                case GridType.CompanionInventory:  //到宠物包裹里
                    if (!SelectedCell.Item.Info.CanStore)
                    {
                        GameScene.Game.ReceiveChat("此物品禁止存入宠物包裹".Lang(), MessageType.System);
                        return;
                    }
                    break;
            }

            //宝石部分, 精炼格子
            if (Item?.Info != null)
            {
                Item.Info.PlaySound();
            }
            SelectedCell.MoveItem(this);
        }

        /// <summary>
        /// 移动到装备单元格
        /// </summary>
        /// <param name="fromCell"></param>
        public void ToEquipment(DXItemCell fromCell)
        {
            if (Locked || ReadOnly) return;        //如果锁定或者属性是只读的 返回

            if (Item != null && (Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;  //如果是婚戒 返回

            if (fromCell == SelectedCell) SelectedCell = null;  //如果（来自单元格==选定单元格）选定单元格=空

            if (!GameScene.Game.CanWearItem(fromCell.Item, (EquipmentSlot)Slot)) return;  //如果（！游戏场景，游戏，可穿戴物品（来自单元格物品，（装备槽）插槽）返回；

            C.ItemMove packet = new C.ItemMove  //道具移动 发包 = 新道具移动
            {
                FromGrid = fromCell.GridType,
                ToGrid = GridType,
                FromSlot = fromCell.Slot,
                ToSlot = Slot
            };

            if (Item != null && Item.Info == fromCell.Item.Info && Item.Count < Item.Info.StackSize &&
                (Item.Flags & UserItemFlags.Bound) == (fromCell.Item.Flags & UserItemFlags.Bound) &&
                (Item.Flags & UserItemFlags.Worthless) == (fromCell.Item.Flags & UserItemFlags.Worthless) &&
                (Item.Flags & UserItemFlags.NonRefinable) == (fromCell.Item.Flags & UserItemFlags.NonRefinable) &&
                (Item.Flags & UserItemFlags.Expirable) == (fromCell.Item.Flags & UserItemFlags.Expirable) &&
                Item.AddedStats.Compare(fromCell.Item.AddedStats) &&
                Item.ExpireTime == fromCell.Item.ExpireTime)
                packet.MergeItem = true;

            Locked = true;
            fromCell.Locked = true;
            CEnvir.Enqueue(packet);
        }

        /// <summary>
        /// 移动到宠物装备单元格
        /// </summary>
        /// <param name="fromCell"></param>
        public void ToCompanionEquipment(DXItemCell fromCell)
        {
            if (Locked || ReadOnly) return;

            if (fromCell == SelectedCell) SelectedCell = null;

            if (!GameScene.Game.CanCompanionWearItem(fromCell.Item, (CompanionSlot)Slot)) return;

            C.ItemMove packet = new C.ItemMove
            {
                FromGrid = fromCell.GridType,
                ToGrid = GridType,
                FromSlot = fromCell.Slot,
                ToSlot = Slot
            };

            if (Item != null && Item.Info == fromCell.Item.Info && Item.Count < Item.Info.StackSize &&
                (Item.Flags & UserItemFlags.Bound) == (fromCell.Item.Flags & UserItemFlags.Bound) &&
                (Item.Flags & UserItemFlags.Worthless) == (fromCell.Item.Flags & UserItemFlags.Worthless) &&
                (Item.Flags & UserItemFlags.NonRefinable) == (fromCell.Item.Flags & UserItemFlags.NonRefinable) &&
                (Item.Flags & UserItemFlags.Expirable) == (fromCell.Item.Flags & UserItemFlags.Expirable) &&
                Item.AddedStats.Compare(fromCell.Item.AddedStats) &&
                Item.ExpireTime == fromCell.Item.ExpireTime)
                packet.MergeItem = true;

            Locked = true;
            fromCell.Locked = true;
            CEnvir.Enqueue(packet);
        }

        /// <summary>
        /// 移动到钓鱼装备单元格
        /// </summary>
        /// <param name="fromCell"></param>
        public void ToFishingEquipment(DXItemCell fromCell)
        {
            if (Locked || ReadOnly) return;

            if (fromCell == SelectedCell) SelectedCell = null;

            if (!GameScene.Game.CanFishingWearItem(fromCell.Item, (FishingSlot)Slot)) return;

            C.ItemMove packet = new C.ItemMove
            {
                FromGrid = fromCell.GridType,
                ToGrid = GridType,
                FromSlot = fromCell.Slot,
                ToSlot = Slot
            };

            if (Item != null && Item.Info == fromCell.Item.Info && Item.Count < Item.Info.StackSize &&
                (Item.Flags & UserItemFlags.Bound) == (fromCell.Item.Flags & UserItemFlags.Bound) &&
                (Item.Flags & UserItemFlags.Worthless) == (fromCell.Item.Flags & UserItemFlags.Worthless) &&
                (Item.Flags & UserItemFlags.NonRefinable) == (fromCell.Item.Flags & UserItemFlags.NonRefinable) &&
                (Item.Flags & UserItemFlags.Expirable) == (fromCell.Item.Flags & UserItemFlags.Expirable) &&
                Item.AddedStats.Compare(fromCell.Item.AddedStats) &&
                Item.ExpireTime == fromCell.Item.ExpireTime)
                packet.MergeItem = true;

            Locked = true;
            fromCell.Locked = true;
            CEnvir.Enqueue(packet);
        }

        /// <summary>
        /// 移动道具到道具单元格
        /// </summary>
        /// <param name="toCell"></param>
        public void MoveItem(DXItemCell toCell)
        {
            ClientBeltLink link;

            #region Belt 快捷药品栏
            if (toCell.GridType == GridType.Belt)  //移动到单元格的类型等快捷物品栏
            {
                ItemInfo info = null;
                ClientUserItem item = null;

                if (GridType == toCell.GridType)
                {
                    info = toCell.QuickInfo;
                    item = toCell.QuickItem;
                }
                if (Item.Info.ItemType == ItemType.ItemPart) return; //快捷栏判断如果是碎片结束

                if (Item.Info.ShouldLinkInfo)
                    toCell.QuickInfo = Item.Info;
                else
                    toCell.QuickItem = Item;

                if (GridType == toCell.GridType)
                {
                    QuickInfo = info;
                    QuickItem = item;

                    link = GameScene.Game.BeltBox.Links[Slot];
                    CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex });
                }

                if (Selected) SelectedCell = null;

                link = GameScene.Game.BeltBox.Links[toCell.Slot];
                CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex });

                return;
            }
            #endregion Belt

            #region Auto Potion  自动喝药栏
            //if (toCell.GridType == GridType.AutoPotion)   //移动到单元格的类型等自动喝药栏
            //{
            //    if (GridType == toCell.GridType) return;
            //    if (!Item.Info.CanAutoPot) return;

            //    if (Selected) SelectedCell = null;

            //    toCell.QuickInfo = Item.Info;

            //    GameScene.Game?.BigPatchBox?.SendUpdate(toCell.Slot);
            //    return;
            //}
            #endregion Auto Potion

            if (GridType == GridType.Belt)
            {
                QuickInfo = null;
                QuickItem = null;

                link = GameScene.Game.BeltBox.Links[Slot];
                CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex });

                if (Selected) SelectedCell = null;
                return;
            }

            //if (GridType == GridType.AutoPotion)
            //{
            //    QuickInfo = null;

            //    GameScene.Game?.BigPatchBox?.SendUpdate(toCell.Slot);
            //    if (Selected) SelectedCell = null;
            //    return;
            //}

            if (GridType == GridType.PatchGrid)
            {
                if (toCell.Item != null)
                {
                    if (toCell?.Item?.Info?.ItemType != ItemType.ItemPart)
                    {
                        return;
                    }
                }
            }

            if (toCell.Linked)
            {
                if (!CheckLink(toCell.HostGrid)) return;

                if (Selected) SelectedCell = null;

                if (Item?.Count > 1)
                {
                    DXItemAmountWindow window = new DXItemAmountWindow("数量".Lang(), Item);

                    if (toCell.GridType == GridType.Sell)
                        window.AmountBox.Value = Item.Count;

                    window.ConfirmButton.MouseClick += (o, e) =>
                    {
                        toCell.LinkedCount = window.Amount;
                        toCell.Link = this;
                    };
                    return;
                }

                toCell.LinkedCount = 1;
                toCell.Link = this;
                return;
            }

            C.ItemMove packet = new C.ItemMove
            {
                FromGrid = GridType,
                ToGrid = toCell.GridType,
                FromSlot = Slot,
                ToSlot = toCell.Slot
            };

            if (toCell.Item != null && toCell.Item.Info == Item.Info && toCell.Item.Count < toCell.Item.Info.StackSize &&
                (Item.Flags & UserItemFlags.Bound) == (toCell.Item.Flags & UserItemFlags.Bound) &&
                (Item.Flags & UserItemFlags.Worthless) == (toCell.Item.Flags & UserItemFlags.Worthless) &&
                (Item.Flags & UserItemFlags.NonRefinable) == (toCell.Item.Flags & UserItemFlags.NonRefinable) &&
                (Item.Flags & UserItemFlags.Expirable) == (toCell.Item.Flags & UserItemFlags.Expirable) &&
                Item.AddedStats.Compare(toCell.Item.AddedStats) &&
                Item.ExpireTime == toCell.Item.ExpireTime)
                packet.MergeItem = true;

            if (Selected) SelectedCell = null;

            Locked = true;
            toCell.Locked = true;
            CEnvir.Enqueue(packet);
        }

        /// <summary>
        /// 移动道具 到道具格子里
        /// </summary>
        /// <param name="toGrid"></param>
        /// <param name="skipCount"></param>
        /// <returns></returns>
        public bool MoveItem(DXItemGrid toGrid, bool skipCount = false)
        {
            if (toGrid.GridType == GridType.Belt || toGrid.GridType == GridType.AutoPotion) return false;

            C.ItemMove packet = new C.ItemMove
            {
                FromGrid = GridType,
                FromSlot = Slot,
            };

            DXItemCell toCell = null;
            foreach (DXItemCell cell in toGrid.Grid)
            {
                if (cell.Locked || !cell.Enabled) continue;

                ClientUserItem toItem = cell.Item;

                if (toItem == null)
                {
                    if (cell.Linked)
                    {
                        if (!CheckLink(toGrid)) return false;

                        if (Selected) SelectedCell = null;

                        switch (toGrid.GridType)
                        {
                            case GridType.RefineSpecial:
                            case GridType.RefinementStoneCrystal:
                                cell.LinkedCount = 1;
                                break;

                            case GridType.MasterRefineFragment1:
                            case GridType.MasterRefineFragment2:
                                cell.LinkedCount = 10;
                                break;

                            case GridType.MasterRefineSpecial:
                                cell.LinkedCount = 1;
                                break;

                            case GridType.MasterRefineStone:
                                cell.LinkedCount = 1;
                                break;

                            default:
                                if (Item.Count > 1 && !skipCount)
                                {
                                    DXItemAmountWindow window = new DXItemAmountWindow("数量".Lang(), Item);

                                    if (cell.GridType == GridType.Sell)
                                        window.AmountBox.Value = Item.Count;

                                    window.ConfirmButton.MouseClick += (o, e) =>
                                    {
                                        cell.LinkedCount = window.Amount;
                                        cell.Link = this;
                                    };

                                    return true;
                                }

                                cell.LinkedCount = Item.Count;
                                break;

                            case GridType.WeaponCraftTemplate:
                                cell.LinkedCount = 1;
                                break;

                            case GridType.WeaponCraftYellow:
                                cell.LinkedCount = 1;
                                break;

                            case GridType.WeaponCraftBlue:
                                cell.LinkedCount = 1;
                                break;

                            case GridType.WeaponCraftRed:
                                cell.LinkedCount = 1;
                                break;

                            case GridType.WeaponCraftPurple:
                                cell.LinkedCount = 1;
                                break;

                            case GridType.WeaponCraftGreen:
                                cell.LinkedCount = 1;
                                break;

                            case GridType.WeaponCraftGrey:
                                cell.LinkedCount = 1;
                                break;
                        }

                        cell.Link = this;
                        return true;
                    }

                    if (toCell == null) toCell = cell;
                    continue;
                }

                if (cell.Linked || toItem.Info != Item.Info || toItem.Count >= toItem.Info.StackSize) continue;
                if ((Item.Flags & UserItemFlags.Bound) != (toItem.Flags & UserItemFlags.Bound)) continue;
                if ((Item.Flags & UserItemFlags.Worthless) != (toItem.Flags & UserItemFlags.Worthless)) continue;
                if ((Item.Flags & UserItemFlags.NonRefinable) != (toItem.Flags & UserItemFlags.NonRefinable)) continue;
                if ((Item.Flags & UserItemFlags.Expirable) != (toItem.Flags & UserItemFlags.Expirable)) continue;
                if (!Item.AddedStats.Compare(toItem.AddedStats)) continue;
                if (Item.ExpireTime != toItem.ExpireTime) continue;

                toCell = cell;
                packet.MergeItem = true;
                break;
            }

            if (toCell == null) return false;

            if (toCell.Selected) SelectedCell = null;

            packet.ToSlot = toCell.Slot;
            packet.ToGrid = toCell.GridType;

            Locked = true;
            toCell.Locked = true;
            CEnvir.Enqueue(packet);

            return true;
        }

        /// <summary>
        /// 检查连接 道具项目 网格
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public bool CheckLink(DXItemGrid grid)
        {
            if (!AllowLink || Item == null || (!Linked && Link != null) || grid == null) return false;

            switch (grid.GridType)
            {
                case GridType.Sell: //卖
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    //如果 单元格类型 = 包裹  宠物包裹  碎片包裹
                    if ((GridType != GridType.Inventory && GridType != GridType.CompanionInventory && GridType != GridType.PatchGrid) || (Item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked || (Item.Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless || !Item.Info.CanSell)
                        return false;
                    break;

                case GridType.RootSell: //一键出售
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    //如果 单元格类型 = 包裹  宠物包裹  碎片包裹
                    if ((GridType != GridType.Inventory && GridType != GridType.CompanionInventory && GridType != GridType.PatchGrid) || (Item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked || (Item.Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless || !Item.Info.CanSell)
                        return false;
                    break;

                case GridType.Repair: //修理
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (GameScene.Game.NPCBox.Page.Types.All(x => x != Item.Info.ItemType) || !Item.Info.CanRepair || Item.CurrentDurability >= Item.MaxDurability || (GameScene.Game.NPCRepairBox.SpecialCheckBox.Checked && CEnvir.Now < Item.NextSpecialRepair))
                        return false;
                    break;

                case GridType.SpecialRepair: //特修
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (GameScene.Game.NPCBox.Page.Types.All(x => x != Item.Info.ItemType) || !Item.Info.CanRepair || Item.CurrentDurability >= Item.MaxDurability || (GameScene.Game.NPCSpecialRepairBox.SpecialCheckBox.Checked && CEnvir.Now < Item.NextSpecialRepair))
                        return false;
                    break;

                case GridType.Storage: //仓库
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    //if (!MapObject.User.InSafeZone) return false;   //如果不是在安全区 直接  返回 假
                    if (GridType != GridType.Inventory && GridType != GridType.Equipment) return false;
                    if (!Item.Info.CanStore) return false;
                    break;
                case GridType.PatchGrid:  //碎片包裹
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (GridType != GridType.Inventory && GridType != GridType.Equipment) return false;
                    if (!Item.Info.CanStore) return false;
                    break;
                case GridType.RefinementStoneIronOre://精炼矿石 铁石
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (Item.Info.Effect != ItemEffect.IronOre || (Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    break;

                case GridType.RefinementStoneSilverOre://精炼矿石 银矿
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (Item.Info.Effect != ItemEffect.SilverOre || (Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    break;

                case GridType.RefinementStoneDiamond: //精炼矿石 金刚石
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (Item.Info.Effect != ItemEffect.Diamond || (Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    break;

                case GridType.RefinementStoneGoldOre: //精炼 金矿
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (Item.Info.Effect != ItemEffect.GoldOre || (Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    break;

                case GridType.RefinementStoneCrystal://精炼 晶石
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (Item.Info.Effect != ItemEffect.Crystal || (Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    break;

                case GridType.RefineBlackIronOre://精炼矿石 黑铁矿
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (Item.Info.Effect != ItemEffect.BlackIronOre || (Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    break;

                case GridType.RefineAccessory: //精炼配件
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if ((Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

                    switch (Item.Info.ItemType)
                    {
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                            break;

                        default:
                            return false;
                    }
                    break;

                case GridType.RefineSpecial:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if ((Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

                    switch (Item.Info.ItemType)
                    {
                        case ItemType.RefineSpecial:
                            if (Item.Info.Shape != 1) return false;
                            break;

                        default:
                            return false;
                    }
                    break;

                case GridType.ItemFragment:  //道具碎片
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if ((Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    //如果 单元格类型 = 包裹  宠物包裹  碎片包裹
                    if ((GridType != GridType.Inventory && GridType != GridType.CompanionInventory && GridType != GridType.PatchGrid) || (Item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked || !Item.CanFragment())
                        return false;
                    break;

                case GridType.Consign:  //寄售
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (GridType != GridType.Inventory && GridType != GridType.Storage) return false;
                    if (GridType == GridType.Inventory && !MapObject.User.InSafeZone) return false;
                    if ((Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

                    if (!Item.Info.CanTrade) return false;
                    if ((Item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound) return false;
                    break;

                case GridType.SendMail:  //邮件
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (GridType != GridType.Inventory && GridType != GridType.Storage) return false;
                    if (GridType == GridType.Inventory && !MapObject.User.InSafeZone) return false;

                    if (!Item.Info.CanTrade) return false;
                    if ((Item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound) return false;
                    break;

                case GridType.TradeUser: //交易
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (GridType != GridType.Inventory && GridType != GridType.Storage && GridType != GridType.Equipment) return false;

                    if (!Item.Info.CanTrade) return false;
                    if ((Item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound) return false;
                    break;

                case GridType.GuildStorage: //行会仓库
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (!Item.Info.CanTrade) return false;
                    if ((Item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound) return false;
                    if (GridType != GridType.Inventory && GridType != GridType.Storage && GridType != GridType.Equipment) return false;

                    if (!Item.Info.CanTrade) return false;
                    if ((Item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound) return false;
                    break;

                case GridType.WeddingRing:  //结婚戒指
                    if (GridType != GridType.Inventory) return false;
                    if (Item.Info.ItemType != ItemType.Ring) return false;
                    break;

                case GridType.AccessoryRefineUpgradeTarget:
                    if ((Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

                    if (GridType != GridType.Inventory && GridType != GridType.Equipment && GridType != GridType.CompanionInventory && GridType != GridType.Storage) return false;

                    switch (Item.Info.ItemType)
                    {
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                            break;

                        default:
                            return false;
                    }
                    if ((Item.Flags & UserItemFlags.Refinable) != UserItemFlags.Refinable) return false;
                    break;

                case GridType.AccessoryRefineLevelTarget:
                    if ((Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

                    if (GridType != GridType.Inventory && GridType != GridType.Equipment && GridType != GridType.CompanionInventory && GridType != GridType.Storage) return false;

                    switch (Item.Info.ItemType)
                    {
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                            break;

                        default:
                            return false;
                    }

                    if ((Item.Flags & UserItemFlags.Refinable) == UserItemFlags.Refinable) return false;

                    if (Item.Level >= Globals.GameAccessoryEXPInfoList.Count) return false;

                    break;

                case GridType.AccessoryRefineLevelItems:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if ((Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

                    if ((Item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked) return false;

                    if (GridType != GridType.Inventory && GridType != GridType.CompanionInventory && GridType != GridType.Storage) return false;

                    switch (Item.Info.ItemType)
                    {
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                            break;

                        default:
                            return false;
                    }

                    if (GameScene.Game.NPCAccessoryLevelBox.TargetCell.Grid[0].Link?.Item?.Info != Item.Info) return false;
                    if ((Item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound && (GameScene.Game.NPCAccessoryLevelBox.TargetCell.Grid[0].Link.Item.Flags & UserItemFlags.Bound) != UserItemFlags.Bound) return false;

                    break;

                case GridType.AccessoryReset:
                    if ((Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

                    if (GridType != GridType.Inventory && GridType != GridType.Equipment && GridType != GridType.CompanionInventory && GridType != GridType.Storage) return false;

                    switch (Item.Info.ItemType)
                    {
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                            break;

                        default:
                            return false;
                    }

                    if (Item.Level >= Globals.GameAccessoryEXPInfoList.Count) return false;
                    break;

                case GridType.MasterRefineFragment1:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (Item.Info.Effect != ItemEffect.Fragment1 || (Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    break;

                case GridType.MasterRefineFragment2:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (Item.Info.Effect != ItemEffect.Fragment2 || (Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    break;

                case GridType.MasterRefineFragment3:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (Item.Info.Effect != ItemEffect.Fragment3 || (Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    break;

                case GridType.MasterRefineStone:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (Item.Info.Effect != ItemEffect.RefinementStone || (Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    break;

                case GridType.MasterRefineSpecial:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if ((Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

                    switch (Item.Info.ItemType)
                    {
                        case ItemType.RefineSpecial:
                            if (Item.Info.Shape != 5) return false;
                            break;

                        default:
                            return false;
                    }
                    break;

                case GridType.WeaponCraftTemplate:
                    if (Item.Info.ItemType != ItemType.Weapon && Item.Info.Effect != ItemEffect.WeaponTemplate) return false;
                    break;

                case GridType.WeaponCraftBlue:
                    if (Item.Info.Effect != ItemEffect.BlueSlot) return false;
                    break;

                case GridType.WeaponCraftGreen:
                    if (Item.Info.Effect != ItemEffect.GreenSlot) return false;
                    break;

                case GridType.WeaponCraftGrey:
                    if (Item.Info.Effect != ItemEffect.GreySlot) return false;
                    break;

                case GridType.WeaponCraftPurple:
                    if (Item.Info.Effect != ItemEffect.PurpleSlot) return false;
                    break;

                case GridType.WeaponCraftRed:
                    if (Item.Info.Effect != ItemEffect.RedSlot) return false;
                    break;

                case GridType.WeaponCraftYellow:
                    if (Item.Info.Effect != ItemEffect.YellowSlot) return false;
                    break;

                case GridType.BookTarget:
                    {
                        if ((Item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked)
                        {
                            return false;
                        }
                        if (GridType != GridType.Inventory && GridType != GridType.CompanionInventory && GridType != GridType.Storage)
                        {
                            return false;
                        }
                        ItemType itemType = Item.Info.ItemType;
                        if (itemType != ItemType.Book)
                        {
                            return false;
                        }
                        if (Item.CurrentDurability > 99) //满页数 不需要升级
                        {
                            return false;
                        }
                        if (Item.Info.Shape == 0) //鉴定书籍
                        {
                            return false;
                        }
                        break;
                    }
                case GridType.BookMaterial:
                    {
                        if ((Item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked)
                        {
                            return false;
                        }
                        if (GridType != GridType.Inventory && GridType != GridType.CompanionInventory && GridType != GridType.Storage)
                        {
                            return false;
                        }
                        if (Item.Info.ItemType != ItemType.Book)
                        {
                            return false;
                        }
                        if (GameScene.Game.NPCBookCombineBox.BookGrid.Grid[0].Link?.Item?.CurrentDurability < Item.CurrentDurability)
                        {
                            return false;
                        }
                        if (GameScene.Game.NPCBookCombineBox.BookGrid.Grid[0].Link?.Item?.Info != Item.Info)
                        {
                            return false;
                        }
                        if (Item.Info.Shape == 0) //鉴定书籍
                        {
                            return false;
                        }
                        break;
                    }
                case GridType.CustomDialog:  //自定义对话框
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (GridType != GridType.Inventory
                        && GridType != GridType.Storage
                        && GridType != GridType.Inventory
                        && GridType != GridType.CompanionInventory
                        && GridType != GridType.PatchGrid) return false;
                    if (GridType == GridType.Inventory && !MapObject.User.InSafeZone) return false;
                    if ((Item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound || (Item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked) return false;
                    break;

                case GridType.AccessoryComposeLevelTarget:  //首饰合成主道具
                    if ((Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

                    if (GridType != GridType.Inventory && GridType != GridType.Equipment && GridType != GridType.CompanionInventory && GridType != GridType.Storage) return false;

                    switch (Item.Info.ItemType)
                    {
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                            break;

                        default:
                            return false;
                    }

                    if ((Item.Flags & UserItemFlags.Refinable) == UserItemFlags.Refinable) return false;

                    if (Item.Level >= 10) return false;

                    break;

                case GridType.AccessoryComposeLevelItems:   //首饰合成副道具
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if ((Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

                    if ((Item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked) return false;

                    if (GridType != GridType.Inventory && GridType != GridType.CompanionInventory && GridType != GridType.Storage) return false;

                    switch (Item.Info.ItemType)
                    {
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                            break;

                        default:
                            return false;
                    }

                    if (GameScene.Game.NPCAccessoryCombineBox.MainJewelry.Grid[0].Link?.Item?.Info != Item.Info) return false;
                    if ((Item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound && (GameScene.Game.NPCAccessoryCombineBox.MainJewelry.Grid[0].Link.Item.Flags & UserItemFlags.Bound) != UserItemFlags.Bound) return false;

                    break;

                case GridType.RefinementStoneCorundum: //精炼矿石 金刚石
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (Item.Info.Effect != ItemEffect.Corundum || (Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    break;
            }

            return true;
        }

        /// <summary>
        /// 角色使用道具
        /// </summary>
        /// <returns></returns>
        public bool UseItem()
        {
            //如果 道具为空 锁定的 只读的 所选单元格 = this   连接&联系为空  可使用道具(道具)  观察者模式 返回false
            if (Item == null || Locked || ReadOnly || SelectedCell == this || (!Linked && Link != null) || !GameScene.Game.CanUseItem(Item) || GameScene.Game.Observer) return false;

            if (GridType == GridType.Belt)
            {
                DXItemCell cell;

                // 快捷信息 为空
                if (QuickInfo != null)
                {
                    cell = GameScene.Game.InventoryBox.Grid.Grid.FirstOrDefault(x => x?.Item?.Info == QuickInfo) ??
                           GameScene.Game.CompanionBox.InventoryGrid?.Grid.FirstOrDefault(x => x?.Item?.Info == QuickInfo);
                }
                else
                    cell = GameScene.Game.InventoryBox.Grid.Grid.FirstOrDefault(x => x?.Item == QuickItem);

                return cell?.UseItem() == true;
            }

            switch (Item.Info.ItemType)
            {
                case ItemType.Weapon:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Weapon].ToEquipment(this);
                    break;

                case ItemType.Armour:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Armour].ToEquipment(this);
                    break;

                case ItemType.Torch:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Torch].ToEquipment(this);
                    break;

                case ItemType.Medicament:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Medicament].ToEquipment(this);
                    break;

                case ItemType.Helmet:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Helmet].ToEquipment(this);
                    break;

                case ItemType.Necklace:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Necklace].ToEquipment(this);
                    break;

                case ItemType.Bracelet:
                    if (GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.BraceletL].Item == null)
                        GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.BraceletL].ToEquipment(this);
                    else
                        GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.BraceletR].ToEquipment(this);
                    break;

                case ItemType.Ring:
                    if (GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.RingL].Item == null)
                        GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.RingL].ToEquipment(this);
                    else
                        GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.RingR].ToEquipment(this);
                    break;

                case ItemType.Shoes:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Shoes].ToEquipment(this);
                    break;

                case ItemType.Poison:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Poison].ToEquipment(this);
                    break;

                case ItemType.Amulet:
                case ItemType.DarkStone:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Amulet].ToEquipment(this);
                    break;

                case ItemType.Flower:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Flower].ToEquipment(this);
                    break;

                case ItemType.Emblem:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Emblem].ToEquipment(this);
                    break;

                case ItemType.FameTitle:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.FameTitle].ToEquipment(this);
                    break;

                case ItemType.Fashion:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Fashion].ToEquipment(this);
                    break;

                case ItemType.Shield:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Shield].ToEquipment(this);
                    break;

                case ItemType.HorseArmour:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.HorseArmour].ToEquipment(this);
                    break;

                case ItemType.CompanionBag:
                    if (GameScene.Game.Companion != null)
                        GameScene.Game.CompanionBox.EquipmentGrid[(int)CompanionSlot.Bag].ToEquipment(this);
                    break;

                case ItemType.CompanionHead:
                    if (GameScene.Game.Companion != null)
                        GameScene.Game.CompanionBox.EquipmentGrid[(int)CompanionSlot.Head].ToEquipment(this);
                    break;

                case ItemType.CompanionBack:
                    if (GameScene.Game.Companion != null)
                        GameScene.Game.CompanionBox.EquipmentGrid[(int)CompanionSlot.Back].ToEquipment(this);
                    break;

                case ItemType.Hook:
                    if (GameScene.Game.FishingBox != null)
                        GameScene.Game.FishingBox.FishingGrid[(int)FishingSlot.Hook].ToEquipment(this);
                    break;
                case ItemType.Float:
                    if (GameScene.Game.FishingBox != null)
                        GameScene.Game.FishingBox.FishingGrid[(int)FishingSlot.Float].ToEquipment(this);
                    break;
                case ItemType.Bait:
                    if (GameScene.Game.FishingBox != null)
                        GameScene.Game.FishingBox.FishingGrid[(int)FishingSlot.Bait].ToEquipment(this);
                    break;
                case ItemType.Finder:
                    if (GameScene.Game.FishingBox != null)
                        GameScene.Game.FishingBox.FishingGrid[(int)FishingSlot.Finder].ToEquipment(this);
                    break;
                case ItemType.Reel:
                    if (GameScene.Game.FishingBox != null)
                        GameScene.Game.FishingBox.FishingGrid[(int)FishingSlot.Reel].ToEquipment(this);
                    break;

                case ItemType.Consumable:  //消耗品
                case ItemType.Scroll:  //卷轴
                case ItemType.CompanionFood:  //宠物粮食
                case ItemType.ItemPart:  //碎片
                    //如果 可以使用的道具 或 包裹 或 宠物装备格子 或 宠物包裹 或 碎片包裹 或钓鱼装备格子
                    if (!GameScene.Game.CanUseItem(Item) ||
                        GridType != GridType.Inventory && GridType != GridType.CompanionEquipment && GridType != GridType.CompanionInventory && GridType != GridType.PatchGrid && GridType != GridType.FishingEquipment) return false;

                    if ((CEnvir.Now < GameScene.Game.UseItemTime && Item.Info.Effect != ItemEffect.ElixirOfPurification)) return false;  // || MapObject.User.Horse != HorseType.None

                    //骑马无法使用物品提示
                    /*if (MapObject.User.Horse != HorseType.None)
                    {
                        //DXMessageBox.Show("骑行状态无法使用。", "提示");
                        GameScene.Game.ReceiveChat("骑行状态无法使用任何物品", MessageType.Hint);
                        return false;
                    }*/

                    GameScene.Game.UseItemTime = CEnvir.Now.AddMilliseconds(Math.Max(250, Item.Info.Durability));

                    Locked = true;

                    //碎片使用
                    CEnvir.Enqueue(new C.ItemUse { Link = new CellLinkInfo { GridType = GridType, Slot = Slot, Count = 1 } });
                    PlayItemSound();
                    break;

                case ItemType.Book:
                    if (!GameScene.Game.CanUseItem(Item) || GridType != GridType.Inventory) return false;

                    if (CEnvir.Now < GameScene.Game.UseItemTime) return false;  // || MapObject.User.Horse != HorseType.None

                    GameScene.Game.UseItemTime = CEnvir.Now.AddMilliseconds(250);
                    Locked = true;

                    CEnvir.Enqueue(new C.ItemUse { Link = new CellLinkInfo { GridType = GridType, Slot = Slot, Count = 1 } });
                    PlayItemSound();
                    break;

                case ItemType.System:
                    if (!GameScene.Game.CanUseItem(Item) || GridType != GridType.Inventory) return false;

                    switch (Item.Info.Effect)
                    {
                        case ItemEffect.GenderChange:
                            if (GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Armour].Item != null)
                            {
                                GameScene.Game.ReceiveChat("你穿着衣服无法改变性别".Lang(), MessageType.System);
                                return false;
                            }

                            GameScene.Game.EditCharacterBox.Visible = true;

                            GameScene.Game.EditCharacterBox.SelectedClass = GameScene.Game.User.Class;
                            GameScene.Game.EditCharacterBox.SelectedGender = GameScene.Game.User.Gender;
                            GameScene.Game.EditCharacterBox.HairColour.BackColour = GameScene.Game.User.HairColour;
                            GameScene.Game.EditCharacterBox.HairNumberBox.Value = GameScene.Game.User.HairType;

                            GameScene.Game.EditCharacterBox.Change = ChangeType.GenderChange;
                            break;

                        case ItemEffect.HairChange:
                            GameScene.Game.EditCharacterBox.Visible = true;

                            GameScene.Game.EditCharacterBox.SelectedClass = GameScene.Game.User.Class;
                            GameScene.Game.EditCharacterBox.SelectedGender = GameScene.Game.User.Gender;
                            GameScene.Game.EditCharacterBox.HairColour.BackColour = GameScene.Game.User.HairColour;
                            GameScene.Game.EditCharacterBox.HairNumberBox.Value = GameScene.Game.User.HairType;

                            GameScene.Game.EditCharacterBox.Change = ChangeType.HairChange;

                            if (GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Armour].Item != null)
                                GameScene.Game.EditCharacterBox.ArmourColour.BackColour = GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Armour].Item.Colour;
                            break;

                        case ItemEffect.ArmourDye:
                            if (GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Armour].Item == null)
                            {
                                GameScene.Game.ReceiveChat("你需要穿上衣服才能染上颜色".Lang(), MessageType.System);
                                return false;
                            }

                            GameScene.Game.EditCharacterBox.Visible = true;

                            GameScene.Game.EditCharacterBox.SelectedClass = GameScene.Game.User.Class;
                            GameScene.Game.EditCharacterBox.SelectedGender = GameScene.Game.User.Gender;

                            GameScene.Game.EditCharacterBox.HairColour.BackColour = GameScene.Game.User.HairColour;
                            GameScene.Game.EditCharacterBox.HairNumberBox.Value = GameScene.Game.User.HairType;

                            GameScene.Game.EditCharacterBox.Change = ChangeType.ArmourDye;

                            GameScene.Game.EditCharacterBox.ArmourColour.BackColour = GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Armour].Item.Colour;
                            break;

                        case ItemEffect.NameChange:
                            GameScene.Game.EditCharacterBox.Visible = true;

                            GameScene.Game.EditCharacterBox.SelectedClass = GameScene.Game.User.Class;
                            GameScene.Game.EditCharacterBox.SelectedGender = GameScene.Game.User.Gender;
                            GameScene.Game.EditCharacterBox.HairColour.BackColour = GameScene.Game.User.HairColour;
                            GameScene.Game.EditCharacterBox.HairNumberBox.Value = GameScene.Game.User.HairType;

                            if (GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Armour].Item != null)
                                GameScene.Game.EditCharacterBox.ArmourColour.BackColour = GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Armour].Item.Colour;

                            GameScene.Game.EditCharacterBox.Change = ChangeType.NameChange;

                            GameScene.Game.EditCharacterBox.CharacterNameTextBox.TextBox.Text = GameScene.Game.User.Name;
                            break;

                        case ItemEffect.FortuneChecker:
                            GameScene.Game.FortuneCheckerBox.Visible = true;
                            break;
                    }

                    break;
                case ItemType.Meat:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 道具声效
        /// </summary>
        private void PlayItemSound()
        {
            if (Item == null) return;

            switch (Item.Info.ItemType)
            {
                case ItemType.Weapon:
                    DXSoundManager.Play(SoundIndex.ItemWeapon);
                    break;

                case ItemType.Armour:
                case ItemType.Fashion:
                    DXSoundManager.Play(SoundIndex.ItemArmour);
                    break;

                case ItemType.Helmet:
                    DXSoundManager.Play(SoundIndex.ItemHelmet);
                    break;

                case ItemType.Necklace:
                    DXSoundManager.Play(SoundIndex.ItemNecklace);
                    break;

                case ItemType.Bracelet:
                    DXSoundManager.Play(SoundIndex.ItemBracelet);
                    break;

                case ItemType.Ring:
                    DXSoundManager.Play(SoundIndex.ItemRing);
                    break;

                case ItemType.Shoes:
                    DXSoundManager.Play(SoundIndex.ItemShoes);
                    break;

                case ItemType.Consumable:
                    DXSoundManager.Play(Item.Info.Shape > 0 ? SoundIndex.ItemDefault : SoundIndex.ItemPotion);
                    break;

                default:
                    DXSoundManager.Play(SoundIndex.ItemDefault);
                    break;
            }
        }

        /// <summary>
        /// 鼠标进入时
        /// </summary>
        public override void OnMouseEnter()
        {
            base.OnMouseEnter();

            if (GridType == GridType.Belt) return;
            GameScene.Game.MouseItem = Item;

            if (Item != null)
                Item.New = false;

            UpdateBorder();
        }

        /// <summary>
        /// 鼠标离开时
        /// </summary>
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            GameScene.Game.MouseItem = null;
            UpdateBorder();
        }

        /// <summary>
        /// 鼠标单击时
        /// </summary>
        /// <param name="e"></param>
        //public override void OnMouseClick(MouseEventArgs e)
        //{
        //    //声望称号 鼠标点击无效
        //    if ((GridType == GridType.Equipment || GridType == GridType.Inspect) && Slot == (int)EquipmentSlot.FameTitle) return;

        //    if (Locked || GameScene.Game.GoldPickedUp || (!Linked && Link != null) || GameScene.Game.Observer) return;

        //    base.OnMouseClick(e);

        //    if (ReadOnly || !Enabled) return;

        //    if (Linked && Link != null)
        //    {
        //        Link = null;

        //        if (SelectedCell == null)
        //            return;
        //    }

        //    switch (e.Button)
        //    {
        //        case MouseButtons.Left:  //鼠标左键

        //            if (CEnvir.Alt)
        //            {
        //                break;
        //            }
        //            if (CEnvir.Shift)
        //            {
        //                if (Item != null)  //如果道具不等空
        //                {
        //                    //如果道具是武器 Shape值是125  或者  道具是武器 Shape值是126
        //                    if (Item.Info.ItemType == ItemType.Weapon && Item.Info.Shape == 125 || Item.Info.ItemType == ItemType.Weapon && Item.Info.Shape == 126)
        //                    {
        //                        if (GameScene.Game?.FishingBox != null)            //如果钓鱼装备面板不为空 呼出
        //                        {
        //                            if (GameScene.Game.FishingBox.Visible)
        //                            {
        //                                GameScene.Game.FishingBox.Hide();
        //                            }
        //                            else GameScene.Game.FishingBox.Show();
        //                        }
        //                    }
        //                }
        //                bool flag = true;
        //               //如果背包满不让打开拆分面板
        //                if(GridType == GridType.Inventory)
        //                {
        //                    bool full = true;
        //                    foreach(var item in GameScene.Game.InventoryBox.Grid.ItemGrid)
        //                    {
        //                        if(item == null)
        //                        {
        //                            full = false;
        //                            break;
        //                        }
        //                    }
        //                    if(full) flag = false;
        //                }

        //                int storagetotal = 0;

        //                //如果仓库满不让打开拆分面板
        //                if (GridType == GridType.Storage)
        //                {
        //                    foreach (var item1 in GameScene.Game.StorageBox.Grid.ItemGrid)
        //                    {
        //                        if (item1 != null)
        //                        {
        //                            storagetotal++;
        //                        }
        //                    }
        //                }
        //                if (GameScene.Game.StorageSize <= storagetotal) flag = false;

        //                //如果道具等空 或 道具的格子不等包裹 仓库 行会仓库 宠物包裹 碎片包裹 或道具数量小于等于1 跳出
        //                if (GameScene.Game.User.Dead || Item == null || (GridType != GridType.Inventory && GridType != GridType.Storage && GridType != GridType.GuildStorage && GridType != GridType.CompanionInventory && GridType != GridType.PatchGrid) || Item.Count <= 1) return;
        //                if (!flag)
        //                {
        //                    GameScene.Game.ReceiveChat("格子已满，请先整理下格子", MessageType.System);
        //                    return;
        //                }
        //                new DXInputWindow(Item, (amount) =>
        //                {
        //                    if (amount <= 0 || amount >= Item.Count) return;

        //                    Locked = true;
        //                    Locked = true;
        //                    CEnvir.Enqueue(new C.ItemSplit { Grid = GridType, Slot = Slot, Count = amount });
        //                },Item.Count, "道具拆分".Lang());
        //                return;
        //            }
        //            if (CEnvir.Ctrl)
        //            {
        //                //镶嵌宝石
        //                if (UseGemOnItem()) return;
        //                //开孔或取下宝石
        //                if (UseDrillOnItem()) return;
        //            }

        //            if (Item != null && SelectedCell == null)
        //            {
        //                PlayItemSound();
        //            }
        //            MoveItem();
        //            break;

        //        case MouseButtons.Middle:  //鼠标中键
        //            //if (Item != null)
        //                //CEnvir.Enqueue(new C.ItemLock { GridType = GridType, SlotIndex = Slot, Locked = (Item.Flags & UserItemFlags.Locked) != UserItemFlags.Locked });
        //            break;

        //        case MouseButtons.Right: //鼠标右键

        //            #region 鼠标右键点击道具到格子里

        //            switch (GridType)   //到格子里
        //            {
        //                case GridType.Belt:
        //                case GridType.AutoPotion:
        //                    if (Item == null) return;

        //                    UseItem();
        //                    break;
        //                case GridType.Inventory:   //从包裹里
        //                    if (Item == null) return;

        //                    if (!Item.Info.CanStore)
        //                    {
        //                        GameScene.Game.ReceiveChat("此物品禁止存入".Lang(), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCRepairBox.IsVisible)  //修理
        //                    {
        //                        if (Item.CurrentDurability >= Item.MaxDurability || !Item.Info.CanRepair)
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepair".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        else if (!MoveItem(GameScene.Game.NPCRepairBox.Grid))
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepairBox".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCSpecialRepairBox.IsVisible)  //特修
        //                    {
        //                        if (Item.CurrentDurability >= Item.MaxDurability || !Item.Info.CanRepair)
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepair".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        else if (!MoveItem(GameScene.Game.NPCSpecialRepairBox.Grid))
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepairBox".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCRootSellBox.IsVisible)
        //                    {
        //                        if (!Item.Info.CanSell)
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRootSell".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        else if (!MoveItem(GameScene.Game.NPCRootSellBox.Grid))
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRootSellBox".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.InventoryBox.IsVisible && GameScene.Game.InventoryBox.InventoryType == InventoryType.Repair)
        //                    {
        //                        GameScene.Game.InventoryBox.RefreshGrid.InvokeMouseClick();
        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCBookCombineBox.IsVisible)
        //                    {
        //                        if (Item.Info.ItemType != ItemType.Book)
        //                        {
        //                            break;
        //                        }
        //                        if (GameScene.Game.NPCBookCombineBox.BookGrid.Grid[0].Link == null)
        //                        {
        //                            if (!MoveItem(GameScene.Game.NPCBookCombineBox.BookGrid))
        //                            {
        //                                GameScene.Game.ReceiveChat($"DXItemCell.BookCombine".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                            }
        //                        }
        //                        else if (!MoveItem(GameScene.Game.NPCBookCombineBox.MaterialGrid1))
        //                        {
        //                            GameScene.Game.ReceiveChat(Item.Info.Lang(p => p.ItemName) + "无法使用".Lang(), MessageType.System);
        //                        }
        //                    }

        //                    if (GameScene.Game.NPCMasterRefineBox.IsVisible)
        //                    {
        //                        switch (Item.Info.Effect)
        //                        {
        //                            case ItemEffect.Fragment1:
        //                                if (MoveItem(GameScene.Game.NPCMasterRefineBox.Fragment1Grid))
        //                                    return;
        //                                break;

        //                            case ItemEffect.Fragment2:
        //                                if (MoveItem(GameScene.Game.NPCMasterRefineBox.Fragment2Grid))
        //                                    return;
        //                                break;

        //                            case ItemEffect.Fragment3:
        //                                if (MoveItem(GameScene.Game.NPCMasterRefineBox.Fragment3Grid))
        //                                    return;
        //                                break;

        //                            case ItemEffect.RefinementStone:
        //                                if (MoveItem(GameScene.Game.NPCMasterRefineBox.RefinementStoneGrid))
        //                                    return;
        //                                break;
        //                        }

        //                        switch (Item.Info.ItemType)
        //                        {
        //                            case ItemType.RefineSpecial:
        //                                if (MoveItem(GameScene.Game.NPCMasterRefineBox.SpecialGrid))
        //                                    return;
        //                                break;
        //                        }
        //                        GameScene.Game.ReceiveChat($"DXItemCell.MasterRefine".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }
        //                    if (GameScene.Game.NPCRefinementStoneBox.IsVisible)
        //                    {
        //                        switch (Item.Info.Effect)
        //                        {
        //                            case ItemEffect.IronOre:
        //                                MoveItem(GameScene.Game.NPCRefinementStoneBox.IronOreGrid);
        //                                return;

        //                            case ItemEffect.SilverOre:
        //                                MoveItem(GameScene.Game.NPCRefinementStoneBox.SilverOreGrid);
        //                                return;

        //                            case ItemEffect.Diamond:
        //                                MoveItem(GameScene.Game.NPCRefinementStoneBox.DiamondGrid);
        //                                return;

        //                            case ItemEffect.GoldOre:
        //                                MoveItem(GameScene.Game.NPCRefinementStoneBox.GoldOreGrid);
        //                                return;

        //                            case ItemEffect.Crystal:
        //                                MoveItem(GameScene.Game.NPCRefinementStoneBox.CrystalGrid);
        //                                return;
        //                        }
        //                        GameScene.Game.ReceiveChat($"DXItemCell.MasterRefine".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }
        //                    if (GameScene.Game.NPCWeaponCraftBox.IsVisible)
        //                    {
        //                        switch (Item.Info.Effect)
        //                        {
        //                            case ItemEffect.WeaponTemplate:
        //                                MoveItem(GameScene.Game.NPCWeaponCraftBox.TemplateCell);
        //                                return;

        //                            case ItemEffect.YellowSlot:
        //                                MoveItem(GameScene.Game.NPCWeaponCraftBox.YellowCell);
        //                                return;

        //                            case ItemEffect.BlueSlot:
        //                                MoveItem(GameScene.Game.NPCWeaponCraftBox.BlueCell);
        //                                return;

        //                            case ItemEffect.RedSlot:
        //                                MoveItem(GameScene.Game.NPCWeaponCraftBox.RedCell);
        //                                return;

        //                            case ItemEffect.PurpleSlot:
        //                                MoveItem(GameScene.Game.NPCWeaponCraftBox.PurpleCell);
        //                                return;

        //                            case ItemEffect.GreenSlot:
        //                                MoveItem(GameScene.Game.NPCWeaponCraftBox.GreenCell);
        //                                return;

        //                            case ItemEffect.GreySlot:
        //                                MoveItem(GameScene.Game.NPCWeaponCraftBox.GreyCell);
        //                                return;
        //                        }
        //                        switch (Item.Info.ItemType)
        //                        {
        //                            case ItemType.Weapon:
        //                                MoveItem(GameScene.Game.NPCWeaponCraftBox.TemplateCell);
        //                                return;
        //                        }
        //                        GameScene.Game.ReceiveChat($"DXItemCell.WeaponCraft".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCItemFragmentBox.IsVisible)
        //                    {
        //                        if (!Item.CanFragment())
        //                            GameScene.Game.ReceiveChat($"DXItemCell.ItemFragment".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        else MoveItem(GameScene.Game.NPCItemFragmentBox.Grid);

        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCAccessoryLevelBox.IsVisible)
        //                    {
        //                        if (GameScene.Game.NPCAccessoryLevelBox.TargetCell.Grid[0].Link == null)
        //                        {
        //                            if (!MoveItem(GameScene.Game.NPCAccessoryLevelBox.TargetCell))
        //                                GameScene.Game.ReceiveChat($"DXItemCell.AccessoryLevel".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        }
        //                        else if (!MoveItem(GameScene.Game.NPCAccessoryLevelBox.Grid))
        //                            GameScene.Game.ReceiveChat($"DXItemCell.AccessoryLevelBox".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);

        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCAccessoryUpgradeBox.IsVisible)
        //                    {
        //                        if (!Item.CanAccessoryUpgrade())
        //                            GameScene.Game.ReceiveChat($"DXItemCell.AccessoryUpgrade".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        else
        //                            MoveItem(GameScene.Game.NPCAccessoryUpgradeBox.TargetCell);

        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCAccessoryResetBox.IsVisible)
        //                    {
        //                        if (!MoveItem(GameScene.Game.NPCAccessoryResetBox.AccessoryGrid))
        //                            GameScene.Game.ReceiveChat($"DXItemCell.AccessoryRese".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);

        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCRefineBox.IsVisible)
        //                    {
        //                        switch (Item.Info.ItemType)
        //                        {
        //                            case ItemType.Ore:
        //                                if (Item.Info.Effect != ItemEffect.BlackIronOre)
        //                                    GameScene.Game.ReceiveChat($"只能使用黑铁矿".Lang(), MessageType.System);
        //                                else
        //                                    MoveItem(GameScene.Game.NPCRefineBox.BlackIronGrid);
        //                                return;

        //                            case ItemType.Necklace:
        //                            case ItemType.Bracelet:
        //                            case ItemType.Ring:
        //                                MoveItem(GameScene.Game.NPCRefineBox.AccessoryGrid);
        //                                return;

        //                            case ItemType.RefineSpecial:
        //                                MoveItem(GameScene.Game.NPCRefineBox.SpecialGrid);
        //                                return;
        //                        }
        //                        GameScene.Game.ReceiveChat($"DXItemCell.Refine".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.MarketConsignBox.IsVisible)
        //                    {
        //                        MoveItem(GameScene.Game.MarketConsignBox.ConsignGrid);
        //                        return;
        //                    }


        //                    if (GameScene.Game.CommunicationBox.IsVisible)
        //                    {
        //                        MoveItem(GameScene.Game.CommunicationBox.Grid);
        //                        return;
        //                    }

        //                    if (GameScene.Game.StorageBox.IsVisible)
        //                    {
        //                        if (!MoveItem(GameScene.Game.StorageBox.Grid))
        //                            GameScene.Game.ReceiveChat("仓库空间不足".Lang(), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.TradeBox.IsVisible)
        //                    {
        //                        if (!MoveItem(GameScene.Game.TradeBox.UserGrid))
        //                            GameScene.Game.ReceiveChat("无法交易此物品".Lang(), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.GuildBox.StorageTab.IsVisible)
        //                    {
        //                        if (!MoveItem(GameScene.Game.GuildBox.StorageGrid))
        //                            GameScene.Game.ReceiveChat("无法将此物品存储到行会仓库中".Lang(), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.CompanionBox.IsVisible)
        //                    {
        //                        if (!MoveItem(GameScene.Game.CompanionBox.InventoryGrid))
        //                            GameScene.Game.ReceiveChat("宠物的包裹没有可用空间".Lang(), MessageType.System);
        //                        return;
        //                    }

        //                    UseItem();
        //                    break;
        //                case GridType.PatchGrid:
        //                    if (Item == null) return;

        //                    if (GameScene.Game.MarketConsignBox.IsVisible)  //移动到商城
        //                    {
        //                        MoveItem(GameScene.Game.MarketConsignBox.ConsignGrid);
        //                        return;
        //                    }

        //                    if (GameScene.Game.CommunicationBox.IsVisible)       //移动到邮件
        //                    {
        //                        MoveItem(GameScene.Game.CommunicationBox.Grid);
        //                        return;
        //                    }

        //                    if (GameScene.Game.StorageBox.IsVisible)  //移动到仓库
        //                    {
        //                        if (!MoveItem(GameScene.Game.StorageBox.Grid))
        //                            GameScene.Game.ReceiveChat("仓库空间不足".Lang(), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.TradeBox.IsVisible)  //移动到交易
        //                    {
        //                        if (!MoveItem(GameScene.Game.TradeBox.UserGrid))
        //                            GameScene.Game.ReceiveChat("无法交易此物品".Lang(), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.GuildBox.StorageTab.IsVisible)  //移动到行会仓库
        //                    {
        //                        if (!MoveItem(GameScene.Game.GuildBox.StorageGrid))
        //                            GameScene.Game.ReceiveChat("无法将此物品存储到行会仓库中".Lang(), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.CompanionBox.IsVisible)  //移动到宠物包裹
        //                    {
        //                        if (!MoveItem(GameScene.Game.CompanionBox.InventoryGrid))
        //                            GameScene.Game.ReceiveChat("宠物的包裹没有可用空间".Lang(), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.InventoryBox.IsVisible)  //移动到包裹，注意因为碎片包裹可见 此值必为真
        //                    {
        //                        if (!MoveItem(GameScene.Game.InventoryBox.Grid))
        //                            GameScene.Game.ReceiveChat("包裹没有可用空间".Lang(), MessageType.System);
        //                        return;
        //                    }
        //                    UseItem();
        //                    break;

        //                case GridType.CompanionInventory:
        //                    if (Item == null) return;

        //                    if (GameScene.Game.NPCBookCombineBox.IsVisible)
        //                    {
        //                        if (Item.Info.ItemType != ItemType.Book)
        //                        {
        //                            break;
        //                        }
        //                        if (GameScene.Game.NPCBookCombineBox.BookGrid.Grid[0].Link == null)
        //                        {
        //                            if (!MoveItem(GameScene.Game.NPCBookCombineBox.BookGrid))
        //                            {
        //                                GameScene.Game.ReceiveChat($"DXItemCell.AccessoryUpgrade".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                            }
        //                        }
        //                        else if (!MoveItem(GameScene.Game.NPCBookCombineBox.MaterialGrid1))
        //                        {
        //                            GameScene.Game.ReceiveChat($"DXItemCell.BookCombineBox".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        }
        //                    }

        //                    if (GameScene.Game.NPCRepairBox.IsVisible)
        //                    {
        //                        if (Item.CurrentDurability >= Item.MaxDurability || !Item.Info.CanRepair)
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepair".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        else if (!MoveItem(GameScene.Game.NPCRepairBox.Grid))
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepairBox".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCSpecialRepairBox.IsVisible)
        //                    {
        //                        if (Item.CurrentDurability >= Item.MaxDurability || !Item.Info.CanRepair)
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepair".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        else if (!MoveItem(GameScene.Game.NPCSpecialRepairBox.Grid))
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepairBox".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCSellBox.IsVisible)  //NPC卖
        //                    {
        //                        if (!Item.Info.CanSell)
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRootSell".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        else if (!MoveItem(GameScene.Game.NPCSellBox.Grid))
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRootSellBox".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCRootSellBox.IsVisible)  //NPC一键出售
        //                    {
        //                        if (!Item.Info.CanSell)
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRootSell".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        else if (!MoveItem(GameScene.Game.NPCRootSellBox.Grid))
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRootSellBox".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCRefineBox.IsVisible)
        //                    {
        //                        switch (Item.Info.ItemType)
        //                        {
        //                            case ItemType.Ore:
        //                                if (Item.Info.Effect != ItemEffect.BlackIronOre)
        //                                    GameScene.Game.ReceiveChat($"只能使用黑铁矿".Lang(), MessageType.System);
        //                                else
        //                                    MoveItem(GameScene.Game.NPCRefineBox.BlackIronGrid);
        //                                return;

        //                            case ItemType.Necklace:
        //                            case ItemType.Bracelet:
        //                            case ItemType.Ring:
        //                                MoveItem(GameScene.Game.NPCRefineBox.AccessoryGrid);
        //                                return;

        //                            case ItemType.RefineSpecial:
        //                                MoveItem(GameScene.Game.NPCRefineBox.SpecialGrid);
        //                                return;
        //                        }
        //                        GameScene.Game.ReceiveChat($"DXItemCell.MasterRefine".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCMasterRefineBox.IsVisible)
        //                    {
        //                        switch (Item.Info.Effect)
        //                        {
        //                            case ItemEffect.Fragment1:
        //                                if (MoveItem(GameScene.Game.NPCMasterRefineBox.Fragment1Grid))
        //                                    return;
        //                                break;

        //                            case ItemEffect.Fragment2:
        //                                if (MoveItem(GameScene.Game.NPCMasterRefineBox.Fragment2Grid))
        //                                    return;
        //                                break;

        //                            case ItemEffect.Fragment3:
        //                                if (MoveItem(GameScene.Game.NPCMasterRefineBox.Fragment3Grid))
        //                                    return;
        //                                break;

        //                            case ItemEffect.RefinementStone:
        //                                if (MoveItem(GameScene.Game.NPCMasterRefineBox.RefinementStoneGrid))
        //                                    return;
        //                                break;
        //                        }

        //                        switch (Item.Info.ItemType)
        //                        {
        //                            case ItemType.RefineSpecial:
        //                                if (MoveItem(GameScene.Game.NPCMasterRefineBox.SpecialGrid))
        //                                    return;
        //                                break;
        //                        }
        //                        GameScene.Game.ReceiveChat($"DXItemCell.MasterRefine".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCAccessoryLevelBox.IsVisible)
        //                    {
        //                        if (GameScene.Game.NPCAccessoryLevelBox.TargetCell.Grid[0].Link == null)
        //                        {
        //                            if (!MoveItem(GameScene.Game.NPCAccessoryLevelBox.TargetCell))
        //                                GameScene.Game.ReceiveChat($"DXItemCell.AccessoryLevel".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        }
        //                        else if (!MoveItem(GameScene.Game.NPCAccessoryLevelBox.Grid))
        //                            GameScene.Game.ReceiveChat($"DXItemCell.AccessoryLevelBox".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);

        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCAccessoryUpgradeBox.IsVisible)
        //                    {
        //                        if (!Item.CanAccessoryUpgrade())
        //                            GameScene.Game.ReceiveChat($"DXItemCell.AccessoryUpgrade".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        else
        //                            MoveItem(GameScene.Game.NPCAccessoryUpgradeBox.TargetCell);

        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCAccessoryResetBox.IsVisible)
        //                    {
        //                        if (!MoveItem(GameScene.Game.NPCAccessoryResetBox.AccessoryGrid))
        //                            GameScene.Game.ReceiveChat($"DXItemCell.AccessoryRese".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);

        //                        return;
        //                    }

        //                    if (GameScene.Game.MarketConsignBox.IsVisible)
        //                    {
        //                        MoveItem(GameScene.Game.MarketConsignBox.ConsignGrid);
        //                        return;
        //                    }

        //                    if (GameScene.Game.CommunicationBox.IsVisible)
        //                    {
        //                        MoveItem(GameScene.Game.CommunicationBox.Grid);
        //                        return;
        //                    }

        //                    if (GameScene.Game.StorageBox.IsVisible)
        //                    {
        //                        if (!MoveItem(GameScene.Game.StorageBox.Grid))
        //                            GameScene.Game.ReceiveChat("仓库空间不足".Lang(), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.TradeBox.IsVisible)
        //                    {
        //                        if (!MoveItem(GameScene.Game.TradeBox.UserGrid))
        //                            GameScene.Game.ReceiveChat("无法交易此物品".Lang(), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.GuildBox.StorageTab.IsVisible)
        //                    {
        //                        if (!MoveItem(GameScene.Game.GuildBox.StorageGrid))
        //                            GameScene.Game.ReceiveChat("无法将此物品存储到行会仓库中".Lang(), MessageType.System);
        //                        return;
        //                    }

        //                    if (Item.Info.ItemType == ItemType.ItemPart)
        //                    {
        //                        if (!MoveItem(GameScene.Game.InventoryBox.PatchGrid))
        //                            GameScene.Game.ReceiveChat("碎片包空间不足转存包裹中".Lang(), MessageType.System);
        //                        return;
        //                    }

        //                    if (!MoveItem(GameScene.Game.InventoryBox.Grid))
        //                        GameScene.Game.ReceiveChat("包裹没有可用空间".Lang(), MessageType.System);

        //                    break;

        //                case GridType.Storage:
        //                    if (Item == null) return;

        //                    if (GameScene.Game.NPCBookCombineBox.IsVisible)
        //                    {
        //                        if (Item.Info.ItemType != ItemType.Book)
        //                        {
        //                            break;
        //                        }
        //                        if (GameScene.Game.NPCBookCombineBox.BookGrid.Grid[0].Link == null)
        //                        {
        //                            if (!MoveItem(GameScene.Game.NPCBookCombineBox.BookGrid))
        //                            {
        //                                GameScene.Game.ReceiveChat($"DXItemCell.AccessoryUpgrade".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                            }
        //                        }
        //                        else if (!MoveItem(GameScene.Game.NPCBookCombineBox.MaterialGrid1))
        //                        {
        //                            GameScene.Game.ReceiveChat($"DXItemCell.BookCombineBox".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        }
        //                    }

        //                    if (GameScene.Game.NPCRepairBox.Visible)
        //                    {
        //                        if (Item.CurrentDurability >= Item.MaxDurability || !Item.Info.CanRepair)
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepair".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        else if (!MoveItem(GameScene.Game.NPCRepairBox.Grid))
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepairBox".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCSpecialRepairBox.Visible)
        //                    {
        //                        if (Item.CurrentDurability >= Item.MaxDurability || !Item.Info.CanRepair)
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepair".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        else if (!MoveItem(GameScene.Game.NPCSpecialRepairBox.Grid))
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepairBox".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCMasterRefineBox.IsVisible)
        //                    {
        //                        switch (Item.Info.Effect)
        //                        {
        //                            case ItemEffect.Fragment1:
        //                                if (MoveItem(GameScene.Game.NPCMasterRefineBox.Fragment1Grid))
        //                                    return;
        //                                break;

        //                            case ItemEffect.Fragment2:
        //                                if (MoveItem(GameScene.Game.NPCMasterRefineBox.Fragment2Grid))
        //                                    return;
        //                                break;

        //                            case ItemEffect.Fragment3:
        //                                if (MoveItem(GameScene.Game.NPCMasterRefineBox.Fragment3Grid))
        //                                    return;
        //                                break;

        //                            case ItemEffect.RefinementStone:
        //                                if (MoveItem(GameScene.Game.NPCMasterRefineBox.RefinementStoneGrid))
        //                                    return;
        //                                break;
        //                        }

        //                        switch (Item.Info.ItemType)
        //                        {
        //                            case ItemType.RefineSpecial:
        //                                if (MoveItem(GameScene.Game.NPCMasterRefineBox.SpecialGrid))
        //                                    return;
        //                                break;
        //                        }
        //                        GameScene.Game.ReceiveChat($"DXItemCell.Refine".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }
        //                    if (GameScene.Game.NPCRefineBox.Visible)
        //                    {
        //                        switch (Item.Info.ItemType)
        //                        {
        //                            case ItemType.Ore:
        //                                if (Item.Info.Effect != ItemEffect.BlackIronOre)
        //                                    GameScene.Game.ReceiveChat($"只能使用黑铁矿".Lang(), MessageType.System);
        //                                else
        //                                    MoveItem(GameScene.Game.NPCRefineBox.BlackIronGrid);
        //                                return;

        //                            case ItemType.Necklace:
        //                            case ItemType.Bracelet:
        //                            case ItemType.Ring:
        //                                MoveItem(GameScene.Game.NPCRefineBox.AccessoryGrid);
        //                                return;
        //                        }
        //                        GameScene.Game.ReceiveChat($"DXItemCell.MasterRefine".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }
        //                    if (GameScene.Game.MarketConsignBox.IsVisible)
        //                    {
        //                        MoveItem(GameScene.Game.MarketConsignBox.ConsignGrid);
        //                        return;
        //                    }
        //                    if (Item.Info.ItemType == ItemType.ItemPart)
        //                    {
        //                        if (!MoveItem(GameScene.Game.InventoryBox.PatchGrid))
        //                            GameScene.Game.ReceiveChat("碎片包空间不足转存包裹中", MessageType.System);
        //                        return;
        //                    }

        //                    MoveItem(GameScene.Game.InventoryBox.Grid, true);
        //                    return;

        //                case GridType.GuildStorage:
        //                    if (Item == null) return;

        //                    if (GameScene.Game.NPCRepairBox.Visible)
        //                    {
        //                        if (Item.CurrentDurability >= Item.MaxDurability || !Item.Info.CanRepair)
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepair".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        else if (!MoveItem(GameScene.Game.NPCRepairBox.Grid))
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepairBox".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCSpecialRepairBox.Visible)
        //                    {
        //                        if (Item.CurrentDurability >= Item.MaxDurability || !Item.Info.CanRepair)
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepair".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        else if (!MoveItem(GameScene.Game.NPCSpecialRepairBox.Grid))
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepairBox".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.MarketConsignBox.IsVisible)
        //                    {
        //                        MoveItem(GameScene.Game.MarketConsignBox.ConsignGrid);
        //                        return;
        //                    }
        //                    if (Item.Info.ItemType == ItemType.ItemPart)
        //                    {
        //                        if (!MoveItem(GameScene.Game.InventoryBox.PatchGrid))
        //                            GameScene.Game.ReceiveChat("碎片包空间不足转存包裹中", MessageType.System);
        //                        return;
        //                    }

        //                    MoveItem(GameScene.Game.InventoryBox.Grid, true);
        //                    return;

        //                case GridType.Equipment:

        //                    if (Item == null) return;

        //                    if (GameScene.Game.NPCRepairBox.Visible)
        //                    {
        //                        if (Item.CurrentDurability >= Item.MaxDurability || !Item.Info.CanRepair)
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepair".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        else if (!MoveItem(GameScene.Game.NPCRepairBox.Grid))
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepairBox".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCSpecialRepairBox.Visible)
        //                    {
        //                        if (Item.CurrentDurability >= Item.MaxDurability || !Item.Info.CanRepair)
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepair".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        else if (!MoveItem(GameScene.Game.NPCSpecialRepairBox.Grid))
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepairBox".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCAccessoryLevelBox.IsVisible)
        //                    {
        //                        if (GameScene.Game.NPCAccessoryLevelBox.TargetCell.Grid[0].Link == null)
        //                        {
        //                            if (!MoveItem(GameScene.Game.NPCAccessoryLevelBox.TargetCell))
        //                                GameScene.Game.ReceiveChat($"DXItemCell.AccessoryLevel".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        }
        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCAccessoryUpgradeBox.IsVisible)
        //                    {
        //                        if (!Item.CanAccessoryUpgrade())
        //                            GameScene.Game.ReceiveChat($"DXItemCell.AccessoryUpgrade".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        else
        //                            MoveItem(GameScene.Game.NPCAccessoryUpgradeBox.TargetCell);

        //                        return;
        //                    }

        //                    if (Item != null && (Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage)
        //                    {
        //                        if (e.Button == MouseButtons.Right)
        //                            CEnvir.Enqueue(new C.MarriageTeleport());
        //                        return;
        //                    }

        //                    if (!MoveItem(GameScene.Game.InventoryBox.Grid))
        //                        GameScene.Game.ReceiveChat("包裹没有可用空间", MessageType.System);

        //                    break;

        //                case GridType.CompanionEquipment:

        //                    if (Item == null) return;

        //                    if (GameScene.Game.NPCRepairBox.Visible)
        //                    {
        //                        if (Item.CurrentDurability >= Item.MaxDurability || !Item.Info.CanRepair)
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepair".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        else if (!MoveItem(GameScene.Game.NPCRepairBox.Grid))
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepairBox".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCSpecialRepairBox.Visible)
        //                    {
        //                        if (Item.CurrentDurability >= Item.MaxDurability || !Item.Info.CanRepair)
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepair".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        else if (!MoveItem(GameScene.Game.NPCSpecialRepairBox.Grid))
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepairBox".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }

        //                    if (!MoveItem(GameScene.Game.InventoryBox.Grid))
        //                        GameScene.Game.ReceiveChat("包裹没有可用空间", MessageType.System);

        //                    break;

        //                case GridType.FishingEquipment:

        //                    if (Item == null) return;

        //                    if (GameScene.Game.NPCRepairBox.Visible)
        //                    {
        //                        if (Item.CurrentDurability >= Item.MaxDurability || !Item.Info.CanRepair)
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepair".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        else if (!MoveItem(GameScene.Game.NPCRepairBox.Grid))
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepairBox".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }

        //                    if (GameScene.Game.NPCSpecialRepairBox.Visible)
        //                    {
        //                        if (Item.CurrentDurability >= Item.MaxDurability || !Item.Info.CanRepair)
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepair".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        else if (!MoveItem(GameScene.Game.NPCSpecialRepairBox.Grid))
        //                            GameScene.Game.ReceiveChat($"DXItemCell.InventoryRepairBox".Lang(Item.Info.Lang(p => p.ItemName)), MessageType.System);
        //                        return;
        //                    }

        //                    if (!MoveItem(GameScene.Game.InventoryBox.Grid))
        //                        GameScene.Game.ReceiveChat("包裹没有可用空间", MessageType.System);

        //                    break;

        //                    //default:
        //                    //throw new ArgumentOutOfRangeException();
        //            }
        //            #endregion

        //            break;
        //    }
        //}

        /// <summary>
        /// 鼠标双击时
        /// </summary>
        /// <param name="e"></param>
        //public override void OnMouseDoubleClick(MouseEventArgs e)
        //{
        //    //声望称号 鼠标点击无效
        //    if ((GridType == GridType.Equipment || GridType == GridType.Inspect) && Slot == (int)EquipmentSlot.FameTitle) return;

        //    if (Locked || GameScene.Game.GoldPickedUp || (!Linked && Link != null) || GameScene.Game.Observer) return;

        //    base.OnMouseDoubleClick(e);

        //    if (ReadOnly || e.Button != MouseButtons.Left) return;

        //    switch (GridType)
        //    {
        //        case GridType.Belt:       //快捷栏
        //        case GridType.AutoPotion:  //自动药水

        //            UseItem();             //使用物品就返回
        //            break;

        //        case GridType.Inventory:          //包裹
        //        case GridType.PatchGrid:    //碎片包裹
        //        case GridType.CompanionInventory:   //宠物包裹
        //        case GridType.CompanionEquipment:   //宠物道具框

        //            UseItem();                //使用物品就返回
        //            return;
        //        case GridType.FishingEquipment:   //钓鱼道具框

        //            UseItem();                //使用物品就返回
        //            return;
        //        case GridType.Storage:           //仓库

        //            UseItem();                //使用物品就返回
        //            return;

        //        case GridType.Equipment:        //装备栏
        //            if (Item == null) return;     //如果道具为空返回

        //            if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage)
        //                CEnvir.Enqueue(new C.MarriageTeleport());

        //            break;
        //    }
        //}

        //public override void OnTouchMoved(TouchEventArgs e)
        //{
        //    base.OnTouchMoved(e);
        //}

        /// <summary>
        /// 鼠标单击时
        /// </summary>
        /// <param name="e"></param>
        //public override void OnDragComplete(TouchEventArgs e)
        //{
        //    if (SelectedCell == null) return;

        //    //声望称号 鼠标点击无效
        //    if ((GridType == GridType.Equipment || GridType == GridType.Inspect) && Slot == (int)EquipmentSlot.FameTitle) return;

        //    if (Locked || GameScene.Game.GoldPickedUp || (!Linked && Link != null) || GameScene.Game.Observer) return;

        //    //base.OnDragComplete(e);

        //    if (ReadOnly || !Enabled) return;

        //    if (Linked && Link != null)
        //    {
        //        Link = null;

        //        if (SelectedCell == null)
        //            return;
        //    }

        //    if (Item != null && SelectedCell == null)
        //    {
        //        PlayItemSound();
        //    }
        //    MoveItem();
        //}

        /// <summary>
        /// 鼠标单击时
        /// </summary>
        /// <param name="e"></param>
        public override void OnTap(TouchEventArgs e)
        {
            //快捷栏已改为锁定药品，单击退出
            if (GridType == GridType.Belt && SelectedCell == null) return;

            //声望称号 鼠标点击无效
            if ((GridType == GridType.Equipment || GridType == GridType.Inspect) && Slot == (int)EquipmentSlot.FameTitle) return;

            if (Locked || GameScene.Game.GoldPickedUp || (!Linked && Link != null) || GameScene.Game.Observer) return;

            base.OnTap(e);

            if (ReadOnly || !Enabled) return;

            if (Linked && Link != null)
            {
                Link = null;

                if (SelectedCell == null)
                    return;
            }

            if (Item != null && SelectedCell == null)
            {
                PlayItemSound();
            }
            MoveItem();
        }

        /// <summary>
        /// 鼠标双击时
        /// </summary>
        /// <param name="e"></param>
        public override void OnTouchUp(TouchEventArgs e)
        {
            if (GridType != GridType.Belt) return;

            if (Locked || GameScene.Game.GoldPickedUp || (!Linked && Link != null) || GameScene.Game.Observer) return;

            base.OnTouchUp(e);

            if (ReadOnly || !Enabled) return;

            if (Item == null) return;
            //药品快捷栏单机使用药品
            if (GridType == GridType.Belt)
                UseItem();
        }

        /// <summary>
        /// 鼠标双击时
        /// </summary>
        /// <param name="e"></param>
        public override void OnDoubleTap(TouchEventArgs e)
        {
            if (SelectedCell != null)
            {
                //把物品放回原处
                if (SelectedCell == this || SelectedCell.Item == null)
                    SelectedCell = null;
            }

            //声望称号 鼠标点击无效
            if ((GridType == GridType.Equipment || GridType == GridType.Inspect) && Slot == (int)EquipmentSlot.FameTitle) return;

            if (Locked || GameScene.Game.GoldPickedUp || (!Linked && Link != null) || GameScene.Game.Observer) return;

            base.OnDoubleTap(e);

            if (ReadOnly) return;

            switch (GridType)
            {
                case GridType.Belt:       //快捷栏
                case GridType.AutoPotion:  //自动药水

                    //UseItem();             //使用物品就返回
                    break;

                case GridType.Inventory:          //包裹
                case GridType.PatchGrid:    //碎片包裹
                case GridType.CompanionInventory:   //宠物包裹
                case GridType.CompanionEquipment:   //宠物道具框

                    UseItem();                //使用物品就返回
                    return;
                case GridType.FishingEquipment:   //钓鱼道具框

                    UseItem();                //使用物品就返回
                    return;
                case GridType.Storage:           //仓库

                    UseItem();                //使用物品就返回
                    return;

                case GridType.Equipment:        //装备栏
                    if (Item == null) return;     //如果道具为空返回

                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage)
                        CEnvir.Enqueue(new C.MarriageTeleport());

                    break;
            }
        }

        public override void OnCheckHold(TouchEventArgs e)
        {
            //声望称号 鼠标点击无效
            if ((GridType == GridType.Equipment || GridType == GridType.Inspect) && Slot == (int)EquipmentSlot.FameTitle) return;

            if (Locked || GameScene.Game.GoldPickedUp || (!Linked && Link != null) || GameScene.Game.Observer) return;

            base.OnCheckHold(e);

            if (ReadOnly || !Enabled) return;

            if (Linked && Link != null)
            {
                Link = null;

                if (SelectedCell == null)
                    return;
            }

            //快捷栏 长按取消快捷
            if (GridType == GridType.Belt && Item != null)
            {
                QuickInfo = null;
                QuickItem = null;
                var link = GameScene.Game.BeltBox.Links[Slot];
                CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex });
                if (Selected) SelectedCell = null;
                return;
            }

            if (Item != null)  //如果道具不等空
            {
                //如果道具是武器 Shape值是125  或者  道具是武器 Shape值是126
                if (Item.Info.ItemType == ItemType.Weapon && Item.Info.Shape == 125 || Item.Info.ItemType == ItemType.Weapon && Item.Info.Shape == 126)
                {
                    if (GameScene.Game?.FishingBox != null)            //如果钓鱼装备面板不为空 呼出
                    {
                        if (GameScene.Game.FishingBox.Visible)
                        {
                            GameScene.Game.FishingBox.Hide();
                        }
                        else GameScene.Game.FishingBox.Show();
                    }
                }
            }
            bool flag = true;
            //如果背包满不让打开拆分面板
            if (GridType == GridType.Inventory)
            {
                bool full = true;
                foreach (var item in GameScene.Game.InventoryBox.Grid.ItemGrid)
                {
                    if (item == null)
                    {
                        full = false;
                        break;
                    }
                }
                if (full) flag = false;
            }

            int storagetotal = 0;

            //如果仓库满不让打开拆分面板
            if (GridType == GridType.Storage)
            {
                foreach (var item1 in GameScene.Game.StorageBox.Grid.ItemGrid)
                {
                    if (item1 != null)
                    {
                        storagetotal++;
                    }
                }
            }
            if (GameScene.Game.StorageSize <= storagetotal) flag = false;

            //如果道具等空 或 道具的格子不等包裹 仓库 行会仓库 宠物包裹 碎片包裹 或道具数量小于等于1 跳出
            if (Item == null || (GridType != GridType.Inventory && GridType != GridType.Storage && GridType != GridType.GuildStorage && GridType != GridType.CompanionInventory && GridType != GridType.PatchGrid) || Item.Count <= 1) return;
            if (!flag)
            {
                GameScene.Game.ReceiveChat("格子已满，请先整理下格子", MessageType.System);
                return;
            }
            new DXInputWindow(Item, (amount) =>
            {
                if (amount <= 0 || amount >= Item.Count) return;

                Locked = true;
                CEnvir.Enqueue(new C.ItemSplit { Grid = GridType, Slot = Slot, Count = amount });
            }, Item.Count, "道具拆分".Lang());
        }

        #endregion Methods

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _FixedBorder = false;
                _FixedBorderColour = false;
                _GridType = GridType.None;
                _HostGrid = null;
                _ItemGrid = null;
                _Locked = false;
                _ReadOnly = false;
                _Selected = false;
                _Slot = 0;
                _ShowCountLabel = false;
                QuickInfoItem = null;
                _QuickInfo = null;
                _QuickItem = null;

                DXItemCell oldLink = _Link;

                _Link = null;
                if (oldLink != null)
                    oldLink.Link = null;

                _LinkedCount = 0;
                _Linked = false;
                _AllowLink = false;

                FixedBorderChanged = null;
                FixedBorderColourChanged = null;
                GridTypeChanged = null;
                HostGridChanged = null;
                ItemChanged = null;
                ItemGridChanged = null;
                LockedChanged = null;
                ReadOnlyChanged = null;
                SelectedChanged = null;
                SlotChanged = null;
                ShowCountLabelChanged = null;
                LinkChanged = null;
                LinkedCountChanged = null;
                LinkedChanged = null;
                AllowLinkChanged = null;

                if (CountLabel != null)
                {
                    if (!CountLabel.IsDisposed)
                        CountLabel.Dispose();

                    CountLabel = null;
                }

                if (LevelLabel != null)
                {
                    if (!LevelLabel.IsDisposed)
                        LevelLabel.Dispose();

                    LevelLabel = null;
                }
            }

            if (SelectedCell == this) SelectedCell = null;
        }
        #endregion IDisposable
    }

    /// <summary>
    /// 绘制道具类型
    /// </summary>
    public enum DrawItemType
    {
        /// <summary>
        /// 绘制道具类型全部
        /// </summary>
        All,
        /// <summary>
        /// 绘制道具类型只有图像
        /// </summary>
        OnlyImage,
        /// <summary>
        /// 绘制道具类型空的
        /// </summary>
        Null,
    }
}
