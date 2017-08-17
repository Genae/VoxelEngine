using Assets.Scripts.Data.Map;
using UnityEngine;

namespace Assets.Scripts.Logic.Tools
{
    public class MouseoverTool : Tool {

        private GameObject _previewBox;
        public Material PreviewMaterial;


        // Update is called once per frame
        void Update () {
	        //_mouseScrollDelta += (int)Input.mouseScrollDelta.y;
            Chunk chunkHit;
            var pos = GetMouseOveredVoxelPos(out chunkHit);
            if (chunkHit != null)
            {
                DrawPreview(pos);
            }
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
            _previewBox.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }

        public override void SwapOverlays()
        {
            OverlayManager.SwapOverlays(false, false, false);
        }
    }
}
