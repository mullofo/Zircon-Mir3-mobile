using Library;
using Library.SystemModels;
using Server.Envir;
using Server.Views.DirectX;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.DirectSound;
using SharpDX.Multimedia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using Color = System.Drawing.Color;
using Frame = Library.Frame;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace Server.Views
{
    public partial class DiyMonShowViewer : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public static DiyMonShowViewer CurrentDiyMonViewer;
        public DXManager Manager;
        public MonsterBodyObject Monster;
        public PlayerBodyObject Player;
        public DiyMagicEffectObject DiyMagicEffectObject;
        public EquipmentObject EquipObject;
        public MapObject CurObject;
        public Point ObjectPoint;
        public MapObject TargetObject = new MapObject();
        public Point TargetPoint;
        public DateTime AnimationTime;

        #region  怪物自定义  

        public Dictionary<MonsterInfo, Dictionary<MirAnimation, Library.Frame>> DiyMonActFrame = new Dictionary<MonsterInfo, Dictionary<MirAnimation, Library.Frame>>();
        public Dictionary<MonsterInfo, Dictionary<MirAnimation, DXSound>> DiyMonActSound = new Dictionary<MonsterInfo, Dictionary<MirAnimation, DXSound>>();
        public Dictionary<MonsterInfo, Dictionary<MirAnimation, MonAnimationEffect>> DiyMonActEffect = new Dictionary<MonsterInfo, Dictionary<MirAnimation, MonAnimationEffect>>();

        //初始化各种自定义信息
        public void InitDiyMonFrams()
        {
            MonAnimationFrame MonAnimationFrame = null;
            Dictionary<MirAnimation, Library.Frame> DefaultMonFrame = null;
            Dictionary<MirAnimation, DXSound> DefaultMonActSound = null;
            Dictionary<MirAnimation, MonAnimationEffect> DefaultMonActEffect = null;

            for (int i = 0; i < SMain.Session.GetCollection<MonAnimationFrame>().Binding.Count; i++)
            {
                MonAnimationFrame = SMain.Session.GetCollection<MonAnimationFrame>().Binding[i];
                if (MonAnimationFrame.Monster == null) continue;
                //动作
                if (!DiyMonActFrame.TryGetValue(MonAnimationFrame.Monster, out DefaultMonFrame))
                {
                    DefaultMonFrame = new Dictionary<MirAnimation, Frame>();
                    DefaultMonFrame[MonAnimationFrame.MonAnimation] = new Frame(MonAnimationFrame.startIndex, MonAnimationFrame.frameCount, MonAnimationFrame.offSet, TimeSpan.FromMilliseconds(MonAnimationFrame.frameDelay)) { Reversed = MonAnimationFrame.Reversed, StaticSpeed = MonAnimationFrame.StaticSpeed };
                    DiyMonActFrame.Add(MonAnimationFrame.Monster, DefaultMonFrame);
                }
                DefaultMonFrame[MonAnimationFrame.MonAnimation] = new Frame(MonAnimationFrame.startIndex, MonAnimationFrame.frameCount, MonAnimationFrame.offSet, TimeSpan.FromMilliseconds(MonAnimationFrame.frameDelay)) { Reversed = MonAnimationFrame.Reversed, StaticSpeed = MonAnimationFrame.StaticSpeed };

                //声音
                if (MonAnimationFrame.ActSound != SoundIndex.None || MonAnimationFrame.ActSoundStr != "")
                {
                    if (!DiyMonActSound.TryGetValue(MonAnimationFrame.Monster, out DefaultMonActSound))
                    {
                        DefaultMonActSound = new Dictionary<MirAnimation, DXSound>();
                        DiyMonActSound.Add(MonAnimationFrame.Monster, DefaultMonActSound);
                    }

                    if (MonAnimationFrame.ActSound != SoundIndex.None)
                    {
                        DefaultMonActSound[MonAnimationFrame.MonAnimation] = DXSoundManager.SoundList[MonAnimationFrame.ActSound];
                    }
                    else
                    {
                        DefaultMonActSound[MonAnimationFrame.MonAnimation] = DXSoundManager.AddDiySound(MonAnimationFrame.ActSoundStr);
                    }
                }
                //伴随魔法
                if (MonAnimationFrame.effectfile != LibraryFile.None)
                {
                    if (!DiyMonActEffect.TryGetValue(MonAnimationFrame.Monster, out DefaultMonActEffect))
                    {
                        DefaultMonActEffect = new Dictionary<MirAnimation, MonAnimationEffect>();
                        DiyMonActEffect.Add(MonAnimationFrame.Monster, DefaultMonActEffect);
                    }

                    DefaultMonActEffect[MonAnimationFrame.MonAnimation] = new MonAnimationEffect()
                    {
                        effectfile = MonAnimationFrame.effectfile,
                        effectfrom = MonAnimationFrame.effectfrom,
                        effectframe = MonAnimationFrame.effectframe,
                        effectdelay = MonAnimationFrame.effectdelay,
                        effectdir = MonAnimationFrame.effectdir,
                    };
                }
            }
        }

        #endregion

        #region DiyMagicEffect
        public DiyMagicEffect DiymagicEffect
        {
            get { return _DiymagicEffect; }
            set
            {
                if (_DiymagicEffect == value) return;

                DiyMagicEffect oldValue = _DiymagicEffect;
                _DiymagicEffect = value;

                OnDiyMagicInfoChanged(oldValue, value);
            }
        }
        private DiyMagicEffect _DiymagicEffect;

        public void OnDiyMagicInfoChanged(DiyMagicEffect oValue, DiyMagicEffect nValue)
        {
            ClearObject();
            if (DiymagicEffect == null)
            {
                return;
            }
            switch (DiymagicEffect.MagicType)
            {
                case DiyMagicType.Point:
                case DiyMagicType.Line:
                case DiyMagicType.FlyDestinationExplosion:
                case DiyMagicType.FiyHitTargetExplosion:
                case DiyMagicType.ExplosionMagic:
                case DiyMagicType.BodyEffect:
                case DiyMagicType.MagicAttackPlayer:
                case DiyMagicType.ActMagic:
                case DiyMagicType.MonAttackMon:
                    DiyMagicEffectObject = new DiyMagicEffectObject(DiymagicEffect);
                    DiyMagicEffectObject.DrawSize = DXPanel.ClientSize;
                    CurObject = DiyMagicEffectObject;
                    break;
                case DiyMagicType.DressLightInner_Man_Down:
                case DiyMagicType.DressLightInner_Man:

                    ItemInfo DressIteminfo = SMain.Session.GetCollection<ItemInfo>().Binding.FirstOrDefault(x => x.ItemType == ItemType.Armour && x.Image == DiymagicEffect.MagicID);
                    if (DressIteminfo != null)
                    {
                        EquipObject = new EquipmentObject(DressIteminfo);
                        EquipObject.DrawSize = DXPanel.ClientSize;
                        EquipObject.HumGender = MirGender.Male;

                        CurObject = EquipObject;
                    }
                    break;
                case DiyMagicType.DressLightInner_WoMan_Down:
                case DiyMagicType.DressLightInner_WoMan:
                    DressIteminfo = SMain.Session.GetCollection<ItemInfo>().Binding.FirstOrDefault(x => x.ItemType == ItemType.Armour && x.Image == DiymagicEffect.MagicID);
                    if (DressIteminfo != null)
                    {
                        EquipObject = new EquipmentObject(DressIteminfo);
                        EquipObject.DrawSize = DXPanel.ClientSize;
                        EquipObject.HumGender = MirGender.Female;

                        CurObject = EquipObject;
                    }
                    break;
                case DiyMagicType.DressLightOut_Man:
                    DressIteminfo = SMain.Session.GetCollection<ItemInfo>().Binding.FirstOrDefault(x => x.ItemType == ItemType.Armour && x.Shape == DiymagicEffect.MagicID);
                    if (DressIteminfo != null)
                    {
                        Player = new PlayerBodyObject(DressIteminfo);
                        Player.DrawSize = DXPanel.ClientSize;
                        Player.HumGender = MirGender.Male;

                        CurObject = Player;
                    }
                    break;
                case DiyMagicType.DressLightOut_WoMan:
                    DressIteminfo = SMain.Session.GetCollection<ItemInfo>().Binding.FirstOrDefault(x => x.ItemType == ItemType.Armour && x.Shape == DiymagicEffect.MagicID);
                    if (DressIteminfo != null)
                    {
                        Player = new PlayerBodyObject(DressIteminfo);
                        Player.DrawSize = DXPanel.ClientSize;
                        Player.HumGender = MirGender.Female;

                        CurObject = Player;
                    }
                    break;
                case DiyMagicType.WeaponLightInner:
                    DressIteminfo = SMain.Session.GetCollection<ItemInfo>().Binding.FirstOrDefault(x => x.ItemType == ItemType.Weapon && x.Shape == DiymagicEffect.MagicID);
                    if (DressIteminfo != null)
                    {
                        EquipObject = new EquipmentObject(DressIteminfo);
                        EquipObject.DrawSize = DXPanel.ClientSize;
                        EquipObject.HumGender = MirGender.Male;

                        CurObject = EquipObject;
                    }
                    break;
                case DiyMagicType.WeaponLightOut_Man:
                    DressIteminfo = SMain.Session.GetCollection<ItemInfo>().Binding.FirstOrDefault(x => x.ItemType == ItemType.Weapon && x.Shape == DiymagicEffect.MagicID);
                    if (DressIteminfo != null)
                    {
                        Player = new PlayerBodyObject(DressIteminfo);
                        Player.DrawSize = DXPanel.ClientSize;
                        Player.HumGender = MirGender.Male;

                        CurObject = Player;
                        CurObject.SetAnimation(MirAnimation.Standing);
                    }
                    break;
                case DiyMagicType.WeaponLightOut_WoMan:
                    DressIteminfo = SMain.Session.GetCollection<ItemInfo>().Binding.FirstOrDefault(x => x.ItemType == ItemType.Weapon && x.Shape == DiymagicEffect.MagicID);
                    if (DressIteminfo != null)
                    {
                        Player = new PlayerBodyObject(DressIteminfo);
                        Player.DrawSize = DXPanel.ClientSize;
                        Player.HumGender = MirGender.Female;

                        CurObject = Player;
                        CurObject.SetAnimation(MirAnimation.Standing);
                    }
                    break;
                case DiyMagicType.MonCombat1:
                case DiyMagicType.MonCombat2:
                case DiyMagicType.MonCombat3:
                case DiyMagicType.MonCombat4:
                case DiyMagicType.MonCombat5:
                case DiyMagicType.MonCombat6:
                case DiyMagicType.MonOnSpawned:
                case DiyMagicType.MonDie:
                    MonsterInfo CurmonsterInfo = SMain.Session.GetCollection<MonsterInfo>().Binding.FirstOrDefault(x => x.AI == DiymagicEffect.MagicID);
                    if (CurmonsterInfo != null)
                    {
                        Monster = new MonsterBodyObject(CurmonsterInfo);
                        Monster.DrawSize = DXPanel.ClientSize;
                        CurObject = Monster;
                        if (DiymagicEffect.MagicType == DiyMagicType.MonCombat1)
                        {
                            CurObject.SetAnimation(MirAnimation.Combat1);
                        }
                        else if (DiymagicEffect.MagicType == DiyMagicType.MonCombat2)
                        {
                            CurObject.SetAnimation(MirAnimation.Combat2);
                        }
                        else if (DiymagicEffect.MagicType == DiyMagicType.MonCombat3)
                        {
                            CurObject.SetAnimation(MirAnimation.Combat3);
                        }

                        else if (DiymagicEffect.MagicType == DiyMagicType.MonCombat4)
                        {
                            CurObject.SetAnimation(MirAnimation.Combat4);
                        }

                        else if (DiymagicEffect.MagicType == DiyMagicType.MonCombat5)
                        {
                            CurObject.SetAnimation(MirAnimation.Combat5);
                        }

                        else if (DiymagicEffect.MagicType == DiyMagicType.MonCombat6)
                        {
                            CurObject.SetAnimation(MirAnimation.Combat6);
                        }
                        else if (DiymagicEffect.MagicType == DiyMagicType.MonOnSpawned)
                        {
                            CurObject.SetAnimation(MirAnimation.Show);
                        }
                        else if (DiymagicEffect.MagicType == DiyMagicType.MonDie)
                        {
                            CurObject.SetAnimation(MirAnimation.Die);
                        }
                    }
                    break;
                default:
                    break;
            }

            MonActEditItem.Enabled = false;
        }

        #endregion

        #region MonsterAIInfo

        public MonDiyAiAction monsterAIInfo
        {
            get { return _monsterAIInfo; }
            set
            {
                if (_monsterAIInfo == value) return;

                MonDiyAiAction oldValue = _monsterAIInfo;
                _monsterAIInfo = value;

                OnMonsterAIInfoChanged(oldValue, value);
            }
        }
        private MonDiyAiAction _monsterAIInfo;

        public void OnMonsterAIInfoChanged(MonDiyAiAction oValue, MonDiyAiAction nValue)
        {
            ClearObject();
            if (monsterAIInfo == null)
            {
                return;
            }

            Monster = new MonsterBodyObject(monsterAIInfo.Monster);

            Monster.DrawSize = DXPanel.ClientSize;
            Monster.SetActSpellMagic(monsterAIInfo.ActionID, monsterAIInfo.MagicID);

            CurObject = Monster;
            CurObject.SetAnimation(monsterAIInfo.ActionID);

            TargetObject.CurrentLocation = TargetPoint;

            MonActEditItem.Enabled = false;
        }

        #endregion

        #region MonsterInfo

        public MonsterInfo monsterInfo
        {
            get { return _monsterInfo; }
            set
            {
                if (_monsterInfo == value) return;

                MonsterInfo oldValue = _monsterInfo;
                _monsterInfo = value;

                OnMonsterInfoChanged(oldValue, value);
            }
        }
        private MonsterInfo _monsterInfo;

        public void OnMonsterInfoChanged(MonsterInfo oValue, MonsterInfo nValue)
        {
            ClearObject();
            if (monsterInfo == null)
            {
                return;
            }

            Monster = new MonsterBodyObject(monsterInfo);

            Monster.DrawSize = DXPanel.ClientSize;

            CurObject = Monster;
            CurObject.SetAnimation(MirAnimation.Standing);

            MonActEditItem.Enabled = true;
        }

        #endregion

        #region ItemInfo

        public ItemInfo Iteminfo
        {
            get { return _Iteminfo; }
            set
            {
                if (_Iteminfo == value) return;

                ItemInfo oldValue = _Iteminfo;
                _Iteminfo = value;

                OnItemInfoChanged(oldValue, value);
            }
        }
        private ItemInfo _Iteminfo;

        public void OnItemInfoChanged(ItemInfo oValue, ItemInfo nValue)
        {
            ClearObject();
            if (Iteminfo == null)
            {
                return;
            }

            EquipObject = new EquipmentObject(Iteminfo);
            EquipObject.DrawSize = DXPanel.ClientSize;
            CurObject = EquipObject;

            Player = new PlayerBodyObject(Iteminfo);
            Player.DrawSize = DXPanel.ClientSize;
            TargetObject = Player;
            TargetObject.CurrentLocation = TargetPoint;
            TargetObject.SetAnimation(MirAnimation.Standing);
            MonActEditItem.Enabled = false;
        }

        #endregion

        public void ClearObject()
        {
            DiyMagicEffectObject = null;
            Monster = null;
            Player = null;
            EquipObject = null;
        }
        public DiyMonShowViewer()
        {
            InitializeComponent();

            CurrentDiyMonViewer = this;
            InitDiyMonFrams();
            MonActComboBox.Items.AddEnum<MirAnimation>();
            MonActEditItem.EditValue = MirAnimation.Standing;

            ObjectPoint = new Point(DXPanel.ClientSize.Width / 4, DXPanel.ClientSize.Height / 3 * 2);
            TargetPoint = new Point(DXPanel.ClientSize.Width / 4 * 3, DXPanel.ClientSize.Height / 3 * 2);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (CurrentDiyMonViewer == this)
                CurrentDiyMonViewer = null;

            Manager.Dispose();
            Manager = null;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Manager = new DXManager(DXPanel);
            Manager.Create();
            DXSoundManager.Create();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (Manager == null) return;
            if (CurObject == null) return;

            Manager.ResetDevice();
            ObjectPoint = new Point(DXPanel.ClientSize.Width / 4, DXPanel.ClientSize.Height / 3 * 2);
            TargetPoint = new Point(DXPanel.ClientSize.Width / 4 * 3, DXPanel.ClientSize.Height / 3 * 2);

            CurObject.DrawSize = DXPanel.ClientSize;
            TargetObject.DrawSize = DXPanel.ClientSize;
        }

        public void Process()
        {
            SEnvir.DiyShowViewerLoopTime = Time.Now;
            UpdateEnvironment();
            RenderEnvironment();
        }

        private void UpdateEnvironment()
        {
            if (SEnvir.DiyShowViewerLoopTime > AnimationTime)
            {
                AnimationTime = SEnvir.DiyShowViewerLoopTime.AddMilliseconds(150);
                if (TargetObject != null)
                {
                    TargetObject.DrawFrameAni++;
                    for (int i = TargetObject.Effects.Count - 1; i >= 0; i--)
                        TargetObject.Effects[i].Process();
                }
                if (CurObject != null)
                {
                    CurObject.DrawFrameAni++;
                    for (int i = CurObject.Effects.Count - 1; i >= 0; i--)
                        CurObject.Effects[i].Process();
                }
            }
        }
        private void RenderEnvironment()
        {
            try
            {
                if (Manager.DeviceLost)
                {
                    Manager.AttemptReset();
                    return;
                }
                if (CurObject == null) return;

                Manager.Device.Clear(ClearFlags.Target, Color.Black.ToRawColorBGRA(), 1, 0);
                Manager.Device.BeginScene();
                Manager.Sprite.Begin(SpriteFlags.AlphaBlend);
                Manager.SetSurface(Manager.MainSurface);
                CurObject.Draw();
                TargetObject.Draw();

                Manager.Sprite.End();
                Manager.Device.EndScene();
                Manager.Device.Present();

            }
            catch (SharpDXException)
            {
                Manager.DeviceLost = true;
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());

                Manager.AttemptRecovery();
            }
        }

        private void MonActEditItem_EditValueChanged(object sender, EventArgs e)
        {
            if (CurObject != null && MonActEditItem.EditValue != null)
            {
                CurObject.SetAnimation((MirAnimation)MonActEditItem.EditValue);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InitDiyMonFrams();
            CurObject.UpdateLibraries();
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (monsterAIInfo != null)
            {
                Monster.SetAnimation(monsterAIInfo.ActionID, false);
            }
            if (DiymagicEffect != null)
            {
                DiyMagicEffectObject?.SetBegin();
            }
            if (monsterInfo != null)
            {
                Monster.LoopShow = false;
            }
        }
    }

    public sealed class DiyMagicEffectObject : MapObject
    {
        public bool LoopShow = true;
        public int DrawTime = 1;

        public DiyMagicEffectObject(DiyMagicEffect info)
        {
            DiyMagicEffectInfo = info;
        }

        public override void Draw()
        {
            foreach (MirEffect ob in Effects)
            {
                ob.Draw();
            }
        }
        public void SetBegin()
        {
            MagicLocations.Clear();

            MagicLocations.Add(DiyMonShowViewer.CurrentDiyMonViewer.ObjectPoint);

            AttackTargets.Clear();

            AttackTargets.Add(DiyMonShowViewer.CurrentDiyMonViewer.CurObject);

            switch (DiyMagicEffectInfo.MagicType)
            {
                case DiyMagicType.Point:
                    break;
                case DiyMagicType.Line:
                    break;
                case DiyMagicType.FlyDestinationExplosion:
                    AttackTargets.Clear();
                    break;
                case DiyMagicType.FiyHitTargetExplosion:
                    MagicLocations.Clear();
                    break;
                case DiyMagicType.ExplosionMagic:
                    break;
                case DiyMagicType.ActMagic:
                    break;
                case DiyMagicType.BodyEffect:
                    break;
                case DiyMagicType.MagicAttackPlayer:
                    break;
                case DiyMagicType.MonOnSpawned:
                    break;
                case DiyMagicType.DressLightInner_Man_Down:
                    break;
                case DiyMagicType.DressLightInner_WoMan_Down:
                    break;
                case DiyMagicType.DressLightInner_Man:
                    break;
                case DiyMagicType.DressLightInner_WoMan:
                    break;
                case DiyMagicType.DressLightOut_Man:
                    break;
                case DiyMagicType.DressLightOut_WoMan:
                    break;
                case DiyMagicType.WeaponLightInner:
                    break;
                case DiyMagicType.WeaponLightOut_Man:
                    break;
                case DiyMagicType.WeaponLightOut_WoMan:
                    break;
                case DiyMagicType.MonAttackMon:
                    break;
                default:
                    break;
            }
            ShowDiyMagicEffect(DiyMagicEffectInfo, Element.None, AttackTargets, MagicLocations);
        }
    }
    public sealed class MonsterBodyObject : MapObject
    {
        public MonsterInfo MonsterInfo;

        public Dictionary<MirAnimation, Frame> DiyMonActFrame = null;
        public Dictionary<MirAnimation, DXSound> DiyMonActSound = new Dictionary<MirAnimation, DXSound>();
        public Dictionary<MirAnimation, MonAnimationEffect> DiyMonActionMagics = null;
        public Dictionary<MirAnimation, DiyMagicEffect> DiyMonSpellMagics = new Dictionary<MirAnimation, DiyMagicEffect>();

        public bool LoopShow = true;
        public int DrawTime = 1;

        public Color DrawColour = Color.White;
        public float Opacity = 1F;

        public int NoDirBodyMagicFrame = 0;
        public DateTime NoDirBodyMagicDrawTime = SEnvir.DiyShowViewerLoopTime;

        public SoundIndex AttackSound, StruckSound, DieSound;

        public int RenderY;

        public MonsterImage Image;

        public MonsterBodyObject(MonsterInfo info)
        {
            MonsterInfo = info;

            UpdateLibraries();
        }
        public override void UpdateLibraries()
        {
            BodyLibrary = null;

            Frames = new Dictionary<MirAnimation, Frame>(FrameSet.DefaultMonster);

            BodyOffSet = 1000;

            AttackSound = SoundIndex.None;
            StruckSound = SoundIndex.None;
            DieSound = SoundIndex.None;

            Image = MonsterInfo.Image;

            #region  怪物自定义  

            int CurShp = MonsterInfo.BodyShape;
            int Lib = CurShp / 10;
            DiyMonShowViewer.CurrentDiyMonViewer.Manager.LibraryList.TryGetValue((LibraryFile)((int)LibraryFile.Mon_1 + Lib), out BodyLibrary);
            BodyShape = CurShp % 10;

            if (DiyMonShowViewer.CurrentDiyMonViewer.DiyMonActFrame.TryGetValue(MonsterInfo, out DiyMonActFrame))
            {
                //读取配置文件并增加动画
                foreach (KeyValuePair<MirAnimation, Frame> frame in DiyMonActFrame)
                    Frames[frame.Key] = frame.Value;
            }
            //配置声音
            /*DiyMonActSound[MirAnimation.Combat1] = DXSoundManager.AddDiySound(MonsterInfo.BodyShape, MirAnimationSound.AttackSound);
            DiyMonActSound[MirAnimation.Struck] = DXSoundManager.AddDiySound(MonsterInfo.BodyShape, MirAnimationSound.StruckSound);
            DiyMonActSound[MirAnimation.Die] = DXSoundManager.AddDiySound(MonsterInfo.BodyShape, MirAnimationSound.DieSound);
            Dictionary<MirAnimation, DXSound> TDiyMonActSound = new Dictionary<MirAnimation, DXSound>();
            if (DiyMonShowViewer.CurrentDiyMonViewer.DiyMonActSound.TryGetValue(MonsterInfo, out TDiyMonActSound))//获取声音
            {
                foreach (KeyValuePair<MirAnimation, DXSound> Sound in TDiyMonActSound)
                    DiyMonActSound[Sound.Key] = Sound.Value;
            }*/

            DiyMonShowViewer.CurrentDiyMonViewer.DiyMonActEffect.TryGetValue(MonsterInfo, out DiyMonActionMagics);

            #endregion
        }

        public override void UpdateFrame()
        {
            if (Frames == null || CurrentFrame == null) return;

            FrameIndex = DrawFrameAni % CurrentFrame.FrameCount;
            if (!LoopShow && FrameIndex == 0)
            {
                DrawTime--;
                ShowSpellDiyMagic();
                FrameIndexChanged(FrameIndex);
            }
            if (DrawTime < 0)
            {
                LoopShow = true;
                DrawTime = 1;
            }
            DrawFrame = FrameIndex + CurrentFrame.StartIndex + CurrentFrame.OffSet * (int)CurDirection;
        }
        public override void SetAnimation(MirAnimation animation, bool Loop = true)
        {
            LoopShow = Loop;
            if (animation == MirAnimation.Standing)
                DiyMonSpellMagics.Clear();

            CurrentAnimation = animation;
            if (!Frames.TryGetValue(CurrentAnimation, out CurrentFrame))
                CurrentFrame = Frame.EmptyFrame;
        }
        public void SetActSpellMagic(MirAnimation mirAnimation, int SpellMagicID)
        {
            DiyMonSpellMagics.Clear();
            DiyMagicEffect MagicEffectinfo = SMain.Session.GetCollection<DiyMagicEffect>().Binding.FirstOrDefault(x => x.MagicID == SpellMagicID && x.MagicType <= DiyMagicType.ExplosionMagic);
            if (MagicEffectinfo != null)
            {
                DiyMonSpellMagics[mirAnimation] = MagicEffectinfo;
            }
        }
        public void ShowSpellDiyMagic()
        {
            if (DiyMonSpellMagics.Count > 0)
            {
                if (DiyMonSpellMagics[CurrentAnimation] != null)
                {
                    ShowDiyMagicEffect(DiyMonSpellMagics[CurrentAnimation], Element.None, AttackTargets, MagicLocations);
                }
            }

        }

        public override void Draw()
        {
            if (BodyLibrary == null) return;
            UpdateFrame();
            DrawShadow(CurrentLocation.X, CurrentLocation.Y);
            DrawBody(CurrentLocation.X, CurrentLocation.Y);
            foreach (MirEffect ob in Effects)
            {
                ob.Draw();
            }
        }
        public int multiple(int a, int b)
        {
            int p, q, temp;
            p = (a > b) ? a : b;              //求两个数中的最大值  
            q = (a > b) ? b : a;    //求两个数中的最小值	
            temp = p;    //最大值赋给p为变量自增作准备 	
            while (true)
            {    //利用循环语句来求满足条件的数值 
                if (p % q == 0) break;                   //只要找到变量的和数能被a或b所整除，则中止循环    	
                p += temp;    //如果条件不满足则变量自身相加  	
            }
            return p;
        }

        public void DrawShadow(int x, int y)
        {
            BodyLibrary.Draw(BodyFrame, x, y, Color.White, true, 0.5f, ImageType.Shadow);
        }
        public void DrawBody(int x, int y)
        {

            BodyLibrary.Draw(BodyFrame, x, y, DrawColour, true, Opacity, ImageType.Image);
            MirLibrary library;

            if (DiyMonActionMagics != null)
            {
                MonAnimationEffect monAnimationEffect;
                if (DiyMonActionMagics.TryGetValue(CurrentAnimation, out monAnimationEffect) && monAnimationEffect.effectfile != LibraryFile.None)
                {
                    if (DiyMonShowViewer.CurrentDiyMonViewer.Manager.LibraryList.TryGetValue(DiyMonActionMagics[CurrentAnimation].effectfile, out library))
                    {
                        if (DiyMonActionMagics[CurrentAnimation].effectframe > 0)
                        {
                            if (SEnvir.DiyShowViewerLoopTime > NoDirBodyMagicDrawTime)
                            {
                                NoDirBodyMagicDrawTime = SEnvir.DiyShowViewerLoopTime.AddMilliseconds(DiyMonActionMagics[CurrentAnimation].effectdelay);
                                NoDirBodyMagicFrame++;
                            }
                            library.DrawBlend(NoDirBodyMagicFrame % DiyMonActionMagics[CurrentAnimation].effectframe + DiyMonActionMagics[CurrentAnimation].effectfrom, x, y, Color.White, true, 1f, ImageType.Image);
                        }
                        else
                        {
                            library.DrawBlend(DrawFrame - CurrentFrame.StartIndex + DiyMonActionMagics[CurrentAnimation].effectfrom, x, y, Color.White, true, 1f, ImageType.Image);
                        }

                    }
                }
            }
        }
        public void FrameIndexChanged(int frameIndex)
        {
            switch (CurrentAnimation)
            {
                case MirAnimation.Standing:
                    break;
                case MirAnimation.Walking:
                    break;
                case MirAnimation.CreepStanding:
                    break;
                case MirAnimation.CreepWalkSlow:
                    break;
                case MirAnimation.CreepWalkFast:
                    break;
                case MirAnimation.Running:
                    break;
                case MirAnimation.Pushed:
                    break;
                case MirAnimation.Combat1:
                case MirAnimation.Combat2:
                case MirAnimation.Combat3:
                case MirAnimation.Combat4:
                case MirAnimation.Combat5:
                case MirAnimation.Combat6:
                case MirAnimation.Combat7:
                case MirAnimation.Combat8:
                case MirAnimation.Combat9:
                case MirAnimation.Combat10:
                case MirAnimation.Combat11:
                case MirAnimation.Combat12:
                case MirAnimation.Combat13:
                case MirAnimation.Combat14:
                case MirAnimation.Combat15:
                    if (frameIndex == 0)
                    {
                        DiyMagicEffect ActSpellMagic = SMain.Session.GetCollection<DiyMagicEffect>().Binding.FirstOrDefault(p => p.MagicType == Functions.GetDiyMagicTypeByAction(CurrentAnimation) && p.MagicID == MonsterInfo.BodyShape);
                        if (ActSpellMagic != null)
                        {
                            ShowDiyMagicEffect(ActSpellMagic, Element.None);
                        }
                        PlayAttackSound();
                    }
                    break;
                case MirAnimation.Harvest:
                    break;
                case MirAnimation.Stance:
                    break;
                case MirAnimation.Struck:
                    if (frameIndex == 0)
                    {
                        DiyMagicEffect ActSpellMagic = SMain.Session.GetCollection<DiyMagicEffect>().Binding.FirstOrDefault(p => p.MagicType == DiyMagicType.MonAttackMon && p.MagicID == MonsterInfo.BodyShape);
                        if (ActSpellMagic != null)
                        {
                            ShowDiyMagicEffect(ActSpellMagic, Element.None);
                        }
                        PlayStruckSound();
                    }
                    break;
                case MirAnimation.Die:
                    if (frameIndex == 0)
                    {
                        DiyMagicEffect ActSpellMagic = SMain.Session.GetCollection<DiyMagicEffect>().Binding.FirstOrDefault(p => p.MagicType == DiyMagicType.MonDie && p.MagicID == MonsterInfo.BodyShape);
                        if (ActSpellMagic != null)
                        {
                            ShowDiyMagicEffect(ActSpellMagic, Element.None);
                        }
                        PlayDieSound();
                    }
                    break;
                case MirAnimation.Dead:
                    break;
                case MirAnimation.Skeleton:
                    break;
                case MirAnimation.Show:
                    break;
                case MirAnimation.Hide:
                    break;
                case MirAnimation.HorseStanding:
                    break;
                case MirAnimation.HorseWalking:
                    break;
                case MirAnimation.HorseRunning:
                    break;
                case MirAnimation.HorseStruck:
                    break;
                case MirAnimation.StoneStanding:
                    break;
                case MirAnimation.DragonRepulseStart:
                    break;
                case MirAnimation.DragonRepulseMiddle:
                    break;
                case MirAnimation.DragonRepulseEnd:
                    break;
                case MirAnimation.ChannellingStart:
                    break;
                case MirAnimation.ChannellingMiddle:
                    break;
                case MirAnimation.ChannellingEnd:
                    break;
                default:
                    break;
            }
        }
        public void PlayDiyMonActSound(MirAnimationSound AnimationSound)
        {

            if (DiyMonActSound == null) return;

            if (MonsterInfo.Image == MonsterImage.DiyMonsMon)
            {
                DXSound Sound;
                switch (AnimationSound)
                {
                    case MirAnimationSound.AttackSound:
                        if (DiyMonActSound.TryGetValue(MirAnimation.Combat1, out Sound))
                        {
                            Sound.Play();
                        }
                        if (DiyMonActSound.TryGetValue(MirAnimation.Combat2, out Sound))
                        {
                            Sound.Play();
                        }
                        if (DiyMonActSound.TryGetValue(MirAnimation.Combat3, out Sound))
                        {
                            Sound.Play();
                        }
                        if (DiyMonActSound.TryGetValue(MirAnimation.Combat4, out Sound))
                        {
                            Sound.Play();
                        }
                        if (DiyMonActSound.TryGetValue(MirAnimation.Combat5, out Sound))
                        {
                            Sound.Play();
                        }
                        if (DiyMonActSound.TryGetValue(MirAnimation.Combat6, out Sound))
                        {
                            Sound.Play();
                        }
                        if (DiyMonActSound.TryGetValue(MirAnimation.Combat7, out Sound))
                        {
                            Sound.Play();
                        }
                        if (DiyMonActSound.TryGetValue(MirAnimation.Combat8, out Sound))
                        {
                            Sound.Play();
                        }
                        if (DiyMonActSound.TryGetValue(MirAnimation.Combat9, out Sound))
                        {
                            Sound.Play();
                        }
                        if (DiyMonActSound.TryGetValue(MirAnimation.Combat10, out Sound))
                        {
                            Sound.Play();
                        }
                        if (DiyMonActSound.TryGetValue(MirAnimation.Combat11, out Sound))
                        {
                            Sound.Play();
                        }
                        if (DiyMonActSound.TryGetValue(MirAnimation.Combat12, out Sound))
                        {
                            Sound.Play();
                        }
                        if (DiyMonActSound.TryGetValue(MirAnimation.Combat13, out Sound))
                        {
                            Sound.Play();
                        }
                        if (DiyMonActSound.TryGetValue(MirAnimation.Combat14, out Sound))
                        {
                            Sound.Play();
                        }
                        if (DiyMonActSound.TryGetValue(MirAnimation.Combat15, out Sound))
                        {
                            Sound.Play();
                        }
                        break;
                    case MirAnimationSound.StruckSound:
                        if (DiyMonActSound.TryGetValue(MirAnimation.Struck, out Sound))
                        {
                            Sound.Play();
                        }
                        break;
                    case MirAnimationSound.DieSound:
                        if (DiyMonActSound.TryGetValue(MirAnimation.Die, out Sound))
                        {
                            Sound.Play();
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        public void PlayAttackSound()
        {
            DXSoundManager.Play(AttackSound);
            PlayDiyMonActSound(MirAnimationSound.AttackSound);
        }
        public void PlayStruckSound()
        {
            DXSoundManager.Play(StruckSound);

            DXSoundManager.Play(SoundIndex.GenericStruckMonster);
            PlayDiyMonActSound(MirAnimationSound.StruckSound);
        }
        public void PlayDieSound()
        {
            DXSoundManager.Play(DieSound);
            PlayDiyMonActSound(MirAnimationSound.DieSound);
        }
    }
    public enum ShowDiyEffectType
    {
        EquipmentIn,
        EquipmentOut,
        MonAction,
        DiyEffect,
        MonAiAction,
    }
    public class EquipmentObject : MapObject
    {
        public DiyMagicEffect DresslightInShow_Down;
        public DiyMagicEffect DresslightInShow;
        public DiyMagicEffect WeaponlightInShow;

        //画内观相关
        public int InnAnimation = 0;
        public DateTime InnAnimationTime;

        public ItemInfo iteminfo;

        public EquipmentObject(ItemInfo info)
        {
            iteminfo = info;

            UpdateLibraries();
        }
        public override void UpdateLibraries()
        {
            DresslightInShow_Down = null;
            DresslightInShow = null;
            WeaponlightInShow = null;
            if (iteminfo.ItemType == ItemType.Armour || iteminfo.ItemType == ItemType.Fashion)
            {
                ClothType = iteminfo.Image;
                WeaponId = 0;
                DresslightInShow_Down = SMain.Session.GetCollection<DiyMagicEffect>().Binding.FirstOrDefault(p => p.MagicID == ClothType && p.MagicType == (HumGender == MirGender.Male ? DiyMagicType.DressLightInner_Man_Down : DiyMagicType.DressLightInner_WoMan_Down));
                DresslightInShow = SMain.Session.GetCollection<DiyMagicEffect>().Binding.FirstOrDefault(p => p.MagicID == ClothType && p.MagicType == (HumGender == MirGender.Male ? DiyMagicType.DressLightInner_Man : DiyMagicType.DressLightInner_WoMan));

            }
            else if (iteminfo.ItemType == ItemType.Weapon)
            {
                ClothType = 942;
                WeaponId = iteminfo.Image;
                WeaponlightInShow = SMain.Session.GetCollection<DiyMagicEffect>().Binding.FirstOrDefault(p => p.MagicID == iteminfo.Shape && p.MagicType == DiyMagicType.WeaponLightInner);
            }

        }
        public override void Draw()
        {
            base.Draw();

            Rectangle DisplayArea = new Rectangle(0, 0, DrawSize.Width / 2, DrawSize.Height);
            Color ForeColour = Color.White;

            MirLibrary library;

            int x = 130;
            int y = 270;

            //画人物内观底层

            //1衣服内观下层;
            if (DresslightInShow_Down != null)
            {
                MirLibrary effectLibrary;
                if (DiyMonShowViewer.CurrentDiyMonViewer.Manager.LibraryList.TryGetValue(DresslightInShow_Down.file, out effectLibrary))
                {
                    MirImage image = null;
                    image = effectLibrary.CreateImage(DresslightInShow_Down.startIndex + (DrawFrameAni % (DresslightInShow_Down.frameCount + 1)), ImageType.Image);
                    if (image != null)
                    {
                        bool oldBlend = DiyMonShowViewer.CurrentDiyMonViewer.Manager.Blending;
                        float oldRate = DiyMonShowViewer.CurrentDiyMonViewer.Manager.BlendRate;

                        DiyMonShowViewer.CurrentDiyMonViewer.Manager.SetBlend(true, 0.8F);


                        DiyMonShowViewer.CurrentDiyMonViewer.Manager.Sprite.Draw(image.Image, new Rectangle(0, 0, image.Width, image.Height), Vector3.Zero, new Vector3(DisplayArea.X + x + image.OffSetX, DisplayArea.Y + y + image.OffSetY, 0), ForeColour);

                        DiyMonShowViewer.CurrentDiyMonViewer.Manager.SetBlend(oldBlend, oldRate);
                    }
                }
            }

            // 画人物体格
            if (!DiyMonShowViewer.CurrentDiyMonViewer.Manager.LibraryList.TryGetValue(LibraryFile.ProgUse, out library)) return;

            if (HumClass == MirClass.Assassin && HumGender == MirGender.Female)
                library.Draw(1160, DisplayArea.X + x, DisplayArea.Y + y, Color.Red, true, 1F, ImageType.Image);

            switch (HumGender)
            {
                case MirGender.Male:
                    library.Draw(0, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                    break;
                case MirGender.Female:
                    library.Draw(1, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                    break;
            }

            // 画人物身上的装备
            if (DiyMonShowViewer.CurrentDiyMonViewer.Manager.LibraryList.TryGetValue(LibraryFile.Equip, out library))
            {

                int index = ClothType;
                if (HumGender == MirGender.Female)
                {
                    index += 10;
                }

                library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Overlay);

                library.Draw(WeaponId, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                library.Draw(WeaponId, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Overlay);
            }

            //1衣服内观下层;
            if (DresslightInShow != null)
            {
                MirLibrary effectLibrary;
                if (DiyMonShowViewer.CurrentDiyMonViewer.Manager.LibraryList.TryGetValue(DresslightInShow.file, out effectLibrary))
                {
                    MirImage image = null;
                    image = effectLibrary.CreateImage(DresslightInShow.startIndex + (DrawFrameAni % (DresslightInShow.frameCount + 1)), ImageType.Image);
                    if (image != null)
                    {
                        bool oldBlend = DiyMonShowViewer.CurrentDiyMonViewer.Manager.Blending;
                        float oldRate = DiyMonShowViewer.CurrentDiyMonViewer.Manager.BlendRate;

                        DiyMonShowViewer.CurrentDiyMonViewer.Manager.SetBlend(true, 0.8F);


                        DiyMonShowViewer.CurrentDiyMonViewer.Manager.Sprite.Draw(image.Image, new Rectangle(0, 0, image.Width, image.Height), Vector3.Zero, new Vector3(DisplayArea.X + x + image.OffSetX, DisplayArea.Y + y + image.OffSetY, 0), ForeColour);

                        DiyMonShowViewer.CurrentDiyMonViewer.Manager.SetBlend(oldBlend, oldRate);
                    }
                }
            }

            //画武器内观
            if (WeaponlightInShow != null)
            {
                MirLibrary effectLibrary;
                if (DiyMonShowViewer.CurrentDiyMonViewer.Manager.LibraryList.TryGetValue(WeaponlightInShow.file, out effectLibrary))
                {
                    MirImage image = null;
                    image = effectLibrary.CreateImage(WeaponlightInShow.startIndex + (DrawFrameAni % (WeaponlightInShow.frameCount + 1)), ImageType.Image);
                    if (image != null)
                    {
                        bool oldBlend = DiyMonShowViewer.CurrentDiyMonViewer.Manager.Blending;
                        float oldRate = DiyMonShowViewer.CurrentDiyMonViewer.Manager.BlendRate;

                        DiyMonShowViewer.CurrentDiyMonViewer.Manager.SetBlend(true, 0.8F);

                        DiyMonShowViewer.CurrentDiyMonViewer.Manager.Sprite.Draw(image.Image, new Rectangle(0, 0, image.Width, image.Height), Vector3.Zero, new Vector3(DisplayArea.X + x + image.OffSetX, DisplayArea.Y + y + image.OffSetY, 0), ForeColour);

                        DiyMonShowViewer.CurrentDiyMonViewer.Manager.SetBlend(oldBlend, oldRate);
                    }
                }
            }
        }
        public override void UpdateFrame()
        {
            if (CurDiyEffectType == ShowDiyEffectType.EquipmentIn)
            {
                if (DresslightInShow != null && SEnvir.DiyShowViewerLoopTime > InnAnimationTime)
                {
                    InnAnimationTime = SEnvir.DiyShowViewerLoopTime.AddMilliseconds(DresslightInShow.frameDelay);
                    InnAnimation++;
                }
            }
        }
    }

    public class PlayerBodyObject : MapObject
    {
        public DiyMagicEffect DresslightOutShow;
        public DiyMagicEffect WeaponlightOutShow;

        public ItemInfo iteminfo;

        public bool DrawWeapon = true;
        public MirLibrary WeaponLibrary1 = null, WeaponLibrary2 = null;

        public int WeaponFrame => DrawFrame + (WeaponId % 10) * WeaponShapeOffSet;
        public int WeaponShapeOffSet = 5000;

        public int ArmourFrame => DrawFrame + (ClothType % 11) * ArmourShapeOffSet + ArmourShift;

        public int ArmourShapeOffSet = 5000;
        public int ArmourShift;
        public Color ArmourColour = Color.White;

        //画外观相关
        public int OutDressAnimation = 0;
        public DateTime OutDressAnimationTime;
        public int OutWeaponAnimation = 0;
        //public DateTime OutWeaponAnimationTime;
        public DateTime FrameStart;

        public PlayerBodyObject(ItemInfo info)
        {
            iteminfo = info;

            UpdateLibraries();
            SetBody();
        }
        public override void UpdateLibraries()
        {
            CurDiyEffectType = ShowDiyEffectType.EquipmentOut;
            Frames = new Dictionary<MirAnimation, Frame>(FrameSet.Players);
            DresslightOutShow = null;
            WeaponlightOutShow = null;

            if (iteminfo.ItemType == ItemType.Armour || iteminfo.ItemType == ItemType.Fashion)
            {
                ClothType = iteminfo.Shape;
                WeaponId = 0;
                DresslightOutShow = SMain.Session.GetCollection<DiyMagicEffect>().Binding.FirstOrDefault(p => p.MagicID == ClothType && p.MagicType == (HumGender == MirGender.Male ? DiyMagicType.DressLightOut_Man : DiyMagicType.DressLightOut_WoMan));

            }
            else if (iteminfo.ItemType == ItemType.Weapon)
            {
                ClothType = 2;
                WeaponId = iteminfo.Shape;
                WeaponlightOutShow = SMain.Session.GetCollection<DiyMagicEffect>().Binding.FirstOrDefault(p => p.MagicID == iteminfo.Shape && p.MagicType == (HumGender == MirGender.Male ? DiyMagicType.WeaponLightOut_Man : DiyMagicType.WeaponLightOut_WoMan));
            }
        }

        public void SetBody()
        {
            WeaponLibrary1 = null;
            WeaponLibrary2 = null;
            BodyLibrary = null;
            LibraryFile file;
            switch (HumClass)
            {
                case MirClass.Warrior:
                case MirClass.Wizard:
                case MirClass.Taoist:
                    ArmourShapeOffSet = 5000;
                    WeaponShapeOffSet = 5000;
                    if (HumGender == MirGender.Male)
                    {
                        if (!ArmourList.TryGetValue(ClothType / 11, out file))
                        {
                            file = LibraryFile.M_Hum;
                            ClothType = 0;
                        }
                    }
                    else
                    {
                        if (!ArmourList.TryGetValue(ClothType / 11 + FemaleOffSet, out file))
                        {
                            file = LibraryFile.WM_Hum;
                            ClothType = 0;
                        }
                    }
                    DiyMonShowViewer.CurrentDiyMonViewer.Manager.LibraryList.TryGetValue(file, out BodyLibrary);
                    break;
                case MirClass.Assassin:
                    ArmourShapeOffSet = 3000;
                    WeaponShapeOffSet = 5000;

                    if (HumGender == MirGender.Male)
                    {
                        if (!ArmourList.TryGetValue(ClothType / 11 + AssassinOffSet, out file))
                        {
                            file = LibraryFile.M_HumA;
                            ClothType = 0;
                        }
                    }
                    else
                    {
                        if (!ArmourList.TryGetValue(ClothType / 11 + AssassinOffSet + FemaleOffSet, out file))
                        {
                            file = LibraryFile.WM_HumA;
                            ClothType = 0;
                        }
                    }
                    DiyMonShowViewer.CurrentDiyMonViewer.Manager.LibraryList.TryGetValue(file, out BodyLibrary);
                    break;
            }

            if (!WeaponList.TryGetValue(WeaponId / 10, out file)) file = LibraryFile.None;
            DiyMonShowViewer.CurrentDiyMonViewer.Manager.LibraryList.TryGetValue(file, out WeaponLibrary1);
        }

        public override void Draw()
        {
            UpdateFrame();
            int DrawX = CurrentLocation.X, DrawY = CurrentLocation.Y;
            int l = int.MaxValue, t = int.MaxValue, r = int.MinValue, b = int.MinValue;

            MirImage image;
            switch (CurDirection)
            {
                case MirDirection.Up:
                case MirDirection.DownLeft:
                case MirDirection.Left:
                case MirDirection.UpLeft:
                    if (!DrawWeapon) break;
                    image = WeaponLibrary1?.GetImage(WeaponFrame);
                    if (image == null) break;

                    WeaponLibrary1.Draw(WeaponFrame, DrawX, DrawY, Color.White, true, 1F, ImageType.Image);

                    l = Math.Min(l, DrawX + image.OffSetX);
                    t = Math.Min(t, DrawY + image.OffSetY);
                    r = Math.Max(r, image.Width + DrawX + image.OffSetX);
                    b = Math.Max(b, image.Height + DrawY + image.OffSetY);
                    break;
                default:
                    if (!DrawWeapon) break;
                    break;
            }

            image = BodyLibrary?.GetImage(ArmourFrame);
            if (image != null)
            {
                BodyLibrary.Draw(ArmourFrame, DrawX, DrawY, Color.White, true, 1F, ImageType.Image);

                if (ArmourColour.ToArgb() != 0)
                    BodyLibrary.Draw(ArmourFrame, DrawX, DrawY, ArmourColour, true, 1F, ImageType.Overlay);

                l = Math.Min(l, DrawX + image.OffSetX);
                t = Math.Min(t, DrawY + image.OffSetY);
                r = Math.Max(r, image.Width + DrawX + image.OffSetX);
                b = Math.Max(b, image.Height + DrawY + image.OffSetY);
            }

            switch (CurDirection)
            {
                case MirDirection.UpRight:
                case MirDirection.Right:
                case MirDirection.DownRight:
                case MirDirection.Down:
                    if (!DrawWeapon) break;
                    image = WeaponLibrary1?.GetImage(WeaponFrame);
                    if (image == null) break;

                    WeaponLibrary1.Draw(WeaponFrame, DrawX, DrawY, Color.White, true, 1F, ImageType.Image);

                    //画武器外观         
                    if (WeaponlightOutShow != null)
                    {
                        MirLibrary Swinglibrary;
                        if (!DiyMonShowViewer.CurrentDiyMonViewer.Manager.LibraryList.TryGetValue(WeaponlightOutShow.file, out Swinglibrary)) break;

                        Swinglibrary.DrawBlend(WeaponlightOutShow.startIndex + OutWeaponAnimation, DrawX, DrawY, Color.White, true, 0.7f, ImageType.Image);
                    }

                    l = Math.Min(l, DrawX + image.OffSetX);
                    t = Math.Min(t, DrawY + image.OffSetY);
                    r = Math.Max(r, image.Width + DrawX + image.OffSetX);
                    b = Math.Max(b, image.Height + DrawY + image.OffSetY);
                    break;
                default:
                    if (!DrawWeapon) break;

                    break;
            }

            //画武器外观         
            if (WeaponlightOutShow != null)
            {
                MirLibrary Swinglibrary;
                if (!DiyMonShowViewer.CurrentDiyMonViewer.Manager.LibraryList.TryGetValue(WeaponlightOutShow.file, out Swinglibrary)) return;

                Swinglibrary.DrawBlend(WeaponlightOutShow.startIndex + OutWeaponAnimation, DrawX, DrawY, Color.White, true, 0.7f, ImageType.Image);
            }
        }

        public override void UpdateFrame()
        {

            DrawFrame = CurrentFrame.StartIndex + CurrentFrame.GetFrame(FrameStart, SEnvir.DiyShowViewerLoopTime, false) + (int)CurDirection * 10;
            if (DrawFrame % 10 == CurrentFrame.FrameCount || CurrentFrame.GetFrame(FrameStart, SEnvir.DiyShowViewerLoopTime, false) == CurrentFrame.OffSet)
            {
                DrawFrame = CurrentFrame.StartIndex + (int)CurDirection * 10; ;
                FrameStart = SEnvir.DiyShowViewerLoopTime;
            }
            if (DresslightOutShow != null)
            {
                if (DresslightOutShow.magicDir == MagicDir.Dir16)//随着人物动作
                {
                    OutDressAnimation = DrawFrame;
                    OutDressAnimationTime = SEnvir.DiyShowViewerLoopTime.AddMilliseconds(DresslightOutShow.frameDelay);
                    //     QiQuShowPic.TextureValid = false;
                }
                else
                {
                    if (SEnvir.DiyShowViewerLoopTime > OutDressAnimationTime)
                    {
                        OutDressAnimationTime = SEnvir.DiyShowViewerLoopTime.AddMilliseconds(DresslightOutShow.frameDelay);
                        OutDressAnimation++;
                        if (DresslightOutShow.magicDir == MagicDir.None)//独立单方向
                        {
                            OutDressAnimation = OutDressAnimation % (DresslightOutShow.frameCount + 1);
                        }
                        else if (DresslightOutShow.magicDir == MagicDir.Dir8)//独立单方向
                        {
                            OutDressAnimation = OutDressAnimation % (DresslightOutShow.frameCount + 1) + (int)CurDirection * DresslightOutShow.frameCount;
                        }
                    }
                }
            }

            if (WeaponlightOutShow != null)
            {
                OutWeaponAnimation = DrawFrame;
            }
        }


        #region Weapon Librarys
        public const int FemaleOffSet = 5000, AssassinOffSet = 50000, RightHandOffSet = 50;
        public static Dictionary<int, LibraryFile> WeaponList = new Dictionary<int, LibraryFile>
        {
            [0] = LibraryFile.M_Weapon1,
            [1] = LibraryFile.M_Weapon2,
            [2] = LibraryFile.M_Weapon3,
            [3] = LibraryFile.M_Weapon4,
            [4] = LibraryFile.M_Weapon5,
            [5] = LibraryFile.M_Weapon6,
            [6] = LibraryFile.M_Weapon7,
            [10] = LibraryFile.M_Weapon10,
            [11] = LibraryFile.M_Weapon11,
            [12] = LibraryFile.M_Weapon12,
            [13] = LibraryFile.M_Weapon13,
            [14] = LibraryFile.M_Weapon14,
            [15] = LibraryFile.M_Weapon15,
            [16] = LibraryFile.M_Weapon16,

            [0 + FemaleOffSet] = LibraryFile.WM_Weapon1,
            [1 + FemaleOffSet] = LibraryFile.WM_Weapon2,
            [2 + FemaleOffSet] = LibraryFile.WM_Weapon3,
            [3 + FemaleOffSet] = LibraryFile.WM_Weapon4,
            [4 + FemaleOffSet] = LibraryFile.WM_Weapon5,
            [5 + FemaleOffSet] = LibraryFile.WM_Weapon6,
            [6 + FemaleOffSet] = LibraryFile.WM_Weapon7,
            [10 + FemaleOffSet] = LibraryFile.WM_Weapon10,
            [11 + FemaleOffSet] = LibraryFile.WM_Weapon11,
            [12 + FemaleOffSet] = LibraryFile.WM_Weapon12,
            [13 + FemaleOffSet] = LibraryFile.WM_Weapon13,
            [14 + FemaleOffSet] = LibraryFile.WM_Weapon14,
            [15 + FemaleOffSet] = LibraryFile.WM_Weapon15,
            [16 + FemaleOffSet] = LibraryFile.WM_Weapon16,

            [120] = LibraryFile.M_WeaponADL1,
            [122] = LibraryFile.M_WeaponADL2,
            [126] = LibraryFile.M_WeaponADL6,
            [120 + RightHandOffSet] = LibraryFile.M_WeaponADR1,
            [122 + RightHandOffSet] = LibraryFile.M_WeaponADR2,
            [126 + RightHandOffSet] = LibraryFile.M_WeaponADR6,

            [110] = LibraryFile.M_WeaponAOH1,
            [111] = LibraryFile.M_WeaponAOH2,
            [112] = LibraryFile.M_WeaponAOH3,
            [113] = LibraryFile.M_WeaponAOH3,
            [114] = LibraryFile.M_WeaponAOH4,
            [115] = LibraryFile.M_WeaponAOH5,
            [116] = LibraryFile.M_WeaponAOH6,


            [120 + FemaleOffSet] = LibraryFile.WM_WeaponADL1,
            [122 + FemaleOffSet] = LibraryFile.WM_WeaponADL2,
            [126 + FemaleOffSet] = LibraryFile.WM_WeaponADL6,
            [120 + FemaleOffSet + RightHandOffSet] = LibraryFile.WM_WeaponADR1,
            [122 + FemaleOffSet + RightHandOffSet] = LibraryFile.WM_WeaponADR2,
            [126 + FemaleOffSet + RightHandOffSet] = LibraryFile.WM_WeaponADR6,

            [110 + FemaleOffSet] = LibraryFile.WM_WeaponAOH1,
            [111 + FemaleOffSet] = LibraryFile.WM_WeaponAOH2,
            [112 + FemaleOffSet] = LibraryFile.WM_WeaponAOH3,
            [113 + FemaleOffSet] = LibraryFile.WM_WeaponAOH3,
            [114 + FemaleOffSet] = LibraryFile.WM_WeaponAOH4,
            [115 + FemaleOffSet] = LibraryFile.WM_WeaponAOH5,
            [116 + FemaleOffSet] = LibraryFile.WM_WeaponAOH6,
        };
        #endregion

        #region Armour Librarys
        public static Dictionary<int, LibraryFile> ArmourList = new Dictionary<int, LibraryFile>
        {
            [0] = LibraryFile.M_Hum,
            [1] = LibraryFile.M_HumEx1,
            [2] = LibraryFile.M_HumEx2,
            [3] = LibraryFile.M_HumEx3,
            [4] = LibraryFile.M_HumEx4,
            [10] = LibraryFile.M_HumEx10,
            [11] = LibraryFile.M_HumEx11,
            [12] = LibraryFile.M_HumEx12,
            [13] = LibraryFile.M_HumEx13,


            [0 + FemaleOffSet] = LibraryFile.WM_Hum,
            [1 + FemaleOffSet] = LibraryFile.WM_HumEx1,
            [2 + FemaleOffSet] = LibraryFile.WM_HumEx2,
            [3 + FemaleOffSet] = LibraryFile.WM_HumEx3,
            [4 + FemaleOffSet] = LibraryFile.WM_HumEx4,
            [10 + FemaleOffSet] = LibraryFile.WM_HumEx10,
            [11 + FemaleOffSet] = LibraryFile.WM_HumEx11,
            [12 + FemaleOffSet] = LibraryFile.WM_HumEx12,
            [13 + FemaleOffSet] = LibraryFile.WM_HumEx13,


            [0 + AssassinOffSet] = LibraryFile.M_HumA,
            [1 + AssassinOffSet] = LibraryFile.M_HumAEx1,
            [2 + AssassinOffSet] = LibraryFile.M_HumAEx2,
            [3 + AssassinOffSet] = LibraryFile.M_HumAEx3,

            [0 + AssassinOffSet + FemaleOffSet] = LibraryFile.WM_HumA,
            [1 + AssassinOffSet + FemaleOffSet] = LibraryFile.WM_HumAEx1,
            [2 + AssassinOffSet + FemaleOffSet] = LibraryFile.WM_HumAEx2,
            [3 + AssassinOffSet + FemaleOffSet] = LibraryFile.WM_HumAEx3,
        };
        #endregion
    }
    public class MapObject
    {
        public MapObject()
        { }
        public Size DrawSize
        {
            get { return _DrawSize; }
            set
            {
                if (_DrawSize == value) return;

                Size oldValue = _DrawSize;
                _DrawSize = value;
                OnSizeChange(oldValue, DrawSize);
            }
        }
        private Size _DrawSize;

        public virtual void OnSizeChange(Size OldSize, Size NewSize)
        {
            CurrentLocation = DiyMonShowViewer.CurrentDiyMonViewer.ObjectPoint;
            MagicLocations.Clear();
            MagicLocations.Add(DiyMonShowViewer.CurrentDiyMonViewer.TargetPoint);
            AttackTargets.Clear();
            AttackTargets.Add(DiyMonShowViewer.CurrentDiyMonViewer.TargetObject);
        }

        //人物内观和外观需要的
        public MirClass HumClass = MirClass.Warrior;
        public MirGender HumGender = MirGender.Male;
        public int ClothType = 0;
        public int WeaponId = 0;

        //当前选中的自定义动画
        public DiyMagicEffect DiyMagicEffectInfo;

        //给怪物和人物同时使用
        public MirDirection CurDirection = MirDirection.Right;
        public Point CurrentLocation = new Point();
        public List<Point> MagicLocations = new List<Point>();
        public List<MapObject> AttackTargets = new List<MapObject>();

        public List<MirEffect> Effects = new List<MirEffect>();
        public int DrawFrameAni = 0;

        public MirLibrary BodyLibrary;
        public int BodyOffSet = 1000;
        public int BodyShape;
        public int DrawFrame = 0;
        public int BodyFrame => DrawFrame + (BodyShape % 10) * BodyOffSet;
        public int FrameIndex;
        public Dictionary<MirAnimation, Frame> Frames;
        public Frame CurrentFrame;
        public MirAnimation CurrentAnimation;

        public virtual void UpdateFrame()
        {
            if (Frames == null || CurrentFrame == null) return;

            FrameIndex = DrawFrameAni % CurrentFrame.FrameCount;

            DrawFrame = FrameIndex + CurrentFrame.StartIndex + CurrentFrame.OffSet * (int)CurDirection;

        }
        public virtual void UpdateLibraries()
        {
        }
        public ShowDiyEffectType CurDiyEffectType
        {
            get { return _CurObjectType; }
            set
            {
                if (_CurObjectType == value) return;
                ShowDiyEffectType oldValue = _CurObjectType;
                _CurObjectType = value;
            }
        }
        private ShowDiyEffectType _CurObjectType;

        public virtual void SetAnimation(MirAnimation animation, bool Loop = true)
        {
            CurrentAnimation = animation;
            if (!Frames.TryGetValue(CurrentAnimation, out CurrentFrame))
                CurrentFrame = Frame.EmptyFrame;
        }

        public virtual void Draw()
        {
            switch (CurDiyEffectType)
            {
                case ShowDiyEffectType.EquipmentIn:

                    break;
                case ShowDiyEffectType.EquipmentOut:

                    break;
                case ShowDiyEffectType.MonAction:
                    break;
                case ShowDiyEffectType.DiyEffect:
                    break;
                case ShowDiyEffectType.MonAiAction:
                    break;
                default:
                    break;
            }
            foreach (MirEffect ob in Effects)
            {
                ob.Draw();
            }
        }

        public void ShowDiyMagicEffect(DiyMagicEffect MagicEffectinfo, Element AttackElement, List<MapObject> AttackTargets, List<Point> MagicLocations)
        {
            Color attackColour = Globals.NoneColour;
            MirEffect spell;
            switch (AttackElement)
            {
                case Element.Fire:
                    attackColour = Globals.FireColour;
                    break;
                case Element.Ice:
                    attackColour = Globals.IceColour;
                    break;
                case Element.Lightning:
                    attackColour = Globals.LightningColour;
                    break;
                case Element.Wind:
                    attackColour = Globals.WindColour;
                    break;
                case Element.Holy:
                    attackColour = Globals.HolyColour;
                    break;
                case Element.Dark:
                    attackColour = Globals.DarkColour;
                    break;
                case Element.Phantom:
                    attackColour = Globals.PhantomColour;
                    break;
            }
            switch (MagicEffectinfo.MagicType)
            {
                case DiyMagicType.Point:
                case DiyMagicType.ExplosionMagic:
                case DiyMagicType.BodyEffect:
                case DiyMagicType.MonAttackMon:
                case DiyMagicType.MagicAttackPlayer:
                case DiyMagicType.MonOnSpawned:
                case DiyMagicType.DressLightInner_Man_Down:
                case DiyMagicType.DressLightInner_WoMan_Down:
                case DiyMagicType.DressLightInner_Man:
                case DiyMagicType.DressLightInner_WoMan:
                case DiyMagicType.DressLightOut_Man:
                case DiyMagicType.DressLightOut_WoMan:
                case DiyMagicType.WeaponLightInner:
                case DiyMagicType.WeaponLightOut_Man:
                case DiyMagicType.WeaponLightOut_WoMan:
                case DiyMagicType.MonDie:
                case DiyMagicType.MonCombat1:
                case DiyMagicType.MonCombat2:
                case DiyMagicType.MonCombat3:
                case DiyMagicType.MonCombat4:
                case DiyMagicType.MonCombat5:
                    foreach (MapObject target in AttackTargets)
                    {
                        if (MagicEffectinfo.magicDir > 0)
                        {
                            Effects.Add(spell = new MirEffect(MagicEffectinfo.startIndex, MagicEffectinfo.frameCount, TimeSpan.FromMilliseconds(MagicEffectinfo.frameDelay), MagicEffectinfo.file, MagicEffectinfo.startLight, MagicEffectinfo.endLight, MagicEffectinfo.lightColour) //Element style?
                            {
                                Blend = true,
                                Target = this,
                                Direction = CurDirection,//8方向
                                DrawColour = attackColour
                            });
                            spell.Process();
                        }
                        else
                        {
                            Effects.Add(spell = new MirEffect(MagicEffectinfo.startIndex, MagicEffectinfo.frameCount, TimeSpan.FromMilliseconds(MagicEffectinfo.frameDelay), MagicEffectinfo.file, MagicEffectinfo.startLight, MagicEffectinfo.endLight, MagicEffectinfo.lightColour) //Element style?
                            {
                                Blend = true,
                                Target = target,
                                DrawColour = attackColour
                            });
                            spell.Process();
                        }
                    }
                    DXSoundManager.Play(MagicEffectinfo.MagicSound);
                    break;
                case DiyMagicType.Line://地面线魔法
                    foreach (Point point in MagicLocations)
                    {
                        Effects.Add(spell = new MirEffect(MagicEffectinfo.startIndex, MagicEffectinfo.frameCount, TimeSpan.FromMilliseconds(MagicEffectinfo.frameDelay), MagicEffectinfo.file, MagicEffectinfo.startLight, MagicEffectinfo.endLight, MagicEffectinfo.lightColour)
                        {
                            MapTarget = point,
                            StartTime = SEnvir.DiyShowViewerLoopTime.AddMilliseconds(500 + Functions.Distance(point, CurrentLocation) * 50),
                            Opacity = 0.8F,
                            DrawType = DrawType.Floor,
                            DrawColour = attackColour
                        });
                        spell.Process();
                    }
                    DXSoundManager.Play(MagicEffectinfo.MagicSound);
                    break;
                case DiyMagicType.FlyDestinationExplosion:
                    foreach (Point point in MagicLocations)
                    {
                        Effects.Add(spell = new MirProjectile(MagicEffectinfo.startIndex, MagicEffectinfo.frameCount, TimeSpan.FromMilliseconds(MagicEffectinfo.frameDelay), MagicEffectinfo.file, MagicEffectinfo.startLight, MagicEffectinfo.endLight, MagicEffectinfo.lightColour, CurrentLocation)
                        {
                            Blend = true,
                            MapTarget = point,
                            Explode = true,
                            DrawColour = attackColour
                        });

                        spell.CompleteAction = () =>
                        {
                            DiyMagicEffect ExplosionMagicEffectinfo = SMain.Session.GetCollection<DiyMagicEffect>().Binding.FirstOrDefault(x => x.MagicID == MagicEffectinfo.ExplosionMagicID && x.MagicType == DiyMagicType.ExplosionMagic);
                            if (ExplosionMagicEffectinfo != null)
                            {
                                spell = new MirEffect(ExplosionMagicEffectinfo.startIndex, ExplosionMagicEffectinfo.frameCount, TimeSpan.FromMilliseconds(ExplosionMagicEffectinfo.frameDelay), ExplosionMagicEffectinfo.file, ExplosionMagicEffectinfo.startLight, ExplosionMagicEffectinfo.endLight, ExplosionMagicEffectinfo.lightColour)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                    DrawColour = attackColour
                                };
                                spell.Process();

                                DXSoundManager.Play(ExplosionMagicEffectinfo.MagicSound);
                            }
                        };
                        spell.Process();
                        if (MagicLocations.Count > 0)
                            DXSoundManager.Play(MagicEffectinfo.MagicSound);
                    }
                    break;
                case DiyMagicType.FiyHitTargetExplosion:
                    foreach (Point point in MagicLocations)
                    {
                        Effects.Add(spell = new MirProjectile(MagicEffectinfo.startIndex, MagicEffectinfo.frameCount, TimeSpan.FromMilliseconds(MagicEffectinfo.frameDelay), MagicEffectinfo.file, MagicEffectinfo.startLight, MagicEffectinfo.endLight, MagicEffectinfo.lightColour, CurrentLocation)
                        {
                            Blend = true,
                            MapTarget = point,
                            DrawColour = attackColour
                        });
                        spell.Process();
                    }

                    foreach (MapObject attackTarget in AttackTargets)
                    {
                        Effects.Add(spell = new MirProjectile(MagicEffectinfo.startIndex, MagicEffectinfo.frameCount, TimeSpan.FromMilliseconds(MagicEffectinfo.frameDelay), MagicEffectinfo.file, MagicEffectinfo.startLight, MagicEffectinfo.endLight, MagicEffectinfo.lightColour, CurrentLocation)
                        {
                            Blend = true,
                            Target = attackTarget,
                            DrawColour = attackColour
                        });

                        spell.CompleteAction = () =>
                        {
                            DiyMagicEffect ExplosionMagicEffectinfo = SMain.Session.GetCollection<DiyMagicEffect>().Binding.FirstOrDefault(x => x.MagicID == MagicEffectinfo.ExplosionMagicID && x.MagicType == DiyMagicType.ExplosionMagic);
                            if (ExplosionMagicEffectinfo != null)
                            {
                                attackTarget.Effects.Add(spell = new MirEffect(ExplosionMagicEffectinfo.startIndex, ExplosionMagicEffectinfo.frameCount, TimeSpan.FromMilliseconds(ExplosionMagicEffectinfo.frameDelay), ExplosionMagicEffectinfo.file, ExplosionMagicEffectinfo.startLight, ExplosionMagicEffectinfo.endLight, ExplosionMagicEffectinfo.lightColour)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                    DrawColour = attackColour
                                });
                                spell.Process();
                                DXSoundManager.Play(ExplosionMagicEffectinfo.MagicSound);
                            }
                        };
                        spell.Process();
                    }
                    if (MagicLocations.Count > 0 || AttackTargets.Count > 0)
                        DXSoundManager.Play(MagicEffectinfo.MagicSound);
                    break;
                default:
                    break;
            }
        }
        public void ShowDiyMagicEffect(DiyMagicEffect MagicEffectinfo, Element AttackElement)
        {
            MirEffect spell;
            if (MagicEffectinfo.magicDir > 0)
            {
                Effects.Add(spell = new MirEffect(MagicEffectinfo.startIndex, MagicEffectinfo.frameCount, TimeSpan.FromMilliseconds(MagicEffectinfo.frameDelay), MagicEffectinfo.file, MagicEffectinfo.startLight, MagicEffectinfo.endLight, MagicEffectinfo.lightColour) //Element style?
                {
                    Blend = true,
                    Target = this,
                    Direction = CurDirection,//8方向
                });
                spell.Process();
            }
            else
            {
                Effects.Add(spell = new MirEffect(MagicEffectinfo.startIndex, MagicEffectinfo.frameCount, TimeSpan.FromMilliseconds(MagicEffectinfo.frameDelay), MagicEffectinfo.file, MagicEffectinfo.startLight, MagicEffectinfo.endLight, MagicEffectinfo.lightColour) //Element style?
                {
                    Blend = true,
                    Target = this,
                });
                spell.Process();
            }
        }
    }
    public class MirProjectile : MirEffect
    {
        public Point Origin { get; set; }
        public int Speed { get; set; }
        public bool Explode { get; set; }

        public int Direction16 { get; set; }
        public bool Has16Directions { get; set; }

        public MirProjectile(int startIndex, int frameCount, TimeSpan frameDelay, LibraryFile file, int startlight, int endlight, Color lightColour, Point origin) : base(startIndex, frameCount, frameDelay, file, startlight, endlight, lightColour)
        {
            Has16Directions = true;

            Origin = origin;
            Speed = 50;
            Explode = false;
        }

        public override void Process()
        {
            Point location = Target?.CurrentLocation ?? MapTarget;

            if (location == Origin)
            {
                CompleteAction?.Invoke();
                Remove();
                return;
            }

            int x = (Origin.X);
            int y = (Origin.Y);

            int x1 = (location.X);
            int y1 = (location.Y);

            Direction16 = Functions.Direction16(new Point(x, y), new Point(x1, y1));
            long duration = Functions.Distance(new Point(x, y), new Point(x1, y1)) * 50;

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

            TimeSpan time = SEnvir.DiyShowViewerLoopTime - StartTime;

            int frame = GetFrame();

            if (Reversed)
                frame = FrameCount - frame - 1;

            DrawFrame = frame + StartIndex + Direction16 * Skip;

            DrawX = x + (int)(time.Ticks / duration);
            DrawY = y;


            if ((SEnvir.DiyShowViewerLoopTime - StartTime).Ticks > duration * 586)
            {
                if (Target == null && !Explode)
                {
                    Size s = Library.GetSize(FrameIndex);

                    if (DrawX + s.Width > 0 && DrawX < DiyMonShowViewer.CurrentDiyMonViewer.CurObject.DrawSize.Width &&
                        DrawY + s.Height > 0 && DrawY < DiyMonShowViewer.CurrentDiyMonViewer.CurObject.DrawSize.Height) return;
                }

                CompleteAction?.Invoke();
                Remove();
                return;
            }
        }
        protected override int GetFrame()
        {
            TimeSpan enlapsed = SEnvir.DiyShowViewerLoopTime - StartTime;

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
    public class MirEffect
    {
        public MapObject Target;
        public Point MapTarget;

        public MirLibrary Library;

        public DateTime StartTime;
        public int StartIndex;
        public int FrameCount;
        public TimeSpan[] Delays;

        public int FrameIndex;

        public Color DrawColour = Color.White;
        public bool Blend;
        public bool Reversed;
        public float Opacity = 1F;
        public float BlendRate = 1F;
        public bool UseOffSet = true;
        public bool Loop = false;

        public int DrawX
        {
            get { return _DrawX; }
            set
            {
                if (_DrawX == value) return;

                _DrawX = value;
            }
        }
        private int _DrawX;

        public int DrawY
        {
            get { return _DrawY; }
            set
            {
                if (_DrawY == value) return;

                _DrawY = value;
            }
        }
        private int _DrawY;

        public int DrawFrame
        {
            get { return _DrawFrmae; }
            set
            {
                if (_DrawFrmae == value) return;

                _DrawFrmae = value;
                FrameAction?.Invoke();
            }
        }
        private int _DrawFrmae;

        public DrawType DrawType = DrawType.Object;

        public int Skip { get; set; }
        public MirDirection Direction { get; set; }

        public Color[] LightColours;
        public int StartLight, EndLight;

        public float FrameLight
        {
            get
            {
                if (SEnvir.DiyShowViewerLoopTime < StartTime) return 0;

                TimeSpan enlapsed = SEnvir.DiyShowViewerLoopTime - StartTime;

                if (Loop)
                    enlapsed = TimeSpan.FromTicks(enlapsed.Ticks % TotalDuration.Ticks);

                return StartLight + (EndLight - StartLight) * enlapsed.Ticks / TotalDuration.Ticks;
            }
        }
        public Color FrameLightColour => LightColours[FrameIndex];
        public Point CurrentLocation => Target?.CurrentLocation ?? MapTarget;
        public Point MovingOffSet => Point.Empty;

        public Action CompleteAction;
        public Action FrameAction;

        public Point AdditionalOffSet;

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

        public MirEffect(int startIndex, int frameCount, TimeSpan frameDelay, LibraryFile file, int startLight, int endLight, Color lightColour)
        {
            StartIndex = startIndex;
            FrameCount = frameCount;
            Skip = 10;

            StartTime = SEnvir.DiyShowViewerLoopTime;
            StartLight = startLight;
            EndLight = endLight;

            Delays = new TimeSpan[FrameCount];
            LightColours = new Color[FrameCount];
            for (int i = 0; i < frameCount; i++)
            {
                Delays[i] = frameDelay;
                LightColours[i] = lightColour;
            }

            DiyMonShowViewer.CurrentDiyMonViewer.Manager.LibraryList.TryGetValue(file, out Library);
        }

        public virtual void Process()
        {
            if (SEnvir.DiyShowViewerLoopTime < StartTime) return;

            if (Target != null)
            {
                DrawX = Target.CurrentLocation.X + AdditionalOffSet.X;
                DrawY = Target.CurrentLocation.Y + AdditionalOffSet.Y;
            }
            else
            {
                DrawX = (MapTarget.X - DiyMonShowViewer.CurrentDiyMonViewer.CurObject.CurrentLocation.X);
                DrawY = (MapTarget.Y - DiyMonShowViewer.CurrentDiyMonViewer.CurObject.CurrentLocation.Y);
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

        protected virtual int GetFrame()
        {
            TimeSpan enlapsed = SEnvir.DiyShowViewerLoopTime - StartTime;

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


        public void Draw()
        {
            if (SEnvir.DiyShowViewerLoopTime < StartTime || Library == null) return;

            if (Blend)
                Library.DrawBlend(DrawFrame, DrawX, DrawY, DrawColour, UseOffSet, BlendRate, ImageType.Image);
            else
                Library.Draw(DrawFrame, DrawX, DrawY, DrawColour, UseOffSet, Opacity, ImageType.Image);
        }

        public void Remove()
        {
            CompleteAction = null;
            FrameAction = null;

            DiyMonShowViewer.CurrentDiyMonViewer.CurObject?.Effects.Remove(this);
            Target?.Effects.Remove(this);
        }
    }

    public enum DrawType
    {
        Floor,
        Object,
        Final,
    }

    public sealed class DXSound
    {
        public string FileName { get; set; }

        public List<SecondarySoundBuffer> BufferList = new List<SecondarySoundBuffer>();

        private WaveFormat Format;
        private byte[] RawData;


        public DateTime ExpireTime { get; set; }
        public bool Loop { get; set; }

        public SoundType SoundType { get; set; }

        public int Volume { get; set; }

        public DXSound(string fileName, SoundType type)
        {
            FileName = Path.Combine(Config.ClientPath + fileName);
            SoundType = type;

            Volume = 100;
        }
        public void Play()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            player.SoundLocation = FileName;
            player.Load();
            player.Play();
            //return;
            if (RawData == null)
            {
                if (!File.Exists(FileName)) return;
                var stream = File.OpenRead(FileName);
                using (SoundStream wStream = new SoundStream(stream))
                {
                    Format = wStream.Format;
                    RawData = new byte[wStream.Length];

                    wStream.Position = 44;
                    wStream.Read(RawData, 0, RawData.Length);
                }
                DXManager.SoundList.Add(this);
                stream.Dispose();
            }


            if (BufferList.Count == 0)
                CreateBuffer();

            if (Loop)
            {
                if (((BufferStatus)BufferList[0].Status & BufferStatus.Playing) != BufferStatus.Playing) BufferList[0].Play(0, PlayFlags.Looping);
                ExpireTime = DateTime.MaxValue;
                return;
            }
            ExpireTime = SEnvir.DiyShowViewerLoopTime.AddMinutes(1);

            for (int i = BufferList.Count - 1; i >= 0; i--)
            {
                if (BufferList[i].IsDisposed)
                {
                    BufferList.RemoveAt(i);
                    continue;
                }

                if ((BufferStatus)BufferList[i].Status == BufferStatus.Playing) continue;

                BufferList[i].Play(0, PlayFlags.None);
                return;
            }

            SecondarySoundBuffer buff = CreateBuffer();
            buff.Play(0, PlayFlags.None);
        }
        public void Stop()
        {
            if (BufferList == null) return;

            if (Loop)
                ExpireTime = SEnvir.DiyShowViewerLoopTime.AddMinutes(1);

            for (int i = BufferList.Count - 1; i >= 0; i--)
            {
                if (BufferList[i].IsDisposed)
                {
                    BufferList.RemoveAt(i);
                    continue;
                }
                BufferList[i].CurrentPosition = 0;
                BufferList[i].Stop();
            }
        }

        private SecondarySoundBuffer CreateBuffer()
        {
            SecondarySoundBuffer buff;
            BufferFlags flags = BufferFlags.ControlVolume;

            flags |= BufferFlags.GlobalFocus;


            BufferList.Add(buff = new SecondarySoundBuffer(DXSoundManager.Device, new SoundBufferDescription { Format = Format, BufferBytes = RawData.Length, Flags = flags })
            {
                Volume = Volume
            });

            buff.Write(RawData, 0, SharpDX.DirectSound.LockFlags.EntireBuffer);

            return buff;
        }
        public void DisposeSoundBuffer()
        {
            RawData = null;

            for (int i = BufferList.Count - 1; i >= 0; i--)
            {
                if (!BufferList[i].IsDisposed)
                    BufferList[i].Dispose();

                BufferList.RemoveAt(i);
            }

            DXManager.SoundList.Remove(this);
            ExpireTime = DateTime.MinValue;
        }

        public void SetVolume()
        {
            Volume = 100;

            for (int i = BufferList.Count - 1; i >= 0; i--)
            {
                if (BufferList[i].IsDisposed)
                {
                    BufferList.RemoveAt(i);
                    continue;
                }

                BufferList[i].Volume = Volume;
            }
        }

        public void UpdateFlags()
        {
            for (int i = BufferList.Count - 1; i >= 0; i--)
            {
                SecondarySoundBuffer buffer = CreateBuffer();

                BufferList[0].GetCurrentPosition(out var play, out var write);
                buffer.CurrentPosition = play;

                if (((BufferStatus)BufferList[0].Status & BufferStatus.Playing) == BufferStatus.Playing)
                    buffer.Play(0, Loop ? PlayFlags.Looping : PlayFlags.None);

                if (!BufferList[0].IsDisposed)
                    BufferList[0].Dispose();

                BufferList.RemoveAt(0);
            }

        }
    }

    public static class DXSoundManager
    {
        private const string SoundPath = @"Sound\";

        public static DirectSound Device;
        public static bool Error;

        #region SoundList

        public static Dictionary<string, DXSound> DiySoundList = new Dictionary<string, DXSound>();
        public static Dictionary<SoundIndex, DXSound> SoundList = new Dictionary<SoundIndex, DXSound>
        {
            #region Music
            [SoundIndex.LoginScene] = new DXSound(SoundPath + @"Opening.wav", SoundType.Music) { Loop = true },
            [SoundIndex.SelectScene] = new DXSound(SoundPath + @"SelChr.wav", SoundType.Music) { Loop = true },
            [SoundIndex.B000] = new DXSound(SoundPath + @"B000.wav", SoundType.Music) { Loop = true },
            [SoundIndex.B2] = new DXSound(SoundPath + @"B2.wav", SoundType.Music) { Loop = true },
            [SoundIndex.B8] = new DXSound(SoundPath + @"B8.wav", SoundType.Music) { Loop = true },
            [SoundIndex.B009D] = new DXSound(SoundPath + @"B009-D.wav", SoundType.Music) { Loop = true },
            [SoundIndex.B009N] = new DXSound(SoundPath + @"B009-N.wav", SoundType.Music) { Loop = true },
            [SoundIndex.B0014D] = new DXSound(SoundPath + @"B0014-D.wav", SoundType.Music) { Loop = true },
            [SoundIndex.B0014N] = new DXSound(SoundPath + @"B0014-N.wav", SoundType.Music) { Loop = true },
            [SoundIndex.B100] = new DXSound(SoundPath + @"B100.wav", SoundType.Music) { Loop = true },
            [SoundIndex.B122] = new DXSound(SoundPath + @"B122.wav", SoundType.Music) { Loop = true },
            [SoundIndex.B300] = new DXSound(SoundPath + @"B300.wav", SoundType.Music) { Loop = true },
            [SoundIndex.B400] = new DXSound(SoundPath + @"B400.wav", SoundType.Music) { Loop = true },
            [SoundIndex.B14001] = new DXSound(SoundPath + @"B14001.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD00] = new DXSound(SoundPath + @"BD00.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD01] = new DXSound(SoundPath + @"BD01.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD02] = new DXSound(SoundPath + @"BD02.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD041] = new DXSound(SoundPath + @"BD041.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD042] = new DXSound(SoundPath + @"BD042.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD50] = new DXSound(SoundPath + @"BD50.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD60] = new DXSound(SoundPath + @"BD60.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD70] = new DXSound(SoundPath + @"BD70.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD99] = new DXSound(SoundPath + @"BD99.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD100] = new DXSound(SoundPath + @"BD100.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD101] = new DXSound(SoundPath + @"BD101.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD210] = new DXSound(SoundPath + @"BD210.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BD211] = new DXSound(SoundPath + @"BD211.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BDUnderseaCave] = new DXSound(SoundPath + @"BDUnderseaCave.wav", SoundType.Music) { Loop = true },
            [SoundIndex.BDUnderseaCaveBoss] = new DXSound(SoundPath + @"BDUnderseaCaveBoss.wav", SoundType.Music) { Loop = true },
            [SoundIndex.D3101] = new DXSound(SoundPath + @"D3101.wav", SoundType.Music) { Loop = true },
            [SoundIndex.D3102] = new DXSound(SoundPath + @"D3102.wav", SoundType.Music) { Loop = true },
            [SoundIndex.D3400] = new DXSound(SoundPath + @"D3400.wav", SoundType.Music) { Loop = true },
            [SoundIndex.Dungeon_1] = new DXSound(SoundPath + @"Dungeon_1.wav", SoundType.Music) { Loop = true },
            [SoundIndex.Dungeon_2] = new DXSound(SoundPath + @"Dungeon_2.wav", SoundType.Music) { Loop = true },
            [SoundIndex.ID1_001] = new DXSound(SoundPath + @"ID1_001.wav", SoundType.Music) { Loop = true },
            [SoundIndex.ID1_002] = new DXSound(SoundPath + @"ID1_002.wav", SoundType.Music) { Loop = true },
            [SoundIndex.ID1_003] = new DXSound(SoundPath + @"ID1_003.wav", SoundType.Music) { Loop = true },
            [SoundIndex.TS001] = new DXSound(SoundPath + @"TS001.wav", SoundType.Music) { Loop = true },
            [SoundIndex.TS002] = new DXSound(SoundPath + @"TS002.wav", SoundType.Music) { Loop = true },
            [SoundIndex.TS003] = new DXSound(SoundPath + @"TS003.wav", SoundType.Music) { Loop = true },
            #endregion

            #region Players
            [SoundIndex.Foot1] = new DXSound(SoundPath + @"1.wav", SoundType.Player),
            [SoundIndex.Foot2] = new DXSound(SoundPath + @"2.wav", SoundType.Player),
            [SoundIndex.Foot3] = new DXSound(SoundPath + @"3.wav", SoundType.Player),
            [SoundIndex.Foot4] = new DXSound(SoundPath + @"4.wav", SoundType.Player),

            [SoundIndex.HorseWalk1] = new DXSound(SoundPath + @"33.wav", SoundType.Player),
            [SoundIndex.HorseWalk2] = new DXSound(SoundPath + @"34.wav", SoundType.Player),
            [SoundIndex.HorseRun] = new DXSound(SoundPath + @"35.wav", SoundType.Player),

            [SoundIndex.GenericStruckPlayer] = new DXSound(SoundPath + @"61.wav", SoundType.Player),

            [SoundIndex.DaggerSwing] = new DXSound(SoundPath + @"50.wav", SoundType.Player),
            [SoundIndex.WoodSwing] = new DXSound(SoundPath + @"51.wav", SoundType.Player),
            [SoundIndex.IronSwordSwing] = new DXSound(SoundPath + @"52.wav", SoundType.Player),
            [SoundIndex.ShortSwordSwing] = new DXSound(SoundPath + @"53.wav", SoundType.Player),
            [SoundIndex.AxeSwing] = new DXSound(SoundPath + @"54.wav", SoundType.Player),
            [SoundIndex.ClubSwing] = new DXSound(SoundPath + @"55.wav", SoundType.Player),
            [SoundIndex.WandSwing] = new DXSound(SoundPath + @"56.wav", SoundType.Player),
            [SoundIndex.FistSwing] = new DXSound(SoundPath + @"57.wav", SoundType.Player),
            [SoundIndex.GlaiveAttack] = new DXSound(SoundPath + @"63.wav", SoundType.Player),
            [SoundIndex.ClawAttack] = new DXSound(SoundPath + @"64.wav", SoundType.Player),

            [SoundIndex.MaleStruck] = new DXSound(SoundPath + @"138.wav", SoundType.Player),
            [SoundIndex.FemaleStruck] = new DXSound(SoundPath + @"139.wav", SoundType.Player),
            [SoundIndex.MaleDie] = new DXSound(SoundPath + @"144.wav", SoundType.Player),
            [SoundIndex.FemaleDie] = new DXSound(SoundPath + @"145.wav", SoundType.Player),

            #endregion

            #region System
            [SoundIndex.ButtonA] = new DXSound(SoundPath + @"103.wav", SoundType.System),
            [SoundIndex.ButtonB] = new DXSound(SoundPath + @"104.wav", SoundType.System),
            [SoundIndex.ButtonC] = new DXSound(SoundPath + @"105.wav", SoundType.System),

            [SoundIndex.SelectWarriorMale] = new DXSound(SoundPath + @"JMCre.wav", SoundType.System),
            [SoundIndex.SelectWarriorFemale] = new DXSound(SoundPath + @"JWCre.wav", SoundType.System),
            [SoundIndex.SelectWizardMale] = new DXSound(SoundPath + @"SMCre.wav", SoundType.System),
            [SoundIndex.SelectWizardFemale] = new DXSound(SoundPath + @"SWCre.wav", SoundType.System),
            [SoundIndex.SelectTaoistMale] = new DXSound(SoundPath + @"DMCre.wav", SoundType.System),
            [SoundIndex.SelectTaoistFemale] = new DXSound(SoundPath + @"DWCre.wav", SoundType.System),
            [SoundIndex.SelectAssassinMale] = new DXSound(SoundPath + @"AMCre.wav", SoundType.System),
            [SoundIndex.SelectAssassinFemale] = new DXSound(SoundPath + @"AWCre.wav", SoundType.System),

            [SoundIndex.TeleportIn] = new DXSound(SoundPath + @"109.wav", SoundType.System),
            [SoundIndex.TeleportOut] = new DXSound(SoundPath + @"110.wav", SoundType.System),

            [SoundIndex.ItemPotion] = new DXSound(SoundPath + @"108.wav", SoundType.System),
            [SoundIndex.ItemWeapon] = new DXSound(SoundPath + @"111.wav", SoundType.System),
            [SoundIndex.ItemArmour] = new DXSound(SoundPath + @"112.wav", SoundType.System),
            [SoundIndex.ItemRing] = new DXSound(SoundPath + @"113.wav", SoundType.System),
            [SoundIndex.ItemBracelet] = new DXSound(SoundPath + @"114.wav", SoundType.System),
            [SoundIndex.ItemNecklace] = new DXSound(SoundPath + @"115.wav", SoundType.System),
            [SoundIndex.ItemHelmet] = new DXSound(SoundPath + @"116.wav", SoundType.System),
            [SoundIndex.ItemShoes] = new DXSound(SoundPath + @"117.wav", SoundType.System),
            [SoundIndex.ItemDefault] = new DXSound(SoundPath + @"118.wav", SoundType.System),

            [SoundIndex.GoldPickUp] = new DXSound(SoundPath + @"120.wav", SoundType.System),
            [SoundIndex.GoldGained] = new DXSound(SoundPath + @"122.wav", SoundType.System),

            #endregion

            #region Magic

            [SoundIndex.SlayingMale] = new DXSound(SoundPath + @"M7-1.wav", SoundType.Magic),
            [SoundIndex.SlayingFemale] = new DXSound(SoundPath + @"M7-2.wav", SoundType.Magic),

            [SoundIndex.EnergyBlast] = new DXSound(SoundPath + @"M12-1.wav", SoundType.Magic),

            [SoundIndex.HalfMoon] = new DXSound(SoundPath + @"M25-1.wav", SoundType.Magic),

            [SoundIndex.FlamingSword] = new DXSound(SoundPath + @"M26-3.wav", SoundType.Magic),
            [SoundIndex.DragonRise] = new DXSound(SoundPath + @"M26-1.wav", SoundType.Magic),
            [SoundIndex.BladeStorm] = new DXSound(SoundPath + @"M34-1.wav", SoundType.Magic),

            [SoundIndex.DestructiveBlow] = new DXSound(SoundPath + @"M103-1.wav", SoundType.Magic),

            [SoundIndex.DefianceStart] = new DXSound(SoundPath + @"M106-3.wav", SoundType.Magic),

            [SoundIndex.AssaultStart] = new DXSound(SoundPath + @"M109-1.wav", SoundType.Magic),

            [SoundIndex.SwiftBladeEnd] = new DXSound(SoundPath + @"M131-2.wav", SoundType.Magic),

            [SoundIndex.FireBallStart] = new DXSound(SoundPath + @"M1-1.wav", SoundType.Magic),
            [SoundIndex.FireBallTravel] = new DXSound(SoundPath + @"M1-2.wav", SoundType.Magic),
            [SoundIndex.FireBallEnd] = new DXSound(SoundPath + @"M1-3.wav", SoundType.Magic),

            [SoundIndex.ThunderBoltStart] = new DXSound(SoundPath + @"M41-1.wav", SoundType.Magic),
            [SoundIndex.ThunderBoltTravel] = new DXSound(SoundPath + @"M41-2.wav", SoundType.Magic),
            [SoundIndex.ThunderBoltEnd] = new DXSound(SoundPath + @"M41-3.wav", SoundType.Magic),

            [SoundIndex.IceBoltStart] = new DXSound(SoundPath + @"M39-1.wav", SoundType.Magic),
            [SoundIndex.IceBoltTravel] = new DXSound(SoundPath + @"M39-2.wav", SoundType.Magic),
            [SoundIndex.IceBoltEnd] = new DXSound(SoundPath + @"M39-3.wav", SoundType.Magic),

            [SoundIndex.GustBlastStart] = new DXSound(SoundPath + @"M67-1.wav", SoundType.Magic),
            [SoundIndex.GustBlastTravel] = new DXSound(SoundPath + @"M67-2.wav", SoundType.Magic),
            [SoundIndex.GustBlastEnd] = new DXSound(SoundPath + @"M67-3.wav", SoundType.Magic),

            [SoundIndex.RepulsionEnd] = new DXSound(SoundPath + @"M8-2.wav", SoundType.Magic),

            [SoundIndex.ElectricShockStart] = new DXSound(SoundPath + @"M20-1.wav", SoundType.Magic),
            [SoundIndex.ElectricShockEnd] = new DXSound(SoundPath + @"M20-3.wav", SoundType.Magic),

            [SoundIndex.GreaterFireBallStart] = new DXSound(SoundPath + @"M5-1.wav", SoundType.Magic),
            [SoundIndex.GreaterFireBallTravel] = new DXSound(SoundPath + @"M5-2.wav", SoundType.Magic),
            [SoundIndex.GreaterFireBallEnd] = new DXSound(SoundPath + @"M5-3.wav", SoundType.Magic),

            [SoundIndex.LightningStrikeStart] = new DXSound(SoundPath + @"M11-1.wav", SoundType.Magic),
            [SoundIndex.LightningStrikeEnd] = new DXSound(SoundPath + @"M11-2.wav", SoundType.Magic),

            [SoundIndex.GreaterIceBoltStart] = new DXSound(SoundPath + @"M40-1.wav", SoundType.Magic),
            [SoundIndex.GreaterIceBoltTravel] = new DXSound(SoundPath + @"M40-2.wav", SoundType.Magic),
            [SoundIndex.GreaterIceBoltEnd] = new DXSound(SoundPath + @"M40-3.wav", SoundType.Magic),

            [SoundIndex.CycloneStart] = new DXSound(SoundPath + @"M74-1.wav", SoundType.Magic),
            [SoundIndex.CycloneEnd] = new DXSound(SoundPath + @"M74-3.wav", SoundType.Magic),

            [SoundIndex.LavaStrikeStart] = new DXSound(SoundPath + @"M9-1.wav", SoundType.Magic),
            //[SoundIndex.LavaStrikeEnd] = new DXSound(SoundPath + @"M9-3.wav", SoundType.Magic),

            [SoundIndex.LightningBeamEnd] = new DXSound(SoundPath + @"M10-1.wav", SoundType.Magic),

            [SoundIndex.TeleportationStart] = new DXSound(SoundPath + @"M21-1.wav", SoundType.Magic),

            [SoundIndex.FireWallStart] = new DXSound(SoundPath + @"M22-1.wav", SoundType.Magic),
            [SoundIndex.FireWallEnd] = new DXSound(SoundPath + @"M22-2.wav", SoundType.Magic),

            [SoundIndex.FireStormStart] = new DXSound(SoundPath + @"M23-1.wav", SoundType.Magic),
            [SoundIndex.FireStormEnd] = new DXSound(SoundPath + @"M23-3.wav", SoundType.Magic),

            [SoundIndex.LightningWaveStart] = new DXSound(SoundPath + @"M24-1.wav", SoundType.Magic),
            [SoundIndex.LightningWaveEnd] = new DXSound(SoundPath + @"M24-3.wav", SoundType.Magic),

            [SoundIndex.FrozenEarthStart] = new DXSound(SoundPath + @"M53-1.wav", SoundType.Magic),
            [SoundIndex.FrozenEarthEnd] = new DXSound(SoundPath + @"M53-3.wav", SoundType.Magic),

            [SoundIndex.BlowEarthStart] = new DXSound(SoundPath + @"M73-1.wav", SoundType.Magic),
            [SoundIndex.BlowEarthTravel] = new DXSound(SoundPath + @"M73-3.wav", SoundType.Magic),
            [SoundIndex.BlowEarthEnd] = new DXSound(SoundPath + @"M73-3.wav", SoundType.Magic),


            [SoundIndex.ExpelUndeadStart] = new DXSound(SoundPath + @"M32-1.wav", SoundType.Magic),
            [SoundIndex.ExpelUndeadStart] = new DXSound(SoundPath + @"M32-3.wav", SoundType.Magic),
            [SoundIndex.MagicShieldStart] = new DXSound(SoundPath + @"M31-1.wav", SoundType.Magic),

            [SoundIndex.IceStormStart] = new DXSound(SoundPath + @"M33-1.wav", SoundType.Magic),
            [SoundIndex.IceStormEnd] = new DXSound(SoundPath + @"M33-3.wav", SoundType.Magic),

            [SoundIndex.DragonTornadoStart] = new DXSound(SoundPath + @"M72-1.wav", SoundType.Magic),
            [SoundIndex.DragonTornadoEnd] = new DXSound(SoundPath + @"M72-3.wav", SoundType.Magic),

            [SoundIndex.GreaterFrozenEarthStart] = new DXSound(SoundPath + @"M53-1.wav", SoundType.Magic),
            [SoundIndex.GreaterFrozenEarthEnd] = new DXSound(SoundPath + @"M53-3.wav", SoundType.Magic),

            [SoundIndex.ChainLightningStart] = new DXSound(SoundPath + @"M111-1.wav", SoundType.Magic),
            [SoundIndex.ChainLightningEnd] = new DXSound(SoundPath + @"M111-3.wav", SoundType.Magic),

            [SoundIndex.FrostBiteStart] = new DXSound(SoundPath + @"m135-2.wav", SoundType.Magic),

            [SoundIndex.HealStart] = new DXSound(SoundPath + @"M2-1.wav", SoundType.Magic),
            [SoundIndex.HealEnd] = new DXSound(SoundPath + @"M2-3.wav", SoundType.Magic),

            [SoundIndex.PoisonDustStart] = new DXSound(SoundPath + @"M6-1.wav", SoundType.Magic),
            [SoundIndex.PoisonDustEnd] = new DXSound(SoundPath + @"M6-3.wav", SoundType.Magic),

            [SoundIndex.ExplosiveTalismanStart] = new DXSound(SoundPath + @"M13-1.wav", SoundType.Magic),
            [SoundIndex.ExplosiveTalismanTravel] = new DXSound(SoundPath + @"M13-2.wav", SoundType.Magic),
            [SoundIndex.ExplosiveTalismanEnd] = new DXSound(SoundPath + @"M13-3.wav", SoundType.Magic),

            [SoundIndex.HolyStrikeStart] = new DXSound(SoundPath + @"M37-1.wav", SoundType.Magic),
            [SoundIndex.HolyStrikeTravel] = new DXSound(SoundPath + @"M37-2.wav", SoundType.Magic),
            [SoundIndex.HolyStrikeEnd] = new DXSound(SoundPath + @"M37-3.wav", SoundType.Magic),

            [SoundIndex.ImprovedHolyStrikeStart] = new DXSound(SoundPath + @"M38-1.wav", SoundType.Magic),
            [SoundIndex.ImprovedHolyStrikeTravel] = new DXSound(SoundPath + @"M38-2.wav", SoundType.Magic),
            [SoundIndex.ImprovedHolyStrikeEnd] = new DXSound(SoundPath + @"M38-3.wav", SoundType.Magic),

            [SoundIndex.MagicResistanceTravel] = new DXSound(SoundPath + @"M14-2.wav", SoundType.Magic),
            [SoundIndex.MagicResistanceEnd] = new DXSound(SoundPath + @"M14-3.wav", SoundType.Magic),

            [SoundIndex.ResilienceTravel] = new DXSound(SoundPath + @"M15-2.wav", SoundType.Magic),
            [SoundIndex.ResilienceEnd] = new DXSound(SoundPath + @"M15-3.wav", SoundType.Magic),

            [SoundIndex.ShacklingTalismanStart] = new DXSound(SoundPath + @"M16-1.wav", SoundType.Magic),
            [SoundIndex.ShacklingTalismanEnd] = new DXSound(SoundPath + @"M16-3.wav", SoundType.Magic),

            [SoundIndex.SummonSkeletonStart] = new DXSound(SoundPath + @"M17-1.wav", SoundType.Magic),
            [SoundIndex.SummonSkeletonEnd] = new DXSound(SoundPath + @"M17-3.wav", SoundType.Magic),

            [SoundIndex.InvisibilityEnd] = new DXSound(SoundPath + @"M18-1.wav", SoundType.Magic),

            [SoundIndex.MassInvisibilityTravel] = new DXSound(SoundPath + @"M19-2.wav", SoundType.Magic),
            [SoundIndex.MassInvisibilityEnd] = new DXSound(SoundPath + @"M19-3.wav", SoundType.Magic),

            [SoundIndex.TaoistCombatKickStart] = new DXSound(SoundPath + @"M36-1.wav", SoundType.Magic),

            [SoundIndex.MassHealStart] = new DXSound(SoundPath + @"M29-1.wav", SoundType.Magic),
            [SoundIndex.MassHealEnd] = new DXSound(SoundPath + @"M29-3.wav", SoundType.Magic),

            [SoundIndex.BloodLustTravel] = new DXSound(SoundPath + @"M94-2.wav", SoundType.Magic),
            [SoundIndex.BloodLustEnd] = new DXSound(SoundPath + @"M94-3.wav", SoundType.Magic),

            [SoundIndex.ResurrectionStart] = new DXSound(SoundPath + @"M77-1.wav", SoundType.Magic),

            [SoundIndex.PurificationStart] = new DXSound(SoundPath + @"M120-1.wav", SoundType.Magic),
            [SoundIndex.PurificationEnd] = new DXSound(SoundPath + @"M120-3.wav", SoundType.Magic),

            [SoundIndex.SummonShinsuStart] = new DXSound(SoundPath + @"M30-1.wav", SoundType.Magic),
            [SoundIndex.SummonShinsuEnd] = new DXSound(SoundPath + @"M30-3.wav", SoundType.Magic),

            [SoundIndex.StrengthOfFaithStart] = new DXSound(SoundPath + @"M123-1.wav", SoundType.Magic),
            [SoundIndex.StrengthOfFaithEnd] = new DXSound(SoundPath + @"M123-3.wav", SoundType.Magic),

            [SoundIndex.PoisonousCloudStart] = new DXSound(SoundPath + @"as_157-1.wav", SoundType.Magic),

            [SoundIndex.CloakStart] = new DXSound(SoundPath + @"as_163.wav", SoundType.Magic),

            [SoundIndex.FullBloom] = new DXSound(SoundPath + @"as_165.wav", SoundType.Magic),
            [SoundIndex.WhiteLotus] = new DXSound(SoundPath + @"as_166.wav", SoundType.Magic),
            [SoundIndex.RedLotus] = new DXSound(SoundPath + @"as_167.wav", SoundType.Magic),

            [SoundIndex.SweetBrier] = new DXSound(SoundPath + @"as_168.wav", SoundType.Magic),
            [SoundIndex.SweetBrierMale] = new DXSound(SoundPath + @"as_168-m.wav", SoundType.Magic),
            [SoundIndex.SweetBrierFemale] = new DXSound(SoundPath + @"as_168-f.wav", SoundType.Magic),

            [SoundIndex.Karma] = new DXSound(SoundPath + @"as_172.wav", SoundType.Magic),
            [SoundIndex.TheNewBeginning] = new DXSound(SoundPath + @"as_174.wav", SoundType.Magic),

            [SoundIndex.SummonPuppet] = new DXSound(SoundPath + @"as_164-1.wav", SoundType.Magic),

            [SoundIndex.WraithGripStart] = new DXSound(SoundPath + @"as_159-1.wav", SoundType.Magic),
            [SoundIndex.HellFireStart] = new DXSound(SoundPath + @"as_160-2.wav", SoundType.Magic),

            [SoundIndex.AbyssStart] = new DXSound(SoundPath + @"M14-3.wav", SoundType.Magic),
            [SoundIndex.FlashOfLightEnd] = new DXSound(SoundPath + @"M123-3-1.wav", SoundType.Magic),

            [SoundIndex.RagingWindStart] = new DXSound(SoundPath + @"M26-1.wav", SoundType.Magic),
            [SoundIndex.EvasionStart] = new DXSound(SoundPath + @"243-5.wav", SoundType.Magic),

            #endregion

            #region Monsters
            [SoundIndex.GenericStruckMonster] = new DXSound(SoundPath + @"61.wav", SoundType.Monster),

            [SoundIndex.ChickenAttack] = new DXSound(SoundPath + @"220-2.wav", SoundType.Monster),
            [SoundIndex.ChickenStruck] = new DXSound(SoundPath + @"220-4.wav", SoundType.Monster),
            [SoundIndex.ChickenDie] = new DXSound(SoundPath + @"220-5.wav", SoundType.Monster),

            [SoundIndex.PigAttack] = new DXSound(SoundPath + @"300-2.wav", SoundType.Monster),
            [SoundIndex.PigStruck] = new DXSound(SoundPath + @"300-4.wav", SoundType.Monster),
            [SoundIndex.PigDie] = new DXSound(SoundPath + @"300-5.wav", SoundType.Monster),

            [SoundIndex.CowAttack] = new DXSound(SoundPath + @"301-2.wav", SoundType.Monster),
            [SoundIndex.CowStruck] = new DXSound(SoundPath + @"301-4.wav", SoundType.Monster),
            [SoundIndex.CowDie] = new DXSound(SoundPath + @"301-5.wav", SoundType.Monster),

            [SoundIndex.DeerAttack] = new DXSound(SoundPath + @"221-2.wav", SoundType.Monster),
            [SoundIndex.DeerStruck] = new DXSound(SoundPath + @"221-4.wav", SoundType.Monster),
            [SoundIndex.DeerDie] = new DXSound(SoundPath + @"221-5.wav", SoundType.Monster),

            [SoundIndex.SheepAttack] = new DXSound(SoundPath + @"258-2.wav", SoundType.Monster),
            [SoundIndex.SheepStruck] = new DXSound(SoundPath + @"258-4.wav", SoundType.Monster),
            [SoundIndex.SheepDie] = new DXSound(SoundPath + @"258-5.wav", SoundType.Monster),

            [SoundIndex.ClawCatAttack] = new DXSound(SoundPath + @"238-2.wav", SoundType.Monster),
            [SoundIndex.ClawCatStruck] = new DXSound(SoundPath + @"238-4.wav", SoundType.Monster),
            [SoundIndex.ClawCatDie] = new DXSound(SoundPath + @"238-5.wav", SoundType.Monster),

            [SoundIndex.WolfAttack] = new DXSound(SoundPath + @"265-2.wav", SoundType.Monster),
            [SoundIndex.WolfStruck] = new DXSound(SoundPath + @"265-4.wav", SoundType.Monster),
            [SoundIndex.WolfDie] = new DXSound(SoundPath + @"265-5.wav", SoundType.Monster),

            [SoundIndex.ForestYetiAttack] = new DXSound(SoundPath + @"230-2.wav", SoundType.Monster),
            [SoundIndex.ForestYetiStruck] = new DXSound(SoundPath + @"230-4.wav", SoundType.Monster),
            [SoundIndex.ForestYetiDie] = new DXSound(SoundPath + @"230-5.wav", SoundType.Monster),

            [SoundIndex.CarnivorousPlantAttack] = new DXSound(SoundPath + @"231-2.wav", SoundType.Monster),
            [SoundIndex.CarnivorousPlantStruck] = new DXSound(SoundPath + @"231-4.wav", SoundType.Monster),
            [SoundIndex.CarnivorousPlantDie] = new DXSound(SoundPath + @"231-5.wav", SoundType.Monster),

            [SoundIndex.OmaAttack] = new DXSound(SoundPath + @"223-2.wav", SoundType.Monster),
            [SoundIndex.OmaStruck] = new DXSound(SoundPath + @"223-4.wav", SoundType.Monster),
            [SoundIndex.OmaDie] = new DXSound(SoundPath + @"223-5.wav", SoundType.Monster),

            [SoundIndex.TigerSnakeAttack] = new DXSound(SoundPath + @"257-2.wav", SoundType.Monster),
            [SoundIndex.TigerSnakeStruck] = new DXSound(SoundPath + @"257-4.wav", SoundType.Monster),
            [SoundIndex.TigerSnakeDie] = new DXSound(SoundPath + @"257-5.wav", SoundType.Monster),

            [SoundIndex.SpittingSpiderAttack] = new DXSound(SoundPath + @"225-2.wav", SoundType.Monster),
            [SoundIndex.SpittingSpiderStruck] = new DXSound(SoundPath + @"225-4.wav", SoundType.Monster),
            [SoundIndex.SpittingSpiderDie] = new DXSound(SoundPath + @"225-5.wav", SoundType.Monster),

            [SoundIndex.ScarecrowAttack] = new DXSound(SoundPath + @"240-2.wav", SoundType.Monster),
            [SoundIndex.ScarecrowStruck] = new DXSound(SoundPath + @"240-4.wav", SoundType.Monster),
            [SoundIndex.ScarecrowDie] = new DXSound(SoundPath + @"240-5.wav", SoundType.Monster),

            [SoundIndex.OmaHeroAttack] = new DXSound(SoundPath + @"224-2.wav", SoundType.Monster),
            [SoundIndex.OmaHeroStruck] = new DXSound(SoundPath + @"224-4.wav", SoundType.Monster),
            [SoundIndex.OmaHeroDie] = new DXSound(SoundPath + @"224-5.wav", SoundType.Monster),

            [SoundIndex.CaveBatAttack] = new DXSound(SoundPath + @"229-2.wav", SoundType.Monster),
            [SoundIndex.CaveBatStruck] = new DXSound(SoundPath + @"229-4.wav", SoundType.Monster),
            [SoundIndex.CaveBatDie] = new DXSound(SoundPath + @"229-5.wav", SoundType.Monster),

            [SoundIndex.ScorpionAttack] = new DXSound(SoundPath + @"228-2.wav", SoundType.Monster),
            [SoundIndex.ScorpionStruck] = new DXSound(SoundPath + @"228-4.wav", SoundType.Monster),
            [SoundIndex.ScorpionDie] = new DXSound(SoundPath + @"228-5.wav", SoundType.Monster),

            [SoundIndex.SkeletonAttack] = new DXSound(SoundPath + @"232-2.wav", SoundType.Monster),
            [SoundIndex.SkeletonStruck] = new DXSound(SoundPath + @"232-4.wav", SoundType.Monster),
            [SoundIndex.SkeletonDie] = new DXSound(SoundPath + @"232-5.wav", SoundType.Monster),

            [SoundIndex.SkeletonAxeManAttack] = new DXSound(SoundPath + @"234-2.wav", SoundType.Monster),
            [SoundIndex.SkeletonAxeManStruck] = new DXSound(SoundPath + @"234-4.wav", SoundType.Monster),
            [SoundIndex.SkeletonAxeManDie] = new DXSound(SoundPath + @"234-5.wav", SoundType.Monster),

            [SoundIndex.SkeletonAxeThrowerAttack] = new DXSound(SoundPath + @"233-2.wav", SoundType.Monster),
            [SoundIndex.SkeletonAxeThrowerStruck] = new DXSound(SoundPath + @"233-4.wav", SoundType.Monster),
            [SoundIndex.SkeletonAxeThrowerDie] = new DXSound(SoundPath + @"233-5.wav", SoundType.Monster),

            [SoundIndex.SkeletonWarriorAttack] = new DXSound(SoundPath + @"235-2.wav", SoundType.Monster),
            [SoundIndex.SkeletonWarriorStruck] = new DXSound(SoundPath + @"235-4.wav", SoundType.Monster),
            [SoundIndex.SkeletonWarriorDie] = new DXSound(SoundPath + @"235-5.wav", SoundType.Monster),

            [SoundIndex.SkeletonLordAttack] = new DXSound(SoundPath + @"236-2.wav", SoundType.Monster),
            [SoundIndex.SkeletonLordStruck] = new DXSound(SoundPath + @"236-4.wav", SoundType.Monster),
            [SoundIndex.SkeletonLordDie] = new DXSound(SoundPath + @"236-5.wav", SoundType.Monster),


            [SoundIndex.CaveMaggotAttack] = new DXSound(SoundPath + @"237-2.wav", SoundType.Monster),
            [SoundIndex.CaveMaggotStruck] = new DXSound(SoundPath + @"237-4.wav", SoundType.Monster),
            [SoundIndex.CaveMaggotDie] = new DXSound(SoundPath + @"237-5.wav", SoundType.Monster),

            [SoundIndex.GhostSorcererAttack] = new DXSound(SoundPath + @"248-2.wav", SoundType.Monster),
            [SoundIndex.GhostSorcererStruck] = new DXSound(SoundPath + @"248-4.wav", SoundType.Monster),
            [SoundIndex.GhostSorcererDie] = new DXSound(SoundPath + @"248-5.wav", SoundType.Monster),

            [SoundIndex.GhostMageAppear] = new DXSound(SoundPath + @"249-0.wav", SoundType.Monster),
            [SoundIndex.GhostMageAttack] = new DXSound(SoundPath + @"249-2.wav", SoundType.Monster),
            [SoundIndex.GhostMageStruck] = new DXSound(SoundPath + @"249-4.wav", SoundType.Monster),
            [SoundIndex.GhostMageDie] = new DXSound(SoundPath + @"249-5.wav", SoundType.Monster),

            [SoundIndex.VoraciousGhostAttack] = new DXSound(SoundPath + @"252-2.wav", SoundType.Monster),
            [SoundIndex.VoraciousGhostStruck] = new DXSound(SoundPath + @"252-4.wav", SoundType.Monster),
            [SoundIndex.VoraciousGhostDie] = new DXSound(SoundPath + @"252-5.wav", SoundType.Monster),

            [SoundIndex.GhoulChampionAttack] = new DXSound(SoundPath + @"253-2.wav", SoundType.Monster),
            [SoundIndex.GhoulChampionStruck] = new DXSound(SoundPath + @"253-4.wav", SoundType.Monster),
            [SoundIndex.GhoulChampionDie] = new DXSound(SoundPath + @"253-5.wav", SoundType.Monster),

            [SoundIndex.ArmoredAntAttack] = new DXSound(SoundPath + @"214-2.wav", SoundType.Monster),
            [SoundIndex.ArmoredAntStruck] = new DXSound(SoundPath + @"214-4.wav", SoundType.Monster),
            [SoundIndex.ArmoredAntDie] = new DXSound(SoundPath + @"214-5.wav", SoundType.Monster),

            [SoundIndex.AntNeedlerAttack] = new DXSound(SoundPath + @"296-2.wav", SoundType.Monster),
            [SoundIndex.AntNeedlerStruck] = new DXSound(SoundPath + @"296-4.wav", SoundType.Monster),
            [SoundIndex.AntNeedlerDie] = new DXSound(SoundPath + @"296-5.wav", SoundType.Monster),

            [SoundIndex.ShellNipperAttack] = new DXSound(SoundPath + @"260-2.wav", SoundType.Monster),
            [SoundIndex.ShellNipperStruck] = new DXSound(SoundPath + @"260-4.wav", SoundType.Monster),
            [SoundIndex.ShellNipperDie] = new DXSound(SoundPath + @"260-5.wav", SoundType.Monster),

            [SoundIndex.VisceralWormAttack] = new DXSound(SoundPath + @"261-2.wav", SoundType.Monster),
            [SoundIndex.VisceralWormStruck] = new DXSound(SoundPath + @"261-4.wav", SoundType.Monster),
            [SoundIndex.VisceralWormDie] = new DXSound(SoundPath + @"261-5.wav", SoundType.Monster),

            [SoundIndex.KeratoidAttack] = new DXSound(SoundPath + @"263-2.wav", SoundType.Monster),
            [SoundIndex.KeratoidStruck] = new DXSound(SoundPath + @"263-4.wav", SoundType.Monster),
            [SoundIndex.KeratoidDie] = new DXSound(SoundPath + @"263-5.wav", SoundType.Monster),

            [SoundIndex.MutantFleaAttack] = new DXSound(SoundPath + @"325-2.wav", SoundType.Monster),
            [SoundIndex.MutantFleaStruck] = new DXSound(SoundPath + @"325-4.wav", SoundType.Monster),
            [SoundIndex.MutantFleaDie] = new DXSound(SoundPath + @"325-5.wav", SoundType.Monster),

            [SoundIndex.PoisonousMutantFleaAttack] = new DXSound(SoundPath + @"326-2.wav", SoundType.Monster),
            [SoundIndex.PoisonousMutantFleaStruck] = new DXSound(SoundPath + @"326-4.wav", SoundType.Monster),
            [SoundIndex.PoisonousMutantFleaDie] = new DXSound(SoundPath + @"326-5.wav", SoundType.Monster),

            [SoundIndex.BlasterMutantFleaAttack] = new DXSound(SoundPath + @"327-2.wav", SoundType.Monster),
            [SoundIndex.BlasterMutantFleaStruck] = new DXSound(SoundPath + @"327-4.wav", SoundType.Monster),
            [SoundIndex.BlasterMutantFleaDie] = new DXSound(SoundPath + @"327-5.wav", SoundType.Monster),


            [SoundIndex.WasHatchlingAttack] = new DXSound(SoundPath + @"271-2.wav", SoundType.Monster),
            [SoundIndex.WasHatchlingStruck] = new DXSound(SoundPath + @"271-4.wav", SoundType.Monster),
            [SoundIndex.WasHatchlingDie] = new DXSound(SoundPath + @"271-5.wav", SoundType.Monster),

            [SoundIndex.CentipedeAttack] = new DXSound(SoundPath + @"266-2.wav", SoundType.Monster),
            [SoundIndex.CentipedeStruck] = new DXSound(SoundPath + @"266-4.wav", SoundType.Monster),
            [SoundIndex.CentipedeDie] = new DXSound(SoundPath + @"266-5.wav", SoundType.Monster),

            [SoundIndex.ButterflyWormAttack] = new DXSound(SoundPath + @"272-2.wav", SoundType.Monster),
            [SoundIndex.ButterflyWormStruck] = new DXSound(SoundPath + @"272-4.wav", SoundType.Monster),
            [SoundIndex.ButterflyWormDie] = new DXSound(SoundPath + @"272-5.wav", SoundType.Monster),

            [SoundIndex.MutantMaggotAttack] = new DXSound(SoundPath + @"268-2.wav", SoundType.Monster),
            [SoundIndex.MutantMaggotStruck] = new DXSound(SoundPath + @"268-4.wav", SoundType.Monster),
            [SoundIndex.MutantMaggotDie] = new DXSound(SoundPath + @"268-5.wav", SoundType.Monster),

            [SoundIndex.EarwigAttack] = new DXSound(SoundPath + @"269-2.wav", SoundType.Monster),
            [SoundIndex.EarwigStruck] = new DXSound(SoundPath + @"269-4.wav", SoundType.Monster),
            [SoundIndex.EarwigDie] = new DXSound(SoundPath + @"269-5.wav", SoundType.Monster),

            [SoundIndex.IronLanceAttack] = new DXSound(SoundPath + @"270-2.wav", SoundType.Monster),
            [SoundIndex.IronLanceStruck] = new DXSound(SoundPath + @"270-4.wav", SoundType.Monster),
            [SoundIndex.IronLanceDie] = new DXSound(SoundPath + @"270-5.wav", SoundType.Monster),

            [SoundIndex.LordNiJaeAttack] = new DXSound(SoundPath + @"267-2.wav", SoundType.Monster),
            [SoundIndex.LordNiJaeStruck] = new DXSound(SoundPath + @"267-4.wav", SoundType.Monster),
            [SoundIndex.LordNiJaeDie] = new DXSound(SoundPath + @"267-5.wav", SoundType.Monster),

            [SoundIndex.RottingGhoulAttack] = new DXSound(SoundPath + @"318-2.wav", SoundType.Monster),
            [SoundIndex.RottingGhoulStruck] = new DXSound(SoundPath + @"318-4.wav", SoundType.Monster),
            [SoundIndex.RottingGhoulDie] = new DXSound(SoundPath + @"318-5.wav", SoundType.Monster),

            [SoundIndex.DecayingGhoulAttack] = new DXSound(SoundPath + @"312-2.wav", SoundType.Monster),
            [SoundIndex.DecayingGhoulStruck] = new DXSound(SoundPath + @"312-4.wav", SoundType.Monster),
            [SoundIndex.DecayingGhoulDie] = new DXSound(SoundPath + @"312-5.wav", SoundType.Monster),

            [SoundIndex.BloodThirstyGhoulAttack] = new DXSound(SoundPath + @"242-2.wav", SoundType.Monster),
            [SoundIndex.BloodThirstyGhoulStruck] = new DXSound(SoundPath + @"242-4.wav", SoundType.Monster),
            [SoundIndex.BloodThirstyGhoulDie] = new DXSound(SoundPath + @"242-5.wav", SoundType.Monster),


            [SoundIndex.SpinedDarkLizardAttack] = new DXSound(SoundPath + @"246-2.wav", SoundType.Monster),
            [SoundIndex.SpinedDarkLizardStruck] = new DXSound(SoundPath + @"246-4.wav", SoundType.Monster),
            [SoundIndex.SpinedDarkLizardDie] = new DXSound(SoundPath + @"246-5.wav", SoundType.Monster),

            [SoundIndex.UmaInfidelAttack] = new DXSound(SoundPath + @"242-2.wav", SoundType.Monster),
            [SoundIndex.UmaInfidelStruck] = new DXSound(SoundPath + @"242-4.wav", SoundType.Monster),
            [SoundIndex.UmaInfidelDie] = new DXSound(SoundPath + @"242-5.wav", SoundType.Monster),

            [SoundIndex.UmaFlameThrowerAttack] = new DXSound(SoundPath + @"243-2.wav", SoundType.Monster),
            [SoundIndex.UmaFlameThrowerStruck] = new DXSound(SoundPath + @"243-4.wav", SoundType.Monster),
            [SoundIndex.UmaFlameThrowerDie] = new DXSound(SoundPath + @"243-5.wav", SoundType.Monster),

            [SoundIndex.UmaAnguisherAttack] = new DXSound(SoundPath + @"244-2.wav", SoundType.Monster),
            [SoundIndex.UmaAnguisherStruck] = new DXSound(SoundPath + @"244-4.wav", SoundType.Monster),
            [SoundIndex.UmaAnguisherDie] = new DXSound(SoundPath + @"244-5.wav", SoundType.Monster),

            [SoundIndex.UmaKingAttack] = new DXSound(SoundPath + @"245-2.wav", SoundType.Monster),
            [SoundIndex.UmaKingStruck] = new DXSound(SoundPath + @"245-4.wav", SoundType.Monster),
            [SoundIndex.UmaKingDie] = new DXSound(SoundPath + @"245-5.wav", SoundType.Monster),


            [SoundIndex.SpiderBatAttack] = new DXSound(SoundPath + @"297-2.wav", SoundType.Monster),
            [SoundIndex.SpiderBatStruck] = new DXSound(SoundPath + @"297-4.wav", SoundType.Monster),
            [SoundIndex.SpiderBatDie] = new DXSound(SoundPath + @"297-5.wav", SoundType.Monster),

            [SoundIndex.ArachnidGazerStruck] = new DXSound(SoundPath + @"304-4.wav", SoundType.Monster),
            [SoundIndex.ArachnidGazerDie] = new DXSound(SoundPath + @"304-5.wav", SoundType.Monster),

            [SoundIndex.LarvaAttack] = new DXSound(SoundPath + @"303-2.wav", SoundType.Monster),
            [SoundIndex.LarvaStruck] = new DXSound(SoundPath + @"303-4.wav", SoundType.Monster),

            [SoundIndex.RedMoonGuardianAttack] = new DXSound(SoundPath + @"305-2.wav", SoundType.Monster),
            [SoundIndex.RedMoonGuardianStruck] = new DXSound(SoundPath + @"305-4.wav", SoundType.Monster),
            [SoundIndex.RedMoonGuardianDie] = new DXSound(SoundPath + @"305-5.wav", SoundType.Monster),

            [SoundIndex.RedMoonProtectorAttack] = new DXSound(SoundPath + @"306-2.wav", SoundType.Monster),
            [SoundIndex.RedMoonProtectorStruck] = new DXSound(SoundPath + @"306-4.wav", SoundType.Monster),
            [SoundIndex.RedMoonProtectorDie] = new DXSound(SoundPath + @"306-5.wav", SoundType.Monster),

            [SoundIndex.VenomousArachnidAttack] = new DXSound(SoundPath + @"307-2.wav", SoundType.Monster),
            [SoundIndex.VenomousArachnidStruck] = new DXSound(SoundPath + @"307-4.wav", SoundType.Monster),
            [SoundIndex.VenomousArachnidDie] = new DXSound(SoundPath + @"307-5.wav", SoundType.Monster),

            [SoundIndex.DarkArachnidAttack] = new DXSound(SoundPath + @"308-2.wav", SoundType.Monster),
            [SoundIndex.DarkArachnidStruck] = new DXSound(SoundPath + @"308-4.wav", SoundType.Monster),
            [SoundIndex.DarkArachnidDie] = new DXSound(SoundPath + @"308-5.wav", SoundType.Monster),

            [SoundIndex.RedMoonTheFallenAttack] = new DXSound(SoundPath + @"302-2.wav", SoundType.Monster),
            [SoundIndex.RedMoonTheFallenStruck] = new DXSound(SoundPath + @"302-4.wav", SoundType.Monster),
            [SoundIndex.RedMoonTheFallenDie] = new DXSound(SoundPath + @"302-5.wav", SoundType.Monster),


            [SoundIndex.ViciousRatAttack] = new DXSound(SoundPath + @"281-2.wav", SoundType.Monster),
            [SoundIndex.ViciousRatStruck] = new DXSound(SoundPath + @"281-4.wav", SoundType.Monster),
            [SoundIndex.ViciousRatDie] = new DXSound(SoundPath + @"281-5.wav", SoundType.Monster),

            [SoundIndex.ZumaSharpShooterAttack] = new DXSound(SoundPath + @"282-2.wav", SoundType.Monster),
            [SoundIndex.ZumaSharpShooterStruck] = new DXSound(SoundPath + @"282-4.wav", SoundType.Monster),
            [SoundIndex.ZumaSharpShooterDie] = new DXSound(SoundPath + @"282-5.wav", SoundType.Monster),

            [SoundIndex.ZumaFanaticAttack] = new DXSound(SoundPath + @"283-2.wav", SoundType.Monster),
            [SoundIndex.ZumaFanaticStruck] = new DXSound(SoundPath + @"283-4.wav", SoundType.Monster),
            [SoundIndex.ZumaFanaticDie] = new DXSound(SoundPath + @"283-5.wav", SoundType.Monster),

            [SoundIndex.ZumaGuardianAttack] = new DXSound(SoundPath + @"284-2.wav", SoundType.Monster),
            [SoundIndex.ZumaGuardianStruck] = new DXSound(SoundPath + @"284-4.wav", SoundType.Monster),
            [SoundIndex.ZumaGuardianDie] = new DXSound(SoundPath + @"284-5.wav", SoundType.Monster),

            [SoundIndex.ZumaKingAppear] = new DXSound(SoundPath + @"285-0.wav", SoundType.Monster),
            [SoundIndex.ZumaKingAttack] = new DXSound(SoundPath + @"285-2.wav", SoundType.Monster),
            [SoundIndex.ZumaKingStruck] = new DXSound(SoundPath + @"285-4.wav", SoundType.Monster),
            [SoundIndex.ZumaKingDie] = new DXSound(SoundPath + @"285-5.wav", SoundType.Monster),


            [SoundIndex.EvilFanaticAttack] = new DXSound(SoundPath + @"335-2.wav", SoundType.Monster),
            [SoundIndex.EvilFanaticStruck] = new DXSound(SoundPath + @"335-4.wav", SoundType.Monster),
            [SoundIndex.EvilFanaticDie] = new DXSound(SoundPath + @"335-5.wav", SoundType.Monster),

            [SoundIndex.MonkeyAttack] = new DXSound(SoundPath + @"332-2.wav", SoundType.Monster),
            [SoundIndex.MonkeyStruck] = new DXSound(SoundPath + @"332-4.wav", SoundType.Monster),
            [SoundIndex.MonkeyDie] = new DXSound(SoundPath + @"332-5.wav", SoundType.Monster),

            [SoundIndex.EvilElephantAttack] = new DXSound(SoundPath + @"336-2.wav", SoundType.Monster),
            [SoundIndex.EvilElephantStruck] = new DXSound(SoundPath + @"336-4.wav", SoundType.Monster),
            [SoundIndex.EvilElephantDie] = new DXSound(SoundPath + @"336-5.wav", SoundType.Monster),

            [SoundIndex.CannibalFanaticAttack] = new DXSound(SoundPath + @"334-2.wav", SoundType.Monster),
            [SoundIndex.CannibalFanaticStruck] = new DXSound(SoundPath + @"334-4.wav", SoundType.Monster),
            [SoundIndex.CannibalFanaticDie] = new DXSound(SoundPath + @"334-5.wav", SoundType.Monster),


            [SoundIndex.SpikedBeetleAttack] = new DXSound(SoundPath + @"264-2.wav", SoundType.Monster),
            [SoundIndex.SpikedBeetleStruck] = new DXSound(SoundPath + @"264-4.wav", SoundType.Monster),
            [SoundIndex.SpikedBeetleDie] = new DXSound(SoundPath + @"264-5.wav", SoundType.Monster),

            [SoundIndex.NumaGruntAttack] = new DXSound(SoundPath + @"309-2.wav", SoundType.Monster),
            [SoundIndex.NumaGruntStruck] = new DXSound(SoundPath + @"309-4.wav", SoundType.Monster),
            [SoundIndex.NumaGruntDie] = new DXSound(SoundPath + @"309-5.wav", SoundType.Monster),

            [SoundIndex.NumaMageAttack] = new DXSound(SoundPath + @"213-2.wav", SoundType.Monster),
            [SoundIndex.NumaMageStruck] = new DXSound(SoundPath + @"213-4.wav", SoundType.Monster),
            [SoundIndex.NumaMageDie] = new DXSound(SoundPath + @"213-5.wav", SoundType.Monster),

            [SoundIndex.NumaEliteAttack] = new DXSound(SoundPath + @"217-2.wav", SoundType.Monster),
            [SoundIndex.NumaEliteStruck] = new DXSound(SoundPath + @"217-4.wav", SoundType.Monster),
            [SoundIndex.NumaEliteDie] = new DXSound(SoundPath + @"217-5.wav", SoundType.Monster),

            [SoundIndex.SandSharkAttack] = new DXSound(SoundPath + @"342-2.wav", SoundType.Monster),
            [SoundIndex.SandSharkStruck] = new DXSound(SoundPath + @"342-4.wav", SoundType.Monster),
            [SoundIndex.SandSharkDie] = new DXSound(SoundPath + @"342-5.wav", SoundType.Monster),

            [SoundIndex.StoneGolemAppear] = new DXSound(SoundPath + @"204-0.wav", SoundType.Monster),
            [SoundIndex.StoneGolemAttack] = new DXSound(SoundPath + @"204-2.wav", SoundType.Monster),
            [SoundIndex.StoneGolemStruck] = new DXSound(SoundPath + @"204-4.wav", SoundType.Monster),
            [SoundIndex.StoneGolemDie] = new DXSound(SoundPath + @"204-5.wav", SoundType.Monster),

            [SoundIndex.WindfurySorceressAttack] = new DXSound(SoundPath + @"344-2.wav", SoundType.Monster),
            [SoundIndex.WindfurySorceressStruck] = new DXSound(SoundPath + @"344-4.wav", SoundType.Monster),
            [SoundIndex.WindfurySorceressDie] = new DXSound(SoundPath + @"344-5.wav", SoundType.Monster),

            [SoundIndex.CursedCactusAttack] = new DXSound(SoundPath + @"294-2.wav", SoundType.Monster),
            [SoundIndex.CursedCactusStruck] = new DXSound(SoundPath + @"294-4.wav", SoundType.Monster),
            [SoundIndex.CursedCactusDie] = new DXSound(SoundPath + @"294-5.wav", SoundType.Monster),

            [SoundIndex.RagingLizardAttack] = new DXSound(SoundPath + @"233-2.wav", SoundType.Monster),
            [SoundIndex.RagingLizardStruck] = new DXSound(SoundPath + @"233-4.wav", SoundType.Monster),
            [SoundIndex.RagingLizardDie] = new DXSound(SoundPath + @"233-5.wav", SoundType.Monster),

            [SoundIndex.SawToothLizardAttack] = new DXSound(SoundPath + @"212-2.wav", SoundType.Monster),
            [SoundIndex.SawToothLizardStruck] = new DXSound(SoundPath + @"212-4.wav", SoundType.Monster),
            [SoundIndex.SawToothLizardDie] = new DXSound(SoundPath + @"212-5.wav", SoundType.Monster),

            [SoundIndex.MutantLizardAttack] = new DXSound(SoundPath + @"234-2.wav", SoundType.Monster),
            [SoundIndex.MutantLizardStruck] = new DXSound(SoundPath + @"234-4.wav", SoundType.Monster),
            [SoundIndex.MutantLizardDie] = new DXSound(SoundPath + @"234-5.wav", SoundType.Monster),

            [SoundIndex.VenomSpitterAttack] = new DXSound(SoundPath + @"235-2.wav", SoundType.Monster),
            [SoundIndex.VenomSpitterStruck] = new DXSound(SoundPath + @"235-4.wav", SoundType.Monster),
            [SoundIndex.VenomSpitterDie] = new DXSound(SoundPath + @"235-5.wav", SoundType.Monster),

            [SoundIndex.SonicLizardAttack] = new DXSound(SoundPath + @"325-2.wav", SoundType.Monster),
            [SoundIndex.SonicLizardStruck] = new DXSound(SoundPath + @"325-4.wav", SoundType.Monster),
            [SoundIndex.SonicLizardDie] = new DXSound(SoundPath + @"325-5.wav", SoundType.Monster),

            [SoundIndex.GiantLizardAttack] = new DXSound(SoundPath + @"244-2.wav", SoundType.Monster),
            [SoundIndex.GiantLizardStruck] = new DXSound(SoundPath + @"244-4.wav", SoundType.Monster),
            [SoundIndex.GiantLizardDie] = new DXSound(SoundPath + @"244-5.wav", SoundType.Monster),

            [SoundIndex.CrazedLizardAttack] = new DXSound(SoundPath + @"335-2.wav", SoundType.Monster),
            [SoundIndex.CrazedLizardStruck] = new DXSound(SoundPath + @"335-4.wav", SoundType.Monster),
            [SoundIndex.CrazedLizardDie] = new DXSound(SoundPath + @"335-5.wav", SoundType.Monster),

            [SoundIndex.TaintedTerrorAttack] = new DXSound(SoundPath + @"361-2.wav", SoundType.Monster),
            [SoundIndex.TaintedTerrorStruck] = new DXSound(SoundPath + @"361-4.wav", SoundType.Monster),
            [SoundIndex.TaintedTerrorDie] = new DXSound(SoundPath + @"361-5.wav", SoundType.Monster),
            [SoundIndex.TaintedTerrorAttack2] = new DXSound(SoundPath + @"361-8.wav", SoundType.Monster),

            [SoundIndex.DeathLordJichonAttack] = new DXSound(SoundPath + @"362-2.wav", SoundType.Monster),
            [SoundIndex.DeathLordJichonStruck] = new DXSound(SoundPath + @"362-4.wav", SoundType.Monster),
            [SoundIndex.DeathLordJichonDie] = new DXSound(SoundPath + @"362-5.wav", SoundType.Monster),
            [SoundIndex.DeathLordJichonAttack2] = new DXSound(SoundPath + @"M25-1.wav", SoundType.Monster),
            [SoundIndex.DeathLordJichonAttack3] = new DXSound(SoundPath + @"362-8.wav", SoundType.Monster),

            [SoundIndex.MinotaurAttack] = new DXSound(SoundPath + @"317-2.wav", SoundType.Monster),
            [SoundIndex.MinotaurStruck] = new DXSound(SoundPath + @"317-4.wav", SoundType.Monster),
            [SoundIndex.MinotaurDie] = new DXSound(SoundPath + @"317-5.wav", SoundType.Monster),

            [SoundIndex.FrostMinotaurAttack] = new DXSound(SoundPath + @"314-2.wav", SoundType.Monster),
            [SoundIndex.FrostMinotaurStruck] = new DXSound(SoundPath + @"314-4.wav", SoundType.Monster),
            [SoundIndex.FrostMinotaurDie] = new DXSound(SoundPath + @"314-5.wav", SoundType.Monster),

            [SoundIndex.BanyaLeftGuardAttack] = new DXSound(SoundPath + @"310-2.wav", SoundType.Monster),
            [SoundIndex.BanyaLeftGuardStruck] = new DXSound(SoundPath + @"310-4.wav", SoundType.Monster),
            [SoundIndex.BanyaLeftGuardDie] = new DXSound(SoundPath + @"310-5.wav", SoundType.Monster),

            [SoundIndex.EmperorSaWooAttack] = new DXSound(SoundPath + @"335-2.wav", SoundType.Monster),
            [SoundIndex.EmperorSaWooStruck] = new DXSound(SoundPath + @"335-4.wav", SoundType.Monster),
            [SoundIndex.EmperorSaWooDie] = new DXSound(SoundPath + @"335-5.wav", SoundType.Monster),

            [SoundIndex.BoneArcherAttack] = new DXSound(SoundPath + @"322-2.wav", SoundType.Monster),
            [SoundIndex.BoneArcherStruck] = new DXSound(SoundPath + @"322-4.wav", SoundType.Monster),
            [SoundIndex.BoneArcherDie] = new DXSound(SoundPath + @"322-5.wav", SoundType.Monster),

            [SoundIndex.BoneCaptainAttack] = new DXSound(SoundPath + @"320-2.wav", SoundType.Monster),
            [SoundIndex.BoneCaptainStruck] = new DXSound(SoundPath + @"320-4.wav", SoundType.Monster),
            [SoundIndex.BoneCaptainDie] = new DXSound(SoundPath + @"320-5.wav", SoundType.Monster),

            [SoundIndex.ArchLichTaeduAttack] = new DXSound(SoundPath + @"321-2.wav", SoundType.Monster),
            [SoundIndex.ArchLichTaeduStruck] = new DXSound(SoundPath + @"321-4.wav", SoundType.Monster),
            [SoundIndex.ArchLichTaeduDie] = new DXSound(SoundPath + @"321-5.wav", SoundType.Monster),

            [SoundIndex.WedgeMothLarvaAttack] = new DXSound(SoundPath + @"273-2.wav", SoundType.Monster),
            [SoundIndex.WedgeMothLarvaStruck] = new DXSound(SoundPath + @"273-4.wav", SoundType.Monster),
            [SoundIndex.WedgeMothLarvaDie] = new DXSound(SoundPath + @"273-5.wav", SoundType.Monster),

            [SoundIndex.LesserWedgeMothAttack] = new DXSound(SoundPath + @"274-2.wav", SoundType.Monster),
            [SoundIndex.LesserWedgeMothStruck] = new DXSound(SoundPath + @"274-4.wav", SoundType.Monster),
            [SoundIndex.LesserWedgeMothDie] = new DXSound(SoundPath + @"274-5.wav", SoundType.Monster),

            [SoundIndex.WedgeMothAttack] = new DXSound(SoundPath + @"275-2.wav", SoundType.Monster),
            [SoundIndex.WedgeMothStruck] = new DXSound(SoundPath + @"275-4.wav", SoundType.Monster),
            [SoundIndex.WedgeMothDie] = new DXSound(SoundPath + @"275-5.wav", SoundType.Monster),

            [SoundIndex.RedBoarAttack] = new DXSound(SoundPath + @"277-2.wav", SoundType.Monster),
            [SoundIndex.RedBoarStruck] = new DXSound(SoundPath + @"277-4.wav", SoundType.Monster),
            [SoundIndex.RedBoarDie] = new DXSound(SoundPath + @"277-5.wav", SoundType.Monster),

            [SoundIndex.ClawSerpentAttack] = new DXSound(SoundPath + @"279-2.wav", SoundType.Monster),
            [SoundIndex.ClawSerpentStruck] = new DXSound(SoundPath + @"279-4.wav", SoundType.Monster),
            [SoundIndex.ClawSerpentDie] = new DXSound(SoundPath + @"279-5.wav", SoundType.Monster),

            [SoundIndex.BlackBoarAttack] = new DXSound(SoundPath + @"277-2.wav", SoundType.Monster),
            [SoundIndex.BlackBoarStruck] = new DXSound(SoundPath + @"277-4.wav", SoundType.Monster),
            [SoundIndex.BlackBoarDie] = new DXSound(SoundPath + @"277-5.wav", SoundType.Monster),

            [SoundIndex.TuskLordAttack] = new DXSound(SoundPath + @"277-2.wav", SoundType.Monster),
            [SoundIndex.TuskLordStruck] = new DXSound(SoundPath + @"277-4.wav", SoundType.Monster),
            [SoundIndex.TuskLordDie] = new DXSound(SoundPath + @"277-5.wav", SoundType.Monster),

            [SoundIndex.RazorTuskAttack] = new DXSound(SoundPath + @"328-2.wav", SoundType.Monster),
            [SoundIndex.RazorTuskStruck] = new DXSound(SoundPath + @"328-4.wav", SoundType.Monster),
            [SoundIndex.RazorTuskDie] = new DXSound(SoundPath + @"328-5.wav", SoundType.Monster),

            [SoundIndex.PinkGoddessAttack] = new DXSound(SoundPath + @"340-2.wav", SoundType.Monster),
            [SoundIndex.PinkGoddessStruck] = new DXSound(SoundPath + @"340-4.wav", SoundType.Monster),
            [SoundIndex.PinkGoddessDie] = new DXSound(SoundPath + @"340-5.wav", SoundType.Monster),

            [SoundIndex.GreenGoddessAttack] = new DXSound(SoundPath + @"340-2.wav", SoundType.Monster),
            [SoundIndex.GreenGoddessStruck] = new DXSound(SoundPath + @"340-4.wav", SoundType.Monster),
            [SoundIndex.GreenGoddessDie] = new DXSound(SoundPath + @"340-5.wav", SoundType.Monster),

            [SoundIndex.MutantCaptainAttack] = new DXSound(SoundPath + @"339-2.wav", SoundType.Monster),
            [SoundIndex.MutantCaptainStruck] = new DXSound(SoundPath + @"339-4.wav", SoundType.Monster),
            [SoundIndex.MutantCaptainDie] = new DXSound(SoundPath + @"339-5.wav", SoundType.Monster),

            [SoundIndex.StoneGriffinAttack] = new DXSound(SoundPath + @"337-2.wav", SoundType.Monster),
            [SoundIndex.StoneGriffinStruck] = new DXSound(SoundPath + @"337-4.wav", SoundType.Monster),
            [SoundIndex.StoneGriffinDie] = new DXSound(SoundPath + @"337-5.wav", SoundType.Monster),

            [SoundIndex.FlameGriffinAttack] = new DXSound(SoundPath + @"337-2.wav", SoundType.Monster),
            [SoundIndex.FlameGriffinStruck] = new DXSound(SoundPath + @"337-4.wav", SoundType.Monster),
            [SoundIndex.FlameGriffinDie] = new DXSound(SoundPath + @"337-5.wav", SoundType.Monster),

            [SoundIndex.WhiteBoneAttack] = new DXSound(SoundPath + @"63.wav", SoundType.Monster),
            [SoundIndex.WhiteBoneStruck] = new DXSound(SoundPath + @"232-4.wav", SoundType.Monster),
            [SoundIndex.WhiteBoneDie] = new DXSound(SoundPath + @"256-5.wav", SoundType.Monster),

            [SoundIndex.ShinsuSmallStruck] = new DXSound(SoundPath + @"289-4.wav", SoundType.Monster),
            [SoundIndex.ShinsuSmallDie] = new DXSound(SoundPath + @"289-5.wav", SoundType.Monster),

            [SoundIndex.ShinsuBigAttack] = new DXSound(SoundPath + @"290-2.wav", SoundType.Monster),
            [SoundIndex.ShinsuBigStruck] = new DXSound(SoundPath + @"290-4.wav", SoundType.Monster),
            [SoundIndex.ShinsuBigDie] = new DXSound(SoundPath + @"290-5.wav", SoundType.Monster),

            [SoundIndex.ShinsuShow] = new DXSound(SoundPath + @"290-0.wav", SoundType.Monster),

            [SoundIndex.CorpseStalkerAttack] = new DXSound(SoundPath + @"212-2.wav", SoundType.Monster),
            [SoundIndex.CorpseStalkerStruck] = new DXSound(SoundPath + @"212-4.wav", SoundType.Monster),
            [SoundIndex.CorpseStalkerDie] = new DXSound(SoundPath + @"212-5.wav", SoundType.Monster),

            [SoundIndex.LightArmedSoldierAttack] = new DXSound(SoundPath + @"206-2.wav", SoundType.Monster),
            [SoundIndex.LightArmedSoldierStruck] = new DXSound(SoundPath + @"206-4.wav", SoundType.Monster),
            [SoundIndex.LightArmedSoldierDie] = new DXSound(SoundPath + @"206-5.wav", SoundType.Monster),

            [SoundIndex.CorrosivePoisonSpitterAttack] = new DXSound(SoundPath + @"293-2.wav", SoundType.Monster),
            [SoundIndex.CorrosivePoisonSpitterStruck] = new DXSound(SoundPath + @"293-4.wav", SoundType.Monster),
            [SoundIndex.CorrosivePoisonSpitterDie] = new DXSound(SoundPath + @"293-5.wav", SoundType.Monster),

            [SoundIndex.PhantomSoldierAttack] = new DXSound(SoundPath + @"347-2.wav", SoundType.Monster),
            [SoundIndex.PhantomSoldierStruck] = new DXSound(SoundPath + @"347-4.wav", SoundType.Monster),
            [SoundIndex.PhantomSoldierDie] = new DXSound(SoundPath + @"347-5.wav", SoundType.Monster),

            [SoundIndex.MutatedOctopusAttack] = new DXSound(SoundPath + @"202-2.wav", SoundType.Monster),
            [SoundIndex.MutatedOctopusStruck] = new DXSound(SoundPath + @"202-4.wav", SoundType.Monster),
            [SoundIndex.MutatedOctopusDie] = new DXSound(SoundPath + @"202-5.wav", SoundType.Monster),

            [SoundIndex.AquaLizardAttack] = new DXSound(SoundPath + @"345-2.wav", SoundType.Monster),
            [SoundIndex.AquaLizardStruck] = new DXSound(SoundPath + @"345-4.wav", SoundType.Monster),
            [SoundIndex.AquaLizardDie] = new DXSound(SoundPath + @"345-5.wav", SoundType.Monster),

            [SoundIndex.CrimsonNecromancerAttack] = new DXSound(SoundPath + @"346-2.wav", SoundType.Monster),
            [SoundIndex.CrimsonNecromancerStruck] = new DXSound(SoundPath + @"346-4.wav", SoundType.Monster),
            [SoundIndex.CrimsonNecromancerDie] = new DXSound(SoundPath + @"346-5.wav", SoundType.Monster),

            [SoundIndex.ChaosKnightAttack] = new DXSound(SoundPath + @"210-2.wav", SoundType.Monster),
            [SoundIndex.ChaosKnightDie] = new DXSound(SoundPath + @"210-5.wav", SoundType.Monster),

            [SoundIndex.PachontheChaosbringerAttack] = new DXSound(SoundPath + @"343-2.wav", SoundType.Monster),
            [SoundIndex.PachontheChaosbringerStruck] = new DXSound(SoundPath + @"343-4.wav", SoundType.Monster),
            [SoundIndex.PachontheChaosbringerDie] = new DXSound(SoundPath + @"343-5.wav", SoundType.Monster),


            [SoundIndex.NumaCavalryAttack] = new DXSound(SoundPath + @"355-2.wav", SoundType.Monster),
            [SoundIndex.NumaCavalryStruck] = new DXSound(SoundPath + @"355-4.wav", SoundType.Monster),
            [SoundIndex.NumaCavalryDie] = new DXSound(SoundPath + @"355-5.wav", SoundType.Monster),

            [SoundIndex.NumaHighMageAttack] = new DXSound(SoundPath + @"359-2.wav", SoundType.Monster),
            [SoundIndex.NumaHighMageStruck] = new DXSound(SoundPath + @"359-4.wav", SoundType.Monster),
            [SoundIndex.NumaHighMageDie] = new DXSound(SoundPath + @"359-5.wav", SoundType.Monster),

            [SoundIndex.NumaStoneThrowerAttack] = new DXSound(SoundPath + @"358-2.wav", SoundType.Monster),
            [SoundIndex.NumaStoneThrowerStruck] = new DXSound(SoundPath + @"358-4.wav", SoundType.Monster),
            [SoundIndex.NumaStoneThrowerDie] = new DXSound(SoundPath + @"358-5.wav", SoundType.Monster),

            [SoundIndex.NumaRoyalGuardAttack] = new DXSound(SoundPath + @"357-2.wav", SoundType.Monster),
            [SoundIndex.NumaRoyalGuardStruck] = new DXSound(SoundPath + @"357-4.wav", SoundType.Monster),
            [SoundIndex.NumaRoyalGuardDie] = new DXSound(SoundPath + @"357-5.wav", SoundType.Monster),

            [SoundIndex.NumaArmoredSoldierAttack] = new DXSound(SoundPath + @"356-2.wav", SoundType.Monster),
            [SoundIndex.NumaArmoredSoldierStruck] = new DXSound(SoundPath + @"356-4.wav", SoundType.Monster),
            [SoundIndex.NumaArmoredSoldierDie] = new DXSound(SoundPath + @"356-5.wav", SoundType.Monster),

            [SoundIndex.IcyRangerAttack] = new DXSound(SoundPath + @"375-2.wav", SoundType.Monster),
            [SoundIndex.IcyRangerStruck] = new DXSound(SoundPath + @"375-4.wav", SoundType.Monster),
            [SoundIndex.IcyRangerDie] = new DXSound(SoundPath + @"375-5.wav", SoundType.Monster),

            [SoundIndex.IcyGoddessAttack] = new DXSound(SoundPath + @"374-2.wav", SoundType.Monster),
            [SoundIndex.IcyGoddessStruck] = new DXSound(SoundPath + @"374-4.wav", SoundType.Monster),
            [SoundIndex.IcyGoddessDie] = new DXSound(SoundPath + @"374-5.wav", SoundType.Monster),

            [SoundIndex.IcySpiritWarriorAttack] = new DXSound(SoundPath + @"378-2.wav", SoundType.Monster),
            [SoundIndex.IcySpiritWarriorStruck] = new DXSound(SoundPath + @"378-4.wav", SoundType.Monster),
            [SoundIndex.IcySpiritWarriorDie] = new DXSound(SoundPath + @"378-5.wav", SoundType.Monster),

            [SoundIndex.GhostKnightAttack] = new DXSound(SoundPath + @"373-2.wav", SoundType.Monster),
            [SoundIndex.GhostKnightStruck] = new DXSound(SoundPath + @"373-4.wav", SoundType.Monster),
            [SoundIndex.GhostKnightDie] = new DXSound(SoundPath + @"373-5.wav", SoundType.Monster),

            [SoundIndex.IcySpiritSpearmanAttack] = new DXSound(SoundPath + @"376-2.wav", SoundType.Monster),
            [SoundIndex.IcySpiritSpearmanStruck] = new DXSound(SoundPath + @"376-4.wav", SoundType.Monster),
            [SoundIndex.IcySpiritSpearmanDie] = new DXSound(SoundPath + @"376-5.wav", SoundType.Monster),

            [SoundIndex.WerewolfAttack] = new DXSound(SoundPath + @"372-2.wav", SoundType.Monster),
            [SoundIndex.WerewolfStruck] = new DXSound(SoundPath + @"372-4.wav", SoundType.Monster),
            [SoundIndex.WerewolfDie] = new DXSound(SoundPath + @"372-5.wav", SoundType.Monster),

            [SoundIndex.WhitefangAttack] = new DXSound(SoundPath + @"371-2.wav", SoundType.Monster),
            [SoundIndex.WhitefangStruck] = new DXSound(SoundPath + @"371-4.wav", SoundType.Monster),
            [SoundIndex.WhitefangDie] = new DXSound(SoundPath + @"371-5.wav", SoundType.Monster),

            [SoundIndex.IcySpiritSoliderAttack] = new DXSound(SoundPath + @"377-2.wav", SoundType.Monster),
            [SoundIndex.IcySpiritSoliderStruck] = new DXSound(SoundPath + @"377-4.wav", SoundType.Monster),
            [SoundIndex.IcySpiritSoliderDie] = new DXSound(SoundPath + @"377-5.wav", SoundType.Monster),

            [SoundIndex.WildBoarAttack] = new DXSound(SoundPath + @"328-2.wav", SoundType.Monster),
            [SoundIndex.WildBoarStruck] = new DXSound(SoundPath + @"328-4.wav", SoundType.Monster),
            [SoundIndex.WildBoarDie] = new DXSound(SoundPath + @"338-5.wav", SoundType.Monster),

            [SoundIndex.FrostLordHwaAttack] = new DXSound(SoundPath + @"379-2.wav", SoundType.Monster),
            [SoundIndex.FrostLordHwaStruck] = new DXSound(SoundPath + @"379-4.wav", SoundType.Monster),
            [SoundIndex.FrostLordHwaDie] = new DXSound(SoundPath + @"379-5.wav", SoundType.Monster),

            [SoundIndex.JinchonDevilAttack] = new DXSound(SoundPath + @"341-2.wav", SoundType.Monster),
            [SoundIndex.JinchonDevilAttack2] = new DXSound(SoundPath + @"341-7.wav", SoundType.Monster),
            [SoundIndex.JinchonDevilAttack3] = new DXSound(SoundPath + @"341-8.wav", SoundType.Monster),
            [SoundIndex.JinchonDevilStruck] = new DXSound(SoundPath + @"341-4.wav", SoundType.Monster),
            [SoundIndex.JinchonDevilDie] = new DXSound(SoundPath + @"341-5.wav", SoundType.Monster),


            [SoundIndex.EscortCommanderAttack] = new DXSound(SoundPath + @"381-2.wav", SoundType.Monster),
            [SoundIndex.EscortCommanderStruck] = new DXSound(SoundPath + @"381-4.wav", SoundType.Monster),
            [SoundIndex.EscortCommanderDie] = new DXSound(SoundPath + @"381-5.wav", SoundType.Monster),

            [SoundIndex.FieryDancerAttack] = new DXSound(SoundPath + @"383-2.wav", SoundType.Monster),
            [SoundIndex.FieryDancerStruck] = new DXSound(SoundPath + @"383-4.wav", SoundType.Monster),
            [SoundIndex.FieryDancerDie] = new DXSound(SoundPath + @"383-5.wav", SoundType.Monster),

            [SoundIndex.EmeraldDancerAttack] = new DXSound(SoundPath + @"384-2.wav", SoundType.Monster),
            [SoundIndex.EmeraldDancerStruck] = new DXSound(SoundPath + @"384-4.wav", SoundType.Monster),
            [SoundIndex.EmeraldDancerDie] = new DXSound(SoundPath + @"384-5.wav", SoundType.Monster),

            [SoundIndex.QueenOfDawnAttack] = new DXSound(SoundPath + @"382-2.wav", SoundType.Monster),
            [SoundIndex.QueenOfDawnStruck] = new DXSound(SoundPath + @"382-4.wav", SoundType.Monster),
            [SoundIndex.QueenOfDawnDie] = new DXSound(SoundPath + @"382-5.wav", SoundType.Monster),

            [SoundIndex.OYoungBeastAttack] = new DXSound(SoundPath + @"388-2.wav", SoundType.Monster),
            [SoundIndex.OYoungBeastStruck] = new DXSound(SoundPath + @"388-4.wav", SoundType.Monster),
            [SoundIndex.OYoungBeastDie] = new DXSound(SoundPath + @"388-5.wav", SoundType.Monster),

            [SoundIndex.YumgonWitchAttack] = new DXSound(SoundPath + @"391-2.wav", SoundType.Monster),
            [SoundIndex.YumgonWitchStruck] = new DXSound(SoundPath + @"391-4.wav", SoundType.Monster),
            [SoundIndex.YumgonWitchDie] = new DXSound(SoundPath + @"391-5.wav", SoundType.Monster),

            [SoundIndex.MaWarlordAttack] = new DXSound(SoundPath + @"389-2.wav", SoundType.Monster),
            [SoundIndex.MaWarlordStruck] = new DXSound(SoundPath + @"389-4.wav", SoundType.Monster),
            [SoundIndex.MaWarlordDie] = new DXSound(SoundPath + @"389-5.wav", SoundType.Monster),

            [SoundIndex.JinhwanSpiritAttack] = new DXSound(SoundPath + @"392-2.wav", SoundType.Monster),
            [SoundIndex.JinhwanSpiritStruck] = new DXSound(SoundPath + @"392-4.wav", SoundType.Monster),
            [SoundIndex.JinhwanSpiritDie] = new DXSound(SoundPath + @"392-5.wav", SoundType.Monster),

            [SoundIndex.JinhwanGuardianAttack] = new DXSound(SoundPath + @"393-2.wav", SoundType.Monster),
            [SoundIndex.JinhwanGuardianStruck] = new DXSound(SoundPath + @"393-4.wav", SoundType.Monster),
            [SoundIndex.JinhwanGuardianDie] = new DXSound(SoundPath + @"393-5.wav", SoundType.Monster),

            [SoundIndex.YumgonGeneralAttack] = new DXSound(SoundPath + @"390-2.wav", SoundType.Monster),
            [SoundIndex.YumgonGeneralStruck] = new DXSound(SoundPath + @"390-4.wav", SoundType.Monster),
            [SoundIndex.YumgonGeneralDie] = new DXSound(SoundPath + @"390-5.wav", SoundType.Monster),

            [SoundIndex.ChiwooGeneralAttack] = new DXSound(SoundPath + @"385-2.wav", SoundType.Monster),
            [SoundIndex.ChiwooGeneralStruck] = new DXSound(SoundPath + @"385-4.wav", SoundType.Monster),
            [SoundIndex.ChiwooGeneralDie] = new DXSound(SoundPath + @"385-5.wav", SoundType.Monster),

            [SoundIndex.DragonQueenAttack] = new DXSound(SoundPath + @"387-2.wav", SoundType.Monster),
            [SoundIndex.DragonQueenStruck] = new DXSound(SoundPath + @"387-4.wav", SoundType.Monster),
            [SoundIndex.DragonQueenDie] = new DXSound(SoundPath + @"387-5.wav", SoundType.Monster),

            [SoundIndex.DragonLordAttack] = new DXSound(SoundPath + @"386-2.wav", SoundType.Monster),
            [SoundIndex.DragonLordStruck] = new DXSound(SoundPath + @"386-4.wav", SoundType.Monster),
            [SoundIndex.DragonLordDie] = new DXSound(SoundPath + @"386-5.wav", SoundType.Monster),

            [SoundIndex.FerociousIceTigerAttack] = new DXSound(SoundPath + @"201-2.wav", SoundType.Monster),
            [SoundIndex.FerociousIceTigerStruck] = new DXSound(SoundPath + @"201-4.wav", SoundType.Monster),
            [SoundIndex.FerociousIceTigerDie] = new DXSound(SoundPath + @"201-5.wav", SoundType.Monster),


            [SoundIndex.SamaFireGuardianAttack] = new DXSound(SoundPath + @"400-2.wav", SoundType.Monster),
            [SoundIndex.SamaFireGuardianStruck] = new DXSound(SoundPath + @"400-4.wav", SoundType.Monster),
            [SoundIndex.SamaFireGuardianDie] = new DXSound(SoundPath + @"400-5.wav", SoundType.Monster),

            [SoundIndex.SamaIceGuardianAttack] = new DXSound(SoundPath + @"398-2.wav", SoundType.Monster),
            [SoundIndex.SamaIceGuardianStruck] = new DXSound(SoundPath + @"398-4.wav", SoundType.Monster),
            [SoundIndex.SamaIceGuardianDie] = new DXSound(SoundPath + @"398-5.wav", SoundType.Monster),

            [SoundIndex.SamaLightningGuardianAttack] = new DXSound(SoundPath + @"397-2.wav", SoundType.Monster),
            [SoundIndex.SamaLightningGuardianStruck] = new DXSound(SoundPath + @"397-4.wav", SoundType.Monster),
            [SoundIndex.SamaLightningGuardianDie] = new DXSound(SoundPath + @"397-5.wav", SoundType.Monster),

            [SoundIndex.SamaWindGuardianAttack] = new DXSound(SoundPath + @"399-2.wav", SoundType.Monster),
            [SoundIndex.SamaWindGuardianStruck] = new DXSound(SoundPath + @"399-4.wav", SoundType.Monster),
            [SoundIndex.SamaWindGuardianDie] = new DXSound(SoundPath + @"399-5.wav", SoundType.Monster),

            [SoundIndex.PhoenixAttack] = new DXSound(SoundPath + @"402-2.wav", SoundType.Monster),
            [SoundIndex.PhoenixStruck] = new DXSound(SoundPath + @"402-4.wav", SoundType.Monster),
            [SoundIndex.PhoenixDie] = new DXSound(SoundPath + @"402-5.wav", SoundType.Monster),

            [SoundIndex.BlackTortoiseAttack] = new DXSound(SoundPath + @"404-2.wav", SoundType.Monster),
            [SoundIndex.BlackTortoiseStruck] = new DXSound(SoundPath + @"404-4.wav", SoundType.Monster),
            [SoundIndex.BlackTortoiseDie] = new DXSound(SoundPath + @"404-5.wav", SoundType.Monster),

            [SoundIndex.BlueDragonAttack] = new DXSound(SoundPath + @"403-2.wav", SoundType.Monster),
            [SoundIndex.BlueDragonStruck] = new DXSound(SoundPath + @"403-4.wav", SoundType.Monster),
            [SoundIndex.BlueDragonDie] = new DXSound(SoundPath + @"403-5.wav", SoundType.Monster),

            #endregion
        };

        #endregion

        public static void Create()
        {
            try
            {
                Device = new DirectSound();
                Device.SetCooperativeLevel(DiyMonShowViewer.CurrentDiyMonViewer.Handle, CooperativeLevel.Normal);
                AdjustVolume();
            }
            catch (Exception)
            {

                Error = true;
            }
        }

        #region  怪物自定义  

        public static void Play(string SoundName)
        {
            if (SoundName == "") return;
            DXSound sound;
            if (DiySoundList.TryGetValue(SoundName, out sound))
            {
                sound.Play();
                return;
            }
            sound = new DXSound(SoundPath + SoundName, SoundType.Monster);
            sound.Play();
            DXSoundManager.DiySoundList.Add(SoundName, sound);
        }
        public static DXSound AddDiySound(string SoundName)
        {
            DXSound sound = null;
            if (SoundName == "") return sound;
            if (!DiySoundList.TryGetValue(SoundName, out sound))
            {
                sound = new DXSound(SoundPath + SoundName, SoundType.Monster);
                DXSoundManager.DiySoundList.Add(SoundName, sound);
            }
            return sound;
        }
        public static DXSound AddDiySound(int MonId, MirAnimationSound mirAnimationSound)
        {
            DXSound sound = null;
            if (MonId < 0) return sound;
            string SoundString = string.Empty;
            switch (mirAnimationSound)
            {
                case MirAnimationSound.AttackSound:
                    SoundString = string.Format("{0}-2.wav", MonId);
                    break;
                case MirAnimationSound.StruckSound:
                    SoundString = string.Format("{0}-4.wav", MonId);
                    break;
                case MirAnimationSound.DieSound:
                    SoundString = string.Format("{0}-5.wav", MonId);
                    break;
                default:
                    break;
            }
            if (!DiySoundList.TryGetValue(SoundString, out sound))
            {
                sound = new DXSound(SoundPath + SoundString, SoundType.Monster);
                DXSoundManager.DiySoundList.Add(SoundString, sound);
            }
            return sound;
        }

        #endregion

        public static void Play(SoundIndex index)
        {
            if (index == SoundIndex.None || Error) return;

            DXSound sound;

            if (SoundList.TryGetValue(index, out sound))
            {
                sound.Play();
                return;
            }
        }

        public static void Stop(SoundIndex index)
        {
            DXSound sound;

            if (!SoundList.TryGetValue(index, out sound)) return;

            sound.Stop();
        }
        public static void StopAllSounds()
        {
            for (int i = DXManager.SoundList.Count - 1; i >= 0; i--)
                DXManager.SoundList[i].Stop();
        }
        public static void AdjustVolume()
        {
            foreach (KeyValuePair<SoundIndex, DXSound> pair in SoundList)
                pair.Value.SetVolume();
        }
        public static void UpdateFlags()
        {
            for (int i = DXManager.SoundList.Count - 1; i >= 0; i--)
                DXManager.SoundList[i].UpdateFlags();
        }

        public static void Unload()
        {
            for (int i = DXManager.SoundList.Count - 1; i >= 0; i--)
                DXManager.SoundList[i].DisposeSoundBuffer();

            if (Device != null)
            {
                if (!Device.IsDisposed)
                    Device.Dispose();

                Device = null;
            }
        }
    }
    public enum SoundType
    {
        None,

        System,
        Music,
        Magic,
        Monster,
        Player,
    }
}