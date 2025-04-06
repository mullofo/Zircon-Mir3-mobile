using Client.Envir;
using Client.Scenes;
using Library;
using SharpDX;
using System;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;

namespace Client.Models
{
    /// <summary>
    /// 特效
    /// </summary>
    public class MirEffect
    {
        /// <summary>
        /// 指定目标
        /// </summary>
        public MapObject Target;

        /// <summary>
        /// 地图坐标
        /// </summary>
        public Point MapTarget;

        /// <summary>
        /// 素材库
        /// </summary>
        public MirLibrary Library;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime;

        /// <summary>
        /// 起始索引
        /// </summary>
        public int StartIndex;

        /// <summary>
        /// 每个方向帧数
        /// </summary>
        public int FrameCount;

        /// <summary>
        /// 每帧的延时
        /// </summary>
        public TimeSpan[] Delays;

        /// <summary>
        /// 帧索引
        /// </summary>
        public int FrameIndex
        {
            get { return _FrameIndex; }
            set
            {
                if (_FrameIndex == value) return;

                _FrameIndex = value;
                FrameIndexAction?.Invoke();
            }
        }
        private int _FrameIndex;

        /// <summary>
        /// 绘制颜色
        /// </summary>
        public Color DrawColour = Color.White;
        /// <summary>
        /// 混合
        /// </summary>
        public bool Blend;
        /// <summary>
        /// 反转
        /// </summary>
        public bool Reversed;
        /// <summary>
        /// 透明度
        /// </summary>
        public float Opacity = 1F;
        /// <summary>
        /// 混合速率
        /// </summary>
        public float BlendRate = 1F;
        public BlendType BlendType = BlendType.NORMAL;
        /// <summary>
        /// 使用偏移量
        /// </summary>
        public bool UseOffSet = true;
        /// <summary>
        /// 循环
        /// </summary>
        public bool Loop = false;
        /// <summary>
        /// X绘制
        /// </summary>
        public int DrawX
        {
            get { return _DrawX; }
            set
            {
                if (_DrawX == value) return;

                _DrawX = value;
                GameScene.Game.MapControl.TextureValid = false;
            }
        }
        private int _DrawX;
        /// <summary>
        /// Y绘制
        /// </summary>
        public int DrawY
        {
            get { return _DrawY; }
            set
            {
                if (_DrawY == value) return;

                _DrawY = value;
                GameScene.Game.MapControl.TextureValid = false;
            }
        }
        private int _DrawY;
        /// <summary>
        /// 绘制框架
        /// </summary>
        public int DrawFrame
        {
            get { return _DrawFrmae; }
            set
            {
                if (_DrawFrmae == value) return;

                _DrawFrmae = value;
                GameScene.Game.MapControl.TextureValid = false;
                FrameAction?.Invoke();
            }
        }
        private int _DrawFrmae;

        /// <summary>
        /// 绘图类型 对象
        /// </summary>
        public DrawType DrawType = DrawType.Object;

        /// <summary>
        /// 略过
        /// </summary>
        public int Skip { get; set; }
        /// <summary>
        /// 方向
        /// </summary>
        public MirDirection Direction { get; set; }
        /// <summary>
        /// 光线颜色
        /// </summary>
        public Color[] LightColours;
        public int StartLight, EndLight;
        /// <summary>
        /// 框架光线
        /// </summary>
        public float FrameLight
        {
            get
            {
                if (CEnvir.Now < StartTime) return 0;

                TimeSpan enlapsed = CEnvir.Now - StartTime;

                if (Loop)
                    enlapsed = TimeSpan.FromTicks(enlapsed.Ticks % TotalDuration.Ticks);

                return StartLight + (EndLight - StartLight) * enlapsed.Ticks / TotalDuration.Ticks;

            }
        }
        /// <summary>
        /// 框架光线颜色
        /// </summary>
        public Color FrameLightColour => LightColours[FrameIndex];
        /// <summary>
        /// 当前坐标
        /// </summary>
        public Point CurrentLocation => Target?.CurrentLocation ?? MapTarget;
        /// <summary>
        /// 移动偏移
        /// </summary>
        public Point MovingOffSet => Target?.MovingOffSet ?? Point.Empty;
        /// <summary>
        /// 完成状态
        /// </summary>
        public Action CompleteAction;
        /// <summary>
        /// 框架作用
        /// </summary>
        public Action FrameAction;
        /// <summary>
        /// 框架索引
        /// </summary>
        public Action FrameIndexAction;
        /// <summary>
        /// 坐标附加偏移值
        /// </summary>
        public Point AdditionalOffSet;

        #region 粒子相关

        #region 粒子方向列表
        public Point[] ptArr1 = new Point[16]
        {
            new Point(40, 34),
            new Point(42, 35),
            new Point(49, 33),
            new Point(59, 31),
            new Point(65, 31),
            new Point(62, 33),
            new Point(54, 36),
            new Point(41, 44),
            new Point(47, 48),
            new Point(47, 45),
            new Point(43, 37),
            new Point(41, 38),
            new Point(40, 36),
            new Point(42, 37),
            new Point(43, 35),
            new Point(46, 34)
        };

        public Point[] ptArr2 = new Point[16]
        {
            new Point(34, 37),
            new Point(47, 38),
            new Point(62, 48),
            new Point(73, 41),
            new Point(76, 34),
            new Point(72, 43),
            new Point(58, 53),
            new Point(50, 48),
            new Point(34, 49),
            new Point(45, 49),
            new Point(55, 49),
            new Point(56, 43),
            new Point(48, 35),
            new Point(46, 40),
            new Point(46, 47),
            new Point(47, 38)
        };

        public Point[] ptArr3 = new Point[16]
        {
            new Point(6, 4),
            new Point(26, 0),
            new Point(44, 2),
            new Point(58, 6),
            new Point(56, 6),
            new Point(52, 10),
            new Point(48, 30),
            new Point(36, 48),
            new Point(10, 49),
            new Point(6, 47),
            new Point(4, 30),
            new Point(2, 20),
            new Point(0, 0),
            new Point(2, 4),
            new Point(3, 0),
            new Point(4, -1)
        };

        public Point ptArr4 = new Point(6, 4);

        public Point ptArr5 = new Point(40, 40);

        public Point[] ptArr6 = new Point[16]
        {
            new Point(14, 4),
            new Point(28, 0),
            new Point(44, 4),
            new Point(58, 6),
            new Point(56, 12),
            new Point(52, 22),
            new Point(48, 32),
            new Point(36, 40),
            new Point(18, 49),
            new Point(2, 47),
            new Point(1, 30),
            new Point(2, 20),
            new Point(0, 14),
            new Point(2, 8),
            new Point(3, 6),
            new Point(1, -1)
        };

        public Point[] ptArr7 = new Point[16]
        {
            new Point(24, 8),
            new Point(35, 15),
            new Point(65, 0),
            new Point(60, 13),
            new Point(60, 20),
            new Point(55, 40),
            new Point(53, 42),
            new Point(30, 52),
            new Point(27, 65),
            new Point(16, 65),
            new Point(20, 50),
            new Point(26, 28),
            new Point(25, 24),
            new Point(30, 16),
            new Point(15, 6),
            new Point(25, 10)
        };

        public Point[] ptArr8 = new Point[8]
        {
            new Point(8, 8),
            new Point(1, 1),
            new Point(1, 1),
            new Point(1, 1),
            new Point(1, 1),
            new Point(2, 3),
            new Point(1, 1),
            new Point(1, 1)
        };
        #endregion

        public MagicType CParticleType
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// 总持续时间
        /// </summary>
        public TimeSpan TotalDuration
        {
            get
            {
                TimeSpan temp = TimeSpan.Zero;

                foreach (TimeSpan delay in Delays)
                    temp += delay;

                return temp;
            }
        }
        /// <summary>
        /// 特效
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="frameCount">方向帧数</param>
        /// <param name="frameDelay">帧播放时间</param>
        /// <param name="file">素材位置</param>
        /// <param name="startLight">光效开</param>
        /// <param name="endLight">光效结束</param>
        /// <param name="lightColour">光效颜色</param>
        public MirEffect(int startIndex, int frameCount, TimeSpan frameDelay, LibraryFile file, int startLight, int endLight, Color lightColour)
        {
            StartIndex = startIndex;
            FrameCount = frameCount;
            Skip = 10;

            StartTime = CEnvir.Now;
            StartLight = startLight;
            EndLight = endLight;

            Delays = new TimeSpan[FrameCount];
            LightColours = new Color[FrameCount];
            for (int i = 0; i < frameCount; i++)
            {
                Delays[i] = frameDelay;
                //Light[i] = startLight + (endLight - startLight)/frameCount*i;
                LightColours[i] = lightColour;
            }

            CEnvir.LibraryList.TryGetValue(file, out Library);

            GameScene.Game.MapControl.Effects.Add(this);
        }

        #region 粒子相关

        public virtual void SetParticle()
        {
            if (Config.EnableParticle)
            {
                switch (CParticleType)
                {
                    case MagicType.DragonTornado:
                        if (FrameIndex < FrameCount - 5)
                        {
                            GameScene.Game.MapControl.m_xSmoke.SetSmokeParticleEx6(new Vector3(
                            (float)(DrawX + 24) + (float)GameScene.Game.MapControl.m_xSmoke.GetRandomNum(-10, 10),
                            (float)(DrawY + 16) + (float)GameScene.Game.MapControl.m_xSmoke.GetRandomNum(0, 20), 0f));
                            GameScene.Game.MapControl.m_xBoom.SetBoomParticle2(new Vector3(DrawX + 24, DrawY + 16, 0f));
                        }
                        break;
                    case MagicType.Cyclone:
                    case MagicType.BlowEarth:
                        GameScene.Game.MapControl.m_xSmoke.SetSmokeParticleEx10(new Vector3(
                            (float)(DrawX + 24) + (float)GameScene.Game.MapControl.m_xSmoke.GetRandomNum(-10, 10),
                            (float)(DrawY + 16) + (float)GameScene.Game.MapControl.m_xSmoke.GetRandomNum(0, 20), 0f));
                        GameScene.Game.MapControl.m_xBoom.SetBoomParticle5(new Vector3(DrawX + 24, DrawY + 16, 0f));
                        break;
                    case MagicType.FrozenEarth:
                    case MagicType.GreaterFrozenEarth:
                        if (FrameIndex < FrameCount - 15)
                        {
                            GameScene.Game.MapControl.m_xSmoke.SetSmokeParticleEx4(new Vector3(DrawX + 24, DrawY, 0f));
                            GameScene.Game.MapControl.m_xBoom.SetBoomParticle(new Vector3(DrawX + 24, DrawY + 16, 0f));
                            //System.Diagnostics.Debug.WriteLine("X = " + DrawX + ", Y = " + DrawY);
                        }
                        break;
                }
            }
        }

        #endregion

        /// <summary>
        /// 过程
        /// </summary>
        public virtual void Process()
        {
            if (CEnvir.Now < StartTime) return;

            if (Target != null)
            {
                DrawX = Target.DrawX + AdditionalOffSet.X;
                DrawY = Target.DrawY + AdditionalOffSet.Y;
            }
            else
            {
                DrawX = (MapTarget.X - MapObject.User.CurrentLocation.X + MapObject.OffSetX) * MapObject.CellWidth - MapObject.User.MovingOffSet.X + AdditionalOffSet.X;
                DrawY = (MapTarget.Y - MapObject.User.CurrentLocation.Y + MapObject.OffSetY) * MapObject.CellHeight - MapObject.User.MovingOffSet.Y + AdditionalOffSet.Y;
            }

            int frame = GetFrame();


            if (frame == FrameCount)
            {
                CompleteAction?.Invoke();
                Remove();
                return;
            }
            if (Reversed)
                frame = FrameCount - frame - 1;

            FrameIndex = frame;
            DrawFrame = FrameIndex + StartIndex + (int)Direction * Skip;
        }

        /// <summary>
        /// 获取帧数
        /// </summary>
        /// <returns></returns>
        protected virtual int GetFrame()
        {
            TimeSpan enlapsed = CEnvir.Now - StartTime;

            if (Loop)
                enlapsed = TimeSpan.FromTicks(enlapsed.Ticks % TotalDuration.Ticks);

            if (Reversed)
            {
                for (int i = 0; i < Delays.Length; i++)
                {
                    enlapsed -= Delays[Delays.Length - 1 - i];
                    if (enlapsed >= TimeSpan.Zero) continue;

                    return i;
                }
            }
            else
            {
                for (int i = 0; i < Delays.Length; i++)
                {
                    enlapsed -= Delays[i];
                    if (enlapsed >= TimeSpan.Zero) continue;

                    return i;
                }
            }

            return FrameCount;
        }

        /// <summary>
        /// 绘制
        /// </summary>
        public void Draw()
        {
            if (CEnvir.Now < StartTime || Library == null) return;

            #region 粒子相关
            if (CEnvir.Now >= GameScene.Game.MapControl.ParticleRenderTime)
            {
                SetParticle();
            }
            #endregion

            if (Blend)
                Library.DrawBlend(DrawFrame, DrawX, DrawY, DrawColour, UseOffSet, BlendRate, ImageType.Image, blendType: BlendType);
            else
                Library.Draw(DrawFrame, DrawX, DrawY, DrawColour, UseOffSet, Opacity, ImageType.Image);
        }
        /// <summary>
        /// 移除
        /// </summary>
        public void Remove()
        {
            CompleteAction = null;
            FrameAction = null;
            FrameIndexAction = null;
            GameScene.Game.MapControl.Effects.Remove(this);
            Target?.Effects.Remove(this);
        }
    }
    /// <summary>
    /// 绘图类型
    /// </summary>
    public enum DrawType
    {
        /// <summary>
        /// 底层
        /// </summary>
        Floor,
        /// <summary>
        /// 对象
        /// </summary>
        Object,
        /// <summary>
        /// 最后
        /// </summary>
        Final,
    }
}
