using Assets.Scripts.GameLogicLayerTD.Runes;
using UnityEngine;

public class TDVillage{

	public Village Village;
    public Mannaz Marker;
    public UpgradeRune Upgrades;

	public TDVillage(GameObject marker){
		Village = new GameObject("Village").AddComponent<Village>();
	    Marker = marker.GetComponentInChildren<Mannaz>();
	    Village.Init(Marker);
	    Village.transform.position = marker.transform.position;
	    Village.transform.parent = GameObject.Find("Map").transform;
	    var prefab = Object.Instantiate(GameObject.Find("village"));
        //TODO Placeholder stuff
	    prefab.transform.parent = Village.transform;
	    prefab.transform.localPosition = Vector3.zero + Vector3.up * 0.5f;
	    prefab.transform.name = "TowerMesh";
	}
}

public class Village : MonoBehaviour
{
    public Mannaz Marker;

    public void Init(Mannaz marker)
    {
        Marker = marker;
    }
}
