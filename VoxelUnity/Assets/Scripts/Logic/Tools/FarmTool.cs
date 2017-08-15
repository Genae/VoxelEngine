using System.Linq;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using Assets.Scripts.Logic.Farming;
using Assets.Scripts.Logic.Jobs;
using UnityEngine;

namespace Assets.Scripts.Logic.Tools
{
    public class FarmTool : Tool {

        private JobController _jobController;
        private MapData _mapData;
        private Vector3 _startPos;
        private GameObject _plane;
        private GameObject _previewBox;
        public Material PreviewMaterial;
        public int MaxLength = 20;
        private int _ySize;
        private bool _yAxisPressed;

        
        protected void Update () {

            CheckYAxis();

            var hit = GetRaycastHitOnMousePosition();
            if (hit.Length == 0)
                return;
            if (Input.GetMouseButtonDown(1))
            {
                StopCreateFarm();
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
                    curPos = Normalize(curPos);
                    _previewBox = DrawPreview(_startPos, curPos, PreviewMaterial, new Color(0f, 0.3f, 0.3f, 0.5f), _previewBox);
                    if (Input.GetMouseButtonUp(0))
                    {
                        var voxels = GetVoxelsInbetween(_startPos, curPos);
                        Farm farm = null;
                        foreach (var vox in voxels)
                        {
                            if (CreateFarmAtPosition(vox))
                            {
                                if (farm == null)
                                {
                                    farm = new GameObject("Farm").AddComponent<Farm>();
                                    farm.transform.parent = GameObject.Find("Map").transform;
                                    farm.CropType = CropManager.Instance.GetCropByName("Wheat");
                                }
                                farm.AddFarmblock(vox);
                            }
                        }
                        StopCreateFarm();
                    }
                }
            }
        }

        private Vector3 Normalize(Vector3 curPos)
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

        protected override void OnDisable()
        {
            StopCreateFarm();
            base.OnDisable();
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

        private void StopCreateFarm()
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

        private void CreatePlane()
        {
            _plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            _plane.transform.localScale = new Vector3(1000, 1000, 1000);
            _plane.GetComponent<MeshRenderer>().enabled = false;
            _plane.transform.position =new Vector3(_startPos.x, _startPos.y+0.5f, _startPos.z);
            _plane.tag = "Plane";
        }

        private bool CreateFarmAtPosition(Vector3 pos)
        {
            if (!Map.Instance.IsInBounds((int) pos.x, (int) pos.y, (int) pos.z))
                return false;
            if (_jobController == null)
            {
                _jobController = GameObject.Find("World").GetComponent<JobController>();
            }
            if (_jobController.HasJob(pos, JobType.CreateSoil))
                return false;

            if (_mapData == null)
            {
                _mapData = Map.Instance.MapData;
            }
            var type = Map.Instance.MapData.GetVoxelMaterial(pos);
            var air = MaterialRegistry.Instance.GetMaterialFromName("Air");
            if(type.Equals(air) || !(type.Equals(MaterialRegistry.Instance.GetMaterialFromName("Dirt")) || type.Equals(MaterialRegistry.Instance.GetMaterialFromName("Grass"))))
                return false;
            type = Map.Instance.MapData.GetVoxelMaterial(pos + Vector3.up);
            if (!type.Equals(air))
                return false;
            type = Map.Instance.MapData.GetVoxelMaterial(pos + Vector3.up + Vector3.up);
            if (!type.Equals(air))
                return false;
            _jobController.AddJob(new CreateSoilJob(pos));
            return true;
        }

        

        public override void SwapOverlays()
        {
            OverlayManager.SwapOverlays(true, true, true);
        }
    }
}
