using Algorithms.MapGeneration.FortuneVoronoi.Nodes;

namespace Algorithms.MapGeneration.FortuneVoronoi.Events
{
    internal class VCircleEvent : VEvent
    {
        public VDataNode NodeN, NodeL, NodeR;
        public double CenterX, CenterY;
        public override double Y => (CenterY + Fortune.Dist(NodeN.DataPoint[0], NodeN.DataPoint[1], CenterX, CenterY));

        protected override double X => CenterX;

        public bool Valid = true;
    }
}