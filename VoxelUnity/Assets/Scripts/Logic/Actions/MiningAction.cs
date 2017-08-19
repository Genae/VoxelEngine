using Assets.Scripts.AI.GOAP;
using Assets.Scripts.Logic.Jobs;
using UnityEngine;

namespace Assets.Scripts.Logic.Actions
{
    public class MiningAction: GOAPAction
    {
        public bool IsJobDone;
        public PositionedJob MyJob;
        public MiningAction()
        {
            AddEffect("hasMined", true);
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
            MyJob = Object.FindObjectOfType<JobController>().AskForJob(JobType.Mining);
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
            Object.FindObjectOfType<JobController>().AcceptJob(MyJob);
        }
    }
}
