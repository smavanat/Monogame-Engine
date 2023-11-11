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
        public static Pathfinding _pathfinding;//For pathfinding info for behaviour nodes.
        //public static Agent parent;

        public AgentBT(Agent _parent): base(_parent) {}

        //public Dictionary<BehaviourNode, int> evals = new Dictionary<BehaviourNode, int>();

        protected override BehaviourNode SetUpTree()
        {
            root = new Selector(new List<BehaviourNode>
            {
                new TimerNode(2f, new List<BehaviourNode>//Testing the timer node
                {
                    new TestNode()
                })
                //The current tree does not work, which is why everything is commented out. This is due to 
                //The fact that we need to implement time delays with a Timer node and general Entity
                //Manager functionality to control time delays (i.e. crops need to wait some time before)
                //becoming fully grown. This is particularly difficult and finnicky with Monogame's system
                //Would require some sort of out of Update way of storing data without it being reset every
                //Frame.
                //new Sequence(new List<BehaviourNode>
                //{
                    
                //}),
                //new Sequence(new List<BehaviourNode>
                //{
                //    new CheckHasTarget(),
                //    new TaskGoToTarget(),
                //    new PlantCrops()
                //}),
                //new Sequence(new List<BehaviourNode>
                //{
                //    new CheckHarvestableLand(parent.map),
                //    new Inverter(new List<BehaviourNode>()
                //    {
                //        new CheckHasTarget(),
                //        new NumberCropsInInventory()
                //    }),
                //    new TaskGoToTarget(),
                //    new HarvestCrops()
                //}),
                
                //new CheckArableLand(parent.map),
                //new NumberCropsInInventory()
                //new Selector(new List<BehaviourNode>
                //{
                //    new Sequence(new List<BehaviourNode>
                //    {
                //        new CheckHasTarget(),
                //        new TaskGoToTarget(),
                //        new PlantCrops()
                //    }),
                //    new CheckArableLand(parent.map)
                //}),
                //new Selector(new List<BehaviourNode>
                //{
                //    new Sequence(new List<BehaviourNode>
                //    {
                //        new CheckHasTarget(),
                //        new TaskGoToTarget(),
                //        new HarvestCrops()
                //    }),
                //    new CheckHarvestableLand(parent.map)
                //})
            });
            return root;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch){}
    }
}
