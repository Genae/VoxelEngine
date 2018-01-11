using MarkLight;
using Assets.Scripts.EngineLayer.Voxels.Containers;
using UnityEngine;
using Assets.Scripts.GameLogicLayerTD;

namespace Assets.Scripts.UI
{
    public class UITDScene : View
    {
        public void ClickStart()
        {
            var tdMap = Map.Instance.gameObject.AddComponent<TDMap>();
            tdMap.BuildMap();
        }


        public void ClickClear()
        {
            TDMap.Instance.Clear();
        }
    }
}
