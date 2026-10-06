using static Cromede.Enemy.State.EnemyStateMachine;

namespace Cromede.Enemy.State
{
    public class EnemyHitState : EnemyState
    {
        public EnemyHitState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            stateMachine.EnterCombat();
            stateMachine.EnemyMovement.Stop();

            stateMachine.EnemyAnimation.SetHit();
        }

        public override void Update()
        {
        }

        public override void OnAnimationEnd()
        {
            if (stateMachine.SpawnDistance > stateMachine.ReturnRange)
            {
                stateMachine.ChangeState(EnemyStateType.Return);
                return;
            }

            stateMachine.ChangeState(EnemyStateType.Chase);
        }
    }
}