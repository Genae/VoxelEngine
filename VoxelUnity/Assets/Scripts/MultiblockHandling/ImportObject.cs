using UnityEditor;

public class ImportObject : EditorWindow
{
    /*public const float Scale = 1f;
    [MenuItem("Importer/Save File")]
    static void SaveFile()
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

    [MenuItem("Importer/Load File")]
    static void LoadFile()
    {
        Debug.Log("Loading File");

        var path = EditorUtility.OpenFilePanel("", "", "vox");


        var objLineStrings = File.ReadAllLines(path);

        List<VData> loadedObjList = new List<VData>();

        foreach (var line in objLineStrings)
        {
            loadedObjList.Add(JsonUtility.FromJson<VData>(line));
        }

        Importer.CreateMultiblock(loadedObjList);
    }*/
}