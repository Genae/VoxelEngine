using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Awesomium.Core;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using FrameEventArgs = OpenTK.FrameEventArgs;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace VoxelEngine.GUI
{
    public class AwsomUI
    {
        //WebView
        private Bitmap _buffer;
        private int _textureId;
        private bool _isDirty;
        private bool _hasFocus;
        private WebView _webView;
        //Others
        protected Rectangle Position;
        private readonly object _lock = new object();

        #region JSFunctions
        const string PAGE_HEIGHT_FUNC = "(function() { " +
            "var bodyElmnt = document.body; var html = document.documentElement; " +
            "var height = Math.max( bodyElmnt.scrollHeight, bodyElmnt.offsetHeight, html.clientHeight, html.scrollHeight, html.offsetHeight ); " +
            "return height; })();";
        #endregion

        #region Creation
        public AwsomUI(string url, Rectangle position)
        {
            Engine.Instance.EnsureWebCore();
            WebCore.QueueWork(()=>CreateView(url, this));
            Position = position;
            Engine.Instance.ui.Add(this);
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
            };
            context._webView = view;
        }
        #endregion
        
        #region Rendering
        private static void CreateTexture(WebView view, AwsomUI context, bool resize = false)
        {
            lock (context._lock)
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
                    var dh = view.ExecuteJavascriptWithResult(PAGE_HEIGHT_FUNC);
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
                context._buffer = b;
                context._isDirty = true;
            }
        }

        public void OnRenderFrame(FrameEventArgs args)
        {
            if (_buffer == null)
                return;
            if (_isDirty)
            {
                lock (_lock)
                {
                    _textureId = TexUtil.BitmapToTexture(_buffer);
                }
            }

            GL.BindTexture(TextureTarget.Texture2D, _textureId);

            GL.Begin(BeginMode.Quads);

            GL.TexCoord2(0, 1);
            GL.Vertex3(Position.X, Position.Y, 0);

            GL.TexCoord2(1, 1);
            GL.Vertex3(Position.X + Position.Width, Position.Y, 0);

            GL.TexCoord2(1, 0);
            GL.Vertex3(Position.X + Position.Width, Position.Y + Position.Height, 0);

            GL.TexCoord2(0, 0);
            GL.Vertex3(Position.X, Position.Y + Position.Height, 0);

            GL.End();
        }
        #endregion

        #region Update

        public void OnUpdateFrame(FrameEventArgs e)
        {
            if (Input.Input.IsMouseInRect(Position, true))
            {
                _hasFocus = true;
                WebCore.QueueWork(() => InjectMouse());
            }
            else
            {
                if (_hasFocus) //lost focus this frame
                {
                    WebCore.QueueWork(ResetMouse);
                    _hasFocus = false;
                }
            }
        }

        private void InjectMouse()
        {
            var mousePos = UiToWebView(Input.Input.GetMousePosition(true));
            _webView.InjectMouseMove((int) mousePos.X, (int) mousePos.Y);
            for (var i = 0; i < 3; i++)
            {
                if (Input.Input.GetMouseButtonDown((OpenTK.Input.MouseButton)i))
                {
                    _webView.InjectMouseDown((MouseButton)i);
                    Console.WriteLine("Click");
                }
            }
            for (var i = 0; i < 3; i++)
            {
                if (Input.Input.GetMouseButtonUp((OpenTK.Input.MouseButton)i))
                {
                    _webView.InjectMouseUp((MouseButton)i);
                }
            }
        }
        private void ResetMouse()
        {
            _webView.InjectMouseMove(0,0);
            for (var i = 0; i < 3; i++)
            {
                if (Input.Input.GetMouseButtonUp((OpenTK.Input.MouseButton)i))
                {
                    _webView.InjectMouseUp((MouseButton)i);
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
}
