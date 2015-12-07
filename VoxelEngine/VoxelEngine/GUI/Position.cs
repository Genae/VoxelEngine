using System;
using System.Drawing;

namespace VoxelEngine.GUI
{
    public class Position
    {
        public virtual int X { get; protected set; }
        public virtual int Y { get; protected set; }
        public virtual int Width { get; protected set; }
        public virtual int Height { get; protected set; }

        public Position(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        protected Position(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public static implicit operator Rectangle(Position rect)
        {
            return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }
        public static implicit operator Position(Rectangle rect)
        {
            return new Position(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }

    public class RelativePosition : Position
    {
        public AnchorPoint Anchor { get; set; }

        public override int X => GetX();
        public override int Y => GetY();

        public RelativePosition(AnchorPoint anchor, int width, int height) : base(width, height)
        {
            Anchor = anchor;
        }

        private int GetX()
        {
            switch (Anchor)
            {
                case AnchorPoint.TopLeft:
                    return 0;
                case AnchorPoint.TopCenter:
                    return (Engine.Instance.ClientRectangle.Width - Width)/2;
                case AnchorPoint.TopRight:
                    return Engine.Instance.ClientRectangle.Width-Width;
                case AnchorPoint.CenterLeft:
                    return 0;
                case AnchorPoint.Center:
                    return (Engine.Instance.ClientRectangle.Width - Width) / 2;
                case AnchorPoint.CenterRight:
                    return Engine.Instance.ClientRectangle.Width - Width;
                case AnchorPoint.BottomLeft:
                    return 0;
                case AnchorPoint.BottomCenter:
                    return (Engine.Instance.ClientRectangle.Width - Width) / 2;
                case AnchorPoint.BottomRight:
                    return Engine.Instance.ClientRectangle.Width - Width;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private int GetY()
        {
            switch (Anchor)
            {
                case AnchorPoint.TopLeft:
                    return Engine.Instance.ClientRectangle.Height - Height;
                case AnchorPoint.TopCenter:
                    return Engine.Instance.ClientRectangle.Height - Height;
                case AnchorPoint.TopRight:
                    return Engine.Instance.ClientRectangle.Height - Height;
                case AnchorPoint.CenterLeft:
                    return (Engine.Instance.ClientRectangle.Height - Height)/2;
                case AnchorPoint.Center:
                    return (Engine.Instance.ClientRectangle.Height - Height) / 2;
                case AnchorPoint.CenterRight:
                    return (Engine.Instance.ClientRectangle.Height - Height) / 2;
                case AnchorPoint.BottomLeft:
                    return 0;
                case AnchorPoint.BottomCenter:
                    return 0;
                case AnchorPoint.BottomRight:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public enum AnchorPoint
        {
            TopLeft,
            TopCenter,
            TopRight,
            CenterLeft,
            Center,
            CenterRight,
            BottomLeft,
            BottomCenter,
            BottomRight
        }
    }
}
