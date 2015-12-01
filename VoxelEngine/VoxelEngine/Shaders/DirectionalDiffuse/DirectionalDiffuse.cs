using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace VoxelEngine.Shaders.DirectionalDiffuse
{
    public class DirectionalDiffuse : Shader
    {
        private const string VSource= "Shaders/DirectionalDiffuse/shader.vert";
        private const string FSource= "Shaders/DirectionalDiffuse/shader.frag";

        public DirectionalDiffuse() : base(LoadFile(VSource), LoadFile(FSource))
        {
        }

        public override void OnRenderFrame(FrameEventArgs e)
        {
            var uniformReference = GL.GetUniformLocation(Program, "viewDirection");
            GL.Uniform3(uniformReference, Engine.Instance.Cameras[0].Forward);
        }
    }
}
