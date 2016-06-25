using System;

namespace Assets.Scripts.Data.Map
{
    public class VoxelData
    {
        private readonly Action _voxelUpdated;
        private int _blockType;
        public bool IsActive { get; set; }


        public int BlockType
        {
            get { return _blockType; }
            set
            {
                _blockType = value;
                IsActive = value != 0;
                _voxelUpdated();
            }
        }

        public VoxelData(bool active, int blockType, Action voxelUpdated)
        {
            IsActive = active;
            _blockType = blockType;
            _voxelUpdated = voxelUpdated;
        }
    }
}
