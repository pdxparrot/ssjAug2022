using Godot;

using System;

namespace pdxpartyparrot.ssjAug2022.NPCs.AI
{
    public abstract class StateMachine<T> : Node where T : SimpleNPC
    {
        private T _owner;

        private IState<T> _previousState;

        private IState<T> _currentState;

        private IState<T> _globalState;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _owner = GetOwner<T>();
        }

        #endregion

        public void SetGlobalState(IState<T> newGlobalState)
        {
            if(_globalState != null) {
                _globalState.Exit(_owner, this);
            }

            _globalState = newGlobalState;
            _globalState.Enter(_owner, this);
        }

        public void Run()
        {
            if(_globalState != null) {
                _globalState.Execute(_owner, this);
            }

            if(_currentState != null) {
                _currentState.Execute(_owner, this);
            }
        }

        public bool IsInState(Type type)
        {
            return _currentState != null && _currentState.GetType() == type;
        }

        public void ChangeState(IState<T> newState)
        {
            _previousState = _currentState;

            if(_currentState != null) {
                _currentState.Exit(_owner, this);
            }

            _currentState = newState;
            _currentState.Enter(_owner, this);
        }

        public void RevertToPreviousState()
        {
            ChangeState(_previousState);
        }
    }
}
