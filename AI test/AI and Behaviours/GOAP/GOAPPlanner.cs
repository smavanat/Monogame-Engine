using AI_test.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using AI_test.Sprites;
//https://github.com/sploreg/goap/tree/master/Assets/Standard%20Assets/Scripts/AI/GOAP
namespace AI_test.AI_and_Behaviours
{   
    public class GOAPPlanner
    {
        /*
         * Plan what sequence of Actions fulfill the goal.
         * Returns null if a plan could not be found, or a list of the actions
         * that must be performed in order to fulfill the goal.
         */
        public Stack<GOAPAction> Plan(Sprite agent, HashSet<GOAPAction> availableActions,
                                            HashSet<KeyValuePair<string, object>> worldState,
                                            HashSet<KeyValuePair<string, object>> goal)
        {
            //Reset the actions so that we can start afresh
            foreach(GOAPAction action in availableActions)
            {
                action.DoReset();
            }

            //Check what actions can be run using their CheckProceduralPrecondition
            HashSet<GOAPAction> usableActions = new HashSet<GOAPAction>();
            foreach (GOAPAction a in availableActions)
            {
                if (a.CheckProceduralPrecondition(agent))
                    usableActions.Add(a);
            }

            //We now have all actions that can run stored in usableActions

            //Build the tree and record leaf nodes that provide a solution to the goal.
            List<GOAPNode> leaves = new List<GOAPNode>();

            //Build graph
            GOAPNode start = new GOAPNode(null, 0, goal, null);
            HashSet<KeyValuePair<string, object>> end = worldState;
            bool success = BuildGraph(end, start, leaves, usableActions, goal);

            if (!success)
            {
                //No plan 
                Debug.WriteLine("No plan");
                return null;
            }

            //Get the cheapest leaf
            GOAPNode cheapest = null;
            foreach (GOAPNode leaf in leaves)
            {
                if (cheapest == null)
                    cheapest = leaf;
                else
                {
                    if (leaf.runningCost < cheapest.runningCost)
                        cheapest = leaf;
                }
            }

            //Get its node and work back through the parents
            List<GOAPAction> result = new List<GOAPAction>();
            GOAPNode n = cheapest;
            while (n != null)
            {
                if (n.action != null)
                    result.Add(n.action);//Insert the action at the front
                n = n.parent;
            }
            //We now have the action list in correct order.

            Stack<GOAPAction> queue = new Stack<GOAPAction>();
            foreach (GOAPAction action in result)
            {
                queue.Push(action);
            }
            //Now we have a plan
            return queue;
        }

        /*
         * Returns true if at least one solution was found.
         * The possible paths are stored in the leaves list. 
         * Each leaf has a "runningCost" value where the lowest cost will be the best action sequence
         */
        private bool BuildGraph(HashSet<KeyValuePair<string, object>> end, GOAPNode parent, List<GOAPNode> leaves, HashSet<GOAPAction> usableActions, HashSet<KeyValuePair<string, object>> goal)
        {
            bool foundOne = false;

            //Go through each action available at this node and see if we can use it here
            foreach(GOAPAction action in usableActions)
            {
                //If the parent state has the conditions for this action's preconditions, we can use it here
                if(InState(action.Effects, parent.state))
                {
                    //Apply the action's effects to the parent state
                    HashSet<KeyValuePair<string, object>> currentState = PopulateState(parent.state, action.Effects, action.Preconditions);
                    GOAPNode node = new GOAPNode(parent, parent.runningCost + action.cost, currentState, action);

                    if(InState(currentState, end))
                    {
                        //We found a solution
                        leaves.Add(node);
                        foundOne = true;
                    }
                    else
                    {
                        //Not at a solution yet, so test all the remaining actions and branch out the tree
                        HashSet<GOAPAction> subset = ActionSubset(usableActions, action);
                        bool found = BuildGraph(end, node, leaves, subset, goal);
                        if (found)
                            foundOne = true;
                    }
                }
            }
            return foundOne;
        }

        //Creates a new set of actions excluding removeMe
        private HashSet<GOAPAction> ActionSubset(HashSet<GOAPAction> actions, GOAPAction removeMe)
        {
            HashSet<GOAPAction> subset = new HashSet<GOAPAction>();
            foreach(GOAPAction action in actions)
            {
                if(!action.Equals(removeMe))
                    subset.Add(action);
            }
            return subset;
        }

        /*
         * Check that all items in 'test' are in 'state'. 
         * If one does not match or is not there then this returns false
         */
        private bool InState(HashSet<KeyValuePair<string, object>> test, HashSet<KeyValuePair<string, object>>state)
        {
            bool allMatch = true;
            foreach(KeyValuePair<string, object> t in test)
            {
                bool match = false;
                foreach(KeyValuePair<string, object> s in state)
                {
                    if (s.Equals(t))
                    {
                        match = true;
                        break;
                    }
                }
                if (!match)
                    allMatch = false;

            }
            return allMatch;
        }

        //Apply the stateAdd to the currentState
        private HashSet<KeyValuePair<string, object>> PopulateState(HashSet<KeyValuePair<string, object>> currentState, HashSet<KeyValuePair<string, object>> stateRemove, HashSet<KeyValuePair<string, object>> stateAdd)
        {
            HashSet<KeyValuePair<string, object>> state = new HashSet<KeyValuePair<string, object>>();
            //Copy the KVPs over as new objects 
            foreach(KeyValuePair<string, object> t in currentState)
            {
                state.Add(new KeyValuePair<string, object>(t.Key, t.Value));
            }

            foreach(KeyValuePair<string, object> change in stateRemove)
            {
                //If the key exists in the current state, remove it
                bool exists = false;

                foreach(KeyValuePair<string, object> s in state)
                {
                    if (s.Equals(change))
                    {
                        exists = true;
                        break;
                    }
                }

                if(exists)
                    state.RemoveWhere((KeyValuePair<string, object> kvp) => { return kvp.Key.Equals(change.Key); });

            }

            foreach(KeyValuePair <string, object> change in stateAdd)
            {
                //If the key exists in the current state, updae the value
                bool exists = false;

                foreach(KeyValuePair<string,object> s in state)
                {
                    if(s.Equals(change))
                    {
                        exists = true;
                        break;
                    }
                }

                if (exists)
                {
                    state.RemoveWhere((KeyValuePair<string, object> kvp) => { return kvp.Key.Equals(change.Key); });
                    KeyValuePair<string, object> updated = new KeyValuePair<string, object>(change.Key, change.Value);
                    state.Add(updated);
                }
                //If it does not exist in the current state, add it
                else
                {
                    state.Add(new KeyValuePair<string, object> ( change.Key, change.Value ));
                }
            }
            return state;
        }

        //Used for building up the graph and holding the running costs of actions
        private class GOAPNode
        {
            public GOAPNode parent;
            public float runningCost;
            public HashSet<KeyValuePair<string, object>> state;
            public GOAPAction action;

            public GOAPNode(GOAPNode _parent, float _runningCost, HashSet<KeyValuePair<string, object>> _state, GOAPAction _action)
            {
                parent = _parent;
                runningCost = _runningCost;
                state = _state;
                action = _action;
            }
        }
    }
}