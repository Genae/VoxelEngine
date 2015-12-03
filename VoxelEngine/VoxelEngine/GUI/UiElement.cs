using System.Drawing;
using OpenTK;
using VoxelEngine.Camera;
using VoxelEngine.GameData;

namespace VoxelEngine.GUI
{
    public class UiElement
    {
        protected Rectangle Position;
        public UiElement(Rectangle position)
        {
            Position = position;
        }
    }
}
