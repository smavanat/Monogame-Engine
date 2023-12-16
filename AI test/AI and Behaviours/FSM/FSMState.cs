using AI_test.Core;
using AI_test.Sprites;

namespace AI_test.AI_and_Behaviours
{
    public interface FSMState
    {
        void Update(FSM fsm, Sprite gameObject);
    }
}
