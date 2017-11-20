using UnityEngine;

public class Rune : MonoBehaviour
{
    public string Name;

    public Rune(string name)
    {
        Name = name;
    }

    public virtual void Start()
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.GetComponent<MeshRenderer>().material.color = InBorders() ? Color.green : Color.red;
        go.transform.parent = transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one * 3;
    }

    private bool InBorders()
    {
        return transform.position.x < 129 && transform.position.x > 0 && transform.position.z < 129 &&
               transform.position.z > 0 && transform.position.y > 0;
    }
}