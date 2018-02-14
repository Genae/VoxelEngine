using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayerTD.Runes
{
    public class UpgradeableRune : Rune
    {
        protected List<UpgradeRune> UpgradeRunes = new List<UpgradeRune>();

        public override void Update()
        {
            base.Update();
            UpgradeRunes.Clear();
            GetUpgradeRunes();
            var mr = transform.parent.GetComponent<MeshRenderer>();
            if (mr != null)
                mr.material.color = UpgradeRunes.Count > 0 ? Color.blue : Color.white;
        }

        public List<UpgradeRune> GetUpgradeRunes()
        {
            if (UpgradeRunes.Count == 0)
                UpgradeRunes = RuneRegistry.Runes.OfType<UpgradeRune>().ToList();
            if (UpgradeRunes.Count > 0)
                UpgradeRunes = UpgradeRunes.Where(u => transform.Equals(u.UpgradeTarget)).ToList();

            foreach (var perthoRune in UpgradeRunes.OfType<Pethro>().ToList())
            {
                var result = perthoRune.Transform(this);
                UpgradeRunes.Remove(perthoRune);
                UpgradeRunes.Add(result);
            }
            return UpgradeRunes;
        }

        public UpgradeableRune(string name) : base(name)
        {
        }
    }
}