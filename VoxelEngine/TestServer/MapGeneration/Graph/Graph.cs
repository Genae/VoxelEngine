using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace TestGame.MapGeneration.Graph
{
    public class Graph
    {
        protected readonly Dictionary<Vector2, Dictionary<Vector2, Edge>> AdjMap;
        protected readonly List<Vector2> Vertices;
        protected readonly List<Edge> Edges;

        protected Graph()
        {
            AdjMap = new Dictionary<Vector2, Dictionary<Vector2, Edge>>();
            Vertices = new List<Vector2>();
            Edges = new List<Edge>();
        }

        public void AddEdge(Edge edge)
        {
            if (!AdjMap.ContainsKey(edge.V1))
                AdjMap.Add(edge.V1, new Dictionary<Vector2, Edge>());
            AdjMap[edge.V1][edge.V2] =  edge;

            if (!AdjMap.ContainsKey(edge.V2))
                AdjMap.Add(edge.V2, new Dictionary<Vector2, Edge>());
            AdjMap[edge.V2][edge.V1] =  edge;

            Edges.Add(edge);

            if (!Vertices.Contains(edge.V1)) Vertices.Add(edge.V1);
            if (!Vertices.Contains(edge.V2)) Vertices.Add(edge.V2);
        }

        public void RemoveEdge(Edge edge, bool removeVertex)
        {
            if(AdjMap.ContainsKey(edge.V1)) AdjMap[edge.V1].Remove(edge.V2);
            if(AdjMap.ContainsKey(edge.V2)) AdjMap[edge.V2].Remove(edge.V1);

            Edges.Remove(edge);

            if (!removeVertex) return;
            if (!AdjMap.ContainsKey(edge.V1)) Vertices.Remove(edge.V1);
            if (!AdjMap.ContainsKey(edge.V2)) Vertices.Remove(edge.V2);
        }

        public void AddVertex(Vector2 vertex)
        {
            if (!Vertices.Contains(vertex)) Vertices.Add(vertex);
        }

        protected IEnumerable<Vector2> GetOutVertices(Vector2 v)
        {
            return AdjMap[v].Keys.ToList();
        }

        public IEnumerable<Edge> GetEdges()
        {
            return Edges;
        }

        public IEnumerable<Vector2> GetVertices()
        {
            return Vertices;
        }

        protected bool HasCycle(Vector2 start)
        {
            var visited = new HashSet<Vector2>();
            var queue = new Queue<KeyValuePair<Vector2, Vector2>>();
            queue.Enqueue(new KeyValuePair<Vector2, Vector2>(start, start));
            while (queue.Count > 0)
            {
                var curr = queue.Peek().Value;
                var last = queue.Dequeue().Key;

                foreach (var v in GetOutVertices(curr).Where(v => v != last))
                {
                    queue.Enqueue(new KeyValuePair<Vector2, Vector2>(curr, v));
                    if (visited.Contains(v))
                        return true;
                }
                visited.Add(curr);
            }
            return false;
        }

        public bool IsEdgeBetweenVectors(Vector2 v1, Vector2 v2)
        {
            return AdjMap.ContainsKey(v1) && AdjMap[v1].ContainsKey(v2);
        }
    }
}