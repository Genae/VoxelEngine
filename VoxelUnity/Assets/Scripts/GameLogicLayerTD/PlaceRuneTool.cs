using System.Collections.Generic;
using Assets.Scripts.AccessLayer.Tools;
using System.Linq;
using Assets.Scripts.GameLogicLayerTD.Runes;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayer.Tools
{
    public class PlaceRuneTool : Tool {

        public GameObject RunePreview;
		public static GameObject MarkerParent;
        public string runeId;
        public Dictionary<string, int> counter = new Dictionary<string, int>();

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

            var currentMarker = Instantiate(RunePreview).transform;
            currentMarker.gameObject.name = "Marker " + runeId + GetRuneCounter();
            counter[runeId]++;
            currentMarker.parent = MarkerParent.transform;
            currentMarker.position = RunePreview.transform.position;
            gameObject.SetActive(false);
            return currentMarker;
        }

        private int GetRuneCounter()
        {
            if (!counter.ContainsKey(runeId))
                counter.Add(runeId, 0);
            return counter[runeId];
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
            Destroy(RunePreview);
            base.OnDisable();
        }

        private void DrawPreview(Vector3 startPos)
        {
            if (RunePreview == null)
            {
                RunePreview = GameObject.CreatePrimitive(PrimitiveType.Plane);
                RunePreview.GetComponent<MeshRenderer>().material = Resources.Load(string.Format("Runes/Materials/{0}", runeId), typeof(Material)) as Material;
                RunePreview.name = "preview";
                AddRuneComponent(RunePreview);
            }
            RunePreview.transform.position = startPos;
            RunePreview.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        
        public virtual void AddRuneComponent(GameObject marker)
        {
            var rune = new GameObject(runeId);
            rune.transform.parent = marker.transform;
            switch (runeId)
            {
                case "fehu":
                    rune.AddComponent<Fehu>();
                    break;
                case "uruz":
                    rune.AddComponent<Uruz>();
                    break;
                case "thurisaz":
                    rune.AddComponent<Thurisaz>();
                    break;
                case "ansuz":
                    rune.AddComponent<Ansuz>();
                    break;
                case "raido":
                    rune.AddComponent<Raido>().Number = GetRuneCounter();
                    break;
                case "kenaz":
                    rune.AddComponent<Kenaz>();
                    break;
                case "gebo":
                    rune.AddComponent<Gebo>();
                    break;
                case "wunjo":
                    rune.AddComponent<Wunjo>();
                    break;
                case "hagalaz":
                    rune.AddComponent<Hagalaz>();
                    break;
                case "naudhiz":
                    rune.AddComponent<Naudhiz>();
                    break;
                case "isa":
                    rune.AddComponent<Isa>();
                    break;
                case "jera":
                    rune.AddComponent<Jera>();
                    break;
                case "ihwaz":
                    rune.AddComponent<Ihwaz>();
                    break;
                case "perthro":
                    rune.AddComponent<Pethro>();
                    break;
                case "algiz":
                    rune.AddComponent<Algiz>();
                    break;
                case "sowilo":
                    rune.AddComponent<Sowilo>();
                    break;
                case "tiwaz":
                    rune.AddComponent<Tiwaz>();
                    break;
                case "berkano":
                    rune.AddComponent<Berkano>();
                    break;
                case "ehwaz":
                    rune.AddComponent<Ehwaz>();
                    break;
                case "mannaz":
                    rune.AddComponent<Mannaz>();
                    break;
                case "laguz":
                    rune.AddComponent<Laguz>();
                    break;
                case "ingwaz":
                    rune.AddComponent<Ingwaz>();
                    break;
                case "othala":
                    rune.AddComponent<Othala>();
                    break;
                case "dagaz":
                    rune.AddComponent<Dagaz>();
                    break;
            }
        }

        public override void SwapOverlays()
        {
            OverlayManager.SwapOverlays(false, false, false);
        }
    }
}
