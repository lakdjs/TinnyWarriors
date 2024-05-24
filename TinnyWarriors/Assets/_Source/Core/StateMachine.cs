using System;
using System.Collections.Generic;

namespace Core
{
    public class StateMachine<T>: IStateMachine where T : AState
    {
        private Dictionary<Type, T> _states;
        private AState _currState;
        
        public StateMachine(params T[] states)
        {
            SetupStates(states);
        }

        public StateMachine() { }

        public Type CurrentState => _currState.GetType();
        
        public void SetupStates(params T[] states)
        {
            _states = new();
            foreach (var state in states)
            {
                _states.Add(state.GetType(),state);
            }

            foreach (var state in _states)
            {
                state.Value.SetOwner(this);
            }
        }
        
        public bool ChangeState<T>()
        {
            _currState?.Exit();
            if (_states.ContainsKey(typeof(T)))
            {
                _currState = _states[typeof(T)];
                _currState.Enter();
                return true;
            }

            return false;
        }
        
        public void Update() => _currState.Update();
    }
}