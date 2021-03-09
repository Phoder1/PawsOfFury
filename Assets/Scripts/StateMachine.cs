namespace Assets.StateMachine
{
    class StateMachine
    {
        State state;
        public StateMachine(State state)
        {
            State = state;
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
                    state = value;
                    if (state != null)
                        state.Enable();
                }
            }
        }
    }
    abstract class State
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
