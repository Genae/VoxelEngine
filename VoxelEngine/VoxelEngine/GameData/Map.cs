using VoxelEngine.Base.Data.Map;
using VoxelEngine.Client.Shaders;

namespace VoxelEngine.Client.GameData
{
    public class Map
    {
        public Shader Shader;
        public MapData MapData;

        protected Map(MapData data)
        {
            MapData = data;
        }
    }
}
