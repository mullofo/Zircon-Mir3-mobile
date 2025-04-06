using Client.Envir;
using Client.Scenes;
using Library;
using System;
using System.Collections.Generic;
using Color = System.Drawing.Color;
using Frame = Library.Frame;
using Point = System.Drawing.Point;
using S = Library.Network.ServerPackets;

namespace Client.Models
{
    /// <summary>
    /// 物体施法对象
    /// </summary>
    public sealed class SpellObject : MapObject
    {
        /// <summary>
        /// 对象类型 物体
        /// </summary>
        public override ObjectType Race => ObjectType.Spell;

        public override bool Blocking => false;
        /// <summary>
        /// 施法特效
        /// </summary>
        public SpellEffect Effect;
        /// <summary>
        /// 主体库
        /// </summary>
        public MirLibrary BodyLibrary;
        /// <summary>
        /// 混合
        /// </summary>
        public bool Blended;
        /// <summary>
        /// 混合速率
        /// </summary>
        public float BlendRate = 1f;
        /// <summary>
        /// 影响力 伤害
        /// </summary>
        public int Power;

        /// <summary>
        /// 物体施法对象信息
        /// </summary>
        /// <param name="info"></param>
        public SpellObject(S.ObjectSpell info)
        {
            ObjectID = info.ObjectID;
            Direction = info.Direction;

            CurrentLocation = info.Location;

            Effect = info.Effect;
            Power = info.Power;

            UpdateLibraries();

            SetFrame(new ObjectAction(MirAction.Standing, Direction, CurrentLocation));
            switch (Effect)
            {
                case SpellEffect.FireWall:
                case SpellEffect.MonsterFireWall:
                case SpellEffect.MonsterDeathCloud:
                    FrameStart -= TimeSpan.FromMilliseconds(CEnvir.Random.Next(300));
                    break;
                case SpellEffect.Tempest:
                    FrameStart -= TimeSpan.FromMilliseconds(CEnvir.Random.Next(1350));
                    break;
                case SpellEffect.IceRain:
                    FrameStart -= TimeSpan.FromMilliseconds(CEnvir.Random.Next(300));
                    Power = CEnvir.Random.Next(20);
                    break;
            }

            GameScene.Game.MapControl.AddObject(this);
        }
        /// <summary>
        /// 更新库
        /// </summary>
        public void UpdateLibraries()
        {
            Frames = new Dictionary<MirAnimation, Frame>();
            switch (Effect)
            {
                case SpellEffect.SafeZone:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.Magic, out BodyLibrary);
                    Frames[MirAnimation.Standing] = new Frame(649, 1, 0, TimeSpan.FromDays(365));
                    Blended = true;
                    BlendRate = 1f;
                    break;
                case SpellEffect.FireWall:
                case SpellEffect.MonsterFireWall:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.Magic, out BodyLibrary);
                    Frames[MirAnimation.Standing] = new Frame(920, 3, 0, TimeSpan.FromMilliseconds(150));
                    Blended = true;
                    LightColour = Color.LightSalmon;
                    BlendRate = 0.55f;
                    Light = 15;
                    break;
                case SpellEffect.IceRain:
                    BodyLibrary = null;
                    Frames[MirAnimation.Standing] = new Frame(0, 20, 0, TimeSpan.FromMilliseconds(250));
                    Blended = true;
                    LightColour = Globals.WindColour;
                    BlendRate = 0.55f;
                    Light = 15;
                    break;
                case SpellEffect.Tempest:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.MagicEx2, out BodyLibrary);
                    Frames[MirAnimation.Standing] = new Frame(920, 10, 0, TimeSpan.FromMilliseconds(150));
                    Blended = true;
                    LightColour = Globals.WindColour;
                    BlendRate = 0.8f;
                    Light = 15;
                    break;
                case SpellEffect.TrapOctagon:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.Magic, out BodyLibrary);
                    Frames[MirAnimation.Standing] = new Frame(640, 10, 0, TimeSpan.FromMilliseconds(100));
                    Blended = true;
                    BlendRate = 0.8f;
                    break;
                case SpellEffect.PoisonousCloud:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.MagicEx4, out BodyLibrary);
                    Frames[MirAnimation.Standing] = new Frame(400, 15, 0, TimeSpan.FromMilliseconds(100));
                    DefaultColour = Color.SaddleBrown;
                    Blended = true;
                    Light = 0;
                    DXSoundManager.Play(SoundIndex.PoisonousCloudStart);
                    break;
                case SpellEffect.DarkSoulPrison:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.MagicEx6, out BodyLibrary);
                    Frames[MirAnimation.Standing] = new Frame(700, 10, 0, TimeSpan.FromMilliseconds(100));
                    Blended = true;
                    Light = 0;
                    DXSoundManager.Play(SoundIndex.DarkSoulPrison);
                    break;
                case SpellEffect.SwordOfVengeance:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.MagicEx6, out BodyLibrary);
                    Frames[MirAnimation.Standing] = new Frame(1000, 8, 0, TimeSpan.FromMilliseconds(100));
                    Frames[MirAnimation.Die] = new Frame(1100, 10, 0, TimeSpan.FromMilliseconds(100));
                    Blended = true;
                    Light = 0;
                    break;
                case SpellEffect.MonsterDeathCloud:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.MonMagicEx2, out BodyLibrary);
                    Frames[MirAnimation.Standing] = new Frame(850, 10, 0, TimeSpan.FromMilliseconds(100));
                    Blended = true;
                    Light = 0;
                    BlendRate = 1F;
                    DXSoundManager.Play(SoundIndex.JinchonDevilAttack3);
                    break;
                case SpellEffect.Rubble:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out BodyLibrary);
                    int index;

                    if (Power > 20)
                        index = 234;
                    else if (Power > 15)
                        index = 233;
                    else if (Power > 10)
                        index = 232;
                    else if (Power > 5)
                        index = 231;
                    else
                        index = 230;

                    Frames[MirAnimation.Standing] = new Frame(index, 1, 0, TimeSpan.FromMilliseconds(100));

                    Light = 0;
                    break;
                case SpellEffect.WarWeaponShell:
                    //CEnvir.LibraryList.TryGetValue(LibraryFile.MonMagicEx3, out BodyLibrary);
                    BodyLibrary = null;

                    Point p = Functions.Move(CurrentLocation, Direction, 10);
                    var eff = new MirProjectile(0, 4, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx3, 0, 0, Globals.NoneColour, p)
                    {
                        //MapTarget = Functions.Move(CurrentLocation, Direction, 5),
                        MapTarget = CurrentLocation,
                        Has16Directions = false,
                        //Skip = 10,
                        Explode = true,
                        // Blend = true,
                        UseOffSet = false,
                        CParticleType = MagicType.WarWeaponShell,
                    };

                    eff.CompleteAction = () =>
                    {
                        new MirEffect(80, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx3, 0, 0, Globals.NoneColour)
                        {
                            MapTarget = CurrentLocation,
                            Blend = true,
                            BlendType = BlendType.HIGHLIGHT,
                            BlendRate = 0.5f,
                        };
                    };
                    break;
            }
        }
        /// <summary>
        /// 过程 冰雨技能
        /// </summary>
        public override void Process()
        {
            base.Process();
            if (FrameIndex == Power)
            {
                if (Effect == SpellEffect.IceRain && (CEnvir.Random.Next(16) == 0))
                {
                    MirProjectile eff;
                    Point p = new Point(CurrentLocation.X, CurrentLocation.Y - 10);
                    Effects.Add(eff = new MirProjectile(700, 7, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx7, 50, 80, Globals.FireColour, p)
                    {
                        MapTarget = CurrentLocation,
                        Skip = 0,
                        Explode = true,
                        Blend = true,
                    });

                    eff.CompleteAction = () =>
                    {
                        Effects.Add(new MirEffect(720, 7, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx7, 100, 100, Globals.NoneColour)
                        {
                            MapTarget = eff.MapTarget,
                            Blend = true,
                        });
                    };
                }
            }
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

            if (Blended)
                BodyLibrary.DrawBlend(DrawFrame, DrawX, DrawY, DrawColour, true, BlendRate, ImageType.Image);
            else
                BodyLibrary.Draw(DrawFrame, DrawX, DrawY, DrawColour, true, 1F, ImageType.Image);
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
        /// <summary>
        /// 在删除时
        /// </summary>
        public override void OnRemoved()
        {
            switch (Effect)
            {
                case SpellEffect.SwordOfVengeance:
                    new MirEffect(1100, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx6, 10, 35, Globals.FireColour)
                    {
                        Blend = true,
                        MapTarget = CurrentLocation,
                    };
                    DXSoundManager.Play(SoundIndex.FireStormEnd);
                    break;
                case SpellEffect.FireWall:
                case SpellEffect.MonsterFireWall:
                    new MirEffect(220, 1, TimeSpan.FromMilliseconds(2500), LibraryFile.ProgUse, 0, 0, Globals.NoneColour)
                    {
                        MapTarget = CurrentLocation,
                        Opacity = 0.8F,
                        DrawType = DrawType.Floor,
                    };

                    new MirEffect(2450 + CEnvir.Random.Next(5) * 10, 10, TimeSpan.FromMilliseconds(250), LibraryFile.Magic, 0, 0, Globals.NoneColour)
                    {
                        Blend = true,
                        MapTarget = CurrentLocation,
                        DrawType = DrawType.Floor,
                    };
                    break;
            }
        }
    }
}
