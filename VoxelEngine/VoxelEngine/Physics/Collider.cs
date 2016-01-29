using OpenTK;
using VoxelEngine.Client.GameData;

namespace VoxelEngine.Client.Physics
{
    //TODO make this abstract and add at least box colliders + sphere colliders
    public class Collider : GameObject
    {

        public float Range;
        public Vector3 Position;

        public Collider(float range, Vector3 position)
        {
            Range = range;
            Position = position;
            EngineClient.Instance.Collider.Add(this);
        }

        public override void Destroy()
        {
            base.Destroy();
            EngineClient.Instance.Collider.Remove(this);
        }


    }
}
