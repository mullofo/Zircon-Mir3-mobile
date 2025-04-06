using Client.Controls;
using Client.Envir;
using Client.Scenes;
using FontStashSharp;
using Library;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended.Input;
using MonoGame.Extended.ViewportAdapters;
using System;

namespace Mir3.Mobile
{
    public class Game1 : Game
    {
        public GraphicsDeviceManager Graphics { get; private set; }
        private SpriteBatch _spriteBatch;
        private DynamicSpriteFont d_font;
        private MouseListener _mouseListener;
        private TouchListener _touchListener;
        private KeyboardListener _keyboardListener;
        public ViewportAdapter _viewportAdapter;
        public static bool updateVersion;
        public static bool loadAssets;
        public static bool onlyOnce;
        //public Texture2D OpenGameTexture;
        public Texture2D LightTexture;
        public Texture2D PalleteTexture;
        //public Texture2D ButtonTexture;
        private Texture2D JoyStickBack;
        private Texture2D JoyStick;
        private StickTexture VirtualStick;
        private int ScreenWidth;
        private int ScreenHeigth;
        private int OpenImgWidth;
        private int OpenOffSetX;
        public static Game1 Game { get; private set; }
        public static INative Native { get; set; }
        public Game1()
        {
            Game = this;
            Graphics = new GraphicsDeviceManager(this);
            Graphics.IsFullScreen = true;
            Graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            TouchPanel.EnabledGestures = GestureType.Tap;
            Graphics.SynchronizeWithVerticalRetrace = true;  //垂直同步
            IsFixedTimeStep = false;
            //TargetElapsedTime = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond / 30))
            //IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //初始化 鼠标，触控，键盘 输入事件
            InitInputListener();

            //
            ScreenWidth = Graphics.PreferredBackBufferWidth;
            ScreenHeigth = Graphics.PreferredBackBufferHeight;
            OpenImgWidth = (int)((1024 * ScreenHeigth) / 768f);
            OpenOffSetX = (ScreenWidth - OpenImgWidth) / 2;

            Native.Initialize();

            CEnvir.UpdateScale();


            base.Initialize();
        }

        protected override void LoadContent()
        {
            ConfigReader.Load();

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Native.InitData();

            //foreach (KeyValuePair<LibraryFile, string> library in Libraries.LibraryList)
            //{
            //    //if (File.Exists(CEnvir.MobileClientPath + library.Value))
            //    //{
            //    //    CEnvir.LibraryList[library.Key] = new MirLibrary(library.Value);
            //    //}
            //    CEnvir.LibraryList[library.Key] = new MirLibrary(library.Value);
            //}
            CEnvir.LibraryList[LibraryFile.StartMobileScene] = new MirLibrary(Libraries.LibraryList[LibraryFile.StartMobileScene]);
            CEnvir.LibraryList[LibraryFile.Interface] = new MirLibrary(Libraries.LibraryList[LibraryFile.Interface]);

            FontSystem fontSystem = new FontSystem();
            fontSystem.AddFont(Native.GetFileBytes("Fonts/pingfang.ttf"));

            CEnvir.Fonts = fontSystem;
            d_font = fontSystem.GetFont(32);
            //using (var stream= GetFileStream("GameOpen.png"))
            //{
            //    OpenGameTexture = Texture2D.FromStream(GraphicsDevice, stream);
            //}

            using (var stream = Native.GetFileStream("Back_Joystick.png"))
            {
                JoyStickBack = Texture2D.FromStream(GraphicsDevice, stream);
            }

            using (var stream = Native.GetFileStream("Joystick.png"))
            {
                JoyStick = Texture2D.FromStream(GraphicsDevice, stream);
            }

            using (var stream = Native.GetFileStream("Light.png"))
            {
                LightTexture = Texture2D.FromStream(GraphicsDevice, stream);
            }

            using (var stream = Native.GetFileStream("Pallete.png"))
            {
                PalleteTexture = Texture2D.FromStream(GraphicsDevice, stream);
            }

            //using (var stream = GetFileStream("button.png"))
            //{
            //    ButtonTexture = Texture2D.FromStream(GraphicsDevice, stream);
            //}

            CEnvir.GrayscaleShader = new Microsoft.Xna.Framework.Graphics.Effect(GraphicsDevice, Native.GetFileBytes("Grayscale.mgfx"));

            VirtualStick = new StickTexture();

            DXManager.Create(GraphicsDevice, _spriteBatch, Graphics);

            DXSoundManager.Create();

            DXManager.SetLight(LightTexture);
            //DXManager.SetOpenImage(OpenGameTexture);
            //DXManager.SetButtonImage(ButtonTexture);
            DXManager.SetColourPallete(PalleteTexture);
            CEnvir.UpdateScale(true);
            DXControl.ActiveScene = new StartMobileScene(CEnvir.GameSize);
            GC.Collect();
        }

        protected override void UnloadContent()
        {
            Unload();
        }

        public void Unload()
        {
            ConfigReader.Save();
            CEnvir.Unload();
            DXManager.Unload();
            DXSoundManager.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            // TODO: Add your update logic here

            UpdateInputListener(gameTime);
            CEnvir.UpdateGame();
            UpdateVirtualStick(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            CEnvir.FrameTime = gameTime.ElapsedGameTime.Ticks / 10000L;
            CEnvir.RenderGame();
            //RenderVirtualStick();
            base.Draw(gameTime);
        }

        private void InitInputListener()
        {
            TouchPanel.EnabledGestures = GestureType.Tap | GestureType.Flick | GestureType.FreeDrag | GestureType.DragComplete | GestureType.Hold | GestureType.Pinch | GestureType.DoubleTap;
            _mouseListener = new MouseListener();
            _touchListener = new TouchListener();
            _keyboardListener = new KeyboardListener();
        }

        private void UpdateInputListener(GameTime gameTime)
        {
            _touchListener.Update(gameTime);
            _mouseListener.Update(gameTime);
            _keyboardListener.Update(gameTime);
        }

        private void UpdateVirtualStick(GameTime gameTime)
        {
            if (DXControl.ActiveScene is GameScene && GameScene.Game.IsStickAvailable())
            {
                VirtualStick.Update(gameTime);

                //float len = Math.Max(Math.Abs(VirtualStick.LeftStick.StartLocation.X - VirtualStick.LeftStick.Pos.X), Math.Abs(VirtualStick.LeftStick.StartLocation.Y - VirtualStick.LeftStick.Pos.Y));
                //if (len < 70)
                //    GameScene.Game.StickMode = MonoGame.Extended.StickMode.Walk;
                //else
                //    GameScene.Game.StickMode = MonoGame.Extended.StickMode.Run;
            }
        }

        public void RenderVirtualStick()
        {
            if (GameScene.Game?.StickMode == MonoGame.Extended.StickMode.Walk) return;
            if (DXControl.ActiveScene is GameScene && GameScene.Game.IsStickAvailable())
            {
                //_spriteBatch.Begin(SpriteSortMode.Deferred, DXManager.AlphaBlend);
                CEnvir.BSCounter++;
                VirtualStick.DrawStick(_spriteBatch, JoyStickBack, JoyStick, CEnvir.UIScale);
                //_spriteBatch.End();
                Vector2 relativeVector = VirtualStick.LeftStick.GetRelativeVector(VirtualStick.aliveZoneSize);
                (DXControl.ActiveScene as GameScene).MapControl.SetStick(relativeVector.X, relativeVector.Y);
            }
            //else if (DXControl.ActiveScene is GameScene && !GameScene.Game.IsStickAvailable())
            //    GameScene.Game.DrawStick = false;
        }
    }
}