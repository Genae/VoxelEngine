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

    //splitting curve into parts to make resulting line closer to controlpolygon. pretty sure this is not exactly bsplines, close enough for me tho
    public static List<Vector3> GetBSplinePoints(List<Vector3> verts, float stepcount)
    {
        //no splitting possible if less then 3 points
        if(verts.Count <= 3) return GetBezierPoints(verts, stepcount);

        var list = new List<Vector3>();
        //first 2 points
        list.Add(verts[0]);
        list.Add(GetMidPointAt(verts[0], verts[1], 0.5f));
        var dynlist = new List<Vector3>();

        for (int i = 1; i < verts.Count-1; i++)
        {
            //get 3 points ready for bezier
            dynlist.Clear();
            dynlist.Add(GetMidPointAt(verts[i-1], verts[i], 0.5f));
            dynlist.Add(verts[i]);
            dynlist.Add(GetMidPointAt(verts[i], verts[i + 1], 0.5f));
            //use bezier
            list.AddRange(GetBezierPoints(dynlist, stepcount));
        }

        //last 2 points
        list.Add(GetMidPointAt(verts[verts.Count-2], verts[verts.Count - 1], 0.5f));
        list.Add(verts[verts.Count - 1]);

        Debug.Log(list.Count);

        return list;
    }

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
