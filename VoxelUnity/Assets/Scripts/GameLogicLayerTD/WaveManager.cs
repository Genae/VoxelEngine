using System.Collections.Generic;
using NUnit.Framework.Constraints;
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
            go.transform.position = instancePath[0];
            go.AddComponent<TDMinion>().SetPath(instancePath);
            
        }
    }
}
