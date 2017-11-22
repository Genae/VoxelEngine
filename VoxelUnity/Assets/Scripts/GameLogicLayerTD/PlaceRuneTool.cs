using Assets.Scripts.AccessLayer.Tools;
using System.Linq;
using Assets.Scripts.GameLogicLayerTD.Runes;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayer.Tools
{
    public class PlaceRuneTool : Tool {

        public GameObject RuneToPlace;
		public static GameObject MarkerParent;
        public string runeId;

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
            if (pos == Vector3.zero)
                return;
			DrawPreview(pos);
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				PlaceRune(pos);
			}
        }

        protected virtual Transform PlaceRune(Vector3 pos)
        {
            var currentMarker = MarkerParent.transform.Find(gameObject.name);
            if (currentMarker == null)
            {
                currentMarker = Instantiate(RuneToPlace).transform;
                currentMarker.gameObject.name = gameObject.name;
                currentMarker.parent = MarkerParent.transform;
            }
            currentMarker.position = RuneToPlace.transform.position;
            gameObject.SetActive(false);
            return currentMarker;
        }
        
		private Vector3 GetPos()
		{
			var hits = GetRaycastHitOnMousePosition();
			if (hits == null || hits.Length == 0)
				return Vector3.zero;
			var firstChunkHit = hits.FirstOrDefault(h => h.collider.gameObject.name.Equals("Table"));
			if(firstChunkHit.transform == null)
				return Vector3.zero;
			return new Vector3((int)firstChunkHit.point.x, (int)firstChunkHit.point.y + 0.01f, (int)firstChunkHit.point.z);
		}

        protected override void OnDisable()
        {
            Destroy(RuneToPlace);
            base.OnDisable();
        }

        private void DrawPreview(Vector3 startPos)
        {
            if (RuneToPlace == null)
            {
                RuneToPlace = GameObject.CreatePrimitive(PrimitiveType.Plane);
                RuneToPlace.GetComponent<MeshRenderer>().material = Resources.Load(string.Format("Runes/Materials/{0}", runeId), typeof(Material)) as Material;
                RuneToPlace.name = "preview";
                AddRuneComponent(RuneToPlace.gameObject, runeId);
            }
            RuneToPlace.transform.position = startPos;
            RuneToPlace.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        
        public virtual void AddRuneComponent(GameObject marker, string runeId)
        {
            var rune = new GameObject();
            rune.transform.parent = marker.transform;
            switch (runeId)
            {

            }
            rune.AddComponent<Raido>().Number = 0;
        }

        public override void SwapOverlays()
        {
            OverlayManager.SwapOverlays(false, false, false);
        }
    }
}
