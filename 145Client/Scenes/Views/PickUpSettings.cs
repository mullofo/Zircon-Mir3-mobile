using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.UserModels;
using Library;
using System.Drawing;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 宠物捡取过滤功能
    /// </summary>
    public sealed class PickUpSettings : DXWindow
    {
        #region Properties
        public DXButton ToSelectButton, ExitButton;

        public override WindowType Type => WindowType.ExitBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        public DXCheckBox SetGoldBox;           //金币
        public DXCheckBox SetWeaponBox;         //武器
        public DXCheckBox SetArmourBox;         //衣服
        public DXCheckBox SetHelmetBox;         //头盔
        public DXCheckBox SetShieldBox;         //盾牌
        public DXCheckBox SetNecklaceBox;       //项链
        public DXCheckBox SetBraceletBox;       //手镯
        public DXCheckBox SetRingBox;           //戒指
        public DXCheckBox SetShoesBox;          //鞋子
        public DXCheckBox SetBookBox;           //书籍
        public DXCheckBox SetPotionBox;         //毒药
        public DXCheckBox SetMeatBox;           //肉
        public DXCheckBox SetCommonBox;         //普通物品
        public DXCheckBox SetSuperiorBox;       //高级物品 
        public DXCheckBox SetEliteBox;          //稀世物品
        public DXCheckBox SetPartsBox;          //碎片
        public DXCheckBox SetConsumableBox;     //药品
        public DXCheckBox SetFishBox;           //鱼类
        public DXCheckBox SetGemBox;            //附魔石
        public DXCheckBox SetDrillBox;          //穿孔材料
        public DXCheckBox SetFashionBox;        //时装
        public DXCheckBox SetMaterialBox;       //材料

        public DXCheckBox[] SetCheckBoxs = new DXCheckBox[(int)ItemType.Medicament + 1];
        public DXCheckBox[] SetItemCheckBox = new DXCheckBox[(int)Rarity.Elite + 1];

        #endregion

        /// <summary>
        /// 宠物捡取过滤面板
        /// </summary>
        public PickUpSettings()
        {
            TitleLabel.Text = @"拾取设置".Lang();

            SetClientSize(new Size(150, 400));

            DXLabel label = new DXLabel
            {
                Text = "道具类型过滤".Lang(),
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                Outline = true,
                Parent = this,         //放在不同的框架内
                Location = new Point(40, 40)
            };

            DXLabel label1 = new DXLabel
            {
                Text = "道具分级过滤".Lang(),
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                Outline = true,
                Parent = this,
                Location = new Point(40, 340)
            };

            SetCheckBoxs[(int)ItemType.Nothing] = SetGoldBox = new DXCheckBox
            {
                Label = { Text = "其他".Lang() },
                Parent = this,
                Checked = true,
            };
            SetGoldBox.Location = new Point(22, 60);

            SetGoldBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.ComSortingConfChanged { Enabled = SetGoldBox.Checked, Slot = ItemType.Nothing });
            };

            SetCheckBoxs[(int)ItemType.Weapon] = SetWeaponBox = new DXCheckBox
            {
                Label = { Text = "武器".Lang() },
                Parent = this,
                Checked = true,
            };
            SetWeaponBox.Location = new Point(22, 80);
            SetWeaponBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.ComSortingConfChanged { Enabled = SetWeaponBox.Checked, Slot = ItemType.Weapon });
            };

            SetCheckBoxs[(int)ItemType.Armour] = SetArmourBox = new DXCheckBox
            {
                Label = { Text = "衣服".Lang() },
                Parent = this,
                Checked = true,
            };
            SetArmourBox.Location = new Point(22, 100);
            SetArmourBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.ComSortingConfChanged { Enabled = SetArmourBox.Checked, Slot = ItemType.Armour });
            };

            SetCheckBoxs[(int)ItemType.Helmet] = SetHelmetBox = new DXCheckBox
            {
                Label = { Text = "头盔".Lang() },
                Parent = this,
                Checked = true,
            };
            SetHelmetBox.Location = new Point(22, 120);
            SetHelmetBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.ComSortingConfChanged { Enabled = SetHelmetBox.Checked, Slot = ItemType.Helmet });
            };

            SetCheckBoxs[(int)ItemType.Shield] = SetShieldBox = new DXCheckBox
            {
                Label = { Text = "盾牌".Lang() },
                Parent = this,
                Checked = true,
            };
            SetShieldBox.Location = new Point(22, 140);
            SetShieldBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.ComSortingConfChanged { Enabled = SetShieldBox.Checked, Slot = ItemType.Shield });
            };
            SetCheckBoxs[(int)ItemType.Necklace] = SetNecklaceBox = new DXCheckBox
            {
                Label = { Text = "项链".Lang() },
                Parent = this,
                Checked = true,
            };
            SetNecklaceBox.Location = new Point(22, 160);
            SetNecklaceBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.ComSortingConfChanged { Enabled = SetNecklaceBox.Checked, Slot = ItemType.Necklace });
            };

            SetCheckBoxs[(int)ItemType.Bracelet] = SetBraceletBox = new DXCheckBox
            {
                Label = { Text = "手镯".Lang() },
                Parent = this,
                Checked = true,
            };
            SetBraceletBox.Location = new Point(22, 180);
            SetBraceletBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.ComSortingConfChanged { Enabled = SetBraceletBox.Checked, Slot = ItemType.Bracelet });
            };

            SetCheckBoxs[(int)ItemType.Ring] = SetRingBox = new DXCheckBox
            {
                Label = { Text = "戒指".Lang() },
                Parent = this,
                Checked = true,
            };
            SetRingBox.Location = new Point(22, 200);
            SetRingBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.ComSortingConfChanged { Enabled = SetRingBox.Checked, Slot = ItemType.Ring });
            };

            SetCheckBoxs[(int)ItemType.Shoes] = SetShoesBox = new DXCheckBox
            {
                Label = { Text = "鞋子".Lang() },
                Parent = this,
                Checked = true,
            };
            SetShoesBox.Location = new Point(22, 220);
            SetShoesBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.ComSortingConfChanged { Enabled = SetShoesBox.Checked, Slot = ItemType.Shoes });
            };

            SetCheckBoxs[(int)ItemType.Book] = SetBookBox = new DXCheckBox
            {
                Label = { Text = "书籍".Lang() },
                Parent = this,
                Checked = true,
            };
            SetBookBox.Location = new Point(22, 240);
            SetBookBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.ComSortingConfChanged { Enabled = SetBookBox.Checked, Slot = ItemType.Book });
            };

            SetCheckBoxs[(int)ItemType.Poison] = SetPotionBox = new DXCheckBox
            {
                Label = { Text = "毒药".Lang() },
                Parent = this,
                Checked = true,
            };
            SetPotionBox.Location = new Point(22, 260);
            SetPotionBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.ComSortingConfChanged { Enabled = SetPotionBox.Checked, Slot = ItemType.Poison });
            };

            SetCheckBoxs[(int)ItemType.Meat] = SetMeatBox = new DXCheckBox
            {
                Label = { Text = "肉".Lang() },
                Parent = this,
                Checked = true,
            };
            SetMeatBox.Location = new Point(34, 280);
            SetMeatBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.ComSortingConfChanged { Enabled = SetMeatBox.Checked, Slot = ItemType.Meat });
            };

            SetCheckBoxs[(int)ItemType.Consumable] = SetConsumableBox = new DXCheckBox
            {
                Label = { Text = "药品".Lang() },
                Parent = this,
                Checked = false,
            };
            SetConsumableBox.Location = new Point(22, 300);
            SetConsumableBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.ComSortingConfChanged { Enabled = SetConsumableBox.Checked, Slot = ItemType.Consumable });
            };

            SetCheckBoxs[(int)ItemType.ItemPart] = SetPartsBox = new DXCheckBox
            {
                Label = { Text = "碎片".Lang() },
                Parent = this,
                Checked = true,
            };
            SetPartsBox.Location = new Point(22, 320);
            SetPartsBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.ComSortingConfChanged { Enabled = SetPartsBox.Checked, Slot = ItemType.ItemPart });
            };

            SetCheckBoxs[(int)ItemType.Fish] = SetFishBox = new DXCheckBox
            {
                Label = { Text = "鱼类".Lang() },
                Parent = this,
                Checked = true,
            };
            SetFishBox.Location = new Point(100, 60);

            SetFishBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.ComSortingConfChanged { Enabled = SetFishBox.Checked, Slot = ItemType.Fish });
            };

            SetCheckBoxs[(int)ItemType.Gem] = SetGemBox = new DXCheckBox
            {
                Label = { Text = "附魔石".Lang() },
                Parent = this,
                Checked = true,
            };
            SetGemBox.Location = new Point(88, 80);

            SetGemBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.ComSortingConfChanged { Enabled = SetGemBox.Checked, Slot = ItemType.Gem });
            };

            SetCheckBoxs[(int)ItemType.Drill] = SetDrillBox = new DXCheckBox
            {
                Label = { Text = "穿孔材料".Lang() },
                Parent = this,
                Checked = true,
            };
            SetDrillBox.Location = new Point(76, 100);

            SetDrillBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.ComSortingConfChanged { Enabled = SetDrillBox.Checked, Slot = ItemType.Drill });
            };

            SetCheckBoxs[(int)ItemType.Fashion] = SetFashionBox = new DXCheckBox
            {
                Label = { Text = "时装".Lang() },
                Parent = this,
                Checked = true,
            };
            SetFashionBox.Location = new Point(100, 120);
            SetFashionBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.ComSortingConfChanged { Enabled = SetFashionBox.Checked, Slot = ItemType.Fashion });
            };

            SetCheckBoxs[(int)ItemType.Material] = SetMaterialBox = new DXCheckBox
            {
                Label = { Text = "材料".Lang() },
                Parent = this,
                Checked = true,
            };
            SetMaterialBox.Location = new Point(100, 140);
            SetMaterialBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.ComSortingConfChanged { Enabled = SetMaterialBox.Checked, Slot = ItemType.Material });
            };


            SetItemCheckBox[(int)Rarity.Common] = SetCommonBox = new DXCheckBox
            {
                Label = { Text = "普通物品".Lang() },
                Parent = this,
                Checked = true,
            };
            SetCommonBox.Location = new Point(40, 360);
            SetCommonBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.ComSortingConf1Changed { Enabled = SetCommonBox.Checked, Slot = Rarity.Common });
            };

            SetItemCheckBox[(int)Rarity.Superior] = SetSuperiorBox = new DXCheckBox
            {
                Label = { Text = "高级物品".Lang() },
                Parent = this,
                Checked = true,
            };
            SetSuperiorBox.Location = new Point(40, 380);
            SetSuperiorBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.ComSortingConf1Changed { Enabled = SetSuperiorBox.Checked, Slot = Rarity.Superior });
            };

            SetItemCheckBox[(int)Rarity.Elite] = SetEliteBox = new DXCheckBox
            {
                Label = { Text = "稀世物品".Lang() },
                Parent = this,
                Checked = true,
            };
            SetEliteBox.Location = new Point(40, 400);
            SetEliteBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.ComSortingConf1Changed { Enabled = SetEliteBox.Checked, Slot = Rarity.Elite });
            };
        }

        /// <summary>
        /// 宠物改变时
        /// </summary>
        public void CompanionChanged()
        {
            if (GameScene.Game.Companion == null)
            {
                Visible = false;
                return;
            }
            foreach (ClientCompanionSortSet sort in GameScene.Game.Companion.Sorts)
            {
                SetCheckBoxs[(int)sort.SetType].Checked = sort.Enabled;
            }

            foreach (ClientCompanionSortLevSet sort in GameScene.Game.Companion.SortsLev)
            {
                SetItemCheckBox[(int)sort.SetType].Checked = sort.Enabled;
            }
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                //这里写入关闭窗口是注销的控件
            }
        }
        #endregion
    }
}
