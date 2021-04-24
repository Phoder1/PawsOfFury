using System;
using UnityEngine;
namespace Assets.StateMachine
{
    public class StateMachine<T> where T : State
    {
        T state;
        public StateMachine(T startState = null)
        {
            State = startState;
        }
        public T State
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
        public void Update() => State?.Update();
        public void Reset()
        {
            if (State != null)
                State.Reset();
        }
        ~StateMachine() => State = null;
        public static implicit operator T(StateMachine<T> x) => x.State;
    }
    public abstract class State
    {
        bool enabled;

        public void Enable()
        {
            if (!enabled)
            {
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
