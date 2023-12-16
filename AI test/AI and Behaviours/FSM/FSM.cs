using AI_test.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI_test.Sprites;

namespace AI_test.AI_and_Behaviours
{
    //Stack-based Finite State Machine
    //Push and pop states to the FSM
    //States should push other states onto the stack and pop themselves off.
    public class FSM
    {
        private Stack<FSMState> stateStack = new Stack<FSMState>();

        public delegate void FSMState(FSM fsm, Sprite gameObject);

        public void Update(Sprite gameObject)
        {
            if (stateStack.Peek() != null)
                stateStack.Peek().Invoke(this, gameObject);
        }

        public void PushState(FSMState state)
        {
            stateStack.Push(state);
        }
        public void PopState()
        {
            stateStack.Pop();
        }
    }
}