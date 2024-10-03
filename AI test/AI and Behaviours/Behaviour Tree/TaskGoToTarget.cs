using AI_test.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using AI_test.ToolsIIStorageIIPrefabs;

namespace AI_test.AI_and_Behaviours
{
    //Makes the agent move towards a target.
    public class TaskGoToTarget:BehaviourNode
    {
        public override NodeState Evaluate()
        {
            Node target = (Node)(GetData("target"));//Retreiving target data
            AgentBT.parent.GetNewTarget(target);//Gets the parent to calculate the path to the target.
            if (Vector2.Distance(AgentBT.parent.Position, target.Position) < 0.1f)//Checks if the character is at the target.
            {
                state = NodeState.SUCCESS;
                return state;
            }
            state = NodeState.RUNNING;
            return state;
        }
    }

    //Checks if the data dictionary has a target set that the agent should move towards.
    public class CheckHasTarget : BehaviourNode
    {
        public override NodeState Evaluate()
        {
            state = parent.parent.GetData("target") == null
                ? NodeState.FAILURE : NodeState.SUCCESS;
            return state;
        }
    }

    //Changes the target tile to have planted crops. Needs addition of game manager funcitonality
    //So that the plants take time to grow, otherwise the agent is stuck in an endless cycle of planting
    //And harvesting
    //Also needs addition of timer node before it so that there is a wait period before the node 
    //Changes type.
    public class PlantCrops : BehaviourNode
    {
        public override NodeState Evaluate()
        {
            Node target = (Node)GetData("target");
            if(Vector2.Distance(AgentBT.parent.Position, target.Position) < 1f)
            {
                //Thread.Sleep(200);
                //System.Timers.Timer timer = new System.Timers.Timer(10000);
                //timer.Elapsed += new System.Timers.ElapsedEventHandler(AgentBT.parent.map.ChangeNodeAfterTime(target.Position))
                //timer.AutoReset = false;
                //AgentBT.parent.map.NodeFromWorldPoint(target.Position).bitValue = 3;
                AgentBT.parent.map.NodeFromWorldPoint(target.Position).bitValue = 5;
                //AgentBT.parent.map.ChangeNodeAfterTime(target.Position, 3);
                AgentBT.root.ClearData("target");
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }

    //Same idea as Plant crops, but instead changes tile type to soil rather than plants.
    public class HarvestCrops : BehaviourNode
    {
        public override NodeState Evaluate()
        {
            Node target = (Node)GetData("target");
            if (Vector2.Distance(AgentBT.parent.Position, target.Position) < 1f)
            {
                Thread.Sleep(200);//Dumb idea. Makes the whole program freeze. Also needs timer for delay
                AgentBT.parent.map.NodeFromWorldPoint(target.Position).bitValue = 4;
                for(int i = 0; i < AgentBT.parent.inv.items.Length; i++)
                {
                    if (AgentBT.parent.inv.items[i] == null)
                    {
                        //AgentBT.parent.inv.items[i] = new Vegetable();
                        //break;
                    }
                }
                AgentBT.root.ClearData("target");
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}
