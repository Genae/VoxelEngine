using VoxelEngine.Base.Data.Map;
using VoxelEngine.Base.Networking;
using VoxelEngine.Server.ClientAdapter;

namespace VoxelEngine.Server
{
    public class EngineServer : IServer
    {
        private static EngineServer _instance;

        public static EngineServer Instance => _instance ?? (_instance = new EngineServer());

        public MapData Map;

        protected EngineServer()
        {}


        public IClientAdapter GetClientAdapter(bool local)
        {
            return local?(IClientAdapter)new LocalClientAdapter():new NetworkClientAdapter();
        }
    }
}
