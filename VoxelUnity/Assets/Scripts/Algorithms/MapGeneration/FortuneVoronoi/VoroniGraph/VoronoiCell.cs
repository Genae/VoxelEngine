using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Algorithms.MapGeneration.FortuneVoronoi.VoroniGraph
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
            v.x = Math.Max(v.x, 0);
            v.x = Math.Min(v.x, xSize);
            v.y = Math.Max(v.y, 0);
            v.y = Math.Min(v.y, ySize);
            return v;
        }
    }
}