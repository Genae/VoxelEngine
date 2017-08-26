using Assets.Scripts.AccessLayer.Jobs;
using Assets.Scripts.AccessLayer.Tools;
using Assets.Scripts.EngineLayer.Voxels.Containers;
using Assets.Scripts.Logic;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayer.Actions
{
    public class MiningAction : SolveJobAction
    {
        public MiningAction() : base("hasMined", "Mining", 1.5f)
        { }
    }
    public class MiningJob : PositionedJob
    {
        public MiningJob(Vector3 position) : base(position, 1.1f, new Color(1f, 0f, 0f, 0.5f), Overlay.Mining, "Mining")
        {
            RemainingTime = 1f;
        }

        protected override void SolveInternal(GameObject actor)
        {
            Map.Instance.MapData.Chunks[(int)Position.x / 16, (int)Position.y / 16, (int)Position.z / 16]
                .MineVoxel((int)Position.x % 16, (int)Position.y % 16, (int)Position.z % 16, actor.GetComponent<Inventory>());
        }
    }
}
