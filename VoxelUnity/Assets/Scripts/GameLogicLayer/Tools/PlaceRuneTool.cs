using Assets.Scripts.AccessLayer.Tools;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayer.Tools
{
    public class PlaceRuneTool : Tool {

        private GameObject _previewBox;
        public Material PreviewMaterial;
		public static GameObject MarkerParent;

		protected override void Start()
		{
			base.Start();
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
				var currentMarker = MarkerParent.transform.Find(gameObject.name);
				if (currentMarker == null) {
					currentMarker = Instantiate (_previewBox).transform;
					currentMarker.gameObject.name = gameObject.name;
					currentMarker.parent = MarkerParent.transform;
				}
				currentMarker.position = _previewBox.transform.position;
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
            _previewBox.transform.localScale = new Vector3(1f, 1f, 1f);

        }

        public override void SwapOverlays()
        {
            OverlayManager.SwapOverlays(false, false, false);
        }
    }

	public class PlaceRuneToolV : PlaceRuneTool {}
	public class PlaceRuneToolP1 : PlaceRuneTool {}
	public class PlaceRuneToolP2 : PlaceRuneTool {}
	public class PlaceRuneToolP3 : PlaceRuneTool {}
	public class PlaceRuneToolTB : PlaceRuneTool {}
	public class PlaceRuneToolF : PlaceRuneTool {}
}
