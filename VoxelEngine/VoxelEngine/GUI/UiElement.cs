using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace VoxelEngine.Client.GUI
{
    public class UiElement
    {
        protected Position Position;
        protected Bitmap Image;
        protected int TextureId;
        protected bool IsDirty;

        protected readonly object Lock = new object();

        public UiElement(Position position)
        {
            Position = position;

            Engine.Instance.UIElements.Add(this);
        }

        public virtual void OnRenderFrame(FrameEventArgs args)
        {
            if (Image == null)
                return;
            if (IsDirty)
            {
                lock (Lock)
                {
                    TextureId = TexUtil.BitmapToTexture(Image);
                }
            }

            GL.BindTexture(TextureTarget.Texture2D, TextureId);

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

        public virtual void OnUpdateFrame(FrameEventArgs e)
        {
            
        }
    }
}
