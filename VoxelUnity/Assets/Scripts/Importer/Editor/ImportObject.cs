using UnityEngine;
using System;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Assets.Scripts.Data.VoxelEntity;

public class ImportObject : EditorWindow
{
    public const float Scale = 1f;
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
                var vertice = new Vector3((float)Convert.ToDouble(split[1]), (float)Convert.ToDouble(split[2]), (float)Convert.ToDouble(split[3]));
                vertices.Add(vertice);
            }
            if (line.StartsWith("vt "))
            {
                var split = line.Split(' ');
                var texCoord = new Vector2((float)Convert.ToDouble(split[1]), (float)Convert.ToDouble(split[2]));
                texCoords.Add(texCoord);
            }
            if (line.StartsWith("vn "))
            {
                var split = line.Split(' ');
                var normal = new Vector3((int)Convert.ToDouble(split[1]), (int)Convert.ToDouble(split[2]), (int)Convert.ToDouble(split[3]));
                normals.Add(normal);
            }
            if (line.StartsWith("f "))
            {
                var split = line.Split(' ');
                var vert1 = split[1].Split('/');
                var vert2 = split[2].Split('/');
                var vert3 = split[3].Split('/');
                var color = GetColor(Convert.ToInt32(vert1[1]), texCoords);
                var face = new Face(color, vertices[Convert.ToInt32(vert1[0])-1], vertices[Convert.ToInt32(vert2[0]) - 1], vertices[Convert.ToInt32(vert3[0]) - 1], normals[Convert.ToInt32(vert1[2]) - 1]);
                

                faces.Add(face);
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
            foreach (var vector3 in pos)
            {
                if (!voxels[face.Color].Contains(vector3))
                {
                    voxels[face.Color].Add(vector3);
                }
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

    public List<Vector3> GetVoxelCenter()
    {
        var vectors = new List<Vector3>();
        var minX = Mathf.Min(Vert1.x, Vert2.x, Vert3.x);
        var minY = Mathf.Min(Vert1.y, Vert2.y, Vert3.y);
        var minZ = Mathf.Min(Vert1.z, Vert2.z, Vert3.z);
        var maxX = Mathf.Max(Vert1.x, Vert2.x, Vert3.x);
        var maxY = Mathf.Max(Vert1.y, Vert2.y, Vert3.y);
        var maxZ = Mathf.Max(Vert1.z, Vert2.z, Vert3.z);

        if (Math.Abs(minX - maxX) < 0.1f*ImportObject.Scale)
        {
            for (var z = minZ; z < maxZ; z++)
            {
                for (var y = minY; y < maxY; y++)
                {
                    vectors.Add(new Vector3(minX + Norm.x * ImportObject.Scale + ImportObject.Scale / -2f, y + ImportObject.Scale / 2f, z + ImportObject.Scale / 2f));
                }
            }
        }
        else if (Math.Abs(minY - maxY) < 0.1f * ImportObject.Scale)
        {
            for (var x = minX; x < maxX; x++)
            {
                for (var z = minZ; z < maxZ; z++)
                {
                    vectors.Add(new Vector3(x + ImportObject.Scale / 2f, minY + Norm.y * ImportObject.Scale + ImportObject.Scale / 2f, z + ImportObject.Scale / 2f));
                }
            }
        }
        else
        {
            for (var x = minX; x < maxX; x++)
            {
                for (var y = minY; y < maxY; y++)
                {
                    vectors.Add(new Vector3(x + ImportObject.Scale / 2f, y + ImportObject.Scale / 2f, minZ + Norm.z * ImportObject.Scale + ImportObject.Scale / -2f));
                }
            }
        }
        return vectors;
    }
}
