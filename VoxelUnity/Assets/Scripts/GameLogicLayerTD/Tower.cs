using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameLogicLayerTD;
using Assets.Scripts.GameLogicLayerTD.Runes;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Algiz Marker;

    public int Range = 50;
    private float currentCooldown;
    public float cooldown = 1;
    private List<ElementType> _elementList;

    // Use this for initialization
    void Start () {
		Debug.Log ("Tower meldet sich zum Dienst");
        _elementList = new List<ElementType>(); //list init
        _elementList.Add(ElementType.Air); //TODO remove, testing if dmg calc works
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
        proj.AddComponent<Projectile>().Init(tdMinion.gameObject,_elementList);
    }
}

internal class Projectile: MonoBehaviour
{
    private GameObject _tarGameObject;
    private float _dmg = 25;
    private List<ElementType> _elementList;

    public void Init(GameObject target, List<ElementType> elementList)
    {
        _tarGameObject = target;
        _elementList = elementList;
    }

    void Update()
    {
        if(_tarGameObject == null)
            Destroy(gameObject);
        this.transform.position += ((_tarGameObject.transform.position - transform.position).normalized * Time.deltaTime * 30);
        if ((_tarGameObject.transform.position - transform.position).magnitude < 1f)
        {
            _tarGameObject.GetComponent<TDMinion>().ApplyDmg(_dmg, _elementList);
            Destroy(gameObject);
        }
    }
}
