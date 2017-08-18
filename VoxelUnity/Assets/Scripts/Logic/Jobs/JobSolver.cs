using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Algorithms.Pathfinding.Utils;
using Assets.Scripts.Data;
using UnityEngine;

namespace Assets.Scripts.Logic.Jobs
{
    public class JobSolver : MonoBehaviour
    {
        public PriorityQueue<JobType> PossibleTypes = new PriorityQueue<JobType>();
        private Job _currentJob;
        private JobController _jobController;
        private WalkingController _walkingController;

        void Update()
        {
            if (_currentJob != null)
            {
                if (_currentJob.Aborted)
                {
                    _currentJob = null;
                    _jobController.AddIdleSolver(this);
                    return;
                }
                if (_walkingController.IsIdle && !_currentJob.GetPossibleWorkLocations().Any(wl => (wl - transform.position).magnitude < 0.6f))
                {
                    _walkingController.MoveToAny(_currentJob.GetPossibleWorkLocations());
                }
                else if (_walkingController.IsIdle && _currentJob.Solve(Time.deltaTime, gameObject))
                {
                    _currentJob = null;
                    _jobController.AddIdleSolver(this);
                }
            }
            else
            {
                var inventory = gameObject.GetComponent<Inventory>();
                if (inventory != null && inventory.IsEmpty())
                {
                    EmptyInventory(inventory);
                }
            }
        }

        private void EmptyInventory(Inventory inventory)
        {
            _currentJob = new EmptyInventoryJob(inventory);
        }

        void Start()
        {
            if (_jobController == null)
            {
                _jobController = GameObject.Find("World").GetComponent<JobController>();
            }
            _jobController.AddIdleSolver(this);
            if (_walkingController == null)
            {
                _walkingController = gameObject.GetComponent<WalkingController>();
            }
        }

        public PriorityQueue<JobType> GetPossibleJobs()
        {
            return PossibleTypes.Copy();
        }

        public JobSolver()
        {
            PossibleTypes.Enqueue(JobType.Mining, 1);
            PossibleTypes.Enqueue(JobType.CreateSoil, 2);
            PossibleTypes.Enqueue(JobType.PlantCrop, 3);
            PossibleTypes.Enqueue(JobType.HarvestCrop, 4);
            PossibleTypes.Enqueue(JobType.Building, 5);
        }

        public void Solve(PositionedJob job)
        {
            _currentJob = job;
        }
    }

    internal class EmptyInventoryJob : Job
    {
        private readonly Inventory _inventory;
        private readonly ItemType _item;
        private Dictionary<Vector3, Inventory> _positions;

        public EmptyInventoryJob(Inventory inventory)
        {
            _inventory = inventory;
            _item = inventory.First();
            FindInventories();
        }

        private void FindInventories()
        {
            var inventories = ItemManager.GetInventoriesFor(_item);
            if (inventories.Count == 0)
                Debug.Log(string.Format("No Inventories for {0} found", _item.Name));
            else
                _positions = inventories.ToDictionary(i => (Vector3) i.gameObject.GetComponent<Item>().GetPosition(), i => i);
        }

        protected override void SolveInternal(GameObject actor)
        {
            var closest = _positions.Keys.OrderBy(k => (k - actor.transform.position).magnitude).First();
            _inventory.TransferItemTo(_item, _positions[closest]);
        }

        public override List<Vector3> GetPossibleWorkLocations()
        {
            var positions = new List<Vector3>();
            if (_positions == null)
            {
                FindInventories();
            }
            else
            {
                foreach (var pos in _positions.Keys.ToList())
                {
                    positions.AddRange(GetStandingPositions(pos));
                }
            }
            return positions;
        }
    }
}
