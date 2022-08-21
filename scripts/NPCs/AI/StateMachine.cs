namespace pdxpartyparrot.ssjAug2022.NPCs.AI
{
    public class StateMachine<T> where T : SimpleNPC
    {
        private State<T> _previousState;

        private State<T> _currentState;

        public StateMachine(T owner, State<T> initialState)
        {
            _currentState = initialState;
            _currentState.Enter(owner, this);
        }

        public void Execute(T owner)
        {
            if(_currentState != null) {
                _currentState.Execute(owner, this);
            }
        }

        public void ChangeState(T owner, State<T> newState)
        {
            if(_currentState != null) {
                _currentState.Exit(owner, this);
            }

            _previousState = _currentState;
            _currentState = newState;
            _currentState.Enter(owner, this);
        }

        public void RevertToPreviousState(T owner)
        {
            ChangeState(owner, _previousState);
        }
    }
}
