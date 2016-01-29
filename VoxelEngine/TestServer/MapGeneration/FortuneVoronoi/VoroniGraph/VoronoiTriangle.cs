using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace TestGame.MapGeneration.FortuneVoronoi.VoroniGraph
{
    public class VoronoiTriangle
    {
        private Vector2 _v1, _v2, _v3;
        internal readonly Peak MainPeak;
        internal readonly Peak SecondaryPeak;
        internal readonly VoronoiCell Cell;

        public VoronoiTriangle(IEnumerable<Vector2> vectors, Peak mainPeak, Peak secondaryPeak, VoronoiCell parent)
        {
            var sorted = new SortedList<float, Vector2>();
            foreach (var vector2 in vectors)
            {
                float pos = vector2.Y;
                while(sorted.ContainsKey(pos))
                {
                    pos += 0.0000001f;
                }
                sorted.Add(pos, vector2);
            }
            _v1 = sorted.ElementAt(0).Value;
            _v2 = sorted.ElementAt(1).Value;
            _v3 = sorted.ElementAt(2).Value;

            MainPeak = mainPeak;
            SecondaryPeak = secondaryPeak;
            Cell = parent;
        }

        internal void Draw(VoronoiTriangle[,] canvas)
        {
            //DrawTriangle(canvas);
        }

        private void FillBottomFlatTriangle(Vector2 v1, Vector2 v2, Vector2 v3, VoronoiTriangle[,] canvas)
        {
            var invslope1 = (v2.X - v1.X) / (v2.Y - v1.Y);
            var invslope2 = (v3.X - v1.X) / (v3.Y - v1.Y);

            var curx1 = v1.X;
            var curx2 = v1.X;

            for (var scanlineY = v1.Y; scanlineY <= v2.Y; scanlineY++)
            {
                DrawLine((int)curx1, (int)curx2, (int)scanlineY, canvas);
                curx1 += invslope1;
                curx2 += invslope2;
            }
        }

        private void FillTopFlatTriangle(Vector2 v1, Vector2 v2, Vector2 v3, VoronoiTriangle[,] canvas)
        {
            var invslope1 = (v3.X - v1.X) / (v3.Y - v1.Y);
            var invslope2 = (v3.X - v2.X) / (v3.Y - v2.Y);

            var curx1 = v3.X;
            var curx2 = v3.X;

            for (var scanlineY = v3.Y; scanlineY > v1.Y; scanlineY--)
            {
                curx1 -= invslope1;
                curx2 -= invslope2;
                DrawLine((int)curx1, (int)curx2, (int)scanlineY, canvas);
            }
        }

        private void DrawTriangle(VoronoiTriangle[,] canvas)
        {

            /* here we know that v1.y <= v2.y <= v3.y */
            /* check for trivial case of bottom-flat triangle */
            if ((int)_v2.Y == (int)_v3.Y)
            {
                FillBottomFlatTriangle(_v1, _v2, _v3, canvas);
            }
            /* check for trivial case of top-flat triangle */
            else if ((int)_v1.Y == (int)_v2.Y)
            {
                FillTopFlatTriangle(_v1, _v2, _v3, canvas);
            }
            else
            {
                /* general case - split the triangle in a topflat and bottom-flat one */
                var v4 = new Vector2((int)(_v1.X + ((_v2.Y - _v1.Y) / (_v3.Y - _v1.Y)) * (_v3.X - _v1.X)), _v2.Y);
                FillBottomFlatTriangle(_v1, _v2, v4, canvas);
                FillTopFlatTriangle(_v2, v4, _v3, canvas);
            }
        }

        private void DrawLine(int x1, int x2, int y, VoronoiTriangle[,] canvas)
        {
            if (y >= canvas.GetLength(1) || (int) y < 0) return;
            for (var x = Math.Max(x1, 0); x <= Math.Min(x2, canvas.GetLength(0)); x++)
            {
                canvas[x, y] = this;
            }
        }
    }
}
