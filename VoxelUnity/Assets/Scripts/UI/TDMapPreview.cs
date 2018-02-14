using System.Collections.Generic;
using Assets.Scripts.EngineLayer.Voxels.Containers;
using Assets.Scripts.GameLogicLayerTD;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class TDMapPreview : MonoBehaviour
    {

        private bool _renderPreview;
        private LineRenderer _lr;
        private CampaignManager _cm;
        public GameObject RunePreview;

        private static TDMapPreview _instance;

        public static TDMapPreview Instance
        {
            get { return _instance ?? (_instance = Map.Instance.gameObject.AddComponent<TDMapPreview>()); }
            set { _instance = value; }
        }

        public static bool IsInstantiated { get { return _instance != null; } }

        void Start()
        {
            _cm = CampaignManager.Instance;
            if ((_lr = gameObject.GetComponent<LineRenderer>()) == null)
            {
                _lr = gameObject.AddComponent<LineRenderer>();
                var mat = new Material(Shader.Find("Standard"));
                mat.color = Color.black;
                _lr.material = mat;
            }
        }

        public void RenderPreview()
        {
            _renderPreview = true;
        }



        void Update()
        {
            if (!_renderPreview)
                return;
            var mapInfo = _cm.GetMapInfo();
            var size = TDMap.GetSize(mapInfo.Village);
            var path = mapInfo.GetPath(size);
            ConfigLineRenderer(path);
            if(CampaignManager.Instance.CurrentLevel != 0) 
                RenderVillagePreview(mapInfo.GetVillagePos(size));
        }

        private void RenderVillagePreview(Vector3 startPos)
        {
            if (RunePreview == null)
            {
                RunePreview = GameObject.CreatePrimitive(PrimitiveType.Plane);
                RunePreview.GetComponent<MeshRenderer>().material = Resources.Load("Runes/Materials/mannaz", typeof(Material)) as Material;
                SetFade(RunePreview.GetComponent<MeshRenderer>().material);
                RunePreview.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 0.5f);
                RunePreview.name = "preview";
                RunePreview.transform.RotateAround(RunePreview.transform.position, Vector3.up, 180);
            }
            RunePreview.transform.position = startPos;
            RunePreview.transform.localScale = new Vector3(1f, 1f, 1f);
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

        private void ConfigLineRenderer(List<Vector3> path)
        {
            _lr.positionCount = path.Count;
            for (var i = 0; i < path.Count; i++)
            {
                _lr.SetPosition(i, path[i]);

            }
            _lr.startWidth = 1;
            _lr.endWidth = 1;
        }
    }
}
