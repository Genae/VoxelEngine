using Assets.Scripts.EngineLayer.Util;
using Assets.Scripts.GameLogicLayerTD.Runes;
using UnityEngine;

public class TDTower{

	public Tower Tower;
    public Algiz Marker;
    public UpgradeRune Upgrades;

	public TDTower(GameObject marker){
		Tower = new GameObject("Tower").AddComponent<Tower>();
	    Marker = marker.GetComponentInChildren<Algiz>();
        Tower.Init(Marker);
		Tower.transform.position = marker.transform.position;
		Tower.transform.parent = GameObject.Find("Map").transform;
	    var prefab = GameObject.Instantiate(GameObject.Find("tower"));
        //TODO Placeholder stuff
	    prefab.transform.parent = Tower.transform;
	    prefab.transform.localPosition = Vector3.zero + Vector3.up * 0.5f;
	    prefab.transform.name = "TowerMesh";
	}

    public void Explode()
    {
        Object.Destroy(Tower);
    }
}
