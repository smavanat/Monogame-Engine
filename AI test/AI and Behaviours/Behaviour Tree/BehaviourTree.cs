using AI_test.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI_test.Sprites;

namespace AI_test.AI_and_Behaviours
{
    //Base behaviour tree class
    public abstract class BehaviourTree : Component
    {
        public static BehaviourNode root = null;//Root behaviour node of the tree.
        public static Agent parent;//This means that the agent can feed some data (such as world info or pathfinding) to the BT

        public BehaviourTree(Agent _parent)
        {
            parent = _parent;
            SetUpTree();
        }

        public override void Update(GameTime gameTime)
        {
            if (root != null)
                root.Evaluate();
        }

        protected abstract BehaviourNode SetUpTree();
    }
}
