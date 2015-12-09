using OpenTK;

namespace TestGame.MapGeneration.FortuneVoronoi.Events
{
    internal class VDataEvent : VEvent
    {
        public readonly Vector2 DataPoint;
        public VDataEvent(Vector2 dp)
        {
            DataPoint = dp;
        }
        public override double Y
        {
            get
            {
                return DataPoint[1];
            }
        }

        protected override double X
        {
            get
            {
                return DataPoint[0];
            }
        }

    }
}