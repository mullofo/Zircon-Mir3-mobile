using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Scenes;
using Client.Scenes.Configs;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static Client.Scenes.Views.BigPatchDialog;
using S = Library.Network.ServerPackets;

namespace Client.Models
{
    /// <summary>
    /// 道具对象
    /// </summary>
    public sealed class ItemObject : MapObject
    {
        public override ObjectType Race => ObjectType.Item;
        /// <summary>
        /// 焦点标签
        /// </summary>
        public DXLabel FocusLabel;

        public override bool Blocking => false;
        /// <summary>
        /// 角色道具
        /// </summary>
        public ClientUserItem Item;
        /// <summary>
        /// 图库
        /// </summary>
        public MirLibrary BodyLibrary;
        /// <summary>
        /// 标签背景颜色
        /// </summary>
        public Color LabelBackColour = Color.FromArgb(30, 0, 24, 48);

        /// <summary>
        /// 道具对象 道具信息
        /// </summary>
        /// <param name="info"></param>
        public ItemObject(S.ObjectItem info)
        {
            ObjectID = info.ObjectID;

            Item = info.Item;

            ItemInfo itemInfo = info.Item.Info;

            if (info.Item.Info.Effect == ItemEffect.ItemPart)
            {
                itemInfo = Globals.ItemInfoList.Binding.First(x => x.Index == Item.AddedStats[Stat.ItemIndex]);

                Title = "[" + "碎片".Lang() + "]";
            }

            Name = Item.Count > 1 ? $"{itemInfo.Lang(p => p.ItemName)} ({Item.Count})" : itemInfo.Lang(p => p.ItemName);

            if ((Item.Flags & UserItemFlags.QuestItem) == UserItemFlags.QuestItem)
                Title = "(" + "任务".Lang() + ")";

            switch (itemInfo.Rarity)   //道具极品类型
            {
                //地面字体颜色 光效
                case Rarity.Common:
                    if (Item.AddedStats.Values.Count > 0 && Item.Info.Effect != ItemEffect.ItemPart)
                    {
                        NameColour = Config.ItemNameGroundCommonColour;

                        if (BigPatchConfig.ChkItemObjBeam)
                        {
                            Effects.Add(new MirEffect(100, 10, TimeSpan.FromMilliseconds(100), LibraryFile.ProgUse, 60, 60, Color.Green)
                            {
                                Target = this,
                                Loop = true,
                                Blend = true,
                                BlendRate = 0.8F,
                            });
                        }
                    }
                    else
                    {
                        switch (itemInfo.ItemType)
                        {
                            case ItemType.Weapon:
                            case ItemType.Armour:
                            case ItemType.Helmet:
                            case ItemType.Necklace:
                            case ItemType.Bracelet:
                            case ItemType.Ring:
                            case ItemType.Shoes:
                            case ItemType.Shield:
                                NameColour = Config.ItemNameGroundEquipColour;
                                break;
                            case ItemType.Nothing:
                            case ItemType.Book:
                            case ItemType.Material:
                                NameColour = Config.ItemNameGroundNothingColour;
                                break;
                            case ItemType.Consumable:
                                NameColour = Config.ItemNameGroundConsumableColour;
                                break;
                            default:
                                NameColour = Config.ItemNameGroundColour;
                                break;
                        }

                        if (BigPatchConfig.ChkItemObjShining)
                        {
                            Effects.Add(new MirEffect(40, 8, TimeSpan.FromMilliseconds(100), LibraryFile.ProgUse, 60, 60, Color.MediumPurple)
                            {
                                Target = this,
                                Loop = true,
                                Blend = true,
                                BlendRate = 0.8F,
                            });
                        }
                    }
                    break;
                case Rarity.Superior:
                    NameColour = Config.ItemNameGroundSuperiorColour;
                    if (BigPatchConfig.ChkItemObjShining && !BigPatchConfig.ChkItemObjBeam)
                    {
                        Effects.Add(new MirEffect(40, 8, TimeSpan.FromMilliseconds(100), LibraryFile.ProgUse, 60, 60, Color.MediumPurple)
                        {
                            Target = this,
                            Loop = true,
                            Blend = true,
                            BlendRate = 0.8F,
                        });
                    }
                    else
                    {
                        if (BigPatchConfig.ChkItemObjBeam && Item.AddedStats.Values.Count > 0)
                        {
                            Effects.Add(new MirEffect(130, 10, TimeSpan.FromMilliseconds(100), LibraryFile.ProgUse, 60, 60, Color.MediumPurple)
                            {
                                Target = this,
                                Loop = true,
                                Blend = true,
                                BlendRate = 0.6F,
                            });
                        }
                    }
                    break;
                case Rarity.Elite:
                    NameColour = Config.ItemNameGroundEliteColour;
                    if (BigPatchConfig.ChkItemObjShining && !BigPatchConfig.ChkItemObjBeam)
                    {
                        Effects.Add(new MirEffect(40, 8, TimeSpan.FromMilliseconds(100), LibraryFile.ProgUse, 60, 60, Color.MediumPurple)
                        {
                            Target = this,
                            Loop = true,
                            Blend = true,
                            BlendRate = 0.8F,
                        });
                    }
                    else
                    {
                        if (BigPatchConfig.ChkItemObjBeam && Item.AddedStats.Values.Count > 0)
                        {
                            Effects.Add(new MirEffect(120, 10, TimeSpan.FromMilliseconds(100), LibraryFile.ProgUse, 60, 60, Color.Purple)
                            {
                                Target = this,
                                Loop = true,
                                Blend = true,
                                BlendRate = 0.8F,
                            });
                        }
                    }
                    break;
            }

            CItemFilterSet item = GameScene.Game?.BigPatchBox?.GetFilterItem(itemInfo.Lang(p => p.ItemName));
            //FilterItem item = GameScene.Game.AutoPotionBox.GetFilterItem(itemInfo.ItemName, itemInfo.Index);
            //if (item != null)
            //{
            if (itemInfo.Rarity == Rarity.Elite || itemInfo.Rarity == Rarity.Superior)
            {
                //>> 极品 [ 狂战戒指 204,81 ] 在 左上↖
                //计算方向
                GameScene.Game.ReceiveChat($">>  " + "发现".Lang() + $"  {itemInfo.Lang(p => p.ItemName)},   " + "位置".Lang() + $":  {CEnvir.GetDirName(User.CurrentLocation, info.Location)},   " + "坐标".Lang() + $":  {info.Location.X.ToString()},{info.Location.Y.ToString()}", MessageType.ItemTips);
            }
            //if (item.show)
            //{
            //    //FocusLabel.Visible = false;
            //    if (NameLabel !=null)
            //    NameLabel.Visible=false;
            //    if(TitleNameLabel !=null)
            //    TitleNameLabel.Visible = false;
            //    //return;
            //}
            //}
            CurrentLocation = info.Location;

            UpdateLibraries();

            SetFrame(new ObjectAction(MirAction.Standing, Direction, CurrentLocation));

            GameScene.Game.MapControl.AddObject(this);
        }
        /// <summary>
        /// 更新库
        /// </summary>
        public void UpdateLibraries()
        {
            Frames = FrameSet.DefaultItem;

            CEnvir.LibraryList.TryGetValue(LibraryFile.Ground, out BodyLibrary);
        }
        /// <summary>
        /// 设置动画
        /// </summary>
        /// <param name="action"></param>
        public override void SetAnimation(ObjectAction action)
        {

            CurrentAnimation = MirAnimation.Standing;
            if (!Frames.TryGetValue(CurrentAnimation, out CurrentFrame))
                CurrentFrame = Frame.EmptyFrame;
        }
        /// <summary>
        /// 绘制
        /// </summary>
        public override void Draw()
        {
            if (BodyLibrary == null) return;

            int drawIndex;

            if (Item.Info.Effect == ItemEffect.Gold)
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
            else
            {
                ItemInfo info = Item.Info;

                if (info.Effect == ItemEffect.ItemPart)
                    info = Globals.ItemInfoList.Binding.First(x => x.Index == Item.AddedStats[Stat.ItemIndex]);

                drawIndex = info.Image;
            }

            Size size = BodyLibrary.GetSize(drawIndex);

            BodyLibrary.Draw(drawIndex, DrawX + (CellWidth - size.Width) / 2, DrawY + (CellHeight - size.Height) / 2, DrawColour, false, 1F, ImageType.Image);
        }
        /// <summary>
        /// 鼠标悬停
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool MouseOver(Point p)
        {
            return false;
        }

        public override void OnRemoved()
        {
        }

        /// <summary>
        /// 道具名字变化
        /// </summary>
        public override void NameChanged()
        {
            base.NameChanged();

            if (string.IsNullOrEmpty(Name))
            {
                FocusLabel = null;
            }
            else
            {
                if (!NameLabels.TryGetValue(Name, out List<DXLabel> focused))
                    NameLabels[Name] = focused = new List<DXLabel>();

                FocusLabel = focused.FirstOrDefault(x => x.ForeColour == NameColour && x.BackColour == LabelBackColour);

                if (FocusLabel != null) return;

                FocusLabel = new DXLabel
                {
                    BackColour = LabelBackColour,
                    ForeColour = Color.White,
                    Outline = true,
                    OutlineColour = Color.Black,
                    Text = Name,
                    Border = true,
                    BorderColour = Color.Black,
                    IsVisible = true,
                };

                FocusLabel.Disposing += (o, e) => focused.Remove(FocusLabel);
                focused.Add(FocusLabel);
            }
        }
        /// <summary>
        /// 绘制道具字体
        /// </summary>
        /// <param name="layer"></param>
        public void DrawFocus(int layer)
        {
            FocusLabel.Location = new Point(DrawX + (48 - FocusLabel.Size.Width) / 2, DrawY - (32 - FocusLabel.Size.Height / 2) + 8 - layer * 16);
            FocusLabel.Draw();
        }
    }
}
