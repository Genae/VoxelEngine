using UnityEngine;
using Mesh = Assets.Scripts.Client.GameData.Mesh;

namespace Assets.Scripts.Client.GameData
{
    class ChunkBorder : Mesh
    {
        public ChunkBorder(float size, Vector3 pos) : base(size, pos, false)
        {
        }
        
        protected override void Load()
        {
            CreateCube();
        }

        public override void OnRenderFrame(FrameEventArgs e)
        {
            RenderLines(true);
        }
    }
}
