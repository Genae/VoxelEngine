using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Map;
using UnityEngine;

namespace Assets.Scripts.Logic.Tools
{
    public class Tool : MonoBehaviour
    {
        void Awake()
        {
            gameObject.SetActive(false);
        }

        protected Vector3 GetMouseOveredVoxelPos(out Chunk chunk)
        {
            chunk = null;
            var hits = GetRaycastHitOnMousePosition();
            if (hits == null || hits.Length == 0)
                return Vector3.zero;
            var firstChunkHit = hits.FirstOrDefault(h => h.collider.gameObject.tag.Equals("Chunk"));
            if(firstChunkHit.transform == null)
                return Vector3.zero;
            chunk = firstChunkHit.transform.gameObject.GetComponent<Chunk>();
            return GetVoxelOnHit(firstChunkHit, chunk);
        }
        
        protected RaycastHit[] GetRaycastHitOnMousePosition()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return Physics.RaycastAll(ray, float.PositiveInfinity); ;
        }

        protected Vector3 GetVoxelOnHit(RaycastHit hit, Chunk chunk)
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
                    return new Vector3(0, 0, 0);
            }
            var vox1 = chunk.ContainerData.GetVoxelActive((int)vec1.x % Chunk.ChunkSize, (int)vec1.y % Chunk.ChunkSize, (int)vec1.z % Chunk.ChunkSize);

            if (IsInChunk(vec1, chunk.transform.position) && vox1)
            {
                return vec1;
            }
            return vec2;
        }

        protected bool IsInChunk(Vector3 vec, Vector3 position)
        {
            return ((int)vec.x / Chunk.ChunkSize) * Chunk.ChunkSize == (int)position.x &&
                   ((int)vec.y / Chunk.ChunkSize) * Chunk.ChunkSize == (int)position.y &&
                   ((int)vec.z / Chunk.ChunkSize) * Chunk.ChunkSize == (int)position.z;
        }

        protected string GetAxis(float x, float y, float z)
        {
            //Debug.Log(x + " " + y + " " + z);
            if (Math.Abs((int)(1 + x) - x - 0.5f) < 0.001f)
            {
                return "x";
            }
            if (Math.Abs((int)(1 + y) - y - 0.5f) < 0.001f)
            {
                return "y";
            }
            if (Math.Abs((int)(1 + z) - z - 0.5f) < 0.001f)
            {
                return "z";
            }
            return "";
        }

        protected IEnumerable<Vector3> GetVoxelsInbetween(Vector3 startPos, Vector3 endPos)
        {
            var inbetween = new List<Vector3>();
            for (var x = (int)Mathf.Min(startPos.x, endPos.x); x <= (int)Mathf.Max(startPos.x, endPos.x); x++)
            {
                for (var y = (int)Mathf.Min(startPos.y, endPos.y); y <= (int)Mathf.Max(startPos.y, endPos.y); y++)
                {
                    for (var z = (int)Mathf.Min(startPos.z, endPos.z); z <= (int)Mathf.Max(startPos.z, endPos.z); z++)
                    {
                        inbetween.Add(new Vector3(x, y, z));
                    }
                }
            }
            return inbetween;
        }

        protected GameObject DrawPreview(Vector3 startPos, Vector3 curPos, Material mat, GameObject previewBox = null)
        {
            if (previewBox == null)
            {
                previewBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                previewBox.GetComponent<MeshRenderer>().material = mat;
            }
            previewBox.transform.position = (curPos - startPos) / 2 + startPos;
            previewBox.transform.position = new Vector3(((curPos - startPos) / 2 + startPos).x, ((curPos - startPos) / 2 + startPos).y, ((curPos - startPos) / 2 + startPos).z);
            previewBox.transform.localScale = new Vector3(Mathf.Abs(startPos.x - curPos.x) + 1.1f, Mathf.Abs(startPos.y - curPos.y) + 1.1f, Mathf.Abs(startPos.z - curPos.z) + 1.1f);
            return previewBox;
        }
    }
}
