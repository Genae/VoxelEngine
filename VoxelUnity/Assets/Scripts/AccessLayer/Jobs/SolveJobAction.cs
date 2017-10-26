using Assets.Scripts.EngineLayer.AI.GOAP;
using Assets.Scripts.GameLogicLayer;
using UnityEngine;

namespace Assets.Scripts.AccessLayer.Jobs
{
    public abstract class SolveJobAction: GOAPAction
    {
        public readonly string Type;
        private readonly float _minRange;
        public bool IsJobDone;
        public PositionedJob MyJob;
        public SolveJobAction(string effect, string type, float minRange = -1)
        {
            Type = type;
            _minRange = minRange;
            AddEffect(effect, true);
            SetMinRange(minRange);
            Cost = 100;
        }

        public override void Reset()
        {
            IsJobDone = false;
            MyJob = null;
            SetMinRange(_minRange);
        }

        public override bool IsDone()
        {
            return IsJobDone;
        }

        public override bool CheckProceduralPrecondition(GameObject agent)
        {
            MyJob = JobController.Instance.AskForJob(Type);
            if (MyJob == null)
                return false;
            Targets = MyJob.GetPossibleWorkLocations();
            return true;
        }

        public override bool Perform(float deltaTime, GameObject agent)
        {
            IsJobDone = MyJob.Solve(deltaTime, agent, agent.GetComponent<JobAutoSolver>() != null ? 1000 : 1);
            return true;
        }
        
        public override void HasBeenChoosen()
        {
            JobController.Instance.AcceptJob(MyJob);
        }
    }
}
