using AI_test.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using AI_test.Core;
using System.Linq;

namespace AI_test.AI_and_Behaviours
{
    public class GOAPAgent : Component
    {
        private FSM stateMachine;

        private FSM.FSMState idleState; //Finds something to do
        private FSM.FSMState moveToState; //Moves to a target
        private FSM.FSMState performActionState; //Performs an action

        private HashSet<GOAPAction> availableActions;
        private Queue<GOAPAction> currentActions;

        // this is the implementing class that provides our world data and listens to
        // feedback on planning
        private IGOAP dataProvider;

        private GOAPPlanner planner;

        public GOAPAgent(Component _parent):base(_parent)
        {
            stateMachine = new FSM();
            availableActions = new HashSet<GOAPAction>();
            currentActions = new Queue<GOAPAction>();
            planner = new GOAPPlanner();
            FindDataProvider();
            CreateIdleState();
            CreateMoveToState();
            CreatePerformActionState();
            stateMachine.PushState(idleState);
            LoadActions();
        }

        public override void Update(GameTime gameTime)
        {
            stateMachine.Update(this.parent as Sprite);
        }

        public void AddAction(GOAPAction action)
        {
            availableActions.Add(action);
        }

        public GOAPAction GetAction(Type action)
        {
            foreach(GOAPAction g in availableActions)
            {
                if(g.GetType().Equals(action))
                    return g;
            }
            return null;
        }

        public void RemoveAction(GOAPAction action) 
        { 
            availableActions.Remove(action);
        }

        private bool HasActionPlan()
        {
            return currentActions.Count > 0;
        }

        private void CreateIdleState()
        {
            idleState = (fsm, gameObject) =>
            {
                //GOAP planning

                //Get the world state and the goal we want to plan for
                HashSet<KeyValuePair<string, object>> worldState = dataProvider.GetWorldState();
                HashSet<KeyValuePair<string, object>> goal = dataProvider.CreateGoalState();

                //Plan
                Queue<GOAPAction> plan = planner.Plan(gameObject, availableActions, worldState, goal);
                if(plan != null)
                {
                    //We have a plan
                    currentActions = plan;
                    dataProvider.PlanFound(goal, plan);

                    fsm.PopState(); //Move to PerformAction State
                    fsm.PushState(performActionState);
                }
                else
                {
                    //We don't have a plan
                    Debug.WriteLine("Failed Plan" + PrettyPrint(goal));
                    dataProvider.PlanFailed(goal);
                    fsm.PopState(); //Move to IdleAction State
                    fsm.PushState(idleState);
                }
            };
        }

        private void CreateMoveToState()
        {
            moveToState = (fsm, gameObject) =>
            {
                //Move to game object
                GOAPAction action = currentActions.Peek();
                if(action.RequiresInRange() && action.target == null)
                {
                    Debug.WriteLine("Action requires a target but has none. Planning failed. You did not assign the target in your Action.checkProceduralPrecondition()");
                    fsm.PopState();//Move
                    fsm.PopState();//Perform action
                    fsm.PushState(idleState);
                    return;
                }

                //Get the agent to move itself
                if (dataProvider.MoveAgent(action))
                    fsm.PopState();

                /*MovableComponent movable = (MovableComponent) gameObj.GetComponent(typeof(MovableComponent));
			    if (movable == null) {
				    Debug.Log("<color=red>Fatal error:</color> Trying to move an Agent that doesn't have a MovableComponent. Please give it one.");
				    fsm.popState(); // move
				    fsm.popState(); // perform
				    fsm.pushState(idleState);
				    return;
			    }

    			float step = movable.moveSpeed * Time.deltaTime;
	    		gameObj.transform.position = Vector3.MoveTowards(gameObj.transform.position, action.target.transform.position, step);

		    	if (gameObj.transform.position.Equals(action.target.transform.position) ) {
			    	// we are at the target location, we are done
				    action.setInRange(true);
				    fsm.popState();
			    }*/
            };
        }

        private void CreatePerformActionState()
        {
            performActionState = (fsm, gameObject) =>
            {
                //Perform the action

                if (!HasActionPlan())
                {
                    //No action to perform.
                    Debug.WriteLine("No Actions to perform");
                    fsm.PopState();
                    fsm.PushState(idleState);
                    dataProvider.ActionsFinished();
                    return;
                }

                GOAPAction action = currentActions.Peek();
                if (action.IsDone())
                {
                    //The action is done so that we can perform the next one
                    currentActions.Dequeue();
                }

                if(HasActionPlan())
                {
                    //Perform the next action
                    action = currentActions.Peek();
                    bool inRange = action.RequiresInRange() ? action.IsInRange() : true;

                    if(inRange)
                    {
                        //We are in range so perform the action.
                        bool success = action.Perform(gameObject);

                        if (!success)
                        {
                            //Action failed, we need to plan again.
                            fsm.PopState();
                            fsm.PushState(idleState);
                            dataProvider.PlanAborted(action);
                        }
                    } 
                    else
                    {
                        //We need to move there first. Push to move state.
                        fsm.PushState(moveToState);
                    }
                }
                else
                {
                    //No actions left, move to plan state.
                    fsm.PopState();
                    fsm.PushState(idleState);
                    dataProvider.ActionsFinished();
                }
            };
        }

        private void FindDataProvider() 
        {
            foreach (var child in parent.children)
            {
                if (typeof(IGOAP).IsAssignableFrom(child.GetType()))
                {
                    Debug.WriteLine("IGOAP found");
                    dataProvider = child as IGOAP;
                    return;
                }
            }
        }

        private void LoadActions()
        {
            List<GOAPAction> actions = parent.GetComponents<GOAPAction>();
            foreach (GOAPAction a in actions)
            {
                availableActions.Add(a);
            }
            Debug.WriteLine("Found actions: " + PrettyPrint(actions));
        }

        public static string PrettyPrint(HashSet<KeyValuePair<string, object>> state)
        {
            String s = "";
            foreach (KeyValuePair<string, object> kvp in state)
            {
                s += kvp.Key + ":" + kvp.Value.ToString();
                s += ", ";
            }
            return s;
        }

        public static string PrettyPrint(Queue<GOAPAction> actions)
        {
            String s = "";
            foreach (GOAPAction a in actions)
            {
                s += a.GetType().Name;
                s += "-> ";
            }
            s += "GOAL";
            return s;
        }

        public static string PrettyPrint(List<GOAPAction> actions)
        {
            String s = "";
            foreach (GOAPAction a in actions)
            {
                s += a.GetType().Name;
                s += ", ";
            }
            return s;
        }

        public static string PrettyPrint(GOAPAction action)
        {
            String s = "" + action.GetType().Name;
            return s;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch){}
    }
}
