using OpenTK;

namespace VoxelEngine.Client.Shaders.ShadowMap
{
    public class ShadowMap : Shader
    {
        private const string VSource= "Shaders/ShadowMap/shader.vert";
        private const string FSource= "Shaders/ShadowMap/shader.frag";

        public ShadowMap() : base(LoadFile(VSource), LoadFile(FSource))
        {
        }

        public override void OnRenderFrame(FrameEventArgs e)
        {
        }
    }
}
