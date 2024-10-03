using AI_test.Core;
using AI_test.Sprites;
using AI_test.ToolsIIStorageIIPrefabs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AI_test.AI_and_Behaviours
{
    public abstract class Worker: Component, IGOAP
    {
        private Inventory inventory;
        public float moveSpeed = 1;
        private Sprite Parent;

        public Worker(Component _parent): base(_parent)
        {
            Parent = parent as Sprite;
            if(inventory == null)
            {
                inventory = Parent.GetComponent<Inventory>();
            }
        }
        
        //Key-Value data that will feed the GOAP actions and system while planning

        public HashSet<KeyValuePair<string, object>> GetWorldState()
        {
            HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();
            worldData.Add(new KeyValuePair<string, object>("HasFirewood", (inventory.numFirewood > 0)));

            return worldData;
        }

        //Implement in subclasses

        public abstract HashSet<KeyValuePair<string, object>> CreateGoalState();

        public void PlanFailed(HashSet<KeyValuePair<string, object>> failedGoal)
        {
            // Not handling this here since we are making sure our goals will always succeed.
            // But normally you want to make sure the world state has changed before running
            // the same goal again, or else it will just fail.
        }

        public void PlanFound(HashSet<KeyValuePair<string, object>> goal, Stack<GOAPAction> actions)
        {
            Debug.WriteLine("Plan Found " + GOAPAgent.PrettyPrint(actions));
        }

        public override void Update(GameTime gameTime){}
        public void ActionsFinished()
        {
            //We have completed our actions for this goal
            Debug.WriteLine("Actions Finished");
        }

        public void PlanAborted(GOAPAction aborter)
        {
            //An action bailed out of the plan. State has been reset to plan again.
            //Take note of what happened and make sure that if you run the same goal again
            //that it can succeed
            Debug.WriteLine("Plan Aborted " + GOAPAgent.PrettyPrint(aborter));
        }

        public bool MoveAgent(GOAPAction nextAction)
        {
            //Move towards the nextAction's target
            float step = moveSpeed;
            //Parent.Position = (Vector2)(nextAction.target.Position -  Parent.Position) * step;
            Vector2 dir = nextAction.target.Position - Parent.Position;
            dir.Normalize();
            Parent.Position += dir * moveSpeed;

            if(Vector2.Distance(Parent.Position, nextAction.target.Position) < 1f)
            {
                //We are at the next target, so finished
                nextAction.SetInRange(true);
                return true;
            }else
                return false;
        }
    }
}