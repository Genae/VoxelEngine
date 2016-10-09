using UnityEngine;
using System;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Assets.Scripts.Data.VoxelEntity;
using Assets.Scripts.Importer;

public class ImportObject : EditorWindow
{
    public const float Scale = 1f;
    [MenuItem("Importer/Save File")]
    static void LoadFile()
    {
        if (Importer.Imported == null)
        {
            Debug.Log("Nothing imported.");
            return;
        }

        

        var path = EditorUtility.SaveFilePanelInProject("savedVox", "", "vox", "SelectLocation", "");
        Debug.Log(path);

        
        File.WriteAllText(path, "");
        foreach(var thing in Importer.Imported)
        {
            File.AppendAllText(path, JsonUtility.ToJson(thing) + "\n");
        }
    }
}