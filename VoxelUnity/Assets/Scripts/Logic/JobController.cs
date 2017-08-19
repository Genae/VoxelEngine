using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Algorithms.Pathfinding.Utils;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Logic.Jobs;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class JobController : MonoBehaviour
    {
        protected readonly Dictionary<JobType, PriorityQueue<PositionedJob>> OpenJobs = new Dictionary<JobType, PriorityQueue<PositionedJob>>();
        protected readonly List<JobSolver> FreeSolvers = new List<JobSolver>();
        protected List<PositionedJob>[,,] Jobs;
        
        void Update()
        {
            foreach (var freeSolver in FreeSolvers.ToArray())
            {
                FindJobFor(freeSolver);
            }
        }

        private void FindJobFor(JobSolver freeSolver)
        {
            var possibleJobs = freeSolver.GetPossibleJobs();
            while (!possibleJobs.IsEmpty())
            {
                var jobType = possibleJobs.Dequeue();
                if (OpenJobs.ContainsKey(jobType) && !OpenJobs[jobType].IsEmpty())
                {
                    foreach (var job in OpenJobs[jobType])
                    {
                        if (!job.GetPossibleWorkLocations().Any())
                        {
                            continue;
                        }

                        freeSolver.Solve(OpenJobs[jobType].Dequeue(job));
                        FreeSolvers.Remove(freeSolver);
                        return;
                    }
                }
            }
        }

        public PositionedJob AskForJob(JobType jobType)
        {
            if (OpenJobs.ContainsKey(jobType) && !OpenJobs[jobType].IsEmpty())
            {
                foreach (var job in OpenJobs[jobType])
                {
                    if (!job.GetPossibleWorkLocations().Any())
                    {
                        continue;
                    }
                    return job;
                }
            }
            return null;
        }

        public void AcceptJob(PositionedJob job)
        {
            OpenJobs[job.GetJobType()].Dequeue(job);
        }

        public void AddJob(PositionedJob job)
        {
            if (Jobs == null)
            {
                if (!Map.Instance.IsDoneGenerating)
                    return;
                Jobs = new List<PositionedJob>[Map.Instance.MapData.Chunks.GetLength(0)*Chunk.ChunkSize, 
                                            Map.Instance.MapData.Chunks.GetLength(1) * Chunk.ChunkSize, 
                                            Map.Instance.MapData.Chunks.GetLength(2) * Chunk.ChunkSize];
            }
            if (!OpenJobs.ContainsKey(job.GetJobType()))
            {
                OpenJobs.Add(job.GetJobType(), new PriorityQueue<PositionedJob>());
            }
            OpenJobs[job.GetJobType()].Enqueue(job, 1);
            if (Jobs[(int) job.Position.x, (int) job.Position.y, (int) job.Position.z] == null)
            {
                Jobs[(int)job.Position.x, (int)job.Position.y, (int)job.Position.z] = new List<PositionedJob>();
            }
            Jobs[(int)job.Position.x, (int)job.Position.y, (int)job.Position.z].Add(job);
        }

        public void SolveJob(PositionedJob job)
        {
            if(Jobs[(int)job.Position.x, (int)job.Position.y, (int)job.Position.z] != null)
                Jobs[(int) job.Position.x, (int) job.Position.y, (int) job.Position.z].Remove(job);
        }

        public bool HasJob(Vector3 pos, JobType jobType)
        {
            if (Jobs == null || Jobs[(int) pos.x, (int) pos.y, (int) pos.z] == null)
                return false;
            return Jobs[(int) pos.x, (int) pos.y, (int) pos.z].Any(j => j.GetJobType().Equals(jobType));
        }

        public void AddIdleSolver(JobSolver solver)
        {
            FreeSolvers.Add(solver);
        }

        public List<PositionedJob> GetJobAt(Vector3 pos)
        {
            return Jobs[(int) pos.x, (int) pos.y, (int) pos.z];
        }
    }
}
