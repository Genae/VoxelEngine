using Assets.Scripts.MultiblockHandling;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class ItemManager
    {
        public static void DropItem(Vector3 pos, string itemName)
        {
            MultiblockLoader.LoadMultiblock("Items/" + itemName, pos - Vector3.one/2);
        }
    }
}