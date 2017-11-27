using Assets.Scripts.EngineLayer.Util;
using Assets.Scripts.GameLogicLayerTD.Runes;
using UnityEngine;

public class TDTower{

	public Tower Tower;
    public Algiz Marker;
    public UpgradeRune Upgrades;

	public TDTower(GameObject marker){
		Tower = GameObject.Instantiate(GameObject.Find("tower")).AddComponent<Tower>();
	    Tower.gameObject.AddComponent<MeshCollider>();
	    Marker = marker.GetComponentInChildren<Algiz>();
        Tower.Init(Marker);
		Tower.transform.position = marker.transform.position;
		Tower.transform.parent = GameObject.Find("Map").transform;
	}

    public void Explode()
    {
        Object.Destroy(Tower);
    }
}
