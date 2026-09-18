namespace Cromede.Player.State
{
    public abstract class PlayerState
    {
        protected PlayerStateMachine stateMachine;
        public PlayerState(PlayerStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public virtual void Enter() { }
        public abstract void Update();
        public virtual void Exit() { }
    }
}
