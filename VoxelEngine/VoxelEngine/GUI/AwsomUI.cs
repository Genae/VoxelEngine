using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using Awesomium.Core;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace VoxelEngine.GUI
{
    public class AwsomUI
    {
        private WebView view;
        private BitmapSurface surface;
        private int tex;
        private Thread t;
        public AwsomUI()
        {
            //tex = TexUtil.LoadTexture(@"GUI\Unbenannt.bmp", true);
            WebSession session = WebCore.CreateWebSession(new WebPreferences(){CustomCSS = "::-webkit-scrollbar { visibility: hidden; }"});
            view = WebCore.CreateWebView(Engine.Instance.Width/2, Engine.Instance.Height/2, session);
            view.IsTransparent = true;

            view.LoadingFrameComplete += (s, e) =>
            {
                if (!e.IsMainFrame)
                    return;
                GenTex((BitmapSurface)view.Surface, new AweRect(0,0,view.Width, view.Height));
                surface = (BitmapSurface) view.Surface;
                surface.SaveToPNG(@"C:\test\test.png");

                surface.Updated += (sender, args) =>
                {
                    GenTex((BitmapSurface) view.Surface, args.DirtyRegion);
                };
            };
        }

        private void GenTex(BitmapSurface surface, AweRect bounds)
        {
            Bitmap b = new Bitmap(view.Width, view.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            BitmapData bits0 = b.LockBits(
                new System.Drawing.Rectangle(0, 0, view.Width, view.Height),
                ImageLockMode.ReadWrite, b.PixelFormat);
            surface.CopyTo(bits0.Scan0, bits0.Stride, 4, false, false);
            b.UnlockBits(bits0);
            //b.Save(@"C:\test\test.bmp");
            tex = TexUtil.BitmapToTexture(b);
        }

        public void SetFPS(int fps)
        {
            view.LoadHTML("<html><head></head><body style=\"color: red; font-size: 40pt; font-family:Comic Sans MS\">FPS: " + fps + "</body></html>");
        }

        public void OnRenderFrame(OpenTK.FrameEventArgs args)
        {
            WebCore.Update();
            if (surface == null)
            {
                return;
            }
            DrawImage(tex);
        }

        public  void DrawImage(int image)
        {
            //GL.Color4(Color.Transparent);

            GL.BindTexture(TextureTarget.Texture2D, image);

            GL.Begin(BeginMode.Quads);

            GL.TexCoord2(0, 1);
            GL.Vertex3(1, 1, 0);

            GL.TexCoord2(1, 1);
            GL.Vertex3(view.Width, 1, 0);

            GL.TexCoord2(1, 0);
            GL.Vertex3(view.Width, view.Height, 0);

            GL.TexCoord2(0, 0);
            GL.Vertex3(1, view.Height, 0);

            GL.End();
        }
    }
}
