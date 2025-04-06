using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Library;
using System.Drawing;
using C = Library.Network.ClientPackets;
using Font = MonoGame.Extended.Font;
using FontStyle = MonoGame.Extended.FontStyle;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 传奇宝箱功能
    /// </summary>
    public sealed class TreasureChest : DXImageControl
    {
        public DXButton Decision, Reset;  //决定 结束 重置
        public DXImageControl Number;        //数量
        public DXLabel ExplainLabel, NumberLabel;   //说明标签   选择标签   数量标签

        public DXItemGrid[] TreasureGrid = new DXItemGrid[15];   //宝箱格子    
        public ClientUserItem[] TreasureArray;  //宝箱数组

        /// <summary>
        /// 传奇宝箱主界面
        /// </summary>
        public TreasureChest()
        {
            LibraryFile = LibraryFile.GameInter2;
            Index = 2900;

            Decision = new DXButton                      //决定按钮
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2912,
                Location = new Point(160, 245)
            };
            Decision.MouseClick += (o, e) =>
            {
                GameScene.Game.TreasureChestBox.Visible = false;
                GameScene.Game.LuckDrawBox.Visible = true;
            };

            Number = new DXImageControl                //数量底图
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2921,
                Location = new Point(15, 235)
            };

            NumberLabel = new DXLabel
            {
                Parent = Number,
                BorderColour = Color.FromArgb(99, 83, 50),
                ForeColour = Color.Moccasin,
                Text = "Treasure.Number".Lang(),
                Location = new Point(15, 2)
            };

            Reset = new DXButton                      //重置按钮
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2926,
                BorderColour = Color.FromArgb(99, 83, 50),
                ForeColour = Color.White,
                Label = { Text = "Treasure.Reset".Lang() },
                Location = new Point(15, 260)
            };
            Reset.MouseClick += (o, e) => { CEnvir.Enqueue(new C.TreasureChange { }); };

            ExplainLabel = new DXLabel          //选择奖励文本
            {
                Parent = this,
                BorderColour = Color.FromArgb(99, 83, 50),
                ForeColour = Color.White,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                Text = "Treasure.Explain".Lang(),
                Location = new Point(15, 170)
            };

            for (int i = 0; i < TreasureGrid.Length; i++)
            {
                TreasureGrid[i] = new DXItemGrid   //宝箱物品格子
                {
                    Parent = this,
                    Location = new Point(22 + i % 5 * 44, 28 + i / 5 * 44),  //位置  
                    GridSize = new Size(1, 1),
                    ItemGrid = new ClientUserItem[1],
                    ReadOnly = true,                         //只读
                };
            }
        }
    }
    /// <summary>
    /// 传奇宝箱抽奖界面
    /// </summary>
    public sealed class LuckDraw : DXImageControl
    {
        public DXButton End;           //结束 
        public DXLabel ChoiceLabel;    //选择标签

        public DXItemGrid[] TreasureGrid = new DXItemGrid[15];   //宝箱格子    
        public ClientUserItem[] TreasureArray;  //宝箱数组
        public DXImageControl[] GridImage = new DXImageControl[15];

        /// <summary>
        /// 传奇宝箱抽奖
        /// </summary>
        public LuckDraw()
        {
            LibraryFile = LibraryFile.GameInter2;
            Index = 2900;

            ChoiceLabel = new DXLabel   //选择提示标签
            {
                Parent = this,
                BorderColour = Color.FromArgb(99, 83, 50),
                ForeColour = Color.White,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                Text = "LuckDraw.Choice".Lang(),
                Location = new Point(15, 170)
            };

            End = new DXButton     //结束按钮
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 2917,
                Location = new Point(90, 245)
            };
            End.MouseClick += (o, e) =>  //鼠标点击时
            {
                for (int i = 0; i < TreasureGrid.Length; i++)   //画抽奖的物品到箱子上
                    TreasureGrid[i].ItemGrid[0] = null;
                for (int i = 0; i < TreasureGrid.Length; i++)  //画箱子图片到格子里
                    GridImage[i].Visible = true;
                ChoiceLabel.Text = "LuckDraw.End".Lang();
                Visible = false;   //关闭界面              
            };

            for (int i = 0; i < TreasureGrid.Length; i++)  //画箱子图片到格子里
                GridImage[i] = new DXImageControl
                {
                    Parent = this,
                    LibraryFile = LibraryFile.GameInter2,
                    Index = 2930,
                    Location = new Point(22 + i % 5 * 44, 28 + i / 5 * 44),
                };

            for (int i = 0; i < TreasureGrid.Length; i++)   //画抽奖的物品到箱子上
            {
                TreasureGrid[i] = new DXItemGrid   //宝箱物品格子
                {
                    Parent = this,
                    Location = new Point(22 + i % 5 * 44, 28 + i / 5 * 44),  //位置坐标  
                    GridSize = new Size(1, 1),
                    ItemGrid = new ClientUserItem[1],
                    ReadOnly = true,                         //只读
                    Opacity = 0.5F,           //把格子做透明度处理，其实这里格子是显示在箱子图片上面
                    Tag = i,
                };
                foreach (DXItemCell cell in TreasureGrid[i].Grid)
                {
                    cell.MouseDoubleClick += (o, e) =>
                    {
                        if (cell.Item != null) return;
                        CEnvir.Enqueue(new C.TreasureSelect { Slot = (int)cell.Parent.Tag });
                    };
                }
            }
        }
    }
}