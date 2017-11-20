using JetBrains.Annotations;

namespace Assets.Scripts.GameLogicLayerTD.Runes
{
    public class ElementRune : UpgradeRune
    {
        public ElementType ElementType { get; private set; }

        public ElementRune(string name, ElementType type) : base(true, false, true, false, false, name)
        {
            ElementType = type;
        }
    }
}