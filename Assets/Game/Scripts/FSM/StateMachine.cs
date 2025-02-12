using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.FSM
{
    [Serializable]
    public class StateMachine<T>
    {
        [SerializeReference] private State<T> _currentStateInstance;
        private readonly Dictionary<Enum, State<T>> _states;

        public StateMachine(Dictionary<Enum, State<T>> states, Enum initialState)
        {
            foreach (var entry in states)
                entry.Value.StateChange += OnStateChanged;

            _states = states;
            _currentStateInstance = states.GetValueOrDefault(initialState);
            CurrentState = initialState;
            CanChangeState = true;
        }

        public bool CanChangeState { get; set; }
        public Enum CurrentState { get; private set; }


        public void Start()
        {
            _currentStateInstance.Enter();
        }


        public void ChangeState<TParam>(Enum nextState, TParam data)
        {
            if (!CanChangeState)
                return;

            var nextStateInstance = _states.GetValueOrDefault(nextState);
            if (nextStateInstance != null)
            {
                _currentStateInstance.Exit();
                if (data != null)
                    nextStateInstance.Enter(data);
                else
                    nextStateInstance.Enter();

                _currentStateInstance = nextStateInstance;
                CurrentState = nextState;
            }
        }

        public void ChangeState(Enum nextState)
        {
            ChangeState<object>(nextState, null);
        }

        private void OnStateChanged(Enum nextState)
        {
            if (nextState != null)
                ChangeState(nextState);
        }

        public void Update()
        {
            _currentStateInstance.Update();
            _currentStateInstance.NextState();
        }
    }
}