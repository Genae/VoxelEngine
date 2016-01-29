using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace TestGame.MapGeneration.FortuneVoronoi.VoroniGraph
{
    public class VoronoiCell
    {
        private readonly List<VoronoiEdge> _edges;
        private readonly Peak _peak;

        public VoronoiCell(List<VoronoiEdge> edges, Peak peak)
        {
            _edges = edges;
            _peak = peak;
        }

        public void Draw(VoronoiTriangle[,] canvas)
        {
            foreach (var tri in SplitIntoTriangles(canvas.GetLength(0), canvas.GetLength(1)))
            {
                tri.Draw(canvas);   
            }
        }

        private IEnumerable<VoronoiTriangle> SplitIntoTriangles(int xSize, int ySize)
        {
            return _edges.Select(voronoiEdge => new VoronoiTriangle(new List<Vector2>() { CutAtEdge(voronoiEdge.V1, xSize, ySize), CutAtEdge(voronoiEdge.V2, xSize, ySize), _peak.PeakPoint }, 
                                                                    new Peak(voronoiEdge.LeftData), 
                                                                    new Peak(voronoiEdge.RightData), 
                                                                    this)).ToList();
        }

        private Vector2 CutAtEdge(Vector2 v, int xSize, int ySize)
        {
            v.X = Math.Max(v.X, 0);
            v.X = Math.Min(v.X, xSize);
            v.Y = Math.Max(v.Y, 0);
            v.Y = Math.Min(v.Y, ySize);
            return v;
        }
    }
}