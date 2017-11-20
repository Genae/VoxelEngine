using MarkLight;
using Assets.Scripts.EngineLayer.Voxels.Containers;
using UnityEngine;
using Assets.Scripts.GameLogicLayerTD;

namespace Assets.Scripts.UI
{
    public class UITDARScene : View
    {
        public void ClickStart()
        {
            var tdMap = Map.Instance.gameObject.AddComponent<TDMap>();
            tdMap.BuildMap();
        }


        public void ClickClear()
        {
            Destroy(Map.Instance.gameObject.GetComponent<TDMap>());
            foreach (Transform child in Map.Instance.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
