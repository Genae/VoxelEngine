using Algorithms.MapGeneration.FortuneVoronoi.VoroniGraph;

namespace Algorithms.MapGeneration.FortuneVoronoi.Nodes
{
    internal class VEdgeNode : VNode
    {
        public VEdgeNode(VoronoiEdge e, bool flipped)
        {
            Edge = e;
            Flipped = flipped;
        }
        public readonly VoronoiEdge Edge;
        public readonly bool Flipped;
        public double Cut(double ys, double x)
        {
            if(!Flipped)
                return (x-Fortune.ParabolicCut(Edge.LeftData[0], Edge.LeftData[1], Edge.RightData[0], Edge.RightData[1], ys));
            return (x - Fortune.ParabolicCut(Edge.RightData[0], Edge.RightData[1], Edge.LeftData[0], Edge.LeftData[1], ys));
        }
    }
}