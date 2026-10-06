using static Cromede.Enemy.State.EnemyStateMachine;

namespace Cromede.Enemy.State
{
    public class EnemyChaseState : EnemyState
    {
        public EnemyChaseState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            //추적 상태 진입 시 전투상태 활성화
            stateMachine.EnterCombat();
        }

        public override void Update()
        {
            //타겟이 없으면 시작 위치로 복귀
            if (!stateMachine.HasTarget)
            {
                stateMachine.ChangeState(EnemyStateType.Return);
                return;
            }

            //시작 위치에서 최대 추적 거리 이상 멀어지면 복귀
            if (stateMachine.SpawnDistance > stateMachine.ReturnRange)
            {
                stateMachine.ChangeState(EnemyStateType.Return);
                return;
            }

            //플레이어가 공격 범위 안에 들어온 경우
            if (stateMachine.TargetDistance <= stateMachine.EnemyAttack.AttackRange)
            {
                //공격하기 위해 이동 멈추기
                stateMachine.EnemyMovement.Stop();

                //플레이어 방향 바라보기
                stateMachine.EnemyMovement.FaceTarget(stateMachine.Target.position);

                //공격 쿨타임이 끝났으면 공격 상태로 전환
                if (stateMachine.EnemyAttack.CanAttack)
                {
                    stateMachine.ChangeState(EnemyStateType.Attack);
                }

                return;
            }

            //공격 범위 밖이면 플레이어 추적
            stateMachine.EnemyMovement.MoveTo(stateMachine.Target.position, stateMachine.EnemyMovement.RunSpeed, stateMachine.EnemyMovement.ChaseStoppingDistance);
        }
    }
}