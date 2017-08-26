using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.MultiblockHandling.Editor
{
    [ExecuteInEditMode]

    public class NoPlayObjectLoader : MonoBehaviour
    {

        public TextAsset TextFile;
        public string oldFileName;
	

        void Update ()
        {
            if (TextFile == null || oldFileName == TextFile.name)
                return;
            oldFileName = TextFile.name;

            var path = AssetDatabase.GetAssetPath(TextFile);
            if (path.Contains("Imported"))
                path = path.Substring(path.IndexOf("Imported", StringComparison.InvariantCulture) + "Imported/".Length);
            if (path.Contains(".txt"))
                path = path.Replace(".txt", "");
            var tra = MultiblockLoader.LoadMultiblock(path, Vector3.zero, transform, 1).transform;
            MultiblockLoader.CleanupCache();

        }
    }
}
