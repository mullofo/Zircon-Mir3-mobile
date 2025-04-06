using Client.Envir;
using Client.Scenes;
using Library;
using Microsoft.Xna.Framework;
using System;
using System.Drawing;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;

namespace Client.Models
{
    /// <summary>
    /// 投射效果
    /// </summary>
    public class MirProjectile : MirEffect
    {
        /// <summary>
        /// 坐标初始值
        /// </summary>
        public Point Origin { get; set; }
        /// <summary>
        /// 速度
        /// </summary>
        public int Speed { get; set; }
        /// <summary>
        /// 爆发
        /// </summary>
        public bool Explode { get; set; }
        /// <summary>
        /// 定向16个方向
        /// </summary>
        public int Direction16 { get; set; }
        /// <summary>
        /// 有16个方向
        /// </summary>
        public bool Has16Directions { get; set; }
        /// <summary>
        /// 抛射效果
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="frameCount"></param>
        /// <param name="frameDelay"></param>
        /// <param name="file"></param>
        /// <param name="startlight"></param>
        /// <param name="endlight"></param>
        /// <param name="lightColour"></param>
        /// <param name="origin"></param>
        public MirProjectile(int startIndex, int frameCount, TimeSpan frameDelay, LibraryFile file, int startlight, int endlight, Color lightColour, Point origin) : base(startIndex, frameCount, frameDelay, file, startlight, endlight, lightColour)
        {
            Has16Directions = true;

            Origin = origin;
            Speed = 50;
            Explode = false;
        }
        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            Point location = Target?.CurrentLocation ?? MapTarget;

            if (location == Origin)
            {
                CompleteAction?.Invoke();
                Remove();
                return;
            }

            int x = (Origin.X - MapObject.User.CurrentLocation.X + MapObject.OffSetX) * MapObject.CellWidth - MapObject.User.MovingOffSet.X;
            int y = (Origin.Y - MapObject.User.CurrentLocation.Y + MapObject.OffSetY) * MapObject.CellHeight - MapObject.User.MovingOffSet.Y;

            int x1 = (location.X - MapObject.User.CurrentLocation.X + MapObject.OffSetX) * MapObject.CellWidth - MapObject.User.MovingOffSet.X;
            int y1 = (location.Y - MapObject.User.CurrentLocation.Y + MapObject.OffSetY) * MapObject.CellHeight - MapObject.User.MovingOffSet.Y;

            //Direction16 = Functions.Direction16(new Point(x, y / 32 * 48), new Point(x1, y1 / 32 * 48));
            Direction16 = Functions.CalcDirection16(x, y / 32 * 48, x1, y1 / 32 * 48);
            //施法过程动画显示速度
            long duration = Functions.Distance(new Point(x, y / 32 * 48), new Point(x1, y1 / 32 * 48)) * TimeSpan.TicksPerMillisecond * 48 / 32;

            if (!Has16Directions)
                Direction16 /= 2;

            if (duration == 0)
            {
                CompleteAction?.Invoke();
                Remove();
                return;
            }

            int x2 = x1 - x;
            int y2 = y1 - y;

            if (x2 == 0) x2 = 1;
            if (y2 == 0) y2 = 1;

            TimeSpan time = CEnvir.Now - StartTime;

            int frame = GetFrame();

            if (Reversed)
                frame = FrameCount - frame - 1;

            DrawFrame = frame + StartIndex + Direction16 * Skip;

            DrawX = x + (int)(time.Ticks / (duration / x2)) + AdditionalOffSet.X;
            DrawY = y + (int)(time.Ticks / (duration / y2)) + AdditionalOffSet.Y;

            if ((CEnvir.Now - StartTime).Ticks > duration)
            {
                if (Target == null && !Explode)
                {
                    Size s = Library.GetSize(FrameIndex);

                    if (DrawX + s.Width > 0 && DrawX < GameScene.Game.Size.Width &&
                        DrawY + s.Height > 0 && DrawY < GameScene.Game.Size.Height) return;
                }

                CompleteAction?.Invoke();
                Remove();
                return;
            }
        }

        #region 粒子相关

        public override void SetParticle()
        {
            if (Config.EnableParticle)
            {
                switch (CParticleType)
                {
                    case MagicType.FireBall:
                        GameScene.Game.MapControl.m_xFlyingTail.SetFlyTailParticle(new Vector3((float)(DrawX + Library.GetOffSet(DrawFrame).X + ptArr2[Direction16].X), (float)(DrawY + Library.GetOffSet(DrawFrame).Y + ptArr2[Direction16].Y), 0f));
                        GameScene.Game.MapControl.m_xSmoke.SetSmokeParticle(new Vector3((float)(DrawX + Library.GetOffSet(DrawFrame).X + ptArr2[Direction16].X), (float)(DrawY + Library.GetOffSet(DrawFrame).Y + ptArr2[Direction16].Y), 0f));
                        break;
                    case MagicType.AdamantineFireBall:
                    case MagicType.MeteorShower:
                        GameScene.Game.MapControl.m_xFlyingTail.SetFlyTailParticle(new Vector3((float)(DrawX + Library.GetOffSet(DrawFrame).X + ptArr1[Direction16].X), (float)(DrawY + Library.GetOffSet(DrawFrame).Y + ptArr1[Direction16].Y), 0f));
                        GameScene.Game.MapControl.m_xSmoke.SetSmokeParticle(new Vector3((float)(DrawX + Library.GetOffSet(DrawFrame).X + ptArr1[Direction16].X), (float)(DrawY + Library.GetOffSet(DrawFrame).Y + ptArr1[Direction16].Y), 0f));
                        break;
                    case MagicType.GustBlast:
                        GameScene.Game.MapControl.m_xFlyingTail.SetFlyTailParticleEx4(new Vector3((float)(DrawX + Library.GetOffSet(DrawFrame).X + ptArr7[Direction16].X), (float)(DrawY + Library.GetOffSet(DrawFrame).Y + ptArr7[Direction16].Y), 0f));
                        GameScene.Game.MapControl.m_xSmoke.SetSmokeParticleEx8(new Vector3((float)(DrawX + Library.GetOffSet(DrawFrame).X + ptArr7[Direction16].X), (float)(DrawY + Library.GetOffSet(DrawFrame).Y + ptArr7[Direction16].Y), 0f));
                        break;
                    case MagicType.IceBolt:
                        GameScene.Game.MapControl.m_xSmoke.SetSmokeParticleEx(new Vector3((float)(DrawX + Library.GetOffSet(DrawFrame).X + ptArr3[Direction16].X), (float)(DrawY + Library.GetOffSet(DrawFrame).Y + ptArr3[Direction16].Y), 0f));
                        break;
                    case MagicType.IceBlades:
                        GameScene.Game.MapControl.m_xSmoke.SetSmokeParticleEx2(new Vector3((float)(DrawX + Library.GetOffSet(DrawFrame).X + ptArr4.X), (float)(DrawY + Library.GetOffSet(DrawFrame).Y + ptArr4.Y), 0f));
                        GameScene.Game.MapControl.m_xFlyingTail.SetFlyTailParticleEx(new Vector3((float)(DrawX + Library.GetOffSet(DrawFrame).X + ptArr4.X), (float)(DrawY + Library.GetOffSet(DrawFrame).Y + ptArr4.Y), 0f));
                        break;
                    case MagicType.EvilSlayer:
                    case MagicType.GreaterEvilSlayer:
                        GameScene.Game.MapControl.m_xFlyingTail.SetFlyTailParticleEx2(new Vector3((float)(DrawX + Library.GetOffSet(DrawFrame).X + ptArr5.X), (float)(DrawY + Library.GetOffSet(DrawFrame).Y + ptArr5.Y), 0f));
                        break;
                    case MagicType.LightningBall:
                        GameScene.Game.MapControl.m_xSmoke.SetSmokeParticleEx3(new Vector3((float)(DrawX + Library.GetOffSet(DrawFrame).X + ptArr6[Direction16].X), (float)(DrawY + Library.GetOffSet(DrawFrame).Y + ptArr6[Direction16].Y), 0f));
                        break;
                    case MagicType.WarWeaponShell:
                        //GameScene.Game.MapControl.m_xSmoke.SetSmokeParticleEx12(new Vector3((float)(DrawX + Library.GetOffSet(DrawFrame).X + ptArr8[Direction16].X), (float)(DrawY + Library.GetOffSet(DrawFrame).Y + ptArr8[Direction16].Y), 0));
                        GameScene.Game.MapControl.m_xSmoke.SetSmokeParticleEx12(new Vector3((float)(DrawX + ptArr8[Direction16].X), (float)(DrawY + ptArr8[Direction16].Y), 0));
                        break;
                }
            }
        }

        #endregion

        /// <summary>
        /// 获取帧
        /// </summary>
        /// <returns></returns>
        protected override int GetFrame()
        {
            TimeSpan enlapsed = CEnvir.Now - StartTime;

            enlapsed = TimeSpan.FromTicks(enlapsed.Ticks % TotalDuration.Ticks);

            for (int i = 0; i < Delays.Length; i++)
            {
                enlapsed -= Delays[i];
                if (enlapsed >= TimeSpan.Zero) continue;

                return i;
            }
            return FrameCount;
        }
    }
}
