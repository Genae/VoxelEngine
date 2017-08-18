using System;
using UnityEngine;

namespace Assets.Scripts.Algorithms.Pathfinding.Utils
{
    public class Vector3I : IComparable<Vector3I>
    {
        public int x;
        public int y;
        public int z;

        public static Vector3I zero
        {
            get { return new Vector3I(0, 0, 0); }
        }

        public Vector3I(int xPos, int yPos, int zPos)
        {
            x = xPos;
            y = yPos;
            z = zPos;
        }
        public Vector3I(Vector3 v)
        {
            x = (int)v.x;
            y = (int)v.y;
            z = (int)v.z;
        }

        public int this[int index]
        {
            get
            {
                int result;
                switch (index)
                {
                    case 0:
                        result = x;
                        break;
                    case 1:
                        result = y;
                        break;
                    case 2:
                        result = z;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index!");
                }
                return result;
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index!");
                }
            }
        }

        public float magnitude
        {
            get
            {
                return (float)Math.Sqrt(sqrMagnitude);
            }
        }

        public float sqrMagnitude
        {
            get
            {
                return x * x + y * y + z * z;
            }
        }
        
        public static implicit operator Vector3(Vector3I v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static implicit operator Vector3I(Vector3 v)
        {
            return new Vector3I((int)v.x, (int)v.y, (int)v.z);
        }

        public int CompareTo(Vector3I other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var xComparison = x.CompareTo(other.x);
            if (xComparison != 0) return xComparison;
            var yComparison = y.CompareTo(other.y);
            if (yComparison != 0) return yComparison;
            return z.CompareTo(other.z);
        }
        
        public override bool Equals(object obj)
        {
            return CompareTo(obj as Vector3I) == 0;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + y.GetHashCode();
            hash = hash * 23 + z.GetHashCode();
            return hash;
        }
    }
}