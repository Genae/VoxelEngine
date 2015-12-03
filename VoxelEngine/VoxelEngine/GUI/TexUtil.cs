using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using Img = System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace VoxelEngine.GUI
{
    public static class TexUtil
    {
        #region Public
        public static void InitTexturing()
        {
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
        }

        public static int CreateRGBTexture(int width, int height, byte[] rgb)
        {
            return CreateTexture(width, height, false, rgb);
        }

        public static int CreateRGBATexture(int width, int height, byte[] rgba)
        {
            return CreateTexture(width, height, true, rgba);
        }

        public static int CreateTextureFromBitmap(Bitmap bitmap)
        {
            Img.BitmapData data = bitmap.LockBits(
              new Rectangle(0, 0, bitmap.Width, bitmap.Height),
              Img.ImageLockMode.ReadOnly,
              Img.PixelFormat.Format32bppArgb);
            var tex = GiveMeATexture();
            GL.BindTexture(TextureTarget.Texture2D, tex);
            GL.TexImage2D(
              TextureTarget.Texture2D,
              0,
              PixelInternalFormat.Rgba,
              data.Width, data.Height,
              0,
              PixelFormat.Bgra,
              PixelType.UnsignedByte,
              data.Scan0);
            bitmap.UnlockBits(data);
            SetParameters();
            return tex;
        }

        public static int CreateTextureFromFile(string path)
        {
            return CreateTextureFromBitmap(new Bitmap(Bitmap.FromFile(new FileInfo(path).FullName)));
        }

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
            int tex;
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out tex);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            Img.BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                Img.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            return tex;
        }

        #endregion
        private static int CreateTexture(int width, int height, bool alpha, byte[] bytes)
        {
            int expectedBytes = width * height * (alpha ? 4 : 3);
            Debug.Assert(expectedBytes == bytes.Length);
            int tex = GiveMeATexture();
            Upload(width, height, alpha, bytes);
            SetParameters();
            return tex;
        }

        private static int GiveMeATexture()
        {
            int tex = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, tex);
            return tex;
        }

        private static void SetParameters()
        {
            GL.TexParameter(
              TextureTarget.Texture2D,
              TextureParameterName.TextureMinFilter,
              (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D,
              TextureParameterName.TextureMagFilter,
              (int)TextureMagFilter.Nearest);
        }

        private static void Upload(int width, int height, bool alpha, byte[] bytes)
        {
            var internalFormat = alpha ? PixelInternalFormat.Rgba : PixelInternalFormat.Rgb;
            var format = alpha ? PixelFormat.Rgba : PixelFormat.Rgb;
            GL.TexImage2D<byte>(
              TextureTarget.Texture2D,
              0,
              internalFormat,
              width, height,
              0,
              format,
              PixelType.UnsignedByte,
              bytes);
        }
    }
}