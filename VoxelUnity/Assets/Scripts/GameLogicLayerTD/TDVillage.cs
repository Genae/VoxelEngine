using System.Linq;
using Assets.Scripts.GameLogicLayerTD;
using Assets.Scripts.GameLogicLayerTD.Runes;
using Assets.Scripts.UI;
using JetBrains.Annotations;
using UnityEngine;

public class TDVillage {

	public Village Village;
    public Mannaz Marker;
    public UpgradeRune Upgrades;

	public TDVillage(GameObject marker)
	{
	    TDFarm.FlattenTerrain(marker.transform.position, 55);
		Village = new GameObject("Village").AddComponent<Village>();
	    Marker = marker.GetComponentInChildren<Mannaz>();
	    Village.Init(Marker);
	    Village.transform.position = marker.transform.position;
	    Village.transform.parent = GameObject.Find("Map").transform;
	    var prefab = Object.Instantiate(GameObject.Find("village"));
        //TODO Placeholder stuff
	    prefab.transform.parent = Village.transform;
	    prefab.transform.localPosition = Vector3.zero + Vector3.up * 0.5f;
	    prefab.transform.name = "VillageMesh";
	}
}

public class Village : MonoBehaviour
{
    public Mannaz Marker;

    public void Init(Mannaz marker)
    {
        Marker = marker;
        var col = gameObject.AddComponent<BoxCollider>();
        col.center = new Vector3(5, 0, 5);
        col.size = new Vector3(30, 15, 20);

        var othala = Marker.GetUpgradeRunes().OfType<Othala>().ToList();
        if (othala.Any())
        {
            ResourceOverview.Instance.Gold.Value += 50 * othala.Count;
            ResourceOverview.Instance.MaxGold = 500 + 150 * othala.Count;
        }

        var uruz = Marker.GetUpgradeRunes().OfType<Uruz>().ToList();
        if (uruz.Any())
        {
            ResourceOverview.Instance.Lives.Value += 5 * uruz.Count;
        }
    }

    void OnMouseDown()
    {
        var naudhiz = Marker.GetUpgradeRunes().OfType<Naudhiz>().ToList();
        if (naudhiz.Any())
        {
            var marker = new GameObject("DefenseTower");
            marker.AddComponent<Algiz>();
            marker.transform.parent = transform;
            marker.transform.localPosition = Vector3.zero;
            var tdTower = new TDTower(marker);
            tdTower.Tower.dmg *= 10;
            TDMap.Instance.Towers.Add(tdTower);
            foreach (var farm in TDMap.Instance.Farms)
            {
                farm.Explode();
            }
        }
    }

    void Start()
    {
    }
}
