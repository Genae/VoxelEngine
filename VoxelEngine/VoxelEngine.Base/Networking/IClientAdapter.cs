using VoxelEngine.Base.Data.Map;

namespace VoxelEngine.Base.Networking
{
    public interface IClientAdapter
    {
        MapData GetMap();
    }
}
