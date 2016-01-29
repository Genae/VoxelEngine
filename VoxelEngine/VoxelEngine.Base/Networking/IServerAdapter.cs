using VoxelEngine.Base.Data.Map;

namespace VoxelEngine.Base.Networking
{
    public interface IServerAdapter
    {
        bool JoinSession(IClient client);
        MapData LoadMapData();
    }
}
