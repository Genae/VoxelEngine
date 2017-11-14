using System;
using Assets.Scripts.AccessLayer.Tools;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayer.Tools
{
    public class PlaceRuneTool : Tool {

        private GameObject _previewBox;
        public Material PreviewMaterial;
		public static GameObject MarkerParent;
        protected string RuneName;

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
				var currentMarker = MarkerParent.transform.Find(gameObject.name);
				if (currentMarker == null) {
					currentMarker = Instantiate (_previewBox).transform;
					currentMarker.gameObject.name = gameObject.name;
					currentMarker.parent = MarkerParent.transform;

					//hehe fuck you stefan
					if (currentMarker.name.Contains ("Upgrade")) {
						currentMarker.gameObject.AddComponent<Upgrade>();
					}

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
			return new Vector3((int)firstChunkHit.point.x, (int)firstChunkHit.point.y + 0.01f, (int)firstChunkHit.point.z);
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
                _previewBox = GameObject.CreatePrimitive(PrimitiveType.Plane);
                _previewBox.GetComponent<MeshRenderer>().material = Resources.Load(string.Format("Runes/Materials/{0}", RuneName), typeof(Material)) as Material;
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

    public class PlaceRuneToolV : PlaceRuneTool //VILLAGE
    {
        public PlaceRuneToolV()
        {
            RuneName = "mannaz";
        }
    }

    public class PlaceRuneToolP1 : PlaceRuneTool //PATH
    {
        public PlaceRuneToolP1()
        {
            RuneName = "raido";
        }
    }

    public class PlaceRuneToolP2 : PlaceRuneTool
    {
        public PlaceRuneToolP2()
        {
            RuneName = "raido";
        }
    }
	public class PlaceRuneToolP3 : PlaceRuneTool
	{
	    public PlaceRuneToolP3()
	    {
	        RuneName = "raido";
	    }
	}
	public class PlaceRuneToolTB : PlaceRuneTool //TOWERBASE
	{
	    public PlaceRuneToolTB()
	    {
	        RuneName = "algiz";
	    }
	}
	public class PlaceRuneToolF : PlaceRuneTool //FARM
	{
	    public PlaceRuneToolF()
	    {
	        RuneName = "jera";
	    }
	}
	public class PlaceRuneToolUS : PlaceRuneTool //SPEEDUPGRADE
	{
		public PlaceRuneToolUS()
		{
			RuneName = "ehwaz";
		}
	}
	public class PlaceRuneToolUAOE : PlaceRuneTool //AOEUPGRADE
	{
		public PlaceRuneToolUAOE()
		{
			RuneName = "hagalaz";
		}
	}
}
