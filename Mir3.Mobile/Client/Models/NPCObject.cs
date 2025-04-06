using Client.Envir;
using Client.Extentions;
using Client.Scenes;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Client.Models
{
    /// <summary>
    /// NPC对象
    /// </summary>
    public sealed class NPCObject : MapObject
    {
        /// <summary>
        /// NPC信息
        /// </summary>
        public static Dictionary<NPCInfo, NPCObject> NPCs = new Dictionary<NPCInfo, NPCObject>();
        /// <summary>
        /// 对象为NPC
        /// </summary>
        public override ObjectType Race => ObjectType.NPC;
        /// <summary>
        /// 当前任务图标
        /// </summary>
        public QuestIcon CurrentIcon;
        /// <summary>
        /// 任务特效
        /// </summary>
        public MirEffect QuestEffect;
        /// <summary>
        /// NPC信息
        /// </summary>
        public NPCInfo NPCInfo;
        /// <summary>
        /// 主体库
        /// </summary>
        public MirLibrary BodyLibrary;
        /// <summary>
        /// 主体偏移量
        /// </summary>
        public int BodyOffSet = 100;
        /// <summary>
        /// 主体形态
        /// </summary>
        public int BodyShape;
        /// <summary>
        /// 旗帜颜色覆盖
        /// </summary>
        public Color overlay = Color.Empty;
        /// <summary>
        /// 主体框架
        /// </summary>
        public int BodyFrame => DrawFrame + BodyShape * BodyOffSet;

        /// <summary>
        /// 攻城旗帜NPC信息
        /// </summary>
        /// <param name="info"></param>
        public NPCObject(S.CustomNpc info)
        {
            ObjectID = info.ObjectID;
            Name = info.Name;
            NameColour = Color.Lime;
            BodyShape = info.ImagIndex;
            CurrentLocation = info.CurrentLocation;
            Direction = info.Direction;
            overlay = info.OverlayColor;
            if (CEnvir.LibraryList.TryGetValue(info.library, out BodyLibrary))
            {
                BodyLibrary.ReadLibrary();
                var frameCount = BodyLibrary.GetFrameCount(BodyFrame);
                var delay = 1f / frameCount;
                Frames = new Dictionary<MirAnimation, Frame>
                {
                    [MirAnimation.Standing] = new Frame(0, frameCount, 0, TimeSpan.FromSeconds(delay))
                };
            }
            else
            {
                Frames = FrameSet.DefaultNPC;
            }

            SetFrame(new ObjectAction(MirAction.Standing, MirDirection.Up, CurrentLocation));
            GameScene.Game?.MapControl.AddObject(this);

            // 检查是否有缓存的NPC外观更新
            if (CEnvir.UpdatedNPCLooks.TryGetValue(info.ObjectID, out var p))
            {
                this.UpdateNPCLook(p.NPCName, p.NameColor, p.OverlayColor, p.Library, p.ImageIndex);

                if (p.UpdateNPCIcon)
                {
                    this.UpdateNPCIcon(p.Icon);
                }
            }
        }

        /// <summary>
        /// NPC对象信息
        /// </summary>
        /// <param name="info"></param>
        public NPCObject(S.ObjectNPC info)
        {
            ObjectID = info.ObjectID;
            NPCInfo = Globals.NPCInfoList.Binding.First(x => x.Index == info.NPCIndex);
            Light = 3;

            string str = SplitName(NPCInfo.Lang(p => p.NPCName));

            Name = str;                      //NPC名字
            NameColour = Config.NPCNameColour;         //NPC名字颜色
            BodyShape = NPCInfo.Image;       //图像 =  NPC INFO 里的设置图片

            CurrentLocation = info.CurrentLocation;

            Direction = info.Direction;

            NPCs[NPCInfo] = this;

            CEnvir.LibraryList.TryGetValue(LibraryFile.NPC, out BodyLibrary);
            switch (NPCInfo.Image)
            {
                case 64:
                case 65:
                case 91:
                case 92:
                case 93:
                case 157:
                case 158:
                case 159:
                case 160:
                case 165:
                case 166:
                case 168:
                case 208:
                case 209:
                case 210:
                case 211:
                case 212:
                case 213:
                case 214:
                case 231:
                case 234:
                case 244:
                    Frames = new Dictionary<MirAnimation, Frame>        //单图站立
                    {
                        [MirAnimation.Standing] = new Frame(0, 1, 0, TimeSpan.FromHours(1))
                    };
                    break;
                case 176:
                case 182:
                case 185:
                case 186:
                case 187:
                case 190:
                case 196:
                case 199:
                case 200:
                case 201:
                    Frames = new Dictionary<MirAnimation, Frame>      //5图站立
                    {
                        [MirAnimation.Standing] = new Frame(0, 5, 0, TimeSpan.FromMilliseconds(200))
                    };
                    break;
                case 118:
                case 119:
                case 120:
                case 121:
                case 122:
                case 123:
                case 124:
                case 174:
                case 226:
                case 227:
                case 235:
                case 236:
                case 237:
                case 238:
                case 239:
                case 240:
                case 241:
                case 242:
                    Frames = new Dictionary<MirAnimation, Frame>     //6图站立
                    {
                        [MirAnimation.Standing] = new Frame(0, 6, 0, TimeSpan.FromMilliseconds(200))
                    };
                    break;
                case 132:
                    Frames = new Dictionary<MirAnimation, Frame>     //7图站立
                    {
                        [MirAnimation.Standing] = new Frame(0, 7, 0, TimeSpan.FromMilliseconds(200))
                    };
                    break;
                case 243:
                    Frames = new Dictionary<MirAnimation, Frame>    //8图站立
                    {
                        [MirAnimation.Standing] = new Frame(0, 8, 0, TimeSpan.FromMilliseconds(200))
                    };
                    break;
                case 87:
                case 161:
                case 162:
                case 163:
                case 164:
                case 205:
                case 207:
                case 232:
                    Frames = new Dictionary<MirAnimation, Frame>     //10图站立
                    {
                        [MirAnimation.Standing] = new Frame(0, 10, 0, TimeSpan.FromMilliseconds(200))
                    };
                    break;
                case 56:
                case 57:
                    Frames = new Dictionary<MirAnimation, Frame>    //12图站立
                    {
                        [MirAnimation.Standing] = new Frame(0, 12, 0, TimeSpan.FromMilliseconds(200))
                    };
                    break;
                case 156:
                    Frames = new Dictionary<MirAnimation, Frame>    //16图站立
                    {
                        [MirAnimation.Standing] = new Frame(0, 16, 0, TimeSpan.FromMilliseconds(200))
                    };
                    break;
                case 178:
                    Frames = new Dictionary<MirAnimation, Frame>    //10图站立
                    {
                        [MirAnimation.Standing] = new Frame(60, 10, 0, TimeSpan.FromMilliseconds(200))
                    };
                    break;
                case 179:
                    Frames = new Dictionary<MirAnimation, Frame>    //10图站立
                    {
                        [MirAnimation.Standing] = new Frame(60, 7, 0, TimeSpan.FromMilliseconds(200))
                    };
                    break;
                default:
                    Frames = FrameSet.DefaultNPC;
                    break;
            }

            SetFrame(new ObjectAction(MirAction.Standing, MirDirection.Up, CurrentLocation));

            GameScene.Game.MapControl.AddObject(this);

            // 检查是否有缓存的NPC外观更新
            if (CEnvir.UpdatedNPCLooks.TryGetValue(info.ObjectID, out var p))
            {
                this.UpdateNPCLook(p.NPCName, p.NameColor, p.OverlayColor, p.Library, p.ImageIndex);

                if (p.UpdateNPCIcon)
                {
                    this.UpdateNPCIcon(p.Icon);
                }
            }

            UpdateQuests();
        }
        /// <summary>
        /// 名字居中由DXLabel负责
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string SplitName(string str)
        {
            if (!str.Contains(" "))
            {
                return str;
            }

            return str.Replace(" ", "\r\n");
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

            DrawShadow();

            DrawBody();
        }
        /// <summary>
        /// 绘制阴影
        /// </summary>
        private void DrawShadow()
        {
            BodyLibrary.Draw(BodyFrame, DrawX, DrawY, Color.White, true, 0.5f, ImageType.Shadow);
        }
        /// <summary>
        /// 绘制主体
        /// </summary>
        private void DrawBody()
        {
            BodyLibrary.Draw(BodyFrame, DrawX, DrawY, DrawColour, true, 1F, ImageType.Image);

            if (overlay != Color.Empty)
            {
                BodyLibrary.Draw(BodyFrame, DrawX, DrawY, overlay, true, 1F, ImageType.Overlay);
            }
        }
        /// <summary>
        /// 混合绘制
        /// </summary>
        public override void DrawBlend()
        {
            if (BodyLibrary == null) return;

            DXManager.SetBlend(true, 0.60F, blendtype: BlendType.HIGHLIGHT);
            DrawBody();
            DXManager.SetBlend(false);
        }
        /// <summary>
        /// 鼠标悬停
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool MouseOver(Point p)
        {
            return BodyLibrary != null && BodyLibrary.VisiblePixel(BodyFrame, new Point(p.X - DrawX, p.Y - DrawY), false, true);
        }

        public override void OnRemoved()
        {
        }

        /// <summary>
        /// 更新任务状态
        /// </summary>
        public override void UpdateQuests()
        {
            if (NPCInfo == null) return;  //如果NPC信息等空 跳出

            if (NPCInfo.CurrentIcon == QuestIcon.None)  //如果当前任务图标为空
            {
                RemoveQuestEffect();  //移除任务状态效果
                return;               //跳出
            }

            if (CurrentIcon == NPCInfo.CurrentIcon) return;    //如果当前任务图标是NPC信息的任务图标 跳出

            RemoveQuestEffect();   //移除任务状态效果

            CurrentIcon = NPCInfo.CurrentIcon;   //当前任务图标等NPC信息里的任务图标

            int startIndex = 0;

            if ((CurrentIcon & QuestIcon.QuestComplete) == QuestIcon.QuestComplete)   //如果任务完成显示1130图标
                startIndex = 1130;
            else if ((CurrentIcon & QuestIcon.NewQuest) == QuestIcon.NewQuest)   //如果任务是新任务显示1080图标
                startIndex = 1080;
            else if ((CurrentIcon & QuestIcon.QuestIncomplete) == QuestIcon.QuestIncomplete)  //如果任务是进行中的任务显示1090图标
                startIndex = 1090;

            QuestEffect = new MirEffect(startIndex, 2, TimeSpan.FromMilliseconds(500), LibraryFile.GameInter, 0, 0, Color.Empty)
            {
                Loop = true,
                MapTarget = CurrentLocation,
                Blend = false,
                DrawType = DrawType.Final,
                AdditionalOffSet = new Point(0, -80)
            };
            QuestEffect.Process();
        }
        /// <summary>
        /// 移除任务状态效果
        /// </summary>
        public void RemoveQuestEffect()
        {
            CurrentIcon = QuestIcon.None;  //任务图标等空
            QuestEffect?.Remove();         //任务特效移除
            QuestEffect = null;            //任务特效等空
        }
        /// <summary>
        /// 移除
        /// </summary>
        public override void Remove()
        {
            base.Remove();

            if (NPCInfo == null) return;   //如果NPC信息等空跳出
            RemoveQuestEffect();           //移除任务状态效果
            NPCs.Remove(NPCInfo);          //NPC移除NPC信息
        }


        // 更新NPC外观
        public void UpdateNPCLook(string name, Color nameColor, Color overlayColor,
            LibraryFile libraryFile, int bodyShape)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Name = SplitName(name);
            }

            if (nameColor != Color.Empty && nameColor != Config.NPCNameColour)
            {
                NameColour = nameColor;
            }

            if (overlayColor != default)
            {
                overlay = overlayColor;
            }

            if (libraryFile != LibraryFile.None)
            {
                CEnvir.LibraryList.TryGetValue(libraryFile, out BodyLibrary);
            }

            if (bodyShape != 0)
            {
                BodyShape = bodyShape;
            }

            switch (BodyShape)
            {
                case 64:
                case 65:
                case 91:
                case 92:
                case 93:
                case 157:
                case 158:
                case 159:
                case 160:
                case 165:
                case 166:
                case 168:
                case 208:
                case 209:
                case 210:
                case 211:
                case 212:
                case 213:
                case 214:
                case 231:
                case 234:
                case 244:
                    Frames = new Dictionary<MirAnimation, Frame>        //单图站立
                    {
                        [MirAnimation.Standing] = new Frame(0, 1, 0, TimeSpan.FromHours(1))
                    };
                    break;
                case 176:
                case 182:
                case 185:
                case 186:
                case 187:
                case 190:
                case 196:
                case 199:
                case 200:
                case 201:
                    Frames = new Dictionary<MirAnimation, Frame>      //5图站立
                    {
                        [MirAnimation.Standing] = new Frame(0, 5, 0, TimeSpan.FromMilliseconds(200))
                    };
                    break;
                case 118:
                case 119:
                case 120:
                case 121:
                case 122:
                case 123:
                case 124:
                case 174:
                case 226:
                case 227:
                case 235:
                case 236:
                case 237:
                case 238:
                case 239:
                case 240:
                case 241:
                case 242:
                    Frames = new Dictionary<MirAnimation, Frame>     //6图站立
                    {
                        [MirAnimation.Standing] = new Frame(0, 6, 0, TimeSpan.FromMilliseconds(200))
                    };
                    break;
                case 132:
                    Frames = new Dictionary<MirAnimation, Frame>     //7图站立
                    {
                        [MirAnimation.Standing] = new Frame(0, 7, 0, TimeSpan.FromMilliseconds(200))
                    };
                    break;
                case 243:
                    Frames = new Dictionary<MirAnimation, Frame>    //8图站立
                    {
                        [MirAnimation.Standing] = new Frame(0, 8, 0, TimeSpan.FromMilliseconds(200))
                    };
                    break;
                case 87:
                case 161:
                case 162:
                case 163:
                case 164:
                case 205:
                case 207:
                case 232:
                    Frames = new Dictionary<MirAnimation, Frame>     //10图站立
                    {
                        [MirAnimation.Standing] = new Frame(0, 10, 0, TimeSpan.FromMilliseconds(200))
                    };
                    break;
                case 56:
                case 57:
                    Frames = new Dictionary<MirAnimation, Frame>    //12图站立
                    {
                        [MirAnimation.Standing] = new Frame(0, 12, 0, TimeSpan.FromMilliseconds(200))
                    };
                    break;
                case 156:
                    Frames = new Dictionary<MirAnimation, Frame>    //16图站立
                    {
                        [MirAnimation.Standing] = new Frame(0, 16, 0, TimeSpan.FromMilliseconds(200))
                    };
                    break;
                case 178:
                    Frames = new Dictionary<MirAnimation, Frame>    //10图站立
                    {
                        [MirAnimation.Standing] = new Frame(60, 10, 0, TimeSpan.FromMilliseconds(200))
                    };
                    break;
                case 179:
                    Frames = new Dictionary<MirAnimation, Frame>    //10图站立
                    {
                        [MirAnimation.Standing] = new Frame(60, 7, 0, TimeSpan.FromMilliseconds(200))
                    };
                    break;
                default:
                    Frames = FrameSet.DefaultNPC;
                    break;
            }

            SetFrame(new ObjectAction(MirAction.Standing, MirDirection.Up, CurrentLocation));

        }

        // 更新头顶图标
        public void UpdateNPCIcon(QuestIcon icon)
        {
            this.NPCInfo.CurrentIcon = icon;

            GameScene.Game?.MiniMapBox?.Update(this.NPCInfo);
            GameScene.Game?.BigMapBox?.Update(this.NPCInfo);

            //GameScene.Game.ChatBox.Photo.Update(this.NPCInfo); // 这个应该不需要
            UpdateQuests();
        }
    }
}

