using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.Algorithms.MapGeneration
{
    internal class Voronoi
    {
        public readonly List<Peak> _voronoiSet;
        private readonly float[,] _heightMap;
        private Vector2 _arraySize;
        private readonly float _voronoiCells;
        private readonly float _voronoiScale;
        

        public Voronoi(Vector2 arraySize, float voronoiCells, float voronoiScale, double voronoiFeatures, int seed)
        {
            _heightMap = new float[(int)arraySize.x, (int)arraySize.y];
            _arraySize = arraySize;
            _voronoiCells = voronoiCells;
            _voronoiScale = voronoiScale;
            HeightmapGenerator.Rand = new Random(seed);
            _voronoiSet = GenerateVoronoiSet(arraySize, voronoiCells, voronoiFeatures);
        }
        protected internal  float[,] GenerateVoronoi(bool domainMap)
        {
            var tx = (int)_arraySize.x;
            var ty = (int)_arraySize.y;
            
            //var tree = new PeakSearchTree(_voronoiSet, 100);
            int mx;
            int my;
            SpanningTree spanningTree = null;
            if (domainMap)
            {
                //Bottom Center Peace
                var start = _voronoiSet[((PeakDistance)(PeakDistances(_voronoiSet, tx/2, 0)[0])).Id];
                spanningTree = new SpanningTree(_voronoiSet, start);
                SetDomainHeight(spanningTree);
            }


            var highestScore = 0.0f;
            for (my = 0; my < ty; my++)
            {
                for (mx = 0; mx < tx; mx++)
                {
                    var peakDistances = PeakDistances(_voronoiSet, mx, my);
                    var peakDistOne = (PeakDistance)peakDistances[0];
                    var peakDistTwo = (PeakDistance)peakDistances[1];
                    var hScore = GetVornoiHeight(domainMap, _voronoiScale, peakDistOne, peakDistTwo, tx, ty, _voronoiCells, _voronoiSet, spanningTree);
                    hScore = Math.Max(0.0f, hScore);
                    hScore = Math.Min(1.0f, hScore);
                    _heightMap[mx, my] = hScore;
                    if (hScore > highestScore)
                    {
                        highestScore = hScore;
                    }
                }
            }
            // Normalise...
            for (my = 0; my < ty; my++)
            {
                for (mx = 0; mx < tx; mx++)
                {
                    var normalisedHeight = _heightMap[mx, my] * (1.0f / highestScore);
                    _heightMap[mx, my] = normalisedHeight;
                }
            }
            return _heightMap;
        }

        private static List<Peak> GenerateVoronoiSet(Vector2 arraySize, float voronoiCells, double voronoiFeatures)
        {
            var tx = (int) arraySize.x;
            var ty = (int) arraySize.y;
            // Create Voronoi set...
            var voronoiSet = new List<Peak>();
            int i;
            for (i = 0; i < voronoiCells; i++)
            {

                var xCoord = (int) Math.Floor(HeightmapGenerator.Frand()*tx);
                var yCoord = (int) Math.Floor(HeightmapGenerator.Frand()*ty);
                var pointHeight = HeightmapGenerator.Frand() - 0.1f;
                if (pointHeight > voronoiFeatures)
                {
                    pointHeight = 0.0f;
                }
                var newPeak = new Peak(new Vector2(xCoord, yCoord)) {PeakHeight = pointHeight + 0.01f};
                voronoiSet.Add(newPeak);
            }
            return voronoiSet;
        }

        private void SetDomainHeight(SpanningTree tree)
        {
            foreach (var peak in _voronoiSet)
            {
                peak.PeakHeight = tree.GetHeight(peak.PeakPoint);
            }
             
        }

        /*protected internal float[,] GenerateCutPattern()
        {
            var tx = (int)_arraySize.X;
            var ty = (int)_arraySize.Y;
            var cutPattern = new float[tx,ty];
            var tree = new PeakSearchTree(_voronoiSet, 30);
            Console.WriteLine("NearestPeakAmount:" + tree.GetNearestPeaks(10, 10, 1).Count);
            
            //delete center Pieces
            var centerDistances = PeakDistances(_voronoiSet, (int)_arraySize.X / 2, (int)_arraySize.Y / 2);
            var toDelete = _voronoiSet.ToArray();
            for (var i = 0; i < centerDistances.Count*0.25; i++)
            {
                _voronoiSet.Remove(toDelete[((PeakDistance)centerDistances[i]).Id]);
            }


            for (var x = 0; x < _arraySize.X; x++)
            {
                var peakDistances = PeakDistances(_voronoiSet, x, 0);
                var peak = _voronoiSet[((PeakDistance)peakDistances[0]).Id];
                peak.PeakHeight = 0f;
                peakDistances = PeakDistances(_voronoiSet, x, (int)_arraySize.Y - 1);
                peak = _voronoiSet[((PeakDistance)peakDistances[0]).Id];
                peak.PeakHeight = 0f;
            }

            for (var x = 0; x <=1; x++)
            {
                x *= (int) _arraySize.X-1;
                for (var y = 0; y < _arraySize.Y; y++)
                {
                    var peakDistances = PeakDistances(_voronoiSet, x, y);
                    var peak = _voronoiSet[((PeakDistance)peakDistances[0]).Id];
                    peak.PeakHeight = 0f;
                }
                    
            }

            //Colorize pixels
            //_voronoiSet = voronoiSetCopy;
            for (var my = 0; my < ty; my++)
            {
                for (var mx = 0; mx < tx; mx++)
                {
                    var peakDistances = PeakDistances(_voronoiSet, mx, my);
                    var peak = _voronoiSet[((PeakDistance) peakDistances[0]).Id];
                    var hScore = peak.PeakHeight;
                    hScore = (int)(hScore*1000) == 0 ? 0.0f : 1.0f;
                    cutPattern[mx, my] = hScore;
                }
            }
            return cutPattern;
        }*/

        private static ArrayList PeakDistances(List<Peak> voronoiSet, int mx, int my)
        {
            var peakDistances = new ArrayList();
            int i;
            for (i = 0; i < voronoiSet.Count; i++)
            {
                var peakI = voronoiSet[i];
                var peakPoint = peakI.PeakPoint;
                var distanceToPeak = (peakPoint - new Vector2(mx, my)).magnitude;
                var newPeakDistance = new PeakDistance {Id = i, Dist = distanceToPeak};
                peakDistances.Add(newPeakDistance);
                //if (mx == 0) Debug.Log(mx + "/" + my + ": " + _voronoiSet[i].PeakPoint);
            }
            peakDistances.Sort();
            return peakDistances;
        }

        private static float GetVornoiHeight(bool domainMap, float voronoiScale, PeakDistance d1, PeakDistance d2, int tx, int ty, double voronoiCells, List<Peak> voronoiSet, SpanningTree tree)
        {
            if (domainMap)
            {
                var peakOne = voronoiSet[d1.Id];
                var peakTwo = voronoiSet[d2.Id];
                var hScore = peakOne.PeakHeight;
                if (tree.IsEdgeBetweenVectors(peakOne.PeakPoint, peakTwo.PeakPoint))
                {
                    hScore = GetSlopeHeight(d1, d2, voronoiSet);
                }

                
                return hScore;
            }
            else
            {
                var scale = (float)(Math.Abs(d1.Dist - d2.Dist) / ((tx + ty) / Math.Sqrt(voronoiCells)));
                var peakOne = voronoiSet[d1.Id];
                var h1 = peakOne.PeakHeight;
                var hScore = h1 - Math.Abs(d1.Dist / d2.Dist) * h1;

                hScore = (hScore * scale * voronoiScale) + (hScore * (1.0f - voronoiScale));            
                return hScore;
            }

        }

        private static float GetSlopeHeight(PeakDistance d1, PeakDistance d2, List<Peak> voronoiSet)
        {
            var pos = d1.Dist/(d1.Dist + d2.Dist);
            if (pos < 0.3) return voronoiSet[d1.Id].PeakHeight;
            if (pos > 0.7) return voronoiSet[d2.Id].PeakHeight;
            pos = (pos - 0.3f)*2.5f;
            var dif = voronoiSet[d2.Id].PeakHeight - voronoiSet[d1.Id].PeakHeight;
            return dif * pos + voronoiSet[d1.Id].PeakHeight;
        }

        protected internal static float[,] Scale(float[,] original, int xSize, int ySize)
        {
            var scaled = new float[xSize, ySize];
            var scaleX = (float)xSize / original.GetLength(0);
            var scaleY = (float)ySize / original.GetLength(1);

            for (var x = 0; x < xSize; x++)
            {
                for (var y = 0; y < ySize; y++)
                {
                    scaled[x, y] = original[(int)(x/scaleX), (int)(y/scaleY)];
                }
            }

            return scaled;
        }
    }



    public class PeakDistance : IComparable
    {
        public int Id;
        public float Dist;

        public int CompareTo(object obj)
        {
            var compare = (PeakDistance)obj;
            var result = Dist.CompareTo(compare.Dist);
            if (result == 0)
            {
                result = Dist.CompareTo(compare.Dist);
            }
            return result;
        }
    }

    public class Peak
    {
        public readonly Vector2 PeakPoint;
        public float PeakHeight;

        public Peak(Vector2 v2)
        {
            PeakPoint = v2;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Peak) obj);
        }

        protected bool Equals(Peak other)
        {
            return PeakPoint.Equals(other.PeakPoint) && PeakHeight.Equals(other.PeakHeight);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (PeakPoint.GetHashCode() * 397);
            }
        }

    }
}