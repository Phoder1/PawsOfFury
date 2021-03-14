using UnityEngine;
namespace Assets.StateMachine
{
    public class StateMachine
    {
        State state;
        public StateMachine(State startState)
        {
            State = startState;
        }
        public State State
        {
            get => state;
            set
            {
                if (state == value)
                    return;

                if (state != null)
                    state.Disable();

                state = value;

                if (state != null)
                    state.Enable();
            }
        }
        public void Update()
        {
            if (State != null)
                State.Update();
        }
        public void Reset()
        {
            if (State != null)
                State.Reset();
        }
        ~StateMachine() => State = null;
    }
    public abstract class State
    {
        bool enabled;

        public void Enable()
        {
            if (!enabled)
            {
                Reset();
                OnEnable();
                enabled = true;
            }
        }
        protected virtual void OnEnable() { }
        public void Disable()
        {
            if (enabled)
            {
                OnDisable();
                enabled = false;
            }
        }
        protected virtual void OnDisable() { }
        public void Update()
        {
            if (enabled)
                OnUpdate();
        }
        protected virtual void OnUpdate() { }
        public void Reset()
        {
            if (enabled)
                OnReset();
        }
        protected virtual void OnReset() { }
    }
}
