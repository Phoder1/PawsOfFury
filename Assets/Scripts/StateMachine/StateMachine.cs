using UnityEngine;
namespace Assets.StateMachine
{
    public class StateMachine
    {
        State state;
        StateMachineHandler stateMachineHandler;
        public StateMachine(State startState)
        {
            stateMachineHandler = StateMachineHandler._instance;
            State = startState;
        }
        public State State
        {
            get => state;
            set
            {
                if (state != value)
                {
                    if (state != null)
                        state.Disable();
                    else
                        stateMachineHandler.Subscribe(this);
                    state = value;
                    if (state != null)
                        state.Enable();
                    else
                        stateMachineHandler.UnSubscribe(this);
                }
            }
        }
        public void Update()
        {
            if (state != null)
                state.Update();
        }
        public void Reset()
        {
            if (state != null)
                state.Update();
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
