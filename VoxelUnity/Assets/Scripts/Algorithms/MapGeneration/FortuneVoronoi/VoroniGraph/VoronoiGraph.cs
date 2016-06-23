using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Algorithms.MapGeneration.FortuneVoronoi.VoroniGraph
{
    public class VoronoiGraph : Graph.Graph
    {
        private List<VoronoiCell> _cells;
        private Dictionary<Peak, List<VoronoiEdge>> _peaks;
        private VoronoiTriangle[,] _triangles;

        public void GenerateCells(int xSize, int ySize)
        {
            _cells = new List<VoronoiCell>();
            _peaks = new Dictionary<Peak, List<VoronoiEdge>>();

            foreach (var edge in GetEdges().Select(edge => edge as VoronoiEdge))
            {
                if(!_peaks.ContainsKey(new Peak(edge.LeftData))) _peaks.Add(new Peak(edge.LeftData), new List<VoronoiEdge>());
                _peaks[new Peak(edge.LeftData)].Add(edge);
            }

            foreach (var peak in _peaks.Keys)
            {
                _cells.Add(new VoronoiCell(_peaks[peak], peak));
            }

            _triangles = new VoronoiTriangle[xSize, ySize];

            foreach (var voronoiCell in _cells)
            {
                voronoiCell.Draw(_triangles);
            }
        }

        public Peak GetPrimaryPeak(int x, int y)
        {
            return _triangles[x, y].MainPeak;
        }

        public Peak GetSecndaryPeak(int x, int y)
        {
            return _triangles[x, y].SecondaryPeak;
        }
    }
}
