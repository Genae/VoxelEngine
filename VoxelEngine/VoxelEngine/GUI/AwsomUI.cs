using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Awesomium.Core;
using OpenTK.Graphics.OpenGL;

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
            tex = TexUtil.LoadTexture(@"GUI\Unbenannt.bmp", true);
            WebSession session = WebCore.CreateWebSession(new WebPreferences(){CustomCSS = "::-webkit-scrollbar { visibility: hidden; }"});
            view = WebCore.CreateWebView(Engine.Instance.Width, Engine.Instance.Height, session);
            view.Source = new Uri("http://www.google.de");

            view.LoadingFrameComplete += (s, e) =>
            {
                if (!e.IsMainFrame)
                    return;
                //GenTex((BitmapSurface)view.Surface);
                surface = (BitmapSurface) view.Surface;
                surface.SaveToPNG(@"C:\test\test.png");
            };
        }

        private void GenTex(BitmapSurface surface)
        {
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out tex);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, view.Width, view.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, surface.Buffer);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        }

        public void OnRenderFrame(OpenTK.FrameEventArgs args)
        {
            /*WebCore.Update();
            if (surface == null)
            {
                return;
            }*/
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
            GL.Vertex3(view.Width/2, 1, 0);

            GL.TexCoord2(1, 0);
            GL.Vertex3(view.Width/2, view.Height/2, 0);

            GL.TexCoord2(0, 0);
            GL.Vertex3(1, view.Height/2, 0);

            GL.End();
        }
    }
}
