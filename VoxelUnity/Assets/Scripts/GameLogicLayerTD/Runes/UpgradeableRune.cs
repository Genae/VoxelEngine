using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayerTD.Runes
{
    public class UpgradeableRune : Rune
    {
        private List<UpgradeRune> _upgradeRunes = new List<UpgradeRune>();

        public override void Update()
        {
            base.Update();
            _upgradeRunes.Clear();
            transform.parent.GetComponent<MeshRenderer>().material.color = GetUpgradeRunes().Count > 0 ? Color.blue : Color.white;
        }

        public List<UpgradeRune> GetUpgradeRunes()
        {
            if (_upgradeRunes.Count == 0)
                _upgradeRunes = RuneRegistry.Runes.OfType<UpgradeRune>().ToList();
            if (_upgradeRunes.Count > 0)
                _upgradeRunes = _upgradeRunes.Where(u => transform.Equals(u.UpgradeTarget)).ToList();

            foreach (var perthoRune in _upgradeRunes.OfType<Pethro>().ToList())
            {
                var result = perthoRune.Transform(this);
                _upgradeRunes.Remove(perthoRune);
                _upgradeRunes.Add(result);
            }
            return _upgradeRunes;
        }

        public UpgradeableRune(string name) : base(name)
        {
        }
    }
}