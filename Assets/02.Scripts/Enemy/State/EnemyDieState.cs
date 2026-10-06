namespace Cromede.Enemy.State
{
    public class EnemyDieState : EnemyState
    {
        public EnemyDieState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            //사망하면 전투상태 해제
            stateMachine.ExitCombat();

            //이동 멈추기
            stateMachine.EnemyMovement.Stop();

            //사망 후 충돌하지 않도록 콜라이더 비활성화
            stateMachine.SetCollider(false);

            //이동 애니메이션 값 초기화
            stateMachine.EnemyAnimation.SetMoveMagnitude(0.0f);

            //사망 애니메이션 재생
            stateMachine.EnemyAnimation.SetDie();
        }
    }
}