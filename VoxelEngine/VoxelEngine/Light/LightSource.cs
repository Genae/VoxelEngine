using OpenTK;
using VoxelEngine.GameData;

namespace VoxelEngine.Light
{
    public class LightSource : GameObject
    {
        public Vector4 Position;

        public LightSource(Vector4 position)
        {
            Position = position;
            Engine.Instance.Lights.Add(this);
        }

        public override void Destroy()
        {
            base.Destroy();
            Engine.Instance.Lights.Remove(this);
        }

        public void OnRenderFrame(FrameEventArgs e)
        {
            
        }
    }
}
