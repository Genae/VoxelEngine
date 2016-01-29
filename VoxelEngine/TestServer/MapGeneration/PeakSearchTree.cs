/*using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenTK;

namespace TestGame.MapGeneration
{
    class PeakSearchTree
    {
        private readonly Dictionary<Vector2, Peak> _peaks;
        private readonly RangeSearchTree<Vector2> _tree;
        private readonly float _step;
        public PeakSearchTree(IEnumerable<Peak> peaks, float step = 10f)
        {
            _step = step;
            _peaks = new Dictionary<Vector2, Peak>();
            foreach (var peak in peaks.Where(peak => !_peaks.ContainsKey(peak.PeakPoint)))
            {
                _peaks.Add(peak.PeakPoint, peak);
            }
            _tree = new RangeSearchTree<Vector2>(2, _peaks.Keys.ToList(), new Vector2Comparer());
        }

        public List<Peak> GetNearestPeaks(int x, int y, int amount)
        {
            var nearPeaks = new List<Peak>();
            var i = 1;
            while (nearPeaks.Count < amount)
            {
                nearPeaks = GetPeaksInSquare(new Vector2(x, y), _step*i++);
                //if (i > 2) Debug.Log("i: " + i + ": " + nearPeaks.Count);
                //if (i > 10) Debug.Log(new Vector2(Mathf.Max(v1 - _step*(i-1), 0), (Mathf.Max(v2 - _step*(i-1), 0))));
                if (i > 10)
                {
                    Console.WriteLine("Nothing found");
                    return null;
                }
            }
            //if(i>2) Debug.Log("i: " + i + ": " + nearPeaks.Count);
            return nearPeaks;
        }

        private List<Peak> GetPeaksInSquare(Vector2 center, float radius)
        {
            var topLeft = new Vector2(Math.Max(center.X - radius, 0), Math.Max(center.Y - radius, 0));
            var bottomRight = new Vector2(Math.Min(center.X + radius, 129), Math.Min(center.Y + radius, 129));
            var list = GetPeaks(topLeft, bottomRight);
            foreach (var peak in list.ToArray().Where(peak => (peak.PeakPoint - center).Length > radius))
            {
                list.Remove(peak);
            }
            return list;
        }

        private List<Peak> GetPeaks(Vector2 topLeft, Vector2 bottom)
        {
            var vectors = _tree.GetAllInRange(topLeft, bottom);
            return GetPeaksFromVectors(vectors);
        }

        private List<Peak> GetPeaksFromVectors(IEnumerable<Vector2> vectors)
        {
            return vectors.Select(vector2 => _peaks[vector2]).ToList();
        }
    }
    class Vector2Comparer : MultiDimensionalComparer<Vector2>
    {
        public override int Compare(Vector2 v1, Vector2 v2, int dimension)
        {
            //Debug.Log(dimension);
            if (dimension == 0)
            {
                if ((int) v1.X == (int) v2.X) return 0;
                return ((int) v1.X - (int) v2.X > 0) ? 1 : -1;
            }
            if (dimension == 1)
            {
                if ((int)v1.Y== (int)v2.Y) return 0;
                return ((int)v1.Y - (int)v2.Y > 0) ? 1 : -1;
            }
            Console.WriteLine(dimension);
            return 0;
        }
    }
}
*/