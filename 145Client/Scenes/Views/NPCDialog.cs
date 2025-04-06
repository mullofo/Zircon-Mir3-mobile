using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.UserModels;
using Library;
using Library.Network.ClientPackets;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;
using Font = System.Drawing.Font;
using S = Library.Network.ServerPackets;

namespace Client.Scenes.Views
{
    /// <summary>
    /// NPC对话功能
    /// </summary>
    public sealed class NPCDialog : DXWindow
    {
        #region Properties

        #region txt脚本
        public static Regex R = new Regex(@"<((.*?)\/(\@.*?))>");
        public static Regex C = new Regex(@"{((.*?)\/(.*?))}");
        public static Regex L = new Regex(@"\(((.*?)\/(.*?))\)");
        public List<DXLabel> TextLabels = new List<DXLabel>();
        public List<DXLabel> TextColors = new List<DXLabel>();
        public List<DXLabel> TextButtons = new List<DXLabel>();
        private int _index = 0;
        #endregion

        /// <summary>
        /// 正则表达式
        /// </summary>
        public static Regex RegexText = new Regex(@"\[(?<Text>.*?):(?<ID>.+?)\]", RegexOptions.Compiled);
        /// <summary>
        /// HTML正则表达式
        /// </summary>
        public static Regex RegexHtml = new Regex(@"[<\[]([^>^\]]*)[>\]]", RegexOptions.IgnoreCase);
        /// <summary>
        /// 是否打开
        /// </summary>
        public bool Opened;
        /// <summary>
        /// 边缘尺寸=22
        /// </summary>
        public const int EdgeSize = 22;
        /// <summary>
        /// 客户端NPC页面
        /// </summary>
        public ClientNPCPage Page;
        /// <summary>
        /// 页面文本
        /// </summary>
        public DXLabel PageText;
        /// <summary>
        /// 子页面
        /// </summary>
        public DXControl[] PageChild;
        /// <summary>
        /// 道具单元格
        /// </summary>
        public DXItemGrid[] Grid;

        public DXImageControl ImgSelectMask;

        public DXImageControl CurrentControl
        {
            get => _CurrentControl;
            set
            {
                if (_CurrentControl == value || value == null)//取消选择
                {
                    _CurrentControl = null;
                    if (ImgSelectMask != null)
                        ImgSelectMask.Visible = false;
                    return;
                }
                if (value != null)//移动遮挡到选中图片前
                {
                    if (ImgSelectMask != null)
                    {
                        ImgSelectMask.Location = new Point(value.Location.X + 17, value.Location.Y + 18);
                        ImgSelectMask.Size = value.Size;
                        ImgSelectMask.Visible = true;
                    }
                }
                _CurrentControl = value;
            }
        }

        private DXImageControl _CurrentControl;
        public int CurrentSelectIndex { get; set; }

        /// <summary>
        /// NPC对应功能打开可见改变
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.NPCGoodsBox != null && !IsVisible)
                GameScene.Game.NPCGoodsBox.Visible = false;

            if (GameScene.Game.NPCSellBox != null && !IsVisible)
                GameScene.Game.NPCSellBox.Visible = false;

            if (GameScene.Game.NPCRootSellBox != null && !IsVisible)
                GameScene.Game.NPCRootSellBox.Visible = false;

            if (GameScene.Game.NPCRepairBox != null && !IsVisible)
                GameScene.Game.NPCRepairBox.Visible = false;

            if (GameScene.Game.NPCSpecialRepairBox != null && !IsVisible)
                GameScene.Game.NPCSpecialRepairBox.Visible = false;

            if (GameScene.Game.NPCRefinementStoneBox != null && !IsVisible)
                GameScene.Game.NPCRefinementStoneBox.Visible = false;

            if (GameScene.Game.NPCRefineBox != null && !IsVisible)
                GameScene.Game.NPCRefineBox.Visible = false;

            if (GameScene.Game.NPCRefineRetrieveBox != null && !IsVisible)
                GameScene.Game.NPCRefineRetrieveBox.Visible = false;

            if (GameScene.Game.NPCQuestListBox != null && !IsVisible)
                GameScene.Game.NPCQuestListBox.Visible = false;

            if (GameScene.Game.NPCQuestBox != null && !IsVisible)
                GameScene.Game.NPCQuestBox.Visible = false;

            if (GameScene.Game.NPCAdoptCompanionBox != null && !IsVisible)
                GameScene.Game.NPCAdoptCompanionBox.Visible = false;

            if (GameScene.Game.NPCCompanionStorageBox != null && !IsVisible)
                GameScene.Game.NPCCompanionStorageBox.Visible = false;

            if (GameScene.Game.NPCWeddingRingBox != null && !IsVisible)
                GameScene.Game.NPCWeddingRingBox.Visible = false;

            if (GameScene.Game.NPCMasterRefineBox != null && !IsVisible)
                GameScene.Game.NPCMasterRefineBox.Visible = false;

            if (GameScene.Game.NPCItemFragmentBox != null && !IsVisible)
                GameScene.Game.NPCItemFragmentBox.Visible = false;

            if (GameScene.Game.NPCAccessoryUpgradeBox != null && !IsVisible)
                GameScene.Game.NPCAccessoryUpgradeBox.Visible = false;

            if (GameScene.Game.NPCAccessoryLevelBox != null && !IsVisible)
                GameScene.Game.NPCAccessoryLevelBox.Visible = false;

            if (GameScene.Game.NPCAccessoryResetBox != null && !IsVisible)
                GameScene.Game.NPCAccessoryResetBox.Visible = false;

            if (GameScene.Game.NPCWeaponCraftBox != null && !IsVisible)
                GameScene.Game.NPCWeaponCraftBox.Visible = false;

            if (GameScene.Game.StorageBox != null && !IsVisible)
                GameScene.Game.StorageBox.Visible = false;

            if (GameScene.Game.MarketSearchBox != null && !IsVisible)
                GameScene.Game.MarketSearchBox.Visible = false;

            if (GameScene.Game.MyMarketBox != null && !IsVisible)
                GameScene.Game.MyMarketBox.Visible = false;

            if (GameScene.Game.AuctionsBox != null && !IsVisible)
                GameScene.Game.AuctionsBox.Visible = false;

            if (GameScene.Game.NPCDKeyBox != null && !IsVisible)
                GameScene.Game.NPCDKeyBox.Visible = false;

            if (GameScene.Game.AdditionalBox != null && !IsVisible)
                GameScene.Game.AdditionalBox.Visible = false;

            if (GameScene.Game.NPCBookCombineBox != null && !IsVisible)
                GameScene.Game.NPCBookCombineBox.Visible = false;

            if (GameScene.Game.NPCPerforationBox != null && !IsVisible)
                GameScene.Game.NPCPerforationBox.Visible = false;

            if (GameScene.Game.NPCEnchantingBox != null && !IsVisible)
                GameScene.Game.NPCEnchantingBox.Visible = false;

            if (GameScene.Game.NPCEnchantmentSynthesisBox != null && !IsVisible)
                GameScene.Game.NPCEnchantmentSynthesisBox.Visible = false;

            if (GameScene.Game.NPCWeaponUpgradeBox != null && !IsVisible)
                GameScene.Game.NPCWeaponUpgradeBox.Visible = false;

            if (GameScene.Game.NPCAncientTombstoneBox != null && !IsVisible)
                GameScene.Game.NPCAncientTombstoneBox.Visible = false;

            if (GameScene.Game.NPCAccessoryCombineBox != null && !IsVisible)
                GameScene.Game.NPCAccessoryCombineBox.Visible = false;

            if (GameScene.Game.NPCBuyBackBox != null && !IsVisible)
                GameScene.Game.NPCBuyBackBox.Visible = false;

            if (GameScene.Game.GoldTradingBusinessBox != null && !IsVisible)
                GameScene.Game.GoldTradingBusinessBox.Visible = false;

            if (GameScene.Game.AccountConsignmentBox != null && !IsVisible)
                GameScene.Game.AccountConsignmentBox.Visible = false;

            if (Opened)
            {
                GameScene.Game.NPCID = 0;
                Opened = false;
                CEnvir.Enqueue(new C.NPCClose());
            }

            //if (IsVisible)
            //{
            //    if (GameScene.Game.CharacterBox.Location.X < Size.Width)
            //        GameScene.Game.CharacterBox.Location = new Point(Size.Width, 0);

            //    GameScene.Game.StorageBox.Location = new Point(GameScene.Game.Size.Width - GameScene.Game.StorageBox.Size.Width, GameScene.Game.InventoryBox.Size.Height);
            //}
            //else if (GameScene.Game.CharacterBox.Location.X == Size.Width)
            //{
            //    GameScene.Game.CharacterBox.ApplySettings();
            //    GameScene.Game.StorageBox.ApplySettings();
            //}
        }
        /// <summary>
        /// 对话框上半部
        /// </summary>
        public DXImageControl TopPanel;
        /// <summary>
        /// 中间内容面板
        /// </summary>
        public DXImageControl ContentPanel;
        /// <summary>
        /// 对话框底部
        /// </summary>
        public DXImageControl BottomPanel;
        /// <summary>
        /// 自定义对话内容面板
        /// </summary>
        public DXImageControl CustomPanel;
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public DXButton Close1Button;

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        #region NPC对话框

        /// <summary>
        /// NPC对话框
        /// </summary>
        public NPCDialog()
        {
            HasTitle = false;                //标题不显示
            HasTopBorder = false;
            Border = false;
            TitleLabel.Text = string.Empty;  //标题标签文本 = 字符串为空
            HasFooter = false;               //页脚不显示
            Movable = true;
            CloseButton.Visible = false; //不显示关闭按钮
            Opacity = 0F;

            Size = new Size(380, 200);

            TopPanel = new DXImageControl      //NPC对话框上半部分
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1353,
                FixedSize = true,
                Size = new Size(380, 35),
                ImageOpacity = 0.85F,
                PassThrough = true,
            };

            BottomPanel = new DXImageControl    //NPC对话框底部
            {
                UseOffSet = true,
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                ImageOpacity = 0.85F,
                Index = 1352,
            };

            ContentPanel = new DXImageControl       //NPC对话框中间部分 内容面板
            {
                Size = new Size(TopPanel.Size.Width, 100),
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1351,
                FixedSize = true,
                TilingMode = TilingMode.Vertically,
                ImageOpacity = 0.85F,
                Location = new Point(0, TopPanel.Size.Height),
                PassThrough = true,
            };

            CustomPanel = new DXImageControl      //自定义面板
            {
                Parent = this,
                Visible = false,
                PassThrough = true,
            };

            /*TitleLabel = new DXLabel     //NPC名标签
            {
                Parent = TopPanel,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Bold),
                ForeColour = Color.FromArgb(213, 197, 172),
                Outline = true,
                OutlineColour = Color.Black,
                PassThrough = true
            };
            TitleLabel.Location = new Point(ClientArea.X / 2, 4);*/

            PageText = new DXLabel  //页面文本
            {
                AutoSize = true,  //自动大小关闭
                Outline = false,  //外形尺寸关闭 //
                DrawFormat = TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis,//绘图格式=文本格式标志分词|文本格式标记单词省略号
                Parent = ContentPanel,
                ForeColour = Color.White,    //文本颜色 白色
                Location = new Point(EdgeSize, 0),
                Size = new Size(ClientArea.Width - EdgeSize, ClientArea.Height - 20),
                PassThrough = true

            };

            Close1Button = new DXButton
            {
                Parent = BottomPanel,
                LibraryFile = LibraryFile.UI1,
                Index = 1222,
                Location = new Point(340, -2),
                Visible = true,
                Hint = "关闭",
            };
            Close1Button.MouseClick += (o, e) => Visible = false;

            ImgSelectMask = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 912,
                PassThrough = true,
                Visible = false
            };
        }

        #region Methods

        #region txt脚本 
        public void NewText(List<string> lines, bool resetIndex = true)
        {
            int btnIndex = 0;
            int colorIndex = 0;
            if (resetIndex)
            {
                _index = 0;
            }

            #region 初始化
            if (TextLabels != null)
            {
                for (int i = 0; i < TextLabels.Count; i++)
                {
                    if (TextLabels[i] != null)
                    {
                        TextLabels[i].Text = "";
                        TextLabels[i].Visible = false;
                    }
                }
            }
            if (TextColors != null)
            {
                for (int i = 0; i < TextColors.Count; i++)
                {
                    if (TextColors[i] != null)
                    {
                        TextColors[i].Text = "";
                        TextColors[i].Visible = false;
                        TextColors[i].Parent = null;
                    }
                }

            }
            if (TextButtons != null)
            {
                for (int i = 0; i < TextButtons.Count; i++)
                {
                    if (TextButtons[i] != null)
                    {
                        TextButtons[i].Text = "";
                        TextButtons[i].Tag = "";
                        TextButtons[i].Visible = false;
                        TextButtons[i].Parent = null;
                    }
                }

            }
            #endregion

            int lastLine = lines.Count;
            int h = 0;
            for (int i = _index; i < lastLine; i++)
            {
                if (i <= TextLabels.Count - 1)
                {
                    TextLabels[i].ForeColour = Color.White;
                    TextLabels[i].Size = new Size(420, 20);
                    TextLabels[i].Location = new Point(20, 2 + (i - _index) * 20);
                    TextLabels[i].Visible = true;
                }
                else
                {
                    TextLabels.Add(new DXLabel
                    {
                        DrawFormat = TextFormatFlags.WordBreak,
                        Visible = true,
                        Parent = ContentPanel,
                        Size = new Size(420, 20),
                        Location = new Point(20, 2 + (i - _index) * 20),
                        ForeColour = Color.White,
                        IsControl = true
                    });
                }

                h += TextLabels[i].Size.Height;

                string currentLine = lines[i];

                List<Match> matchList = R.Matches(currentLine).Cast<Match>().ToList();
                matchList.AddRange(C.Matches(currentLine).Cast<Match>());
                matchList.AddRange(L.Matches(currentLine).Cast<Match>());

                int oldLength = currentLine.Length;

                foreach (Match match in matchList.OrderBy(o => o.Index).ToList())
                {
                    int offSet = oldLength - currentLine.Length;

                    Capture capture = match.Groups[1].Captures[0];
                    string txt = match.Groups[2].Captures[0].Value;
                    string action = match.Groups[3].Captures[0].Value;
                    if (C.Match(match.Value).Success)
                    {
                        var httpPattern = new Regex(@".*http.*(?=\/)");
                        if (httpPattern.IsMatch(capture.Value))
                        {
                            txt = httpPattern.Match(capture.Value).Value;
                            action = capture.Value.Replace(txt + @"/", "");
                        }
                    }
                    if (L.Match(match.Value).Success)
                    {
                        var httpPattern = new Regex(@"(?<=\/)http.*(?=\))");
                        if (httpPattern.IsMatch(match.Value))
                        {
                            action = httpPattern.Match(match.Value).Value;
                            txt = capture.Value.Replace(@"/" + action, "");
                        }
                    }
                    currentLine = currentLine.Remove(capture.Index - 1 - offSet, capture.Length + 2).Insert(capture.Index - 1 - offSet, txt);
                    string text = currentLine.Substring(0, capture.Index - 1 - offSet) + " ";
                    Size size = DXLabel.GetSize(text, TextLabels[i].Font, true, new Size(4096, 4096));
                    if (R.Match(match.Value).Success)
                    {
                        NewButton(btnIndex, txt, action, TextLabels[i].Location.Add(new Point(size.Width - 12, 0)));
                        btnIndex++;
                    }

                    if (C.Match(match.Value).Success)
                    {
                        NewColour(colorIndex, txt, action, TextLabels[i].Location.Add(new Point(size.Width - 12, 0)));
                        colorIndex++;
                    }

                    if (L.Match(match.Value).Success)
                    {
                        NewButton(btnIndex, txt, action, TextLabels[i].Location.Add(new Point(size.Width - 12, 0)));
                        btnIndex++;
                    }
                }

                TextLabels[i].Text = currentLine;

            }
            var w = TopPanel.Size.Width - 40;

            ContentPanel.Size = new Size(TopPanel.Size.Width, h);
            BottomPanel.Location = new Point(0, ContentPanel.Location.Y + ContentPanel.Size.Height + 16);
            Size = new Size(TopPanel.Size.Width, BottomPanel.Location.Y + BottomPanel.Size.Height - 16);
        }
        private void NewButton(int index, string text, string key, Point p)
        {
            if (index <= TextButtons.Count - 1)
            {
                TextButtons[index].Location = p;
                TextButtons[index].Tag = key;
                TextButtons[index].Text = text;
                TextButtons[index].Visible = true;
                TextButtons[index].Parent = ContentPanel;
                return;
            }
            DXLabel temp = new DXLabel
            {
                AutoSize = true,
                Visible = true,
                Parent = ContentPanel,
                Location = p,
                Text = text,
                ForeColour = Color.Yellow,
                Tag = key
            };

            temp.MouseEnter += (o, e) => temp.ForeColour = Color.Red;
            temp.MouseLeave += (o, e) => temp.ForeColour = Color.Yellow;
            temp.MouseDown += (o, e) => temp.ForeColour = Color.Yellow;
            temp.MouseUp += (o, e) => temp.ForeColour = Color.Red;

            temp.MouseClick += (o, e) =>
            {
                var realKey = (o as DXLabel).Tag.ToString();
                if (realKey.StartsWith("http"))
                {
                    System.Diagnostics.Process.Start(realKey);
                    return;
                }

                if (realKey.ToUpper() == "@Exit".ToUpper())
                {
                    this.Visible = false;
                    return;
                }

                if (CEnvir.Now <= GameScene.Game.NPCTime) return;

                GameScene.Game.NPCTime = CEnvir.Now.AddMilliseconds(500);
                CEnvir.Enqueue(new C.NPCCall { ObjectID = GameScene.Game.NPCID, Key = $"[{realKey}]" });
            };

            TextButtons.Add(temp);

        }
        private void NewColour(int index, string text, string colour, Point p)
        {

            Color textColour = colour.StartsWith("#") ? ColorTranslator.FromHtml(colour) : Color.FromName(colour);

            if (index <= TextColors.Count - 1)
            {
                TextColors[index].Location = p;
                TextColors[index].Text = text;
                TextColors[index].Visible = true;
                TextColors[index].Parent = ContentPanel;
                TextColors[index].ForeColour = textColour;
                return;
            }
            DXLabel temp = new DXLabel
            {
                AutoSize = true,
                Visible = true,
                Parent = ContentPanel,
                Location = p,
                Text = text,
                ForeColour = textColour
            };

            TextColors.Add(temp);
        }
        #endregion

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="ch"></param>
        private void UpdateLocations(int ch = 0)
        {
            if (CustomPanel.Visible == true)
            {
                var c_w = ClientArea.Width - EdgeSize;  //NPC文本大小
                var c_h = ch;
                if (c_h + EdgeSize > CustomPanel.Library.GetSize(CustomPanel.Index).Height)
                {
                    CustomPanel.Size = new Size(CustomPanel.Size.Width, c_h + EdgeSize + 10);
                    Size = CustomPanel.Size;
                }
                PageText.Size = new Size(c_w, Size.Height);
                if (null == Page || null == Page.bg) return;
                if (Page.bg.close == 1)                    //右上角
                {
                    CloseButton.Location = new Point(CustomPanel.Size.Width - CloseButton.Size.Width + Page.bg.close_offset_x, 0 + Page.bg.close_offset_y);
                }
                else if (Page.bg.close == 2)               //右下角
                {
                    CloseButton.Location = new Point(CustomPanel.Size.Width - CloseButton.Size.Width + Page.bg.close_offset_x, CustomPanel.Size.Height - CloseButton.Size.Height + Page.bg.close_offset_y);
                }
                else if (Page.bg.close == 3)               //左下角
                {
                    CloseButton.Location = new Point(0 + Page.bg.close_offset_x, CustomPanel.Size.Height - CloseButton.Size.Height + Page.bg.close_offset_y);
                }
                else if (Page.bg.close == 4)               //左上角
                {
                    CloseButton.Location = new Point(0 + Page.bg.close_offset_x, 0 + Page.bg.close_offset_y);
                }
                return;
            }
            else
            {
                var w = TopPanel.Size.Width - 2 * EdgeSize;  //NPC文本大小
                var h = ch == 0 ? DXLabel.GetHeight(PageText, w).Height : ch;
                PageText.Size = new Size(w, h);
                h = Math.Max(h, 120);
                ContentPanel.Size = new Size(TopPanel.Size.Width, h);
                BottomPanel.Location = new Point(0, ContentPanel.Location.Y + ContentPanel.Size.Height + 16);
                Size = new Size(TopPanel.Size.Width, BottomPanel.Location.Y + BottomPanel.Size.Height - 16);
            }
        }
        /// <summary>
        /// 可见改变时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);
            if (!IsVisible)
            {
                GameScene.Game.InventoryBox.InventoryType = InventoryType.Inventory;
            }
        }
        /// <summary>
        /// 设置显示的NPC面板
        /// </summary>
        /// <param name="bg"></param>
        public void SetShowBg(NPCCustomBG bg)
        {
            if (null != bg && (bg.url != "" || (bg.file != -1 && bg.idx != -1)))
            {
                //显示自定义窗口
                TopPanel.Visible = false;//隐藏掉原来的bg
                BottomPanel.Visible = false;
                ContentPanel.Visible = false;
                CustomPanel.Visible = true;

                if (bg.file != -1 && bg.idx != -1)
                {
                    CustomPanel.LibraryFile = (LibraryFile)bg.file;
                    CustomPanel.Index = bg.idx;
                    CustomPanel.FixedSize = true;
                    int w = bg.size_w != 0 ? bg.size_w : CustomPanel.Library.GetSize(CustomPanel.Index).Width;
                    int h = bg.size_h != 0 ? bg.size_h : CustomPanel.Library.GetSize(CustomPanel.Index).Height;
                    CustomPanel.Size = new Size(w, h);
                    Size = CustomPanel.Size;

                }
                else if (bg.url != "")
                {

                }

                if (bg.center == 0)
                {
                    Location = new Point(0 + bg.offset_x, 0 + bg.offset_y);
                }
                else
                {
                    Location = new Point(CEnvir.Target.ClientSize.Width / 2 - CustomPanel.Size.Width / 2 + bg.offset_x,
                        CEnvir.Target.ClientSize.Height / 2 - CustomPanel.Size.Height / 2 + bg.offset_y);
                }

                Close1Button.Parent = BottomPanel;
                TitleLabel.Parent = CustomPanel;
                PageText.Parent = CustomPanel;
                Close1Button.Visible = bg.close == 0 ? false : true;
                Movable = bg.drag == 1 ? true : false;
                if ("" != bg.title)
                {
                    TitleLabel.Text = bg.title;
                }
            }
            else
            {
                Location = new Point(0, 0);
                //显示默认的背景内容
                TopPanel.Visible = true;
                BottomPanel.Visible = true;
                ContentPanel.Visible = true;

                CustomPanel.Visible = false;
                Close1Button.Parent = BottomPanel;
                if (null != bg)
                {
                    Close1Button.Visible = bg.close == 0 ? false : true;
                    Movable = bg.drag == 1 ? true : false;
                }
                else
                {
                    Movable = false;
                    Close1Button.Visible = true;
                }
                TitleLabel.Parent = TopPanel;
                PageText.Parent = ContentPanel;
            }
        }
        /// <summary>
        /// 设置显示的字体
        /// </summary>
        /// <param name="font"></param>
        public void SetShowFont(NPCCustomFont font)
        {
            if (null == font)
            {
                //原来的样子
                PageText.ForeColour = Color.White;
                PageText.Location = new Point(EdgeSize, 0);
                PageText.Font = new Font(Config.FontName, CEnvir.FontSize(9F));//设置字体会改变大小 所有先设置字体
                PageText.Size = new Size(ClientArea.Width - EdgeSize, ClientArea.Height - EdgeSize * 2);
            }
            else
            {
                if (font.size != 0)
                {
                    PageText.Font = new Font(Config.FontName, CEnvir.FontSize(font.size));
                }
                else
                {
                    PageText.Font = new Font(Config.FontName, CEnvir.FontSize(9F));
                }
                PageText.Size = new Size(ClientArea.Width - EdgeSize, ClientArea.Height - EdgeSize * 2);
                if (font.color != "")
                {
                    PageText.ForeColour = ColorTranslator.FromHtml(Convert.ToString(font.color));
                }
                else
                {
                    PageText.ForeColour = Color.White;
                }
                PageText.Location = new Point(EdgeSize + font.offset_x, 0 + font.offset_y);
            }
            if (CustomPanel.Visible == true)
            {
                PageText.Location = new Point(PageText.Location.X, PageText.Location.Y + EdgeSize);
            }
        }
        /// <summary>
        /// 设置显示所需的道具
        /// </summary>
        /// <param name="needItems"></param>
        public void SetShowNeedItems(List<NPCCustomNeedItems> needItems)
        {
            if (null == needItems || needItems.Count == 0) return;

            Grid = new DXItemGrid[needItems.Count];
            for (int i = 0; i < needItems.Count; i++)
            {
                Grid[i] = new DXItemGrid
                {
                    Parent = this,
                    Opacity = 1F,
                    GridSize = new Size(1, 1),
                    GridType = GridType.CustomDialog,
                    AllowLink = true,
                    Linked = true
                };

                int temp_x = needItems[i].pos_x == 0 ? 10 + i * Grid[i].Size.Width : needItems[i].pos_x;
                int temp_y = needItems[i].pos_y == 0 ? 10 : needItems[i].pos_y;
                Grid[i].Location = new Point(temp_x, temp_y);
                if (CustomPanel.Visible == true)
                {
                    Grid[i].Parent = CustomPanel;
                }
                else
                {
                    Grid[i].Parent = ContentPanel;
                }
                if (needItems[i].file != -1 && needItems[i].idx != -1)
                {
                    Grid[i].ImagePanel.Visible = true;
                    Grid[i].ImagePanel.LibraryFile = (LibraryFile)needItems[i].file;
                    Grid[i].ImagePanel.Index = needItems[i].idx;
                    Grid[i].Border = false;
                }
            }
        }
        /// <summary>
        /// 清除页面
        /// </summary>
        public void ClearPage()
        {
            if (PageChild != null)
                foreach (var cell in PageChild)
                    if (null != cell) cell.Dispose();
            PageChild = null;
        }
        /// <summary>
        /// 清除道具格子
        /// </summary>
        public void ClearGrid()
        {
            if (Grid != null)
                foreach (DXItemGrid cell in Grid)
                    cell.Dispose();
            Grid = null;
        }
        /// <summary>
        /// 响应
        /// </summary>
        /// <param name="info"></param>
        public void Response(S.NPCResponse info)
        {
            GameScene.Game.NPCID = info.ObjectID;
            NPCDialogType dialogType;
            List<ClientNPCGood> goods;
            //显示NPC名字
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.Race != ObjectType.NPC || ob.ObjectID != info.ObjectID) continue;

                TitleLabel.Text = ((NPCObject)ob).NPCInfo.Lang(p => p.NPCName);

                break;
            }
            if (info.Page != null && info.Page.Count > 0)
            {
                NewText(info.Page);
                dialogType = info.NPCDialogType;
                goods = info.Goods;
            }
            else
            {
                dialogType = info.NpcPage.DialogType;
                goods = info.NpcPage.Goods;
                var npcPage = info.NpcPage;
                npcPage.Say = npcPage.Say.Replace('(', '[').Replace(')', ']').Replace('【', '[');

                Page = npcPage;

                //PageText.Text = R.Replace(Page.Say, @"${Text}");
                ClearPage();
                ClearGrid();
                SetShowBg(Page.bg);
                SetShowFont(Page.font);
                SetShowNeedItems(Page.needItems);
                UpdateLocations(ProcessText());
            }
            GameScene.Game.NPCBox.Visible = true;

            Opened = true;
            if (GameScene.Game.NPCQuestListBox.Visible)
            {
                GameScene.Game.NPCQuestListBox.Location = new Point(0, Size.Height);
            }
            GameScene.Game.NPCGoodsBox.Visible = false;                 //买
            GameScene.Game.NPCSellBox.Visible = false;                  //卖
            GameScene.Game.NPCRootSellBox.Visible = false;              //一键出售
            GameScene.Game.NPCRepairBox.Visible = false;                //修理
            GameScene.Game.NPCSpecialRepairBox.Visible = false;         //特修
            GameScene.Game.NPCRefineBox.Visible = false;                //精炼
            GameScene.Game.NPCRefinementStoneBox.Visible = false;       //精炼石
            GameScene.Game.NPCRefineRetrieveBox.Visible = false;        //高级精炼
            GameScene.Game.NPCAdoptCompanionBox.Visible = false;        //宠物管理
            GameScene.Game.NPCCompanionStorageBox.Visible = false;      //宠物存放
            GameScene.Game.NPCWeddingRingBox.Visible = false;           //结婚戒指
            GameScene.Game.NPCItemFragmentBox.Visible = false;          //道具分解碎片
            GameScene.Game.NPCAccessoryUpgradeBox.Visible = false;      //附件定义升级
            GameScene.Game.NPCAccessoryLevelBox.Visible = false;        //附件设置等级
            GameScene.Game.NPCMasterRefineBox.Visible = false;          //大师精炼
            GameScene.Game.NPCAccessoryResetBox.Visible = false;        //附件设置
            GameScene.Game.NPCWeaponCraftBox.Visible = false;           //武器制作
            GameScene.Game.StorageBox.Visible = false;                  //仓库
            GameScene.Game.MarketSearchBox.Visible = false;             //寄售
            GameScene.Game.MyMarketBox.Visible = false;                 //我的寄售
            GameScene.Game.NPCDKeyBox.Visible = false;                  //D键功能
            GameScene.Game.AdditionalBox.Visible = false;               //额外属性
            GameScene.Game.NPCBookCombineBox.Visible = false;           //技能书合成
            GameScene.Game.NPCPerforationBox.Visible = false;           //镶嵌打孔
            GameScene.Game.NPCEnchantingBox.Visible = false;            //镶嵌附魔
            GameScene.Game.NPCEnchantmentSynthesisBox.Visible = false;  //附魔合成
            GameScene.Game.NPCWeaponUpgradeBox.Visible = false;         //新版武器升级
            GameScene.Game.NPCAncientTombstoneBox.Visible = false;      //古代墓碑
            GameScene.Game.NPCAccessoryCombineBox.Visible = false;      //新版首饰合成
            GameScene.Game.NPCBuyBackBox.Visible = false;               //回购
            GameScene.Game.GoldTradingBusinessBox.Visible = false;      //金币交易行
            GameScene.Game.AuctionsBox.Visible = false;                 //拍卖行
            GameScene.Game.AccountConsignmentBox.Visible = false;       //账号寄售行

            switch (dialogType)  //页面显示的位置坐标
            {
                case NPCDialogType.None:
                    break;
                case NPCDialogType.BuySell:   //买
                    GameScene.Game.NPCGoodsBox.Location = new Point(0, GameScene.Game.NPCBox.Size.Height); //不遮挡NPC对话框
                    GameScene.Game.NPCGoodsBox.Visible = goods.Count > 0;
                    GameScene.Game.NPCGoodsBox.NewGoods(goods);
                    //GameScene.Game.InventoryBox.Location = new Point(Location.X + Size.Width, Location.Y);
                    if (GameScene.Game.Observer)   //如果是观察者
                        GameScene.Game.InventoryBox.Visible = false;
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.Sell:      //卖
                    GameScene.Game.NPCSellBox.Visible = Page.Types.Count > 0;
                    GameScene.Game.NPCSellBox.Location = new Point(0, Size.Height);
                    //GameScene.Game.InventoryBox.Location = new Point(Location.X + Size.Width, Location.Y);
                    if (GameScene.Game.Observer)   //如果是观察者
                        GameScene.Game.InventoryBox.Visible = false;
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.Repair:  //普通修理
                    GameScene.Game.InventoryBox.Visible = true;
                    GameScene.Game.InventoryBox.InventoryType = InventoryType.Repair;
                    //GameScene.Game.InventoryBox.Location = new Point(Location.X + Size.Width, Location.Y);
                    if (GameScene.Game.Observer)   //如果是观察者
                        GameScene.Game.InventoryBox.Visible = false;
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.SpecialRepair:      //特殊修理
                    GameScene.Game.NPCSpecialRepairBox.Visible = true;
                    GameScene.Game.NPCSpecialRepairBox.Location = new Point(0, Size.Height);
                    //GameScene.Game.InventoryBox.Location = new Point(Location.X + Size.Width, Location.Y);
                    if (GameScene.Game.Observer)   //如果是观察者
                        GameScene.Game.InventoryBox.Visible = false;
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.RootSell:   //一键出售
                    GameScene.Game.NPCRootSellBox.Visible = true;
                    GameScene.Game.NPCRootSellBox.Location = new Point(0, Size.Height);
                    //GameScene.Game.InventoryBox.Location = new Point(Location.X + Size.Width, Location.Y);
                    if (GameScene.Game.Observer)   //如果是观察者
                        GameScene.Game.InventoryBox.Visible = false;
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.RefinementStone:   //精炼石
                    GameScene.Game.NPCRefinementStoneBox.Visible = true;
                    GameScene.Game.NPCRefinementStoneBox.Location = new Point(0, Size.Height);
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.Refine:   //精炼
                    GameScene.Game.NPCRefineBox.Visible = true;
                    GameScene.Game.NPCRefineBox.Location = new Point(0, Size.Height);
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.MasterRefine:  //大师精炼
                    GameScene.Game.NPCMasterRefineBox.Visible = true;
                    GameScene.Game.NPCMasterRefineBox.Location = new Point(0, Size.Height);
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.RefineRetrieve:  //高级精炼
                    GameScene.Game.NPCRefineRetrieveBox.Location = new Point(0, Size.Height);
                    GameScene.Game.NPCRefineRetrieveBox.Visible = true;
                    GameScene.Game.NPCRefineRetrieveBox.RefreshList();
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.CompanionManage:  //宠物管理
                    GameScene.Game.NPCCompanionStorageBox.Visible = true;
                    GameScene.Game.NPCCompanionStorageBox.Location = new Point(0, Size.Height);
                    GameScene.Game.NPCAdoptCompanionBox.Visible = true;
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.WeddingRing:  //结婚
                    GameScene.Game.NPCWeddingRingBox.Visible = true;
                    GameScene.Game.NPCWeddingRingBox.Location = new Point(0, Size.Height);
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.ItemFragment:   //道具分解碎片
                    GameScene.Game.NPCItemFragmentBox.Visible = true;
                    GameScene.Game.NPCItemFragmentBox.Location = new Point(0, Size.Height);
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.AccessoryRefineUpgrade:   //升级熔炼首饰
                    GameScene.Game.NPCAccessoryUpgradeBox.Visible = true;
                    GameScene.Game.NPCAccessoryUpgradeBox.Location = new Point(0, Size.Height);
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.AccessoryRefineLevel:  //熔炼首饰
                    GameScene.Game.NPCAccessoryLevelBox.Visible = true;
                    GameScene.Game.NPCAccessoryLevelBox.Location = new Point(0, Size.Height);
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.AccessoryReset:    //熔炼首饰重置
                    GameScene.Game.NPCAccessoryResetBox.Visible = true;
                    GameScene.Game.NPCAccessoryResetBox.Location = new Point(0, Size.Height);
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.WeaponCraft:   //武器制作
                    GameScene.Game.NPCWeaponCraftBox.Visible = true;
                    GameScene.Game.NPCWeaponCraftBox.Location = new Point(0, Size.Height);
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.Storage:    //仓库
                    GameScene.Game.StorageBox.Visible = true;
                    GameScene.Game.StorageBox.Location = new Point(0, 0);
                    if (!GameScene.Game.Observer)   //如果不是观察者
                        GameScene.Game.InventoryBox.Location = new Point(GameScene.Game.StorageBox.Size.Width - 20, 0);
                    else
                        GameScene.Game.InventoryBox.Visible = false;
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.MarketSearch:  //寄售
                    GameScene.Game.MarketSearchBox.DlgType = CurrencyType.Gold;
                    GameScene.Game.MarketSearchBox.Visible = true;
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.GameGoldMarketSearch:  //元宝寄售
                    GameScene.Game.MarketSearchBox.DlgType = CurrencyType.GameGold;
                    GameScene.Game.MarketSearchBox.Visible = true;
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.MyMarket:  //我的寄售
                    GameScene.Game.NPCBox.Visible = false;
                    GameScene.Game.MyMarketBox.Visible = true;
                    if (!GameScene.Game.Observer)   //如果不是观察者
                        GameScene.Game.InventoryBox.Location = new Point(Location.X + Size.Width, Location.Y);
                    else
                        GameScene.Game.InventoryBox.Visible = false;
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.DKey:     //D键功能
                    if (info.Page == null)
                        GameScene.Game.NPCDKeyBox.Visible = true;
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.Additional:        //额外属性
                    GameScene.Game.AdditionalBox.Location = new Point(0, Size.Height);
                    GameScene.Game.AdditionalBox.Visible = true;
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.BookCombine:        //技能书合成
                    GameScene.Game.NPCBookCombineBox.Visible = true;
                    GameScene.Game.NPCBookCombineBox.Location = new Point(0, Size.Height);
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.Perforation:      //镶嵌打孔
                    GameScene.Game.NPCPerforationBox.Visible = true;
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.Enchanting:      //镶嵌附魔
                    GameScene.Game.NPCEnchantingBox.Visible = true;
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.EnchantmentSynthesis:      //镶嵌合成
                    GameScene.Game.NPCEnchantmentSynthesisBox.Visible = true;
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.WeaponUpgrade:   //新版武器升级
                    GameScene.Game.NPCWeaponUpgradeBox.Visible = true;
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.AncientTombstone:
                    GameScene.Game.NPCAncientTombstoneBox.Visible = true;
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.AccessoryCombine:
                    GameScene.Game.NPCAccessoryCombineBox.Visible = true;
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.GoldTradingBusiness:
                    GameScene.Game.GoldTradingBusinessBox.Visible = true;
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.Auctions:
                    GameScene.Game.AuctionsBox.Visible = true;
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
                case NPCDialogType.BuyBack:
                    ShowBackGoods(new S.NPCBuyBack
                    {
                        BackGoods = goods,
                        PageIndex = info.CurrentPageIndex,
                        TotalPage = info.TotalPageIndex
                    });
                    break;
                case NPCDialogType.AccountConsignment:
                    GameScene.Game.AccountConsignmentBox.Visible = true;
                    GameScene.Game.NPCQuestListBox.Visible = false;
                    break;
            }
        }

        public void ShowBackGoods(S.NPCBuyBack p)
        {
            var goods = p.BackGoods;
            if (goods?.Count > 0)
            {
                GameScene.Game.NPCBuyBackBox.Visible = true;
                GameScene.Game.NPCBuyBackBox.NewGoods(goods, p.PageIndex, p.TotalPage);
                GameScene.Game.NPCBuyBackBox.Location = new Point(0, Size.Height);
                GameScene.Game.NPCQuestListBox.Visible = false;
            }
            else if (GameScene.Game.NPCBuyBackBox.PageIndex == 1)
            {
                GameScene.Game.NPCBuyBackBox.Visible = false;
                GameScene.Game.NPCQuestListBox.Visible = false;
            }
        }

        /// <summary>
        /// 添加自定义高度
        /// </summary>
        /// <param name="size"></param>
        /// <param name="curLine"></param>
        /// <param name="height"></param>
        private void Add_DefHeight(Size size, ref int curLine, ref int height)
        {
            Add_DefHeight(size.Height, ref curLine, ref height);
        }

        /// <summary>
        /// 添加自定义高度
        /// </summary>
        /// <param name="maxHeight"></param>
        /// <param name="curLine"></param>
        /// <param name="height"></param>
        private void Add_DefHeight(int maxHeight, ref int curLine, ref int height)
        {
            Size DefSize = DXLabel.GetSize("A", PageText.Font, PageText.Outline, new Size(4096, 4096));
            int addLineCount = (int)Math.Ceiling((float)maxHeight / DefSize.Height);
            addLineCount = addLineCount < 1 ? 1 : addLineCount;
            curLine += addLineCount;
            height += addLineCount * DefSize.Height + 2;        //默认2个像素的行高
        }
        /// <summary>
        /// 添加字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="offset"></param>
        /// <param name="curLine"></param>
        /// <param name="height"></param>
        /// <param name="result"></param>
        /// <param name="curLineMaxHeight"></param>
        /// <param name="Outline"></param>
        /// <param name="font"></param>
        private void AddStringAt(string str, ref int offset, ref int curLine, ref int height, ref List<_RichInfo> result,
            ref int curLineMaxHeight, bool Outline = false, Font font = null)
        {
            Color color = PageText.ForeColour;
            AddStringAt(str, ref offset, ref curLine, ref height, ref result, ref curLineMaxHeight, color, Outline, font);
        }
        /// <summary>
        /// 添加字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="offset"></param>
        /// <param name="curLine"></param>
        /// <param name="height"></param>
        /// <param name="result"></param>
        /// <param name="curLineMaxHeight"></param>
        /// <param name="color"></param>
        /// <param name="Outline"></param>
        /// <param name="font"></param>
        private void AddStringAt(string str, ref int offset, ref int curLine, ref int height, ref List<_RichInfo> result,
            ref int curLineMaxHeight, Color color, bool Outline = false, Font font = null)
        {
            if (null != font)
            {
                if (color.IsEmpty) color = PageText.ForeColour;
                var size = DXLabel.GetSize(str, font, Outline, new Size(4096, 4096), TextFormatFlags.SingleLine);
                var _Leading = TextRenderer.MeasureText(DXManager.Graphics, "\n", font, new Size(999, 9999), TextFormatFlags.SingleLine).Width;
                if (size.Height > curLineMaxHeight) curLineMaxHeight = size.Height;
                if (size.Width > _Leading) size.Width -= _Leading;
                string endStrtext = str;
                while (offset + size.Width > PageText.Size.Width)
                {
                    if (size.Height > curLineMaxHeight) curLineMaxHeight = size.Height;
                    //当前文本的长度
                    var nowWidth = PageText.Size.Width - offset;
                    //剩余文本的长度
                    size.Width = size.Width - nowWidth;
                    //获取对应长度的文字内容
                    string Richtext = "";
                    for (int cc = 1; cc < str.Length; cc++)
                    {
                        var tempccstr = Richtext + endStrtext.Substring(0, 1);
                        var tempccsize = TextRenderer.MeasureText(DXManager.Graphics, tempccstr, font, new Size(2048, 1024), TextFormatFlags.SingleLine);
                        if (tempccsize.Width > nowWidth)
                        {
                            break;
                        }
                        endStrtext = endStrtext.Substring(1, endStrtext.Length - 1);
                        Richtext = tempccstr;
                    }
                    if (Richtext != "")
                    {
                        //构造一个_RichInfo
                        result.Add(new _RichInfo()
                        {
                            id = (short)curLine,
                            type = 0,
                            point = new Point(offset, height),
                            text = Richtext,
                            size = new Size(nowWidth, size.Height),
                            color = color,
                            font = font.Size
                        });
                    }
                    Add_DefHeight(curLineMaxHeight, ref curLine, ref height);
                    offset = 0;
                    curLineMaxHeight = 0;   //换行后要清空最大高度
                }
                if (size.Width != 0 && endStrtext != "")
                {
                    //构造一个_RichInfo
                    result.Add(new _RichInfo()
                    {
                        id = (short)curLine,
                        type = 0,
                        point = new Point(offset, height),
                        text = endStrtext,
                        size = new Size(size.Width, size.Height),
                        color = color,
                        font = font.Size
                    });
                    offset += size.Width;
                    if (size.Height > curLineMaxHeight) curLineMaxHeight = size.Height;
                }
            }
        }
        /// <summary>
        /// 执行数据操作
        /// </summary>
        /// <param name="data"></param>
        private void DoData_Action(string data)
        {
            if ("" == data) return;
            if ('|' != data[0])
            {
                List<CellLinkInfo> links = new List<CellLinkInfo>();
                if (Grid != null)
                {
                    foreach (var grid in Grid)
                    {
                        if (grid.Grid.Length > 0 && grid.Grid[0].Item != null && grid.Grid[0].Link != null)
                        {
                            links.Add(new CellLinkInfo()
                            {
                                GridType = grid.Grid[0].GridType,
                                Slot = grid.Grid[0].Item.Slot,
                                Count = grid.Grid[0].LinkedCount,
                            });
                        }
                    }
                }
                if (CurrentControl != null)
                {
                    links.Add(
                        new CellLinkInfo()
                        {
                            GridType = GridType.NPCSelect,
                            Slot = int.Parse((string)CurrentControl.Tag),

                        });
                }
                CEnvir.Enqueue(new C.NPCButton { ButtonID = int.Parse(data), links = links });
                return;
            }

            var info = data.Split('|');
            if (info.Length == 0) return;
            foreach (var action in info)
            {
                if (action.IndexOf("url:") != -1)
                {
                    if (GameScene.Game.Observer) return;
                    string machstr = action.Trim(' ').Substring(action.IndexOf("url:") + 4).Trim(' ');
                    System.Diagnostics.Process.Start(machstr);
                }
                else if (action.IndexOf("pay:") != -1)
                {
                    if (GameScene.Game.Observer) return;
                    string machstr = action.Trim(' ').Substring(action.IndexOf("pay:") + 4).Trim(' ');
                    DXMessageBox box = new DXMessageBox("NPCDialog.Recharge".Lang(), "充值".Lang(), DXMessageBoxButtons.YesNo);
                    box.YesButton.MouseClick += (o1, e1) =>
                    {
                        if (string.IsNullOrEmpty(machstr)) return;
                        System.Diagnostics.Process.Start(machstr);
                    };
                }
                else if (action.IndexOf("action:") != -1)
                {
                    string machstr = action.Trim(' ').Substring(action.IndexOf("action:") + 7).Trim(' ');
                    if (machstr == "close")
                    {
                        Visible = false;
                    }
                }
                else if (action.IndexOf("item:") != -1)
                {
                    string machstr = action.Trim(' ').Substring(action.IndexOf("item:") + 5).Trim(' ');
                    var Sizer = machstr.Split('+');
                    var itemname = (Sizer.Length == 1 || Sizer.Length == 2) ? Sizer[0] : "";
                    var itemnoTip = (Sizer.Length == 2) ? Sizer[1] : "";
                    if (itemname != "")
                    {
                        DXItemCell cell;
                        cell = GameScene.Game.InventoryBox.Grid.Grid.FirstOrDefault(x => x?.Item?.Info.ItemName == itemname);
                        if (cell?.UseItem() == true)
                        {
                            GameScene.Game.ReceiveChat(@"使用物品成功".Lang(), MessageType.System);
                        }
                        else
                        {
                            if (itemnoTip != "") GameScene.Game.ReceiveChat(itemnoTip, MessageType.System); ;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 处理界面显示结果
        /// </summary>
        /// <param name="result"></param>
        private void ProcessResult(List<_RichInfo> result)
        {
            try
            {
                CurrentControl = null;
                PageChild = new DXControl[result.Count];
                for (int i = 0; i < result.Count; i++)
                {
                    _RichInfo richInfo = result[i];

                    if (0 == richInfo.type)   //文本
                    {
                        PageChild[i] = new DXLabel()
                        {
                            Parent = PageText,
                            Font = new Font(Config.FontName, CEnvir.FontSize(richInfo.font)),
                            Outline = PageText.Outline,
                            ForeColour = richInfo.color,
                            OutlineColour = PageText.OutlineColour,
                            PassThrough = true,
                            Location = richInfo.realpoint.IsEmpty ? richInfo.point : richInfo.realpoint,
                            Text = richInfo.text,
                        };
                    }
                    else if (1 == richInfo.type) //处理[text]
                    {
                        DXLabel e_text = new DXLabel()
                        {
                            Parent = PageText,
                            Font = new Font(Config.FontName, CEnvir.FontSize(richInfo.font)),
                            Outline = false,
                            ForeColour = richInfo.color,
                            OutlineColour = PageText.OutlineColour,
                            Location = richInfo.realpoint.IsEmpty ? richInfo.point : richInfo.realpoint,
                            Text = richInfo.text,
                            Sound = SoundIndex.ButtonC,
                            Tag = richInfo.data
                        };
                        PageChild[i] = e_text;
                        e_text.MouseEnter += (o, e) =>     //鼠标移动上去
                        {
                            if (GameScene.Game.Observer) return;
                            e_text.ForeColour = Color.Red;
                            e_text.Font = new Font(PageText.Font.FontFamily, PageText.Font.Size, FontStyle.Underline); //增加文字下划线
                        };
                        e_text.MouseLeave += (o, e) =>   //鼠标离开
                        {
                            if (GameScene.Game.Observer) return;
                            e_text.ForeColour = Color.Yellow;
                            e_text.Font = new Font(PageText.Font.FontFamily, PageText.Font.Size, FontStyle.Regular); //去掉文字下划线
                        };
                        DateTime NextButtonTime = DateTime.MinValue;
                        e_text.MouseClick += (o, e) =>
                        {
                            DXLabel but = o as DXLabel;
                            if (GameScene.Game.Observer || null == but) return;
                            if (int.Parse((string)but.Tag) == 0)
                            {
                                Visible = false;
                                return;
                            }

                            if (CEnvir.Now < NextButtonTime) return;

                            NextButtonTime = CEnvir.Now.AddSeconds(1);
                            DoData_Action((string)but.Tag);
                        };
                    }
                    else if (2 == richInfo.type) //Link
                    {

                        DXLabel e_text = new DXLabel()
                        {
                            Parent = PageText,
                            Font = new Font(Config.FontName, CEnvir.FontSize(richInfo.font), richInfo.underline ? FontStyle.Underline : FontStyle.Regular),
                            Outline = false,
                            ForeColour = richInfo.color,
                            OutlineColour = PageText.OutlineColour,
                            Location = richInfo.realpoint.IsEmpty ? richInfo.point : richInfo.realpoint,
                            Text = richInfo.text,
                            Sound = SoundIndex.ButtonC,
                            Tag = richInfo.data,
                            Tag_0 = richInfo.item
                        };
                        PageChild[i] = e_text;
                        e_text.MouseEnter += (o, e) =>     //鼠标移动上去
                        {
                            if (GameScene.Game.Observer) return;
                            e_text.ForeColour = richInfo.hcolor;
                            if (!richInfo.underline)
                                e_text.Font = new Font(PageText.Font.FontFamily, PageText.Font.Size, FontStyle.Underline); //增加文字下划线
                            //显示物品信息
                            if (null != richInfo.item && !string.IsNullOrEmpty(richInfo.item))
                            {
                                var data = new ClientUserItem(Globals.ItemInfoList.Binding.First(x => x.ItemName == richInfo.item), 1);
                                GameScene.Game.MouseItem = data;
                            }
                        };
                        e_text.MouseLeave += (o, e) =>   //鼠标离开
                        {
                            if (GameScene.Game.Observer) return;
                            e_text.ForeColour = richInfo.color;
                            if (!richInfo.underline)
                                e_text.Font = new Font(PageText.Font.FontFamily, PageText.Font.Size, FontStyle.Regular); //去掉文字下划线
                            GameScene.Game.MouseItem = null;
                        };
                        e_text.MouseDown += (o, e) =>   //鼠标按下
                        {
                            if (GameScene.Game.Observer) return;
                            e_text.ForeColour = richInfo.dcolor;
                        };
                        e_text.MouseUp += (o, e) =>   //鼠标抬起
                        {
                            if (GameScene.Game.Observer) return;
                            e_text.ForeColour = richInfo.hcolor;
                        };
                        DateTime NextButtonTime = DateTime.MinValue;
                        e_text.MouseClick += (o, e) =>
                        {
                            DXLabel but = o as DXLabel;
                            if (GameScene.Game.Observer || null == but) return;
                            if ((string)but.Tag == "")
                            {
                                Visible = false;
                                return;
                            }

                            if (CEnvir.Now < NextButtonTime) return;

                            NextButtonTime = CEnvir.Now.AddSeconds(1);

                            DoData_Action((string)but.Tag);
                        };
                    }
                    else if (3 == richInfo.type)
                    {
                        DXImageControl e_img = null;
                        if (richInfo.count != 0)
                        {
                            //动画
                            e_img = new DXAnimatedControl  //角色动画
                            {
                                Animated = true,   //动画开启
                                FrameCount = richInfo.count,  //动画帧数 100帧
                                AnimationDelay = richInfo.delay == 0 ? TimeSpan.FromMilliseconds(1000) : TimeSpan.FromMilliseconds(richInfo.delay),
                                Parent = PageText,
                                Location = richInfo.realpoint.IsEmpty ? richInfo.point : richInfo.realpoint,
                                Sound = SoundIndex.ButtonC,
                                Tag = richInfo.data,
                                Tag_0 = richInfo.item,
                                IsControl = true,
                                Visible = true,
                                Size = richInfo.size,
                                LibraryFile = (LibraryFile)richInfo.file,
                                BaseIndex = richInfo.idx,
                                FixedSize = true,
                                Blend = true,
                            };
                        }
                        else
                        {
                            //图片
                            e_img = new DXImageControl()
                            {
                                Parent = PageText,
                                Location = richInfo.realpoint.IsEmpty ? richInfo.point : richInfo.realpoint,
                                Sound = SoundIndex.ButtonC,
                                Tag = richInfo.data,
                                Tag_0 = richInfo.item,
                                IsControl = true,
                                Visible = true,
                                Size = richInfo.size,
                                LibraryFile = (LibraryFile)richInfo.file,
                                Index = richInfo.idx,
                                FixedSize = true,
                            };
                        }
                        PageChild[i] = e_img;
                        e_img.MouseEnter += (o, e) =>     //鼠标移动上去
                        {
                            if (GameScene.Game.Observer) return;
                            //显示物品信息
                            if (null != richInfo.item && !string.IsNullOrEmpty(richInfo.item))
                            {
                                var data = new ClientUserItem(Globals.ItemInfoList.Binding.First(x => x.ItemName == richInfo.item), 1);
                                if (richInfo.choose > 0)
                                {
                                    string[] strary = { "", "奖勋机会：百发百中", "奖励机会：十拿九稳", "奖励机会：小赌怡情", "奖励机会：中赌刺激", "奖励机会：大赌伤身", "奖励机会：希望渺茫", "奖助机会：痴心妄想" };
                                    data.ExInfo = strary[richInfo.choose];
                                    data.ExInfoOnly = false;   //置true就是显示奖励几率
                                }

                                GameScene.Game.MouseItem = data;
                            }
                        };
                        e_img.MouseLeave += (o, e) =>   //鼠标离开
                        {
                            if (GameScene.Game.Observer) return;
                            GameScene.Game.MouseItem = null;
                        };
                        e_img.MouseDown += (o, e) =>   //鼠标按下
                        {
                            if (GameScene.Game.Observer) return;
                        };
                        DateTime NextButtonTime = DateTime.MinValue;
                        e_img.MouseClick += (o, e) =>
                        {

                            DXImageControl but = o as DXImageControl;

                            if (GameScene.Game.Observer || null == but) return;
                            if ((string)but.Tag == "")
                            {
                                //Visible = false;
                                return;
                            }
                            if (richInfo.select)
                            {
                                CurrentControl = but;
                                return;


                            }


                            if (CEnvir.Now < NextButtonTime) return;

                            NextButtonTime = CEnvir.Now.AddSeconds(1);
                            DoData_Action((string)but.Tag);
                        };
                    }
                    else if (4 == richInfo.type)
                    {
                        //按钮
                        DxMirButton e_btn = new DxMirButton()
                        {
                            Parent = PageText,
                            Location = richInfo.realpoint.IsEmpty ? richInfo.point : richInfo.realpoint,
                            Sound = SoundIndex.ButtonC,
                            Tag = richInfo.data,
                            Tag_0 = richInfo.item,
                            IsControl = true,
                            Visible = true,
                            Size = richInfo.size,
                            LibraryFile = (LibraryFile)richInfo.file,
                            Index = richInfo.idx,
                            FixedSize = true,
                            MirButtonType = (MirButtonType)richInfo.mirbtntype,

                        };
                        PageChild[i] = e_btn;
                        e_btn.MouseEnter += (o, e) =>     //鼠标移动上去
                        {
                            if (GameScene.Game.Observer) return;
                            //显示物品信息
                            if (null != richInfo.item && !string.IsNullOrEmpty(richInfo.item))
                            {
                                var data = new ClientUserItem(Globals.ItemInfoList.Binding.First(x => x.ItemName == richInfo.item), 1);
                                GameScene.Game.MouseItem = data;
                            }
                        };
                        e_btn.MouseLeave += (o, e) =>   //鼠标离开
                        {
                            if (GameScene.Game.Observer) return;
                            GameScene.Game.MouseItem = null;
                        };
                        e_btn.MouseDown += (o, e) =>   //鼠标按下
                        {
                            if (GameScene.Game.Observer) return;
                        };
                        DateTime NextButtonTime = DateTime.MinValue;
                        e_btn.MouseClick += (o, e) =>
                        {
                            DXImageControl but = o as DXImageControl;
                            if (GameScene.Game.Observer || null == but) return;
                            if ((string)but.Tag == "")
                            {
                                Visible = false;
                                return;
                            }

                            if (CEnvir.Now < NextButtonTime) return;

                            NextButtonTime = CEnvir.Now.AddSeconds(1);
                            DoData_Action((string)but.Tag);
                        };
                    }
                }
                for (int i = 0; i < result.Count; i++)
                {
                    _RichInfo richInfo = result[i];
                    if (richInfo.tier == 1)
                        PageChild[i].TopShow_Parent();
                    else if (richInfo.tier == -1)
                        PageChild[i].BottomShow_Parent();
                }
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }
        }
        /// <summary>
        /// 进程文本
        /// </summary>
        /// <returns></returns>
        private int ProcessText()
        {
            List<DXControl> Controls = new List<DXControl>();
            int height = 0;
            int curLine = 0;
            try
            {
                //匹配到标签位置
                List<_RichInfo> result = new List<_RichInfo>();

                Regex regline = new Regex(@".*\n", RegexOptions.Compiled);
                MatchCollection matchlines = regline.Matches(Page.Say + "\n");
                for (int i = 0; i < matchlines.Count; i++)
                {
                    //解析单行中是否有富文本内容
                    string line = matchlines[i].Value.TrimEnd('\n');//.Replace('\n', '\0');
                    MatchCollection labelstest = RegexHtml.Matches(line);
                    int offset = 0;
                    int beginIndex = 0;
                    int curLineMaxHeight = 0;
                    if (labelstest.Count == 0)
                    {
                        Size DefSize = DXLabel.GetSize("A", PageText.Font, PageText.Outline, new Size(4096, 4096));
                        AddStringAt(line, ref offset, ref curLine, ref height, ref result, ref curLineMaxHeight, PageText.Outline, PageText.Font);
                        Add_DefHeight(DefSize.Height, ref curLine, ref height);
                        offset = 0;
                        continue;
                    }
                    else
                    {
                        for (int j = 0; j < labelstest.Count; j++)
                        {
                            var matchCollection = labelstest[j];
                            curLineMaxHeight = 0;
                            if (beginIndex != matchCollection.Index && matchCollection.Index > beginIndex)
                            {
                                string strtext = line.Substring(beginIndex, matchCollection.Index - beginIndex).TrimStart('\t');
                                if (strtext != "")
                                {
                                    AddStringAt(strtext, ref offset, ref curLine, ref height, ref result, ref curLineMaxHeight, PageText.Outline, PageText.Font);
                                }
                            }
                            beginIndex = matchCollection.Index + matchCollection.Length;
                            //处理完开头 处理<>中的内容
                            //首先<>中的内容
                            string machstr = line.Substring(matchCollection.Index, matchCollection.Length);
                            var info = machstr.Split(' ');
                            if (info.Length != 0)      //先处理font
                            {
                                float r_size = 9f;
                                Color r_color = PageText.ForeColour;
                                Color r_hcolor = PageText.ForeColour;
                                Color r_dcolor = PageText.ForeColour;
                                string r_text = "";
                                string r_data = "";
                                string r_item = "";
                                int i_file = 0;
                                int i_idx = 0;
                                int i_count = 0;
                                int i_delay = 0;
                                int i_w = 0;
                                int i_h = 0;
                                string i_data = "";
                                string i_item = "";
                                int i_tier = 0;
                                bool b_sel = false;
                                int i_cho = 0;
                                int i_pos_x = 0;
                                int i_pos_y = 0;
                                int i_mirbtntype = 0;
                                bool b_underline = true;
                                if (info[0].IndexOf("font") != -1)
                                {
                                    //处理文字内容
                                    foreach (var cc in info)
                                    {
                                        if (cc.IndexOf("size=") != -1)
                                        {
                                            r_size = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.IndexOf("size=") + 5), @"[^0-9]+", ""));
                                        }
                                        else if (cc.IndexOf("color=") != -1)
                                        {
                                            r_color = ColorTranslator.FromHtml(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.IndexOf("color=") + 6), @"[^0-9a-fA-Fx]+", ""));
                                        }
                                        else if (cc.ToUpper().IndexOf("X=") != -1)
                                        {
                                            i_pos_x = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("X=") + 2), @"[^0-9]+", ""));
                                        }
                                        else if (cc.ToUpper().IndexOf("Y=") != -1)
                                        {
                                            i_pos_y = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("Y=") + 2), @"[^0-9]+", ""));
                                        }
                                        else if (cc.ToUpper().IndexOf("TIER=") != -1)
                                        {
                                            i_tier = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("TIER=") + 5), @"[^0-9-]+", ""));
                                        }

                                    }
                                    if (i_pos_x == 0 && i_pos_y == 0)
                                    {
                                        if (j + 1 < labelstest.Count)
                                        {
                                            AddStringAt(line.Substring(beginIndex, labelstest[j + 1].Index - beginIndex).Trim(' '), ref offset,
                                                ref curLine, ref height, ref result, ref curLineMaxHeight, r_color, PageText.Outline, new Font(Config.FontName, CEnvir.FontSize(r_size)));
                                            beginIndex = labelstest[j + 1].Index + labelstest[j + 1].Length;
                                            j++;
                                        }
                                    }
                                    else
                                    {
                                        if (j + 1 < labelstest.Count)
                                        {
                                            result.Add(new _RichInfo()
                                            {
                                                id = (short)0,
                                                type = 0,
                                                point = new Point(0, 0),
                                                text = line.Substring(beginIndex, labelstest[j + 1].Index - beginIndex).Trim(' '),
                                                size = new Size(4096, 4096),
                                                color = r_color,
                                                font = r_size,
                                                realpoint = (i_pos_x != 0 || i_pos_y != 0) ? new Point(i_pos_x, i_pos_y) : new Point(0, 0)
                                            });
                                            beginIndex = labelstest[j + 1].Index + labelstest[j + 1].Length;
                                            j++;
                                        }
                                    }
                                }
                                else if (info[0].IndexOf("link") != -1)
                                {
                                    foreach (var cc in info)
                                    {
                                        if (cc.IndexOf("size=") != -1)
                                        {
                                            r_size = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.IndexOf("size=") + 5), @"[^0-9]+", ""));
                                        }
                                        else if (cc.IndexOf("dcolor=") != -1)
                                        {
                                            r_dcolor = ColorTranslator.FromHtml(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.IndexOf("color=") + 6), @"[^0-9a-fA-Fx]+", ""));
                                        }
                                        else if (cc.IndexOf("hcolor=") != -1)
                                        {
                                            r_hcolor = ColorTranslator.FromHtml(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.IndexOf("color=") + 6), @"[^0-9a-fA-Fx]+", ""));
                                        }
                                        else if (cc.IndexOf("color=") != -1)
                                        {
                                            r_color = ColorTranslator.FromHtml(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.IndexOf("color=") + 6), @"[^0-9a-fA-Fx]+", ""));
                                        }
                                        else if (cc.IndexOf("text=") != -1)
                                        {
                                            r_text = cc.Substring(cc.IndexOf("text=") + 5).Trim('"');
                                        }
                                        else if (cc.IndexOf("data=") != -1)
                                        {
                                            r_data = cc.Substring(cc.IndexOf("data=") + 5).Trim('"');
                                        }
                                        else if (cc.IndexOf("item=") != -1)
                                        {
                                            r_item = cc.Substring(cc.IndexOf("item=") + 5).Trim('"');
                                        }
                                        else if (cc.ToUpper().IndexOf("X=") != -1)
                                        {
                                            i_pos_x = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("X=") + 2), @"[^0-9]+", ""));
                                        }
                                        else if (cc.ToUpper().IndexOf("Y=") != -1)
                                        {
                                            i_pos_y = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("Y=") + 2), @"[^0-9]+", ""));
                                        }
                                        else if (cc.ToUpper().IndexOf("TIER=") != -1)
                                        {
                                            i_tier = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("TIER=") + 5), @"[^0-9-]+", ""));
                                        }
                                        else if (cc.ToUpper().IndexOf("UNDERLINE=") != -1)
                                        {
                                            b_underline = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("UNDERLINE=") + 10), @"[^0-9-]+", "")) == 1;
                                        }
                                    }
                                    if (i_pos_x == 0 && i_pos_y == 0)
                                    {
                                        var sSize = DXLabel.GetSize(r_text, new Font(Config.FontName, CEnvir.FontSize(r_size)),
                                           PageText.Outline, new Size(4096, 4096), TextFormatFlags.SingleLine);
                                        if (sSize.Height > curLineMaxHeight) curLineMaxHeight = sSize.Height;
                                        //构造一个_RichInfo
                                        result.Add(new _RichInfo()
                                        {
                                            id = (short)curLine,
                                            type = 2,
                                            point = new Point(offset, height),
                                            text = r_text,
                                            color = r_color,
                                            dcolor = r_dcolor,
                                            hcolor = r_hcolor,
                                            font = r_size,
                                            data = r_data,
                                            size = sSize,
                                            item = r_item,
                                            underline = b_underline,
                                        });
                                        offset += sSize.Width;
                                        if (offset > PageText.Size.Width)
                                        {
                                            Add_DefHeight(curLineMaxHeight, ref curLine, ref height);
                                            offset = 0;
                                            curLineMaxHeight = 0;
                                        }
                                    }
                                    else
                                    {
                                        result.Add(new _RichInfo()
                                        {
                                            id = (short)0,
                                            type = 2,
                                            point = new Point(0, 0),
                                            text = r_text,
                                            color = r_color,
                                            dcolor = r_dcolor,
                                            hcolor = r_hcolor,
                                            font = r_size,
                                            data = r_data,
                                            size = new Size(4096, 4096),
                                            item = r_item,
                                            underline = b_underline,
                                            realpoint = (i_pos_x != 0 || i_pos_y != 0) ? new Point(i_pos_x, i_pos_y) : new Point(0, 0)
                                        });

                                    }
                                }
                                else if (info[0].IndexOf("img") != -1)
                                {
                                    //处理图片内容
                                    foreach (var cc in info)
                                    {
                                        if (cc == "") continue;
                                        if (cc.ToUpper().IndexOf("FILE=") != -1)
                                        {
                                            i_file = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("FILE=") + 5), @"[^0-9]+", ""));
                                        }
                                        else if (cc.ToUpper().IndexOf("IDX=") != -1)
                                        {
                                            i_idx = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("IDX=") + 4), @"[^0-9]+", ""));
                                        }
                                        else if (cc.ToUpper().IndexOf("COUNT=") != -1)
                                        {
                                            i_count = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("COUNT=") + 6), @"[^0-9]+", ""));
                                        }
                                        else if (cc.ToUpper().IndexOf("DELAY=") != -1)
                                        {
                                            i_delay = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("DELAY=") + 6), @"[^0-9]+", ""));
                                        }
                                        else if (cc.ToUpper().IndexOf("W=") != -1)
                                        {
                                            i_w = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("W=") + 2), @"[^0-9]+", ""));
                                        }
                                        else if (cc.ToUpper().IndexOf("H=") != -1)
                                        {
                                            i_h = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("H=") + 2), @"[^0-9]+", ""));
                                        }
                                        else if (cc.ToUpper().IndexOf("DATA=") != -1)
                                        {
                                            i_data = cc.Substring(cc.ToUpper().IndexOf("DATA=") + 5).Trim('"');
                                        }
                                        else if (cc.ToUpper().IndexOf("ITEM=") != -1)
                                        {
                                            i_item = cc.Substring(cc.ToUpper().IndexOf("ITEM=") + 5).Trim('"');
                                        }
                                        else if (cc.ToUpper().IndexOf("X=") != -1)
                                        {
                                            i_pos_x = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("X=") + 2), @"[^0-9]+", ""));
                                        }
                                        else if (cc.ToUpper().IndexOf("Y=") != -1)
                                        {
                                            i_pos_y = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("Y=") + 2), @"[^0-9]+", ""));
                                        }
                                        else if (cc.ToUpper().IndexOf("TIER=") != -1)
                                        {
                                            i_tier = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("TIER=") + 5), @"[^0-9-]+", ""));
                                        }
                                        else if (cc.ToUpper().IndexOf("SELECT=") != -1)
                                        {
                                            b_sel = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("SELECT=") + 7), @"[^0-9-]+", "")) == 1;
                                        }
                                        else if (cc.ToUpper().IndexOf("CHOOSE=") != -1)
                                        {
                                            i_cho = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("SELECT=") + 7), @"[^0-9-]+", ""));
                                        }
                                    }
                                    //w影响offset h影响 curLineMaxHeight
                                    //i_w和i_h 如果没有设置的 用图片本身的大小
                                    if (i_file == 0 || i_idx == 0) continue;

                                    MirLibrary Library = null;
                                    if (CEnvir.LibraryList.TryGetValue((LibraryFile)i_file, out Library))
                                    {
                                        i_w = i_w == 0 ? Library.GetSize(i_idx).Width : i_w;
                                        i_h = i_h == 0 ? Library.GetSize(i_idx).Height : i_h;
                                    }
                                    if (i_w != 0 && i_h != 0)
                                    {
                                        //物品默认偏移2个像素
                                        offset += 2;
                                        height += 2;
                                        //构造一个_RichInfo
                                        result.Add(new _RichInfo()
                                        {
                                            id = (short)curLine,
                                            type = 3,
                                            point = new Point(offset, height),
                                            data = i_data,
                                            size = new Size(i_w, i_h),
                                            item = i_item,
                                            file = i_file,
                                            idx = i_idx,
                                            count = i_count,
                                            delay = i_delay,
                                            realpoint = (i_pos_x != 0 || i_pos_y != 0) ? new Point(i_pos_x, i_pos_y) : new Point(0, 0),
                                            select = b_sel,
                                            choose = i_cho,
                                        });
                                        if (i_pos_x == 0 && i_pos_y == 0)
                                        {
                                            offset += i_w;
                                            if (i_h > curLineMaxHeight) curLineMaxHeight = i_h;
                                            if (offset > PageText.Size.Width)
                                            {
                                                Add_DefHeight(curLineMaxHeight, ref curLine, ref height);
                                                offset = 0;
                                                curLineMaxHeight = 0;
                                            }
                                        }
                                    }
                                }
                                else if (info[0].IndexOf("btn") != -1)
                                {
                                    //处理btn
                                    foreach (var cc in info)
                                    {
                                        if (cc == "") continue;
                                        if (cc.ToUpper().IndexOf("FILE=") != -1)
                                        {
                                            i_file = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("FILE=") + 5), @"[^0-9]+", ""));
                                        }
                                        else if (cc.ToUpper().IndexOf("IDX=") != -1)
                                        {
                                            i_idx = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("IDX=") + 4), @"[^0-9]+", ""));
                                        }
                                        else if (cc.ToUpper().IndexOf("W=") != -1)
                                        {
                                            i_w = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("W=") + 2), @"[^0-9]+", ""));
                                        }
                                        else if (cc.ToUpper().IndexOf("H=") != -1)
                                        {
                                            i_h = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("H=") + 2), @"[^0-9]+", ""));
                                        }
                                        else if (cc.ToUpper().IndexOf("DATA=") != -1)
                                        {
                                            i_data = cc.Substring(cc.ToUpper().IndexOf("DATA=") + 5).Trim('"');
                                        }
                                        else if (cc.ToUpper().IndexOf("ITEM=") != -1)
                                        {
                                            i_item = cc.Substring(cc.ToUpper().IndexOf("ITEM=") + 5).Trim('"');
                                        }
                                        else if (cc.ToUpper().IndexOf("X=") != -1)
                                        {
                                            i_pos_x = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("X=") + 2), @"[^0-9]+", ""));
                                        }
                                        else if (cc.ToUpper().IndexOf("Y=") != -1)
                                        {
                                            i_pos_y = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("Y=") + 2), @"[^0-9]+", ""));
                                        }
                                        else if (cc.ToUpper().IndexOf("TIER=") != -1)
                                        {
                                            i_tier = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("TIER=") + 5), @"[^0-9-]+", ""));
                                        }
                                        else if (cc.ToUpper().IndexOf("MIRBTNTYPE=") != -1)
                                        {
                                            i_mirbtntype = int.Parse(System.Text.RegularExpressions.Regex.Replace(cc.Substring(cc.ToUpper().IndexOf("MIRBTNTYPE=") + 11), @"[^0-9-]+", ""));
                                        }
                                    }
                                    //w影响offset h影响 curLineMaxHeight
                                    //i_w和i_h 如果没有设置的 用图片本身的大小
                                    if (i_file == 0 || i_idx == 0) continue;
                                    MirLibrary Library = null;
                                    if (CEnvir.LibraryList.TryGetValue((LibraryFile)i_file, out Library))
                                    {
                                        i_w = i_w == 0 ? Library.GetSize(i_idx).Width : i_w;
                                        i_h = i_h == 0 ? Library.GetSize(i_idx).Height : i_h;
                                    }
                                    if (i_w != 0 && i_h != 0)
                                    {
                                        Point Richpos_ = new Point(offset + 2, height + 2);
                                        //构造一个_RichInfo
                                        result.Add(new _RichInfo()
                                        {
                                            id = (short)curLine,
                                            type = 4,
                                            point = Richpos_,
                                            data = i_data,
                                            size = new Size(i_w, i_h),
                                            item = i_item,
                                            file = i_file,
                                            idx = i_idx,
                                            tier = i_tier,
                                            realpoint = (i_pos_x != 0 || i_pos_y != 0) ? new Point(i_pos_x, i_pos_y) : new Point(0, 0),
                                            mirbtntype = i_mirbtntype
                                        });
                                        if (i_pos_x == 0 && i_pos_y == 0)
                                        {
                                            offset += i_w;
                                            if (i_h > curLineMaxHeight) curLineMaxHeight = i_h;
                                            if (offset > PageText.Size.Width)
                                            {
                                                Add_DefHeight(curLineMaxHeight, ref curLine, ref height);
                                                offset = 0;
                                                curLineMaxHeight = 0;
                                            }
                                        }
                                    }
                                }
                                else if (info[0].IndexOf("[") != -1)
                                {
                                    //处理[text]
                                    MatchCollection labels = RegexText.Matches(info[0]);
                                    //for(int q =0; q < labels.Count; q++ )
                                    if (labels.Count > 0)
                                    {
                                        //不支持换行
                                        string text = labels[0].Groups["Text"].Value;
                                        string number = labels[0].Groups["ID"].Value;
                                        var sSize = DXLabel.GetSize(text, PageText.Font, PageText.Outline, new Size(4096, 4096), TextFormatFlags.SingleLine);
                                        if (sSize.Height > curLineMaxHeight) curLineMaxHeight = sSize.Height;
                                        //构造一个_RichInfo
                                        result.Add(new _RichInfo()
                                        {
                                            id = (short)curLine,
                                            type = 1,
                                            point = new Point(offset, height),
                                            text = text,
                                            color = Color.Yellow,
                                            font = 9f,
                                            data = number,
                                            size = sSize
                                        });
                                        offset += sSize.Width;
                                        if (offset > PageText.Size.Width)
                                        {
                                            Add_DefHeight(curLineMaxHeight, ref curLine, ref height);
                                            offset = 0;
                                            curLineMaxHeight = 0;
                                        }
                                    }
                                }
                            }
                        }
                        if (beginIndex != line.Length)
                        {
                            AddStringAt(line.Substring(beginIndex, line.Length - beginIndex), ref offset,
                                            ref curLine, ref height, ref result, ref curLineMaxHeight, PageText.ForeColour, PageText.Outline, PageText.Font);
                        }
                        Add_DefHeight(curLineMaxHeight, ref curLine, ref height);
                        offset = 0;
                    }
                }

                ProcessResult(result);
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }
            return height;
        }

        #endregion Methods

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Page = null;

                if (PageText != null)
                {
                    if (!PageText.IsDisposed)
                        PageText.Dispose();

                    PageText = null;
                }
                if (TopPanel != null)
                {
                    if (!TopPanel.IsDisposed)
                        TopPanel.Dispose();
                    TopPanel = null;
                }
                if (ContentPanel != null)
                {
                    if (!ContentPanel.IsDisposed)
                        ContentPanel.Dispose();
                    ContentPanel = null;
                }

                if (BottomPanel != null)
                {
                    if (!BottomPanel.IsDisposed)
                        BottomPanel.Dispose();
                    BottomPanel = null;
                }
                if (CustomPanel != null)
                {
                    if (!CustomPanel.IsDisposed)
                        CustomPanel.Dispose();
                    CustomPanel = null;
                }
                if (Close1Button != null)
                {
                    if (!Close1Button.IsDisposed)
                        Close1Button.Dispose();
                    Close1Button = null;
                }
                Opened = false;
                ClearGrid();
            }
        }

        #endregion

        #endregion

        #region 富文本

        /// <summary>
        /// 富文本信息
        /// </summary>
        public struct _RichInfo
        {
            /// <summary>
            /// 单独的id
            /// </summary>
            public short id;
            /// <summary>
            /// 显示的类型 0 font  1 [text]  2 link 3 img  4 btn 
            /// </summary>
            public short type;
            /// <summary>
            /// 显示的文字
            /// </summary>
            public string text;
            /// <summary>
            /// 占用的大小
            /// </summary>
            public Size size;
            /// <summary>
            /// 显示的颜色
            /// </summary>
            public Color color;
            /// <summary>
            /// 显示的颜色
            /// </summary>
            public Color hcolor;
            /// <summary>
            /// 显示的颜色
            /// </summary>
            public Color dcolor;
            /// <summary>
            /// 起始坐标()左上角为(0,0)
            /// </summary>
            public Point point;
            /// <summary>
            /// 字体的大小
            /// </summary>
            public float font;
            /// <summary>
            /// 附加数据
            /// </summary>
            public string data;

            /// <summary>
            /// 图片资源
            /// </summary>
            public int file;
            /// <summary>
            /// 图片资源索引
            /// </summary>
            public int idx;
            /// <summary>
            /// 动画的数量
            /// </summary>
            public int count;
            /// <summary>
            /// 动画帧的间隔
            /// </summary>
            public int delay;

            /// <summary>
            /// 自定义的按钮样式对应MirButtonType
            /// </summary>
            public int mirbtntype;

            /// <summary>
            /// 道具名
            /// </summary>
            public string item;
            /// <summary>
            /// 如果脚本中设置了x,y 要使用该坐标
            /// </summary>
            public Point realpoint;
            /// <summary>
            /// 0正常排序,-1排在底层 1排在顶层
            /// </summary>
            public int tier;
            /// <summary>
            /// 是否可选
            /// </summary>
            public bool select;
            /// <summary>
            /// 概率序号
            /// </summary>
            public int choose;
            /// <summary>
            /// 显示下划线
            /// </summary>
            public bool underline;
        }
        #endregion

    }

    #region NPC购买

    /// <summary>
    /// NPC购买功能
    /// </summary>
    public class NPCGoodsDialog : DXWindow
    {
        #region Properties

        #region SelectedCell
        /// <summary>
        /// 选定单元格
        /// </summary>
        public NPCGoodsCell SelectedCell
        {
            get => _SelectedCell;
            set
            {
                if (_SelectedCell == value) return;

                NPCGoodsCell oldValue = _SelectedCell;
                _SelectedCell = value;

                OnSelectedCellChanged(oldValue, value);
            }
        }
        private NPCGoodsCell _SelectedCell;
        public event EventHandler<EventArgs> SelectedCellChanged;
        public void OnSelectedCellChanged(NPCGoodsCell oValue, NPCGoodsCell nValue)
        {
            if (oValue != null)
            {
                oValue.Selected = false;
            }
            if (nValue != null)
            {
                nValue.Selected = true;
            }
            BuyButton.Enabled = nValue != null;
            SelectedCellChanged?.Invoke(this, EventArgs.Empty);
        }

        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);

            if (IsVisible)
            {
                if (GameScene.Game.InventoryBox == null) return;
                GameScene.Game.InventoryBox.Visible = true;
                GameScene.Game.InventoryBox.InventoryType = InventoryType.BuySell;
                GameScene.Game.InventoryBox.BringToFront();
            }
            else
            {
                if (BuyButton == null) return;
                BuyButton.Enabled = nValue;
            }
        }
        #endregion

        protected DXVScrollBar ScrollBar;
        public DXCheckBox GuildCheckBox;
        public DXButton Close1Button;

        public List<NPCGoodsCell> Cells = new List<NPCGoodsCell>();
        protected DXButton BuyButton;
        public DXImageControl ClientPanel;
        public DXControl GoodsPanel;

        /*public override void OnClientAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            base.OnClientAreaChanged(oValue, nValue);

            if (ClientPanel == null) return;

            //这个Size是 国定的不能动的
            //ClientPanel.Size = ClientArea.Size;
            //ClientPanel.Location = ClientArea.Location;

        }*/

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        #endregion

        /// <summary>
        /// NPC购买界面
        /// </summary>
        public NPCGoodsDialog()
        {
            //TitleLabel.Text = "商品";
            HasTitle = false;   //标题不显示
            HasFooter = false;  //不显示页脚
            Movable = false;
            CloseButton.Visible = false;

            SetClientSize(new Size(272, 4 * 43 + 1));

            ClientPanel = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1260,
                Opacity = 0.7F,
                Location = new Point(0, 0),
                IsControl = true,
                PassThrough = true,
            };

            GoodsPanel = new DXControl
            {
                Parent = ClientPanel,
                Size = new Size(245, 165),//ClientArea.Size,
                Location = new Point(8, 14),//ClientArea.Location,
                PassThrough = true,
            };

            ScrollBar = new DXVScrollBar   //购物滚动条
            {
                Parent = ClientPanel,
                Size = new Size(24, ClientArea.Height - 1),
                Change = 42,
            };
            ScrollBar.Location = new Point(ClientArea.Right - ScrollBar.Size.Width, ClientArea.Y - 10);
            ScrollBar.ValueChanged += (o, e) => UpdateLocations();

            MouseWheel += ScrollBar.DoMouseWheel;
            ClientPanel.MouseWheel += ScrollBar.DoMouseWheel;

            BuyButton = new DXButton     //购买按钮
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1263,
                Location = new Point(40, Size.Height - 43),
                Parent = this,
                Enabled = false,
            };
            BuyButton.MouseClick += (o, e) => Buy();

            GuildCheckBox = new DXCheckBox
            {
                Parent = this,
                Enabled = false,
                Hint = "使用行会资金".Lang(),
            };
            GuildCheckBox.Location = new Point(85, BuyButton.Location.Y + (BuyButton.Size.Height - GuildCheckBox.Size.Height) / 2 + 2);

            Close1Button = new DXButton   //关闭按钮
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1221,
            };
            Close1Button.Location = new Point(256, 181);
            Close1Button.MouseClick += (o, e) => Visible = false;

        }

        #region Methods
        /// <summary>
        /// 新商品
        /// </summary>
        /// <param name="goods"></param>
        public virtual void NewGoods(IList<ClientNPCGood> goods)
        {
            foreach (NPCGoodsCell cell in Cells)
                cell.Dispose();

            Cells.Clear();

            foreach (ClientNPCGood good in goods)
            {
                NPCGoodsCell cell;
                if (good.Currency != Globals.Currency)           //买卖货币图标更改
                {
                    // 找到目标物品的image值
                    ItemInfo targetIteminfo = Globals.ItemInfoList.Binding.First(x => x.ItemName == good.Currency);
                    // 作为参数传入
                    Cells.Add(cell = new NPCGoodsCell(targetIteminfo.Image)
                    {
                        Parent = GoodsPanel,
                        Good = good
                    });
                }
                else
                {
                    Cells.Add(cell = new NPCGoodsCell
                    {
                        Parent = GoodsPanel,
                        Good = good,
                    });
                }
                cell.ItemCell.MouseClick += (o, e) => SelectedCell = cell;
                cell.ItemDetial.MouseClick += (o, e) => SelectedCell = cell;

                cell.ItemCell.MouseWheel += ScrollBar.DoMouseWheel;
                cell.ItemDetial.MouseWheel += ScrollBar.DoMouseWheel;

                cell.ItemCell.MouseDoubleClick += (o, e) => Buy();
                cell.ItemDetial.MouseDoubleClick += (o, e) => Buy();
            }

            if (goods.Count < 4)
            {
                ScrollBar.MaxValue = 4 * 42;
            }
            else
            {
                ScrollBar.MaxValue = goods.Count * 42;
            }
            ScrollBar.VisibleSize = 4 * 42;
            SetClientSize(new Size(ClientArea.Width, Math.Min(ScrollBar.MaxValue, 4 * 43 - 3) + 22));
            ScrollBar.Size = new Size(ScrollBar.Size.Width, ClientPanel.Size.Height - 60);
            //为滚动条自定义皮肤 -1为不设置
            ScrollBar.SetSkin(LibraryFile.UI1, -1, -1, -1, 1225);

            BuyButton.Location = new Point(105, Size.Height - 40);
            GuildCheckBox.Location = new Point(80, BuyButton.Location.Y + (BuyButton.Size.Height - GuildCheckBox.Size.Height) / 2 + 2);
            ScrollBar.Value = 0;
            UpdateLocations();
        }
        /// <summary>
        /// 更新坐标
        /// </summary>
        protected virtual void UpdateLocations()
        {
            int y = -ScrollBar.Value;

            foreach (NPCGoodsCell cell in Cells)
            {
                cell.Location = new Point(10, y);

                y += cell.Size.Height + 3;
            }
        }
        /// <summary>
        /// 购买
        /// </summary>
        protected virtual void Buy()
        {
            if (GameScene.Game.Observer) return;

            if (SelectedCell == null) return;

            long gold = MapObject.User.Gold;

            if (GuildCheckBox.Checked && GameScene.Game.GuildBox.GuildInfo != null)
                gold = GameScene.Game.GuildBox.GuildInfo.GuildFunds;

            if (SelectedCell.Good.Currency != Globals.Currency) // 以物换物 应该不涉及回购
            {
                //此处客户端发包给服务端判断
                CEnvir.Enqueue(new C.NPCBuy { Index = SelectedCell.Good.Index, Amount = 1, GuildFunds = GuildCheckBox.Checked });
                GuildCheckBox.Checked = false;
            }
            else //正常金币购物
            {
                if (SelectedCell.Good.Item.StackSize > 1) // 可以堆叠的物品
                {
                    long maxCount = SelectedCell.Good.Item.StackSize;

                    maxCount = Math.Min(maxCount, gold / SelectedCell.Good.Cost);

                    if (SelectedCell.Good.Item.Weight > 0)
                    {
                        switch (SelectedCell.Good.Item.ItemType)
                        {
                            case ItemType.Amulet:
                            case ItemType.Poison:
                                if (MapObject.User.Stats[Stat.BagWeight] - MapObject.User.BagWeight < SelectedCell.Good.Item.Weight)
                                {
                                    GameScene.Game.ReceiveChat($"NPCDialog.BuyCount".Lang(SelectedCell.Good.Item.ItemName), MessageType.System);
                                    return;
                                }
                                break;
                            default:
                                maxCount = Math.Min(maxCount, (MapObject.User.Stats[Stat.BagWeight] - MapObject.User.BagWeight) / SelectedCell.Good.Item.Weight);
                                break;
                        }
                    }

                    if (maxCount < 0)
                    {
                        GameScene.Game.ReceiveChat($"NPCDialog.BuyCount".Lang(SelectedCell.Good.Item.ItemName), MessageType.System);
                        return;
                    }

                    ClientUserItem item = new ClientUserItem(SelectedCell.Good.Item, (int)Math.Min(int.MaxValue, maxCount));

                    DXItemAmountWindow window = new DXItemAmountWindow("购买物品".Lang(), item);
                    window.ConfirmButton.MouseClick += (o, e) =>
                    {
                        CEnvir.Enqueue(new C.NPCBuy { Index = SelectedCell.Good.Index, Amount = window.Amount, GuildFunds = GuildCheckBox.Checked });
                        GuildCheckBox.Checked = false;
                    };

                }
                else
                {
                    if (MapObject.User.Stats[Stat.BagWeight] - MapObject.User.BagWeight < SelectedCell.Good.Item.Weight)
                    {
                        GameScene.Game.ReceiveChat($"NPCDialog.BuyCount".Lang(SelectedCell.Good.Item.ItemName), MessageType.System);
                        return;
                    }

                    if (SelectedCell.Good.Cost > gold)
                    {
                        GameScene.Game.ReceiveChat($"NPCDialog.BuyGold".Lang(SelectedCell.Good.Item.ItemName), MessageType.System);
                        return;
                    }

                    CEnvir.Enqueue(new C.NPCBuy
                    {
                        Index = SelectedCell.IsBuybackCell ? SelectedCell.Good.UserItem.Index : SelectedCell.Good.Index,
                        IsBuyback = SelectedCell.IsBuybackCell,
                        Amount = 1,
                        GuildFunds = GuildCheckBox.Checked
                    });
                    GuildCheckBox.Checked = false;
                }
            }
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _SelectedCell = null;
                SelectedCellChanged = null;

                if (GoodsPanel != null)
                {
                    if (!GoodsPanel.IsDisposed)
                        GoodsPanel.Dispose();

                    GoodsPanel = null;
                }

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                if (GuildCheckBox != null)
                {
                    if (!GuildCheckBox.IsDisposed)
                        GuildCheckBox.Dispose();

                    GuildCheckBox = null;
                }

                if (BuyButton != null)
                {
                    if (!BuyButton.IsDisposed)
                        BuyButton.Dispose();

                    BuyButton = null;
                }

                if (ClientPanel != null)
                {
                    if (!ClientPanel.IsDisposed)
                        ClientPanel.Dispose();

                    ClientPanel = null;
                }

                if (Cells != null)
                {
                    for (int i = 0; i < Cells.Count; i++)
                    {
                        if (Cells[i] != null)
                        {
                            if (!Cells[i].IsDisposed)
                                Cells[i].Dispose();

                            Cells[i] = null;
                        }
                    }

                    Cells.Clear();
                    Cells = null;
                }
            }
        }
        #endregion
    }

    #endregion

    #region NPC商品单元

    /// <summary>
    /// NPC商品单元格
    /// </summary>
    public sealed class NPCGoodsCell : DXControl
    {
        #region Properties

        #region Good
        /// <summary>
        /// 商品
        /// </summary>
        public ClientNPCGood Good
        {
            get => _Good;
            set
            {
                if (_Good == value) return;

                ClientNPCGood oldValue = _Good;
                _Good = value;

                OnGoodChanged(oldValue, value);
            }
        }
        private ClientNPCGood _Good;
        public event EventHandler<EventArgs> GoodChanged;
        public void OnGoodChanged(ClientNPCGood oValue, ClientNPCGood nValue)
        {
            if (nValue.UserItem != null)
            {
                ItemCell.Item = new ClientUserItem(nValue.UserItem, 1) { Flags = UserItemFlags.None };
            }
            else
            {
                ItemCell.Item = new ClientUserItem(nValue.Item, 1) { Flags = UserItemFlags.None };
            }

            switch (nValue.Item.ItemType)
            {
                case ItemType.Weapon:
                case ItemType.Armour:
                case ItemType.Fashion:
                case ItemType.Helmet:
                case ItemType.Necklace:
                case ItemType.Bracelet:
                case ItemType.Ring:
                case ItemType.Shoes:
                case ItemType.Book:
                    ItemCell.Item.Flags |= UserItemFlags.NonRefinable;
                    break;
            }
            ItemNameLabel.Text = nValue.Item.Lang(p => p.ItemName);

            //以物换物 则显示不同的价格
            if (nValue.Currency != Globals.Currency)
            {
                CostLabel.Text = nValue.CurrencyCost.ToString("##,##0");
            }
            else
            {
                CostLabel.Text = nValue.Cost.ToString("##,##0");
            }
            if (GoldIcon.Visible)
            {
                CostLabel.Location = new Point(GoldIcon.Location.X - CostLabel.Size.Width, GoldIcon.Location.Y + GoldIcon.Size.Height - CostLabel.Size.Height);
            }
            else
            {
                CostLabel.Location = new Point(Size.Width - CostLabel.Size.Width - 20, Size.Height - CostLabel.Size.Height);
            }


            CostLabel.ForeColour = nValue.Cost > MapObject.User.Gold ? Color.Red : Color.White;

            switch (nValue.Item.ItemType)
            {
                case ItemType.Nothing:
                    RequirementLabel.Text = string.Empty;
                    break;
                case ItemType.Meat:
                    RequirementLabel.Text = $"品质".Lang() + $": {nValue.Item.Durability / 1000}";
                    RequirementLabel.ForeColour = Color.Wheat;
                    break;
                case ItemType.Ore:
                    RequirementLabel.Text = $"纯度".Lang() + $": {nValue.Item.Durability / 1000}";
                    RequirementLabel.ForeColour = Color.Wheat;
                    break;
                case ItemType.Consumable:
                case ItemType.Scroll:
                case ItemType.Weapon:
                case ItemType.Armour:
                case ItemType.Fashion:
                case ItemType.Torch:
                case ItemType.Helmet:
                case ItemType.Necklace:
                case ItemType.Bracelet:
                case ItemType.Ring:
                case ItemType.Shoes:
                case ItemType.Medicament:
                case ItemType.Poison:
                case ItemType.Amulet:
                case ItemType.DarkStone:

                    if (GameScene.Game.CanUseItem(ItemCell.Item))
                    {
                        RequirementLabel.Text = "可用物品".Lang();
                        RequirementLabel.ForeColour = Color.Aquamarine;
                    }
                    else
                    {
                        RequirementLabel.Text = "不可用物品".Lang();
                        RequirementLabel.ForeColour = Color.Red;
                    }
                    break;
            }
            GoodChanged?.Invoke(this, EventArgs.Empty);
        }

        // 判断是否是回购的物品
        public bool IsBuybackCell => Good?.UserItem != null;
        #endregion

        #region Selected
        /// <summary>
        /// 选定的道具
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

            ItemCell.Border = Selected;
            ItemCell.FixedBorder = Selected; //FixedBorder 代替了原本的 Border
            ItemCell.BorderColour = Selected ? Color.FromArgb(198, 166, 99) : Color.FromArgb(99, 83, 50); //边框颜色

            ItemCell.UpdateBorder();

            ItemDetial.Border = Selected;
            ItemDetial.BorderColour = Color.LightSkyBlue;
            ItemDetial.BackColour = Selected ? Color.LightSkyBlue : Color.Empty;

            ItemDetial.Opacity = 0.4f;

            SelectedChanged?.Invoke(this, EventArgs.Empty);  //所选的内容更改 
        }

        #endregion

        public DXItemCell ItemCell;
        public DXControl ItemDetial;
        public DXImageControl GoldIcon;
        public DXLabel ItemNameLabel, RequirementLabel, CostLabel;
        public int imageNum = 121;

        #endregion

        //以物换物 则显示不同的图标, 默认图标为 119 一个金字
        /// <summary>
        /// NPC商品单元格
        /// </summary>
        /// <param name="imageNum"></param>
        public NPCGoodsCell(int imageNum = 121)
        {
            DrawTexture = true;
            //BackColour = Color.FromArgb(25, 20, 0);
            //Border = true;
            //ForeColour = Color.White;
            //BorderColour = Color.FromArgb(198, 166, 99);
            Size = new Size(235, 39);

            ItemCell = new DXItemCell   //道具单元
            {
                Parent = this,
                Location = new Point((Size.Height - DXItemCell.CellHeight) / 2, 0), //(Size.Height - DXItemCell.CellHeight) / 2),
                FixedBorder = false,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                //FixedBorderColour = true,
                ShowCountLabel = false,
                BorderColour = Color.FromArgb(198, 166, 99),
                Border = false,
            };


            ItemDetial = new DXControl
            {
                Parent = this,
                Size = new Size(Size.Width - 39 - 5, 37),
                Location = new Point(DXItemCell.CellWidth + 4, 1),
                Border = false,
            };

            ItemNameLabel = new DXLabel
            {
                Parent = ItemDetial,
                Location = new Point(5, 5),
                ForeColour = Color.Gold,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };

            RequirementLabel = new DXLabel   //需求标签
            {
                Parent = ItemDetial,
                Text = "需求".Lang(),
                IsControl = false,
                Location = new Point(5, 20),
            };
            //RequirementLabel.Location = new Point(ItemCell.Location.X * 2 + ItemCell.Size.Width, ItemCell.Location.Y + ItemCell.Size.Height - RequirementLabel.Size.Height);


            GoldIcon = new DXImageControl    //金币图标
            {
                LibraryFile = LibraryFile.StoreItems,
                Index = imageNum,
                Parent = ItemDetial,
                IsControl = false,
            };
            GoldIcon.Location = new Point(ItemDetial.Size.Width - GoldIcon.Size.Width, ItemDetial.Size.Height - GoldIcon.Size.Height + 2);

            CostLabel = new DXLabel    //金额标签
            {
                Parent = ItemDetial,
                IsControl = false,
            };
        }

        #region Methods
        /// <summary>
        /// 更新文字颜色
        /// </summary>
        public void UpdateColours()
        {
            CostLabel.ForeColour = Good.Cost > MapObject.User.Gold ? Color.Red : Color.White;

            switch (Good.Item.ItemType)
            {
                case ItemType.Consumable:
                case ItemType.Scroll:
                case ItemType.Weapon:
                case ItemType.Armour:
                case ItemType.Fashion:
                case ItemType.Torch:
                case ItemType.Helmet:
                case ItemType.Necklace:
                case ItemType.Bracelet:
                case ItemType.Ring:
                case ItemType.Shoes:
                case ItemType.Medicament:
                case ItemType.Poison:
                case ItemType.Amulet:
                case ItemType.DarkStone:
                    RequirementLabel.ForeColour = GameScene.Game.CanUseItem(ItemCell.Item) ? Color.Aquamarine : Color.Red;
                    break;
            }
        }
        /// <summary>
        /// 鼠标进入时
        /// </summary>
        public override void OnMouseEnter()
        {
            base.OnMouseEnter();

            GameScene.Game.MouseItem = ItemCell.Item;
        }
        /// <summary>
        /// 鼠标离开时
        /// </summary>
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            GameScene.Game.MouseItem = null;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Good = null;
                GoodChanged = null;

                _Selected = false;
                SelectedChanged = null;

                if (ItemCell != null)
                {
                    if (!ItemCell.IsDisposed)
                        ItemCell.Dispose();

                    ItemCell = null;
                }

                if (GoldIcon != null)
                {
                    if (!GoldIcon.IsDisposed)
                        GoldIcon.Dispose();

                    GoldIcon = null;
                }

                if (ItemNameLabel != null)
                {
                    if (!ItemNameLabel.IsDisposed)
                        ItemNameLabel.Dispose();

                    ItemNameLabel = null;
                }

                if (RequirementLabel != null)
                {
                    if (!RequirementLabel.IsDisposed)
                        RequirementLabel.Dispose();

                    RequirementLabel = null;
                }

                if (CostLabel != null)
                {
                    if (!CostLabel.IsDisposed)
                        CostLabel.Dispose();

                    CostLabel = null;
                }
            }
        }
        #endregion
    }

    #endregion

    #region NPC出售

    /// <summary>
    /// NPC出售功能
    /// </summary>
    public sealed class NPCSellDialog : DXWindow
    {
        #region Properties

        public DXItemGrid Grid;
        public DXButton SellButton;
        public DXLabel GoldLabel;
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (IsVisible)
            {
                if (GameScene.Game.InventoryBox == null) return;
                GameScene.Game.InventoryBox.Visible = true;
                GameScene.Game.InventoryBox.InventoryType = InventoryType.Sell;
                GameScene.Game.InventoryBox.BringToFront();
            }
            else
                Grid.ClearLinks();
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// NPC出售界面
        /// </summary>
        public NPCSellDialog()
        {
            //TitleLabel.Text = "出售";
            HasTitle = false;   //标题不显示
            CloseButton.Visible = false; //不显示关闭按钮

            Grid = new DXItemGrid   //道具格子
            {
                GridSize = new Size(7, 7),
                Parent = this,
                GridType = GridType.Sell,
                BackColour = Color.FromArgb(36, 13, 5),
                BorderColour = Color.FromArgb(90, 60, 50),
                Linked = true
            };

            Movable = false;  //可移动关闭
            SetClientSize(new Size(Grid.Size.Width, Grid.Size.Height));
            Grid.Location = ClientArea.Location;

            foreach (DXItemCell cell in Grid.Grid)
            {
                cell.LinkChanged += Cell_LinkChanged;
            }

            GoldLabel = new DXLabel   //出售金额
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left + 80, ClientArea.Bottom - 45),
                Text = "0",
                Size = new Size(ClientArea.Width - 80, 20),
                Sound = SoundIndex.GoldPickUp
            };

            new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left, ClientArea.Bottom - 45),
                Text = "总价".Lang(),
                Size = new Size(79, 20),
                IsControl = false,
            };

            DXButton selectAll = new DXButton
            {
                Label = { Text = "全选".Lang() },
                Location = new Point(ClientArea.X, GoldLabel.Location.Y + GoldLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight)
            };
            selectAll.MouseClick += (o, e) =>
            {
                foreach (DXItemCell cell in GameScene.Game.InventoryBox.Grid.Grid)
                {
                    if (!cell.CheckLink(Grid)) continue;

                    cell.MoveItem(Grid, true);
                }
            };

            SellButton = new DXButton   //出售按钮
            {
                Label = { Text = "出售".Lang() },
                Location = new Point(ClientArea.Right - 80, GoldLabel.Location.Y + GoldLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,  //按钮类型 小按钮
                Parent = this,
                Size = new Size(79, SmallButtonHeight),
                Enabled = false,
            };
            SellButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                List<CellLinkInfo> links = new List<CellLinkInfo>();

                foreach (DXItemCell cell in Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    links.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }

                CEnvir.Enqueue(new C.NPCSell { Links = links });
            };
        }

        #region Methods
        /// <summary>
        /// 单元连接改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cell_LinkChanged(object sender, EventArgs e)
        {
            long sum = 0;
            int count = 0;
            foreach (DXItemCell cell in Grid.Grid)
            {
                if (cell.Link?.Item == null) continue;

                count++;
                sum += cell.Link.Item.Price(cell.LinkedCount);
            }

            GoldLabel.Text = sum.ToString("#,##0");

            SellButton.Enabled = count > 0;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                if (SellButton != null)
                {
                    if (!SellButton.IsDisposed)
                        SellButton.Dispose();

                    SellButton = null;
                }

                if (GoldLabel != null)
                {
                    if (!GoldLabel.IsDisposed)
                        GoldLabel.Dispose();

                    GoldLabel = null;
                }
            }
        }
        #endregion
    }

    #endregion

    #region NPC一键出售

    /// <summary>
    /// NPC一键出售功能
    /// </summary>
    public sealed class NPCRootSellDialog : DXWindow
    {
        #region Properties

        public DXItemGrid Grid;
        public DXButton SellButton;
        public DXLabel GoldLabel;
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (IsVisible)
            {
                if (GameScene.Game.InventoryBox == null) return;
                GameScene.Game.InventoryBox.Visible = true;
                GameScene.Game.InventoryBox.InventoryType = InventoryType.RootSell;
                GameScene.Game.InventoryBox.BringToFront();
            }
            else
                Grid.ClearLinks();
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// NPC一键出售界面
        /// </summary>
        public NPCRootSellDialog()
        {
            TitleLabel.Text = "出售".Lang();

            Grid = new DXItemGrid
            {
                GridSize = new Size(7, 7),
                Parent = this,
                GridType = GridType.RootSell,
                Linked = true
            };

            Movable = false;  //可移动关闭
            SetClientSize(new Size(Grid.Size.Width, Grid.Size.Height + 50));
            Grid.Location = ClientArea.Location;

            foreach (DXItemCell cell in Grid.Grid)
            {
                cell.LinkChanged += Cell_LinkChanged;
            }


            GoldLabel = new DXLabel   //出售金额
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left + 80, ClientArea.Bottom - 45),
                Text = "0",
                Size = new Size(ClientArea.Width - 80, 20),
                Sound = SoundIndex.GoldPickUp
            };

            new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left, ClientArea.Bottom - 45),
                Text = "总价".Lang(),
                Size = new Size(79, 20),
                IsControl = false,
            };

            DXButton selectAll = new DXButton
            {
                Label = { Text = "全选".Lang() },
                Location = new Point(ClientArea.X, GoldLabel.Location.Y + GoldLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight)
            };
            selectAll.MouseClick += (o, e) =>
            {
                foreach (DXItemCell cell in GameScene.Game.InventoryBox.Grid.Grid)
                {
                    if (!cell.CheckLink(Grid)) continue;

                    cell.MoveItem(Grid, true);
                }
            };

            SellButton = new DXButton   //出售按钮
            {
                Label = { Text = "出售".Lang() },
                Location = new Point(ClientArea.Right - 80, GoldLabel.Location.Y + GoldLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,  //按钮类型 小按钮
                Parent = this,
                Size = new Size(79, SmallButtonHeight),
                Enabled = false,
            };
            SellButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                List<CellLinkInfo> links = new List<CellLinkInfo>();

                foreach (DXItemCell cell in Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    links.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }

                CEnvir.Enqueue(new C.NPCRootSell { Links = links });
            };
        }

        #region Methods
        /// <summary>
        /// 单元连接改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cell_LinkChanged(object sender, EventArgs e)
        {
            long sum = 0;
            int count = 0;
            foreach (DXItemCell cell in Grid.Grid)
            {
                if (cell.Link?.Item == null) continue;

                count++;
                sum += cell.Link.Item.Price(cell.LinkedCount);
            }

            GoldLabel.Text = sum.ToString("#,##0");

            SellButton.Enabled = count > 0;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                if (SellButton != null)
                {
                    if (!SellButton.IsDisposed)
                        SellButton.Dispose();

                    SellButton = null;
                }

                if (GoldLabel != null)
                {
                    if (!GoldLabel.IsDisposed)
                        GoldLabel.Dispose();

                    GoldLabel = null;
                }
            }
        }
        #endregion
    }

    #endregion

    #region NPC修理

    /// <summary>
    /// NPC修理功能
    /// </summary>
    public sealed class NPCRepairDialog : DXWindow
    {
        #region Properties
        public DXItemGrid Grid;

        public DXLabel GoldLabel;
        public DXButton RepairButton, GuildStorageButton;
        public DXCheckBox SpecialCheckBox;
        public DXCheckBox GuildCheckBox;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (IsVisible)
            {
                if (GameScene.Game.InventoryBox == null) return;
                GameScene.Game.InventoryBox.Visible = true;
                GameScene.Game.InventoryBox.InventoryType = InventoryType.Repair;
                GameScene.Game.InventoryBox.BringToFront();
            }
            else
                Grid.ClearLinks();
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// NPC修理界面
        /// </summary>
        public NPCRepairDialog()
        {
            //TitleLabel.Text = "修理";
            HasTitle = false;   //标题不显示
            CloseButton.Visible = false; //不显示关闭按钮
            Movable = false;

            Grid = new DXItemGrid   //道具格子
            {
                GridSize = new Size(7, 7),
                Parent = this,
                GridType = GridType.Repair,
                BackColour = Color.FromArgb(36, 13, 5),
                BorderColour = Color.FromArgb(90, 60, 50),
                Linked = true
            };

            SetClientSize(new Size(Grid.Size.Width, Grid.Size.Height + 70));
            Grid.Location = ClientArea.Location;

            foreach (DXItemCell cell in Grid.Grid)
                cell.LinkChanged += (o, e) => CalculateCost();

            GoldLabel = new DXLabel   //金额标签
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left + 80, ClientArea.Bottom - 65),
                Text = "0",
                Size = new Size(ClientArea.Width - 80, 20),
            };

            new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left, ClientArea.Bottom - 65),
                Text = "修理费用".Lang(),
                Size = new Size(79, 20),
                IsControl = false,
            };

            DXButton inventory = new DXButton
            {
                Label = { Text = "背包".Lang() },
                Location = new Point(ClientArea.X, GoldLabel.Location.Y + GoldLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight)
            };
            inventory.MouseClick += (o, e) =>
            {
                foreach (DXItemCell cell in GameScene.Game.InventoryBox.Grid.Grid)
                {
                    if (!cell.CheckLink(Grid)) continue;

                    cell.MoveItem(Grid, true);
                }
            };

            DXButton equipment = new DXButton
            {
                Label = { Text = "装备".Lang() },
                Location = new Point(ClientArea.X + 5 + inventory.Size.Width, GoldLabel.Location.Y + GoldLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight)
            };
            equipment.MouseClick += (o, e) =>
            {
                foreach (DXItemCell cell in GameScene.Game.CharacterBox.Grid)
                {
                    if (!cell.CheckLink(Grid)) continue;

                    cell.MoveItem(Grid, true);
                }
            };

            DXButton storage = new DXButton
            {
                Label = { Text = "仓库".Lang() },
                Location = new Point(ClientArea.X, GoldLabel.Location.Y + GoldLabel.Size.Height + inventory.Size.Height + 5 + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight),
            };
            storage.MouseClick += (o, e) =>
            {
                foreach (DXItemCell cell in GameScene.Game.StorageBox.Grid.Grid)
                {
                    if (!cell.CheckLink(Grid)) continue;

                    cell.MoveItem(Grid, true);
                }
            };

            GuildStorageButton = new DXButton
            {
                Label = { Text = "行会仓库".Lang() },
                Location = new Point(ClientArea.X + inventory.Size.Width + 5, GoldLabel.Location.Y + GoldLabel.Size.Height + inventory.Size.Height + 5 + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight),
                Enabled = false,
            };
            GuildStorageButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.GuildBox.GuildInfo == null) return;

                foreach (DXItemCell cell in GameScene.Game.GuildBox.StorageGrid.Grid)
                {
                    if (!cell.CheckLink(Grid)) continue;

                    cell.MoveItem(Grid, true);
                }
            };

            SpecialCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "特修".Lang() },
                Checked = Config.SpecialRepair,
            };
            SpecialCheckBox.Location = new Point(ClientArea.Right - 80 - SpecialCheckBox.Size.Width - 5, GoldLabel.Location.Y + GoldLabel.Size.Height + 7);
            SpecialCheckBox.CheckedChanged += (o, e) =>
            {
                Config.SpecialRepair = SpecialCheckBox.Checked;

                if (SpecialCheckBox.Checked)
                    foreach (DXItemCell cell in Grid.Grid)
                    {
                        if (cell.Item == null) continue;
                        if (CEnvir.Now > cell.Item.NextSpecialRepair) continue;


                        cell.Link = null;
                    }
                CalculateCost();
            };

            GuildCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "使用行会资金".Lang() },
                Enabled = false,
            };
            GuildCheckBox.Location = new Point(ClientArea.Right - 80 - GuildCheckBox.Size.Width - 5, GoldLabel.Location.Y + GoldLabel.Size.Height + SpecialCheckBox.Size.Height + 5 + 7);
            GuildCheckBox.CheckedChanged += (o, e) => CalculateCost();

            RepairButton = new DXButton
            {
                Label = { Text = "修理".Lang() },
                Location = new Point(ClientArea.Right - 80, GoldLabel.Location.Y + GoldLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight),
                Enabled = false,
            };
            RepairButton.MouseClick += (o, e) =>  //修理按钮鼠标单击
            {
                if (GameScene.Game.Observer) return;   //如果是观察者 返回

                List<CellLinkInfo> links = new List<CellLinkInfo>();  //新单元格链接信息

                foreach (DXItemCell cell in Grid.Grid)  //遍历数组 网格单元格内容
                {
                    if (cell.Link == null) continue;  //如果 单元格链接等空 继续

                    links.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });  //链接添加（新的单元格链接信息{单元格链接计数 网格类型 位置}）

                    cell.Link.Locked = true;
                    cell.Link = null;
                }

                CEnvir.Enqueue(new C.NPCRepair { Links = links, Special = SpecialCheckBox.Checked, GuildFunds = GuildCheckBox.Checked });

                GuildCheckBox.Checked = false;
            };
        }

        #region Methods
        /// <summary>
        /// 计算修理成本
        /// </summary>
        private void CalculateCost()
        {
            long sum = 0;
            int count = 0;
            foreach (DXItemCell cell in Grid.Grid)
            {
                if (cell.Link?.Item == null) continue;

                sum += cell.Link.Item.RepairCost(SpecialCheckBox.Checked);
                count++;
            }

            if (GuildCheckBox.Checked)
            {
                GoldLabel.ForeColour = sum > GameScene.Game.GuildBox.GuildInfo.GuildFunds ? Color.Red : Color.White;
            }
            else
            {
                GoldLabel.ForeColour = sum > MapObject.User.Gold ? Color.Red : Color.White;
            }

            GoldLabel.Text = sum.ToString("#,##0");

            RepairButton.Enabled = sum <= MapObject.User.Gold && count > 0;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                if (GoldLabel != null)
                {
                    if (!GoldLabel.IsDisposed)
                        GoldLabel.Dispose();

                    GoldLabel = null;
                }

                if (RepairButton != null)
                {
                    if (!RepairButton.IsDisposed)
                        RepairButton.Dispose();

                    RepairButton = null;
                }

                if (GuildStorageButton != null)
                {
                    if (!GuildStorageButton.IsDisposed)
                        GuildStorageButton.Dispose();

                    GuildStorageButton = null;
                }

                if (SpecialCheckBox != null)
                {
                    if (!SpecialCheckBox.IsDisposed)
                        SpecialCheckBox.Dispose();

                    SpecialCheckBox = null;
                }

                if (GuildCheckBox != null)
                {
                    if (!GuildCheckBox.IsDisposed)
                        GuildCheckBox.Dispose();

                    GuildCheckBox = null;
                }
            }
        }
        #endregion
    }

    #endregion

    #region NPC特修

    /// <summary>
    /// NPC特修功能
    /// </summary>
    public sealed class NPCSpecialRepairDialog : DXWindow
    {
        #region Properties
        public DXItemGrid Grid;

        public DXLabel GoldLabel;
        public DXButton RepairButton, GuildStorageButton;
        public DXCheckBox SpecialCheckBox;
        public DXCheckBox GuildCheckBox;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (IsVisible)
            {
                if (GameScene.Game.InventoryBox == null) return;
                GameScene.Game.InventoryBox.Visible = true;
                GameScene.Game.InventoryBox.InventoryType = InventoryType.SpecialRepair;
                GameScene.Game.InventoryBox.BringToFront();
            }
            else
                Grid.ClearLinks();
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// NPC特修界面
        /// </summary>
        public NPCSpecialRepairDialog()
        {
            TitleLabel.Text = "特修".Lang();
            Movable = false;

            Grid = new DXItemGrid   //道具格子
            {
                GridSize = new Size(7, 7),
                Parent = this,
                GridType = GridType.Repair,
                Linked = true
            };

            SetClientSize(new Size(Grid.Size.Width, Grid.Size.Height + 70));
            Grid.Location = ClientArea.Location;

            foreach (DXItemCell cell in Grid.Grid)
                cell.LinkChanged += (o, e) => CalculateCost();

            GoldLabel = new DXLabel  //金额标签
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left + 80, ClientArea.Bottom - 65),
                Text = "0",
                Size = new Size(ClientArea.Width - 80, 20),
            };

            new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left, ClientArea.Bottom - 65),
                Text = "修理费用".Lang(),
                Size = new Size(79, 20),
                IsControl = false,
            };

            DXButton inventory = new DXButton
            {
                Label = { Text = "背包".Lang() },
                Location = new Point(ClientArea.X, GoldLabel.Location.Y + GoldLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight)
            };
            inventory.MouseClick += (o, e) =>
            {
                foreach (DXItemCell cell in GameScene.Game.InventoryBox.Grid.Grid)
                {
                    if (!cell.CheckLink(Grid)) continue;

                    cell.MoveItem(Grid, true);
                }
            };

            DXButton equipment = new DXButton
            {
                Label = { Text = "装备".Lang() },
                Location = new Point(ClientArea.X + 5 + inventory.Size.Width, GoldLabel.Location.Y + GoldLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight)
            };
            equipment.MouseClick += (o, e) =>
            {
                foreach (DXItemCell cell in GameScene.Game.CharacterBox.Grid)
                {
                    if (!cell.CheckLink(Grid)) continue;

                    cell.MoveItem(Grid, true);
                }
            };

            DXButton storage = new DXButton
            {
                Label = { Text = "仓库".Lang() },
                Location = new Point(ClientArea.X, GoldLabel.Location.Y + GoldLabel.Size.Height + inventory.Size.Height + 5 + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight),
            };
            storage.MouseClick += (o, e) =>
            {
                foreach (DXItemCell cell in GameScene.Game.StorageBox.Grid.Grid)
                {
                    if (!cell.CheckLink(Grid)) continue;

                    cell.MoveItem(Grid, true);
                }
            };

            GuildStorageButton = new DXButton
            {
                Label = { Text = "行会仓库".Lang() },
                Location = new Point(ClientArea.X + inventory.Size.Width + 5, GoldLabel.Location.Y + GoldLabel.Size.Height + inventory.Size.Height + 5 + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight),
                Enabled = false,
            };
            GuildStorageButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.GuildBox.GuildInfo == null) return;

                foreach (DXItemCell cell in GameScene.Game.GuildBox.StorageGrid.Grid)
                {
                    if (!cell.CheckLink(Grid)) continue;

                    cell.MoveItem(Grid, true);
                }
            };

            SpecialCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "特修".Lang() },
                Checked = Config.SpecialRepair,
                Visible = false,
            };
            SpecialCheckBox.Location = new Point(ClientArea.Right - 80 - SpecialCheckBox.Size.Width - 5, GoldLabel.Location.Y + GoldLabel.Size.Height + 7);
            SpecialCheckBox.CheckedChanged += (o, e) =>
            {
                Config.SpecialRepair = SpecialCheckBox.Checked;

                if (SpecialCheckBox.Checked)
                    foreach (DXItemCell cell in Grid.Grid)
                    {
                        if (cell.Item == null) continue;
                        if (CEnvir.Now > cell.Item.NextSpecialRepair) continue;
                        cell.Link = null;
                    }

                CalculateCost();
            };

            GuildCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "使用行会资金".Lang() },
                Enabled = false,
            };
            GuildCheckBox.Location = new Point(ClientArea.Right - GuildCheckBox.Size.Width, GoldLabel.Location.Y + GoldLabel.Size.Height + SpecialCheckBox.Size.Height + 17);
            GuildCheckBox.CheckedChanged += (o, e) => CalculateCost();

            RepairButton = new DXButton
            {
                Label = { Text = "特修".Lang() },
                Location = new Point(ClientArea.Right - 80, GoldLabel.Location.Y + GoldLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight),
                Enabled = false,
            };
            RepairButton.MouseClick += (o, e) =>  //修理按钮鼠标单击
            {
                if (GameScene.Game.Observer) return;   //如果是观察者 返回

                List<CellLinkInfo> links = new List<CellLinkInfo>();  //新单元格链接信息

                foreach (DXItemCell cell in Grid.Grid)  //遍历数组 网格单元格内容
                {
                    if (cell.Link == null) continue;  //如果 单元格链接等空 继续

                    links.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });  //链接添加（新的单元格链接信息{单元格链接计数 网格类型 位置}）

                    cell.Link.Locked = true;
                    cell.Link = null;
                }

                CEnvir.Enqueue(new C.NPCSpecialRepair { Links = links, Special = SpecialCheckBox.Checked, GuildFunds = GuildCheckBox.Checked });

                GuildCheckBox.Checked = false;
            };
        }

        #region Methods
        /// <summary>
        /// 计算特修成本
        /// </summary>
        private void CalculateCost()
        {
            long sum = 0;
            int count = 0;
            foreach (DXItemCell cell in Grid.Grid)
            {
                if (cell.Link?.Item == null) continue;

                sum += cell.Link.Item.RepairCost(SpecialCheckBox.Checked);
                count++;
            }

            if (GuildCheckBox.Checked)
            {
                GoldLabel.ForeColour = sum > GameScene.Game.GuildBox.GuildInfo.GuildFunds ? Color.Red : Color.White;
            }
            else
            {
                GoldLabel.ForeColour = sum > MapObject.User.Gold ? Color.Red : Color.White;
            }

            GoldLabel.Text = sum.ToString("#,##0");

            RepairButton.Enabled = sum <= MapObject.User.Gold && count > 0;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                if (GoldLabel != null)
                {
                    if (!GoldLabel.IsDisposed)
                        GoldLabel.Dispose();

                    GoldLabel = null;
                }

                if (RepairButton != null)
                {
                    if (!RepairButton.IsDisposed)
                        RepairButton.Dispose();

                    RepairButton = null;
                }

                if (GuildStorageButton != null)
                {
                    if (!GuildStorageButton.IsDisposed)
                        GuildStorageButton.Dispose();

                    GuildStorageButton = null;
                }

                if (SpecialCheckBox != null)
                {
                    if (!SpecialCheckBox.IsDisposed)
                        SpecialCheckBox.Dispose();

                    SpecialCheckBox = null;
                }

                if (GuildCheckBox != null)
                {
                    if (!GuildCheckBox.IsDisposed)
                        GuildCheckBox.Dispose();

                    GuildCheckBox = null;
                }
            }
        }
        #endregion
    }

    #endregion

    #region NPC精炼

    /// <summary>
    /// NPC精炼功能
    /// </summary>
    public sealed class NPCRefineDialog : DXWindow
    {
        #region Properties

        #region RefineType
        /// <summary>
        /// 精炼类型
        /// </summary>
        public RefineType RefineType
        {
            get => _RefineType;
            set
            {
                if (_RefineType == value) return;

                RefineType oldValue = _RefineType;
                _RefineType = value;

                OnRefineTypeChanged(oldValue, value);
            }
        }
        private RefineType _RefineType;
        public event EventHandler<EventArgs> RefineTypeChanged;
        public void OnRefineTypeChanged(RefineType oValue, RefineType nValue)
        {
            switch (oValue)
            {
                case RefineType.None:
                    SubmitButton.Enabled = true;
                    break;
                case RefineType.Durability:
                    DurabilityCheckBox.Checked = false;
                    break;
                case RefineType.DC:
                    DCCheckBox.Checked = false;
                    break;
                case RefineType.SpellPower:
                    SPCheckBox.Checked = false;
                    break;
                case RefineType.Fire:
                    FireCheckBox.Checked = false;
                    break;
                case RefineType.Ice:
                    IceCheckBox.Checked = false;
                    break;
                case RefineType.Lightning:
                    LightningCheckBox.Checked = false;
                    break;
                case RefineType.Wind:
                    WindCheckBox.Checked = false;
                    break;
                case RefineType.Holy:
                    HolyCheckBox.Checked = false;
                    break;
                case RefineType.Dark:
                    DarkCheckBox.Checked = false;
                    break;
                case RefineType.Phantom:
                    PhantomCheckBox.Checked = false;
                    break;
            }

            switch (nValue)
            {
                case RefineType.None:
                    SubmitButton.Enabled = false;
                    break;
                case RefineType.Durability:
                    DurabilityCheckBox.Checked = true;
                    break;
                case RefineType.DC:
                    DCCheckBox.Checked = true;
                    break;
                case RefineType.SpellPower:
                    SPCheckBox.Checked = true;
                    break;
                case RefineType.Fire:
                    FireCheckBox.Checked = true;
                    break;
                case RefineType.Ice:
                    IceCheckBox.Checked = true;
                    break;
                case RefineType.Lightning:
                    LightningCheckBox.Checked = true;
                    break;
                case RefineType.Wind:
                    WindCheckBox.Checked = true;
                    break;
                case RefineType.Holy:
                    HolyCheckBox.Checked = true;
                    break;
                case RefineType.Dark:
                    DarkCheckBox.Checked = true;
                    break;
                case RefineType.Phantom:
                    PhantomCheckBox.Checked = true;
                    break;
            }
            RefineTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region RefineQuality
        /// <summary>
        /// 精炼质量
        /// </summary>
        public RefineQuality RefineQuality
        {
            get => _RefineQuality;
            set
            {
                if (_RefineQuality == value) return;

                RefineQuality oldValue = _RefineQuality;
                _RefineQuality = value;

                OnRefineQualityChanged(oldValue, value);
            }
        }
        private RefineQuality _RefineQuality;
        public event EventHandler<EventArgs> RefineQualityChanged;
        public void OnRefineQualityChanged(RefineQuality oValue, RefineQuality nValue)
        {
            switch (nValue)
            {
                case RefineQuality.Rush:
                    DurationLabel.Text = "";
                    break;
                default:
                    DurationLabel.Text = Globals.RefineTimes[nValue].Lang(false);
                    break;
            }
            RefineQualityChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXItemGrid BlackIronGrid, AccessoryGrid, SpecialGrid;

        public DXCheckBox DurabilityCheckBox, DCCheckBox, SPCheckBox, FireCheckBox, IceCheckBox, LightningCheckBox, WindCheckBox, HolyCheckBox, DarkCheckBox, PhantomCheckBox;
        public DXButton SubmitButton;

        public DXComboBox RefineQualityBox;
        public DXLabel DurationLabel;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
                GameScene.Game.InventoryBox.Visible = true;

            if (!IsVisible)
            {
                BlackIronGrid.ClearLinks();
                AccessoryGrid.ClearLinks();
                SpecialGrid.ClearLinks();
            }
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// NPC精炼界面
        /// </summary>
        public NPCRefineDialog()
        {
            TitleLabel.Text = "精炼".Lang();

            SetClientSize(new Size(360, 280));

            DXLabel label = new DXLabel
            {
                Text = "黑铁矿".Lang(),
                Location = ClientArea.Location,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };

            BlackIronGrid = new DXItemGrid   //放黑铁格子
            {
                GridSize = new Size(5, 1),
                Parent = this,
                GridType = GridType.RefineBlackIronOre,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };

            label = new DXLabel
            {
                Text = "特殊".Lang(),
                Location = new Point(BlackIronGrid.Location.X + BlackIronGrid.Size.Width + DXItemCell.CellWidth - 7, label.Location.Y),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };

            SpecialGrid = new DXItemGrid   //放其它道具格子
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.RefineSpecial,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };

            label = new DXLabel
            {
                Text = "首饰".Lang(),
                Location = new Point(ClientArea.Location.X, BlackIronGrid.Location.Y + BlackIronGrid.Size.Height + 10),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };

            AccessoryGrid = new DXItemGrid   //放首饰格子
            {
                GridSize = new Size(5, 1),
                Parent = this,
                GridType = GridType.RefineAccessory,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };

            //SetClientSize(new Size(380, SpecialGrid.Location.Y + SpecialGrid.Size.Height - ClientArea.Y + 2));

            DCCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "破坏".Lang() },
                ReadOnly = true,
            };
            DCCheckBox.MouseClick += (o, e) => RefineType = RefineType.DC;

            SPCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "全系列魔法".Lang() },
                ReadOnly = true,
            };
            SPCheckBox.MouseClick += (o, e) => RefineType = RefineType.SpellPower;

            FireCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "火".Lang() },
                ReadOnly = true,
            };
            FireCheckBox.MouseClick += (o, e) => RefineType = RefineType.Fire;

            IceCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "冰".Lang() },
                ReadOnly = true,
            };
            IceCheckBox.MouseClick += (o, e) => RefineType = RefineType.Ice;

            LightningCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "雷".Lang() },
                ReadOnly = true,
            };
            LightningCheckBox.MouseClick += (o, e) => RefineType = RefineType.Lightning;

            WindCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "风".Lang() },
                ReadOnly = true,
            };
            WindCheckBox.MouseClick += (o, e) => RefineType = RefineType.Wind;

            HolyCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "神圣".Lang() },
                ReadOnly = true,
            };
            HolyCheckBox.MouseClick += (o, e) => RefineType = RefineType.Holy;

            DarkCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "暗黑".Lang() },
                ReadOnly = true,
            };
            DarkCheckBox.MouseClick += (o, e) => RefineType = RefineType.Dark;

            PhantomCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "幻影".Lang() },
                ReadOnly = true,
            };
            PhantomCheckBox.MouseClick += (o, e) => RefineType = RefineType.Phantom;


            DCCheckBox.Location = new Point(ClientArea.Right - DCCheckBox.Size.Width - 300, ClientArea.Y + 180);
            SPCheckBox.Location = new Point(ClientArea.Right - SPCheckBox.Size.Width - 216, ClientArea.Y + 180);

            FireCheckBox.Location = new Point(ClientArea.Right - FireCheckBox.Size.Width - 300, ClientArea.Y + 203);
            IceCheckBox.Location = new Point(ClientArea.Right - IceCheckBox.Size.Width - 216, ClientArea.Y + 203);
            LightningCheckBox.Location = new Point(ClientArea.Right - LightningCheckBox.Size.Width - 141, ClientArea.Y + 203);
            WindCheckBox.Location = new Point(ClientArea.Right - WindCheckBox.Size.Width - 65, ClientArea.Y + 203);
            HolyCheckBox.Location = new Point(ClientArea.Right - HolyCheckBox.Size.Width - 300, ClientArea.Y + 220);
            DarkCheckBox.Location = new Point(ClientArea.Right - DarkCheckBox.Size.Width - 216, ClientArea.Y + 220);
            PhantomCheckBox.Location = new Point(ClientArea.Right - PhantomCheckBox.Size.Width - 300, ClientArea.Y + 237);

            SubmitButton = new DXButton          //确认按钮
            {
                Label = { Text = "确认".Lang() },
                Size = new Size(80, SmallButtonHeight),
                Parent = this,
                ButtonType = ButtonType.SmallButton,
                Enabled = false,
            };
            SubmitButton.Location = new Point(ClientArea.Right - SubmitButton.Size.Width - 10, ClientArea.Bottom - SubmitButton.Size.Height);
            SubmitButton.MouseClick += (o, e) =>   //鼠标点击
            {
                if (GameScene.Game.Observer) return;

                List<CellLinkInfo> ores = new List<CellLinkInfo>();
                List<CellLinkInfo> items = new List<CellLinkInfo>();
                List<CellLinkInfo> specials = new List<CellLinkInfo>();

                foreach (DXItemCell cell in BlackIronGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    ores.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }
                foreach (DXItemCell cell in AccessoryGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    items.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }
                foreach (DXItemCell cell in SpecialGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    specials.Add(new CellLinkInfo { Count = 1, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }

                CEnvir.Enqueue(new C.NPCRefine { RefineType = RefineType, RefineQuality = RefineQuality, Ores = ores, Items = items, Specials = specials });
            };

            RefineQualityBox = new DXComboBox   //精炼质量选择框
            {
                Parent = this,
                Size = new Size(80, DXComboBox.DefaultNormalHeight),
            };
            RefineQualityBox.SelectedItemChanged += (o, e) => RefineQuality = (RefineQuality?)RefineQualityBox.SelectedItem ?? RefineQuality.Standard;
            RefineQualityBox.Location = new Point(ClientArea.X + 40, AccessoryGrid.Location.Y + AccessoryGrid.Size.Height + 25);

            //foreach (KeyValuePair<RefineQuality, TimeSpan> pair in Globals.RefineTimes)

            new DXListBoxItem
            {
                Parent = RefineQualityBox.ListBox,
                Label = { Text = $"马上".Lang() },
                Item = RefineQuality.Rush
            };
            new DXListBoxItem
            {
                Parent = RefineQualityBox.ListBox,
                Label = { Text = $"快速".Lang() },
                Item = RefineQuality.Quick
            };
            new DXListBoxItem
            {
                Parent = RefineQualityBox.ListBox,
                Label = { Text = $"标准".Lang() },
                Item = RefineQuality.Standard
            };
            new DXListBoxItem
            {
                Parent = RefineQualityBox.ListBox,
                Label = { Text = $"谨慎".Lang() },
                Item = RefineQuality.Careful
            };
            new DXListBoxItem
            {
                Parent = RefineQualityBox.ListBox,
                Label = { Text = $"精确".Lang() },
                Item = RefineQuality.Precise
            };

            label = new DXLabel
            {
                Parent = this,
                Text = "品质".Lang(),
            };
            label.Location = new Point(RefineQualityBox.Location.X - label.Size.Width - 5, RefineQualityBox.Location.Y + (RefineQualityBox.Size.Height - label.Size.Height) / 2);

            DurationLabel = new DXLabel   //持续时间标签
            {
                Parent = this,
                Location = new Point(RefineQualityBox.Location.X + RefineQualityBox.Size.Width + 5, RefineQualityBox.Location.Y + (RefineQualityBox.Size.Height - label.Size.Height) / 2)
            };

            RefineQualityBox.ListBox.SelectItem(RefineQuality.Standard);
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _RefineType = 0;
                RefineTypeChanged = null;

                _RefineQuality = 0;
                RefineQualityChanged = null;

                if (BlackIronGrid != null)
                {
                    if (!BlackIronGrid.IsDisposed)
                        BlackIronGrid.Dispose();

                    BlackIronGrid = null;
                }

                if (AccessoryGrid != null)
                {
                    if (!AccessoryGrid.IsDisposed)
                        AccessoryGrid.Dispose();

                    AccessoryGrid = null;
                }

                if (SpecialGrid != null)
                {
                    if (!SpecialGrid.IsDisposed)
                        SpecialGrid.Dispose();

                    SpecialGrid = null;
                }

                if (DurabilityCheckBox != null)
                {
                    if (!DurabilityCheckBox.IsDisposed)
                        DurabilityCheckBox.Dispose();

                    DurabilityCheckBox = null;
                }

                if (DCCheckBox != null)
                {
                    if (!DCCheckBox.IsDisposed)
                        DCCheckBox.Dispose();

                    DCCheckBox = null;
                }

                if (SPCheckBox != null)
                {
                    if (!SPCheckBox.IsDisposed)
                        SPCheckBox.Dispose();

                    SPCheckBox = null;
                }

                if (FireCheckBox != null)
                {
                    if (!FireCheckBox.IsDisposed)
                        FireCheckBox.Dispose();

                    FireCheckBox = null;
                }

                if (IceCheckBox != null)
                {
                    if (!IceCheckBox.IsDisposed)
                        IceCheckBox.Dispose();

                    IceCheckBox = null;
                }

                if (LightningCheckBox != null)
                {
                    if (!LightningCheckBox.IsDisposed)
                        LightningCheckBox.Dispose();

                    LightningCheckBox = null;
                }

                if (WindCheckBox != null)
                {
                    if (!WindCheckBox.IsDisposed)
                        WindCheckBox.Dispose();

                    WindCheckBox = null;
                }

                if (HolyCheckBox != null)
                {
                    if (!HolyCheckBox.IsDisposed)
                        HolyCheckBox.Dispose();

                    HolyCheckBox = null;
                }

                if (DarkCheckBox != null)
                {
                    if (!DarkCheckBox.IsDisposed)
                        DarkCheckBox.Dispose();

                    DarkCheckBox = null;
                }

                if (PhantomCheckBox != null)
                {
                    if (!PhantomCheckBox.IsDisposed)
                        PhantomCheckBox.Dispose();

                    PhantomCheckBox = null;
                }

                if (SubmitButton != null)
                {
                    if (!SubmitButton.IsDisposed)
                        SubmitButton.Dispose();

                    SubmitButton = null;
                }

                if (RefineQualityBox != null)
                {
                    if (!RefineQualityBox.IsDisposed)
                        RefineQualityBox.Dispose();

                    RefineQualityBox = null;
                }

                if (DurationLabel != null)
                {
                    if (!DurationLabel.IsDisposed)
                        DurationLabel.Dispose();

                    DurationLabel = null;
                }
            }
        }
        #endregion
    }

    #endregion

    #region 精炼取回

    /// <summary>
    /// 精炼取回对话框
    /// </summary>
    public sealed class NPCRefineRetrieveDialog : DXWindow
    {
        #region Properties

        #region SelectedCell
        /// <summary>
        /// 选定单元格
        /// </summary>
        public NPCRefineCell SelectedCell
        {
            get => _SelectedCell;
            set
            {
                if (_SelectedCell == value) return;

                NPCRefineCell oldValue = _SelectedCell;
                _SelectedCell = value;

                OnSelectedCellChanged(oldValue, value);
            }
        }
        private NPCRefineCell _SelectedCell;
        public event EventHandler<EventArgs> SelectedCellChanged;
        public void OnSelectedCellChanged(NPCRefineCell oValue, NPCRefineCell nValue)
        {
            if (oValue != null) oValue.Selected = false;
            if (nValue != null) nValue.Selected = true;

            RetrieveButton.Enabled = nValue != null;

            SelectedCellChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public List<ClientRefineInfo> Refines = new List<ClientRefineInfo>();
        private DXVScrollBar ScrollBar;

        public List<NPCRefineCell> Cells = new List<NPCRefineCell>();
        private DXButton RetrieveButton;
        public DXControl ClientPanel;

        public override void OnClientAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            base.OnClientAreaChanged(oValue, nValue);

            if (ClientPanel == null) return;

            ClientPanel.Size = ClientArea.Size;
            ClientPanel.Location = ClientArea.Location;
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// 精炼取回对话框
        /// </summary>
        public NPCRefineRetrieveDialog()
        {
            TitleLabel.Text = "精炼".Lang();

            HasFooter = true;
            Movable = false;

            SetClientSize(new Size(491, 302));

            ClientPanel = new DXControl
            {
                Parent = this,
                Size = ClientArea.Size,
                Location = ClientArea.Location,
                PassThrough = true,
            };

            ScrollBar = new DXVScrollBar   //滚动条
            {
                Parent = this,
                Size = new Size(14, ClientArea.Height - 1),
            };
            ScrollBar.Location = new Point(ClientArea.Right - ScrollBar.Size.Width - 2, ClientArea.Y + 1);
            ScrollBar.ValueChanged += (o, e) => UpdateLocations();

            MouseWheel += ScrollBar.DoMouseWheel;

            RetrieveButton = new DXButton   //取回按钮
            {
                Location = new Point((Size.Width - 80) / 2, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = "取回".Lang() },
                Enabled = false,
            };
            RetrieveButton.MouseClick += (o, e) => Retrieve();
        }

        #region Methods
        /// <summary>
        /// 刷新列表
        /// </summary>
        public void RefreshList()
        {
            foreach (NPCRefineCell cell in Cells)
                cell.Dispose();

            Cells.Clear();

            foreach (ClientRefineInfo refine in Refines)
            {
                NPCRefineCell cell;
                Cells.Add(cell = new NPCRefineCell
                {
                    Parent = ClientPanel,
                    Refine = refine
                });
                cell.MouseClick += (o, e) => SelectedCell = cell;
                cell.MouseWheel += ScrollBar.DoMouseWheel;
                cell.MouseDoubleClick += (o, e) => Retrieve();
            }

            ScrollBar.MaxValue = Refines.Count * 43 - 2;
            SetClientSize(new Size(ClientArea.Width, Math.Min(Math.Max(3 * 43 - 2, ScrollBar.MaxValue), 7 * 43 - 3) + 1));
            ScrollBar.VisibleSize = ClientArea.Height;
            ScrollBar.Size = new Size(ScrollBar.Size.Width, ClientArea.Height - 2);

            RetrieveButton.Location = new Point((Size.Width - 80) / 2, Size.Height - 43);
            ScrollBar.Value = 0;
            UpdateLocations();
        }
        /// <summary>
        /// 更新位置
        /// </summary>
        private void UpdateLocations()
        {
            int y = -ScrollBar.Value + 1;

            foreach (NPCRefineCell cell in Cells)
            {
                cell.Location = new Point(1, y);

                y += cell.Size.Height + 3;
            }
        }
        /// <summary>
        /// 取回
        /// </summary>
        public void Retrieve()
        {
            if (GameScene.Game.Observer) return;
            if (SelectedCell == null) return;

            CEnvir.Enqueue(new C.NPCRefineRetrieve { Index = SelectedCell.Refine.Index });
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _SelectedCell = null;
                SelectedCellChanged = null;

                Refines.Clear();
                Refines = null;

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                if (Cells != null)
                {
                    for (int i = 0; i < Cells.Count; i++)
                    {
                        if (Cells[i] != null)
                        {
                            if (!Cells[i].IsDisposed)
                                Cells[i].Dispose();

                            Cells[i] = null;
                        }

                    }
                    Cells.Clear();
                    Cells = null;
                }

                if (RetrieveButton != null)
                {
                    if (!RetrieveButton.IsDisposed)
                        RetrieveButton.Dispose();

                    RetrieveButton = null;
                }

                if (ClientPanel != null)
                {
                    if (!ClientPanel.IsDisposed)
                        ClientPanel.Dispose();

                    ClientPanel = null;
                }
            }
        }
        #endregion
    }

    #endregion

    #region NPC精炼类型

    /// <summary>
    /// NPC精炼类型选择
    /// </summary>
    public sealed class NPCRefineCell : DXControl
    {
        #region Properties

        #region Refine
        /// <summary>
        /// 精炼类型信息
        /// </summary>
        public ClientRefineInfo Refine
        {
            get => _Refine;
            set
            {
                if (_Refine == value) return;

                ClientRefineInfo oldValue = _Refine;
                _Refine = value;

                OnRefineChanged(oldValue, value);
            }
        }
        private ClientRefineInfo _Refine;
        public event EventHandler<EventArgs> RefineChanged;
        public void OnRefineChanged(ClientRefineInfo oValue, ClientRefineInfo nValue)
        {
            ItemCell.Item = Refine.Weapon;
            ItemNameLabel.Text = Refine.Weapon.Info.Lang(p => p.ItemName);

            switch (Refine.Type)
            {
                case RefineType.Durability:
                    RefineTypeLabel.Text = "持久".Lang();
                    break;
                case RefineType.DC:
                    RefineTypeLabel.Text = "破坏".Lang();
                    break;
                case RefineType.SpellPower:
                    RefineTypeLabel.Text = "全系列魔法".Lang();
                    break;
                case RefineType.Fire:
                    RefineTypeLabel.Text = "火元素".Lang();
                    break;
                case RefineType.Ice:
                    RefineTypeLabel.Text = "冰元素".Lang();
                    break;
                case RefineType.Lightning:
                    RefineTypeLabel.Text = "雷元素".Lang();
                    break;
                case RefineType.Wind:
                    RefineTypeLabel.Text = "风元素".Lang();
                    break;
                case RefineType.Holy:
                    RefineTypeLabel.Text = "神圣元素".Lang();
                    break;
                case RefineType.Dark:
                    RefineTypeLabel.Text = "暗黑元素".Lang();
                    break;
                case RefineType.Phantom:
                    RefineTypeLabel.Text = "幻影元素".Lang();
                    break;
                case RefineType.Reset:
                    RefineTypeLabel.Text = "重置".Lang();
                    break;
            }

            MaxChanceLabel.Text = $"{Refine.MaxChance}%";
            ChanceLabel.Text = $"{Refine.Chance}%";

            if (CEnvir.Now > Refine.RetrieveTime)
            {
                RetrieveTimeLabel.Text = "完成".Lang();
                RetrieveTimeLabel.ForeColour = Color.LightSeaGreen;
            }
            else
            {
                RetrieveTimeLabel.Text = (Refine.RetrieveTime - CEnvir.Now).Lang(true);

                RetrieveTimeLabel.ProcessAction = () =>
                {
                    if (Refine == null || CEnvir.Now > Refine.RetrieveTime)
                    {
                        RetrieveTimeLabel.Text = "完成".Lang();
                        RetrieveTimeLabel.ForeColour = Color.LightSeaGreen;
                        RetrieveTimeLabel.ProcessAction = null;
                        return;
                    }

                    RetrieveTimeLabel.Text = (Refine.RetrieveTime - CEnvir.Now).Lang(true);
                    RetrieveTimeLabel.ForeColour = Color.White;
                };
            }
            RefineChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Selected
        /// <summary>
        /// 选择
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
            Border = Selected;
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);
            ItemCell.BorderColour = Selected ? Color.FromArgb(198, 166, 99) : Color.FromArgb(99, 83, 50);
            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXItemCell ItemCell;

        public DXLabel ItemNameLabel, RefineTypeLabel, ChanceLabel, MaxChanceLabel, RetrieveTimeLabel;

        #endregion

        /// <summary>
        /// NPC精炼类型选择
        /// </summary>
        public NPCRefineCell()
        {
            DrawTexture = true;
            BackColour = Color.FromArgb(25, 20, 0);

            BorderColour = Color.FromArgb(198, 166, 99);
            Size = new Size(483, 40);

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
                ShowCountLabel = false,
            };
            ItemNameLabel = new DXLabel   //道具名字标签
            {
                Parent = this,
                Location = new Point(ItemCell.Location.X * 2 + ItemCell.Size.Width, ItemCell.Location.Y),
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };

            RefineTypeLabel = new DXLabel
            {
                Parent = this,
                Text = "精炼类型".Lang(),
                IsControl = false,
            };
            RefineTypeLabel.Location = new Point(ItemCell.Location.X * 2 + ItemCell.Size.Width, ItemCell.Location.Y + ItemCell.Size.Height - RefineTypeLabel.Size.Height);

            RefineTypeLabel = new DXLabel
            {
                Parent = this,
                Text = "没有".Lang(),
                IsControl = false,
                ForeColour = Color.White,
                Location = new Point(RefineTypeLabel.Location.X + RefineTypeLabel.Size.Width, RefineTypeLabel.Location.Y)
            };

            ChanceLabel = new DXLabel
            {
                Parent = this,
                Text = "成功几率".Lang(),
                IsControl = false,
            };
            ChanceLabel.Location = new Point(300 - ChanceLabel.Size.Width, ItemNameLabel.Location.Y);

            ChanceLabel = new DXLabel
            {
                Parent = this,
                Text = "0%",
                IsControl = false,
                ForeColour = Color.White,
                Location = new Point(ChanceLabel.Location.X + ChanceLabel.Size.Width, ChanceLabel.Location.Y)
            };

            MaxChanceLabel = new DXLabel
            {
                Parent = this,
                Text = "最大成功几率".Lang(),
                IsControl = false,
            };
            MaxChanceLabel.Location = new Point(300 - MaxChanceLabel.Size.Width, RefineTypeLabel.Location.Y);

            MaxChanceLabel = new DXLabel
            {
                Parent = this,
                Text = "0%",
                IsControl = false,
                ForeColour = Color.White,
                Location = new Point(MaxChanceLabel.Location.X + MaxChanceLabel.Size.Width, MaxChanceLabel.Location.Y)
            };

            RetrieveTimeLabel = new DXLabel
            {
                Parent = this,
                Text = "剩余时间".Lang(),
                IsControl = false,
            };
            RetrieveTimeLabel.Location = new Point(390 - RetrieveTimeLabel.Size.Width, RefineTypeLabel.Location.Y);

            RetrieveTimeLabel = new DXLabel
            {
                Parent = this,
                Text = "0 " + "秒".Lang(),
                IsControl = false,
                ForeColour = Color.White,
                Location = new Point(RetrieveTimeLabel.Location.X + RetrieveTimeLabel.Size.Width, RetrieveTimeLabel.Location.Y)
            };
        }

        #region Methods
        /// <summary>
        /// 鼠标进入时
        /// </summary>
        public override void OnMouseEnter()
        {
            base.OnMouseEnter();

            GameScene.Game.MouseItem = ItemCell.Item;
        }
        /// <summary>
        /// 鼠标离开时
        /// </summary>
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            GameScene.Game.MouseItem = null;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Refine = null;
                RefineChanged = null;

                _Selected = false;
                SelectedChanged = null;

                if (ItemCell != null)
                {
                    if (!ItemCell.IsDisposed)
                        ItemCell.Dispose();

                    ItemCell = null;
                }

                if (ItemNameLabel != null)
                {
                    if (!ItemNameLabel.IsDisposed)
                        ItemNameLabel.Dispose();

                    ItemNameLabel = null;
                }

                if (RefineTypeLabel != null)
                {
                    if (!RefineTypeLabel.IsDisposed)
                        RefineTypeLabel.Dispose();

                    RefineTypeLabel = null;
                }

                if (ChanceLabel != null)
                {
                    if (!ChanceLabel.IsDisposed)
                        ChanceLabel.Dispose();

                    ChanceLabel = null;
                }

                if (MaxChanceLabel != null)
                {
                    if (!MaxChanceLabel.IsDisposed)
                        MaxChanceLabel.Dispose();

                    MaxChanceLabel = null;
                }

                if (RetrieveTimeLabel != null)
                {
                    if (!RetrieveTimeLabel.IsDisposed)
                        RetrieveTimeLabel.Dispose();

                    RetrieveTimeLabel = null;
                }
            }
        }
        #endregion
    }

    #endregion

    #region NPC任务接取

    /// <summary>
    /// NPC任务接取列表
    /// </summary>
    public sealed class NPCQuestListDialog : DXWindow
    {
        #region Properties

        #region NPCInfo
        /// <summary>
        /// NPC信息
        /// </summary>
        public NPCInfo NPCInfo
        {
            get => _NPCInfo;
            set
            {
                if (_NPCInfo == value) return;

                NPCInfo oldValue = _NPCInfo;
                _NPCInfo = value;

                OnNPCInfoChanged(oldValue, value);
            }
        }
        private NPCInfo _NPCInfo;
        public event EventHandler<EventArgs> NPCInfoChanged;
        public void OnNPCInfoChanged(NPCInfo oValue, NPCInfo nValue)
        {
            NPCInfoChanged?.Invoke(this, EventArgs.Empty);


            UpdateQuestDisplay();
        }

        #endregion

        #region SelectedQuest
        /// <summary>
        /// 选定的任务
        /// </summary>
        public NPCQuestRow SelectedQuest
        {
            get => _SelectedQuest;
            set
            {
                if (_SelectedQuest == value)
                {
                    if (_SelectedQuest != null && _SelectedQuest.UserQuest != null && _SelectedQuest.UserQuest.Completed)
                    {
                        OnSelectedQuestChanged(_SelectedQuest, value);
                    }
                    return;
                }

                NPCQuestRow oldValue = _SelectedQuest;
                _SelectedQuest = value;

                OnSelectedQuestChanged(oldValue, value);
            }
        }
        private NPCQuestRow _SelectedQuest;
        public event EventHandler<EventArgs> SelectedQuestChanged;
        public void OnSelectedQuestChanged(NPCQuestRow oValue, NPCQuestRow nValue)
        {
            if (oValue != null)
                oValue.Selected = false;
            if (SelectedQuest == null)
            {
                GameScene.Game.NPCQuestBox.Visible = false;
                return;
            }

            if (SelectedQuest?.QuestInfo == null)
            {
                return;
            }
            SelectedQuest.Selected = true;

            HasChoice = false;

            foreach (QuestReward reward in SelectedQuest.QuestInfo.Rewards)
            {
                switch (MapObject.User.Class)
                {
                    case MirClass.Warrior:
                        if ((reward.Class & RequiredClass.Warrior) != RequiredClass.Warrior) continue;
                        break;
                    case MirClass.Wizard:
                        if ((reward.Class & RequiredClass.Wizard) != RequiredClass.Wizard) continue;
                        break;
                    case MirClass.Taoist:
                        if ((reward.Class & RequiredClass.Taoist) != RequiredClass.Taoist) continue;
                        break;
                    case MirClass.Assassin:
                        if ((reward.Class & RequiredClass.Assassin) != RequiredClass.Assassin) continue;
                        break;
                }
            }
            GameScene.Game.NPCQuestBox.NPCInfo = NPCInfo;
            GameScene.Game.NPCQuestBox.SelectedQuest = SelectedQuest;
            GameScene.Game.NPCQuestBox.Visible = true;

            SelectedQuestChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);
            if (!Visible)
            {
                SelectedQuest = null;
            }
        }
        /// <summary>
        /// NPC显示任务行
        /// </summary>
        public NPCQuestRow[] Rows;
        /// <summary>
        /// 任务背景图
        /// </summary>
        public DXImageControl Background;
        /// <summary>
        /// 任务信息列表
        /// </summary>
        public List<QuestInfo> Quests = new List<QuestInfo>();
        /// <summary>
        /// 滚动条
        /// </summary>
        public DXVScrollBar ScrollBar;
        /// <summary>
        /// 任务计数
        /// </summary>
        public DXLabel Count;
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public DXButton ShutButton;
        /// <summary>
        /// 是否可以选择
        /// </summary>
        public bool HasChoice;

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// NPC任务接取面板
        /// </summary>
        public NPCQuestListDialog()
        {
            Opacity = 1F;
            Movable = false;
            HasFooter = false;
            HasTopBorder = false;
            Location = new Point(0, GameScene.Game.NPCBox.Size.Height);

            Background = new DXImageControl    //背景图
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 900,
                Parent = this,
            };

            ScrollBar = new DXVScrollBar    //滚动条
            {
                Size = new Size(15, 22 * 7),
                Parent = Background,
                Location = new Point(Size.Width + 356, 30),
                VisibleSize = 7,                       //可见尺寸为7格
                Change = 1,                             //改变为1格
            };
            ScrollBar.ValueChanged += ScrollBar_ValueChanged;
            //为滚动条自定义皮肤 -1为不设置
            ScrollBar.SetSkin(LibraryFile.GameInter, -1, -1, -1, 1225);

            Size = Background.Size;

            Rows = new NPCQuestRow[7];

            for (int i = 0; i < Rows.Length; i++)
            {
                Rows[i] = new NPCQuestRow
                {
                    Parent = Background,
                    Location = new Point(18, 49 + i * 16)
                };
                int index = i;
                Rows[index].MouseClick += (o, e) =>
                {
                    if (Rows[index].QuestInfo == null) return;

                    SelectedQuest = Rows[index];
                };
                Rows[i].MouseWheel += ScrollBar.DoMouseWheel;
            }
            MouseWheel += ScrollBar.DoMouseWheel;

            var label = new DXLabel
            {
                Parent = Background,
                Location = new Point(15, Size.Height - 35),
                Size = new Size(173, 15),
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = "可接收任务".Lang(),
                ForeColour = Color.White
            };
            Count = new DXLabel    //任务数量
            {
                Size = new Size(50, 15),
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Parent = Background,
                Location = new Point(label.Location.X + label.Size.Width + 17, label.Location.Y),
                ForeColour = Color.White
            };

            ShutButton = new DXButton  //关闭按钮
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1221,
                Parent = Background,
                Location = new Point(347, 177),
            };
            ShutButton.MouseClick += (o, e) => Visible = false;
        }

        #region Method

        /// <summary>
        /// 滚动条变化时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateQuestDisplay();
        }
        /// <summary>
        /// 更新任务显示
        /// </summary>
        public void UpdateQuestDisplay()
        {
            if (NPCInfo == null)   //任务信息等空  就不显示
            {
                Visible = false;
                return;
            }
            Location = new Point(0, GameScene.Game.NPCBox.Size.Height);

            Quests.Clear();
            //availableQuests 可接任务                 currentQuests 当前任务                   completeQuests 完成的任务
            List<QuestInfo> availableQuests = new List<QuestInfo>(), currentQuests = new List<QuestInfo>(), completeQuests = new List<QuestInfo>();

            foreach (QuestInfo quest in NPCInfo.FinishQuests)  //遍历结束任务
            {
                ClientUserQuest userQuest = GameScene.Game.QuestLog.FirstOrDefault(x => x.Quest == quest);   //角色任务日志里的任务内容

                if (userQuest == null || userQuest.Completed) continue;   //如果角色任务日志等空  或者 任务完成 忽略

                if (!userQuest.IsComplete)        //如果不是已完成的任务
                    currentQuests.Add(quest);     //当前任务添加
                else
                    completeQuests.Add(quest);    //完成任务添加
            }

            foreach (QuestInfo quest in NPCInfo.StartQuests)  //遍历开始任务
            {
                if (!GameScene.Game.CanAccept(quest)) continue;   //如果不是可以结束的任务类型 忽略

                //每日 可重复 奇遇 不显示
                //if(quest.QuestType == QuestType.Daily || quest.QuestType == QuestType.Repeatable || quest.QuestType == QuestType.Hidden)
                //    continue;

                //每日任务不直接列出
                if (quest.QuestType == QuestType.Daily)
                {
                    continue;
                }
                ///如果不是包含当前任务 且 不是包含完成任务
                if (!currentQuests.Contains(quest) && !completeQuests.Contains(quest))
                    availableQuests.Add(quest);     //增加可接任务
            }

            completeQuests.Sort((x1, x2) => string.Compare(x1.QuestName, x2.QuestName, StringComparison.Ordinal));
            availableQuests.Sort((x1, x2) => string.Compare(x1.QuestName, x2.QuestName, StringComparison.Ordinal));
            currentQuests.Sort((x1, x2) => string.Compare(x1.QuestName, x2.QuestName, StringComparison.Ordinal));

            Quests.AddRange(completeQuests);  //任务添加范围 完成任务
            Quests.AddRange(availableQuests); //任务添加范围 可接任务
            Quests.AddRange(currentQuests);   //任务添加范围 当前任务

            Visible = Quests.Count > 0;   //必须是任务条数大于0才显示任务

            if (Quests.Count == 0) return;  //如果任务计数等0 跳出

            Count.Text = availableQuests.Count.ToString();
            QuestInfo previousQuest = SelectedQuest?.QuestInfo;  //以前的任务

            _SelectedQuest = null;

            UpdateScrollBar();

            if (previousQuest != null)  //如果以前的任务不等空
            {
                foreach (NPCQuestRow row in Rows)   //遍历任务行
                {
                    if (row.QuestInfo != previousQuest) continue;

                    _SelectedQuest = row;
                    break;
                }
            }

            GameScene.Game.NPCQuestBox.SelectedQuest = SelectedQuest;
        }
        /// <summary>
        /// 更新滚动条
        /// </summary>
        public void UpdateScrollBar()
        {
            ScrollBar.MaxValue = Quests.Count;

            for (int i = 0; i < Rows.Length; i++)
            {
                Rows[i].QuestInfo = i + ScrollBar.Value >= Quests.Count ? null : Quests[i + ScrollBar.Value];
            }
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _NPCInfo = null;
                NPCInfoChanged = null;

                Quests.Clear();
                Quests = null;

                HasChoice = false;

                _SelectedQuest = null;
                SelectedQuestChanged = null;

                if (Rows != null)
                {
                    for (int i = 0; i < Rows.Length; i++)
                    {
                        if (Rows[i] != null)
                        {
                            if (!Rows[i].IsDisposed)
                                Rows[i].Dispose();

                            Rows[i] = null;
                        }

                    }

                    Rows = null;
                }
                if (Background != null)
                {
                    if (!Background.IsDisposed)
                        Background.Dispose();

                    Background = null;
                }
                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }
                if (Count != null)
                {
                    if (!Count.IsDisposed)
                        Count.Dispose();

                    Count = null;
                }
                if (ShutButton != null)
                {
                    if (!ShutButton.IsDisposed)
                        ShutButton.Dispose();

                    ShutButton = null;
                }
            }
        }
        #endregion
    }

    #endregion

    #region NPC任务内容

    /// <summary>
    /// NPC任务内容框架
    /// </summary>
    public sealed class NPCQuestDialog : DXWindow
    {
        #region Properties

        #region NPCInfo
        /// <summary>
        /// NPC信息
        /// </summary>
        public NPCInfo NPCInfo
        {
            get => _NPCInfo;
            set
            {
                if (_NPCInfo == value) return;

                NPCInfo oldValue = _NPCInfo;
                _NPCInfo = value;

                OnNPCInfoChanged(oldValue, value);
            }
        }
        private NPCInfo _NPCInfo;
        public event EventHandler<EventArgs> NPCInfoChanged;
        public void OnNPCInfoChanged(NPCInfo oValue, NPCInfo nValue)
        {
            NPCInfoChanged?.Invoke(this, EventArgs.Empty);


            UpdateQuestDisplay();
        }

        #endregion

        #region SelectedQuest
        /// <summary>
        /// 选定的任务
        /// </summary>
        public NPCQuestRow SelectedQuest
        {
            get => _SelectedQuest;
            set
            {
                if (_SelectedQuest == value) return;

                NPCQuestRow oldValue = _SelectedQuest;
                _SelectedQuest = value;

                OnSelectedQuestChanged(oldValue, value);
            }
        }
        private NPCQuestRow _SelectedQuest;
        public event EventHandler<EventArgs> SelectedQuestChanged;
        public void OnSelectedQuestChanged(NPCQuestRow oValue, NPCQuestRow nValue)
        {
            if (oValue != null)
                oValue.Selected = false;

            foreach (DXItemCell cell in RewardGrid.Grid)
            {
                cell.Item = null;
                cell.Tag = null;
            }

            foreach (DXItemCell cell in ChoiceGrid.Grid)
            {
                cell.Item = null;
                cell.Tag = null;
            }

            if (SelectedQuest?.QuestInfo == null)
            {
                TaskView.RemoveAll();
                DescriptionLabel.Text = string.Empty;

                AcceptButton.Visible = false;
                CompleteButton.Visible = false;
                EndLabel.Text = string.Empty;
                Visible = false;
                return;
            }

            SelectedQuest.Selected = true;

            int standard = 0, choice = 0;
            HasChoice = false;
            RandomChoice = false;

            foreach (QuestReward reward in SelectedQuest.QuestInfo.Rewards)
            {
                switch (MapObject.User.Class)
                {
                    case MirClass.Warrior:
                        if ((reward.Class & RequiredClass.Warrior) != RequiredClass.Warrior) continue;
                        break;
                    case MirClass.Wizard:
                        if ((reward.Class & RequiredClass.Wizard) != RequiredClass.Wizard) continue;
                        break;
                    case MirClass.Taoist:
                        if ((reward.Class & RequiredClass.Taoist) != RequiredClass.Taoist) continue;
                        break;
                    case MirClass.Assassin:
                        if ((reward.Class & RequiredClass.Assassin) != RequiredClass.Assassin) continue;
                        break;
                }

                UserItemFlags flags = UserItemFlags.None;
                TimeSpan duration = TimeSpan.FromSeconds(reward.Duration);

                if (reward.Bound)
                    flags |= UserItemFlags.Bound;

                if (duration != TimeSpan.Zero)
                    flags |= UserItemFlags.Expirable;

                ClientUserItem item = new ClientUserItem(reward.Item, reward.Amount)
                {
                    Flags = flags,
                    ExpireTime = duration
                };

                ChooseReward.Visible = false;
                ChoiceGrid.Visible = false;

                if (reward.Choice)  //任务里选择奖励
                {
                    if (choice >= ChoiceGrid.Grid.Length) continue;

                    HasChoice = true;

                    ChoiceGrid.Grid[choice].Item = item;
                    ChoiceGrid.Grid[choice].Tag = reward;
                    choice++;
                }
                else if (reward.Random)
                {
                    ChoiceGrid.Grid[choice].Item = item;
                    ChoiceGrid.Grid[choice].Tag = reward;
                    choice++;
                    RandomChoice = true;
                }
                else
                {
                    if (standard >= RewardGrid.Grid.Length) continue;

                    RewardGrid.Grid[standard].Item = item;
                    RewardGrid.Grid[standard].Tag = reward;
                    standard++;
                }
            }

            if (HasChoice || RandomChoice)
            {
                ChooseReward.Visible = true;
                ChoiceGrid.Visible = true;
                ChooseReward.Index = RandomChoice ? 924 : HasChoice ? 923 : -1;//随机奖励
            }

            if (HasChoice)
                SelectedCell = null;

            QuestNameLabel.Text = SelectedQuest.QuestInfo.QuestName;
            DescriptionLabel.Text = GameScene.Game.GetQuestText(SelectedQuest.QuestInfo, SelectedQuest.UserQuest, false);
            GameScene.Game.GetTaskText(TaskView, SelectedQuest.QuestInfo, SelectedQuest.UserQuest);

            EndLabel.Text = string.Format("({0}) {1}", SelectedQuest.QuestInfo?.FinishNPC?.Region?.Map?.Description ?? "无".Lang(), SelectedQuest.QuestInfo?.FinishNPC?.NPCName ?? "无".Lang());
            EndLabel.Location = new Point(EndLabelButton.Location.X - EndLabel.Size.Width - 2, EndLabelButton.Location.Y + 2);

            AcceptButton.Visible = SelectedQuest.UserQuest == null;
            CompleteButton.Visible = SelectedQuest.UserQuest != null && SelectedQuest.UserQuest.IsComplete;

            SelectedQuestChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region SelectedCell
        /// <summary>
        /// 选定的道具
        /// </summary>
        public DXItemCell SelectedCell
        {
            get => _SelectedCell;
            set
            {
                DXItemCell oldValue = _SelectedCell;
                _SelectedCell = value;

                OnSelectedCellChanged(oldValue, value);
            }
        }
        private DXItemCell _SelectedCell;
        public event EventHandler<EventArgs> SelectedCellChanged;
        public void OnSelectedCellChanged(DXItemCell oValue, DXItemCell nValue)
        {
            if (oValue != null)
            {
                oValue.FixedBorder = false;
                oValue.Border = false;
                oValue.FixedBorderColour = false;
                oValue.BorderColour = Color.Lime;
            }

            if (nValue != null)
            {
                nValue.Border = true;
                nValue.FixedBorder = true;
                nValue.FixedBorderColour = true;
                nValue.BorderColour = Color.Lime;
            }

            SelectedCellChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXImageControl BackGround, Reward, ChooseReward;

        public DXLabel QuestNameLabel, DescriptionLabel, EndLabel;
        public DXButton EndLabelButton;
        public DXListView TaskView;

        public DXItemGrid RewardGrid, ChoiceGrid;

        public DXButton AcceptButton, CompleteButton, RefuseButton, Closs1Button;

        public ClientUserItem[] RewardArray, ChoiceArray;

        public bool HasChoice;

        public bool RandomChoice;


        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// NPC任务内容面板
        /// </summary>
        public NPCQuestDialog()
        {
            //TitleLabel.Text = "任务";

            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            CloseButton.Visible = false;
            Opacity = 0F;

            //SetClientSize(new Size(300, 380));
            Size s = GameInterLibrary.GetSize(950);
            Size = s;
            Location = new Point(GameScene.Game.NPCBox.Size.Width, 0);

            BackGround = new DXImageControl   //背景图
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 950,
                Opacity = 0.7F,
                Location = new Point(0, 0),
                IsControl = true,
                PassThrough = true,
            };

            DXControl panel = new DXControl
            {
                Size = new Size(ClientArea.Width, 2),
                Location = new Point(ClientArea.X, ClientArea.Top),
                Parent = this,
                DrawTexture = true,
            };

            QuestNameLabel = new DXLabel   //任务名字标签
            {
                AutoSize = false,
                Size = new Size(ClientArea.Width - 4, 18),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                Location = new Point(ClientArea.X + 5, panel.Location.Y + panel.Size.Height + 25),
                Parent = this,
            };

            /*var label = new DXLabel
            {
                Text = "内容",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(ClientArea.X, panel.Location.Y + panel.Size.Height + 5),
            };*/

            DescriptionLabel = new DXLabel   //任务说明标签
            {
                AutoSize = false,
                Size = new Size(ClientArea.Width - 35, 80),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                Location = new Point(ClientArea.X + 5, panel.Location.Y + panel.Size.Height + 75),
                Parent = this,
            };

            /*label = new DXLabel
            {
                Text = "任务",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(ClientArea.X, DescriptionLabel.Location.Y + DescriptionLabel.Size.Height + 5),
            };*/


            //TasksLabel = new DXLabel     //完成条件标签
            //{
            //    AutoSize = false,
            //    Size = new Size(ClientArea.Width - 5, 65),
            //    //Border = true,
            //    //BorderColour = Color.FromArgb(198, 166, 99),
            //    ForeColour = Color.White,
            //    Location = new Point(ClientArea.X + 5, DescriptionLabel.Location.Y + DescriptionLabel.Size.Height + 34),
            //    Parent = this,
            //};
            TaskView = new DXListView
            {
                HasHeader = false,
                HasVScrollBar = false,
                ItemBorder = false,
                SelectedBorder = false,
                ItemSelectedBackColour = Color.Empty,
                ItemSelectedBorderColour = Color.Empty,
                ItemForeColour = Color.White,
                Parent = this,
                Size = new Size(330, 105),
                Location = new Point(ClientArea.X + 6, DescriptionLabel.Location.Y + DescriptionLabel.Size.Height + 2),
            };
            TaskView.InsertColumn(0, "序号", 36, 16, "序号");
            TaskView.InsertColumn(1, "条件", 190, 16, "完成条件");
            TaskView.InsertColumn(2, "数量", 128, 16, "完成数量");

            Reward = new DXImageControl  //奖励
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 922,
                Parent = this,
                IsControl = false,
                Location = new Point(ClientArea.X + 2, TaskView.Location.Y + TaskView.Size.Height + 15),
            };

            RewardArray = new ClientUserItem[5];
            RewardGrid = new DXItemGrid
            {
                Parent = this,
                Location = new Point(ClientArea.X + 10, Reward.Location.Y + Reward.Size.Height + 2),
                GridSize = new Size(RewardArray.Length, 1),
                ItemGrid = RewardArray,
                ReadOnly = true,
            };

            ChooseReward = new DXImageControl  //选择奖励
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 923,
                Parent = this,
                IsControl = false,
                Location = new Point(ClientArea.X + 4, RewardGrid.Location.Y + RewardGrid.Size.Height + 4),
                Visible = false,
            };

            ChoiceArray = new ClientUserItem[4];
            ChoiceGrid = new DXItemGrid
            {
                Parent = this,
                Location = new Point(ClientArea.X + 10, ChooseReward.Location.Y + ChooseReward.Size.Height + 4),
                GridSize = new Size(ChoiceArray.Length, 1),
                ItemGrid = ChoiceArray,
                ReadOnly = true,
                Visible = false,
            };

            foreach (DXItemCell cell in ChoiceGrid.Grid)
            {
                cell.MouseClick += (o, e) =>
                {
                    if (RandomChoice) return;
                    if (((DXItemCell)o).Item == null) return;

                    SelectedCell = (DXItemCell)o;
                };
            }

            //var label = new DXLabel
            //{
            //    Text = "结束".Lang(),
            //    Parent = this,
            //    Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
            //    //ForeColour = Color.FromArgb(198, 166, 99),
            //    Outline = true,
            //    OutlineColour = Color.Black,
            //    IsControl = false,
            //    Location = new Point(ClientArea.X + 210, DescriptionLabel.Location.Y + DescriptionLabel.Size.Height + 15),
            //};

            EndLabelButton = new DXButton   //npc大地图按钮
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 137,
                Parent = this,
                Location = new Point(310, 180),
            };
            EndLabelButton.MouseClick += (o, e) =>  //鼠标点击时
            {
                if (SelectedQuest?.QuestInfo?.FinishNPC?.Region?.Map == null) return;

                GameScene.Game.BigMapBox.Visible = true;
                GameScene.Game.BigMapBox.Opacity = 1F;
                GameScene.Game.BigMapBox.SelectedInfo = SelectedQuest.QuestInfo.FinishNPC.Region.Map;
            };

            EndLabel = new DXLabel   //结束标签
            {
                Parent = this,
                ForeColour = Color.White,
                Location = new Point(EndLabelButton.Location.X - 50, EndLabelButton.Location.Y),
            };

            AcceptButton = new DXButton   //接受任务按钮
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 977,
                Parent = this,
                Location = new Point(ClientArea.X + (ClientArea.Size.Width - 300), ChoiceGrid.Location.Y + ChoiceGrid.Size.Height + 36),
                Size = new Size(100, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Visible = false,
            };
            AcceptButton.MouseEnter += (o, e) => AcceptButton.Index = 975;
            AcceptButton.MouseLeave += (o, e) => AcceptButton.Index = 977;
            AcceptButton.MouseClick += (o, e) =>
            {
                if (SelectedQuest?.QuestInfo == null) return;

                CEnvir.Enqueue(new C.QuestAccept { Index = SelectedQuest.QuestInfo.Index });
            };

            CompleteButton = new DXButton   //完成任务按钮
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 997,
                Parent = this,
                Location = new Point(ClientArea.X + (ClientArea.Size.Width - 300), ChoiceGrid.Location.Y + ChoiceGrid.Size.Height + 36),
                Size = new Size(100, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Visible = false,
            };
            CompleteButton.MouseEnter += (o, e) => CompleteButton.Index = 995;
            CompleteButton.MouseLeave += (o, e) => CompleteButton.Index = 997;
            CompleteButton.MouseClick += (o, e) =>
            {
                if (SelectedQuest?.QuestInfo == null) return;

                if (HasChoice && !RandomChoice && SelectedCell == null)
                {
                    GameScene.Game.ReceiveChat("请选择一个奖励".Lang(), MessageType.System);
                    return;
                }

                CEnvir.Enqueue(new C.QuestComplete { Index = SelectedQuest.QuestInfo.Index, ChoiceIndex = ((QuestReward)SelectedCell?.Tag)?.Index ?? 0 });
            };

            RefuseButton = new DXButton   //拒绝任务按钮 关闭
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 987,
                Parent = this,
                Location = new Point(ClientArea.X + (ClientArea.Size.Width - 130), ChoiceGrid.Location.Y + ChoiceGrid.Size.Height + 36),
                Size = new Size(100, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
            };
            RefuseButton.MouseEnter += (o, e) => RefuseButton.Index = 985;
            RefuseButton.MouseLeave += (o, e) => RefuseButton.Index = 987;
            RefuseButton.MouseClick += (o, e) =>
            {
                GameScene.Game.NPCQuestListBox.SelectedQuest = null;
            };

            Closs1Button = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1221,
                Parent = this,
                Location = new Point(328, 438),
            };
            Closs1Button.MouseClick += (o, e) =>
            {
                GameScene.Game.NPCQuestListBox.SelectedQuest = null;
            };
        }

        #region Methods
        /// <summary>
        /// 更新任务显示
        /// </summary>
        public void UpdateQuestDisplay()
        {
            if (NPCInfo == null)
            {
                Visible = false;
                return;
            }

            if (SelectedQuest?.QuestInfo != null)
            {
                DescriptionLabel.Text = GameScene.Game.GetQuestText(SelectedQuest.QuestInfo, SelectedQuest.UserQuest, false);
                GameScene.Game.GetTaskText(TaskView, SelectedQuest.QuestInfo, SelectedQuest.UserQuest);

                AcceptButton.Visible = SelectedQuest.UserQuest == null;
                CompleteButton.Visible = SelectedQuest.UserQuest != null && SelectedQuest.UserQuest.IsComplete;
            }
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _NPCInfo = null;
                NPCInfoChanged = null;

                HasChoice = false;
                RandomChoice = false;

                _SelectedQuest = null;
                SelectedQuestChanged = null;

                _SelectedCell = null;
                SelectedCellChanged = null;

                if (BackGround != null)
                {
                    if (!BackGround.IsDisposed)
                        BackGround.Dispose();

                    BackGround = null;
                }

                if (QuestNameLabel != null)
                {
                    if (!QuestNameLabel.IsDisposed)
                        QuestNameLabel.Dispose();

                    QuestNameLabel = null;
                }

                if (TaskView != null)
                {
                    if (!TaskView.IsDisposed)
                        TaskView.Dispose();

                    TaskView = null;
                }

                if (DescriptionLabel != null)
                {
                    if (!DescriptionLabel.IsDisposed)
                        DescriptionLabel.Dispose();

                    DescriptionLabel = null;
                }

                if (EndLabel != null)
                {
                    if (!EndLabel.IsDisposed)
                        EndLabel.Dispose();

                    EndLabel = null;
                }

                if (RewardGrid != null)
                {
                    if (!RewardGrid.IsDisposed)
                        RewardGrid.Dispose();

                    RewardGrid = null;
                }

                if (ChoiceGrid != null)
                {
                    if (!ChoiceGrid.IsDisposed)
                        ChoiceGrid.Dispose();

                    ChoiceGrid = null;
                }

                if (AcceptButton != null)
                {
                    if (!AcceptButton.IsDisposed)
                        AcceptButton.Dispose();

                    AcceptButton = null;
                }

                if (CompleteButton != null)
                {
                    if (!CompleteButton.IsDisposed)
                        CompleteButton.Dispose();

                    CompleteButton = null;
                }

                if (RefuseButton != null)
                {
                    if (!RefuseButton.IsDisposed)
                        RefuseButton.Dispose();

                    RefuseButton = null;
                }

                if (Reward != null)
                {
                    if (!Reward.IsDisposed)
                        Reward.Dispose();

                    Reward = null;
                }

                if (ChooseReward != null)
                {
                    if (!ChooseReward.IsDisposed)
                        ChooseReward.Dispose();

                    ChooseReward = null;
                }

                if (Closs1Button != null)
                {
                    if (!Closs1Button.IsDisposed)
                        Closs1Button.Dispose();

                    Closs1Button = null;
                }

                if (EndLabelButton != null)
                {
                    if (!EndLabelButton.IsDisposed)
                        EndLabelButton.Dispose();

                    EndLabelButton = null;
                }

                RewardArray = null;
                ChoiceArray = null;
            }
        }
        #endregion
    }

    #endregion

    #region NPC任务行

    /// <summary>
    /// NPC任务行
    /// </summary>
    public sealed class NPCQuestRow : DXControl
    {
        #region Properties

        #region QuestInfo
        /// <summary>
        /// 任务信息
        /// </summary>
        public QuestInfo QuestInfo
        {
            get => _QuestInfo;
            set
            {
                QuestInfo oldValue = _QuestInfo;
                _QuestInfo = value;

                OnQuestInfoChanged(oldValue, value);
            }
        }
        private QuestInfo _QuestInfo;
        public event EventHandler<EventArgs> QuestInfoChanged;
        public void OnQuestInfoChanged(QuestInfo oValue, QuestInfo nValue)
        {
            if (QuestInfo == null)
            {
                Selected = false;
                UserQuest = null;
                QuestNameLabel.Text = string.Empty;
                QuestIcon.Visible = false;
            }
            else
            {
                UserQuest = GameScene.Game.QuestLog.FirstOrDefault(x => x.Quest == QuestInfo);
                if (QuestInfo.QuestType == QuestType.Repeatable && UserQuest != null && !UserQuest.IsComplete && GameScene.Game.User.RepeatableQuestRemains > 0)
                {
                    {
                        UserQuest = null;
                    }
                }

                if (QuestInfo.QuestType == QuestType.Hidden && UserQuest != null && !UserQuest.IsComplete)
                {
                    {
                        UserQuest = null;
                    }
                }

                if (QuestInfo.QuestType == QuestType.Daily && UserQuest != null && !UserQuest.IsComplete && GameScene.Game.User.DailyQuestRemains > 0)
                {
                    UserQuest = null;
                }

                QuestNameLabel.Text = QuestInfo.QuestName;
                QuestIcon.Visible = true;
            }

            if (UserQuest == null)
                QuestIcon.BaseIndex = 83; //Available
            else if (!UserQuest.IsComplete)
                QuestIcon.BaseIndex = 85; //Completed
            else
                QuestIcon.BaseIndex = 93; //Current

            QuestInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region UserQuest
        /// <summary>
        /// 角色任务
        /// </summary>
        public ClientUserQuest UserQuest
        {
            get => _UserQuest;
            set
            {
                ClientUserQuest oldValue = _UserQuest;
                _UserQuest = value;

                OnUserQuestChanged(oldValue, value);
            }
        }
        private ClientUserQuest _UserQuest;
        public event EventHandler<EventArgs> UserQuestChanged;
        public void OnUserQuestChanged(ClientUserQuest oValue, ClientUserQuest nValue)
        {
            UserQuestChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

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
            //Border = Selected;
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.Empty;

            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXAnimatedControl QuestIcon;
        public DXLabel QuestNameLabel;

        #endregion

        /// <summary>
        /// NPC任务行内容
        /// </summary>
        public NPCQuestRow()
        {
            DrawTexture = true;
            BackColour = Color.Empty;

            Size = new Size(326, 16);

            QuestIcon = new DXAnimatedControl      //任务图标
            {
                Parent = this,
                Location = new Point(0, 0),
                Loop = true,
                LibraryFile = LibraryFile.Interface,
                BaseIndex = 83,
                FrameCount = 2,
                AnimationDelay = TimeSpan.FromSeconds(1),
                Visible = false,
                IsControl = false,
            };

            QuestNameLabel = new DXLabel          //任务名字标签
            {
                Location = new Point(42, 0),
                Parent = this,
                ForeColour = Color.White,
                IsControl = false,
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _QuestInfo = null;
                QuestInfoChanged = null;

                _UserQuest = null;
                UserQuestChanged = null;

                _Selected = false;
                SelectedChanged = null;

                if (QuestIcon != null)
                {
                    if (!QuestIcon.IsDisposed)
                        QuestIcon.Dispose();

                    QuestIcon = null;
                }

                if (QuestNameLabel != null)
                {
                    if (!QuestNameLabel.IsDisposed)
                        QuestNameLabel.Dispose();

                    QuestNameLabel = null;
                }
            }
        }
        #endregion
    }

    #endregion

    #region NPC宠物购买

    /// <summary>
    /// NPC宠物购买功能
    /// </summary>
    public sealed class NPCAdoptCompanionDialog : DXWindow
    {
        #region Properties

        public MonsterObject CompanionDisplay;
        public Point CompanionDisplayPoint;

        public DXLabel NameLabel, IndexLabel, PriceLabel;
        public DXButton LeftButton, RightButton, AdoptButton, UnlockButton;

        public DXTextBox CompanionNameTextBox;

        public List<CompanionInfo> AvailableCompanions = new List<CompanionInfo>();

        #region SelectedCompanionInfo
        /// <summary>
        /// 选定宠物信息
        /// </summary>
        public CompanionInfo SelectedCompanionInfo
        {
            get => _SelectedCompanionInfo;
            set
            {
                if (_SelectedCompanionInfo == value) return;

                CompanionInfo oldValue = _SelectedCompanionInfo;
                _SelectedCompanionInfo = value;

                OnSelectedCompanionInfoChanged(oldValue, value);
            }
        }
        private CompanionInfo _SelectedCompanionInfo;
        public event EventHandler<EventArgs> SelectedCompanionInfoChanged;
        public void OnSelectedCompanionInfoChanged(CompanionInfo oValue, CompanionInfo nValue)
        {
            CompanionDisplay = null;

            if (SelectedCompanionInfo?.MonsterInfo == null) return;

            CompanionDisplay = new MonsterObject(SelectedCompanionInfo);

            RefreshUnlockButton();

            if (SelectedCompanionInfo.Price > 0)
            {
                PriceLabel.Text = SelectedCompanionInfo.Price.ToString("#,##0");
                PriceLabel.Text += " " + "金币".Lang();
                PriceLabel.ForeColour = Color.White;
            }
            else
            {
                PriceLabel.Text = SelectedCompanionInfo.GameGoldPrice.ToString("#,##0");
                PriceLabel.Text += " " + "赞助币".Lang();
                PriceLabel.ForeColour = Color.Yellow;
            }
            NameLabel.Text = SelectedCompanionInfo.MonsterInfo.MonsterName;
            SelectedCompanionInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region SelectedIndex
        /// <summary>
        /// 选定序号索引
        /// </summary>
        public int SelectedIndex
        {
            get => _SelectedIndex;
            set
            {
                int oldValue = _SelectedIndex;
                _SelectedIndex = value;

                OnSelectedIndexChanged(oldValue, value);
            }
        }
        private int _SelectedIndex;
        public event EventHandler<EventArgs> SelectedIndexChanged;
        public void OnSelectedIndexChanged(int oValue, int nValue)
        {
            if (SelectedIndex >= Globals.CompanionInfoList.Count) return;

            SelectedCompanionInfo = Globals.CompanionInfoList[SelectedIndex];

            IndexLabel.Text = $"{SelectedIndex + 1} of {Globals.CompanionInfoList.Count}";

            LeftButton.Enabled = SelectedIndex > 0;

            RightButton.Enabled = SelectedIndex < Globals.CompanionInfoList.Count - 1;

            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region AdoptAttempted
        /// <summary>
        /// 尝试购买
        /// </summary>
        public bool AdoptAttempted
        {
            get => _AdoptAttempted;
            set
            {
                if (_AdoptAttempted == value) return;

                bool oldValue = _AdoptAttempted;
                _AdoptAttempted = value;

                OnAdoptAttemptedChanged(oldValue, value);
            }
        }
        private bool _AdoptAttempted;
        public event EventHandler<EventArgs> AdoptAttemptedChanged;
        public void OnAdoptAttemptedChanged(bool oValue, bool nValue)
        {
            RefreshUnlockButton();
            AdoptAttemptedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region CompanionNameValid
        /// <summary>
        /// 判断宠物的名字是否有效
        /// </summary>
        public bool CompanionNameValid
        {
            get => _CompanionNameValid;
            set
            {
                if (_CompanionNameValid == value) return;

                bool oldValue = _CompanionNameValid;
                _CompanionNameValid = value;

                OnCompanionNameValidChanged(oldValue, value);
            }
        }
        private bool _CompanionNameValid;
        public event EventHandler<EventArgs> CompanionNameValidChanged;
        public void OnCompanionNameValidChanged(bool oValue, bool nValue)
        {
            RefreshUnlockButton();
            CompanionNameValidChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public bool CanAdopt => GameScene.Game.User != null && SelectedCompanionInfo != null && SelectedCompanionInfo.Price <= GameScene.Game.User.Gold && SelectedCompanionInfo.GameGoldPrice <= GameScene.Game.User.GameGold && !AdoptAttempted && !UnlockButton.Visible && CompanionNameValid;


        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// NPC宠物购买面板
        /// </summary>
        public NPCAdoptCompanionDialog()
        {
            TitleLabel.Text = "选择宠物".Lang();

            Movable = true;

            SetClientSize(new Size(275, 130));
            CompanionDisplayPoint = new Point(40, 95);

            NameLabel = new DXLabel     //宠物名字标签
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            NameLabel.SizeChanged += (o, e) =>
            {
                NameLabel.Location = new Point(CompanionDisplayPoint.X + 25 - NameLabel.Size.Width / 2, CompanionDisplayPoint.Y + 30);
            };

            IndexLabel = new DXLabel   //索引序号标签
            {
                Parent = this,
                Location = new Point(CompanionDisplayPoint.X, 200),
            };
            IndexLabel.SizeChanged += (o, e) =>
            {
                IndexLabel.Location = new Point(CompanionDisplayPoint.X + 25 - IndexLabel.Size.Width / 2, CompanionDisplayPoint.Y + 55);
            };

            LeftButton = new DXButton  //向左按钮
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 32,
                Location = new Point(CompanionDisplayPoint.X - 20, CompanionDisplayPoint.Y + 55)
            };
            LeftButton.MouseClick += (o, e) => SelectedIndex--;

            RightButton = new DXButton  //向右按钮
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 37,
                Location = new Point(CompanionDisplayPoint.X + 60, CompanionDisplayPoint.Y + 55)
            };
            RightButton.MouseClick += (o, e) => SelectedIndex++;

            DXLabel label = new DXLabel
            {
                Parent = this,
                Text = "价格".Lang()
            };
            label.Location = new Point(160 - label.Size.Width, CompanionDisplayPoint.Y);

            PriceLabel = new DXLabel   //金额标签
            {
                Parent = this,
                Location = new Point(160, CompanionDisplayPoint.Y),
                ForeColour = Color.White,
            };

            CompanionNameTextBox = new DXTextBox   //宠物名字输入框
            {
                Parent = this,
                Location = new Point(160, CompanionDisplayPoint.Y + 25),
                Size = new Size(120, 20)
            };
            CompanionNameTextBox.TextBox.TextChanged += TextBox_TextChanged;

            label = new DXLabel
            {
                Parent = this,
                Text = "名字".Lang()
            };
            label.Location = new Point(CompanionNameTextBox.Location.X - label.Size.Width, CompanionNameTextBox.Location.Y + (CompanionNameTextBox.Size.Height - label.Size.Height) / 2);

            AdoptButton = new DXButton
            {
                Parent = this,
                Location = new Point(CompanionNameTextBox.Location.X, CompanionNameTextBox.Location.Y + 27),
                Size = new Size(120, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "选择".Lang() }
            };
            AdoptButton.MouseClick += AdoptButton_MouseClick;

            UnlockButton = new DXButton     //解锁按钮
            {
                Parent = this,
                Location = new Point(ClientArea.Right - 80, ClientArea.Y),
                Size = new Size(80, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "解锁".Lang() }
            };

            UnlockButton.MouseClick += UnlockButton_MouseClick;

            SelectedIndex = 0;  //选定索引ID
        }

        #region Methods
        /// <summary>
        /// 文本输入改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            CompanionNameValid = Globals.CharacterReg.IsMatch(CompanionNameTextBox.TextBox.Text);

            if (string.IsNullOrEmpty(CompanionNameTextBox.TextBox.Text))
                CompanionNameTextBox.BorderColour = Color.FromArgb(198, 166, 99);
            else
                CompanionNameTextBox.BorderColour = CompanionNameValid ? Color.Green : Color.Red;
        }
        /// <summary>
        /// 鼠标点击选择按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdoptButton_MouseClick(object sender, MouseEventArgs e)
        {
            AdoptAttempted = true;

            CEnvir.Enqueue(new C.CompanionAdopt { Index = SelectedCompanionInfo.Index, Name = CompanionNameTextBox.TextBox.Text });
        }
        /// <summary>
        /// 鼠标点击解锁按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnlockButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (GameScene.Game.Inventory.All(x => x == null || x.Info.Effect != ItemEffect.CompanionTicket))   //如果 玩家背包库存为空 或者 道具不是宠物解锁劵
            {
                GameScene.Game.ReceiveChat("NPCDialog.CompanionUnlock".Lang(), MessageType.System);   //提示并跳过
                return;
            }

            DXMessageBox box = new DXMessageBox($"NPCDialog.CompanionTicket".Lang(SelectedCompanionInfo.MonsterInfo.MonsterName), "解锁宠物".Lang(), DXMessageBoxButtons.YesNo);

            box.YesButton.MouseClick += (o1, e1) =>
            {
                CEnvir.Enqueue(new C.CompanionUnlock { Index = SelectedCompanionInfo.Index });

                UnlockButton.Enabled = false;
            };
        }
        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            CompanionDisplay?.Process();
        }

        /// <summary>
        /// 在后面绘制
        /// </summary>
        protected override void OnAfterDraw()
        {
            base.OnAfterDraw();

            if (CompanionDisplay == null) return;

            int x = DisplayArea.X + CompanionDisplayPoint.X;
            int y = DisplayArea.Y + CompanionDisplayPoint.Y;

            if (CompanionDisplay.Image == MonsterImage.Companion_Donkey)
            {
                x += 10;
                y -= 5;
            }

            CompanionDisplay.DrawShadow(x, y);
            CompanionDisplay.DrawBody(x, y);
        }
        /// <summary>
        /// 刷新解锁按钮
        /// </summary>
        public void RefreshUnlockButton()
        {
            if (Globals.CompanionInfoList.Count == 0) return;
            UnlockButton.Visible = !SelectedCompanionInfo.Available && !AvailableCompanions.Contains(SelectedCompanionInfo);

            if (GameScene.Game.User == null || SelectedCompanionInfo == null || SelectedCompanionInfo.Price <= GameScene.Game.User.Gold || SelectedCompanionInfo.GameGoldPrice <= GameScene.Game.User.GameGold)
            {
                if (SelectedCompanionInfo.Price > 0)
                {
                    PriceLabel.ForeColour = Color.White;
                }
                else
                {
                    PriceLabel.ForeColour = Color.Yellow;
                }
            }
            else
                PriceLabel.ForeColour = Color.Red;

            AdoptButton.Enabled = CanAdopt;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                CompanionDisplay = null;
                CompanionDisplayPoint = Point.Empty;

                _SelectedCompanionInfo = null;
                SelectedCompanionInfoChanged = null;

                _SelectedIndex = 0;
                SelectedIndexChanged = null;

                _AdoptAttempted = false;
                AdoptAttemptedChanged = null;

                _CompanionNameValid = false;
                CompanionNameValidChanged = null;

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (IndexLabel != null)
                {
                    if (!IndexLabel.IsDisposed)
                        IndexLabel.Dispose();

                    IndexLabel = null;
                }

                if (PriceLabel != null)
                {
                    if (!PriceLabel.IsDisposed)
                        PriceLabel.Dispose();

                    PriceLabel = null;
                }

                if (LeftButton != null)
                {
                    if (!LeftButton.IsDisposed)
                        LeftButton.Dispose();

                    LeftButton = null;
                }

                if (RightButton != null)
                {
                    if (!RightButton.IsDisposed)
                        RightButton.Dispose();

                    RightButton = null;
                }

                if (AdoptButton != null)
                {
                    if (!AdoptButton.IsDisposed)
                        AdoptButton.Dispose();

                    AdoptButton = null;
                }

                if (UnlockButton != null)
                {
                    if (!UnlockButton.IsDisposed)
                        UnlockButton.Dispose();

                    UnlockButton = null;
                }

                if (CompanionNameTextBox != null)
                {
                    if (!CompanionNameTextBox.IsDisposed)
                        CompanionNameTextBox.Dispose();

                    CompanionNameTextBox = null;
                }
            }
        }
        #endregion
    }

    #endregion

    #region 宠物寄存

    /// <summary>
    /// 宠物寄存仓库
    /// </summary>
    public sealed class NPCCompanionStorageDialog : DXWindow
    {
        #region Properties

        private DXVScrollBar ScrollBar;

        public NPCCompanionStorageRow[] Rows;

        public List<ClientUserCompanion> Companions = new List<ClientUserCompanion>();

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// 宠物寄存仓库
        /// </summary>
        public NPCCompanionStorageDialog()
        {
            TitleLabel.Text = "仓库".Lang();

            Movable = false;

            SetClientSize(new Size(198, 349));

            Rows = new NPCCompanionStorageRow[4];

            for (int i = 0; i < Rows.Length; i++)
            {
                Rows[i] = new NPCCompanionStorageRow
                {
                    Parent = this,
                    Location = new Point(ClientArea.X, ClientArea.Y + i * 88),
                };
            }

            ScrollBar = new DXVScrollBar  //滚动条
            {
                Parent = this,
                Location = new Point(ClientArea.Right - 15, ClientArea.Y + 1),
                Size = new Size(14, Rows.Length * 87 - 1),
                VisibleSize = Rows.Length,
                Change = 1,
            };
            ScrollBar.ValueChanged += (o, e) => UpdateScrollBar();
        }

        #region Methods
        /// <summary>
        /// 更新滚动条
        /// </summary>
        public void UpdateScrollBar()
        {
            ScrollBar.MaxValue = Companions.Count;

            for (int i = 0; i < Rows.Length; i++)
            {
                Rows[i].UserCompanion = i + ScrollBar.Value >= Companions.Count ? null : Companions[i + ScrollBar.Value];
            }
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Companions.Clear();
                Companions = null;

                if (Rows != null)
                {
                    for (int i = 0; i < Rows.Length; i++)
                    {
                        if (Rows[i] != null)
                        {
                            if (!Rows[i].IsDisposed)
                                Rows[i].Dispose();

                            Rows[i] = null;
                        }

                    }

                    Rows = null;
                }

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }
            }
        }
        #endregion
    }

    #endregion

    #region  NPC宠物寄存取回

    /// <summary>
    /// NPC宠物寄存取回功能
    /// </summary>
    public sealed class NPCCompanionStorageRow : DXControl
    {
        #region Properties

        #region UserCompanion
        /// <summary>
        /// 角色宠物判断
        /// </summary>
        public ClientUserCompanion UserCompanion
        {
            get => _UserCompanion;
            set
            {
                ClientUserCompanion oldValue = _UserCompanion;
                _UserCompanion = value;

                OnUserCompanionChanged(oldValue, value);
            }
        }
        private ClientUserCompanion _UserCompanion;
        public event EventHandler<EventArgs> UserCompanionChanged;
        public void OnUserCompanionChanged(ClientUserCompanion oValue, ClientUserCompanion nValue)
        {
            UserCompanionChanged?.Invoke(this, EventArgs.Empty);

            if (UserCompanion == null)
            {
                Visible = false;
                return;
            }

            Visible = true;

            CompanionDisplay = new MonsterObject(UserCompanion.CompanionInfo);

            NameLabel.Text = UserCompanion.Name;
            LevelLabel.Text = $"等级".Lang() + $" {UserCompanion.Level}";

            if (UserCompanion == GameScene.Game.Companion)
                Selected = true;
            else
            {
                Selected = false;

                if (!string.IsNullOrEmpty(UserCompanion.CharacterName))
                {
                    RetrieveButton.Enabled = false;
                    RetrieveButton.Hint = $"你当前的宠物是".Lang() + $" {UserCompanion.CharacterName}.";
                }
                else
                {
                    RetrieveButton.Enabled = true;
                    RetrieveButton.Hint = null;
                }
            }
        }

        #endregion

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
            Border = Selected;
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);

            RetrieveButton.Visible = !Selected;
            StoreButton.Visible = Selected;


            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public MonsterObject CompanionDisplay;
        public Point CompanionDisplayPoint;
        public DXLabel NameLabel, LevelLabel;
        public DXButton StoreButton, RetrieveButton;

        #endregion

        /// <summary>
        /// NPC宠物寄存取回功能
        /// </summary>
        public NPCCompanionStorageRow()
        {
            DrawTexture = true;
            BackColour = Color.FromArgb(25, 20, 0);

            BorderColour = Color.FromArgb(198, 166, 99);
            Size = new Size(180, 85);
            CompanionDisplayPoint = new Point(10, 45);

            NameLabel = new DXLabel     //名字标签
            {
                Parent = this,
                Location = new Point(85, 5)
            };

            LevelLabel = new DXLabel    //等级标签
            {
                Parent = this,
                Location = new Point(85, 30)
            };

            StoreButton = new DXButton
            {
                Parent = this,
                Location = new Point(85, 60),
                Size = new Size(80, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "寄存".Lang() },
                Visible = false
            };
            StoreButton.MouseClick += StoreButton_MouseClick;

            RetrieveButton = new DXButton
            {
                Parent = this,
                Location = new Point(85, 60),
                Size = new Size(80, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "取回".Lang() }
            };
            RetrieveButton.MouseClick += RetrieveButton_MouseClick;
        }

        #region Methods
        /// <summary>
        /// 鼠标点击寄存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StoreButton_MouseClick(object sender, MouseEventArgs e)
        {
            CEnvir.Enqueue(new C.CompanionStore { Index = UserCompanion.Index });
        }
        /// <summary>
        /// 鼠标点击取回按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RetrieveButton_MouseClick(object sender, MouseEventArgs e)
        {
            CEnvir.Enqueue(new C.CompanionRetrieve { Index = UserCompanion.Index });
        }
        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            CompanionDisplay?.Process();
        }
        /// <summary>
        /// 在后面绘制
        /// </summary>
        protected override void OnAfterDraw()
        {
            base.OnAfterDraw();

            if (CompanionDisplay == null) return;

            int x = DisplayArea.X + CompanionDisplayPoint.X;
            int y = DisplayArea.Y + CompanionDisplayPoint.Y;

            if (CompanionDisplay.Image == MonsterImage.Companion_Donkey)
            {
                x += 10;
                y -= 5;
            }

            CompanionDisplay.DrawShadow(x, y);
            CompanionDisplay.DrawBody(x, y);
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _UserCompanion = null;
                UserCompanionChanged = null;

                _Selected = false;
                SelectedChanged = null;

                CompanionDisplay = null;
                CompanionDisplayPoint = Point.Empty;

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (LevelLabel != null)
                {
                    if (!LevelLabel.IsDisposed)
                        LevelLabel.Dispose();

                    LevelLabel = null;
                }

                if (StoreButton != null)
                {
                    if (!StoreButton.IsDisposed)
                        StoreButton.Dispose();

                    StoreButton = null;
                }

                if (RetrieveButton != null)
                {
                    if (!RetrieveButton.IsDisposed)
                        RetrieveButton.Dispose();

                    RetrieveButton = null;
                }
            }
        }
        #endregion
    }

    #endregion

    #region NPC结婚戒指

    /// <summary>
    /// NPC结婚戒指功能
    /// </summary>
    public sealed class NPCWeddingRingDialog : DXWindow
    {
        #region Properties

        public DXItemGrid RingGrid;
        public DXButton BindButton;

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// 结婚戒指功能面板
        /// </summary>
        public NPCWeddingRingDialog()
        {
            HasTitle = false;
            SetClientSize(new Size(60, 85));
            CloseButton.Visible = false;

            DXLabel label = new DXLabel
            {
                Text = "戒指".Lang(),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = ClientArea.Location,
                AutoSize = false,
                Size = new Size(ClientArea.Width, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            };
            RingGrid = new DXItemGrid
            {
                Parent = this,
                Location = new Point(ClientArea.X + (ClientArea.Width - 36) / 2, label.Size.Height + label.Location.Y + 5),
                GridSize = new Size(1, 1),
                Linked = true,
                GridType = GridType.WeddingRing,
            };

            RingGrid.Grid[0].LinkChanged += (o, e) => BindButton.Enabled = RingGrid.Grid[0].Item != null;
            RingGrid.Grid[0].BeforeDraw += (o, e) => Draw(RingGrid.Grid[0], 31);

            BindButton = new DXButton
            {
                Size = new Size(50, SmallButtonHeight),
                Location = new Point((ClientArea.Width - 50) / 2 + ClientArea.X, ClientArea.Bottom - SmallButtonHeight),
                Label = { Text = "绑定".Lang() },
                Parent = this,
                ButtonType = ButtonType.SmallButton,
                Enabled = false,
            };
            BindButton.MouseClick += (o, e) =>
            {
                if (RingGrid.Grid[0].Item == null || RingGrid.Grid[0].Item.Info.ItemType != ItemType.Ring) return;


                CEnvir.Enqueue(new C.MarriageMakeRing { Slot = RingGrid.Grid[0].Link.Slot });

                RingGrid.Grid[0].Link = null;
            };
        }

        #region Methods
        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void Draw(DXItemCell cell, int index)
        {
            if (InterfaceLibrary == null) return;

            if (cell.Item != null) return;

            Size s = InterfaceLibrary.GetSize(index);
            int x = (cell.Size.Width - s.Width) / 2 + cell.DisplayArea.X;
            int y = (cell.Size.Height - s.Height) / 2 + cell.DisplayArea.Y;

            InterfaceLibrary.Draw(index, x, y, Color.White, false, 0.2F, ImageType.Image);
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (RingGrid != null)
                {
                    if (!RingGrid.IsDisposed)
                        RingGrid.Dispose();

                    RingGrid = null;
                }

                if (BindButton != null)
                {
                    if (!BindButton.IsDisposed)
                        BindButton.Dispose();

                    BindButton = null;
                }
            }
        }
        #endregion
    }

    #endregion

    #region NPC精炼石合成

    /// <summary>
    /// NPC精炼石合成功能
    /// </summary>
    public sealed class NPCRefinementStoneDialog : DXWindow
    {
        #region Properties

        public DXItemGrid IronOreGrid, GoldOreGrid, DiamondGrid, SilverOreGrid, CrystalGrid;
        public DXNumberTextBox GoldBox;

        public DXButton SubmitButton;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
                GameScene.Game.InventoryBox.Visible = true;

            if (!IsVisible)
            {
                GoldOreGrid.ClearLinks();
                DiamondGrid.ClearLinks();
                SilverOreGrid.ClearLinks();
                IronOreGrid.ClearLinks();
                CrystalGrid.ClearLinks();

                GoldBox.Value = 0;
            }
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// NPC精炼石合成面板
        /// </summary>
        public NPCRefinementStoneDialog()
        {
            TitleLabel.Text = "制炼石".Lang();

            SetClientSize(new Size(491, 130));

            DXLabel label = new DXLabel
            {
                Text = "铁矿".Lang(),
                Location = new Point(ClientArea.X + 21, ClientArea.Y),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };
            IronOreGrid = new DXItemGrid  //铁矿格子 4个
            {
                GridSize = new Size(4, 1),
                Parent = this,
                GridType = GridType.RefinementStoneIronOre,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };

            label = new DXLabel
            {
                Text = "银矿".Lang(),
                Location = new Point(IronOreGrid.Size.Width + 5 + IronOreGrid.Location.X, label.Location.Y),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };
            SilverOreGrid = new DXItemGrid   //银矿格子 4个
            {
                GridSize = new Size(4, 1),
                Parent = this,
                GridType = GridType.RefinementStoneSilverOre,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };

            label = new DXLabel
            {
                Text = "金刚石".Lang(),
                Location = new Point(SilverOreGrid.Size.Width + 5 + SilverOreGrid.Location.X, label.Location.Y),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };
            DiamondGrid = new DXItemGrid   //金刚石格子 4个
            {
                GridSize = new Size(4, 1),
                Parent = this,
                GridType = GridType.RefinementStoneDiamond,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };

            label = new DXLabel
            {
                Text = "金矿".Lang(),
                Location = new Point(ClientArea.X + 21, IronOreGrid.Location.Y + IronOreGrid.Size.Height + 10),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };
            GoldOreGrid = new DXItemGrid   //金矿格子 2个
            {
                GridSize = new Size(2, 1),
                Parent = this,
                GridType = GridType.RefinementStoneGoldOre,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };

            label = new DXLabel
            {
                Text = "结晶".Lang(),
                Location = new Point(IronOreGrid.Size.Width + 5 + IronOreGrid.Location.X, IronOreGrid.Location.Y + IronOreGrid.Size.Height + 10),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };
            CrystalGrid = new DXItemGrid     //结晶格子
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.RefinementStoneCrystal,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };

            label = new DXLabel
            {
                Text = "金币".Lang(),
                Location = new Point(SilverOreGrid.Size.Width + 5 + SilverOreGrid.Location.X, SilverOreGrid.Location.Y + SilverOreGrid.Size.Height + 10),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };
            GoldBox = new DXNumberTextBox
            {
                Location = new Point(label.Location.X + 6, label.Location.Y + label.Size.Height + 5),
                Parent = this,
                MaxValue = 2000000000,
                Size = new Size(36 * 4 - 5, 16)
            };

            foreach (DXItemCell cell in IronOreGrid.Grid)   //遍历格子
            {
                cell.LinkChanged += (o, e) => UpdateButton();   //链接变化
            }
            foreach (DXItemCell cell in SilverOreGrid.Grid)
            {
                cell.LinkChanged += (o, e) => UpdateButton();
            }
            foreach (DXItemCell cell in DiamondGrid.Grid)
            {
                cell.LinkChanged += (o, e) => UpdateButton();
            }
            foreach (DXItemCell cell in GoldOreGrid.Grid)
            {
                cell.LinkChanged += (o, e) => UpdateButton();
            }
            foreach (DXItemCell cell in CrystalGrid.Grid)
            {
                cell.LinkChanged += (o, e) => UpdateButton();
            }

            GoldBox.ValueChanged += (o, e) => UpdateButton();

            SubmitButton = new DXButton
            {
                Label = { Text = "提交".Lang() },
                Size = new Size(80, SmallButtonHeight),
                Parent = this,
                Enabled = false,
                ButtonType = ButtonType.SmallButton,
                Location = new Point(GoldBox.Location.X + GoldBox.Size.Width - 78, GoldBox.Location.Y + GoldBox.Size.Height + 5),
            };
            SubmitButton.MouseClick += (o, e) =>   //鼠标点击
            {
                if (GameScene.Game.Observer) return;

                List<CellLinkInfo> iron = new List<CellLinkInfo>();
                foreach (DXItemCell cell in IronOreGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    iron.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }
                if (iron.Count < 4)
                {
                    GameScene.Game.ReceiveChat("你需要4个铁矿来制造一块制炼石".Lang(), MessageType.System);
                    return;
                }

                List<CellLinkInfo> silver = new List<CellLinkInfo>();
                foreach (DXItemCell cell in SilverOreGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    silver.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }
                if (silver.Count < 4)
                {
                    GameScene.Game.ReceiveChat("你需要4个银矿来制造一块制炼石".Lang(), MessageType.System);
                    return;
                }

                List<CellLinkInfo> diamond = new List<CellLinkInfo>();
                foreach (DXItemCell cell in DiamondGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    diamond.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }
                if (diamond.Count < 4)
                {
                    GameScene.Game.ReceiveChat("你需要4个金刚石来制造一块制炼石".Lang(), MessageType.System);
                    return;
                }

                List<CellLinkInfo> gold = new List<CellLinkInfo>();
                foreach (DXItemCell cell in GoldOreGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    gold.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }
                if (gold.Count < 2)
                {
                    GameScene.Game.ReceiveChat("你需要2个金矿来制造一块制炼石".Lang(), MessageType.System);
                    return;
                }

                List<CellLinkInfo> crystal = new List<CellLinkInfo>();
                foreach (DXItemCell cell in CrystalGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    crystal.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }
                if (crystal.Count < 1)
                {
                    GameScene.Game.ReceiveChat("你需要1个结晶来制作一块制炼石".Lang(), MessageType.System);
                    return;
                }

                if (GoldBox.Value > GameScene.Game.User.Gold)
                {
                    GameScene.Game.ReceiveChat("金币不足".Lang(), MessageType.System);
                    return;
                }

                //发包精炼石合成
                CEnvir.Enqueue(new C.NPCRefinementStone { IronOres = iron, SilverOres = silver, DiamondOres = diamond, GoldOres = gold, Crystal = crystal, Gold = GoldBox.Value });

                GoldBox.Value = 0;
            };
        }

        #region Methods
        /// <summary>
        /// 更新按钮
        /// </summary>
        public void UpdateButton()
        {
            SubmitButton.Enabled = false;

            if (GoldBox.Value > GameScene.Game.User.Gold)
            {
                GoldBox.BorderColour = Color.Red;
                return;
            }
            GoldBox.BorderColour = Color.FromArgb(198, 166, 99);

            foreach (DXItemCell cell in IronOreGrid.Grid)
            {
                if (cell.Link == null) return;
            }
            foreach (DXItemCell cell in SilverOreGrid.Grid)
            {
                if (cell.Link == null) return;
            }
            foreach (DXItemCell cell in DiamondGrid.Grid)
            {
                if (cell.Link == null) return;
            }
            foreach (DXItemCell cell in GoldOreGrid.Grid)
            {
                if (cell.Link == null) return;
            }
            foreach (DXItemCell cell in CrystalGrid.Grid)
            {
                if (cell.Link == null) return;
            }

            SubmitButton.Enabled = true;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (IronOreGrid != null)
                {
                    if (!IronOreGrid.IsDisposed)
                        IronOreGrid.Dispose();

                    IronOreGrid = null;
                }

                if (SilverOreGrid != null)
                {
                    if (!SilverOreGrid.IsDisposed)
                        SilverOreGrid.Dispose();

                    SilverOreGrid = null;
                }

                if (DiamondGrid != null)
                {
                    if (!DiamondGrid.IsDisposed)
                        DiamondGrid.Dispose();

                    DiamondGrid = null;
                }

                if (GoldOreGrid != null)
                {
                    if (!GoldOreGrid.IsDisposed)
                        GoldOreGrid.Dispose();

                    GoldOreGrid = null;
                }

                if (CrystalGrid != null)
                {
                    if (!CrystalGrid.IsDisposed)
                        CrystalGrid.Dispose();

                    CrystalGrid = null;
                }

                if (GoldBox != null)
                {
                    if (!GoldBox.IsDisposed)
                        GoldBox.Dispose();

                    GoldBox = null;
                }

                if (SubmitButton != null)
                {
                    if (!SubmitButton.IsDisposed)
                        SubmitButton.Dispose();

                    SubmitButton = null;
                }
            }
        }
        #endregion
    }

    #endregion

    #region NPC分解碎片

    /// <summary>
    /// NPC分解碎片
    /// </summary>
    public sealed class NPCItemFragmentDialog : DXWindow
    {
        #region Properties

        public DXItemGrid Grid;
        public DXButton FragmentButton;
        public DXLabel CostLabel;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
                GameScene.Game.InventoryBox.Visible = true;

            if (!IsVisible)
                Grid.ClearLinks();
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// NPC分解物品界面
        /// </summary>
        public NPCItemFragmentDialog()
        {
            TitleLabel.Text = "分解物品".Lang();

            Grid = new DXItemGrid
            {
                GridSize = new Size(7, 3),
                Parent = this,
                GridType = GridType.ItemFragment,
                Linked = true
            };

            Movable = false;
            SetClientSize(new Size(Grid.Size.Width, Grid.Size.Height + 50));
            Grid.Location = ClientArea.Location;

            foreach (DXItemCell cell in Grid.Grid)
                cell.LinkChanged += (o, e) => CalculateCost();

            CostLabel = new DXLabel   //金额标签
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left + 80, ClientArea.Bottom - 45),
                Text = "0",
                Size = new Size(ClientArea.Width - 80, 20),
                Sound = SoundIndex.GoldPickUp
            };

            new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left, ClientArea.Bottom - 45),
                Text = "分解成本".Lang(),
                Size = new Size(79, 20),
                IsControl = false,
            };

            DXButton selectAll = new DXButton
            {
                Label = { Text = "全选".Lang() },
                Location = new Point(ClientArea.X, CostLabel.Location.Y + CostLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight)
            };
            selectAll.MouseClick += (o, e) =>
            {
                foreach (DXItemCell cell in GameScene.Game.InventoryBox.Grid.Grid)
                {
                    if (!cell.CheckLink(Grid)) continue;

                    cell.MoveItem(Grid, true);
                }
            };

            FragmentButton = new DXButton
            {
                Label = { Text = "分解".Lang() },
                Location = new Point(ClientArea.Right - 80, CostLabel.Location.Y + CostLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight),
                Enabled = false,
            };
            FragmentButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                List<CellLinkInfo> links = new List<CellLinkInfo>();

                foreach (DXItemCell cell in Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    links.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }

                CEnvir.Enqueue(new C.NPCFragment { Links = links });
            };
        }

        #region Methods
        /// <summary>
        /// 计算分解成本
        /// </summary>
        private void CalculateCost()
        {
            int sum = 0;
            int count = 0;
            foreach (DXItemCell cell in Grid.Grid)
            {
                if (cell.Link?.Item == null) continue;

                sum += cell.Link.Item.FragmentCost();
                count++;
            }

            CostLabel.ForeColour = sum > MapObject.User.Gold ? Color.Red : Color.White;

            CostLabel.Text = sum.ToString("#,##0");

            FragmentButton.Enabled = sum <= MapObject.User.Gold && count > 0;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                if (FragmentButton != null)
                {
                    if (!FragmentButton.IsDisposed)
                        FragmentButton.Dispose();

                    FragmentButton = null;
                }

                if (CostLabel != null)
                {
                    if (!CostLabel.IsDisposed)
                        CostLabel.Dispose();

                    CostLabel = null;
                }
            }
        }
        #endregion
    }

    #endregion

    #region NPC大师精炼

    /// <summary>
    /// NPC大师精炼功能
    /// </summary>
    public sealed class NPCMasterRefineDialog : DXWindow
    {
        #region Properties

        #region RefineType
        /// <summary>
        /// 精炼类型
        /// </summary>
        public RefineType RefineType
        {
            get => _RefineType;
            set
            {
                if (_RefineType == value) return;

                RefineType oldValue = _RefineType;
                _RefineType = value;

                OnRefineTypeChanged(oldValue, value);
            }
        }
        private RefineType _RefineType;
        public event EventHandler<EventArgs> RefineTypeChanged;
        public void OnRefineTypeChanged(RefineType oValue, RefineType nValue)
        {
            switch (oValue)
            {
                case RefineType.None:
                    SubmitButton.Enabled = true;
                    EvaluateButton.Enabled = true;
                    break;
                case RefineType.DC:
                    DCCheckBox.Checked = false;
                    break;
                case RefineType.SpellPower:
                    SPCheckBox.Checked = false;
                    break;
                case RefineType.Fire:
                    FireCheckBox.Checked = false;
                    break;
                case RefineType.Ice:
                    IceCheckBox.Checked = false;
                    break;
                case RefineType.Lightning:
                    LightningCheckBox.Checked = false;
                    break;
                case RefineType.Wind:
                    WindCheckBox.Checked = false;
                    break;
                case RefineType.Holy:
                    HolyCheckBox.Checked = false;
                    break;
                case RefineType.Dark:
                    DarkCheckBox.Checked = false;
                    break;
                case RefineType.Phantom:
                    PhantomCheckBox.Checked = false;
                    break;
            }

            switch (nValue)
            {
                case RefineType.None:
                    SubmitButton.Enabled = false;
                    EvaluateButton.Enabled = false;
                    break;
                case RefineType.DC:
                    DCCheckBox.Checked = true;
                    break;
                case RefineType.SpellPower:
                    SPCheckBox.Checked = true;
                    break;
                case RefineType.Fire:
                    FireCheckBox.Checked = true;
                    break;
                case RefineType.Ice:
                    IceCheckBox.Checked = true;
                    break;
                case RefineType.Lightning:
                    LightningCheckBox.Checked = true;
                    break;
                case RefineType.Wind:
                    WindCheckBox.Checked = true;
                    break;
                case RefineType.Holy:
                    HolyCheckBox.Checked = true;
                    break;
                case RefineType.Dark:
                    DarkCheckBox.Checked = true;
                    break;
                case RefineType.Phantom:
                    PhantomCheckBox.Checked = true;
                    break;
            }

            RefineTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXItemGrid Fragment1Grid, Fragment2Grid, Fragment3Grid, RefinementStoneGrid, SpecialGrid;

        public DXCheckBox DCCheckBox, SPCheckBox, FireCheckBox, IceCheckBox, LightningCheckBox, WindCheckBox, HolyCheckBox, DarkCheckBox, PhantomCheckBox;
        public DXButton SubmitButton, EvaluateButton;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
                GameScene.Game.InventoryBox.Visible = true;

            if (!IsVisible)
            {
                Fragment1Grid.ClearLinks();
                Fragment2Grid.ClearLinks();
                Fragment3Grid.ClearLinks();
                RefinementStoneGrid.ClearLinks();
                SpecialGrid.ClearLinks();
            }
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// NPC大师精炼面板
        /// </summary>
        public NPCMasterRefineDialog()
        {
            TitleLabel.Text = "大师精炼".Lang();

            SetClientSize(new Size(491, 130));

            DXLabel label = new DXLabel
            {
                Text = "碎片I".Lang(),
                Location = ClientArea.Location,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };
            Fragment1Grid = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.MasterRefineFragment1,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };

            label = new DXLabel
            {
                Text = "碎片II".Lang(),
                Location = new Point(label.Size.Width + 5 + label.Location.X, label.Location.Y),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };
            Fragment2Grid = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.MasterRefineFragment2,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };

            label = new DXLabel
            {
                Text = "碎片III".Lang(),
                Location = new Point(label.Size.Width + 5 + label.Location.X, label.Location.Y),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };
            Fragment3Grid = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.MasterRefineFragment3,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };

            label = new DXLabel
            {
                Text = "制炼石".Lang(),
                Location = new Point(ClientArea.Location.X, Fragment3Grid.Location.Y + Fragment3Grid.Size.Height + 10),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };
            RefinementStoneGrid = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.MasterRefineStone,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };

            label = new DXLabel
            {
                Text = "特别".Lang(),
                Location = new Point(Fragment3Grid.Location.X - 5, label.Location.Y),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };
            SpecialGrid = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.MasterRefineSpecial,
                Linked = true,
                Location = new Point(label.Location.X + 5, label.Location.Y + label.Size.Height + 5)
            };

            SetClientSize(new Size(491, SpecialGrid.Location.Y + SpecialGrid.Size.Height - ClientArea.Y + 2));

            DCCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "破坏".Lang() },
                ReadOnly = true,
            };
            DCCheckBox.MouseClick += (o, e) => RefineType = RefineType.DC;
            SPCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "全系列魔法".Lang() },
                ReadOnly = true,
            };
            SPCheckBox.MouseClick += (o, e) => RefineType = RefineType.SpellPower;

            FireCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "火".Lang() },
                ReadOnly = true,
            };
            FireCheckBox.MouseClick += (o, e) => RefineType = RefineType.Fire;

            IceCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "冰".Lang() },
                ReadOnly = true,
            };
            IceCheckBox.MouseClick += (o, e) => RefineType = RefineType.Ice;

            LightningCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "雷".Lang() },
                ReadOnly = true,
            };
            LightningCheckBox.MouseClick += (o, e) => RefineType = RefineType.Lightning;

            WindCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "风".Lang() },
                ReadOnly = true,
            };
            WindCheckBox.MouseClick += (o, e) => RefineType = RefineType.Wind;

            HolyCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "神圣".Lang() },
                ReadOnly = true,
            };
            HolyCheckBox.MouseClick += (o, e) => RefineType = RefineType.Holy;

            DarkCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "暗黑".Lang() },
                ReadOnly = true,
            };
            DarkCheckBox.MouseClick += (o, e) => RefineType = RefineType.Dark;

            PhantomCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "幻影".Lang() },
                ReadOnly = true,
            };
            PhantomCheckBox.MouseClick += (o, e) => RefineType = RefineType.Phantom;

            DCCheckBox.Location = new Point(ClientArea.Right - DCCheckBox.Size.Width - 240, ClientArea.Y + 50);
            SPCheckBox.Location = new Point(ClientArea.Right - SPCheckBox.Size.Width - 156, ClientArea.Y + 50);

            FireCheckBox.Location = new Point(ClientArea.Right - FireCheckBox.Size.Width - 240, ClientArea.Y + 73);
            IceCheckBox.Location = new Point(ClientArea.Right - IceCheckBox.Size.Width - 156, ClientArea.Y + 73);
            LightningCheckBox.Location = new Point(ClientArea.Right - LightningCheckBox.Size.Width - 81, ClientArea.Y + 73);
            WindCheckBox.Location = new Point(ClientArea.Right - WindCheckBox.Size.Width - 5, ClientArea.Y + 73);
            HolyCheckBox.Location = new Point(ClientArea.Right - HolyCheckBox.Size.Width - 240, ClientArea.Y + 90);
            DarkCheckBox.Location = new Point(ClientArea.Right - DarkCheckBox.Size.Width - 156, ClientArea.Y + 90);
            PhantomCheckBox.Location = new Point(ClientArea.Right - PhantomCheckBox.Size.Width - 240, ClientArea.Y + 107);

            EvaluateButton = new DXButton
            {
                Label = { Text = "评估".Lang() },
                Size = new Size(80, SmallButtonHeight),
                Parent = this,
                ButtonType = ButtonType.SmallButton,
                Enabled = false,
            };
            EvaluateButton.Location = new Point(ClientArea.Right - EvaluateButton.Size.Width, ClientArea.Top + EvaluateButton.Size.Height);
            EvaluateButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                List<CellLinkInfo> frag1 = new List<CellLinkInfo>();
                List<CellLinkInfo> frag2 = new List<CellLinkInfo>();
                List<CellLinkInfo> frag3 = new List<CellLinkInfo>();
                List<CellLinkInfo> stone = new List<CellLinkInfo>();
                List<CellLinkInfo> special = new List<CellLinkInfo>();

                foreach (DXItemCell cell in Fragment1Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    frag1.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });
                }
                foreach (DXItemCell cell in Fragment2Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    frag2.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });
                }
                foreach (DXItemCell cell in Fragment3Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    frag3.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });
                }
                foreach (DXItemCell cell in RefinementStoneGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    stone.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });
                }
                foreach (DXItemCell cell in SpecialGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    special.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });
                }

                if (frag1.Count < 1 || frag1[0].Count != 10)
                {
                    GameScene.Game.ReceiveChat("你需要碎片(I) x10 交给锻炼大师".Lang(), MessageType.System);
                    return;
                }

                if (frag2.Count < 1 || frag2[0].Count != 10)
                {
                    GameScene.Game.ReceiveChat("你需要碎片(II) x10 交给精练大师".Lang(), MessageType.System);
                    return;
                }

                if (frag3.Count < 1)
                {
                    GameScene.Game.ReceiveChat("你需要碎片(III) 交给精练大师".Lang(), MessageType.System);
                    return;
                }

                if (stone.Count < 1)
                {
                    GameScene.Game.ReceiveChat("你需要制炼石x1来进行精炼".Lang(), MessageType.System);
                    return;
                }

                DXMessageBox box = new DXMessageBox("你确定要支付评估费用吗".Lang(), "评估".Lang(), DXMessageBoxButtons.YesNo);

                box.YesButton.MouseClick += (o1, e1) => CEnvir.Enqueue(new C.NPCMasterRefineEvaluate { RefineType = RefineType, Fragment1s = frag1, Fragment2s = frag2, Fragment3s = frag3, Stones = stone, Specials = special });
            };

            label = new DXLabel
            {
                Text = $"费用".Lang() + $": {Globals.MasterRefineEvaluateCost:#,##0}",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.Right - label.Size.Width, EvaluateButton.Location.Y + EvaluateButton.Size.Height + 5);

            SubmitButton = new DXButton
            {
                Label = { Text = "提交".Lang() },
                Size = new Size(80, SmallButtonHeight),
                Parent = this,
                ButtonType = ButtonType.SmallButton,
                Enabled = false,
            };
            SubmitButton.Location = new Point(ClientArea.Right - SubmitButton.Size.Width, ClientArea.Bottom - SubmitButton.Size.Height);
            SubmitButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                List<CellLinkInfo> frag1 = new List<CellLinkInfo>();
                List<CellLinkInfo> frag2 = new List<CellLinkInfo>();
                List<CellLinkInfo> frag3 = new List<CellLinkInfo>();
                List<CellLinkInfo> stone = new List<CellLinkInfo>();
                List<CellLinkInfo> special = new List<CellLinkInfo>();

                foreach (DXItemCell cell in Fragment1Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    frag1.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }
                foreach (DXItemCell cell in Fragment2Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    frag2.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }
                foreach (DXItemCell cell in Fragment3Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    frag3.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }
                foreach (DXItemCell cell in RefinementStoneGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    stone.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }
                foreach (DXItemCell cell in SpecialGrid.Grid)
                {
                    if (cell.Link == null) continue;

                    special.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }

                if (frag1.Count < 1 || frag1[0].Count != 10)
                {
                    GameScene.Game.ReceiveChat("你需要碎片(I) x10 来大师精炼".Lang(), MessageType.System);
                    return;
                }

                if (frag2.Count < 1 || frag2[0].Count != 10)
                {
                    GameScene.Game.ReceiveChat("你需要碎片(II) x10 来大师精炼".Lang(), MessageType.System);
                    return;
                }

                if (frag3.Count < 1)
                {
                    GameScene.Game.ReceiveChat("你需要碎片(III) x1 来大师精炼".Lang(), MessageType.System);
                    return;
                }

                if (stone.Count < 1)
                {
                    GameScene.Game.ReceiveChat("你需要制炼石 x1 来大师精炼".Lang(), MessageType.System);
                    return;
                }

                CEnvir.Enqueue(new C.NPCMasterRefine { RefineType = RefineType, Fragment1s = frag1, Fragment2s = frag2, Fragment3s = frag3, Stones = stone, Specials = special });
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _RefineType = 0;
                RefineTypeChanged = null;

                if (Fragment1Grid != null)
                {
                    if (!Fragment1Grid.IsDisposed)
                        Fragment1Grid.Dispose();

                    Fragment1Grid = null;
                }

                if (Fragment2Grid != null)
                {
                    if (!Fragment2Grid.IsDisposed)
                        Fragment2Grid.Dispose();

                    Fragment2Grid = null;
                }

                if (Fragment3Grid != null)
                {
                    if (!Fragment3Grid.IsDisposed)
                        Fragment3Grid.Dispose();

                    Fragment3Grid = null;
                }

                if (RefinementStoneGrid != null)
                {
                    if (!RefinementStoneGrid.IsDisposed)
                        RefinementStoneGrid.Dispose();

                    RefinementStoneGrid = null;
                }

                if (SpecialGrid != null)
                {
                    if (!SpecialGrid.IsDisposed)
                        SpecialGrid.Dispose();

                    SpecialGrid = null;
                }

                if (DCCheckBox != null)
                {
                    if (!DCCheckBox.IsDisposed)
                        DCCheckBox.Dispose();

                    DCCheckBox = null;
                }

                if (SPCheckBox != null)
                {
                    if (!SPCheckBox.IsDisposed)
                        SPCheckBox.Dispose();

                    SPCheckBox = null;
                }

                if (FireCheckBox != null)
                {
                    if (!FireCheckBox.IsDisposed)
                        FireCheckBox.Dispose();

                    FireCheckBox = null;
                }

                if (IceCheckBox != null)
                {
                    if (!IceCheckBox.IsDisposed)
                        IceCheckBox.Dispose();

                    IceCheckBox = null;
                }

                if (LightningCheckBox != null)
                {
                    if (!LightningCheckBox.IsDisposed)
                        LightningCheckBox.Dispose();

                    LightningCheckBox = null;
                }

                if (WindCheckBox != null)
                {
                    if (!WindCheckBox.IsDisposed)
                        WindCheckBox.Dispose();

                    WindCheckBox = null;
                }

                if (HolyCheckBox != null)
                {
                    if (!HolyCheckBox.IsDisposed)
                        HolyCheckBox.Dispose();

                    HolyCheckBox = null;
                }

                if (DarkCheckBox != null)
                {
                    if (!DarkCheckBox.IsDisposed)
                        DarkCheckBox.Dispose();

                    DarkCheckBox = null;
                }

                if (PhantomCheckBox != null)
                {
                    if (!PhantomCheckBox.IsDisposed)
                        PhantomCheckBox.Dispose();

                    PhantomCheckBox = null;
                }

                if (SubmitButton != null)
                {
                    if (!SubmitButton.IsDisposed)
                        SubmitButton.Dispose();

                    SubmitButton = null;
                }
            }
        }
        #endregion
    }

    #endregion

    #region NPC首饰升级

    /// <summary>
    /// NPC首饰升级功能
    /// </summary>
    public sealed class NPCAccessoryUpgradeDialog : DXWindow
    {
        #region Properties

        #region RefineType
        /// <summary>
        /// 精炼类型
        /// </summary>
        public RefineType RefineType
        {
            get => _RefineType;
            set
            {
                if (_RefineType == value) return;

                RefineType oldValue = _RefineType;
                _RefineType = value;

                OnRefineTypeChanged(oldValue, value);
            }
        }
        private RefineType _RefineType;
        public event EventHandler<EventArgs> RefineTypeChanged;
        public void OnRefineTypeChanged(RefineType oValue, RefineType nValue)
        {
            switch (oValue)
            {
                case RefineType.None:
                    SubmitButton.Enabled = true;
                    break;
                case RefineType.DC:
                    DCCheckBox.Checked = false;
                    break;
                case RefineType.SpellPower:
                    SPCheckBox.Checked = false;
                    break;
                case RefineType.Fire:
                    FireCheckBox.Checked = false;
                    break;
                case RefineType.Ice:
                    IceCheckBox.Checked = false;
                    break;
                case RefineType.Lightning:
                    LightningCheckBox.Checked = false;
                    break;
                case RefineType.Wind:
                    WindCheckBox.Checked = false;
                    break;
                case RefineType.Holy:
                    HolyCheckBox.Checked = false;
                    break;
                case RefineType.Dark:
                    DarkCheckBox.Checked = false;
                    break;
                case RefineType.Phantom:
                    PhantomCheckBox.Checked = false;
                    break;
                case RefineType.Health:
                    HealthCheckBox.Checked = false;
                    break;
                case RefineType.Mana:
                    ManaCheckBox.Checked = false;
                    break;
                case RefineType.AC:
                    ACCheckBox.Checked = false;
                    break;
                case RefineType.MR:
                    MRCheckBox.Checked = false;
                    break;
                case RefineType.Accuracy:
                    AccuracyCheckBox.Checked = false;
                    break;
                case RefineType.Agility:
                    AgilityCheckBox.Checked = false;
                    break;
                case RefineType.HealthPercent:
                    HealthPercentCheckBox.Checked = false;
                    break;
                case RefineType.ManaPercent:
                    ManaPercentCheckBox.Checked = false;
                    break;
                case RefineType.DCPercent:
                    DCPercentCheckBox.Checked = false;
                    break;
                case RefineType.SPPercent:
                    SPPercentCheckBox.Checked = false;
                    break;
            }

            switch (nValue)
            {
                case RefineType.None:
                    SubmitButton.Enabled = false;
                    break;
                case RefineType.DC:
                    DCCheckBox.Checked = true;
                    break;
                case RefineType.SpellPower:
                    SPCheckBox.Checked = true;
                    break;
                case RefineType.Fire:
                    FireCheckBox.Checked = true;
                    break;
                case RefineType.Ice:
                    IceCheckBox.Checked = true;
                    break;
                case RefineType.Lightning:
                    LightningCheckBox.Checked = true;
                    break;
                case RefineType.Wind:
                    WindCheckBox.Checked = true;
                    break;
                case RefineType.Holy:
                    HolyCheckBox.Checked = true;
                    break;
                case RefineType.Dark:
                    DarkCheckBox.Checked = true;
                    break;
                case RefineType.Phantom:
                    PhantomCheckBox.Checked = true;
                    break;
                case RefineType.Health:
                    HealthCheckBox.Checked = true;
                    break;
                case RefineType.Mana:
                    ManaCheckBox.Checked = true;
                    break;
                case RefineType.AC:
                    ACCheckBox.Checked = true;
                    break;
                case RefineType.MR:
                    MRCheckBox.Checked = true;
                    break;
                case RefineType.Accuracy:
                    AccuracyCheckBox.Checked = true;
                    break;
                case RefineType.Agility:
                    AgilityCheckBox.Checked = true;
                    break;
                case RefineType.HealthPercent:
                    HealthPercentCheckBox.Checked = true;
                    break;
                case RefineType.ManaPercent:
                    ManaPercentCheckBox.Checked = true;
                    break;
                case RefineType.DCPercent:
                    DCPercentCheckBox.Checked = true;
                    break;
                case RefineType.SPPercent:
                    SPPercentCheckBox.Checked = true;
                    break;
            }

            RefineTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXItemGrid TargetCell;

        public DXCheckBox DCPercentCheckBox, SPPercentCheckBox, HealthPercentCheckBox, ManaPercentCheckBox,
                          FireCheckBox, IceCheckBox, LightningCheckBox, WindCheckBox, HolyCheckBox, DarkCheckBox, PhantomCheckBox,
                          DCCheckBox, SPCheckBox, HealthCheckBox, ManaCheckBox,
                          ACCheckBox, MRCheckBox, AccuracyCheckBox, AgilityCheckBox;

        public DXButton SubmitButton;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
                GameScene.Game.InventoryBox.Visible = true;

            if (!IsVisible)
                TargetCell.ClearLinks();

        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// NPC首饰升级面板
        /// </summary>
        public NPCAccessoryUpgradeDialog()
        {
            TitleLabel.Text = "首饰升级".Lang();

            SetClientSize(new Size(491, 130));
            Movable = false;

            DXLabel label = new DXLabel
            {
                Text = "首饰".Lang(),
                Location = new Point(ClientArea.X + 65, ClientArea.Y + 15),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };
            TargetCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.AccessoryRefineUpgradeTarget,
                Linked = true,
                Location = new Point(label.Location.X - 3, label.Location.Y + label.Size.Height + 5)
            };

            DCPercentCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "破坏".Lang() + " 1%" },
                ReadOnly = true,
            };
            DCPercentCheckBox.MouseClick += (o, e) => RefineType = RefineType.DCPercent;

            SPPercentCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "全系列魔法".Lang() + " 1%" },
                ReadOnly = true,
            };
            SPPercentCheckBox.MouseClick += (o, e) => RefineType = RefineType.SPPercent;

            HealthPercentCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "生命值".Lang() + " 1%" },
                ReadOnly = true,
            };
            HealthPercentCheckBox.MouseClick += (o, e) => RefineType = RefineType.HealthPercent;

            ManaPercentCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "魔法值".Lang() + " 1%" },
                ReadOnly = true,
            };
            ManaPercentCheckBox.MouseClick += (o, e) => RefineType = RefineType.ManaPercent;

            DCCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "破坏".Lang() + " 0-1" },
                ReadOnly = true,
            };
            DCCheckBox.MouseClick += (o, e) => RefineType = RefineType.DC;

            SPCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "全系列魔法".Lang() + " 0-1" },
                ReadOnly = true,
            };
            SPCheckBox.MouseClick += (o, e) => RefineType = RefineType.SpellPower;

            FireCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "火".Lang() + " +1" },
                ReadOnly = true,
            };
            FireCheckBox.MouseClick += (o, e) => RefineType = RefineType.Fire;

            IceCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "冰".Lang() + " +1" },
                ReadOnly = true,
            };
            IceCheckBox.MouseClick += (o, e) => RefineType = RefineType.Ice;

            LightningCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "雷".Lang() + " +1" },
                ReadOnly = true,
            };
            LightningCheckBox.MouseClick += (o, e) => RefineType = RefineType.Lightning;

            WindCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "风".Lang() + " +1" },
                ReadOnly = true,
            };
            WindCheckBox.MouseClick += (o, e) => RefineType = RefineType.Wind;

            HolyCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "神圣".Lang() + " +1" },
                ReadOnly = true,
            };
            HolyCheckBox.MouseClick += (o, e) => RefineType = RefineType.Holy;

            DarkCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "暗黑".Lang() + " +1" },
                ReadOnly = true,
            };
            DarkCheckBox.MouseClick += (o, e) => RefineType = RefineType.Dark;

            PhantomCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "幻影".Lang() + " +1" },
                ReadOnly = true,
            };
            PhantomCheckBox.MouseClick += (o, e) => RefineType = RefineType.Phantom;

            HealthCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "生命值".Lang() + " +10" },
                ReadOnly = true,
            };
            HealthCheckBox.MouseClick += (o, e) => RefineType = RefineType.Health;

            ManaCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "魔法值".Lang() + " +10" },
                ReadOnly = true,
            };
            ManaCheckBox.MouseClick += (o, e) => RefineType = RefineType.Mana;

            ACCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "物理防御".Lang() + " 1-1" },
                ReadOnly = true,
            };
            ACCheckBox.MouseClick += (o, e) => RefineType = RefineType.AC;

            MRCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "魔法防御".Lang() + " 1-1" },
                ReadOnly = true,
            };
            MRCheckBox.MouseClick += (o, e) => RefineType = RefineType.MR;

            AccuracyCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "准确".Lang() + " +1" },
                ReadOnly = true,
            };
            AccuracyCheckBox.MouseClick += (o, e) => RefineType = RefineType.Accuracy;

            AgilityCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "敏捷".Lang() + " +1" },
                ReadOnly = true,
            };
            AgilityCheckBox.MouseClick += (o, e) => RefineType = RefineType.Agility;

            DCPercentCheckBox.Location = new Point(ClientArea.Right - DCPercentCheckBox.Size.Width - 280, ClientArea.Y + 5);
            SPPercentCheckBox.Location = new Point(ClientArea.Right - SPPercentCheckBox.Size.Width - 186, ClientArea.Y + 5);
            HealthPercentCheckBox.Location = new Point(ClientArea.Right - HealthPercentCheckBox.Size.Width - 101, ClientArea.Y + 5);
            ManaPercentCheckBox.Location = new Point(ClientArea.Right - ManaPercentCheckBox.Size.Width - 15, ClientArea.Y + 5);

            DCCheckBox.Location = new Point(ClientArea.Right - DCCheckBox.Size.Width - 280, ClientArea.Y + 22);
            SPCheckBox.Location = new Point(ClientArea.Right - SPCheckBox.Size.Width - 186, ClientArea.Y + 22);
            HealthCheckBox.Location = new Point(ClientArea.Right - HealthCheckBox.Size.Width - 101, ClientArea.Y + 22);
            ManaCheckBox.Location = new Point(ClientArea.Right - ManaCheckBox.Size.Width - 15, ClientArea.Y + 22);

            ACCheckBox.Location = new Point(ClientArea.Right - ACCheckBox.Size.Width - 280, ClientArea.Y + 39);
            MRCheckBox.Location = new Point(ClientArea.Right - MRCheckBox.Size.Width - 186, ClientArea.Y + 39);
            AccuracyCheckBox.Location = new Point(ClientArea.Right - AccuracyCheckBox.Size.Width - 101, ClientArea.Y + 39);
            AgilityCheckBox.Location = new Point(ClientArea.Right - AgilityCheckBox.Size.Width - 15, ClientArea.Y + 39);

            new DXLabel
            {
                Text = "攻击元素".Lang(),
                Location = new Point(ClientArea.Right - HealthCheckBox.Size.Width - 150, ClientArea.Y + 73),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };

            FireCheckBox.Location = new Point(ClientArea.Right - FireCheckBox.Size.Width - 280, ClientArea.Y + 90);
            IceCheckBox.Location = new Point(ClientArea.Right - IceCheckBox.Size.Width - 186, ClientArea.Y + 90);
            LightningCheckBox.Location = new Point(ClientArea.Right - LightningCheckBox.Size.Width - 101, ClientArea.Y + 90);
            WindCheckBox.Location = new Point(ClientArea.Right - WindCheckBox.Size.Width - 15, ClientArea.Y + 90);
            HolyCheckBox.Location = new Point(ClientArea.Right - HolyCheckBox.Size.Width - 280, ClientArea.Y + 105);
            DarkCheckBox.Location = new Point(ClientArea.Right - DarkCheckBox.Size.Width - 186, ClientArea.Y + 105);
            PhantomCheckBox.Location = new Point(ClientArea.Right - PhantomCheckBox.Size.Width - 101, ClientArea.Y + 105);

            SubmitButton = new DXButton
            {
                Label = { Text = "升级".Lang() },
                Size = new Size(80, SmallButtonHeight),
                Parent = this,
                ButtonType = ButtonType.SmallButton,
                Enabled = false,
            };
            SubmitButton.Location = new Point(ClientArea.Left + 40, ClientArea.Bottom - SubmitButton.Size.Height - 5);
            SubmitButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;
                ;

                DXItemCell cell = TargetCell.Grid[0];

                if (cell.Link == null) return;

                CellLinkInfo targetLink = new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot };
                cell.Link.Locked = true;
                cell.Link = null;

                CEnvir.Enqueue(new C.NPCAccessoryUpgrade { Target = targetLink, RefineType = RefineType });
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _RefineType = 0;
                RefineTypeChanged = null;

                if (TargetCell != null)
                {
                    if (!TargetCell.IsDisposed)
                        TargetCell.Dispose();

                    TargetCell = null;
                }

                if (DCCheckBox != null)
                {
                    if (!DCCheckBox.IsDisposed)
                        DCCheckBox.Dispose();

                    DCCheckBox = null;
                }

                if (SPCheckBox != null)
                {
                    if (!SPCheckBox.IsDisposed)
                        SPCheckBox.Dispose();

                    SPCheckBox = null;
                }

                if (FireCheckBox != null)
                {
                    if (!FireCheckBox.IsDisposed)
                        FireCheckBox.Dispose();

                    FireCheckBox = null;
                }

                if (IceCheckBox != null)
                {
                    if (!IceCheckBox.IsDisposed)
                        IceCheckBox.Dispose();

                    IceCheckBox = null;
                }

                if (LightningCheckBox != null)
                {
                    if (!LightningCheckBox.IsDisposed)
                        LightningCheckBox.Dispose();

                    LightningCheckBox = null;
                }

                if (WindCheckBox != null)
                {
                    if (!WindCheckBox.IsDisposed)
                        WindCheckBox.Dispose();

                    WindCheckBox = null;
                }

                if (HolyCheckBox != null)
                {
                    if (!HolyCheckBox.IsDisposed)
                        HolyCheckBox.Dispose();

                    HolyCheckBox = null;
                }

                if (DarkCheckBox != null)
                {
                    if (!DarkCheckBox.IsDisposed)
                        DarkCheckBox.Dispose();

                    DarkCheckBox = null;
                }

                if (PhantomCheckBox != null)
                {
                    if (!PhantomCheckBox.IsDisposed)
                        PhantomCheckBox.Dispose();

                    PhantomCheckBox = null;
                }

                if (SubmitButton != null)
                {
                    if (!SubmitButton.IsDisposed)
                        SubmitButton.Dispose();

                    SubmitButton = null;
                }

                if (HealthCheckBox != null)
                {
                    if (!HealthCheckBox.IsDisposed)
                        HealthCheckBox.Dispose();

                    HealthCheckBox = null;
                }

                if (ManaCheckBox != null)
                {
                    if (!ManaCheckBox.IsDisposed)
                        ManaCheckBox.Dispose();

                    ManaCheckBox = null;
                }

                if (ACCheckBox != null)
                {
                    if (!ACCheckBox.IsDisposed)
                        ACCheckBox.Dispose();

                    ACCheckBox = null;
                }

                if (MRCheckBox != null)
                {
                    if (!MRCheckBox.IsDisposed)
                        MRCheckBox.Dispose();

                    MRCheckBox = null;
                }

                if (AccuracyCheckBox != null)
                {
                    if (!AccuracyCheckBox.IsDisposed)
                        AccuracyCheckBox.Dispose();

                    AccuracyCheckBox = null;
                }

                if (AgilityCheckBox != null)
                {
                    if (!AgilityCheckBox.IsDisposed)
                        AgilityCheckBox.Dispose();

                    AgilityCheckBox = null;
                }
            }
        }
        #endregion
    }

    #endregion

    #region NPC首饰熔炼

    /// <summary>
    /// NPC首饰熔炼功能
    /// </summary>
    public sealed class NPCAccessoryLevelDialog : DXWindow
    {
        #region Properties

        public DXItemGrid TargetCell;
        public DXItemGrid Grid;
        public DXButton LevelUpButton;
        public DXLabel CostLabel;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
                GameScene.Game.InventoryBox.Visible = true;

            if (!IsVisible)
            {
                TargetCell.ClearLinks();
                Grid.ClearLinks();
            }
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// NPC首饰熔炼面板
        /// </summary>
        public NPCAccessoryLevelDialog()
        {
            TitleLabel.Text = "首饰熔炼".Lang();

            Grid = new DXItemGrid
            {
                GridSize = new Size(7, 3),
                Parent = this,
                GridType = GridType.AccessoryRefineLevelItems,
                Linked = true
            };

            Movable = false;
            SetClientSize(new Size(Grid.Size.Width, Grid.Size.Height + 110));
            Grid.Location = new Point(ClientArea.X, ClientArea.Y + 60);

            foreach (DXItemCell cell in Grid.Grid)
                cell.LinkChanged += (o, e) => CalculateCost();

            DXLabel label = new DXLabel
            {
                Text = "首饰".Lang(),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2, ClientArea.Y);

            TargetCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.AccessoryRefineLevelTarget,
                Linked = true,
            };
            TargetCell.Location = new Point(label.Location.X + (label.Size.Width - TargetCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);

            CostLabel = new DXLabel   //金额标签
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left + 80, ClientArea.Bottom - 45),
                Text = "0",
                Size = new Size(ClientArea.Width - 80, 20),
                Sound = SoundIndex.GoldPickUp
            };

            new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left, ClientArea.Bottom - 45),
                Text = "熔炼成本".Lang(),
                Size = new Size(79, 20),
                IsControl = false,
            };

            DXButton selectAll = new DXButton
            {
                Label = { Text = "全选".Lang() },
                Location = new Point(ClientArea.X, CostLabel.Location.Y + CostLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight)
            };
            selectAll.MouseClick += (o, e) =>
            {
                foreach (DXItemCell cell in GameScene.Game.InventoryBox.Grid.Grid)
                {
                    if (!cell.CheckLink(Grid)) continue;

                    cell.MoveItem(Grid, true);
                }
            };

            LevelUpButton = new DXButton
            {
                Label = { Text = "熔炼".Lang() },
                Location = new Point(ClientArea.Right - 80, CostLabel.Location.Y + CostLabel.Size.Height + 5),
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(79, SmallButtonHeight),
                Enabled = false,
            };
            LevelUpButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                List<CellLinkInfo> links = new List<CellLinkInfo>();

                DXItemCell target = TargetCell.Grid[0];

                if (target.Link == null) return;

                CellLinkInfo targetLink = new CellLinkInfo { Count = target.LinkedCount, GridType = target.Link.GridType, Slot = target.Link.Slot };
                target.Link.Locked = true;
                target.Link = null;

                foreach (DXItemCell cell in Grid.Grid)
                {
                    if (cell.Link == null) continue;

                    links.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }

                CEnvir.Enqueue(new C.NPCAccessoryLevelUp { Target = targetLink, Links = links });
            };
        }

        #region Methods
        /// <summary>
        /// 计算熔炼成本
        /// </summary>
        private void CalculateCost()
        {
            int count = 0;
            foreach (DXItemCell cell in Grid.Grid)
            {
                if (cell.Link?.Item == null) continue;

                count++;
            }

            CostLabel.ForeColour = count > MapObject.User.Gold ? Color.Red : Color.White;

            //CostLabel.Text = count.ToString("#,##0");

            LevelUpButton.Enabled = count > 0;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                if (LevelUpButton != null)
                {
                    if (!LevelUpButton.IsDisposed)
                        LevelUpButton.Dispose();

                    LevelUpButton = null;
                }

                if (CostLabel != null)
                {
                    if (!CostLabel.IsDisposed)
                        CostLabel.Dispose();

                    CostLabel = null;
                }
            }
        }
        #endregion
    }

    #endregion

    #region NPC首饰重置

    /// <summary>
    /// NPC首饰重置功能
    /// </summary>
    public sealed class NPCAccessoryResetDialog : DXWindow
    {
        #region Properties

        public DXItemGrid AccessoryGrid;
        public DXButton ResetButton;

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// NPC首饰重置面板
        /// </summary>
        public NPCAccessoryResetDialog()
        {
            HasTitle = false;
            SetClientSize(new Size(100, 105));
            CloseButton.Visible = false;

            DXLabel label = new DXLabel
            {
                Text = "首饰".Lang(),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = ClientArea.Location,
                AutoSize = false,
                Size = new Size(ClientArea.Width, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            };
            AccessoryGrid = new DXItemGrid
            {
                Parent = this,
                Location = new Point(ClientArea.X + (ClientArea.Width - 36) / 2, label.Size.Height + label.Location.Y + 5),
                GridSize = new Size(1, 1),
                Linked = true,
                GridType = GridType.AccessoryReset,
            };

            AccessoryGrid.Grid[0].LinkChanged += (o, e) => ResetButton.Enabled = AccessoryGrid.Grid[0].Item != null;
            AccessoryGrid.Grid[0].BeforeDraw += (o, e) => Draw(AccessoryGrid.Grid[0], 31);

            ResetButton = new DXButton
            {
                Size = new Size(50, SmallButtonHeight),
                Location = new Point((ClientArea.Width - 50) / 2 + ClientArea.X, ClientArea.Bottom - SmallButtonHeight),
                Label = { Text = "重置".Lang() },
                Parent = this,
                ButtonType = ButtonType.SmallButton,
                Enabled = false,
            };

            label = new DXLabel
            {
                Text = $"费用".Lang() + $": {Globals.AccessoryResetCost:#,##0}",
                Parent = this,
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(ClientArea.X, ResetButton.Location.Y - 25),
                AutoSize = false,
                Size = new Size(ClientArea.Width, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            };

            ResetButton.MouseClick += (o, e) =>   //重置按钮鼠标点击
            {
                if (AccessoryGrid.Grid[0].Item == null) return;

                if (GameScene.Game.Observer) return;

                switch (AccessoryGrid.Grid[0].Item.Info.ItemType)
                {
                    case ItemType.Ring:
                    case ItemType.Bracelet:
                    case ItemType.Necklace:
                        break;
                    default:
                        return;
                }

                CellLinkInfo targetLink = new CellLinkInfo { Count = AccessoryGrid.Grid[0].LinkedCount, GridType = AccessoryGrid.Grid[0].Link.GridType, Slot = AccessoryGrid.Grid[0].Link.Slot };

                AccessoryGrid.Grid[0].Link.Locked = true;
                AccessoryGrid.Grid[0].Link = null;

                CEnvir.Enqueue(new C.NPCAccessoryReset { Cell = targetLink });
            };
        }

        #region Methods
        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void Draw(DXItemCell cell, int index)
        {
            if (InterfaceLibrary == null) return;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (AccessoryGrid != null)
                {
                    if (!AccessoryGrid.IsDisposed)
                        AccessoryGrid.Dispose();

                    AccessoryGrid = null;
                }

                if (ResetButton != null)
                {
                    if (!ResetButton.IsDisposed)
                        ResetButton.Dispose();

                    ResetButton = null;
                }
            }
        }
        #endregion
    }

    #endregion

    #region NPC武器工艺

    /// <summary>
    /// NPC武器工艺功能
    /// </summary>
    public class NPCWeaponCraftWindow : DXWindow
    {
        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        private DXComboBox ClassComboBox;

        private DXImageControl PreviewImageBox;

        public DXItemGrid TemplateCell;

        public DXItemGrid YellowCell;
        public DXItemGrid BlueCell;
        public DXItemGrid RedCell;
        public DXItemGrid PurpleCell;
        public DXItemGrid GreenCell;
        public DXItemGrid GreyCell;

        private DXLabel ClassLabel;

        private DXButton AttemptButton;

        #region RequiredClass
        /// <summary>
        /// 要求职业
        /// </summary>
        public RequiredClass RequiredClass
        {
            get { return _RequiredClass; }
            set
            {
                if (_RequiredClass == value) return;

                RequiredClass oldValue = _RequiredClass;
                _RequiredClass = value;

                OnRequiredClassChanged(oldValue, value);
            }
        }
        private RequiredClass _RequiredClass;
        public event EventHandler<EventArgs> RequiredClassChanged;
        public virtual void OnRequiredClassChanged(RequiredClass oValue, RequiredClass nValue)
        {
            if (TemplateCell.Grid[0].Item == null || TemplateCell.Grid[0].Item.Info.Effect == ItemEffect.WeaponTemplate)
            {
                switch (RequiredClass)
                {
                    case RequiredClass.None:
                        PreviewImageBox.Index = 1110;
                        break;
                    case RequiredClass.Warrior:
                        PreviewImageBox.Index = 1111;
                        break;
                    case RequiredClass.Wizard:
                        PreviewImageBox.Index = 1112;
                        break;
                    case RequiredClass.Taoist:
                        PreviewImageBox.Index = 1113;
                        break;
                    case RequiredClass.Assassin:
                        PreviewImageBox.Index = 1114;
                        break;
                }
            }
            else
            {
                PreviewImageBox.Index = TemplateCell.Grid[0].Item.Info.Image;
            }

            AttemptButton.Enabled = CanCraft;

            RequiredClassChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        /// <summary>
        /// 成本
        /// </summary>
        public long Cost
        {
            get
            {
                long cost = Globals.CraftWeaponPercentCost;

                if (TemplateCell.Grid[0].Item != null && TemplateCell.Grid[0].Item.Info.Effect != ItemEffect.WeaponTemplate)
                {
                    switch (TemplateCell.Grid[0].Item.Info.Rarity)
                    {
                        case Rarity.Common:
                            cost = Globals.CommonCraftWeaponPercentCost;
                            break;
                        case Rarity.Superior:
                            cost = Globals.SuperiorCraftWeaponPercentCost;
                            break;
                        case Rarity.Elite:
                            cost = Globals.EliteCraftWeaponPercentCost;
                            break;
                    }
                }
                return cost;
            }
        }

        public bool CanCraft => Cost <= GameScene.Game.User.Gold && TemplateCell.Grid[0].Link != null && RequiredClass != RequiredClass.None;

        /// <summary>
        /// NPC武器工艺面板
        /// </summary>
        public NPCWeaponCraftWindow()
        {
            TitleLabel.Text = "武器工艺".Lang();

            HasFooter = false;

            SetClientSize(new Size(250, 280));

            DXLabel label = new DXLabel
            {
                Text = "模板".Lang() + " / " + "武器".Lang(),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2 + 50, ClientArea.Y);

            TemplateCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.WeaponCraftTemplate,
                Linked = true,
            };
            TemplateCell.Location = new Point(label.Location.X + (label.Size.Width - TemplateCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);
            TemplateCell.Grid[0].LinkChanged += (o, e) =>
            {
                if (TemplateCell.Grid[0].Item == null || TemplateCell.Grid[0].Item.Info.Effect == ItemEffect.WeaponTemplate)
                {
                    ClassLabel.Text = "职业".Lang();
                    switch (RequiredClass)
                    {
                        case RequiredClass.None:
                            PreviewImageBox.Index = 1110;
                            break;
                        case RequiredClass.Warrior:
                            PreviewImageBox.Index = 1111;
                            break;
                        case RequiredClass.Wizard:
                            PreviewImageBox.Index = 1112;
                            break;
                        case RequiredClass.Taoist:
                            PreviewImageBox.Index = 1113;
                            break;
                        case RequiredClass.Assassin:
                            PreviewImageBox.Index = 1114;
                            break;

                    }
                }
                else
                {
                    ClassLabel.Text = "状态".Lang();
                    PreviewImageBox.Index = TemplateCell.Grid[0].Item.Info.Image;
                }

                ClassLabel.Location = new Point(ClientArea.X + (ClientArea.Width - ClassLabel.Size.Width) / 2, ClientArea.Y + 185);

                AttemptButton.Enabled = CanCraft;
            };


            label = new DXLabel
            {
                Text = "黄色".Lang(),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2, ClientArea.Y + 60);
            YellowCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.WeaponCraftYellow,
                Linked = true,
            };
            YellowCell.Location = new Point(label.Location.X + (label.Size.Width - YellowCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);

            label = new DXLabel
            {
                Text = "蓝色".Lang(),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2 + 50, ClientArea.Y + 60);
            BlueCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.WeaponCraftBlue,
                Linked = true,
            };
            BlueCell.Location = new Point(label.Location.X + (label.Size.Width - BlueCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);

            label = new DXLabel
            {
                Text = "红色".Lang(),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2 + 100, ClientArea.Y + 60);
            RedCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.WeaponCraftRed,
                Linked = true,
            };
            RedCell.Location = new Point(label.Location.X + (label.Size.Width - RedCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);

            label = new DXLabel
            {
                Text = "紫色".Lang(),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2, ClientArea.Y + 120);
            PurpleCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.WeaponCraftPurple,
                Linked = true,
            };
            PurpleCell.Location = new Point(label.Location.X + (label.Size.Width - PurpleCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);

            label = new DXLabel
            {
                Text = "绿色".Lang(),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2 + 50, ClientArea.Y + 120);
            GreenCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.WeaponCraftGreen,
                Linked = true,
            };
            GreenCell.Location = new Point(label.Location.X + (label.Size.Width - GreenCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);

            label = new DXLabel
            {
                Text = "灰色".Lang(),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };
            label.Location = new Point(ClientArea.X + (ClientArea.Width - label.Size.Width) / 2 + 100, ClientArea.Y + 120);
            GreyCell = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.WeaponCraftGrey,
                Linked = true,
            };
            GreyCell.Location = new Point(label.Location.X + (label.Size.Width - GreyCell.Size.Width) / 2, label.Location.Y + label.Size.Height + 5);

            ClassLabel = new DXLabel
            {
                Text = "职业".Lang(),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Underline)
            };
            ClassLabel.Location = new Point(ClientArea.X + (ClientArea.Width - ClassLabel.Size.Width) / 2, ClientArea.Y + 185);

            #region Class
            ClassComboBox = new DXComboBox
            {
                Parent = this,
                Size = new Size(GreenCell.Size.Width + 48, DXComboBox.DefaultNormalHeight),
            };
            ClassComboBox.Location = new Point(GreenCell.Location.X + 1, ClientArea.Y + 185);
            ClassComboBox.SelectedItemChanged += (o, e) =>
            {
                RequiredClass = (RequiredClass?)ClassComboBox.SelectedItem ?? RequiredClass.None;
            };

            new DXListBoxItem
            {
                Parent = ClassComboBox.ListBox,
                Label = { Text = $"无".Lang() },
                Item = RequiredClass.None
            };

            new DXListBoxItem
            {
                Parent = ClassComboBox.ListBox,
                Label = { Text = $"战士".Lang() },
                Item = RequiredClass.Warrior
            };
            new DXListBoxItem
            {
                Parent = ClassComboBox.ListBox,
                Label = { Text = $"法师".Lang() },
                Item = RequiredClass.Wizard
            };
            new DXListBoxItem
            {
                Parent = ClassComboBox.ListBox,
                Label = { Text = $"道士".Lang() },
                Item = RequiredClass.Taoist
            };

            new DXListBoxItem
            {
                Parent = ClassComboBox.ListBox,
                Label = { Text = $"刺客".Lang() },
                Item = RequiredClass.Assassin
            };

            ClassComboBox.ListBox.SelectItem(RequiredClass.None);
            #endregion

            #region Preview  预览

            PreviewImageBox = new DXImageControl
            {
                Parent = this,
                Location = new Point(ClientArea.X + 20, ClientArea.Y + ClientArea.Height / 2 - 76),
                LibraryFile = LibraryFile.Equip,
                Index = 1110,
                Border = true,
            };

            #endregion

            AttemptButton = new DXButton
            {
                Parent = this,
                Location = new Point(YellowCell.Location.X, ClientArea.Y + 260),
                Size = new Size(YellowCell.Size.Width + 99, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "制作".Lang() }
            };
            AttemptButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                if (TemplateCell.Grid[0].Link == null) return;

                C.NPCWeaponCraft packet = new C.NPCWeaponCraft
                {
                    Class = RequiredClass,

                    Template = new CellLinkInfo { Count = TemplateCell.Grid[0].LinkedCount, GridType = TemplateCell.Grid[0].Link.GridType, Slot = TemplateCell.Grid[0].Link.Slot }
                };

                TemplateCell.Grid[0].Link.Locked = true;
                TemplateCell.Grid[0].Link = null;

                if (YellowCell.Grid[0].Link != null)
                {
                    packet.Yellow = new CellLinkInfo { Count = YellowCell.Grid[0].LinkedCount, GridType = YellowCell.Grid[0].Link.GridType, Slot = YellowCell.Grid[0].Link.Slot };
                    YellowCell.Grid[0].Link.Locked = true;
                    YellowCell.Grid[0].Link = null;
                }

                if (BlueCell.Grid[0].Link != null)
                {
                    packet.Blue = new CellLinkInfo { Count = BlueCell.Grid[0].LinkedCount, GridType = BlueCell.Grid[0].Link.GridType, Slot = BlueCell.Grid[0].Link.Slot };
                    BlueCell.Grid[0].Link.Locked = true;
                    BlueCell.Grid[0].Link = null;
                }

                if (RedCell.Grid[0].Link != null)
                {
                    packet.Red = new CellLinkInfo { Count = RedCell.Grid[0].LinkedCount, GridType = RedCell.Grid[0].Link.GridType, Slot = RedCell.Grid[0].Link.Slot };
                    RedCell.Grid[0].Link.Locked = true;
                    RedCell.Grid[0].Link = null;
                }

                if (PurpleCell.Grid[0].Link != null)
                {
                    packet.Purple = new CellLinkInfo { Count = PurpleCell.Grid[0].LinkedCount, GridType = PurpleCell.Grid[0].Link.GridType, Slot = PurpleCell.Grid[0].Link.Slot };
                    PurpleCell.Grid[0].Link.Locked = true;
                    PurpleCell.Grid[0].Link = null;
                }

                if (GreenCell.Grid[0].Link != null)
                {
                    packet.Green = new CellLinkInfo { Count = GreenCell.Grid[0].LinkedCount, GridType = GreenCell.Grid[0].Link.GridType, Slot = GreenCell.Grid[0].Link.Slot };
                    GreenCell.Grid[0].Link.Locked = true;
                    GreenCell.Grid[0].Link = null;
                }

                if (GreyCell.Grid[0].Link != null)
                {
                    packet.Grey = new CellLinkInfo { Count = GreyCell.Grid[0].LinkedCount, GridType = GreyCell.Grid[0].Link.GridType, Slot = GreyCell.Grid[0].Link.Slot };
                    GreyCell.Grid[0].Link.Locked = true;
                    GreyCell.Grid[0].Link = null;
                }

                CEnvir.Enqueue(packet);
                AttemptButton.Enabled = CanCraft;
            };
        }
    }

    #endregion

    #region NPC D键菜单

    /// <summary>
    /// NPC D键菜单功能
    /// </summary>
    public class NPCDKeyDialog : DXWindow
    {
        public DXImageControl Background;
        public DXButton Close1Button;

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        public List<DXLabel> labels = new List<DXLabel>();

        public DXLabel PageContent;

        /// <summary>
        /// 响应
        /// </summary>
        /// <param name="menu"></param>
        public void Response(S.DKey menu)
        {
            if (menu.Options != null)
            {
                foreach (DXLabel toDisposeLabel in labels)
                {
                    toDisposeLabel.Dispose();
                }
                labels.Clear();
                foreach (DKeyPage option in menu.Options)
                {
                    DXLabel label;
                    if (option.targetNPCIndex > 0)
                    {
                        label = new DXLabel
                        {
                            Text = option.text,
                            Size = new Size(80, 40),
                            Font = new Font(Config.FontName, 9, FontStyle.Regular), //字体样式大小  
                            Parent = PageContent,
                            Outline = false,
                            Sound = SoundIndex.ButtonC,
                            ForeColour = Color.Yellow,
                            Location = new Point(option.labelLocationX, option.labelLocationY),
                        };
                        label.MouseEnter += (o, e) =>     //鼠标移动上去
                        {
                            if (GameScene.Game.Observer) return;
                            label.ForeColour = Color.Red;
                            label.Font = new Font(label.Font.FontFamily, label.Font.Size, FontStyle.Underline); //增加文字下划线
                        };

                        label.MouseLeave += (o, e) =>   //鼠标离开
                        {
                            if (GameScene.Game.Observer) return;
                            label.ForeColour = Color.Yellow;
                            label.Font = new Font(label.Font.FontFamily, label.Font.Size, FontStyle.Regular); //去掉文字下划线
                        };

                        label.MouseClick += (o, e) => CEnvir.Enqueue(new C.NPCCall { ObjectID = option.targetNPCIndex, isDKey = true });
                    }
                    else
                    {
                        label = new DXLabel
                        {
                            Text = option.text,
                            Size = new Size(option.text.Length * 20 + 10, 40),// 10号字体大概长宽各20, 但是允许重叠
                            Font = new Font(Config.FontName, 9, FontStyle.Regular), //字体样式大小
                            Parent = PageContent,
                            Outline = false,
                            ForeColour = option.textColor,
                            Location = new Point(option.labelLocationX, option.labelLocationY),
                        };
                    }

                    labels.Add(label);
                }
            }
        }

        /// <summary>
        /// NPC D键菜单面板
        /// </summary>
        public NPCDKeyDialog()
        {
            HasTitle = false;   //不显示页头
            HasFooter = false;  //页脚不显示
            Movable = true;    //不可移动
            CloseButton.Visible = false;
            Opacity = 0F;
            PassThrough = true;  //穿透开启
            SetClientSize(new Size(380, 380));

            Background = new DXImageControl  //底图
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1348,
                Parent = this,
            };

            PageContent = new DXLabel
            {
                Text = "",
                AutoSize = false,  //自动大小关闭
                Outline = false,   //外形尺寸关闭
                Parent = this,
                ForeColour = Color.White,    //文本颜色 白色
                Location = new Point(ClientArea.X + 10, ClientArea.Y + 10),
                Size = new Size(ClientArea.Width - 10, ClientArea.Height - 10),
            };

            Close1Button = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1221,
                Parent = this,
                Location = new Point(340, 337),
            };
            Close1Button.MouseClick += (o, e) => Visible = false;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Background != null)
                {
                    if (!Background.IsDisposed)
                        Background.Dispose();

                    Background = null;
                }

                if (Close1Button != null)
                {
                    if (!Close1Button.IsDisposed)
                        Close1Button.Dispose();

                    Close1Button = null;
                }
            }
        }
    }

    /// <summary>
    /// NPC额外属性加点功能
    /// </summary>
    public sealed class AdditionalDialog : DXWindow
    {
        public Dictionary<Stat, DXLabel> HermitDisplayStats = new Dictionary<Stat, DXLabel>();
        public Dictionary<Stat, DXLabel> HermitAttackStats = new Dictionary<Stat, DXLabel>();
        public DXLabel RemainingLabel;   //剩余点数标签

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        /// <summary>
        /// NPC额外属性加点面板
        /// </summary>
        public AdditionalDialog()
        {
            TitleLabel.Text = "额外属性".Lang();

            HasFooter = false;  //页脚不显示
            Movable = true;    //不可移动
            SetClientSize(new Size(300, 450));

            //加点
            DXLabel label = new DXLabel
            {
                Parent = this,
                Text = "物理防御".Lang(),
            };
            label.Location = new Point(25 - label.Size.Width + 50, 60);
            HermitDisplayStats[Stat.MaxAC] = new DXLabel
            {
                Parent = this,
                Location = new Point(label.Location.X + label.Size.Width - 5, label.Location.Y),
                ForeColour = Color.White,
                Text = "0-0"
            };

            label = new DXLabel
            {
                Parent = this,
                Text = "魔法防御".Lang()
            };
            label.Location = new Point(120 - label.Size.Width + 50, 60);
            HermitDisplayStats[Stat.MaxMR] = new DXLabel
            {
                Parent = this,
                Location = new Point(label.Location.X + label.Size.Width - 5, label.Location.Y),
                ForeColour = Color.White,
                Text = "0-0"
            };

            label = new DXLabel
            {
                Parent = this,
                Text = "破坏".Lang()
            };
            label.Location = new Point(210 - label.Size.Width + 50, 60);
            HermitDisplayStats[Stat.MaxDC] = new DXLabel
            {
                Parent = this,
                Location = new Point(label.Location.X + label.Size.Width - 5, label.Location.Y),
                ForeColour = Color.White,
                Text = "0-0"
            };

            label = new DXLabel
            {
                Parent = this,
                Text = "自然系魔法".Lang()
            };
            label.Location = new Point(60 - label.Size.Width + 50, 90);
            HermitDisplayStats[Stat.MaxMC] = new DXLabel
            {
                Parent = this,
                Location = new Point(label.Location.X + label.Size.Width - 5, label.Location.Y),
                ForeColour = Color.White,
                Text = "0-0"
            };

            label = new DXLabel
            {
                Parent = this,
                Text = "灵魂系魔法".Lang()
            };
            label.Location = new Point(190 - label.Size.Width + 50, 90);
            HermitDisplayStats[Stat.MaxSC] = new DXLabel
            {
                Parent = this,
                Location = new Point(label.Location.X + label.Size.Width - 5, label.Location.Y),
                ForeColour = Color.White,
                Text = "0-0"
            };

            label = new DXLabel
            {
                Parent = this,
                Text = "生命值".Lang()
            };
            label.Location = new Point(60 - label.Size.Width + 50, 120);
            HermitDisplayStats[Stat.Health] = new DXLabel
            {
                Parent = this,
                Location = new Point(label.Location.X + label.Size.Width - 5, label.Location.Y),
                ForeColour = Color.White,
                Text = "0"
            };

            label = new DXLabel
            {
                Parent = this,
                Text = "魔法值".Lang()
            };
            label.Location = new Point(190 - label.Size.Width + 50, 120);
            HermitDisplayStats[Stat.Mana] = new DXLabel
            {
                Parent = this,
                Location = new Point(label.Location.X + label.Size.Width - 5, label.Location.Y),
                ForeColour = Color.White,
                Text = "0"
            };

            #region Attack  攻击元素

            label = new DXLabel
            {
                Parent = this,
                Text = "攻击元素".Lang()
            };
            label.Location = new Point(100 - label.Size.Width, 160);

            DXImageControl icon = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.ProgUse,
                Index = 600,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "火".Lang(),
            };
            icon.Location = new Point(label.Location.X + label.Size.Width, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2);
            HermitAttackStats[Stat.FireAttack] = new DXLabel
            {
                Parent = this,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.ProgUse,
                Index = 601,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "冰".Lang(),
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 50, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2);
            HermitAttackStats[Stat.IceAttack] = new DXLabel
            {
                Parent = this,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.ProgUse,
                Index = 602,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "雷".Lang(),
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 100, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2);
            HermitAttackStats[Stat.LightningAttack] = new DXLabel
            {
                Parent = this,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.ProgUse,
                Index = 603,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "风".Lang(),
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 150, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2);
            HermitAttackStats[Stat.WindAttack] = new DXLabel
            {
                Parent = this,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.ProgUse,
                Index = 604,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "神圣".Lang(),
            };
            icon.Location = new Point(label.Location.X + label.Size.Width, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2 + 25);
            HermitAttackStats[Stat.HolyAttack] = new DXLabel
            {
                Parent = this,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y + 25),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.ProgUse,
                Index = 605,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "暗黑".Lang(),
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 50, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2 + 25);
            HermitAttackStats[Stat.DarkAttack] = new DXLabel
            {
                Parent = this,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y + 25),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.ProgUse,
                Index = 606,
                ForeColour = Color.FromArgb(60, 60, 60),
                Hint = "幻影".Lang(),
            };
            icon.Location = new Point(label.Location.X + label.Size.Width + 100, label.Location.Y + (label.Size.Height - icon.Size.Height) / 2 + 25);
            HermitAttackStats[Stat.PhantomAttack] = new DXLabel
            {
                Parent = this,
                Location = new Point(icon.Location.X + icon.Size.Width, label.Location.Y + 25),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = icon,
            };

            #endregion

            label = new DXLabel
            {
                Parent = this,
                Text = "未分配点数".Lang(),
            };
            label.Location = new Point(Size.Width / 4 * 2 - label.Size.Width + 25, 250);

            RemainingLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(label.Location.X + label.Size.Width - 5, label.Location.Y),
                ForeColour = Color.White,
                Text = "0"
            };

            DXCheckBox check = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "显示确认".Lang() },
                Checked = true,
            };
            check.Location = new Point(Size.Width - check.Size.Width - 10, Size.Height - check.Size.Height - 10);

            DXButton but = new DXButton
            {
                Parent = this,
                Location = new Point(50, 280),
                Label = { Text = "物理防御".Lang() },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(80, SmallButtonHeight)
            };
            but.MouseClick += (o, e) =>
            {
                if (MapObject.User.HermitPoints == 0) return;

                if (check.Checked)
                {
                    DXMessageBox box = new DXMessageBox("确认增加你的物理防御".Lang(), "附加属性确认".Lang(), DXMessageBoxButtons.YesNo);

                    box.YesButton.MouseClick += (o1, e1) =>
                    {
                        CEnvir.Enqueue(new C.Hermit { Stat = Stat.MaxAC });
                    };
                }
                else
                {
                    CEnvir.Enqueue(new C.Hermit { Stat = Stat.MaxAC });
                }
            };

            but = new DXButton
            {
                Parent = this,
                Location = new Point(180, but.Location.Y),
                Label = { Text = "魔法防御".Lang() },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(80, SmallButtonHeight),
            };
            but.MouseClick += (o, e) =>
            {
                if (MapObject.User.HermitPoints == 0) return;

                if (check.Checked)
                {
                    DXMessageBox box = new DXMessageBox("确认增加你的魔法防御".Lang(), "附加属性确认".Lang(), DXMessageBoxButtons.YesNo);

                    box.YesButton.MouseClick += (o1, e1) =>
                    {
                        CEnvir.Enqueue(new C.Hermit { Stat = Stat.MaxMR });
                    };
                }
                else
                {
                    CEnvir.Enqueue(new C.Hermit { Stat = Stat.MaxMR });
                }
            };

            but = new DXButton
            {
                Parent = this,
                Location = new Point(50, but.Location.Y + 25),
                Label = { Text = "生命值".Lang() },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(80, SmallButtonHeight)
            };
            but.MouseClick += (o, e) =>
            {
                if (MapObject.User.HermitPoints == 0) return;

                if (check.Checked)
                {
                    DXMessageBox box = new DXMessageBox("确认增加你的生命值".Lang(), "附加属性确认".Lang(), DXMessageBoxButtons.YesNo);

                    box.YesButton.MouseClick += (o1, e1) =>
                    {
                        CEnvir.Enqueue(new C.Hermit { Stat = Stat.Health });
                    };
                }
                else
                {
                    CEnvir.Enqueue(new C.Hermit { Stat = Stat.Health });
                }
            };

            but = new DXButton
            {
                Parent = this,
                Location = new Point(180, but.Location.Y),
                Label = { Text = "魔法值".Lang() },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(80, SmallButtonHeight)
            };
            but.MouseClick += (o, e) =>
            {
                if (MapObject.User.HermitPoints == 0) return;

                if (check.Checked)
                {
                    DXMessageBox box = new DXMessageBox("确认增加你的魔法值".Lang(), "附加属性确认".Lang(), DXMessageBoxButtons.YesNo);

                    box.YesButton.MouseClick += (o1, e1) =>
                    {
                        CEnvir.Enqueue(new C.Hermit { Stat = Stat.Mana });
                    };
                }
                else
                {
                    CEnvir.Enqueue(new C.Hermit { Stat = Stat.Mana });
                }
            };

            but = new DXButton
            {
                Parent = this,
                Location = new Point(50, but.Location.Y + 25),
                Label = { Text = "破坏".Lang() },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(80, SmallButtonHeight)
            };
            but.MouseClick += (o, e) =>
            {
                if (MapObject.User.HermitPoints == 0) return;

                if (check.Checked)
                {
                    DXMessageBox box = new DXMessageBox("确认增加你的破坏".Lang(), "附加属性确认".Lang(), DXMessageBoxButtons.YesNo);

                    box.YesButton.MouseClick += (o1, e1) =>
                    {
                        CEnvir.Enqueue(new C.Hermit { Stat = Stat.MaxDC });
                    };
                }
                else
                {
                    CEnvir.Enqueue(new C.Hermit { Stat = Stat.MaxDC });
                }
            };

            but = new DXButton
            {
                Parent = this,
                Location = new Point(50, but.Location.Y + 25),
                Label = { Text = "自然系魔法".Lang() },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(80, SmallButtonHeight)
            };
            but.MouseClick += (o, e) =>
            {
                if (MapObject.User.HermitPoints == 0) return;

                if (check.Checked)
                {
                    DXMessageBox box = new DXMessageBox("确认增加你的自然系魔法".Lang(), "附加属性确认".Lang(), DXMessageBoxButtons.YesNo);

                    box.YesButton.MouseClick += (o1, e1) =>
                    {
                        CEnvir.Enqueue(new C.Hermit { Stat = Stat.MaxMC });
                    };
                }
                else
                {
                    CEnvir.Enqueue(new C.Hermit { Stat = Stat.MaxMC });
                }
            };

            but = new DXButton
            {
                Parent = this,
                Location = new Point(180, but.Location.Y),
                Label = { Text = "灵魂系魔法".Lang() },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(80, SmallButtonHeight)
            };
            but.MouseClick += (o, e) =>
            {
                if (MapObject.User.HermitPoints == 0) return;

                if (check.Checked)
                {
                    DXMessageBox box = new DXMessageBox("确认增加你的灵魂系魔法".Lang(), "附加属性确认".Lang(), DXMessageBoxButtons.YesNo);

                    box.YesButton.MouseClick += (o1, e1) =>
                    {
                        CEnvir.Enqueue(new C.Hermit { Stat = Stat.MaxSC });
                    };
                }
                else
                {
                    CEnvir.Enqueue(new C.Hermit { Stat = Stat.MaxSC });
                }
            };

            but = new DXButton
            {
                Parent = this,
                Location = new Point(115, but.Location.Y + 25),
                Label = { Text = "攻击元素".Lang() },
                ButtonType = ButtonType.SmallButton,
                Size = new Size(80, SmallButtonHeight)
            };
            but.MouseClick += (o, e) =>
            {
                if (MapObject.User.HermitPoints == 0) return;

                if (check.Checked)
                {
                    DXMessageBox box = new DXMessageBox("确认增加你的攻击元素".Lang(), "附加属性确认".Lang(), DXMessageBoxButtons.YesNo);

                    box.YesButton.MouseClick += (o1, e1) =>
                    {
                        CEnvir.Enqueue(new C.Hermit { Stat = Stat.WeaponElement });
                    };
                }
                else
                {
                    CEnvir.Enqueue(new C.Hermit { Stat = Stat.WeaponElement });
                }
            };
        }
        /// <summary>
        /// 更新属性状态
        /// </summary>
        public void UpdateStats()
        {
            foreach (KeyValuePair<Stat, DXLabel> pair in HermitDisplayStats)
            {
                pair.Value.Text = MapObject.User.HermitStats.GetFormat(pair.Key);
            }

            foreach (KeyValuePair<Stat, DXLabel> pair in HermitAttackStats)
            {
                if (MapObject.User.HermitStats[pair.Key] > 0)
                {
                    pair.Value.Text = $"+{MapObject.User.HermitStats[pair.Key]}";
                    pair.Value.ForeColour = Color.White;
                    ((DXImageControl)pair.Value.Tag).ForeColour = Color.White;
                }
                else
                {
                    pair.Value.Text = "0";
                    pair.Value.ForeColour = Color.FromArgb(60, 60, 60);
                    ((DXImageControl)pair.Value.Tag).ForeColour = Color.FromArgb(60, 60, 60);
                }
            }

            RemainingLabel.Text = MapObject.User.HermitPoints.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                foreach (KeyValuePair<Stat, DXLabel> pair in HermitDisplayStats)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }
                HermitDisplayStats.Clear();
                HermitDisplayStats = null;

                foreach (KeyValuePair<Stat, DXLabel> pair in HermitAttackStats)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }
                HermitAttackStats.Clear();
                HermitAttackStats = null;

                if (RemainingLabel != null)
                {
                    if (!RemainingLabel.IsDisposed)
                        RemainingLabel.Dispose();

                    RemainingLabel = null;
                }
            }
        }
    }

    #endregion

    #region NPC副本右边计时对话框

    /// <summary>
    /// NPC副本右边计时对话框
    /// </summary>
    public sealed class NPCReplicaDialog : DXWindow
    {
        public DXImageControl RightImage, RightExplainImage;      //右边框倒计时图标 右边框文字说明图标
        public DXLabel ExplainLabel, TimeLabel;
        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        public DateTime Expiry;

        /// <summary>
        /// NPC副本右边计时面板
        /// </summary>
        public NPCReplicaDialog()
        {
            Size = new Size(250, 80);

            Opacity = 0F;    //透明度

            Location = ClientArea.Location;

            HasTitle = false;               //标题不显示
            HasFooter = false;              //页脚不显示
            HasTopBorder = false;           //上边框不显示
            TitleLabel.Visible = false;     //标题标签不显示
            CloseButton.Visible = false;    //关闭按钮不显示

            RightImage = new DXImageControl                         //右边框倒计时图标
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 6901,
                ImageOpacity = 0.5F,           //图片透明度
                Location = new Point(120, 5),  //显示位置
            };

            TimeLabel = new DXLabel      //倒计时标签
            {
                Parent = RightImage,
                Location = new Point(ClientArea.Left + 10, 7),
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold), //字体样式大小
                BorderColour = Color.FromArgb(99, 83, 50),                              //边框颜色
                ForeColour = Color.White,                                               //字体颜色
                Text = "00:00",
                Size = new Size(80, 25),
            };

            RightExplainImage = new DXImageControl        //右边框文字说明图标
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 6900,
                ImageOpacity = 0.5F,           //图片透明度
                Location = new Point(48, 5),   //显示位置
            };

            ExplainLabel = new DXLabel            //说明标签
            {
                Parent = RightExplainImage,
                Location = new Point(ClientArea.Left + 10, 5),
                BorderColour = Color.FromArgb(99, 83, 50),      //边框颜色
                ForeColour = Color.White,                       //字体颜色
                Text = "副本说明".Lang(),
                Size = new Size(80, 20),
            };

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {

                if (RightImage != null)
                {
                    if (!RightImage.IsDisposed)
                        RightImage.Dispose();

                    RightImage = null;
                }

                if (RightExplainImage != null)
                {
                    if (!RightExplainImage.IsDisposed)
                        RightExplainImage.Dispose();

                    RightExplainImage = null;
                }
            }
        }
    }

    #endregion

    #region NPC副本顶部计时对话框

    /// <summary>
    /// NPC副本顶部计时对话框
    /// </summary>
    public sealed class NPCTopTagDialog : DXWindow
    {
        public DXImageControl NPCTimeImage;      //计时图标
        public DXLabel MapLabel, TimeLabel;
        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        public DateTime Expiry;

        /// <summary>
        /// NPC副本顶部计时面板
        /// </summary>
        public NPCTopTagDialog()
        {
            Size = new Size(110, 60);

            Opacity = 0F;    //透明度

            Location = ClientArea.Location;

            HasTitle = false;               //标题不显示
            HasFooter = false;              //页脚不显示
            HasTopBorder = false;           //上边框不显示
            TitleLabel.Visible = false;     //标题标签不显示
            CloseButton.Visible = false;    //关闭按钮不显示

            NPCTimeImage = new DXImageControl       //计时图标
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 1050,
                Location = new Point(1, 1),  //显示位置
            };

            MapLabel = new DXLabel                 //倒计时标签
            {
                Parent = NPCTimeImage,
                Location = new Point(ClientArea.Width / 2 - 20, 10),
                ForeColour = Color.White,                    //字体颜色
                Text = "副本名称".Lang(),
                //Size = new Size(100, 25),
            };

            TimeLabel = new DXLabel               //说明标签
            {
                Parent = NPCTimeImage,
                Location = new Point(ClientArea.Width / 2 - 17, 30),
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold), //字体样式大小
                ForeColour = Color.White,                      //字体颜色
                Text = "00:00:00",
                //Size = new Size(100, 25),
            };

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (NPCTimeImage != null)
                {
                    if (!NPCTimeImage.IsDisposed)
                        NPCTimeImage.Dispose();

                    NPCTimeImage = null;
                }
            }
        }
    }

    #endregion

    #region NPC技能书合成

    /// <summary>
    /// NPC技能书合成功能
    /// </summary>
    public sealed class NPCBookCombineDialog : DXWindow
    {
        public DXItemGrid BookGrid, MaterialGrid1, MaterialGrid2;

        public DXButton CombineButton;

        public DXLabel CostLabel, CostTextLabel;

        public override WindowType Type => WindowType.None;

        public override bool CustomSize => false;

        public override bool AutomaticVisibility => false;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);
            if (GameScene.Game.InventoryBox != null)
            {
                if (base.IsVisible)
                {
                    GameScene.Game.InventoryBox.Visible = true;
                }
                if (!base.IsVisible)
                {
                    BookGrid.ClearLinks();
                    MaterialGrid1.ClearLinks();
                }
            }
        }

        /// <summary>
        /// NPC技能书合成面板
        /// </summary>
        public NPCBookCombineDialog()
        {
            TitleLabel.Text = "技能书合成".Lang();
            SetClientSize(new Size(180, 150));
            Movable = false;

            DXLabel targetBookText = new DXLabel
            {
                Text = "技能书".Lang(),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9f), FontStyle.Underline)
            };
            targetBookText.Location = new Point(base.ClientArea.X + 10, base.ClientArea.Y + 18);
            BookGrid = new DXItemGrid     //主书格子
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.BookTarget,
                Linked = true
            };
            BookGrid.Location = new Point(targetBookText.Location.X + (targetBookText.Size.Width - BookGrid.Size.Width) / 2, targetBookText.Location.Y + targetBookText.Size.Height + 5);

            DXLabel bookMaterial = new DXLabel
            {
                Text = "材料1".Lang(),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9f), FontStyle.Underline)
            };
            bookMaterial.Location = new Point(BookGrid.Location.X + BookGrid.Size.Width * 2, targetBookText.Location.Y);
            MaterialGrid1 = new DXItemGrid   //材料格子1
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.BookMaterial,
                Linked = true
            };
            MaterialGrid1.Location = new Point(bookMaterial.Location.X + (bookMaterial.Size.Width - MaterialGrid1.Size.Width) / 2, bookMaterial.Location.Y + bookMaterial.Size.Height + 5);

            DXLabel bookMaterial2 = new DXLabel
            {
                Text = "材料2".Lang(),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9f), FontStyle.Underline)
            };
            bookMaterial2.Location = new Point(BookGrid.Location.X + BookGrid.Size.Width * 3, targetBookText.Location.Y);
            MaterialGrid2 = new DXItemGrid   //材料格子2
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.BookMaterial,
                Linked = true
            };
            MaterialGrid2.Location = new Point(bookMaterial2.Location.X + (bookMaterial2.Size.Width - MaterialGrid1.Size.Width) / 2, bookMaterial2.Location.Y + bookMaterial2.Size.Height + 5);

            CostLabel = new DXLabel   //金额标签
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(base.ClientArea.Left + 80, base.ClientArea.Bottom - 55),
                Text = "0",
                Size = new Size(base.ClientArea.Width - 80, 20),
                Sound = SoundIndex.GoldPickUp
            };

            CostTextLabel = new DXLabel   //合成花费标签
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                DrawFormat = (TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter),
                Parent = this,
                Location = new Point(base.ClientArea.Left, base.ClientArea.Bottom - 55),
                Text = "合成花费".Lang(),
                Size = new Size(79, 20),
                IsControl = false
            };

            DXItemCell[] bookGrid = BookGrid.Grid;  //单元格 图书
            foreach (DXItemCell cell in bookGrid)
            {
                cell.LinkChanged += delegate
                {
                    UpdateButton();
                };
            }

            DXItemCell[] materialGrid = MaterialGrid1.Grid;  //单元格 材料1
            foreach (DXItemCell dXItemCell2 in materialGrid)
            {
                dXItemCell2.LinkChanged += delegate
                {
                    UpdateButton();
                };
            }

            DXItemCell[] materialGrid2 = MaterialGrid2.Grid;  //单元格 材料2
            foreach (DXItemCell dXItemCell2 in materialGrid2)
            {
                dXItemCell2.LinkChanged += delegate
                {
                    UpdateButton();
                };
            }

            DXButton combineButton = new DXButton();
            combineButton.Label.Text = "点击合成".Lang();
            combineButton.Location = new Point(base.ClientArea.Right - 80, CostLabel.Location.Y + CostLabel.Size.Height + 10);
            combineButton.ButtonType = ButtonType.SmallButton;
            combineButton.Parent = this;
            combineButton.Size = new Size(79, DXControl.SmallButtonHeight);
            combineButton.Enabled = false;

            CombineButton = combineButton;

            CombineButton.MouseClick += delegate
            {
                if (!GameScene.Game.Observer)
                {
                    string bookName = "";
                    CellLinkInfo bookLink = new CellLinkInfo();

                    foreach (DXItemCell cell in BookGrid.Grid)
                    {
                        if (cell.Link == null)
                        {
                            continue;
                        }
                        bookLink.Count = cell.LinkedCount;
                        bookLink.GridType = cell.Link.GridType;
                        bookLink.Slot = cell.Link.Slot;

                        bookName = cell.Link.Item.Info.Lang(p => p.ItemName);
                        cell.Link.Locked = true;
                        cell.Link = null;
                    }

                    List<CellLinkInfo> materialLink = new List<CellLinkInfo>();

                    foreach (DXItemCell cell in MaterialGrid1.Grid)
                    {
                        if (cell.Link == null || bookName != cell.Link.Item.Info.Lang(p => p.ItemName))
                        {
                            continue;
                        }
                        materialLink.Add(new CellLinkInfo
                        {
                            Count = cell.LinkedCount,
                            GridType = cell.Link.GridType,
                            Slot = cell.Link.Slot
                        });
                        cell.Link.Locked = false;
                        cell.Link = null;
                    }

                    foreach (DXItemCell cell in MaterialGrid2.Grid)
                    {
                        if (cell.Link == null || bookName != cell.Link.Item.Info.Lang(p => p.ItemName))
                        {
                            continue;
                        }
                        materialLink.Add(new CellLinkInfo
                        {
                            Count = cell.LinkedCount,
                            GridType = cell.Link.GridType,
                            Slot = cell.Link.Slot
                        });
                        cell.Link.Locked = false;
                        cell.Link = null;
                    }

                    CEnvir.Enqueue(new NPCBookRefine
                    {
                        OriginalBook = bookLink,
                        Material = materialLink
                    });
                }
            };
        }
        /// <summary>
        /// 更新按钮
        /// </summary>
        private void UpdateButton()
        {
            CombineButton.Enabled = false;

            foreach (DXItemCell cell in BookGrid.Grid)
            {
                if (cell.Link == null)
                {
                    CostLabel.Text = "0";
                    return;
                }
            }

            foreach (DXItemCell cell in MaterialGrid1.Grid)
            {
                if (cell.Link == null)
                {
                    CostLabel.Text = "0";
                    return;
                }
            }
            if (CalculateCost())
            {
                CombineButton.Enabled = true;
            }
        }
        /// <summary>
        /// 计算技能书合成成本
        /// </summary>
        /// <returns></returns>
        public bool CalculateCost()
        {
            int totalCost = 0;
            CostLabel.Text = totalCost.ToString("#,##0");

            foreach (DXItemCell cell in BookGrid.Grid)
            {
                if (cell.Link?.Item != null)
                {
                    totalCost += cell.Link.Item.Info.RequiredAmount * 1000;
                }
            }

            foreach (DXItemCell cell in MaterialGrid1.Grid)
            {
                if (cell.Link?.Item != null)
                {
                    totalCost += cell.Link.Item.CurrentDurability * Globals.BookCombineFeePerDurability;
                }
            }

            foreach (DXItemCell cell in MaterialGrid2.Grid)
            {
                if (cell.Link?.Item != null)
                {
                    totalCost += cell.Link.Item.CurrentDurability * Globals.BookCombineFeePerDurability;
                }
            }

            CostLabel.ForeColour = ((totalCost > MapObject.User.Gold) ? Color.Red : Color.White);
            CostLabel.Text = totalCost.ToString("#,##0");

            if (totalCost > MapObject.User.Gold)
            {
                return false;
            }
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
            {
                return;
            }
            if (BookGrid != null)
            {
                if (!BookGrid.IsDisposed)
                {
                    BookGrid.Dispose();
                }
                BookGrid = null;
            }
            if (MaterialGrid1 != null)
            {
                if (!MaterialGrid1.IsDisposed)
                {
                    MaterialGrid1.Dispose();
                }
                MaterialGrid1 = null;
            }
            if (MaterialGrid2 != null)
            {
                if (!MaterialGrid2.IsDisposed)
                {
                    MaterialGrid2.Dispose();
                }
                MaterialGrid2 = null;
            }
            if (CombineButton != null)
            {
                if (!CombineButton.IsDisposed)
                {
                    CombineButton.Dispose();
                }
                CombineButton = null;
            }
            if (CostLabel != null)
            {
                if (!CostLabel.IsDisposed)
                {
                    CostLabel.Dispose();
                }
                CostLabel = null;
            }

            if (CostTextLabel != null)
            {
                if (!CostTextLabel.IsDisposed)
                {
                    CostTextLabel.Dispose();
                }
                CostTextLabel = null;
            }
        }
    }

    #endregion

    #region NPC宝石镶嵌打孔功能(开发中)

    /// <summary>
    /// NPC宝石镶嵌打孔功能(开发中)
    /// </summary>
    public sealed class NPCPerforationDialog : DXImageControl
    {
        public DXButton CloseButton, SureButton, CancelButton;
        public DXLabel TextLabel;

        /// <summary>
        /// NPC宝石镶嵌打孔界面
        /// </summary>
        public NPCPerforationDialog()
        {
            LibraryFile = LibraryFile.GameInter;
            Index = 5700;

            CloseButton = new DXButton  //关闭按钮
            {
                Parent = this,
                Location = new Point(158, 1),
                LibraryFile = LibraryFile.GameInter,
                Index = 1221,
                Hint = "关闭".Lang(),
            };
            CloseButton.MouseEnter += (o, e) => CloseButton.Index = 1220;  //鼠标进入
            CloseButton.MouseLeave += (o, e) => CloseButton.Index = 1221;  //鼠标离开
            CloseButton.MouseClick += (o, e) => Visible = false;        //鼠标点击

            TextLabel = new DXLabel
            {
                Text = "装备打孔".Lang(),
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),             //字体 格式  文字大小
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,   //绘图格式  文本格式标志 垂直中心  水平居中
                Parent = this,
                Size = new Size(80, 20),
                ForeColour = Color.FromArgb(190, 165, 110),        //前色
                BorderColour = Color.Black,       //边框颜色
            };
            TextLabel.Location = new Point(Size.Width / 2 - TextLabel.Size.Width / 2, 8);

            SureButton = new DXButton //开始按钮
            {
                Parent = this,
                Index = 5707,
                LibraryFile = LibraryFile.GameInter,
            };
            SureButton.Location = new Point(Size.Width / 2 - SureButton.Size.Width - 5, 278);

            CancelButton = new DXButton  //取消按钮
            {
                Parent = this,
                Index = 4722,
                LibraryFile = LibraryFile.GameInter,
            };
            CancelButton.Location = new Point(Size.Width / 2 + 5, 278);
            CancelButton.MouseClick += (o, e) => Visible = false;
        }
    }

    /// <summary>
    /// NPC宝石镶嵌附魔功能(开发中)
    /// </summary>
    public sealed class NPCEnchantingDialog : DXImageControl   //镶嵌附魔
    {
        public DXButton CloseButton, SureButton, CancelButton;
        public DXLabel TextLabel;

        /// <summary>
        /// NPC宝石镶嵌附魔面板
        /// </summary>
        public NPCEnchantingDialog()
        {
            LibraryFile = LibraryFile.GameInter;
            Index = 5700;

            CloseButton = new DXButton  //关闭按钮
            {
                Parent = this,
                Location = new Point(158, 1),
                LibraryFile = LibraryFile.GameInter,
                Index = 1221,
                Hint = "关闭".Lang(),
            };
            CloseButton.MouseEnter += (o, e) => CloseButton.Index = 1220;  //鼠标进入
            CloseButton.MouseLeave += (o, e) => CloseButton.Index = 1221;  //鼠标离开
            CloseButton.MouseClick += (o, e) => Visible = false;        //鼠标点击

            TextLabel = new DXLabel
            {
                Text = "装备附魔".Lang(),
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),             //字体 格式  文字大小
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,   //绘图格式  文本格式标志 垂直中心  水平居中
                Parent = this,
                Size = new Size(80, 20),
                ForeColour = Color.FromArgb(190, 165, 110),        //前色
                BorderColour = Color.Black,       //边框颜色
            };
            TextLabel.Location = new Point(Size.Width / 2 - TextLabel.Size.Width / 2, 8);

            SureButton = new DXButton //开始按钮
            {
                Parent = this,
                Index = 5707,
                LibraryFile = LibraryFile.GameInter,
            };
            SureButton.Location = new Point(Size.Width / 2 - SureButton.Size.Width - 5, 278);

            CancelButton = new DXButton  //取消按钮
            {
                Parent = this,
                Index = 4722,
                LibraryFile = LibraryFile.GameInter,
            };
            CancelButton.Location = new Point(Size.Width / 2 + 5, 278);
            CancelButton.MouseClick += (o, e) => Visible = false;
        }
    }

    /// <summary>
    /// NPC宝石附魔石合成功能
    /// </summary>
    public sealed class NPCEnchantmentSynthesisDialog : DXImageControl
    {
        public DXButton CloseButton, SureButton, CancelButton;
        public DXLabel TextLabel;
        public DXItemGrid MaterialGrid1, MaterialGrid2, MaterialGrid3, ComposeGrid;
        public DXLabel Material1, Material2, Material3, Compose;
        public DXAnimatedControl ComposeCartoon;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
                GameScene.Game.InventoryBox.Visible = true;

            if (!IsVisible)
            {
                MaterialGrid1.ClearLinks();
                MaterialGrid2.ClearLinks();
                MaterialGrid3.ClearLinks();
            }
        }

        /// <summary>
        /// NPC宝石附魔合成面板
        /// </summary>
        public NPCEnchantmentSynthesisDialog()
        {
            LibraryFile = LibraryFile.GameInter;
            Index = 5701;

            CloseButton = new DXButton  //关闭按钮
            {
                Parent = this,
                Location = new Point(160, 1),
                LibraryFile = LibraryFile.GameInter,
                Index = 1221,
                Hint = "关闭".Lang(),
            };
            CloseButton.MouseEnter += (o, e) => CloseButton.Index = 1220;  //鼠标进入
            CloseButton.MouseLeave += (o, e) => CloseButton.Index = 1221;  //鼠标离开
            CloseButton.MouseClick += (o, e) => Visible = false;        //鼠标点击

            TextLabel = new DXLabel
            {
                Text = "附魔石合成".Lang(),
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),             //字体 格式  文字大小
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,   //绘图格式  文本格式标志 垂直中心  水平居中
                Parent = this,
                Size = new Size(80, 20),
                ForeColour = Color.FromArgb(190, 165, 110),        //前色
                BorderColour = Color.Black,       //边框颜色
            };
            TextLabel.Location = new Point(Size.Width / 2 - TextLabel.Size.Width / 2, 8);

            CancelButton = new DXButton  //取消按钮
            {
                Parent = this,
                Index = 4722,
                LibraryFile = LibraryFile.GameInter,
            };
            CancelButton.Location = new Point(Size.Width / 2 + 5, 285);
            CancelButton.MouseClick += (o, e) => Visible = false;

            MaterialGrid1 = new DXItemGrid                    //材料格子1
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.GemMaterial,
                BackColour = Color.FromArgb(24, 12, 12),      //背景色
                Border = true,                                //边界
                BorderColour = Color.FromArgb(99, 83, 50),    //边框颜色
                Linked = true,
            };
            MaterialGrid1.Location = new Point((Size.Width - MaterialGrid1.Size.Width) / 2, 45);
            MaterialGrid1.MouseEnter += (s, e) => Material1.Visible = true;
            MaterialGrid1.MouseLeave += (s, e) => Material1.Visible = false;

            Material1 = new DXLabel
            {
                Parent = this,
                Hint = "可以放置相同等级的宝石进行升级".Lang(),
                Size = new Size(6, 6),
            };
            Material1.Location = new Point((Size.Width - Material1.Size.Width) / 2, 60);

            MaterialGrid2 = new DXItemGrid                    //材料格子2
            {
                GridSize = new Size(1, 1),
                Parent = this,
                Location = new Point(25, 125),
                GridType = GridType.GemMaterial,
                BackColour = Color.FromArgb(24, 12, 12),      //背景色
                Border = true,                                //边界
                BorderColour = Color.FromArgb(99, 83, 50),    //边框颜色
                Linked = true
            };
            MaterialGrid2.MouseEnter += (s, e) => Material2.Visible = true;
            MaterialGrid2.MouseLeave += (s, e) => Material2.Visible = false;

            Material2 = new DXLabel
            {
                Parent = this,
                Location = new Point(40, 140),
                Hint = "可以放置相同等级的宝石进行升级".Lang(),
                Size = new Size(6, 6),
            };

            MaterialGrid3 = new DXItemGrid                    //材料格子3
            {
                GridSize = new Size(1, 1),
                Parent = this,
                Location = new Point(130, 125),
                GridType = GridType.GemMaterial,
                BackColour = Color.FromArgb(24, 12, 12),      //背景色
                Border = true,                                //边界
                BorderColour = Color.FromArgb(99, 83, 50),    //边框颜色
                Linked = true
            };
            MaterialGrid3.MouseEnter += (s, e) => Material3.Visible = true;
            MaterialGrid3.MouseLeave += (s, e) => Material3.Visible = false;

            Material3 = new DXLabel
            {
                Parent = this,
                Location = new Point(145, 140),
                Hint = "可以放置相同等级的宝石进行升级".Lang(),
                Size = new Size(6, 6),
            };

            ComposeGrid = new DXItemGrid                      //合成结果
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.ChanceMaterial,
                Border = false,                               //边界
                Linked = true
            };
            ComposeGrid.Location = new Point((Size.Width - ComposeGrid.Size.Width) / 2 - 2, 213);
            ComposeGrid.MouseEnter += (s, e) => Compose.Visible = true;
            ComposeGrid.MouseLeave += (s, e) => Compose.Visible = false;

            Compose = new DXLabel
            {
                Parent = this,
                Location = new Point(145, 140),
                Hint = "显示升级成功后所获得的宝石".Lang(),
                Size = new Size(6, 6),
            };
            Compose.Location = new Point((Size.Width - Compose.Size.Width) / 2 - 2, 228);

            foreach (DXItemCell cell in MaterialGrid1.Grid)   //遍历格子
            {
                cell.LinkChanged += (o, e) => UpdateButton();   //链接变化
            }
            foreach (DXItemCell cell in MaterialGrid2.Grid)
            {
                cell.LinkChanged += (o, e) => UpdateButton();
            }
            foreach (DXItemCell cell in MaterialGrid3.Grid)
            {
                cell.LinkChanged += (o, e) => UpdateButton();
            }

            ComposeCartoon = new DXAnimatedControl  //合成过程动画
            {
                LibraryFile = LibraryFile.GameInter,
                BaseIndex = 5710,
                Animated = true,
                AnimationDelay = TimeSpan.FromSeconds(5),
                FrameCount = 20,
                Parent = this,
                Location = new Point(0, 25),
                Loop = false,
                Visible = false,
                Blend = true,
            };

            SureButton = new DXButton //开始按钮
            {
                Parent = this,
                Index = 5707,
                LibraryFile = LibraryFile.GameInter,
            };
            SureButton.Location = new Point(Size.Width / 2 - SureButton.Size.Width - 5, 285);
            SureButton.MouseClick += (o, e) =>   //鼠标点击开始按钮
            {
                if (GameScene.Game.Observer) return;   //如果是观察者跳出

                List<CellLinkInfo> Material1 = new List<CellLinkInfo>();
                foreach (DXItemCell cell in MaterialGrid1.Grid)
                {
                    if (cell.Link == null) continue;

                    Material1.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }

                List<CellLinkInfo> Material2 = new List<CellLinkInfo>();
                foreach (DXItemCell cell in MaterialGrid2.Grid)
                {
                    if (cell.Link == null) continue;

                    Material2.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }

                List<CellLinkInfo> Material3 = new List<CellLinkInfo>();
                foreach (DXItemCell cell in MaterialGrid3.Grid)
                {
                    if (cell.Link == null) continue;

                    Material3.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                    cell.Link.Locked = true;
                    cell.Link = null;
                }

                if (Material1.Count < 1 && Material2.Count < 1 || Material1.Count < 1 && Material3.Count < 1 || Material2.Count < 1 && Material3.Count < 1)
                {
                    GameScene.Game.ReceiveChat("你需要放入2个以上相同的宝石才能升级合成".Lang(), MessageType.System);
                    return;
                }

                ComposeCartoon.Visible = true;

                //发包精炼石合成
                //CEnvir.Enqueue(new C.NPCEnchantmentSynthesis { MaterialGrid1 = Material1, MaterialGrid2 = Material2, MaterialGrid3 = Material3 });

            };
        }

        /// <summary>
        /// 更新按钮
        /// </summary>
        public void UpdateButton()
        {
            SureButton.Enabled = false;

            foreach (DXItemCell cell in MaterialGrid1.Grid)
            {
                if (cell.Link == null) return;
                SureButton.Enabled = true;
            }
            foreach (DXItemCell cell in MaterialGrid2.Grid)
            {
                if (cell.Link == null) return;
                SureButton.Enabled = true;
            }
            foreach (DXItemCell cell in MaterialGrid3.Grid)
            {
                if (cell.Link == null) return;
                SureButton.Enabled = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (CloseButton != null)
                {
                    if (!CloseButton.IsDisposed)
                        CloseButton.Dispose();

                    CloseButton = null;
                }

                if (TextLabel != null)
                {
                    if (!TextLabel.IsDisposed)
                        TextLabel.Dispose();

                    TextLabel = null;
                }

                if (SureButton != null)
                {
                    if (!SureButton.IsDisposed)
                        SureButton.Dispose();

                    SureButton = null;
                }

                if (CancelButton != null)
                {
                    if (!CancelButton.IsDisposed)
                        CancelButton.Dispose();

                    CancelButton = null;
                }

                if (MaterialGrid1 != null)
                {
                    if (!MaterialGrid1.IsDisposed)
                        MaterialGrid1.Dispose();

                    MaterialGrid1 = null;
                }

                if (Material1 != null)
                {
                    if (!Material1.IsDisposed)
                        Material1.Dispose();

                    Material1 = null;
                }

                if (MaterialGrid2 != null)
                {
                    if (!MaterialGrid2.IsDisposed)
                        MaterialGrid2.Dispose();

                    MaterialGrid2 = null;
                }

                if (Material2 != null)
                {
                    if (!Material2.IsDisposed)
                        Material2.Dispose();

                    Material2 = null;
                }

                if (MaterialGrid3 != null)
                {
                    if (!MaterialGrid3.IsDisposed)
                        MaterialGrid3.Dispose();

                    MaterialGrid3 = null;
                }

                if (Material3 != null)
                {
                    if (!Material3.IsDisposed)
                        Material3.Dispose();

                    Material3 = null;
                }

                if (ComposeGrid != null)
                {
                    if (!ComposeGrid.IsDisposed)
                        ComposeGrid.Dispose();

                    ComposeGrid = null;
                }

                if (Compose != null)
                {
                    if (!Compose.IsDisposed)
                        Compose.Dispose();

                    Compose = null;
                }
            }
        }
    }

    #endregion

    #region 新版武器升级

    /// <summary>
    /// 新版武器升级
    /// </summary>
    public sealed class NPCWeaponUpgradeDialog : DXWindow
    {
        #region dx

        /// <summary>
        /// 背景图
        /// </summary>
        public DXImageControl Background;
        /// <summary>
        /// 破坏标签
        /// </summary>
        public DXLabel DestroyLabel;
        /// <summary>
        /// 全系列标签
        /// </summary>
        public DXLabel FullRangeLabel;
        /// <summary>
        /// 元素标签
        /// </summary>
        public DXLabel ElementLabel;
        /// <summary>
        /// 持久标签
        /// </summary>
        public DXLabel LastingLabel;
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public DXButton ShutButton;
        /// <summary>
        /// 破坏按钮
        /// </summary>
        public DXImageControl DestroyImage;
        /// <summary>
        /// 全系列按钮
        /// </summary>
        public DXImageControl FullRangeImage;
        /// <summary>
        /// 选择元素的列表框
        /// </summary>
        public DXComboBox CombElementChanged;
        /// <summary>
        /// 元素火按钮
        /// </summary>
        public DXImageControl FireImage;
        /// <summary>
        /// 元素冰按钮
        /// </summary>
        public DXImageControl IceImage;
        /// <summary>
        /// 元素雷按钮
        /// </summary>
        public DXImageControl LightningImage;
        /// <summary>
        /// 元素风按钮
        /// </summary>
        public DXImageControl WindImage;
        /// <summary>
        /// 元素神圣按钮
        /// </summary>
        public DXImageControl HolyImage;
        /// <summary>
        /// 元素暗黑按钮
        /// </summary>
        public DXImageControl DarkImage;
        /// <summary>
        /// 元素幻影按钮
        /// </summary>
        public DXImageControl PhantomImage;
        /// <summary>
        /// 持久按钮
        /// </summary>
        public DXImageControl LastingImage;
        /// <summary>
        /// 开始制炼按钮
        /// </summary>
        public DXButton StartButton;
        /// <summary>
        /// 武器等级标签
        /// </summary>
        public DXLabel IncrementLabel;
        /// <summary>
        /// 成功几率标签
        /// </summary>
        public DXLabel SuccessRateLabel;
        /// <summary>
        /// 失败几率标签
        /// </summary>
        public DXLabel FailureRateLabel;
        /// <summary>
        /// 需要时间标签
        /// </summary>
        public DXLabel TimeCostLabel;
        /// <summary>
        /// 成功几率值标签
        /// </summary>
        public DXLabel SuccessRateValueLabel;
        /// <summary>
        /// 失败几率值标签
        /// </summary>
        public DXLabel FailureRateValueLabel;
        /// <summary>
        /// 需要时间值标签
        /// </summary>
        public DXLabel TimeCostValueLabel;
        /// <summary>
        /// 所需金币标签
        /// </summary>
        public DXLabel SpendGoldLabel;
        /// <summary>
        /// 待升级的武器格子
        /// </summary>
        public DXItemGrid UpgradeWeaponGrid;
        /// <summary>
        /// 黑铁材料的格子
        /// </summary>
        public DXItemGrid BlackIronGrid;
        /// <summary>
        /// 初级碎片格子
        /// </summary>
        public DXItemGrid Fragment1Grid;
        /// <summary>
        /// 中级碎片格子
        /// </summary>
        public DXItemGrid Fragment2Grid;
        /// <summary>
        /// 高级碎片格子
        /// </summary>
        public DXItemGrid Fragment3Grid;
        /// <summary>
        /// 制炼石格子
        /// </summary>
        public DXItemGrid RefinementStoneGrid;
        /// <summary>
        /// 剩余时间标签
        /// </summary>
        public DXLabel RemainingTimeLabel;
        /// <summary>
        /// 升级成功图片
        /// </summary>
        public DXImageControl UpdateSuccessful;
        #endregion

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        /// <summary>
        /// 默认升级类型 攻击
        /// </summary>
        private RefineType currentSelection = RefineType.DC;
        /// <summary>
        /// 当前升级类型选择
        /// </summary>
        public RefineType CurrentSelection
        {
            get
            {
                return currentSelection;
            }
            set
            {
                currentSelection = value;
                UpdateRequirements(UpgradeWeaponGrid?.Grid[0]?.Item);
            }
        }
        /// <summary>
        /// 元素默认类型火
        /// </summary>
        private Element selectedElement = Element.Fire;
        private ClientRefineInfo _refineInfo;
        /// <summary>
        /// 精炼信息
        /// </summary>
        public ClientRefineInfo RefineInfo
        {
            get
            {
                return _refineInfo;
            }
            set
            {
                _refineInfo = value;
                RefreshRetrieveInfo();
            }
        }
        /// <summary>
        /// 剩余时间
        /// </summary>
        public TimeSpan RemainingTime => RefineInfo == null || RefineInfo.RetrieveTime <= CEnvir.Now ? TimeSpan.Zero : RefineInfo.RetrieveTime - CEnvir.Now;

        /// <summary>
        /// 新版武器升级
        /// </summary>
        public NPCWeaponUpgradeDialog()
        {
            Size = new Size(360, 510);
            Opacity = 0F;
            HasTitle = false;
            CloseButton.Visible = false;    //关闭按钮不显示

            Background = new DXImageControl   //背景图
            {
                Index = 2800,
                Size = new Size(350, 505),
                LibraryFile = LibraryFile.GameInter2,
                Parent = this,
            };

            UpdateSuccessful = new DXImageControl  //升级成功图
            {
                Index = 2811,
                LibraryFile = LibraryFile.GameInter2,
                Parent = this,
                Location = new Point(12, 100),
                Visible = false,
            };

            #region 选项按钮

            DestroyLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(12, 41),
                ForeColour = Color.FromArgb(135, 130, 135),
                AutoSize = false,
                Size = new Size(80, 42),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };
            DestroyLabel.MouseClick += (o, e) =>
            {
                if (RefineInfo != null)
                {
                    GameScene.Game.ReceiveChat("武器升级中无法切换".Lang(), MessageType.System);
                    return;
                }
                DestroyImage.Visible = true;
                FullRangeImage.Visible = false;
                LastingImage.Visible = false;
                FireImage.Visible = false;
                IceImage.Visible = false;
                LightningImage.Visible = false;
                WindImage.Visible = false;
                HolyImage.Visible = false;
                DarkImage.Visible = false;
                PhantomImage.Visible = false;
                CurrentSelection = RefineType.DC;
            };

            FullRangeLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(95, 41),
                AutoSize = false,
                Size = new Size(80, 42),
            };
            FullRangeLabel.MouseClick += (o, e) =>
            {
                if (RefineInfo != null)
                {
                    GameScene.Game.ReceiveChat("武器升级中无法切换".Lang(), MessageType.System);
                    return;
                }
                DestroyImage.Visible = false;
                FullRangeImage.Visible = true;
                LastingImage.Visible = false;
                FireImage.Visible = false;
                IceImage.Visible = false;
                LightningImage.Visible = false;
                WindImage.Visible = false;
                HolyImage.Visible = false;
                DarkImage.Visible = false;
                PhantomImage.Visible = false;
                CurrentSelection = RefineType.SpellPower;
            };

            ElementLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(175, 41),
                AutoSize = false,
                Size = new Size(80, 42),
            };
            ElementLabel.MouseClick += (o, e) =>
            {
                if (RefineInfo != null)
                {
                    GameScene.Game.ReceiveChat("武器升级中无法切换".Lang(), MessageType.System);
                    return;
                }
                DestroyImage.Visible = false;
                FullRangeImage.Visible = false;
                LastingImage.Visible = false;
                CombElementChanged.Visible = true;
            };

            LastingLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(258, 41),
                AutoSize = false,
                Size = new Size(80, 42),
            };
            LastingLabel.MouseClick += (o, e) =>
            {
                if (RefineInfo != null)
                {
                    GameScene.Game.ReceiveChat("武器升级中无法切换".Lang(), MessageType.System);
                    return;
                }
                DestroyImage.Visible = false;
                FullRangeImage.Visible = false;
                LastingImage.Visible = true;
                FireImage.Visible = false;
                IceImage.Visible = false;
                LightningImage.Visible = false;
                WindImage.Visible = false;
                HolyImage.Visible = false;
                DarkImage.Visible = false;
                PhantomImage.Visible = false;
                CurrentSelection = RefineType.Durability;
            };

            ShutButton = new DXButton
            {
                Parent = this,
                Index = 113,
                LibraryFile = LibraryFile.Interface,
                Hint = "关闭".Lang(),
                Location = new Point(315, 1),
            };
            ShutButton.MouseEnter += (o, e) => CloseButton.Index = 112;
            ShutButton.MouseLeave += (o, e) => CloseButton.Index = 113;
            ShutButton.MouseClick += (o, e) => Visible = false;

            DestroyImage = new DXImageControl
            {
                Parent = this,
                Index = 2790,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(11, 41),
                Visible = true,
            };
            FullRangeImage = new DXImageControl
            {
                Parent = this,
                Index = 2791,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(93, 41),
                Visible = false,
            };

            #endregion

            #region 元素选择框

            CombElementChanged = new DXComboBox  //选择元素
            {
                Parent = this,
                Border = true,
                BorderColour = Color.FromArgb(67, 64, 57),
                Visible = false,
            };
            CombElementChanged.Location = new Point(178, 70);
            new DXListBoxItem
            {
                Parent = CombElementChanged.ListBox,
                Label = { Text = $"火".Lang() },
                Item = Element.Fire,
            };
            new DXListBoxItem
            {
                Parent = CombElementChanged.ListBox,
                Label = { Text = $"冰".Lang() },
                Item = Element.Ice,
            };
            new DXListBoxItem
            {
                Parent = CombElementChanged.ListBox,
                Label = { Text = $"雷".Lang() },
                Item = Element.Lightning,
            };
            new DXListBoxItem
            {
                Parent = CombElementChanged.ListBox,
                Label = { Text = $"风".Lang() },
                Item = Element.Wind,
            };
            new DXListBoxItem
            {
                Parent = CombElementChanged.ListBox,
                Label = { Text = $"神圣".Lang() },
                Item = Element.Holy,
            };
            new DXListBoxItem
            {
                Parent = CombElementChanged.ListBox,
                Label = { Text = $"暗黑".Lang() },
                Item = Element.Dark,
            };
            new DXListBoxItem
            {
                Parent = CombElementChanged.ListBox,
                Label = { Text = $"幻影".Lang() },
                Item = Element.Phantom,
            };
            CombElementChanged.Size = new Size(75, 18);

            CombElementChanged.SelectedItemChanged += (o, e) =>
            {
                DXComboBox comb = o as DXComboBox;
                UpdateElementChanged((Element)comb.ListBox.SelectedItem.Item);
            };

            #endregion

            #region 元素按钮图
            FireImage = new DXImageControl
            {
                Parent = this,
                Index = 2792,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(175, 41),
                Visible = false,
            };
            FireImage.MouseClick += (o, e) =>
            {
                CombElementChanged.Visible = true;
            };

            IceImage = new DXImageControl
            {
                Parent = this,
                Index = 2793,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(175, 41),
                Visible = false,
            };
            IceImage.MouseClick += (o, e) =>
            {
                CombElementChanged.Visible = true;
            };

            LightningImage = new DXImageControl
            {
                Parent = this,
                Index = 2794,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(175, 41),
                Visible = false,
            };
            LightningImage.MouseClick += (o, e) =>
            {
                CombElementChanged.Visible = true;
            };

            WindImage = new DXImageControl
            {
                Parent = this,
                Index = 2795,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(175, 41),
                Visible = false,
            };
            WindImage.MouseClick += (o, e) =>
            {
                CombElementChanged.Visible = true;
            };

            HolyImage = new DXImageControl
            {
                Parent = this,
                Index = 2796,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(175, 41),
                Visible = false,
            };
            HolyImage.MouseClick += (o, e) =>
            {
                CombElementChanged.Visible = true;
            };

            DarkImage = new DXImageControl
            {
                Parent = this,
                Index = 2797,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(175, 41),
                Visible = false,
            };
            DarkImage.MouseClick += (o, e) =>
            {
                CombElementChanged.Visible = true;
            };

            PhantomImage = new DXImageControl
            {
                Parent = this,
                Index = 2798,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(175, 41),
                Visible = false,
            };
            PhantomImage.MouseClick += (o, e) =>
            {
                CombElementChanged.Visible = true;
            };

            LastingImage = new DXImageControl
            {
                Parent = this,
                Index = 2799,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(257, 41),
                Visible = false,
            };
            FireImage.MouseClick += (o, e) =>
            {
                CombElementChanged.Visible = true;
            };
            #endregion

            #region 合成信息

            IncrementLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.FromArgb(220, 220, 220),
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Size = new Size(80, 16),
                Location = new Point(230, 116),
                Text = "?? → ???",
                Visible = true,
            };

            SuccessRateLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.FromArgb(220, 220, 220),
                AutoSize = false,
                DrawFormat = TextFormatFlags.Left,
                Size = new Size(70, 16),
                Location = new Point(220, 148),
                Text = "成功率".Lang(),
                Visible = true,
            };
            SuccessRateValueLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.FromArgb(220, 220, 220),
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Size = new Size(70, 16),
                Location = new Point(260, 148),
                Text = "%",
                Visible = true,
            };

            FailureRateLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.FromArgb(220, 220, 220),
                AutoSize = false,
                DrawFormat = TextFormatFlags.Left,
                Size = new Size(70, 16),
                Location = new Point(220, 183),
                Text = "失败率".Lang(),
                Visible = true,
            };
            FailureRateValueLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.FromArgb(220, 220, 220),
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Size = new Size(70, 16),
                Location = new Point(260, 183),
                Text = "%",
                Visible = true,
            };

            TimeCostLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.FromArgb(220, 220, 220),
                AutoSize = false,
                DrawFormat = TextFormatFlags.Left,
                Size = new Size(70, 16),
                Location = new Point(220, 218),
                Text = "等待时间".Lang(),
                Visible = true,
            };
            TimeCostValueLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.FromArgb(220, 220, 220),
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Size = new Size(70, 16),
                Location = new Point(260, 218),
                Text = "分",
                Visible = true,
            };

            SpendGoldLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.FromArgb(220, 220, 220),
                AutoSize = false,
                DrawFormat = TextFormatFlags.Right,
                Size = new Size(200, 16),
                Location = new Point(80, 274),
                Text = "0",
                Visible = true,
            };

            #endregion

            UpgradeWeaponGrid = new DXItemGrid   //放要升级的武器
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.UpgradeWeapon,
                DrawTexture = true,
                BackColour = Color.FromArgb(24, 12, 12),
                Linked = true,
                Location = new Point(85, 155),
                Visible = true,
            };
            UpgradeWeaponGrid.Grid[0].LinkChanged += MaterialChanged;

            BlackIronGrid = new DXItemGrid   //放黑铁格子
            {
                GridSize = new Size(5, 1),
                Parent = this,
                GridType = GridType.RefineBlackIronOre,
                DrawTexture = true,
                BackColour = Color.FromArgb(24, 12, 12),
                Linked = true,
                Location = new Point(80, 305),
                Visible = true,
            };
            foreach (var cell in BlackIronGrid.Grid)
            {
                cell.LinkChanged += MaterialChanged;
            }

            Fragment1Grid = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.MasterRefineFragment1,
                DrawTexture = true,
                BackColour = Color.FromArgb(24, 12, 12),
                Linked = true,
                Location = new Point(80, 350),
                Visible = true,
                Hint = "放入初级碎片".Lang(),
            };
            foreach (var cell in Fragment1Grid.Grid)
            {
                cell.LinkChanged += MaterialChanged;
            }

            Fragment2Grid = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.MasterRefineFragment2,
                DrawTexture = true,
                BackColour = Color.FromArgb(24, 12, 12),
                Linked = true,
                Location = new Point(120, 350),
                Visible = true,
                Hint = "放入中级碎片".Lang(),
            };
            foreach (var cell in Fragment2Grid.Grid)
            {
                cell.LinkChanged += MaterialChanged;
            }

            Fragment3Grid = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.MasterRefineFragment3,
                DrawTexture = true,
                BackColour = Color.FromArgb(24, 12, 12),
                Linked = true,
                Location = new Point(160, 350),
                Visible = true,
                Hint = "放入高级碎片".Lang(),
            };
            foreach (var cell in Fragment3Grid.Grid)
            {
                cell.LinkChanged += MaterialChanged;
            }

            RefinementStoneGrid = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.MasterRefineStone,
                DrawTexture = true,
                BackColour = Color.FromArgb(24, 12, 12),
                Linked = true,
                Location = new Point(80, 395),
                Visible = true,
                Hint = "放入制炼石".Lang(),
            };
            foreach (var cell in RefinementStoneGrid.Grid)
            {
                cell.LinkChanged += MaterialChanged;
            }

            StartButton = new DXButton
            {
                Parent = this,
                Index = 2823,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(135, 450),
            };
            StartButton.MouseClick += StartOrRetrieve;

            RemainingTimeLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.DarkRed,
                AutoSize = true,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Location = new Point(145, 420),
                Text = "00:00:00",
                Visible = true,
            };
            RemainingTimeLabel.AfterDraw += (sender, args) =>
            {
                if (RefineInfo != null)
                {
                    StartButton.Index = 2852;
                    if (RefineInfo.RetrieveTime <= CEnvir.Now)
                    {
                        RemainingTimeLabel.Text = "00:00:00";
                    }
                    else
                    {
                        RemainingTimeLabel.Text = RemainingTime.ToString(@"hh\:mm\:ss");
                    }
                }
            };
        }
        /// <summary>
        /// 视图改变时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);
            if (GameScene.Game.InventoryBox != null)
            {
                if (base.IsVisible)
                {
                    GameScene.Game.InventoryBox.Visible = true;
                }
                if (!base.IsVisible)
                {
                    UpgradeWeaponGrid.ClearLinks();
                    BlackIronGrid.ClearLinks();
                    Fragment1Grid.ClearLinks();
                    Fragment2Grid.ClearLinks();
                    Fragment3Grid.ClearLinks();
                    RefinementStoneGrid.ClearLinks();
                }
            }

            // 重新打开NPC时 刷新一下剩余时间
            RefreshRetrieveInfo();
        }
        /// <summary>
        /// 更新元素变化
        /// </summary>
        /// <param name="element"></param>
        private void UpdateElementChanged(Element element)
        {
            selectedElement = element;
            switch (element)
            {
                case Element.Fire:
                    CombElementChanged.Visible = false;
                    DestroyImage.Visible = false;
                    FullRangeImage.Visible = false;
                    LastingImage.Visible = false;
                    FireImage.Visible = true;
                    IceImage.Visible = false;
                    LightningImage.Visible = false;
                    WindImage.Visible = false;
                    HolyImage.Visible = false;
                    DarkImage.Visible = false;
                    PhantomImage.Visible = false;
                    CurrentSelection = RefineType.Fire;
                    break;
                case Element.Ice:
                    CombElementChanged.Visible = false;
                    DestroyImage.Visible = false;
                    FullRangeImage.Visible = false;
                    LastingImage.Visible = false;
                    FireImage.Visible = false;
                    IceImage.Visible = true;
                    LightningImage.Visible = false;
                    WindImage.Visible = false;
                    HolyImage.Visible = false;
                    DarkImage.Visible = false;
                    PhantomImage.Visible = false;
                    CurrentSelection = RefineType.Ice;
                    break;
                case Element.Lightning:
                    CombElementChanged.Visible = false;
                    DestroyImage.Visible = false;
                    FullRangeImage.Visible = false;
                    LastingImage.Visible = false;
                    FireImage.Visible = false;
                    IceImage.Visible = false;
                    LightningImage.Visible = true;
                    WindImage.Visible = false;
                    HolyImage.Visible = false;
                    DarkImage.Visible = false;
                    PhantomImage.Visible = false;
                    CurrentSelection = RefineType.Lightning;
                    break;
                case Element.Wind:
                    CombElementChanged.Visible = false;
                    DestroyImage.Visible = false;
                    FullRangeImage.Visible = false;
                    LastingImage.Visible = false;
                    FireImage.Visible = false;
                    IceImage.Visible = false;
                    LightningImage.Visible = false;
                    WindImage.Visible = true;
                    HolyImage.Visible = false;
                    DarkImage.Visible = false;
                    PhantomImage.Visible = false;
                    CurrentSelection = RefineType.Wind;
                    break;
                case Element.Holy:
                    CombElementChanged.Visible = false;
                    DestroyImage.Visible = false;
                    FullRangeImage.Visible = false;
                    LastingImage.Visible = false;
                    FireImage.Visible = false;
                    IceImage.Visible = false;
                    LightningImage.Visible = false;
                    WindImage.Visible = false;
                    HolyImage.Visible = true;
                    DarkImage.Visible = false;
                    PhantomImage.Visible = false;
                    CurrentSelection = RefineType.Holy;
                    break;
                case Element.Dark:
                    CombElementChanged.Visible = false;
                    DestroyImage.Visible = false;
                    FullRangeImage.Visible = false;
                    LastingImage.Visible = false;
                    FireImage.Visible = false;
                    IceImage.Visible = false;
                    LightningImage.Visible = false;
                    WindImage.Visible = false;
                    HolyImage.Visible = false;
                    DarkImage.Visible = true;
                    PhantomImage.Visible = false;
                    CurrentSelection = RefineType.Dark;
                    break;
                case Element.Phantom:
                    CombElementChanged.Visible = false;
                    DestroyImage.Visible = false;
                    FullRangeImage.Visible = false;
                    LastingImage.Visible = false;
                    FireImage.Visible = false;
                    IceImage.Visible = false;
                    LightningImage.Visible = false;
                    WindImage.Visible = false;
                    HolyImage.Visible = false;
                    DarkImage.Visible = false;
                    PhantomImage.Visible = true;
                    CurrentSelection = RefineType.Phantom;
                    break;
            }
        }
        /// <summary>
        /// 材料改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MaterialChanged(object sender, EventArgs args)
        {
            ClientUserItem weapon = UpgradeWeaponGrid.Grid[0].Item;

            if (UpgradeWeaponGrid.Grid[0] == null || UpgradeWeaponGrid.Grid[0].Item == null)
            {
                UpdateRequirements(weapon);
                return;
            }

            UpdateRequirements(weapon);
        }
        /// <summary>
        /// 更新要求
        /// </summary>
        /// <param name="weapon">武器等空</param>
        private void UpdateRequirements(ClientUserItem weapon = null)
        {
            StartButton.Enabled = false;
            if (weapon == null)
            {
                IncrementLabel.Text = "? → ?";
                SuccessRateValueLabel.Text = "0%";
                FailureRateValueLabel.Text = "0%";
                TimeCostValueLabel.Text = "0" + "分钟".Lang();
                SpendGoldLabel.Text = "0";
                return;
            }

            int level = 0;
            int maxAddAllowed = Globals.WeaponUpgradeInfoList.Binding.Max(x => x.Increment);
            switch (CurrentSelection)
            {
                case RefineType.DC:
                    level = weapon.AddedStats[Stat.MaxDC] + 1;
                    break;
                case RefineType.SpellPower:
                    level = Math.Max(weapon.AddedStats[Stat.MaxMC], weapon.AddedStats[Stat.MaxSC]) + 1;
                    break;
                case RefineType.Durability:
                    //持久无限升级
                    level = 0;
                    break;
                case RefineType.Fire:
                case RefineType.Ice:
                case RefineType.Lightning:
                case RefineType.Wind:
                case RefineType.Holy:
                case RefineType.Dark:
                case RefineType.Phantom:
                    level = weapon.AddedStats.GetWeaponElementValue() + 1;
                    break;
                default:
                    return;
            }

            if (level > maxAddAllowed)
            {
                GameScene.Game.ReceiveChat("你的武器达到升级上限无法再升级了".Lang(), MessageType.System);
                return;
            }

            int goldRequired, ironRequired, frag1Required, frag2Required, frag3Required, stoneRequired;
            if (CurrentSelection == RefineType.Durability)
            {
                IncrementLabel.Text = $"{weapon.MaxDurability / 1000} → {weapon.MaxDurability / 1000 + 1}";
                SuccessRateValueLabel.Text = $"100%";
                FailureRateValueLabel.Text = $"0%";
                TimeCostValueLabel.Text = $"0" + "分钟".Lang();
                SpendGoldLabel.Text = $"100000";

                goldRequired = 100000;
                ironRequired = 5;
                frag1Required = 0;
                frag2Required = 0;
                frag3Required = 0;
                stoneRequired = 1;
                StartButton.Hint = $"WeaponUpgradeNew.StartHint".Lang(ironRequired, frag1Required, frag2Required, frag3Required, stoneRequired);
            }
            else
            {
                WeaponUpgradeNew upgradeInfo = Globals.WeaponUpgradeInfoList.Binding.FirstOrDefault(x => level == x.Increment);
                if (upgradeInfo == null) return;
                if (currentSelection == RefineType.Fire
                    || currentSelection == RefineType.Ice
                    || currentSelection == RefineType.Lightning
                    || currentSelection == RefineType.Wind
                    || currentSelection == RefineType.Holy
                    || currentSelection == RefineType.Dark
                    || currentSelection == RefineType.Phantom)
                {
                    if (weapon.AddedStats.GetWeaponElementAsElement() == Element.None)
                    {
                        //武器没有攻击元素 可以任意选择
                        string element = Functions.GetEnumDescription(selectedElement);
                        element = element.Substring(0, element.IndexOf('（'));
                        IncrementLabel.Text = $"{element} {upgradeInfo.Increment - 1} → {upgradeInfo.Increment}";
                    }
                    else
                    {
                        //武器有攻击元素 只能升级现在的元素
                        string element = Functions.GetEnumDescription(weapon.AddedStats.GetWeaponElementAsElement());
                        element = element.Substring(0, element.IndexOf('（'));
                        IncrementLabel.Text = $"{element} {upgradeInfo.Increment - 1} → {upgradeInfo.Increment}";
                    }
                }
                else
                {
                    IncrementLabel.Text = $"{upgradeInfo.Increment - 1} → {upgradeInfo.Increment}";
                }

                SuccessRateValueLabel.Text = $"{upgradeInfo.SuccessRate}%";
                FailureRateValueLabel.Text = $"{upgradeInfo.FailureRate}%";
                TimeCostValueLabel.Text = $"{upgradeInfo.TimeCost}" + "分钟".Lang();
                SpendGoldLabel.Text = $"{upgradeInfo.SpendGold}";


                goldRequired = upgradeInfo.SpendGold;
                ironRequired = upgradeInfo.BlackIronOre;
                frag1Required = upgradeInfo.BasicFragment;
                frag2Required = upgradeInfo.AdvanceFragment;
                frag3Required = upgradeInfo.SeniorFragment;
                stoneRequired = upgradeInfo.RefinementStone;
                StartButton.Hint = $"WeaponUpgradeNew.StartHint".Lang(ironRequired, frag1Required, frag2Required, frag3Required, stoneRequired);
            }

            for (int i = 0; i < ironRequired; i++)
            {
                if (BlackIronGrid.Grid[i] == null)
                {
                    return;
                }
            }

            if (frag1Required > 0)
            {
                if (Fragment1Grid.Grid[0].Item == null ||
                    Fragment1Grid.Grid[0].Item.Count < frag1Required)
                {
                    return;
                }
            }
            if (frag2Required > 0)
            {
                if (Fragment2Grid.Grid[0].Item == null ||
                    Fragment2Grid.Grid[0].Item.Count < frag2Required)
                {
                    return;
                }
            }
            if (frag3Required > 0)
            {
                if (Fragment3Grid.Grid[0].Item == null ||
                    Fragment3Grid.Grid[0].Item.Count < frag3Required)
                {
                    return;
                }
            }
            if (stoneRequired > 0)
            {
                if (RefinementStoneGrid.Grid[0].Item == null ||
                    RefinementStoneGrid.Grid[0].Item.Count < stoneRequired)
                {
                    return;
                }
            }

            StartButton.Enabled = GameScene.Game.User.Gold > goldRequired;
        }
        /// <summary>
        /// 启动或者取回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void StartOrRetrieve(object sender, EventArgs args)
        {
            if (RefineInfo != null)
            {
                StartButton.Hint = "取回升级武器失败则消失".Lang();
                Retrieve();
                return;
            }

            StartRefine(sender, args);
        }
        /// <summary>
        /// 取回
        /// </summary>
        private void Retrieve()
        {
            if (GameScene.Game.Observer) return;  //如果是观察者跳过

            CEnvir.Enqueue(new C.NPCWeaponUpgradeRetrieve { Index = RefineInfo.Index });
        }
        /// <summary>
        /// 开始精炼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void StartRefine(object sender, EventArgs args)
        {
            if (GameScene.Game.Observer) return;  //如果是观察者跳过

            if (UpgradeWeaponGrid.Grid[0].Item == null) return;   //如果要升级的道具为空，跳过
            CellLinkInfo weapon = new CellLinkInfo { Count = UpgradeWeaponGrid.Grid[0].LinkedCount, GridType = UpgradeWeaponGrid.Grid[0].Link.GridType, Slot = UpgradeWeaponGrid.Grid[0].Link.Slot };
            UpgradeWeaponGrid.Grid[0].Link.Locked = false;
            UpgradeWeaponGrid.Grid[0].Link = null;

            List<CellLinkInfo> iron = new List<CellLinkInfo>();
            List<CellLinkInfo> frag1 = new List<CellLinkInfo>();
            List<CellLinkInfo> frag2 = new List<CellLinkInfo>();
            List<CellLinkInfo> frag3 = new List<CellLinkInfo>();
            List<CellLinkInfo> stone = new List<CellLinkInfo>();

            foreach (DXItemCell cell in BlackIronGrid.Grid)
            {
                if (cell.Link == null) continue;

                iron.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                cell.Link.Locked = false;
                cell.Link = null;
            }
            foreach (DXItemCell cell in Fragment1Grid.Grid)
            {
                if (cell.Link == null) continue;

                frag1.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                cell.Link.Locked = false;
                cell.Link = null;
            }
            foreach (DXItemCell cell in Fragment2Grid.Grid)
            {
                if (cell.Link == null) continue;

                frag2.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                cell.Link.Locked = false;
                cell.Link = null;
            }
            foreach (DXItemCell cell in Fragment3Grid.Grid)
            {
                if (cell.Link == null) continue;

                frag3.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                cell.Link.Locked = false;
                cell.Link = null;
            }
            foreach (DXItemCell cell in RefinementStoneGrid.Grid)
            {
                if (cell.Link == null) continue;

                stone.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                cell.Link.Locked = false;
                cell.Link = null;
            }

            CEnvir.Enqueue(new NPCWeaponUpgrade
            {
                refineType = CurrentSelection,
                weapon = weapon,
                iron = iron,
                frag1 = frag1,
                frag2 = frag2,
                frag3 = frag3,
                stone = stone
            });
        }
        /// <summary>
        /// 刷新精炼信息
        /// </summary>
        private void RefreshRetrieveInfo()
        {
            if (RefineInfo == null)  //精炼信息为空
            {
                StartButton.Index = 2822;
                UpgradeWeaponGrid.Visible = true;
                UpdateSuccessful.Visible = false;
                return;
            }
            if (RemainingTime <= TimeSpan.Zero)  //剩余时间小于0
            {
                StartButton.Index = 2852;
                StartButton.Enabled = true;
                UpgradeWeaponGrid.Visible = false;
                UpdateSuccessful.Visible = true;
                GameScene.Game.ReceiveChat($"WeaponUpgradeNew.Refresh".Lang(RefineInfo?.Weapon?.Info?.Lang(p => p.ItemName)), MessageType.System);
            }
        }

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Background != null)
                {
                    if (!Background.IsDisposed)
                        Background.Dispose();

                    Background = null;
                }

                if (DestroyLabel != null)
                {
                    if (!DestroyLabel.IsDisposed)
                        DestroyLabel.Dispose();

                    DestroyLabel = null;
                }

                if (FullRangeLabel != null)
                {
                    if (!FullRangeLabel.IsDisposed)
                        FullRangeLabel.Dispose();

                    FullRangeLabel = null;
                }

                if (ElementLabel != null)
                {
                    if (!ElementLabel.IsDisposed)
                        ElementLabel.Dispose();

                    ElementLabel = null;
                }

                if (LastingLabel != null)
                {
                    if (!LastingLabel.IsDisposed)
                        LastingLabel.Dispose();

                    LastingLabel = null;
                }

                if (ShutButton != null)
                {
                    if (!ShutButton.IsDisposed)
                        ShutButton.Dispose();

                    ShutButton = null;
                }

                if (DestroyImage != null)
                {
                    if (!DestroyImage.IsDisposed)
                        DestroyImage.Dispose();

                    DestroyImage = null;
                }

                if (FullRangeImage != null)
                {
                    if (!FullRangeImage.IsDisposed)
                        FullRangeImage.Dispose();

                    FullRangeImage = null;
                }

                if (CombElementChanged != null)
                {
                    if (!CombElementChanged.IsDisposed)
                        CombElementChanged.Dispose();

                    CombElementChanged = null;
                }

                if (FireImage != null)
                {
                    if (!FireImage.IsDisposed)
                        FireImage.Dispose();

                    FireImage = null;
                }

                if (IceImage != null)
                {
                    if (!IceImage.IsDisposed)
                        IceImage.Dispose();

                    IceImage = null;
                }

                if (LightningImage != null)
                {
                    if (!LightningImage.IsDisposed)
                        LightningImage.Dispose();

                    LightningImage = null;
                }

                if (WindImage != null)
                {
                    if (!WindImage.IsDisposed)
                        WindImage.Dispose();

                    WindImage = null;
                }

                if (HolyImage != null)
                {
                    if (!HolyImage.IsDisposed)
                        HolyImage.Dispose();

                    HolyImage = null;
                }

                if (DarkImage != null)
                {
                    if (!DarkImage.IsDisposed)
                        DarkImage.Dispose();

                    DarkImage = null;
                }

                if (PhantomImage != null)
                {
                    if (!PhantomImage.IsDisposed)
                        PhantomImage.Dispose();

                    PhantomImage = null;
                }

                if (LastingImage != null)
                {
                    if (!LastingImage.IsDisposed)
                        LastingImage.Dispose();

                    LastingImage = null;
                }

                if (StartButton != null)
                {
                    if (!StartButton.IsDisposed)
                        StartButton.Dispose();

                    StartButton = null;
                }

                if (IncrementLabel != null)
                {
                    if (!IncrementLabel.IsDisposed)
                        IncrementLabel.Dispose();

                    IncrementLabel = null;
                }

                if (SuccessRateLabel != null)
                {
                    if (!SuccessRateLabel.IsDisposed)
                        SuccessRateLabel.Dispose();

                    SuccessRateLabel = null;
                }

                if (FailureRateLabel != null)
                {
                    if (!FailureRateLabel.IsDisposed)
                        FailureRateLabel.Dispose();

                    FailureRateLabel = null;
                }

                if (TimeCostLabel != null)
                {
                    if (!TimeCostLabel.IsDisposed)
                        TimeCostLabel.Dispose();

                    TimeCostLabel = null;
                }

                if (SuccessRateValueLabel != null)
                {
                    if (!SuccessRateValueLabel.IsDisposed)
                        SuccessRateValueLabel.Dispose();

                    SuccessRateValueLabel = null;
                }

                if (FailureRateValueLabel != null)
                {
                    if (!FailureRateValueLabel.IsDisposed)
                        FailureRateValueLabel.Dispose();

                    FailureRateValueLabel = null;
                }

                if (TimeCostValueLabel != null)
                {
                    if (!TimeCostValueLabel.IsDisposed)
                        TimeCostValueLabel.Dispose();

                    TimeCostValueLabel = null;
                }

                if (SpendGoldLabel != null)
                {
                    if (!SpendGoldLabel.IsDisposed)
                        SpendGoldLabel.Dispose();

                    SpendGoldLabel = null;
                }

                if (UpgradeWeaponGrid != null)
                {
                    if (!UpgradeWeaponGrid.IsDisposed)
                        UpgradeWeaponGrid.Dispose();

                    UpgradeWeaponGrid = null;
                }

                if (BlackIronGrid != null)
                {
                    if (!BlackIronGrid.IsDisposed)
                        BlackIronGrid.Dispose();

                    BlackIronGrid = null;
                }

                if (Fragment1Grid != null)
                {
                    if (!Fragment1Grid.IsDisposed)
                        Fragment1Grid.Dispose();

                    Fragment1Grid = null;
                }

                if (Fragment2Grid != null)
                {
                    if (!Fragment2Grid.IsDisposed)
                        Fragment2Grid.Dispose();

                    Fragment2Grid = null;
                }

                if (Fragment3Grid != null)
                {
                    if (!Fragment3Grid.IsDisposed)
                        Fragment3Grid.Dispose();

                    Fragment3Grid = null;
                }

                if (RefinementStoneGrid != null)
                {
                    if (!RefinementStoneGrid.IsDisposed)
                        RefinementStoneGrid.Dispose();

                    RefinementStoneGrid = null;
                }

                if (UpdateSuccessful != null)
                {
                    if (!UpdateSuccessful.IsDisposed)
                        UpdateSuccessful.Dispose();

                    UpdateSuccessful = null;
                }
            }
        }
        #endregion
    }

    #endregion

    #region NPC古代墓碑玩法

    /// <summary>
    /// 古代墓碑
    /// </summary>
    public sealed class NPCAncientTombstoneDialog : DXImageControl
    {
        #region DX
        /// <summary>
        /// 墓碑
        /// </summary>
        public DXImageControl Tombstone;
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public DXButton CloseButton;
        /// <summary>
        /// 第一字符容器
        /// </summary>
        public DXImageControl FirstCharacter;
        public DXImageControl FirstCharacterBase;
        /// <summary>
        /// 第一字符动画
        /// </summary>
        public DXAnimatedControl FirstCharacterRing;
        /// <summary>
        /// 第二字符容器
        /// </summary>
        public DXImageControl SecondCharacter;
        public DXImageControl SecondCharacterBase;
        /// <summary>
        /// 第二字符动画
        /// </summary>
        public DXAnimatedControl SecondCharacterRing;
        /// <summary>
        /// 第三字符容器
        /// </summary>
        public DXImageControl ThirdCharacter;
        public DXImageControl ThirdCharacterBase;
        /// <summary>
        /// 第三字符动画
        /// </summary>
        public DXAnimatedControl ThirdCharacterRing;
        /// <summary>
        /// 输入按钮
        /// </summary>
        public DXButton InputButton;
        /// <summary>
        /// 取消按钮
        /// </summary>
        public DXButton CancelButton;
        /// <summary>
        /// 成功打开
        /// </summary>
        public DXAnimatedControl SuccessOpen;
        /// <summary>
        /// 输入失败
        /// </summary>
        public DXAnimatedControl InputFailed;
        /// <summary>
        /// 文字1
        /// </summary>
        public DXImageControl Word1, Word2, Word3, Word4, Word5, Word6, Word7, Word8,
                              Word9, Word10, Word11, Word12, Word13, Word14, Word15, Word16,
                              Word17, Word18, Word19, Word20, Word21, Word22, Word23, Word24;
        /// <summary>
        /// 选定光圈
        /// </summary>
        public DXImageControl DetermineCircle1, DetermineCircle2, DetermineCircle3;
        /// <summary>
        /// 三个字符锁定
        /// </summary>
        public bool Locking1, Locking2, Locking3;

        public string CorrectChar1, SelectedChar1;
        public string CorrectChar2, SelectedChar2;
        public string CorrectChar3, SelectedChar3;

        public Dictionary<string, DXImageControl> CharDict;

        #endregion

        /// <summary>
        /// 古代墓碑面板
        /// </summary>
        public NPCAncientTombstoneDialog()
        {
            LibraryFile = LibraryFile.GameInter2;
            Index = 2300;

            SelectedChar1 = string.Empty;
            SelectedChar2 = string.Empty;
            SelectedChar3 = string.Empty;

            Tombstone = new DXImageControl
            {
                Parent = this,
                Location = new Point(75, 50),
                LibraryFile = LibraryFile.GameInter2,
                Index = 2360,
            };

            CloseButton = new DXButton
            {
                Parent = this,
                Location = new Point(318, 0),
                LibraryFile = LibraryFile.GameInter,
                Index = 1221,
                Hint = "关闭".Lang(),
            };
            CloseButton.MouseEnter += (o, e) => CloseButton.Index = 1220;  //鼠标进入
            CloseButton.MouseLeave += (o, e) => CloseButton.Index = 1221;  //鼠标离开
            CloseButton.MouseClick += (o, e) => Visible = false;           //鼠标点击

            FirstCharacterBase = new DXImageControl
            {
                Parent = this,
                Location = new Point(210, 40),
                LibraryFile = LibraryFile.GameInter2,
                Index = 2320,
            };

            FirstCharacter = new DXImageControl
            {
                Parent = FirstCharacterBase,
                Location = new Point(28, 24),
                LibraryFile = LibraryFile.GameInter2,
                Index = 2333,
                ForeColour = Color.Black,
                Visible = false
            };
            FirstCharacter.IndexChanged += (sender, args) =>
            {
                FirstCharacter.Location = new Point(44 - (FirstCharacter.Size.Width / 2),
                    43 - (FirstCharacter.Size.Height / 2));
                FirstCharacter.Visible = true;
            };

            FirstCharacterRing = new DXAnimatedControl  //选择第一个字的时候，光圈显示一下
            {
                BaseIndex = 2400,
                LibraryFile = LibraryFile.GameInter2,
                Animated = true,
                AnimationDelay = TimeSpan.FromSeconds(1),
                FrameCount = 7,
                Parent = this,
                Loop = false,
                Location = new Point(138, -35),
                Blend = true,
                Visible = false,
            };
            FirstCharacterRing.AfterAnimation += (sender, args) =>
            {
                FirstCharacterRing.Visible = false;
            };

            SecondCharacterBase = new DXImageControl
            {
                Parent = this,
                Location = new Point(15, 125),
                LibraryFile = LibraryFile.GameInter2,
                Index = 2321,
            };

            SecondCharacter = new DXImageControl
            {
                Parent = SecondCharacterBase,
                Location = new Point(28, 24),
                LibraryFile = LibraryFile.GameInter2,
                Index = 2338,
                ForeColour = Color.Black,
                Visible = false
            };
            SecondCharacter.IndexChanged += (sender, args) =>
            {
                SecondCharacter.Location = new Point(44 - (SecondCharacter.Size.Width / 2),
                    43 - (SecondCharacter.Size.Height / 2));
                SecondCharacter.Visible = true;
            };

            SecondCharacterRing = new DXAnimatedControl  //选择第二个字的时候，光圈显示一下
            {
                BaseIndex = 2400,
                LibraryFile = LibraryFile.GameInter2,
                Animated = true,
                AnimationDelay = TimeSpan.FromSeconds(1),
                FrameCount = 7,
                Parent = this,
                Loop = false,
                Location = new Point(-55, 50),
                Blend = true,
                Visible = false,
            };
            SecondCharacterRing.AfterAnimation += (sender, args) =>
            {
                SecondCharacterRing.Visible = false;
            };

            ThirdCharacterBase = new DXImageControl
            {
                Parent = this,
                Location = new Point(245, 225),
                LibraryFile = LibraryFile.GameInter2,
                Index = 2322,
            };

            ThirdCharacter = new DXImageControl
            {
                Parent = ThirdCharacterBase,
                Location = new Point(28, 24),
                LibraryFile = LibraryFile.GameInter2,
                Index = 2345,
                ForeColour = Color.Black,
                Visible = false
            };
            ThirdCharacter.IndexChanged += (sender, args) =>
            {
                ThirdCharacter.Location = new Point(44 - (ThirdCharacter.Size.Width / 2),
                    43 - (ThirdCharacter.Size.Height / 2));
                ThirdCharacter.Visible = true;
            };

            ThirdCharacterRing = new DXAnimatedControl  //选择第三个字的时候，光圈显示一下
            {
                BaseIndex = 2400,
                LibraryFile = LibraryFile.GameInter2,
                Animated = true,
                AnimationDelay = TimeSpan.FromSeconds(1),
                FrameCount = 7,
                Parent = this,
                Loop = false,
                Location = new Point(173, 152),
                Blend = true,
                Visible = false,
            };
            ThirdCharacterRing.AfterAnimation += (sender, args) =>
            {
                ThirdCharacterRing.Visible = false;
            };

            InputButton = new DXButton    //输入按钮 确定
            {
                Parent = this,
                Location = new Point(50, 497),
                LibraryFile = LibraryFile.GameInter2,
                Index = 2312,
            };
            InputButton.MouseClick += (sender, args) =>
            {
                Submit();
            };

            CancelButton = new DXButton   //取消按钮  把选择的字取消
            {
                Parent = this,
                Location = new Point(220, 497),
                LibraryFile = LibraryFile.GameInter2,
                Index = 2317,
            };
            CancelButton.MouseClick += (sender, args) =>
            {
                Reset();
            };

            SuccessOpen = new DXAnimatedControl  //文字输入成功以后，开门的动画，然后直接人物就到2图等候厅
            {
                BaseIndex = 2370,
                LibraryFile = LibraryFile.GameInter2,
                Animated = true,
                AnimationDelay = TimeSpan.FromSeconds(1),
                FrameCount = 14,
                Parent = this,
                Loop = false,
                Blend = true,
                Location = new Point(-330, -95),
                Visible = false,
            };
            SuccessOpen.AfterAnimation += (sender, args) =>
            {
                if (AllCharsRight())
                {
                    CEnvir.Enqueue(new C.EnterAncientTomb
                    {
                        FirstChar = SelectedChar1,
                        SecondChar = SelectedChar2,
                        ThirdChar = SelectedChar3,
                    });
                    SuccessOpen.Visible = false;
                    SuccessOpen.SendToBack();
                    Reset(true);
                }
            };

            InputFailed = new DXAnimatedControl  //文字输入失败以后，动画一结束，直接把人物踢回到迷失
            {
                BaseIndex = 2360,
                LibraryFile = LibraryFile.GameInter2,
                Animated = true,
                AnimationDelay = TimeSpan.FromSeconds(1),
                FrameCount = 7,
                Parent = this,
                Loop = false,
                Location = new Point(75, 50),
                Visible = false,
            };
            InputFailed.AfterAnimation += (sender, args) =>
            {
                CEnvir.Enqueue(new C.EnterAncientTomb
                {
                    FirstChar = SelectedChar1,
                    SecondChar = SelectedChar2,
                    ThirdChar = SelectedChar3,
                });
                InputFailed.Visible = false;
                InputFailed.SendToBack();
            };

            DetermineCircle1 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2323,
                Visible = false,
            };

            DetermineCircle2 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2323,
                Visible = false,
            };

            DetermineCircle3 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2323,
                Visible = false,
            };

            #region 选择文字

            #region 第一排

            Word1 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2330,
                Location = new Point(52, 365),
                ZoomSize = new Size(25, 25),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "1"
            };
            Word1.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word1.Zoom = false;
                Word1.ForeColour = Color.Yellow;
                Word1.Location = new Point(52, 360);
            };
            Word1.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word1.Zoom = true;
                    Word1.ForeColour = Color.FromArgb(100, 65, 20);
                    Word1.Location = new Point(52, 365);
                }
            };
            Word1.MouseClick += OnCharClick;

            Word2 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2331,
                Location = new Point(84, 366),
                ZoomSize = new Size(25, 25),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "2"
            };
            Word2.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word2.Zoom = false;
                Word2.ForeColour = Color.Yellow;
                Word2.Location = new Point(81, 360);
            };
            Word2.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word2.Zoom = true;
                    Word2.ForeColour = Color.FromArgb(100, 65, 20);
                    Word2.Location = new Point(84, 366);
                }
            };
            Word2.MouseClick += OnCharClick;

            Word3 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2332,
                Location = new Point(118, 366),
                ZoomSize = new Size(25, 25),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "3"
            };
            Word3.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word3.Zoom = false;
                Word3.ForeColour = Color.Yellow;
                Word3.Location = new Point(115, 360);
            };
            Word3.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word3.Zoom = true;
                    Word3.ForeColour = Color.FromArgb(100, 65, 20);
                    Word3.Location = new Point(118, 366);
                }
            };
            Word3.MouseClick += OnCharClick;

            Word4 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2333,
                Location = new Point(151, 366),
                ZoomSize = new Size(25, 25),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "4"
            };
            Word4.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word4.Zoom = false;
                Word4.ForeColour = Color.Yellow;
                Word4.Location = new Point(147, 360);
            };
            Word4.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word4.Zoom = true;
                    Word4.ForeColour = Color.FromArgb(100, 65, 20);
                    Word4.Location = new Point(151, 366);
                }
            };
            Word4.MouseClick += OnCharClick;

            Word5 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2334,
                Location = new Point(178, 372),
                ZoomSize = new Size(25, 25),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "5"
            };
            Word5.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word5.Zoom = false;
                Word5.ForeColour = Color.Yellow;
                Word5.Location = new Point(172, 366);
            };
            Word5.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word5.Zoom = true;
                    Word5.ForeColour = Color.FromArgb(100, 65, 20);
                    Word5.Location = new Point(178, 372);
                }
            };
            Word5.MouseClick += OnCharClick;

            Word6 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2335,
                Location = new Point(217, 366),
                ZoomSize = new Size(25, 25),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "6"
            };
            Word6.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word6.Zoom = false;
                Word6.ForeColour = Color.Yellow;
                Word6.Location = new Point(213, 358);
            };
            Word6.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word6.Zoom = true;
                    Word6.ForeColour = Color.FromArgb(100, 65, 20);
                    Word6.Location = new Point(217, 366);
                }
            };
            Word6.MouseClick += OnCharClick;

            Word7 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2336,
                Location = new Point(249, 370),
                ZoomSize = new Size(18, 18),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "7"
            };
            Word7.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word7.Zoom = false;
                Word7.ForeColour = Color.Yellow;
                Word7.Location = new Point(247, 367);
            };
            Word7.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word7.Zoom = true;
                    Word7.ForeColour = Color.FromArgb(100, 65, 20);
                    Word7.Location = new Point(249, 370);
                }
            };
            Word7.MouseClick += OnCharClick;

            Word8 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2337,
                Location = new Point(277, 372),
                ZoomSize = new Size(25, 25),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "8"
            };
            Word8.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word8.Zoom = false;
                Word8.ForeColour = Color.Yellow;
                Word8.Location = new Point(269, 367);
            };
            Word8.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word8.Zoom = true;
                    Word8.ForeColour = Color.FromArgb(100, 65, 20);
                    Word8.Location = new Point(277, 372);
                }
            };
            Word8.MouseClick += OnCharClick;

            #endregion

            #region 第二排

            Word9 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2338,
                Location = new Point(49, 402),
                ZoomSize = new Size(20, 20),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "9"
            };
            Word9.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word9.Zoom = false;
                Word9.ForeColour = Color.Yellow;
                Word9.Location = new Point(42, 398);
            };
            Word9.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word9.Zoom = true;
                    Word9.ForeColour = Color.FromArgb(100, 65, 20);
                    Word9.Location = new Point(49, 402);
                }
            };
            Word9.MouseClick += OnCharClick;

            Word10 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2339,
                Location = new Point(85, 400),
                ZoomSize = new Size(25, 25),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "10"
            };
            Word10.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word10.Zoom = false;
                Word10.ForeColour = Color.Yellow;
                Word10.Location = new Point(84, 395);
            };
            Word10.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word10.Zoom = true;
                    Word10.ForeColour = Color.FromArgb(100, 65, 20);
                    Word10.Location = new Point(85, 400);
                }
            };
            Word10.MouseClick += OnCharClick;

            Word11 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2340,
                Location = new Point(115, 401),
                ZoomSize = new Size(20, 20),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "11"
            };
            Word11.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word11.Zoom = false;
                Word11.ForeColour = Color.Yellow;
                Word11.Location = new Point(111, 398);
            };
            Word11.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word11.Zoom = true;
                    Word11.ForeColour = Color.FromArgb(100, 65, 20);
                    Word11.Location = new Point(115, 401);
                }
            };
            Word11.MouseClick += OnCharClick;

            Word12 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2341,
                Location = new Point(150, 399),
                ZoomSize = new Size(25, 25),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "12"
            };
            Word12.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word12.Zoom = false;
                Word12.ForeColour = Color.Yellow;
                Word12.Location = new Point(146, 392);
            };
            Word12.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word12.Zoom = true;
                    Word12.ForeColour = Color.FromArgb(100, 65, 20);
                    Word12.Location = new Point(150, 399);
                }
            };
            Word12.MouseClick += OnCharClick;

            Word13 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2342,
                Location = new Point(181, 400),
                ZoomSize = new Size(25, 25),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "13"
            };
            Word13.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word13.Zoom = false;
                Word13.ForeColour = Color.Yellow;
                Word13.Location = new Point(176, 394);
            };
            Word13.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word13.Zoom = true;
                    Word13.ForeColour = Color.FromArgb(100, 65, 20);
                    Word13.Location = new Point(181, 400);
                }
            };
            Word13.MouseClick += OnCharClick;

            Word14 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2343,
                Location = new Point(213, 400),
                ZoomSize = new Size(25, 25),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "14"
            };
            Word14.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word14.Zoom = false;
                Word14.ForeColour = Color.Yellow;
                Word14.Location = new Point(205, 393);
            };
            Word14.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word14.Zoom = true;
                    Word14.ForeColour = Color.FromArgb(100, 65, 20);
                    Word14.Location = new Point(213, 400);
                }
            };
            Word14.MouseClick += OnCharClick;

            Word15 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2344,
                Location = new Point(252, 400),
                ZoomSize = new Size(25, 25),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "15"
            };
            Word15.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word15.Zoom = false;
                Word15.ForeColour = Color.Yellow;
                Word15.Location = new Point(251, 396);
            };
            Word15.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word15.Zoom = true;
                    Word15.ForeColour = Color.FromArgb(100, 65, 20);
                    Word15.Location = new Point(252, 400);
                }
            };
            Word15.MouseClick += OnCharClick;

            Word16 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2345,
                Location = new Point(281, 400),
                ZoomSize = new Size(25, 25),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "16"
            };
            Word16.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word16.Zoom = false;
                Word16.ForeColour = Color.Yellow;
                Word16.Location = new Point(273, 390);
            };
            Word16.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word16.Zoom = true;
                    Word16.ForeColour = Color.FromArgb(100, 65, 20);
                    Word16.Location = new Point(281, 400);
                }
            };
            Word16.MouseClick += OnCharClick;

            #endregion

            #region 第三排

            Word17 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2346,
                Location = new Point(52, 432),
                ZoomSize = new Size(25, 25),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "17"
            };
            Word17.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word17.Zoom = false;
                Word17.ForeColour = Color.Yellow;
                Word17.Location = new Point(48, 424);
            };
            Word17.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word17.Zoom = true;
                    Word17.ForeColour = Color.FromArgb(100, 65, 20);
                    Word17.Location = new Point(52, 432);
                }
            };
            Word17.MouseClick += OnCharClick;

            Word18 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2347,
                Location = new Point(84, 435),
                ZoomSize = new Size(20, 20),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "18"
            };
            Word18.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word18.Zoom = false;
                Word18.ForeColour = Color.Yellow;
                Word18.Location = new Point(80, 430);
            };
            Word18.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word18.Zoom = true;
                    Word18.ForeColour = Color.FromArgb(100, 65, 20);
                    Word18.Location = new Point(84, 435);
                }
            };
            Word18.MouseClick += OnCharClick;

            Word19 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2348,
                Location = new Point(117, 432),
                ZoomSize = new Size(25, 25),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "19"
            };
            Word19.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word19.Zoom = false;
                Word19.ForeColour = Color.Yellow;
                Word19.Location = new Point(113, 425);
            };
            Word19.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word19.Zoom = true;
                    Word19.ForeColour = Color.FromArgb(100, 65, 20);
                    Word19.Location = new Point(117, 432);
                }
            };
            Word19.MouseClick += OnCharClick;

            Word20 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2349,
                Location = new Point(150, 433),
                ZoomSize = new Size(25, 25),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "20"
            };
            Word20.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word20.Zoom = false;
                Word20.ForeColour = Color.Yellow;
                Word20.Location = new Point(146, 428);
            };
            Word20.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word20.Zoom = true;
                    Word20.ForeColour = Color.FromArgb(100, 65, 20);
                    Word20.Location = new Point(150, 433);
                }
            };
            Word20.MouseClick += OnCharClick;

            Word21 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2350,
                Location = new Point(184, 433),
                ZoomSize = new Size(25, 25),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "21"
            };
            Word21.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word21.Zoom = false;
                Word21.ForeColour = Color.Yellow;
                Word21.Location = new Point(179, 426);
            };
            Word21.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word21.Zoom = true;
                    Word21.ForeColour = Color.FromArgb(100, 65, 20);
                    Word21.Location = new Point(184, 433);
                }
            };
            Word21.MouseClick += OnCharClick;

            Word22 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2351,
                Location = new Point(215, 433),
                ZoomSize = new Size(25, 25),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "22"
            };
            Word22.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word22.Zoom = false;
                Word22.ForeColour = Color.Yellow;
                Word22.Location = new Point(210, 422);
            };
            Word22.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word22.Zoom = true;
                    Word22.ForeColour = Color.FromArgb(100, 65, 20);
                    Word22.Location = new Point(215, 433);
                }
            };
            Word22.MouseClick += OnCharClick;

            Word23 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2352,
                Location = new Point(253, 433),
                ZoomSize = new Size(25, 25),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "23"
            };
            Word23.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word23.Zoom = false;
                Word23.ForeColour = Color.Yellow;
                Word23.Location = new Point(251, 425);
            };
            Word23.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word23.Zoom = true;
                    Word23.ForeColour = Color.FromArgb(100, 65, 20);
                    Word23.Location = new Point(253, 433);
                }
            };
            Word23.MouseClick += OnCharClick;

            Word24 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2353,
                Location = new Point(284, 433),
                ZoomSize = new Size(25, 25),   //缩放尺寸
                Zoom = true,
                ForeColour = Color.FromArgb(100, 65, 20),  //灰化
                Tag = "24"
            };
            Word24.MouseEnter += (s, e) =>  //鼠标进入
            {
                Word24.Zoom = false;
                Word24.ForeColour = Color.Yellow;
                Word24.Location = new Point(281, 426);
            };
            Word24.MouseLeave += (s, e) =>  //鼠标离开
            {
                if (Locking1 == false || Locking2 == false || Locking3 == false)
                {
                    Word24.Zoom = true;
                    Word24.ForeColour = Color.FromArgb(100, 65, 20);
                    Word24.Location = new Point(284, 433);
                }
            };
            Word24.MouseClick += OnCharClick;

            #endregion

            CharDict = new Dictionary<string, DXImageControl>
            {
                ["1"] = Word1,
                ["2"] = Word2,
                ["3"] = Word3,
                ["4"] = Word4,
                ["5"] = Word5,
                ["6"] = Word6,
                ["7"] = Word7,
                ["8"] = Word8,
                ["9"] = Word9,
                ["10"] = Word10,
                ["11"] = Word11,
                ["12"] = Word12,
                ["13"] = Word13,
                ["14"] = Word14,
                ["15"] = Word15,
                ["16"] = Word16,
                ["17"] = Word17,
                ["18"] = Word18,
                ["19"] = Word19,
                ["20"] = Word20,
                ["21"] = Word21,
                ["22"] = Word22,
                ["23"] = Word23,
                ["24"] = Word24,
            };

            #endregion
        }

        public bool AllCharsRight()
        {
            if (string.IsNullOrEmpty(SelectedChar1) || !CharDict.ContainsKey(SelectedChar1) ||
                string.IsNullOrEmpty(SelectedChar2) || !CharDict.ContainsKey(SelectedChar2) ||
                string.IsNullOrEmpty(SelectedChar3) || !CharDict.ContainsKey(SelectedChar3))
            {
                return false;
            }

            if (string.IsNullOrEmpty(CorrectChar1) || string.IsNullOrEmpty(CorrectChar2) ||
                string.IsNullOrEmpty(CorrectChar3))
            {
                return false;
            }

            if (SelectedChar1 != CorrectChar1 || SelectedChar2 != CorrectChar2 || SelectedChar3 != CorrectChar3)
            {
                return false;
            }

            return true;
        }

        public void Submit()
        {
            if (AllCharsRight())
            {
                SuccessOpen.Visible = true;
                SuccessOpen.Animated = true;
                SuccessOpen.AnimationStart = CEnvir.Now;
            }
            else
            {
                InputFailed.Visible = true;
                InputFailed.Animated = true;
                InputFailed.AnimationStart = CEnvir.Now;
            }
        }

        public void Reset(bool resetAll = false)
        {
            SelectedChar1 = string.Empty;
            SelectedChar2 = string.Empty;
            SelectedChar3 = string.Empty;

            FirstCharacterRing.Visible = false;
            SecondCharacterRing.Visible = false;
            ThirdCharacterRing.Visible = false;

            DetermineCircle1.Visible = false;
            DetermineCircle2.Visible = false;
            DetermineCircle3.Visible = false;

            if (!string.IsNullOrEmpty(CorrectChar1))
            {
                FirstCharacter.Index = CharDict[CorrectChar1].Index;
                FirstCharacter.ForeColour = Color.Black;
            }
            if (!string.IsNullOrEmpty(CorrectChar2))
            {
                SecondCharacter.Index = CharDict[CorrectChar2].Index;
                SecondCharacter.ForeColour = Color.Black;
            }
            if (!string.IsNullOrEmpty(CorrectChar3))
            {
                ThirdCharacter.Index = CharDict[CorrectChar3].Index;
                ThirdCharacter.ForeColour = Color.Black;
            }

            if (resetAll)
            {
                CorrectChar1 = string.Empty;
                CorrectChar2 = string.Empty;
                CorrectChar3 = string.Empty;

                FirstCharacter.Visible = false;
                SecondCharacter.Visible = false;
                ThirdCharacter.Visible = false;
            }

        }

        private void SetRingLocation(DXImageControl ring, int num)
        {
            num -= 1;
            int row = num / 8;
            int col = num % 8 + 1;

            ring.Location = new Point(34 * col, 358 + row * 33);
        }

        private void OnCharClick(object o, MouseEventArgs e)
        {
            DXImageControl charClicked = o as DXImageControl;
            if (charClicked == null) return;

            if (string.IsNullOrEmpty(SelectedChar1))
            {
                SelectedChar1 = charClicked.Tag as string;
                DetermineCircle1.Visible = true;

                SetRingLocation(DetermineCircle1, int.Parse(SelectedChar1));
                FirstCharacterRing.Animated = true;
                FirstCharacterRing.Visible = true;
                FirstCharacterRing.AnimationStart = CEnvir.Now;

                FirstCharacter.Index = CharDict[SelectedChar1].Index;
                FirstCharacter.ForeColour = Color.Yellow;
            }
            else if (string.IsNullOrEmpty(SelectedChar2))
            {
                SelectedChar2 = charClicked.Tag as string;
                DetermineCircle2.Visible = true;

                SetRingLocation(DetermineCircle2, int.Parse(SelectedChar2));
                SecondCharacterRing.Animated = true;
                SecondCharacterRing.Visible = true;
                SecondCharacterRing.AnimationStart = CEnvir.Now;

                SecondCharacter.Index = CharDict[SelectedChar2].Index;
                SecondCharacter.ForeColour = Color.Yellow;
            }
            else if (string.IsNullOrEmpty(SelectedChar3))
            {
                SelectedChar3 = charClicked.Tag as string;
                DetermineCircle3.Visible = true;

                SetRingLocation(DetermineCircle3, int.Parse(SelectedChar3));
                ThirdCharacterRing.Animated = true;
                ThirdCharacterRing.Visible = true;
                ThirdCharacterRing.AnimationStart = CEnvir.Now;

                ThirdCharacter.Index = CharDict[SelectedChar3].Index;
                ThirdCharacter.ForeColour = Color.Yellow;
            }
            else
            {
                return;
            }

            charClicked.Zoom = true;
            charClicked.ForeColour = Color.Yellow;
            charClicked.Location = charClicked.Location;

        }

        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);
            // 古墓任务的序号
            CorrectChar1 = GameScene.Game.QuestLog
                .FirstOrDefault(x => x.Quest.Index == CEnvir.PenetraliumKeyA && !string.IsNullOrEmpty(x.ExtraInfo))?.ExtraInfo;
            CorrectChar2 = GameScene.Game.QuestLog
                .FirstOrDefault(x => x.Quest.Index == CEnvir.PenetraliumKeyB && !string.IsNullOrEmpty(x.ExtraInfo))?.ExtraInfo;
            CorrectChar3 = GameScene.Game.QuestLog
                .FirstOrDefault(x => x.Quest.Index == CEnvir.PenetraliumKeyC && !string.IsNullOrEmpty(x.ExtraInfo))?.ExtraInfo;

            if (!string.IsNullOrEmpty(CorrectChar1))
            {
                FirstCharacter.Index = CharDict[CorrectChar1].Index;
            }
            if (!string.IsNullOrEmpty(CorrectChar2))
            {
                SecondCharacter.Index = CharDict[CorrectChar2].Index;
            }
            if (!string.IsNullOrEmpty(CorrectChar3))
            {
                ThirdCharacter.Index = CharDict[CorrectChar3].Index;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (CloseButton != null)
                {
                    if (!CloseButton.IsDisposed)
                        CloseButton.Dispose();

                    CloseButton = null;
                }

                if (Tombstone != null)
                {
                    if (!Tombstone.IsDisposed)
                        Tombstone.Dispose();

                    Tombstone = null;
                }

                if (FirstCharacter != null)
                {
                    if (!FirstCharacter.IsDisposed)
                        FirstCharacter.Dispose();

                    FirstCharacter = null;
                }

                if (FirstCharacterBase != null)
                {
                    if (!FirstCharacterBase.IsDisposed)
                        FirstCharacterBase.Dispose();

                    FirstCharacterBase = null;
                }

                if (FirstCharacterRing != null)
                {
                    if (!FirstCharacterRing.IsDisposed)
                        FirstCharacterRing.Dispose();

                    FirstCharacterRing = null;
                }

                if (CancelButton != null)
                {
                    if (!CancelButton.IsDisposed)
                        CancelButton.Dispose();

                    CancelButton = null;
                }

                if (SecondCharacter != null)
                {
                    if (!SecondCharacter.IsDisposed)
                        SecondCharacter.Dispose();

                    SecondCharacter = null;
                }

                if (SecondCharacterBase != null)
                {
                    if (!SecondCharacterBase.IsDisposed)
                        SecondCharacterBase.Dispose();

                    SecondCharacterBase = null;
                }

                if (SecondCharacterRing != null)
                {
                    if (!SecondCharacterRing.IsDisposed)
                        SecondCharacterRing.Dispose();

                    SecondCharacterRing = null;
                }

                if (ThirdCharacter != null)
                {
                    if (!ThirdCharacter.IsDisposed)
                        ThirdCharacter.Dispose();

                    ThirdCharacter = null;
                }

                if (ThirdCharacterBase != null)
                {
                    if (!ThirdCharacterBase.IsDisposed)
                        ThirdCharacterBase.Dispose();

                    ThirdCharacterBase = null;
                }

                if (ThirdCharacterRing != null)
                {
                    if (!ThirdCharacterRing.IsDisposed)
                        ThirdCharacterRing.Dispose();

                    ThirdCharacterRing = null;
                }

                if (InputButton != null)
                {
                    if (!InputButton.IsDisposed)
                        InputButton.Dispose();

                    InputButton = null;
                }

                if (CancelButton != null)
                {
                    if (!CancelButton.IsDisposed)
                        CancelButton.Dispose();

                    CancelButton = null;
                }

                if (SuccessOpen != null)
                {
                    if (!SuccessOpen.IsDisposed)
                        SuccessOpen.Dispose();

                    SuccessOpen = null;
                }

                if (InputFailed != null)
                {
                    if (!InputFailed.IsDisposed)
                        InputFailed.Dispose();

                    InputFailed = null;
                }

                if (DetermineCircle1 != null)
                {
                    if (!DetermineCircle1.IsDisposed)
                        DetermineCircle1.Dispose();

                    DetermineCircle1 = null;
                }

                if (DetermineCircle2 != null)
                {
                    if (!DetermineCircle2.IsDisposed)
                        DetermineCircle2.Dispose();

                    DetermineCircle2 = null;
                }

                if (DetermineCircle3 != null)
                {
                    if (!DetermineCircle3.IsDisposed)
                        DetermineCircle3.Dispose();

                    DetermineCircle3 = null;
                }

                if (Word1 != null)
                {
                    if (!Word1.IsDisposed)
                        Word1.Dispose();

                    Word1 = null;
                }

                if (Word2 != null)
                {
                    if (!Word2.IsDisposed)
                        Word2.Dispose();

                    Word2 = null;
                }

                if (Word3 != null)
                {
                    if (!Word3.IsDisposed)
                        Word3.Dispose();

                    Word3 = null;
                }

                if (Word4 != null)
                {
                    if (!Word4.IsDisposed)
                        Word4.Dispose();

                    Word4 = null;
                }

                if (Word5 != null)
                {
                    if (!Word5.IsDisposed)
                        Word5.Dispose();

                    Word5 = null;
                }

                if (Word6 != null)
                {
                    if (!Word6.IsDisposed)
                        Word6.Dispose();

                    Word6 = null;
                }

                if (Word7 != null)
                {
                    if (!Word7.IsDisposed)
                        Word7.Dispose();

                    Word7 = null;
                }

                if (Word8 != null)
                {
                    if (!Word8.IsDisposed)
                        Word8.Dispose();

                    Word8 = null;
                }

                if (Word9 != null)
                {
                    if (!Word9.IsDisposed)
                        Word9.Dispose();

                    Word9 = null;
                }

                if (Word10 != null)
                {
                    if (!Word10.IsDisposed)
                        Word10.Dispose();

                    Word10 = null;
                }

                if (Word11 != null)
                {
                    if (!Word11.IsDisposed)
                        Word11.Dispose();

                    Word11 = null;
                }

                if (Word12 != null)
                {
                    if (!Word12.IsDisposed)
                        Word12.Dispose();

                    Word12 = null;
                }

                if (Word13 != null)
                {
                    if (!Word13.IsDisposed)
                        Word13.Dispose();

                    Word13 = null;
                }

                if (Word14 != null)
                {
                    if (!Word14.IsDisposed)
                        Word14.Dispose();

                    Word14 = null;
                }

                if (Word15 != null)
                {
                    if (!Word15.IsDisposed)
                        Word15.Dispose();

                    Word15 = null;
                }

                if (Word16 != null)
                {
                    if (!Word16.IsDisposed)
                        Word16.Dispose();

                    Word16 = null;
                }

                if (Word17 != null)
                {
                    if (!Word17.IsDisposed)
                        Word17.Dispose();

                    Word17 = null;
                }

                if (Word18 != null)
                {
                    if (!Word18.IsDisposed)
                        Word18.Dispose();

                    Word18 = null;
                }

                if (Word19 != null)
                {
                    if (!Word19.IsDisposed)
                        Word19.Dispose();

                    Word19 = null;
                }

                if (Word20 != null)
                {
                    if (!Word20.IsDisposed)
                        Word20.Dispose();

                    Word20 = null;
                }

                if (Word21 != null)
                {
                    if (!Word21.IsDisposed)
                        Word21.Dispose();

                    Word21 = null;
                }

                if (Word22 != null)
                {
                    if (!Word22.IsDisposed)
                        Word22.Dispose();

                    Word22 = null;
                }

                if (Word23 != null)
                {
                    if (!Word23.IsDisposed)
                        Word23.Dispose();

                    Word23 = null;
                }

                if (Word24 != null)
                {
                    if (!Word24.IsDisposed)
                        Word24.Dispose();

                    Word24 = null;
                }
            }
        }
    }

    #endregion

    #region 新版首饰合成

    /// <summary>
    /// 新版首饰合成功能
    /// </summary>
    public sealed class NPCAccessoryCombineDialog : DXImageControl
    {
        #region DX

        /// <summary>
        /// 关闭按钮
        /// </summary>
        public DXButton CloseButton;
        /// <summary>
        /// 成功率
        /// </summary>
        public DXLabel SuccessRate;
        /// <summary>
        /// 提升成功率
        /// </summary>
        public DXLabel PromotionRate;
        /// <summary>
        /// 主首饰
        /// </summary>
        public DXItemGrid MainJewelry;
        /// <summary>
        /// 副首饰1
        /// </summary>
        public DXItemGrid AccessoryJewelry1;
        /// <summary>
        /// 副首饰2
        /// </summary>
        public DXItemGrid AccessoryJewelry2;
        /// <summary>
        /// 刚玉石
        /// </summary>
        public DXItemGrid Corundum;
        /// <summary>
        /// 结晶石
        /// </summary>
        public DXItemGrid Crystal;
        /// <summary>
        /// 属性选择框
        /// </summary>
        public DXComboBox AttributeComboBox;
        /// <summary>
        /// 合成按钮
        /// </summary>
        public DXButton CombineButton;

        /// <summary>
        /// 选择合成属性
        /// </summary>
        public JewelryRefineType JewelryRefineType
        {
            get { return _JewelryRefineType; }
            set
            {
                if (_JewelryRefineType == value) return;

                JewelryRefineType oldValue = _JewelryRefineType;
                _JewelryRefineType = value;
            }
        }
        private JewelryRefineType _JewelryRefineType;

        #endregion

        public NPCAccessoryCombineDialog()
        {
            LibraryFile = LibraryFile.GameInter1;
            Index = 50;

            CloseButton = new DXButton  //关闭按钮
            {
                Parent = this,
                Location = new Point(255, 1),
                LibraryFile = LibraryFile.GameInter,
                Index = 1221,
                Hint = "关闭".Lang(),
            };
            CloseButton.MouseEnter += (o, e) => CloseButton.Index = 1220;  //鼠标进入
            CloseButton.MouseLeave += (o, e) => CloseButton.Index = 1221;  //鼠标离开
            CloseButton.MouseClick += (o, e) => Visible = false;        //鼠标点击

            SuccessRate = new DXLabel
            {
                Parent = this,
                Location = new Point(250, 111),
                Text = "0%",
            };

            PromotionRate = new DXLabel
            {
                Parent = this,
                Location = new Point(250, 157),
                Text = "0%",
            };

            MainJewelry = new DXItemGrid     //主首饰格子
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.AccessoryComposeLevelTarget,
                Linked = true,
                Location = new Point(120, 53),
            };

            AccessoryJewelry1 = new DXItemGrid     //副首饰格子1
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.AccessoryComposeLevelItems,
                Linked = true,
                Location = new Point(172, 53),
                Visible = false,
            };

            AccessoryJewelry2 = new DXItemGrid     //副首饰格子2
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.AccessoryComposeLevelItems,
                Linked = true,
                Location = new Point(222, 53),
                Visible = false,
            };

            Corundum = new DXItemGrid     //钢玉石
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.RefinementStoneCorundum,
                Linked = true,
                Location = new Point(120, 98),
                Visible = false,
            };

            Crystal = new DXItemGrid     //结晶石
            {
                GridSize = new Size(1, 1),
                Parent = this,
                GridType = GridType.RefinementStoneCrystal,
                Linked = true,
                Location = new Point(120, 144),
                Visible = false,
            };

            CombineButton = new DXButton //合成按钮
            {
                Parent = this,
                Index = 60,
                LibraryFile = LibraryFile.GameInter1,
                Label = { Text = "合成".Lang() },
                Location = new Point(195, 203),
            };
            CombineButton.MouseClick += CombineButtonOnMouseClick;

            #region 属性
            AttributeComboBox = new DXComboBox
            {
                Parent = this,
                Size = new Size(70, DXComboBox.DefaultNormalHeight),
            };
            AttributeComboBox.Location = new Point(100, 208);
            AttributeComboBox.SelectedItemChanged += (o, e) =>
            {
                JewelryRefineType = (JewelryRefineType?)AttributeComboBox.SelectedItem ?? JewelryRefineType.DC;
            };

            new DXListBoxItem
            {
                Parent = AttributeComboBox.ListBox,
                Label = { Text = $"攻击".Lang() },
                Item = JewelryRefineType.DC
            };
            new DXListBoxItem
            {
                Parent = AttributeComboBox.ListBox,
                Label = { Text = $"自然".Lang() },
                Item = JewelryRefineType.MC
            };
            new DXListBoxItem
            {
                Parent = AttributeComboBox.ListBox,
                Label = { Text = $"灵魂".Lang() },
                Item = JewelryRefineType.SC
            };
            new DXListBoxItem
            {
                Parent = AttributeComboBox.ListBox,
                Label = { Text = $"火元素".Lang() },
                Item = JewelryRefineType.FireAttack
            };
            new DXListBoxItem
            {
                Parent = AttributeComboBox.ListBox,
                Label = { Text = $"冰元素".Lang() },
                Item = JewelryRefineType.IceAttack
            };
            new DXListBoxItem
            {
                Parent = AttributeComboBox.ListBox,
                Label = { Text = $"雷元素".Lang() },
                Item = JewelryRefineType.LightningAttack
            };
            new DXListBoxItem
            {
                Parent = AttributeComboBox.ListBox,
                Label = { Text = $"风元素".Lang() },
                Item = JewelryRefineType.WindAttack
            };
            new DXListBoxItem
            {
                Parent = AttributeComboBox.ListBox,
                Label = { Text = $"神圣元素".Lang() },
                Item = JewelryRefineType.HolyAttack
            };
            new DXListBoxItem
            {
                Parent = AttributeComboBox.ListBox,
                Label = { Text = $"暗黑元素".Lang() },
                Item = JewelryRefineType.DarkAttack
            };
            new DXListBoxItem
            {
                Parent = AttributeComboBox.ListBox,
                Label = { Text = $"幻影元素".Lang() },
                Item = JewelryRefineType.PhantomAttack
            };

            AttributeComboBox.ListBox.SelectItem(JewelryRefineType.DC);
            #endregion

            foreach (DXItemCell cell in Crystal.Grid)
            {
                cell.LinkChanged += delegate
                {
                    UpdateSuccessRate();
                };
            }

            DXItemCell[] MainJewelryGrid = MainJewelry.Grid;
            foreach (DXItemCell cell in MainJewelryGrid)
            {
                cell.LinkChanged += delegate
                {
                    UpdateGridVisible();
                    UpdateSuccessRate();
                };
            }
        }

        /// <summary>
        /// 更新格子显示
        /// </summary>
        private void UpdateGridVisible()
        {
            foreach (DXItemCell cell in MainJewelry.Grid)
            {
                if (cell.Link == null)
                {
                    AccessoryJewelry1.Visible = false;
                    AccessoryJewelry2.Visible = false;
                    Corundum.Visible = false;
                    Crystal.Visible = false;
                    return;
                }
            }

            foreach (DXItemCell cell in MainJewelry.Grid)
            {
                if (cell.Link != null && cell.Link.Item.Info.Rarity == Rarity.Common)
                {
                    AccessoryJewelry1.Visible = true;
                    AccessoryJewelry2.Visible = true;
                    Corundum.Visible = true;
                    Crystal.Visible = true;
                    return;
                }
            }

            foreach (DXItemCell cell in MainJewelry.Grid)
            {
                if (cell.Link != null && cell.Link.Item.Info.Rarity != Rarity.Common)
                {
                    AccessoryJewelry1.Visible = true;
                    AccessoryJewelry2.Visible = false;
                    Corundum.Visible = true;
                    Crystal.Visible = true;
                    return;
                }
            }
        }

        /// <summary>
        /// 更新成功率信息
        /// </summary>
        private void UpdateSuccessRate()
        {
            //更新成功率信息
            ClientUserItem mainItem = MainJewelry.Grid?[0]?.Link?.Item;
            if (mainItem == null)
            {
                SuccessRate.Text = "0%";
                PromotionRate.Text = "0%";
                return;
            }

            long? crtstalAmount = Crystal.Grid?[0]?.Link?.Link?.LinkedCount;
            int addedRate = (int)(crtstalAmount ?? 0);

            int baseRate = 0;
            int currentLevel = mainItem.Level;

            switch (mainItem.Info.Rarity)
            {
                case Rarity.Common:
                    baseRate = Math.Max(0, CEnvir.ClientControl.CommonItemSuccess - (currentLevel - 1) * CEnvir.ClientControl.CommonItemReduce);
                    break;
                case Rarity.Superior:
                    baseRate = Math.Max(0, CEnvir.ClientControl.SuperiorItemSuccess - (currentLevel - 1) * CEnvir.ClientControl.SuperiorItemReduce);
                    break;
                case Rarity.Elite:
                    baseRate = Math.Max(0, CEnvir.ClientControl.EliteItemSuccess - (currentLevel - 1) * CEnvir.ClientControl.EliteItemReduce);
                    break;
                default:
                    baseRate = 0;
                    break;
            }

            SuccessRate.Text = $"{baseRate + addedRate}%";
            PromotionRate.Text = $"{addedRate}%";
        }

        /// <summary>
        /// 发包开始升级
        /// </summary>
        private void CombineButtonOnMouseClick(object sender, MouseEventArgs e)
        {
            if (GameScene.Game.User.Gold < CEnvir.ClientControl.ACGoldRateCostEdit)
            {
                GameScene.Game.ReceiveChat("金币不足无法合成".Lang(), MessageType.System);
                return;
            }

            ClientUserItem mainItem = MainJewelry.Grid[0]?.Link?.Item;
            if (mainItem == null)
            {
                GameScene.Game.ReceiveChat("没有主首饰无法合成".Lang(), MessageType.System);
                return;
            }

            if (AccessoryJewelry1.Grid?[0]?.Link?.Item == null)
            {
                GameScene.Game.ReceiveChat("没有副首饰无法合成".Lang(), MessageType.System);
                return;
            }

            if (mainItem.Info.Rarity == Rarity.Common && AccessoryJewelry2.Grid?[0]?.Link?.Item == null)
            {
                GameScene.Game.ReceiveChat("副首饰不够无法合成".Lang(), MessageType.System);
                return;
            }

            if (mainItem.Level != AccessoryJewelry1.Grid?[0]?.Link?.Item.Level)
            {
                GameScene.Game.ReceiveChat("首饰等级不符无法合成".Lang(), MessageType.System);
                return;
            }

            ClientUserItem corundum = Corundum.Grid?[0]?.Link?.Item;
            if (corundum == null)
            {
                GameScene.Game.ReceiveChat("没有矿石无法合成".Lang(), MessageType.System);
                return;
            }

            CEnvir.Enqueue(new AccessoryCombineRequest
            {
                RefineType = (JewelryRefineType)AttributeComboBox.ListBox.SelectedItem.Item,
                MainItem = new CellLinkInfo
                {
                    Count = MainJewelry.Grid[0].LinkedCount,
                    GridType = MainJewelry.Grid[0].Link.GridType,
                    Slot = MainJewelry.Grid[0].Link.Slot
                },
                ExtraItem1 = new CellLinkInfo
                {
                    Count = AccessoryJewelry1.Grid[0].LinkedCount,
                    GridType = AccessoryJewelry1.Grid[0].Link.GridType,
                    Slot = AccessoryJewelry1.Grid[0].Link.Slot
                },
                ExtraItem2 = mainItem.Info.Rarity == Rarity.Common ? new CellLinkInfo
                {
                    Count = AccessoryJewelry2.Grid[0].LinkedCount,
                    GridType = AccessoryJewelry2.Grid[0].Link.GridType,
                    Slot = AccessoryJewelry2.Grid[0].Link.Slot
                } : null,
                Corundum = new CellLinkInfo
                {
                    Count = Corundum.Grid[0].LinkedCount,
                    GridType = Corundum.Grid[0].Link.GridType,
                    Slot = Corundum.Grid[0].Link.Slot
                },
                Crystal = Crystal.Grid[0]?.Link?.Item == null ? null : new CellLinkInfo
                {
                    Count = Crystal.Grid[0].LinkedCount,
                    GridType = Crystal.Grid[0].Link.GridType,
                    Slot = Crystal.Grid[0].Link.Slot
                }

            });

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (CloseButton != null)
                {
                    if (!CloseButton.IsDisposed)
                        CloseButton.Dispose();

                    CloseButton = null;
                }

                if (SuccessRate != null)
                {
                    if (!SuccessRate.IsDisposed)
                        SuccessRate.Dispose();

                    SuccessRate = null;
                }

                if (PromotionRate != null)
                {
                    if (!PromotionRate.IsDisposed)
                        PromotionRate.Dispose();

                    PromotionRate = null;
                }

                if (MainJewelry != null)
                {
                    if (!MainJewelry.IsDisposed)
                        MainJewelry.Dispose();

                    MainJewelry = null;
                }

                if (AccessoryJewelry1 != null)
                {
                    if (!AccessoryJewelry1.IsDisposed)
                        AccessoryJewelry1.Dispose();

                    AccessoryJewelry1 = null;
                }

                if (AccessoryJewelry2 != null)
                {
                    if (!AccessoryJewelry2.IsDisposed)
                        AccessoryJewelry2.Dispose();

                    AccessoryJewelry2 = null;
                }

                if (Corundum != null)
                {
                    if (!Corundum.IsDisposed)
                        Corundum.Dispose();

                    Corundum = null;
                }

                if (Crystal != null)
                {
                    if (!Crystal.IsDisposed)
                        Crystal.Dispose();

                    Crystal = null;
                }

                if (CombineButton != null)
                {
                    if (!CombineButton.IsDisposed)
                        CombineButton.Dispose();

                    CombineButton = null;
                }

                if (AttributeComboBox != null)
                {
                    if (!AttributeComboBox.IsDisposed)
                        AttributeComboBox.Dispose();

                    AttributeComboBox = null;
                }
            }
        }
    }

    #endregion

    #region NPC回购
    /// <summary>
    /// NPC回购
    /// </summary>
    public sealed class NPCBuyBackDialog : NPCGoodsDialog
    {

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
                Pages.Text = $@"{value} / {TotalPage}";
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
                Pages.Text = $@"{CurrentPage} / {value}";
            }
        }

        private DXLabel Pages;

        private DXGoodGrid SellGrid;

        private DXControl RightPanel;

        private DXButton BuyButton2, LeftButton, RightButton, Close2Button;
        public NPCBuyBackDialog() : base()
        {
            HasTopBorder = false;
            Opacity = 0f;

            ClientPanel.Index = 1260;
            //调整原坐标
            //ClientPanel.Location = ClientPanel.Location.Add(new Point(0, 0));
            //GoodsPanel.Location = new Point(19, 32);

            //ScrollBar.Location = ScrollBar.Location.Add(new Point(0, 0));

            //右侧
            RightPanel = new DXImageControl
            {
                Index = 1261,
                LibraryFile = LibraryFile.UI1,
                Size = new Size(200, Size.Height),
                Location = ClientPanel.Location.Add(new Point(291, 0)),
                Parent = this,
            };

            SellGrid = new DXGoodGrid
            {
                GridSize = new Size(4, 3),
                Parent = RightPanel,
                VisibleHeight = 8,
                Border = false,
                Visible = false,
                Location = new Point(23, 40)
            };
            SellGrid.CellDbClick = () => BuyItem();
            BuyButton2 = new DXButton     //购买按钮
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1265,
                Location = new Point(67, SellGrid.Location.Y + SellGrid.Size.Height + 6),
                Parent = RightPanel
            };
            BuyButton2.MouseClick += BuyButton2_MouseClick;

            LeftButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1268,
                Location = new Point(BuyButton2.Location.X - 40 - 3, BuyButton2.Location.Y),
                Parent = RightPanel
            };

            LeftButton.MouseClick += LeftButton_MouseClick;

            RightButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1270,
                Location = new Point(BuyButton2.Location.X + BuyButton2.Size.Width + 3, BuyButton2.Location.Y),
                Parent = RightPanel
            };
            RightButton.MouseClick += RightButton_MouseClick;

            Pages = new DXLabel
            {
                Parent = RightPanel,
                Size = new Size(105, 20),
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White
            };
            Pages.Location = new Point(46, 7);

            Close2Button = new DXButton   //关闭按钮
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1221,
            };
            Close2Button.Location = new Point(448, 157);
            Close2Button.MouseClick += (o, e) => SetClientSize(new Size(272, 4 * 43 + 35));
        }

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);
            if (!nValue)
            {
                RightPanel.Visible = false;
                Close2Button.Visible = false;
            }
        }


        private void RightButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (CurrentPage + 1 > TotalPage) return;
            CurrentPage += 1;
            GoBuy();
        }

        private void LeftButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (CurrentPage - 1 < 1) return;
            CurrentPage -= 1;
            GoBuy();
        }

        private void BuyButton2_MouseClick(object sender, MouseEventArgs e)
        {
            BuyItem();
        }
        private void BuyItem()
        {
            if (SellGrid.SelectedGoodCell?.ItemCell == null) return;
            if (MapObject.User.Stats[Stat.BagWeight] - MapObject.User.BagWeight < SellGrid.SelectedGoodCell.Good.Item.Weight)
            {
                GameScene.Game.ReceiveChat($"NPCDialog.BuyCount".Lang(SelectedCell.Good.Item.ItemName), MessageType.System);
                return;
            }
            long gold = MapObject.User.Gold;

            if (GuildCheckBox.Checked && GameScene.Game.GuildBox.GuildInfo != null)
                gold = GameScene.Game.GuildBox.GuildInfo.GuildFunds;

            if (SellGrid.SelectedGoodCell.Good.Cost > gold)
            {
                GameScene.Game.ReceiveChat($"NPCDialog.BuyGold".Lang(SelectedCell.Good.Item.ItemName), MessageType.System);
                return;
            }

            CEnvir.Enqueue(new C.NPCBuy
            {
                Index = Convert.ToInt32(SellGrid.SelectedGoodCell.ItemCell.Tag),
                IsBuyback = true,
                Amount = 1,
                GuildFunds = GuildCheckBox.Checked,
                PageIndex = CurrentPage
            });
            GuildCheckBox.Checked = false;
        }
        public void AddGoodCell(ClientNPCGood good)
        {
            NPCGoodsCell cell;
            if (good.Currency != Globals.Currency)           //买卖货币图标更改
            {
                // 找到目标物品的image值
                ItemInfo targetIteminfo = Globals.ItemInfoList.Binding.First(x => x.ItemName == good.Currency);
                // 作为参数传入
                Cells.Add(cell = new NPCGoodsCell(targetIteminfo.Image)
                {
                    Parent = GoodsPanel,
                    Good = good
                });
            }
            else
            {
                Cells.Add(cell = new NPCGoodsCell
                {
                    Parent = GoodsPanel,
                    Good = good,
                });
            }
            cell.MouseClick += (o, e) =>
            {
                SelectedCell = cell;
                GoodsPanel.InvokeMouseClick();
            };
            //cell.MouseClick += (o, e) => SelectedCell = cell;
            cell.ItemCell.MouseClick += (o, e) => SelectedCell = cell;
            cell.ItemDetial.MouseClick += (o, e) => SelectedCell = cell;

            cell.ItemCell.MouseWheel += ScrollBar.DoMouseWheel;
            cell.ItemDetial.MouseWheel += ScrollBar.DoMouseWheel;

            cell.ItemCell.MouseDoubleClick += (o, e) => Buy();
            cell.ItemDetial.MouseDoubleClick += (o, e) => Buy();
        }
        public void NewGoods(IList<ClientNPCGood> goods, int pageIndex, int total)
        {

            if (pageIndex == 1)
            {
                NewGoods(goods);
                ScrollBar.MaxValue = (total - 1) * 8 * ScrollBar.Change;
                _maxValue = 0;
            }
            else
            {
                for (var i = 0; i < goods.Count; i++)
                {
                    AddGoodCell(goods[i]);
                }
                ScrollBar.MaxValue = (total - 1) * 8 * ScrollBar.Change;
                base.UpdateLocations();
            }
        }
        public override void NewGoods(IList<ClientNPCGood> goods)
        {
            base.NewGoods(goods);
            ScrollBar.Change = 42;
            GoodsPanel.Size = new Size(GoodsPanel.Size.Width, ScrollBar.Change * 4);
            ScrollBar.VisibleSize = 100;
            ScrollBar.Size = new Size(ScrollBar.Size.Width, 150);
            BuyButton.Location = BuyButton.Location.Add(new Point(0, 4));
        }

        int _maxValue = 0;
        protected override void UpdateLocations()
        {
            if (ScrollBar.Value - _maxValue > ScrollBar.Change * 8)
            {
                ScrollBar.Value = _maxValue + ScrollBar.Change;
            }
            var value = ScrollBar.Value;
            if (value > _maxValue && (value * 1f / ScrollBar.Change == 1 || value / ScrollBar.Change % 8 == 0))
            {
                _maxValue = value;
                PageIndex = value / ScrollBar.Change / 8 + 2;
                CEnvir.Enqueue(new C.NPCBuyBack { PageIndex = PageIndex });
                return;
            }

            var size = ScrollBar.Value % ScrollBar.Change;

            int y = -(size != 0 ? ScrollBar.Value - size + ScrollBar.Change : ScrollBar.Value);

            foreach (NPCGoodsCell cell in Cells)
            {
                cell.Location = new Point(10, y);

                y += cell.Size.Height + 3;
            }
        }
        protected override void Buy()
        {
            CurrentPage = 1;
            GoBuy();
        }
        private void GoBuy()
        {
            if (GameScene.Game.Observer) return;

            if (SelectedCell == null) return;

            var item = SelectedCell.ItemCell.Item?.Info;
            if (null == item) return;

            RightPanel.Visible = true;

            CEnvir.Enqueue(new C.NPCBuyBackSeach { Index = item.Index, PageIndex = CurrentPage });
            SetClientSize(new Size(520, 225));
        }

        public void UpdateSellGrid(S.NPCBuyBackSeach seach)
        {
            ClientNPCGood[] goods = seach.BackGoods.ToArray();
            SellGrid.Visible = true;
            SellGrid.ItemGrid = goods;
            CurrentPage = seach.PageIndex;
            TotalPage = seach.TotalPage;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                SellGrid.TryDispose();
                RightPanel.TryDispose();
                BuyButton2.TryDispose();
                LeftButton.TryDispose();
                RightButton.TryDispose();
                Pages.TryDispose();
                Close2Button.TryDispose();
            }
        }
    }
    #endregion

}