using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayerTD.Runes
{
    public class Mannaz : UpgradeableRune //Village
    {
        public Mannaz() : base("Mannaz")
        {
        }


        public override void Update()
        {
            base.Update();
            transform.position = new Vector3(250, 0, 250);
            var mannaz = RuneRegistry.Runes.OfType<Mannaz>().ToList();
            if (mannaz.Count >= 1)
            {
                TDMapPreview.Instance.RenderPreview();
            }
        }
    }
}
