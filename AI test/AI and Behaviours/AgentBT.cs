using AI_test.Core;
using AI_test.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_test.AI_and_Behaviours
{
    public class AgentBT : BehaviourTree
    {
        public static float _speed;
        public static Pathfinding _pathfinding;
        public static Agent parent;

        public AgentBT(Agent _parent)
        {
            parent = _parent;
        }

        protected override BehaviourNode SetUpTree()
        {
            throw new NotImplementedException();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch){}
    }
}
