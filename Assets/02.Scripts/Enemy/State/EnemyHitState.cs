using static Cromede.Enemy.State.EnemyStateMachine;

namespace Cromede.Enemy.State
{
    public class EnemyHitState : EnemyState
    {
        public EnemyHitState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            //피격 상태 진입 시 전투상태 유지
            stateMachine.EnterCombat();

            //피격 애니메이션 중에는 이동하지 않도록 멈추기
            stateMachine.EnemyMovement.Stop();

            //피격 애니메이션 재생
            stateMachine.EnemyAnimation.SetHit();
        }

        public override void OnAnimationEnd()
        {
            //피격 후 최대 추적 거리를 벗어났으면 시작 위치로 복귀
            if (stateMachine.SpawnDistance > stateMachine.ReturnRange)
            {
                stateMachine.ChangeState(EnemyStateType.Return);
                return;
            }

            //복귀할 필요가 없으면 다시 플레이어 추적
            stateMachine.ChangeState(EnemyStateType.Chase);
        }
    }
}