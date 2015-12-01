using System;
using OpenTK;

namespace VoxelEngine.Shaders.DirectionalDiffuse
{
    public class DirectionalDiffuse : Shader
    {
        private const string VSource= "Shaders/DirectionalDiffuse/shader.vert";
        private const string FSource= "Shaders/DirectionalDiffuse/shader.frag";
        private float _angle;
        private readonly Vector3 _direction = new Vector3(0.7f, -1.0f, 0.3f);

        public DirectionalDiffuse() : base(LoadFile(VSource), LoadFile(FSource))
        {
        }

        public override void OnRenderFrame(FrameEventArgs e)
        {
            //var uniformReference = GL.GetUniformLocation(Program, "viewDirection");
            //GL.Uniform3(uniformReference, Engine.Instance.Cameras[0].Forward);
            _angle = (_angle < 360f) ? (_angle + 1f * (float)e.Time) : 0f;
            SetVariable("direction", new Vector3(1f, (float)Math.Cos(_angle), (float)Math.Sin(_angle)).Normalized());
        }
    }
}
