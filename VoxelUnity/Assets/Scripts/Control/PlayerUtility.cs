using System;
using UnityEngine;
using Assets.Scripts.Data.Map;
using System.Collections.Generic;
using Assets.Scripts.Data.Material;


namespace Assets.Scripts.Control
{
    public class PlayerUtility : MonoBehaviour
    {

        private Camera _mainCamera;
        private MapData _mapData;
        private Chunk _clickedChunk;
        private Ray ray;
        private int _mouseScrollDelta;
        //hardcoded mapheight since I dont know where it is set
        private int _maxMapHeight;

        private List<Vector3> _voxelPosList = new List<Vector3>();

        void Start()
        {
            _mainCamera = transform.gameObject.GetComponentInChildren<Camera>();
        }

        void Update()
        {
            RemoveVoxels();
            
            Debug.DrawRay(ray.origin, ray.direction * 10000, Color.yellow);
        }

        private void RemoveVoxels()
        {
            _mouseScrollDelta += (int)Input.mouseScrollDelta.y;
            if (Input.GetMouseButtonDown(0))
            {
                _mouseScrollDelta = 0;
                var hit = GetRaycastHitOnMousePosition();
                if (hit.collider != null)
                {
                    _clickedChunk = hit.transform.gameObject.GetComponent<Chunk>();
                    if (hit.transform.gameObject.tag == "Chunk")
                    {
                        _voxelPosList.Add(GetVoxelOnHit(hit));
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                var hit = GetRaycastHitOnMousePosition();
                if (hit.collider != null)
                {
                    _clickedChunk = hit.transform.gameObject.GetComponent<Chunk>();
                    if (hit.transform.gameObject.tag == "Chunk")
                    {
                        _voxelPosList.Add(GetVoxelOnHit(hit));

                        GetVoxelsInbetween();

                        foreach (Vector3 voxPos in _voxelPosList)
                        {
                            ClearVoxelAtPosition(voxPos);
                        }
                        _voxelPosList.Clear();
                    }
                }
            }
        }

        private void GetVoxelsInbetween()
        {
            if (_voxelPosList.Count < 2)
                return;
            var pos1 = _voxelPosList[0];
            var pos2 = _voxelPosList[1];
            var x1 = (int)pos1.x;
            var x2 = (int)pos2.x;
            var y1 = (int)pos1.y;
            var z1 = (int)pos1.z;
            var z2 = (int)pos2.z;

            //outofbounds catching
            if(y1 + _mouseScrollDelta > _maxMapHeight)
            {
                _mouseScrollDelta = _maxMapHeight - y1 - 1;
            }
            if(y1 + _mouseScrollDelta < 0)
            {
                _mouseScrollDelta = -y1 + 1;
            }

            if(_mouseScrollDelta >= 0)
            {
                if (x1 <= x2 && z1 <= z2)
                {
                    for (int x = x1; x <= x2; x++)
                    {
                        for(int y = y1; y <= pos1.y + _mouseScrollDelta; y++)
                        {
                            for (int z = z1; z <= z2; z++)
                            {
                                _voxelPosList.Add(new Vector3(x, y, z));
                            }
                        }
                    }
                }
                else if (x1 >= x2 && z1 <= z2)
                {
                    for (int x = x2; x <= x1; x++)
                    {
                        for (int y = y1; y <= pos1.y + _mouseScrollDelta; y++)
                        {
                            for (int z = z1; z <= z2; z++)
                            {
                                _voxelPosList.Add(new Vector3(x, y, z));
                            }
                        }
                    }
                }
                else if (x1 <= x2 && z1 >= z2)
                {
                    for (int x = x1; x <= x2; x++)
                    {
                        for(int y = y1; y <= pos1.y + _mouseScrollDelta; y++)
                        {
                            for (int z = z2; z <= z1; z++)
                            {
                                _voxelPosList.Add(new Vector3(x, y, z));
                            }
                        }
                    }
                }
                else if (x1 >= x2 && z1 >= z2)
                {
                    for (int x = x2; x <= x1; x++)
                    {
                        for (int y = y1; y <= pos1.y + _mouseScrollDelta; y++)
                        {
                            for (int z = z2; z <= z1; z++)
                            {
                                _voxelPosList.Add(new Vector3(x, y, z));
                            }
                        }
                    }
                }
            }
            if (_mouseScrollDelta < 0)
            {
                if (x1 <= x2 && z1 <= z2)
                {
                    for (int x = x1; x <= x2; x++)
                    {
                        for (int y = y1 + _mouseScrollDelta; y <= y1; y++)
                        {
                            for (int z = z1; z <= z2; z++)
                            {
                                _voxelPosList.Add(new Vector3(x, y, z));
                            }
                        }
                    }
                }
                else if (x1 >= x2 && z1 <= z2)
                {
                    for (int x = x2; x <= x1; x++)
                    {
                        for (int y = y1 + _mouseScrollDelta; y <= y1; y++)
                        {
                            for (int z = z1; z <= z2; z++)
                            {
                                _voxelPosList.Add(new Vector3(x, y, z));
                            }
                        }
                    }
                }
                else if (x1 <= x2 && z1 >= z2)
                {
                    for (int x = x1; x <= x2; x++)
                    {
                        for (int y = y1 + _mouseScrollDelta; y <= y1; y++)
                        {
                            for (int z = z2; z <= z1; z++)
                            {
                                _voxelPosList.Add(new Vector3(x, y, z));
                            }
                        }
                    }
                }
                else if (x1 >= x2 && z1 >= z2)
                {
                    for (int x = x2; x <= x1; x++)
                    {
                        for (int y = y1 + _mouseScrollDelta; y <= y1; y++)
                        {
                            for (int z = z2; z <= z1; z++)
                            {
                                _voxelPosList.Add(new Vector3(x, y, z));
                            }
                        }
                    }
                }
            }
        }

        private void ClearVoxelAtPosition(Vector3 pos)
        {
            if(_mapData == null)
            {
                //will not work in Start() since Map is not active at the start 
                _mapData = GameObject.Find("Map").GetComponent<Map>().MapData;
                _maxMapHeight = _mapData.Chunks.GetLength(0) * Chunk.ChunkSize2;
            }
            //for performance reasons this could be used to replace the meshcolliders TODO?
            var chunk = _mapData.Chunks[(int)pos.x / Chunk.ChunkSize2, (int)pos.y / Chunk.ChunkSize2, (int)pos.z / Chunk.ChunkSize2];
            chunk.SetVoxelType((int)pos.x % Chunk.ChunkSize2, (int)pos.y % Chunk.ChunkSize2, (int)pos.z % Chunk.ChunkSize2, MaterialRegistry.Air);
        }

        private RaycastHit GetRaycastHitOnMousePosition()
        {
            RaycastHit hit;
            ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit, float.PositiveInfinity);
            return hit;
        }

        //only use if hit already contains raycast information
        private Vector3 GetVoxelOnHit(RaycastHit hit)
        {

            Vector3 vec1;
            Vector3 vec2;
            switch (GetAxis(hit.point.x, hit.point.y, hit.point.z))
            {
                case "x":
                    vec1 = new Vector3((int)hit.point.x, (int)(hit.point.y + 0.5f), (int)(hit.point.z + 0.5f));
                    vec2 = new Vector3(vec1.x + 1, vec1.y, vec1.z);
                    break;
                case "y":
                    vec1 = new Vector3((int)(hit.point.x + 0.5f), (int)hit.point.y, (int)(hit.point.z + 0.5f));
                    vec2 = new Vector3(vec1.x, vec1.y + 1, vec1.z);
                    break;
                case "z":
                    vec1 = new Vector3((int)(hit.point.x + 0.5f), (int)(hit.point.y + 0.5f), (int)hit.point.z);
                    vec2 = new Vector3(vec1.x, vec1.y, vec1.z + 1);
                    break;
                default:
                    Debug.Log("GetVoxelOnHit() error");
                    return new Vector3(0,0,0);
            }
            var vox1 = _clickedChunk.ContainerData.GetVoxelActive((int)vec1.x % Chunk.ChunkSize2, (int)vec1.y % Chunk.ChunkSize2, (int)vec1.z % Chunk.ChunkSize2);

            if (IsInChunk(vec1, _clickedChunk.transform.position) && vox1)
            {
                return vec1;
            }
            else
            {
                return vec2;
            }
        }

        private bool IsInChunk(Vector3 vec, Vector3 position)
        {
            return ((int) vec.x/Chunk.ChunkSize2)*Chunk.ChunkSize2 == (int) position.x &&
                   ((int) vec.y/Chunk.ChunkSize2)*Chunk.ChunkSize2 == (int) position.y &&
                   ((int) vec.z/Chunk.ChunkSize2)*Chunk.ChunkSize2 == (int) position.z;
        }

        private string GetAxis(float x, float y, float z)
        {
            if(Math.Abs((int)x - x + 0.5f) < 0.001f)
            {
                return "x";
            }
            if (Math.Abs((int)y - y + 0.5f) < 0.001f)
            {
                return "y";
            }
            if (Math.Abs((int)z - z +  0.5f) < 0.001f)
            {
                return "z";
            }
            return "";
        }
    }
}

