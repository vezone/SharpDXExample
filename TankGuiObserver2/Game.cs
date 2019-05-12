namespace TankGuiObserver2
{
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

        bool _isEnterPressed;
        bool _isTabPressed;
        bool _isFPressed;
        DirectInput _directInput;
        Keyboard _keyboard;
        GameRender _gameRender;

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

            RenderTarget2D = new RenderTarget(Factory2D, Surface, new RenderTargetProperties(
                                 new PixelFormat(Format.Unknown, AlphaMode.Premultiplied)));
            
            //WEB_SOCKET
            Connect();
            System.Threading.Tasks.Task.Delay(1000);

            _gameRender = new GameRender(RenderForm, Factory2D, RenderTarget2D);
            
            _directInput = new DirectInput();
            _keyboard = new Keyboard(_directInput);
            _keyboard.Properties.BufferSize = 128; 
            _keyboard.Acquire();
        }

        public void RunGame()
        {
            RenderLoop.Run(RenderForm, Draw);
        }

        public void Draw()
        {
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
                    }
                    else
                    {
                        _isFPressed = false;
                    }
                }
                else if (key == Key.Return)
                {
                    _isEnterPressed = true;
                }
            }
            
            //Drawing a gama
            if (_isEnterPressed)
            {
                _gameRender.DrawMap(_spectatorClass.Map);
                _gameRender.DrawInteractiveObjects(_spectatorClass.Map.InteractObjects);
                _gameRender.DrawClientInfo();
            }

            if (_spectatorClass.Map != null && !_isEnterPressed)
            {
                _gameRender.DrawLogo();
            }

            if (_spectatorClass.Map == null)
            {
                _gameRender.DrawWaitingLogo();
            }
            
            if (_isTabPressed)
            {
                //_gameRender.DrawClientsInfo();
            }
            if (_isFPressed)
            {
                _gameRender.DrawFPS();
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
            _gameRender.Dispose();
        }

    }
}
