using UnityEngine;

namespace Assets.Scripts.Data
{
    public class MaterialRegistry : MonoBehaviour
    {
        public Color[] Materials;
        public Material Default;
        public static int AtlasSize = 16;

        // Use this for initialization
        void Start ()
        {
            CreateColorAtlas();
        }

        private void CreateColorAtlas()
        {
            var tex = new Texture2D(AtlasSize, AtlasSize, TextureFormat.ARGB32, false);
            var i = 0;
            foreach (var material in Materials)
            {
                tex.SetPixel(i/AtlasSize, i%AtlasSize, material);
                i++;
            }
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Point;
            tex.Apply();
            var col = tex.GetPixel(0, 1);
            Default.mainTexture = tex;
        }

        // Update is called once per frame
        void Update () {
	
        }
    }
}
