using UnityEngine;

namespace Assets.Scripts.Algorithms.MapGeneration.FortuneVoronoi.Nodes
{
    internal class VDataNode : VNode
    {
        public VDataNode(Vector2 dp)
        {
            DataPoint = dp;
        }
        public readonly Vector2 DataPoint;
    }
}