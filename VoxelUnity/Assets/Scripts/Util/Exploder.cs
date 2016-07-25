using UnityEngine;
using System.Collections;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;

namespace Assets.Scripts.Util
{
	public class Exploder : MonoBehaviour {
		
		public static void Explode(VoxelContainer voxelContainer){

			var vc = voxelContainer;
			var cd = vc.ContainerData;

			for (int x = 0; x < cd.Size; x++) {
				for (int y = 0; y < cd.Size; y++) {
					for (int z = 0; z < cd.Size; z++) {
						//if (x % 2 == 1 && y % 2 == 1 && z % 2 == 1) {
							if (cd.Voxels [x, y, z] == null)
								continue;
							if (cd.Voxels [x, y, z].IsActive) {
								var pos = new Vector3 (cd.Position.x + x, cd.Position.y + y, cd.Position.z + z);
								var go = (GameObject)Instantiate (GameObject.CreatePrimitive(PrimitiveType.Cube), pos, Quaternion.identity);
								var colorID = cd.Voxels [x, y, z].BlockType;
								//var mat = MaterialRegistry.GetColorIndex (cd.Voxels [x, y, z].BlockType);
								//go.GetComponent<MeshRenderer>().sharedMaterial == mat; //this is madness stafan pls insert material ffs
							}
						//}
					}
				}
			}



		}
	}
}
