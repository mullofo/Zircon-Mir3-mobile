using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.UserModels;
using Library;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Text;
using C = Library.Network.ClientPackets;
using Font = MonoGame.Extended.Font;
using FontStyle = MonoGame.Extended.FontStyle;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 寄售商城功能
    /// </summary>
    public sealed class AccountConsignmentDialog : DXWindow
    {
        #region Properites

        public DXImageControl AccountBackGround;

        #region 寄售账号
        public DXTextBox ItemNameBox;
        public DXComboBox ItemTypeBox;
        public DXButton Close1Button, ClearButton, BuyButton, SearchButton, NameSearchButton, PreviousButton, NextButton;
        public DXLabel PageValue;
        #endregion

        #region 上架账号
        public DXTextBox ConsignCostBox;
        public DXButton ConsignButton;
        #endregion

        public AccountConsignmentRow[] SearchRows;
        public ClientAccountConsignmentInfo[] SearchResults;
        public DateTime NextSearchTime;

        /// <summary>
        /// 分类页，当前页
        /// </summary>
        public int PageIndex { get; set; } = 1;

        private int _currentPage = 1;
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage
        {
            get => _currentPage; set
            {
                _currentPage = value;
                PageValue.Text = $@"{value} / {TotalPage}";
            }
        }

        private int _totalPage;
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage
        {
            get => _totalPage;
            set
            {
                if (value == _totalPage) return;
                _totalPage = value;
                PageValue.Text = $@"{CurrentPage} / {value}";
            }
        }

        /// <summary>
        /// 价格
        /// </summary>
        public long Price
        {
            get => _Price;
            set
            {
                if (_Price == value) return;

                long oldValue = _Price;
                _Price = value;

                OnPriceChanged(oldValue, value);
            }
        }
        private long _Price;
        public event EventHandler<EventArgs> PriceChanged;
        public void OnPriceChanged(long oValue, long nValue)
        {
            PriceChanged?.Invoke(this, EventArgs.Empty);
        }

        #region SelectedRow

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);
            if (nValue)
            {
                Search();
            }
        }
        /// <summary>
        /// 选定行
        /// </summary>
        public AccountConsignmentRow SelectedRow
        {
            get => _SelectedRow;
            set
            {
                if (_SelectedRow == value) return;

                AccountConsignmentRow oldValue = _SelectedRow;
                _SelectedRow = value;

                OnSelectedRowChanged(oldValue, value);
            }
        }
        private AccountConsignmentRow _SelectedRow;
        public event EventHandler<EventArgs> SelectedRowChanged;
        public void OnSelectedRowChanged(AccountConsignmentRow oValue, AccountConsignmentRow nValue)
        {
            if (oValue != null)
                oValue.Selected = false;

            if (nValue != null)
                nValue.Selected = true;

            SelectedRowChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);

            if (SearchRows == null) return;

            if (SearchResults == null)
                Search();
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// 账号寄售面板
        /// </summary>
        public AccountConsignmentDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            IgnoreMoveBounds = true;
            CloseButton.Visible = false;
            Opacity = 0F;

            Size = new Size(700, 364);
            Location = ClientArea.Location;

            AccountBackGround = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 1092,
                IsControl = true,
                PassThrough = true,
                ImageOpacity = 0.85F,
                Location = new Point(0, 0)
            };

            Close1Button = new DXButton
            {
                Parent = this,
                Index = 1221,
                LibraryFile = LibraryFile.UI1,
                Hint = "关闭",
            };
            Close1Button.Location = new Point(Size.Width - Close1Button.Size.Width - 8, Size.Height - 47);
            Close1Button.MouseClick += (o, e) => Visible = false;

            DXLabel label = new DXLabel       //标题标签
            {
                Text = "角色寄售行",
                Parent = this,
                Location = new Point(313, 15),
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };

            label = new DXLabel
            {
                Parent = this,
                Location = new Point(15, 52),
                AutoSize = false,
                Size = new Size(105, 18),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
                Text = "角色名",
            };

            label = new DXLabel
            {
                Parent = this,
                Location = new Point(120, 52),
                AutoSize = false,
                Size = new Size(60, 18),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
                Text = "职业",
            };

            label = new DXLabel
            {
                Parent = this,
                Location = new Point(180, 52),
                AutoSize = false,
                Size = new Size(60, 18),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
                Text = "等级",
            };

            label = new DXLabel
            {
                Parent = this,
                Location = new Point(240, 52),
                AutoSize = false,
                Size = new Size(60, 18),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
                Text = "性别",
            };

            label = new DXLabel
            {
                Parent = this,
                Location = new Point(300, 52),
                AutoSize = false,
                Size = new Size(60, 18),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
                Text = "坐骑",
            };

            label = new DXLabel
            {
                Parent = this,
                Location = new Point(360, 52),
                AutoSize = false,
                Size = new Size(80, 18),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
                Text = "忠诚度",
            };

            label = new DXLabel
            {
                Parent = this,
                Location = new Point(443, 52),
                AutoSize = false,
                Size = new Size(80, 18),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
                Text = "售价",
            };

            label = new DXLabel
            {
                Parent = this,
                Location = new Point(525, 52),
                AutoSize = false,
                Size = new Size(160, 18),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
                Text = "查询",
            };

            #region 寄售

            ItemNameBox = new DXTextBox
            {
                Parent = this,
                Size = new Size(135, 20),
                Location = new Point(270, 289),
                Border = false,
            };
            ItemNameBox.TextBox.KeyPress += TextBox_KeyPress;

            ItemTypeBox = new DXComboBox
            {
                Parent = this,
                Location = new Point(270, 289),
                Size = new Size(135, DXComboBox.DefaultNormalHeight),
                Border = false,
                DropDownHeight = 100
            };

            new DXListBoxItem
            {
                Parent = ItemTypeBox.ListBox,
                Label = { Text = $"全部".Lang() },
                Item = null
            };

            Type type = typeof(MirClass);

            for (MirClass i = MirClass.Warrior; i <= MirClass.Taoist; i++)
            {
                new DXListBoxItem
                {
                    Parent = ItemTypeBox.ListBox,
                    Label = { Text = i.Lang() },
                    Item = i
                };
            }

            ItemTypeBox.ListBox.SelectItem(null);

            SearchButton = new DXButton   //按职业搜索
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1900,
                Location = new Point(412, 283),
                Hint = "按职业搜索".Lang(),
            };
            SearchButton.MouseClick += (o, e) => Search();

            NameSearchButton = new DXButton   //按名字搜索
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1900,
                Location = new Point(412, 313),
                Hint = "按角色名搜索".Lang(),
            };
            NameSearchButton.MouseClick += (o, e) => Search();

            PreviousButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1894,
                Parent = this,
                Location = new Point(26, 295),
                Hint = "上一页".Lang(),
            };
            PreviousButton.MouseClick += PreviousButton_MouseClick;

            NextButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1896,
                Parent = this,
                Location = new Point(77, 295),
                Hint = "下一页".Lang(),
            };
            NextButton.MouseClick += NextButton_MouseClick;

            PageValue = new DXLabel
            {
                Parent = this,
                IsControl = false,
                AutoSize = false,
                Size = new Size(104, 23),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
                Location = new Point(10, 12),
            };

            ClearButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1898,
                Location = new Point(126, 295),
                Hint = "刷新".Lang(),
            };
            ClearButton.MouseClick += (o, e) =>
            {
                ItemNameBox.TextBox.Text = "";
                ItemTypeBox.ListBox.SelectItem(null);
                Search();
            };

            BuyButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1906,
                Location = new Point(176, 291),
                Hint = "购买".Lang(),
            };
            BuyButton.MouseClick += BuyButton_MouseClick;

            SearchRows = new AccountConsignmentRow[5];

            for (int i = 0; i < SearchRows.Length; i++)
            {
                var row = new AccountConsignmentRow
                {
                    Parent = this,
                    Location = new Point(15, 88 + i * 34),
                };
                row.MouseClick += (o, e) => { SelectedRow = row; };
                SearchRows[i] = row;
            }

            #endregion

            #region 委托

            label = new DXLabel
            {
                Parent = this,
                Location = new Point(505, 282),
                AutoSize = false,
                Size = new Size(102, 18),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
                Text = "当前角色上架",
            };

            ConsignCostBox = new DXTextBox
            {
                Location = new Point(513, 309),
                Size = new Size(87, 19),
                Parent = this,
                Border = false,
            };
            ConsignCostBox.TextBox.TextChanged += (o, e) =>
            {
                long price;
                long.TryParse(ConsignCostBox.TextBox.Text, out price);

                Price = price;
            };

            ConsignButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1904,
                Location = new Point(623, 291),
                Hint = "出售当前角色(赞助币)".Lang(),
            };
            ConsignButton.MouseClick += ConsignButton_MouseClick;

            #endregion

        }

        #region Methods
        private void NextButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (CurrentPage + 1 > TotalPage) return;
            CurrentPage += 1;
            RefreshConsignList(CurrentPage);
        }

        private void PreviousButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (CurrentPage - 1 < 1) return;
            CurrentPage -= 1;
            RefreshConsignList(CurrentPage);
        }
        /// <summary>
        /// 搜索
        /// </summary>
        public void Search()
        {
            SearchResults = null;

            //SearchScrollBar.MaxValue = 0;

            foreach (AccountConsignmentRow row in SearchRows)
            {
                row.Loading = true;
                row.Visible = true;
            }

            CEnvir.Enqueue(new C.SellCharacterSearch
            {

            });
        }
        /// <summary>
        /// 刷新列表
        /// </summary>
        public void RefreshConsignList(int page = 1)
        {
            if (SearchResults == null && page > TotalPage) return;

            var Value = (page - 1) * 5;

            for (int i = 0; i < SearchRows.Length; i++)
            {
                if (i + Value >= SearchResults.Length)
                {
                    SearchRows[i].MarketInfo = null;
                    SearchRows[i].Loading = false;
                    SearchRows[i].Visible = false;
                    continue;
                }

                if (SearchResults[i + Value] == null)
                {
                    SearchRows[i].Loading = true;
                    SearchRows[i].Visible = true;
                    //SearchResults[i + Value] = new ClientAccountConsignmentInfo { Loading = true };
                    //CEnvir.Enqueue(new C.MarketPlaceSearchIndex { Index = i + Value });
                    continue;
                }

                //if (SearchResults[i + Value].Loading) continue;

                SearchRows[i].Loading = false;
                SearchRows[i].MarketInfo = SearchResults[i + Value];
            }

        }
        /// <summary>
        /// 鼠标点击购买按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuyButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (SelectedRow?.MarketInfo == null)
            {
                return;
            }
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("角色名：" + SelectedRow.MarketInfo.Name);
            stringBuilder.Append("\n\n");
            stringBuilder.Append("等级：" + SelectedRow.MarketInfo.Level);
            stringBuilder.Append("\n\n");
            stringBuilder.Append("职业：" + SelectedRow.MarketInfo.Class.Lang());
            stringBuilder.Append("\n\n");
            stringBuilder.Append("总费用" + ": " + SelectedRow.MarketInfo.Price);

            new DXConfirmWindow(stringBuilder.ToString(), DXMessageBoxButtons.YesNo, delegate
            {
                //BuyButton.Enabled = false;
                CEnvir.Enqueue(new C.MarketPlaceJiaoseBuy
                {
                    Index = SelectedRow.MarketInfo.Index,
                    Count = SelectedRow.MarketInfo.Price
                });
            });
        }
        /// <summary>
        /// 鼠标点击寄售按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConsignButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (Price == 0) return;

            CEnvir.Enqueue(new C.SellCharacter
            {
                Price = Price,
            });
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
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                #region 寄售

                if (ItemNameBox != null)
                {
                    if (!ItemNameBox.IsDisposed)
                        ItemNameBox.Dispose();

                    ItemNameBox = null;
                }

                if (ItemTypeBox != null)
                {
                    if (!ItemTypeBox.IsDisposed)
                        ItemTypeBox.Dispose();

                    ItemTypeBox = null;
                }

                if (BuyButton != null)
                {
                    if (!BuyButton.IsDisposed)
                        BuyButton.Dispose();

                    BuyButton = null;
                }

                if (SearchButton != null)
                {
                    if (!SearchButton.IsDisposed)
                        SearchButton.Dispose();

                    SearchButton = null;
                }

                if (NameSearchButton != null)
                {
                    if (!NameSearchButton.IsDisposed)
                        NameSearchButton.Dispose();

                    NameSearchButton = null;
                }

                #endregion

                #region 上架

                if (ConsignCostBox != null)
                {
                    if (!ConsignCostBox.IsDisposed)
                        ConsignCostBox.Dispose();

                    ConsignCostBox = null;
                }

                if (ConsignButton != null)
                {
                    if (!ConsignButton.IsDisposed)
                        ConsignButton.Dispose();

                    ConsignButton = null;
                }

                NextSearchTime = DateTime.MinValue;

                #endregion            
            }
        }
        #endregion
    }
    /// <summary>
    /// 角色寄售内容条
    /// </summary>
    public sealed class AccountConsignmentRow : DXControl
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
            if (Selected)
            {
                BackColour = Color.FromArgb(80, 80, 125);
                EquipmentButton.IsControl = true;
                InventoryButton.IsControl = true;
                MagicButton.IsControl = true;
            }
            else
            {
                BackColour = Color.Empty;
                EquipmentButton.IsControl = false;
                InventoryButton.IsControl = false;
                MagicButton.IsControl = false;
            }

            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region MarketInfo
        /// <summary>
        /// 寄售信息
        /// </summary>
        public ClientAccountConsignmentInfo MarketInfo
        {
            get => _MarketInfo;
            set
            {
                ClientAccountConsignmentInfo oldValue = _MarketInfo;
                _MarketInfo = value;

                OnMarketInfoChanged(oldValue, value);
            }
        }
        private ClientAccountConsignmentInfo _MarketInfo;
        public event EventHandler<EventArgs> MarketInfoChanged;
        public void OnMarketInfoChanged(ClientAccountConsignmentInfo oValue, ClientAccountConsignmentInfo nValue)
        {
            Visible = MarketInfo != null;

            if (MarketInfo == null) return;

            NameLabel.Text = MarketInfo.Name;

            Type itemType = MarketInfo.Class.GetType(); ;
            MemberInfo[] infos = itemType.GetMember(MarketInfo.Class.ToString());

            DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();

            MirClassLabel.Text = $"{description?.Description ?? MarketInfo.Class.ToString()}";

            LevelLabel.Text = $"{MarketInfo.Level}";

            Type horseType = MarketInfo.Horse.GetType(); ;
            MemberInfo[] horseinfos = horseType.GetMember(MarketInfo.Horse.ToString());

            DescriptionAttribute horsedescription = horseinfos[0].GetCustomAttribute<DescriptionAttribute>();

            HorseLabel.Text = $"{horsedescription?.Description ?? MarketInfo.Horse.ToString()}";

            BonusPoolLabel.Text = $"{MarketInfo.Myself}";

            Type genderType = MarketInfo.Gender.GetType(); ;
            MemberInfo[] genderinfos = genderType.GetMember(MarketInfo.Gender.ToString());

            DescriptionAttribute genderdescription = genderinfos[0].GetCustomAttribute<DescriptionAttribute>();

            MirGenderLabel.Text = $"{genderdescription?.Description ?? MarketInfo.Gender.ToString()}"; ;

            PriceLabel.Text = $"{MarketInfo.Price.ToString("###0")}";

            MarketInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Loading
        /// <summary>
        /// 加载
        /// </summary>
        public bool Loading
        {
            get => _Loading;
            set
            {
                if (_Loading == value) return;

                bool oldValue = _Loading;
                _Loading = value;

                OnLoadingChanged(oldValue, value);
            }
        }
        private bool _Loading;
        public event EventHandler<EventArgs> LoadingChanged;
        public void OnLoadingChanged(bool oValue, bool nValue)
        {
            MirClassLabel.Visible = !Loading;
            LevelLabel.Visible = !Loading;
            MirGenderLabel.Visible = !Loading;
            HorseLabel.Visible = !Loading;
            BonusPoolLabel.Visible = !Loading;
            PriceLabel.Visible = !Loading;
            EquipmentButton.Visible = !Loading;
            InventoryButton.Visible = !Loading;
            MagicButton.Visible = !Loading;

            if (Loading)
            {
                NameLabel.Text = "加载中".Lang();
            }
            else
                NameLabel.Text = "";

            LoadingChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXLabel NameLabel, MirClassLabel, LevelLabel, MirGenderLabel, PriceLabel, HorseLabel, BonusPoolLabel;

        public DXButton EquipmentButton, InventoryButton, MagicButton;

        #endregion

        /// <summary>
        /// 寄售内容条
        /// </summary>
        public AccountConsignmentRow()
        {
            DrawTexture = true;
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.Empty;

            Visible = false;

            var font = new Font(Config.FontName, CEnvir.FontSize(11F));
            Size = new Size(670, TextRenderer.MeasureText(null, "字", font).Height);

            NameLabel = new DXLabel   //名字
            {
                Parent = this,
                Location = new Point(2, 0),
                AutoSize = false,
                Size = new Size(100, Size.Height),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
                IsControl = false,
                Font = font,
            };

            MirClassLabel = new DXLabel   //职业
            {
                Parent = this,
                Location = new Point(104, 0),
                AutoSize = false,
                Size = new Size(60, Size.Height),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
                IsControl = false,
                Font = font,
            };

            LevelLabel = new DXLabel    //等级
            {
                Parent = this,
                Location = new Point(164, 0),
                AutoSize = false,
                Size = new Size(60, Size.Height),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
                IsControl = false,
                Font = font,
            };

            MirGenderLabel = new DXLabel   //性别
            {
                Parent = this,
                Location = new Point(226, 0),
                AutoSize = false,
                Size = new Size(60, Size.Height),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
                IsControl = false,
                Font = font,
            };

            HorseLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(285, 0),
                AutoSize = false,
                Size = new Size(60, Size.Height),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
                IsControl = false,
                Font = font,
            };

            BonusPoolLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(348, 0),
                AutoSize = false,
                Size = new Size(78, Size.Height),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
                IsControl = false,
                Font = font,
            };

            PriceLabel = new DXLabel    //价格
            {
                Parent = this,
                Location = new Point(429, 0),
                AutoSize = false,
                Size = new Size(78, Size.Height),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.Yellow,
                IsControl = false,
                Font = font,
            };

            EquipmentButton = new DXButton
            {
                Parent = this,
                Size = new Size(33, SmallButtonHeight),
                Location = new Point(518, 0),
                Label = { Text = "装备".Lang() },
                ButtonType = ButtonType.SmallButton,
                IsControl = false,
            };
            EquipmentButton.MouseClick += EquipButton_MouseClick;

            InventoryButton = new DXButton
            {
                Parent = this,
                Size = new Size(33, SmallButtonHeight),
                Location = new Point(573, 0),
                Label = { Text = "背包".Lang() },
                ButtonType = ButtonType.SmallButton,
                IsControl = false,
            };
            InventoryButton.MouseClick += PackSackButton_MouseClick;

            MagicButton = new DXButton
            {
                Parent = this,
                Size = new Size(33, SmallButtonHeight),
                Location = new Point(628, 0),
                Label = { Text = "技能".Lang() },
                ButtonType = ButtonType.SmallButton,
                IsControl = false,
            };
            MagicButton.MouseClick += MageryButton_MouseClick;
        }

        private void EquipButton_MouseClick(object sender, MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (GameScene.Game != null && MarketInfo != null)
            {
                CEnvir.Enqueue(new C.Inspect
                {
                    Index = MarketInfo.Index
                });
            }
        }

        private void PackSackButton_MouseClick(object sender, MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (GameScene.Game != null && MarketInfo != null)
            {
                CEnvir.Enqueue(new C.InspectPackSack
                {
                    Index = MarketInfo.Index
                });
            }
        }

        private void MageryButton_MouseClick(object sender, MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (GameScene.Game != null && MarketInfo != null)
            {
                CEnvir.Enqueue(new C.InspectMagery
                {
                    Index = MarketInfo.Index
                });
            }
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Selected = false;
                SelectedChanged = null;

                _Loading = false;
                LoadingChanged = null;

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (MirClassLabel != null)
                {
                    if (!MirClassLabel.IsDisposed)
                        MirClassLabel.Dispose();

                    MirClassLabel = null;
                }

                if (LevelLabel != null)
                {
                    if (!LevelLabel.IsDisposed)
                        LevelLabel.Dispose();

                    LevelLabel = null;
                }

                if (MirGenderLabel != null)
                {
                    if (!MirGenderLabel.IsDisposed)
                        MirGenderLabel.Dispose();

                    MirGenderLabel = null;
                }

                if (PriceLabel != null)
                {
                    if (!PriceLabel.IsDisposed)
                        PriceLabel.Dispose();

                    PriceLabel = null;
                }

                if (InventoryButton != null)
                {
                    if (!InventoryButton.IsDisposed)
                        InventoryButton.Dispose();

                    InventoryButton = null;
                }

                if (MagicButton != null)
                {
                    if (!MagicButton.IsDisposed)
                        MagicButton.Dispose();

                    MagicButton = null;
                }
            }
        }
        #endregion
    }
}
