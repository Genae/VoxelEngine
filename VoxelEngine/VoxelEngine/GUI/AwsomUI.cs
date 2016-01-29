using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Awesomium.Core;
using OpenTK;
using FrameEventArgs = OpenTK.FrameEventArgs;
using MouseButton = OpenTK.Input.MouseButton;

namespace VoxelEngine.Client.GUI
{
    public class AwsomUI : UiElement
    {
        //WebView
        private bool _hasFocus;
        private WebView _webView;
        private JSObject _jsObject;
        private List<Callback> Callbacks = new List<Callback>();
        //Others

        #region JSFunctions
        const string PageHeightFunc = "(function() { " +
            "var bodyElmnt = document.body; var html = document.documentElement; " +
            "var height = Math.max( bodyElmnt.scrollHeight, bodyElmnt.offsetHeight, html.clientHeight, html.scrollHeight, html.offsetHeight ); " +
            "return height; })();";
        #endregion

        #region Creation
        public AwsomUI(string url, Position position) : base(position)
        {
            Engine.Instance.EnsureWebCore();
            WebCore.QueueWork(()=>CreateView(url, this));
        }

        private static void CreateView(string url, AwsomUI context)
        {
            var view = WebCore.CreateWebView(context.Position.Width, context.Position.Height, WebCore.Sessions.Last());
            view.IsTransparent = true;
            view.Source = new Uri("file:///" + new FileInfo(url).FullName);

            view.LoadingFrameComplete += (s, e) =>
            {
                if (!e.IsMainFrame)
                    return;
                
                // Take snapshots of the page.
                CreateTexture((WebView)s, context);
                var surface = ((BitmapSurface)view.Surface);
                surface.Updated += (sender, args) =>
                {
                    CreateTexture((WebView)s, context);
                };

                context._jsObject = view.CreateGlobalJavascriptObject("cSharp");
            };
            context._webView = view;
        }

        public void BindCallback(Callback cb)
        {
            Callbacks.Add(cb);
        }
        #endregion

        #region Rendering
        private static void CreateTexture(WebView view, AwsomUI context, bool resize = false)
        {
            lock (context.Lock)
            {
                if (!view.IsLive)
                {
                    // Dispose the view.
                    view.Dispose();
                    return;
                }
                if (!view.IsDocumentReady)
                    return;

                if (resize)
                {
                    var dh = view.ExecuteJavascriptWithResult(PageHeightFunc);
                    var docHeight = dh.IsInteger ? (int)dh : 0;

                    Error lastError = view.GetLastError();

                    // Report errors.
                    if (lastError != Error.None)
                        Console.WriteLine("Error: {0} occurred while getting the page's height.", lastError);

                    // Exit if the operation failed or the height is 0.
                    if (docHeight != 0)
                    {
                        if (docHeight != view.Height)
                        {
                            view.Resize(view.Width, docHeight);
                            return;
                        }
                    }
                }
                
                var b = new Bitmap(view.Width, view.Height, PixelFormat.Format32bppArgb);
                var bits0 = b.LockBits(
                    new Rectangle(0, 0, view.Width, view.Height),
                    ImageLockMode.ReadWrite, b.PixelFormat);
                ((BitmapSurface)view.Surface).CopyTo(bits0.Scan0, bits0.Stride, 4, false, false);
                b.UnlockBits(bits0);
                context.Image = b;
                context.IsDirty = true;
            }
        }
        #endregion

        #region Update
        public override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            if (Input.Input.IsMouseInRect(Position, true))
            {
                _hasFocus = true;
                WebCore.QueueWork(InjectMouse);
            }
            else
            {
                if (_hasFocus) //lost focus this frame
                {
                    WebCore.QueueWork(ResetMouse);
                    _hasFocus = false;
                }
            }

            if(_jsObject != null)
            WebCore.QueueWork(() =>
            {
                foreach (var cb in Callbacks.Where(c => !c.Bound))
                {
                    _jsObject.Bind(cb.MethodName, cb.HasReturnValue, cb.Handler);
                }
            });
        }

        private void InjectMouse()
        {
            var mousePos = UiToWebView(Input.Input.GetMousePosition(true));
            _webView.InjectMouseMove((int) mousePos.X, (int) mousePos.Y);
            for (var i = 0; i < 3; i++)
            {
                if (Input.Input.GetMouseButtonDown((MouseButton)i))
                {
                    _webView.InjectMouseDown((Awesomium.Core.MouseButton)i);
                }
            }
            for (var i = 0; i < 3; i++)
            {
                if (Input.Input.GetMouseButtonUp((MouseButton)i))
                {
                    _webView.InjectMouseUp((Awesomium.Core.MouseButton)i);
                }
            }
        }
        private void ResetMouse()
        {
            _webView.InjectMouseMove(0,0);
            for (var i = 0; i < 3; i++)
            {
                if (Input.Input.GetMouseButtonUp((MouseButton)i))
                {
                    _webView.InjectMouseUp((Awesomium.Core.MouseButton)i);
                }
            }
        }

        private Vector2 UiToWebView(Vector2 pos)
        {
            var relX = (pos.X - Position.X)/Position.Width;
            var relY = 1 - (pos.Y - Position.Y)/Position.Height;
            return new Vector2(relX * _webView.Width, relY * _webView.Height);
        }

        #endregion
    }

    public class Callback
    {
        public Callback(string methodName, bool hasReturnValue, JavascriptMethodEventHandler handler)
        {
            MethodName = methodName;
            HasReturnValue = hasReturnValue;
            Handler = handler;
        }

        public bool Bound { get; set; }
        public string MethodName { get; set; }
        public bool HasReturnValue { get; set; }
        public JavascriptMethodEventHandler Handler { get; set; }
    }
}
