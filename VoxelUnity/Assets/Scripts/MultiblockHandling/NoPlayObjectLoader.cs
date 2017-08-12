using System.IO;
using Assets.Scripts.MultiblockHandling;
using UnityEngine;
using Assets.Scripts.Data.Multiblock;

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

        var tra = MultiblockLoader.LoadMultiblock(TextFile.name).transform;
	    tra.parent = this.transform;
	}
}
