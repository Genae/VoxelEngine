using TestGame.MapGeneration.FortuneVoronoi.Nodes;

namespace TestGame.MapGeneration.FortuneVoronoi.Events
{
    internal class VCircleEvent : VEvent
    {
        public VDataNode NodeN, NodeL, NodeR;
        public double CenterX, CenterY;
        public override double Y
        {
            get
            {
                return (CenterY + Fortune.Dist(NodeN.DataPoint[0], NodeN.DataPoint[1], CenterX, CenterY));
            }
        }

        protected override double X
        {
            get
            {
                return CenterX;
            }
        }

        public bool Valid = true;
    }
}