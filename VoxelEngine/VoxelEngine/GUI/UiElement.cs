using System.Drawing;

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
