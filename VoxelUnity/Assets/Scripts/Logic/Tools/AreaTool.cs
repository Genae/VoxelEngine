using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Map;
using UnityEngine;

namespace Assets.Scripts.Logic.Tools
{
    public abstract class AreaTool : Tool
    {
        private Vector3 _startPos;
        private GameObject _plane;
        private GameObject _previewBox;
        private int _ySize;
        private bool _yAxisPressed;
        public int MaxLength = 20;

        protected virtual void Update()
        {
            CheckYAxis();
            var hit = GetRaycastHitOnMousePosition();
            if (hit.Length == 0)
                return;
            if (Input.GetMouseButtonDown(1))
            {
                StopArea();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                StartArea();
            }
            else
            {
                if (hit.Any(h => h.collider.gameObject.tag.Equals("Plane")))
                {
                    var myHit = hit.First(h => h.collider.gameObject.tag.Equals("Plane"));
                    var curPos = new Vector3((int)(myHit.point.x + 0.5f), _startPos.y + _ySize, (int)(myHit.point.z + 0.5f));
                    curPos = Normalize(curPos);
                    _previewBox = DrawPreview(_startPos, curPos, FindObjectOfType<MouseController>().PreviewMaterial, GetPreviewColor(), _previewBox);
                    if (Input.GetMouseButtonUp(0))
                    {
                        var voxels = GetVoxelsInbetween(_startPos, curPos);
                        StartAction(voxels);
                        StopArea();
                    }
                }
            }
        }

        protected abstract void StartAction(IEnumerable<Vector3> voxels);
        protected abstract Color GetPreviewColor();

        protected Vector3 Normalize(Vector3 curPos)
        {
            var xLength = curPos.x - _startPos.x;
            var yLength = curPos.y - _startPos.y;
            var zLength = curPos.z - _startPos.z;
            if (Mathf.Abs(xLength) > MaxLength)
            {
                curPos.x = _startPos.x + (Mathf.Abs(xLength) * MaxLength) / xLength;
            }
            if (Mathf.Abs(yLength) > MaxLength)
            {
                curPos.y = _startPos.y + (Mathf.Abs(yLength) * MaxLength) / yLength;
            }
            if (Mathf.Abs(zLength) > MaxLength)
            {
                curPos.z = _startPos.z + (Mathf.Abs(zLength) * MaxLength) / zLength;
            }

            return curPos;
        }

        protected void CreatePlane()
        {
            _plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            _plane.transform.localScale = new Vector3(1000, 1000, 1000);
            _plane.GetComponent<MeshRenderer>().enabled = false;
            _plane.transform.position = new Vector3(_startPos.x, _startPos.y + 0.5f, _startPos.z);
            _plane.tag = "Plane";
        }
        
        protected override void OnDisable()
        {
            StopArea();
            base.OnDisable();
        }

        protected void StopArea()
        {
            _ySize = 0;
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

        protected void StartArea()
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
        protected void CheckYAxis()
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
    }
}