using AI_test.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Security.AccessControl;
using System.Timers;
using System.Diagnostics;
//https://gamedev-resources.com/get-started-with-behavior-trees/#Getting_started
//https://github.com/Yecats/UnityBehaviorTreeVisualizer/wiki/Standard-Behavior-Tree-Nodes

namespace AI_test.AI_and_Behaviours
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    //General Behaviour Tree Node class
    public class BehaviourNode
    {
        protected NodeState state;//This control the state of the node - Success, Failure or running
        public BehaviourNode parent;//Parent
        protected List<BehaviourNode> children = new List<BehaviourNode>();//All child nodes
        public int evaluationCount = 0;//How many times the node has been run. This currently resets every frame. Need to try and fix/find workaround in future.
        public bool IsFirstEvaluation => evaluationCount == 0;//Bool which returns true if the node has not yet been evaluated

        Dictionary<string, object> dataContext = new Dictionary<string, object>();//Holds all data. Only root node holds the tree data usually, but can set it to be any node.

        //Constructors for the node.
        public BehaviourNode()
        {
            parent = null;
        }

        public BehaviourNode(List<BehaviourNode> children)
        {
            foreach(BehaviourNode child in children)
                Attach(child);
        }

        //Adds children to the node
        private void Attach(BehaviourNode node)
        {
            node.parent = this;
            children.Add(node);
        }

        //Runs the process associated with the node and returns one of the three nodestates
        public virtual NodeState Evaluate() { evaluationCount++; return NodeState.FAILURE; }

        //Sets a data value
        public void SetData(string key, object value)
        {
            dataContext[key] = value;
        }

        //Returns a data value by going up the tree and checking if the node has the desired value. This is a recursive process
        public object GetData(string key)
        {
            object value = null;
            if(dataContext.TryGetValue(key, out value))
                return value;

            BehaviourNode node = parent;
            while(node != null)
            {
                value = node.GetData(key);
                if(value != null)
                    return value;
                node = node.parent;
            }
            return null;
        }

        //Same process as GetData(), but instead of returning the value, it deletes it.
        public bool ClearData(string key)
        {
            if (dataContext.ContainsKey(key))
            {
                dataContext.Remove(key);
                return true;
            }

            BehaviourNode node = parent;
            while(node != null)
            {
                bool cleared = node.ClearData(key);
                if(cleared)
                    return true;
                node = node.parent;
            }
            return false;
        }
    }

    //This is a special type of Composite Behaviour Node called a sequence. It runs all of its children.
    //If at least one returns failure, the sequence will also return failure.
    public class Sequence : BehaviourNode
    {
        public Sequence() : base() { }
        public Sequence(List<BehaviourNode> children) : base(children) { }

        public override NodeState Evaluate()
        {
            bool anyChildIsRunning = false;

            foreach(BehaviourNode node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        return state;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            }
            state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }
    }

    //This is another Composite Behaviour Node called a Selector. It runs all of its children until one returns success.
    //If all of its children return failure, the Selector also returns failure.
    public class Selector : BehaviourNode
    {
        public Selector() : base() { }
        public Selector(List<BehaviourNode> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (BehaviourNode node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        state = NodeState.SUCCESS;
                        return state;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return state;
                    default:
                        continue;
                }
            }

            state = NodeState.FAILURE;
            return state;
        }
    }

    //This is a type of Decorator Behaviour Node called an Inverter. It inverts the state returned
    //by its child.
    public class Inverter : BehaviourNode
    {
        public Inverter() : base() { }

        public Inverter(List<BehaviourNode> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach(BehaviourNode node in children)
            {
                if (node.Evaluate() == NodeState.SUCCESS)
                    state = NodeState.FAILURE;
                else if(node.Evaluate() == NodeState.FAILURE)
                    state = NodeState.SUCCESS;
            }
            return state;
        }
    }

    //The Timer Decorator runs its node for a set amount of time.
    //Current implementation does not work correctly. Needs fixing. Possibly due to evaluations 
    //Not being handled properly and the tree being reset every frame.
    public class TimerNode : BehaviourNode
    {
        private float startTime;
        private float timeToWait;
        private float elapsedTime;

        public TimerNode(float _timeToWait, List<BehaviourNode> children): base(children)
        {
            timeToWait = _timeToWait;
        }

        public override NodeState Evaluate()
        {
            //Confirm that a valid child node was passed in the constructor
            if(children.Count == 0 || children[0] == null)
                return NodeState.FAILURE;

            //Run the child node and calculate the elapsed time
            NodeState originalState = children[0].Evaluate();
            AgentBT.root.SetData("evaluationCount", 0);

            //If this is the first eval, then the start time needs to be set up.
            //Have a look at difference between gametime.ElapsedGameTime and gameTime.TotalGameTime
            if ((int)GetData("evaluationCount") == 0)
            {
                AgentBT.root.SetData("startTime", (float)AgentBT.parent._gameTime.TotalGameTime.TotalSeconds);
                AgentBT.root.SetData("evaluationCount", (int)GetData("evaluationCount")+1);
            }
                

            //Calculate how much time has passed.
            elapsedTime = (float)AgentBT.parent._gameTime.TotalGameTime.TotalSeconds - (float)GetData("startTime");

            //If more time has passed than we wanted, it's time to stop.
            if (elapsedTime > timeToWait)
            {
                //Debug.WriteLine(startTime);
                //Debug.WriteLine("SUCCESS");
                return NodeState.SUCCESS;
            }
            return NodeState.RUNNING;
        }
    }

    //Similar to the Timer Decorator, except the Delay node only runs its child once, after the set
    //time has passed. Also needs fixing for same problem as timer.
    public class Delay : BehaviourNode
    {
        private float startTime;
        private float timeToWait;

        //Excecutes a timer and then runs the child node once

        public Delay(float _timeToWait, List<BehaviourNode> children)
        {
            timeToWait = _timeToWait;
        }

        public override NodeState Evaluate()
        {
            if(children.Count == 0 || children[0] == null) 
                return NodeState.FAILURE;

            float elapsedTime = (float)AgentBT.parent._gameTime.ElapsedGameTime.TotalSeconds - startTime;

            if (IsFirstEvaluation)
                startTime = (float)AgentBT.parent._gameTime.ElapsedGameTime.TotalSeconds;
            else if (elapsedTime > timeToWait)
            {
                NodeState originalState = children[0].Evaluate();
                return originalState;
            }
            return NodeState.RUNNING;
        }
    }

    //Just a test node I made to test the Timer and Delay decorators.
    public class TestNode : BehaviourNode
    {
        public override NodeState Evaluate()
        {
            //Debug.WriteLine((float)AgentBT.parent._gameTime.TotalGameTime.TotalSeconds);
            return NodeState.SUCCESS;
        }
    }
}
