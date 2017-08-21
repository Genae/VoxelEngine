using Assets.Scripts.AI.GOAP;
using Assets.Scripts.Logic.Jobs;
using UnityEngine;

namespace Assets.Scripts.Logic.Actions
{
    public class SolveJobAction: GOAPAction
    {
        private readonly JobType _type;
        public bool IsJobDone;
        public PositionedJob MyJob;
        public SolveJobAction(string effect, JobType type)
        {
            _type = type;
            AddEffect(effect, true);
            Cost = 100;
        }

        public override void Reset()
        {
            IsJobDone = false;
            MyJob = null;
        }

        public override bool IsDone()
        {
            return IsJobDone;
        }

        public override bool CheckProceduralPrecondition(GameObject agent)
        {
            MyJob = JobController.Instance.AskForJob(_type);
            if (MyJob == null)
                return false;
            Targets = MyJob.GetPossibleWorkLocations();
            return true;
        }

        public override bool Perform(float deltaTime, GameObject agent)
        {
            IsJobDone = MyJob.Solve(deltaTime, agent);
            return true;
        }

        public override bool RequiresInRange()
        {
            return true;
        }

        public override void HasBeenChoosen()
        {
            JobController.Instance.AcceptJob(MyJob);
        }
    }
}
