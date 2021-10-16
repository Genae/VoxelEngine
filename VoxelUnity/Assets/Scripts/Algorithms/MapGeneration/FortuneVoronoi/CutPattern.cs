using System.Collections.Generic;
using System.Linq;
using Algorithms.MapGeneration.FortuneVoronoi.VoroniGraph;
using UnityEngine;

namespace Algorithms.MapGeneration.FortuneVoronoi
{
    class CutPattern
    {
        private readonly VoronoiGraph _graph;
        private readonly int _width;
        private readonly int _height;
        private readonly IEnumerable<Vector2> _peaks;

        public CutPattern(VoronoiGraph graph, int width, int height, IEnumerable<Vector2> peaks)
        {
            _graph = graph;
            _width = width;
            _height = height;
            _peaks = peaks;
        }

        public float[,] GenerateCutPattern()
        {
            var cutPattern = WhiteCanvas();
            RemoveEdges();
            foreach (var voronoiEdge in _graph.GetEdges())
            {
                DrawEdge(cutPattern, voronoiEdge as VoronoiEdge);
            }
            FillAllBlack(cutPattern);
            foreach (var vector in _peaks)
            {
                cutPattern[(int) vector[0], (int) vector[1]] = 0.7f;
            }
            foreach (var vector in _graph.GetVertices())
            {
                if ((int)vector[0] < 0 || (int)vector[1] < 0 || (int)vector[0] >= cutPattern.GetLength(0) || (int)vector[1] >= cutPattern.GetLength(1))
                    continue;
                cutPattern[(int)vector[0], (int)vector[1]] = 0.3f;
            }
            return cutPattern;
        }

        private void FillAllBlack(float[,] cutPattern)
        {
            for (var x = 0; x < cutPattern.GetLength(0); x++)
            {
                FillBlack(cutPattern, x, 0);
                FillBlack(cutPattern, x, cutPattern.GetLength(1) - 1);
            }
            for (var y = 0; y < cutPattern.GetLength(1); y++)
            {
                FillBlack(cutPattern, 0, y);
                FillBlack(cutPattern, cutPattern.GetLength(0)-1, y);
            }
        }

        private void RemoveEdges()
        {
            foreach (var voronoiEdge in _graph.GetEdges().ToArray().Where(voronoiEdge => voronoiEdge.IsPartlyInfinite || voronoiEdge.IsInfinite))
            {
                _graph.RemoveEdge(voronoiEdge, true);
            }

            var amount = new Dictionary<Vector2, int>();
            foreach (var voronoiEdge in _graph.GetEdges())
            {
                if (!amount.ContainsKey(voronoiEdge.V1)) amount.Add(voronoiEdge.V1, 0);
                if (!amount.ContainsKey(voronoiEdge.V2)) amount.Add(voronoiEdge.V2, 0);
                amount[voronoiEdge.V1]++;
                amount[voronoiEdge.V2]++;
            }
           
            while (amount.Any(x => x.Value <= 1))
            {
                var killIt = amount.FirstOrDefault(x => x.Value <= 1);
                if (amount[killIt.Key] == 0)
                {
                    amount.Remove(killIt.Key);
                    continue;
                }
                var edge = _graph.GetEdges().First(x => x.V1.Equals(killIt.Key) || x.V2.Equals(killIt.Key));
                _graph.RemoveEdge(edge, true);
                amount[edge.V1]--;
                amount[edge.V2]--;
                if (amount[killIt.Key] == 0)
                {
                    amount.Remove(killIt.Key);
                }
            }
        }

        private float[,] WhiteCanvas()
        {
            var canvas = new float[_width, _height];
            for (var x = 0; x < canvas.GetLength(0); x++)
            {
                for (var y = 0; y < canvas.GetLength(1); y++)
                {
                    canvas[x, y] = 1;
                }
            }
            return canvas;
        }

        private void DrawEdge(float[,] cutPattern, VoronoiEdge voronoiEdge)
        {
            if (voronoiEdge.IsPartlyInfinite || voronoiEdge.IsInfinite) return;
            var dirvec = voronoiEdge.V2 - voronoiEdge.V1;
            var myDir = new Vector2((float) dirvec[0], (float) dirvec[1]);
            //Debug.Log("Left: " + voronoiEdge.V2);
            //Debug.Log("Right: " + voronoiEdge.V1);
            while (myDir.magnitude != 0)
            {
                if ((int) (voronoiEdge.V1[0] + myDir.x) < 0 ||
                    (int) (voronoiEdge.V1[1] + myDir.y) < 0 ||
                    (int) (voronoiEdge.V1[0] + myDir.x) >= cutPattern.GetLength(0) ||
                    (int) (voronoiEdge.V1[1] + myDir.y) >= cutPattern.GetLength(1))
                {
                    myDir *= 0;
                    continue;
                }
                cutPattern[(int)(voronoiEdge.V1[0] + myDir.x), (int)(voronoiEdge.V1[1]+myDir.y)] = 0.5f;
                var scale = ((myDir.magnitude - 0.5f)/myDir.magnitude);
                myDir *= scale < 0 ? 0 : scale;
            }
        }

        private void FillBlack(float[,] cutPattern, int startx, int starty)
        {
            if (startx < 0 || starty < 0 || startx >= cutPattern.GetLength(0) || starty >= cutPattern.GetLength(1))
                return;
            if (cutPattern[startx, starty] < 1)
            {
                cutPattern[startx, starty] = 0f;
                return;
            }
            cutPattern[startx, starty] = 0f;
            FillBlack(cutPattern, startx - 1, starty);
            FillBlack(cutPattern, startx + 1, starty);
            FillBlack(cutPattern, startx, starty - 1);
            FillBlack(cutPattern, startx, starty + 1);
        }
    }
}
