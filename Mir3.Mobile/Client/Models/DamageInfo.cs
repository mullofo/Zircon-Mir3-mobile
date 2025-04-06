using Client.Envir;
using Client.Scenes;
using Library;
using System;
using System.Drawing;

namespace Client.Models
{
    /*
        public class DamageInfoOld
        {
            public static List<DXLabel> Labels = new List<DXLabel>();

            public string Text;
            public Color ForeColour;
            public Color OutlineColour;

            public DateTime StartTime;
            public TimeSpan Duration;
            public int OffsetX = 25;
            public int OffsetY = 50;
            public bool Shift;

            public DXLabel Label;

            public DamageInfo(int damage)
            {
                Text = damage.ToString("+#,##0;-#,##0");
                StartTime = CEnvir.Now;
                Duration = TimeSpan.FromSeconds(2);
                OutlineColour = Color.Black;

                if (damage <= -500)
                    ForeColour = Color.Orange;
                else if (damage <= -100)
                    ForeColour = Color.Green;
                else if (damage < 0)
                    ForeColour = Color.Red;
                else
                    ForeColour = Color.Blue;


                CreateLabel();
            }
            public DamageInfo(string text, Color textColour)
            {
                Text = text;
                ForeColour = textColour;
                StartTime = CEnvir.Now;
                Duration = TimeSpan.FromSeconds(2);
                OutlineColour = Color.Black;

                CreateLabel();
            }

            public void CreateLabel()
            {
                Label = Labels.FirstOrDefault(x => x.Text == Text && x.ForeColour == ForeColour && x.OutlineColour == OutlineColour);

                if (Label != null) return;

                Label = new DXLabel
                {
                    Text = Text,
                    ForeColour = ForeColour,
                    Outline = true,
                    OutlineColour = OutlineColour,
                    IsVisible = true,
                    Font = new Font(Config.FontName, 10),
                };

                Labels.Add(Label);
                Label.Disposing += (o, e) => Labels.Remove(Label);
            }

            public void Draw(int DrawX, int DrawY)
            {
                TimeSpan time = CEnvir.Now - StartTime;

                int x = (int)(time.Ticks / (Duration.Ticks / OffsetX) % OffsetX);
                int y = (int)(time.Ticks / (Duration.Ticks / OffsetY) % OffsetY);

                if (Shift)
                    x -= Label.Size.Width - 5;

                Point location = new Point(GameScene.Game.Location.X + DrawX + x + 13 , GameScene.Game.Location.Y + DrawY - y - 30);


               // location.X += (48 - Label.Size.Width) / 2;
                location.Y -= 32  - Label.Size.Height;

                Label.Location = location;

                Label.Draw();
            }

        }*/

    /// <summary>
    /// 受伤信息
    /// </summary>
    public class DamageInfo
    {
        #region 数值
        /// <summary>
        /// 数值
        /// </summary>
        public int Value
        {
            get { return _Value; }
            set
            {
                if (_Value == value) return;

                int oldValue = _Value;
                _Value = value;

                OnValueChanged(oldValue, value);
            }
        }
        private int _Value;
        public event EventHandler<EventArgs> ValueChanged;
        public virtual void OnValueChanged(int oValue, int nValue)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region 格挡
        /// <summary>
        /// 挡住
        /// </summary>
        public bool Block
        {
            get { return _Block; }
            set
            {
                if (_Block == value) return;

                bool oldValue = _Block;
                _Block = value;

                OnBlockChanged(oldValue, value);
            }
        }
        private bool _Block;
        public event EventHandler<EventArgs> BlockChanged;
        public virtual void OnBlockChanged(bool oValue, bool nValue)
        {
            BlockChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region 闪避
        /// <summary>
        /// 未击中
        /// </summary>
        public bool Miss
        {
            get { return _Miss; }
            set
            {
                if (_Miss == value) return;

                bool oldValue = _Miss;
                _Miss = value;

                OnMissChanged(oldValue, value);
            }
        }
        private bool _Miss;
        public event EventHandler<EventArgs> MissChanged;
        public virtual void OnMissChanged(bool oValue, bool nValue)
        {
            MissChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region 暴击
        /// <summary>
        /// 暴击
        /// </summary>
        public bool Critical
        {
            get { return _Critical; }
            set
            {
                if (_Critical == value) return;

                bool oldValue = _Critical;
                _Critical = value;

                OnCriticalChanged(oldValue, value);
            }
        }
        private bool _Critical;
        public event EventHandler<EventArgs> CriticalChanged;
        public virtual void OnCriticalChanged(bool oValue, bool nValue)
        {
            CriticalChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region 致命一击
        /// <summary>
        /// 致命一击
        /// </summary>
        public bool FatalAttack
        {
            get { return _FatalAttack; }
            set
            {
                if (_FatalAttack == value) return;

                bool oldValue = _FatalAttack;
                _FatalAttack = value;

                OnFatalAttackChanged(oldValue, value);
            }
        }
        private bool _FatalAttack;
        public event EventHandler<EventArgs> FatalAttackChanged;
        public virtual void OnFatalAttackChanged(bool oValue, bool nValue)
        {
            FatalAttackChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region 会心一击
        /// <summary>
        /// 会心一击
        /// </summary>
        public bool CriticalHit
        {
            get { return _CriticalHit; }
            set
            {
                if (_CriticalHit == value) return;

                bool oldValue = _CriticalHit;
                _CriticalHit = value;

                OnCriticalHitChanged(oldValue, value);
            }
        }
        private bool _CriticalHit;
        public event EventHandler<EventArgs> CriticalHitChanged;
        public virtual void OnCriticalHitChanged(bool oValue, bool nValue)
        {
            CriticalHitChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region 暴捶
        /// <summary>
        /// 暴捶
        /// </summary>
        public bool DamageAdd
        {
            get { return _DamageAdd; }
            set
            {
                if (_DamageAdd == value) return;

                bool oldValue = _DamageAdd;
                _DamageAdd = value;

                OnDamageAddChanged(oldValue, value);
            }
        }
        private bool _DamageAdd;
        public event EventHandler<EventArgs> DamageAddChanged;
        public virtual void OnDamageAddChanged(bool oValue, bool nValue)
        {
            DamageAddChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region 暴毒
        /// <summary>
        /// 暴毒
        /// </summary>
        public bool GreenPosionPro
        {
            get { return _GreenPosionPro; }
            set
            {
                if (_GreenPosionPro == value) return;

                bool oldValue = _GreenPosionPro;
                _GreenPosionPro = value;

                OnGreenPosionProChanged(oldValue, value);
            }
        }
        private bool _GreenPosionPro;
        public event EventHandler<EventArgs> GreenPosionProChanged;
        public virtual void OnGreenPosionProChanged(bool oValue, bool nValue)
        {
            GreenPosionProChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region 抽蓝
        /// <summary>
        /// 抽蓝
        /// </summary>
        public bool SmokingMP
        {
            get { return _SmokingMP; }
            set
            {
                if (_SmokingMP == value) return;

                bool oldValue = _SmokingMP;
                _SmokingMP = value;

                OnSmokingMPChanged(oldValue, value);
            }
        }
        private bool _SmokingMP;
        public event EventHandler<EventArgs> SmokingMPChanged;
        public virtual void OnSmokingMPChanged(bool oValue, bool nValue)
        {
            SmokingMPChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        /// <summary>
        /// 库
        /// </summary>
        public static MirLibrary Library;
        /// <summary>
        /// 开始时间=系统当前时间
        /// </summary>
        public DateTime StartTime { get; set; } = CEnvir.Now;
        /// <summary>
        /// 飘血延迟时间
        /// </summary>
        public TimeSpan AppearDelay { get; set; } = TimeSpan.FromMilliseconds(800);
        /// <summary>
        /// 停顿延迟时间
        /// </summary>
        public TimeSpan ShowDelay { get; set; } = TimeSpan.FromMilliseconds(20);
        /// <summary>
        /// 慢慢消失时间
        /// </summary>
        public TimeSpan HideDelay { get; set; } = TimeSpan.FromMilliseconds(450);

        public int BlueWidth = 9, RedWidth = 9, GreenWidth = 11, OrangeWidth = 13, WhiteWidth = 20;
        public int BlueIndex = 71, RedIndex = 72, GreenIndex = 73, OrangeIndex = 74, WhiteIndex = 75;

        /// <summary>
        /// 受伤信息
        /// </summary>
        static DamageInfo()
        {
            CEnvir.LibraryList.TryGetValue(LibraryFile.Interface, out Library);
        }

        /// <summary>
        /// 飘血的高度
        /// </summary>
        public int DrawHeight = 75;
        /// <summary>
        /// 绘制Y值
        /// </summary>
        public int DrawY { get; set; }
        /// <summary>
        /// 绘制X值
        /// </summary>
		public int DrawX { get; set; }
        /// <summary>
        /// 显示
        /// </summary>
        public bool Visible = true;
        /// <summary>
        /// 底部等Y值-20
        /// </summary>
        public int Bottom => DrawY - 20;
        /// <summary>
        /// 透明度
        /// </summary>
        public float Opacity;

        public void Process(DamageInfo previous)
        {
            if (Library == null) return;

            TimeSpan visibleTime = CEnvir.Now - StartTime;

            int oldY = DrawY;
            int oldX = DrawX;
            if (visibleTime < AppearDelay + ShowDelay) //逐渐上升
            {
                //百分比
                decimal percent = visibleTime.Ticks / (decimal)(AppearDelay + ShowDelay).Ticks;

                if (DrawY != 0)
                {

                }
                DrawY = (int)(DrawHeight * percent);

                DrawX = -DrawY;

                Opacity = (float)(percent * 3);

            }
            //else if (visibleTime < AppearDelay + ShowDelay)
            //{
            //    DrawY = DrawHeight;
            //    Opacity = 1;
            //}
            else if (visibleTime < AppearDelay/* + ShowDelay */+ HideDelay) //逐渐消失
            {
                //20 Travel distance?
                visibleTime -= AppearDelay/* + ShowDelay*/;
                decimal percent = visibleTime.Ticks / (decimal)HideDelay.Ticks;

                DrawY = DrawHeight + (int)(40 * percent);
                DrawX = -(int)DrawY;
                Opacity = 1 - (float)(percent);
            }
            else
            {
                Visible = false;
                return;
            }

            if (previous != null && previous.Visible)
                DrawY = Math.Min(DrawY, previous.Bottom);

            if (oldY != DrawY)
                GameScene.Game.MapControl.TextureValid = false;
        }

        /// <summary>
        /// 绘制坐标值
        /// </summary>
        /// <param name="drawX"></param>
        /// <param name="drawY"></param>
        public void Draw(int drawX, int drawY)
        {
            if (Library == null) return;

            drawY -= DrawY + 10;

            drawX += 24;
            Size size;
            if (Value == 0)
            {
                if (Miss)
                {
                    size = Library.GetSize(180);
                    drawX -= size.Width / 2;

                    Library.Draw(180, drawX, drawY, Color.White, false, Opacity, ImageType.Image);
                }
                else
                if (Block)
                {
                    size = Library.GetSize(181);
                    drawX -= size.Width / 2;

                    Library.Draw(181, drawX, drawY, Color.White, false, Opacity, ImageType.Image);
                }

                //Block
            }
            else
            {
                string text = Value.ToString("+#0;-#0");

                int index;
                int width;
                if (Value <= -1000)
                {
                    //White
                    index = WhiteIndex;
                    width = WhiteWidth;
                }
                else if (Value <= -500)
                {
                    //Orange
                    index = OrangeIndex;
                    width = OrangeWidth;
                }
                else if (Value <= -100)
                {
                    //Green
                    index = GreenIndex;
                    width = GreenWidth;
                }
                else if (Value < 0)
                {
                    //Red
                    index = RedIndex;
                    width = RedWidth;
                }
                else
                {
                    //Blue
                    index = BlueIndex;
                    width = BlueWidth;
                }
                drawX -= width * text.Length / 2;

                if (Critical && Value < 0)
                {
                    size = Library.GetSize(182);
                    drawX -= size.Width / 2;

                    Library.Draw(182, drawX, drawY, Color.White, false, Opacity, ImageType.Image);
                    drawX += size.Width + 5;
                }
                else if (FatalAttack && Value < 0)
                {
                    size = Library.GetSize(183);
                    drawX -= size.Width / 2;

                    Library.Draw(183, drawX, drawY, Color.White, false, Opacity, ImageType.Image);
                    drawX += size.Width + 5;
                }
                else if (CriticalHit && Value < 0)
                {
                    size = Library.GetSize(184);
                    drawX -= size.Width / 2;

                    Library.Draw(184, drawX, drawY, Color.White, false, Opacity, ImageType.Image);
                    drawX += size.Width + 5;
                }
                else if (DamageAdd && Value < 0)
                {
                    size = Library.GetSize(185);
                    drawX -= size.Width / 2;

                    Library.Draw(185, drawX, drawY, Color.White, false, Opacity, ImageType.Image);
                    drawX += size.Width + 5;
                }
                else if (GreenPosionPro && Value < 0)
                {
                    size = Library.GetSize(186);
                    drawX -= size.Width / 2;

                    Library.Draw(186, drawX, drawY, Color.White, false, Opacity, ImageType.Image);
                    drawX += size.Width + 5;
                }
                else if (SmokingMP && Value < 0)
                {
                    size = Library.GetSize(190);
                    drawX -= size.Width / 2;

                    Library.Draw(190, drawX, drawY, Color.White, false, Opacity, ImageType.Image);
                    drawX += size.Width + 5;
                }

                size = Library.GetSize(index);

                for (int i = 0; i < text.Length; i++)
                {
                    int number;

                    if (!int.TryParse(text[i].ToString(), out number))
                    {
                        if (text[i] == '+')
                            number = 10;
                        else if (text[i] == '-')
                            number = 11;
                        else continue;
                    }

                    Library.Draw(index, drawX, drawY, Color.White, new Rectangle(width * number, 0, Math.Min(size.Width - width * number, width), size.Height), Opacity, ImageType.Image);
                    drawX += width;
                }
            }
        }
    }
}
