using System;
using UnityEngine;
using Assets.Scripts.Data.Map;
using UnityEditor;


namespace Assets.Scripts.Control
{
    public class PlayerUtility : MonoBehaviour
    {

        private Camera _mainCamera;
        private Chunk _clickedChunk;
        private Ray ray;

        void Start()
        {
            _mainCamera = this.gameObject.GetComponentInChildren<Camera>();
            if (_mainCamera == null) Debug.Log("_mainCamera in PlayerUtility.cs is null");
        }

        void Update()
        {

            GetClickedVoxel();
            
            Debug.DrawRay(ray.origin, ray.direction * 10000, Color.yellow);

        }

        private void GetClickedVoxel()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out hit, float.PositiveInfinity);
                if (hit.collider != null)
                {
                    Debug.Log(hit.transform.gameObject.name);
                    _clickedChunk = hit.transform.gameObject.GetComponent<Chunk>();
                    if (hit.transform.gameObject.tag == "Chunk")
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
                                return;
                        }
                        var vox1 = _clickedChunk.ChunkData.Voxels[(int)vec1.x % Chunk.ChunkSize, (int)vec1.y % Chunk.ChunkSize, (int)vec1.z % Chunk.ChunkSize];
                        var vox2 = _clickedChunk.ChunkData.Voxels[(int)vec2.x % Chunk.ChunkSize, (int)vec2.y % Chunk.ChunkSize, (int)vec2.z % Chunk.ChunkSize];

                        if (IsInChunk(vec1, _clickedChunk.transform.position) && vox1.IsActive)
                        {
                            vox1.BlockType = 0;
                        }
                        else
                        {
                            vox2.BlockType = 0;
                        }
                    }
                }
            }
        }

        private bool IsInChunk(Vector3 vec, Vector3 position)
        {
            return ((int) vec.x/Chunk.ChunkSize)*Chunk.ChunkSize == (int) position.x &&
                   ((int) vec.y/Chunk.ChunkSize)*Chunk.ChunkSize == (int) position.y &&
                   ((int) vec.z/Chunk.ChunkSize)*Chunk.ChunkSize == (int) position.z;
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

