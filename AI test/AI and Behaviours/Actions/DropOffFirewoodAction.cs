using AI_test.Core;
using AI_test.Sprites;
using AI_test.ToolsIIStorageIIPrefabs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AI_test.AI_and_Behaviours
{
    internal class DropOffFirewoodAction : GOAPAction
    {
        private bool droppedOffFirewood = false;
        private SupplyPile targetSupplyPile; //Where we drop off the firewood

        public DropOffFirewoodAction() 
        {
            AddPrecondition("HasFirewood", true); //Can't drop off firewood if we don't have some
            AddEffect("HasFirewood", false); //We now have no firewood
            AddEffect("CollectFirewood", true); //We collected firewood
        }

        public override void Reset()
        {
            droppedOffFirewood = false;
            targetSupplyPile = null;
        }

        public override bool IsDone()
        {
            return droppedOffFirewood;
        }

        public override bool RequiresInRange()
        {
            return true; //Yes we need to be near a supply pile to drop off firewood
        }

        public override bool CheckProceduralPrecondition(Sprite agent)
        {
            //Find the nearest supply pile that has spare firewood.
            List<SupplyPile> supplyPiles = EntityManager.FindObjectsOfType<SupplyPile>();
            SupplyPile closest = null;
            float closestDist = 0;

            foreach(SupplyPile supplyPile in supplyPiles)
            {
                if (closest == null)
                {
                    //First one so choose it for now
                    closest = supplyPile;
                    closestDist = (supplyPile.Position - agent.Position).Length();
                }
                else
                {
                    //Checks if this one is closer than the last
                    float dist = (supplyPile.Position - agent.Position).Length();
                    if (dist < closestDist)
                    {
                        //Found a closer one so use it
                        closest = supplyPile;
                        closestDist = dist;
                    }
                }
            }
            if(closest == null)
            {
                return false;
            }

            targetSupplyPile = closest;
            target = targetSupplyPile;

            return closest != null;
        }

        public override bool Perform(Sprite agent)
        {
            Inventory inventory = agent.GetComponent<Inventory>();
            targetSupplyPile.numFirewood += inventory.numFirewood;
            droppedOffFirewood = true;
            inventory.numFirewood = 0;

            return true;
        }

        public override void Update(GameTime gameTime){}

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch){}
    }
}