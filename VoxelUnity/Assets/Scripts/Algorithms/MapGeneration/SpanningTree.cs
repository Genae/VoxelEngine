using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Algorithms.MapGeneration.Graph;
using UnityEngine;

namespace Assets.Scripts.Algorithms.MapGeneration
{
    class SpanningTree : Graph.Graph 
    {
        private readonly Dictionary<Vector2, float> _heights; 
 
        public SpanningTree(IEnumerable<Peak> peaks, Peak start)
        {
            _heights = new Dictionary<Vector2, float>();
            var vertices = peaks.Select(peak => peak.PeakPoint).ToList();
            FillKruskal(vertices);
            CalculateHeights(start.PeakPoint);
        }

        private void CalculateHeights(Vector2 start)
        {
            var queue = new Queue<KeyValuePair<Vector2, Vector2>>();
            queue.Enqueue(new KeyValuePair<Vector2, Vector2>(start, start));
            _heights[start] = 0;
            while (queue.Count > 0)
            {
                var curr = queue.Peek().Value;
                var last = queue.Dequeue().Key;

                foreach (var v in GetOutVertices(curr).Where(v => v != last))
                {
                    queue.Enqueue(new KeyValuePair<Vector2, Vector2>(curr, v));
                }
                if (last != curr)
                {
                    _heights.Add(curr, _heights[last] + AdjMap[last][curr].Weight);
                }
                
            }
            var max = _heights.Values.Max();
            foreach (var height in _heights.Keys.ToArray())
            {
                _heights[height] = _heights[height] / max;
            }

        }

        private static IEnumerable<Edge> GetPossibleEdges(List<Vector2> vertices)
        {
            var possibleEdges = new List<Edge>();
            foreach (var vector in vertices)
            {
                possibleEdges.AddRange(vertices.Where(vector2 => vector != vector2).Select(vector2 => new Edge(vector, vector2)));
            }
            return possibleEdges;
        }

        private void FillKruskal(List<Vector2> vertices)
        {
            var sorted = new SortedDictionary<float, Edge>();
            foreach (var possibleEdge in GetPossibleEdges(vertices))
            {
                while (sorted.ContainsKey(possibleEdge.Weight)) 
                    possibleEdge.Weight += 0.0001f;
                sorted.Add(possibleEdge.Weight, possibleEdge);
            }
            foreach (var edge in sorted.Values)
            {
                AddEdge(edge);
                if(HasCycle(edge.V1))
                    RemoveEdge(edge, true);
            }
        }

        public float GetHeight(Vector2 peakPoint)
        {
            return _heights[peakPoint];
        }
    }
}
