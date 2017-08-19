using System.Collections.Generic;

namespace Assets.Scripts.AI.GOAP
{
    public interface IGOAP
    {

        HashSet<KeyValuePair<string, object>> GetWorldState();

        HashSet<KeyValuePair<string, object>> CreateGoalState();

        void PlanFailed(HashSet<KeyValuePair<string, object>> failedGoal);

        void PlanFound(HashSet<KeyValuePair<string, object>> goal, Queue<GOAPAction> actions);

        void ActionsFinished();

        void PlanAborted(GOAPAction aborter);

        bool MoveAgent(GOAPAction nextAction);
    }
}
