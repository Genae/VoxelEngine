using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using Img = System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace VoxelEngine.Client.GUI
{
    public static class TexUtil
    {
        public static int LoadTexture(string file, bool argb)
        {
            Bitmap bitmap;
            if (argb)
            {
                byte[] B = File.ReadAllBytes(new FileInfo(file).FullName);
                GCHandle GCH = GCHandle.Alloc(B, GCHandleType.Pinned);
                IntPtr Scan0 = (IntPtr)((int)(GCH.AddrOfPinnedObject()) + 54);
                int W = Marshal.ReadInt32(Scan0, -36);
                int H = Marshal.ReadInt32(Scan0, -32);
                bitmap = new Bitmap(W, H, 4 * W, Img.PixelFormat.Format32bppArgb, Scan0);
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                GCH.Free();
            }
            else
            {
                bitmap = new Bitmap(new FileInfo(file).FullName);
            }
            return BitmapToTexture(bitmap);
        }

        public static int BitmapToTexture(Bitmap bitmap)
        {
            int tex;
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out tex);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            Img.BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                Img.ImageLockMode.ReadOnly, Img.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            return tex;
        }
    }
}