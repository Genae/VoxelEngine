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
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        SphereRenderer = go.GetComponent<MeshRenderer>();
        go.transform.parent = transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one *0.5f;
    }

    public virtual void Update()
    {
        if(SphereRenderer != null)
            if(InBorders())
                SphereRenderer.material.color =  Color.green;
            else
                SphereRenderer.material.color = Color.red;
    }

    private bool InBorders()
    {
        return transform.position.x <= 129 && transform.position.x >= 0 && transform.position.z <= 129 && transform.position.z >= 0 && transform.position.y >= 0;
    }
}