using System.Linq;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayerTD.Runes
{
    public class Algiz : UpgradeableRune { //Tower

        private GameObject _rangeIndicator;
        MeshRenderer meshRenderer;
        public Algiz() : base("Algiz")
        {
        }

        void Start()
        {
            _rangeIndicator = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            _rangeIndicator.transform.parent = transform;
            meshRenderer = _rangeIndicator.GetComponent<MeshRenderer>();
            SetFade(meshRenderer.material);
            meshRenderer.material.color = new Color(0.1f, 0.1f, 0.1f, 0.1f);
        }

        public override void Update()
        {
            base.Update();
            meshRenderer.enabled = !ResourceOverview.Instance.Running;
            if (ResourceOverview.Instance.Running)
                return;
            float range = Tower.Range;
            var thurisaz = UpgradeRunes.OfType<Thurisaz>().ToList();
            if (thurisaz.Any())
            {
                var rangeMultiplier = Mathf.Pow(1.5f, thurisaz.Count);
                range = range * rangeMultiplier * Tower.ThurisazRangeMultiplier;
            }
            _rangeIndicator.transform.position = transform.position + Vector3.up * 3;
            _rangeIndicator.transform.localScale = new Vector3(range * 2, 0.1f, range * 2);
        }

        private void SetFade(Material material)
        {
            material.SetFloat("_Mode", 2);
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
        }
    }
}