using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_test.AI_and_Behaviours
{
    //Any agent that wants to use GOAP must implement this interface. It provides info
    //To the GOAP planner so that it can decide what actions to take.
    public interface IGOAP
    {
        //Starting state of the agent and the world
        HashSet<KeyValuePair<string, object>> GetWorldState();

        //Give the planner a new goal so that it can figure out the appropriate actions for it.
        HashSet<KeyValuePair<string, object>> CreateGoalState();

        //No sequence of actions could be found for the required goal. Another goal must be found
        void PlanFailed(HashSet<KeyValuePair<string, object>> failedGoal);

        //Actions that the agent will perform if a plan is found
        void PlanFound(HashSet<KeyValuePair<string, object>> goal, Queue<GOAPAction> actions);

        //All actions complete and the goal was reached
        void ActionsFinished();

        //One of the actions caused the plan to abort. The action is returned
        void PlanAborted(GOAPAction aborter);

        //Called during update. Move the agent towards the target in order for the next action to 
        //be performed. Returns true if at target, else returns false.
        bool MoveAgent(GOAPAction nextAction);
    }
}
