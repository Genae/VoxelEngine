using System.Linq;
using Assets.Scripts.Algorithms.Pathfinding.Utils;
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
                if (_walkingController.IsIdle && !_currentJob.GetPossibleWorkLocations().Any(wl => (wl-transform.position).magnitude < 0.6f))
                {
                    _walkingController.MoveTo(_currentJob.GetPossibleWorkLocations().First());
                }
                if (_walkingController.IsIdle && _currentJob.Solve(Time.deltaTime))
                {
                    _currentJob = null;
                    _jobController.AddIdleSolver(this);
                }
            }
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
        }

        public void Solve(Job job)
        {
            _currentJob = job;
        }
    }
}
