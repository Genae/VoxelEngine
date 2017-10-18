using Assets.Scripts.AccessLayer.Tools;
using Assets.Scripts.EngineLayer.Voxels.Containers.Chunks;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayer.Tools
{
    public class PlaceRuneTool : Tool {

        private GameObject _previewBox;
        public Material PreviewMaterial;
		private static GameObject MarkerParent;

		void Start()
		{
			if (MarkerParent == null) {
				MarkerParent = new GameObject("Markers");
			}
		}

        // Update is called once per frame
        void Update () {
			var pos = GetPos();
			DrawPreview(pos);
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				var currentMarker = MarkerParent.transform.Find (gameObject.name).gameObject;
				if (currentMarker == null) {
					currentMarker = Instantiate (_previewBox);
					currentMarker.transform.parent = MarkerParent.transform;
				}
				currentMarker.transform.position = _previewBox.transform.position;
			}
        }

		private Vector3 GetPos()
		{
			var hits = GetRaycastHitOnMousePosition();
			if (hits == null || hits.Length == 0)
				return Vector3.zero;
			var firstChunkHit = hits.FirstOrDefault(h => h.collider.gameObject.name.Equals("Table"));
			if(firstChunkHit.transform == null)
				return Vector3.zero;
			return firstChunkHit.point;
		}

        protected override void OnDisable()
        {
            Destroy(_previewBox);
            base.OnDisable();
        }

        private void DrawPreview(Vector3 startPos)
        {
            if (_previewBox == null)
            {
                _previewBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _previewBox.GetComponent<MeshRenderer>().material = PreviewMaterial;
                _previewBox.name = "preview";
            }
            _previewBox.transform.position = startPos;
            _previewBox.transform.localScale = new Vector3(10f, 10f, 10f);

        }

        public override void SwapOverlays()
        {
            OverlayManager.SwapOverlays(false, false, false);
        }
    }
}
