using AI_test.Core;
using System.Collections.Generic;
using AI_test.Sprites;
//https://github.com/sploreg/goap/tree/master/Assets/Standard%20Assets/Scripts/AI/GOAP
namespace AI_test.AI_and_Behaviours
{
    public abstract class GOAPAction : Component
    {
        //The state that is required for the action to run
        private HashSet<KeyValuePair<string, object>> preconditions;
        //Change to state after the action has been run
        private HashSet<KeyValuePair<string, object>> effects;

        //Checks if you are within range of the target of the action.
        private bool inRange = false;

        //The cost of performing the action. Changing the weight will affect which
        //Actions are chosen during planning.
        public float cost = 1f;

        //The target of the action. Can be null.
        public Sprite target;

        public GOAPAction()
        {
            preconditions = new HashSet<KeyValuePair<string, object>>();
            effects = new HashSet<KeyValuePair<string, object>>();
        }

        //Resets action to default values
        public void DoReset()
        {
            inRange = false;
            target = null;
            Reset();
        }

        //Resets any variables that need to be reset before planning happens again.
        public abstract void Reset();

        //Checks if the action is complete.
        public abstract bool IsDone();

        //Procedurally checks if the action can run. This is not needed for all actions.
        public abstract bool CheckProceduralPrecondition(Sprite agent);

        //Run the action. Returns true if the action performed successfully or false if 
        //The action can no longer be performed. In this case the action queue should clear
        //And the goal cannot be reached.
        public abstract bool Perform(Sprite agent);

        //States if the action needs to be in range of a target object. If not then the 
        //MoveTo state will not run for this action.
        public abstract bool RequiresInRange();

        //Checks if in range of the target. Will be set by MoveTo state and reset every time
        //The action is performed.
        public bool IsInRange()
        {
            return inRange;
        }

        public void SetInRange(bool _inRange)
        {
            this.inRange = _inRange;
        }

        //Adds precondition to the action
        public void AddPrecondition(string key, object value)
        {
            preconditions.Add(new KeyValuePair<string, object> ( key, value ) );
        }

        //Removes a precondition from the action.
        public void RemovePrecondition(string key)
        {
            KeyValuePair<string, object> remove = default( KeyValuePair<string, object> );
            foreach(KeyValuePair<string, object> pair in preconditions)
            {
                if(pair.Key == key )
                    remove = pair;
            }
            if(!default(KeyValuePair<string, object>).Equals(remove))
                preconditions.Remove(remove);
        }

        //Adds an effect
        public void AddEffect(string key, object value)
        {
            effects.Add(new KeyValuePair<string, object>(key, value));
        }

        //Removes an effect
        public void RemoveEffect(string key)
        {
            KeyValuePair<string, object> remove = default(KeyValuePair<string, object>);
            foreach (KeyValuePair<string, object> pair in effects)
            {
                if (pair.Key == key)
                    remove = pair;
            }
            if (!default(KeyValuePair<string, object>).Equals(remove))
                effects.Remove(remove);
        }

        public HashSet<KeyValuePair<string, object>> Preconditions
        {
            get { return preconditions; }
        }

        public HashSet<KeyValuePair<string, object>> Effects 
        { 
            get { return effects; }
        }
    }
}
