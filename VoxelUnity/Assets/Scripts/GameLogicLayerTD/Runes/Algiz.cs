using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayerTD.Runes
{
    public class Algiz : Rune { //Tower

        public override void Update()
        {
            base.Update();
            transform.parent.GetComponent<MeshRenderer>().material.color = GetUpgradeRunes().Count > 0 ? Color.blue : Color.white;
        }

        public List<UpgradeRune> GetUpgradeRunes()
        {
            var upgrades = RuneRegistry.Runes.OfType<UpgradeRune>().ToList();
            if (upgrades.Count > 0)
                upgrades = upgrades.Where(u => transform.Equals(u.UpgradeTarget)).ToList();
            return upgrades;
        }

        public Algiz() : base("Algiz")
        {
        }
    }
}