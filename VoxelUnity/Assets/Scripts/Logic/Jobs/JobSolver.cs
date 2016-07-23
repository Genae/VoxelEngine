using Assets.Scripts.Algorithms.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.Logic.Jobs
{
    public class JobSolver : MonoBehaviour
    {
        public PriorityQueue<JobType> PossibleTypes = new PriorityQueue<JobType>();
        private Job _currentJob;
        private JobController _jobController;

        void Update()
        {
            if (_currentJob != null)
            {
                if (_currentJob.Solve(Time.deltaTime))
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
            _currentJob = job;
        }
    }
}
