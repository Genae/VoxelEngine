using VoxelEngine.Base.Data.Map;
using VoxelEngine.Base.Networking;

namespace VoxelEngine.Server.ClientAdapter
{
    public class LocalClientAdapter : IClientAdapter
    {
        public MapData GetMap()
        {
            return EngineServer.Instance.Map;
        }
    }
}
