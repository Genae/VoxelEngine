using Assets.Scripts.Base.Data.Map;
using UnityEngine;

namespace Assets.Scripts.Client.GameData
{
    public class Map
    {
        public Shader Shader;
        public MapData MapData;

        protected Map(MapData data)
        {
            MapData = data;
        }
    }
}
