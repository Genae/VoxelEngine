using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.AI.GOAP
{
    public abstract class GOAPAction
    {

        private readonly HashSet<KeyValuePair<string, object>> _preconditions;
        private readonly HashSet<KeyValuePair<string, object>> _effects;

        private bool _inRange;

        public float Cost = 1f;

        public List<Vector3> Targets = new List<Vector3>();

        public GOAPAction()
        {
            _preconditions = new HashSet<KeyValuePair<string, object>>();
            _effects = new HashSet<KeyValuePair<string, object>>();
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
            _preconditions.Add(new KeyValuePair<string, object>(key, value));
        }

        public void RemovePrecondition(string key)
        {
            var remove = default(KeyValuePair<string, object>);
            foreach (var kvp in _preconditions)
            {
                if (kvp.Key.Equals(key))
                {
                    remove = kvp;
                }
                if (!default(KeyValuePair<string, object>).Equals(remove))
                {
                    _preconditions.Remove(remove);
                }
            }
        }

        public void AddEffect(string key, object value)
        {
            _effects.Add(new KeyValuePair<string, object>(key, value));
        }

        public void RemoveEffect(string key)
        {
            var remove = default(KeyValuePair<string, object>);
            foreach (var kvp in _effects)
            {
                if (kvp.Key.Equals(key))
                {
                    remove = kvp;
                }
                if (!default(KeyValuePair<string, object>).Equals(remove))
                {
                    _effects.Remove(remove);
                }
            }
        }

        public HashSet<KeyValuePair<string, object>> Preconditions
        {
            get
            {
                return _preconditions;
            }
        }

        public HashSet<KeyValuePair<string, object>> Effects
        {
            get
            {
                return _effects;
            }
        }

        public abstract void HasBeenChoosen();
    }
}
