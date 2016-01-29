using VoxelEngine.Base.Data.Map;
using VoxelEngine.Base.Networking;

namespace VoxelEngine.Client.ServerAdapter
{
    public abstract class LocalServerAdapter : IServerAdapter
    {
        private IClientAdapter _adapter;
        protected abstract IServer StartLocalServer(IClient client);

        public bool JoinSession(IClient client)
        {
            var server = StartLocalServer(client);
            _adapter = server?.GetClientAdapter(true);
            return _adapter != null;
        }
        
        public MapData LoadMapData()
        {
            return _adapter.GetMap();
        }
    }
}
