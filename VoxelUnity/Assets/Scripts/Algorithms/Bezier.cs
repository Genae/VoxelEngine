using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bezier : MonoBehaviour {

    /*void Start()
    {
        var list = new List<Vector3>();
        list.Add(new Vector3(0,0,0));
        list.Add(new Vector3(1,1,1));
        list = GetBezierPoints(list, 10);
        foreach (var p in list)
        {
            Debug.Log(p);
        }
    }*/

    public static List<Vector3> GetBezierPoints(List<Vector3> verts, float stepcount)
    {
        var vertcount = verts.Count;
        float stepint = 1 / stepcount;
        var list = new List<Vector3>();

        for (float i = 0; i < 1; i += stepint)
        {
            list.Add(GetBezierAtValue(verts, i));
        }
        return list;
    }

    private static Vector3 GetBezierAtValue(List<Vector3> verts, float value)
    {
        var vertCount = verts.Count;

        if (vertCount == 1)
        {
            return verts[0];
        }
        else
        {
            var list = new List<Vector3>();
            for (int i = 0; i < vertCount - 1; i++)
            {
                list.Add(GetMidPointAt(verts[i], verts[i+1], value));
            }
            return GetBezierAtValue(list, value);
        }
    }

    private static Vector3 GetMidPointAt(Vector3 p1, Vector3 p2, float value)
    {
        return (1-value) * p1 + value * p2;
    }
}
