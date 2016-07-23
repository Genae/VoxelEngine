using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Logic.Tools
{
    public class DeleteTool : Tool {

        private MapData _mapData;
        private Vector3 _startPos;
        private GameObject _plane;
        private GameObject _previewBox;

        private Vector3 lastCur;

        public Material PreviewMaterial;
        private int _mouseScrollDelta;


        // Update is called once per frame
        void Update () {
	        //_mouseScrollDelta += (int)Input.mouseScrollDelta.y;
            var hit = GetRaycastHitOnMousePosition();
            if (hit.Length == 0)
                return;
            if (Input.GetMouseButtonDown(1))
            {
                StopDelete();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                _mouseScrollDelta = 0;
                Chunk chunkHit;
                var pos = GetMouseOveredVoxelPos(out chunkHit);
                if (chunkHit != null)
                {
                    _startPos = pos;
                    CreatePlane();
                }
            }
            else
            {
                if(hit.Any(h => h.collider.gameObject.tag.Equals("Plane")))
                {
                    var myHit = hit.First(h => h.collider.gameObject.tag.Equals("Plane"));
                    var curPos = new Vector3((int)(myHit.point.x + 0.5f), _startPos.y, (int)(myHit.point.z + 0.5f));
                    if (lastCur != curPos)
                    {
                        Debug.Log(curPos);
                    }
                    lastCur = curPos;
                    DrawPreview(_startPos, curPos);
                    if (Input.GetMouseButtonUp(0))
                    {
                        var voxels = GetVoxelsInbetween(_startPos, curPos);
                        foreach (var vox in voxels)
                        {
                            ClearVoxelAtPosition(vox);
                            StopDelete();
                        }
                    }
                }
            }
            
        }

        private void StopDelete()
        {
            if (_plane != null)
            {
                DestroyImmediate(_plane);
                _plane = null;
            }
            if (_previewBox != null)
            {
                DestroyImmediate(_previewBox);
                _previewBox = null;
            }
        }

        private void DrawPreview(Vector3 startPos, Vector3 curPos)
        {
            if (_previewBox == null)
            {
                _previewBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _previewBox.GetComponent<MeshRenderer>().material = PreviewMaterial;
            }
            _previewBox.transform.position = (curPos - startPos) / 2 + startPos;
            _previewBox.transform.position = new Vector3(((curPos - startPos) / 2 + startPos).x, ((curPos - startPos) / 2 + startPos).y, ((curPos - startPos) / 2 + startPos).z);
            _previewBox.transform.localScale = new Vector3(Mathf.Abs(startPos.x - curPos.x) + 1.1f, _mouseScrollDelta + 1.1f, Mathf.Abs(startPos.z - curPos.z) + 1.1f);
        }

        private void CreatePlane()
        {
            _plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            _plane.transform.localScale = new Vector3(1000, 1000, 1000);
            _plane.GetComponent<MeshRenderer>().enabled = false;
            _plane.transform.position =new Vector3(_startPos.x, _startPos.y+0.5f, _startPos.z);
            _plane.tag = "Plane";
        }

        private void ClearVoxelAtPosition(Vector3 pos)
        {
            if (_mapData == null)
            {
                _mapData = GameObject.Find("Map").GetComponent<Map>().MapData;
            }
            //for performance reasons this could be used to replace the meshcolliders TODO?
            var chunk = _mapData.Chunks[(int)pos.x / Chunk.ChunkSize, (int)pos.y / Chunk.ChunkSize, (int)pos.z / Chunk.ChunkSize];
            chunk.SetVoxelType((int)pos.x % Chunk.ChunkSize, (int)pos.y % Chunk.ChunkSize, (int)pos.z % Chunk.ChunkSize, MaterialRegistry.Air);
        }
    }
}
