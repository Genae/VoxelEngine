using UnityEngine;

namespace Assets.Scripts.Logic.Jobs
{
    public abstract class Job : MonoBehaviour
    {
        public abstract JobType GetJobType();

        public abstract bool Solve(float deltaTime);
    }

    public enum JobType
    {
        Mining
    }
}
