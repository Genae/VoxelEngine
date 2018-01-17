using System.Linq;
using Assets.Scripts.UI;

namespace Assets.Scripts.GameLogicLayerTD.Runes
{
    public class Raido : Rune //path
    {
        public int Number;
        public Raido() : base("Raido")
        {
        }

        public override void Update()
        {
            base.Update();
            var raidos = RuneRegistry.Runes.OfType<Raido>().ToList();
            if (raidos.Count >= 4)
            {
                TDMapPreview.Instance.RenderPreview();
            }
        }
    }
}
