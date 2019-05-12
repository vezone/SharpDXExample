namespace TankGuiObserver2
{
    using System.Diagnostics;

    using SharpDX.Direct2D1;
    using SharpDX.Direct3D;
    using SharpDX.Direct3D11;
    using SharpDX.DXGI;
    using SharpDX.Windows;
    using SharpDX.DirectInput;
    using SharpDX.Mathematics.Interop;
    using AlphaMode = SharpDX.Direct2D1.AlphaMode;
    using Device = SharpDX.Direct3D11.Device;
    using Factory = SharpDX.DXGI.Factory;

    using TankCommon.Objects;
    using SharpDX;
    using SharpDX.DirectWrite;

    public class TextAnimation
    {
        private Stopwatch _textTimer;
        private string cString;
        private string _animatedString;

        public TextAnimation()
        {
            _textTimer = new Stopwatch();
            _textTimer.Start();
        }

        public void SetAnimatedString(string animatedString)
        {
            _animatedString = animatedString;
            cString = animatedString;
        }

        public string GetAnimatedString()
        {
            return _animatedString;
        }

        //frame, ms
        public void AnimationStart(string frame, int ms)
        {
            if (_textTimer.ElapsedMilliseconds > ms)
            {
                _animatedString += frame;
                if (_animatedString.Length > (cString.Length + 3))
                    _animatedString = cString;
                _textTimer.Reset();
                _textTimer.Start();
            }
        }

    }

    class Game : System.IDisposable
    {
        RenderForm RenderForm;
        RenderTarget RenderTarget2D;
        SharpDX.Direct2D1.Factory Factory2D;
        Surface Surface;
        SwapChain SwapChain;
        Device Device;

        GuiSpectator _spectatorClass;
        System.Threading.Thread _clientThread;
        GameRender _gameRender;
        RawColor4 blackScreen = new RawColor4(0.0f, 0.0f, 0.0f, 1.0f);

        public int FPSCounter = 0;
        public Stopwatch FPSTimer;

        bool _isEnterPressed;
        bool _isTabPressed;
        bool _isFPressed;
        DirectInput _directInput;
        Keyboard _keyboard;

        SharpDX.DirectWrite.Factory directFactory;
        private TextFormat textFormat;
        private SolidColorBrush backgroundBrush;
        private SolidColorBrush defaultBrush;
        private SolidColorBrush greenBrush;
        private SolidColorBrush redBrush;
        Color4 backgroundBrushColor;
        Color4 nonVisibleBrushColor;
        private RectangleF _fullTextBackground;
        private RectangleF _textRect = new RectangleF(25, 5, 150, 30);
        private RectangleF _textRect2 = new RectangleF(25, 5, 150, 30);
        private RectangleF _textRectMenu = new RectangleF(55, 50, 450, 30);
        private static string fpsms;

        //TRASH
        TextAnimation _textAnimation;

        public Game(string windowName, 
            int windowWidth, int windowHeight,
            bool isWindowed = true)
        {
            RenderForm = new RenderForm(windowName);
            RenderForm.Width = windowWidth;
            RenderForm.Height = windowHeight;
            RenderForm.AllowUserResizing = false;

            var desc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription =
                    new ModeDescription(
                        (int)(RenderForm.Width),
                        (int)(RenderForm.Height),
                        new Rational(60, 1),
                        Format.R8G8B8A8_UNorm),
                IsWindowed = isWindowed,
                OutputHandle = RenderForm.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            Device.CreateWithSwapChain(DriverType.Hardware,
                DeviceCreationFlags.BgraSupport,
                new SharpDX.Direct3D.FeatureLevel[] { SharpDX.Direct3D.FeatureLevel.Level_10_0 },
                desc, out Device, out SwapChain);

            Factory2D = new SharpDX.Direct2D1.Factory();
            Factory factory = SwapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(RenderForm.Handle,
                    WindowAssociationFlags.IgnoreAll);

            Texture2D backBuffer = Texture2D.FromSwapChain<Texture2D>(SwapChain, 0);
            Surface = backBuffer.QueryInterface<Surface>();

            //_map = TankCommon.MapManager.LoadMap(100, 'с', 5, 40);

            //textRenderer
            backgroundBrushColor = new Color4(0.3f, 0.3f, 0.3f, 0.5f);
            nonVisibleBrushColor = new Color4(0.3f, 0.3f, 0.3f, 0.0f);
            RenderTarget2D = new RenderTarget(Factory2D, Surface, new RenderTargetProperties(
                                 new PixelFormat(Format.Unknown, AlphaMode.Premultiplied)));
            defaultBrush = new SolidColorBrush(RenderTarget2D, Color.White);
            greenBrush = new SolidColorBrush(RenderTarget2D, Color.Green);
            redBrush = new SolidColorBrush(RenderTarget2D, Color.Red);
            backgroundBrush = new SolidColorBrush(RenderTarget2D, backgroundBrushColor);
            
            directFactory =
                new SharpDX.DirectWrite.Factory(SharpDX.DirectWrite.FactoryType.Shared);
            textFormat = new TextFormat(directFactory, "Arial", FontWeight.Regular, FontStyle.Normal, 24.0f);
            
            
            _fullTextBackground = new RectangleF(_textRect.Left,
                _textRect.Top,
                _textRect.Width, _textRect.Height);

            //WEB_SOCKET
            Connect();
            System.Threading.Tasks.Task.Delay(1000);

            _gameRender = new GameRender(Factory2D, RenderTarget2D);

            FPSTimer = new Stopwatch();
            FPSTimer.Start();

            _directInput = new DirectInput();
            _keyboard = new Keyboard(_directInput);
            _keyboard.Properties.BufferSize = 128;
            _keyboard.Acquire();

            _textAnimation = new TextAnimation();
            _textAnimation.SetAnimatedString("Waiting for connection to the server");
        }

        public void RunGame()
        {
            RenderLoop.Run(RenderForm, Draw);
        }

        public void Draw()
        {
            FPSCounter++;
            RenderTarget2D.BeginDraw();
            KeyboardState kbs = _keyboard.GetCurrentState();//_keyboard.Poll();
            foreach (var key in kbs.PressedKeys)
            {
                if (key == Key.Tab)
                {
                    _isTabPressed = true;
                }
                else if (key == Key.F)
                {
                    if (!_isFPressed)
                    {
                        _isFPressed = true;
                        //backgroundBrush.Color = nonVisibleBrushColor;
                    }
                    else
                    {
                        _isFPressed = false;
                        //backgroundBrush.Color = backgroundBrushColor;
                    }
                }
                else if (key == Key.Return)
                {
                    _isEnterPressed = true;
                }
            }
            
            if (FPSTimer.ElapsedMilliseconds > 1000)
            {
                int fps = (int)((1000.0f * FPSCounter) / FPSTimer.ElapsedMilliseconds);
                int ms = (int)FPSTimer.ElapsedMilliseconds / FPSCounter;
                fpsms = string.Format("{0}fps, {1}ms", fps, ms);
                
                RenderForm.Text = "SharpDX Demo " + fpsms;
                FPSTimer.Reset();
                FPSTimer.Stop();
                FPSTimer.Start();
                FPSCounter = 0;
            }
            
            if (_spectatorClass.Map != null)
            {
                RenderTarget2D.Clear(blackScreen);
                RenderTarget2D.DrawText("Press enter to run game observer",
                    textFormat, _textRectMenu, defaultBrush);
                if (_isEnterPressed)
                {
                    RenderTarget2D.Clear(blackScreen);
                    _gameRender.DrawMap(_spectatorClass.Map);//_map);
                    _gameRender.DrawInteractiveObjects(_spectatorClass.Map.InteractObjects);
                }
            }

            if (_spectatorClass.Map == null)
            {
                _textAnimation.AnimationStart(".", 300);
                RenderTarget2D.Clear(blackScreen);
                RenderTarget2D.DrawText(_textAnimation.GetAnimatedString(),
                    textFormat, _textRectMenu, defaultBrush);
            }

            //if (FPSTimer.ElapsedMilliseconds > 900 && _spectatorClass.Map == null)
            //{
            //    RenderTarget2D.Clear(blackScreen);
            //    str3 += ".";
            //    RenderTarget2D.DrawText(str + str3,
            //        textFormat, _textRectMenu, defaultBrush);
            //    if (str3.Length > 2)
            //        str3 = "";
            //}

            if (_isTabPressed)
            {
                //_gameRender.DrawClientsInfo();
            }
            if (_isFPressed)
            {
                RenderTarget2D.Clear(blackScreen);
                RenderTarget2D.FillRectangle(_fullTextBackground, backgroundBrush);
                RenderTarget2D.DrawText(fpsms, textFormat, _textRect, greenBrush);
            }
            
            RenderTarget2D.EndDraw();
            SwapChain.Present(0, PresentFlags.None);
        }
        
        public void Connect()
        {
            var tokenSource = new System.Threading.CancellationTokenSource();
            var clientCore = new TankClient.ClientCore("ws://127.0.0.1:2000", string.Empty);
            _spectatorClass = new GuiSpectator(tokenSource.Token);

            _clientThread = new System.Threading.Thread(() => {
                clientCore.Run(false, _spectatorClass.Client, tokenSource.Token);
            });
            _clientThread.Start();
        }

        public void Dispose()
        {
            RenderForm.Dispose();
            RenderTarget2D.Dispose();
            Factory2D.Dispose();
            Surface.Dispose();
            SwapChain.Dispose();
            Device.ImmediateContext.ClearState();
            Device.ImmediateContext.Flush();
            Device.Dispose();
        }

    }
}
