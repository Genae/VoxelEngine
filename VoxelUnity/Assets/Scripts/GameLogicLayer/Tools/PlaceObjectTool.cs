using Assets.Scripts.AccessLayer.Tools;
using Assets.Scripts.AccessLayer.Worlds;
using Assets.Scripts.Data;
using Assets.Scripts.EngineLayer.Voxels.Containers.Chunks;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayer.Tools
{
    public class PlaceObjectTool : Tool {

        private GameObject _preview;
        private GameObject _previewRotation;
        private GameObject _previewObj;
        public ItemType ItemToPlace;

        void Awake()
        {
            ItemToPlace = ItemManager.GetItemType("Chest");
        }
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
                obj.name = ItemToPlace.Name;
                var item = obj.transform.GetChild(0).gameObject;
                var c = item.GetComponent<Renderer>().material.color;
                item.GetComponent<Renderer>().material.color = new Color(c.r, c.g, c.b, 1);
                ItemManager.ActivateObject(item, ItemToPlace);
            }
            if (chunkHit != null && World.At(pos + Vector3.up).IsAir())
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
                _previewObj = ItemManager.GetModel(startPos, ItemToPlace);
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
