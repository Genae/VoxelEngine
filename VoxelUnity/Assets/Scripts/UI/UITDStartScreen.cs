using MarkLight;
using Assets.Scripts.EngineLayer.Voxels.Containers;
using UnityEngine;
using Assets.Scripts.GameLogicLayerTD;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI
{
    public class UITDStartScreen : View
    {
        public void ClickStartAR()
        {
            SceneManager.LoadScene(1);
        }

        public void ClickStartDefault()
        {
            SceneManager.LoadScene(2);
        }
    }
}
