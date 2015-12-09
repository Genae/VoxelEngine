using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using OpenTK;
using TestGame.MapGeneration.FortuneVoronoi;
using Random = System.Random;

namespace TestGame.MapGeneration
{
    class HeightmapGenerator
    {
        public readonly float[,] Values;
        public readonly float[,] BottomValues;
        public readonly float[,] CutPattern;
        public readonly List<Vector2> Border;
        public const float DiamondSquareDelta = 0.5f;
        protected internal static Random Rand;

        public HeightmapGenerator(int width, int height, int seed)
        {
            Rand = new Random(seed);
            var watch = new Stopwatch();
            //Diamond Square
            watch.Start();
            var valuesDs = (new DiamondSquare(DiamondSquareDelta, width, height)).Generate(Rand);
            watch.Stop();
            Console.WriteLine("" + watch.Elapsed);

            //Voronoi
            watch.Start();
            var voronoi = new Voronoi(new Vector2(width, height), 20, -1f, 1.0f, seed);
            var valuesV = voronoi.GenerateVoronoi(false);
            watch.Stop();
            Console.WriteLine("" + watch.Elapsed);

            //Domain Map
            watch.Start();
            var valuesDm = voronoi.GenerateVoronoi(true);
            watch.Stop();
            Console.WriteLine("" + watch.Elapsed);

            //Combining Them
            watch.Start();
            Values = valuesV;
            valuesV = MulArrays(valuesDm, valuesV);
            Values = MulArrays(valuesV, valuesDs);
            watch.Stop();
            Console.WriteLine("" + watch.Elapsed);

            //CutPattern
            watch.Start();
            //voronoi = new Voronoi(new Vector2(width, height), 100, -1f, 1.0f, seed);
            //var peaks = voronoi._voronoiSet.Select(peak => peak.PeakPoint).ToList();
            //var vGraph = Fortune.ComputeVoronoiGraph(peaks);
            var dcp = new DiamondCutPattern(valuesDs);
            //vGraph.GenerateCells(width, height);
            CutPattern = dcp.GenerateCutPattern();
            watch.Stop();
            Console.WriteLine("" + watch.Elapsed);


            //Borders 
            watch.Start();
            Border = GetBorders(CutPattern);
            watch.Stop();
            Console.WriteLine("" + watch.Elapsed);


            //Bottom
            watch.Start();
            voronoi = new Voronoi(new Vector2(width, height), 5, -1f, 1.0f, seed);
            BottomValues = GenerateBottomValues(Values, voronoi.GenerateVoronoi(false));
            try
            {
               // SmoothEdges(BottomValues);
            }
            catch (Exception x)
            {
                Console.WriteLine(x);
            }
            watch.Stop();
            Console.WriteLine("" + watch.Elapsed);

        }

        private float[,] GenerateBottomValues(float[,] values, float[,] voronoi)
        {
            var bottom = new float[values.GetLength(0), values.GetLength(1)];
            for (var x = 0; x < values.GetLength(0); x++)
            {
                for (var y = 0; y < values.GetLength(1); y++)
                {
                    var down = (Math.Min(x, values.GetLength(0) - x) * Math.Min(y, values.GetLength(1) - y)) / (float)(values.GetLength(0)*values.GetLength(1));
                    if (!Border.Contains(new Vector2(x, y)))
                    {
                        bottom[x, y] = values[x, y]-down*6-voronoi[x, y];
                    }
                    else
                    {
                        bottom[x, y] = values[x, y];
                    }
                }
            }
            return bottom;
        }

        /*private void SmoothEdges(float[,] values)
        {
            for (var x = 1; x < values.GetLength(0)-2; x++)
            {
                for (var y = 1; y < values.GetLength(1)-2; y++)
                {
                    SmoothVector(values, x, y);
                }
            }
        }

        private void SmoothVector(float[,] values, int x, int y)
        {
            if (x < 1 || x >= values.GetLength(0)-2 || y < 1 || y >= values.GetLength(1)-2) return;
            var highest = Mathf.Max(values[x + 1, y], values[x - 1, y], values[x, y + 1], values[x, y - 1]);
            //if (x == 50) Debug.Log(highest + "/" + values[x, y]);
            if (highest - values[x, y] > 0.09f)
            {
                values[x, y] = highest - 0.08f;// - Frand()*0.01f;
                SmoothVector(values, x + 1, y);
                SmoothVector(values, x - 1, y); 
                SmoothVector(values, x, y - 1);
                SmoothVector(values, x, y + 1);
            }
        }*/

        private static float[,] MulArrays(float[,] valuesD, float[,] valuesV, float ratio = 0.5f)
        {
            var mulArray = new float[valuesD.GetLength(0), valuesD.GetLength(1)];
            for (var y = 0; y < valuesD.GetLength(1); y++)
            {
                for (var x = 0; x < valuesD.GetLength(0); x++)
                {
                    mulArray[x, y] = (valuesD[x, y]+valuesV[x, y])*ratio;
                }
            }
            return mulArray;
        }

        protected internal static float Frand()
        {
            return (float) Rand.NextDouble();
        }


        /*public static void GenerateImage(string path, float[,] values)
        {
            var tex = new Texture2D(values.GetLength(0), values.GetLength(1));
            for (var x = 0; x < values.GetLength(0); x++)
            {
                for (var y = 0; y < values.GetLength(1); y++)
                {
                    tex.SetPixel(x, y, new Color(values[x, y], values[x, y], values[x, y]));
                }
            }
            var img = tex.EncodeToPNG();
            File.WriteAllBytes(path, img);
        }*/




        private List<Vector2> GetBorders(float[,] cutPattern)
        {
            var border = new List<Vector2>();
            var ysize = cutPattern.GetLength(1);
            var xsize = cutPattern.GetLength(0);


            for (var x = 0; x < cutPattern.GetLength(0); x++)
            {
                for (var y = 0; y < cutPattern.GetLength(0); y++)
                {
                    //Right Border
                    if ((x - 1) > 0 && cutPattern[(x - 1),y] <= 0)
                    {
                        if (CanGenerateTriangle(x, y, cutPattern))
                        {
                            border.Add(new Vector2(x, y));
                            continue;
                        }

                    }
                    //Left Border
                    if ((x + 1) < xsize && cutPattern[(x + 1), y] <= 0)
                    {
                        if (CanGenerateTriangle(x, y, cutPattern))
                        {
                            border.Add(new Vector2(x, y));
                            continue;
                        }
                    }
                    //Top Border
                    if ((y - 1) > 0 && cutPattern[x, (y - 1)] <= 0)
                    {
                        if (CanGenerateTriangle(x, y, cutPattern))
                        {
                            border.Add(new Vector2(x, y));
                            continue;
                        }
                    }
                    //Bottom Border
                    if ((y + 1) < ysize && cutPattern[x, (y + 1)] <= 0)
                    {
                        if (CanGenerateTriangle(x, y, cutPattern))
                        {
                            border.Add(new Vector2(x, y));
                            continue;
                        }
                    }
                    //Right Top Border
                    if ((x - 1) > 0 && (y - 1) > 0 && cutPattern[(x - 1), (y - 1)] <= 0)
                    {
                        if (CanGenerateTriangle(x, y, cutPattern))
                        {
                            border.Add(new Vector2(x, y));
                            continue;
                        }

                    }
                    //Left Top Border
                    if ((x + 1) < xsize && (y - 1) > 0 && cutPattern[(x + 1), (y - 1)] <= 0)
                    {
                        if (CanGenerateTriangle(x, y, cutPattern))
                        {
                            border.Add(new Vector2(x, y));
                            continue;
                        }
                    }
                    //Right Bottom Border
                    if ((x - 1) > 0 && (y + 1) < ysize && cutPattern[(x - 1), (y + 1)] <= 0)
                    {
                        if (CanGenerateTriangle(x, y, cutPattern))
                        {
                            border.Add(new Vector2(x, y));
                            continue;
                        }

                    }
                    //Left Bottom Border
                    if ((x + 1) < xsize && (y + 1) < ysize && cutPattern[(x + 1), (y + 1)] <= 0)
                    {
                        if (CanGenerateTriangle(x, y, cutPattern))
                        {
                            border.Add(new Vector2(x, y));
                            //continue;
                        }
                    }
                }
            }
            return border;
        }

        private static bool CanGenerateTriangle(int x, int y, float[,] cutPattern)
        {
            return TriangleExists(new Vector2(x - 1, y), new Vector2(x, y - 1), new Vector2(x, y), cutPattern) ||
                   TriangleExists(new Vector2(x, y - 1), new Vector2(x + 1, y - 1), new Vector2(x, y), cutPattern) ||
                   TriangleExists(new Vector2(x + 1, y), new Vector2(x + 1, y - 1), new Vector2(x, y), cutPattern) ||
                   TriangleExists(new Vector2(x - 1, y), new Vector2(x - 1, y + 1), new Vector2(x, y), cutPattern) ||
                   TriangleExists(new Vector2(x, y + 1), new Vector2(x - 1, y + 1), new Vector2(x, y), cutPattern) ||
                   TriangleExists(new Vector2(x + 1, y), new Vector2(x, y + 1), new Vector2(x, y), cutPattern);
        }

        private static bool TriangleExists(Vector2 v1, Vector2 v2, Vector2 v3, float[,] cutPattern)
        {
            var xSize = cutPattern.GetLength(0);
            var ySize = cutPattern.GetLength(0);

            if (v1.X < 0 || v1.X >= xSize || v1.Y < 0 || v1.Y >= ySize) return false;
            if (v2.X < 0 || v2.X >= xSize || v2.Y < 0 || v2.Y >= ySize) return false;
            if (v3.X < 0 || v3.X >= xSize || v3.Y < 0 || v3.Y >= ySize) return false;

            return cutPattern[(int)v1.X, (int)v1.Y] > 0 && cutPattern[(int)v2.X, (int)v2.Y] > 0 && cutPattern[(int)v3.X, (int)v3.Y] > 0;
        }
    }


}