namespace Cromede.Enemy.State
{
    public class EnemyDieState : EnemyState
    {
        public EnemyDieState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            stateMachine.ExitCombat();
            stateMachine.EnemyMovement.Stop();
            stateMachine.SetCollider(false);

            stateMachine.EnemyAnimation.SetMoveMagnitude(0.0f);
            stateMachine.EnemyAnimation.SetDie();
        }

        public override void Update()
        {
        }
    }
}