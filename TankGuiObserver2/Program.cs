namespace TankGuiObserver2
{
    using System;
    using SharpDX;
    using SharpDX.Direct2D1;
    using SharpDX.DirectWrite;
    using SharpDX.Windows;
    using System.Windows.Forms;

    using D2DFactory = SharpDX.Direct2D1.Factory;
    using DWriteFactory = SharpDX.DirectWrite.Factory;

    
    //
    //public class Program
    //{
    //    private static D2DFactory d2dFactory;
    //    private static DWriteFactory dwFactory;
    //    private static RenderForm mainForm;
    //
    //    private static WindowRenderTarget renderTarget;
    //
    //    private static TextFormat textFormat;
    //    private static TextLayout textLayout;
    //
    //    //Various brushes for our example
    //    private static SolidColorBrush backgroundBrush;
    //    private static SolidColorBrush defaultBrush;
    //    private static SolidColorBrush greenBrush;
    //    private static SolidColorBrush redBrush;
    //
    //    private static CustomColorRenderer textRenderer;
    //
    //
    //    private static RectangleF fullTextBackground;
    //
    //    //This one is only a measured region
    //    private static RectangleF textRegionRect;
    //
    //
    //    private static string introText = @"Hello from SharpDX, this is a long text to show some more advanced features like paragraph alignment, custom drawing...";
    //
    //    [STAThread]
    //    static void Main(string[] args)
    //    {
    //        mainForm = new RenderForm("Advanced Text rendering demo");
    //
    //        d2dFactory = new D2DFactory();
    //        dwFactory = new DWriteFactory(SharpDX.DirectWrite.FactoryType.Shared);
    //
    //        textRenderer = new CustomColorRenderer();
    //
    //        CreateResources();
    //
    //        var bgcolor = new Color4(0.1f, 0.1f, 0.1f, 1.0f);
    //
    //        //This is the offset where we start our text layout
    //        Vector2 offset = new Vector2(202.0f, 250.0f);
    //
    //        textFormat = new TextFormat(dwFactory, "Arial", FontWeight.Regular, FontStyle.Normal, 16.0f);
    //        textLayout = new TextLayout(dwFactory, introText, textFormat, 300.0f, 200.0f);
    //
    //        //Apply various modifications to text
    //        textLayout.SetUnderline(true, new TextRange(0, 5));
    //        textLayout.SetDrawingEffect(greenBrush, new TextRange(10, 20));
    //        textLayout.SetFontSize(24.0f, new TextRange(6, 4));
    //        textLayout.SetFontFamilyName("Comic Sans MS", new TextRange(11, 7));
    //
    //        //Measure full layout
    //        var textSize = textLayout.Metrics;
    //        fullTextBackground = new RectangleF(textSize.Left + offset.X, 
    //            textSize.Top + offset.Y, textSize.Width, textSize.Height);
    //
    //        //Measure text to apply background to
    //        var metrics = textLayout.HitTestTextRange(53, 4, 0.0f, 0.0f)[0];
    //        textRegionRect = new RectangleF(metrics.Left + offset.X, metrics.Top + offset.Y, metrics.Width, metrics.Height);
    //
    //        //Assign render target and brush to our custom renderer
    //        textRenderer.AssignResources(renderTarget, defaultBrush);
    //
    //        RenderLoop.Run(mainForm, () =>
    //        {
    //            renderTarget.BeginDraw();
    //            renderTarget.Clear(bgcolor);
    //
    //            renderTarget.FillRectangle(fullTextBackground, backgroundBrush);
    //
    //            renderTarget.FillRectangle(textRegionRect, redBrush);
    //
    //            textLayout.Draw(textRenderer, offset.X, offset.Y);
    //
    //            try
    //            {
    //                renderTarget.EndDraw();
    //            }
    //            catch
    //            {
    //                CreateResources();
    //            }
    //        });
    //
    //        d2dFactory.Dispose();
    //        dwFactory.Dispose();
    //        renderTarget.Dispose();
    //    }
    //
    //    private static void CreateResources()
    //    {
    //        if (renderTarget != null) { renderTarget.Dispose(); }
    //        if (defaultBrush != null) { defaultBrush.Dispose(); }
    //        if (greenBrush != null) { greenBrush.Dispose(); }
    //        if (redBrush != null) { redBrush.Dispose(); }
    //        if (backgroundBrush != null) { backgroundBrush.Dispose(); }
    //
    //
    //        HwndRenderTargetProperties wtp = new HwndRenderTargetProperties();
    //        wtp.Hwnd = mainForm.Handle;
    //        wtp.PixelSize = new Size2(mainForm.Width, mainForm.Height);
    //        wtp.PresentOptions = PresentOptions.Immediately;
    //        renderTarget = new WindowRenderTarget(d2dFactory, new RenderTargetProperties(), wtp);
    //
    //        defaultBrush = new SolidColorBrush(renderTarget, Color.White);
    //        greenBrush = new SolidColorBrush(renderTarget, Color.Green);
    //        redBrush = new SolidColorBrush(renderTarget, Color.Red);
    //        backgroundBrush = new SolidColorBrush(renderTarget, new Color4(0.3f, 0.3f, 0.3f, 0.5f));
    //
    //        textRenderer.AssignResources(renderTarget, defaultBrush);
    //
    //    }
    //}

    class Program
    {
        static void Main(string[] args)
        {
            using (Game game = new Game("Battle city v0.1", 1920, 1080, true))
            {
                game.RunGame();
            }
    
        }
    }
}
