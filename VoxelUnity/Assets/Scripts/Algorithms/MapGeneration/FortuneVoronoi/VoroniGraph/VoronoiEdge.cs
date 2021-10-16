using System;
using Algorithms.MapGeneration.Graph;
using UnityEngine;

namespace Algorithms.MapGeneration.FortuneVoronoi.VoroniGraph
{
    public class VoronoiEdge : Edge
    {
        public Vector2 RightData, LeftData;

        public VoronoiEdge()
        {
            V1 = GetUnknownVertex();
            V2 = GetUnknownVertex();
        }
        public override void AddVertex(Vector2 v)
        {
            if (IsUnknownVertex(V1))
                V1 = v;
            else if (IsUnknownVertex(V2))
            {
                V2 = v;
                if (!IsPartlyInfinite)
                    CalculateWeight();
                else
                    Weight = float.PositiveInfinity;
            }
            else throw new Exception("Tried to add third vertex!");
        }

        public Vector2 FixedPoint
        {
            get
            {
                if (IsInfinite)
                    return 0.5f * (LeftData + RightData);
                return !IsInfiniteVertex(V1) ? V1 : V2;
            }
        }
        public Vector2 DirectionVector
        {
            get
            {
                if (!IsPartlyInfinite)
                    return (V2 - V1) * (1.0f / (V1 - V2).magnitude);
                if (LeftData[0].Equals(RightData[0]))
                {
                    if (LeftData[1] < RightData[1])
                        return new Vector2(-1, 0);
                    return new Vector2(1, 0);
                }
                var erg = new Vector2(-(RightData[1] - LeftData[1]) / (RightData[0] - LeftData[0]), 1f);
                if (RightData[0] < LeftData[0])
                    erg *= (-1);
                erg *= (1.0f / (float)Math.Sqrt(erg.sqrMagnitude));
                return erg;
            }
        }
    }
}