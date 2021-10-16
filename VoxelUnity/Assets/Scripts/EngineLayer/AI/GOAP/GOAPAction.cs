using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EngineLayer.AI.GOAP
{
    public abstract class GOAPAction
    {

        private readonly Dictionary<string, object> _preconditions;
        private readonly Dictionary<string, object> _effects;

        private float _minRange;

        public float Cost = 1f;

        public List<Vector3> Targets = new List<Vector3>();
        
        public GOAPAction()
        {
            _preconditions = new Dictionary<string, object>();
            _effects = new Dictionary<string, object>();
        }

        public void DoReset()
        {
            _minRange = -1;
            Targets.Clear();
            Reset();
        }

        public abstract void Reset();

        public abstract bool IsDone();

        public abstract bool CheckProceduralPrecondition(GameObject agent);

        public abstract bool Perform(float deltaTime, GameObject agent);

        public bool RequiresInRange => !_minRange.Equals(-1);

        public void SetMinRange(float minRange)
        {
            _minRange = minRange;
        }

        public virtual bool IsInRange(GOAPAgent a)
        {
            return Targets.Any(t => (t-a.transform.position).magnitude <= _minRange);
        }
        
        public void AddPrecondition(string key, object value)
        {
            _preconditions.Add(key, value);
        }

        public void RemovePrecondition(string key)
        {
            _preconditions.Remove(key);
        }

        public void AddEffect(string key, object value)
        {
            _effects.Add(key, value);
        }

        public void RemoveEffect(string key)
        {
            _effects.Remove(key);
        }

        public Dictionary<string, object> Preconditions => _preconditions;

        public Dictionary<string, object> Effects => _effects;

        public abstract void HasBeenChoosen();
    }
}
