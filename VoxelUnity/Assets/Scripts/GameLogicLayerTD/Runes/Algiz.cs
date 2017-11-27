using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayerTD.Runes
{
    public class Algiz : Rune { //Tower

        private List<UpgradeRune> _upgradeRunes = new List<UpgradeRune>();
        
        public override void Update()
        {
            base.Update();
            _upgradeRunes.Clear();
            transform.parent.GetComponent<MeshRenderer>().material.color = GetUpgradeRunes().Count > 0 ? Color.blue : Color.white;
        }

        public List<UpgradeRune> GetUpgradeRunes()
        {
            if(_upgradeRunes.Count == 0)
                _upgradeRunes = RuneRegistry.Runes.OfType<UpgradeRune>().ToList();
            if (_upgradeRunes.Count > 0)
                _upgradeRunes = _upgradeRunes.Where(u => transform.Equals(u.UpgradeTarget)).ToList();
            
            foreach (var perthoRune in _upgradeRunes.OfType<Pethro>().ToList())
            {
                var result = TransformRune(perthoRune);
                _upgradeRunes.Remove(perthoRune);
                _upgradeRunes.Add(result);
            }
            return _upgradeRunes;
        }

        private UpgradeRune TransformRune(Pethro perthoRune)
        {
            var index = Random.Range(0, 16);
            UpgradeRune result = null;
            switch (index)
            {
                case 0:
                    result = perthoRune.gameObject.AddComponent<Ansuz>();
                    break;
                case 1:
                    result = perthoRune.gameObject.AddComponent<Berkano>();
                    break;
                case 2:
                    result = perthoRune.gameObject.AddComponent<Dagaz>();
                    break;
                case 3:
                    result = perthoRune.gameObject.AddComponent<Ehwaz>();
                    break;
                case 4:
                    result = perthoRune.gameObject.AddComponent<Gebo>();
                    break;
                case 5:
                    result = perthoRune.gameObject.AddComponent<Hagalaz>();
                    break;
                case 6:
                    result = perthoRune.gameObject.AddComponent<Ihwaz>();
                    break;
                case 7:
                    result = perthoRune.gameObject.AddComponent<Isa>();
                    break;
                case 8:
                    result = perthoRune.gameObject.AddComponent<Kenaz>();
                    break;
                case 9:
                    result = perthoRune.gameObject.AddComponent<Laguz>();
                    break;
                case 10:
                    result = perthoRune.gameObject.AddComponent<Naudhiz>();
                    break;
                case 11:
                    result = perthoRune.gameObject.AddComponent<Othala>();
                    break;
                case 12:
                    result = perthoRune.gameObject.AddComponent<Sowilo>();
                    break;
                case 13:
                    result = perthoRune.gameObject.AddComponent<Thurisaz>();
                    break;
                case 14:
                    result = perthoRune.gameObject.AddComponent<Tiwaz>();
                    break;
                case 15:
                    result = perthoRune.gameObject.AddComponent<Wunjo>();
                    break;
            }
            Debug.Log("morphed pertho into " + result.Name);
            Destroy(perthoRune);
            return result;
        }

        public Algiz() : base("Algiz")
        {
        }
    }
}