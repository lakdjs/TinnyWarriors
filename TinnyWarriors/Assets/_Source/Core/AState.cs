namespace Core
{
    public abstract class AState
    {
        protected IStateMachine _owner;

        public void SetOwner(IStateMachine owner) => _owner = owner;
        
        public virtual void Enter(){}
        public virtual void Update(){}
        public virtual void Exit(){}
    }
}