using static Cromede.Enemy.State.EnemyStateMachine;

namespace Cromede.Enemy.State
{
    public class EnemyReturnState : EnemyState
    {
        public EnemyReturnState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            //복귀 상태에서는 전투상태 해제
            stateMachine.ExitCombat();

            //시작 위치로 달려서 복귀
            stateMachine.EnemyMovement.MoveTo(stateMachine.SpawnPosition, stateMachine.EnemyMovement.RunSpeed, 0.0f);
        }

        public override void Update()
        {
            //시작 위치까지 복귀 완료
            if (stateMachine.EnemyMovement.HasReachedDestination)
            {
                //이동 멈추기
                stateMachine.EnemyMovement.Stop();

                //석상 타입 몬스터는 다시 석상 상태로 전환
                if (stateMachine.StartType == EnemyStartType.Dormant)
                {
                    stateMachine.ChangeState(EnemyStateType.Dormant);
                    return;
                }

                //일반 몬스터는 대기 상태로 전환
                stateMachine.ChangeState(EnemyStateType.Idle);
                return;
            }

            //아직 도착하지 않았으면 계속 시작 위치로 이동
            stateMachine.EnemyMovement.MoveTo(stateMachine.SpawnPosition, stateMachine.EnemyMovement.RunSpeed, 0.0f);
        }
    }
}