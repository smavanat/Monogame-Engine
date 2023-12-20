using AI_test.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI_test.ToolsIIStorageIIPrefabs;
using AI_test.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace AI_test.AI_and_Behaviours
{
    public class ChopFirewoodAction : GOAPAction
    {
        private bool chopped = false;
        private ChoppingBlock targetChoppingBlock; //Where we chop the firewood

        private float startTime = 0;
        public float workDuration = 2;//Seconds

        public ChopFirewoodAction()
        {
            //AddPrecondition("HasTool", true);//We need a tool to do this
            AddPrecondition("HasFirewood", false); //If we have firewood we don't want more
            AddEffect("HasFirewood", true);
        }

        public override void Reset()
        {
            chopped = false;
            targetChoppingBlock = null;
            startTime = 0;
        }

        public override bool IsDone()
        {
            return chopped;
        }

        public override bool RequiresInRange()
        {
            return true; //Yes we need to be near a chopping block
        }
        
        public override bool CheckProceduralPrecondition(Sprite agent)
        {
            //Find the nearest chopping block that we can chop our wood at.
            List<ChoppingBlock> blocks = EntityManager.FindObjectsOfType<ChoppingBlock>();
            ChoppingBlock closest = null;
            float closestDist = 0;

            foreach (ChoppingBlock block in blocks)
            {
                if (closest == null)
                {
                    //First one so choose it for now
                    closest = block;
                    closestDist = (block.Position - agent.Position).Length();
                }
                else
                {
                    //Checks if this one is closer than the last
                    float dist = (block.Position - agent.Position).Length();
                    if(dist < closestDist)
                    {
                        //A closer one has been found, so we should use it
                        closest = block;
                        closestDist = dist;
                    }
                }
            }
            if (closest == null)
                return false;

            targetChoppingBlock = closest;
            target = targetChoppingBlock;

            return closest != null;
        }

        public override bool Perform(Sprite agent)
        {
            if (startTime == 0)
                startTime = Time.time;

            if ((float)(Time.time - startTime) > workDuration)
            {
                //Finished Chopping
                Inventory inventory = agent.GetComponent<Inventory>();
                if (inventory != null)
                {
                    inventory.numFirewood += 5;
                    chopped = true;
                }
            }
            return true;
        }

        public override void Update(GameTime gameTime){}
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch){}
    }
}