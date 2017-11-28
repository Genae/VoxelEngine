using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayerTD.Runes
{
    public class Pethro : UpgradeRune //Random?
    {
        public Pethro() : base(true, true, true, true, false, "Pethro")
        {
        }
        
        public UpgradeRune Transform(UpgradeableRune upgradeableRune)
        {
            if (upgradeableRune is Algiz)
                return TransformTower();
            if (upgradeableRune is Jera)
                return TransformFarm();
            return null;
        }

        private UpgradeRune TransformFarm()
        {
            var perthoRune = this;
            var index = Random.Range(0, 5);
            UpgradeRune result = null;
            switch (index)
            {
                case 0:
                    result = perthoRune.gameObject.AddComponent<Fehu>();
                    break;
                case 1:
                    result = perthoRune.gameObject.AddComponent<Ingwaz>();
                    break;
                case 2:
                    result = perthoRune.gameObject.AddComponent<Sowilo>();
                    break;
                case 3:
                    result = perthoRune.gameObject.AddComponent<Uruz>();
                    break;
                case 4:
                    result = perthoRune.gameObject.AddComponent<Wunjo>();
                    break;
            }
            Debug.Log("morphed pertho into " + result.Name);
            Destroy(perthoRune);
            return result;
        }

        private UpgradeRune TransformTower()
        {
            var perthoRune = this;
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
    }
}
