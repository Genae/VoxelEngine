using OpenTK;

namespace VoxelEngine.Client.Shaders.DirectionalShadow
{
    public class DirectionalShadow : Shader
    {
        private const string VSource= "Shaders/DirectionalShadow/shader.vert";
        private const string FSource= "Shaders/DirectionalShadow/shader.frag";

        private static DirectionalShadow _instance;
        public static DirectionalShadow Instance
        {
            get
            {
                if (_instance == null)
                    return (_instance = new DirectionalShadow());
                return _instance;
            }
        }

        protected DirectionalShadow() : base(LoadFile(VSource), LoadFile(FSource))
        {}

        public override void OnRenderFrame(FrameEventArgs e)
        {}
    }
}
