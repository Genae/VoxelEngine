using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.Algorithms.MapGeneration
{
    class DiamondSquare
    {
        private readonly float _diamondSquareDelta;
        private readonly int _width;
        private readonly int _height;
        private Random _rand;

        public DiamondSquare(float diamondSquareDelta, int width, int height)
        {
            _diamondSquareDelta = diamondSquareDelta;
            _width = width;
            _height = height;
        }

        public float[,] Generate(Random rand)
        {
            _rand = rand;
            var heightMap = new float[_width, _height];
            var heightRange = 1.0f;
            var step = _width - 1;
            heightMap[0, 0] = 0.5f;
            heightMap[_width - 1, 0] = 0.5f;
            heightMap[0, _height - 1] = 0.5f;
            heightMap[_width - 1, _height - 1] = 0.5f;
            while (step > 1)
            {
                heightMap = Diamond(heightMap, step, heightRange);
                heightMap = Square(heightMap, step, heightRange);
                heightRange *= _diamondSquareDelta;
                step >>= 1;
            }
            return heightMap;
        }

        private float[,] Diamond(float[,] heightMap, int step, float heightRange)
        {
            for (var tx = 0; tx < _width - 1; tx += step)
            {
                for (var ty = 0; ty < _height - 1; ty += step)
                {
                    var sx = tx + (step >> 1);
                    var sy = ty + (step >> 1);
                    var points = new Vector2[4];
                    points[0] = new Vector2(tx, ty);
                    points[1] = new Vector2(tx + step, ty);
                    points[2] = new Vector2(tx, ty + step);
                    points[3] = new Vector2(tx + step, ty + step);
                    heightMap = CalculateHeight(heightMap, sx, sy, points, heightRange);
                }
            }

            return heightMap;
        }

        private float[,] Square(float[,] heightMap, int step, float heightRange)
        {
            for (var tx = 0; tx < _width - 1; tx += step)
            {
                for (var ty = 0; ty < _height - 1; ty += step)
                {
                    var halfstep = step >> 1;
                    var x1 = tx + halfstep;
                    var y1 = ty;
                    var x2 = tx;
                    var y2 = ty + halfstep;
                    var points1 = _calculateSquarePoints(x1, y1, halfstep);
                    var points2 = _calculateSquarePoints(x2, y2, halfstep);
                    heightMap = CalculateHeight(heightMap, x1, y1, points1, heightRange);
                    heightMap = CalculateHeight(heightMap, x2, y2, points2, heightRange);
                }
            }

            return heightMap;
        }

        private Vector2[] _calculateSquarePoints(int x, int y, int halfstep)
        {
            var points = new Vector2[4];
            points[0] = new Vector2(x - halfstep, y);
            points[1] = new Vector2(x, y - halfstep);
            points[2] = new Vector2(x + halfstep, y);
            points[3] = new Vector2(x, y + halfstep);
            return points;
        }

        private float[,] CalculateHeight(float[,] heightMap, int tx, int ty, IEnumerable<Vector2> points, float heightRange)
        {
            points = points.Select(p => CalculatePoint(p));
            var h = points.Aggregate(0.0f, (acc, point) => acc + heightMap[(int)point.x, (int)point.y] / 4);

            heightMap[tx, ty] = h + ((float)_rand.NextDouble() * heightRange - heightRange / 2);

            if (tx == 0)
            {
                heightMap[_width - 1, ty] = h;
            }
            else if (tx == _width - 1)
            {
                heightMap[0, ty] = h;
            }
            else if (ty == 0)
            {
                heightMap[tx, _height - 1] = h;
            }
            else if (ty == _height - 1)
            {
                heightMap[tx, 0] = h;
            }

            return heightMap;
        }

        private float Clamp(float f, int i, int i1)
        {
            if (f < i)
                return i;
            if (f > i1)
                return i1;
            return f;
        }

        private Vector2 CalculatePoint(Vector2 point)
        {
            if (point.x < 0)
            {
                point.x += (_width - 1);
            }
            else if (point.x > _width)
            {
                point.x -= (_width - 1);
            }
            else if (point.y < 0)
            {
                point.y += _height - 1;
            }
            else if (point.y > _height)
            {
                point.y -= _height - 1;
            }

            return point;
        }
    }
}