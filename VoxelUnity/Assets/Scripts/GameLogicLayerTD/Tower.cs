using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameLogicLayerTD;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Algiz Marker;

    public int Range = 30;
    private float currentCooldown;
    public float cooldown = 1;

    // Use this for initialization
    void Start () {
		Debug.Log ("Tower meldet sich zum Dienst");
	}

    public void Init(Algiz marker)
    {
        Marker = marker;
    }

	// Update is called once per frame
	void Update ()
	{
        if (Marker.GetUpgradeRunes().Count > 0)
            transform.GetComponentInChildren<MeshRenderer>().material.color = Color.blue;

	    var minionsInRange = FindObjectsOfType<TDMinion>().Where(minion => (minion.transform.position - transform.position).magnitude < Range).ToList();
	    foreach (var minion in minionsInRange)
	    {
	        Debug.DrawLine(transform.position + Vector3.up * 25, minion.transform.position);
        }
	    currentCooldown -= Time.deltaTime;
	    if (currentCooldown > 0 || minionsInRange.Count == 0)
	        return;
	    currentCooldown = cooldown;
	    AttackUnit(minionsInRange[0]);
    }

    private void AttackUnit(TDMinion tdMinion)
    {
        var proj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        proj.name = "Prjoctile";
        proj.transform.position = transform.position + Vector3.up * 25;
        proj.transform.localScale = Vector3.one * 0.7f;
        proj.AddComponent<Projectile>().TarGameObject = tdMinion.gameObject;
    }
}

internal class Projectile: MonoBehaviour
{
    public GameObject TarGameObject;

    void Update()
    {
        if(TarGameObject == null)
            Destroy(gameObject);
        this.transform.position += ((TarGameObject.transform.position - transform.position).normalized * Time.deltaTime * 30);
        if ((TarGameObject.transform.position - transform.position).magnitude < 1f)
        {
            Destroy(TarGameObject.gameObject);
            Destroy(gameObject);
        }
    }
}
