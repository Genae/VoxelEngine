namespace EngineLayer.Voxels.Data
{
    public class VoxelData
    {
        private int _blockType;

        internal bool IsActive
        {
            get { return _blockType != 0; }
        }

        internal int BlockType
        {
            get { return _blockType; }
            set
            {
                _blockType = value;
            }
        }

        internal VoxelData(int blockType)
        {
            BlockType = blockType;
        }
    }
}
