using System;

namespace Game.Scripts.FSM
{
    public abstract class State<T>
    {
        protected readonly T ctx;
        private bool _stateHasBeenChanged;
        protected string stateName;

        protected State(T ctx)
        {
            this.ctx = ctx;
            stateName = GetType().Name;
        }

        public event Action<Enum> StateChange;

        protected void StateChangeInvoke(Enum nextState)
        {
            if (!_stateHasBeenChanged)
                StateChange?.Invoke(nextState);
        }

        public virtual void Enter<TParam>(TParam data)
        {
            Enter();
        }

        public virtual void Enter()
        {
            _stateHasBeenChanged = false;
        }

        public abstract void Update();

        public virtual void Exit()
        {
            _stateHasBeenChanged = true;
        }

        public abstract void NextState();
    }
}