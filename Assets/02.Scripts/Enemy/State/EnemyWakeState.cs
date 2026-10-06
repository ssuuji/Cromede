using static Cromede.Enemy.State.EnemyStateMachine;

namespace Cromede.Enemy.State
{
    public class EnemyWakeState : EnemyState
    {
        public EnemyWakeState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            //깨어나는 동안 전투상태로 변경
            stateMachine.EnterCombat();

            //깨어나는 애니메이션 중에는 이동하지 않음
            stateMachine.EnemyMovement.Stop();

            //깨어남 애니메이션 재생
            stateMachine.EnemyAnimation.SetWake();
        }

        public override void OnAnimationEnd()
        {
            stateMachine.ChangeState(EnemyStateType.Chase);
        }
    }
}