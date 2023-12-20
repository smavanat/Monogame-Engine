using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using AI_test.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_test.AI_and_Behaviours
{
    public class Woodcutter : Worker
    {
        //Our only goal will ever be to chop logs
        //The ChopFirewoodAction will be able to fulfill this goal

        public Woodcutter(Component _parent) : base(_parent) {}

        public override HashSet<KeyValuePair<string, object>> CreateGoalState()
        {
            HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

            goal.Add(new KeyValuePair<string, object>("CollectFirewood", true));
            return goal;
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch){}
    }
}
