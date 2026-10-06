using static Cromede.Enemy.State.EnemyStateMachine;

namespace Cromede.Enemy.State
{
    public class EnemyWakeState : EnemyState
    {
        public EnemyWakeState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            stateMachine.EnterCombat();
            stateMachine.EnemyMovement.Stop();

            stateMachine.EnemyAnimation.SetWake();
        }

        public override void Update()
        {
        }

        public override void OnAnimationEnd()
        {
            stateMachine.ChangeState(EnemyStateType.Chase);
        }
    }
}