using AI_test.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_test.AI_and_Behaviours
{
    public class PlantCropsTask : BehaviourNode
    {
        Grid map;
        private Node[] path;
        int targetIndex = 0;
        Node currentWaypoint;

        public PlantCropsTask(Grid _map)
        {
            map = _map;
        }

        public override NodeState Evaluate()
        {
            currentWaypoint = path[targetIndex];
            AgentBT.parent.GetNewTarget(currentWaypoint);

            state = NodeState.RUNNING;
            return state;
        }

        private void FindCropTiles()
        {
            List<Node> list = new List<Node>();
            foreach(Node node in map.grid)
            {
                if(node.bitValue == 4)
                    list.Add(node);

            }
            path = list.ToArray();
        }
    }
}
