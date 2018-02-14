using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EngineLayer.Voxels.Containers;
using Assets.Scripts.UI;
using UnityEngine;

public class Rune : MonoBehaviour
{
    public string Name;
    public MeshRenderer SphereRenderer;
    public bool ExtendedTracking;

    public Rune(string name)
    {
        Name = name;
    }

    public virtual void Start()
    {
        transform.parent.gameObject.AddComponent<ClickRemover>();
        if (FindObjectOfType<RuneRegistry>() == null)
        {
            new GameObject("RuneRegistry").AddComponent<RuneRegistry>();
        }
        if ((SphereRenderer = transform.GetComponentInChildren<MeshRenderer>()) == null)
        {
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            SphereRenderer = go.GetComponent<MeshRenderer>();
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one * 0.1f;
        }
    }

    public virtual void Update()
    {
        RuneRegistry.Add(this);
        if(SphereRenderer != null)
        {
            if(ExtendedTracking)
                SphereRenderer.material.color =  Color.yellow;
            else
                SphereRenderer.material.color = Color.green;
            SphereRenderer.enabled = !UITDScene.Instance.Running;
        }
    }

    private bool InBorders()
    {
        return transform.position.x <= Map.Instance.Size && transform.position.x >= 0 && transform.position.z <= Map.Instance.Size && transform.position.z >= 0 && transform.position.y >= 0;
    }

}

public class ClickRemover : MonoBehaviour
{
    private bool _clickedOnce;
    void OnMouseDown()
    {
        if (UITDScene.Instance.Running)
            return;
        if (_clickedOnce)
        {
            Destroy(gameObject);
        }
        _clickedOnce = true;
    }
}

public class RuneRegistry : MonoBehaviour
{
    public static List<Rune> Runes = new List<Rune>();
    public static List<Rune> RunesThisUpdate = new List<Rune>();
    public static void Add(Rune rune)
    {
        RunesThisUpdate.Add(rune);
    }

    public void LateUpdate()
    {
        Runes = RunesThisUpdate.ToList();
        RunesThisUpdate.Clear();
    }

    public static Dictionary<string, int> GetPlacedRuned()
    {
        var dic = new Dictionary<string, int>();
        foreach (var rune in Runes)
        {
            if (!dic.ContainsKey(rune.Name.ToLower()))
            {
                dic[rune.Name.ToLower()] = 0;
            }
            dic[rune.Name.ToLower()]++;
        }
        return dic;
    }
}