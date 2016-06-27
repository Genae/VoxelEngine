using System;

namespace Assets.Scripts.Data.Map
{
    public class VoxelData
    {
        private int _blockType;
        internal bool IsActive { get; set; }
        
        internal int BlockType
        {
            get { return _blockType; }
            set
            {
                _blockType = value;
                IsActive = value != 0;
            }
        }

        public VoxelData(bool active, int blockType)
        {
            IsActive = active;
            _blockType = blockType;
        }
    }
}
