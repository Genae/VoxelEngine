namespace Assets.Scripts.Base.Data.Map
{
    public class VoxelData
    {
        public bool IsActive { get; set; }
        public int BlockType { get; set; }

        public VoxelData()
        {
            IsActive = false;
            BlockType = 1;
        }
    }
}
