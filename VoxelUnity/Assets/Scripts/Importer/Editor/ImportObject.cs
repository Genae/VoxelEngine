using UnityEngine;
using System;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.VoxelEntity;

public class ImportObject : EditorWindow {

    [MenuItem("Importer/loadFile")]
    static void LoadFile()
    {

        var path = EditorUtility.OpenFilePanelWithFilters("SelectObjectFile", "", new[] {"Wavefront Object", "obj" });
        Debug.Log(path);

        var lines = File.ReadAllLines(path);
        var vertices = new List<Vector3>();
        var texCoords = new List<Vector2>();
        var normals = new List<Vector3>();
        var faces = new List<Face>();

        foreach(var line in lines)
        {
            if(line.StartsWith("v "))
            {
                var split = line.Split(' ');
                var vertice = new Vector3((float)Convert.ToDouble(split[1])+0.1f, (float)Convert.ToDouble(split[2]) + 0.1f, (float)Convert.ToDouble(split[3]) + 0.1f);
                vertices.Add(vertice);
                Debug.Log(vertice);
            }
            if (line.StartsWith("vt "))
            {
                var split = line.Split(' ');
                var texCoord = new Vector2((float)Convert.ToDouble(split[1]), (float)Convert.ToDouble(split[2]));
                texCoords.Add(texCoord);
                Debug.Log(texCoord);
            }
            if (line.StartsWith("vn "))
            {
                var split = line.Split(' ');
                var normal = new Vector3((int)Convert.ToDouble(split[1]), (int)Convert.ToDouble(split[2]), (int)Convert.ToDouble(split[3]));
                normals.Add(normal);
                Debug.Log(normal);
            }
            if (line.StartsWith("f "))
            {
                var split = line.Split(' ');
                var vert1 = split[1].Split('/');
                var vert2 = split[2].Split('/');
                var vert3 = split[3].Split('/');
                var color = GetColor(Convert.ToInt32(vert1[1]), texCoords);
                var face = new Face(color, vertices[Convert.ToInt32(vert1[0])-1], vertices[Convert.ToInt32(vert2[0]) - 1], vertices[Convert.ToInt32(vert3[0]) - 1], vertices[Convert.ToInt32(vert1[2]) - 1]);
                

                faces.Add(face);
                Debug.Log(face);
            }
        }
        CreateEntity(faces);
    }

    private static void CreateEntity(List<Face> faces)
    {
        var voxels = new Dictionary<Color, List<Vector3>>();

        foreach(var face in faces)
        {
            if (!voxels.ContainsKey(face.Color))
            {
                voxels[face.Color] = new List<Vector3>();
            }
            var pos = face.GetVoxelCenter();
            if (!voxels[face.Color].Contains(pos))
            {
                voxels[face.Color].Add(pos);
            }

        }
        VoxelEntity.InstantiateVoxels(Vector3.zero, voxels);
    }

    private static Color GetColor(int index, List<Vector2> texCoords)
    {
        return Color.black;
    }
}

public class Face
{
    public Color Color;
    public Vector3 Vert1;
    public Vector3 Vert2;
    public Vector3 Vert3;
    public Vector3 Norm;

    public Face(Color color, Vector3 vert1, Vector3 vert2, Vector3 vert3, Vector3 norm)
    {
        Color = color;
        Vert1 = vert1;
        Vert2 = vert2;
        Vert3 = vert3;
        Norm = norm;
    }

    public Vector3 GetVoxelCenter()
    {
        var length1 = (Vert1 - Vert2).magnitude;
        var length2 = (Vert1 - Vert3).magnitude;
        var length3 = (Vert2 - Vert3).magnitude;
        var voxSize = Mathf.Min(length1, length2, length3);
        var sideC = Mathf.Max(length1, length2, length3);

        if(length1 == sideC)
        {
            return Vert1 + (Vert2 - Vert1) / 2 + Norm * (voxSize / 2);
        }
        else if (length2 == sideC)
        {
            return Vert1 + (Vert3 - Vert1) / 2 + Norm * (voxSize / 2);
        }
        else
        {
            return Vert2 + (Vert3 - Vert2) / 2 + Norm * (voxSize / 2);
        }
    }

}
