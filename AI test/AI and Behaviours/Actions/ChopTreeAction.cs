using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using AI_test.ToolsIIStorageIIPrefabs;
using AI_test.Sprites;
using AI_test.Core;
using Microsoft.Xna.Framework.Graphics;

namespace AI_test.AI_and_Behaviours.Actions
{
    public class ChopTreeAction : GOAPAction
    {
        private bool chopped = false;
        private Tree targetTree; //Where we get the logs from

        private float startTime = 0;
        public float workDuration = 2;//seconds

        public ChopTreeAction()
        {
            AddPrecondition("HasLogs", false);//If we have logs we don't want more  
            AddEffect("HasLogs", true);
        }

        public override void Reset()
        {
            chopped = false;
            targetTree = null;
            startTime = 0;
        }

        public override bool IsDone()
        {
            return chopped;
        }

        public override bool RequiresInRange()
        {
            return true;//Yes we want to be near a tree
        }

        public override bool CheckProceduralPrecondition(Sprite agent)
        {
            //Find the nearest tree that we can chop
            List<Tree> trees = EntityManager.FindObjectsOfType<Tree>();
            Tree closest = null;
            float closestDist = 0;

            foreach(Tree t in trees)
            {
                if(closest == null)
                {
                    //First one so choose it for now
                    closest = t;
                    closestDist = (t.Position - agent.Position).Length();
                }
                else
                {
                    //Is this one closer than the last
                    float dist = (t.Position - agent.Position).Length();
                    if(dist < closestDist)
                    {
                        //We found a closer one so use it
                        closest = t;
                        closestDist = dist;
                    }
                }
            }
            if (closest == null)
                return false;
            targetTree = closest;
            target = targetTree;

            return closest != null;
        }

        public override bool Perform(Sprite agent)
        {
            if(startTime == 0) startTime = Time.time;

            if(Time.time -  startTime > workDuration)
            {
                //Finished chopping 
                Inventory inventory = agent.GetComponent<Inventory>();
                inventory.numLogs += 1;
                chopped = true;
            }
            return true;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch){}
        public override void Update(GameTime gameTime){}
    }
}
