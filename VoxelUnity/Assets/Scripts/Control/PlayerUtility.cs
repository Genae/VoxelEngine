using UnityEngine;
using Assets.Scripts.Data.Map;


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
                        var x = ((int)hit.point.x + 1) % Chunk.ChunkSize;
                        var y = ((int)hit.point.y + 1) % Chunk.ChunkSize;
                        var z = ((int)hit.point.z + 1) % Chunk.ChunkSize;
                        Debug.Log("Position in Chunk: " + x + "|" + y + "|" + z);
                        _clickedChunk.ChunkData.Voxels[x, y, z].BlockType = 0;
                    }
                }
            }
        }

    }
}

