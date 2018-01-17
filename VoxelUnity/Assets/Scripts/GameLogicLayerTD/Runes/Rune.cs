using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EngineLayer.Voxels.Containers;
using UnityEngine;

public class Rune : MonoBehaviour
{
    public string Name;
    public MeshRenderer SphereRenderer;

    public Rune(string name)
    {
        Name = name;
    }

    public virtual void Start()
    {
        if (FindObjectOfType<RuneRegistry>() == null)
        {
            new GameObject("RuneRegistry").AddComponent<RuneRegistry>();
        }
        if ((SphereRenderer = transform.GetComponentInChildren<MeshRenderer>()) == null)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            SphereRenderer = go.GetComponent<MeshRenderer>();
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one * 0.5f;
        }
    }

    public virtual void Update()
    {
        RuneRegistry.Add(this);
        if(SphereRenderer != null)
            if(InBorders())
                SphereRenderer.material.color =  Color.green;
            else
                SphereRenderer.material.color = Color.red;
    }

    private bool InBorders()
    {
        return transform.position.x <= Map.Instance.Size && transform.position.x >= 0 && transform.position.z <= Map.Instance.Size && transform.position.z >= 0 && transform.position.y >= 0;
    }
}

public class RuneRegistry : MonoBehaviour
{
    public static List<Rune> Runes;
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
}