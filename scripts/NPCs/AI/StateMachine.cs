namespace pdxpartyparrot.ssjAug2022.NPCs.AI
{
    public class StateMachine<T> where T : SimpleNPC
    {
        private T _owner;

        private IState<T> _previousState;

        private IState<T> _currentState;

        private IState<T> _globalState;

        public StateMachine(T owner, IState<T> initialState = null, IState<T> globalState = null)
        {
            _owner = owner;

            _currentState = initialState;
            if(_currentState != null) {
                _currentState.Enter(_owner, this);
            }

            _globalState = globalState;
            if(_globalState != null) {
                _globalState.Enter(_owner, this);
            }
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
