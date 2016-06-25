using UnityEngine;
using Assets.Scripts.Data.Map;


namespace Assets.Scripts.Control
{
    public class PlayerUtility : MonoBehaviour
    {

        private Camera _mainCamera;

        private Ray ray;

        void Start()
        {
            _mainCamera = this.gameObject.GetComponentInChildren<Camera>();
            if (_mainCamera == null) Debug.Log("_mainCamera in PlayerUtility is null");
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out hit, float.PositiveInfinity);
                if (hit.collider != null)
                {
                    Debug.Log(hit.transform.gameObject.name);
                    if(hit.transform.gameObject.tag == "Chunk")
                    {
                        var pos = hit.point;
                        int x = (int)pos.x + 1;
                        int y = (int)pos.y + 1;
                        int z = (int)pos.z + 1;
                        Debug.Log(pos + " : " + x + "|" + y + "|" + z);
                        x = x % Chunk.ChunkSize;
                        y = y % Chunk.ChunkSize;
                        z = z % Chunk.ChunkSize;
                        Debug.Log("Position in Chunk: " + x + "|" + y + "|" + z);
                    }
                }
            }
            
            Debug.DrawRay(ray.origin, ray.direction * 10000, Color.yellow);

        }

    }
}

