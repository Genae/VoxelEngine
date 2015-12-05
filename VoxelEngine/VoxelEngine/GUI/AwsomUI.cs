using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using Awesomium.Core;
using OpenTK.Graphics.OpenGL;
using FrameEventArgs = OpenTK.FrameEventArgs;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace VoxelEngine.GUI
{
    public class AwsomUI
    {
        private Bitmap _buffer;
        private int _textureId;
        private bool _isDirty;
        protected Rectangle Position;

        #region JSFunctions
        const string PAGE_HEIGHT_FUNC = "(function() { " +
            "var bodyElmnt = document.body; var html = document.documentElement; " +
            "var height = Math.max( bodyElmnt.scrollHeight, bodyElmnt.offsetHeight, html.clientHeight, html.scrollHeight, html.offsetHeight ); " +
            "return height; })();";
        #endregion

        public AwsomUI(string url, Rectangle position)
        {
            WebCore.QueueWork(()=>CreateView(url, this));
            Position = position;
            Engine.Instance.ui.Add(this);
        }

        private static void CreateView(string url, AwsomUI context)
        {
            var view = WebCore.CreateWebView(context.Position.Width, context.Position.Height, WebCore.Sessions.Last());
            view.IsTransparent = true;
            view.Source = new Uri("file:///" + new FileInfo(url).FullName);

            /*var surface = ((BitmapSurface) view.Surface);
            surface.Updated += (sender, args) =>
            {
                GenTex(view, args.DirtyRegion);
            };*/

            view.LoadingFrameComplete += (s, e) =>
            {
                if (!e.IsMainFrame)
                    return;
                
                // Take snapshots of the page.
                CreateTexture((WebView)s, context);
            };
        }

        private static void CreateTexture(WebView view, AwsomUI context)
        {
            if (!view.IsLive)
            {
                // Dispose the view.
                view.Dispose();
                return;
            }
            var surface = (BitmapSurface)view.Surface;
            var docHeight = (int) view.ExecuteJavascriptWithResult(PAGE_HEIGHT_FUNC);

            Error lastError = view.GetLastError();

            // Report errors.
            if (lastError != Error.None)
                Console.WriteLine("Error: {0} occurred while getting the page's height.", lastError);

            // Exit if the operation failed or the height is 0.
            if (docHeight == 0)
                return;

            if (docHeight != view.Height)
            {
                view.Resize(view.Width, docHeight);
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

        public void OnRenderFrame(FrameEventArgs args)
        {
            if (_buffer == null)
                return;
            if(_isDirty)
                _textureId = TexUtil.BitmapToTexture(_buffer);

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
    }
}
