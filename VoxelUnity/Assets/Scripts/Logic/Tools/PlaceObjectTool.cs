using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using Assets.Scripts.MultiblockHandling;
using UnityEngine;

namespace Assets.Scripts.Logic.Tools
{
    public class PlaceObjectTool : Tool {

        private GameObject _preview;
        private GameObject _previewRotation;
        private GameObject _previewObj;
        public string ObjectToPlace;

        // Update is called once per frame
        void Update ()
        {
            if (_preview == null)
            {
                _preview = new GameObject("preview");
                _previewRotation = new GameObject("rotation");
                _previewRotation.transform.parent = _preview.transform;
            }
            Chunk chunkHit;
            var pos = GetMouseOveredVoxelPos(out chunkHit);
            if (Input.GetKeyDown(KeyCode.R))
            {
                _previewRotation.transform.RotateAround(pos, Vector3.up, 90);
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var obj = Instantiate(_previewRotation);
                obj.transform.position += _preview.transform.position;
                obj.name = ObjectToPlace;
            }
            if (chunkHit != null && Map.Instance.MapData.GetVoxelMaterial(pos + Vector3.up).Equals(MaterialRegistry.Instance.GetMaterialFromName("Air")))
            {
                DrawPreview(pos);
            }
            else
            {
                if (_previewObj != null)
                    Destroy(_previewObj);
            }
        }
        protected override void OnDisable()
        {
            if (_preview != null)
                Destroy(_preview);
            base.OnDisable();
        }

        private void DrawPreview(Vector3 startPos)
        {
            if (_previewObj == null)
            {
                _previewObj = MultiblockLoader.LoadMultiblock(ObjectToPlace).gameObject;
                _previewObj.transform.parent = _previewRotation.transform;
                _previewObj.transform.localRotation = Quaternion.identity;
                _previewObj.transform.localPosition = Vector3.zero;
            }
            _preview.transform.position = startPos + Vector3.up - Vector3.one / 2f;
            var r = _previewObj.GetComponent<Renderer>();
            r.material.color = new Color(r.material.color.r, r.material.color.g, r.material.color.b, 0.5f);
            
        }

        public override void SwapOverlays()
        {
            OverlayManager.SwapOverlays(false, false, false);
        }
    }
}
