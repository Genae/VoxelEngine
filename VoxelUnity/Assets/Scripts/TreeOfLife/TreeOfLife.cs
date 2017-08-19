using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using UnityEngine;

public class TreeOfLife : MonoBehaviour {

	Stem[] StemOfLife;

    public float Scale = 100;
	//Root[] RootsOfLife;

	public GameObject DebugPrefab;
    
    public void GenerateTree()
    {
        DebugPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //debug
        CreateStemOfLife(Vector3.zero, 20, Random.Range(2,4), 40f);

        foreach (Stem s in StemOfLife)
        {
            foreach (TreeNode tn in s.Nodes)
            {
                //Instantiate(DebugPrefab, transform.position + tn.position, Quaternion.identity);
            }
            for (int i = 0; i < s.Nodes.Length - 1; i++)
            {
                s.Nodes[i].thickness = (s.Nodes.Length-i)/2f;
            }
            for (int i = 0; i < s.Nodes.Length - 1; i++)
            {
                ResourceManager.DrawCapsule(transform.position + s.Nodes[i].position, transform.position + s.Nodes[i + 1].position, (s.Nodes[i].thickness + s.Nodes[i+1].thickness)/2f, MaterialRegistry.Instance.GetMaterialFromName("OakWood"));
                //Debug.DrawLine(transform.position + s.Nodes[i].position, transform.position + s.Nodes[i + 1].position, Color.green, 10000f);
            }
        }

    }

    void CreateStemOfLife(Vector3 basePos, int nodeCount, int stemCount, float minDistance){
		StemOfLife = new Stem[stemCount];
		Vector3 StemBase = basePos;
		Vector3[] StemEnds = getStemEnds (stemCount, minDistance);
		StemOfLife[0] = new Stem(StemBase, StemEnds[0], nodeCount); // first = base Stem

		for (int i = 1; i < stemCount; i++) {
			var splitIndex = Random.Range (6, 12); //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
			TreeNode branchStemBase = StemOfLife [0].Nodes [splitIndex];
			StemOfLife [i] = new Stem (branchStemBase, StemEnds[i], nodeCount-splitIndex);
		}
	}

	Vector3[] getStemEnds(int number, float minDistance){
		Vector3[] ends = new Vector3[number];
		if (number > 1) {
			float minDist = 0;
			while (minDist < minDistance) {
				minDist = float.MaxValue;
				for (int i = 0; i < number; i++) {
					ends [i] = (Random.onUnitSphere + Vector3.up * 2).normalized * Scale;
				}
				for (int i = 0; i < number; i++) {
					for (int j = i + 1; j < number; j++) {
						var dist = Vector3.Distance (ends [i], ends [j]);
						if (minDist > dist) {
							minDist = dist;
						}
					}
				}
			}
		} else {
			ends [0] = (Random.onUnitSphere + Vector3.up * 2).normalized * Scale;
		}
		return ends;
	}




	// Update is called once per frame
	void Update () {
		
	}

}
