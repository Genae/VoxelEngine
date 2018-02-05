using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameLogicLayerTD;
using Assets.Scripts.GameLogicLayerTD.Runes;
using Assets.Scripts.UI;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Algiz Marker;

    public int Range = 50;
    public float currentCooldown;
    public float cooldown = 1;
    public float atkspeed = 1;
    public float moneyOnHit;
    public float splashRadius;
    public float slowIntesity = 1;
    public int moneyOnKill = 1;
    public float dmg = 25;
    private List<ElementType> _elementList;

    public TDMinion BeamTarget;
    public float BeamMultiplier;
    private LineRenderer _lr;

    public float BoostTime = 0;
    public float BoostSpeedup = 1;

    public GameObject Rotator;

    // Use this for initialization
    void Start ()
    {
        Rotator = new GameObject();
        Rotator.transform.parent = transform.GetChild(0);
        Rotator.transform.localPosition = new Vector3(0, 50, 0);
        _lr = gameObject.AddComponent<LineRenderer>();
        var mat = new Material(Shader.Find("Standard"));
        mat.color = Color.magenta;
        _lr.material = mat;

        Debug.Log ("Tower meldet sich zum Dienst");
        _elementList = GetElements();
        if(_elementList.Count > 0)
            Debug.Log("Elements active: " + _elementList.Select(e => e.ToString()).Aggregate((workingSentence, next) => next + ", " + workingSentence));

        var upgradeRunes = Marker.GetUpgradeRunes();
        var ehwaz = upgradeRunes.OfType<Ehwaz>().ToList();
        if (ehwaz.Any())
        {
            atkspeed = Mathf.Pow(1.5f, ehwaz.Count);
        }

        var gebo = upgradeRunes.OfType<Gebo>().ToList();
        if (gebo.Any())
        {
            moneyOnHit = gebo.Count;
        }

        var hagalaz = upgradeRunes.OfType<Hagalaz>().ToList();
        if (hagalaz.Any())
        {
            splashRadius = hagalaz.Count * 7;
        }

        var isa = upgradeRunes.OfType<Isa>().ToList();
        if (isa.Any())
        {
            slowIntesity = Mathf.Pow(0.7f, isa.Count);
        }

        var othala = upgradeRunes.OfType<Othala>().ToList();
        if (othala.Any())
        {
            moneyOnKill = (int)Mathf.Pow(2, othala.Count);
        }

        var i = 0;
        foreach (var upgradeRune in upgradeRunes)
        {
            var p = GameObject.CreatePrimitive(PrimitiveType.Plane);
            
            p.transform.localScale = Vector3.one * 0.5f;
            p.GetComponent<MeshRenderer>().material = upgradeRune.transform.parent.GetComponent<MeshRenderer>().material;
            p.transform.parent = Rotator.transform;
            var q = Instantiate(p);
            q.transform.parent = p.transform;
            q.transform.localPosition = Vector3.zero;
            q.transform.Rotate(new Vector3(0,0,1), 180);
            p.transform.localPosition = new Vector3(10, 0, 0);
            p.transform.localRotation = Quaternion.Euler(90, 0, -90);
            p.transform.RotateAround(Rotator.transform.position, Vector3.up, i++*360f/upgradeRunes.Count);
        }
    }

    public void Init(Algiz marker)
    {
        Marker = marker;
    }

    void OnMouseDown()
    {
        var naudhiz = Marker.GetUpgradeRunes().OfType<Naudhiz>().ToList();
        if (naudhiz.Any())
        {
            ResourceOverview.Instance.Gold.Value -= 50 * naudhiz.Count;
            BoostTime = 10;
            BoostSpeedup = Mathf.Pow(3f, naudhiz.Count);
            currentCooldown = 0;
        }
    }

    private List<ElementType> GetElements()
    {
        var list = new List<ElementType>();
        var elements = Marker.GetUpgradeRunes().OfType<ElementRune>().Select(er => er.ElementType).ToList();
        if (elements.Count <= 1)
            return elements;
        var count = new Dictionary<ElementType, int>();
        foreach (var elementType in elements)
        {
            if (!count.ContainsKey(elementType))
                count[elementType] = 0;
            count[elementType]++;
        }
        count = count.OrderByDescending(k => k.Value).ToDictionary(k => k.Key, k => k.Value);
        if (!Marker.GetUpgradeRunes().OfType<Ihwaz>().Any())
        {
            var mainElement = count.First();
            if (Marker.GetUpgradeRunes().OfType<Wunjo>().Any())
            {
                for (var i = 0; i < mainElement.Value; i++)
                {
                    list.Add(mainElement.Key);  //multiple of one
                }
            }
            else
            {
                list.Add(mainElement.Key);      //only one rune
            }
        }
        else
        {
            if (Marker.GetUpgradeRunes().OfType<Wunjo>().Any())
            {
                return elements;                //all runes
            }
            else
            {
                foreach (var elem in count)
                {
                    list.Add(elem.Key);         //only one element per type
                }
            }
        }
        return list;
    }

    // Update is called once per frame
	void Update ()
	{
	    if (BeamTarget != null)
	    {
            _lr.positionCount = 2;
	        _lr.SetPosition(0, transform.position + Vector3.up * 15);
	        _lr.SetPosition(1, BeamTarget.transform.position);
	        _lr.startWidth = 1;
	        _lr.endWidth = 1;
        }
	    _lr.enabled = BeamTarget != null;

        if(Rotator != null)
            Rotator.transform.RotateAround(Rotator.transform.position, Vector3.up, 75*Time.deltaTime);


	    var minionsInRange = WaveManager.AliveMinions.Where(minion => (minion.transform.position - transform.position).magnitude < Range).ToList();
	    foreach (var minion in minionsInRange)
	    {
	        Debug.DrawLine(transform.position + Vector3.up * 25, minion.transform.position);
        }
	    BoostTime -= Time.deltaTime;
	    if (BoostTime <= 0)
	    {
	        BoostSpeedup = 1;
	    }
	    currentCooldown -= Time.deltaTime * atkspeed;
        if (currentCooldown > 0 || minionsInRange.Count == 0)
	        return;
	    currentCooldown = cooldown / BoostSpeedup;
	    AttackUnits(minionsInRange);
    }

    private void AttackUnits(List<TDMinion> minionsInRange)
    {
        var thurisaz = Marker.GetUpgradeRunes().OfType<Thurisaz>().ToList();
        var sowilo = Marker.GetUpgradeRunes().OfType<Sowilo>().ToList();
        if (thurisaz.Any()) //ground attack
        {
            var rangeMultiplier = Mathf.Pow(1.5f, thurisaz.Count);
            minionsInRange = minionsInRange.Where(minion => (minion.transform.position - transform.position).magnitude < Range * rangeMultiplier * 0.2f).ToList();
            foreach (var minion in minionsInRange)
            {
                HitMinion(minion);
            }
            currentCooldown *= 2;
            var proj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            proj.name = "AoE";
            proj.transform.position = transform.position + Vector3.up*3;
            proj.transform.localScale = new Vector3(Range * rangeMultiplier * 0.2f * 2, 0.1f, Range * rangeMultiplier * 0.2f * 2);
            proj.AddComponent<AoE>();
        }
        else if (sowilo.Any())
        {
            if (BeamTarget == null)
            {
                BeamTarget = minionsInRange.OrderByDescending(m => m.DistanceMoved).First();
                BeamMultiplier = 0.5f;
            }
            HitMinion(BeamTarget, BeamMultiplier);
            BeamMultiplier *= Mathf.Pow(1.1f, sowilo.Count);
            currentCooldown *= 0.2f;
        }
        else // projectile
        {
            var tdMinion = minionsInRange.OrderByDescending(m => m.DistanceMoved).First();
            var proj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            proj.name = "Projectile";
            proj.transform.position = transform.position + Vector3.up * 10; //projectile spawn height
            proj.transform.localScale = Vector3.one * 0.7f;
            proj.AddComponent<Projectile>().Init(tdMinion.gameObject, this, splashRadius, atkspeed);
        }
    }


    public void HitMinion(TDMinion unit, float dmgMultiplier = 1f)
    {
        if (unit.ApplyDmg(dmg * dmgMultiplier, _elementList))
        {
            ResourceOverview.Instance.Gold.Value += 5 * moneyOnKill;
        }
        unit.ApplySlow(slowIntesity);
        ResourceOverview.Instance.Gold.Value += (int)moneyOnHit;

    }
}

internal class AoE : MonoBehaviour
{
    private float cooldown = 1f;
    MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));
        SetToRenderTransparent(meshRenderer.material);
    }

    private void SetToRenderTransparent(Material standardShaderMaterial)
    {
        standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        standardShaderMaterial.SetInt("_ZWrite", 0);
        standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
        standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
        standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        standardShaderMaterial.renderQueue = 3000;
    }

    void Update()
    {
        cooldown -= Time.deltaTime;
        if(cooldown <= 0)
            Destroy(gameObject);
        var myColor = meshRenderer.material.color;
        myColor.a = cooldown;
        meshRenderer.material.color = myColor;
    }
}

internal class Projectile: MonoBehaviour
{
    private float _splashDamageRadius;
    private float _atkspeed;
    private GameObject _tarGameObject;
    private Tower _tower;

    public void Init(GameObject target, Tower tower, float splashDamageRadius, float atkspeed)
    {
        _tarGameObject = target;
        _splashDamageRadius = splashDamageRadius;
        _atkspeed = atkspeed;
        _tower = tower;
    }

    void Update()
    {
        if (_tarGameObject == null)
        {
            Destroy(gameObject);
            return;
        }
        transform.position += ((_tarGameObject.transform.position - transform.position).normalized * Time.deltaTime * 30 * _atkspeed);
        if ((_tarGameObject.transform.position - transform.position).magnitude < 1f)
        {
            //splash
            if (_splashDamageRadius <= 0)
            {
                _tower.HitMinion(_tarGameObject.GetComponent<TDMinion>());
            }
            else
            {
                var splashUnits = WaveManager.AliveMinions.Where(minion => (minion.transform.position - _tarGameObject.transform.position).magnitude < _splashDamageRadius).ToList();
                foreach (var unit in splashUnits)
                {
                    _tower.HitMinion(unit);
                }
            }
            Destroy(gameObject);
        }
    }

}
