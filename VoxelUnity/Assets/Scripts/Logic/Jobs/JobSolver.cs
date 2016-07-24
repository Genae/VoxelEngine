using System.Linq;
using Assets.Scripts.Algorithms.Pathfinding;
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
        }

        public void Solve(Job job)
        {
            Debug.Log("job assigned");
            _currentJob = job;
        }
    }
}
