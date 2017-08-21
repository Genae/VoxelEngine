using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.AI.GOAP
{
    public abstract class GOAPAction
    {

        private readonly Dictionary<string, object> _preconditions;
        private readonly Dictionary<string, object> _effects;

        private bool _inRange;

        public float Cost = 1f;

        public List<Vector3> Targets = new List<Vector3>();

        public GOAPAction()
        {
            _preconditions = new Dictionary<string, object>();
            _effects = new Dictionary<string, object>();
        }

        public void DoReset()
        {
            _inRange = false;
            Targets.Clear();
            Reset();
        }

        public abstract void Reset();

        public abstract bool IsDone();

        public abstract bool CheckProceduralPrecondition(GameObject agent);

        public abstract bool Perform(float deltaTime, GameObject agent);

        public abstract bool RequiresInRange();

        public bool IsInRange()
        {
            return _inRange;
        }

        public void SetInRange(bool val)
        {
            _inRange = val;
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

        public Dictionary<string, object> Preconditions
        {
            get
            {
                return _preconditions;
            }
        }

        public Dictionary<string, object> Effects
        {
            get
            {
                return _effects;
            }
        }

        public abstract void HasBeenChoosen();
    }
}
