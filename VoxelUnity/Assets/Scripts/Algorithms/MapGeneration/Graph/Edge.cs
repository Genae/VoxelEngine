using System;
using UnityEngine;

namespace Algorithms.MapGeneration.Graph
{
    public class Edge
    {
        internal Vector2 V1;
        internal Vector2 V2;
        internal float Weight;

        private static readonly Vector2 VvInfinite = new Vector2(Single.PositiveInfinity, Single.PositiveInfinity);
        private static readonly Vector2 VvUnkown = new Vector2(Single.NaN, Single.NaN);

        public bool IsInfinite
        {
            get { return IsInfiniteVertex(V1) && IsInfiniteVertex(V2); }
        }
        public bool IsPartlyInfinite
        {
            get { return IsInfiniteVertex(V1) || IsInfiniteVertex(V2); }
        }

        public Edge(Vector2 v1, Vector2 v2)
        {
            V1 = v1;
            V2 = v2;
            CalculateWeight();
        }

        protected Edge()
        {}

        public virtual void AddVertex(Vector2 v)
        {
            if (IsUnknownVertex(V1))
                V1 = v;
            else if (IsUnknownVertex(V2))
                V2 = v;
            else throw new Exception("Tried to add third vertex!");
        }

        protected void CalculateWeight()
        {
            Weight = (V1 - V2).magnitude;
        }

        public static bool IsUnknownVertex(Vector2 v)
        {
            return (Single.IsNaN(v[0]) && Single.IsNaN(v[1]));
        }

        public static Vector2 GetUnknownVertex()
        {
            return VvUnkown;
        }

        public static bool IsInfiniteVertex(Vector2 v)
        {
            return (Single.IsPositiveInfinity(v[0]) && Single.IsPositiveInfinity(v[1]));
        }

        public static Vector2 GetInfiniteVertex()
        {
            return VvInfinite;
        }
    }
}