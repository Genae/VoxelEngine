namespace VoxelEngine.Base.Networking
{
    public interface IServer
    {
        IClientAdapter GetClientAdapter(bool local);
    }
}
