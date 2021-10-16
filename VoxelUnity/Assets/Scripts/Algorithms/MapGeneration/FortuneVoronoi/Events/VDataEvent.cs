using UnityEngine;

namespace Algorithms.MapGeneration.FortuneVoronoi.Events
{
    internal class VDataEvent : VEvent
    {
        public readonly Vector2 DataPoint;
        public VDataEvent(Vector2 dp)
        {
            DataPoint = dp;
        }
        public override double Y => DataPoint[1];

        protected override double X => DataPoint[0];
    }
}