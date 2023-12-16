using AI_test.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI_test.ToolsIIStorageIIPrefabs;

namespace AI_test.AI_and_Behaviours
{
    //Sets arable nodes as the target data
    internal class CheckArableLand : BehaviourNode
    {
        Grid map;
        public CheckArableLand(Grid _map) 
        {
            map = _map;
        }

        public override NodeState Evaluate()
        {
            Queue<Node> queue = new Queue<Node>();
            foreach (Node node in map.grid)
            {
                if (node.bitValue == 4)
                    queue.Enqueue(node);
            }
            if(queue.Count > 2)
            {
                AgentBT.root.SetData("target", queue.Dequeue());
                state = NodeState.SUCCESS;
                return state;
            }
            state = NodeState.FAILURE;
            return state;
        }
    }

    //Sets harvestable nodes as the target data
    internal class CheckHarvestableLand : BehaviourNode
    {
        Grid map;
        public CheckHarvestableLand(Grid _map)
        {
            map = _map;
        }

        public override NodeState Evaluate()
        {
            Queue<Node> queue = new Queue<Node>();
            foreach (Node node in map.grid)
            {
                if (node.bitValue == 3)
                    queue.Enqueue(node);
            }
            if (queue.Count > 2)
            {
                AgentBT.root.SetData("target", queue.Dequeue());
                state = NodeState.SUCCESS;
                return state;
            }
            state = NodeState.FAILURE;
            return state;
        }
    }

    internal class NumberCropsInInventory : BehaviourNode
    {
        public override NodeState Evaluate()
        {
            int total = 0;
            for (int i = 0; i < AgentBT.parent.inv.items.Length; i++)
            {
                if (AgentBT.parent.inv.items[i] != null && AgentBT.parent.inv.items[i].GetType() == typeof(Vegetable))
                    total++;
            }

            if(total != 0)
                Debug.WriteLine(total);
            if(total < 3)
            {
                return NodeState.FAILURE;
            }
            return NodeState.SUCCESS;
        }
    }
}
