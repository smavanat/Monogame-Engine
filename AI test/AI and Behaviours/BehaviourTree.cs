using AI_test.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_test.AI_and_Behaviours
{
    public abstract class BehaviourTree : Component
    {
        private BehaviourNode root = null;

        public BehaviourTree()
        {
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
