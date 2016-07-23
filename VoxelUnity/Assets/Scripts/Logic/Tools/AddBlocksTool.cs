using System.Linq;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Logic.Tools
{
    public class AddBlocksTool : Tool {

        private MapData _mapData;
        private Vector3 _startPos;
        private GameObject _plane;
        private GameObject _previewBox;
        public Material PreviewMaterial;
        public int BlockMaterialId;
        private int _ySize;
        private bool _yAxisPressed;

        
        // ReSharper disable once UnusedMember.Local
        void Update () {

            CheckYAxis();

            var hit = GetRaycastHitOnMousePosition();
            if (hit.Length == 0)
                return;
            if (Input.GetMouseButtonDown(1))
            {
                StopAdd();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                _ySize = 0;
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
                    var curPos = new Vector3((int)(myHit.point.x + 0.5f), _startPos.y + _ySize, (int)(myHit.point.z + 0.5f));
                    PreviewMaterial.color = MaterialRegistry.MaterialFromId(BlockMaterialId).Color;
                    _previewBox = DrawPreview(_startPos, curPos, PreviewMaterial, _previewBox);
                    if (Input.GetMouseButtonUp(0))
                    {
                        var voxels = GetVoxelsInbetween(_startPos, curPos);
                        foreach (var vox in voxels)
                        {
                            AddVoxelAtPosition(vox);
                            StopAdd();
                        }
                    }
                }
            }
        }

        void OnDisable()
        {
            StopAdd();
        }

        private void CheckYAxis()
        {
            if (!_yAxisPressed)
            {
                if (Input.GetAxis("EnlargeSelectionY") < 0)
                {
                    _ySize -= 1;
                    _yAxisPressed = true;
                }
                else if (Input.GetAxis("EnlargeSelectionY") > 0)
                {
                    _ySize += 1;
                    _yAxisPressed = true;
                }
            }
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_yAxisPressed && Input.GetAxis("EnlargeSelectionY") == 0)
            {
                _yAxisPressed = false;
            }
        }

        private void StopAdd()
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

        private void CreatePlane()
        {
            _plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            _plane.transform.localScale = new Vector3(1000, 1000, 1000);
            _plane.GetComponent<MeshRenderer>().enabled = false;
            _plane.transform.position =new Vector3(_startPos.x, _startPos.y+0.5f, _startPos.z);
            _plane.tag = "Plane";
        }

        private void AddVoxelAtPosition(Vector3 pos)
        {
            if (_mapData == null)
            {
                _mapData = GameObject.Find("Map").GetComponent<Map>().MapData;
            }
            //for performance reasons this could be used to replace the meshcolliders TODO?
            var chunk = _mapData.Chunks[(int)pos.x / Chunk.ChunkSize, (int)pos.y / Chunk.ChunkSize, (int)pos.z / Chunk.ChunkSize];
            chunk.SetVoxelType((int)pos.x % Chunk.ChunkSize, (int)pos.y % Chunk.ChunkSize, (int)pos.z % Chunk.ChunkSize, MaterialRegistry.MaterialFromId(BlockMaterialId));
        }
    }
}
