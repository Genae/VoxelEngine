using OpenTK;
using VoxelEngine.Client.GameData;

namespace VoxelEngine.Client.Light
{
    public class LightSource : GameObject
    {
        public Vector4 Position;

        public LightSource(Vector4 position)
        {
            Position = position;
            EngineClient.Instance.Lights.Add(this);
        }

        public override void Destroy()
        {
            base.Destroy();
            EngineClient.Instance.Lights.Remove(this);
        }

        public void OnRenderFrame(FrameEventArgs e)
        {
            GenerateShadowMap();
        }

        protected virtual void GenerateShadowMap()
        {
        }
    }
}
