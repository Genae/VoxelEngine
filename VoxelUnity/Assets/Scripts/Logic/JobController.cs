using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Algorithms.Pathfinding;
using Assets.Scripts.Logic.Jobs;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class JobController : MonoBehaviour
    {
        protected readonly Dictionary<JobType, PriorityQueue<Job>> OpenJobs = new Dictionary<JobType, PriorityQueue<Job>>();
        protected readonly List<JobSolver> FreeSolvers = new List<JobSolver>();

        void Update()
        {
            foreach (var freeSolver in FreeSolvers.ToArray())
            {
                var possibleJobs = freeSolver.GetPossibleJobs();
                var job = possibleJobs.Dequeue();
                if (OpenJobs.ContainsKey(job) && !OpenJobs[job].IsEmpty())
                {
                    freeSolver.Solve(OpenJobs[job].Dequeue());
                    FreeSolvers.Remove(freeSolver);
                }
            }
        }

        public void AddJob(Job job)
        {
            if (!OpenJobs.ContainsKey(job.GetJobType()))
            {
                OpenJobs.Add(job.GetJobType(), new PriorityQueue<Job>());
            }
            OpenJobs[job.GetJobType()].Enqueue(job, 1);
        }

        public bool HasJob(Vector3 pos, JobType jobType)
        {
            return OpenJobs.ContainsKey(jobType) && OpenJobs[jobType].Any(j => j.GetJobType().Equals(jobType) && j.transform.position.Equals(pos));
        }

        public void AddIdleSolver(JobSolver solver)
        {
            FreeSolvers.Add(solver);
        }
    }
}
