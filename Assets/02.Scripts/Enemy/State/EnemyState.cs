namespace Cromede.Enemy.State
{
    public abstract class EnemyState
    {
        protected EnemyStateMachine stateMachine;

        public EnemyState(EnemyStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void Exit() { }
        public virtual void OnAnimationEnd() { }
    }
}