using System;

namespace TestGame.MapGeneration
{
    class DiamondCutPattern
    {
        private readonly float[,] _ds;

        public DiamondCutPattern(float[,] ds)
        {
            _ds = ds;
        }

        public float[,] GenerateCutPattern()
        {
            var _width = _ds.GetLength(0);
            var _height = _ds.GetLength(1);
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    var border = Math.Sqrt((_width/2 - x)*(_width/2 - x) + (_height/2 - y)*(_height/2 - y)) / (0.7f*_width);
                    if (_ds[x, y]*1.3 > border)
                        _ds[x, y] = 1;
                    else
                        _ds[x, y] = 0;
                }
            }
            return _ds;
        }
    }
}
