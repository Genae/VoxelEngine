using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayerTD
{
    public class WaveManager : MonoBehaviour
    {
        private float currentCooldown;
        public float cooldown = 3;

        void Update()
        {
            if (TDMap.Instance.Path.Count == 0)
                return; //no path
            currentCooldown -= Time.deltaTime;
            if (currentCooldown > 0)
                return;
            currentCooldown = cooldown;
            SpawnUnit(TDMap.Instance.Path);
        }
        

        private void SpawnUnit(List<Vector3> instancePath)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.parent = transform;

            //TODO change, just testing stuff here
            var tdminion = go.AddComponent<TDMinion>();
            int rnd = (int) Random.Range(0, 4);
            var list = new List<ElementType>();
            list.Add((ElementType)rnd);
            //INIT
            tdminion.Init(instancePath, list);

        }
    }
}
